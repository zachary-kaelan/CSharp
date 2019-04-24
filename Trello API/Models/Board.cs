using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trello_API.Models
{
    public struct Board
    {
        public string id { get; private set; }
        public string name { get; private set; }
        public object prefs { get; private set; }
        public object labelNames { get; private set; }
        public object limits { get; private set; }
        public object[] memberships { get; private set; }
    }
}
