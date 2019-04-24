using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZachLib;
using ZachLib.Logging;

namespace LoggerTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.AddLog("Test", LogType.FolderFilesByDate);
            LogManager.Start();
            Random gen = new Random();
            for (int i = 0; i < 16; ++i)
            {
                LogManager.Enqueue("Test", EntryType.DEBUG, new object[] { gen.NextDouble() });
            }
            LogManager.Stop();
            SpinWait.SpinUntil(() => !LogManager.hasWork);
        }
    }
}
