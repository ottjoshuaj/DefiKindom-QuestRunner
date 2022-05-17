using System;
using DefiKindom_QuestRunner.Properties;

using Nethereum.Contracts;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

namespace DefiKindom_QuestRunner.Managers.Contracts.Base
{
    internal class BaseContract
    {
        internal Function BuildContract(Account account, AbiManager.AbiTypes abiType, string functionName)
        {
            string contractAddress;

            switch (abiType)
            {
                case AbiManager.AbiTypes.Hero:
                    contractAddress = Settings.Default.HeroContractAddress;
                    break;
                case AbiManager.AbiTypes.Quest:
                    contractAddress = Settings.Default.QuestContractAddress;
                    break;
                case AbiManager.AbiTypes.Profile:
                    contractAddress = Settings.Default.ProfileContractAddress;
                    break;
                case AbiManager.AbiTypes.Jewel:
                    contractAddress = Settings.Default.JewelContractAddress;
                    break;

                default:
                    throw new Exception($"{functionName} isn't supported by this method. Please use the other override method to pass in a contract address");
            }

            return new Web3(account, Settings.Default.CurrentRpcUrl).Eth
                .GetContract(AbiManager.GetAbi(abiType), contractAddress).GetFunction(functionName);
        }

        internal Function BuildContract(Account account, AbiManager.AbiTypes abiType, string contractAddress, string functionName)
        {
            return new Web3(account, Settings.Default.CurrentRpcUrl).Eth
                .GetContract(AbiManager.GetAbi(abiType), contractAddress).GetFunction(functionName);
        }
    }
}
