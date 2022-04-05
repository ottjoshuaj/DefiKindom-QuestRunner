using System;
using System.Windows.Forms;

namespace DefiKindom_QuestRunner
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmConsole());
        }
    }
}
//https://github.com/0rtis/dfk -- Contracts
//https://github.com/harmony-one/chrome-extension-wallet/blob/master/readme/hrc20.md   (HRC/NFT's)