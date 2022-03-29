using System;
using System.Linq;

using PubSub;
using Telerik.WinControls.UI;

using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Managers.Contracts;
using Telerik.WinControls;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmSendHeroesToWallets : RadForm
    {
        static Hub eventHub = Hub.Default;

        public frmSendHeroesToWallets()
        {
            InitializeComponent();
        }

        private void frmSendHeroesToWallets_Load(object sender, EventArgs e)
        {

        }

        private async void btnSendHeroesToWallets_Click(object sender, EventArgs e)
        {
            var herosXfered = 0;

            btnSendHeroesToWallets.Enabled = true;
            btnSendHeroesToWallets.Text = @"Sending Heroes...";

            var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
            if (sourceWallet != null)
            {
                var availableHeroesToSendToWallets =
                    sourceWallet.AvailableHeroes.Where(x => x != sourceWallet.AssignedHero);

                foreach (var heroToSend in availableHeroesToSendToWallets)
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
                            WalletManager.UpdateWalletAssignedHero(nextWalletWithOutHero.WalletAccount.Address,
                                heroToSend);

                            eventHub.Publish(new MessageEvent() { Content = $"[Wallet:{nextWalletWithOutHero.Address}] => Received => [Hero:{heroToSend}]" });

                            herosXfered++;
                        }
                    }
                }

                WalletManager.ReloadWalletHeroes();

                WalletManager.SaveWallets();

                RadMessageBox.Show(this, $"{herosXfered} heroes transferred to wallets...", "Heroes Sent To Wallets");
            }
            else
            {
                
            }

            btnSendHeroesToWallets.Text = @"Send Heroes";
            btnSendHeroesToWallets.Enabled = false;
        }
    }
}
