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

        private static string GlobalFormatting(LogUpdate update)
        {
            if (update.Type == EntryType.DICTIONARY)
                logs[update.LogName].WriteLineAsync(update.LogContent.Single().ToString());
            else
            {
                string time = update.Timestamp.ToString(timeFormat);

                if (update.Type == EntryType.HTTP)
                    return GlobalHTTP(update);
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

        #region DATA/FILE
        public static LogEntry GlobalDATA(LogUpdate update)
        {
            LogEntry logEntry = new LogEntry();
            logEntry.LogName = update.LogName;
            logEntry.FileName = update.LogContent.First().ToString();
            logEntry.FileContent = String.Join(
                NEW_LINE,
                update.FileContent.Select(
                    o => JSON.Serialize(o)
                )
            );
            logEntry.Type = EntryType.DATA;

            return logEntry;
        }

        public static LogEntry GlobalFILE

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
            return GlobalHTTP(update, LogManager.Format, DateTime.Now.ToString(LogManager.TimeFormat));
        }

        public static string GlobalHTTP(LogUpdate update, string format)
        {
            return GlobalHTTP(update, format, DateTime.Now.ToString(LogManager.TimeFormat));
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
        public EntryType Type { get; set; }
        public string LogName { get; set; }
        public string IsFile { get; set; }
        public string FileName { get; set; }
        public string Entry { get; set; }
        public string FileContent { get; set; }
    }
}
