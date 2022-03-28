using System;
using System.Numerics;
using System.Threading.Tasks;

using DefiKindom_QuestRunner.Properties;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class QuestContractHandler
    {

        public async Task<int> GetHeroStamina(Account account, int heroId)
        {
            try
            {
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                //Lets run some routines to get info about each account
                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Quest),
                    Settings.Default.QuestContractAddress);
                var contractFunction = contract.GetFunction("getCurrentStamina");
                var contractResult = await contractFunction.CallAsync<BigInteger>(heroId);

                return (int) contractResult;
            }
            catch (Exception ex)
            {

            }

            return -1;
        }
    }
}


//transactionHash = await multiplyFunction.SendTransactionAsync(senderAddress, 7);
// var txHash = await contractFunction.SendTransactionAsync(wallet.Address, wallet.AssignedHero);
//var receipt = await MineAndGetReceiptAsync(web3, txHash);