using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis
{
    class TinderData
    {
        public string name;
        public DateTime birth_date;
        public float score;
        public int num_photos;
        public bool? group_matched;
        public string birth_date_info;
        public string bio;
        public int? insta_media_count;

        [Jil.JilDirective(Name = "instagram", IsUnion = true)]
        public string instagram_user;
        [Jil.JilDirective(Name = "instagram", IsUnion = true)]
        public TinderInstaData instagram_info;

        public object jobs;
        public object schools;
        public object city;
        public object facebook;

        public int? distance_mi;
        public string type;
        public TinderSpotifyData spotify;

        public string snapchat;
        public string venmo;

        public class TinderInstaData
        {
            public string username;
            public int media_count;
            public DateTime last_fetch;
        }

        public class TinderSpotifyData
        {
            public string theme_track;
            public int num_artists;
        }
    }
}
