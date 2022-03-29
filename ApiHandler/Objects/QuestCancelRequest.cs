using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class QuestCancelRequest
    {
        [JsonProperty("wallet")]
        public SmallWalletItem Wallet { get; set; }

        [JsonProperty("heroId")]
        public int HeroId { get; set; }
    }
}
