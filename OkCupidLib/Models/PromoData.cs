using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class PromoData
    {
        public long now { get; protected set; }
        public long addtime { get; protected set; }
        public bool saw_report { get; protected set; }
        public string id { get; protected set; }
        public string userid { get; protected set; }
        public bool autoboost { get; protected set; }
        public int visits { get; protected set; }
        public int impressions { get; protected set; }
        public int votes { get; protected set; }
        public int duration { get; protected set; }
        public int contacts { get; protected set; }
    }
}
