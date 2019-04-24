using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class InstagramPhoto
    {
        public string caption { get; protected set; }
        public int height { get; protected set; }
        public int width { get; protected set; }
        public int likes { get; protected set; }
        public string large_path { get; protected set; }
        public string medium_path { get; protected set; }
        public string small_path { get; protected set; }
        public InstagramThumbnailPath thumb_paths { get; protected set; }
    }

    public class InstagramURL
    {
        public string url { get; protected set; }
    }

    public class InstagramThumbnailPath
    {
        public string small { get; protected set; }
    }
}
