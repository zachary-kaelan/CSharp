using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CataclysmTesting.Models
{
    public enum AmmoEffects
    {
        NONE,
        NOGIB,
        RECOVER_2,
        RECOVER_6,
        RECOVER_8,
        RECOVER_10,
        RECOVER_20,
        RECOVER_25,
        RECOVER_30,
        RECOVER_35,
        RECOVER_40,
        RECOVER_45,
        EXPLOSIVE_SMALL,
        IGNITE,
        BEANBAG
    }

    public class AmmoTemp
    {
        public string type { get; set; }
        public string id { get; set; }
        public int price { get; set; }
        public string name { get; set; }
        public string volume { get; set; }
        public string weight { get; set; }
        public int bashing { get; set; }
        public string ammo_type { get; set; }
        public float prop_damage { get; set; }
        public int pierce { get; set; }
        public int dispersion { get; set; }
        public int loudness { get; set; }
        public string[] effects { get; set; }
        public int to_hit { get; set; }
        public int cutting { get; set; }
    }

    public class Ammo
    {
        public int RecoveryChance { get; set; }
    }
}
