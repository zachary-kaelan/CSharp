using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jil;

namespace PPLib
{
    public enum EntryType
    {
        UICHANGE,
        STATUS,
        DEBUG,
        NTWRK,
        ERROR,
        DATA,
        DICTIONARY,
        HTTP
    };

    public class LogManager : ConcurrentQueue<LogUpdate>, IDisposable
    {
        private SortedDictionary<string, StreamWriter> logs { get; set; }
        private SortedDictionary<string, string> logDirectories { get; set; }
        private Thread loggerThread { get; set; }
        private string format { get; set; }
        private string timeFormat { get; set; }
        private const string NEW_LINE = "\r\n\t\t\t\t\t";
        private static readonly string TODAY = DateTime.Now.ToString("MM.dd.yyyy") + ".txt";
        public static string LOGGER_PATH = null;

        public event EntryHandler OnEntry;
        public delegate void EntryHandler(object sender, string logName, string entry);

        public LogManager(string entryFormat, string timeFormat) : base(File.Exists(LOGGER_PATH + "Queue.txt") ? JSON.Deserialize<LogUpdate[]>(File.ReadAllText(LOGGER_PATH + "Queue.txt")) : new LogUpdate[] { })
        {
            if (Directory.Exists(LOGGER_PATH))
                Directory.CreateDirectory(LOGGER_PATH);
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            this.format = entryFormat;
            this.timeFormat = timeFormat;
            logs = new SortedDictionary<string, StreamWriter>();
            logDirectories = new SortedDictionary<string, string>();
        }

        public LogManager(string entryFormat, string timeFormat, params string[] paths) : this(entryFormat, timeFormat)
        {
            AddLogs(paths);
        }

        public void AddLog(string path)
        {
            if (Directory.Exists(path))
            {
                //if (!Directory.Exists(path))
                 //   Directory.CreateDirectory(path);

                Directory.GetFiles(path).ToList().FindAll(f => new FileInfo(f).Length == 0)
                    .ForEach(f => File.Delete(f));

                string key = Path.GetDirectoryName(path);
                if (Directory.EnumerateFiles(path).Select(f => Path.GetFileName(f)).Any(f => Char.IsDigit(f[0]) && !f.Contains(' ')) && !logs.ContainsKey(key))
                    logs.Add(key, new StreamWriter(path + TODAY));
                else if (!logDirectories.ContainsKey(key))
                    logDirectories.Add(key, path);
            }
            else
            {
                string dirPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                dirPath = null;

                string key = Path.GetFileNameWithoutExtension(path);
                if (!logs.ContainsKey(key))
                    logs.Add(key, new StreamWriter(path));
            }
        }

        public void AddLogs(params string[] paths)
        {
            foreach (string path in paths)
            {
                AddLog(path);
            }
        }

        public void Start(ThreadPriority priority = ThreadPriority.BelowNormal, bool isBackground = true)
        {
            loggerThread = new Thread(() => ManageLogging());
            loggerThread.Priority = priority;
            loggerThread.IsBackground = isBackground;
            loggerThread.Start();
        }

        public void Stop(bool forceStop = false)
        {
            if (!forceStop)
            {
                SpinWait.SpinUntil(() => this.IsEmpty);
                loggerThread.Abort();
            }
            else
            {
                loggerThread.Abort();
                while (!this.IsEmpty)
                    this.TryDequeue(out _);
            }

        }

        private void ManageLogging()
        {
            LogUpdate update = new LogUpdate();
            while(true)
            {
                SpinWait.SpinUntil(() => !this.IsEmpty);

                while(this.TryDequeue(out update))
                {
                    if (update.Type == EntryType.DICTIONARY)
                        logs[update.LogName].WriteLineAsync(update.LogContent.Single().ToString());
                    else
                    {
                        string time = update.Timestamp.ToString(timeFormat);

                        if (update.Type == EntryType.HTTP)
                        {
                            string entry = String.Format(
                                format,
                                time,
                                "HTTP",
                                update.LogContent[0],
                                String.Join(
                                    NEW_LINE,
                                    update.LogContent.Skip(1)
                                )
                            );

                            OnEntry(this, update.LogName, entry);
                            logs[update.LogName].WriteLineAsync(entry);
                            entry = null;
                        }
                        else
                        {
                            if (update.FileContent.Length > 0)
                            {
                                string filename = logDirectories[update.LogName] + update.LogContent.First().ToString() + ".txt";
                                File.WriteAllText(
                                    filename,
                                    String.Join(
                                        NEW_LINE,
                                        update.FileContent.Select(
                                            o => JSON.Serialize(o)
                                        )
                                    )
                                );

                                if (update.Type != EntryType.DATA)
                                {
                                    string entry = String.Format(
                                        format,
                                        time,
                                        update.Type.ToString(),
                                        update.File,
                                        Path.GetDirectoryName(filename) + filename
                                    );
                                    OnEntry(this, update.LogName, entry);
                                    logs[update.LogName].WriteLineAsync(entry);
                                    entry = null;
                                }
                            }

                            if (update.LogContent.Length > 0)
                            {
                                string entry = String.Format(
                                    format,
                                    time,
                                    update.Type.ToString(),
                                    update.File,
                                    String.Join(
                                        NEW_LINE,
                                        update.LogContent
                                    )
                                );
                                OnEntry(this, update.LogName, entry);
                                logs[update.LogName].WriteLineAsync(entry);
                            }
                        }
                    }
                }
            }
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            this.Dispose(false);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                string[] keys = this.logs.Keys.ToArray();
                foreach(var key in keys)
                {
                    logs[key].Flush();
                    logs[key].Dispose();
                    logs[key].Close();
                    logs[key] = null;
                }
                this.logs = null;
                this.ToArray().SaveAs(LOGGER_PATH + "Queue.txt");

                if (this.loggerThread != null)
                {
                    this.loggerThread.Abort();
                    this.loggerThread.Join(250);
                    this.loggerThread = null;
                }
                

                if (disposing)
                {
                    while (!this.IsEmpty)
                        this.TryDequeue(out _);
                    this.format = null;
                    this.logDirectories = null;
                    this.timeFormat = null;
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
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

    public struct LogUpdate
    {
        public string LogName { get; set; }
        public object[] FileContent { get; set; }
        public object[] LogContent { get; set; }
        public EntryType Type { get; set; }
        public string File { get; set; }
        public DateTime Timestamp { get; set; }

        public LogUpdate(string log, object[] data)
        {
            this.LogName = log;
            this.FileContent = new object[] { data };
            this.LogContent = null;
            this.File = null;
            this.Timestamp = DateTime.Now;
            this.Type = EntryType.DATA;
        }

        public LogUpdate(string log, EntryType type, params object[] logcontent)
        {
            this.LogName = log;
            this.FileContent = null;
            this.LogContent = logcontent;
            this.File = null;
            this.Timestamp = DateTime.Now;
            this.Type = type;
        }

        public LogUpdate(string log, string file, EntryType type, object[] logcontent, params object[] filecontent)
        {
            this.LogName = log;
            this.FileContent = filecontent;
            this.LogContent = logcontent;
            this.File = file;
            this.Timestamp = DateTime.Now;
            this.Type = type;
        }

        public LogUpdate(string log, string entry)
        {
            this.LogName = log;
            this.FileContent = null;
            this.LogContent = new object[] { entry };
            this.File = null;
            this.Timestamp = DateTime.Now;
            this.Type = EntryType.DICTIONARY;
        }

        /*public LogUpdate(string log, string httpCode, params object[] logcontent)
        {
            this.LogName = log;
            this.FileContent = null;
            this.LogContent = logcontent;
            this.File = httpCode;
            this.Timestamp = DateTime.Now;
            this.Type = EntryType.HTTP;
        }*/

        /*public LogUpdate(string log, Exception error, string file = "Unknown", params object[] logcontent)
        {
            this.LogName = log;
            this.FileContent = null;
            this.LogContent = logcontent.Concat();
            this.File = file;
            this.Timestamp = DateTime.Now;
            this.Type = EntryType.ERROR;
        }*/
    }
    
    /*public struct LogUpdate
    {
        private const string FILENAME_DATE_FORMAT = "MM.dd.yyyy";
        public string LogName { get; set; }             // Name of the log file and/or directory of log files to write to
        public string Name { get; set; }
        public EntryType Type { get; set; }
        public string Filename { get; set; }            // Name of the file involved, and/or the name of the unique log file of full data 
        public DateTime Timestamp { get; set; }         // Date and time that the update occured
        public string Source { get; set; }
        public object[] Content { get; set; }
        private const string NEW_LINE = "\r\n\t\t\t\t\t";

        public LogUpdate(string log, string filename, EntryType type, string src, params object[] content)
        {
            this.LogName = log;
            this.Filename = filename;
            this.Type = type;
            this.Source = src;
            this.Content = content;
            this.Name = null;
            this.Timestamp = DateTime.Now;
        }

        public LogUpdate(string log, string name, string src = null, params object[] content)
        {
            this.LogName = log;
            this.Name = name;
            this.Type = EntryType.DATA;
            this.Timestamp = DateTime.Now;
            this.Filename = (String.IsNullOrWhiteSpace(src) ? this.Timestamp.ToString(FILENAME_DATE_FORMAT) : src) + ".txt";
            this.Source = null;
            this.Content = content;
        }

        public void SerializeTo(string path)
        {
            File.WriteAllText(
                path,
                String.Join(
                    "\r\n\r\n",
                    this.Content.Select(o => JSON.Serialize(o))
                )
            );
        }

        public string Format(string format, string timeFormat)
        {
            return String.Format(
                format,
                this.Timestamp.ToString(timeFormat),
                this.Type.ToString(),
                String.Join(NEW_LINE, this.Content)
            );
        }
    }*/
}
