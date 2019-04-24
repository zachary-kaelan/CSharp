using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace WargamingAPI.Models
{
   

    public struct Armor
    {
        public int front { get; private set; }
        public int sides { get; private set; }
        public int rear { get; private set; }
    }

    public struct VehicleArmor
    {
        public Armor turret { get; private set; }
        public Armor hull { get; private set; }
    }

    public struct Shell
    {
        public ShellType type { get; private set; }
        public int penetration { get; private set; }
        public int damage { get; private set; }
    }

    public struct Images
    {
        public string preview { get; private set; }
        public string normal { get; private set; }
        public string original { get; private set; }
    }

    public struct Request
    {
        public string status { get; private set; }
        public Dictionary<string, int> meta { get; private set; }
        public Dictionary<string, object> data { get; private set; }

        public static Request Load(string text)
        {
            return JSON.Deserialize<Request>(text);
        }
    }

    public struct Request<T>
    {
        public string status { get; private set; }
        public Dictionary<string, int> meta { get; private set; }
        public T data { get; private set; }

        public static Request Load(string text)
        {
            return JSON.Deserialize<Request>(text);
        }
    }
}
