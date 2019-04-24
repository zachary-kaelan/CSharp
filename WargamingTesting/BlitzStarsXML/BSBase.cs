using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingTesting.BlitzStarsXML
{
    public class BSBase
    {
        public string name { get; private set; }
        public Dictionary<string, string> names { get; private set; }
        public int id { get; private set; }
        public int tier { get; private set; }
        public string nation { get; private set; }
    }
}
