using System;
using System.Globalization;

using Telerik.WinControls.UI;

using DefiKindom_QuestRunner.Properties;


namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmPreferences : RadForm
    {
        public frmPreferences()
        {
            InitializeComponent();
        }

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

        private void btnSavePreferences_Click(object sender, EventArgs e)
        {
            //Local/Hosted NodeJS Server
            Settings.Default.ExecutorApi = txtNodeJsServerEndpoint.Text;

            //App Behavior Settings
            Settings.Default.MinimizeToTray = chkHideToTrayOnMinimize.Checked;

            //Instance Intervals
            Settings.Default.JewelInstanceMsInterval = int.Parse(txtJewelInstanceInterval.Value.ToString(CultureInfo.InvariantCulture));
            Settings.Default.QuestInstanceMsInterval = int.Parse(txtQuestInterval.Value.ToString(CultureInfo.InvariantCulture));

            
            //RPC Settings
            Settings.Default.CurrentRpcUrl = cmbRPCSettings.DropDownListElement.Text;
            Settings.Default.CurrentRpcShard = cmbRpcChain.DropDownListElement.SelectedValue.ToString();

            Settings.Default.Save();

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
