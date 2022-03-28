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
            this.txtEncryptionKey = new System.Windows.Forms.TextBox();
            this.btnSetKey = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
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
            this.txtEncryptionKey.Size = new System.Drawing.Size(320, 38);
            this.txtEncryptionKey.TabIndex = 1;
            // 
            // btnSetKey
            // 
            this.btnSetKey.Location = new System.Drawing.Point(222, 155);
            this.btnSetKey.Name = "btnSetKey";
            this.btnSetKey.Size = new System.Drawing.Size(110, 23);
            this.btnSetKey.TabIndex = 2;
            this.btnSetKey.Text = "Set Key";
            this.btnSetKey.UseVisualStyleBackColor = true;
            this.btnSetKey.Click += new System.EventHandler(this.btnSetKey_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(316, 35);
            this.label2.TabIndex = 3;
            this.label2.Text = "Note: Save this value somewhere!  You will need to re-apply this value if you eve" +
    "r reinstall this application!";
            // 
            // frmSetApplicationEncryptionKey
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 190);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSetKey);
            this.Controls.Add(this.txtEncryptionKey);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmSetApplicationEncryptionKey";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Application Encryption Key Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtEncryptionKey;
        private System.Windows.Forms.Button btnSetKey;
        private System.Windows.Forms.Label label2;
    }
}