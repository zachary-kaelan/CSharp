using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace ZachLib.Logging
{
    public static class LogFormatting
    {
        private const string NEW_LINE = "\r\n\t\t\t\t\t";

        private static LogEntry GlobalFormatting(LogUpdate update)
        {
            if (update.Type == EntryType.DictionaryEntry)
                return new LogEntry()
                {
                    Type = EntryType.DICTIONARY,
                    Entry = update.LogContent.Single().ToString(),
                    IsFile = false,
                    LogName = update.LogName
                };
            else
            {
                string time = LogManager.GetDefaultTimeFormat(update.Timestamp);

                if (update.Type == EntryType.HTTP)
                    return new LogEntry()
                    {
                        Entry = GlobalHTTP(update),
                        LogName = update.LogName,
                        IsFile = false,
                        Type = EntryType.HTTP
                    };
                else if (update.FileContent.Length > 0)
                    return update.Type == EntryType.DATA ? GlobalDATA(update) : GlobalFILE(update);
                else
                    return new LogEntry()
                    {
                        Entry = GlobalGeneric(update),
                        LogName = update.LogName,
                        Type = update.Type,
                        IsFile = false
                    };
            }
        }

        #region Generic
        public static string GlobalGeneric(LogUpdate update)
        {
            return GlobalGeneric(update, LogManager.Format);
        }

        public static string GlobalGeneric(LogUpdate update, string format)
        {
            return GlobalGeneric(update, format, LogManager.GetDefaultTimeFormat());
        }

        public static string GlobalGeneric(LogUpdate update, string format, string time)
        {
            return String.Format(
                format,
                time,
                update.Type.ToString(),
                update.File,
                String.Join(
                    NEW_LINE,
                    update.LogContent
                )
            );
        }
#endregion

        #region DATA/FILE
        public static LogEntry GlobalDATA(LogUpdate update)
        {
            LogEntry logEntry = new LogEntry();
            logEntry.LogName = update.LogName;
            logEntry.FileName = update.LogContent.First().ToString();
            logEntry.IsFile = true;
            logEntry.FileContent = String.Join(
                NEW_LINE,
                update.FileContent.Select(
                    o => JSON.Serialize(o)
                )
            );
            logEntry.Type = EntryType.DATA;

            return logEntry;
        }

        public static LogEntry GlobalFILE(LogUpdate update)
        {
            return GlobalFILE(update, LogManager.Format);
        }

        public static LogEntry GlobalFILE(LogUpdate update, string format)
        {
            return GlobalFILE(update, format, LogManager.GetDefaultTimeFormat());
        }

        public static LogEntry GlobalFILE(LogUpdate update, string format, string time)
        {
            LogEntry logEntry = GlobalDATA(update);

            logEntry.Type = EntryType.FILE;
            logEntry.Entry = String.Format(
                format,
                time,
                update.Type.ToString(),
                update.File,
                Path.GetDirectoryName(logEntry.FileName) + logEntry.FileName
            );

            return logEntry;

            //OnEntry(this, update.LogName, entry);
        }
#endregion

        #region HTTP
        public static string GlobalHTTP(LogUpdate update)
        {
            return GlobalHTTP(update, LogManager.Format, LogManager.GetDefaultTimeFormat());
        }

        public static string GlobalHTTP(LogUpdate update, string format)
        {
            return GlobalHTTP(update, format, LogManager.GetDefaultTimeFormat());
        }

        public static string GlobalHTTP(LogUpdate update, string format, string time)
        {
            return String.Format(
                format,
                time,
                "HTTP",
                update.LogContent[0],
                String.Join(
                    NEW_LINE,
                    update.LogContent.Skip(1)
                )
            );
            //OnEntry(this, update.LogName, entry);
        }
#endregion
    }

    public struct LogEntry
    {
        public string LogName { get; set; }
        public bool IsFile { get; set; }
        public string FileName { get; set; }
        public string Entry { get; set; }
        public string FileContent { get; set; }
    }
}
