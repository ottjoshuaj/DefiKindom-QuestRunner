using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class WalletCreateRequest
    {
        [JsonProperty("namePrefix")]
        public string NamePrefix { get; set; }

        [JsonProperty("createAmount")]
        public int CreateAmount { get; set; }
    }
}
