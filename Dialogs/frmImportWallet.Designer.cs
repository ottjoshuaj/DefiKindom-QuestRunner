using Telerik.WinControls.UI;

namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmImportWallet
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
            this.txtName = new Telerik.WinControls.UI.RadTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnImport = new Telerik.WinControls.UI.RadButton();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPrivateKey = new Telerik.WinControls.UI.RadTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.materialBlueGreyTheme1 = new Telerik.WinControls.Themes.MaterialBlueGreyTheme();
            this.materialPinkTheme1 = new Telerik.WinControls.Themes.MaterialPinkTheme();
            this.materialTealTheme1 = new Telerik.WinControls.Themes.MaterialTealTheme();
            this.materialTheme1 = new Telerik.WinControls.Themes.MaterialTheme();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnImport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrivateKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(12, 28);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(282, 20);
            this.txtName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Wallet Name:";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(175, 265);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(119, 38);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "Import";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Recovery Words:";
            // 
            // txtPrivateKey
            // 
            this.txtPrivateKey.Location = new System.Drawing.Point(12, 87);
            this.txtPrivateKey.Multiline = true;
            this.txtPrivateKey.Name = "txtPrivateKey";
            // 
            // 
            // 
            this.txtPrivateKey.RootElement.StretchVertically = true;
            this.txtPrivateKey.Size = new System.Drawing.Size(282, 78);
            this.txtPrivateKey.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 172);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(281, 74);
            this.label2.TabIndex = 7;
            this.label2.Text = "Note: If wallet has multiple accounts under it then they will all be imported. Th" +
    "ey will take on the provided name along with a number value after it. ";
            // 
            // frmImportWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 334);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtPrivateKey);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportWallet";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "     Import an existing wallet";
            this.Load += new System.EventHandler(this.frmImportWallet_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnImport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrivateKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private RadTextBox txtName;
        private System.Windows.Forms.Label label1;
        private RadButton btnImport;
        private System.Windows.Forms.Label label3;
        private RadTextBox txtPrivateKey;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.Themes.MaterialBlueGreyTheme materialBlueGreyTheme1;
        private Telerik.WinControls.Themes.MaterialPinkTheme materialPinkTheme1;
        private Telerik.WinControls.Themes.MaterialTealTheme materialTealTheme1;
        private Telerik.WinControls.Themes.MaterialTheme materialTheme1;
    }
}