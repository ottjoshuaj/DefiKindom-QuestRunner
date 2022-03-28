using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using PubSub;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmSendONEToWallets : RadForm
    {
        static Hub eventHub = Hub.Default;

        public frmSendONEToWallets()
        {
            InitializeComponent();
        }

        private void frmSendONEToWallets_Load(object sender, EventArgs e)
        {

        }

        private async void btnSendOne_Click(object sender, EventArgs e)
        {
            if (txtOneAmountToSend.Value == 0)
            {
                RadMessageBox.Show(this, "Amount must be greater than 0!", "Invalid ONE Amount");
                return;
            }

            Enabled = false;

            //Update TExt
            btnSendOne.Text = @"Sending ONE...Please wait...";


            try
            {
                var walletsToBeFunded = chkSendToNewWalletsOnly.Checked
                    ? WalletManager.GetWallets().Where(x => x.CurrentBalance < 5).ToList()
                    : WalletManager.GetWallets();

                var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                if (sourceWallet != null)
                {
                    //Does source wallet have enough to source the wallets?
                    var totalOneToBeConsumed = txtOneAmountToSend.Value * walletsToBeFunded.Count;
                    if (totalOneToBeConsumed > sourceWallet.CurrentBalance)
                    {
                        RadMessageBox.Show(this, $"Your trying to fund {walletsToBeFunded.Count} Wallets but do not have enough ONE on your source wallet!\r\nYou Need a total of: {totalOneToBeConsumed} ONE", "Insufficient ONE Balance on Source account!");
                        return;
                    }

                    foreach (var walletToFundWallet in walletsToBeFunded)
                    {
                        var fundRequestResponse = await new OneContractHandler().SendHarmonyONE(sourceWallet.WalletAccount,
                            walletToFundWallet.Address, txtOneAmountToSend.Value);
                        if (fundRequestResponse.Success)
                        {
                            //Update wallet with new balace
                            var updatedWalletBalance = await new OneContractHandler().CheckHarmonyONEBalance(walletToFundWallet.WalletAccount);
                            walletToFundWallet.CurrentBalance = updatedWalletBalance;

                            eventHub.Publish(new MessageEvent()
                            {
                                Content =
                                    $"[Transaction Hash:{fundRequestResponse.TransactionHash}] => [Wallet:{walletToFundWallet.Address}] has been funded with a total of {txtOneAmountToSend.Value} ONE."
                            });


                            //onboard wallet now to DFK
                            var dfkOnboardResult = await
                                new ProfileContractHandler().CreateProfile(walletToFundWallet.WalletAccount,
                                    walletToFundWallet.Name);
                            if (dfkOnboardResult)
                            {
                                eventHub.Publish(new MessageEvent()
                                {
                                    Content =
                                        $"[Wallet:{walletToFundWallet.Address}] has been onboarded to DFK!"
                                });

                                var newDfkProfile =
                                    await new ProfileContractHandler().GetProfile(walletToFundWallet.WalletAccount);

                                walletToFundWallet.DfkProfile = newDfkProfile;
                            }
                        }
                        else
                        {
                            eventHub.Publish(new MessageEvent() { Content = $"[Wallet:{walletToFundWallet.Address}] failed to send ONE to wallet." });
                        }
                    }


                    WalletManager.SaveWallets();

                    RadMessageBox.Show(this, $@"{walletsToBeFunded.Count} funded. Each with {txtOneAmountToSend.Value} ONE", @"Wallets Funded!");
                }
                else
                {
                    RadMessageBox.Show(this, $@"You must have a SOURCE wallet with ONE in it before trying to fund other wallets/accounts!!!", @"No Source Wallet Found!");
                }

                Close();
            }
            catch(Exception ex)
            {
                eventHub.Publish(new NotificationEvent { IsError = true, Content = ex.Message });
            }

            btnSendOne.Text = "Send ONE";

            Enabled = true;
        }
    }
}
