using System;
using System.Linq;
using System.Timers;

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
            Init,
            WantsToQuest,
            Questing,
            WantsToCancelQuest,
            WaitingOnStamina,
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
            eventHub.Subscribe<NeedJewelEvent>(NeedJewelEventHandler);
        }

        public QuestEngine(DfkWallet wallet, QuestTypes type) : this()
        {
            DfkWallet = wallet;
            QuestType = type;

            QuestCurrentMode = QuestActivityMode.Init;
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
        public void Start()
        {
            HookUpTimer();

            eventHub.Publish(new MessageEvent {Content = $"[Wallet:{DfkWallet.Name} => {DfkWallet.Address}] Added to QuestEngine & QuestInstance is running..."});

            //Call timer first time Manually
            TimerCheckInstanceStatusOnElapsed(null, null);
        }

        public void Stop()
        {
            timerCheckInstanceStatus.Enabled = false;
            timerCheckInstanceStatus.Elapsed -= null;
            timerCheckInstanceStatus.Dispose();
            timerCheckInstanceStatus = null;
        }

        #endregion

        #region Timer Event

        private async void TimerCheckInstanceStatusOnElapsed(object sender, ElapsedEventArgs e)
        {
            timerCheckInstanceStatus.Enabled = false;

            try
            {
                switch (QuestCurrentMode)
                {
                    //First time quest instance is firing up
                    case QuestActivityMode.Init:
                        //Check current wallet status
                        DfkWallet.AssignedHeroQuestStatus =
                            await new QuestContractHandler().GetHeroQuestStatus(DfkWallet.WalletAccount,
                                DfkWallet.AssignedHero);

                        if (DfkWallet.ReadyToWork)
                        {
                            //Do we have quest data on the block chain?
                            if (DfkWallet.AssignedHeroQuestStatus != null &&
                                !DfkWallet.AssignedHeroQuestStatus.IsQuesting)
                            {
                                //Wallet CAN work on the blockchain and isnt on a active quest
                                QuestCurrentMode = QuestActivityMode.WantsToQuest;

                                //Recall timer to force jewel request
                                TimerCheckInstanceStatusOnElapsed(null, null);
                            }
                            else
                            {
                                //No available quest info available
                                //lets move to "waiting on stamina so that it checks and fires up wants to quest
                                QuestCurrentMode = QuestActivityMode.WantsToQuest;

                                //Recall timer to force jewel request
                                TimerCheckInstanceStatusOnElapsed(null, null);
                            }
                        }
                        break;

                    //We know 100% we're questing, now we must determine if its time to CANCEL!
                    case QuestActivityMode.Questing:
                        //Compare start time to now. If time >= 155 minutes then we must move to CANCEL event
                        if (DfkWallet.ReadyToWork && DfkWallet.AssignedHeroQuestStatus != null &&
                            DfkWallet.AssignedHeroQuestStatus.IsQuesting)
                        {
                            DfkWallet.AssignedHeroStamina = await
                                new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);

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
                                    eventHub.Publish(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.NeedJewel));

                                    //Recall timer to force jewel request
                                    TimerCheckInstanceStatusOnElapsed(null, null);
                                }
                            }
                        }
                        break;

                    //We've quested, canceled, and waiting on stamina to start again!
                    case QuestActivityMode.WaitingOnStamina:
                        //We're waiting on stamina.  Check Hero stamina. If 15 > then 
                        //switch to QUESTMODE then call timer manually
                        DfkWallet.AssignedHeroStamina = await
                            new QuestContractHandler().GetHeroStamina(DfkWallet.WalletAccount, DfkWallet.AssignedHero);

                        if (DfkWallet.AssignedHeroStamina >= 15)
                        {
                            QuestCurrentMode = QuestActivityMode.WantsToQuest;


                            //Recall timer to force jewel request
                            TimerCheckInstanceStatusOnElapsed(null, null);
                        }

                        //Tell system your questing
                        eventHub.Publish(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.WaitingOnStamina));
                        break;

                    case QuestActivityMode.WantsToCancelQuest:
                        if (_hasTheJewel)
                        {
                            //Lets cancel the quest
                            var cancelQuestResponse = await
                                new QuestContractHandler().CancelQuesting(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);
                            if (cancelQuestResponse)
                            {
                                QuestCurrentMode = QuestActivityMode.WaitingOnStamina;

                                //Tell system your not questing now
                                eventHub.Publish(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingCanceled));
                            }

                            //Lets tell jewel manager we're done
                            eventHub.Publish(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel));
                        }
                        break;

                    case QuestActivityMode.WantsToQuest:
                        if (_hasTheJewel)
                        {
                            //Start the quest
                            var startQuestResponse = await
                                new QuestContractHandler().StartQuesting(DfkWallet.WalletAccount,
                                    DfkWallet.AssignedHero);
                            if (startQuestResponse)
                            {
                                QuestCurrentMode = QuestActivityMode.Questing;

                                //Tell system your questing
                                eventHub.Publish(new WalletsOnQuestsMessageEvent(DfkWallet, WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing));
                            }

                            //Lets tell jewel manager we're done
                            eventHub.Publish(new NeedJewelEvent(DfkWallet, NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }

            timerCheckInstanceStatus.Enabled = true;
        }

        #endregion

        #region Event Subscriptions

        private void NeedJewelEventHandler(NeedJewelEvent jewelEvent)
        {
            if (jewelEvent.Wallet.Address.Trim().ToUpper() == DfkWallet.Address.Trim().ToUpper())
            {
                switch (jewelEvent.RequestType)
                {
                    case NeedJewelEvent.JewelEventRequestTypes.JewelMovedOffAccount:
                        _hasTheJewel = false;

                        TimerCheckInstanceStatusOnElapsed(null, null);
                        break;

                    case NeedJewelEvent.JewelEventRequestTypes.JewelMovedToAccount:
                        _hasTheJewel = true;
                        break;
                }
            }
        }

        #endregion

        #region Internal Methods

        void HookUpTimer()
        {
            timerCheckInstanceStatus = new Timer(Settings.Default.QuestInstanceMsInterval);
            timerCheckInstanceStatus.Elapsed += TimerCheckInstanceStatusOnElapsed;
            timerCheckInstanceStatus.Enabled = true;
            timerCheckInstanceStatus.Start();
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
