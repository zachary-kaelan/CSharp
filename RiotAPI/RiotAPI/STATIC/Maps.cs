using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI.STATIC
{
    public struct MapDataDto
    {
        public Dictionary<string, MapDetailsDto> data { get; set; }
        public string version { get; set; }
        public string type { get; set; }
    }

    public struct MapDetailsDto
    {
        public string mapName { get; set; }
        public ImageDto image { get; set; }
        public long mapId { get; set; }
        public List<long> unpurchaseableItemList { get; set; }
    }
}
