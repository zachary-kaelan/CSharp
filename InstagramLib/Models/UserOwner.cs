using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.Models
{
    public class UserOwner
    {
        public string full_name { get; private set; }
        public string id { get; private set; }
        public bool is_private { get; private set; }
        public string profile_pic_url { get; private set; }
        public string username { get; private set; }
    }
}
