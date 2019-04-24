using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace OkCupidLib.Models
{
    public class SyncData
    {
        public Dictionary<string, object> modelupdate { get; protected set; }
        public object[] activity { get; protected set; }
        public int implevel { get; protected set; }
        public bool autoboost { get; protected set; }
        public PromoData lastpromo { get; protected set; }
        public object impressions { get; protected set; }
        public object promo { get; protected set; }
        public object[] queue { get; protected set; }
    }
}
