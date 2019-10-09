using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace CataclysmTesting.Models
{
    [Flags]
    public enum GunFlags
    {
        NONE = 0,
        NEVER_JAMS = 1,
        RELOAD_AND_SHOOT,
        PRIMITIVE_RANGED_WEAPON = 4,
        BELT_CLIP = 8,
        STR_RELOAD = 16,
        RELOAD_EJECT = 32,
        RELOAD_ONE = 64,
        MODE_BURST = 128,
        FIRE_100 = 256,
        FIRESTARTER = 512,
        NO_UNLOAD = 1024,
        PUMP_ACTION = 2048,
        FIRE_TWOHAND = 4096,
        BELTED = 8192,
        WATER_FRIENDLY = 16384
    }

    [Flags]
    public enum GunAmmoEffects
    {
        NONE,
        NO_BOOM,
        FLARE
    }

    public enum RangedSkill
    {
        None,
        Archery,
        SubmachineGuns
    }

    public enum GunModes
    {
        DEFAULT,
        SINGLE,
        NPC_AVOID
    }

    [Flags]
    public enum Faults
    {
        NONE,
        FAULT_GUN_BLACKPOWDER,
        FAULT_GUN_CLOGGED,
        FAULT_GUN_CHAMBER_SPENT = 4
    }

    public class GunTemp
    {
        [JilDirective("abstract")]
        public string template { get; set; }
        public string id { get; set; }
        [JilDirective("copy-from")]
        public string copy_from { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int barrel_length { get; set; }
        public int reload_noise_volume { get; set; }
        public string weight { get; set; }
        public string volume { get; set; }
        public int range { get; set; }
        public int price { get; set; }
        public int to_hit { get; set; }
        public int bashing { get; set; }
        public string ammo { get; set; }
        public string skill { get; set; }
        public int ranged_damage { get; set; }
        public int dispersion { get; set; }
        public int durability { get; set; }
        public int min_cycle_recoil { get; set; }
        public int burst { get; set; }
        public string[] faults { get; set; }
        public int magazine_well { get; set; }
        public object[] magazines { get; set; }
        public object[][] modes { get; set; }
        public string[] built_in_mods { get; set; }
        public int clip_size { get; set; }
        public int reload { get; set; }
        public object flags { get; set; }
        public string[] ammo_effects { get; set; }
        public int loudness { get; set; }
        public int min_strength { get; set; }
        [JilDirective("override")]
        public bool override_template { get; set; }
        public int pierce { get; set; }
        public Dictionary<string, float> Proportional { get; set; }
    }
}
