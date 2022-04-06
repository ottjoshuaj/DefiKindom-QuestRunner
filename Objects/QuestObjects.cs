using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace DefiKindom_QuestRunner
{
    public class DkHeroQuestStatus
    {
        public DkHeroQuestStatus()
        {
            HeroesOnQuest = new List<int>();
        }

        public string Id { get; set; }
        public string ContractAddress { get; set; }
        public List<int> HeroesOnQuest { get; set; }
        public string PlayerAddress { get; set; }
        public DateTime? QuestStartedAt { get; set; }
        public DateTime? QuestCompletesAt { get; set; }
        public int StartBlock { get; set; }
        public int Attempts { get; set; }
        public int Status { get; set; }

        [JsonIgnore]
        public bool IsQuesting
        {
            get
            {
                if (ContractAddress.Trim() == "0x0000000000000000000000000000000000000000")
                    return false;

                return true;
            }
        }

        [JsonIgnore]
        public bool WantsToComplete => DateTime.Now >= QuestCompletesAt;

        [JsonIgnore]
        public bool WantsToCancel
        {
            get
            {
                //Check time from start to finish...if its past the amount of time... need canceled ?
                var now = DateTime.Now;
                var timeBetweenStartAndNow = now.Subtract(QuestStartedAt.GetValueOrDefault());

                return timeBetweenStartAndNow.TotalMinutes >= 155;
            }
        }
    }


    public class QuestAddressToType
    {
        public int TypeId { get; set; }
    }

    public class DfkHeroInformation
    {

    }
}
