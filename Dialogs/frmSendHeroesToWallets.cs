using System;
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
            var herosXfered = 0;

            var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
            if (sourceWallet != null)
            {
                var sourceHeroes = new HeroContractHandler().GetWalletHeroes(sourceWallet.WalletAccount).Result.ToList();
                if (sourceHeroes.Count > 0)
                {
                    //remove the source heroid thats assigned from the list to send
                    sourceHeroes.Remove(sourceWallet.AssignedHero);

                    var walletsNeedingHeroes =
                        WalletManager.GetWallets().Where(x => x.AssignedHero == 0 && x.HasDkProfile);
                    foreach (var walletsNeedingHero in walletsNeedingHeroes)
                    {
                        var heroToSend = sourceHeroes.FirstOrDefault();
                        if (heroToSend == 0)
                        {
                            break;
                        }

                        //Send hero to current wallet that needs a hero
                        var heroSentToWalletResult =
                            await new HeroContractHandler().SendHeroToWallet(sourceWallet,
                                walletsNeedingHero.WalletAccount,
                                heroToSend);
                        if (heroSentToWalletResult)
                        {
                            await eventHub.PublishAsync(new MessageEvent()
                            {
                                Content = $"[Wallet:{walletsNeedingHero.Address}] => Received => [Hero:{heroToSend}]"
                            });

                            herosXfered++;
                        }

                        //Remove the hero from source list so we dont try to send it somewhere else
                        sourceHeroes.Remove(heroToSend);
                    }
                }

                WalletManager.ReloadWalletHeroes();

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