using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DefiKindom_QuestRunner.Properties;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class OneContractHandler
    {
        public async Task<bool> SendHarmonyONE(Account account, string destAddress, decimal amount)
        {
            try
            {
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);
                var transaction = await web3.Eth.GetEtherTransferService()
                    .TransferEtherAndWaitForReceiptAsync(destAddress, amount, 2);
                if (transaction.Succeeded())
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public async Task<decimal> CheckHarmonyONEBalance(Account account)
        {
            try
            {
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);
                return Web3.Convert.FromWei(await web3.Eth.GetBalance.SendRequestAsync(account.Address));
            }
            catch (Exception ex)
            {

            }

            return 0;
        }
    }
}
