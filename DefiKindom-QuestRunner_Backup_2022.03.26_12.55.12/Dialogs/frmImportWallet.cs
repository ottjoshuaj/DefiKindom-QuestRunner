using System;
using System.Windows.Forms;

using DefiKindom_QuestRunner.Helpers;
using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Objects;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmImportWallet : Form
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

            if (txtName.Text.Trim().Length > 0 &&
                txtMnemonicWords.Text.Trim().Length > 0)
            {
                //Attempt to load wallet
                var existingWallet = WalletHelper.LoadTrueWallet(txtMnemonicWords.Text.Trim());
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
                                MnemonicPhrase = txtMnemonicWords.Text
                            });
                        }

                        currentWalletCount++;
                    }

                    MessageBox.Show(@"Wallet successfully imported!", @"Wallet Imported");

                    Close();
                }
                else
                {
                    MessageBox.Show(@"Please check your mnemonic words and password!", @"Unable to import wallet!");
                }
            }
            else
            {
                MessageBox.Show(@"You must provide a wallet name, password, and mnemonic words to import!", @"Missing Fields!");
            }

            Enabled = true;
        }
    }
}
