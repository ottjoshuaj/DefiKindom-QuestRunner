using System;

using Telerik.WinControls;
using Telerik.WinControls.UI;

using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Managers.Contracts;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmSendNormalJewel : RadForm
    {
        public frmSendNormalJewel()
        {
            InitializeComponent();
        }

        private void frmSendJewel_Load(object sender, EventArgs e)
        {

        }

        private async void btnTransferJewel_Click(object sender, EventArgs e)
        {
            btnTransferJewel.Enabled = false;

            if (txtJewelAmount.Value > 0 &&
                !string.IsNullOrWhiteSpace(txtDestinationAddress.Text))
            {
                var sourceWallet = await WalletManager.GetJewelHolder();
                var result = await new JewelContractHandler().JewelXBalance(sourceWallet.Holder, txtDestinationAddress.Text.Trim(),
                    txtJewelAmount.Value);
                if (result)
                {
                    RadMessageBox.Show(this, "Your jewel was successfully moved to the destination wallet!",
                        "Jewel Transferred!");

                    Close();
                }
                else
                {
                    RadMessageBox.Show(this, "An error occurred during jewel transfer.  Wait a few seconds and try again...(busy Blockchain/RPC!)",
                        "Error Transferring!");
                }
            }
            else
            {
                RadMessageBox.Show(this, "You must provide an amount and destination address transfer jewel!",
                    "Missing keys");
            }

            btnTransferJewel.Enabled = true;
        }
    }
}
