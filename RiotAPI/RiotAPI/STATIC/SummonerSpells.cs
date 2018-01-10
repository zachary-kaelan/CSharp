using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI.STATIC
{
    public struct SummonerSpellListDto
    {
        public Dictionary<string, SummonerSpellDto> data { get; set; }
        public string version { get; set; }
        public string type { get; set; }
    }


    public struct SummonerSpellDto
    {
        public List<SpellVarsDto> vars { get; set; }
        public ImageDto image { get; set; }
        public string costBurn { get; set; }
        public List<double> cooldown { get; set; }
        public List<string> effectBurn { get; set; }
        public int id { get; set; }
        public string cooldownBurn { get; set; }
        public string tooltip { get; set; }
        public int maxrank { get; set; }
        public string rangeBurn { get; set; }
        public string description { get; set; }
        public List<List<double>> effect { get; set; }
        public string key { get; set; }
        public Dictionary<string, List<string>> leveltip { get; set; }
        public List<string> modes { get; set; }
        public string resource { get; set; }
        public string name { get; set; }
        public string costType { get; set; }
        public string sanitizedDescription { get; set; }
        public string sanitizedTooltip { get; set; }
        public object range { get; set; } // List<int> or string("self")
        public List<int> cost { get; set; }
        public int summonerLevel { get; set; }
    }

    // LevelTipDto - Dictionary<string, List<string>>
}
