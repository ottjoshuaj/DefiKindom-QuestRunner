﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using DefiKindom_QuestRunner.Managers;
using DefiKindom_QuestRunner.Managers.Contracts;
using DefiKindom_QuestRunner.Objects;
using DefiKindom_QuestRunner.Properties;

using Nethereum.Web3;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class NftContractHandler
    {
        public async Task<NftBalanceInfo> GetNftBalances(DfkWallet wallet)
        {
            var nftBalance = new NftBalanceInfo
            {
                Tears = await GetNftBalance(wallet, ProfileContractHandler.NftTypes.Tears),
                YellowEggs = await GetNftBalance(wallet, ProfileContractHandler.NftTypes.YellowEgg),
                ShvasRunes = await GetNftBalance(wallet, ProfileContractHandler.NftTypes.ShvasRune)
            };

            return nftBalance;
        }

        async Task<int> GetNftBalance(DfkWallet wallet, ProfileContractHandler.NftTypes nftType)
        {
            try
            {
                var web3 = new Web3(wallet.WalletAccount, Settings.Default.CurrentRpcUrl);
                var contract = web3.Eth.GetContract(AbiManager.GetAbi(AbiManager.AbiTypes.Erc20),
                    GetNftContractAddress(nftType));
                var contractFunction = contract.GetFunction("balanceOf");
                var contractResult = await contractFunction.CallAsync<BigInteger>(wallet.Address);

                return (int)contractResult;
            }
            catch
            {

            }

            return 0;
        }

        public async Task<bool> TransferNft(DfkWallet wallet, string destAddress, ProfileContractHandler.NftTypes nftType, int amount)
        {
            try
            {
                var response = await new QuickRequest().GetDfkApiResponse<GeneralTransactionResponse>(
                    "/api/nft/transfer", new NftTransferRequest
                    {
                        Wallet = new SmallWalletItem
                        {
                            Name = wallet.Name,
                            Address = wallet.Address,
                            PrivateKey = wallet.PrivateKey,
                            PublicKey = wallet.PublicKey,
                            MnemonicPhrase = wallet.MnemonicPhrase
                        },
                        DestinationAddress = destAddress,
                        NftAddress = GetNftContractAddress(nftType),
                        Amount = amount
                    });

                if (response != null)
                    return response.Success;
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        string GetNftContractAddress(ProfileContractHandler.NftTypes nftType)
        {
            switch (nftType)
            {
                case ProfileContractHandler.NftTypes.ShvasRune:
                    return "0x66F5BfD910cd83d3766c4B39d13730C911b2D286";
                case ProfileContractHandler.NftTypes.Tears:
                    return "0x24eA0D436d3c2602fbfEfBe6a16bBc304C963D04";
                case ProfileContractHandler.NftTypes.YellowEgg:
                    return "0x3dB1fd0Ad479A46216919758144FD15A21C3e93c";
            }

            return "";
        }
    }
}