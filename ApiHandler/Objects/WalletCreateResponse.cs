using Newtonsoft.Json;

using DefiKindom_QuestRunner.Objects;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class WalletCreateResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("wallet")]
        public DfkWallet Wallet { get; set; }
    }
}
