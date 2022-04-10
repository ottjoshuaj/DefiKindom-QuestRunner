using System;
using System.Collections.Generic;
using System.Linq;
using NBitcoin.Protocol;
using Newtonsoft.Json;

namespace DefiKindom_QuestRunner.Objects
{
    
    public class DfkWallet
    {
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

        [JsonProperty("heroesInWallet")]
        public List<int> AvailableHeroes { get; set; }

        [JsonIgnore]
        public int AssignedHero {
            get
            {
                if (AvailableHeroes != null && AvailableHeroes.Count > 0)
                    return AvailableHeroes.First();

                return 0;
            }
        }

        [JsonProperty("assignedHeroStamina")]
        public int AssignedHeroStamina { get; set; }

        [JsonProperty("currentBalance")]
        public decimal CurrentBalance { get; set; }

        [JsonProperty("isHoldingTheJewel")]
        public bool IsHoldingTheJewel { get; set; }

        [JsonProperty("jewelBalance")]
        public decimal JewelBalance { get; set; }

        [JsonProperty("dfkProfile")]
        public object DfkProfile { get; set; }

        [JsonProperty("assignedHeroQuestStatus")]
        public DkHeroQuestStatus AssignedHeroQuestStatus { get; set; }

        [JsonProperty("heroProfiles")]
        public List<HeroProfile> HeroProfiles { get; set; }

        [JsonIgnore]
        public Nethereum.Web3.Accounts.Account WalletAccount { get; set; }

        [JsonIgnore]

        public DateTime? QuestStartedAt => AssignedHeroQuestStatus?.QuestStartedAt;

        [JsonIgnore]

        public DateTime? QuestCompletesAt => AssignedHeroQuestStatus?.QuestCompletesAt;


        [JsonIgnore]
        public bool QuestNeedsCompleted
        {
            get
            {
                //If no quest status then we know we're not questing!
                if (AssignedHeroQuestStatus == null) return false;

                //Check to see the time rIGHT NOW is equal to or GREATEr then the time
                //DFK says the quest completes at
                return DateTime.Now >= AssignedHeroQuestStatus.QuestCompletesAt;
            }
        }

        [JsonIgnore]
        public bool QuestNeedsCanceled
        {
            get
            {
                //If no quest status then we know we're not questing!
                if (AssignedHeroQuestStatus == null) return false;

                //Check time from start to finish...if its past the amount of time... need canceled ?
                var now = DateTime.Now;
                var timeBetweenStartAndNow = now.Subtract(AssignedHeroQuestStatus.QuestStartedAt.GetValueOrDefault());

                return timeBetweenStartAndNow.TotalMinutes >= 155;
            }
        }

        //If no quest status then we know we're not questing! //TOdO: FIX THIS, BASED ON ASSIGNED HERO DETAILS
        [JsonIgnore]
        public bool IsQuesting
        {
            get
            {
                if (AssignedHeroQuestStatus == null) return false;
                return AssignedHeroQuestStatus.ContractAddress != "0x0000000000000000000000000000000000000000";
            }
        }


        [JsonIgnore]
        public bool ReadyToWork => HasDkProfile && CurrentBalance > 1 && AssignedHero > 0;

        [JsonIgnore]
        public bool HasDkProfile => DfkProfile != null;

        [JsonIgnore]
        public int TotalAccountHeroes => AvailableHeroes?.Count ?? 0;

        [JsonIgnore]
        public bool HasAssignedHero => AssignedHero > 0;
    }
}