using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Logging
{
    public enum EntryType
    {
        UICHANGE,
        STATUS,
        DEBUG,
        NTWRK,
        ERROR,
        FILE,
        HTTP,
        PRESET
    };

    public struct LogUpdate : IDisposable
    {
        public string LogName { get; set; }
        public object[] FileContent { get; set; }
        public object[] LogContent { get; set; }
        public EntryType Type { get; set; }
        public string File { get; set; }
        public DateTime Timestamp { get; set; }
        private bool disposedValue { get; set; }

        public LogUpdate(string logName, EntryType type) : this()
        {
            LogName = logName;
            Timestamp = DateTime.Now;
            Type = type;
            disposedValue = false;
        }

        public LogUpdate(string log, string fileName, object data) : this(log, EntryType.PRESET)
        {
            FileContent = new object[] { data };
            File = fileName;
        }

        public LogUpdate(string log, string fileName, object data, Exception error) : this(log, EntryType.PRESET)
        {
            FileContent = new object[] { data, error };
            File = fileName;
        }

        public LogUpdate(string log, string fileName, bool withLogEntry, object data) : this(log, EntryType.PRESET)
        {
            FileContent = new object[] { data };
            File = fileName;
        }

        public LogUpdate(string log, string fileName, bool withLogEntry, object data, Exception error) : this(log, EntryType.PRESET)
        {
            FileContent = new object[] { data, error };
            File = fileName;
        }

        public LogUpdate(string log, EntryType type, string entry1, string entry2) : this(log, type)
        {
            LogContent = new object[] { entry1, entry2 };
        }

        public LogUpdate(string log, object[] logContent) : this(log, EntryType.PRESET)
        {
            LogContent = logContent;
        }

        public LogUpdate(string log, EntryType type, object[] logContent) : this(log, type)
        {
            LogContent = logContent;
        }

        public LogUpdate(string log, string fileName, object[] logcontent, object data) : this(log, fileName, true, data)
        {
            LogContent = logcontent;
        }

        public LogUpdate(string log, string fileName, object[] logcontent, object data, Exception error) : this(log, fileName, true, data, error)
        {
            LogContent = logcontent;
        }

        public LogUpdate(string log, string key, string value) : this(log, EntryType.PRESET)
        {
            LogContent = new object[] { key, value };
        }
        
        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    File = null;
                    LogName = null;
                    Timestamp = new DateTime();
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
