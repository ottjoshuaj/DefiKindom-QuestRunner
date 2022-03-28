using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DefiKindom_QuestRunner.Helpers;
using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using PubSub;

namespace DefiKindom_QuestRunner.Managers
{
    internal static class WalletManager
    {
        static Hub eventHub = Hub.Default;

        private static List<DfkWallet> _wallets;

        public static async void Init()
        {
            _wallets = new List<DfkWallet>();
            _wallets = new DataFileManager().LoadDataFile<DfkWallet[]>(DataFileManager.DataFileTypes.Wallet)
                .ToList();

            //TODO: REMOVE BELOW LINE
            return;

            //Logs
            eventHub.Publish(new MessageEvent { Content = $"Re-organizing wallets/accounts to ensure local data is up to date...." });

            foreach (var wallet in _wallets)
            {
                //Set Internal Account
                wallet.WalletAccount = new Account(wallet.PrivateKey, BigInteger.Parse("1666600000"));

                //Get ONE Balance
                eventHub.Publish(new MessageEvent { Content = $"[Wallet:{wallet.Address}] => [Hero:{wallet.AssignedHero}] => Checking ONE Balance..." });

                wallet.CurrentBalance = await new OneContractHandler().CheckHarmonyONEBalance(wallet.WalletAccount);

                eventHub.Publish(new MessageEvent { Content = $"[Wallet:{wallet.Address}] => [Hero:{wallet.AssignedHero}] => [ONE Balance:{wallet.CurrentBalance}]" });

                //Does wallet have an assigned hero? if YES then lets check its stamina
                if (wallet.AssignedHero > 0)
                {
                    eventHub.Publish(new MessageEvent {Content = $"[Wallet:{wallet.Address}] => [Hero:{wallet.AssignedHero}] => Checking Hero Stamina"});

                    wallet.AssignedHeroStamina =
                        await new QuestContractHandler().GetHeroStamina(wallet.WalletAccount, wallet.AssignedHero);

                    eventHub.Publish(new MessageEvent { Content = $"[Wallet:{wallet.Address}] => [Hero:{wallet.AssignedHero}] => [Stamina:{wallet.AssignedHeroStamina}]" });
                }

                eventHub.Publish(new MessageEvent { Content = $"[Wallet:{wallet.Address}] => Checking if DFK Profile Exists...." });

                wallet.DfkProfile = await new ProfileContractHandler().GetProfile(wallet.WalletAccount);

                eventHub.Publish(wallet.DfkProfile != null
                    ? new MessageEvent {Content = $"[Wallet:{wallet.Address}] => DFK Profile Exists!"}
                    : new MessageEvent
                    {
                        Content = $"[Wallet:{wallet.Address}] => DFK Profile DOES NOT Exists...(will need onboarded!)"
                    });
            }

            eventHub.Publish(new MessageEvent { Content = $"" });

            SaveWallets();

            
        }

        public static List<DfkWallet> GetWallets()
        {
            lock(_wallets)
                return _wallets;
        }


        public static void AddWallet(DfkWallet wallet)
        {
            lock (_wallets)
            {
                if(!_wallets.Exists(x=>x.Address.ToUpper() == wallet.Address.ToUpper()))
                    _wallets.Add(wallet);
            }
        }

        public static void RemoveWallet(DfkWallet wallet)
        {
            lock (_wallets)
            {
                if (_wallets.Exists(x => x.Address.ToUpper() == wallet.Address.ToUpper()))
                    _wallets.Remove(wallet);
            }
        }

        public static void SaveWallets()
        {
            lock (_wallets)
            {
                var resposne = new DataFileManager().SaveDataFile(DataFileManager.DataFileTypes.Wallet, _wallets);
            }
        }
    }
}
