using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class FullProfile : APIProfile
    {
        public List<InstagramPhoto> instagram { get; protected set; }
        public SnapshotPhoto[] more_photos { get; protected set; }
        public Interest[] interests { get; protected set; }
    }
}
