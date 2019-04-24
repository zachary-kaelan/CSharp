using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Logging
{
    public enum EntryType
    {
        NONE,
        UICHANGE,
        STATUS,
        DEBUG,
        NTWRK,
        ERROR,
        FILE,
        HTTP,
        PRESET
    };

    [Flags]
    internal enum LogUpdateFlags
    {
        None = 0,
        IsListItem,
        DoLogFileEntry = 2,
        DoFullErrorLogEntry = 4,
        HasFileContent = 8,
        HasLogContent = 16,
        HasFilename = 32,
        HasMoreFileContent = 64
    }

    public class LogUpdate : IDisposable
    {
        public string LogName { get; set; }
        public List<object> FileContent { get; set; }
        public List<object> LogContent { get; set; }
        public EntryType Type { get; set; }
        public string File { get; set; }
        public TimeSpan Timestamp { get; set; }
        private bool disposedValue { get; set; }
        internal LogUpdateFlags Flags { get; set; }

        public LogUpdate()
        {
            FileContent = null;
            LogContent = null;
            File = null;
            Flags = LogUpdateFlags.None;
        }

        public LogUpdate(string log, EntryType type, bool isListItem = false) : this()
        {
            LogName = log;
            if (!isListItem)
                Timestamp = DateTime.Now.TimeOfDay;
            else
                Flags |= LogUpdateFlags.IsListItem;
            disposedValue = false;
            Type = type;
        }

        public LogUpdate(string log, string fileName, object data) : this(log, EntryType.PRESET)
        {
            FileContent = new List<object> { data };
            File = fileName;
        }

        public LogUpdate(string log, string fileName, object data, Exception error, bool fullErrorLogEntry = true) : this(log, EntryType.PRESET)
        {
            FileContent = new List<object> { data, error };
            File = fileName;
            if (fullErrorLogEntry)
                Flags = LogUpdateFlags.DoFullErrorLogEntry;
        }

        public LogUpdate(string log, string fileName, bool withLogEntry, object data, bool isListItem = false) : this(log, fileName, withLogEntry, new object[] { data }, isListItem) { }

        public LogUpdate(string log, string fileName, bool withLogEntry, IEnumerable<object> data, bool isListItem = false) : this(log, EntryType.PRESET)
        {
            FileContent = new List<object>(data);
            File = fileName;
            if (withLogEntry)
                Flags |= LogUpdateFlags.DoLogFileEntry;
            if (isListItem)
                Flags |= LogUpdateFlags.IsListItem;
        }

        public LogUpdate(string log, string fileName, bool withLogEntry, object data, Exception error, bool fullErrorLogEntry = true, bool isListItem = false) : this(log, EntryType.PRESET)
        {
            FileContent = new List<object> { data, error };
            File = fileName;
            if (withLogEntry)
                Flags |= LogUpdateFlags.DoLogFileEntry;
            if (isListItem)
                Flags |= LogUpdateFlags.IsListItem;
            if (fullErrorLogEntry)
                Flags |= LogUpdateFlags.DoFullErrorLogEntry;
        }

        public LogUpdate(string log, EntryType type, string entry1, string entry2) : this(log, type)
        {
            LogContent = new List<object> { entry1, entry2 };
        }

        public LogUpdate(string log, IEnumerable<object> logContent) : this(log, EntryType.PRESET)
        {
            LogContent = new List<object>(logContent);
        }

        public LogUpdate(string log, string entry) : this(log, EntryType.PRESET)
        {
            LogContent = new List<object> { entry };
        }

        public LogUpdate(string log, EntryType type, IEnumerable<object> logContent) : this(log, type)
        {
            LogContent = new List<object>(logContent);
        }

        public LogUpdate(string log, EntryType type, string entry) : this(log, type)
        {
            LogContent = new List<object> { entry };
        }

        /*public LogUpdate(string log, string fileName, object[] logcontent, object data, bool isListItem = false) : this(log, fileName, true, data, isListItem)
        {
            LogContent = logcontent;
        }*/

        public LogUpdate(string log, string fileName, IEnumerable<object> logcontent, IEnumerable<object> data, bool isListItem = false) : this(log, fileName, true, data, isListItem)
        {
            LogContent = new List<object>(logcontent);
            FileContent = new List<object>(data);
            if (isListItem)
                Flags |= LogUpdateFlags.IsListItem;
        }

        public LogUpdate(string log, string fileName, IEnumerable<object> logcontent, object data, Exception error, bool fullErrorLogEntry = true, bool isListItem = false) : this(log, fileName, true, data, error, isListItem)
        {
            LogContent = new List<object>(logcontent);
            FileContent = new List<object>() { data, error };
            if (isListItem)
                Flags |= LogUpdateFlags.IsListItem;
            if (fullErrorLogEntry)
                Flags |= LogUpdateFlags.DoFullErrorLogEntry;
        }

        public LogUpdate(string log, string key, string value) : this(log, EntryType.PRESET)
        {
            LogContent = new List<object> { key, value };
        }
        
        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    File = null;
                    LogName = null;
                    Timestamp = new TimeSpan();
                }

                LogContent = null;
                FileContent = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Enqueue()
        {
            LogManager.Enqueue(this);
        }

        internal void Initialize()
        {
            if (!String.IsNullOrWhiteSpace(File))
                Flags |= LogUpdateFlags.HasFilename;
            if (FileContent != null && FileContent.Count > 0)
            {
                Flags |= LogUpdateFlags.HasFileContent;
                if (FileContent.Count > 1)
                    Flags |= LogUpdateFlags.HasMoreFileContent;
            }
            if (LogContent != null && LogContent.Any())
                Flags |= LogUpdateFlags.HasLogContent;
            if (Flags > 0 && Flags.HasFlag(LogUpdateFlags.None))
                Flags &= ~LogUpdateFlags.None;
        }
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
