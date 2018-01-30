using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPLib
{
    public struct ZipCode
    {
        public string Zip { get; set; }
        public string Type { get; set; }
        public string PrimaryCity { get; set; }
        public string County { get; set; }
        public string[] AcceptableCities { get; set; }
        public string[] UnacceptableCities { get; set; }
    }
}
