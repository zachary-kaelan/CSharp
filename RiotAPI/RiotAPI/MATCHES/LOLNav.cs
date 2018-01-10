using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using RestSharp;
using Jil;

namespace RiotAPI.MATCHES
{
    public class LOLNav
    {
        const string loginJSON = "{\"username\":\"meeko0111\",\"password\":\"9218820zj\",\"remember\":true,\"region\":\"NA1\",\"language\":\"en_US\",\"lang\":\"en_US\"}";
        private RestClient client { get; set; }
        private readonly CookieContainer Cookies = new CookieContainer();
        private readonly CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        private readonly CompareOptions opts = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols;
        private bool setup = false;
        public int gamesCount { get; set; }
        private string vaporToken { get; set; }

        public LOLNav()
        {
            this.gamesCount = 0;
            this.client = new RestClient();
            this.client.CookieContainer = this.Cookies;
            this.client.AddDefaultHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            this.client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            this.client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8");
            this.client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
        }

        public void Login()
        {
            RestRequest request = new RestRequest("https://login.leagueoflegends.com", Method.GET);
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Host", "login.leagueoflegends.com");
            request.AddHeader("Upgrade-Insecure-Requests", "1");

            var response = this.client.Execute(request);
            string query = response.Headers.ToList().Find(p => compInf.Compare(p.Name, "Location", opts) == 0).Value.ToString();
            request = new RestRequest(query, Method.GET);
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Host", "auth.riotgames.com");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            this.client.Execute(request);

            request = new RestRequest("https://auth.riotgames.com/authz/status", Method.POST);
            string queryJSON = "{\"query\":\""
                + query.Substring(37) +
                "\",\"region\":\"NA1\"}";
            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddParameter("application/json", queryJSON, ParameterType.RequestBody);
            request.AddHeader("Origin", "https://auth.riotgames.com");
            request.AddHeader("Host", "auth.riotgames.com");
            this.client.Execute(request);

            request = new RestRequest("https://auth.riotgames.com/authz/auth");
            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddParameter("application/json", loginJSON, ParameterType.RequestBody);
            request.AddHeader("Origin", "https://auth.riotgames.com");
            request.AddHeader("Host", "auth.riotgames.com");
            request.AddHeader("Referer", query);
            response = this.client.Execute(request);
            string oauth = JSON.DeserializeDynamic(response.Content).location;

            request = new RestRequest(oauth, Method.GET);
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Host", "login.leagueoflegends.com");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddHeader("Referer", query);
            response = this.client.Execute(request);
            Match matches = Regex.Match(response.Content, "name=\"token\" value=\"(.+?)\">.+?name=\"state\" value=\"(.+?)\">");
            this.vaporToken = this.Cookies.GetCookies(new Uri(".leagueoflegends.com")).Cast<Cookie>().ToList().Find(c => c.Name == "PVPNET_TOKEN_NA").Value;

            request = new RestRequest("https://login.lolesports.com/sso/login", Method.POST);
            request.AddHeader("Cache-Control", "max-age=0");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Referer", oauth);
            request.AddHeader("Host", "login.lolesports.com");
            request.AddHeader("Origin", "https://login.leagueoflegends.com");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddParameter("token", matches.Groups[1].Value, ParameterType.GetOrPost);
            request.AddParameter("state", matches.Groups[2].Value, ParameterType.GetOrPost);
            this.client.Execute(request);

            request = new RestRequest("https://login.leagueoflegends.com/sso/callback", Method.POST);
            request.AddHeader("Cache-Control", "max-age=0");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Referer", "https://login.lolesports.com/sso/login");
            request.AddHeader("Host", "login.leagueoflegends.com");
            request.AddHeader("Origin", "https://login.lolesports.com");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddParameter("token", matches.Groups[1].Value, ParameterType.GetOrPost);
            request.AddParameter("state", matches.Groups[2].Value, ParameterType.GetOrPost);
            this.client.Execute(request);
        }

        public MatchHistory GetMatchHistory(int begIndex=0, int endIndex=15)
        {
            RestRequest request = new RestRequest("http://matchhistory.na.leagueoflegends.com/en/");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Upgrade-Insecure-Requests", "1");
            request.AddHeader("Referer", "https://na.leagueoflegends.com/");
            request.AddHeader("Host", "matchhistory.na.leagueoflegends.com");

            Parameter beg = new Parameter();
            beg.Name = "begIndex";
            beg.Value = begIndex;
            beg.Type = ParameterType.QueryString;

            Parameter end = new Parameter();
            end.Name = "endIndex";
            end.Value = endIndex;
            end.Type = ParameterType.QueryString;

            if (!setup)
            {
                client.AddDefaultHeader("Origin", "http://matchhistory.na.leagueoflegends.com");
                client.AddDefaultHeader("Referer", "http://matchhistory.na.leagueoflegends.com/en/");
                client.AddDefaultHeader("Host", "acs.leagueoflegends.com");
                setup = true;
            }

            client.BaseUrl = new Uri("https://acs.leagueoflegends.com/v1/");
            request = new RestRequest("stats/player_history/auth", Method.OPTIONS);
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Access-Control-Request-Headers", "authorization,region");
            request.AddHeader("Access-Control-Request-Method", "GET");
            request.AddParameter(beg);
            request.AddParameter(end);
            client.Execute(request);

            request.Resource = "deltas/auth";
            request.Parameters.RemoveAll(p => p.Name.Contains("Index"));
            client.Execute(request);
            
            request = new RestRequest("stats/player_history/auth", Method.GET);
            request.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("Region", "NA");
            request.AddHeader("Authorization", "Vapor " + this.vaporToken);
            request.AddParameter(beg);
            request.AddParameter(end);
            var response = client.Execute(request);
            MatchHistory matchHist = JSON.Deserialize<MatchHistory>(response.Content);
            if (this.gamesCount == 0)
                this.gamesCount = matchHist.games.gameCount;

            return matchHist;
        }
    }

    public struct MatchHistory
    {
        public string accountId;
        public GameList games;
        public string platformId;
        public List<int> shownQueues;
    }

    public struct GameList
    {
        public int gameCount;
        public int gameIndexBegin;
        public int gameIndexEnd;
        public List<Game> games;
        public int gameTimestampBegin;
        public int gameTimestampEnd;
    }

    public struct Game
    {
        public long gameCreation;
        public int gameDuration;
        public long gameId;
        public string gameMode;
        public string gameType;
        public string gameVersion;
        public int mapId;
        public List<ParticipantIdentityDto> participantIdentities;
        public List<Participant> participants;
        public string platformId;
        public int queueId;
        public int seasonId;
    }

    public struct ParticipantIdentityDto
    {
        public int participantId;
        public PlayerDto player;
    }

    public struct PlayerDto
    {
        public long accountId;
        public long currentAccountId;
        public string currentPlatformId;
        public string matchHistoryUri;
        public string platformId;
        public int profileIcon;
        public long summonerId;
        public string summonerName;
    }

    public struct Participant
    {
        public int championId;
        public string highestAchievedSeasonTier;
        public List<Dictionary<string, int>> masteries;
        public int participantId;
        public List<Dictionary<string, int>> runes;
        public int spell1Id;
        public int spell2Id;
        public Dictionary<string, object> stats;
        public int teamId;
        public ParticipantTimelineDto timeline;
    }

    public struct Stats
    {
        public int assists;
        public bool causedEarlySurrender;
        public int champLevel;
        public int combatPlayerScore;

    }
}
