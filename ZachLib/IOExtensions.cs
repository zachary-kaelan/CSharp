using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jil;
using Newtonsoft.Json;

namespace ZachLib
{
    public static class IOExtensions
    {
        public static void TryWriteLineAsync(this StreamWriter streamWriter, string line, int lowBound = 5)
        {
            int upperBound = lowBound * 3;
            while (true)
            {
                try
                {
                    Console.WriteLine(line);
                    streamWriter.WriteLineAsync(line);
                    break;
                }
                catch
                {
                    Thread.Sleep(Utils.RANDOM.Next(lowBound, upperBound));
                }
            }
        }

        #region SaveAs
        public static void SaveAs(this Object obj, string path)
        {
            if (obj != null)
                File.WriteAllText(path, JSON.Serialize(obj));
        }

        public static void SaveAs(this Object obj, string path, Formatting format)
        {
            if (obj != null)
                File.WriteAllText(path, JsonConvert.SerializeObject(obj, format));
        }
#endregion

        public static void SaveDictAs(this IEnumerable<KeyValuePair<string, string>> dict, string path, string separator = " :=: ")
        {
            if (dict != null)
                File.WriteAllLines(
                    path, dict.Select(
                        kv => kv.Key + separator + kv.Value
                    )
                );
        }
    }
}
