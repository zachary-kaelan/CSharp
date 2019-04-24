using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trello_API.Models
{
    public struct Membership
    {
        public bool deactivated { get; private set; }
        public string id { get; private set; }
        public string idMember { get; private set; }
        public string memberType { get; private set; }
        public bool unconfirmed { get; private set; }
    }

    public struct Member
    {
        public string fullName { get; private set; }
        public string id { get; private set; }
        public string username { get; private set; }
    }
}
