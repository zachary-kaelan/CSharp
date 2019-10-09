using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.API_Models
{
    public class BaseUser
    {
        public string full_name { get; protected set; }
        public bool is_private { get; protected set; }
        public bool is_verified { get; protected set; }
        public string pk { get; protected set; }
        public string profie_pic_id { get; protected set; }
        public string profile_pic_url { get; protected set; }
        public string username { get; protected set; }
    }
}
