using System;
using System.Threading;

using DefiKindom_QuestRunner.Managers.Engines;

namespace DefiKindom_QuestRunner.Managers
{
    internal class QuestEngineItem
    {
        public QuestEngineItem()
        {
            UniqueId = Guid.NewGuid().ToString();
        }

        public string UniqueId { get; }
        public QuestEngine Engine { get; set; }
        public Thread ExecutingThread { get; set; }
    }
}
