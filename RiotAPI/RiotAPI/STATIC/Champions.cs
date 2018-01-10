using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI.STATIC
{
    public struct ChampionListDto
    {
        public Dictionary<string, string> keys { get; set; }
        public Dictionary<string, ChampionDto> data { get; set; }
        public string version { get; set; }
        public string type { get; set; }
        public string format { get; set; }
    }

    public struct ChampionDto
    {
        public Dictionary<string, int> InfoDto { get; set; }
        public List<string> enemytips { get; set; }
        public Dictionary<string, double> StatsDto { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public ImageDto image { get; set; }
        public List<string> tags { get; set; }
        public string partype { get; set; }
        public List<SkinDto> skins { get; set; }
        public PassiveDto passive { get; set; }
        public List<RecommendedDto> recommended { get; set; }
        public List<string> allytips { get; set; }
        public string key { get; set; }
        public string lore { get; set; }
        public int id { get; set; }
        public string blurb { get; set; }
        public List<ChampionSpellDto> spells { get; set; }
    }

    // Stats - Dictionary<string, double>
    // Info - Dictionary<string, int>

    public struct SkinDto
    {
        public int num { get; set; }
        public string name { get; set; }
        public int id { get; set; }
    }

    public struct PassiveDto
    {
        public ImageDto image { get; set; }
        public string sanitizedDescription { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public struct RecommendedDto
    {
        public string map { get; set; }
        public List<BlockDto> blocks { get; set; }
        public string champion { get; set; }
        public string title { get; set; }
        public bool priority { get; set; }
        public string mode { get; set; }
        public string type { get; set; }
    }

    public struct BlockDto
    {
        public Dictionary<string, int> items { get; set; } // List of BlockItemDto
        public bool recMath { get; set; }
        public string type { get; set; }
    }

    // BlockItemDto - Dictionary<string, int>
    public struct ChampionSpellDto
    {
        public string cooldownBurn { get; set; }
        public string resource { get; set; }
        public Dictionary<string, List<string>> leveltip { get; set; }
        public List<SpellVarsDto> vars { get; set; }
        public string costType { get; set; }
        public ImageDto image { get; set; }
        public string sanitizedDescription { get; set; }
        public string sanitizedTooltip { get; set; }
        public List<List<double>> effect { get; set; }
        public string tooltip { get; set; }
        public int maxrank { get; set; }
        public string costBurn { get; set; }
        public string rangeBurn { get; set; }
        public object range { get; set; } // either List<int> or the string "self"
        public List<double> cooldown { get; set; }
        public List<int> cost { get; set; }
        public string key { get; set; }
        public string description { get; set; }
        public List<string> effectBurn { get; set; }
        public List<ImageDto> altimages { get; set; }
        public string name { get; set; }
    }

    // LevelTipDto - Dictionary<string, List<string>>
}
