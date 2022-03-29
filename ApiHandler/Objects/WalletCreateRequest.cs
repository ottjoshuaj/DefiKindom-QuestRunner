using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class WalletCreateRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
