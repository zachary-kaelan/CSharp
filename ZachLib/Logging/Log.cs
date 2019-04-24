using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Logging
{
    public enum LogType
    {
        SingleFile,
        SingleFileNonLog,
        FolderFiles,
        FolderFilesByDate,
        FolderFilesNonLog
    }

    public enum FormattingType
    {
        Single,
        Global,
        Multiple
    };

    public class Log : IDictionary<EntryType, Func<LogUpdate, string>>, IDisposable
    {
        public static FileMode LogFileMode = FileMode.Create;
        private const string FILE_DIVISOR = "\r\n\r\n\t\t~~~\t\t\r\n\r\n";
        private static readonly string TODAY = DateTime.Now.ToString("MM.dd.yyyy") + ".txt";

        public string EntryFormat { get; set; }
        public string TimeFormat { get; set; }
        public FormattingType Formatting { get; set; }
        public LogType Type { get; set; }
        public bool LogFileEntries { get; set; }
        private Dictionary<EntryType, Func<LogUpdate, string>> EntryFormatters { get; set; }

        private bool isInitialized { get; set; }
        public string Path { get; private set; }
        public string DataPath { get; private set; }
        public string Name { get; private set; }
        internal StreamWriter LogStream { get; private set; }
        private FileStream LogFile { get; set; }

        public Func<LogUpdate, string> FormatEntry { get; private set; }
        public string this[LogUpdate update] {
            get => Formatting != FormattingType.Multiple ? 
                FormatEntry(update) : 
                EntryFormatters[update.Type](update); }

        private object _logLock = new object();

        public bool TryLogEntry(string entry)
        {
            if ((Type == LogType.SingleFile ||
                    Type == LogType.SingleFileNonLog ||
                    Type == LogType.FolderFilesByDate
                ) && !String.IsNullOrWhiteSpace(entry))
            {
                lock (_logLock)
                {
                    LogStream.TryWriteLineAsync(entry);
                    LogStream.Flush();
                }
            }
            else
                return false;
            return true;
        }

        #region Initialization
        #region Constructors
        private Log() : base()
        {
            EntryFormatters = new Dictionary<EntryType, Func<LogUpdate, string>>();
            LogFileEntries = false;
            isInitialized = false;
        }

        public Log(LogType type) : this()
        {
            Type = type;
        }

        public Log(LogType type, string name) : this(type)
        {
            this.Name = String.IsNullOrWhiteSpace(name) ? type.ToString() : name;
        }

        public Log(LogType type, string name, EntryType singleType, Func<LogUpdate, string> format) : this(type, name)
        {
            EntryFormatters.Add(singleType, format);
        }

        public Log(LogType type, EntryType singleType, bool logFileEntries = true, Func<LogUpdate, string> format = null) : this(type, singleType.ToString(), singleType, format)
        {
            LogFileEntries = logFileEntries;
        }

        public Log(LogType type, string name, EntryType singleType, bool logFileEntries = true, Func<LogUpdate, string> format = null) : this(type, name, singleType, format)
        {
            LogFileEntries = logFileEntries;
        }

        public Log(LogType type, string name, IEnumerable<KeyValuePair<EntryType, Func<LogUpdate, string>>> formats) : this(type, name)
        {
            EntryFormatters.Merge(formats);
        }

        public Log(LogType type, IEnumerable<KeyValuePair<EntryType, Func<LogUpdate, string>>> formats) : this(type)
        {
            EntryFormatters.Merge(formats);
        }

        public Log(LogType type, params KeyValuePair<EntryType, Func<LogUpdate, string>>[] formats) : this(type, formats.AsEnumerable()) { }

        public Log(LogType type, string name, params KeyValuePair<EntryType, Func<LogUpdate, string>>[] formats) : this(type, name, formats.AsEnumerable()) { }
#endregion

        public void SetFormatting(string entryFormat, string timeFormat, params KeyValuePair<EntryType, Func<LogUpdate, string>>[] formats)
        {
            EntryFormatters.Merge(formats);
            EntryFormat = entryFormat;
            TimeFormat = timeFormat;
        }

        public void Initialize()
        {
            if (isInitialized)
                return;
            isInitialized = true;
            Path = LogManager.CURSESSION_PATH;

            if (EntryFormatters.Count() == 1)
            {
                Formatting = FormattingType.Single;
                var single = EntryFormatters.Single();
                FormatEntry = single.Value;
            }
            else if (EntryFormatters.Any())
            {
                FormatEntry = u => EntryFormatters[u.Type](u);
                Formatting = FormattingType.Multiple;
            }
            else
            {
                Formatting = FormattingType.Global;
                FormatEntry = u => LogManager.FormatEntryUsingDefault(u);
            }

            if (String.IsNullOrWhiteSpace(Name))
            {
                if (Formatting == FormattingType.Single)
                    Name = EntryFormatters.Single().Key.ToString();
                else
                    Name = Type.ToString();
            }

            Path += Name;

            if (Type == LogType.SingleFile || Type == LogType.SingleFileNonLog)
            {
                Path += ".txt";
                lock(_logLock)
                {
                    LogFile = new FileStream(Path, LogFileMode, FileAccess.Write, FileShare.ReadWrite);
                    LogStream = new StreamWriter(LogFile);
                    if (Type == LogType.SingleFile)
                        LogStream.NewLine = FILE_DIVISOR;
                }
            }
            else
            {
                Path += @"\";
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
                DataPath = Path + (Type == LogType.FolderFilesNonLog ? "" : @"ErroneousObjects\");
                if (!Directory.Exists(DataPath))
                    Directory.CreateDirectory(DataPath);

                if (Type == LogType.FolderFilesByDate)
                {
                    Path += TODAY;
                    lock (_logLock)
                    {
                        LogFile = new FileStream(Path, LogFileMode, FileAccess.Write, FileShare.ReadWrite);
                        LogStream = new StreamWriter(LogFile);
                    }
                }
            }
        }
#endregion

        #region IDictionary Support
        public Func<LogUpdate, string> this[EntryType key] { get => EntryFormatters[key]; set => EntryFormatters[key] = value; }

        public ICollection<EntryType> Keys => EntryFormatters.Keys;

        public ICollection<Func<LogUpdate, string>> Values => EntryFormatters.Values;

        public int Count => EntryFormatters.Count;

        public bool IsReadOnly => ((IDictionary<EntryType, Func<LogUpdate, string>>)EntryFormatters).IsReadOnly;

        public void Add(EntryType key, Func<LogUpdate, string> value)
        {
            EntryFormatters.Add(key, value);
        }

        public void Add(KeyValuePair<EntryType, Func<LogUpdate, string>> item)
        {
            ((IDictionary<EntryType, Func<LogUpdate, string>>)EntryFormatters).Add(item);
        }

        public void Clear()
        {
            EntryFormatters.Clear();
        }

        public bool Contains(KeyValuePair<EntryType, Func<LogUpdate, string>> item)
        {
            return EntryFormatters.Contains(item);
        }

        public bool ContainsKey(EntryType key)
        {
            return EntryFormatters.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<EntryType, Func<LogUpdate, string>>[] array, int arrayIndex)
        {
            ((IDictionary<EntryType, Func<LogUpdate, string>>)EntryFormatters).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<EntryType, Func<LogUpdate, string>>> GetEnumerator()
        {
            return EntryFormatters.GetEnumerator();
        }

        public bool Remove(EntryType key)
        {
            return EntryFormatters.Remove(key);
        }

        public bool Remove(KeyValuePair<EntryType, Func<LogUpdate, string>> item)
        {
            return ((IDictionary<EntryType, Func<LogUpdate, string>>)EntryFormatters).Remove(item);
        }

        public bool TryGetValue(EntryType key, out Func<LogUpdate, string> value)
        {
            return EntryFormatters.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EntryFormatters.GetEnumerator();
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FormatEntry = null;
                    EntryFormat = null;
                    TimeFormat = null;
                    Path = null;
                    Name = null;
                }

                EntryFormatters.Clear();
                EntryFormatters = null;

                lock(_logLock)
                {
                    if (LogStream != null)
                    {
                        LogStream.Flush();
                        LogStream.Close();
                        LogStream = null;
                    }
                }

                disposedValue = true;
            }
        }

        //TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Log() {
          // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
          Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
