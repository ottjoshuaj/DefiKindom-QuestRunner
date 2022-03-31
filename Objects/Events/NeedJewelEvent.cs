using DefiKindom_QuestRunner.EngineManagers.Engines;
using DefiKindom_QuestRunner.Objects;

namespace DefiKindom_QuestRunner
{
    internal class NeedJewelEvent
    {
        public enum JewelEventRequestTypes
        {
            NeedJewel,
            FinishedWithJewel,
            JewelMovedOffAccount,
            JewelMovedToAccount
        }

        public NeedJewelEvent(DfkWallet wallet, JewelEventRequestTypes type, QuestEngine.QuestActivityMode currentActivity)
        {
            Wallet = wallet;
            RequestType = type;
            QuestActivityMode = currentActivity;
        }

        public QuestEngine.QuestActivityMode QuestActivityMode { get; set; }

        public JewelEventRequestTypes RequestType { get; set; }

        public DfkWallet Wallet { get; }
    }
}
