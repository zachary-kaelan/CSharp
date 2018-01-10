using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using System.Web;

namespace TimeStation
{
    class Program
    {
        const string TimesPat = "\"nowrap\">(In|Out) at( .+?) On (.+?)<";
        const string InOutPat = @"(?(DEFINE)(?'padding'.+\s+<[^>]+>)(?'capture'([^<]*)))(?<=<tr valign)(?>.+\s+<[^>]+>(?P>capture)){7}(?=<\/td>\s*<\/tr>)";
        const string ActivityPat = "nowrap\"" + @" ?> (?:<.+>) ? (.+?) <\/(?:span|td)";

        static void Main(string[] args)
        {
            CookieContainer cookies = new CookieContainer();
            RestClient client = new RestClient("https://www.mytimestation.com/");
            client.CookieContainer = cookies;
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate");
            client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8");

            client.Execute(new RestRequest(Method.GET));
            client.AddDefaultHeader("Referer", "https://www.mytimestation.com/Login.asp");
            client.RemoveDefaultParameter("Accept-Encoding");
            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");

            RestRequest request = new RestRequest("Login.asp", Method.POST);
            request.AddHeader("Origin", "https://www.mytimestation.com");
            request.AddParameter("Password", "I15Zac$0208", ParameterType.GetOrPost);
            request.AddParameter("eMail", "z.johnson@insightpest.com", ParameterType.GetOrPost);
            request.AddParameter("submit", "Login", ParameterType.GetOrPost);
            client.Execute(request);

            var dates = Regex.Matches(
                client.Execute(
                    new RestRequest("User_Activity.asp", Method.GET)
                ).Content,
                InOutPat,
                RegexOptions.RightToLeft,
                TimeSpan.FromMilliseconds(1000)
            ).Cast<Match>()
            .Select(
                m => Regex.Matches(m.Value, InOutPat).Cast<Match>()
                .Select(inner => inner.Groups[1].Value).ToList()
            ).ToArray().GroupBy(
                m => new KeyValuePair<string, string>(m[0], m[1]),
                m => m.GetRange(2, 2).Select(
                    t => m[1] + " " + t
                ).Concat(new string[] { m[4]}).ToArray()
            ).Select(g => new Day(g));

            var days = matches.Select(
                d => new KeyValuePair<string, IEnumerable<string>>(
                    d.Key.Trim(), d.Select(
                        t => t.Replace(
                            d.Key, ""
                        )
                    )
                )
            );

            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        }
    }

    class Day
    {
        public string location { get; set; }
        public DateTime date { get; set; }
        public KeyValuePair<DateTime, DateTime>[] InOut { get; set; }
        public TimeSpan hours { get; set; }

        public Day(IGrouping<KeyValuePair<string, string>, string[]> day)
        {
            date = DateTime.Parse(day.Key.Key).Date;
            location = day.Key.Value;
            InOut = day.Select(
                io => new KeyValuePair<DateTime, DateTime>(
                    DateTime.Parse(io[0]),
                    DateTime.Parse(io[1])
                )
            ).ToArray();
            hours = TimeSpan.FromMinutes(day.Sum(d => TimeSpan.Parse(d[2]).TotalMinutes));
        }
    }
}
