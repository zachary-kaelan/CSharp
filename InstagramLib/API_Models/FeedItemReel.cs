using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.API_Models
{
    public class FeedItemReel : FeedItem
    {
        public int caption_position { get; private set; }
        public long expiring_at { get; private set; }
        public byte has_shared_to_fb { get; private set; }
        public bool supports_reel_reactions { get; private set; }
    }
}
