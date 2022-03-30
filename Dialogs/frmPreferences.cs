using System;
using System.Globalization;
using System.Threading.Tasks;
using Telerik.WinControls;
using Telerik.WinControls.UI;

using DefiKindom_QuestRunner.Properties;
using PubSub;

using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmPreferences : RadForm
    {
        #region Internals

        Hub eventHub = Hub.Default;

        #endregion

        #region Constructor(s)

        public frmPreferences()
        {
            InitializeComponent();

        }

        #endregion

        #region Form Events

        private void frmPreferences_Load(object sender, EventArgs e)
        {
            cmbRPCSettings.Items.Add("https://api.harmony.one");
            cmbRPCSettings.Items.Add("https://s1.api.harmony.one");
            cmbRPCSettings.Items.Add("https://s2.api.harmony.one");
            cmbRPCSettings.Items.Add("https://s3.api.harmony.one");


            cmbRpcChain.Items.Add(new RadListDataItem("Shard 0", "1666600000"));
            cmbRpcChain.Items.Add(new RadListDataItem("Shard 1", "1666600001"));
            cmbRpcChain.Items.Add(new RadListDataItem("Shard 2", "1666600002"));
            cmbRpcChain.Items.Add(new RadListDataItem("Shard 3", "1666600003"));


            //Select one based on current config settings
            cmbRPCSettings.DropDownListElement.SelectedValue = Settings.Default.CurrentRpcUrl;
            cmbRpcChain.DropDownListElement.SelectedValue = Settings.Default.CurrentRpcShard;

            //App Behavior
            chkHideToTrayOnMinimize.Checked = Settings.Default.MinimizeToTray;


            //Instance Intervals
            txtJewelInstanceInterval.Value = Settings.Default.JewelInstanceMsInterval;
            txtQuestInterval.Value = Settings.Default.QuestInstanceMsInterval;


            //Local/Hosted NodeJS Server
            txtNodeJsServerEndpoint.Text = Settings.Default.ExecutorApi;
        }

        #endregion

        #region Button Events

        private async void btnSavePreferences_Click(object sender, EventArgs e)
        {
            var oldServer = Settings.Default.ExecutorApi;

            Settings.Default.ExecutorApi = txtNodeJsServerEndpoint.Text;
            Settings.Default.Save();

            var serverIsValid = await CheckForServer();
            if (!serverIsValid)
            {
                Settings.Default.ExecutorApi = oldServer;
                Settings.Default.Save();

                RadMessageBox.Show(this, "DFKQR+ URL is in-accessible! Double check the url and ensure it is correct",
                    "URL Unreachable");

                return;
            }

            //Local/Hosted NodeJS Server
            Settings.Default.ExecutorApi = txtNodeJsServerEndpoint.Text;

            //App Behavior Settings
            Settings.Default.MinimizeToTray = chkHideToTrayOnMinimize.Checked;

            //Instance Intervals
            var intervalsChanged =
                Convert.ToInt32(txtJewelInstanceInterval.Value) != Settings.Default.JewelInstanceMsInterval ||
                Convert.ToInt32(txtQuestInterval.Value) != Settings.Default.QuestInstanceMsInterval;


            Settings.Default.JewelInstanceMsInterval = int.Parse(txtJewelInstanceInterval.Value.ToString(CultureInfo.InvariantCulture));
            Settings.Default.QuestInstanceMsInterval = int.Parse(txtQuestInterval.Value.ToString(CultureInfo.InvariantCulture));

            
            //RPC Settings
            Settings.Default.CurrentRpcUrl = cmbRPCSettings.DropDownListElement.Text;
            Settings.Default.CurrentRpcShard = cmbRpcChain.DropDownListElement.SelectedValue.ToString();

            Settings.Default.Save();

            if (intervalsChanged)
            {
                await eventHub.PublishAsync(new PreferenceUpdateEvent
                {
                    JewelTimerInterval = Settings.Default.JewelInstanceMsInterval,
                    QuestInstanceInterval = Settings.Default.QuestInstanceMsInterval
                });
            }

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region NodeJs Server Verifier

        private async Task<bool> CheckForServer()
        {
            try
            {
                var response =
                    await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>("/api/verify");
                if (response != null)
                    return response.Success;
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        #endregion
    }
}
//PreferenceUpdateEvent