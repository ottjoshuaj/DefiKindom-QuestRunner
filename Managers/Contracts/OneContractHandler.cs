using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Numerics;

using DefiKindom_QuestRunner.Properties;

using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class OneContractHandler
    {
        public async Task<OneFundRequestResponse> SendHarmonyONE(Account account, string destAddress, decimal amount)
        {
            try
            {
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                var transactionInput =
                    new TransactionInput
                    {
                        Value = new HexBigInteger(Web3.Convert.ToWei(amount)),
                        To = destAddress,
                        From = account.Address
                    };

                transactionInput.GasPrice = new HexBigInteger(BigInteger.Parse("30000000000"));
                transactionInput.MaxFeePerGas = new HexBigInteger(BigInteger.Parse("30000000000"));

                var transactionHash = await web3.TransactionManager.SendTransactionAsync(transactionInput);

                return new OneFundRequestResponse { Success = true, TransactionHash = transactionHash };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return new OneFundRequestResponse { Success = false};
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
                Debug.WriteLine(ex);
            }

            return 0;
        }
    }
}
