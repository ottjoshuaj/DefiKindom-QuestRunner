namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmPreferences
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPreferences));
            this.radGroupBox1 = new Telerik.WinControls.UI.RadGroupBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.cmbRpcChain = new Telerik.WinControls.UI.RadDropDownList();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.cmbRPCSettings = new Telerik.WinControls.UI.RadDropDownList();
            this.btnSavePreferences = new Telerik.WinControls.UI.RadButton();
            this.materialTheme1 = new Telerik.WinControls.Themes.MaterialTheme();
            this.tabControl = new Telerik.WinControls.UI.RadPageView();
            this.tabAppSettings = new Telerik.WinControls.UI.RadPageViewPage();
            this.radGroupBox3 = new Telerik.WinControls.UI.RadGroupBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.txtQuestInterval = new Telerik.WinControls.UI.RadSpinEditor();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.txtJewelInstanceInterval = new Telerik.WinControls.UI.RadSpinEditor();
            this.radGroupBox2 = new Telerik.WinControls.UI.RadGroupBox();
            this.chkHideToTrayOnMinimize = new Telerik.WinControls.UI.RadCheckBox();
            this.tabBlockChainSettings = new Telerik.WinControls.UI.RadPageViewPage();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.txtNodeJsServerEndpoint = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).BeginInit();
            this.radGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRpcChain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRPCSettings)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSavePreferences)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabAppSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox3)).BeginInit();
            this.radGroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuestInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtJewelInstanceInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox2)).BeginInit();
            this.radGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideToTrayOnMinimize)).BeginInit();
            this.tabBlockChainSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNodeJsServerEndpoint)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radGroupBox1
            // 
            this.radGroupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox1.Controls.Add(this.radLabel2);
            this.radGroupBox1.Controls.Add(this.cmbRpcChain);
            this.radGroupBox1.Controls.Add(this.radLabel1);
            this.radGroupBox1.Controls.Add(this.cmbRPCSettings);
            this.radGroupBox1.HeaderText = "RPC Settings";
            this.radGroupBox1.Location = new System.Drawing.Point(3, 19);
            this.radGroupBox1.Name = "radGroupBox1";
            this.radGroupBox1.Size = new System.Drawing.Size(296, 492);
            this.radGroupBox1.TabIndex = 0;
            this.radGroupBox1.Text = "RPC Settings";
            this.radGroupBox1.ThemeName = "Material";
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(18, 130);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(48, 18);
            this.radLabel2.TabIndex = 3;
            this.radLabel2.Text = "ChainID:";
            // 
            // cmbRpcChain
            // 
            this.cmbRpcChain.DropDownAnimationEnabled = true;
            this.cmbRpcChain.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbRpcChain.Location = new System.Drawing.Point(18, 154);
            this.cmbRpcChain.Name = "cmbRpcChain";
            this.cmbRpcChain.Size = new System.Drawing.Size(259, 36);
            this.cmbRpcChain.TabIndex = 2;
            this.cmbRpcChain.ThemeName = "Material";
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(18, 62);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(46, 18);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "RPC Url:";
            // 
            // cmbRPCSettings
            // 
            this.cmbRPCSettings.DropDownAnimationEnabled = true;
            this.cmbRPCSettings.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.cmbRPCSettings.Location = new System.Drawing.Point(18, 86);
            this.cmbRPCSettings.Name = "cmbRPCSettings";
            this.cmbRPCSettings.Size = new System.Drawing.Size(259, 36);
            this.cmbRPCSettings.TabIndex = 0;
            this.cmbRPCSettings.ThemeName = "Material";
            // 
            // btnSavePreferences
            // 
            this.btnSavePreferences.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePreferences.Location = new System.Drawing.Point(610, 594);
            this.btnSavePreferences.Name = "btnSavePreferences";
            this.btnSavePreferences.Size = new System.Drawing.Size(125, 36);
            this.btnSavePreferences.TabIndex = 1;
            this.btnSavePreferences.Text = "Save && Close";
            this.btnSavePreferences.ThemeName = "Material";
            this.btnSavePreferences.Click += new System.EventHandler(this.btnSavePreferences_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabAppSettings);
            this.tabControl.Controls.Add(this.tabBlockChainSettings);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedPage = this.tabAppSettings;
            this.tabControl.Size = new System.Drawing.Size(898, 575);
            this.tabControl.TabIndex = 2;
            this.tabControl.ThemeName = "Material";
            // 
            // tabAppSettings
            // 
            this.tabAppSettings.Controls.Add(this.radGroupBox3);
            this.tabAppSettings.Controls.Add(this.radGroupBox2);
            this.tabAppSettings.ItemSize = new System.Drawing.SizeF(158F, 49F);
            this.tabAppSettings.Location = new System.Drawing.Point(6, 55);
            this.tabAppSettings.Name = "tabAppSettings";
            this.tabAppSettings.Size = new System.Drawing.Size(886, 514);
            this.tabAppSettings.Text = "Application Settings";
            // 
            // radGroupBox3
            // 
            this.radGroupBox3.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox3.Controls.Add(this.txtNodeJsServerEndpoint);
            this.radGroupBox3.Controls.Add(this.radLabel5);
            this.radGroupBox3.Controls.Add(this.radLabel4);
            this.radGroupBox3.Controls.Add(this.txtQuestInterval);
            this.radGroupBox3.Controls.Add(this.radLabel3);
            this.radGroupBox3.Controls.Add(this.txtJewelInstanceInterval);
            this.radGroupBox3.HeaderText = "Instance Settings";
            this.radGroupBox3.Location = new System.Drawing.Point(321, 12);
            this.radGroupBox3.Name = "radGroupBox3";
            this.radGroupBox3.Size = new System.Drawing.Size(559, 499);
            this.radGroupBox3.TabIndex = 2;
            this.radGroupBox3.Text = "Instance Settings";
            this.radGroupBox3.ThemeName = "Material";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(14, 137);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(211, 21);
            this.radLabel4.TabIndex = 3;
            this.radLabel4.Text = " Quest Manager Interval (In ms)";
            this.radLabel4.ThemeName = "Material";
            // 
            // txtQuestInterval
            // 
            this.txtQuestInterval.Location = new System.Drawing.Point(14, 164);
            this.txtQuestInterval.Maximum = new decimal(new int[] {
            1705032704,
            1,
            0,
            0});
            this.txtQuestInterval.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtQuestInterval.Name = "txtQuestInterval";
            this.txtQuestInterval.NullableValue = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtQuestInterval.Size = new System.Drawing.Size(266, 36);
            this.txtQuestInterval.TabIndex = 2;
            this.txtQuestInterval.ThemeName = "Material";
            this.txtQuestInterval.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(14, 54);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(207, 21);
            this.radLabel3.TabIndex = 1;
            this.radLabel3.Text = "Jewel Manager Interval (In ms)";
            this.radLabel3.ThemeName = "Material";
            // 
            // txtJewelInstanceInterval
            // 
            this.txtJewelInstanceInterval.Location = new System.Drawing.Point(14, 81);
            this.txtJewelInstanceInterval.Maximum = new decimal(new int[] {
            1705032704,
            1,
            0,
            0});
            this.txtJewelInstanceInterval.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtJewelInstanceInterval.Name = "txtJewelInstanceInterval";
            this.txtJewelInstanceInterval.NullableValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtJewelInstanceInterval.Size = new System.Drawing.Size(266, 36);
            this.txtJewelInstanceInterval.TabIndex = 0;
            this.txtJewelInstanceInterval.ThemeName = "Material";
            this.txtJewelInstanceInterval.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // radGroupBox2
            // 
            this.radGroupBox2.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this.radGroupBox2.Controls.Add(this.chkHideToTrayOnMinimize);
            this.radGroupBox2.HeaderText = "App Behavior";
            this.radGroupBox2.Location = new System.Drawing.Point(6, 12);
            this.radGroupBox2.Name = "radGroupBox2";
            this.radGroupBox2.Size = new System.Drawing.Size(299, 499);
            this.radGroupBox2.TabIndex = 1;
            this.radGroupBox2.Text = "App Behavior";
            this.radGroupBox2.ThemeName = "Material";
            // 
            // chkHideToTrayOnMinimize
            // 
            this.chkHideToTrayOnMinimize.Location = new System.Drawing.Point(15, 67);
            this.chkHideToTrayOnMinimize.Name = "chkHideToTrayOnMinimize";
            this.chkHideToTrayOnMinimize.Size = new System.Drawing.Size(186, 19);
            this.chkHideToTrayOnMinimize.TabIndex = 0;
            this.chkHideToTrayOnMinimize.Text = " Hide to tray on minimize";
            this.chkHideToTrayOnMinimize.ThemeName = "Material";
            // 
            // tabBlockChainSettings
            // 
            this.tabBlockChainSettings.Controls.Add(this.radGroupBox1);
            this.tabBlockChainSettings.ItemSize = new System.Drawing.SizeF(189F, 49F);
            this.tabBlockChainSettings.Location = new System.Drawing.Point(6, 55);
            this.tabBlockChainSettings.Name = "tabBlockChainSettings";
            this.tabBlockChainSettings.Size = new System.Drawing.Size(886, 514);
            this.tabBlockChainSettings.Text = "ONE BlockChain Settings";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(761, 594);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(125, 36);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ThemeName = "Material";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // radLabel5
            // 
            this.radLabel5.Location = new System.Drawing.Point(14, 236);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(235, 21);
            this.radLabel5.TabIndex = 4;
            this.radLabel5.Text = "DFKQR+ NodeJS Contract Endpoint";
            this.radLabel5.ThemeName = "Material";
            // 
            // txtNodeJsServerEndpoint
            // 
            this.txtNodeJsServerEndpoint.Location = new System.Drawing.Point(14, 263);
            this.txtNodeJsServerEndpoint.Name = "txtNodeJsServerEndpoint";
            this.txtNodeJsServerEndpoint.Size = new System.Drawing.Size(518, 36);
            this.txtNodeJsServerEndpoint.TabIndex = 5;
            this.txtNodeJsServerEndpoint.ThemeName = "Material";
            // 
            // frmPreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 653);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnSavePreferences);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(900, 690);
            this.Name = "frmPreferences";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.ThemeName = "Material";
            this.Load += new System.EventHandler(this.frmPreferences_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox1)).EndInit();
            this.radGroupBox1.ResumeLayout(false);
            this.radGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRpcChain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRPCSettings)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSavePreferences)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabAppSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox3)).EndInit();
            this.radGroupBox3.ResumeLayout(false);
            this.radGroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuestInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtJewelInstanceInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGroupBox2)).EndInit();
            this.radGroupBox2.ResumeLayout(false);
            this.radGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chkHideToTrayOnMinimize)).EndInit();
            this.tabBlockChainSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNodeJsServerEndpoint)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGroupBox radGroupBox1;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadDropDownList cmbRPCSettings;
        private Telerik.WinControls.UI.RadButton btnSavePreferences;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadDropDownList cmbRpcChain;
        private Telerik.WinControls.Themes.MaterialTheme materialTheme1;
        private Telerik.WinControls.UI.RadPageView tabControl;
        private Telerik.WinControls.UI.RadPageViewPage tabAppSettings;
        private Telerik.WinControls.UI.RadPageViewPage tabBlockChainSettings;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox3;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadSpinEditor txtQuestInterval;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadSpinEditor txtJewelInstanceInterval;
        private Telerik.WinControls.UI.RadGroupBox radGroupBox2;
        private Telerik.WinControls.UI.RadCheckBox chkHideToTrayOnMinimize;
        private Telerik.WinControls.UI.RadTextBox txtNodeJsServerEndpoint;
        private Telerik.WinControls.UI.RadLabel radLabel5;
    }
}