using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using DefiKindom_QuestRunner.Managers.Engines;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;
using NBitcoin;
using Nethereum.Contracts;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class QuestContractHandler
    {
        public async Task<DkHeroQuestStatus> GetHeroQuestStatus(Account account, int heroId)
        {
            var timesCheckingForQuestStatus = 0;
            var foundHeroQuestData = false;
            var heroQuestStatus = new DkHeroQuestStatus();

            //Since RPC is unreachable sometimes, loop till it populates the information!
            //This is the MOST IMPORTANT method in the app !  WE 100% NEED the result!
            while (!foundHeroQuestData)
            {
                try
                {
                    //If we've checked twice. Sleep for 2 seconds to prevent 
                    //being blocked by the RPC for spamming
                    if (timesCheckingForQuestStatus == 2)
                    {
                        Thread.Sleep(2000);
                        timesCheckingForQuestStatus = 0;
                    }

                    var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                    //Lets run some routines to get info about each account
                    var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Quest),
                        Settings.Default.QuestContractAddress);
                    var contractFunction = contract.GetFunction("getHeroQuest");
                    var contractResult = await contractFunction.CallDecodingToDefaultAsync(heroId);

                    //TODO: In future we will support MULTIPLE Heroes farming etc. We will need to loop the array of results 
                    //here and update the object tier
                    var firstQuestResults = contractResult.FirstOrDefault();
                    var subItems = firstQuestResults.Result as List<ParameterOutput>;

                    heroQuestStatus.Id = subItems[0].ConvertToString();
                    heroQuestStatus.ContractAddress = subItems[1].ConvertToString();
                    heroQuestStatus.HeroesOnQuest = subItems[2].ConvertToIntList();
                    heroQuestStatus.PlayerAddress = subItems[3].ConvertToString();

                    if (subItems[4].ConvertToDateTime() != null)
                        heroQuestStatus.QuestStartedAt = subItems[4].ConvertToDateTime()?.ToLocalTime();

                    heroQuestStatus.StartBlock = subItems[5].ConvertToInt();

                    if (subItems[6].ConvertToDateTime() != null)
                        heroQuestStatus.QuestCompletesAt = subItems[6].ConvertToDateTime()?.ToLocalTime();


                    heroQuestStatus.Attempts = subItems[7].ConvertToInt();
                    heroQuestStatus.Status = subItems[8].ConvertToInt();

                    //Change to true to dump out of method
                    foundHeroQuestData = true;
                    timesCheckingForQuestStatus++;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return heroQuestStatus;
        }

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


        public async Task<bool> CancelQuesting(DfkWallet wallet, int heroId)
        {
            try
            {
                var response = await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>(
                    "/api/quest/cancel",
                    new QuestStartRequest
                    {
                        Wallet = new SmallWalletItem
                        {
                            Name = wallet.Name,
                            Address = wallet.Address,
                            PrivateKey = wallet.PrivateKey,
                            PublicKey = wallet.PublicKey,
                            MnemonicPhrase = wallet.MnemonicPhrase
                        },
                        HeroId = heroId
                    });

                if (response != null)
                    return response.Success;

                /*
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl)
                {
                    TransactionManager =
                    {
                        UseLegacyAsDefault = true,
                        EstimateOrSetDefaultGasIfNotSet = true
                    }
                };

                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Quest),
                    Settings.Default.QuestContractAddress.Trim());
                var gas = new HexBigInteger(new BigInteger(400000));
                var value = new HexBigInteger(new BigInteger(0));

                var receiptForSetFunctionCall = await contract.GetFunction("cancelQuest")
                    .SendTransactionAndWaitForReceiptAsync(account.Address, gas, value, null, heroId);
                return !string.IsNullOrWhiteSpace(receiptForSetFunctionCall.TransactionHash);
                */
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public async Task<bool> CompleteQuesting(DfkWallet wallet, int heroId)
        {
            try
            {
                var response = await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>(
                    "/api/quest/complete",
                    new QuestStartRequest
                    {
                        Wallet = new SmallWalletItem
                        {
                            Name = wallet.Name,
                            Address = wallet.Address,
                            PrivateKey = wallet.PrivateKey,
                            PublicKey = wallet.PublicKey,
                            MnemonicPhrase = wallet.MnemonicPhrase
                        },
                        HeroId = heroId
                    });

                if(response != null)
                    return response.Success;

                /*
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl)
                {
                    TransactionManager =
                    {
                        UseLegacyAsDefault = true,
                        EstimateOrSetDefaultGasIfNotSet = true
                    }
                };

                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Quest),
                    Settings.Default.QuestContractAddress.Trim());
                var gas = new HexBigInteger(new BigInteger(400000));
                var value = new HexBigInteger(new BigInteger(0));

                var receiptForSetFunctionCall = await contract.GetFunction("cancelQuest")
                    .SendTransactionAndWaitForReceiptAsync(account.Address, gas, value, null, heroId);
                return !string.IsNullOrWhiteSpace(receiptForSetFunctionCall.TransactionHash);
                */
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public async Task<bool> StartQuesting(DfkWallet wallet, int heroId)
        {
            try
            {
                var response = await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>(
                    "/api/quest/start",
                    new QuestStartRequest
                    {
                        Wallet = new SmallWalletItem
                        {
                            Name = wallet.Name,
                            Address = wallet.Address,
                            PrivateKey = wallet.PrivateKey,
                            PublicKey = wallet.PublicKey,
                            MnemonicPhrase = wallet.MnemonicPhrase
                        },
                        HeroId = heroId
                    });

                if (response != null)
                    return response.Success;

                /*
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl)
                {
                    TransactionManager =
                    {
                        UseLegacyAsDefault = true,
                        EstimateOrSetDefaultGasIfNotSet = true
                    }
                };

                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Quest),
                    Settings.Default.QuestContractAddress.Trim());

                var gas = new HexBigInteger(new BigInteger(400000));
                var value = new HexBigInteger(new BigInteger(0));

                var contractFunction = contract.GetFunction("startQuest");


                var receiptForSetFunctionCall = await contractFunction
                    .SendTransactionAndWaitForReceiptAsync(account.Address, gas, value, null, new[] { heroId }, Settings.Default.MiningQuestContractAddress.Trim(), 5);
                return !string.IsNullOrWhiteSpace(receiptForSetFunctionCall.TransactionHash);
                */
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public async Task<QuestAddressToType> GetQuestType(Account account, QuestEngine.QuestTypes questType)
        {
            try
            {
                var questAddress = "";

                switch (questType)
                {
                    case QuestEngine.QuestTypes.Fishing:
                        questAddress = Settings.Default.FishingQuestContractAddress;
                        break;

                    case QuestEngine.QuestTypes.Foraging:
                        questAddress = Settings.Default.ForagingQuestContractAddress;
                        break;

                    case QuestEngine.QuestTypes.Mining:
                        questAddress = Settings.Default.MiningQuestContractAddress;
                        break;

                    case QuestEngine.QuestTypes.WishingWell:
                        questAddress = Settings.Default.WishingWellContractAddress;
                        break;
                }

                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                //Lets run some routines to get info about each account
                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Quest),
                    Settings.Default.QuestContractAddress);
                var contractFunction = contract.GetFunction("questAddressToType");
                var contractResult =
                    await contractFunction.CallDecodingToDefaultAsync(questAddress);

                if (contractResult != null && contractResult.Count > 0)
                {
                    return new QuestAddressToType
                    {
                        TypeId = contractResult[0].ConvertToInt()
                    };
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }
}


//transactionHash = await multiplyFunction.SendTransactionAsync(senderAddress, 7);
// var txHash = await contractFunction.SendTransactionAsync(wallet.Address, wallet.AssignedHero);
//var receipt = await MineAndGetReceiptAsync(web3, txHash);