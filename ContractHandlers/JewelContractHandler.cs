﻿using System;
using System.Threading.Tasks;
using System.Numerics;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class JewelContractHandler
    {
        public async Task<decimal> CheckJewelBalance(Account account)
        {
            try
            {
                if (account == null)
                {
                    //What wallet failed?
                    return 0;
                }

                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                //Lets run some routines to get info about each account
                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Jewel),
                    Settings.Default.JewelContractAddress);
                var contractFunction = contract.GetFunction("balanceOf");
                var contractResult = await contractFunction.CallDecodingToDefaultAsync(account.Address);
                var balance = Web3.Convert.FromWei(BigInteger.Parse(contractResult[0].ConvertToString()));

                return balance;
            }
            catch (Exception ex)
            {

            }

            return -1;
        }

        public async Task<bool> SendJewelToAccount(DfkWallet source, DfkWallet destination)
        {
            try
            {
                if (source.Address.ToLower() == destination.Address.ToLower())
                    return true;

                var response = await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>(
                    QuickRequest.ApiRequestTypes.MoveJewel, new JewelTransferRequest
                    {
                        Wallet = new SmallWalletItem
                        {
                            Name = source.Name,
                            Address = source.Address,
                            PrivateKey = source.PrivateKey,
                            PublicKey = source.PublicKey,
                            MnemonicPhrase = source.MnemonicPhrase
                        },
                        DestinationAddress = destination.Address
                    });

                if (response != null)
                    return response.Success;

                //JewelTransferRequest

                /*
                var web3 = new Web3(source.WalletAccount, Settings.Default.CurrentRpcUrl)
                {
                    TransactionManager =
                    {
                        UseLegacyAsDefault = true,
                        CalculateOrSetDefaultGasPriceFeesIfNotSet = true
                    }
                };

                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Jewel),
                    Settings.Default.JewelContractAddress);

                var gas = new HexBigInteger(new BigInteger(400000));
                var value = new HexBigInteger(new BigInteger(0));
                var transaction = contract.GetFunction("transferAll").SendTransactionAsync(source.WalletAccount.Address, gas, value, destination.Address);
                transaction.Wait();

                return true;
                */
            }
            catch (Exception ex)
            {

            }

            return false;
        }

    }
}
