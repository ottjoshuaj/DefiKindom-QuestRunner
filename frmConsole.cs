using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Telerik.WinControls;
using Telerik.WinControls.UI;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;

using PubSub;

using DefiKindom_QuestRunner.Dialogs;
using DefiKindom_QuestRunner.EngineManagers;
using DefiKindom_QuestRunner.EngineManagers.Engines;
using DefiKindom_QuestRunner.Helpers;
using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner
{
    public partial class frmConsole : RadForm
    {
        #region RadMenu Internals

        private RadMenuItem mnuAutoStartQuestingOnStartup;
        private RadMenuItem mnuStartQuestEngine;
        private RadMenuItem mnuStopQuestEngine;

        #endregion

        #region Internals

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private decimal _startingJewelAmount = 0;
        private frmManageAllWallets manageAllWalletsFormInstance;
        Hub eventHub = Hub.Default;

        #endregion

        #region Delegates

        private delegate void AddConsoleMsgDelegate(string text, bool isError = false);
        private delegate void UpdateWalletCountToolStripLabelDelegate(string text);
        private delegate void SetWalletsOnQuestToolStripLabelDelegate(string text);
        private delegate void ShowDesktopAlertDelegate(NotificationEvent noticeEvent);
        private delegate void EnableUxControlsDelegate(bool enabled);
        private delegate void EnableDisableQuestStartMenusDelegate(bool startInstanceEnabled, bool stopInstanceEnabled);
        private delegate void HandleJewelProfitUxUpdatesDelegate(JewelInformation jewelInfo);

        #endregion

        #region Constructor

        public frmConsole()
        {
            InitializeComponent();

            //Add Menu Items
            BuildMainMenu();

            //Setup Form Events
            Shown += OnShown;
            Resize += OnResize;

            ThemeResolutionService.ApplicationThemeName = "Material";

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

            if (string.IsNullOrWhiteSpace(Settings.Default.CurrentRpcUrl))
            {
                Settings.Default.CurrentRpcUrl = "https://api.harmony.one";
                Settings.Default.Save();
            }

            try
            {
                XmlConfigurator.Configure();
                var appender = (LogManager.GetRepository() as Hierarchy).Root.Appenders
                    .OfType<FileAppender>()
                    .First();

                var logFileName = $"LogFile-{DateTime.Now.ToShortDateString().Replace("/", "-")}-{DateTime.Now.Hour}.txt";

                appender.File = $"{Application.StartupPath}\\Logs\\{logFileName}";
                appender.ActivateOptions();
            }
            catch (Exception ex)
            {

            }
        }

        private async void OnShown(object sender, EventArgs e)
        {
            await Task.Run(HandleWalletIndex);
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && Settings.Default.MinimizeToTray)
                Hide();
        }

        #endregion

        #region Menu Items

        void BuildMainMenu()
        {
            var mnuViewLogs = new RadMenuItem();
            mnuViewLogs.Text = @"View Logs Folder";

            mnuFile.Items.AddRange(mnuViewLogs, new RadMenuSeparatorItem(), mnuPreferences);

            var mnuImport = new RadMenuItem();
            mnuImport.Text = @"Import";
            mnuImport.Items.AddRange(mnuImportExistingWallet, mnuImportWalletDataFile);

            mnuWallets.Items.AddRange(mnuImport,
                mnuExportWalletDataFile, radMenuSeparatorItem2, mnuManageAllWallets, radMenuSeparatorItem3, mnuRunWalletInit);


            mnuStartQuestEngine = new RadMenuItem();
            mnuStartQuestEngine.Text = @"Start Quest Engine";
            mnuStartQuestEngine.Click += mnuStartQuestEngine_Click;
            mnuActions.Items.AddRange(mnuStartQuestEngine);

            mnuActions.Items.AddRange(new RadMenuSeparatorItem());

            mnuStopQuestEngine = new RadMenuItem();
            mnuStopQuestEngine.Text = @"Stop Quest Engine";
            mnuStopQuestEngine.Click += mnuStopQuestEngine_Click;
            mnuActions.Items.AddRange(mnuStopQuestEngine);


            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;

            /*
            mnuAutoStartQuestingOnStartup = new RadMenuItem();
            mnuAutoStartQuestingOnStartup.Text = @"Start Quest Engine on Startup";
            mnuAutoStartQuestingOnStartup.CheckOnClick = true;
            mnuAutoStartQuestingOnStartup.IsChecked = false;
            mnuAutoStartQuestingOnStartup.Click += mnuStartQuestEngineOnStartup_Click;
            mnuActions.Items.AddRange(mnuAutoStartQuestingOnStartup);
            */

            mnuPreferences.Click += mnuPreferences_Click;
            mnuViewLogs.Click += mnuViewLogs_Click;
        }

        private void mnuViewLogs_Click(object sender, EventArgs e)
        {
            Process.Start($"{Application.StartupPath}\\Logs");
        }

        private async void mnuStartQuestEngine_Click(object sender, EventArgs e)
        {
            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;

            AddConsoleMessage("Re-Indexing Wallets...");

            AddConsoleMessage("Re-Indexing Complete!");

            AddConsoleMessage("Loading Quest Instances for questing wallets...");

            var instancesStarted = await WalletManager.OnBoardQuestInstances();

            AddConsoleMessage($"({instancesStarted}) Quest Instances instantiated...");

            mnuStopQuestEngine.Enabled = true;
        }

        private async void mnuStopQuestEngine_Click(object sender, EventArgs e)
        {
            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;

            AddConsoleMessage("Stopping All Quest Instances...");

            await WalletManager.OnBoardQuestInstances();

            AddConsoleMessage($"Quest Instances stopped...");

            mnuStartQuestEngine.Enabled = true;
        }

        private void mnuStartQuestEngineOnStartup_Click(object sender, EventArgs e)
        {
            //Settings.Default.AutoStartQuestingOnStartup = mnuAutoStartQuestingOnStartup.IsChecked;
            //Settings.Default.Save();
        }

        private void mnuPreferences_Click(object sender, EventArgs e)
        {
            new frmPreferences().ShowDialog(this);
        }

        private void mnuImportExistingWallet_Click(object sender, EventArgs e)
        {
            new frmImportWallet().ShowDialog(this);
        }

        private void mnuManageAllWallets_Click(object sender, EventArgs e)
        {
            if (manageAllWalletsFormInstance == null)
            {
                manageAllWalletsFormInstance = new frmManageAllWallets();
                manageAllWalletsFormInstance.Closed += (o, args) =>
                {
                    manageAllWalletsFormInstance.Dispose();
                    manageAllWalletsFormInstance = null;
                };
                manageAllWalletsFormInstance.Show();
            }

            if (manageAllWalletsFormInstance.WindowState == FormWindowState.Minimized)
                manageAllWalletsFormInstance.WindowState = FormWindowState.Normal;

            manageAllWalletsFormInstance.BringToFront();
        }

        private async void mnuImportWalletDataFile_Click(object sender, EventArgs e)
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
                    var fileWalletObjects =
                        dfm.LoadDataFile<DfkWallet[]>(DataFileManager.DataFileTypes.Wallet, fileToImport);
                    if (fileWalletObjects != null)
                    {
                        var savedSuccess = dfm.SaveDataFile(DataFileManager.DataFileTypes.Wallet, fileWalletObjects);

                        RadMessageBox.Show(savedSuccess ? "Wallets Imported Successfully!" : "Wallets Failed To Import!");

                        //Init app with new import wallets
                        await Task.Run(HandleWalletIndex);
                    }
                }
            }
        }

        private async void mnuRunWalletInit_Click(object sender, EventArgs e)
        {
            EnableUxControls(false);

            //Init app wallet manager
            await WalletManager.Init();
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog(this);
        }

        private void mnuStartStopMining_Click(object sender, EventArgs e)
        {
            //mnuStartStopMining.Enabled = false;

            //StartQuestInstances();

            //mnuStartStopMining.Enabled = false;
        }

        #endregion

        #region Console & Toolstrip Control Updates

        void EnableUxControls(bool enabled)
        {
            if (InvokeRequired)
                Invoke(new EnableUxControlsDelegate(EnableUxControls), enabled);
            else
            {
                mnuMainMenu.SuspendLayout();
                mnuMainMenu.Enabled = enabled;
                mnuMainMenu.ResumeLayout(true);

                toolBar.Enabled = enabled;
            }
        }

        void AddConsoleMessage(string text, bool isError = false)
        {
            if (InvokeRequired)
                Invoke(new AddConsoleMsgDelegate(AddConsoleMessage), text, isError);
            else
            {
                txtStatusConsole.SuspendLayout();

                if (txtStatusConsole.Lines.Length > 5000)
                    txtStatusConsole.Text = "";

                txtStatusConsole.SelectionColor = Color.Blue;
                txtStatusConsole.AppendText($"[{DateTime.Now.ToShortDateString()}:{DateTime.Now.ToShortTimeString()}]");
                txtStatusConsole.ScrollToCaret();
                txtStatusConsole.SelectionColor = isError ? Color.Red : Color.Black;
                txtStatusConsole.AppendText($" => {text}{Environment.NewLine}");

                txtStatusConsole.ScrollToCaret();

                txtStatusConsole.ResumeLayout();

                WriteToLogFile(text);
            }
        }

        void SetWalletCountToolStripLabel(string text)
        {
            if (InvokeRequired)
                Invoke(new UpdateWalletCountToolStripLabelDelegate(SetWalletCountToolStripLabel), text);
            else
                toolStripWalletCount.Text = text;
        }

        void SetWalletsOnQuestToolStripLabel(string text)
        {
            if (InvokeRequired)
                Invoke(new SetWalletsOnQuestToolStripLabelDelegate(SetWalletCountToolStripLabel), text);
            else
                toolStripWalletsQuesting.Text = text;
        }

        void ShowDesktopAlert(NotificationEvent noticeEvent)
        {
            if (InvokeRequired)
                Invoke(new ShowDesktopAlertDelegate(ShowDesktopAlert), noticeEvent);
            else
            {
                desktopAlert.CaptionText = noticeEvent.IsError ? "ERROR" : "INFO";
                desktopAlert.ContentText = noticeEvent.Content;
                desktopAlert.Show();
            }
        }

        #endregion

        #region Control Data Population

        void LoadUXControls()
        {
            //In the future we will populate this stuff on demand
            //Build RPC Info
            cmbRpcUrls.ComboBoxElement.Items.Add(new RadListDataItem("https://api.harmony.one"));
            cmbRpcUrls.ComboBoxElement.Items.Add(new RadListDataItem("https://s1.api.harmony.one"));
            cmbRpcUrls.ComboBoxElement.Items.Add(new RadListDataItem("https://s2.api.harmony.one"));
            cmbRpcUrls.ComboBoxElement.Items.Add(new RadListDataItem("https://s3.api.harmony.one"));
            cmbRpcUrls.ComboBoxElement.Items.Add(new RadListDataItem("https,://rpc.hermesdefi.io"));

            if (string.IsNullOrWhiteSpace(Settings.Default.CurrentRpcUrl))
            {
                cmbRpcUrls.ComboBoxElement.SelectedIndex = 0;

                Settings.Default.CurrentRpcUrl = Settings.Default.CurrentRpcUrl;
                Settings.Default.Save();
            }
            else
            {
                cmbRpcUrls.ComboBoxElement.SelectedText = Settings.Default.CurrentRpcUrl;
            }
        }

        #endregion

        #region Control Event Handlers

        void HookControlEvents()
        {
            Closing += (sender, args) =>
            {
                WalletManager.SaveWallets();
            };

            txtStatusConsole.KeyPress += (sender, args) => { args.Handled = true; };

            cmbRpcUrls.ComboBoxElement.SelectedIndexChanged += (sender, args) =>
            {
                Settings.Default.CurrentRpcUrl = cmbRpcUrls.ComboBoxElement.SelectedText;
                Settings.Default.Save();
            };
        }

        #endregion

        #region Event Subscription Handler

        private void LoadEventSubscriptions()
        {
            eventHub.Subscribe<MessageEvent>(this, msgEvent =>
            {
                if (!string.IsNullOrWhiteSpace(msgEvent.Content))
                    AddConsoleMessage(msgEvent.Content);
            });

            eventHub.Subscribe<WalletDataRefreshEvent>(this, wdRefreshEvent =>
            {
                if (wdRefreshEvent.Complete)
                {
                    EnableUxControls(true);
                }
            });

            eventHub.Subscribe<NotificationEvent>(this, ShowDesktopAlert);


            eventHub.Subscribe<WalletsOnQuestsMessageEvent>(this, msgEvent =>
            {
                switch (msgEvent.OnQuestMessageEventType)
                {
                    case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing:
                        AddConsoleMessage($"[Wallet {msgEvent.Wallet.Name} => {msgEvent.Wallet.Address}] => Started Questing....");
                        break;

                    case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingCanceled:
                        AddConsoleMessage($"[Wallet {msgEvent.Wallet.Name} => {msgEvent.Wallet.Address}] => Stopped Questing....");
                        break;
                }
            });


            eventHub.Subscribe<JewelInformationEvent>(this, jewelInfoEvent =>
            {
                if (jewelInfoEvent.JewelInformation != null)
                {
                    //jewelInfoEvent.JewelInformation
                    AddConsoleMessage($"[Current Jewel Holder:{jewelInfoEvent.JewelInformation.Holder.Address}] => Currently Holds Your Jewels in da sack!");

                    HandleJewelProfitUxUpdates(jewelInfoEvent.JewelInformation);
                }
            });
        }

        #endregion

        #region Quest Instance Methods

        void StartQuestInstances()
        {
            foreach (var wallet in WalletManager.GetWallets().Where(wallet => wallet.ReadyToWork))
            {
                QuestEngineManager.AddQuestEngine(new QuestEngine(wallet, QuestEngine.QuestTypes.Mining));
            }
        }

        void StopQuestInstances()
        {
            QuestEngineManager.KillAllInstances();
        }

        #endregion

        #region Notification Icon Handler(s)

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        #endregion

        #region Quest Instance Start/Stop Handler(s)

        void HandleQuestStartStopEnabled(bool startInstanceEnabled, bool stopInstanceEnabled)
        {
            if (InvokeRequired)
                Invoke(new EnableDisableQuestStartMenusDelegate(HandleQuestStartStopEnabled), startInstanceEnabled, stopInstanceEnabled);
            else
            {
                mnuStartQuestEngine.Enabled = startInstanceEnabled;
                mnuStopQuestEngine.Enabled = stopInstanceEnabled;
            }
        }

        #endregion

        #region Jewel Profit Handler

        void HandleJewelProfitUxUpdates(JewelInformation jewelInfo)
        {
            if (InvokeRequired)
                Invoke(new HandleJewelProfitUxUpdatesDelegate(HandleJewelProfitUxUpdates), jewelInfo);
            else
            {
                //Lets tally up old jewel amount to new
                if (_startingJewelAmount == 0)
                {
                    _startingJewelAmount = jewelInfo.Balance;

                    var formatTotalJewel = $"{_startingJewelAmount:0.####}";

                    toolStripJewelAmount.Text = $@"Jewel Earned (0/{formatTotalJewel})";
                }
                else
                {
                    var formatTotalJewel = $"{_startingJewelAmount:0.####}";

                    var jewelProfit = (jewelInfo.Balance - _startingJewelAmount);
                    toolStripJewelAmount.Text = jewelProfit == 0 ? $@"Jewel Earned (0/{_startingJewelAmount}" : $@"Jewel Earned ({jewelProfit}/{formatTotalJewel})";
                }
            }
        }

        #endregion

        #region Wallet Index/Inital Loading Handler

        async void HandleWalletIndex()
        {
            HandleQuestStartStopEnabled(false, false);

            if (Settings.Default.FirstInit)
            {
                //Do a FULL Index
                EnableUxControls(false);

                AddConsoleMessage("Initializing Wallets & Connecting to the Block-Chain...");

                await WalletManager.Init();

                AddConsoleMessage("Wallets have been initialized!");

                SetWalletCountToolStripLabel($@"Total Wallets: {WalletManager.GetWallets().Count}");

                EnableUxControls(true);

                AddConsoleMessage("Starting the jewel monitoring system...");

                await JewelManager.Init();

                AddConsoleMessage("Jewel monitoring system started");

                EnableUxControls(true);


                Settings.Default.FirstInit = false;
                Settings.Default.LastWalletIndex = DateTime.Now;
                Settings.Default.Save();

                /*
                if (Settings.Default.AutoStartQuestingOnStartup)
                {
                    AddConsoleMessage("Loading Quest Instances for questing wallets...");

                    var instancesStarted = await WalletManager.OnBoardQuestInstances();

                    AddConsoleMessage($"({instancesStarted}) Quest Instances instantiated...");
                }
                */
            }
            else
            {
                var quickIndex = true;

                //Do A quick Index OR a full index if its been longer 
                var timeSinceLastIndex = DateTime.Now.Subtract(Settings.Default.LastWalletIndex);
                if (timeSinceLastIndex.TotalMinutes > 60)
                {
                    Settings.Default.LastWalletIndex = DateTime.Now;
                    Settings.Default.Save();

                    quickIndex = false;
                }



                //Do a FULL Index
                EnableUxControls(false);

                AddConsoleMessage("Initializing Wallets & Connecting to the Block-Chain...");

                await WalletManager.Init(quickIndex);

                AddConsoleMessage("Wallets have been initialized!");

                SetWalletCountToolStripLabel($@"Total Wallets: {WalletManager.GetWallets().Count}");

                EnableUxControls(true);

                AddConsoleMessage("Starting the jewel monitoring system...");

                await JewelManager.Init();

                AddConsoleMessage("Jewel monitoring system started");

                EnableUxControls(true);


                /*
                if (Settings.Default.AutoStartQuestingOnStartup)
                {
                    AddConsoleMessage("Loading Quest Instances for questing wallets...");

                    var instancesStarted = await WalletManager.OnBoardQuestInstances();

                    AddConsoleMessage($"({instancesStarted}) Quest Instances instantiated...");
                }
                */
            }


            if (QuestEngineManager.Count > 0)
                HandleQuestStartStopEnabled(false, true);
            else
                HandleQuestStartStopEnabled(true, false);

        }

        #endregion

        #region Logs Handler

        async void WriteToLogFile(string msg)
        {
            //Logger.Info(msg);
            await Task.Run(() => Logger.Info(msg));
        }

        #endregion
    }
}
