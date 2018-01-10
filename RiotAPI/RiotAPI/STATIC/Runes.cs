using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI.STATIC
{
    public struct RuneListDto
    {
        public Dictionary<string, RuneDto> data { get; set; }
        public string version { get; set; }
        public string type { get; set; }
    }

    public struct RuneDto
    {
        public Dictionary<string, double> stats { get; set; }
        public string name { get; set; }
        public List<string> tags { get; set; }
        public ImageDto image { get; set; }
        public string sanitizedDescription { get; set; }
        public MetaDataDto rune { get; set; }
        public int id { get; set; }
        public string description { get; set; }
    }

    // RuneStatsDto - Dictionary<string, double>

    public struct MetaDataDto
    {
        public string tier { get; set; }
        public string type { get; set; }
        public bool isRune { get; set; }
    }
}
