using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestPacUpdates
{
    public struct Location_CodeOnly
    {
        public int Location { get; set; }
    }

    public struct Location_CodeAndOrder
    {
        public int Location { get; set; }
        public int Order { get; set; }
    }

    public struct Employee_BareMin
    {
        public string Username { get; set; }
    }
}
