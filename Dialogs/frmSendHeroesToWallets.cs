using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using PubSub;
using Telerik.WinControls.UI;
using Telerik.WinControls;

using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Managers.Contracts;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmSendHeroesToWallets : RadForm
    {
        #region Internals

        private bool isBusy;
        static Hub eventHub = Hub.Default;

        #endregion

        #region Delegates

        private delegate void ShowRadAlertMessageBoxDelegate(string text, string title);

        #endregion

        #region Constructor(s)

        public frmSendHeroesToWallets()
        {
            InitializeComponent();

            FormClosing += OnFormClosing;
        }

        #endregion

        #region Form Events


        private void frmSendHeroesToWallets_Load(object sender, EventArgs e)
        {

        }


        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = isBusy;
        }

        #endregion

        #region Button Events

        private async void btnSendHeroesToWallets_Click(object sender, EventArgs e)
        {
            btnSendHeroesToWallets.Enabled = false;
            btnSendHeroesToWallets.Text = @"Sending Heroes...";

            isBusy = true;

            await Task.Run(SendHeroesToWallets);

            isBusy = false;

            btnSendHeroesToWallets.Text = @"Send Heroes";
            btnSendHeroesToWallets.Enabled = true;
        }

        #endregion

        #region Send Heroes To Wallets

        async Task<bool> SendHeroesToWallets()
        {
            var heroesTransferedOffSourceWallet = new List<int>();
            var herosXfered = 0;

            var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
            if (sourceWallet != null)
            {
                var sourceHeroes = new HeroContractHandler().GetWalletHeroes(sourceWallet.WalletAccount).Result.ToList();
                if (sourceHeroes.Count > 0)
                {
                    var walletsNeedingHeroes =
                        WalletManager.GetWallets().Where(x => x.AssignedHero == 0 && x.HasDkProfile).ToList();

                    for (var i = 0; i < walletsNeedingHeroes.Count; i++)
                    {
                        var walletNeedingHero = walletsNeedingHeroes[i];
                        
                        //Does wallet have a hero? Lets 100% make sure
                        var walletHeroCheck =
                            await new HeroContractHandler().GetWalletHeroes(walletNeedingHero.WalletAccount);
                        if (walletHeroCheck != null)
                        {
                            walletNeedingHero.AvailableHeroes = walletHeroCheck;

                            WalletManager.SaveWallets();

                            continue;
                        }

                        var heroToSend = sourceHeroes.FirstOrDefault();
                        if (heroToSend == 0)
                        {
                            break;
                        }

                        //Send hero to current wallet that needs a hero
                        var heroSentToWalletResult =
                            await new HeroContractHandler().SendHeroToWallet(sourceWallet,
                                walletNeedingHero.WalletAccount,
                                heroToSend);
                        if (heroSentToWalletResult)
                        {
                            await eventHub.PublishAsync(new MessageEvent()
                            {
                                Content = $"[Wallet:{walletNeedingHero.Address}] => Received => [Hero:{heroToSend}]"
                            });


                            //Force "wallet" we are cleaning up to have no assigned hero, no stam, and clear out available heroes
                            WalletManager.GetWallet(walletNeedingHero.Address).AvailableHeroes?.Add(heroToSend);
                            WalletManager.GetWallet(walletNeedingHero.Address).AssignedHeroStamina = await new QuestContractHandler().GetHeroStamina(walletNeedingHero.WalletAccount, heroToSend);

                            //Remove the hero from source list so we dont try to send it somewhere else
                            sourceHeroes.Remove(heroToSend);
                            heroesTransferedOffSourceWallet.Add(heroToSend);


                            herosXfered++;
                        }
                        else
                        {
                            //Transaction failed. Try again
                            //Check if hero somehow still got xfered due to TX lag
                            var heroList =
                                await new HeroContractHandler().GetWalletHeroes(walletNeedingHero.WalletAccount);
                            if (heroList != null)
                            {
                                if (heroList.Any(x => x == heroToSend))
                                {
                                    //Ok so hero eventually did arrive.  Update stuff
                                    WalletManager.GetWallet(walletNeedingHero.Address).AvailableHeroes?.Add(heroToSend);
                                    WalletManager.GetWallet(walletNeedingHero.Address).AssignedHeroStamina = await new QuestContractHandler().GetHeroStamina(walletNeedingHero.WalletAccount, heroToSend);
                                }
                                else
                                {
                                    i -= 1;
                                }
                            }
                            else
                            {
                                i -= 1;
                            }
                        }
                    }
                }

                //Remove all heroes sent to other wallets off the source wallet
                foreach (var heroIdToRemove in heroesTransferedOffSourceWallet)
                    sourceWallet.AvailableHeroes.Remove(heroIdToRemove);

                WalletManager.SaveWallets();


                ShowRadAlertMessageBox( $"{herosXfered} heroes transferred to wallets...", "Heroes Sent To Wallets");
            }
            else
            {
                ShowRadAlertMessageBox("Unable to send heroes!  No Source/Primary Wallet Found!", "Missing Primary/Source Wallet");
            }

            return true;
        }

        #endregion

        #region Messagebox Handlers

        void ShowRadAlertMessageBox(string text, string title)
        {
            if (InvokeRequired)
                Invoke(new ShowRadAlertMessageBoxDelegate(ShowRadAlertMessageBox), text, title);
            else
            {
                RadMessageBox.Show(this, text, title);
            }
        }

        #endregion
    }
}