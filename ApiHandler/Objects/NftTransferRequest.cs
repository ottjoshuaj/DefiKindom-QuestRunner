using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class NftTransferRequest
    {
        [JsonProperty("wallet")]
        public SmallWalletItem Wallet { get; set; }

        [JsonProperty("destinationAddress")]
        public string DestinationAddress { get; set; }

        [JsonProperty("nftAddress")]
        public string NftAddress { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }
    }
}