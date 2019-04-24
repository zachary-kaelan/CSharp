using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.API_Models
{
    public class FeedVideo : FeedImage
    {
        public long id { get; private set; }
        public byte type { get; private set; }
    }

    internal class VideoVersions
    {
        public FeedVideo[] candidates { get; private set; }
    }
}
