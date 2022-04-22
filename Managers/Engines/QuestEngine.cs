using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using PubSub;

using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;
using Timer = System.Timers.Timer;

namespace DefiKindom_QuestRunner.Managers.Engines
{
    internal class QuestEngine : IDisposable
    {
        #region Internals

        readonly Hub _eventHub = Hub.Default;
        private Timer timerCheckInstanceStatus;
        private Timer timerNftTransferHandler;

        private int _failedToStartQuestTimes;
        private DateTime? _lastStaminaCheck;
        private DateTime _lastOneBalanceCheck = DateTime.Now;

        #endregion
        
        #region Enums

        public enum QuestTypes
        {
            Mining,
            Foraging,
            Fishing,
            WishingWell
        }

        public enum QuestActivityMode
        {
            WantsToQuest,
            Questing,
            WantsToCancelQuest,
            WantsToCompleteQuest,
            WaitingOnStamina,
            Ignore
        }

        #endregion

        #region Properties

        public DfkWallet DfkWallet { get; private set; }

        public QuestTypes QuestType { get; }

        public QuestActivityMode QuestCurrentMode { get; private set; }

        #endregion

        #region Constructor(s)

        QuestEngine()
        {
            _eventHub.Subscribe<PreferenceUpdateEvent>(PreferencesEventHandler);
        }

        public QuestEngine(DfkWallet wallet, QuestTypes type, QuestActivityMode mode) : this()
        {
            DfkWallet = wallet;
            QuestType = type;

            QuestCurrentMode = mode;
        }

        #endregion

        #region Public Engine Start/Stop Methods

        //Self managing wallet mining, returning to mine , cancel, etc
        public async void Start()
        {
            await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStarting));

            //Build NFT Transfer Timer
            //timerNftTransferHandler = new Timer(3600000);
            //timerNftTransferHandler.Elapsed += TimerNftTransferHandlerOnElapsed;
            //timerNftTransferHandler.Enabled = true;
            //timerNftTransferHandler.Start();

            //Build Timer
            timerCheckInstanceStatus = new Timer(Settings.Default.QuestInstanceMsInterval);
            timerCheckInstanceStatus.Elapsed += TimerCheckInstanceStatusOnElapsed;

            //We need to determine what mode the instance is starting as and determine if the app needs the jewel
            switch (QuestCurrentMode)
            {
                case QuestActivityMode.WantsToCompleteQuest:
                case QuestActivityMode.WantsToCancelQuest:
                case QuestActivityMode.WantsToQuest:
                    JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, true);

                    await _eventHub.PublishAsync(new WalletQuestStatusEvent
                    {
                        Name = DfkWallet.Name,
                        CurrentActivityMode = QuestCurrentMode,
                        HeroStamina = DfkWallet.AssignedHeroStamina,
                        HeroId = DfkWallet.AssignedHero,
                        WalletAddress = DfkWallet.Address,
                        ContractAddress = "",
                        StartedAt = null,
                        CompletesAt = null
                    });

                    //Jewel Request went out. No sense in loop till we actually get hold of the Jewel
                    timerCheckInstanceStatus.Enabled = false;
                    break;

                default:
                    //All Other modes , timer can be enabled so we can manage status
                    timerCheckInstanceStatus.Enabled = true;
                    break;
            }

            //Start Timer
            timerCheckInstanceStatus.Start();
        }

        public async void Stop()
        {
            timerCheckInstanceStatus.Enabled = false;
            timerCheckInstanceStatus.Elapsed -= null;
            timerCheckInstanceStatus.Dispose();
            timerCheckInstanceStatus = null;

            if (timerNftTransferHandler != null)
            {
                timerNftTransferHandler.Elapsed -= null;
                timerNftTransferHandler.Enabled = false;
                timerNftTransferHandler.Dispose();
                timerNftTransferHandler = null;
            }

            await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStopping));
        }

        #endregion

        #region Public Manager Methods

        public async Task<bool> AddJewel(bool isRetryAttempt = false)
        {
            int stamina;

            if (!isRetryAttempt)
            {
                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Has received the JEWEL..." });
            }

            //ALWAYS grab the most recent quest status.
            //And override current status
            DfkWallet.AssignedHeroQuestStatus =
                await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                    DfkWallet.AssignedHero);

            if (DfkWallet.AssignedHeroQuestStatus.IsQuesting)
            {
                if (DfkWallet.AssignedHeroQuestStatus.WantsToComplete)
                    QuestCurrentMode = QuestActivityMode.WantsToCompleteQuest;
                else if (DfkWallet.AssignedHeroQuestStatus.WantsToCancel)
                    QuestCurrentMode = QuestActivityMode.WantsToCancelQuest;
                else
                {
                    //Well we're actively questing and we shouldnt be here! 
                    QuestCurrentMode = QuestActivityMode.Questing;

                    //Release Jewel 
                    JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, false);

                    if (timerCheckInstanceStatus != null)
                        timerCheckInstanceStatus.Enabled = true;

                    return false;
                }
            }
            else
            {
                QuestCurrentMode = QuestActivityMode.WantsToQuest;
            }

            switch (QuestCurrentMode)
            {
                case QuestActivityMode.WantsToCancelQuest:
                    //Lets cancel the quest
                    var cancelQuestResponse = await
                        new QuestContractHandler().CancelQuesting(DfkWallet,
                            DfkWallet.AssignedHero);
                    if (cancelQuestResponse)
                    {
                        QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                        //Tell system your not questing now
                        await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                            WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingCanceled));

                        await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Quest Canceled..." });

                        // Wait till blockchain catches up and cancels quest
                        while (DfkWallet.AssignedHeroQuestStatus.IsQuesting)
                        {
                            DfkWallet.AssignedHeroQuestStatus =
                                await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);

                            Thread.Sleep(5000);
                        }

                        //Release Jewel
                        JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, false);
                    }
                    else
                    {
                        //Failed to cancel. Re-try in 5 seconds
                        Thread.Sleep(5000);
                        await AddJewel(true);
                    }
                    break;

                case QuestActivityMode.WantsToCompleteQuest:
                    var completeQuestResponse = await
                        new QuestContractHandler().CompleteQuesting(DfkWallet,
                            DfkWallet.AssignedHero);
                    if (completeQuestResponse)
                    {
                        QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                        //Tell system your not questing now
                        await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                            WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingComplete));

                        await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Quest Completed..." });

                        // Wait till blockchain catches up and completes quest
                        while (DfkWallet.AssignedHeroQuestStatus.IsQuesting)
                        {
                            DfkWallet.AssignedHeroQuestStatus =
                                await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);

                            Thread.Sleep(5000);
                        }

                        //Give up the Jewel so next instance can run
                        JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, false);
                    }
                    else
                    {
                        //Failed to complete. Re-try in 5 seconds
                        Thread.Sleep(5000);
                        await AddJewel(true);
                    }
                    break;

                case QuestActivityMode.WantsToQuest:
                    //Make sure they have enough stamina before attempting to quest!
                    stamina = await
                        new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);
                    if (stamina < 15)
                    {
                        QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                        //Tell system your questing
                        _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                            WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.WaitingOnStamina));

                        //Lets tell jewel manager we're done (only if we successfully started quest contract)
                        JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, false);

                        _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Cant Start Quest (Canceling). (Stamina Too Low => {stamina})" });
                    }
                    else
                    {
                        //Start the quest
                        var startQuestResponse = await
                            new QuestContractHandler().StartQuesting(DfkWallet,
                                DfkWallet.AssignedHero);
                        if (startQuestResponse)
                        {
                            QuestCurrentMode = QuestActivityMode.Questing;

                            //Tell system your questing
                            await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing));

                            // Wait till blockchain catches up and starts the quest
                            while (!DfkWallet.AssignedHeroQuestStatus.IsQuesting)
                            {
                                DfkWallet.AssignedHeroQuestStatus =
                                    await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                        DfkWallet.AssignedHero);

                                Thread.Sleep(5000);
                            }

                            //Lets tell jewel manager we're done (only if we succesfully started quest contract)
                            JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, false);

                            await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Quest Started..." });

                           
                        }
                        else
                        {
                            //Failed to start. Re-try in 5 seconds
                            Thread.Sleep(5000);
                            await AddJewel(true);
                        }
                    }
                    break;
            }

            if (timerCheckInstanceStatus != null && !isRetryAttempt)
                timerCheckInstanceStatus.Enabled = true;

            return true;
        }

        /// <summary>
        /// Jewel was removed from the instance. Go Back to "Stamina" monitoring
        /// And then engage quest start, wait, etc again
        /// </summary>
        public void RemoveJewel()
        {
            _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => No longer has the JEWEL..." });

            QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

            if (timerCheckInstanceStatus != null)
                timerCheckInstanceStatus.Enabled = true;
        }

        #endregion

        #region Timer Event

        private async void TimerCheckInstanceStatusOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (timerCheckInstanceStatus != null)
                timerCheckInstanceStatus.Enabled = false;

            try
            {
                //Always make sure we have a balance
                await EnsureONEBalanceExists();

                try
                {
                    await _eventHub.PublishAsync(new WalletQuestStatusEvent
                    {
                        CurrentActivityMode = QuestCurrentMode,
                        HeroStamina = DfkWallet.AssignedHeroStamina,
                        HeroId = DfkWallet.AssignedHero,
                        WalletAddress = DfkWallet.Address,
                        Name = DfkWallet.Name,
                        ContractAddress = DfkWallet.AssignedHeroQuestStatus != null
                            ? DfkWallet.AssignedHeroQuestStatus.ContractAddress
                            : "",
                        StartedAt = DfkWallet.IsQuesting ? DfkWallet.AssignedHeroQuestStatus?.QuestStartedAt : null,
                        CompletesAt = DfkWallet.IsQuesting ? DfkWallet.AssignedHeroQuestStatus?.QuestCompletesAt : null,
                    });
                }
                catch (Exception msgException)
                {

                }

                switch (QuestCurrentMode)
                {
                    //We know 100% we're questing, now we must determine if its time to CANCEL!
                    case QuestActivityMode.Questing:
                        DfkWallet.AssignedHeroQuestStatus = await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                            DfkWallet.AssignedHero);

                        //We're marked as questing, but are we really?
                        if (DfkWallet.IsQuesting)
                        {
                            if (DfkWallet.QuestNeedsCompleted)
                            {
                                //Tell the system we want to cancel and we need the jewel
                                QuestCurrentMode = QuestActivityMode.WantsToCompleteQuest;

                                //Lets tell jewel manager we need the jewel
                                JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, true);

                                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Wants To Complete...(Queued)." });

                                //Don't re-enable timer. Timer will engage once jewel is used and moved
                                return;
                            }
                            
                            if (DfkWallet.QuestNeedsCanceled)
                            {
                                //Tell the system we want to cancel and we need the jewel
                                QuestCurrentMode = QuestActivityMode.WantsToCancelQuest;

                                //Lets tell jewel manager we need the jewel
                                JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, true);

                                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Wants To Cancel...(Queued)." });

                                //Don't re-enable timer. Timer will engage once jewel is used and moved
                                return;
                            }
                        }
                        else
                        {
                            //Not questing?  Should be waiting on stamina then
                            QuestCurrentMode = QuestActivityMode.WaitingOnStamina;
                        }
                        break;

                    //We've quested, canceled, and waiting on stamina to start again!
                    case QuestActivityMode.WaitingOnStamina:
                        if (_lastStaminaCheck == null)
                        {
                            _lastStaminaCheck = DateTime.Now;

                            DfkWallet.AssignedHeroStamina = await
                                new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);

                            //Available stam > 15. Lets move to quest mode
                            if (DfkWallet.AssignedHeroStamina > 15)
                            {
                                //Set wants to quest mode
                                QuestCurrentMode = QuestActivityMode.WantsToQuest;

                                //Delete prior quest status
                                DfkWallet.AssignedHeroQuestStatus = null;
                                
                                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Has 15+ Stamina..Wants to Quest!..." });

                                //Signal the need for JEWEL
                                JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, true);

                                //Don't re-enable timer. Timer will engage once jewel is used and moved
                                return;
                            }
                        }
                        else
                        {
                            //Only Check Stamina every 15 minutes
                            var now = DateTime.Now;
                            var timeBetweenStartAndNow = now.Subtract(_lastStaminaCheck.GetValueOrDefault());
                            if (timeBetweenStartAndNow.TotalMinutes >= 15)
                            {
                                var stamina = await
                                    new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);

                                DfkWallet.AssignedHeroStamina = stamina;

                                //Available stam > 15. Lets move to quest mode
                                if (DfkWallet.AssignedHeroStamina > 15)
                                {
                                    QuestCurrentMode = QuestActivityMode.WantsToQuest;

                                    await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Has 15+ Stamina..Wants to Quest!..." });

                                    //Signal the need for JEWEL
                                    JewelManager.SetWalletNeedStatus(DfkWallet, QuestCurrentMode, true);

                                    //Don't re-enable timer. Timer will engage once jewel is used and moved
                                    return;
                                }

                                //Reset so next time we make sure to grab stamina
                                _lastStaminaCheck = null;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Instance Exception => {ex.Message} => {ex.StackTrace}" });
            }

            if (timerCheckInstanceStatus != null)
                timerCheckInstanceStatus.Enabled = true;
        }

        private async void TimerNftTransferHandlerOnElapsed(object sender, ElapsedEventArgs e)
        {
            timerNftTransferHandler.Enabled = false;

            try
            {
                DfkWallet.NftBalanceInfo = await new NftContractHandler().GetNftBalances(DfkWallet);

                switch (DfkWallet.Address.Trim())
                {
                    case "0x209A4C72310Ba0EA9fE98595112c0E16dE84DeFF":
                    case "0x975E19965a89933166FBD0DbD0c3CC2ec0Bc2098":
                    case "0x79853D7f75902ea4304206D82C49484cD8FCf2C2":
                    case "0x62aD520259C9ec865b6ff28886C8dEA88bCcFA3D":
                    case "0x2C77A921f48693f00aFCa1D6349779AA60066519":
                        break;

                    default:
                        //Get Source Wallet
                        var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                        if (sourceWallet != null)
                        {
                            //Transfer Svhas Runes
                            if (DfkWallet.NftBalanceInfo.ShvasRunes > 0)
                            {
                                await new NftContractHandler().TransferNft(DfkWallet, sourceWallet.Address,
                                    ProfileContractHandler.NftTypes.ShvasRune, DfkWallet.NftBalanceInfo.ShvasRunes);
                            }

                            //Transfer Tears
                            if (DfkWallet.NftBalanceInfo.Tears > 5)
                            {
                                await new NftContractHandler().TransferNft(DfkWallet, sourceWallet.Address,
                                    ProfileContractHandler.NftTypes.Tears, DfkWallet.NftBalanceInfo.Tears);
                            }

                            DfkWallet.NftBalanceInfo = await new NftContractHandler().GetNftBalances(DfkWallet);
                        }
                        break;
                }

                //Save updates
                WalletManager.SaveWallets();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            timerNftTransferHandler.Enabled = true;
        }

        async Task EnsureONEBalanceExists()
        {
            //Only run once an hour. If under 5 ONE send 10 to wallet
            var now = DateTime.Now;
            var timeBetweenStartAndNow = now.Subtract(_lastOneBalanceCheck);
            if (timeBetweenStartAndNow.TotalMinutes >= 60)
            {
                var currentTotalOne = await new OneContractHandler().CheckHarmonyONEBalance(DfkWallet.WalletAccount);
                if (currentTotalOne < 5)
                {
                    var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                    if (sourceWallet != null)
                    {
                        var sendOneResult = await new OneContractHandler().SendHarmonyONE(sourceWallet, DfkWallet.Address, 10);
                        if (sendOneResult.Success)
                        {
                            await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Was very low on ONE! Added 10 more ONE!" });
                        }
                    }
                }

                _lastOneBalanceCheck = DateTime.Now;
            }
        }

        #endregion

        #region Event Subscriptions

        private void PreferencesEventHandler(PreferenceUpdateEvent evt)
        {
            if(timerCheckInstanceStatus != null)
                timerCheckInstanceStatus.Interval = evt.QuestInstanceInterval;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            timerCheckInstanceStatus.Elapsed -= null;
            timerCheckInstanceStatus.Dispose();
            timerCheckInstanceStatus = null;
        }

        #endregion
    }
}