using System;
using System.Windows.Forms;

using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Objects;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmImportWallet : RadForm
    {
        private frmConsole parent;

        public frmImportWallet()
        {
            InitializeComponent();

            parent = Owner as frmConsole;
        }

        private void frmImportWallet_Load(object sender, EventArgs e)
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            Enabled = false;

            try
            {
                if (txtName.Text.Trim().Length > 0 &&
                    txtPrivateKey.Text.Trim().Length > 0)
                {
                    var walletsImported = 0;

                    //Attempt to load wallet
                    var existingWallet = WalletManager.ImportWallet(txtPrivateKey.Text.Trim());
                    if (existingWallet != null)
                    {
                        //Lets get count of wallet to know our new name
                        var currentWalletCount = WalletManager.GetWallets().Count;
                        currentWalletCount++;

                        //Lets add all wallet accounts UNDER the main wallet here
                        foreach (var subAccountAddress in existingWallet.GetAddresses())
                        {
                            var accountDetails = existingWallet.GetAccount(subAccountAddress);
                            if (accountDetails != null)
                            {
                                WalletManager.AddWallet(new DfkWallet
                                {
                                    Name = $"${txtName.Text} - ${currentWalletCount}",
                                    Address = accountDetails.Address,
                                    PrivateKey = accountDetails.PrivateKey,
                                    PublicKey = accountDetails.PublicKey,
                                    MnemonicPhrase = txtPrivateKey.Text
                                });
                            }

                            currentWalletCount++;
                            walletsImported++;
                        }

                        RadMessageBox.Show($@"{walletsImported} Wallet(s)/Account(s) successfully imported!",
                            @"Wallet Imported");

                        Close();
                    }
                    else
                    {
                        RadMessageBox.Show(@"Please check your mnemonic words!", @"Unable to import wallet!");
                    }
                }
                else
                {
                    RadMessageBox.Show(@"You must provide a wallet name and mnemonic words to import!",
                        @"Missing Fields!");
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(@"Please check your mnemonic words!", @"Error during import!");
            }

            Enabled = true;
        }
    }
}
