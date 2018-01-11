using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jil;
using Newtonsoft.Json;

namespace ZachLib.Logging
{
    public static class LogManager
    {
        #region Setup
        static LogManager()
        {
            if (!Directory.Exists(LOGGER_PATH))
                Directory.CreateDirectory(LOGGER_PATH);
            CURSESSION_PATH = LOGGER_PATH + Process.GetCurrentProcess().ProcessName + @"\";
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            if (!Directory.Exists(CURSESSION_PATH))
                Directory.CreateDirectory(CURSESSION_PATH);

            queue = new ConcurrentQueue<LogUpdate>(
                File.Exists(QUEUE_PATH) ?
                    JSON.Deserialize<IEnumerable<LogUpdate>>(File.ReadAllText(QUEUE_PATH)) :
                    Enumerable.Empty<LogUpdate>()
            );
            Format = "[{0}] [{1}]:\t{2} ~ {3}";
            isRunning = false;
        }

        private static void Initialize()
        {
            var values = logs.Values;
            foreach (var value in values)
            {
                value.Initialize();
            }
        }

        private static readonly object _queueLock = new object();
        private static ConcurrentQueue<LogUpdate> queue = null;
        private static SortedDictionary<string, Log> logs = new SortedDictionary<string, Log>();
        private static Thread loggerThread = new Thread(() => ManageLogging()) {
            Priority = ThreadPriority.BelowNormal,
            IsBackground = true,
            Name = "LoggerThread"
        };
        private static int FileEntriesNotLogged = 0;

        public static string Format { get; private set; }
        private static string TimeFormat = "[-][d’:’]h’:’mm’:’ss[.FFF]";
        private const string NEW_LINE = "\r\n\t\t\t\t\t";
        private const string FILE_DIVISOR = "\r\n\r\n\t\t~~~\t\t\r\n\r\n";
        private static readonly string TODAY = DateTime.Now.ToString("MM.dd.yyyy") + ".txt";

        public static readonly string LOGGER_PATH = @"C:\Users\" + Environment.UserName + @"\AppData\Local\ZachLogs\";
        public static string CURSESSION_PATH = null;
        private static readonly string QUEUE_PATH = LOGGER_PATH + "Queue.txt";
        public static bool isRunning { get; private set; }

        public static event EntryHandler OnEntry;
        public delegate void EntryHandler(object sender, string logName, string entry);
#endregion

        #region AddLog
        /// <summary>
        /// Adds the default Log (logs by date, entries by time) to the Dictionary.
        /// </summary>
        /// <param name="name">The name that the Log will be referenced by, as well as the name of the Folder the logs will reside in.</param>
        public static void AddLog(string name)
        {
            AddLog(name, LogType.FolderFilesByDate);
        }

        /// <summary>
        /// Adds a log of the specified type and name to the Dictionary, with the default entry-formatting behavior.
        /// </summary>
        /// <param name="type">How the log's entries will be interpreted and handled.</param>
        public static void AddLog(string name, LogType type)
        {
            Log log = new Log(type, name);
            log.Formatting = FormattingType.Global;
            AddLog(log);
        }

        public static void AddLog(Log log)
        {
            bool wasRunning = isRunning;
            if (isRunning)
                Stop(true);

            lock (_queueLock)
            {
                logs.Add(log.Name, log);
                logs[log.Name].Initialize();
            }

            if (wasRunning)
            {
                Start();
                Enqueue(
                    "LogManager",
                    EntryType.STATUS,
                    new object[] {
                        "Log Added",
                        log.Name
                    }
                );
            }
        }
#endregion

        #region AddLogs
        public static void AddLogs(params string[] names)
        {
            foreach (string name in names)
            {
                AddLog(name);
            }
        }

        public static void AddLogs(params KeyValuePair<string, LogType>[] logs)
        {
            foreach(var log in logs)
            {
                AddLog(log.Key, log.Value);
            }
        }

        public static void AddLogs(params Log[] logs)
        {
            foreach(var log in logs)
            {
                AddLog(log);
            }
        }
        #endregion

        #region SetGlobalFormat
        public static void SetGlobalFormat(string entryFormat, string timeFormat)
        {
            Format = entryFormat;
            TimeFormat = timeFormat;
        }

        public static void SetGlobalFormat(string entryFormat)
        {
            Format = entryFormat;
        }
        #endregion

        #region GetDefaultTimeFormat
        public static string GetDefaultTimeFormat()
        {
            return DateTime.Now.ToString(TimeFormat);
        }

        public static string GetDefaultTimeFormat(DateTime dateTime)
        {
            return dateTime.ToString(TimeFormat);
        }
        #endregion

        #region Enqueue
        public static void Enqueue(LogUpdate update)
        {
            queue.Enqueue(update);
        }

        public static void Enqueue(string logName, EntryType type)
        {
            queue.Enqueue(new LogUpdate(logName, type));
        }

        public static void Enqueue(string log, string fileName, object data)
        {
            queue.Enqueue(new LogUpdate(log, fileName, data));
        }

        public static void Enqueue(string log, string fileName, object data, Exception error)
        {
            queue.Enqueue(new LogUpdate(log, fileName, data, error));
        }

        public static void Enqueue(string log, string fileName, bool withLogEntry, object data)
        {
            queue.Enqueue(new LogUpdate(log, fileName, withLogEntry, data));
        }

        public static void Enqueue(string log, string fileName, bool withLogEntry, object data, Exception error)
        {
            queue.Enqueue(new LogUpdate(log, fileName, withLogEntry, data, error));
        }

        public static void Enqueue(string log, EntryType entryType, string entry1, string entry2)
        {
            queue.Enqueue(new LogUpdate(log, entryType, entry1, entry2));
        }

        public static void Enqueue(string log, object[] logContent)
        {
            queue.Enqueue(new LogUpdate(log, logContent));
        }

        public static void Enqueue(string log, EntryType type, object[] logContent)
        {
            queue.Enqueue(new LogUpdate(log, type, logContent));
        }

        public static void Enqueue(string log, string fileName, object[] logcontent, object data)
        {
            queue.Enqueue(new LogUpdate(log, fileName, logcontent, data));
        }

        public static void Enqueue(string log, string fileName, object[] logcontent, object data, Exception error)
        {
            queue.Enqueue(new LogUpdate(log, fileName, logcontent, data, error));
        }

        public static void Enqueue(string log, string key, string value)
        {
            queue.Enqueue(new LogUpdate(log, key, value));
        }
        #endregion

        #region Start
        public static void Start()
        {
            if (!isRunning)
            {
                isRunning = true;
                FileEntriesNotLogged = 0;
                loggerThread.Start();

                Enqueue(
                    "LogManager",
                    EntryType.STATUS,
                    new object[]
                    {
                        "Starting...",
                        "",
                        "Thread Priority: \t" + loggerThread.Priority.ToString(),
                        "Is Background: \t" + loggerThread.IsBackground.ToString(),
                        "Number of Logs: \t" + logs.Count.ToString()
                    }
                );
            }
        }

        public static void Start(ThreadPriority priority)
        {
            lock(_queueLock)
            {
                loggerThread.Priority = priority;
            }
            Start();
        }

        public static void Start(bool isBackground)
        {
            lock(_queueLock)
            {
                loggerThread.IsBackground = isBackground;
            }
            Start();
        }

        public static void Start(ThreadPriority priority, bool isBackground)
        {
            lock(_queueLock)
            {
                loggerThread.Priority = priority;
                loggerThread.IsBackground = isBackground;
            }
            Start();
        }
#endregion

        public static void Stop(bool forceStop = false)
        {
            if (isRunning && !disposedValue)
            {
                if (!forceStop)
                {
                    Enqueue(
                        "LogManager",
                        EntryType.STATUS,
                        new object[] {
                            "Stopping...",
                            "",
                            FileEntriesNotLogged.ToString() + " file entries not logged.",
                            queue.Count.ToString() + " items remaining in queue."
                        }
                    );
                    SpinWait.SpinUntil(() => queue.IsEmpty);
                    GC.Collect();
                }
                
                loggerThread.Abort();
                loggerThread.Join(forceStop ? 250 : 2500);

                lock (_queueLock)
                {
                    isRunning = false;
                }
            }
        }
        
        public static bool LogExists(string name, out Log log)
        {
            return logs.TryGetValue(name, out log);
        }

        public static string FormatEntryUsingDefault(LogUpdate update)
        {
            Log log = logs[update.LogName];
            if (update.Type == EntryType.PRESET)
            {
                switch (log.Type)
                {
                    case LogType.FolderFiles:
                        update.Type = update.FileContent.Length > 1 ? 
                            EntryType.ERROR : 
                            EntryType.FILE;
                        
                        if (!log.LogFileEntries)
                            update.FileContent.SaveAs(
                                log.DataPath + update.File + ".txt",
                                Formatting.Indented,
                                FILE_DIVISOR
                            );
                        break;

                    case LogType.FolderFilesNonLog:
                        update.FileContent.SaveAs(
                            log.DataPath + update.File + ".txt",
                            FILE_DIVISOR
                        );

                        update.File = null;
                        update.Type = EntryType.FILE;
                        break;

                    case LogType.SingleFileNonLog:
                        return update.LogContent.Length > 1 ? 
                            String.Join(" :=: ", update.LogContent) : 
                            update.LogContent.First().ToString();

                    case LogType.SingleFile:
                        return String.Join(FILE_DIVISOR, update.LogContent);
                }

                if (log.LogFileEntries)
                    return FormatEntryUsingDefault(update);
                else
                    return null;
            }
            else
            {
                //LogType type = LogType.FolderFilesByDate;

                if ((update.Type == EntryType.FILE || update.Type == EntryType.ERROR) && !String.IsNullOrWhiteSpace(update.File) && update.FileContent.Any())
                    update.FileContent.SaveAs(
                        log.DataPath + update.File + ".txt",
                        Formatting.Indented,
                        FILE_DIVISOR
                    );

                if (update.Type == EntryType.ERROR && update.FileContent.Any())
                {
                    Exception error = (Exception)update.FileContent.Last();

                    return String.Format(
                        Format,
                        GetDefaultTimeFormat(update.Timestamp),
                        "ERROR",
                        error.Message,
                        error.ToErrString(5)
                    );
                }
                else
                {
                    bool hasFileName = !String.IsNullOrWhiteSpace(update.File);
                    return String.Format(
                        Format,
                        GetDefaultTimeFormat(update.Timestamp),
                        update.Type.ToString(),
                        hasFileName ?
                            update.File :
                            update.LogContent.First(),
                        String.Join(
                            NEW_LINE, 
                            hasFileName ? 
                                update.LogContent :
                                update.LogContent.Skip(1)
                        )
                    );
                }
            }
        }
        
        private static void ManageLogging()
        {
            LogUpdate update = new LogUpdate();
            try
            {
                while (true)
                {
                    SpinWait.SpinUntil(() => !queue.IsEmpty);

                    lock (_queueLock)
                    {
                        while (queue.TryDequeue(out update))
                        {
                            if (logs.TryGetValue(update.LogName, out Log log))
                            {
                                string entry = log[update];
                                if (!log.TryLogEntry(entry))
                                    ++FileEntriesNotLogged;
                                else
                                    OnEntry(null, update.LogName, entry);
                            }
                            else
                                Enqueue("LogManager", EntryType.ERROR, new object[] { update.LogName, "Could not find a log of this name." });

                            update.Dispose();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Enqueue("LogManager", update.LogName + " " + TODAY, update, e);
                isRunning = false;
                Stop();
            }
        }

        #region IDisposable Support
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Dispose(false);
        }
        
        private static bool disposedValue = false; // To detect redundant calls

        private static void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Stop(!disposing);
                lock (queue)
                {
                    queue.ToArray().SaveAs(LOGGER_PATH + "Queue.txt");

                    var values = logs.Values.ToArray();
                    foreach (var value in values)
                    {
                        value.Dispose();
                    }
                    logs.Clear();
                    logs = null;

                    if (disposing)
                    {
                        while (!queue.IsEmpty)
                            queue.TryDequeue(out _);
                        queue = null;
                        Format = null;
                        TimeFormat = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LogManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public static void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
