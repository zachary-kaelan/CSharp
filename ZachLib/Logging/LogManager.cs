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
//using Newtonsoft.Json;

namespace ZachLib.Logging
{
    public static class LogManager
    {
        #region Setup
        static LogManager()
        {
            if (!Directory.Exists(LOGGER_PATH))
                Directory.CreateDirectory(LOGGER_PATH);
            CURSESSION_PATH = LOGGER_PATH + @"\";
            Thread.GetDomain().UnhandledException += LogManager_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Dispose(false);
            if (!Directory.Exists(CURSESSION_PATH))
                Directory.CreateDirectory(CURSESSION_PATH);

            try
            {
                queue = new ConcurrentQueue<LogUpdate>(
                    File.Exists(QUEUE_PATH) ?
                        JSON.Deserialize<IEnumerable<LogUpdate>>(File.ReadAllText(QUEUE_PATH)) :
                        Enumerable.Empty<LogUpdate>()
                );
            }
            catch
            {
                queue = new ConcurrentQueue<LogUpdate>();
            }

            Format = "[{0}] [{1}]:\t{2} ~ {3}";
            currentlyLogging = false;
            AddLog("LogManager", LogType.FolderFilesByDate);
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
        private static readonly object _managerLock = new object();
        private static ConcurrentQueue<LogUpdate> queue = null;
        private static SortedDictionary<string, Log> logs = new SortedDictionary<string, Log>();
        private static Thread loggerThread = new Thread(() => ManageLogging()) {
            Priority = ThreadPriority.BelowNormal,
            IsBackground = false,
            Name = "LoggerThread"
        };
        private static int FileEntriesNotLogged = 0;

        public static string Format { get; private set; }
        //private static string TimeFormat = "hh’:’mm’:’ss.fff";
        private static string TimeFormat = @"hh\:mm\:ss\.fff";
        private const string NEW_LINE = "\r\n\t\t\t\t\t";
        private const string FILE_DIVISOR = "\r\n\r\n\t\t~~~\t\t\r\n\r\n";
        private static readonly string TODAY = Utils.Now.ToString("MM.dd.yyyy") + ".txt";

        public static readonly string LOGGER_PATH = @"C:\Users\" + Environment.UserName + @"\AppData\Local\ZachLogs\";
        public static string CURSESSION_PATH = null;
        private static readonly string QUEUE_PATH = LOGGER_PATH + "Queue.txt";
        public static bool isRunning { get => loggerThread.IsAlive; }
        public static bool hasWork { get => isRunning && !queue.IsEmpty && !currentlyLogging; }
        private static bool currentlyLogging { get; set; }

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
            lock (_queueLock)
            {
                logs.Add(log.Name, log);
                logs[log.Name].Initialize();
            }

            Enqueue(
                "LogManager",
                EntryType.STATUS,
                new object[] {
                    "Log Added",
                    log.Name
                }
            );
            Start();
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
            return DateTime.Now.TimeOfDay.ToString(TimeFormat);
        }

        public static string GetDefaultTimeFormat(TimeSpan time)
        {
            return time.ToString(TimeFormat);
        }
        #endregion

        #region Enqueue
        public static void Enqueue(LogUpdate update)
        {
            update.Initialize();
            queue.Enqueue(update);
        }

        public static void Enqueue(string logName, EntryType type)
        {
            Enqueue(new LogUpdate(logName, type));
        }

        public static void Enqueue(string logName, EntryType type, string entry)
        {
            Enqueue(new LogUpdate(logName, type, entry));
        }

        public static void Enqueue(string log, string fileName, object data)
        {
            Enqueue(new LogUpdate(log, fileName, data));
        }

        public static void Enqueue(string log, string fileName, object data, Exception error)
        {
            Enqueue(new LogUpdate(log, fileName, data, error));
        }

        public static void Enqueue(string log, string fileName, bool withLogEntry, object data, bool isListItem = false)
        {
            Enqueue(new LogUpdate(log, fileName, withLogEntry, data, isListItem));
        }

        public static void Enqueue(string log, string fileName, bool withLogEntry, object data, Exception error, bool isListItem = false)
        {
            Enqueue(new LogUpdate(log, fileName, withLogEntry, data, error, isListItem));
        }

        public static void Enqueue(string log, EntryType entryType, string entry1, string entry2)
        {
            Enqueue(new LogUpdate(log, entryType, entry1, entry2));
        }

        public static void Enqueue(string log, object[] logContent)
        {
            Enqueue(new LogUpdate(log, logContent));
        }

        public static void Enqueue(string log, EntryType type, object[] logContent)
        {
            Enqueue(new LogUpdate(log, type, logContent));
        }

        /*public static void Enqueue(string log, string fileName, object[] logcontent, object data, bool isListItem = false)
        {
            Enqueue(new LogUpdate(log, fileName, logcontent, data, isListItem));
        }*/

        public static void Enqueue(string log, string fileName, object[] logcontent, object[] data, bool isListItem = false)
        {
            Enqueue(new LogUpdate(log, fileName, logcontent, data, isListItem));
        }

        public static void Enqueue(string log, string fileName, object[] logcontent, object data, Exception error, bool isListItem = false)
        {
            Enqueue(new LogUpdate(log, fileName, logcontent, data, error, isListItem));
        }

        public static void Enqueue(string log, string key, string value)
        {
            Enqueue(new LogUpdate(log, key, value));
        }
        #endregion

        #region Start
        private static void StartLogging(bool clearPrevious = false)
        {
            if (!isRunning)
            {
                if (clearPrevious)
                {
                    while (!queue.IsEmpty)
                        queue.TryDequeue(out _);
                }

                //isRunning = true;
                FileEntriesNotLogged = 0;

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

                loggerThread.Start();
            }
        }

        public static void Start(bool clearPrevious = false)
        {
            lock (_managerLock)
            {
                lock(_queueLock)
                {
                    loggerThread.Priority = (ThreadPriority)Math.Min(4, (int)Thread.CurrentThread.Priority + 1);
                }
                StartLogging(clearPrevious);
            }
        }

        public static void Start(ThreadPriority priority, bool clearPrevious = false)
        {
            lock (_managerLock)
            {
                lock (_queueLock)
                {
                    loggerThread.Priority = priority;
                }
                StartLogging(clearPrevious);
            }
        }

        public static void Start(bool waitForTerminate, bool clearPrevious)
        {
            lock(_managerLock)
            {
                lock (_queueLock)
                {
                    loggerThread.IsBackground = waitForTerminate;
                }
                StartLogging(clearPrevious);
            }
        }

        public static void Start(ThreadPriority priority, bool waitForTerminate, bool clearPrevious = false)
        {
            lock(_managerLock)
            {
                lock (_queueLock)
                {
                    loggerThread.Priority = priority;
                    loggerThread.IsBackground = waitForTerminate;
                }
                StartLogging(clearPrevious);
            }
        }
#endregion

        public static void Stop(bool forceStop = false)
        {
            lock(_managerLock)
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
                        var threadAlive = loggerThread.IsAlive;
                        SpinWait.SpinUntil(() => queue.IsEmpty);
                        GC.Collect();
                    }

                    loggerThread.Abort();
                    loggerThread.Join(forceStop ? 250 : 2500);

                    /*lock (_queueLock)
                    {
                        isRunning = false;
                    }*/
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
                        update.Type = update.Flags.HasFlag(LogUpdateFlags.HasMoreFileContent) ? 
                            EntryType.ERROR : 
                            EntryType.FILE;
                        
                        if (!log.LogFileEntries)
                            update.FileContent.SaveDividedAs(
                                log.DataPath + update.File + ".txt",
                                Options.PrettyPrint,
                                FILE_DIVISOR
                            );
                        break;

                    case LogType.FolderFilesByDate:
                        update.Type = update.Flags.HasFlag(LogUpdateFlags.HasMoreFileContent) ?
                            EntryType.ERROR :
                            EntryType.FILE;

                        if (!log.LogFileEntries)
                            update.FileContent.SaveDividedAs(
                                log.DataPath + update.File + ".txt",
                                Options.PrettyPrint,
                                FILE_DIVISOR
                            );
                        break;

                    case LogType.FolderFilesNonLog:
                        update.FileContent.SaveDividedAs(
                            log.DataPath + update.File + ".txt",
                            Options.PrettyPrint,
                            FILE_DIVISOR
                        );

                        update.File = null;
                        update.Flags &= ~LogUpdateFlags.HasFilename;
                        update.Type = EntryType.FILE;
                        break;

                    case LogType.SingleFileNonLog:
                        return update.LogContent.Count > 1 ? 
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

                if ((update.Type == EntryType.FILE || update.Type == EntryType.ERROR) && update.Flags.HasFlag(LogUpdateFlags.HasFilename) && update.Flags.HasFlag(LogUpdateFlags.HasFileContent))
                {
                    string path = log.DataPath + update.File + ".txt";
                    update.FileContent.SaveDividedAs(
                        path,
                        Options.PrettyPrint,
                        FILE_DIVISOR
                    );

                    if (log.LogFileEntries)
                    {
                        string fileLogEntry = new FileInfo(path).Length + " bytes written to " + update.File + ".txt";
                        if (!update.Flags.HasFlag(LogUpdateFlags.HasLogContent))
                        {
                            update.LogContent = new List<object> { fileLogEntry };
                            update.Flags |= LogUpdateFlags.HasLogContent;
                        }
                        else
                            update.LogContent.Add(fileLogEntry);
                    }
                }

                if (update.Type == EntryType.ERROR && update.Flags.HasFlag(LogUpdateFlags.HasFileContent))
                {
                    Exception error = (Exception)update.FileContent.Last();

                    string errHeader = error.HResult.ToString() + " @ " + error.Source;
                    string errEntry = update.Flags.HasFlag(LogUpdateFlags.IsListItem) ||
                                        !update.Flags.HasFlag(LogUpdateFlags.DoFullErrorLogEntry) ?
                                            error.Message :
                                            error.ToErrString(5);

                    if (update.Flags.HasFlag(LogUpdateFlags.HasLogContent))
                    {
                        if (update.LogContent.Count > 1)
                        {
                            update.LogContent.Add(errHeader);
                            update.LogContent.Add(errEntry);
                        }
                        else
                            update.LogContent.Add(errEntry);
                    }
                    else
                    {
                        update.LogContent = new List<object>() { errHeader, errEntry };
                        update.Flags |= LogUpdateFlags.HasLogContent;
                    }
                    /*return update.IsListItem || !update.FullErrorLogEntry ?
                        "\t\t\t\t\t" + errHeader + " ~ " + error.Message : 
                        String.Format(
                            Format,
                            GetDefaultTimeFormat(update.Timestamp),
                            "ERROR",
                            errHeader,
                            error.ToErrString(5)
                        );*/
                }
                else if (update.Flags.HasFlag(LogUpdateFlags.HasFilename))
                {
                    if (update.Flags.HasFlag(LogUpdateFlags.HasLogContent))
                        update.LogContent.Insert(0, update.File);
                    else
                        update.LogContent = new List<object>() { update.File };
                }
                /*{
                    

                    return update.IsListItem ?
                        content1 + " ~ " + content2 :
                        String.Format(
                            Format,
                            GetDefaultTimeFormat(update.Timestamp),
                            update.Type.ToString(),
                            content1,
                            content2
                        );
                }*/

                if (update.Flags.HasFlag(LogUpdateFlags.HasLogContent))
                {
                    if (update.Flags.HasFlag(LogUpdateFlags.IsListItem))
                        return String.Join(" ~ ", update.LogContent);
                    else if (update.LogContent.Count == 2)
                        return String.Format(
                            Format,
                            GetDefaultTimeFormat(update.Timestamp),
                            update.Type.ToString(),
                            update.LogContent[0],
                            update.LogContent[1]
                        );
                    else if (update.LogContent.Count == 1 || update.LogContent[1] == null || String.IsNullOrWhiteSpace(update.LogContent[1].ToString()))
                    {
                        List<object> entry2Candidates = new List<object>() { update.File };
                        if (update.FileContent != null)
                            entry2Candidates.AddRange(update.FileContent);
                        entry2Candidates.Add("Flags: " + update.Flags.ToString());

                        if (update.LogContent.Count == 1)
                            return String.Format(
                                Format,
                                GetDefaultTimeFormat(update.Timestamp),
                                update.Type.ToString(),
                                update.LogContent[0],
                                entry2Candidates.FirstOrDefault(o => o != null)
                            );
                        else
                            return String.Format(
                                Format,
                                GetDefaultTimeFormat(update.Timestamp),
                                update.Type.ToString(),
                                update.LogContent[0],
                                entry2Candidates.FirstOrDefault(o => o != null),
                                String.Join(NEW_LINE, update.LogContent.Skip(2))
                            );
                    }
                    else
                    {
                        return String.Format(
                            Format,
                            GetDefaultTimeFormat(update.Timestamp),
                            update.Type.ToString(),
                            update.LogContent[0],
                            String.Join(NEW_LINE, update.LogContent.Skip(1))
                        );
                    }
                }
                else
                {
                    var errEntry = new LogUpdate(
                        "LogManager",
                        log.Name + " - " + GetDefaultTimeFormat(update.Timestamp),
                        new object[]
                        {
                            "Log entry given with no log content",
                            log.Name,
                            "Flags: " + update.Flags.ToString()
                        },
                        new object[]
                        {
                            update,
                            log
                        }
                    );

                    LogManager.Enqueue(errEntry);
                    return null;
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

                    currentlyLogging = true;
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
                                    OnEntry?.Invoke(null, update.LogName, entry);
                            }
                            else
                                Enqueue("LogManager", EntryType.ERROR, new object[] { update.LogName, "Could not find a log of this name." });

                            update.Dispose();
                        }
                    }
                    currentlyLogging = false;
                }
            }
            catch (Exception e)
            {
                currentlyLogging = false;
                var mngrLog = logs["LogManager"];
                var mngrUpdate = new LogUpdate("LogManager", update.LogName + " " + TODAY, update, e);
                try
                {
                    if (!mngrLog.TryLogEntry(mngrLog[mngrUpdate]))
                    {
                        ++FileEntriesNotLogged;
                        update.SaveAs(mngrLog.DataPath + mngrUpdate.File + ".txt", Options.PrettyPrintExcludeNullsCamelCase);
                        mngrLog.LogStream.WriteLine(
                            Format,
                            GetDefaultTimeFormat(),
                            EntryType.ERROR,
                            update.LogName,
                            e.ToErrString(5)
                        );
                    }
                }
                catch
                {
                    update.SaveAs(mngrLog.DataPath + mngrUpdate.File + ".txt", Options.PrettyPrintExcludeNullsCamelCase);
                    mngrLog.LogStream.WriteLine(
                        Format,
                        GetDefaultTimeFormat(),
                        EntryType.ERROR,
                        update.LogName,
                        e.ToErrString(5)
                    );
                }
                mngrLog.LogStream.Flush();
                ThreadPool.QueueUserWorkItem(
                    o =>
                    {
                        Thread.Sleep(150);
                        Stop(true);
                    }
                );
            }
        }

        public static void WaitAndFlush()
        {
            SpinWait.SpinUntil(() => queue.IsEmpty);
            Flush();
        }

        public static void Flush()
        {
            lock(_queueLock)
            {
                foreach(var log in logs.Values)
                {
                    log.LogStream.Flush();
                }
            }
            Enqueue("LogManager", EntryType.DEBUG, "Flushed log streams");
        }

        #region IDisposable Support
        private static void LogManager_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            string processName = Process.GetCurrentProcess().ProcessName;
            LogManager.Enqueue(
                new LogUpdate(
                    "LogManager",
                    processName,
                    new object[]
                    {
                        processName,
                        "Unhandled " + (e.IsTerminating ? "terminating " : "") + "exception",
                        "Log Names: " + String.Join(", ", logs.Keys),
                        "Queue Entry Types: [" + String.Join(", ", queue.AsEnumerable().Select(u => u.Type)) + "]"
                    },
                    sender,
                    exception
                )
            );
            Dispose(true);
        }

        private static bool disposedValue = false; // To detect redundant calls

        private static void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Stop(!disposing);
                lock (queue)
                {
                    if (!queue.IsEmpty)
                        queue.ToArray().SaveAs(LOGGER_PATH + "Queue.txt");
                    else
                        File.WriteAllText(LOGGER_PATH + "Queue.txt", "[]");

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
