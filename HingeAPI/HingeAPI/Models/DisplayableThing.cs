using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HingeAPI.Models
{
    public sealed class DisplayableThing
    {
        public string displayName { get; private set; }
        public string id { get; private set; }
        public bool selected { get; private set; }
    }
}
