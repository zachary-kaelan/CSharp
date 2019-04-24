using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class SnapshotPhoto
    {
        public string full { get; protected set; }
        public string full_small { get; protected set; }
        public SnapshotPhotoInfo info { get; protected set; }
        public string original { get; protected set; }
    }

    public class SnapshotPhotoInfo
    {
        public string caption { get; protected set; }
        public string id { get; protected set; }
        public SnapshotPhotoParametersFull full { get; protected set; }
        public SnapshotPhotoParameters original { get; protected set; }
    }

    public class SnapshotPhotoParameters
    {
        public PhotoSize size { get; protected set; }
        public PhotoSize thumbnail { get; protected set; }
    }

    public class SnapshotPhotoParametersFull : SnapshotPhotoParameters
    {
        public PointPercentages center { get; protected set; }
    }

    public struct PointPercentages
    {
        public int x { get; protected set; }
        public int y { get; protected set; }
        public int x_percent { get; protected set; }
        public int y_percent { get; protected set; }
    }

    public struct PhotoSize
    {
        public int height { get; protected set; }
        public int width { get; protected set; }
    }

}
