namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmSendONEToWallets
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSendONEToWallets));
            this.radDesktopAlert1 = new Telerik.WinControls.UI.RadDesktopAlert(this.components);
            this.radPanel1 = new Telerik.WinControls.UI.RadPanel();
            this.chkSendToNewWalletsOnly = new Telerik.WinControls.UI.RadCheckBox();
            this.btnSendOne = new Telerik.WinControls.UI.RadButton();
            this.txtOneAmountToSend = new Telerik.WinControls.UI.RadSpinEditor();
            this.lblSendOneHeader = new Telerik.WinControls.UI.RadLabel();
            this.materialTheme1 = new Telerik.WinControls.Themes.MaterialTheme();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).BeginInit();
            this.radPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSendToNewWalletsOnly)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSendOne)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOneAmountToSend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSendOneHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radPanel1
            // 
            this.radPanel1.BackColor = System.Drawing.Color.White;
            this.radPanel1.Controls.Add(this.chkSendToNewWalletsOnly);
            this.radPanel1.Controls.Add(this.btnSendOne);
            this.radPanel1.Controls.Add(this.txtOneAmountToSend);
            this.radPanel1.Controls.Add(this.lblSendOneHeader);
            this.radPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel1.Location = new System.Drawing.Point(0, 0);
            this.radPanel1.Name = "radPanel1";
            this.radPanel1.Size = new System.Drawing.Size(334, 252);
            this.radPanel1.TabIndex = 0;
            // 
            // chkSendToNewWalletsOnly
            // 
            this.chkSendToNewWalletsOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSendToNewWalletsOnly.Location = new System.Drawing.Point(22, 129);
            this.chkSendToNewWalletsOnly.Name = "chkSendToNewWalletsOnly";
            this.chkSendToNewWalletsOnly.Size = new System.Drawing.Size(281, 19);
            this.chkSendToNewWalletsOnly.TabIndex = 3;
            this.chkSendToNewWalletsOnly.Text = "Send to NEW Wallets Only (Balance < 5)";
            this.chkSendToNewWalletsOnly.ThemeName = "Material";
            this.chkSendToNewWalletsOnly.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            // 
            // btnSendOne
            // 
            this.btnSendOne.Location = new System.Drawing.Point(22, 175);
            this.btnSendOne.Name = "btnSendOne";
            this.btnSendOne.Size = new System.Drawing.Size(290, 40);
            this.btnSendOne.TabIndex = 2;
            this.btnSendOne.Text = "Send ONE";
            this.btnSendOne.ThemeName = "Material";
            this.btnSendOne.Click += new System.EventHandler(this.btnSendOne_Click);
            // 
            // txtOneAmountToSend
            // 
            this.txtOneAmountToSend.Location = new System.Drawing.Point(22, 70);
            this.txtOneAmountToSend.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.txtOneAmountToSend.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtOneAmountToSend.Name = "txtOneAmountToSend";
            this.txtOneAmountToSend.NullableValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtOneAmountToSend.Size = new System.Drawing.Size(300, 36);
            this.txtOneAmountToSend.TabIndex = 1;
            this.txtOneAmountToSend.ThemeName = "Material";
            this.txtOneAmountToSend.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblSendOneHeader
            // 
            this.lblSendOneHeader.AutoSize = false;
            this.lblSendOneHeader.Location = new System.Drawing.Point(22, 12);
            this.lblSendOneHeader.Name = "lblSendOneHeader";
            this.lblSendOneHeader.Size = new System.Drawing.Size(300, 43);
            this.lblSendOneHeader.TabIndex = 0;
            this.lblSendOneHeader.Text = "How much ONE to you want to send from your SOURCE wallet to the other wallets?";
            // 
            // frmSendONEToWallets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 252);
            this.Controls.Add(this.radPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSendONEToWallets";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Send ONE To Wallets";
            this.ThemeName = "Material";
            this.Load += new System.EventHandler(this.frmSendONEToWallets_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel1)).EndInit();
            this.radPanel1.ResumeLayout(false);
            this.radPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkSendToNewWalletsOnly)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSendOne)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOneAmountToSend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSendOneHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadDesktopAlert radDesktopAlert1;
        private Telerik.WinControls.UI.RadPanel radPanel1;
        private Telerik.WinControls.UI.RadButton btnSendOne;
        private Telerik.WinControls.UI.RadSpinEditor txtOneAmountToSend;
        private Telerik.WinControls.UI.RadLabel lblSendOneHeader;
        private Telerik.WinControls.UI.RadCheckBox chkSendToNewWalletsOnly;
        private Telerik.WinControls.Themes.MaterialTheme materialTheme1;
    }
}