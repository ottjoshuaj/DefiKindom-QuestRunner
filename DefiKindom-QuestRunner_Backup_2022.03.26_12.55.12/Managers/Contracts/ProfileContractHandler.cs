using System;
using System.Threading.Tasks;

using DefiKindom_QuestRunner.Abis.Objects;
using DefiKindom_QuestRunner.Properties;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class ProfileContractHandler
    {
        public async Task<bool> CreateProfile(Account account, string profileName)
        {
            try
            {
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                //Lets run some routines to get info about each account
                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Profile),
                    Settings.Default.ProfileContractAddress);
                var contractFunction = contract.GetFunction("createProfile");
               
                return await contractFunction.CallAsync<bool>(profileName, 0, 0);
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public async Task<DfkProfile> GetProfile(Account account)
        {
            try
            {
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                //Lets run some routines to get info about each account
                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Profile),
                    Settings.Default.ProfileContractAddress);
                var contractFunction = contract.GetFunction("getProfileByAddress");

                var profile = await contractFunction.CallDecodingToDefaultAsync(account.Address);
                if (profile != null)
                {
                    return new DfkProfile(profile[0].ConvertToInt(), profile[1].ConvertToString(),
                        profile[2].ConvertToString(), profile[3].ConvertToDateTime(),
                        profile[4].ConvertToInt(), profile[5].ConvertToInt(), profile[6].ConvertToInt());
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }

    }
}


/*
        .createProfile(
            sourceWallet.name,
            0,
            0,

//transactionHash = await multiplyFunction.SendTransactionAsync(senderAddress, 7);
// var txHash = await contractFunction.SendTransactionAsync(wallet.Address, wallet.AssignedHero);
//var receipt = await MineAndGetReceiptAsync(web3, txHash);
 *
 */
