using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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

        static Hub eventHub = Hub.Default;

        #endregion

        #region Delegates

        private delegate void ShowRadAlertMessageBoxDelegate(string text, string title);

        #endregion

        #region Constructor(s)

        public frmSendHeroesToWallets()
        {
            InitializeComponent();
        }

        #endregion

        #region Form Events


        private void frmSendHeroesToWallets_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region Button Events

        private async void btnSendHeroesToWallets_Click(object sender, EventArgs e)
        {
            btnSendHeroesToWallets.Enabled = false;
            btnSendHeroesToWallets.Text = @"Sending Heroes...";

            Closing += delegate(object o, CancelEventArgs args) { args.Cancel = true; };

            await Task.Run(SendHeroesToWallets);

            Closing -= null;

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
                }

                foreach (var heroToSend in sourceHeroes)
                {
                    //do NOT send the source wallet "primary" hero
                    if (heroToSend == sourceWallet.AssignedHero) continue;

                    var nextWalletWithOutHero = WalletManager.GetWallets().FirstOrDefault(x => x.AssignedHero == 0 && x.HasDkProfile);

                    if (nextWalletWithOutHero != null)
                    {
                        var heroSentToWalletResult =
                            await new HeroContractHandler().SendHeroToWallet(sourceWallet, nextWalletWithOutHero.WalletAccount,
                                heroToSend);
                        if (heroSentToWalletResult)
                        {
                            await eventHub.PublishAsync(new MessageEvent() { Content = $"[Wallet:{nextWalletWithOutHero.Address}] => Received => [Hero:{heroToSend}]" });

                            herosXfered++;
                        }
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