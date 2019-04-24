using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class DiscoveryRows
    {
        public DiscoveryResult[] data { get; protected set; }
    }

    public class DiscoveryResult : BaseProfileModel
    {
        public Likes likes { get; protected set; }
        public SnapshotPhoto[] photos { get; protected set; }
        public SnapshotPhoto[] more_photos { get; protected set; }
        public Question question { get; protected set; }
        public List<InstagramPhoto> instagram { get; protected set; }
    }

    public struct Likes
    {
        public byte mutual_like { get; protected set; }
        public byte mutual_like_vote { get; protected set; }
        public byte passed_on { get; protected set; }
        public byte recycled { get; protected set; }
        public byte? they_like { get; protected set; }
        public byte via_spotlight { get; protected set; }
        public byte you_like { get; protected set; }
    }

    public struct LastContacts
    {
        public byte forward { get; protected set; }
        public byte reverse { get; protected set; }
    }
}
