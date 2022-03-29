﻿namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmManageAllWallets
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn1 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn2 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn3 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewCheckBoxColumn gridViewCheckBoxColumn4 = new Telerik.WinControls.UI.GridViewCheckBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.TableViewDefinition tableViewDefinition1 = new Telerik.WinControls.UI.TableViewDefinition();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmManageAllWallets));
            this.materialBlueGreyTheme1 = new Telerik.WinControls.Themes.MaterialBlueGreyTheme();
            this.materialPinkTheme1 = new Telerik.WinControls.Themes.MaterialPinkTheme();
            this.materialTealTheme1 = new Telerik.WinControls.Themes.MaterialTealTheme();
            this.materialTheme1 = new Telerik.WinControls.Themes.MaterialTheme();
            this.gridWallets = new Telerik.WinControls.UI.RadGridView();
            this.radStatusStrip1 = new Telerik.WinControls.UI.RadStatusStrip();
            this.lblWalletStatusInfo = new Telerik.WinControls.UI.RadLabelElement();
            this.radMenuItem1 = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuSendOneAndOnboardToDfk = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuOnboardToDfk = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuSendHeroesToWallets = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuRecallHerosToSourceWallet = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem1 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.mnuGenerateNewWallets = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem3 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.mnuReIInitSelectedWallets = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuGridAction = new Telerik.WinControls.UI.RadContextMenu(this.components);
            this.mnuGridActionSetAsPrimaryWallet = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem4 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.mnuGridActionHeroOptions = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuGridActionSendHeroToWallet = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem6 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.mnuGridActionRecallHeroToSource = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem7 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.mnuGridActionRebuildHeroProfile = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem5 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.mnuGridActionSendOneToWallet = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuGridActionOnBoardDfk = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSeparatorItem8 = new Telerik.WinControls.UI.RadMenuSeparatorItem();
            this.mnuGridActionSendJewelTo = new Telerik.WinControls.UI.RadMenuItem();
            this.mnuMainMenu = new Telerik.WinControls.UI.RadMenu();
            this.commandBarSeparator1 = new Telerik.WinControls.UI.CommandBarSeparator();
            this.lblWalletsThatCanQuest = new Telerik.WinControls.UI.RadLabelElement();
            ((System.ComponentModel.ISupportInitialize)(this.gridWallets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridWallets.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radStatusStrip1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnuMainMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // gridWallets
            // 
            this.gridWallets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridWallets.Location = new System.Drawing.Point(0, 37);
            // 
            // 
            // 
            this.gridWallets.MasterTemplate.AllowAddNewRow = false;
            this.gridWallets.MasterTemplate.AllowColumnChooser = false;
            this.gridWallets.MasterTemplate.AllowColumnReorder = false;
            this.gridWallets.MasterTemplate.AllowColumnResize = false;
            this.gridWallets.MasterTemplate.AllowDragToGroup = false;
            this.gridWallets.MasterTemplate.AllowRowResize = false;
            this.gridWallets.MasterTemplate.AllowSearchRow = true;
            this.gridWallets.MasterTemplate.AutoGenerateColumns = false;
            this.gridWallets.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewCheckBoxColumn1.FieldName = "ReadyToWork";
            gridViewCheckBoxColumn1.HeaderText = "Ready To Work";
            gridViewCheckBoxColumn1.MaxWidth = 100;
            gridViewCheckBoxColumn1.MinWidth = 100;
            gridViewCheckBoxColumn1.Name = "colWalletReadyToWork";
            gridViewCheckBoxColumn1.ReadOnly = true;
            gridViewCheckBoxColumn1.Width = 100;
            gridViewCheckBoxColumn2.FieldName = "IsPrimarySourceWallet";
            gridViewCheckBoxColumn2.HeaderText = "Is Primary/Source";
            gridViewCheckBoxColumn2.MaxWidth = 130;
            gridViewCheckBoxColumn2.MinWidth = 130;
            gridViewCheckBoxColumn2.Name = "colIsPrimarySourceWallet";
            gridViewCheckBoxColumn2.ReadOnly = true;
            gridViewCheckBoxColumn2.Width = 130;
            gridViewTextBoxColumn1.FieldName = "Name";
            gridViewTextBoxColumn1.HeaderText = "Name";
            gridViewTextBoxColumn1.MaxWidth = 150;
            gridViewTextBoxColumn1.MinWidth = 150;
            gridViewTextBoxColumn1.Name = "colName";
            gridViewTextBoxColumn1.ReadOnly = true;
            gridViewTextBoxColumn1.Width = 150;
            gridViewTextBoxColumn2.FieldName = "Address";
            gridViewTextBoxColumn2.HeaderText = "Address";
            gridViewTextBoxColumn2.MaxWidth = 320;
            gridViewTextBoxColumn2.MinWidth = 320;
            gridViewTextBoxColumn2.Name = "colAddress";
            gridViewTextBoxColumn2.ReadOnly = true;
            gridViewTextBoxColumn2.Width = 320;
            gridViewCheckBoxColumn3.FieldName = "HasDkProfile";
            gridViewCheckBoxColumn3.HeaderText = "Has DK Profile";
            gridViewCheckBoxColumn3.MaxWidth = 100;
            gridViewCheckBoxColumn3.MinWidth = 100;
            gridViewCheckBoxColumn3.Name = "colHasDKProfile";
            gridViewCheckBoxColumn3.ReadOnly = true;
            gridViewCheckBoxColumn3.Width = 100;
            gridViewTextBoxColumn3.FieldName = "AssignedHero";
            gridViewTextBoxColumn3.HeaderText = "Assigned Hero";
            gridViewTextBoxColumn3.MaxWidth = 150;
            gridViewTextBoxColumn3.MinWidth = 150;
            gridViewTextBoxColumn3.Name = "colAssignedHero";
            gridViewTextBoxColumn3.ReadOnly = true;
            gridViewTextBoxColumn3.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn3.Width = 150;
            gridViewTextBoxColumn4.FieldName = "AssignedHeroStamina";
            gridViewTextBoxColumn4.HeaderText = "Hero Stamina";
            gridViewTextBoxColumn4.MaxWidth = 150;
            gridViewTextBoxColumn4.MinWidth = 150;
            gridViewTextBoxColumn4.Name = "colHeroStamina";
            gridViewTextBoxColumn4.ReadOnly = true;
            gridViewTextBoxColumn4.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn4.Width = 150;
            gridViewCheckBoxColumn4.FieldName = "AssignedHeroQuestStatus.IsQuesting";
            gridViewCheckBoxColumn4.HeaderText = "Hero Is Questing";
            gridViewCheckBoxColumn4.MaxWidth = 150;
            gridViewCheckBoxColumn4.MinWidth = 150;
            gridViewCheckBoxColumn4.Name = "colHeroIsQuesting";
            gridViewCheckBoxColumn4.ReadOnly = true;
            gridViewCheckBoxColumn4.Width = 150;
            gridViewTextBoxColumn5.FieldName = "CurrentBalance";
            gridViewTextBoxColumn5.HeaderText = "ONE Balance";
            gridViewTextBoxColumn5.MaxWidth = 200;
            gridViewTextBoxColumn5.MinWidth = 200;
            gridViewTextBoxColumn5.Name = "colONEBalance";
            gridViewTextBoxColumn5.ReadOnly = true;
            gridViewTextBoxColumn5.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn5.Width = 200;
            gridViewTextBoxColumn6.FieldName = "JewelBalance";
            gridViewTextBoxColumn6.HeaderText = "JEWEL Balance";
            gridViewTextBoxColumn6.MaxWidth = 200;
            gridViewTextBoxColumn6.MinWidth = 200;
            gridViewTextBoxColumn6.Name = "colJewelBalance";
            gridViewTextBoxColumn6.ReadOnly = true;
            gridViewTextBoxColumn6.Width = 200;
            gridViewTextBoxColumn7.FieldName = "QuestStartedAt";
            gridViewTextBoxColumn7.HeaderText = "Quest Started";
            gridViewTextBoxColumn7.MinWidth = 200;
            gridViewTextBoxColumn7.Name = "colQuestStartedAt";
            gridViewTextBoxColumn7.ReadOnly = true;
            gridViewTextBoxColumn7.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn7.Width = 200;
            gridViewTextBoxColumn8.FieldName = "QuestCompletesAt";
            gridViewTextBoxColumn8.HeaderText = "Quest Completes At";
            gridViewTextBoxColumn8.MinWidth = 200;
            gridViewTextBoxColumn8.Name = "colQuestCompletesAt";
            gridViewTextBoxColumn8.ReadOnly = true;
            gridViewTextBoxColumn8.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            gridViewTextBoxColumn8.Width = 200;
            this.gridWallets.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewCheckBoxColumn1,
            gridViewCheckBoxColumn2,
            gridViewTextBoxColumn1,
            gridViewTextBoxColumn2,
            gridViewCheckBoxColumn3,
            gridViewTextBoxColumn3,
            gridViewTextBoxColumn4,
            gridViewCheckBoxColumn4,
            gridViewTextBoxColumn5,
            gridViewTextBoxColumn6,
            gridViewTextBoxColumn7,
            gridViewTextBoxColumn8});
            this.gridWallets.MasterTemplate.EnableSorting = false;
            this.gridWallets.MasterTemplate.HorizontalScrollState = Telerik.WinControls.UI.ScrollState.AlwaysShow;
            this.gridWallets.MasterTemplate.MultiSelect = true;
            this.gridWallets.MasterTemplate.ViewDefinition = tableViewDefinition1;
            this.gridWallets.Name = "gridWallets";
            this.gridWallets.ReadOnly = true;
            this.gridWallets.ShowCellErrors = false;
            this.gridWallets.ShowGroupPanel = false;
            this.gridWallets.Size = new System.Drawing.Size(894, 525);
            this.gridWallets.TabIndex = 0;
            this.gridWallets.ThemeName = "Material";
            // 
            // radStatusStrip1
            // 
            this.radStatusStrip1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.lblWalletStatusInfo,
            this.commandBarSeparator1,
            this.lblWalletsThatCanQuest});
            this.radStatusStrip1.Location = new System.Drawing.Point(0, 562);
            this.radStatusStrip1.Name = "radStatusStrip1";
            this.radStatusStrip1.Size = new System.Drawing.Size(894, 24);
            this.radStatusStrip1.TabIndex = 2;
            // 
            // lblWalletStatusInfo
            // 
            this.lblWalletStatusInfo.Name = "lblWalletStatusInfo";
            this.radStatusStrip1.SetSpring(this.lblWalletStatusInfo, false);
            this.lblWalletStatusInfo.Text = "Managing (0) Wallets";
            this.lblWalletStatusInfo.TextWrap = true;
            // 
            // radMenuItem1
            // 
            this.radMenuItem1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.mnuSendOneAndOnboardToDfk,
            this.mnuOnboardToDfk,
            this.mnuSendHeroesToWallets,
            this.mnuRecallHerosToSourceWallet,
            this.radMenuSeparatorItem1,
            this.mnuGenerateNewWallets,
            this.radMenuSeparatorItem3,
            this.mnuReIInitSelectedWallets});
            this.radMenuItem1.Name = "radMenuItem1";
            this.radMenuItem1.Text = "Wallet Options";
            // 
            // mnuSendOneAndOnboardToDfk
            // 
            this.mnuSendOneAndOnboardToDfk.Name = "mnuSendOneAndOnboardToDfk";
            this.mnuSendOneAndOnboardToDfk.Text = "Send ONE To Wallets";
            this.mnuSendOneAndOnboardToDfk.Click += new System.EventHandler(this.mnuSendOneAndOnboardToDfk_Click);
            // 
            // mnuOnboardToDfk
            // 
            this.mnuOnboardToDfk.Name = "mnuOnboardToDfk";
            this.mnuOnboardToDfk.Text = "Onboard Wallets To DFK (If NOT already)";
            this.mnuOnboardToDfk.Click += new System.EventHandler(this.mnuOnboardToDfk_Click);
            // 
            // mnuSendHeroesToWallets
            // 
            this.mnuSendHeroesToWallets.Name = "mnuSendHeroesToWallets";
            this.mnuSendHeroesToWallets.Text = "Send Heroes To Wallets (Ones w/o Heroes)";
            this.mnuSendHeroesToWallets.Click += new System.EventHandler(this.mnuSendHeroesToWallets_Click);
            // 
            // mnuRecallHerosToSourceWallet
            // 
            this.mnuRecallHerosToSourceWallet.Name = "mnuRecallHerosToSourceWallet";
            this.mnuRecallHerosToSourceWallet.Text = "Recall Heroes To Source Wallet";
            this.mnuRecallHerosToSourceWallet.Click += new System.EventHandler(this.mnuRecallHerosToSourceWallet_Click);
            // 
            // radMenuSeparatorItem1
            // 
            this.radMenuSeparatorItem1.Name = "radMenuSeparatorItem1";
            this.radMenuSeparatorItem1.Text = "radMenuSeparatorItem1";
            this.radMenuSeparatorItem1.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mnuGenerateNewWallets
            // 
            this.mnuGenerateNewWallets.Name = "mnuGenerateNewWallets";
            this.mnuGenerateNewWallets.Text = "Generate New Wallets";
            this.mnuGenerateNewWallets.Click += new System.EventHandler(this.mnuGenerateNewWallets_Click);
            // 
            // radMenuSeparatorItem3
            // 
            this.radMenuSeparatorItem3.Name = "radMenuSeparatorItem3";
            this.radMenuSeparatorItem3.Text = "radMenuSeparatorItem3";
            this.radMenuSeparatorItem3.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mnuReIInitSelectedWallets
            // 
            this.mnuReIInitSelectedWallets.Name = "mnuReIInitSelectedWallets";
            this.mnuReIInitSelectedWallets.Text = "Re-Init Selected Wallet(s)";
            this.mnuReIInitSelectedWallets.Click += new System.EventHandler(this.mnuReIInitSelectedWallets_Click);
            // 
            // mnuGridAction
            // 
            this.mnuGridAction.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.mnuGridActionSetAsPrimaryWallet,
            this.radMenuSeparatorItem4,
            this.mnuGridActionHeroOptions,
            this.radMenuSeparatorItem5,
            this.mnuGridActionSendOneToWallet,
            this.mnuGridActionOnBoardDfk,
            this.radMenuSeparatorItem8,
            this.mnuGridActionSendJewelTo});
            this.mnuGridAction.ThemeName = "Material";
            // 
            // mnuGridActionSetAsPrimaryWallet
            // 
            this.mnuGridActionSetAsPrimaryWallet.AccessibleDescription = "mnuGridActionSetAsPrimaryWallet";
            this.mnuGridActionSetAsPrimaryWallet.Name = "mnuGridActionSetAsPrimaryWallet";
            this.mnuGridActionSetAsPrimaryWallet.Text = "Set as source/primary wallet";
            // 
            // radMenuSeparatorItem4
            // 
            this.radMenuSeparatorItem4.Name = "radMenuSeparatorItem4";
            this.radMenuSeparatorItem4.Text = "radMenuSeparatorItem4";
            this.radMenuSeparatorItem4.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mnuGridActionHeroOptions
            // 
            this.mnuGridActionHeroOptions.AccessibleDescription = "mnuGridActionHeroOptions";
            this.mnuGridActionHeroOptions.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.mnuGridActionSendHeroToWallet,
            this.radMenuSeparatorItem6,
            this.mnuGridActionRecallHeroToSource,
            this.radMenuSeparatorItem7,
            this.mnuGridActionRebuildHeroProfile});
            this.mnuGridActionHeroOptions.Name = "mnuGridActionHeroOptions";
            this.mnuGridActionHeroOptions.Text = "Hero Options";
            // 
            // mnuGridActionSendHeroToWallet
            // 
            this.mnuGridActionSendHeroToWallet.Name = "mnuGridActionSendHeroToWallet";
            this.mnuGridActionSendHeroToWallet.Text = "Send hero to wallet";
            // 
            // radMenuSeparatorItem6
            // 
            this.radMenuSeparatorItem6.Name = "radMenuSeparatorItem6";
            this.radMenuSeparatorItem6.Text = "radMenuSeparatorItem6";
            this.radMenuSeparatorItem6.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mnuGridActionRecallHeroToSource
            // 
            this.mnuGridActionRecallHeroToSource.Name = "mnuGridActionRecallHeroToSource";
            this.mnuGridActionRecallHeroToSource.Text = "Re-call hero to source/primary wallet";
            // 
            // radMenuSeparatorItem7
            // 
            this.radMenuSeparatorItem7.Name = "radMenuSeparatorItem7";
            this.radMenuSeparatorItem7.Text = "radMenuSeparatorItem7";
            this.radMenuSeparatorItem7.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mnuGridActionRebuildHeroProfile
            // 
            this.mnuGridActionRebuildHeroProfile.Name = "mnuGridActionRebuildHeroProfile";
            this.mnuGridActionRebuildHeroProfile.Text = "Rebuild Hero Profile";
            // 
            // radMenuSeparatorItem5
            // 
            this.radMenuSeparatorItem5.Name = "radMenuSeparatorItem5";
            this.radMenuSeparatorItem5.Text = "radMenuSeparatorItem5";
            this.radMenuSeparatorItem5.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mnuGridActionSendOneToWallet
            // 
            this.mnuGridActionSendOneToWallet.Name = "mnuGridActionSendOneToWallet";
            this.mnuGridActionSendOneToWallet.Text = "Send ONE to wallet";
            // 
            // mnuGridActionOnBoardDfk
            // 
            this.mnuGridActionOnBoardDfk.Name = "mnuGridActionOnBoardDfk";
            this.mnuGridActionOnBoardDfk.Text = "Onboard wallet to DFK";
            // 
            // radMenuSeparatorItem8
            // 
            this.radMenuSeparatorItem8.Name = "radMenuSeparatorItem8";
            this.radMenuSeparatorItem8.Text = "radMenuSeparatorItem8";
            this.radMenuSeparatorItem8.TextAlignment = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mnuGridActionSendJewelTo
            // 
            this.mnuGridActionSendJewelTo.Name = "mnuGridActionSendJewelTo";
            this.mnuGridActionSendJewelTo.Text = "Send jewel to wallet";
            // 
            // mnuMainMenu
            // 
            this.mnuMainMenu.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radMenuItem1});
            this.mnuMainMenu.Location = new System.Drawing.Point(0, 0);
            this.mnuMainMenu.Name = "mnuMainMenu";
            this.mnuMainMenu.Size = new System.Drawing.Size(894, 37);
            this.mnuMainMenu.TabIndex = 0;
            this.mnuMainMenu.ThemeName = "Material";
            // 
            // commandBarSeparator1
            // 
            this.commandBarSeparator1.Name = "commandBarSeparator1";
            this.radStatusStrip1.SetSpring(this.commandBarSeparator1, false);
            this.commandBarSeparator1.Text = "";
            this.commandBarSeparator1.VisibleInOverflowMenu = false;
            // 
            // lblWalletsThatCanQuest
            // 
            this.lblWalletsThatCanQuest.Name = "lblWalletsThatCanQuest";
            this.radStatusStrip1.SetSpring(this.lblWalletsThatCanQuest, false);
            this.lblWalletsThatCanQuest.Text = "(0) Wallets Are Quest Ready";
            this.lblWalletsThatCanQuest.TextWrap = true;
            // 
            // frmManageAllWallets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 586);
            this.Controls.Add(this.gridWallets);
            this.Controls.Add(this.mnuMainMenu);
            this.Controls.Add(this.radStatusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(896, 616);
            this.Name = "frmManageAllWallets";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Your Wallets/Accounts";
            this.ThemeName = "Material";
            this.Load += new System.EventHandler(this.frmManageAllWallets_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridWallets.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridWallets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radStatusStrip1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mnuMainMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Telerik.WinControls.Themes.MaterialBlueGreyTheme materialBlueGreyTheme1;
        private Telerik.WinControls.Themes.MaterialPinkTheme materialPinkTheme1;
        private Telerik.WinControls.Themes.MaterialTealTheme materialTealTheme1;
        private Telerik.WinControls.Themes.MaterialTheme materialTheme1;
        private Telerik.WinControls.UI.RadGridView gridWallets;
        private Telerik.WinControls.UI.RadStatusStrip radStatusStrip1;
        private Telerik.WinControls.UI.RadLabelElement lblWalletStatusInfo;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem1;
        private Telerik.WinControls.UI.RadMenuItem mnuSendOneAndOnboardToDfk;
        private Telerik.WinControls.UI.RadMenuItem mnuOnboardToDfk;
        private Telerik.WinControls.UI.RadMenuItem mnuSendHeroesToWallets;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem1;
        private Telerik.WinControls.UI.RadMenuItem mnuGenerateNewWallets;
        private Telerik.WinControls.UI.RadMenuItem mnuReIInitSelectedWallets;
        private Telerik.WinControls.UI.RadMenuItem mnuRecallHerosToSourceWallet;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem3;
        private Telerik.WinControls.UI.RadContextMenu mnuGridAction;
        private Telerik.WinControls.UI.RadMenuItem mnuGridActionSetAsPrimaryWallet;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem4;
        private Telerik.WinControls.UI.RadMenuItem mnuGridActionHeroOptions;
        private Telerik.WinControls.UI.RadMenuItem mnuGridActionSendHeroToWallet;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem6;
        private Telerik.WinControls.UI.RadMenuItem mnuGridActionRecallHeroToSource;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem7;
        private Telerik.WinControls.UI.RadMenuItem mnuGridActionRebuildHeroProfile;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem5;
        private Telerik.WinControls.UI.RadMenuItem mnuGridActionSendJewelTo;
        private Telerik.WinControls.UI.RadMenuItem mnuGridActionSendOneToWallet;
        private Telerik.WinControls.UI.RadMenuItem mnuGridActionOnBoardDfk;
        private Telerik.WinControls.UI.RadMenuSeparatorItem radMenuSeparatorItem8;
        private Telerik.WinControls.UI.RadMenu mnuMainMenu;
        private Telerik.WinControls.UI.CommandBarSeparator commandBarSeparator1;
        private Telerik.WinControls.UI.RadLabelElement lblWalletsThatCanQuest;
    }
}