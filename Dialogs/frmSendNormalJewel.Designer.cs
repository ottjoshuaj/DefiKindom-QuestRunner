namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmSendNormalJewel
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
            this.materialTheme1 = new Telerik.WinControls.Themes.MaterialTheme();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDestinationAddress = new Telerik.WinControls.UI.RadTextBox();
            this.btnTransferJewel = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.txtJewelAmount = new Telerik.WinControls.UI.RadSpinEditor();
            ((System.ComponentModel.ISupportInitialize)(this.txtDestinationAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTransferJewel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtJewelAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Amount:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(238, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Destination Wallet Address:";
            // 
            // txtDestinationAddress
            // 
            this.txtDestinationAddress.Location = new System.Drawing.Point(241, 85);
            this.txtDestinationAddress.Multiline = true;
            this.txtDestinationAddress.Name = "txtDestinationAddress";
            // 
            // 
            // 
            this.txtDestinationAddress.RootElement.StretchVertically = true;
            this.txtDestinationAddress.Size = new System.Drawing.Size(311, 39);
            this.txtDestinationAddress.TabIndex = 11;
            this.txtDestinationAddress.ThemeName = "Material";
            // 
            // btnTransferJewel
            // 
            this.btnTransferJewel.Location = new System.Drawing.Point(332, 157);
            this.btnTransferJewel.Name = "btnTransferJewel";
            this.btnTransferJewel.Size = new System.Drawing.Size(220, 38);
            this.btnTransferJewel.TabIndex = 10;
            this.btnTransferJewel.Text = "Transfer";
            this.btnTransferJewel.ThemeName = "Material";
            this.btnTransferJewel.Click += new System.EventHandler(this.btnTransferJewel_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.AutoSize = false;
            this.radLabel1.Location = new System.Drawing.Point(25, 12);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(542, 44);
            this.radLabel1.TabIndex = 13;
            this.radLabel1.Text = "This utility allows you to transfer normal jewel from one the current jewel holde" +
    "r to the destination address/privatekey";
            this.radLabel1.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            // 
            // txtJewelAmount
            // 
            this.txtJewelAmount.Location = new System.Drawing.Point(25, 88);
            this.txtJewelAmount.Maximum = new decimal(new int[] {
            1705032704,
            1,
            0,
            0});
            this.txtJewelAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtJewelAmount.Name = "txtJewelAmount";
            this.txtJewelAmount.NullableValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtJewelAmount.Size = new System.Drawing.Size(166, 36);
            this.txtJewelAmount.TabIndex = 14;
            this.txtJewelAmount.ThemeName = "Material";
            this.txtJewelAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // frmSendNormalJewel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 217);
            this.Controls.Add(this.txtJewelAmount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDestinationAddress);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.btnTransferJewel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSendNormalJewel";
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
            ((System.ComponentModel.ISupportInitialize)(this.txtDestinationAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTransferJewel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtJewelAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.Themes.MaterialTheme materialTheme1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private Telerik.WinControls.UI.RadTextBox txtDestinationAddress;
        private Telerik.WinControls.UI.RadButton btnTransferJewel;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadSpinEditor txtJewelAmount;
    }
}