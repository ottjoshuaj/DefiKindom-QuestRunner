using DefiKindom_QuestRunner.EngineManagers.Engines;

namespace DefiKindom_QuestRunner
{
    internal class WalletQuestStatusEvent
    {
        public WalletQuestStatusEvent()
        {
            CurrentActivityMode = QuestEngine.QuestActivityMode.Ignore;
        }

        public string WalletAddress { get; set; }

        public int HeroStamina { get; set; }

        public int HeroId { get; set; }

        public QuestEngine.QuestActivityMode CurrentActivityMode { get; set; }

        public string ReadableActivityMode => CurrentActivityMode.ToString();
    }
}
