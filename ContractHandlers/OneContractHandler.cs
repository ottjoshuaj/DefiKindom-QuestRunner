using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Numerics;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class OneContractHandler
    {
        public async Task<FundWalletWithOneResponse> SendHarmonyONE(DfkWallet sourceWallet, string destinationAddress, int amount)
        {
            try
            {
                //FundWalletWithOne , FundWalletWithOneResponse
                var response = await new QuickRequest().GetDfkApiResponse<FundWalletWithOneResponse>(
                   "/api/wallets/fund", new FundWalletWithOne
                    {
                        Wallet = new SmallWalletItem
                        {
                            Name = sourceWallet.Name,
                            Address = sourceWallet.Address,
                            PrivateKey = sourceWallet.PrivateKey,
                            PublicKey = sourceWallet.PublicKey,
                            MnemonicPhrase = sourceWallet.MnemonicPhrase
                        },
                        DestinationAddress = destinationAddress,
                        Amount = amount
                        
                    });


                return response;


                /*
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
                */
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                return new FundWalletWithOneResponse
                {
                    Success = false
                };
            }
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
