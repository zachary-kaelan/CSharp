using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WargamingAPI.Models;

namespace WargamingAPI.TankMath
{
    public static class DamagePenMath
    {
        public const int NORMALIZATION = 2;

        public static int GetDamage()
        {
            return 0;
        }

        public static bool Penetrates(int penetration, int armor, double angle, int caliber, ShellType type)
        {
            if (type == ShellType.HOLLOW_CHARGE)
            {
                if (angle >= 85)
                    return false;
            }
            else if (type == ShellType.ARMOR_PIERCING || type == ShellType.ARMOR_PIERCING_CR)
            {
                double overmatch = caliber / armor;
                if (angle >= 70 && overmatch < 3.0)
                    return false;
                angle -= overmatch > 2.0 ? (NORMALIZATION * 1.4 * overmatch) : NORMALIZATION;
            }

            return false;
        }

        public static int ExplosionDamage(double nominalDamage, double impactDistance, double splashRadius, double nominalArmorThickness, double spallCoefficient = 1)
        {
            return Convert.ToInt32((0.5 * nominalDamage * (1.0 - (impactDistance / splashRadius))) - (1.1 * nominalArmorThickness * spallCoefficient));
        }

        public static int ExplosionDamage(double damage, double armorThickness, double spallCoefficient = 1)
        {
            return Convert.ToInt32((0.5 * damage) - (1.1 * armorThickness * spallCoefficient));
        }
    }
}
