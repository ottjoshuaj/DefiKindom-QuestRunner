using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    //TODO: Add support later for other quest types
    internal class QuestStartRequest
    {
        [JsonProperty("wallet")]
        public SmallWalletItem Wallet { get; set; }

        [JsonProperty("heroId")]
        public int HeroId { get; set; }
    }
}
