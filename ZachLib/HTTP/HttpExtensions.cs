using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Match = System.Text.RegularExpressions.Match;

namespace ZachLib
{
    public static class HttpExtensions
    {
        //public static readonly TimeSpan ts100 = TimeSpan.FromMilliseconds(100);
        //public static readonly TimeSpan ts250 = TimeSpan.FromMilliseconds(250);


        public static bool IsOK(this HttpStatusCode code)
        {
            return (int)code < 400;
        }

        public static IEnumerable<Match> GetMatches(this HttpWebRequest request, Regex rgx)
        {
            return rgx.Matches(request.GetResponseString()).Cast<Match>();
        }

        public static Match GetMatch(this HttpWebRequest request, Regex rgx)
        {
            return rgx.Match(request.GetResponseString());
        }

        public static Dictionary<string, string> ToDictionary(this HttpWebRequest request, Regex rgx)
        {
            return rgx.ToDictionary(request.GetResponseString());
        }

        public static IEnumerable<Dictionary<string, string>> ToDictionaries(this HttpWebRequest request, Regex rgx)
        {
            return rgx.ToDictionaries(request.GetResponseString());
        }

        /*public static IEnumerable<T> AsEnumerable<T>(this HttpWebRequest request, Regex rgx)
        {
            return rgx.AsEnumerable<T>(request.GetResponseString());
        }*/

        public static string GetResponseString(this HttpWebRequest request)
        {
            var response = ((HttpWebResponse)request.GetResponse());
            request = null;
            return new StreamReader(
                response.ContentEncoding == "gzip" ?
                    new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) :
                    response.GetResponseStream()
                ).ReadToEnd();
        }
    }
}
