using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace InstagramLib.API_Models
{
    public class FeedItemProfile : FeedItem
    {
        //[JilDirective("can_view_more_preview_comments")]
        public bool can_view_more_preview_comments { get; private set; }
        public int comment_count { get; private set; }
        public bool comment_likes_enabled { get; private set; }
        public bool comment_threading_enabled { get; private set; }
        public bool has_more_comments { get; private set; }

        public bool has_liked { get; private set; }
        public bool like_count { get; private set; }
        public string[] top_likers { get; private set; }

        public BaseFeedItem[] carousel_media { get; private set; }
    }
}
