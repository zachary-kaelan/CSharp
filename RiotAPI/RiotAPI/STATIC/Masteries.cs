using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI.STATIC
{
    public struct MasteryListDto
    {
        public Dictionary<string, MasteryDto> data { get; set; }
        public string version { get; set; }
        public Dictionary<string, List<MasteryTreeItemDto>> tree { get; set; }
        public string type { get; set; }
    }

    // MasteryTreeDto - Dictionary<string, Lit<MasteryTreeItemDto>

        /*
    public struct MasteryTreeListDto
    {
        public List<MasteryTreeItemDto> masteryTreeItems { get; set; }
    }
    */

    public struct MasteryTreeItemDto
    {
        public int masteryId { get; set; }
        public string prereq { get; set; }
    }

    public struct MasteryDto
    {
        public string prereq { get; set; }
        public string masteryTree { get; set; } // Cunning, Ferocity, Resolve
        public string name { get; set; }
        public int ranks { get; set; }
        public ImageDto image { get; set; }
        public int id { get; set; }
        public List<string> description { get; set; }
    }
}
