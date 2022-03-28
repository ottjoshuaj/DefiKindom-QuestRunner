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
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnImport = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMnemonicWords = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
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
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Wallet Name:";
            // 
            // btnImport
            // 
            this.btnImport.Location = new System.Drawing.Point(219, 252);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 2;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Wallet Mnemonic Words:";
            // 
            // txtMnemonicWords
            // 
            this.txtMnemonicWords.Location = new System.Drawing.Point(12, 80);
            this.txtMnemonicWords.Multiline = true;
            this.txtMnemonicWords.Name = "txtMnemonicWords";
            this.txtMnemonicWords.Size = new System.Drawing.Size(282, 78);
            this.txtMnemonicWords.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 165);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(281, 84);
            this.label2.TabIndex = 7;
            this.label2.Text = "Note: If wallet has multiple accounts under it then they will all be imported. Th" +
    "ey will take on the provided name along with a number value after it. ";
            // 
            // frmImportWallet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 287);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMnemonicWords);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmImportWallet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import an existing wallet";
            this.Load += new System.EventHandler(this.frmImportWallet_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMnemonicWords;
        private System.Windows.Forms.Label label2;
    }
}