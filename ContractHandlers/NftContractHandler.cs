using System;
using System.Numerics;

using System.Threading.Tasks;
using DefiKindom_QuestRunner.ApiHandler;
using DefiKindom_QuestRunner.ApiHandler.Objects;
using DefiKindom_QuestRunner.Managers.Contracts.Base;
using DefiKindom_QuestRunner.Objects;

namespace DefiKindom_QuestRunner.Managers.Contracts
{
    internal class NftContractHandler : BaseContract
    {
        public enum NftTypes
        {
            YellowEgg,
            ShvasRune,
            Tears
        }

        public async Task<NftBalanceInfo> GetNftBalances(DfkWallet wallet)
        {
            var nftBalance = new NftBalanceInfo
            {
                Tears = await GetNftBalance(wallet, NftTypes.Tears),
                YellowEggs = await GetNftBalance(wallet, NftTypes.YellowEgg),
                ShvasRunes = await GetNftBalance(wallet, NftTypes.ShvasRune)
            };

            return nftBalance;
        }

        async Task<int> GetNftBalance(DfkWallet wallet, NftTypes nftType)
        {
            try
            {
                var contractFunction = BuildContract(wallet.WalletAccount, AbiManager.AbiTypes.Erc20,
                    GetNftContractAddress(nftType), "balanceOf");
                var contractResult = await contractFunction.CallAsync<BigInteger>(wallet.Address);

                return (int) contractResult;
            }
            catch
            {

            }

            return 0;
        }

        public async Task<bool> TransferNft(DfkWallet wallet, string destAddress, NftTypes nftType, int amount)
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

        string GetNftContractAddress(NftTypes nftType)
        {
            switch (nftType)
            {
                case NftTypes.ShvasRune:
                    return "0x66F5BfD910cd83d3766c4B39d13730C911b2D286";
                case NftTypes.Tears:
                    return "0x24eA0D436d3c2602fbfEfBe6a16bBc304C963D04";
                case NftTypes.YellowEgg:
                    return "0x3dB1fd0Ad479A46216919758144FD15A21C3e93c";
            }

            return "";
        }
    }
}