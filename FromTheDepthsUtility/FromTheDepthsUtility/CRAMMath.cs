using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public static class CRAMMath
    {
        public static double ShellDiameter(double numGaugeIncreasers)
        {
            return 200.0 + (2000 * (1 - Math.Pow(0.95, numGaugeIncreasers)));
        }

        public static double ShellVolume(double diameter, double fuses)
        {
            return Math.Pow(diameter / 400.0, 1.8) - (0.25 * fuses);
        }
        
        // Base muzzle velocity scales linearly from 60 m/s at 200mm to 100 m/s at 2000mm
        // Base can be up to doubled by using a long barrel.
        // Base kinetic damage is 2 * V * (v)elocity
        // Base AP is (3 + v) / 150

        public static double MuzzleVelocity(double diameter)
        {
            return ((Math.Max(200.0, diameter) - 200.0) * (1.0 / 45.0)) + 60.0;
        }

        // Packing rate is 0.1 per second per effective material box
        // Each box counts as 0.5 effective, plus 1 for every attached autoloader
        // Packing density = Ptotal / V

        public static double PackingDensity(double pelletsTotal, double volume)
        {
            return pelletsTotal / volume;
        }

        // 1 density unit holds V pellets depending on gauge
        // 100 density max
        // Each density unit is only 90% as effective as the last
        public static double EffectivePelletCount(double pelletsRaw, double density, double pelletsTotal, double volume)
        {
            return 10.0 * volume * (pelletsRaw / pelletsTotal) * (1.0 - Math.Pow(0.9, density));
        }

        public static double EffectivePelletCount(double volume, double density)
        {
            return 10.0 * volume * (1.0 - Math.Pow(0.9, density));
        }

        // For multiple pellet types, density is computed from total number of pellets
        // Effective number of each type is then calculated usingthe number of pellets f that type.

        // Hardener: +100 kinetic damage and +1.5 AP per pellet
        // High explosive: +200 explosive damage per pellet
        // EMP: +10V EMP damage per pellet
        // Fragmentation: V fragments per pellet.
        //  Each fragment deals 100 kinetic damage at AP 6 regardless of main shell stats
        //  Above the maximum 60 pellets, the uncapped total damage is redistributed among 60 fragments

        public static ShellDamage DamageTypes(double velocity, double volume, double hardner = 0, double highExplosive = 0, double EMP = 0, double fragments = 0)
        {
            ShellDamage totalDamage = new ShellDamage(
                velocity, 2.0 * volume * velocity,
                3 + (velocity / 150.0)
            );

            double totalPellets = hardner + highExplosive + EMP + fragments;
            double density = PackingDensity(totalPellets, volume);
            double effectivePellets = EffectivePelletCount(volume, density);

            double effectiveHardner = (hardner / totalPellets) * effectivePellets;
            double effectiveHighExplosive = (highExplosive / totalPellets) * effectivePellets;
            double effectiveEMP = (EMP / totalPellets) * effectivePellets;
            double effectiveFrag = (fragments / totalPellets) * effectivePellets;

            totalDamage.Kinetic += 100.0 * effectiveHardner;
            totalDamage.AP += 1.5 * effectiveHardner;
            totalDamage.Explosive += 200.0 * effectiveHighExplosive;
            totalDamage.EMP += effectiveEMP * (10.0 * volume);
            if (effectiveFrag > 0)
                totalDamage.Fragmentation = new Fragments(effectiveFrag);

            return totalDamage;
        }

        public static double ShellHealth(double diameter)
        {
            return 300.0 * Math.PI * Math.Pow(diameter, 2.0);
        }

        public static double MinimumReloadTime(double diameter)
        {
            return Math.Pow(diameter / 400.0, 1.5);
        }

        public static double NetReloadTime(double minReload, double ammoAutoLoaderConnections)
        {
            return minReload * (1.0 * Math.Pow(10.0 / (1.0 + ammoAutoLoaderConnections), 0.5));
        }

        public static double TraverseSpeed(double motorBarrels, double volume)
        {
            return 145.75 * ((motorBarrels + 1.0) / volume + 0.1);
        }
    }
}
