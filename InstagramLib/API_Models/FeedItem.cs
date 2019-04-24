using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.API_Models
{
    public class FeedItem : BaseFeedItem
    {
        public Caption caption { get; private set; }
        public bool caption_is_edited { get; private set; }

        public string client_cache_key { get; private set; }
        public string code { get; private set; }
        public string organic_tracking_token { get; private set; }

        public long device_timestamp { get; private set; }
        public long taken_at { get; private set; }
        public byte filter_type { get; private set; }
        public bool photo_of_you { get; private set; }
    }
}
