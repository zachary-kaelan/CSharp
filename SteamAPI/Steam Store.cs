using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jil;
using PPLib;
using RestSharp;
using RGX.STEAM;

namespace SteamAPI
{
    public static class Steam_Store
    {
        private static readonly AppTags RGX_TAGS = new AppTags();

        private static readonly RestClient client = new RestClient("store.steampowered.com/");
        static Steam_Store()
        {
            client.AddDefaultParameter("json", "1", ParameterType.GetOrPost);
            client.AddDefaultParameter("language", "english", ParameterType.GetOrPost);
        }

        public List<UserReview> GetReviews(UInt32 appid)
        {
            return GetReviews(appid.ToString());
        }

        public List<UserReview> GetReviews(string appid)
        {
            List<UserReview> reviews = new List<UserReview>();

            int total = -1;
            int offset = 0;
            client.AddDefaultHeader("start_offset", "0");
            RestRequest request = new RestRequest("appreviews/" + appid);

            do
            {
                dynamic dyn = JSON.DeserializeDynamic(client.Execute(request).Content);
                if (total == -1)
                    total = dyn.query_summary.total_reviews;
                reviews.AddRange(dyn.reviews);
                offset += 20;
                client.RemoveDefaultParameter("start_offset");
                client.AddDefaultHeader("start_offset", offset.ToString());
            } while (offset < total);

            client.RemoveDefaultParameter("start_offset");
            request = null;
            return reviews;
        }

        public IEnumerable<string> GetTags(string appid)
        {
            return RGX_TAGS.MatchesValues(
                client.Execute(
                    new RestRequest(
                        "apphover/" + appid,
                        Method.GET
                    )
                ).Content,
                "Tag"
            );
        }
    }

    public struct UserReview
    {
        public string recommendationid { get; set; }
        public Author author { get; set; }
        public string language { get; set; }
        public string review { get; set; }
        public int timestamp_created { get; set; }
        public int timestamp_updated { get; set; }
        public bool voted_up { get; set; }
        public int votes_up { get; set; }
        public int votes_funny { get; set; }
        public string weighted_vote_score { get; set; }
        public string comment_count { get; set; }
        public bool steam_purchase { get; set; }
        public bool received_for_free { get; set; }
        public bool written_during_early_access { get; set; }
    }

    public struct Author
    {
        public string steamid { get; set; }
        public int num_games_owned { get; set; }
        public int num_reviews { get; set; }
        public int playtime_forever { get; set; }
        public int playtime_last_two_weeks { get; set; }
        public int last_played { get; set; }
    }
}
