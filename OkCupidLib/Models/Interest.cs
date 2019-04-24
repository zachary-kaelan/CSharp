using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidLib.Models
{
    public class Interest
    {
        public byte added { get; protected set; }
        public string id { get; protected set; }
        public string name { get; protected set; }
        public bool official { get; protected set; }
        public int users { get; protected set; }
    }

    public class InterestListModel : Interest
    {
        public bool confirmed { get; protected set; }
    }
}
