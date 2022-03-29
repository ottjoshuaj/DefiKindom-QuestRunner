using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class GeneralTransactionResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("receipt")]
        public object Receipt { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }

        [JsonProperty("serverKey")]
        public string UniqueServerKey { get; set; }
    }
}
