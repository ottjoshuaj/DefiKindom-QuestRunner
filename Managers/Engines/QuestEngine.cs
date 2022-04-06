using System;
using System.Linq;
using System.Timers;

using PubSub;

using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.Managers.Engines
{
    internal class QuestEngine : IDisposable
    {
        #region Internals

        readonly Hub _eventHub = Hub.Default;
        private Timer timerCheckInstanceStatus;
        private bool _hasTheJewel;
        private int _failedToStartQuestTimes;
        private bool _needsRefreshQuestStatus = true;
        private DateTime? _lastStaminaCheck;

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

        public DfkWallet DfkWallet { get; }

        public QuestTypes QuestType { get; }

        public QuestActivityMode QuestCurrentMode { get; private set; }

        #endregion

        #region Constructor(s)

        QuestEngine()
        {
            _eventHub.Subscribe<PreferenceUpdateEvent>(PreferencesEventHandler);
            _eventHub.Subscribe<JewelEvent>(JewelMovementHandler);
        }

        public QuestEngine(DfkWallet wallet, QuestTypes type, QuestActivityMode mode) : this()
        {
            DfkWallet = wallet;
            QuestType = type;

            QuestCurrentMode = mode;
        }

        #endregion

        #region Public Methods

        //Self managing wallet mining, returning to mine , cancel, etc
        public async void Start()
        {
            await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStarting));

            //Build Timer
            timerCheckInstanceStatus = new Timer(Settings.Default.QuestInstanceMsInterval);
            timerCheckInstanceStatus.Elapsed += TimerCheckInstanceStatusOnElapsed;

            //We need to determine what mode the instance is starting as and determine if the app needs the jewel
            switch (QuestCurrentMode)
            {
                case QuestActivityMode.WantsToCompleteQuest:
                case QuestActivityMode.WantsToCancelQuest:
                case QuestActivityMode.WantsToQuest:
                    await _eventHub.PublishAsync(new JewelEvent(DfkWallet, JewelEvent.JewelEventRequestTypes.NeedJewel, QuestCurrentMode));
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

            await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStopping));
        }

        #endregion

        #region Timer Event

        private async void TimerCheckInstanceStatusOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (timerCheckInstanceStatus != null)
                timerCheckInstanceStatus.Enabled = false;

            try
            {
                await _eventHub.PublishAsync(new WalletQuestStatusEvent
                {
                    CurrentActivityMode = QuestCurrentMode,
                    HeroStamina = DfkWallet.AssignedHeroStamina,
                    HeroId = DfkWallet.AssignedHero,
                    WalletAddress = DfkWallet.Address,
                    Name = DfkWallet.Name,
                    ContractAddress = DfkWallet.AssignedHeroQuestStatus != null ? DfkWallet.AssignedHeroQuestStatus.ContractAddress : "",
                    StartedAt = DfkWallet.IsQuesting ? DfkWallet.AssignedHeroQuestStatus?.QuestStartedAt : null,
                    CompletesAt = DfkWallet.IsQuesting ? DfkWallet.AssignedHeroQuestStatus?.QuestCompletesAt : null,
                });

                switch (QuestCurrentMode)
                {
                    //We know 100% we're questing, now we must determine if its time to CANCEL!
                    case QuestActivityMode.Questing:
                        //We're marked as questing, but are we really?
                        if (DfkWallet.IsQuesting)
                        {
                            //Get Quest Status
                            if (DfkWallet.AssignedHeroQuestStatus == null)
                            {
                                var questState =
                                    await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                        DfkWallet.AssignedHero);
                                if (questState != null)
                                {
                                    //Save state
                                    DfkWallet.AssignedHeroQuestStatus = questState;

                                    //Save and Save State
                                    WalletManager.GetWallet(DfkWallet.Address).AssignedHeroQuestStatus =
                                        questState;
                                    WalletManager.SaveWallets();
                                }
                            }

                            if (DfkWallet.QuestNeedsCompleted)
                            {
                                //Tell the system we want to cancel and we need the jewel
                                QuestCurrentMode = QuestActivityMode.WantsToCompleteQuest;

                                //Lets tell jewel manager we need the jewel
                                await _eventHub.PublishAsync(new JewelEvent(DfkWallet,
                                    JewelEvent.JewelEventRequestTypes.NeedJewel, QuestCurrentMode));

                                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Wants To Complete...(Queued)." });
                            }
                            else if (DfkWallet.QuestNeedsCanceled)
                            {
                                //Tell the system we want to cancel and we need the jewel
                                QuestCurrentMode = QuestActivityMode.WantsToCancelQuest;

                                //Lets tell jewel manager we need the jewel
                                await _eventHub.PublishAsync(new JewelEvent(DfkWallet,
                                    JewelEvent.JewelEventRequestTypes.NeedJewel, QuestCurrentMode));

                                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Wants To Cancel...(Queued)." });
                            }
                            else
                            {
                                
                            }
                        }
                        else
                        {
                            if (_needsRefreshQuestStatus)
                            {
                                //Lets double check quest status
                                var questStatus =
                                    await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                        DfkWallet.AssignedHero);
                                if (questStatus != null)
                                {
                                    _needsRefreshQuestStatus = false;

                                    DfkWallet.AssignedHeroQuestStatus = questStatus;

                                    WalletManager.GetWallet(DfkWallet.Address).AssignedHeroQuestStatus = questStatus;
                                    WalletManager.SaveWallets();

                                    //Ok ARE WE REALLY Questing? 
                                    if (!DfkWallet.IsQuesting)
                                    {
                                        QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                        //Tell system your not questing now
                                        await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                            WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.WaitingOnStamina));
                                    }
                                }
                            }
                        }
                        break;

                    //We've quested, canceled, and waiting on stamina to start again!
                    case QuestActivityMode.WaitingOnStamina:
                        if (_lastStaminaCheck == null)
                        {
                            _lastStaminaCheck = DateTime.Now;

                            var stamina = await
                                new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);

                            DfkWallet.AssignedHeroStamina = stamina;

                            //Available stam > 15. Lets move to quest mode
                            if (DfkWallet.AssignedHeroStamina >= 15)
                            {
                                //Set wants to quest mode
                                QuestCurrentMode = QuestActivityMode.WantsToQuest;

                                //Delete prior quest status
                                DfkWallet.AssignedHeroQuestStatus = null;
                                
                                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Has 15+ Stamina..Wants to Quest!..." });

                                //Signal the need for JEWEL
                                await _eventHub.PublishAsync(new JewelEvent(DfkWallet,
                                    JewelEvent.JewelEventRequestTypes.NeedJewel, QuestCurrentMode));
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
                                if (DfkWallet.AssignedHeroStamina >= 15)
                                {
                                    QuestCurrentMode = QuestActivityMode.WantsToQuest;

                                    await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Has 15+ Stamina..Wants to Quest!..." });

                                    //Signal the need for JEWEL
                                    await _eventHub.PublishAsync(new JewelEvent(DfkWallet,
                                        JewelEvent.JewelEventRequestTypes.NeedJewel, QuestCurrentMode));
                                }

                                //Reset so next time we make sure to grab stamina
                                _lastStaminaCheck = null;
                            }
                        }
                        break;

                    case QuestActivityMode.WantsToCompleteQuest:
                        if (_hasTheJewel)
                        {
                            //Make sure we still want to complete the quest (if its already completed then no sense)
                            var questStatus =
                                await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);
                            if (questStatus != null)
                            {
                                //Set Quest Status
                                DfkWallet.AssignedHeroQuestStatus = questStatus;

                                if (!questStatus.IsQuesting)
                                {
                                    //Prior contract call finally executed. Update app telling it to move into stamina mode
                                    QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                    _hasTheJewel = false;

                                    //Give up the Jewel so next instance can run
                                    await _eventHub.PublishAsync(new JewelEvent(DfkWallet,
                                        JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));
                                    break;
                                }

                                //We sure we want to complete ?
                                if (questStatus.WantsToComplete)
                                {
                                    //So YES it does want to complete! So let the standard flow happen
                                }
                                else if(questStatus.WantsToCancel)
                                {
                                    //Actually we should cancel. Enough time has passed
                                    QuestCurrentMode = QuestActivityMode.WantsToCancelQuest;

                                    break;
                                }
                            }
                            
                            //Lets cancel the quest
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

                                //Check to see if the hero has enough stamina. Might as well start questing as we complete!
                                var stamina = await
                                    new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);

                                DfkWallet.AssignedHeroStamina = stamina;

                                WalletManager.GetWallet(DfkWallet.Address).AssignedHeroQuestStatus = null;
                                WalletManager.SaveWallets();

                                if (stamina >= 15)
                                {
                                    QuestCurrentMode = QuestActivityMode.WantsToQuest;

                                    await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Has enough stamina. Start Questing..." });
                                }
                                else
                                {
                                    _hasTheJewel = false;

                                    //Give up the Jewel so next instance can run
                                    await _eventHub.PublishAsync(new JewelEvent(DfkWallet,
                                        JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));
                                }
                            }
                        }
                        break;

                    case QuestActivityMode.WantsToCancelQuest:
                        if (_hasTheJewel)
                        {
                            //Make sure we still want to cancel the quest (if its already canceled then no sense)
                            var questStatus =
                                await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);
                            if (questStatus != null)
                            {
                                //Set Quest Status
                                DfkWallet.AssignedHeroQuestStatus = questStatus;

                                if (!questStatus.IsQuesting)
                                {
                                    //Prior contract call finally executed. Update app telling it to move into stamina mode
                                    QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                    _hasTheJewel = false;

                                    //Give up the Jewel so next instance can run
                                    await _eventHub.PublishAsync(new JewelEvent(DfkWallet,
                                        JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));
                                    break;
                                }

                                //We sure we want to complete ?
                                if (questStatus.WantsToComplete)
                                {
                                    //Should complete! 
                                    //Actually we should cancel. Enough time has passed
                                    QuestCurrentMode = QuestActivityMode.WantsToCompleteQuest;

                                    break;
                                }

                                if (questStatus.WantsToCancel)
                                {
                                    //Yes we want to cancel. Let the flow happen
                                }
                            }

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

                                //Check to see if the hero has enough stamina. Might as well start questing as we complete!
                                var stamina = await
                                    new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);

                                DfkWallet.AssignedHeroStamina = stamina;

                                WalletManager.GetWallet(DfkWallet.Address).AssignedHeroQuestStatus = null;
                                WalletManager.SaveWallets();

                                if (stamina >= 15)
                                {
                                    QuestCurrentMode = QuestActivityMode.WantsToQuest;

                                    await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Has enough stamina. Start Questing..." });
                                }
                                else
                                {
                                    _hasTheJewel = false;

                                    //Give up the Jewel so next instance can run
                                    await _eventHub.PublishAsync(new JewelEvent(DfkWallet,
                                        JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));
                                }
                            }
                        }
                        break;

                    case QuestActivityMode.WantsToQuest:
                        if (_hasTheJewel)
                        {
                            //Add a check to see if we're questing already
                            var questStatus =
                                await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);
                            if (questStatus != null)
                            {
                                //Set Quest Status
                                DfkWallet.AssignedHeroQuestStatus = questStatus;

                                //Questing? 
                                if (questStatus.IsQuesting)
                                {
                                    QuestCurrentMode = QuestActivityMode.Questing;

                                    //Tell system your questing
                                    await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                        WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing));

                                    //Lets tell jewel manager we're done (only if we successfully started quest contract)
                                    await _eventHub.PublishAsync(new JewelEvent(DfkWallet, JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                    await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Already Questing...(Instance Resumed & Jewel Moving To Next Wallet)" });
                                }
                                else
                                {
                                    //Make sure we have a HERO and its NOT ZERO
                                    if (DfkWallet.AssignedHero == 0)
                                    {
                                        //Lets make sure we always have a hero
                                        var walletHeroCheck =
                                            await new HeroContractHandler().GetWalletHeroes(DfkWallet.WalletAccount);
                                        if (walletHeroCheck != null)
                                        {
                                            var newAssignedHero = DfkWallet.AvailableHeroes.First();
                                            var heroStamina =
                                                await new QuestContractHandler().GetHeroStamina(
                                                    DfkWallet.WalletAccount, newAssignedHero);

                                            //Set Object properties
                                            DfkWallet.AssignedHeroStamina = heroStamina;
                                            
                                            //Update Wallet Objects
                                            WalletManager.GetWallet(DfkWallet).AssignedHeroStamina = heroStamina;
                                            WalletManager.SaveWallets();

                                            //Can this hero even quest?
                                            if (heroStamina < 15)
                                            {
                                                QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                                //Tell system your questing
                                                await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                                    WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.WaitingOnStamina));

                                                //Lets tell jewel manager we're done (only if we successfully started quest contract)
                                                await _eventHub.PublishAsync(new JewelEvent(DfkWallet, JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                                if (timerCheckInstanceStatus != null)
                                                    timerCheckInstanceStatus.Enabled = true;

                                                return;
                                            }
                                        }
                                        else
                                        {
                                            //Wallet doesnt have any assign heroes or available heroes at all.... kick this instance out!
                                            //Tell system your questing
                                            await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                                WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStopping));

                                            //Lets tell jewel manager we're done (only if we successfully started quest contract)
                                            await _eventHub.PublishAsync(new JewelEvent(DfkWallet, JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                            QuestEngineManager.RemoveQuestEngine(DfkWallet, QuestTypes.Mining);

                                            return;
                                        }
                                    }

                                    //Make sure they have enough stamina before attempting to quest!
                                    var stamina = await
                                        new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);
                                    if (stamina < 15)
                                    {
                                        QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                        //Tell system your questing
                                        await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                            WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.WaitingOnStamina));

                                        //Lets tell jewel manager we're done (only if we succesfully started quest contract)
                                        await _eventHub.PublishAsync(new JewelEvent(DfkWallet, JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                        await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Cant Start Quest (Canceling). (Stamina Too Low => {stamina})" });
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

                                            //Lets tell jewel manager we're done (only if we succesfully started quest contract)
                                            await _eventHub.PublishAsync(new JewelEvent(DfkWallet, JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                            await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Quest Started..." });

                                            //Since we create quest. Lets get the info about it
                                            var questState = await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount, DfkWallet.AssignedHero);
                                            if (questState != null)
                                            {
                                                WalletManager.GetWallet(DfkWallet.Address).AssignedHeroQuestStatus =
                                                    questState;
                                                WalletManager.SaveWallets();
                                            }
                                        }
                                        else
                                        {
                                            _failedToStartQuestTimes++;

                                            if (_failedToStartQuestTimes >= 2)
                                            {
                                                //Ok so why are we failing?  Maybe the assigned hero is no longer with us?
                                                //Get hero list for wallet
                                                var heroesInWallet =
                                                    await new HeroContractHandler().GetWalletHeroes(DfkWallet.WalletAccount);
                                                if (heroesInWallet.Count > 0)
                                                {
                                                    if (!heroesInWallet.Contains(DfkWallet.AssignedHero))
                                                    {
                                                        await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Hero Assignment Incorrect! [Had HeroId: {DfkWallet.AssignedHero}] => Getting available hero" });

                                                        //Shit, our hero was swiped!  Set our AssignedHero as our current hero
                                                        DfkWallet.AvailableHeroes = heroesInWallet;

                                                        //Set Assigned Hero and current hero list
                                                        WalletManager.GetWallet(DfkWallet.Address).AvailableHeroes =
                                                            DfkWallet.AvailableHeroes;

                                                        //Get this hero's stamina
                                                        var heroStamina =
                                                            await new QuestContractHandler().GetHeroStamina(
                                                                DfkWallet.WalletAccount, DfkWallet.AssignedHero);

                                                        //Set Stamina
                                                        WalletManager.GetWallet(DfkWallet.Address).AssignedHeroStamina =
                                                            heroStamina;

                                                        //Save changes
                                                        WalletManager.SaveWallets();

                                                        await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Hero Assignment Corrected! [Proper HeroId: {DfkWallet.AssignedHero}]" });

                                                        if (heroStamina < 15)
                                                        {
                                                            //We cant be questing.  so lets let go of the jewel and go back to wait for stamina mode
                                                            QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                                            //Tell system your waiting on stamina
                                                            await _eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                                                WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing));

                                                            //Lets tell jewel manager we're done (only if we succesfully started quest contract)
                                                            await _eventHub.PublishAsync(new JewelEvent(DfkWallet, JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                                            await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Wants To Quest Canceled (Not enough stamina!)" });
                                                        }
                                                    }
                                                }
                                            }
                                            else if (_failedToStartQuestTimes > 5)
                                            {
                                                //For some reason we cant do shit right now.  LEts move the queue along and move this hero/wallet to wait on stamina
                                                QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                                //Lets tell jewel manager we're done (only if we successfully started quest contract)
                                                await _eventHub.PublishAsync(new JewelEvent(DfkWallet, JewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Unable to send hero questing.  Blockchain issues/slowness. Moving to next in line (will come back to this wallet)" });

                                                _hasTheJewel = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await _eventHub.PublishAsync(new MessageEvent { Content = $@"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] => Instance Exception => {ex.Message}" });
            }


            if (timerCheckInstanceStatus != null)
                timerCheckInstanceStatus.Enabled = true;
        }

        #endregion

        #region Event Subscriptions

        private void JewelMovementHandler(JewelEvent evt)
        {
            if (evt.Wallet.Address.Trim().ToUpper() == DfkWallet.Address.Trim().ToUpper())
            {
                switch (evt.RequestType)
                {
                    case JewelEvent.JewelEventRequestTypes.JewelMovedToAccount:
                        //Set as HAS JEWEL
                        _hasTheJewel = true;

                        //Instantly call the timer method so that way it dumps directly into 
                        //whatever status it needs to execute
                        TimerCheckInstanceStatusOnElapsed(null, null);
                        break;

                    case JewelEvent.JewelEventRequestTypes.JewelMovedOffAccount:
                        //Set as DOESNT HAVE JEWEL
                        _hasTheJewel = false;

                        //Instance is done with the jewel. Enable timer so it can continue where it needs
                        TimerCheckInstanceStatusOnElapsed(null, null);
                        break;
                }
            }
        }

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