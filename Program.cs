using System;
using System.Diagnostics;
using System.Windows.Forms;

using Telerik.WinControls;

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
            if(Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                RadMessageBox.Show("Application is already running. Only one instance of this application is allowed. Exiting...", "Application Already Running");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmConsole());
        }
    }
}
