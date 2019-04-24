using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace InstagramLib.API_Models
{
    public class UserTag
    {
        public int? start_time_in_video_in_sec { get; private set; }
        public int? duration_in_video_in_seconds { get; private set; }
        public float[] position { get; private set; }
    }

    public class UserTags
    {
        [JilDirective("in")]
        public UserTag[] _in { get; private set; }
    }
}
