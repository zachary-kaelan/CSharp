using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Forest_Weapons_Comparison
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public struct Weapon
    {
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float DPS { get; set; }
        public byte DamageUpgrades { get; set; }
        public byte SpeedUpgrades { get; set; }
    }
}
