using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Slingshottest
{
    class Program
    {
        public const string path = @"C:\DocUploads\logs\";
        static void Main(string[] args)
        {
            string text = File.ReadAllText(path + "SlingshotTest.txt");
            string text2 = DecodeEncodedNonAsciiCharacters(text);
            string text3 = Regex.Unescape(text2);


            /*byte[] data = Encoding.ASCII.GetBytes(
                Regex.Unescape(
                    File.ReadAllText(path + "SlingshotTest.txt")
                )
            );
            byte[] data2 = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, data);*/
            //string text2 = Encoding.UTF8.GetString(data2);
            //string text = File.ReadAllText(path + "SlingshotTest.txt");
            //string text2 = Regex.Unescape(text);
            File.WriteAllText(path + "SlingshotTest2.txt", text2);
            File.WriteAllText(path + "SlingshotTest3.txt", text3);
        }

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m => {
                    return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                });
        }
    }
}
