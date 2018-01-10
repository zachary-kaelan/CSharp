using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAPI
{
    public interface App
    {
        UInt32 appid { get; set; }
        string name { get; set; }
    }

    public struct OwnedGame : App
    {
        public UInt32 appid { get; set; }
        public string name { get; set; }
        public int playtime_forever { get; set; }
        public string img_icon_url { get; set; }
        public string img_logo_url { get; set; }
        public bool has_community_visible_stats { get; set; }
    }

    public interface SteamUser
    {
        UInt64 steamid { get; set; }
    }
}
