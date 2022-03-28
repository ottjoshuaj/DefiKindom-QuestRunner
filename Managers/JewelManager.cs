using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

using PubSub;

using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.Managers
{
    internal static class JewelManager
    {
        #region Internals

        internal static List<DfkWallet> _walletsNeedingJewel = new List<DfkWallet>();

        static Hub eventHub = Hub.Default;

        private static Timer timerToCheckWhoNextGetsJewel;
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
        public static async Task Init()
        {
            if (MonitorIsReady) return;

            //Subscribe to events
            eventHub.Subscribe<NeedJewelEvent>(InstanceJewelEvent);

            //Get The Jewel Owner
            await eventHub.PublishAsync(new MessageEvent { Content = $"Checking for jewel holder...." });

            var jewelInfo = await WalletManager.GetJewelHolder();
            if (jewelInfo != null)
            {
                _currentWalletWithTheJewel = jewelInfo.Holder;

                //Publish Jewel Information to screen
                await eventHub.PublishAsync(new JewelInformationEvent { JewelInformation = jewelInfo });

                //Update the Wallet List so things are accurate
                WalletManager.SetJewelWalletHolder(jewelInfo);
            }
            

            //New Timer Instance
            timerToCheckWhoNextGetsJewel = new Timer(Settings.Default.JewelInstanceMsInterval);
            timerToCheckWhoNextGetsJewel.Elapsed += TimerToCheckWhoNextGetsJewelOnElapsed;
            timerToCheckWhoNextGetsJewel.Enabled = true;
            timerToCheckWhoNextGetsJewel.Start();

            //No wallets in queue. No need to do anything else.
            //Lets just monitor the jewel location
            //First time.  Find the location of the jewel
            var daJewelHolder = await WalletManager.GetJewelHolder();
            if (daJewelHolder != null)
            {
                //Publish message event
                await eventHub.PublishAsync(new ManageWalletGridEvent { Wallet = daJewelHolder.Holder });

                WalletManager.SaveWallets();
            }


            //Mark Monitor as READY for the app to be allowed to work
            MonitorIsReady = true;
        }

        #endregion

        #region Event Subscriptions

        private static void InstanceJewelEvent(NeedJewelEvent instance)
        {
            switch (instance.RequestType)
            {
                case NeedJewelEvent.JewelEventRequestTypes.NeedJewel:
                    //Incoming wallet needs the jewel
                    lock (_walletsNeedingJewel)
                    {
                        //Was wallet instance already in list? No sense in double adding!
                        if (_walletsNeedingJewel.All(x =>
                                x.Address.Trim().ToUpper() != instance.Wallet.Address.Trim().ToUpper()))
                        {
                            _walletsNeedingJewel.Add(instance.Wallet);

                            eventHub.PublishAsync(new MessageEvent { Content = $"[Wallet:{instance.Wallet.Name} => {instance.Wallet.Address}] => Wants the jewel...(Queued)." });
                        }
                    }
                    break;


                case NeedJewelEvent.JewelEventRequestTypes.FinishedWithJewel:
                    //Incoming wallet needs the jewel
                    lock (_walletsNeedingJewel)
                    {
                        //WAllet is DONE with the jewel, make sure wallet is in the queue list and remove it
                        if (_walletsNeedingJewel.Any(x => x.Address != instance.Wallet.Address))
                        {
                            _walletsNeedingJewel.Remove(instance.Wallet);

                            eventHub.PublishAsync(new MessageEvent { Content = $"[Wallet:{instance.Wallet.Name} => {instance.Wallet.Address}] => Is Finished With Jewel...(Removed from Queue)." });
                        }
                    }

                    _jewelIsBusy = false;
                    break;
            }
        }

        #endregion

        #region Timer that checks who is next in line for da Jewel

        private static async void TimerToCheckWhoNextGetsJewelOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_jewelIsBusy) return; //Dont execute anything since an instance is STILL using the jewel

            timerToCheckWhoNextGetsJewel.Enabled = false;

            DfkWallet walletNextInQueue;

            //Timer hit (everyone 2 seconds)
            //Lets see whos requesting the jewel
            lock (_walletsNeedingJewel)
            {
                //Is there ANY wallets in the queue atm?
                walletNextInQueue = _walletsNeedingJewel.FirstOrDefault();
            }

            if (walletNextInQueue == null)
            {
                timerToCheckWhoNextGetsJewel.Enabled = true;
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
                    await eventHub.PublishAsync(new ManageWalletGridEvent { Wallet = _currentWalletWithTheJewel });

                    WalletManager.SaveWallets();
                }
                else
                {
                    //Not good , couldnt find the jewel holder!
                    timerToCheckWhoNextGetsJewel.Enabled = true;
                    return;
                }
            }

            //Is current wallet with jewel the same as who is next in quest?
            //Meaning, is the person who WANTS the jewel the person who HAS DA JEWEL?
            if (_currentWalletWithTheJewel.Address.Trim().ToUpper() == walletNextInQueue.Address.Trim().ToUpper())
            {
                //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                _jewelIsBusy = true;

                await eventHub.PublishAsync(new MessageEvent { Content = $"[Destination Wallet:${walletNextInQueue.Address}] => already has the jewel!" });

                //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                await eventHub.PublishAsync(new NeedJewelEvent(walletNextInQueue, NeedJewelEvent.JewelEventRequestTypes.JewelMovedToAccount));
            }
            else
            {
                eventHub.Publish(new MessageEvent
                {
                    Content =
                        $"[Source Wallet:{_currentWalletWithTheJewel.Address}] => [Destination Wallet:${walletNextInQueue.Address}] => Moving Jewel To New Destination"
                });

                var result =
                    await new JewelContractHandler().SendJewelToAccount(_currentWalletWithTheJewel, walletNextInQueue);
                if (result)
                {
                    //If Jewel is moving to a new account. Lets check for balance changes
                    var jewelHolder = await WalletManager.GetJewelHolder(walletNextInQueue.WalletAccount.Address);
                    if (jewelHolder != null)
                    {
                        //Publish message event
                        await eventHub.PublishAsync(new ManageWalletGridEvent { Wallet = _currentWalletWithTheJewel });
                    }

                    //Tell current wallet jewel has been removed from their account
                    await eventHub.PublishAsync(new NeedJewelEvent(_currentWalletWithTheJewel, NeedJewelEvent.JewelEventRequestTypes.JewelMovedOffAccount));

                    //Since success we need to tell the system that this current wallet now holds the jewel
                    _currentWalletWithTheJewel = walletNextInQueue;

                    //Mark Jewel Busy (this will swap to false once the instance is DONE with its process)
                    _jewelIsBusy = true;

                    await eventHub.PublishAsync(new MessageEvent { Content = $"[Source Wallet:{_currentWalletWithTheJewel.Address}] => [Destination Wallet:${walletNextInQueue.Address}] => Jewel has been moved!" });

                    //Successfully sent the jewel to the address. Lets raise the event so the instance knows!
                    await eventHub.PublishAsync(new NeedJewelEvent(walletNextInQueue, NeedJewelEvent.JewelEventRequestTypes.JewelMovedToAccount));
                }
            }


            timerToCheckWhoNextGetsJewel.Enabled = true;
        }

        #endregion
    }
}
