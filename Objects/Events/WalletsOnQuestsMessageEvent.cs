using DefiKindom_QuestRunner.Objects;

namespace DefiKindom_QuestRunner
{
    internal class WalletsOnQuestsMessageEvent
    {
        public enum OnQuestMessageEventTypes
        {
            Questing,
            QuestingCanceled,
            WaitingOnStamina
        }

        public DfkWallet Wallet { get; }

        public OnQuestMessageEventTypes OnQuestMessageEventType { get; }

        public WalletsOnQuestsMessageEvent(DfkWallet wallet, OnQuestMessageEventTypes eventType)
        {
            Wallet = wallet;
            OnQuestMessageEventType = eventType;
        }
    }
}
