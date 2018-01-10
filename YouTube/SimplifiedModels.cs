using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTube
{
    public struct VideoListModel
    {
        public string Title { get; set; }
        public string ID { get; set; }
        public string When { get; set; }
        public ChannelListModel Channel { get; set; }
        public string Description { get; set; }
        public TimeSpan Length { get; set; }
        public int Views { get; set; }
        public int PercentDurationWatched { get; set; }
    }

    public struct ChannelListModel
    {
        public string Name { get; set; }
        public Thumbnail Thumbnail { get; set; }
        public string URL { get; set; }
        public string ID { get; set; }
    }
}
