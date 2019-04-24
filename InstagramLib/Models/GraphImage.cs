using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.Models
{
    public class GraphImage
    {
        public DisplayResource[] display_resources { get; private set; }
        public UserOwner owner { get; private set; }
        public long taken_at_timestamp { get; private set; }
        public bool viewer_has_liked { get; private set; }
        public bool viewer_has_saved { get; private set; }
        public bool viewer_has_saved_to_collection { get; private set; }

        public struct DisplayResource
        {
            public int config_height { get; private set; }
            public int config_width { get; private set; }
            public string src { get; private set; }
        }

        public class TaggedUser : UserTemp<UserBase>
        {
            public float x { get; private set; }
            public float y { get; private set; }
        }

        public EdgesTemp<TaggedUser> edge_media_to_tagged_user { get; private set; }
        public string id { get; private set; }
        public bool is_video { get; private set; }
        public string shortcode { get; private set; }
    }
}
