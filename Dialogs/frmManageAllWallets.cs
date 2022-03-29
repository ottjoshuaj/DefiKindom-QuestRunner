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

        private delegate void ShowRadAlertMessageBoxDelegate(string text, string title);

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
            gridWallets.ContextMenuOpening += GridWalletsContextMenuOpening;

            //Right click context menu event hookup
            mnuGridActionSetAsPrimaryWallet.Click += MnuGridActionSetAsPrimaryWalletClick;
            mnuGridActionSendJewelTo.Click += MnuGridActionSendJewelToOnClick;
            mnuGridActionSendHeroToWallet.Click += MnuGridActionSendHeroToWalletOnClick;
            mnuGridActionRecallHeroToSource.Click += MnuGridActionRecallHeroToSourceOnClick;
            mnuGridActionRebuildHeroProfile.Click += MnuGridActionRebuildHeroProfileOnClick;

            mnuGridActionSendOneToWallet.Click += MnuGridActionSendOneToWalletOnClick;
            mnuGridActionOnBoardDfk.Click += MnuGridActionOnBoardDfk_Click;

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
                mnuGridActionSendJewelTo.Enabled = false;
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
            eventHub.Subscribe<WalletsGeneratedEvent>(Subscriber);
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

        void Subscriber(WalletsGeneratedEvent obj)
        {
            LoadDataToGrid();
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
            if (RadMessageBox.Show(this,
                    "Are you sure you want to delete all selected wallets?\r\nIt will stop any active QuestInstances for those wallets as well.",
                    "Delete Selected Wallets",
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

                //Wallet Instance
                var walletItem = selectedRow.DataBoundItem as DfkWallet;
                if (walletItem != null)
                {
                    mnuGridActionSendHeroToWallet.Enabled = walletItem.AssignedHero <= 0;

                    //if(walletItem.HasDkProfile && walletItem.CurrentBalance > 0)
                    if (walletItem.HasDkProfile)
                        mnuGridActionOnBoardDfk.Enabled = false;
                    else if (!walletItem.HasDkProfile && walletItem.CurrentBalance > 0)
                        mnuGridActionOnBoardDfk.Enabled = true;

                    mnuGridActionRecallHeroToSource.Enabled = !walletItem.IsPrimarySourceWallet;
                    mnuGridActionSendOneToWallet.Enabled = !walletItem.IsPrimarySourceWallet;
                }
            }
        }

        private void GridWalletsContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            e.ContextMenu = mnuGridAction.DropDown;
        }

        #endregion

        #region Menu Events

        private void mnuSendOneAndOnboardToDfk_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            new frmSendONEToWallets().ShowDialog(this);

            LoadDataToGrid();

            mnuMainMenu.Enabled = true;
        }

        private async void mnuOnboardToDfk_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            //Get wallets that need onboarded
            var onboardedAccounts = false;

            foreach (var wallet in WalletManager.GetWallets().Where(x => !x.HasDkProfile))
            {
                wallet.DfkProfile = await new ProfileContractHandler().GetProfile(wallet.WalletAccount);
                if (wallet.DfkProfile == null)
                {
                    var dfkOnboardResult = await
                        new ProfileContractHandler().CreateProfile(wallet,
                            wallet.Name);
                    if (dfkOnboardResult)
                    {
                        await eventHub.PublishAsync(new MessageEvent()
                        {
                            Content =
                                $"[Wallet:{wallet.Address}] has been onboarded to DFK!"
                        });

                        wallet.DfkProfile = await new ProfileContractHandler().GetProfile(wallet.WalletAccount);


                        onboardedAccounts = true;
                    }
                }
            }

            if (onboardedAccounts)
                WalletManager.SaveWallets();

            mnuMainMenu.Enabled = true;
        }

        private void mnuSendHeroesToWallets_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            new frmSendHeroesToWallets().ShowDialog(this);

            LoadDataToGrid();

            mnuMainMenu.Enabled = true;
        }

        private void mnuGenerateNewWallets_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            new frmCreateNewWallets().ShowDialog(this);

            LoadDataToGrid();

            mnuMainMenu.Enabled = true;
        }

        private void mnuReIInitSelectedWallets_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            //Get 


            mnuMainMenu.Enabled = true;
        }

        private void mnuRecallHerosToSourceWallet_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            //Get 


            mnuMainMenu.Enabled = true;
        }

        #endregion

        #region Grid Context Menu

        private void MnuGridActionRebuildHeroProfileOnClick(object sender, EventArgs e)
        {
            mnuGridActionRebuildHeroProfile.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var wallet = selRow.DataBoundItem as DfkWallet;
                if (wallet != null)
                {
                    WalletManager.ReloadWalletHeroData(wallet.Address);
                    WalletManager.SaveWallets();

                    LoadDataToGrid();

                    RadMessageBox.Show(this, $"{wallet.Name} Hero Data and Profiles Rebuilt...",
                        "Hero Data Rebuilt");
                }
            }

            mnuGridActionRebuildHeroProfile.Enabled = true;
        }

        private async void MnuGridActionRecallHeroToSourceOnClick(object sender, EventArgs e)
        {
            mnuGridActionRecallHeroToSource.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var wallet = selRow.DataBoundItem as DfkWallet;
                if (wallet != null)
                {
                    if (wallet.AssignedHero > 0)
                    {
                        //We're sending the current wallet hero BACK to the source wallet
                        var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                        if (sourceWallet != null)
                        {
                            var heroSentToWalletResult =
                                await new HeroContractHandler().SendHeroToWallet(wallet, sourceWallet.WalletAccount,
                                    wallet.AssignedHero);
                            if (heroSentToWalletResult)
                            {
                                await eventHub.PublishAsync(new MessageEvent()
                                {
                                    Content =
                                        $"[Wallet:{sourceWallet.Address}] => Received => [Hero:{wallet.AssignedHero}] (Recalled Action)"
                                });
                            }

                            WalletManager.ReloadWalletHeroData(sourceWallet.Address);

                            WalletManager.SaveWallets();

                            LoadDataToGrid();

                            ShowRadAlertMessageBox($"{wallet.Name}'s Hero has been recalled back to source wallet!",
                                "Hero Recalled");
                        }
                    }
                }
            }

            mnuGridActionRecallHeroToSource.Enabled = true;
        }

        private async void MnuGridActionSendHeroToWalletOnClick(object sender, EventArgs e)
        {
            mnuGridActionSendHeroToWallet.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var walletToReceiveHero = selRow.DataBoundItem as DfkWallet;
                if (walletToReceiveHero != null)
                {
                    if (walletToReceiveHero.AssignedHero > 0)
                    {
                        ShowRadAlertMessageBox(
                            "This wallet already has a hero assigned.\r\nOnly a single hero is supported currently!",
                            "Hero Exists!");
                        return;
                    }

                    var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                    if (sourceWallet != null)
                    {
                        var firstAvailableHero =
                            sourceWallet.AvailableHeroes.FirstOrDefault(x => x != sourceWallet.AssignedHero);

                        var heroSentToWalletResult =
                            await new HeroContractHandler().SendHeroToWallet(sourceWallet,
                                walletToReceiveHero.WalletAccount,
                                firstAvailableHero);
                        if (heroSentToWalletResult)
                        {
                            await eventHub.PublishAsync(new MessageEvent()
                            {
                                Content =
                                    $"[Wallet:{walletToReceiveHero.Address}] => Received => [Hero:{firstAvailableHero}]"
                            });
                        }

                        WalletManager.ReloadWalletHeroData(walletToReceiveHero.Address);

                        WalletManager.SaveWallets();

                        LoadDataToGrid();

                        ShowRadAlertMessageBox(
                            $"Hero {firstAvailableHero} transferred to wallet {walletToReceiveHero.Address}",
                            "Hero Sent To Wallet");
                    }
                }
            }

            mnuGridActionSendHeroToWallet.Enabled = true;
        }

        private async void MnuGridActionSendJewelToOnClick(object sender, EventArgs e)
        {
            if (RadMessageBox.Show(this, "Are you sure you want to send Jewel to this wallet?", "Send Jewel",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            mnuGridActionSendJewelTo.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var sendJewelTo = selRow.DataBoundItem as DfkWallet;
                if (sendJewelTo != null)
                {
                    //Find the jewel holder
                    var jewelHolder = await WalletManager.GetJewelHolder();
                    if (jewelHolder != null)
                    {
                        var sendJewelResponse =
                            new JewelContractHandler().SendJewelToAccount(jewelHolder.Holder, sendJewelTo);

                        if (sendJewelResponse.Result)
                        {
                            ShowRadAlertMessageBox($"Jewel has been transferred to wallet: {sendJewelTo.Name}!",
                                "Jewel Transferred!");
                        }
                        else
                        {
                            ShowRadAlertMessageBox(
                                $"An error occurred trying to send the jewel to: {sendJewelTo.Name}!",
                                "Jewel Transferred ERROR");
                        }

                    }


                    LoadDataToGrid();
                }
            }

            mnuGridActionSendJewelTo.Enabled = false;
        }

        private void MnuGridActionSetAsPrimaryWalletClick(object sender, EventArgs e)
        {
            mnuGridActionSetAsPrimaryWallet.Enabled = false;

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

            mnuGridActionSetAsPrimaryWallet.Enabled = false;
        }

        private void MnuGridActionSendOneToWalletOnClick(object sender, EventArgs e)
        {
            mnuGridActionSendOneToWallet.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                new frmSendONEToWallets(selRow.DataBoundItem as DfkWallet).ShowDialog(this);

                LoadDataToGrid();
            }

            mnuGridActionSendOneToWallet.Enabled = true;
        }

        private async void MnuGridActionOnBoardDfk_Click(object sender, EventArgs e)
        {
            mnuOnboardToDfk.Enabled = false;

            //Get wallets that need onboarded
            var onboardedAccounts = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var walletToOnBoard = selRow.DataBoundItem as DfkWallet;
                if (walletToOnBoard != null)
                {
                    var dfkOnboardResult = await
                        new ProfileContractHandler().CreateProfile(walletToOnBoard,
                            walletToOnBoard.Name);
                    if (dfkOnboardResult)
                    {
                        eventHub.Publish(new MessageEvent()
                        {
                            Content =
                                $"[Wallet:{walletToOnBoard.Address}] has been onboarded to DFK!"
                        });

                        var newDfkProfile =
                            await new ProfileContractHandler().GetProfile(walletToOnBoard.WalletAccount);

                        walletToOnBoard.DfkProfile = newDfkProfile;


                        onboardedAccounts = true;
                    }
                }

                if (onboardedAccounts)
                    WalletManager.SaveWallets();

                LoadDataToGrid();
            }
        }

        #endregion

        #region Data Handler Methods

        void LoadDataToGrid()
        {
            if (InvokeRequired)
                Invoke(new LoadDataToGridDelegate(LoadDataToGrid));
            else
            {
                var wallets = WalletManager.GetWallets();
                if (wallets != null)
                {
                    //Refresh grid
                    gridWallets.SuspendLayout();
                    gridWallets.DataSource = wallets;
                    gridWallets.Refresh();
                    gridWallets.ResumeLayout();


                    lblWalletStatusInfo.Text = $@"Managing ({wallets.Count}) Wallets";
                    lblWalletsThatCanQuest.Text = $@"({wallets.Count(x=>x.ReadyToWork)}) Wallets Are Quest Ready";
                }
            }
        }

        void LoadSelectedWalletToUx(DfkWallet rowDataItem)
        {
            if (rowDataItem == null) return;


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

        #region MessageBox Alert

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