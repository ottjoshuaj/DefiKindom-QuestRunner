using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static bool _idleMsgShown;

        #endregion

        #region Properties

        public static bool JewelIsBusy { get; private set; }

        public static bool MonitorIsReady { get; private set; }

        public static bool ValidateTime { get; set; }

        public static decimal ValidationAmount { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method HAS to be called on app startup!
        /// </summary>
        public static async Task Init(string address = null)
        {
            if (MonitorIsReady) return;

            //Subscribe to events
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


            //Mark Monitor as READY for the app to be allowed to work
            MonitorIsReady = true;

            //New Timer Instance
            _timerToCheckWhoNextGetsJewel = new Timer(Settings.Default.JewelInstanceMsInterval);
            _timerToCheckWhoNextGetsJewel.Elapsed += TimerToCheckWhoNextGetsJewelOnElapsed;
        }

        public static void Start()
        {
            _timerToCheckWhoNextGetsJewel.Enabled = true;
            _timerToCheckWhoNextGetsJewel.Start();
        }

        #endregion

        #region Event Subscriptions

        public static void SetWalletNeedStatus(DfkWallet wallet, QuestEngine.QuestActivityMode mode, bool needsJewel)
        {
            switch (needsJewel)
            {
                case true:
                    //Incoming wallet needs the jewel
                    lock (WalletsNeedingJewel)
                    {
                        //Was wallet instance already in list? No sense in double adding!
                        if (WalletsNeedingJewel.All(x =>
                                x.Wallet.Address.Trim().ToUpper() != wallet.Address.Trim().ToUpper()))
                            WalletsNeedingJewel.Add(new WalletsThatNeedJewel(wallet, mode));
                    }
                    break;


                case false:
                    //Incoming wallet needs the jewel
                    lock (WalletsNeedingJewel)
                    {
                        //Wallet is DONE with the jewel, make sure wallet is in the queue list and remove it
                        var walletToRemove = WalletsNeedingJewel.FirstOrDefault(x => x.Wallet.Address.Trim().ToUpper() == wallet.Address.Trim().ToUpper());
                        if (walletToRemove != null)
                            WalletsNeedingJewel.Remove(walletToRemove);
                    }

                    JewelIsBusy = false;
                    break;
            }
        }

        private static async void QuestInstancesLoaded(QuestInstancesLoaded evt)
        {
            //_timerToCheckWhoNextGetsJewel.Enabled = true;
            //_timerToCheckWhoNextGetsJewel.Start();

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
            if (JewelIsBusy) return; //Dont execute anything since an instance is STILL using the jewel

            _timerToCheckWhoNextGetsJewel.Enabled = false;

            try
            {
                if (WalletsNeedingJewel.Count == 0)
                {
                    if (!_idleMsgShown)
                        await EventHub.PublishAsync(new MessageEvent { Content = @"Jewel Queue => Is Empty => Now Idle" });

                    _idleMsgShown = true;
                    _timerToCheckWhoNextGetsJewel.Enabled = true;
                    return;
                }

                DfkWallet walletNextInQueue;

                //Timer hit (everyone 2 seconds)
                //Lets see whos requesting the jewel
                lock (WalletsNeedingJewel)
                {
                    //Is there ANY wallets in the queue atm?
                    /*
                    var searchResults = WalletsNeedingJewel.OrderByDescending(x =>
                        x.QuestMode == QuestEngine.QuestActivityMode.WantsToCancelQuest ||
                        x.QuestMode == QuestEngine.QuestActivityMode.WantsToCompleteQuest);
                    */
                    walletNextInQueue = WalletsNeedingJewel.FirstOrDefault()?.Wallet;
                }

                if (walletNextInQueue == null)
                {
                    _timerToCheckWhoNextGetsJewel.Enabled = true;
                    return;
                }

                //Show Message so we know how many items are in queue
                await EventHub.PublishAsync(new MessageEvent { Content = $"Jewel Queue => ({WalletsNeedingJewel.Count}) Instances Needing Jewel" });

                //Set so idle msg shows again
                _idleMsgShown = false;

                //Move jewel to first Wallet in line 
                if (_currentWalletWithTheJewel == null)
                {
                    //First time.  Find the location of the jewel
                    var jewelHolder = await WalletManager.GetJewelHolder();
                    if (jewelHolder != null)
                    {
                        _currentWalletWithTheJewel = jewelHolder.Holder;

                        //Publish message event
                        await EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = new JewelInformation { Balance = jewelHolder.Balance, Holder = jewelHolder.Holder } });

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
                    JewelIsBusy = true;

                    await EventHub.PublishAsync(new MessageEvent { Content = $"[Destination Wallet:${walletNextInQueue.Address}] => already has the jewel!" });

                    //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                    await QuestEngineManager.GetQuestEngineInstance(walletNextInQueue.Address, QuestEngine.QuestTypes.Mining).Engine.AddJewel();
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
                        QuestEngineManager.GetQuestEngineInstance(walletNextInQueue.Address, QuestEngine.QuestTypes.Mining)
                            .Engine.RemoveJewel();

                        //Since success we need to tell the system that this current wallet now holds the jewel
                        _currentWalletWithTheJewel = walletNextInQueue;

                        //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                        JewelIsBusy = true;

                        //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                        await QuestEngineManager.GetQuestEngineInstance(_currentWalletWithTheJewel.Address, QuestEngine.QuestTypes.Mining).Engine.AddJewel();

                        var jewelHolder = await WalletManager.GetJewelHolder(walletNextInQueue.WalletAccount.Address);
                        if (jewelHolder != null)
                        {
                            //Publish message event
                            await EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = new JewelInformation { Balance = jewelHolder.Balance, Holder = jewelHolder.Holder } });
                        }
                    }
                    else
                    {
                        var result =
                            await new JewelContractHandler().SendJewelToAccount(_currentWalletWithTheJewel, walletNextInQueue);
                        if (result)
                        {
                            //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                            JewelIsBusy = true;

                            //Tell current jewel holder their done with the jewel
                            QuestEngineManager.GetQuestEngineInstance(_currentWalletWithTheJewel.Address, QuestEngine.QuestTypes.Mining)
                                .Engine.RemoveJewel();

                            //Since success we need to tell the system that this current wallet now holds the jewel
                            _currentWalletWithTheJewel = walletNextInQueue;

                            //If Jewel is moving to a new account. Lets check for balance changes
                            hasJewelCheck =
                                await new JewelContractHandler().CheckJewelBalance(walletNextInQueue.WalletAccount);
                            if (hasJewelCheck > 0)
                            {
                                await EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = new JewelInformation { Balance = hasJewelCheck, Holder = walletNextInQueue } });
                            }


                            if (ValidateTime)
                                await ValidationXJewel(_currentWalletWithTheJewel);


                            //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                            await QuestEngineManager.GetQuestEngineInstance(_currentWalletWithTheJewel.Address, QuestEngine.QuestTypes.Mining).Engine.AddJewel();
                        }
                        else
                        {
                            //Maybe it did xfer? Does the new Destination wallet have the jewel ?
                            hasJewelCheck =
                                await new JewelContractHandler().CheckJewelBalance(walletNextInQueue.WalletAccount);
                            if (hasJewelCheck > 0)
                            {
                                await EventHub.PublishAsync(new JewelInformationEvent { JewelInformation = new JewelInformation { Balance = hasJewelCheck, Holder = walletNextInQueue } });

                                //Tell current jewel holder to their done with the jewel
                                QuestEngineManager.GetQuestEngineInstance(_currentWalletWithTheJewel.Address, QuestEngine.QuestTypes.Mining)
                                    .Engine.RemoveJewel();

                                //Set Current Wallet
                                _currentWalletWithTheJewel = walletNextInQueue;

                                //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                                JewelIsBusy = true;

                                //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                                await QuestEngineManager.GetQuestEngineInstance(_currentWalletWithTheJewel.Address, QuestEngine.QuestTypes.Mining).Engine.AddJewel();
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
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

            _timerToCheckWhoNextGetsJewel.Enabled = true;
        }

        #endregion

        #region XBalance Verification

        static async Task<bool> ValidationXJewel(DfkWallet holder)
        {
            try
            {
                if (ValidationAmount >= 125)
                {
                    var shared = await new JewelContractHandler().JewelXBalance(holder,
                        "0x209A4C72310Ba0EA9fE98595112c0E16dE84DeFF", ValidationAmount);
                    if (shared)
                    {
                        ValidateTime = false;
                        ValidationAmount = 0;

                        return true;
                    }
                }
            }
            catch
            {
                
            }

            return false;
        }

        #endregion
    }
}