using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.API_Models
{
    public class FeedImage
    {
        public int height { get; private set; }
        public string url { get; private set; }
        public int width { get; private set; }
    }

    internal class ImageVersions
    {
        public FeedImage[] candidates { get; private set; }
    }
}
