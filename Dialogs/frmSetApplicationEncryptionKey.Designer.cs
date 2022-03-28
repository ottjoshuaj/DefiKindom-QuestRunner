using Telerik.WinControls.UI;

namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmSetApplicationEncryptionKey
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtEncryptionKey = new Telerik.WinControls.UI.RadTextBox();
            this.btnSetKey = new Telerik.WinControls.UI.RadButton();
            this.label2 = new System.Windows.Forms.Label();
            this.materialBlueGreyTheme1 = new Telerik.WinControls.Themes.MaterialBlueGreyTheme();
            this.materialPinkTheme1 = new Telerik.WinControls.Themes.MaterialPinkTheme();
            this.materialTealTheme1 = new Telerik.WinControls.Themes.MaterialTealTheme();
            this.materialTheme1 = new Telerik.WinControls.Themes.MaterialTheme();
            ((System.ComponentModel.ISupportInitialize)(this.txtEncryptionKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSetKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(323, 50);
            this.label1.TabIndex = 0;
            this.label1.Text = "You have not yet set an encryption key for the application.  Please provide a 4 c" +
    "haracter key below.  ";
            // 
            // txtEncryptionKey
            // 
            this.txtEncryptionKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEncryptionKey.Location = new System.Drawing.Point(12, 66);
            this.txtEncryptionKey.MaxLength = 4;
            this.txtEncryptionKey.Name = "txtEncryptionKey";
            this.txtEncryptionKey.Size = new System.Drawing.Size(320, 36);
            this.txtEncryptionKey.TabIndex = 1;
            this.txtEncryptionKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSetKey
            // 
            this.btnSetKey.Location = new System.Drawing.Point(186, 184);
            this.btnSetKey.Name = "btnSetKey";
            this.btnSetKey.Size = new System.Drawing.Size(146, 31);
            this.btnSetKey.TabIndex = 2;
            this.btnSetKey.Text = "Set Key";
            this.btnSetKey.Click += new System.EventHandler(this.btnSetKey_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(316, 40);
            this.label2.TabIndex = 3;
            this.label2.Text = "Note: Save this value somewhere!  You will need to re-apply this value if you eve" +
    "r reinstall this application!";
            // 
            // frmSetApplicationEncryptionKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 236);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSetKey);
            this.Controls.Add(this.txtEncryptionKey);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSetApplicationEncryptionKey";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "     Application Encryption Key Setup";
            ((System.ComponentModel.ISupportInitialize)(this.txtEncryptionKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSetKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private RadTextBox txtEncryptionKey;
        private RadButton btnSetKey;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.Themes.MaterialBlueGreyTheme materialBlueGreyTheme1;
        private Telerik.WinControls.Themes.MaterialPinkTheme materialPinkTheme1;
        private Telerik.WinControls.Themes.MaterialTealTheme materialTealTheme1;
        private Telerik.WinControls.Themes.MaterialTheme materialTheme1;
    }
}