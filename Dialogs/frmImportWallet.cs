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
                    //Attempt to load wallet
                    var existingWallet = WalletManager.ImportWallet(txtPrivateKey.Text.Trim());
                    if (existingWallet != null)
                    {
                        if (WalletManager.GetWallet(existingWallet.Address) == null)
                        {
                            WalletManager.AddWallet(new DfkWallet
                            {
                                Name = $"${txtName.Text}",
                                Address = existingWallet.Address,
                                PrivateKey = existingWallet.PrivateKey,
                                PublicKey = existingWallet.PublicKey,
                                MnemonicPhrase = txtPrivateKey.Text
                            });


                            WalletManager.SaveWallets();

                            RadMessageBox.Show($@"Your wallet has been imported",
                                @"Wallet Imported");

                            Close();
                        }
                        else
                        {
                            RadMessageBox.Show(@"Please check private key!", @"Unable to import wallet!");
                        }
                    }
                    else
                    {
                        RadMessageBox.Show(@"Please check private key!", @"Unable to import wallet!");
                    }
                }
                else
                {
                    RadMessageBox.Show(@"You must provide a wallet name and private key to import!",
                        @"Missing Fields!");
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(@"Please check your private key!", @"Error during import!");
            }

            Enabled = true;
        }
    }
}
