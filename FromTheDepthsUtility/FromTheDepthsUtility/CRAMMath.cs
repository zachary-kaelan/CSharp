using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public static class CRAMMath
    {
        public static float ShellDiameter(float numGaugeIncreasers)
        {
            return 200f + (2000 * (1 - (float)Math.Pow(0.95, numGaugeIncreasers)));
        }

        public static float ShellVolume(float diameter, float fuses)
        {
            return (float)Math.Pow(diameter / 400f, 1.8) - (0.25f * fuses);
        }
        
        // Base muzzle velocity scales linearly from 60 m/s at 200mm to 100 m/s at 2000mm
        // Base can be up to doubled by using a long barrel.
        // Base kinetic damage is 2 * V * (v)elocity
        // Base AP is (3 + v) / 150

        public static float MuzzleVelocity(float diameter)
        {
            return ((Math.Max(200f, diameter) - 200f) * (1f / 45f)) + 60f;
        }

        // Packing rate is 0.1 per second per effective material box
        // Each box counts as 0.5 effective, plus 1 for every attached autoloader
        // Packing density = Ptotal / V

        public static float PackingDensity(float pelletsTotal, float volume)
        {
            return pelletsTotal / volume;
        }

        // 1 density unit holds V pellets depending on gauge
        // 100 density max
        // Each density unit is only 90% as effective as the last
        public static float EffectivePelletCount(float pelletsRaw, float density, float pelletsTotal, float volume)
        {
            return 10f * volume * (pelletsRaw / pelletsTotal) * (1f - (float)Math.Pow(0.9, density));
        }

        public static float EffectivePelletCount(float volume, float density)
        {
            return 10f * volume * (1f - (float)Math.Pow(0.9, density));
        }

        // For multiple pellet types, density is computed from total number of pellets
        // Effective number of each type is then calculated usingthe number of pellets f that type.

        // Hardener: +100 kinetic damage and +1.5 AP per pellet
        // High explosive: +200 explosive damage per pellet
        // EMP: +10V EMP damage per pellet
        // Fragmentation: V fragments per pellet.
        //  Each fragment deals 100 kinetic damage at AP 6 regardless of main shell stats
        //  Above the maximum 60 pellets, the uncapped total damage is redistributed among 60 fragments

        public static ShellDamage DamageTypes(float velocity, float volume, float hardner = 0, float highExplosive = 0, float EMP = 0, float fragments = 0)
        {
            ShellDamage totalDamage = new ShellDamage(
                velocity, 2f * volume * velocity,
                3 + (velocity / 150f)
            );

            float totalPellets = hardner + highExplosive + EMP + fragments;
            float density = PackingDensity(totalPellets, volume);
            float effectivePellets = EffectivePelletCount(volume, density);

            float effectiveHardner = (hardner / totalPellets) * effectivePellets;
            float effectiveHighExplosive = (highExplosive / totalPellets) * effectivePellets;
            float effectiveEMP = (EMP / totalPellets) * effectivePellets;
            float effectiveFrag = (fragments / totalPellets) * effectivePellets;

            totalDamage.Kinetic += 100f * effectiveHardner;
            totalDamage.AP += 1.5f * effectiveHardner;
            totalDamage.Explosive += 200f * effectiveHighExplosive;
            totalDamage.EMP += effectiveEMP * (10f * volume);
            if (effectiveFrag > 0)
                totalDamage.Fragmentation = new Fragments(effectiveFrag);

            return totalDamage;
        }

        public static float ShellHealth(float diameter)
        {
            return (float)(300f * Math.PI * Math.Pow(diameter, 2f));
        }

        public static float MinimumReloadTime(float diameter)
        {
            return (float)Math.Pow(diameter / 400f, 1.5);
        }

        public static float NetReloadTime(float minReload, float ammoAutoLoaderConnections)
        {
            return minReload * (1f * (float)Math.Pow(10f / (1f + ammoAutoLoaderConnections), 0.5));
        }

        public static float TraverseSpeed(float motorBarrels, float volume)
        {
            return 145.75f * ((motorBarrels + 1f) / volume + 0.1f);
        }
    }
}
