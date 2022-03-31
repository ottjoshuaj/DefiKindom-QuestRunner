using System;
using System.Linq;
using System.Timers;
using DefiKindom_QuestRunner.Managers;
using PubSub;

using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.EngineManagers.Engines
{
    internal class QuestEngine : IDisposable
    {
        #region Internals

        Hub eventHub = Hub.Default;
        private Timer timerCheckInstanceStatus;
        private bool _hasTheJewel;

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
            eventHub.Subscribe<PreferenceUpdateEvent>(PreferencesEventHandler);
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
            await eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStarting));

            //Build Timer
            timerCheckInstanceStatus = new Timer(Settings.Default.QuestInstanceMsInterval);
            timerCheckInstanceStatus.Elapsed += TimerCheckInstanceStatusOnElapsed;

            //We need to determine what mode the instance is starting as and determine if the app needs the jewel
            switch (QuestCurrentMode)
            {
                case QuestActivityMode.WantsToCompleteQuest:
                case QuestActivityMode.WantsToCancelQuest:
                case QuestActivityMode.WantsToQuest:
                    await eventHub.PublishAsync(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.NeedJewel, QuestCurrentMode));
                    await eventHub.PublishAsync(new WalletQuestStatusEvent
                    {
                        CurrentActivityMode = QuestCurrentMode,
                        HeroStamina = DfkWallet.AssignedHeroStamina,
                        HeroId = DfkWallet.AssignedHero,
                        WalletAddress = DfkWallet.Address
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

            await eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStopping));
        }

        #endregion

        #region Public Has Jewel Methods

        public void SetAsJewelOwner(bool hasTheJewel)
        {
            if (hasTheJewel)
            {
                //Set as HAS JEWEL
                _hasTheJewel = true;

                //Instantly call the timer method so that way it dumps directly into 
                //whatever status it needs to execute
                TimerCheckInstanceStatusOnElapsed(null, null);
            }
            else
            {
                //Set as DOESNT HAVE JEWEL
                _hasTheJewel = false;

                //Instance is done with the jewel. Enable timer so it can continue where it needs
                timerCheckInstanceStatus.Enabled = true;
            }
        }

        #endregion

        #region Timer Event

        private async void TimerCheckInstanceStatusOnElapsed(object sender, ElapsedEventArgs e)
        {
            timerCheckInstanceStatus.Enabled = false;

            try
            {
                await eventHub.PublishAsync(new WalletQuestStatusEvent
                {
                    CurrentActivityMode = QuestCurrentMode,
                    HeroStamina = DfkWallet.AssignedHeroStamina,
                    HeroId = DfkWallet.AssignedHero,
                    WalletAddress = DfkWallet.Address
                });

                switch (QuestCurrentMode)
                {
                    //We know 100% we're questing, now we must determine if its time to CANCEL!
                    case QuestActivityMode.Questing:
                        //Compare start time to now. If time >= 155 minutes then we must move to CANCEL event
                        if (DfkWallet.ReadyToWork && DfkWallet.AssignedHeroQuestStatus != null &&
                            DfkWallet.AssignedHeroQuestStatus.IsQuesting)
                        {
                            DfkWallet.AssignedHeroStamina = await
                                new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);

                            //How much stamina has been used?. Determine MAX Stam - current = how much has been used. If >= 15 time to cancel
                            var heroProfile =
                                DfkWallet.HeroProfiles.FirstOrDefault(x => x.Id == DfkWallet.AssignedHero);
                            if (heroProfile != null)
                            {
                                var staminaUsed = (heroProfile.Stats.Stamina - DfkWallet.AssignedHeroStamina);
                                if (staminaUsed >= 15)
                                {
                                    //Tell the system we want to cancel and we need the jewel
                                    QuestCurrentMode = QuestActivityMode.WantsToCancelQuest;

                                    //Lets tell jewel manager we need the jewel
                                    JewelManager.InstanceJewelEvent(new NeedJewelEvent(DfkWallet,
                                        NeedJewelEvent.JewelEventRequestTypes.NeedJewel, QuestCurrentMode));

                                    //await eventHub.PublishAsync(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.NeedJewel, QuestCurrentMode));

                                    if (timerCheckInstanceStatus != null)
                                        timerCheckInstanceStatus.Enabled = true;
                                }
                                else
                                if (timerCheckInstanceStatus != null)
                                    timerCheckInstanceStatus.Enabled = true;
                            }
                            else
                            if (timerCheckInstanceStatus != null)
                                timerCheckInstanceStatus.Enabled = true;
                        }
                        else
                        if (timerCheckInstanceStatus != null)
                            timerCheckInstanceStatus.Enabled = true;

                        break;

                    //We've quested, canceled, and waiting on stamina to start again!
                    case QuestActivityMode.WaitingOnStamina:
                        //We're waiting on stamina.  Check Hero stamina. If 15 > then 
                        //switch to QUESTMODE then call timer manually
                        DfkWallet.AssignedHeroStamina = await
                            new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);

                        //Available stam > 15. Lets move to quest mode
                        if (DfkWallet.AssignedHeroStamina >= 15)
                        {
                            QuestCurrentMode = QuestActivityMode.WantsToQuest;
                        }

                        if(timerCheckInstanceStatus != null)
                            timerCheckInstanceStatus.Enabled = true;
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
                                if (!questStatus.IsQuesting)
                                {
                                    //Prior contract call finally executed. Update app telling it to move into stamina mode
                                    QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                    if (timerCheckInstanceStatus != null)
                                        timerCheckInstanceStatus.Enabled = true;
                                    break;
                                }
                            }


                            //Lets cancel the quest
                            var cancelQuestResponse = await
                                new QuestContractHandler().CompleteQuesting(DfkWallet,
                                    DfkWallet.AssignedHero);
                            if (cancelQuestResponse)
                            {
                                QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                //Tell system your not questing now
                                await eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                    WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingComplete));

                                //Lets tell jewel manager we're done (only if we successfully completed quest contract)
                                JewelManager.InstanceJewelEvent(new NeedJewelEvent(DfkWallet,
                                    NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                //await eventHub.PublishAsync(new NeedJewelEvent(DfkWallet,
                                //    NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));
                                _hasTheJewel = false;

                                if (timerCheckInstanceStatus != null)
                                    timerCheckInstanceStatus.Enabled = true;
                            }
                            else
                            if (timerCheckInstanceStatus != null)
                                timerCheckInstanceStatus.Enabled = true;
                        }
                        else
                        if (timerCheckInstanceStatus != null)
                            timerCheckInstanceStatus.Enabled = true;
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
                                if (!questStatus.IsQuesting)
                                {
                                    //Prior contract call finally executed. Update app telling it to move into stamina mode
                                    QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                    if (timerCheckInstanceStatus != null)
                                        timerCheckInstanceStatus.Enabled = true;
                                    break;
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
                                await eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                    WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingCanceled));

                                //Lets tell jewel manager we're done (only if we succesfully canceled quest contract)
                                JewelManager.InstanceJewelEvent(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                //await eventHub.PublishAsync(new NeedJewelEvent(DfkWallet,
                                //    NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));
                                _hasTheJewel = false;


                                if (timerCheckInstanceStatus != null)
                                    timerCheckInstanceStatus.Enabled = true;
                            }
                            else
                            if (timerCheckInstanceStatus != null)
                                timerCheckInstanceStatus.Enabled = true;
                        }
                        else
                        {
                            if (timerCheckInstanceStatus != null)
                                timerCheckInstanceStatus.Enabled = true;
                        }
                        break;

                    case QuestActivityMode.WantsToQuest:
                        if (_hasTheJewel)
                        {
                            //Add a check to see if we're questing already
                            var amIQuestingAlready =
                                await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);
                            if (amIQuestingAlready != null)
                            {
                                if (amIQuestingAlready.IsQuesting)
                                {
                                    QuestCurrentMode = QuestActivityMode.Questing;

                                    //Tell system your questing
                                    await eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                        WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing));

                                    //Lets tell jewel manager we're done (only if we successfully started quest contract)
                                    JewelManager.InstanceJewelEvent(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));
                                    //await eventHub.PublishAsync(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                    if (timerCheckInstanceStatus != null)
                                        timerCheckInstanceStatus.Enabled = true;
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
                                        await eventHub.PublishAsync(new WalletsOnQuestsMessageEvent(DfkWallet,
                                            WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing));

                                        //Lets tell jewel manager we're done (only if we succesfully started quest contract)
                                        JewelManager.InstanceJewelEvent(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));
                                        //await eventHub.PublishAsync(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel, QuestCurrentMode));

                                        if (timerCheckInstanceStatus != null)
                                            timerCheckInstanceStatus.Enabled = true;
                                    }
                                    else
                                    {
                                        if (timerCheckInstanceStatus != null)
                                            timerCheckInstanceStatus.Enabled = true;
                                    }
                                }
                            }
                            else
                            {
                                if (timerCheckInstanceStatus != null)
                                    timerCheckInstanceStatus.Enabled = true;
                            }
                        }
                        else
                        {
                            if (timerCheckInstanceStatus != null)
                                timerCheckInstanceStatus.Enabled = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

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