namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmCreateNewWallets
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
            this.radDesktopAlert1 = new Telerik.WinControls.UI.RadDesktopAlert(this.components);
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.btnGenerateWallets = new Telerik.WinControls.UI.RadButton();
            this.txtNewWalletAmount = new Telerik.WinControls.UI.RadSpinEditor();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnGenerateWallets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNewWalletAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.BackColor = System.Drawing.Color.White;
            this.radPanel1.Controls.Add(this.btnGenerateWallets);
            this.radPanel1.Controls.Add(this.txtNewWalletAmount);
            this.radPanel1.Controls.Add(this.radLabel1);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(334, 174);
            this.radPanel1.TabIndex = 0;
            // 
            // btnGenerateWallets
            // 
            this.btnGenerateWallets.Location = new System.Drawing.Point(199, 113);
            this.btnGenerateWallets.Name = "btnGenerateWallets";
            this.btnGenerateWallets.Size = new System.Drawing.Size(110, 24);
            this.btnGenerateWallets.TabIndex = 2;
            this.btnGenerateWallets.Text = "Generate";
            this.btnGenerateWallets.Click += new System.EventHandler(this.btnGenerateWallets_Click);
            // 
            // txtNewWalletAmount
            // 
            this.txtNewWalletAmount.Location = new System.Drawing.Point(22, 61);
            this.txtNewWalletAmount.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.txtNewWalletAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtNewWalletAmount.Name = "txtNewWalletAmount";
            this.txtNewWalletAmount.NullableValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtNewWalletAmount.Size = new System.Drawing.Size(300, 20);
            this.txtNewWalletAmount.TabIndex = 1;
            this.txtNewWalletAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radLabel1
            // 
            this.radLabel1.AutoSize = false;
            this.radLabel1.Location = new System.Drawing.Point(22, 12);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(300, 43);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "How many wallets would you like to dynamically generate?";
            // 
            // frmCreateNewWallets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 174);
            this.Controls.Add(this.radPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCreateNewWallets";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "  Create New Wallets";
            this.Load += new System.EventHandler(this.frmCreateNewWallets_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnGenerateWallets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNewWalletAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadDesktopAlert radDesktopAlert1;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadButton btnGenerateWallets;
        private Telerik.WinControls.UI.RadSpinEditor txtNewWalletAmount;
        private Telerik.WinControls.UI.RadLabel radLabel1;
    }
}