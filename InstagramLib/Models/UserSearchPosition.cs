using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.Models
{
    public class UserSearchPosition
    {
        public int position { get; set; }
        public API_Models.BaseUser user { get; set; }
    }
}
