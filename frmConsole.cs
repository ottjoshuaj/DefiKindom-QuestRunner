using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
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
using Telerik.Charting;

namespace DefiKindom_QuestRunner
{
    public partial class frmConsole : RadForm
    {
        #region Chart Internals

        private List<WalletsOnQuestsMessageEvent> _walletsInQuestInstance = new List<WalletsOnQuestsMessageEvent>();

        #endregion

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
        private delegate void ShowDesktopAlertDelegate(NotificationEvent noticeEvent);
        private delegate void EnableUxControlsDelegate(bool enabled);
        private delegate void EnableDisableQuestStartMenusDelegate(bool startInstanceEnabled, bool stopInstanceEnabled);
        private delegate void HandleJewelProfitUxUpdatesDelegate(JewelInformation jewelInfo);
        private delegate void HandleWalletsOnQuestsMessageEventDelegate(WalletsOnQuestsMessageEvent msgEvent);

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
            Closing += OnClosing;

            ThemeResolutionService.ApplicationThemeName = "Material";

            LoadEventSubscriptions();

            LoadUXControls();
            HookControlEvents();
            HookUpStatCharts();
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

            //Make sure NodeJs server is running
            await Task.Run(CheckForServer);
        }

        private void OnResize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && Settings.Default.MinimizeToTray)
                Hide();
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;

            AddConsoleMessage("Stopping All Quest Instances...");

            QuestEngineManager.KillAllInstances();

            AddConsoleMessage($"Quest Instances stopped...");
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



            mnuExportWalletDataFile.Click += MnuExportWalletDataFileOnClick;

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
            if (!WalletManager.GetWallets().Any(wallet => wallet.AssignedHero > 0 && wallet.HasDkProfile))
            {
                RadMessageBox.Show(this,
                    "There are not any wallets capable of questing!\r\nMake sure they have ONE, has a hero and is onboarded to DFK.",
                    "  No Questable Wallets Found");
                return;
            }

            mnuImportWalletDataFile.Enabled = false;
            mnuImportExistingWallet.Enabled = false;
            
            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;

            AddConsoleMessage("Loading Quest Instances for questing wallets...");

            var instancesStarted = await WalletManager.OnBoardQuestInstances();

            AddConsoleMessage($"({instancesStarted}) Quest Instances instantiated...");

            mnuStopQuestEngine.Enabled = true;
        }

        private void mnuStopQuestEngine_Click(object sender, EventArgs e)
        {
            if (QuestEngineManager.Count == 0)
            {
                //No running quests. No sense in attempting to stop any
                RadMessageBox.Show(this,
                    "There are currently running quest instances running.",
                    "  Warning");
                return;
            }

            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;

            AddConsoleMessage("Stopping All Quest Instances...");

            QuestEngineManager.KillAllInstances();

            AddConsoleMessage($"Quest Instances stopped...");

            mnuStartQuestEngine.Enabled = true;
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
                Filter = @"json files (*.json)|*.json",
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

        private void MnuExportWalletDataFileOnClick(object sender, EventArgs e)
        {
            try
            {
                var exportWalletDataFile = new SaveFileDialog();
                exportWalletDataFile.DefaultExt = "dfk,json";
                exportWalletDataFile.Filter = @"json files (*.json)|*.json";
                exportWalletDataFile.Title = @"Save Wallet Data File As";

                var exportResponse = exportWalletDataFile.ShowDialog(this);
                if (exportResponse == DialogResult.OK && !string.IsNullOrWhiteSpace(exportWalletDataFile.FileName))
                {
                    //Grab contents of current wallets data file
                    WalletManager.SaveWallets();

                    var dataManager = new DataFileManager();
                    var wallets = dataManager.LoadDataFile<List<DfkWallet>>(DataFileManager.DataFileTypes.Wallet);
                    var walletContents = dataManager.Serialize(wallets);

                    File.WriteAllText(exportWalletDataFile.FileName, walletContents);

                    RadMessageBox.Show(this, $"Wallet Data File Exported To:\r\n{exportWalletDataFile.FileName}", "Data File Export");
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(this, $"Wallet Data File Export Error:\r\n{ex.Message}", "Data File Export Error");
            }
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

                UpdateChartInformation(msgEvent);
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
            }
            else
            {
                var quickIndex = true;

                //Do A quick Index OR a full index if its been longer 
                var timeSinceLastIndex = DateTime.Now.Subtract(Settings.Default.LastWalletIndex);
                if (timeSinceLastIndex.TotalMinutes > 120)
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
            }


            if (QuestEngineManager.Count > 0)
                HandleQuestStartStopEnabled(false, true);
            else
                HandleQuestStartStopEnabled(true, false);

            //Set initial chart
            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[1].SetValue(0, Convert.ToDouble(WalletManager.GetWallets().Count));
        }

        #endregion

        #region Logs Handler

        async void WriteToLogFile(string msg)
        {
            //Logger.Info(msg);
            await Task.Run(() => Logger.Info(msg));
        }

        #endregion

        #region Chart Management

        void UpdateChartInformation(WalletsOnQuestsMessageEvent msgEvent)
        {
            if(InvokeRequired)
                Invoke(new HandleWalletsOnQuestsMessageEventDelegate(UpdateChartInformation), msgEvent);
            else
            {
                try
                {
                    double? currentOfflineValue;
                    double? currentOnlineValue;

                    switch (msgEvent.OnQuestMessageEventType)
                    {
                        case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStarting:
                            //Get current "online
                            currentOnlineValue =
                                (double)chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[0].GetValue(0);
                            currentOnlineValue = currentOnlineValue + 1;

                            //instance starting. Take it from "Idle" to "online"
                            currentOfflineValue =
                                (double)chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[1].GetValue(0);
                            currentOfflineValue = currentOfflineValue - 1;

                            //Set New Values
                            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[0].SetValue(0, Convert.ToDouble(currentOnlineValue));
                            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[1].SetValue(1, Convert.ToDouble(currentOfflineValue));
                            break;

                        case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStopping:
                            //Get current "online
                            currentOnlineValue =
                                (double)chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[0].GetValue(0);
                            currentOnlineValue = currentOnlineValue - 1;

                            currentOfflineValue =
                                (double)chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[1].GetValue(0);
                            currentOfflineValue = currentOfflineValue + 1;

                            //Set New Values
                            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[0].SetValue(0, Convert.ToDouble(currentOnlineValue));
                            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[1].SetValue(0, Convert.ToDouble(currentOfflineValue));
                            break;
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        void HookUpStatCharts()
        {
            chartBotsOnline.AreaType = ChartAreaType.Pie;
            var pieSeriesBotsOnline = new PieSeries();
            pieSeriesBotsOnline.DataPoints.Add(new PieDataPoint(0, "Online"));
            pieSeriesBotsOnline.DataPoints.Add(new PieDataPoint(0, "Offline"));
            pieSeriesBotsOnline.ShowLabels = true;
            pieSeriesBotsOnline.LabelMode = PieLabelModes.Horizontal;
            chartBotsOnline.LegendTitle = "Bots Online";
            chartBotsOnline.ShowLegend = true;
            chartBotsOnline.Series.Add(pieSeriesBotsOnline);


            chartHeroesQuesting.AreaType = ChartAreaType.Pie;
            var pieSeriesQuestInstanceStatus = new PieSeries();
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Waiting On Stamina"));
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Questing"));
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Waiting To Cancel Quest"));
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Waiting On Start Quest"));
            pieSeriesQuestInstanceStatus.ShowLabels = true;
            pieSeriesQuestInstanceStatus.LabelMode = PieLabelModes.Horizontal;
            chartHeroesQuesting.LegendTitle = "Quest Instance Activity";
            chartHeroesQuesting.ShowLegend = true;
            chartHeroesQuesting.Series.Add(pieSeriesQuestInstanceStatus);
            chartHeroesQuesting.Visible = false; //TOdO REMOVE ME
        }

        #endregion

        #region NodeJs Server Handlers

        private async void CheckForServer()
        {
            try
            {
                var response =
                    await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>(QuickRequest.ApiRequestTypes
                        .TestEndpoint);
                if (!response.Success)
                {
                    RadMessageBox.Show(this, response.Error.ToString(),
                        "Server Down!");

                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                RadMessageBox.Show(this, "DFKQR+ Server is down. Application can not run unless it is up!",
                    "Server Down!");

                Application.Exit();
            }
        }

        #endregion
    }
}
