using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace RiotAPI.STATIC
{
    public struct GoldDto
    {
        public int sell { get; set; }
        public int total { get; set; }
        
        public int BASE { get; set; }
        public bool purchaseable { get; set; }
    }

    // GroupDto - dictionary
    
    
    public struct ItemDto
    {
        public GoldDto gold { get; set; }
        public string plaintext { get; set; }
        public bool hideFromAll { get; set; }
        public bool inStore { get; set; }
        public List<string> into { get; set; }
        public int id { get; set; }
        public Dictionary<string, double> stats { get; set; }
        public string colloq { get; set; }
        public Dictionary<string, bool> maps { get; set; }
        public int specialRecipe { get; set; }
        public ImageDto image { get; set; }
        public string description { get; set; }
        public List<string> tags { get; set; }
        public Dictionary<string, string> effect { get; set; }
        public string requiredChampion { get; set; }
        public List<string> from { get; set; }
        public string group { get; set; }
        public bool consumeOnFull { get; set; }
        public string name { get; set; }
        public bool consumed { get; set; }
        public string sanitizedDescription { get; set; }
        public int depth { get; set; }
        public int stacks { get; set; }
    }

    public struct ItemListDto
    {
        public Dictionary<string, ItemDto> data { get; set; }
        public string version { get; set; }
        public List<ItemTreeDto> tree { get; set; }
        public List<Dictionary<string, string>> groups {get; set;}
        public string type { get; set; }
    }

    public struct ItemTreeDto
    {
        public string header { get; set; }
        public List<string> tags { get; set; }
    }

    // InventoryDataStats are all doubles, can be parsed to a dictionary.
}
