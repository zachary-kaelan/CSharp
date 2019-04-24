using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.API_Models
{
    public class BaseUser
    {
        public string full_name { get; private set; }
        public bool is_private { get; private set; }
        public bool is_verified { get; private set; }
        public int pk { get; private set; }
        public string profie_pic_id { get; private set; }
        public string profile_pic_url { get; private set; }
        public string username { get; private set; }
    }
}
