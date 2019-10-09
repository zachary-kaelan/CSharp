using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HingeAPI.Models
{
    public enum PhotoSource : byte
    {
        facebook,
        camera,
        instagram
    }

    public sealed class Photo
    {
        public BoundingBox boundingBox { get; private set; }
        public string caption { get; private set; }
        public string cdnId { get; private set; }
        public ushort height { get; private set; }
        public string location { get; private set; }
        public PhotoSource source { get; private set; }
        public string sourceId { get; private set; }
        public string url { get; private set; }
        public Video[] videos { get; private set; }
        public ushort width { get; private set; }

        public struct BoundingBoxCorner
        {
            public ushort x { get; set; }
            public ushort y { get; set; }
        }

        public struct BoundingBox
        {
            public BoundingBoxCorner bottomRight { get; set; }
            public BoundingBoxCorner topLeft { get; set; }
        }
    }
}
