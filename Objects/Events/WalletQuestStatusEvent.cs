using System;
using DefiKindom_QuestRunner.EngineManagers.Engines;

namespace DefiKindom_QuestRunner
{
    internal class WalletQuestStatusEvent
    {
        public WalletQuestStatusEvent()
        {
            CurrentActivityMode = QuestEngine.QuestActivityMode.Ignore;
        }

        public string Name { get; set; }

        public string WalletAddress { get; set; }

        public string ContractAddress { get; set; }

        public int HeroStamina { get; set; }

        public int HeroId { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletesAt { get; set; }

        public QuestEngine.QuestActivityMode CurrentActivityMode { get; set; }

        public string ReadableActivityMode => CurrentActivityMode.ToString();
    }
}
