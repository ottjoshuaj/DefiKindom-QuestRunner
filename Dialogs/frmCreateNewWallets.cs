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
using DefiKindom_QuestRunner.Objects;
using PubSub;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace DefiKindom_QuestRunner.Dialogs
{
    public partial class frmCreateNewWallets : RadForm
    {
        static Hub eventHub = Hub.Default;

        public frmCreateNewWallets()
        {
            InitializeComponent();
        }

        private void frmCreateNewWallets_Load(object sender, EventArgs e)
        {

        }

        private async void btnGenerateWallets_Click(object sender, EventArgs e)
        {
            Enabled = false;

            try
            {
                var walletsGenerated = 0;

                for (var i = 0; i < txtNewWalletAmount.Value; i++)
                {
                    var walletName = GenerateWord(10);
                    var walletNameExists = WalletManager.GetWallets().Any(x => x.Name.Trim() == walletName);
                    while (walletNameExists)
                    {
                        walletName = GenerateWord(10);
                        walletNameExists = WalletManager.GetWallets().Any(x => x.Name.Trim() == walletName);
                    }

                    var newWallet = await WalletManager.CreateWallet(walletName);
                    if (newWallet != null)
                    {
                        walletsGenerated++;

                        WalletManager.AddWallet(newWallet);
                    }
                }

                WalletManager.SaveWallets();

                RadMessageBox.Show(this, $@"Created {walletsGenerated} new random wallets and added them to the manager.", @"Wallets Generated");

                await eventHub.PublishAsync(new WalletsGeneratedEvent { TotalWallets = WalletManager.GetWallets().Count });

                Close();
            }
            catch(Exception ex)
            {
                await eventHub.PublishAsync(new NotificationEvent { IsError = true, Content = ex.Message });
            }

            Enabled = true;
        }

        #region Utility Methods

        string GenerateWord(int requestedLength)
        {
            Random rnd = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
            string[] vowels = { "a", "e", "i", "o", "u" };

            string word = "";

            if (requestedLength == 1)
                word = GetRandomLetter(rnd, vowels);
            else
            {
                for (var i = 0; i < requestedLength; i += 2)
                    word += GetRandomLetter(rnd, consonants) + GetRandomLetter(rnd, vowels);

                word = word.Replace("q", "qu").Substring(0, requestedLength); // We may generate a string longer than requested length, but it doesn't matter if cut off the excess.
            }

            return word;
        }

        string GetRandomLetter(Random rnd, string[] letters)
        {
            return letters[rnd.Next(0, letters.Length - 1)];
        }

        #endregion
    }
}
