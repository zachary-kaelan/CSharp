using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingAPI.Models
{
    public class BaseModule : IEquatable<BaseModule>
    {
        public string name { get; protected set; }
        public int weight { get; protected set; }
        public int tier { get; protected set; }
        
        public bool Equals(BaseModule other)
        {
            if (Object.ReferenceEquals(other, null)) return false;
            if (Object.ReferenceEquals(this, other)) return true;

            return name == other.name;
        }

        public override int GetHashCode()
        {
            return (name == null ? 0 : name.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            BaseModule other = (BaseModule)obj;
            return other.name == name;
        }

        public BaseModule()
        {
            name = null;
            weight = 0;
            tier = 0;
        }

        public BaseModule(string name, int weight, int tier)
        {
            this.name = name;
            this.weight = weight;
            this.tier = tier;
        }
    }

    public interface IListedModule
    {
        string nation { get; }
        int[] tanks { get; }
        int module_id { get; }
    }

    public class BaseSuspension : BaseModule
    {
        public int load_limit { get; protected set; }
        public int traverse_speed { get; protected set; }
    }

    public sealed class Suspension : BaseSuspension, IListedModule
    {
        public string nation { get; private set; }
        public int[] tanks { get; private set; }
        public int module_id { get; private set; }
    }

    public class BaseEngine : BaseModule
    {
        public int power { get; protected set; }
        public double fire_chance { get; protected set; }
    }

    public sealed class Engine : BaseEngine, IListedModule
    {
        public string nation { get; private set; }
        public int[] tanks { get; private set; }
        public int module_id { get; private set; }
    }

    public class BaseGunWithShells : BaseGun
    {
        public Shell[] shells { get; protected set; }
    }

    public class BaseGun : BaseModule
    {
        public double dispersion { get; protected set; }
        public double aim_time { get; protected set; }
    }
                
    public sealed class GunDetailed : BaseGun
    {
        public int move_down_arc { get; private set; }
        public int caliber { get; private set; }
        public int move_up_arc { get; private set; }
        public double fire_rate { get; private set; }
        public double clip_reload_time { get; private set; }
        public int clip_capacity { get; private set; }
        public double traverse_speed { get; private set; }
        public double reload_time { get; private set; }
    }

    public sealed class Gun : BaseGunWithShells, IListedModule
    {
        public string nation { get; private set; }
        public int[] tanks { get; private set; }
        public int module_id { get; private set; }
    }

    public class BaseTurret : BaseModule
    {
        public int hp { get; private set; }
        public int view_range { get; protected set; }
        public int traverse_right_arc { get; protected set; }
        public int traverse_left_arc { get; protected set; }
    }

    public class BaseTurretWithArmor : BaseTurret
    {
        public Armor armor { get; protected set; }
    }

    public sealed class BaseTurretWithTraverse : BaseTurret
    {
        public int traverse_speed { get; private set; }
    }

    public sealed class Turret : BaseTurretWithArmor, IListedModule
    {
        public string nation { get; private set; }
        public int[] tanks { get; private set; }
        public int module_id { get; private set; }
    }

    public struct ModulesRequest
    {
        public Suspension[] suspensions { get; private set; }
        public Gun[] guns { get; private set; }
        public Turret[] turrets { get; private set; }
        public Engine[] engines { get; private set; }
    }

    public struct ModulesCombination
    {
        public int this[ModuleType type]
        {
            get
            {
                switch(type)
                {
                    case ModuleType.vehicleChassis:
                        return SuspensionID;

                    case ModuleType.vehicleEngine:
                        return EngineID;

                    case ModuleType.vehicleGun:
                        return GunID;

                    case ModuleType.vehicleTurret:
                        return TurretID;

                    default:
                        return -1;
                }
            }
        }
        public int GunID { get; private set; }
        public int EngineID { get; private set; }
        public int TurretID { get; private set; }
        public int SuspensionID { get; private set; }

        public ModulesCombination(int gun, int engine, int turret, int suspension)
        {
            GunID = gun;
            EngineID = engine;
            TurretID = turret;
            SuspensionID = suspension;
        }

        public int[] ToVector()
        {
            return new int[]
            {
                GunID,
                EngineID,
                TurretID,
                SuspensionID
            };
        }
    }
}
