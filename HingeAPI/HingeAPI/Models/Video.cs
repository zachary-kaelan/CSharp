using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HingeAPI.Models
{
    public sealed class Video
    {
        public ushort height { get; private set; }
        public string quality { get; private set; }
        public string url { get; private set; }
        public ushort width { get; private set; }
    }
}
