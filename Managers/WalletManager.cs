using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using PubSub;

using DefiKindom_QuestRunner.Helpers;
using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using DefiKindom_QuestRunner.Managers.Engines;

namespace DefiKindom_QuestRunner.Managers
{
    internal static class WalletManager
    {
        #region Internals

        static Hub eventHub = Hub.Default;

        internal static List<DfkWallet> Wallets = new List<DfkWallet>();

        internal static bool loadedQuestDataInfo;

        #endregion

        #region Properties

        public static bool IsLoaded { get; private set; }

        #endregion

        #region Init Method

        public static void InitSubscription()
        {
            SubscribeToEvents();
        }

        public static async Task Init(bool isQuickInit = false)
        {
            try
            {
                Wallets = new DataFileManager().LoadDataFile<DfkWallet[]>(DataFileManager.DataFileTypes.Wallet)
                    .ToList();
            }
            catch (Exception ex)
            {
                Wallets = new List<DfkWallet>();

                await eventHub.PublishAsync(new MessageEvent
                    { Content = $"No Wallets. Please import a wallet or generate new one(s)..." });

                return;
            }


            //Loop Wallets
            if (Wallets.Count > 0)
            {
                for (var i = 0; i < Wallets.Count; i++)
                {
                    Wallets[i] = await InitWallet(Wallets[i], isQuickInit);
                }

                SaveWallets();
            }

            await eventHub.PublishAsync(new WalletDataRefreshEvent { Complete = true });

            IsLoaded = true;
        }

        #endregion

        #region Event Subscribe & Events

        static void SubscribeToEvents()
        {
            //NeedJewelEvent
            eventHub.Subscribe<JewelEvent>(NeedJewelEventRaise);
            eventHub.Subscribe<WalletsOnQuestsMessageEvent>(UpdateWalletOnQuestEvent);
        }

        static void NeedJewelEventRaise(JewelEvent evt)
        {
            if (evt != null)
            {
                switch (evt.RequestType)
                {
                    case JewelEvent.JewelEventRequestTypes.JewelMovedToAccount:
                        lock (Wallets)
                        {
                            var jewelHolder = Wallets.FirstOrDefault(x => x.JewelBalance > 0);

                            foreach (var wallet in Wallets)
                            {
                                wallet.IsHoldingTheJewel = wallet.Address.Trim().ToUpper() ==
                                                           evt.Wallet.Address.Trim().ToUpper();
                                wallet.JewelBalance = jewelHolder?.JewelBalance ?? 0;
                            }
                        }
                        break;
                }
            }
        }

        static async void UpdateWalletOnQuestEvent(WalletsOnQuestsMessageEvent msgEvent)
        {
            DfkWallet wallet;

            lock (Wallets)
            {
                wallet = Wallets.FirstOrDefault(x =>
                    x.Address.ToLower().Trim() == msgEvent.Wallet.Address.ToLower().Trim());
            }

            switch (msgEvent.OnQuestMessageEventType)
            {
                case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing:
                case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingCanceled:
                case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingComplete:
                    if (wallet != null)
                    {
                        wallet.AssignedHeroQuestStatus =
                            await new QuestContractHandler().GetHeroQuestStatus(wallet.WalletAccount,
                                wallet.AssignedHero);
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods

        public static async void ReloadWalletHeroes()
        {
            foreach (var wallet in GetWallets())
            {
                var walletHeroes = await new HeroContractHandler().GetWalletHeroes(wallet.WalletAccount);
                wallet.AvailableHeroes = walletHeroes;

                if (wallet.HeroProfiles == null)
                    wallet.HeroProfiles = new List<HeroProfile>();
                else
                    wallet.HeroProfiles.Clear();

                //Pull Profile Details for each hero
                foreach (var heroId in wallet.AvailableHeroes)
                {
                    var heroDetails = await new HeroContractHandler().GetHeroDetails(wallet.WalletAccount, heroId);
                    if (heroDetails == null) continue;

                    if (wallet.HeroProfiles.Any(x => x.Id == heroDetails.Id)) continue;

                    wallet.HeroProfiles.Add(heroDetails);
                }

                //Is there an assigned hero? If not assign first one in the list!
                if (wallet.AvailableHeroes == null)
                    wallet.AssignedHeroStamina = 0;
                else if (wallet.AvailableHeroes != null && wallet.AvailableHeroes.Count == 0)
                    wallet.AssignedHeroStamina = 0;

                //Get current stamina info for the hero
                wallet.AssignedHeroStamina =
                    await new QuestContractHandler().GetHeroStamina(wallet.WalletAccount,
                        wallet.AssignedHero);
            }
        }

        public static async Task<bool> ReloadDfkProfile(string address)
        {
            DfkWallet wallet;

            lock (Wallets)
                wallet = Wallets.FirstOrDefault(x => x.Address.Trim().ToUpper() == address.Trim().ToUpper());

            if (wallet != null)
            {
                var profile = await new ProfileContractHandler().GetProfile(wallet.WalletAccount);
                if (profile != null)
                    wallet.DfkProfile = profile;
            }

            return true;
        }

        public static async Task<bool> ReloadOneData(string address)
        {
            DfkWallet wallet;

            lock (Wallets)
                wallet = Wallets.FirstOrDefault(x => x.Address.Trim().ToUpper() == address.Trim().ToUpper());

            if (wallet != null)
            {
                wallet.CurrentBalance = await new OneContractHandler().CheckHarmonyONEBalance(wallet.WalletAccount);
            }

            return true;
        }

        public static async Task<bool> ReloadWalletHeroData(string address)
        {
            DfkWallet wallet;

            lock(Wallets)
                wallet = Wallets.FirstOrDefault(x => x.Address.Trim().ToUpper() == address.Trim().ToUpper());

            if (wallet != null)
            {
                var walletHeroes = await new HeroContractHandler().GetWalletHeroes(wallet.WalletAccount);
                if (walletHeroes.Count == 0)
                {
                    wallet.AssignedHeroStamina = 0;
                    wallet.AssignedHeroQuestStatus = null;
                    wallet.HeroProfiles = null;
                    wallet.AvailableHeroes?.Clear();
                }
                else
                {
                    wallet.AvailableHeroes = walletHeroes;

                    if (wallet.HeroProfiles == null)
                        wallet.HeroProfiles = new List<HeroProfile>();
                    else
                        wallet.HeroProfiles.Clear();

                    //Pull Profile Details for each hero
                    foreach (var heroId in wallet.AvailableHeroes)
                    {
                        var heroDetails = await new HeroContractHandler().GetHeroDetails(wallet.WalletAccount, heroId);
                        if (heroDetails == null) continue;

                        if (wallet.HeroProfiles.Any(x => x.Id == heroDetails.Id)) continue;

                        wallet.HeroProfiles.Add(heroDetails);
                    }

                    //Always make sure the FIRST index of heroes is who is assigned to the wallet
                    if (wallet.AvailableHeroes == null)
                        wallet.AssignedHeroStamina = 0;
                    else if (wallet.AvailableHeroes != null && wallet.AvailableHeroes.Count == 0)
                        wallet.AssignedHeroStamina = 0;

                    //Get current stamina info for the hero
                    wallet.AssignedHeroStamina =
                        await new QuestContractHandler().GetHeroStamina(wallet.WalletAccount,
                            wallet.AssignedHero);
                }

                return true;
            }

            return false;
        }

        public static async Task<DfkWallet> InitWallet(DfkWallet wallet, bool isQuickInit = false)
        {
            //Set Internal Account
            wallet.WalletAccount = GetWalletAccount(wallet.PrivateKey);

            if (!loadedQuestDataInfo)
            {
                //Build Quest Address to Type Info
                var mineQuestInfo =
                    await new QuestContractHandler().GetQuestType(Wallets.FirstOrDefault()?.WalletAccount, QuestEngine.QuestTypes.Mining);
                var fishQuestInfo =
                    await new QuestContractHandler().GetQuestType(Wallets.FirstOrDefault()?.WalletAccount, QuestEngine.QuestTypes.Fishing);
                var forageQuestInfo =
                    await new QuestContractHandler().GetQuestType(Wallets.FirstOrDefault()?.WalletAccount, QuestEngine.QuestTypes.Foraging);
                var wishWellQuestInfo =
                    await new QuestContractHandler().GetQuestType(Wallets.FirstOrDefault()?.WalletAccount, QuestEngine.QuestTypes.WishingWell);

                if(mineQuestInfo != null)
                    Settings.Default.MiningQuestTypeId = mineQuestInfo.TypeId;

                if (fishQuestInfo != null)
                    Settings.Default.FishingQuestTypeId = fishQuestInfo.TypeId;

                if (forageQuestInfo != null)
                    Settings.Default.ForagingQuestTypeId = forageQuestInfo.TypeId;

                if (wishWellQuestInfo != null)
                    Settings.Default.WishingWellQuestTypeId = wishWellQuestInfo.TypeId;

                Settings.Default.Save();

                loadedQuestDataInfo = true;
            }

            await eventHub.PublishAsync(new MessageEvent
            { Content = $"[Wallet:{wallet.Name} => {wallet.WalletAccount.Address}] => Loaded!" });

            if (!isQuickInit)
            {
                //Check to see if onboarded to DFK
                await eventHub.PublishAsync(new MessageEvent
                    { Content = $"[Wallet:{wallet.Name} => {wallet.Address}] => Checking if DFK Profile Exists...." });

                var dfkProfile = await new ProfileContractHandler().GetProfile(wallet.WalletAccount);
                if (dfkProfile != null)
                    wallet.DfkProfile = dfkProfile;

                await eventHub.PublishAsync(wallet.DfkProfile != null
                    ? new MessageEvent { Content = $"[Wallet:{wallet.Name} => {wallet.Address}] => DFK Profile Exists!" }
                    : new MessageEvent
                    {
                        Content =
                            $"[Wallet:{wallet.Name} => {wallet.Address}] => DFK Profile DOES NOT Exists...(will need onboarded!)"
                    });

                //Check to see what heroes are on each wallet
                await eventHub.PublishAsync(new MessageEvent
                { Content = $"[Wallet:{wallet.Name} => {wallet.Address}] => Building a list of heroes on wallet...." });

                var walletHeroes = await new HeroContractHandler().GetWalletHeroes(wallet.WalletAccount);
                wallet.AvailableHeroes = walletHeroes;

                //Build Proper Hero Profiles (This is the stats/info on the NFT cards)
                if (wallet.HeroProfiles == null)
                    wallet.HeroProfiles = new List<HeroProfile>();
                else
                    wallet.HeroProfiles.Clear();

                if (wallet.AvailableHeroes != null)
                {
                    foreach (var heroId in wallet.AvailableHeroes)
                    {
                        var heroDetails = await new HeroContractHandler().GetHeroDetails(wallet.WalletAccount, heroId);
                        if (heroDetails == null) continue;

                        if (wallet.HeroProfiles.Any(x => x.Id == heroDetails.Id)) continue;

                        wallet.HeroProfiles.Add(heroDetails);

                        await eventHub.PublishAsync(new MessageEvent
                            { Content = $"[Wallet:{wallet.Name} => {wallet.Address}] => [HeroId:{heroId}] => Profile Pulled..." });
                    }
                }

                await eventHub.PublishAsync(new MessageEvent
                    { Content = $"[Wallet:{wallet.Name} => {wallet.Address}] => Hero list built" });

                //Find out if the wallet still has an assigned hero
                if (wallet.AvailableHeroes == null)
                    wallet.AssignedHeroStamina = 0;
                else if (wallet.AvailableHeroes != null && wallet.AvailableHeroes.Count == 0)
                    wallet.AssignedHeroStamina = 0;

                //Get Hero status on startup
                if (wallet.AssignedHero > 0)
                {
                    //Get Hero Stamina
                    await eventHub.PublishAsync(new MessageEvent
                    {
                        Content =
                            $"[Wallet:{wallet.Name} => {wallet.Address}] => [Hero:{wallet.AssignedHero}] => Checking Hero Stamina"
                    });

                    wallet.AssignedHeroStamina =
                        await new QuestContractHandler().GetHeroStamina(wallet.WalletAccount,
                            wallet.AssignedHero);

                    await eventHub.PublishAsync(new MessageEvent
                    {
                        Content =
                            $"[Wallet:{wallet.Name} => {wallet.Address}] => [Hero:{wallet.AssignedHero}] => [Stamina:{wallet.AssignedHeroStamina}]"
                    });

                    //Get Hero Quest Status
                    await eventHub.PublishAsync(new MessageEvent
                    {
                        Content =
                            $"[Wallet:{wallet.Name} => {wallet.Address}] => [Hero:{wallet.AssignedHero}] => Checking Hero Quest Status"
                    });

                    wallet.AssignedHeroQuestStatus =
                        await new QuestContractHandler().GetHeroQuestStatus(wallet.WalletAccount,
                            wallet.AssignedHero);

                    if (wallet.AssignedHeroQuestStatus != null && wallet.AssignedHeroQuestStatus.IsQuesting)
                    {
                        await eventHub.PublishAsync(new MessageEvent
                        {
                            Content =
                                $"[Wallet:{wallet.Name} => {wallet.Address}] => [Hero:{wallet.AssignedHero}] => Hero is actively questing..."
                        });
                    }
                    else
                        await eventHub.PublishAsync(new MessageEvent
                        {
                            Content =
                                $"[Wallet:{wallet.Name} => {wallet.Address}] => [Hero:{wallet.AssignedHero}] => Hero is NOT actively questing..."
                        });
                }

                //Get ONE Balance
                await eventHub.PublishAsync(new MessageEvent
                    {Content = $"[Wallet:{wallet.Name} => {wallet.Address}] => Checking ONE Balance..."});

                wallet.CurrentBalance =
                    await new OneContractHandler().CheckHarmonyONEBalance(wallet.WalletAccount);

                await eventHub.PublishAsync(new MessageEvent
                    {Content = $"[Wallet:{wallet.Name} => {wallet.Address}] => [ONE Balance:{wallet.CurrentBalance}]"});

            }

            return wallet;
        }

        public static List<DfkWallet> GetWallets()
        {
            lock(Wallets)
                return Wallets.ToList();
        }

        public static DfkWallet GetWallet(string address)
        {
            lock (Wallets)
                return Wallets.FirstOrDefault(x => x.Address.Trim().ToLower() == address.Trim().ToLower());
        }

        public static DfkWallet GetWallet(DfkWallet wallet)
        {
            lock (Wallets)
                return Wallets.FirstOrDefault(x => x.Address.Trim().ToLower() == wallet.Address.Trim().ToLower());
        }

        public static void AddWallet(DfkWallet wallet)
        {
            lock (Wallets)
            {
                if(!Wallets.Exists(x=>x.Address.ToUpper() == wallet.Address.ToUpper()))
                    Wallets.Add(wallet);
            }
        }

        public static void RemoveWallet(DfkWallet wallet)
        {
            lock (Wallets)
            {
                if (Wallets.Exists(x => x.Address.ToUpper() == wallet.Address.ToUpper()))
                    Wallets.Remove(wallet);
            }
        }

        public static void SetAsSourceWallet(DfkWallet newSourceWallet)
        {
            lock (Wallets)
            {
                foreach (var wallet in Wallets)
                {
                    wallet.IsPrimarySourceWallet =
                        wallet.Address.Trim().ToUpper() == newSourceWallet.Address.Trim().ToUpper();
                }
            }

            SaveWallets();
        }

        public static void SaveWallets()
        {
            lock (Wallets)
            {
                var resposne = new DataFileManager().SaveDataFile(DataFileManager.DataFileTypes.Wallet, Wallets);
            }
        }

        public static async Task<DfkWallet> CreateWallet(string name)
        {
            var newWalletResponse = await new QuickRequest().GetDfkApiResponse<WalletCreateResponse>(
                "/api/wallets/create",
                new WalletCreateRequest
                    { Name = name });

            if (newWalletResponse.Success) return newWalletResponse.Wallet;
            return null;
        }

        public static Account ImportWallet(string privateKey)
        {
            return new Account(privateKey, Chain.MainNet);
        }

        public static Account GetWalletAccount(string privateKey)
        {
            return new Account(privateKey, BigInteger.Parse("1666600000"));
        }

        public static async Task<JewelInformation> GetJewelHolder(string address = null)
        {
            //Get Jewel Info by address
            if (!string.IsNullOrWhiteSpace(address))
            {
                var selWallet = GetWallet(address);
                if (selWallet != null)
                {
                    var jewelBalance = await new JewelContractHandler().CheckJewelBalance(selWallet.WalletAccount);
                    if (jewelBalance > 0)
                    {
                        selWallet.JewelBalance = jewelBalance;
                        selWallet.IsHoldingTheJewel = true;

                        lock (Wallets)
                        {
                            foreach (var wallet in Wallets.Where(wallet => wallet.Address.Trim().ToUpper() != selWallet.Address.Trim().ToUpper()))
                            {
                                wallet.JewelBalance = 0;
                                wallet.IsHoldingTheJewel = false;
                            }
                        }

                        return new JewelInformation
                        {
                            Holder = selWallet,
                            Balance = jewelBalance
                        };
                    }
                }
            }


            JewelInformation jewelInformation = null;
            List<DfkWallet> tempWallets;

            //Get wallets by ref here
            lock (Wallets)
                tempWallets = Wallets.ToList();

            //Loop list of wallets till we find the jewel owner
            foreach (var wallet in tempWallets)
            {
                if (jewelInformation == null)
                {
                    //Jewel hasnt been found yet. Keep looking
                    var jewelBalance = await new JewelContractHandler().CheckJewelBalance(wallet.WalletAccount);
                    if (jewelBalance > 0)
                    {
                        jewelInformation = new JewelInformation
                        {
                            Holder = wallet,
                            Balance = jewelBalance
                        };

                        wallet.JewelBalance = jewelBalance;
                        wallet.IsHoldingTheJewel = true;
                    }
                    else
                    {
                        //Didnt find jewel on this wallet
                        wallet.JewelBalance = 0;
                        wallet.IsHoldingTheJewel = false;
                    }
                }
                else
                {
                    //Jewel has already been found. Just update current wallet as NOT holding wallet
                    wallet.JewelBalance = 0;
                    wallet.IsHoldingTheJewel = false;
                }

            }

            return jewelInformation;
        }

        public static void SetJewelWalletHolder(JewelInformation jewelInfo)
        {
            lock (Wallets)
            {
                foreach (var wallet in Wallets)
                {
                    if (wallet.Address.Trim().ToLower() == jewelInfo.Holder.Address.Trim().ToLower())
                    {
                        wallet.IsHoldingTheJewel = true;
                        wallet.JewelBalance = jewelInfo.Balance;
                    }
                    else
                    {
                        wallet.IsHoldingTheJewel = false;
                        wallet.JewelBalance = 0;
                    }
                }
            }

            SaveWallets();
        }

        #endregion

        #region Quest Instance Methods

        public static async Task<int> OnBoardQuestInstances()
        {
            var instancesStarted = 0;

            IEnumerable<DfkWallet> instancesToOnBoard;

            lock (Wallets)
                instancesToOnBoard = Wallets.Where(wallet => wallet.HasDkProfile).ToList();

            foreach (var wallet in instancesToOnBoard)
            {
                //Check Hero Stamina and current quest status
                wallet.AvailableHeroes = await new HeroContractHandler().GetWalletHeroes(wallet.WalletAccount);
                wallet.AssignedHeroQuestStatus =
                    await new QuestContractHandler().GetHeroQuestStatus(wallet.WalletAccount, wallet.AssignedHero);
                wallet.AssignedHeroStamina =
                    await new QuestContractHandler().GetHeroStamina(wallet.WalletAccount, wallet.AssignedHero);

                //Wallet is already questing 
                if (wallet.IsQuesting)
                {
                    //Make sure appropriate hero questing is set
                    //Push the hero on quest and make sure its FIRST in the list!
                    if (wallet.AvailableHeroes.All(x => x != wallet.AssignedHeroQuestStatus.HeroesOnQuest.First()))
                        wallet.AvailableHeroes.Insert(0, wallet.AssignedHeroQuestStatus.HeroesOnQuest.First());
                    else
                    {
                        //Make sure hero id is ALWAYS first
                        if (wallet.AvailableHeroes.First() != wallet.AssignedHeroQuestStatus.HeroesOnQuest.First())
                        {
                            wallet.AvailableHeroes.Remove(wallet.AssignedHeroQuestStatus.HeroesOnQuest.First());
                            wallet.AvailableHeroes.Insert(0, wallet.AssignedHeroQuestStatus.HeroesOnQuest.First());
                        }
                    }

                    if (wallet.QuestNeedsCompleted)
                    {
                        QuestEngineManager.AddQuestEngine(new QuestEngine(wallet, QuestEngine.QuestTypes.Mining,
                            QuestEngine.QuestActivityMode.WantsToCompleteQuest));

                        await eventHub.PublishAsync(new MessageEvent
                        {
                            Content =
                                $"[Wallet:{wallet.Name} = {wallet.Address}] => [Hero:{wallet.AssignedHero}] => (Actively Questing/Needs Completed)"
                        });
                    } else if (wallet.QuestNeedsCanceled)
                    {
                        QuestEngineManager.AddQuestEngine(new QuestEngine(wallet, QuestEngine.QuestTypes.Mining,
                            QuestEngine.QuestActivityMode.WantsToCancelQuest));

                        await eventHub.PublishAsync(new MessageEvent
                        {
                            Content =
                                $"[Wallet:{wallet.Name} = {wallet.Address}] => [Hero:{wallet.AssignedHero}] => (Actively Questing/Needs Canceled)"
                        });
                    }
                    else
                    {
                        QuestEngineManager.AddQuestEngine(new QuestEngine(wallet, QuestEngine.QuestTypes.Mining,
                            QuestEngine.QuestActivityMode.Questing));

                        await eventHub.PublishAsync(new MessageEvent
                        {
                            Content =
                                $"[Wallet:{wallet.Name} = {wallet.Address}] => [Hero:{wallet.AssignedHero}] => (Actively Questing)"
                        });
                    }

                    instancesStarted++;
                }
                else
                {
                    //Lets always make sure we have the proper hero assigned and get proper stamina amount before
                    //EVER starting a quest instance
                    var heroesOnAccount = await new HeroContractHandler().GetWalletHeroes(wallet.WalletAccount);
                    if (heroesOnAccount != null)
                    {
                        instancesStarted++;

                        //Just ensure account is set
                        wallet.AvailableHeroes = heroesOnAccount;
                        wallet.AssignedHeroStamina =
                            await new QuestContractHandler().GetHeroStamina(wallet.WalletAccount, wallet.AssignedHero);

                        //Wallet isnt questing. Is there enough stamina ?
                        if (wallet.AssignedHeroStamina >= 15)
                        {
                            QuestEngineManager.AddQuestEngine(new QuestEngine(wallet, QuestEngine.QuestTypes.Mining,
                                QuestEngine.QuestActivityMode.WantsToQuest));

                            await eventHub.PublishAsync(new MessageEvent
                            {
                                Content =
                                    $"[Wallet:{wallet.Name} = {wallet.Address}] => [Hero:{wallet.AssignedHero}] => Quest Instance created! (Has Stamina! Wants to Quest)"
                            });
                        }
                        else
                        {
                            QuestEngineManager.AddQuestEngine(new QuestEngine(wallet, QuestEngine.QuestTypes.Mining,
                                QuestEngine.QuestActivityMode.WaitingOnStamina));

                            await eventHub.PublishAsync(new MessageEvent
                            {
                                Content =
                                    $"[Wallet:{wallet.Name} = {wallet.Address}] => [Hero:{wallet.AssignedHero}] => Quest Instance created! (Waiting On Stamina)"
                            });
                        }
                    }
                }
            }

            //Update wallet data file
            SaveWallets();


            //Tell Jewel Manager to start processing
            JewelManager.StartEngine();

            return instancesStarted;
        }

        #endregion
    }
}
