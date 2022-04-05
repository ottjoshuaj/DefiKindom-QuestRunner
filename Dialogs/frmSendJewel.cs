using System;

using Telerik.WinControls;
using Telerik.WinControls.UI;

using Nethereum.Signer;
using Nethereum.Web3.Accounts;

using DefiKindom_QuestRunner.Managers.Contracts;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmSendJewel : RadForm
    {
        public frmSendJewel()
        {
            InitializeComponent();
        }

        private void frmSendJewel_Load(object sender, EventArgs e)
        {

        }

        private async void btnTransferJewel_Click(object sender, EventArgs e)
        {
            btnTransferJewel.Enabled = false;

            if (!string.IsNullOrWhiteSpace(txtSourcePrivateKey.Text) &&
                !string.IsNullOrWhiteSpace(txtDestinationPrivateKey.Text))
            {
                var sourceWallet = new Account(txtSourcePrivateKey.Text.Trim(), Chain.MainNet);
                var destinationWallet = new Account(txtDestinationPrivateKey.Text.Trim(), Chain.MainNet);

                var result = await new JewelContractHandler().TransferLockeDJewel(sourceWallet, destinationWallet);
                if (result)
                {
                    RadMessageBox.Show(this, "Your locked jewel was successfully moved to the destinatio nwallet!",
                        "Locked Jewel Transferred!");

                    Close();
                }
                else
                {
                    RadMessageBox.Show(this, "An error occurred during locked jewel transfer.  Wait a few seconds and try again...(busy Blockchain/RPC!)",
                        "Error Transferring!");
                }
            }
            else
            {
                RadMessageBox.Show(this, "You must provide a source and destination private key to xfer locked jewel!",
                    "Missing keys");
            }

            btnTransferJewel.Enabled = true;
        }
    }
}
