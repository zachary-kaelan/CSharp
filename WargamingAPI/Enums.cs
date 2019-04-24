using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WargamingAPI
{
    public enum ModuleType
    {
        vehicleChassis,
        vehicleTurret,
        vehicleGun,
        vehicleEngine,
        vehicleHull
    }

    public enum ArmorType
    {
        Front,
        Side,
        Rear
    }

    public enum VehicleType
    {
        Light,
        Medium,
        Heavy,
        TankDestroyer
    }

    public enum ShellType
    {
        ARMOR_PIERCING,
        HOLLOW_CHARGE,
        HIGH_EXPLOSIVE,
        ARMOR_PIERCING_CR
    }

    public enum LODDistance
    {
        MEDIUM,
        FAR
    }
}
