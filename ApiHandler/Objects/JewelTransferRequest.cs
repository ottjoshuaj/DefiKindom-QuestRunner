using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class JewelTransferRequest
    {
        [JsonProperty("wallet")]
        public SmallWalletItem Wallet { get; set; }

        [JsonProperty("destinationAddress")]
        public string DestinationAddress { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}
