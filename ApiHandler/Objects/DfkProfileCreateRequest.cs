using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class DfkProfileCreateRequest
    {
        [JsonProperty("wallet")]
        public SmallWalletItem Wallet { get; set; }
    }
}
