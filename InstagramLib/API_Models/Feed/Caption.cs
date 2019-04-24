using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace InstagramLib.API_Models
{
    public class Caption
    {
        public byte bit_flags { get; private set; }
        public string content_type { get; private set; }
        public long created_at { get; private set; }
        public long media_id { get; private set; }
        public long pk { get; private set; }
        public string status { get; private set; }
        public string text { get; private set; }
        public byte type { get; private set; }
    }
}
