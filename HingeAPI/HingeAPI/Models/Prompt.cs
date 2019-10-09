using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HingeAPI.Models
{
    public sealed class Prompt
    {
        public string id { get; private set; }
        public bool isActive { get; private set; }
        public byte maxLength { get; private set; }
        public string placeholder { get; private set; }
        public string prompt { get; private set; }
    }
}
