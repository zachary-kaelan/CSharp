using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingAPI.Models
{
    

    public class TankObject : IEquatable<TankObject>
    {
        public int tank_id { get; protected set; }

        public bool Equals(TankObject other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return tank_id == other.tank_id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (TankObject)obj;
            return tank_id == other.tank_id;
        }

        public override int GetHashCode()
        {
            return tank_id.GetHashCode();
        }
    }

    public class BaseVehicle : TankObject
    {
        public int[] suspensions { get; protected set; }
        public string description { get; protected set; }
        public int[] engines { get; protected set; }
        public Dictionary<string, int> prices_xp { get; protected set; }
        public Dictionary<string, int> next_tanks { get; protected set; }
        public Dictionary<string, ModuleTreeNode> modules_tree { get; protected set; }

        public struct ModuleTreeNode
        {
            public string name { get; private set; }
            public int[] next_modules { get; private set; }
            public int[] next_tanks { get; private set; }
            public bool is_default { get; private set; }
            public int price_xp { get; private set; }
            public int price_credit { get; private set; }
            public int module_id { get; private set; }
            public ModuleType type { get; private set; }
        }

        public string nation { get; protected set; }
        public bool is_premium { get; protected set; }
        public Images images { get; protected set; }

        public Cost? cost { get; protected set; }

        public struct Cost
        {
            public int price_credit { get; private set; }
            public int price_gold { get; private set; }
        }

        public VehicleProfile default_profile { get; protected set; }
        public int tier { get; protected set; }
        public string type { get; protected set; }
        public int[] guns { get; protected set; }
        public int[] turrets { get; protected set; }
        public string name { get; protected set; }

        private const string TANK_INFO_STRING = "{0} - {1} - {2} - {3}";
        public override string ToString()
        {
            return String.Format(TANK_INFO_STRING, name, nation, tier, is_premium);
        }
    }

    public class VehicleProfile : BaseVehicleProfile
    {
        public int firepower { get; protected set; }
        public Shell[] shells { get; protected set; }
        public int shot_efficiency { get; protected set; }
        public int gun_id { get; protected set; }
        public int protection { get; protected set; }
        public int engine_id { get; protected set; }
        public int suspension_id { get; protected set; }
        public int? turret_id { get; protected set; }
        public int maneuverability { get; protected set; }
        public int hull_hp { get; protected set; }
        public int max_weight { get; protected set; }
        public int battle_level_range_min { get; protected set; }
        public int battle_level_range_max { get; protected set; }

        public GunDetailed gun { get; protected set; }
        public BaseEngine engine { get; protected set; }
        public BaseSuspension suspension { get; protected set; }
        public BaseTurretWithTraverse turret { get; protected set; }
    }

    public abstract class BaseVehicleProfile
    {
        public int weight { get; protected set; }
        public string profile_id { get; protected set; }
        public int signal_range { get; protected set; }

        public VehicleArmor armor { get; protected set; }

        public int speed_forward { get; protected set; }
        public int speed_backward { get; protected set; }

        public int max_ammo { get; protected set; }
        public bool is_default { get; protected set; }

        public int hull_weight { get; protected set; }
        public int hp { get; protected set; }
    }

    public class ListVehicleProfile : BaseVehicleProfile
    {
        public ListEngine engine { get; protected set; }
        public ListSuspension suspension { get; protected set; }
        public ListGun gun { get; protected set; }
        public ListTurret turret { get; protected set; }
        public ListShell[] shells { get; protected set; }

        public struct ListEngine
        {
            public int power { get; private set; }
        }

        public struct ListSuspension
        {
            public int traverse_speed { get; private set; }
            public int load_limit { get; private set; }
        }

        public struct ListTurret
        {
            public int traverse_left_arc { get; private set; }
            public int traverse_right_arc { get; private set; }
            public int view_range { get; private set; }
            public int traverse_speed { get; private set; }
        }

        public struct ListGun
        {
            public int move_down_arc { get; private set; }
            public int caliber { get; private set; }
            public int move_up_arc { get; private set; }
            public double fire_rate { get; private set; }
            public double clip_reload_time { get; private set; }
            public int clip_capacity { get; private set; }
            public double traverse_speed { get; private set; }
            public double reload_time { get; private set; }
            public double dispersion { get; private set; }
            public double aim_time { get; private set; }
        }

        public struct ListShell
        {
            public int damage { get; private set; }
            public int penetration { get; private set; }
        }
    }
}
