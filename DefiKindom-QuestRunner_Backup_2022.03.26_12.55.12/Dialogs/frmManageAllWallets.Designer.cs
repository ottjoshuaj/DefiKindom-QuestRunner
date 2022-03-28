namespace DefiKindom_QuestRunner.Dialogs
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
            this.gridWallets = new System.Windows.Forms.DataGridView();
            this.colEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colIsPrimarySourceWallet = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWalletAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDfkProfileExists = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colAssignedHero = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAssignedHeroStamina = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsOnQuest = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridWallets)).BeginInit();
            this.SuspendLayout();
            // 
            // gridWallets
            // 
            this.gridWallets.AllowUserToAddRows = false;
            this.gridWallets.AllowUserToDeleteRows = false;
            this.gridWallets.AllowUserToResizeColumns = false;
            this.gridWallets.AllowUserToResizeRows = false;
            this.gridWallets.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gridWallets.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gridWallets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridWallets.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colEnabled,
            this.colIsPrimarySourceWallet,
            this.colName,
            this.colWalletAddress,
            this.colDfkProfileExists,
            this.colAssignedHero,
            this.colAssignedHeroStamina,
            this.colIsOnQuest});
            this.gridWallets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridWallets.Location = new System.Drawing.Point(0, 0);
            this.gridWallets.Name = "gridWallets";
            this.gridWallets.ReadOnly = true;
            this.gridWallets.Size = new System.Drawing.Size(1018, 664);
            this.gridWallets.TabIndex = 0;
            // 
            // colEnabled
            // 
            this.colEnabled.HeaderText = "Enabled To Mine";
            this.colEnabled.Name = "colEnabled";
            this.colEnabled.ReadOnly = true;
            this.colEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colEnabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colEnabled.Width = 83;
            // 
            // colIsPrimarySourceWallet
            // 
            this.colIsPrimarySourceWallet.HeaderText = "Primary/Source Account";
            this.colIsPrimarySourceWallet.Name = "colIsPrimarySourceWallet";
            this.colIsPrimarySourceWallet.ReadOnly = true;
            this.colIsPrimarySourceWallet.Width = 116;
            // 
            // colName
            // 
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Wallet Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 86;
            // 
            // colWalletAddress
            // 
            this.colWalletAddress.DataPropertyName = "Address";
            this.colWalletAddress.HeaderText = "Address";
            this.colWalletAddress.Name = "colWalletAddress";
            this.colWalletAddress.ReadOnly = true;
            this.colWalletAddress.Width = 70;
            // 
            // colDfkProfileExists
            // 
            this.colDfkProfileExists.HeaderText = "Has DFK Profile";
            this.colDfkProfileExists.Name = "colDfkProfileExists";
            this.colDfkProfileExists.ReadOnly = true;
            this.colDfkProfileExists.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colDfkProfileExists.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colDfkProfileExists.Width = 98;
            // 
            // colAssignedHero
            // 
            this.colAssignedHero.DataPropertyName = "AssignedHero";
            this.colAssignedHero.HeaderText = "Assigned Hero";
            this.colAssignedHero.Name = "colAssignedHero";
            this.colAssignedHero.ReadOnly = true;
            this.colAssignedHero.Width = 93;
            // 
            // colAssignedHeroStamina
            // 
            this.colAssignedHeroStamina.HeaderText = "Hero Stamina";
            this.colAssignedHeroStamina.Name = "colAssignedHeroStamina";
            this.colAssignedHeroStamina.ReadOnly = true;
            this.colAssignedHeroStamina.Width = 88;
            // 
            // colIsOnQuest
            // 
            this.colIsOnQuest.HeaderText = "Is Mining";
            this.colIsOnQuest.Name = "colIsOnQuest";
            this.colIsOnQuest.ReadOnly = true;
            this.colIsOnQuest.Width = 69;
            // 
            // frmManageAllWallets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 664);
            this.Controls.Add(this.gridWallets);
            this.DoubleBuffered = true;
            this.Name = "frmManageAllWallets";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Your Wallets/Accounts";
            this.Load += new System.EventHandler(this.frmManageAllWallets_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridWallets)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridWallets;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colEnabled;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIsPrimarySourceWallet;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWalletAddress;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colDfkProfileExists;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAssignedHero;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAssignedHeroStamina;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIsOnQuest;
    }
}