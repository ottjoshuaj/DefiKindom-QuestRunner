﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using PubSub;

using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Managers.Engines;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.Managers
{
    internal static class JewelManager
    {
        #region Internal Objects

        public class WalletsThatNeedJewel
        {
            public WalletsThatNeedJewel(DfkWallet wallet, QuestEngine.QuestActivityMode mode)
            {
                Wallet = wallet;
                QuestMode = mode;
            }

            public DfkWallet Wallet { get; }

            public QuestEngine.QuestActivityMode QuestMode { get; }
        }
         

        #endregion

        #region Internals

        internal static List<WalletsThatNeedJewel> WalletsNeedingJewel = new List<WalletsThatNeedJewel>();

        internal static int _jewelMoveAttempts = 0;
        static readonly Hub EventHub = Hub.Default;
        private static Timer _timerToCheckWhoNextGetsJewel;
        private static DfkWallet _currentWalletWithTheJewel;
        private static bool _jewelIsBusy;

        #endregion

        #region Properties

        public static bool MonitorIsReady { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method HAS to be called on app startup!
        /// </summary>
        public static async Task Init(string address = null)
        {
            if (MonitorIsReady) return;

            //Subscribe to events
            EventHub.Subscribe<NeedJewelEvent>(InstanceJewelEvent);
            EventHub.Subscribe<PreferenceUpdateEvent>(PreferencesEventHandler);
            EventHub.Subscribe<QuestInstancesLoaded>(QuestInstancesLoaded);

            //Get The Jewel Owner
            await EventHub.PublishAsync(new MessageEvent { Content = $"Checking for jewel holder...." });

            //See if the last "known" holder has the jewel else it will loop wallets to find it
            var jewelInfo = await WalletManager.GetJewelHolder(address);
            if (jewelInfo != null)
            {
                _currentWalletWithTheJewel = jewelInfo.Holder;

                //Publish Jewel Information to screen
                await EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = jewelInfo });

                //Update the Wallet List so things are accurate
                WalletManager.SetJewelWalletHolder(jewelInfo);
                WalletManager.SaveWallets();

                //Set properties so we can speed it up after app restart
                Settings.Default.LastKnownJewelHolder = jewelInfo.Holder.Address;
                Settings.Default.Save();
            }
            

            //New Timer Instance
            _timerToCheckWhoNextGetsJewel = new Timer(Settings.Default.JewelInstanceMsInterval);
            _timerToCheckWhoNextGetsJewel.Elapsed += TimerToCheckWhoNextGetsJewelOnElapsed;

            //Mark Monitor as READY for the app to be allowed to work
            MonitorIsReady = true;
        }

        public static void InstanceJewelEvent(NeedJewelEvent instance)
        {
            switch (instance.RequestType)
            {
                case NeedJewelEvent.JewelEventRequestTypes.NeedJewel:
                    //Incoming wallet needs the jewel
                    lock (WalletsNeedingJewel)
                    {
                        //Was wallet instance already in list? No sense in double adding!
                        if (WalletsNeedingJewel.All(x =>
                                x.Wallet.Address.Trim().ToUpper() != instance.Wallet.Address.Trim().ToUpper()))
                            WalletsNeedingJewel.Add(new WalletsThatNeedJewel(instance.Wallet, instance.QuestActivityMode));
                    }
                    break;


                case NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel:
                    //Incoming wallet needs the jewel
                    lock (WalletsNeedingJewel)
                    {
                        //Wallet is DONE with the jewel, make sure wallet is in the queue list and remove it
                        var walletToRemove = WalletsNeedingJewel.FirstOrDefault(x => x.Wallet.Address == instance.Wallet.Address);
                        if (walletToRemove != null)
                            WalletsNeedingJewel.Remove(walletToRemove);
                    }

                    _jewelIsBusy = false;
                    break;
            }
        }

        #endregion

        #region Event Subscriptions

        private static async void QuestInstancesLoaded(QuestInstancesLoaded evt)
        {
            _timerToCheckWhoNextGetsJewel.Enabled = true;
            _timerToCheckWhoNextGetsJewel.Start();

            await EventHub.PublishAsync(new MessageEvent { Content = $"Jewel Manager (All Instances Loaded to Queue)...." });
        }

        private static void PreferencesEventHandler(PreferenceUpdateEvent evt)
        {
            if(_timerToCheckWhoNextGetsJewel != null)
                _timerToCheckWhoNextGetsJewel.Interval = evt.JewelTimerInterval;
        }

        #endregion

        #region Timer that checks who is next in line for da Jewel

        private static async void TimerToCheckWhoNextGetsJewelOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_jewelIsBusy) return; //Dont execute anything since an instance is STILL using the jewel

            _timerToCheckWhoNextGetsJewel.Enabled = false;

            DfkWallet walletNextInQueue;

            //Timer hit (everyone 2 seconds)
            //Lets see whos requesting the jewel
            lock (WalletsNeedingJewel)
            {
                //Is there ANY wallets in the queue atm?
                var searchResults = WalletsNeedingJewel.OrderByDescending(x =>
                    x.QuestMode == QuestEngine.QuestActivityMode.WantsToCancelQuest ||
                    x.QuestMode == QuestEngine.QuestActivityMode.WantsToCompleteQuest);
                walletNextInQueue = searchResults.FirstOrDefault()?.Wallet;
            }

            if (walletNextInQueue == null)
            {
                _timerToCheckWhoNextGetsJewel.Enabled = true;
                return;
            }


            //Move jewel to first Wallet in line 
            if (_currentWalletWithTheJewel == null)
            {
                //First time.  Find the location of the jewel
                var jewelHolder = await WalletManager.GetJewelHolder();
                if (jewelHolder != null)
                {
                    _currentWalletWithTheJewel = jewelHolder.Holder;

                    //Publish message event
                    EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = new JewelInformation { Balance = jewelHolder.Balance, Holder = jewelHolder.Holder } });

                    WalletManager.SaveWallets();
                }
                else
                {
                    //Not good , couldnt find the jewel holder!
                    _timerToCheckWhoNextGetsJewel.Enabled = true;
                    return;
                }
            }

            //Is current wallet with jewel the same as who is next in quest?
            //Meaning, is the person who WANTS the jewel the person who HAS DA JEWEL?
            if (_currentWalletWithTheJewel.Address.Trim().ToUpper() == walletNextInQueue.Address.Trim().ToUpper())
            {
                //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                _jewelIsBusy = true;

                await EventHub.PublishAsync(new MessageEvent { Content = $"[Destination Wallet:${walletNextInQueue.Address}] => already has the jewel!" });

                //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                var questInstance = QuestEngineManager.GetQuestEngineInstance(walletNextInQueue.Address, QuestEngine.QuestTypes.Mining);
                questInstance?.Engine.SetAsJewelOwner(true);
            }
            else
            {
                decimal hasJewelCheck;

                //Do we have the damn jewel?  Sometimes we get stuck in this weird loop even though
                //a COMPLETE has come back from the api.... This "EXTRA" check will see if the current wallet that wants the
                //jewel has it or not.
                hasJewelCheck = await new JewelContractHandler().CheckJewelBalance(walletNextInQueue.WalletAccount);
                if (hasJewelCheck > 0)
                {
                    //Tell current jewel holder to their done with the jewel
                    var oldQuestInstance = QuestEngineManager.GetQuestEngineInstance(_currentWalletWithTheJewel.Address, QuestEngine.QuestTypes.Mining);
                    oldQuestInstance?.Engine.SetAsJewelOwner(false);

                    //Since success we need to tell the system that this current wallet now holds the jewel
                    _currentWalletWithTheJewel = walletNextInQueue;

                    //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                    _jewelIsBusy = true;

                    //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                    var questInstance = QuestEngineManager.GetQuestEngineInstance(walletNextInQueue.Address, QuestEngine.QuestTypes.Mining);
                    questInstance?.Engine.SetAsJewelOwner(true);

                    var jewelHolder = await WalletManager.GetJewelHolder(walletNextInQueue.WalletAccount.Address);
                    if (jewelHolder != null)
                    {
                        //Publish message event
                        EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = new JewelInformation { Balance = jewelHolder.Balance, Holder = jewelHolder.Holder } });
                    }
                }
                else
                {
                    var result =
                        await new JewelContractHandler().SendJewelToAccount(_currentWalletWithTheJewel, walletNextInQueue);
                    if (result)
                    {
                        //If Jewel is moving to a new account. Lets check for balance changes
                        var jewelHolder = await WalletManager.GetJewelHolder(walletNextInQueue.WalletAccount.Address);
                        if (jewelHolder != null)
                        {
                            //Publish message event
                            EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = new JewelInformation { Balance = jewelHolder.Balance, Holder = jewelHolder.Holder } });
                        }

                        //Tell current jewel holder to their done with the jewel
                        var oldQuestInstance = QuestEngineManager.GetQuestEngineInstance(_currentWalletWithTheJewel.Address, QuestEngine.QuestTypes.Mining);
                        oldQuestInstance?.Engine.SetAsJewelOwner(false);

                        //Since success we need to tell the system that this current wallet now holds the jewel
                        _currentWalletWithTheJewel = walletNextInQueue;

                        //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                        _jewelIsBusy = true;

                        //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                        var questInstance = QuestEngineManager.GetQuestEngineInstance(walletNextInQueue.Address, QuestEngine.QuestTypes.Mining);
                        questInstance?.Engine.SetAsJewelOwner(true);
                    }
                    else
                    {
                        //Maybe it did xfer? Does the new Destination wallet have the jewel ?
                        hasJewelCheck =
                            await new JewelContractHandler().CheckJewelBalance(walletNextInQueue.WalletAccount);
                        if (hasJewelCheck > 0)
                        {
                            EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = new JewelInformation { Balance = hasJewelCheck, Holder = walletNextInQueue } });

                            //Tell current jewel holder to their done with the jewel
                            var oldQuestInstance = QuestEngineManager.GetQuestEngineInstance(_currentWalletWithTheJewel.Address, QuestEngine.QuestTypes.Mining);
                            oldQuestInstance?.Engine.SetAsJewelOwner(false);

                            //Set Current Wallet
                            _currentWalletWithTheJewel = walletNextInQueue;

                            //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                            _jewelIsBusy = true;

                            //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                            var questInstance = QuestEngineManager.GetQuestEngineInstance(walletNextInQueue.Address, QuestEngine.QuestTypes.Mining);
                            questInstance?.Engine.SetAsJewelOwner(true);
                        }


                        //We got a FAIL to send jewel to the account....not sure why though?  Transaction timeout ? Failure ?
                        //Timer will loop and try sending again.
                        _jewelMoveAttempts++;

                        if (_jewelMoveAttempts >= 3)
                        {
                            //Maybe we already have the jewel?
                        }
                    }
                }
            }


            _timerToCheckWhoNextGetsJewel.Enabled = true;
        }

        #endregion
    }
}