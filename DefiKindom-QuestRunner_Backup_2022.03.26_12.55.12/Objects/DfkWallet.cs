using System;

using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.Objects
{
    
    internal class DfkWallet
    {
        [JsonProperty("keyId")]
        public int KeyId { get; set; }

        [JsonProperty("isPrimarySourceWallet")]
        public bool IsPrimarySourceWallet { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }

        [JsonProperty("privateKey")]
        public string PrivateKey { get; set; }

        [JsonProperty("mnemonicPhrase")]
        public string MnemonicPhrase { get; set; }

        [JsonProperty("walletPassword")]
        public string WalletPassword { get; set; }

        [JsonProperty("availableHeroes")]
        public DfkHeroList AvailableHeroes { get; set; }

        [JsonProperty("assignedHero")]
        public int AssignedHero { get; set; }

        [JsonProperty("assignedHeroStamina")]
        public int AssignedHeroStamina { get; set; }

        [JsonProperty("isOnQuest")]
        public bool IsOnQuest { get; set; }

        [JsonProperty("questStartedAt")]
        public DateTime QuestStartedAt { get; set; }

        [JsonProperty("questCompletesAt")]
        public DateTime QuestCompletesAt { get; set; }

        [JsonProperty("currentBalance")]
        public decimal CurrentBalance { get; set; }

        [JsonProperty("isHoldingTheJewel")]
        public bool IsHoldingTheJewel { get; set; }

        [JsonProperty("jewelBalance")]
        public string JewelBalance { get; set; }

        [JsonProperty("dfkProfile")]
        public object DfkProfile { get; set; }

        [JsonIgnore]
        public Nethereum.Web3.Accounts.Account WalletAccount { get; set; }

        public object Enabled { get; set; }
    }


    internal class DfkHeroList
    {
        [JsonProperty("heroes")]
        public int[] HeroIds { get; set; }
    }

    internal class DfkHero
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}