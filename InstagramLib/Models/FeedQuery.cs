using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.Models
{
    public class FeedQuery
    {
        public string id { get; private set; }
        public string profile_pic_url { get; private set; }
        public string username { get; private set; }
        public EdgeWebFeedTimeLine edge_web_feed_timeline { get; private set; }
    }

    public class EdgeWebFeedTimeLine : EdgesTemp<GraphImage>
    {
        public PageInfo page_info { get; private set; }
    }
}
