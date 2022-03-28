namespace DefiKindom_QuestRunner
{
    partial class frmConsole
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripWalletCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPreferences = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWallets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuManageWallets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCreateRandomWallets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImportExistingWallet = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuManageAllWallets = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExportWalletDataFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImportWalletDataFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuActions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRPCOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuManageRPCSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.cmbRpcUrls = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.txtStatusConsole = new System.Windows.Forms.RichTextBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripWalletCount});
            this.statusStrip1.Location = new System.Drawing.Point(0, 604);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(967, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripWalletCount
            // 
            this.toolStripWalletCount.Name = "toolStripWalletCount";
            this.toolStripWalletCount.Overflow = System.Windows.Forms.ToolStripItemOverflow.Always;
            this.toolStripWalletCount.Size = new System.Drawing.Size(952, 17);
            this.toolStripWalletCount.Spring = true;
            this.toolStripWalletCount.Text = "Welcome to DFK QRunner+";
            this.toolStripWalletCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuWallets,
            this.mnuActions,
            this.mnuRPCOptions});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(967, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuPreferences});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuPreferences
            // 
            this.mnuPreferences.Name = "mnuPreferences";
            this.mnuPreferences.Size = new System.Drawing.Size(135, 22);
            this.mnuPreferences.Text = "Preferences";
            // 
            // mnuWallets
            // 
            this.mnuWallets.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuManageWallets,
            this.toolStripSeparator2,
            this.mnuExportWalletDataFile,
            this.mnuImportWalletDataFile});
            this.mnuWallets.Name = "mnuWallets";
            this.mnuWallets.Size = new System.Drawing.Size(57, 20);
            this.mnuWallets.Text = "&Wallets";
            // 
            // mnuManageWallets
            // 
            this.mnuManageWallets.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCreateRandomWallets,
            this.mnuImportExistingWallet,
            this.toolStripSeparator1,
            this.mnuManageAllWallets});
            this.mnuManageWallets.Name = "mnuManageWallets";
            this.mnuManageWallets.Size = new System.Drawing.Size(194, 22);
            this.mnuManageWallets.Text = "Manage Wallets";
            // 
            // mnuCreateRandomWallets
            // 
            this.mnuCreateRandomWallets.Name = "mnuCreateRandomWallets";
            this.mnuCreateRandomWallets.Size = new System.Drawing.Size(197, 22);
            this.mnuCreateRandomWallets.Text = "Create Random Wallets";
            this.mnuCreateRandomWallets.Click += new System.EventHandler(this.mnuCreateRandomWallets_Click);
            // 
            // mnuImportExistingWallet
            // 
            this.mnuImportExistingWallet.Name = "mnuImportExistingWallet";
            this.mnuImportExistingWallet.Size = new System.Drawing.Size(197, 22);
            this.mnuImportExistingWallet.Text = "Import Existing Wallet";
            this.mnuImportExistingWallet.Click += new System.EventHandler(this.mnuImportExistingWallet_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
            // 
            // mnuManageAllWallets
            // 
            this.mnuManageAllWallets.Name = "mnuManageAllWallets";
            this.mnuManageAllWallets.Size = new System.Drawing.Size(197, 22);
            this.mnuManageAllWallets.Text = "Manage All Wallets";
            this.mnuManageAllWallets.Click += new System.EventHandler(this.mnuManageAllWallets_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(191, 6);
            // 
            // mnuExportWalletDataFile
            // 
            this.mnuExportWalletDataFile.Name = "mnuExportWalletDataFile";
            this.mnuExportWalletDataFile.Size = new System.Drawing.Size(194, 22);
            this.mnuExportWalletDataFile.Text = "Export Wallet Data File";
            // 
            // mnuImportWalletDataFile
            // 
            this.mnuImportWalletDataFile.Name = "mnuImportWalletDataFile";
            this.mnuImportWalletDataFile.Size = new System.Drawing.Size(194, 22);
            this.mnuImportWalletDataFile.Text = "Import Wallet Data File";
            this.mnuImportWalletDataFile.Click += new System.EventHandler(this.mnuImportWalletDataFile_Click);
            // 
            // mnuActions
            // 
            this.mnuActions.Name = "mnuActions";
            this.mnuActions.Size = new System.Drawing.Size(59, 20);
            this.mnuActions.Text = "&Actions";
            // 
            // mnuRPCOptions
            // 
            this.mnuRPCOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuManageRPCSettings});
            this.mnuRPCOptions.Name = "mnuRPCOptions";
            this.mnuRPCOptions.Size = new System.Drawing.Size(41, 20);
            this.mnuRPCOptions.Text = "&RPC";
            // 
            // mnuManageRPCSettings
            // 
            this.mnuManageRPCSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmbRpcUrls});
            this.mnuManageRPCSettings.Name = "mnuManageRPCSettings";
            this.mnuManageRPCSettings.Size = new System.Drawing.Size(187, 22);
            this.mnuManageRPCSettings.Text = "Manage RPC Settings";
            // 
            // cmbRpcUrls
            // 
            this.cmbRpcUrls.Name = "cmbRpcUrls";
            this.cmbRpcUrls.Size = new System.Drawing.Size(121, 23);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(967, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // txtStatusConsole
            // 
            this.txtStatusConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatusConsole.Location = new System.Drawing.Point(0, 49);
            this.txtStatusConsole.Name = "txtStatusConsole";
            this.txtStatusConsole.Size = new System.Drawing.Size(967, 555);
            this.txtStatusConsole.TabIndex = 3;
            this.txtStatusConsole.Text = "";
            // 
            // frmConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 626);
            this.Controls.Add(this.txtStatusConsole);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmConsole";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DFK QRunner+";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuWallets;
        private System.Windows.Forms.ToolStripMenuItem mnuManageWallets;
        private System.Windows.Forms.ToolStripMenuItem mnuCreateRandomWallets;
        private System.Windows.Forms.ToolStripMenuItem mnuImportExistingWallet;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuManageAllWallets;
        private System.Windows.Forms.ToolStripMenuItem mnuPreferences;
        private System.Windows.Forms.ToolStripMenuItem mnuActions;
        private System.Windows.Forms.RichTextBox txtStatusConsole;
        private System.Windows.Forms.ToolStripMenuItem mnuRPCOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuManageRPCSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuExportWalletDataFile;
        private System.Windows.Forms.ToolStripMenuItem mnuImportWalletDataFile;
        private System.Windows.Forms.ToolStripStatusLabel toolStripWalletCount;
        private System.Windows.Forms.ToolStripComboBox cmbRpcUrls;
    }
}

