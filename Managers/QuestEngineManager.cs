using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

using DefiKindom_QuestRunner.Managers.Engines;
using DefiKindom_QuestRunner.Objects;


namespace DefiKindom_QuestRunner.Managers
{
    internal static class QuestEngineManager
    {
        #region Internals

        internal static List<QuestEngineItem> QuestEnginesRunning = new List<QuestEngineItem>();

        #endregion

        #region Properties

        public static int Count
        {
            get
            {
                lock(QuestEnginesRunning)
                    return QuestEnginesRunning.Count;
            }
        }

        #endregion

        #region Public Methods

        public static List<QuestEngineItem> GetAllQuestInstances()
        {
            lock (QuestEnginesRunning)
                return QuestEnginesRunning;
        }

        public static void AddQuestEngine(QuestEngine engine)
        {
            lock (QuestEnginesRunning)
            {
                if (!QuestEnginesRunning.Exists(x =>
                        x.Engine.DfkWallet.Address.ToUpper() == engine.DfkWallet.Address.ToUpper()))
                {
                    var newQuestEngineInstance = new QuestEngineItem
                    {
                        Engine = engine,
                        ExecutingThread = new Thread(engine.Start)
                        {
                            IsBackground = true
                        }
                    };

                    //Fire Thread
                    newQuestEngineInstance.ExecutingThread.Start();

                    //Add to internal management list
                    QuestEnginesRunning.Add(newQuestEngineInstance);
                }
            }
        }

        public static void RemoveQuestEngine(DfkWallet wallet, QuestEngine.QuestTypes type)
        {
            var instanceToRemove = GetQuestEngineInstance(wallet.Address, type);
            if (instanceToRemove != null)
            {
                DestroyInstance(instanceToRemove);

                lock (QuestEnginesRunning)
                    QuestEnginesRunning.Remove(instanceToRemove);
            }
        }

        public static QuestEngineItem GetQuestEngineInstance(string address, QuestEngine.QuestTypes type)
        {
            lock (QuestEnginesRunning)
                return QuestEnginesRunning.FirstOrDefault(x => x.Engine.DfkWallet.Address.Trim().ToUpper() == address.Trim().ToUpper() && x.Engine.QuestType == type);
        }

        public static void KillAllInstances()
        {
            lock (QuestEnginesRunning)
            {
                //Loop through and kill all instance
                foreach (var instance in QuestEnginesRunning)
                    DestroyInstance(instance);

                //Clear list
                QuestEnginesRunning.Clear();
            }
        }

        #endregion

        #region Internals

        static void DestroyInstance(QuestEngineItem engine)
        {
            //Stop the instance & dispose
            try
            {
                engine.Engine.Stop();
                engine.ExecutingThread.Abort();
                engine.ExecutingThread = null;
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}

