using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
using Jil;
using ZachLib;

namespace SteamDiscussions
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Net.CookieContainer cookies = new System.Net.CookieContainer();
            RestClient client = new RestClient("http://steamcommunity.com/forum/11814497/");
            client.CookieContainer = cookies;
            RestRequest request = new RestRequest("General/render/5/", Method.GET);
            request.AddParameter("start", "0", ParameterType.QueryString);
            request.AddParameter("count", "50", ParameterType.QueryString);
            Regex rgx = new Regex(@"forum_General_11814497_5_(?<TopicID>\d+)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

            int total_count = 782;
            List<string> topicIDsTemp = new List<string>();

            for (int i = 50; i < total_count; i += 50)
            {
                string html = JSON.DeserializeDynamic(
                    client.Execute(
                        request
                    ).Content
                ).topics_html;
                topicIDsTemp.AddRange(rgx.MatchesValues(html, "TopicID"));
                html = null;

                request = (RestRequest)request.AddOrUpdateParameter(
                    "start",
                    i - 50,
                    ParameterType.QueryString
                );
            }

            client = new RestClient("http://steamcommunity.com/comment/ForumTopic/render/103582791441335905/");
            client.CookieContainer = cookies;
            request = new RestRequest("364040961436825335/", Method.POST);
            //string string1 = "InitializeForumTopic( ";
            string string2 = "\"comments_raw\":";
            //string string3 = "		);";
            string string4 = ",\"track_navigation\":";
            Dictionary<string, IEnumerable<Comment>> topics = new Dictionary<string, IEnumerable<Comment>>();

            request.AddParameter("sessionid", "32deddf79bc9a5b88366cc6f", ParameterType.GetOrPost);
            request.AddParameter("count", 50);
            request.AddParameter("oldestfirst", true);
            request.AddParameter("include_raw", true);
            request.AddParameter("extended_data", "{\"topic_permissions\":{\"can_view\":1,\"can_post\":1,\"can_reply\":1,\"is_banned\":0,\"can_delete\":0,\"can_edit\":0},\"original_poster\":54402267,\"topic_gidanswer\":\"0\",\"forum_appid\":381210,\"forum_public\":1,\"forum_type\":\"General\",\"forum_gidfeature\":\"5\"}", ParameterType.GetOrPost);

            string[] topicIDs = topicIDsTemp.Distinct().ToArray();
            topicIDsTemp = null;
            foreach (string topicID in topicIDs)
            {
                List<Comment> comments = new List<Comment>();
                int start = 0;
                request = (RestRequest)request.AddOrUpdateParameter("feature2", topicID, ParameterType.GetOrPost);
                do
                {
                    request = (RestRequest)request.AddOrUpdateParameter("start", start, ParameterType.GetOrPost);
                    start += 50;

                    request = (RestRequest)request.AddOrUpdateParameter("topicID", topicID, ParameterType.UrlSegment);
                    string html = client.Execute(request).Content;
                    
                    //string html2 = html.Substring(0, html.IndexOf(string3));
                    //html = html.Substring(html.IndexOf(string2));

                    List<KeyValuePair<string, Comment>> comments = new List<KeyValuePair<string, Comment>>();
                    while (html != "[]")
                    {
                        int index2 = html.IndexOf(string2) + string2.Length;
                        html = html.Substring(index2, html.IndexOf(string4) - index2);

                        Dictionary<string, Comment> commentsTemp = new Dictionary<string, Comment>();

                        do
                        {
                            commentsTemp = JSON.Deserialize<Dictionary<string, Comment>>(html);
                            comments.AddRange(commentsTemp);
                        } while ()
                        topics.Add(topicID, comments.Where(c => Utils.COMPARE_INFO.IsSuffix(c.Value.author, "bhvr", Utils.IGNORE_CASE_AND_SYMBOLS).Select(c => c.Value));
                    }

                    html = null;
                } while (true);

                

                
            }

            var goodTopics = topics.Select(
                t => new Tuple<string, IEnumerable<Comment>, int>(
                    t.Key, t.Value,
                    t.Value.Count()
                )
            ).OrderByDescending(t => t.Item3).TakeWhile(t => t.Item3 > 0);

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }

    public struct Comment
    {
        public string author { get; set; }
        public string text { get; set; }
    }
}
