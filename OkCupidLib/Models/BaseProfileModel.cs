using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class BaseUserModel
    {
        public bool inactive { get; protected set; }
        public string userid { get; protected set; }
        public bool staff { get; protected set; }
        public string username { get; protected set; }
        public Thumbs[] thumbs { get; protected set; }
        public bool isAdmin { get; protected set; }
    }

    public class BaseProfileModel : BaseUserModel
    {
        public UserInfo userinfo { get; protected set; }
    }
}
