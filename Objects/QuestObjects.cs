﻿using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace DefiKindom_QuestRunner
{
    internal class DkHeroQuestStatus
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
        public bool IsQuesting => Status == 1 && ContractAddress != "0x0000000000000000000000000000000000000000";
    }


    internal class QuestAddressToType
    {
        public int TypeId { get; set; }
    }

    internal class DfkHeroInformation
    {

    }
}