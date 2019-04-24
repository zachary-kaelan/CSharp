using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrackingLib
{
    internal class InstagramSessionData
    {
        public string rhx_gis { get; private set; }
        public string nonce { get; private set; }
        public float mid_pct { get; private set; }
        public string rollout_hash { get; private set; }
    }
}
