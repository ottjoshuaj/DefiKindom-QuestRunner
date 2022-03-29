using System;
using System.Linq;
using System.Windows.Forms;
using PubSub;
using Telerik.WinControls.UI;

using DefiKindom_QuestRunner.EngineManagers;
using DefiKindom_QuestRunner.EngineManagers.Engines;
using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using Telerik.WinControls;


namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmManageAllWallets : RadForm
    {
        #region Internals

        static Hub eventHub = Hub.Default;

        #endregion

        #region Delegates

        private delegate void LoadDataToGridDelegate();

        #endregion

        #region Constructor(s)

        public frmManageAllWallets()
        {
            InitializeComponent();

            gridWallets.CurrentRowChanged += GridWallets_CurrentRowChanged;
            gridWallets.UserDeletedRow += GridWalletsOnUserDeletedRow;
            gridWallets.UserDeletingRow += GridWalletsOnUserDeletingRow;
            gridWallets.SelectionChanged += GridWalletsOnSelectionChanged;

            SubScribeToEvents();
        }

        #endregion

        #region Form Events

        private void frmManageAllWallets_Load(object sender, EventArgs e)
        {
            LoadDataToGrid();

            //Is quest instance running?
            if (QuestEngineManager.GetAllQuestInstances().Count > 0)
            {
                mnuSendJewelToSelectedWallet.Enabled = false;
            }
        }

        #endregion

        #region Subscribe Events

        void SubScribeToEvents()
        {
            //WalletJewelMovedEvent == jewel has changed ownereship, update UX
            eventHub.Subscribe<WalletJewelMovedEvent>(UpdateGridWithJewelLocationInfo);
            eventHub.Subscribe<ManageWalletGridEvent>(UpdateWalletGridEvent);
            eventHub.Subscribe<WalletsOnQuestsMessageEvent>(UpdateWalletOnQuestEvent);

        }

        void UpdateGridWithJewelLocationInfo(WalletJewelMovedEvent evt)
        {
            LoadDataToGrid();
        }

        void UpdateWalletGridEvent(ManageWalletGridEvent evt)
        {
            LoadDataToGrid();
        }

        void UpdateWalletOnQuestEvent(WalletsOnQuestsMessageEvent msgEvent)
        {
            switch (msgEvent.OnQuestMessageEventType)
            {
                case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing:
                    break;

                case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingCanceled:
                    break;
            }
        }

        #endregion

        #region Grid Events

        private void GridWallets_CurrentRowChanged(object sender, CurrentRowChangedEventArgs e)
        {
            LoadSelectedWalletToUx(e.CurrentRow.DataBoundItem as DfkWallet);

        }

        private void GridWalletsOnUserDeletedRow(object sender, GridViewRowEventArgs e)
        {

        }

        private void GridWalletsOnUserDeletingRow(object sender, GridViewRowCancelEventArgs e)
        {
            if (RadMessageBox.Show(this, "Are you sure you want to delete all selected wallets?\r\nIt will stop any active QuestInstances for those wallets as well.", "Delete Selected Wallets",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
            else
            {
                foreach (var rowToDelete in e.Rows)
                {
                    var dfkWallet = rowToDelete.DataBoundItem as DfkWallet;
                    if (dfkWallet != null)
                    {
                        //TODO: in future remove all quest types!
                        QuestEngineManager.RemoveQuestEngine(dfkWallet, QuestEngine.QuestTypes.Mining);
                    }

                    //Now Remove from Wallet Manager
                    WalletManager.RemoveWallet(dfkWallet);
                }

                //Save wallet updates
                WalletManager.SaveWallets();
            }
        }

        private void GridWalletsOnSelectionChanged(object sender, EventArgs e)
        {
            var selectedRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selectedRow != null)
            {
                LoadSelectedWalletToUx(selectedRow.DataBoundItem as DfkWallet);
            }
        }

        #endregion

        #region Menu Events

        private void mnuSelectWalletAsPrimary_Click(object sender, EventArgs e)
        {
            mnuSelectWalletAsPrimary.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var rowDataItem = selRow.DataBoundItem as DfkWallet;
                if (rowDataItem != null)
                {
                    WalletManager.SetAsSourceWallet(rowDataItem);

                    gridWallets.Refresh();
                }
            }

            mnuSelectWalletAsPrimary.Enabled = false;
        }

        private void mnuSendOneAndOnboardToDfk_Click(object sender, EventArgs e)
        {
            new frmSendONEToWallets().ShowDialog(this);

            LoadDataToGrid();
        }

        private async void mnuOnboardToDfk_Click(object sender, EventArgs e)
        {
            mnuOnboardToDfk.Enabled = false;

            //Get wallets that need onboarded
            var onboardedAccounts = false;

            foreach (var wallet in WalletManager.GetWallets().Where(x => !x.HasDkProfile))
            {
                //var onboardResult = await new ProfileContractHandler().CreateProfile()
                var dfkOnboardResult = await
                    new ProfileContractHandler().CreateProfile(wallet.WalletAccount,
                        wallet.Name);
                if (dfkOnboardResult)
                {
                    eventHub.Publish(new MessageEvent()
                    {
                        Content =
                            $"[Wallet:{wallet.Address}] has been onboarded to DFK!"
                    });

                    var newDfkProfile =
                        await new ProfileContractHandler().GetProfile(wallet.WalletAccount);

                    wallet.DfkProfile = newDfkProfile;


                    onboardedAccounts = true;
                }

            }

            if (onboardedAccounts)
                WalletManager.SaveWallets();

            mnuOnboardToDfk.Enabled = true;
        }

        private void mnuSendHeroesToWallets_Click(object sender, EventArgs e)
        {
            new frmSendHeroesToWallets().ShowDialog(this);

            LoadDataToGrid();
        }

        private void mnuSendJewelToSelectedWallet_Click(object sender, EventArgs e)
        {
            if (RadMessageBox.Show(this, "Are you sure you want to send Jewel to this wallet?", "Send Jewel",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            mnuSendJewelToSelectedWallet.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var rowDataItem = selRow.DataBoundItem as DfkWallet;
                if (rowDataItem != null)
                {
                    WalletManager.SetAsSourceWallet(rowDataItem);

                    LoadDataToGrid();
                }
            }

            mnuSendJewelToSelectedWallet.Enabled = false;
        }

        private void mnuGenerateNewWallets_Click(object sender, EventArgs e)
        {
            new frmCreateNewWallets().ShowDialog(this);

            LoadDataToGrid();
        }

        private void mnuReIInitSelectedWallets_Click(object sender, EventArgs e)
        {
            mnuReIInitSelectedWallets.Enabled = false;

            //Get 


            mnuReIInitSelectedWallets.Enabled = true;
        }

        private void mnuRecallHerosToSourceWallet_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Data Handler Methods

        void LoadDataToGrid()
        {
            if (InvokeRequired)
                Invoke(new LoadDataToGridDelegate(LoadDataToGrid));
            else
            {
                var currentWallets = WalletManager.GetWallets();

                //Refresh grid
                gridWallets.SuspendLayout();
                gridWallets.DataSource = currentWallets;
                gridWallets.Refresh();
                gridWallets.ResumeLayout();

                lblWalletStatusInfo.Text = $@"Managing ({currentWallets.Count}) Wallets";
            }
        }

        void LoadSelectedWalletToUx(DfkWallet rowDataItem)
        {
            if(rowDataItem == null) return;

            /*
                lblIsQuesting.Text = "Yes";
                lblQuestStartedAt.Text = rowDataItem.AssignedHeroQuestStatus.QuestStartedAt.ToString();
                lblQuestCompletesAt.Text = rowDataItem.AssignedHeroQuestStatus.QuestCompletesAt.ToString();
                lblQuestAddress.Text = rowDataItem.AssignedHeroQuestStatus.ContractAddress;
                lblQuestStartBlock.Text = rowDataItem.AssignedHeroQuestStatus.StartBlock.ToString();
                lblQuestHeroIds.Text = string.Join(",", rowDataItem.AssignedHeroQuestStatus.HeroesOnQuest.ToArray());
                lblQuestStatus.Text = rowDataItem.AssignedHeroQuestStatus.Status.ToString();
                lblQuestAttempts.Text = rowDataItem.AssignedHeroQuestStatus.Attempts.ToString();

                groupQuestOptions.Enabled = true;

                if (rowDataItem.AssignedHeroQuestStatus.IsQuesting)
                {
                    ddQuestTypes.Enabled = false;
                    btnStartQuest.Enabled = false;
                    btnStopQuest.Enabled = true;
                }
                else
                {
                    ddQuestTypes.Enabled = true;
                    btnStartQuest.Enabled = true;
                    btnStopQuest.Enabled = false;
                }
             */
        }


        #endregion
    }
}
