namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmSendJewel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSendJewel));
            this.materialTheme1 = new Telerik.WinControls.Themes.MaterialTheme();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSourcePrivateKey = new Telerik.WinControls.UI.RadTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDestinationPrivateKey = new Telerik.WinControls.UI.RadTextBox();
            this.btnTransferJewel = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtSourcePrivateKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDestinationPrivateKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTransferJewel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Source Wallet Private Key:";
            // 
            // txtSourcePrivateKey
            // 
            this.txtSourcePrivateKey.Location = new System.Drawing.Point(22, 184);
            this.txtSourcePrivateKey.Multiline = true;
            this.txtSourcePrivateKey.Name = "txtSourcePrivateKey";
            // 
            // 
            // 
            this.txtSourcePrivateKey.RootElement.StretchVertically = true;
            this.txtSourcePrivateKey.Size = new System.Drawing.Size(282, 78);
            this.txtSourcePrivateKey.TabIndex = 8;
            this.txtSourcePrivateKey.ThemeName = "Material";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(331, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Destination Wallet Private Key:";
            // 
            // txtDestinationPrivateKey
            // 
            this.txtDestinationPrivateKey.Location = new System.Drawing.Point(331, 184);
            this.txtDestinationPrivateKey.Multiline = true;
            this.txtDestinationPrivateKey.Name = "txtDestinationPrivateKey";
            // 
            // 
            // 
            this.txtDestinationPrivateKey.RootElement.StretchVertically = true;
            this.txtDestinationPrivateKey.Size = new System.Drawing.Size(282, 78);
            this.txtDestinationPrivateKey.TabIndex = 11;
            this.txtDestinationPrivateKey.ThemeName = "Material";
            // 
            // btnTransferJewel
            // 
            this.btnTransferJewel.Location = new System.Drawing.Point(360, 304);
            this.btnTransferJewel.Name = "btnTransferJewel";
            this.btnTransferJewel.Size = new System.Drawing.Size(253, 38);
            this.btnTransferJewel.TabIndex = 10;
            this.btnTransferJewel.Text = "Transfer All Jewel";
            this.btnTransferJewel.ThemeName = "Material";
            this.btnTransferJewel.Click += new System.EventHandler(this.btnTransferJewel_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.AutoSize = false;
            this.radLabel1.Location = new System.Drawing.Point(25, 12);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(588, 85);
            this.radLabel1.TabIndex = 13;
            this.radLabel1.Text = resources.GetString("radLabel1.Text");
            this.radLabel1.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            // 
            // frmSendJewel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 365);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDestinationPrivateKey);
            this.Controls.Add(this.btnTransferJewel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSourcePrivateKey);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSendJewel";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "  Send Jewel To Wallet";
            this.ThemeName = "Material";
            this.Load += new System.EventHandler(this.frmSendJewel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtSourcePrivateKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDestinationPrivateKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTransferJewel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.Themes.MaterialTheme materialTheme1;
        private System.Windows.Forms.Label label3;
        private Telerik.WinControls.UI.RadTextBox txtSourcePrivateKey;
        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadTextBox txtDestinationPrivateKey;
        private Telerik.WinControls.UI.RadButton btnTransferJewel;
        private Telerik.WinControls.UI.RadLabel radLabel1;
    }
}