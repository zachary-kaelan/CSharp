using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.API_Models
{
    public class BaseFeedItem
    {
        public string id { get; private set; }
        public long pk { get; private set; }

        public bool is_reel_media { get; private set; }
        public byte media_type { get; private set; }
        public int original_height { get; private set; }
        public int original_width { get; private set; }
        public UserTags usertags { get; private set; }

        internal ImageVersions image_versions2 { get; set; }
        public FeedImage image => image_versions2.candidates[0];

        public bool is_video => video_versions != null;
        public float? video_duration { get; private set; }
        internal VideoVersions video_versions { get; set; }
        public FeedVideo video => !is_video ? null : video_versions.candidates[0];
        public int? view_count { get; private set; }
        public bool? has_audio { get; private set; }

        public long SaveAs(string folder)
        {
            string path = folder + Path.GetFileName(new Uri(image.url).LocalPath);
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(image.url, path);
            }
            return new FileInfo(path).Length;
        }
    }
}
