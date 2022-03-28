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

        public NeedJewelEvent(DfkWallet wallet, JewelEventRequestTypes type)
        {
            Wallet = wallet;
            RequestType = type;
        }

        public JewelEventRequestTypes RequestType { get; set; }

        public DfkWallet Wallet { get; }
    }
}
