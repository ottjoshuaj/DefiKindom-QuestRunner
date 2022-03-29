using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using DefiKindom_QuestRunner.Objects;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using DefiKindom_QuestRunner.Properties;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class HeroContractHandler
    {
        public async Task<List<int>> GetWalletHeroes(Account account)
        {
            try
            {
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Hero),
                    Settings.Default.HeroContractAddress);
                var contractFunction = contract.GetFunction("getUserHeroes");
                var contractResult =
                    await contractFunction.CallDecodingToDefaultAsync(account.Address);

                if (contractResult != null && contractResult.Count > 0)
                    return contractResult[0].ConvertToIntList();
            }
            catch (Exception ex)
            {

            }

            return new List<int>();
        }


        public async Task<HeroProfile> GetHeroDetails(Account account, int heroId)
        {
            try
            {
                var web3 = new Web3(account, Settings.Default.CurrentRpcUrl);

                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Hero),
                    Settings.Default.HeroContractAddress);
                var contractFunction = contract.GetFunction("getHero");
                var contractResult = await contractFunction.CallDecodingToDefaultAsync(heroId);

                if (contractResult != null && contractResult.Count > 0)
                {
                    if (contractResult[0].Result is List<ParameterOutput> profileData)
                    {
                        var profileId = profileData[0].ConvertToInt();

                        var profileSummoningInfo = profileData[1].ConvertToParamOutputList();
                        var profileInfo = profileData[2].ConvertToParamOutputList();
                        var profileState = profileData[3].ConvertToParamOutputList();
                        var profileStats = profileData[4].ConvertToParamOutputList();
                        var profilePrimaryStatGrowth = profileData[5].ConvertToParamOutputList();
                        var secondaryPrimaryStatGrowth = profileData[6].ConvertToParamOutputList();
                        var profileProfessions = profileData[7].ConvertToParamOutputList();

                        return new HeroProfile
                        {
                            Id = profileId,
                            SummonInfo = new HeroSummonInfo
                            {
                                SummonedTime = profileSummoningInfo[0].ConvertToDateTime(),
                                NextSummonTime = profileSummoningInfo[1].ConvertToDateTime(),
                                SummonerId = profileSummoningInfo[2].ConvertToInt(),
                                AssistantId = profileSummoningInfo[3].ConvertToInt(),
                                Summons = profileSummoningInfo[4].ConvertToInt(),
                                MaxSummons = profileSummoningInfo[5].ConvertToInt()
                            },
                            Info = new HeroInfo
                            {
                                StatGenes = profileInfo[0].ConvertToInt(),
                                VisualGenes = profileInfo[1].ConvertToInt(),
                                Rarity = profileInfo[2].ConvertToInt(),
                                Shiny = profileInfo[3].ConvertToBool(),
                                Generation = profileInfo[4].ConvertToInt(),
                                FirstName = profileInfo[5].ConvertToInt(),
                                LastName = profileInfo[6].ConvertToInt(),
                                ShinyStyle = profileInfo[7].ConvertToInt(),
                                Class = profileInfo[8].ConvertToInt(),
                                SubClass = profileInfo[9].ConvertToInt()
                            },
                            State = new HeroState
                            {
                                StaminaFullAt = profileState[0].ConvertToDateTime(),
                                HPFullAt = profileState[1].ConvertToDateTime(),
                                MPFullAt = profileState[2].ConvertToDateTime(),
                                Level = profileState[3].ConvertToInt(),
                                XP = profileState[4].ConvertToInt(),
                                CurrentQuest = profileState[5].ConvertToString(),
                                SP = profileState[6].ConvertToInt(),
                                Status = profileState[7].ConvertToInt(),
                            },
                            Stats = new HeroStats
                            {
                                Strength = profileStats[0].ConvertToInt(),
                                Intelligence = profileStats[1].ConvertToInt(),
                                Wisdom = profileStats[2].ConvertToInt(),
                                Luck = profileStats[3].ConvertToInt(),
                                Agility = profileStats[4].ConvertToInt(),
                                Vitality = profileStats[5].ConvertToInt(),
                                Endurance = profileStats[6].ConvertToInt(),
                                Dexterity = profileStats[7].ConvertToInt(),
                                HP = profileStats[8].ConvertToInt(),
                                MP = profileStats[9].ConvertToInt(),
                                Stamina = profileStats[10].ConvertToInt()
                            },
                            PrimaryStatGrowth = new HeroStateGrowth
                            {
                                Strength = profilePrimaryStatGrowth[0].ConvertToInt(),
                                Intelligence = profilePrimaryStatGrowth[1].ConvertToInt(),
                                Wisdom = profilePrimaryStatGrowth[2].ConvertToInt(),
                                Luck = profilePrimaryStatGrowth[3].ConvertToInt(),
                                Agility = profilePrimaryStatGrowth[4].ConvertToInt(),
                                Vitality = profilePrimaryStatGrowth[5].ConvertToInt(),
                                Endurance = profilePrimaryStatGrowth[6].ConvertToInt(),
                                Dexterity = profilePrimaryStatGrowth[7].ConvertToInt(),

                                HPSm = profilePrimaryStatGrowth[8].ConvertToInt(),
                                HPRg = profilePrimaryStatGrowth[9].ConvertToInt(),
                                HPLg = profilePrimaryStatGrowth[10].ConvertToInt(),
                                MPSm = profilePrimaryStatGrowth[11].ConvertToInt(),
                                MPRg = profilePrimaryStatGrowth[12].ConvertToInt(),
                                MPLg = profilePrimaryStatGrowth[13].ConvertToInt()
                            },
                            SecondaryStatGrowth = new HeroStateGrowth
                            {
                                Strength = secondaryPrimaryStatGrowth[0].ConvertToInt(),
                                Intelligence = secondaryPrimaryStatGrowth[1].ConvertToInt(),
                                Wisdom = secondaryPrimaryStatGrowth[2].ConvertToInt(),
                                Luck = secondaryPrimaryStatGrowth[3].ConvertToInt(),
                                Agility = secondaryPrimaryStatGrowth[4].ConvertToInt(),
                                Vitality = secondaryPrimaryStatGrowth[5].ConvertToInt(),
                                Endurance = secondaryPrimaryStatGrowth[6].ConvertToInt(),
                                Dexterity = secondaryPrimaryStatGrowth[7].ConvertToInt(),

                                HPSm = secondaryPrimaryStatGrowth[8].ConvertToInt(),
                                HPRg = secondaryPrimaryStatGrowth[9].ConvertToInt(),
                                HPLg = secondaryPrimaryStatGrowth[10].ConvertToInt(),
                                MPSm = secondaryPrimaryStatGrowth[11].ConvertToInt(),
                                MPRg = secondaryPrimaryStatGrowth[12].ConvertToInt(),
                                MPLg = secondaryPrimaryStatGrowth[13].ConvertToInt()
                            },
                            Professions = new HeroStateProfessions
                            {
                                Mining = profileProfessions[0].ConvertToInt(),
                                Gardening = profileProfessions[1].ConvertToInt(),
                                Foraging = profileProfessions[2].ConvertToInt(),
                                Fishing = profileProfessions[3].ConvertToInt()
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return null;
        }


        public async Task<bool> SendHeroToWallet(DfkWallet wallet, Account destination, int heroId)
        {
            try
            {
                var response = await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>(
                    "/api/hero/transfer",
                    new TransferHeroRequest
                    {
                        Wallet = new SmallWalletItem
                        {
                            Name = wallet.Name,
                            Address = wallet.Address,
                            PrivateKey = wallet.PrivateKey,
                            PublicKey = wallet.PublicKey,
                            MnemonicPhrase = wallet.MnemonicPhrase
                        },
                        DestinationAddress = destination.Address, HeroId = heroId
                    });

                if (response != null)
                    return response.Success;

                /*
                var web3 = new Web3(source, Settings.Default.CurrentRpcUrl)
                {
                    TransactionManager =
                    {
                        UseLegacyAsDefault = true,
                        CalculateOrSetDefaultGasPriceFeesIfNotSet = true
                    }
                };

                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Hero),
                    Settings.Default.HeroContractAddress);

                var gas = new HexBigInteger(new BigInteger(400000));
                var value = new HexBigInteger(new BigInteger(0));
                var transaction = contract.GetFunction("transferFrom").SendTransactionAsync(source.Address, gas, value, destination.Address);
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
