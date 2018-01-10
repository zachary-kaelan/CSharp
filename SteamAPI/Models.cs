using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamAPI
{
    public struct AppModel
    {
        public string AppID { get; private set; }
        public string Name { get; private set; }
        public bool IsCommunityIntegrated { get; private set; }
        public string Version { get; private set; }

        public Dictionary<string, string> Icons { get; private set; }
    }
}
