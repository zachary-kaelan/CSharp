using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RiotAPI.MATCHES;
using System.IO;
using Jil;
using RateLimiter;
using System.Net;
using RiotAPI.STATIC;

namespace RiotAPI.MATCHES
{
    public class Client
    {
        private const string APIKey = "RGAPI-a2848882-9f00-4517-8a37-b75caf1857f3";
        private const string ID = "52860022";
        private const string AccountID = "208875684";
        private const string version = "/v3/";
        public const string MAIN_PATH = @"E:\RiotAPIData\";
        public const string STATIC_PATH = MAIN_PATH + @"Static\";
        public const string VERSIONS_PATH = STATIC_PATH + "versions.txt";
        public const string MATCH_HIST_PATH = MAIN_PATH + @"Match History\";
        public const int MAX_REQUESTS_PER_SEC = 20;
        public const int MAX_REQUEST_PER_2MIN = 100;

        /*
        public int requestsPerSec { get; set; }
        public int requestsPer2Min { get; set; }
        public Stopwatch secTimer { get; set; }
        public Stopwatch MinTimer {get; set;}
        */

        private RestClient client { get; set; }
        private bool backup { get; set; }
        private TimeLimiter rateLimiter { get; set; }
        //private RateMonitor monitor { get; set; }
        //private Thread managerThread { get; set; }
        //private SpinLock rateLock { get; set; }

        public Client(bool backup = false)
        {
            //this.monitor = new RateMonitor(Thread.CurrentThread);
            //this.rateLock = new SpinLock();
            //this.managerThread = new Thread(() => this.ThreadManager(this.monitor));
            /*rateLimiter = TimeLimiter.Compose(
                new CountByIntervalAwaitableConstraint(20, TimeSpan.FromSeconds(1)),
                new CountByIntervalAwaitableConstraint(200, TimeSpan.FromMinutes(2))
            );*/

            this.backup = backup;
            this.client = new RestClient("https://na1.api.riotgames.com/lol/");
            this.client.AddDefaultParameter("X-Riot-Token", APIKey, ParameterType.HttpHeader);
            this.client.Proxy = new RateLimiter();

            List<string> versions = JSON.Deserialize<List<string>>(
                this.client.Execute(
                    new RestRequest(
                        "static-data" + version + "versions"
                    )
                ).Content
            );

            if (File.ReadAllLines(VERSIONS_PATH).Length < versions.Count)
            {
                this.UpdateStaticData();
                File.WriteAllLines(VERSIONS_PATH, versions);
            }
        }

        public void UpdateStaticData()
        {
            string[] staticTypes = new string[]
            {
                "champions",
                "items",
                "maps",
                "masteries",
                "runes",
                "summoner-spells"
            };

            foreach(string staticType in staticTypes)
            {
                File.WriteAllText(
                    STATIC_PATH + staticType + ".txt",
                    this.client.Execute(
                        new RestRequest(
                            "static-data" + version + staticType + "?tags=all",
                            Method.GET
                        )
                    ).Content
                );
            }
            
        }

        public List<long> GetGameIDs()
        {
            List<long> gameIds = new List<long>();
            LOLNav lolnav = new LOLNav();
            lolnav.Login();
            MatchHistory matchHist = lolnav.GetMatchHistory();
            gameIds.AddRange(matchHist.games.games.Select(g => g.gameId));
            int count = matchHist.games.gameCount;
            for (int i = 16; i <= count; i += 15)
            {
                matchHist = lolnav.GetMatchHistory(i, (count - i >= 15 ? (i + 15) : count));
                gameIds.AddRange(matchHist.games.games.Select(g => g.gameId));
            }

            if (this.backup)
                File.WriteAllLines(MAIN_PATH + "GameIDs.txt", gameIds.Select(g => g.ToString()));

            return gameIds;
        }

        public List<MatchDto> GetFullMatchHistory()
        {
            List<string> matches =
                this.GetGameIDs().Select(i => i.ToString()
                ).Except(
                    Directory.GetFiles(
                        MATCH_HIST_PATH).Select(
                        m => new String(
                            new FileInfo(m).Name.TakeWhile(
                                c => c != '.'
                            ).ToArray()))).Select(gameId =>
                    this.rateLimiter.Perform<string>(() =>
                        Task.Factory.StartNew<string>(() =>
                        this.client.Execute(
                            new RestRequest(
                                "match" + version + "matches/" +
                                gameId, Method.GET)
                    ).Content)).Result).ToList();

            if (backup)
                matches.ForEach(m => 
                    File.WriteAllText(MATCH_HIST_PATH + JSON.DeserializeDynamic(m).gameId + ".txt", m)
                );

            return matches.Select(m => JSON.Deserialize<MatchDto>(m)).ToList();
        }

        
    }

    public struct RateLimiter : IWebProxy
    {
        public ICredentials Credentials { get; set; }
        private static TimeLimiter ratelimit = TimeLimiter.Compose(
                new CountByIntervalAwaitableConstraint(20, TimeSpan.FromSeconds(1)),
                new CountByIntervalAwaitableConstraint(200, TimeSpan.FromMinutes(2))
            );

        public Uri GetProxy(Uri url)
        {
            Task t = ratelimit.Perform(() => null);
            t.Start();
            t.Wait();
            return url;
        }

        public bool IsBypassed(Uri url)
        {
            return true;
        }
    }

    /*
    public struct RateMonitor
    {
        public Stopwatch timer { get; set; }
        public int requests { get; set; }
        public Thread curThread { get; set; }
        public int[] requestsHistory { get; set; }

        public RateMonitor(Thread thread)
        {
            curThread = thread;
            requests = 0;
            requestsHistory = new int[Client.MAX_REQUEST_PER_2MIN];

            timer = new Stopwatch();
            timer.Start();
        }

        public void RequestMade()
        {
            requestsHistory[]
            ++this.requests;
        }
    }*/
}
