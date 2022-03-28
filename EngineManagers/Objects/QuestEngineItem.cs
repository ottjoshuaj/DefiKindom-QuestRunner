using System;
using System.Threading;

using DefiKindom_QuestRunner.EngineManagers.Engines;

namespace DefiKindom_QuestRunner.EngineManagers
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
