﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

using Telerik.WinControls;
using Telerik.WinControls.UI;
using Telerik.Charting;

using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;

using PubSub;

using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using DefiKindom_QuestRunner.Dialogs;
using DefiKindom_QuestRunner.Discord;
using DefiKindom_QuestRunner.Helpers;
using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Managers.Engines;
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

        private RadMenuItem mnuManageWallets;
        private RadMenuItem mnuSendOneAndOnboardToDfk;
        private RadMenuItem mnuOnboardToDfk;
        private RadMenuItem mnuSendHeroesToWallets;
        private RadMenuItem mnuRecallHerosToSourceWallet;
        private RadMenuItem mnuGenerateNewWallets;


        #endregion

        #region Internals

        private readonly DiscordBot discordBot = new DiscordBot();
        readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly Hub _eventHub = Hub.Default;

        private DataTable _tableQuestInstances;
        private System.Timers.Timer _timerJewelTotalCheck;

        private decimal _startingJewelAmount = 0;
        private decimal _currentJewelProfit = 0;
        private decimal _currentJewelTotal = 0;

        private decimal _totalSentOutAlready = 0;

        //For charts
        double currentOfflineValue = 0;
        double currentOnlineValue = 0;

        #endregion

        #region Delegates

        private delegate void AddConsoleMsgDelegate(string text, bool isError = false);
        private delegate void UpdateWalletCountToolStripLabelDelegate(string text);
        private delegate void ShowDesktopAlertDelegate(NotificationEvent noticeEvent);
        private delegate void EnableUxControlsDelegate(bool enabled);
        private delegate void EnableDisableQuestStartMenusDelegate(bool startInstanceEnabled, bool stopInstanceEnabled);
        private delegate void HandleJewelProfitUxUpdatesDelegate(JewelInformation jewelInfo);
        private delegate void HandleWalletsOnQuestsMessageEventDelegate(WalletsOnQuestsMessageEvent msgEvent);
        private delegate void ShowRadAlertMessageBoxDelegate(string text, string title);
        private delegate void LoadDataToGridDelegate();
        private delegate void UpdateStatusLabelDelegate(string text);
        private delegate void WalletQuestStatusEventDelegate(WalletQuestStatusEvent evt);


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
            VisibleChanged += OnVisibleChanged;
            
            ThemeResolutionService.ApplicationThemeName = Settings.Default.UXTheme;

            LoadEventSubscriptions();

            LoadUXControls();
            HookControlEvents();
        }

        #endregion

        #region Form Events

        private async void Form1_Load(object sender, EventArgs e)
        {
            WalletManager.InitSubscription();

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

                var logFileName = $"LogFile-{DateTime.Now.ToShortDateString().Replace("/", "-")}.txt";

                appender.File = $"{Application.StartupPath}\\Logs\\{logFileName}";
                appender.ActivateOptions();
            }
            catch (Exception ex)
            {

            }

            await Task.Run(HandleWalletIndex);

            //Make sure NodeJs server is running
            await Task.Run(CheckForServer);

            LoadDataToGrid();
            HookUpStatCharts();

            //gridQuestInstances.VirtualMode = true;
            _tableQuestInstances = new DataTable("questInstances");
            _tableQuestInstances.Columns.Add("Id");
            _tableQuestInstances.Columns.Add("Name");
            _tableQuestInstances.Columns.Add("Address");
            _tableQuestInstances.Columns.Add("Mode");
            _tableQuestInstances.Columns.Add("Contract Address");
            _tableQuestInstances.Columns.Add("Hero Id");
            _tableQuestInstances.Columns.Add("HeroStamina");
            _tableQuestInstances.Columns.Add("Started At");
            _tableQuestInstances.Columns.Add("Completes At");

            gridQuestInstances.AutoGenerateColumns = true;
            gridQuestInstances.ReadOnly = true;
            gridQuestInstances.AllowSearchRow = true;
            gridQuestInstances.AllowMultiColumnSorting = false;
            gridQuestInstances.EnableSorting = true;
            gridQuestInstances.EnableGrouping = true;
            gridQuestInstances.ShowGroupPanel = true;
            gridQuestInstances.ShowGroupedColumns = true;
            gridQuestInstances.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
            gridQuestInstances.DataSource = _tableQuestInstances;
            gridQuestInstances.ShowCellErrors = false;

            //Setup grid column settings
            gridQuestInstances.Columns[0].MinWidth = 75;
            gridQuestInstances.Columns[1].MinWidth = 200;
            gridQuestInstances.Columns[2].MinWidth = 375;
            gridQuestInstances.Columns[3].MinWidth = 175;
            gridQuestInstances.Columns[4].MinWidth = 375;
            gridQuestInstances.Columns[5].MinWidth = 100;
            gridQuestInstances.Columns[6].MinWidth = 100;
            gridQuestInstances.Columns[7].MinWidth = 250;
            gridQuestInstances.Columns[8].MinWidth = 250;

            //Fix all columns alignments
            foreach (var col in gridQuestInstances.Columns)
            {
                col.HeaderTextAlignment = ContentAlignment.MiddleCenter;
                col.TextAlignment = ContentAlignment.MiddleCenter;
            }

            //Is quest instance running?
            if (QuestEngineManager.GetAllQuestInstances().Count > 0)
            {
                mnuGridActionSendJewelTo.Enabled = false;
            }

            //Setup Discord Bot Timer (sends info to discord channel)
            _timerJewelTotalCheck = new System.Timers.Timer(3600000);
            _timerJewelTotalCheck.Elapsed += TimerJewelTotalCheckOnElapsed;
            _timerJewelTotalCheck.Enabled = true;
            _timerJewelTotalCheck.Start();
        }

        private async void OnShown(object sender, EventArgs e)
        {
            await discordBot.Start();
        }

        private void OnResize(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Maximized:
                    break;

                case FormWindowState.Minimized:
                    if (Settings.Default.MinimizeToTray)
                        Hide();
                    break;

                case FormWindowState.Normal:
                    break;
            }
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;

            AddConsoleMessage("Stopping All Quest Instances...");

            QuestEngineManager.KillAllInstances();

            AddConsoleMessage($"Quest Instances stopped...");

            WalletManager.SaveWallets();
        }

        private void OnVisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                //Force scroll of console
                txtStatusConsole.ScrollToCaret();
            }
        }

        #endregion

        #region Menu Items

        void BuildMainMenu()
        {
            var mnuViewLogs = new RadMenuItem();
            mnuViewLogs.Text = @"View Logs Folder";

            mnuFile.Items.AddRange(mnuViewLogs, new RadMenuSeparatorItem(), mnuPreferences);
            
            // ACTIONS MENU
            mnuStartQuestEngine = new RadMenuItem();
            mnuStartQuestEngine.Text = @"Start Quest Engine";
            mnuStartQuestEngine.Click += mnuStartQuestEngine_Click;
            mnuActions.Items.AddRange(mnuStartQuestEngine);

            mnuActions.Items.AddRange(new RadMenuSeparatorItem());

            mnuStopQuestEngine = new RadMenuItem();
            mnuStopQuestEngine.Text = @"Stop Quest Engine";
            mnuStopQuestEngine.Click += mnuStopQuestEngine_Click;
            mnuActions.Items.AddRange(mnuStopQuestEngine);

            //Wallet Menu
            var mnuImport = new RadMenuItem();
            mnuImport.Text = @"Import";
            mnuImport.Items.AddRange(mnuImportExistingWallet, mnuImportWalletDataFile);

            //Manage Wallet Menu Items
            mnuManageWallets = new RadMenuItem();
            mnuManageWallets.Text = @"Manage";

            mnuSendOneAndOnboardToDfk = new RadMenuItem();
            mnuSendOneAndOnboardToDfk.Text = @"Send ONE To Wallets";
            mnuSendOneAndOnboardToDfk.Click += mnuSendOneAndOnboardToDfk_Click;

            mnuOnboardToDfk = new RadMenuItem();
            mnuOnboardToDfk.Text = @"Onboard Wallets To DFK (If NOT already)";
            mnuOnboardToDfk.Click += mnuOnboardToDfk_Click;


            mnuSendHeroesToWallets = new RadMenuItem();
            mnuSendHeroesToWallets.Text = @"Send Heroes To Wallets (Ones w/o Heroes)";
            mnuSendHeroesToWallets.Click += mnuSendHeroesToWallets_Click;

            mnuRecallHerosToSourceWallet = new RadMenuItem();
            mnuRecallHerosToSourceWallet.Text = @"Recall Heroes To Source Wallet";
            mnuRecallHerosToSourceWallet.Click += mnuSendHeroesToWallets_Click;

            mnuGenerateNewWallets = new RadMenuItem();
            mnuGenerateNewWallets.Text = @"Generate New Wallets";
            mnuGenerateNewWallets.Click += mnuGenerateNewWallets_Click;

            var mnuXferLockedJewel = new RadMenuItem();
            mnuXferLockedJewel.Text = @"Transfer Locked Jewel";
            mnuXferLockedJewel.Click += MnuXferLockedJewelOnClick;

            var mnuXferJewel = new RadMenuItem();
            mnuXferJewel.Text = @"Transfer Jewel";
            mnuXferJewel.Click += MnuXferJewelOnClick;

            //Add menu items under the MANAGE section
            mnuManageWallets.Items.AddRange(mnuSendOneAndOnboardToDfk, mnuOnboardToDfk, mnuSendHeroesToWallets,
                mnuRecallHerosToSourceWallet, new RadMenuSeparatorItem(), mnuXferLockedJewel, mnuXferJewel, new RadMenuSeparatorItem(), mnuGenerateNewWallets);


            //Add all menu items to wallet menu
            mnuWallets.Items.AddRange(mnuImport,
                mnuExportWalletDataFile, new RadMenuSeparatorItem(),
                mnuRunWalletInit, new RadMenuSeparatorItem(), mnuManageWallets);


            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;


            mnuExportWalletDataFile.Click += MnuExportWalletDataFileOnClick;
            mnuPreferences.Click += mnuPreferences_Click;
            mnuViewLogs.Click += mnuViewLogs_Click;


            //            //ThemeResolutionService.ApplicationThemeName = Settings.Default.UXTheme;
        }

        private void mnuSendOneAndOnboardToDfk_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            new frmSendONEToWallets().ShowDialog(this);

            LoadDataToGrid();

            mnuMainMenu.Enabled = true;
        }

        private async void mnuOnboardToDfk_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            //Get wallets that need onboarded
            var onboardedAccounts = false;

            foreach (var wallet in WalletManager.GetWallets().Where(x => !x.HasDkProfile))
            {
                wallet.DfkProfile = await new ProfileContractHandler().GetProfile(wallet.WalletAccount);
                if (wallet.DfkProfile == null)
                {
                    var dfkOnboardResult = await
                        new ProfileContractHandler().CreateProfile(wallet,
                            wallet.Name);
                    if (dfkOnboardResult)
                    {
                        await _eventHub.PublishAsync(new MessageEvent()
                        {
                            Content =
                                $"[Wallet:{wallet.Address}] has been onboarded to DFK!"
                        });

                        wallet.DfkProfile = await new ProfileContractHandler().GetProfile(wallet.WalletAccount);


                        onboardedAccounts = true;
                    }
                }
            }

            if (onboardedAccounts)
                WalletManager.SaveWallets();

            mnuMainMenu.Enabled = true;
        }

        private void mnuSendHeroesToWallets_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            new frmSendHeroesToWallets().ShowDialog(this);

            LoadDataToGrid();

            mnuMainMenu.Enabled = true;
        }

        private void mnuGenerateNewWallets_Click(object sender, EventArgs e)
        {
            mnuMainMenu.Enabled = false;

            new frmCreateNewWallets().ShowDialog(this);

            LoadDataToGrid();

            mnuMainMenu.Enabled = true;
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

            //AddConsoleMessage("Wallet Data Re-Indexing before quest engine starts...");

            //await WalletManager.Init();

            AddConsoleMessage("Loading Quest Instances for questing wallets...");

            var instancesStarted = await WalletManager.OnBoardQuestInstances();

            AddConsoleMessage($"({instancesStarted}) Quest Instances initialized and started...");

            //Tell Discord
            //discordBot.SendMessage($"({instancesStarted}) Quest Instances initialized and started...");

            //Tell Jewel Timer to GO
            _eventHub.PublishAsync(new QuestInstancesLoaded());


            mnuStopQuestEngine.Enabled = true;
        }

        private async void mnuStopQuestEngine_Click(object sender, EventArgs e)
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

            discordBot.SendMessage($"Stopping quest instances...");

            QuestEngineManager.KillAllInstances();

            AddConsoleMessage($"Quest Instances stopped...");

            discordBot.SendMessage($"All Quest Instances have been stopped...");

            mnuStartQuestEngine.Enabled = true;

            //Force this grid to empty
            _tableQuestInstances.Rows.Clear();
            gridQuestInstances.Invalidate();
            gridQuestInstances.Refresh();
        }

        private void mnuPreferences_Click(object sender, EventArgs e)
        {
            new frmPreferences().ShowDialog(this);
        }

        private void mnuImportExistingWallet_Click(object sender, EventArgs e)
        {
            new frmImportWallet().ShowDialog(this);

            //Refresh grid
            LoadDataToGrid();
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

                        //Load grid Data
                        LoadDataToGrid();
                    }
                }
            }
        }

        private async void mnuRunWalletInit_Click(object sender, EventArgs e)
        {
            mnuManageWallets.Enabled = false;
            gridWallets.Enabled = false;

            EnableUxControls(false);

            //Init app wallet manager
            await WalletManager.Init();

            AddConsoleMessage("Wallet database indexed...");

            mnuManageWallets.Enabled = true;
            gridWallets.Enabled = true;
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

        private void MnuXferLockedJewelOnClick(object sender, EventArgs e)
        {
            new frmSendJewel().ShowDialog(this);
        }

        private void MnuXferJewelOnClick(object sender, EventArgs e)
        {
            new frmSendNormalJewel().ShowDialog(this);
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
                //desktopAlert.CaptionText = noticeEvent.IsError ? "ERROR" : "INFO";
                //desktopAlert.ContentText = noticeEvent.Content;
                //desktopAlert.Show();
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


            gridWallets.UserDeletedRow += GridWalletsOnUserDeletedRow;
            gridWallets.UserDeletingRow += GridWalletsOnUserDeletingRow;
            gridWallets.SelectionChanged += GridWalletsOnSelectionChanged;
            gridWallets.ContextMenuOpening += GridWalletsContextMenuOpening;

            //Right click context menu event hookup
            mnuGridActionSetAsPrimaryWallet.Click += MnuGridActionSetAsPrimaryWalletClick;
            mnuGridActionSendJewelTo.Click += MnuGridActionSendJewelToOnClick;
            mnuGridActionSendHeroToWallet.Click += MnuGridActionSendHeroToWalletOnClick;
            mnuGridActionRecallHeroToSource.Click += MnuGridActionRecallHeroToSourceOnClick;
            mnuGridActionRebuildHeroProfile.Click += MnuGridActionRebuildHeroProfileOnClick;
            mnuRebuildWalletProfile.Click += MnuRebuildWalletProfileOnClick;

            mnuGridActionSendOneToWallet.Click += MnuGridActionSendOneToWalletOnClick;
            mnuGridActionOnBoardDfk.Click += MnuGridActionOnBoardDfk_Click;
        }

        #endregion

        #region Grid Events


        private void GridWalletsOnUserDeletedRow(object sender, GridViewRowEventArgs e)
        {

        }

        private void GridWalletsOnUserDeletingRow(object sender, GridViewRowCancelEventArgs e)
        {
            if (RadMessageBox.Show(this,
                    "Are you sure you want to delete all selected wallets?\r\nIt will stop any active QuestInstances for those wallets as well.",
                    "Delete Selected Wallets",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                e.Cancel = true;
            else
            {
                foreach (var rowToDelete in e.Rows)
                {
                    var dfkWallet = rowToDelete.DataBoundItem as DfkWallet;
                    if (dfkWallet != null)
                    {
                        //TODO: in future remove all quest types!
                        QuestEngineManager.RemoveQuestEngine(dfkWallet, QuestEngine.QuestTypes.Mining);
                    }

                    //Now Remove from Wallet Manager
                    WalletManager.RemoveWallet(dfkWallet);
                }

                //Save wallet updates
                WalletManager.SaveWallets();
            }
        }

        private void GridWalletsOnSelectionChanged(object sender, EventArgs e)
        {
            var selectedRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selectedRow != null)
            {
                //Wallet Instance
                var walletItem = selectedRow.DataBoundItem as DfkWallet;
                if (walletItem != null)
                {
                    mnuGridActionSendHeroToWallet.Enabled = walletItem.AssignedHero <= 0;

                    //if(walletItem.HasDkProfile && walletItem.CurrentBalance > 0)
                    if (walletItem.HasDkProfile)
                        mnuGridActionOnBoardDfk.Enabled = false;
                    else if (!walletItem.HasDkProfile && walletItem.CurrentBalance > 0)
                        mnuGridActionOnBoardDfk.Enabled = true;

                    mnuGridActionRecallHeroToSource.Enabled = !walletItem.IsPrimarySourceWallet;
                    mnuGridActionSendOneToWallet.Enabled = !walletItem.IsPrimarySourceWallet;
                }
            }
        }

        private void GridWalletsContextMenuOpening(object sender, ContextMenuOpeningEventArgs e)
        {
            e.ContextMenu = mnuGridAction.DropDown;
        }

        #endregion

        #region Grid Context Menu

        private async void MnuRebuildWalletProfileOnClick(object sender, EventArgs e)
        {
            HandleGridContextMenuVisibility(false);

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var wallet = selRow.DataBoundItem as DfkWallet;
                if (wallet != null)
                {
                    await WalletManager.ReloadOneData(wallet.Address);
                    await WalletManager.ReloadWalletHeroData(wallet.Address);
                    await WalletManager.ReloadDfkProfile(wallet.Address);

                    WalletManager.SaveWallets();

                    selRow.InvalidateRow();
                }
            }

            HandleGridContextMenuVisibility(true);
        }

        private async void MnuGridActionRebuildHeroProfileOnClick(object sender, EventArgs e)
        {
            HandleGridContextMenuVisibility(false);

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var wallet = selRow.DataBoundItem as DfkWallet;
                if (wallet != null)
                {
                    await WalletManager.ReloadWalletHeroData(wallet.Address);

                    WalletManager.SaveWallets();

                    selRow.InvalidateRow();
                }
            }

            HandleGridContextMenuVisibility(true);
        }

        private async void MnuGridActionRecallHeroToSourceOnClick(object sender, EventArgs e)
        {
            HandleGridContextMenuVisibility(false);

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var wallet = selRow.DataBoundItem as DfkWallet;
                if (wallet != null)
                {
                    if (wallet.AvailableHeroes.Count > 0)
                    {
                        //We're sending the current wallet hero BACK to the source wallet
                        var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                        if (sourceWallet != null)
                        {
                            UpdateStatusLabelMessage("Moving Heroes Back To Source Wallet....");

                            foreach (var heroId in wallet.AvailableHeroes)
                            {
                                var heroSentToWalletResult =
                                    await new HeroContractHandler().SendHeroToWallet(wallet, sourceWallet.WalletAccount,
                                        heroId);
                                if (heroSentToWalletResult)
                                {
                                    await _eventHub.PublishAsync(new MessageEvent()
                                    {
                                        Content =
                                            $"[Wallet:{sourceWallet.Address}] => Received => [Hero:{heroId}] (Recalled Action)"
                                    });
                                }
                            }

                            await WalletManager.ReloadWalletHeroData(sourceWallet.Address);
                            await WalletManager.ReloadWalletHeroData(wallet.Address);

                            //Force "wallet" we are cleaning up to have no assigned hero, no stam, and clear out available heroes
                            WalletManager.GetWallet(wallet.Address).AssignedHeroStamina = 0;
                            WalletManager.GetWallet(wallet.Address).AvailableHeroes?.Clear();

                            WalletManager.SaveWallets();

                            LoadDataToGrid();

                            UpdateStatusLabelMessage("Idle");

                            ShowRadAlertMessageBox($"{wallet.Name}'s Hero has been recalled back to source wallet!",
                                "Hero Recalled");
                        }
                    }
                }
            }

            HandleGridContextMenuVisibility(true);
        }

        private async void MnuGridActionSendHeroToWalletOnClick(object sender, EventArgs e)
        {
            HandleGridContextMenuVisibility(false);

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var walletToReceiveHero = selRow.DataBoundItem as DfkWallet;
                if (walletToReceiveHero != null)
                {
                    if (walletToReceiveHero.AssignedHero > 0)
                    {
                        ShowRadAlertMessageBox(
                            "This wallet already has a hero assigned.\r\nOnly a single hero is supported currently!",
                            "Hero Exists!");
                        return;
                    }

                    var sourceWallet = WalletManager.GetWallets().FirstOrDefault(x => x.IsPrimarySourceWallet);
                    if (sourceWallet != null)
                    {
                        var firstAvailableHero =
                            sourceWallet.AvailableHeroes.FirstOrDefault(x => x != sourceWallet.AssignedHero);

                        var heroSentToWalletResult =
                            await new HeroContractHandler().SendHeroToWallet(sourceWallet,
                                walletToReceiveHero.WalletAccount,
                                firstAvailableHero);
                        if (heroSentToWalletResult)
                        {
                            await _eventHub.PublishAsync(new MessageEvent()
                            {
                                Content =
                                    $"[Wallet:{walletToReceiveHero.Address}] => Received => [Hero:{firstAvailableHero}]"
                            });
                        }

                        //Remove hero from source wallet 
                        sourceWallet.AvailableHeroes =
                            await new HeroContractHandler().GetWalletHeroes(sourceWallet.WalletAccount);

                        //Add hero to destination wallet
                        walletToReceiveHero.AvailableHeroes =
                            await new HeroContractHandler().GetWalletHeroes(walletToReceiveHero.WalletAccount);

                        //Save Wallet data
                        WalletManager.SaveWallets();

                        ShowRadAlertMessageBox(
                            $"Hero {firstAvailableHero} transferred to wallet {walletToReceiveHero.Address}",
                            "Hero Sent To Wallet");
                    }
                }


                selRow.InvalidateRow();
            }

            HandleGridContextMenuVisibility(true);
        }

        private async void MnuGridActionSendJewelToOnClick(object sender, EventArgs e)
        {
            HandleGridContextMenuVisibility(false);

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var sendJewelTo = selRow.DataBoundItem as DfkWallet;
                if (sendJewelTo != null)
                {
                    //Find the jewel holder
                    var jewelHolder = await WalletManager.GetJewelHolder(Settings.Default.LastKnownJewelHolder);
                    if (jewelHolder != null)
                    {
                        var sendJewelResponse =
                            new JewelContractHandler().SendJewelToAccount(jewelHolder.Holder, sendJewelTo);

                        AddConsoleMessage(sendJewelResponse.Result
                            ? $"Jewel has been transferred to wallet: {sendJewelTo.Name}!"
                            : $"An error occurred trying to send the jewel to: {sendJewelTo.Name}!");
                    }
                }
            }

            HandleGridContextMenuVisibility(true);
        }

        private void MnuGridActionSetAsPrimaryWalletClick(object sender, EventArgs e)
        {
            HandleGridContextMenuVisibility(false);

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var rowDataItem = selRow.DataBoundItem as DfkWallet;
                if (rowDataItem != null)
                {
                    WalletManager.SetAsSourceWallet(rowDataItem);

                    gridWallets.Refresh();
                }
            }

            HandleGridContextMenuVisibility(true);
        }

        private void MnuGridActionSendOneToWalletOnClick(object sender, EventArgs e)
        {
            HandleGridContextMenuVisibility(false);

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                new frmSendONEToWallets(selRow.DataBoundItem as DfkWallet).ShowDialog(this);

                selRow.InvalidateRow();
            }

            HandleGridContextMenuVisibility(true);
        }

        private async void MnuGridActionOnBoardDfk_Click(object sender, EventArgs e)
        {
            mnuOnboardToDfk.Enabled = false;

            HandleGridContextMenuVisibility(false);

            //Get wallets that need onboarded
            var onboardedAccounts = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var walletToOnBoard = selRow.DataBoundItem as DfkWallet;
                if (walletToOnBoard != null)
                {
                    var dfkOnboardResult = await
                        new ProfileContractHandler().CreateProfile(walletToOnBoard,
                            walletToOnBoard.Name);
                    if (dfkOnboardResult)
                    {
                        await _eventHub.PublishAsync(new MessageEvent()
                        {
                            Content =
                                $"[Wallet:{walletToOnBoard.Address}] has been onboarded to DFK!"
                        });

                        var newDfkProfile =
                            await new ProfileContractHandler().GetProfile(walletToOnBoard.WalletAccount);

                        walletToOnBoard.DfkProfile = newDfkProfile;


                        onboardedAccounts = true;
                    }
                }

                if (onboardedAccounts)
                    WalletManager.SaveWallets();

                selRow.InvalidateRow();
            }

            HandleGridContextMenuVisibility(true);
        }

        #endregion

        #region Event Subscription Handler

        private void LoadEventSubscriptions()
        {
            //Inline Subscription Handlers
            _eventHub.Subscribe<MessageEvent>(this, msgEvent =>
            {
                if (!string.IsNullOrWhiteSpace(msgEvent.Content))
                    AddConsoleMessage(msgEvent.Content);
            });

            _eventHub.Subscribe<WalletDataRefreshEvent>(this, wdRefreshEvent =>
            {
                if (wdRefreshEvent.Complete)
                {
                    EnableUxControls(true);
                }
            });

            _eventHub.Subscribe<NotificationEvent>(this, ShowDesktopAlert);

            _eventHub.Subscribe<WalletsOnQuestsMessageEvent>(this, UpdateChartInformation);

            _eventHub.Subscribe<JewelInformationEvent>(this, jewelInfoEvent =>
            {
                if (jewelInfoEvent.JewelInformation != null)
                    HandleJewelProfitUxUpdates(jewelInfoEvent.JewelInformation);
            });

            //Full function subscription handlers
            _eventHub.Subscribe<WalletsGeneratedEvent>(Subscriber);
            _eventHub.Subscribe<WalletQuestStatusEvent>(UpdateGridQuestStatusInfo);
        }

        void UpdateGridQuestStatusInfo(WalletQuestStatusEvent evt)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new WalletQuestStatusEventDelegate(UpdateGridQuestStatusInfo), evt);
                else
                {
                    var foundRow = false;
                    var currentRowCount = gridQuestInstances.RowCount + 1;

                    lock (_tableQuestInstances)
                    {
                        for (var i = 0; i < _tableQuestInstances.Rows.Count; i++)
                        {
                            if (_tableQuestInstances.Rows[i][2].ToString() != evt.WalletAddress) continue;

                            _tableQuestInstances.Rows[i][3] = evt.ReadableActivityMode;
                            _tableQuestInstances.Rows[i][4] =
                                evt.ContractAddress != "0x0000000000000000000000000000000000000000"
                                    ? evt.ContractAddress
                                    : "";
                            _tableQuestInstances.Rows[i][5] = evt.HeroId;
                            _tableQuestInstances.Rows[i][6] = evt.HeroStamina;
                            _tableQuestInstances.Rows[i][7] = evt.StartedAt != null ? evt.StartedAt.ToString() : "";
                            _tableQuestInstances.Rows[i][8] = evt.CompletesAt != null ? evt.CompletesAt.ToString() : "";

                            foundRow = true;
                            break;
                        }

                        if (!foundRow)
                        {
                            var row = _tableQuestInstances.NewRow();
                            row[0] = currentRowCount;
                            row[1] = evt.Name;
                            row[2] = evt.WalletAddress;
                            row[3] = evt.ReadableActivityMode;
                            row[4] = evt.ContractAddress != "0x0000000000000000000000000000000000000000"
                                ? evt.ContractAddress
                                : "";
                            row[5] = evt.HeroId;
                            row[6] = evt.HeroStamina;
                            row[7] = evt.StartedAt != null ? evt.StartedAt.ToString() : "";
                            row[8] = evt.CompletesAt != null ? evt.CompletesAt.ToString() : "";
                            _tableQuestInstances.Rows.Add(row);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        void Subscriber(WalletsGeneratedEvent obj)
        {
            LoadDataToGrid();
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
                Debug.WriteLine($"Jewel Balance: {jewelInfo.Balance}");

                //Lets tally up old jewel amount to new (this only happens on startup)
                if (_startingJewelAmount == 0)
                {
                    _startingJewelAmount = jewelInfo.Balance;
                    _currentJewelTotal = jewelInfo.Balance;

                    AddConsoleMessage($@"Jewel Manager Init.... Current Bag of Jewel => {$"{_startingJewelAmount:0.0#}"}");

                    toolStripJewelAmount.Text = _currentJewelProfit == 0 ? $@"Jewel Earned (0/{$"{_startingJewelAmount:0.0#}"}" : $@"Jewel Earned ({$"{_currentJewelProfit:0.0#}"}/{$"{_currentJewelTotal:0.0#}"})";

                    //Lets trigger a notice to discord
                    TimerJewelTotalCheckOnElapsed(null, null);
                }
                else
                {
                    //Take start balance and subtract the balance out of it. (new balance is == or > then starting)
                    var currentProfit = (jewelInfo.Balance - _startingJewelAmount);  //500 = balance, starting was 200 == 300 profit this cycle
                    var currentProfitMinusPercentage = (currentProfit * Convert.ToDecimal(.23)); // times 23% = 69  (so current profit is 300-69,   and 500 - 69

                    var currentTotal = (jewelInfo.Balance - currentProfitMinusPercentage); //This would show 431
                    var currentProfitTotal = (currentProfit - currentProfitMinusPercentage); //300 - 69 =

                    //Set global vars
                    _currentJewelProfit = currentProfitTotal;
                    _currentJewelTotal = currentTotal;

                    //Have we gone over 100?  (if profit was like 278 - 0  = SEND IT, add 278 to amount sent out,     
                    var totalToSend = (currentProfitMinusPercentage - _totalSentOutAlready);  //  285 - 278 = 7.... not >= 200, dont sent.

                    if (totalToSend >= 100)
                    {
                        JewelManager.ValidateTime = true;
                        JewelManager.ValidationAmount = totalToSend;

                        _totalSentOutAlready += currentProfitMinusPercentage;
                    }
                    
                    //Update toolstrip
                    toolStripJewelAmount.Text = _currentJewelProfit == 0 ? $@"Jewel Earned (0/{$"{_startingJewelAmount:0.0#}"}" : $@"Jewel Earned ({$"{_currentJewelProfit:0.0#}"}/{$"{_currentJewelTotal:0.0#}"})";
                }

                LoadDataToGrid();
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
                //Do a FULL Index
                EnableUxControls(false);

                AddConsoleMessage("Initializing Wallets & Connecting to the Block-Chain...");

                await WalletManager.Init(true);

                AddConsoleMessage("Wallets have been initialized!");

                SetWalletCountToolStripLabel($@"Total Wallets: {WalletManager.GetWallets().Count}");

                EnableUxControls(true);

                AddConsoleMessage("Initializing Jewel Monitoring System");

                await JewelManager.Init(Settings.Default.LastKnownJewelHolder);

                AddConsoleMessage("Jewel Monitoring System initialized!");

                EnableUxControls(true);
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

        #region Chart Management

        void UpdateChartInformation(WalletsOnQuestsMessageEvent msgEvent)
        {
            if(InvokeRequired)
                Invoke(new HandleWalletsOnQuestsMessageEventDelegate(UpdateChartInformation), msgEvent);
            else
            {
                try
                {
                    var totalWalletCount = WalletManager.GetWallets().Count;
                    double percentOnline;
                    double percentOffline;

                    switch (msgEvent.OnQuestMessageEventType)
                    {
                        case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStarting:
                            currentOnlineValue += 1;
                            currentOfflineValue -= 1;

                            //MAX VALUE IS 100.  So outta TOTAL wallets (x) 
                            percentOnline = (currentOnlineValue / totalWalletCount) * 100;
                            percentOffline = (currentOfflineValue / totalWalletCount) * 100;

                            //Set New Values
                            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[0].SetValue(0, percentOnline);
                            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[1].SetValue(1, percentOffline);
                            break;

                        case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.InstanceStopping:
                            currentOnlineValue -= 1;
                            currentOfflineValue += 1;

                            //MAX VALUE IS 100.  So outta TOTAL wallets (x) 
                            percentOnline = (currentOnlineValue / totalWalletCount) * 100;
                            percentOffline = (currentOfflineValue / totalWalletCount) * 100;

                            //Set New Values
                            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[0].SetValue(0, Convert.ToDouble(percentOnline));
                            chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[1].SetValue(0, Convert.ToDouble(percentOffline));
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
            var walletInfo = WalletManager.GetWallets();

            //Set total current offline value
            currentOfflineValue = walletInfo.Count;

            chartBotsOnline.AreaType = ChartAreaType.Pie;
            var pieSeriesBotsOnline = new PieSeries();
            pieSeriesBotsOnline.DataPoints.Add(new PieDataPoint(0, "Online"));
            pieSeriesBotsOnline.DataPoints.Add(new PieDataPoint(walletInfo.Count, "Offline"));
            pieSeriesBotsOnline.ShowLabels = true;
            pieSeriesBotsOnline.LabelMode = PieLabelModes.Horizontal;
            chartBotsOnline.LegendTitle = "Bots Online";
            chartBotsOnline.ShowLegend = true;
            chartBotsOnline.Series.Add(pieSeriesBotsOnline);


            chartHeroesQuesting.AreaType = ChartAreaType.Pie;
            var pieSeriesQuestInstanceStatus = new PieSeries();
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Need Stamina"));
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Questing"));
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Wants To Cancel"));
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Wants To Complete"));
            pieSeriesQuestInstanceStatus.DataPoints.Add(new PieDataPoint(0, "Wants To Quest"));
            pieSeriesQuestInstanceStatus.ShowLabels = true;
            pieSeriesQuestInstanceStatus.LabelMode = PieLabelModes.Horizontal;
            chartHeroesQuesting.LegendTitle = "Quest Instance Activity";
            chartHeroesQuesting.ShowLegend = true;
            chartHeroesQuesting.Series.Add(pieSeriesQuestInstanceStatus);
            chartHeroesQuesting.Visible = true;
        }

        #endregion

        #region NodeJs Server Handlers
        
        private async void CheckForServer()
        {
            try
            {
                var response =
                    await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>("/api/verify");
                if (response != null)
                {
                    if(!response.Success)
                        ShowRadAlertMessageBox("DFKQR+ URL is in-accessible!  This connection is REQUIRED to properly work!", "Server Down!");
                }
            }
            catch (Exception ex)
            {
                ShowRadAlertMessageBox("DFKQR+ URL is in-accessible!  This connection is REQUIRED to properly work!",
                    "Server Down!");
            }
        }

        #endregion

        #region MessageBox Alert

        void ShowRadAlertMessageBox(string text, string title)
        {
            if (InvokeRequired)
                Invoke(new ShowRadAlertMessageBoxDelegate(ShowRadAlertMessageBox), text, title);
            else
            {
                RadMessageBox.Show(this, text, title);
            }
        }

        #endregion

        #region Status Box Handler

        void UpdateStatusLabelMessage(string text)
        {
            if (InvokeRequired)
                Invoke(new UpdateStatusLabelDelegate(UpdateStatusLabelMessage), text);
            else
            {
               
            }
        }

        #endregion

        #region Data Handler Methods

        void LoadDataToGrid()
        {
            if (InvokeRequired)
                Invoke(new LoadDataToGridDelegate(LoadDataToGrid));
            else
            {
                var wallets = WalletManager.GetWallets();
                if (wallets != null)
                {
                    //Refresh grid
                    gridWallets.SuspendLayout();
                    gridWallets.DataSource = wallets;
                    gridWallets.Refresh();
                    gridWallets.ResumeLayout();

                    //TODO: FIX ME
                    //lblWalletsThatCanQuest.Text = $@"({wallets.Count(x => x.ReadyToWork)}) Wallets Are Quest Ready";
                }
            }
        }

        #endregion

        #region Grid ContextMenu Handlers

        void HandleGridContextMenuVisibility(bool visible)
        {
            foreach (var item in mnuGridAction.Items)
                item.Enabled = visible;
        }

        #endregion

        #region Discord Bot Management

        private async void TimerJewelTotalCheckOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                await discordBot.SendMessage($"Current JEWEL Profits: {_currentJewelProfit:0.0#}.  Total Jewel In Da Bag: {_currentJewelTotal:0.0#} JEWEL");
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
