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
using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;
using Telerik.Charting;

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

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private decimal _startingJewelAmount = 0;
        readonly Hub _eventHub = Hub.Default;

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

                var logFileName = $"LogFile-{DateTime.Now.ToShortDateString().Replace("/", "-")}-{DateTime.Now.Hour}.txt";

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

            //Is quest instance running?
            if (QuestEngineManager.GetAllQuestInstances().Count > 0)
            {
                mnuGridActionSendJewelTo.Enabled = false;
            }
        }

        private async void OnShown(object sender, EventArgs e)
        {

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

            //Add menu items under the MANAGE section
            mnuManageWallets.Items.AddRange(mnuSendOneAndOnboardToDfk, mnuOnboardToDfk, mnuSendHeroesToWallets,
                mnuRecallHerosToSourceWallet, new RadMenuSeparatorItem(), mnuGenerateNewWallets);


            //Add all menu items to wallet menu
            mnuWallets.Items.AddRange(mnuImport,
                mnuExportWalletDataFile, new RadMenuSeparatorItem(),
                mnuRunWalletInit, new RadMenuSeparatorItem(), mnuManageWallets);


            mnuStartQuestEngine.Enabled = false;
            mnuStopQuestEngine.Enabled = false;


            mnuExportWalletDataFile.Click += MnuExportWalletDataFileOnClick;
            mnuPreferences.Click += mnuPreferences_Click;
            mnuViewLogs.Click += mnuViewLogs_Click;
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

        private void MnuGridActionRebuildHeroProfileOnClick(object sender, EventArgs e)
        {
            mnuGridActionRebuildHeroProfile.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var wallet = selRow.DataBoundItem as DfkWallet;
                if (wallet != null)
                {
                    WalletManager.ReloadWalletHeroData(wallet.Address);
                    WalletManager.SaveWallets();

                    LoadDataToGrid();

                    RadMessageBox.Show(this, $"{wallet.Name} Hero Data and Profiles Rebuilt...",
                        "Hero Data Rebuilt");
                }
            }

            mnuGridActionRebuildHeroProfile.Enabled = true;
        }

        private async void MnuGridActionRecallHeroToSourceOnClick(object sender, EventArgs e)
        {
            mnuGridActionRecallHeroToSource.Enabled = false;

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

                            WalletManager.SaveWallets();

                            LoadDataToGrid();

                            UpdateStatusLabelMessage("Idle");

                            ShowRadAlertMessageBox($"{wallet.Name}'s Hero has been recalled back to source wallet!",
                                "Hero Recalled");
                        }
                    }
                }
            }

            mnuGridActionRecallHeroToSource.Enabled = true;
        }

        private async void MnuGridActionSendHeroToWalletOnClick(object sender, EventArgs e)
        {
            mnuGridActionSendHeroToWallet.Enabled = false;

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

                        WalletManager.ReloadWalletHeroData(walletToReceiveHero.Address);

                        WalletManager.SaveWallets();

                        LoadDataToGrid();

                        ShowRadAlertMessageBox(
                            $"Hero {firstAvailableHero} transferred to wallet {walletToReceiveHero.Address}",
                            "Hero Sent To Wallet");
                    }
                }
            }

            mnuGridActionSendHeroToWallet.Enabled = true;
        }

        private async void MnuGridActionSendJewelToOnClick(object sender, EventArgs e)
        {
            if (RadMessageBox.Show(this, "Are you sure you want to send Jewel to this wallet?", "Send Jewel",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            mnuGridActionSendJewelTo.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                var sendJewelTo = selRow.DataBoundItem as DfkWallet;
                if (sendJewelTo != null)
                {
                    //Find the jewel holder
                    var jewelHolder = await WalletManager.GetJewelHolder();
                    if (jewelHolder != null)
                    {
                        var sendJewelResponse =
                            new JewelContractHandler().SendJewelToAccount(jewelHolder.Holder, sendJewelTo);

                        if (sendJewelResponse.Result)
                        {
                            ShowRadAlertMessageBox($"Jewel has been transferred to wallet: {sendJewelTo.Name}!",
                                "Jewel Transferred!");
                        }
                        else
                        {
                            ShowRadAlertMessageBox(
                                $"An error occurred trying to send the jewel to: {sendJewelTo.Name}!",
                                "Jewel Transferred ERROR");
                        }

                    }


                    LoadDataToGrid();
                }
            }

            mnuGridActionSendJewelTo.Enabled = false;
        }

        private void MnuGridActionSetAsPrimaryWalletClick(object sender, EventArgs e)
        {
            mnuGridActionSetAsPrimaryWallet.Enabled = false;

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

            mnuGridActionSetAsPrimaryWallet.Enabled = false;
        }

        private void MnuGridActionSendOneToWalletOnClick(object sender, EventArgs e)
        {
            mnuGridActionSendOneToWallet.Enabled = false;

            var selRow = gridWallets.SelectedRows.FirstOrDefault();
            if (selRow != null)
            {
                new frmSendONEToWallets(selRow.DataBoundItem as DfkWallet).ShowDialog(this);

                LoadDataToGrid();
            }

            mnuGridActionSendOneToWallet.Enabled = true;
        }

        private async void MnuGridActionOnBoardDfk_Click(object sender, EventArgs e)
        {
            mnuOnboardToDfk.Enabled = false;

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

                LoadDataToGrid();
            }
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

            _eventHub.Subscribe<WalletsOnQuestsMessageEvent>(this, msgEvent =>
            {
                switch (msgEvent.OnQuestMessageEventType)
                {
                    case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.Questing:
                        AddConsoleMessage($"[Wallet {msgEvent.Wallet.Name} => {msgEvent.Wallet.Address}] => Started Questing....");
                        break;

                    case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingCanceled:
                        AddConsoleMessage($"[Wallet {msgEvent.Wallet.Name} => {msgEvent.Wallet.Address}] => Canceled Questing....");
                        break;

                    case WalletsOnQuestsMessageEvent.OnQuestMessageEventTypes.QuestingComplete:
                        AddConsoleMessage($"[Wallet {msgEvent.Wallet.Name} => {msgEvent.Wallet.Address}] => Completed Questing....");
                        break;
                }

                UpdateChartInformation(msgEvent);
            });

            _eventHub.Subscribe<JewelInformationEvent>(this, jewelInfoEvent =>
            {
                if (jewelInfoEvent.JewelInformation != null)
                {
                    //jewelInfoEvent.JewelInformation
                    AddConsoleMessage($"[Current Jewel Holder:{jewelInfoEvent.JewelInformation.Holder.Address}] => Currently Holds Your Jewels in da sack!");

                    HandleJewelProfitUxUpdates(jewelInfoEvent.JewelInformation);
                }
            });

            //Full function subscription handlers
            _eventHub.Subscribe<WalletJewelMovedEvent>(UpdateGridWithJewelLocationInfo);
            _eventHub.Subscribe<ManageWalletGridEvent>(UpdateWalletGridEvent);
            _eventHub.Subscribe<WalletsGeneratedEvent>(Subscriber);
        }

        void UpdateGridWithJewelLocationInfo(WalletJewelMovedEvent evt)
        {
            LoadDataToGrid();
        }

        void UpdateWalletGridEvent(ManageWalletGridEvent evt)
        {
            LoadDataToGrid();
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
            try
            {
                chartBotsOnline.GetSeries<PieSeries>(0).DataPoints[1].SetValue(0, Convert.ToDouble(WalletManager.GetWallets().Count));
            } catch {}

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
            var walletInfo = WalletManager.GetWallets();


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
                lblAppStatus.Text = text;
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
    }
}
