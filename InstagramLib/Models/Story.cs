using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.Models
{
    public class StoriesTray
    {
        public bool has_besties_media { get; private set; }
        public int ranked_position { get; private set; }
        public UserOwner user { get; private set; }
    }

    public class Story
    {
        public StoryTaggedUser[] reel_mentions { get; private set; }
        public int media_type { get; private set; }
        public double video_duration { get; private set; }
        public VideoVersion[] video_versions { get; private set; }
    }

    public struct StoryTaggedUser
    {
        public double height { get; private set; }
        public UserOwner user { get; private set; }
        public double weidth { get; private set; }
        public double x { get; private set; }
        public double y { get; private set; }
        public int z { get; private set; }
       
    }

    public class UserReelTray
    {
        public string status { get; private set; }
        public Story[] tray { get; private set; }
    }

    public struct VideoVersion
    {
        public int height { get; private set; }
        public long id { get; private set; }
        public int type { get; private set; }
        public string url { get; private set; }
        public int width { get; private set; }
    }
}
