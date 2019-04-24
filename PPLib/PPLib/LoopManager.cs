using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZachLib;
using ZachLib.Logging;

namespace PPLib
{
    public enum UpdateType
    {
        Finished,
        Error,
        Nonexistant
    }

    public class LoopManager
    {
        static LoopManager()
        {
            LogManager.AddLog("LoopManager", LogType.FolderFilesByDate);
        }

        public static readonly ParallelOptions PARALLEL_OPTS = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 8
        };

        public static readonly object PARALLEL_LOCK = new object();

        public int updatesCount = 0;
        public string[] completed { get; private set; }
        public StreamWriter finishedWriter { get; private set; }
        public StreamWriter erroredWriter { get; private set; }
        public ConcurrentBag<string> finished { get; private set; }
        public ConcurrentBag<KeyValuePair<string, string>> errored { get; private set; }
        public int updatesIter = 0;
        public int erroredCount = 0;
        public string[] Updates { get; private set; }

        public LoopManager(string updatesPath, string finishedPath, string erroredPath, Func<string, bool> isHeader, bool clearErrored = false)
        {
            LogManager.Enqueue("LoopManager", EntryType.DEBUG, "Reading files...");
            finished = new ConcurrentBag<string>();
            errored = new ConcurrentBag<KeyValuePair<string, string>>();

            IEnumerable<string> updates = File.ReadAllLines(updatesPath);
            if (isHeader(updates.First()))
                updates = updates.Skip(1);
            if (updates.First().Contains(','))
            {
                var split = updates.First().Split(',');
                int index = Array.FindIndex(split, u => !isHeader(u) && !u.Contains(','));
                updates = updates.Select(u => u.Split(',')[index]);
            }

            if (File.Exists(finishedPath))
            {
                string[] finishedLines = null;
                if (clearErrored)
                {
                    finishedLines = File.ReadAllLines(finishedPath);
                    File.Delete(erroredPath);
                }
                else
                    finishedLines = File.ReadAllLines(finishedPath).Concat(File.ReadAllLines(erroredPath)).ToArray();
                if (finishedLines != null && finishedLines.Any())
                {
                    Array.Sort(finishedLines);
                    updates = updates.Where(u => Array.BinarySearch(finishedLines, u) < 0);
                    if (!updates.Any() && !clearErrored)
                        updates = updates.Concat(File.ReadAllLines(erroredPath));
                }
                //finishedLines = null;
            }
            Updates = updates.ToArray();
            updatesCount = Updates.Length;

            LogManager.Enqueue(
                "LoopManager",
                EntryType.DEBUG,
                new object[]
                {
                    "Paths:",
                    "Updates\t- " + updatesPath + " - \t" + updatesCount,
                    "Finished\t- " + finishedPath + " - \t" + finished.Count,
                    "Errored\t- " + erroredPath + " - \t" + errored.Count
                }
            );

            finishedWriter = new StreamWriter(finishedPath) { AutoFlush = true };
            erroredWriter = new StreamWriter(erroredPath) { AutoFlush = true };
        }

        public void LogUpdate(string key, string display, UpdateType type = UpdateType.Finished)
        {
            LogUpdate update = null;
            switch (type)
            {
                case UpdateType.Error:
                    errored.Add(new KeyValuePair<string, string>(key, display));
                    update = new LogUpdate("LoopManager", EntryType.ERROR, true);
                    update.LogContent = new List<object>() { key, display };
                    update.Enqueue();
                    ++erroredCount;
                    break;

                case UpdateType.Finished:
                    finished.Add(key);
                    break;

                case UpdateType.Nonexistant:
                    if (!String.IsNullOrWhiteSpace(display))
                    {
                        update = new LogUpdate("LoopManager", EntryType.ERROR, true);
                        update.LogContent = new List<object>() { key, display };
                        update.Enqueue();
                    }
                    --updatesCount;
                    return;

            }

            lock (LoopManager.PARALLEL_LOCK)
            {
                ++updatesIter;
                if (updatesIter % 32 == 0)
                    LogUpdates();
            }
        }

        public void LogUpdates()
        {
            Console.WriteLine("\r\n\t~~~~~~~~\r\n");
            var logString = String.Format("\t---\t{0} down, {1} to go, {2} errors.\t---", updatesIter, updatesCount - updatesIter, erroredCount);
            Console.WriteLine(logString);
            LogManager.Enqueue("LoopManager", EntryType.DEBUG, "Logging updates...", logString);

            if (!errored.IsEmpty)
            {
                foreach (var erroredUpdate in errored.AsEnumerable())
                {
                    Console.WriteLine(erroredUpdate.Key.ToString() + " - " + erroredUpdate.Value);
                    erroredWriter.WriteLine(erroredUpdate.Key.ToString());
                }
            }

            Console.WriteLine("\r\n\t~~~~~~~~\r\n");
            foreach (var finishedUpdate in finished.AsEnumerable())
            {
                Console.WriteLine("\t" + finishedUpdate);
                finishedWriter.WriteLine(finishedUpdate);
            }

            errored = new ConcurrentBag<KeyValuePair<string, string>>();
            finished = new ConcurrentBag<string>();
        }

        public void EndLoop(ParallelLoopResult loop, bool readline = true)
        {
            Console.WriteLine("STARTED");
            SpinWait.SpinUntil(() => loop.IsCompleted);
            LogUpdates();
            finishedWriter.Close();
            finishedWriter = null;
            erroredWriter.Close();
            erroredWriter = null;
            LogManager.Enqueue("LoopManager", EntryType.DEBUG, "FINISHED");
            Console.WriteLine("FINISHED");
            if (readline)
                Console.ReadLine();
        }
    }

    public class LoopManager<T>
    {
        public int updatesCount = 0;
        public string[] completed { get; private set; }
        public StreamWriter finishedWriter { get; private set; }
        public StreamWriter erroredWriter { get; private set; }
        public ConcurrentBag<object> finished { get; private set; }
        public ConcurrentBag<KeyValuePair<object, string>> errored { get; private set; }
        public int updatesIter = 0;
        public int erroredCount = 0;
        public T[] Updates { get; private set; }
        public Func<T, object> keySelector { get; private set; }

        public LoopManager(string updatesPath, string finishedPath, string erroredPath, Func<T, object> keySelector, bool clearErrored = false)
        {
            this.keySelector = keySelector;
            LogManager.Enqueue("LoopManager", EntryType.DEBUG, "Reading files...");
            finished = new ConcurrentBag<object>();
            errored = new ConcurrentBag<KeyValuePair<object, string>>();
            T[] updates = Utils.LoadCSV<T>(updatesPath);
            if (File.Exists(finishedPath))
            {
                string[] finishedLines = null;
                if (clearErrored)
                {
                    finishedLines = File.ReadAllLines(finishedPath);
                    File.Delete(erroredPath);
                }
                else
                    finishedLines = File.ReadAllLines(finishedPath).Concat(File.ReadAllLines(erroredPath)).ToArray();
                Array.Sort(finishedLines);
                updates = updates.Where(u => Array.BinarySearch(finishedLines, keySelector(u).ToString()) < 0).ToArray();
                //finishedLines = null;
            }
            updatesCount = updates.Length;

            LogManager.Enqueue(
                "LoopManager",
                EntryType.DEBUG,
                new object[]
                {
                    "Paths:",
                    "Updates\t- " + updatesPath + " - \t" + updatesCount,
                    "Finished\t- " + finishedPath + " - \t" + finished.Count,
                    "Errored\t- " + erroredPath + " - \t" + errored.Count
                }
            );
            
            finishedWriter = new StreamWriter(finishedPath) { AutoFlush = true };
            erroredWriter = new StreamWriter(erroredPath) { AutoFlush = true };
            Updates = updates;
        }

        /*public static Dictionary<string, string> StartLoop(string updatesPath, string finishedPath, string erroredPath)
        {
            finished = new ConcurrentBag<string>();
            errored = new ConcurrentBag<string>();
            Dictionary<string, string> updates = Utils.LoadCSVDictionary(updatesPath);
            if (File.Exists(finishedPath))
            {
                var finishedLines = File.ReadAllLines(finishedPath).Concat(File.ReadLines(erroredPath)).ToArray();
                Array.Sort(finishedLines);
                foreach (var key in updates.Keys.Where(u => Array.BinarySearch(finishedLines, u) >= 0).ToArray())
                {
                    updates.Remove(key);
                }
            }
            updatesCount = updates.Count;
            finishedWriter = new StreamWriter(finishedPath) { AutoFlush = true };
            erroredWriter = new StreamWriter(erroredPath) { AutoFlush = true };
            return updates;
        }*/

        public void LogUpdate(object key, string display, UpdateType type = UpdateType.Finished)
        {
            LogUpdate update = null;
            switch (type)
            {
                case UpdateType.Error:
                    errored.Add(new KeyValuePair<object, string>(key, display));
                    update = new LogUpdate("LoopManager", EntryType.ERROR, true);
                    update.LogContent = new List<object>() { key, display };
                    update.Enqueue();
                    ++erroredCount;
                    break;

                case UpdateType.Finished:
                    finished.Add(key);
                    break;

                case UpdateType.Nonexistant:
                    if (!String.IsNullOrWhiteSpace(display))
                    {
                        update = new LogUpdate("LoopManager", EntryType.ERROR, true);
                        update.LogContent = new List<object>() { key, display };
                        update.Enqueue();
                    }
                    --updatesCount;
                    return;

            }

            lock (LoopManager.PARALLEL_LOCK)
            {
                ++updatesIter;
                if (updatesIter % 32 == 0)
                    LogUpdates();
            }
        }

        public void LogUpdate(T obj, string display = null, UpdateType type = UpdateType.Finished) => LogUpdate(keySelector(obj), display, type);

        public void LogUpdates()
        {
            Console.WriteLine("\r\n\t~~~~~~~~\r\n");
            var logString = String.Format("\t---\t{0} down, {1} to go, {2} errors.\t---", updatesIter, updatesCount - updatesIter, erroredCount);
            Console.WriteLine(logString);
            LogManager.Enqueue("LoopManager", EntryType.DEBUG, "Logging updates...", logString);

            if (!errored.IsEmpty)
            {
                foreach (var erroredUpdate in errored.AsEnumerable())
                {
                    Console.WriteLine(erroredUpdate.Key.ToString() + " - " + erroredUpdate.Value);
                    erroredWriter.WriteLine(erroredUpdate.Key.ToString());
                }
            }

            Console.WriteLine("\r\n\t~~~~~~~~\r\n");
            foreach (var finishedUpdate in finished.AsEnumerable())
            {
                Console.WriteLine("\t" + finishedUpdate);
                finishedWriter.WriteLine(finishedUpdate);
            }

            errored = new ConcurrentBag<KeyValuePair<object, string>>();
            finished = new ConcurrentBag<object>();
        }

        public void EndLoop(ParallelLoopResult loop, bool readline = true)
        {
            Console.WriteLine("STARTED");
            SpinWait.SpinUntil(() => loop.IsCompleted);
            LogUpdates();
            finishedWriter.Close();
            finishedWriter = null;
            erroredWriter.Close();
            erroredWriter = null;
            LogManager.Enqueue("LoopManager", EntryType.DEBUG, "FINISHED");
            Console.WriteLine("FINISHED");
            if (readline)
                Console.ReadLine();
        }
    }
}
