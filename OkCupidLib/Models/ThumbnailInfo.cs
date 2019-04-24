using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class ThumbnailInfo
    {
        public string path { get; protected set; }
        public int when_taken { get; protected set; }
        public int type { get; protected set; }
        public int lower_right_x { get; protected set; }
        public int lower_right_y { get; protected set; }
        public int ordinal { get; protected set; }
        public string caption { get; protected set; }
        public int width { get; protected set; }
        public int height { get; protected set; }
        public bool has_photos { get; protected set; }
        public long when_uploaded { get; protected set; }
        public int upper_left_x { get; protected set; }
        public int upper_left_y { get; protected set; }
        public string thumbnail { get; protected set; }
        public string picid { get; protected set; }
    }
}
