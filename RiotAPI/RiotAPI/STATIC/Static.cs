using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI.STATIC
{
    public struct ImageDto
    {
        public string full { get; set; }
        public string group { get; set; }
        public string sprite { get; set; }
        public int h { get; set; } // height
        public int w { get; set; } // width
        public int y { get; set; }
        public int x { get; set; }
    }

    public struct SpellVarsDto
    {
        public string ranksWith { get; set; }
        public string dyn { get; set; }
        public string link { get; set; }
        public List<double> coeff { get; set; }
        public string key { get; set; }
    }
}
