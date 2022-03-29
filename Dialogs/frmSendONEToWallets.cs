using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using PubSub;
using Telerik.WinControls;
using Telerik.WinControls.UI;

using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmSendONEToWallets : RadForm
    {
        #region Delegates

        private delegate void ShowRadAlertMessageBoxDelegate(string text, string title);

        #endregion

        #region Internals

        private bool isBusy;
        Hub eventHub = Hub.Default;
        DfkWallet _wallet = null;

        #endregion

        #region Constructor(s)

        public frmSendONEToWallets()
        {
            InitializeComponent();

            FormClosing += OnFormClosing;
        }

        public frmSendONEToWallets(DfkWallet wallet) : this()
        {
            _wallet = wallet;
        }

        #endregion

        #region Form Events

        private void frmSendONEToWallets_Load(object sender, EventArgs e)
        {
            if (_wallet != null)
            {
                lblSendOneHeader.Text = $@"How much ONE to you want to send from your SOURCE wallet to wallet: {_wallet.Name}?";
                chkSendToNewWalletsOnly.Visible = false;
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = isBusy;
        }

        #endregion

        #region Button Events

        private async void btnSendOne_Click(object sender, EventArgs e)
        {
            if (txtOneAmountToSend.Value == 0)
            {
                RadMessageBox.Show(this, "Amount must be greater than 0!", "Invalid ONE Amount");
                return;
            }

            btnSendOne.Enabled = false;

            isBusy = true;

            //Update TExt
            btnSendOne.Text = @"Sending ONE...Please wait...";

            await Task.Run(SendOneToWallets);

            isBusy = false;

            btnSendOne.Text = @"Send ONE";
            btnSendOne.Enabled = true;
        }

        #endregion

        #region Send One To Wallets

        async Task<bool> SendOneToWallets()
        {
            try
            {
                if (_wallet != null)
                {
                    var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                    if (sourceWallet != null)
                    {
                        //Does source wallet have enough to source the wallets?
                        var totalOneToBeConsumed = txtOneAmountToSend.Value * 1;
                        if (totalOneToBeConsumed > sourceWallet.CurrentBalance)
                        {
                            ShowRadAlertMessageBox($"You do not have enough ONE on your source wallet!\r\nYou Need a total of: {totalOneToBeConsumed} ONE", "Insufficient ONE Balance on Source account!");
                            return false;
                        }

                        var fundRequestResponse = await new OneContractHandler().SendHarmonyONE(sourceWallet,
                            _wallet.Address, Convert.ToInt32(txtOneAmountToSend.Value));
                        if (fundRequestResponse.Success)
                        {
                            //Update Wallet Balance
                            _wallet.CurrentBalance = fundRequestResponse.Balance;

                            await eventHub.PublishAsync(new MessageEvent()
                            {
                                Content =
                                    $"[Wallet:{_wallet.Address}] has been funded with a total of {fundRequestResponse.Balance} ONE."
                            });
                        }
                        else
                        {
                            await eventHub.PublishAsync(new MessageEvent() { Content = $"[Wallet:{_wallet.Address}] failed to send ONE to wallet." });
                        }


                        WalletManager.SaveWallets();

                        ShowRadAlertMessageBox($@"{_wallet.Name} funded. Each with {txtOneAmountToSend.Value} ONE", @"Wallets Funded!");
                    }
                    else
                    {
                        ShowRadAlertMessageBox($@"You must have a SOURCE wallet with ONE in it before trying to fund other wallets/accounts!!!", @"No Source Wallet Found!");
                    }
                }
                else
                {
                    var walletsToBeFunded = chkSendToNewWalletsOnly.Checked
                        ? WalletManager.GetWallets().Where(x => x.CurrentBalance < 5).ToList()
                        : WalletManager.GetWallets().ToList();

                    if (walletsToBeFunded.Count == 0)
                    {
                        ShowRadAlertMessageBox($@"All accounts have the minimum of ONE to run", @"No Source Wallet Found!");
                    }

                    var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                    if (sourceWallet != null)
                    {
                        //Does source wallet have enough to source the wallets?
                        var totalOneToBeConsumed = txtOneAmountToSend.Value * walletsToBeFunded.Count;
                        if (totalOneToBeConsumed > sourceWallet.CurrentBalance)
                        {
                            ShowRadAlertMessageBox($"Your trying to fund {walletsToBeFunded.Count} Wallets but do not have enough ONE on your source wallet!\r\nYou Need a total of: {totalOneToBeConsumed} ONE", "Insufficient ONE Balance on Source account!");
                            return false;
                        }

                        foreach (var walletToFundWallet in walletsToBeFunded)
                        {
                            var fundRequestResponse = await new OneContractHandler().SendHarmonyONE(sourceWallet,
                                walletToFundWallet.Address, Convert.ToInt32(txtOneAmountToSend.Value));
                            if (fundRequestResponse != null && fundRequestResponse.Success)
                            {
                                await eventHub.PublishAsync(new MessageEvent()
                                {
                                    Content =
                                        $"[Wallet:{walletToFundWallet.Address}] has been funded with a total of {fundRequestResponse.Balance} ONE."
                                });
                            }
                            else
                            {
                                await eventHub.PublishAsync(new MessageEvent() { Content = $"[Wallet:{walletToFundWallet.Address}] failed to send ONE to wallet." });
                            }
                        }


                        WalletManager.SaveWallets();

                        ShowRadAlertMessageBox($@"{walletsToBeFunded.Count} funded. Each with {txtOneAmountToSend.Value} ONE", @"Wallets Funded!");
                    }
                    else
                    {
                        ShowRadAlertMessageBox($@"You must have a SOURCE wallet with ONE in it before trying to fund other wallets/accounts!!!", @"No Source Wallet Found!");
                    }
                }


                Close();
            }
            catch (Exception ex)
            {
                await eventHub.PublishAsync(new NotificationEvent { IsError = true, Content = ex.Message });
            }

            return true;
        }

        #endregion

        #region MessageBox Handler

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
