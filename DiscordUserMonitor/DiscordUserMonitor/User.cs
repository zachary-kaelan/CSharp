using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordUserMonitor
{
    public class User
    {
        public string username { get; private set; }
        public string discriminator { get; private set; }
        public string id { get; private set; }
        public string avatar { get; private set; }
    }
}
