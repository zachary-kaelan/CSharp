using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WargamingAPI.Models;
using WargamingAPI.TankMath;

namespace WargamingAPI
{
    

    public sealed class VehicleStats
    {
        public double Weight { get; private set; }
        public int HorsePower { get; private set; }
        public double HorsePowerToTonRatio { get; private set; }
        public double TraverseSpeed { get; private set; }
        public int SpeedForward { get; private set; }
        public int SpeedBackward { get; private set; }

        public VehicleInfo Info { get; private set; }
        public int SignalRange { get; private set; }
        public Dictionary<ShellType, ShellInfo> Shells { get; private set; }
        public Shell DefaultShell { get; private set; }
        public ArmorStats TurretArmor { get; private set; }
        public ArmorStats HullArmor { get; private set; }
        public double FireChance { get; private set; }
        public int LoadLimit { get; private set; }
        public int AmmoCapacity { get; private set; }
        public int Hitpoints { get; private set; }
        public Dictionary<ModuleType, BaseModule> ModulesInfo { get; private set; }
        public CamoInfo Camo { get; private set; }
        
        public double Dispersion { get; private set; }
        public double AimTime { get; private set; }
        public int ClipCapacity { get; private set; }
        public int GunElevation { get; private set; }
        public int Caliber { get; private set; }
        public int GunDepression { get; private set; }
        public double ReloadTime { get; private set; }
        public double GunTraverseSpeed { get; private set; }
        public double ClipReloadTime { get; private set; }
        public double FireRate { get; private set; }
        public double DamagePerMinute { get; private set; }
        public double AdvDamagePerMinute { get; private set; }

        public int ViewRange { get; private set; }
        public int GunArcLeft { get; private set; }
        public int GunArcRight { get; private set; }
        public int TurretTraverseSpeed { get; private set; }

        public VehicleStats(GunDetailed gun, BaseTurretWithTraverse turret, BaseEngine engine, BaseSuspension tracks, BaseVehicle vehicle, CamoInfo camo)
        {
            Camo = camo;
            var profile = vehicle.default_profile;
            int weight = profile.hull_weight + gun.weight + turret.weight + engine.weight + tracks.weight;
            Weight = weight / 1000.0;
            HorsePower = engine.power;
            HorsePowerToTonRatio = HorsePower / Weight;
            TraverseSpeed = tracks.traverse_speed * ((double)engine.power / profile.engine.power) * (profile.weight / (double)weight);
            SpeedForward = profile.speed_forward;
            SpeedBackward = profile.speed_backward;

            //var gunShells = API.GUNS[profile.gun_id]
            //Shells = gunShells.ToDictionary(s => s.type, s => new ShellInfo(s));
            //DefaultShell = gunShells[0];
            Info = new VehicleInfo(vehicle);
            SignalRange = profile.signal_range;
            TurretArmor = new ArmorStats();
            HullArmor = new ArmorStats(profile.armor.hull);
            FireChance = engine.fire_chance;
            LoadLimit = tracks.load_limit;
            AmmoCapacity = profile.max_ammo;
            Hitpoints = profile.hull_hp + turret.hp;

            Dispersion = gun.dispersion;
            AimTime = gun.aim_time;
            ClipCapacity = gun.clip_capacity;
            GunElevation = gun.move_up_arc;
            GunDepression = gun.move_down_arc;
            ReloadTime = gun.reload_time;
            GunTraverseSpeed = gun.traverse_speed;
            ClipReloadTime = gun.clip_reload_time;
            FireRate = gun.fire_rate;
            DamagePerMinute = FireRate * DefaultShell.damage;
            AdvDamagePerMinute = (FireRate + 1) * DefaultShell.damage;

            ViewRange = turret.view_range;
            GunArcLeft = turret.traverse_left_arc;
            GunArcRight = turret.traverse_right_arc;
            TurretTraverseSpeed = turret.traverse_speed;

            ModulesInfo = new Dictionary<ModuleType, BaseModule>(4);
            ModulesInfo.Add(ModuleType.vehicleEngine, engine);
            ModulesInfo.Add(ModuleType.vehicleGun, gun);
            ModulesInfo.Add(ModuleType.vehicleTurret, turret);
            ModulesInfo.Add(ModuleType.vehicleChassis, tracks);
            ModulesInfo.Add(ModuleType.vehicleHull, new BaseModule(vehicle.name, profile.hull_weight, vehicle.tier));
        }

        public static VehicleStats operator +(VehicleStats stats, VehicleStats other)
        {
            return null;
        }

        private const string PROFILE_ID_FORMAT = "{0}-{1}-{2}-{3}";
        public static string GetProfileID(int gunid, int engineid, int turretid, int suspensionid) => String.Join("-", new int[] { gunid, engineid, turretid, suspensionid }.OrderBy(m => m));

        /*public KeyValuePair<int, int> MaxRammingDamage(VehicleStats other, ArmorType hitArea)
        {
            int combinedWeight = Weight + other.Weight;
            double damagePotential = (0.5 * combinedWeight * (TopSpeed.Forward * TopSpeed.Forward));
            return new KeyValuePair<int, int>(
                DamagePenMath.ExplosionDamage(
                    damagePotential * (1.0 - (Weight / combinedWeight)),
                    HullArmor.Front
                ), DamagePenMath.ExplosionDamage(
                    damagePotential * (1.0 - (other.Weight / combinedWeight)),
                    other.HullArmor[hitArea]
                )
            );
        }*/
        //public VehicleStats ChangeModules(Gun gun, Engine engine, )
    }

    public struct ShellInfo
    {
        public int Penetration { get; private set; }
        public int Alpha { get; private set; }
        public int ShellDamage { get; private set; }
        public double DamagePerMinute { get; private set; }
        public double AdvDamagePerMinute { get; private set; }

        internal int Count { get; private set; }
        internal double Frequency { get; private set; }

        public ShellInfo(Shell shell, ListVehicleProfile.ListGun gun)
        {
            Penetration = shell.penetration;
            ShellDamage = shell.damage;
            Alpha = ShellDamage * gun.clip_capacity;
            Count = 0;
            Frequency = 1;
            DamagePerMinute = ShellDamage * gun.fire_rate;
            AdvDamagePerMinute = (DamagePerMinute + Alpha) * (60.0 / (60 + (gun.clip_reload_time * (gun.clip_capacity - 1))));
        }
    }

    public struct CamoInfo
    {
        public double Stationary { get; private set; }
        public double Moving { get; private set; }
        public double Shooting { get; private set; }

        public CamoInfo(double stopped, double move, double shoot)
        {
            Stationary = stopped;
            Moving = move;
            Shooting = shoot;
        }
    }

    public class Info
    {
        public string Name { get; protected set; }
        public int Tier { get; protected  set; }
        public int ID { get; protected set; }

        public override string ToString()
        {
            return Name + " - " + Tier.ToString();
        }
    }

    public sealed class VehicleInfo : Info
    {
        public string Nation { get; private set; }
        public Images Images { get; private set; }
        public VehicleType Type { get; private set; }
        public string Description { get; private set; }
        public int BattleLevelMin { get; private set; }
        public int BattleLevelMax { get; private set; }
        public bool IsPremium { get; private set; }

        public VehicleInfo(BaseVehicle vehicle)
        {
            Name = vehicle.name;
            Nation = vehicle.nation;
            Images = vehicle.images;
            Type = vehicle.type == "AT-SPG" ? VehicleType.TankDestroyer : (VehicleType)Enum.Parse(typeof(VehicleType), vehicle.type.Replace("Tank", ""), true);
            Tier = vehicle.tier;
            Description = vehicle.description;
            BattleLevelMin = vehicle.default_profile.battle_level_range_min;
            BattleLevelMax = vehicle.default_profile.battle_level_range_max;
            ID = vehicle.tank_id;
            IsPremium = vehicle.is_premium;
        }
    }

    public class StaticVehicleStats : Info
    {
        public int HorsePower { get; private set; }
        public int SpeedForward { get; private set; }
        public int SpeedBackward { get; private set; }
        public int EngineID { get; private set; }
        public int SuspensionID { get; private set; }

        public VehicleInfo Info { get; private set; }
        public int SignalRange { get; private set; }
        public ArmorStats HullArmor { get; private set; }
        public double FireChance { get; private set; }
        public int LoadLimit { get; private set; }

        public int GunArcLeft { get; private set; }
        public int GunArcRight { get; private set; }

        public double BaseTraverse { get; private set; }
        internal int BaseHP { get; private set; }
        internal int BaseWeight { get; private set; }
        internal int StockWeight { get; private set; }
        internal int[] Guns { get; private set; }
        internal int[] Turrets { get; private set; }
        public DynamicVehicleStats[] Profiles { get; private set; }

        public StaticVehicleStats(BaseVehicle vehicleStock, ListVehicleProfile[] topSuspensionEngineProfiles)
        {
            var profile = topSuspensionEngineProfiles.First();
            int[] profileIDs = profile.profile_id.Split('-').Select(i => Convert.ToInt32(i)).ToArray();
            EngineID = profileIDs.First(i => vehicleStock.engines.Contains(i));
            SuspensionID = profileIDs.First(i => i != EngineID && vehicleStock.suspensions.Contains(i));
            var engine = API.ENGINES[EngineID];
            var tracks = API.SUSPENSIONS[SuspensionID];

            BaseWeight = profile.hull_weight + engine.weight + tracks.weight;
            StockWeight = vehicleStock.default_profile.weight;
            BaseHP = vehicleStock.default_profile.hull_hp;
            BaseTraverse = tracks.traverse_speed;

            HorsePower = engine.power;
            BaseTraverse = tracks.traverse_speed * ((double)engine.power / vehicleStock.default_profile.engine.power);
            SpeedForward = profile.speed_forward;
            SpeedBackward = profile.speed_backward;
            
            Info = new VehicleInfo(vehicleStock);
            SignalRange = profile.signal_range;
            HullArmor = new ArmorStats(profile.armor.hull);
            FireChance = engine.fire_chance;
            LoadLimit = tracks.load_limit;

            GunArcLeft = vehicleStock.default_profile.turret.traverse_left_arc;
            GunArcRight = vehicleStock.default_profile.turret.traverse_right_arc;

            Name = vehicleStock.name;
            Tier = vehicleStock.tier;
            ID = vehicleStock.tank_id;
            Guns = vehicleStock.guns;
            Turrets = vehicleStock.turrets;
            Profiles = topSuspensionEngineProfiles.Select(p => new DynamicVehicleStats(p, this)).ToArray();
        }
    }

    public struct DynamicVehicleStats
    {
        public GunInfo Gun { get; private set; }
        public TurretInfo Turret { get; private set; }

        public int AmmoCapacity { get; private set; }
        public double TraverseSpeed { get; private set; }
        public double Weight { get; private set; }
        public double HorsePowerToTonRatio { get; private set; }
        public int HP { get; private set; }

        public DynamicVehicleStats(ListVehicleProfile profile, StaticVehicleStats staticStats)
        {
            List<int> ids = profile.profile_id.Split('-').Select(i => Convert.ToInt32(i)).ToList();
            ids.Remove(staticStats.EngineID);
            ids.Remove(staticStats.SuspensionID);
            int gunID = ids.First(i => staticStats.Guns.Contains(i));
            ids.Remove(gunID);
            int turretID = ids.Single();
            ids = null;

            Gun = new GunInfo(profile.gun, gunID);
            Turret = new TurretInfo(profile.turret, turretID);
            AmmoCapacity = profile.max_ammo;
            HP = Turret.HP + staticStats.BaseHP;
            Weight = staticStats.BaseWeight + Gun.Weight + Turret.Weight;
            TraverseSpeed = staticStats.BaseTraverse * (staticStats.StockWeight / Weight);
            Weight /= 1000;
            HorsePowerToTonRatio = staticStats.HorsePower / Weight;
        }

        public sealed class GunInfo : Info
        {
            public double Dispersion { get; private set; }
            public double AimTime { get; private set; }
            public int ClipCapacity { get; private set; }
            public int GunElevation { get; private set; }
            public int Caliber { get; private set; }
            public int GunDepression { get; private set; }
            public double ReloadTime { get; private set; }
            public double GunTraverseSpeed { get; private set; }
            public double ClipReloadTime { get; private set; }
            public double FireRate { get; private set; }
            public Dictionary<ShellType, ShellInfo> Shells { get; private set; }
            public Shell DefaultShell { get; private set; }
            public int Weight { get; private set; }

            public GunInfo(ListVehicleProfile.ListGun gun, int id)
            {
                Dispersion = gun.dispersion;
                AimTime = gun.aim_time;
                ClipCapacity = gun.clip_capacity;
                GunElevation = gun.move_up_arc;
                GunDepression = gun.move_down_arc;
                ReloadTime = gun.reload_time;
                GunTraverseSpeed = gun.traverse_speed;
                ClipReloadTime = gun.clip_reload_time;
                FireRate = gun.fire_rate;
                Caliber = gun.caliber;

                var module = API.GUNS[id];
                DefaultShell = module.shells.First();
                Shells = module.shells.ToDictionary(s => s.type, s => new ShellInfo(s, gun));
                Name = module.name;
                Tier = module.tier;
                ID = id;
                Weight = module.weight;
            }
        }

        public sealed class TurretInfo : Info
        {
            public int HP { get; private set; }
            public int TraverseSpeed { get; private set; }
            public int ViewRange { get; private set; }
            public ArmorStats Armor { get; private set; }
            public int Weight { get; private set; }

            public TurretInfo(ListVehicleProfile.ListTurret turret, int id)
            {
                TraverseSpeed = turret.traverse_speed;
                ViewRange = turret.view_range;

                var module = API.TURRETS[id];
                Armor = new ArmorStats(module.armor);
                HP = module.hp;
                Name = module.name;
                Tier = module.tier;
                ID = id;
                Weight = module.weight;
            }
        }
    }

    public struct ArmorStats
    {
        public int Front { get; private set; }
        public int Sides { get; private set; }
        public int Rear { get; private set; }
        public int this[ArmorType direction] { get => direction == ArmorType.Front ? Front : (direction == ArmorType.Rear ? Rear : Sides); }

        public ArmorStats(Armor armor)
        {
            Front = armor.front;
            Sides = armor.sides;
            Rear = armor.rear;
        }
    }
}
