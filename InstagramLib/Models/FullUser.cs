using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InstagramLib.API_Models;

namespace InstagramLib.Models
{
    public class FullUser : BaseUser
    {
        public Edges.EdgeOwnerToTimelineMedia edge_owner_to_timeline_media { get; set; }
    }
}
