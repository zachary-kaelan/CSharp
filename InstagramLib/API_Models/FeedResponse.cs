using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.API_Models
{
    internal class FeedResponse
    {
        public bool auto_load_more_enabled { get; private set; }
        public bool more_available { get; private set; }
        public int num_results { get; private set; }
        public FeedItem[] items { get; private set; }
    }
}
