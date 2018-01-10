using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAPI
{

    public struct GetOwnedGames
    {
        public UInt64 steamid { get; set; }
        public bool include_appinfo { get; set; }
        public bool include_played_free_games { get; set; }
        public UInt32[] appids_filter { get; set; }
    }
}
