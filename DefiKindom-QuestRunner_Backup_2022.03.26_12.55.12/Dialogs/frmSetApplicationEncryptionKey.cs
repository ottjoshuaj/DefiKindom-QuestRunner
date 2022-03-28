using System;
using System.Windows.Forms;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmSetApplicationEncryptionKey : Form
    {
        public frmSetApplicationEncryptionKey()
        {
            InitializeComponent();
        }

        private void btnSetKey_Click(object sender, EventArgs e)
        {
            if (txtEncryptionKey.Text.Trim().Length < 4 || txtEncryptionKey.Text.Trim().Length > 4)
            {
                MessageBox.Show(@"Your key HAS TO BE 4 characters long. No less and no more!", @"Invalid Key");
            }
            else
            {
                Properties.Settings.Default.EncryptionKey = txtEncryptionKey.Text;
                Properties.Settings.Default.Save();

                Close();
            }
        }
    }
}
