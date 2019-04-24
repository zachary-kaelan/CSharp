using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramLib.API_Models;
using Jil;
using RestSharp;

namespace InstagramLib
{
    public static class API
    {
        private const string CLIENT_ID = "b4eb75efa3b447659a6b0af625c87880";
        private static RestClient _client = null;
        private static RestClient _ICLIENT = null;
        private static long current_user_id { get; set; }

        private static void Initialize()
        {
            _client = new RestClient("https://api.instagram.com/");
            RestRequest request = new RestRequest("oauth/authorize/", Method.GET);
            request.AddParameter("client_id", CLIENT_ID, ParameterType.GetOrPost);
            request.AddParameter("redirect_uri", "https://www.example.com/", ParameterType.GetOrPost);
            request.AddParameter("response_type", "code", ParameterType.GetOrPost);
            request.AddParameter("scope", "basic public_content");

            _ICLIENT = new RestClient("http://i.instagram.com/api/v1/");
        }

        //public static BaseFeedItem

        public static string[] GetFollowing()
        {
            var variables = new Variables(current_user_id, 24);
            var variablesParam = new Parameter(
                "variables",
                JSON.Serialize(
                    variables,
                    Options.ExcludeNulls
                ),
                ParameterType.QueryString
            );

            RestRequest request = new RestRequest("https://www.instagram.com/graphql/query/");
            request.AddQueryParameter("query_hash", "c56ee0ae1f89cdbd1c89e2bc6b8f3d18");
            request.AddParameter(variablesParam);

            int count = 0;
            List<string> userIDs = new List<string>();
            var response = JSON.DeserializeDynamic(_client.Execute(request).Content).data.user.edge_follow;
            count = (int)response.count;
            while (userIDs.Count < count)
            {
                foreach(var keyvalue in response.edges)
                {
                    userIDs.Add(keyvalue.Value.id);
                }
                var pageInfo = (PageInfo)response.page_info;
                if (pageInfo.has_next_page)
                {
                    variables = new Variables(variables, 12, pageInfo.end_cursor);
                    variablesParam.Value = JSON.Serialize(variables, Options.ExcludeNulls);
                    response = JSON.DeserializeDynamic(_client.Execute(request).Content).data.user.edge_follow;
                }
            }

            return userIDs.ToArray();
        }

        private class Variables
        {
            public string id { get; private set; }
            public bool include_reel { get; private set; }
            public bool fetch_mutual { get; private set; }
            public int first { get; private set; }
            public string after { get; private set; }

            private Variables() { }

            public Variables(long id, int first)
            {
                this.id = id.ToString();
                this.first = first;
                include_reel = true;
                fetch_mutual = false;
                after = null;
            }

            public Variables(Variables prev, int first, string after)
            {
                id = prev.id;
                include_reel = prev.include_reel;
                fetch_mutual = prev.fetch_mutual;
                this.first = first;
                this.after = after;
            }
        }

        private class PageInfo
        {
            public string end_cursor { get; private set; }
            public bool has_next_page { get; private set; }
        }
    }
}
