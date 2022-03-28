using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using PubSub;

using DefiKindom_QuestRunner.Dialogs;
using DefiKindom_QuestRunner.Helpers;
using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;


namespace DefiKindom_QuestRunner
{
    public partial class frmConsole : Form
    {
        #region Internals

        Hub eventHub = Hub.Default;

        #endregion

        #region Constructor

        public frmConsole()
        {
            InitializeComponent();

            LoadEventSubscriptions();

            LoadUXControls();
            HookControlEvents();
        }

        #endregion

        #region Form Events

        private void Form1_Load(object sender, EventArgs e)
        {
            //Make sure a encryption key is set!
            if (Settings.Default.EncryptionKey.Length == 0)
                new frmSetApplicationEncryptionKey().ShowDialog(this);

            addConsoleMsg("Initializing Wallets & Connecting to blockchain...");

            //Init app wallet manager
            WalletManager.Init();
            var wallets = WalletManager.GetWallets();


            addConsoleMsg("Wallets have been initialized!");

            toolStripWalletCount.Text = $@"Total Wallets: {wallets.Count} / Total Signable Wallets: {wallets.Count(x=>x.WalletAccount != null)}";
        }

        #endregion

        #region Menu Items

        private void mnuCreateRandomWallets_Click(object sender, EventArgs e)
        {

        }

        private void mnuImportExistingWallet_Click(object sender, EventArgs e)
        {
            new frmImportWallet().ShowDialog(this);
        }

        private void mnuManageAllWallets_Click(object sender, EventArgs e)
        {
            new frmManageAllWallets().ShowDialog(this);
        }

        private void mnuImportWalletDataFile_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                AddExtension = true,
                DefaultExt = "dfk,json",
                Filter = @"dfkqr files (*.dfk)|*.dfk|json files (*.json)|*.json",
                InitialDirectory = $"{Application.StartupPath}\\Data",
                Title = @"Browse DFKQR+ Files"
            };

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var fileToImport = openDialog.FileName;
                if (fileToImport != null)
                {
                    var dfm = new DataFileManager();
                    var fileWalletObjects = dfm.LoadDataFile<DfkWallet[]>(DataFileManager.DataFileTypes.Wallet, fileToImport);
                    if (fileWalletObjects != null)
                    {
                        var savedSuccess = dfm.SaveDataFile(DataFileManager.DataFileTypes.Wallet, fileWalletObjects);
                        MessageBox.Show(savedSuccess ? "Wallets Imported Successfully!" : "Wallets Failed To Import!");
                    }
                }
            }
        }

        #endregion

        #region Console Output

        public void addConsoleMsg(string text, bool isError = false)
        {
            txtStatusConsole.SuspendLayout();
            
            txtStatusConsole.SelectionColor = Color.Blue;
            txtStatusConsole.AppendText($"[{DateTime.Now.ToShortDateString()}:{DateTime.Now.ToShortTimeString()}]");
            txtStatusConsole.ScrollToCaret();
            txtStatusConsole.SelectionColor = isError ? Color.Red : Color.Black;
            txtStatusConsole.AppendText($" => {text}{Environment.NewLine}");
            txtStatusConsole.ScrollToCaret();

            txtStatusConsole.ResumeLayout();
        }

        #endregion

        #region Helper Methods



        #endregion

        #region Control Data Population

        void LoadUXControls()
        {
            //In the future we will populate this stuff on demand
            //Build RPC Info
            cmbRpcUrls.Items.Add("https://api.harmony.one");
            cmbRpcUrls.Items.Add("https://s1.api.harmony.one");
            cmbRpcUrls.Items.Add("https://s2.api.harmony.one");
            cmbRpcUrls.Items.Add("https://s3.api.harmony.one");
            cmbRpcUrls.Items.Add("https,://rpc.hermesdefi.io");

            if (string.IsNullOrWhiteSpace(Settings.Default.CurrentRpcUrl))
            {
                Settings.Default.CurrentRpcUrl = cmbRpcUrls.Items[0].ToString();
                Settings.Default.Save();
            }
        }

        #endregion

        #region Control Event Handlers

        void HookControlEvents()
        {
            txtStatusConsole.KeyPress += (sender, args) =>
            {
                args.Handled = true;
            };

            cmbRpcUrls.SelectedIndexChanged += (sender, args) =>
            {
                Settings.Default.CurrentRpcUrl = cmbRpcUrls.SelectedText;
                Settings.Default.Save();
            };
        }

        #endregion

        #region Event Subscription Handler

        private void LoadEventSubscriptions()
        {
            this.eventHub.Subscribe<MessageEvent>(this, msgEvent =>
            {
                if(!string.IsNullOrWhiteSpace(msgEvent.Content))
                    addConsoleMsg(msgEvent.Content);
            });
        }

        #endregion
    }
}
