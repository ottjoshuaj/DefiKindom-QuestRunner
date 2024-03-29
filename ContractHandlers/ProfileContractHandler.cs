﻿using System;
using System.Threading.Tasks;
using System.Numerics;

using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using DefiKindom_QuestRunner.Abis.Objects;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class ProfileContractHandler
    {
        public enum NftTypes
        {
            YellowEgg,
            ShvasRune,
            Tears
        }

        public async Task<bool> CreateProfile(DfkWallet wallet, string profileName)
        {
            try
            {
                var response = await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>(
                    "/api/profile/create", new DfkProfileCreateRequest
                    {
                        Wallet = new SmallWalletItem
                        {
                            Name = wallet.Name,
                            Address = wallet.Address,
                            PrivateKey = wallet.PrivateKey,
                            PublicKey = wallet.PublicKey,
                            MnemonicPhrase = wallet.MnemonicPhrase
                        }
                    });

                if(response != null)
                    return response.Success;

                /*
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl)
                {
                    TransactionManager =
                    {
                        UseLegacyAsDefault = true,
                        CalculateOrSetDefaultGasPriceFeesIfNotSet = true
                    }
                };

                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Profile),
                    Settings.Default.ProfileContractAddress);


                var gas = new HexBigInteger(new BigInteger(400000));
                var value = new HexBigInteger(new BigInteger(0));
                var transaction = contract.GetFunction("createProfile").SendTransactionAsync(account.Address, gas, value, profileName, 0, 0);
                transaction.Wait();

                return true;
                */
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