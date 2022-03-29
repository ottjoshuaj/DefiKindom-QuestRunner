using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.ApiHandler.Objects
{
    internal class FundWalletWithOneResponse : GeneralTransactionResponse
    {

        [JsonProperty("balance")]
        public decimal Balance { get; set; }
    }
}
