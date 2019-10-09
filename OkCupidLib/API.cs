using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Jil;
using OkCupidLib.Models;
using RestSharp;
using ZachLib;
using ZachLib.HTTP;

namespace OkCupidLib
{
    public class API
    {
        static API()
        {
            foreach(var cookie in Utils.LoadDictionary(PATH_MAIN + "Cookies.txt"))
            {
                COOKIES.Add(
                    new Cookie(
                        cookie.Key,
                        cookie.Value,
                        "/",
                        "okcupid.com"
                    )
                );
            }

            CLIENT_MAIN.AddDefaultHeader("Accept", "application/json, text/javascript, */*; q=0.01");
            CLIENT_MAIN.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            CLIENT_MAIN.AddDefaultHeader("Accept-Language", "en-US,en;q=0.9");
            CLIENT_MAIN.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");
        }

        public const string PATH_MAIN = @"E:\OkCupid\";

        private static readonly CookieContainer COOKIES = new CookieContainer();
        private static readonly RestClient CLIENT_MAIN = new RestClient("www.okcupid.com/")
        {
            CookieContainer = COOKIES,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36"
        };

        public static SyncData SyncData { get; protected set; }

        private static SyncData Sync() =>
            JSON.Deserialize<SyncData>(
                CLIENT_MAIN.Execute(
                    new RestRequest(
                        "json/spotlight/sync?spotlight=1",
                        Method.GET
                    )
                ).Content
            );

        private static readonly Lazy<Regex> RGX_DOUBLETAKE = new Lazy<Regex>(() => new Regex(@"^Ok\.QuickMatchParams = (.+);", RegexOptions.Multiline));
        public static DoubleTake GetDoubleTake()
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(
                CLIENT_MAIN.Execute(
                    new RestRequest(
                        "doubletake",
                        Method.GET
                    )
                ).Content
            );

            var node = doc.DocumentNode;
            node = node.SelectSingleNode("/html/body/script[11]");
            var doubletake = JSON.Deserialize<DoubleTake>(RGX_DOUBLETAKE.Value.Match(node.InnerText).Groups[1].Value);
            node = null;
            doc = null;
            return doubletake;
        }

        public static T GetProfile<T>(string userid, string fields) =>
            JSON.Deserialize<T>(
                CLIENT_MAIN.Execute(
                    new RestRequest(
                        "1/apitun/profile/" + userid + "/fields",
                        Method.GET
                    ).AddOrUpdateParameter("fields", fields, ParameterType.QueryString)
                ).Content
            );

        public static T GetProfile<T>(string userid, Fields fields) => GetProfile<T>(userid, fields.ToString());

        public static Profile GetProfile(string userid) => GetProfile<Profile>(userid, "thumbs.limit(1),userinfo,percentages,online,detail_tags,personality_traits");

        #region Discovery
        private static DiscoveryResultsData GetDiscoverySectionData(DiscoverySearchSection section, int limit)
        {
            var query = new DiscoveryResultsQuery(section, limit);
            return JSON.Deserialize<DiscoveryResultsData>(
                CLIENT_MAIN.Execute(
                    new RestRequest(
                        "1/apitun/discovery/section",
                        Method.POST
                    ).AddJsonBody(query)
                ).Content
            );
        }

        public static DiscoveryResult[] GetDiscoverySection(DiscoverySearchSection section, int limit)
        {
            var results = GetDiscoverySectionData(section, limit).results.rows.data;
            if (section == DiscoverySearchSection.instagram)
            {
                int count = results.Length;
                for (int i = 0; i < count; ++i)
                {
                    results[i].instagram.RemoveAll(p => p.height == 0);
                }
            }
            return results;
        }

        public static (Question[], DiscoveryResult[]) GetImportantQuestionsDiscovery(DiscoverySearchSection section, int limit)
        {
            var data = GetDiscoverySectionData(section, limit);
            return (data.header_component.metadata.questions, data.results.rows.data);
        }
        #endregion

        #region Questions
        public static QuestionListModel[] SearchQuestions(string query) =>
            JSON.Deserialize<QuestionListModel[]>(
                CLIENT_MAIN.Execute(
                    new RestRequest(
                        "1/apitun/questions/search",
                        Method.GET
                    ).AddOrUpdateParameter("query", query, ParameterType.QueryString)
                ).Content
            );

        public static QuestionInfo GetQuestionInfo(int id) =>
            JSON.Deserialize<QuestionInfo>(
                CLIENT_MAIN.Execute(
                    new RestRequest(
                        "1/apitun/questions/" + id.ToString(),
                        Method.GET
                    )speb
                ).Content
            );
        #endregion

        public static SearchResultsEnumerator<InterestListModel> SearchInterests(string query) =>
            new SearchResultsEnumerator<InterestListModel>(
                JSON.Deserialize<DataResponse<InterestListModel>>(
                    CLIENT_MAIN.Execute(
                        new RestRequest(
                            "1/apitun/interests/query",
                            Method.GET
                        ).AddOrUpdateParameter(
                            "q", query, 
                            ParameterType.QueryString
                        )
                    ).Content
                )
            );

        public static SearchResultsEnumerator<FullProfile> SearchMatches(MatchSearchQuery query) => SearchMatches<FullProfile>(query);

        public static SearchResultsEnumerator<T> SearchMatches<T>(MatchSearchQuery query) where T : BaseUserModel =>
            new SearchResultsEnumerator<T>(
                JSON.Deserialize<DataResponse<T>>(
                    CLIENT_MAIN.Execute(
                        new RestRequest(
                            "/1/apitun/match/search",
                            Method.POST
                        ).AddJsonBody(query)
                    ).Content
                )
            );

        internal static DataResponse<T> SearchResultsNextPage<T>(string cursor, string resource, object data = null)
        {
            RestRequest request = new RestRequest(resource);
            if (data == null)
            {
                // is interests search
                request.Method = Method.GET;
                request.AddParameter("after", cursor, ParameterType.QueryString);
            }                
            else
            {
                // is matches search
                request.Method = Method.POST;
                var query = (MatchSearchQuery)data;
                query.after = cursor;
                request.AddJsonBody(query);
            }
            return JSON.Deserialize<DataResponse<T>>(
                CLIENT_MAIN.Execute(request).Content
            );
        }

        public static SearchResultsEnumerator<T> GetConnections<T>(bool outgoing, Fields fields, int limit = 20) =>
            new SearchResultsEnumerator<T>(
                JSON.Deserialize<DataResponse<T>>(
                    CLIENT_MAIN.Execute(
                        new RestRequest(
                            "1/apitun/connections/" + (outgoing ? "outgoing" : "incoming"),
                            Method.GET
                        ).AddOrUpdateParameters(
                            new Parameter()
                            {
                                Name = "limit",
                                Value = limit,
                                Type = ParameterType.QueryString
                            },
                            new Parameter()
                            {
                                Name = "fields",
                                Value = fields.ToString(),
                                Type = ParameterType.QueryString
                            }
                        )
                    ).Content
                )
            );

        public static SearchResultsEnumerator<ConnectionsProfile> GetConnections(bool outgoing, int limit = 20) => GetConnections<ConnectionsProfile>(outgoing, ConnectionsProfile.FIELDS_DEFAULT, limit);
    }
}
