using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class TransferHeroRequest
    {
        [JsonProperty("wallet")]
        public SmallWalletItem Wallet { get; set; }

        [JsonProperty("destinationAddress")]
        public string DestinationAddress { get; set; }

        [JsonProperty("heroId")]
        public int HeroId { get; set; }
    }
}
