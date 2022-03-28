using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DefiKindom_QuestRunner.Managers;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmManageAllWallets : Form
    {
        public frmManageAllWallets()
        {
            InitializeComponent();
        }

        private void frmManageAllWallets_Load(object sender, EventArgs e)
        {
            foreach (var wallet in WalletManager.GetWallets())
            {
                gridWallets.Rows.Add(wallet.Enabled, wallet.IsPrimarySourceWallet, wallet.Name, wallet.Address, wallet.DfkProfile != null, wallet.AssignedHero, wallet.AssignedHeroStamina);
            }
        }
    }
}
