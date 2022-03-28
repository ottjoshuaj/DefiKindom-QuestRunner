namespace DefiKindom_QuestRunner.Dialogs
{
    partial class frmSendHeroesToWallets
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSendHeroesToWallets));
            this.btnSendHeroesToWallets = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.btnSendHeroesToWallets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSendHeroesToWallets
            // 
            this.btnSendHeroesToWallets.Location = new System.Drawing.Point(25, 208);
            this.btnSendHeroesToWallets.Name = "btnSendHeroesToWallets";
            this.btnSendHeroesToWallets.Size = new System.Drawing.Size(290, 40);
            this.btnSendHeroesToWallets.TabIndex = 4;
            this.btnSendHeroesToWallets.Text = "Send Heroes";
            this.btnSendHeroesToWallets.Click += new System.EventHandler(this.btnSendHeroesToWallets_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.AutoSize = false;
            this.radLabel1.Location = new System.Drawing.Point(25, 45);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(300, 115);
            this.radLabel1.TabIndex = 3;
            this.radLabel1.Text = "Send heroes from SOURCE wallet to wallets that do NOT have an assigned hero.  (Wi" +
    "ll stop when the source wallet runs out of heroes)";
            this.radLabel1.TextAlignment = System.Drawing.ContentAlignment.TopLeft;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.radLabel1);
            this.panel1.Controls.Add(this.btnSendHeroesToWallets);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(345, 284);
            this.panel1.TabIndex = 5;
            // 
            // frmSendHeroesToWallets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 284);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSendHeroesToWallets";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Send Heroes To Wallets";
            this.Load += new System.EventHandler(this.frmSendHeroesToWallets_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnSendHeroesToWallets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnSendHeroesToWallets;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.Panel panel1;
    }
}