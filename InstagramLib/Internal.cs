using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using InstagramLib.Models;
using Jil;
using RestSharp;

namespace InstagramLib
{
    public static class Internal
    {
        private static Random GEN = new Random();

        private const string USERNAME = "zach_the_cat_guy";
        private const string PASSWORD = "pyromaniac1029";
        private static Lazy<RestClient> _client = new Lazy<RestClient>(() => Initialize());
        public static RestClient CLIENT { get => _client.Value; }
        private static readonly CookieContainer COOKIES = new CookieContainer();
        private static Lazy<Regex> _queryRGX = new Lazy<Regex>(
            () => new Regex(
                @"^\s" + "*<link rel=\"preload\" href=\"([^\"]+)\" as=\"fetch\"",
                RegexOptions.Compiled | RegexOptions.Multiline,
                TimeSpan.FromMilliseconds(100)
            )
        );
        private static string CSRF_TOKEN = null;
        private static EdgeWebFeedTimeLine _feed = null;
        public static EdgeWebFeedTimeLine InstagramFeed { get => _client.Value != null ? _feed : null; }
        private static RestClient _apiClient = new RestClient("https://i.instagram.com")
        {
            FollowRedirects = false,
            CookieContainer = COOKIES,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36+"
        };
        private static RestClient _internalClient = new RestClient("https://www.instagram.com")
        {
            FollowRedirects = true,
            CookieContainer = COOKIES,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36"
        };
        private static string[] USERNAMES_LIST = null;//USERNAME == "zach_the_cat_guy" ? File.ReadAllLines(@"C:\Program Files\Microsoft Games\Mahjong\en-US\resources\assets\Stalking\FilteredUsersMain.txt") : File.ReadAllLines(@"C:\Program Files\Microsoft Games\Mahjong\en-US\resources\assets\Stalking\FilteredUsersOther.txt");

        public static RestClient Initialize()
        {
            var response = _internalClient.Execute(new RestRequest("/", Method.GET));
            _internalClient.BaseUrl = new Uri("https://www.instagram.com/");

            RestRequest request = new RestRequest("accounts/login/ajax/", Method.POST);
            request.AddParameter("username", USERNAME, ParameterType.GetOrPost);
            request.AddParameter("password", PASSWORD, ParameterType.GetOrPost);
            request.AddParameter("queryParams", "{}", ParameterType.GetOrPost);
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("X-Instagram-Ajax", "1");
            request.AddHeader("X-CSRFToken", GetCSRF(response));

            response = _internalClient.Execute(request);
            CSRF_TOKEN = GetCSRF(response);
            var queryMatch = _queryRGX.Value.Match(response.Content);

            _apiClient.RemoveDefaultParameter("Accept");
            _apiClient.AddDefaultHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            return _internalClient;
        }

        public const string STORIES_DIR = @"%temp%\InstagramStories\";
        public static Dictionary<UserOwner, Story[]> GetStories()
        {
            if (!Directory.Exists(STORIES_DIR))
                Directory.CreateDirectory(STORIES_DIR);
            var reelsResponse = _apiClient.Execute(new RestRequest("/api/v1/feed/reels_tray/", Method.GET));
            var users = JSON.Deserialize<Stories>(reelsResponse.Content).tray;
            
            foreach(var user in users)
            {
                if (USERNAMES_LIST.Contains(user.user.username))
                {
                    var userTrayResponse = _apiClient.Execute(new RestRequest("/api/v1/feed/user/" + user.user.id + "/reel_media", Method.GET));
                    var stories = JSON.Deserialize<UserReelTray>(userTrayResponse.Content).tray;
                    if (stories != null && stories.Any())
                    {
                        stories = stories.Where(s => s.media_type != 1).ToArray();
                        if (stories.Any())
                        {
                            var userStoriesDir = STORIES_DIR + user.user.username + @"\";
                            if (!Directory.Exists(userStoriesDir))
                                Directory.CreateDirectory(userStoriesDir);
                            foreach(var story in stories)
                            {
                                var video = story.video_versions.OrderByDescending(v => v.height * v.width).First();
                                File.WriteAllBytes(userStoriesDir + new Uri(video.url).Segments.Last(), _apiClient.Execute(new RestRequest(video.url, Method.GET)).RawBytes);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static UserSearchPosition[] Search(string query)
        {
            RestRequest request = new RestRequest("web/search/topsearch/");
            request.AddParameter("context", "blended", ParameterType.QueryString);
            request.AddParameter("query", query, ParameterType.QueryString);
            request.AddParameter("rank_token", GEN.NextDouble(), ParameterType.QueryString);
            request.AddParameter("include_reel", true, ParameterType.QueryString);

            var response = _internalClient.Execute(request);
            return JSON.Deserialize<UserSearchPosition[]>(JSON.DeserializeDynamic(response.Content).users.ToString());
        }

        public static FullUser GetUser(string username) =>
            JSON.Deserialize<FullUser>(
                JSON.DeserializeDynamic(
                    _internalClient.Execute(
                        new RestRequest(username + "/").AddParameter("__a", 1, ParameterType.QueryString)
                    ).Content
                ).graphql.user.ToString()
            );

        private static string GetCSRF(IRestResponse response) => response.Cookies.First(c => c.Name == "csrftoken" && !String.IsNullOrWhiteSpace(c.Value)).Value;
    }
}
