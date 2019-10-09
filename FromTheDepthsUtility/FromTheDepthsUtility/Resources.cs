using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public static class ResourceMath
    {
        // Each resource zone has a growth rate g and maximum storage R
        // r is the amount of currently stored resources

        public static float ResourceGenerationRate(float growthRate, float maxResources, float currentResources)
        {
            return (0.1f * growthRate * maxResources) / Math.Max(10f, currentResources);
            // Resource generation is maximizd when current resources reaches 10.
            // Max rate is gR / 100
        }

        public static float GatheringRatePerComponent(float currentResources)
        {
            return 0.05f * (float)Math.Pow(currentResources, 0.5);
        }

        public static float SteadyStateResourceLevel(float growthRate, float maxResources, float numberOfMachines)
        {
            return (float)Math.Pow((2f * growthRate * maxResources) / numberOfMachines, 2f / 3f);
        }

        // When below maximum
        public static float SteadyStateExtractionRate(float growthRate, float maxResources, float numberOfMachines)
        {
            return 0.5f * (float)Math.Pow(numberOfMachines, 2f / 3f) * (float)Math.Pow(2f * growthRate * maxResources, 1f / 3f);
        }

        public static float MachinesForMaxExtractionRate(float growthRate, float maxResources)
        {
            // Assuming continuous gathering
            return (2f * growthRate * maxResources) / (float)Math.Pow(10f, 1.5);
        }

        // Each cubic meter of resource storage stores 2000 units
        // Resources from wrecked ships can be picked up within 500m
        // Transfer range between allies is 1m per 100 total storage
        // Minimum of 100m and maximum of 2000m (100 cubic metres)

        // Ammo Processor converts 5 material to 20 ammo every 2 seconds
        // Ammo storage containers regenerate 1 ammo per second

        // Fuel refineries turn materials into fuel
        // The number of units of fuel generated per unit of material = Efficiency
        // Each cycle consumes 10 units of material and takes ProcessTime

        // number of c(R)ackers
        // number of c(O)kers
        // number of (D)esalters
        // y = altitude of refinery
        public static float ProcessingEfficiency(float crackers, float cokers, float desalters, float altitude)
        {
            return ((10f * ((0.5f * crackers) + 1f) * ((0.3f * cokers) + 1f)) / Math.Max(altitude / 5f, 1f)) * (float)Math.Pow(1.1, Math.Min(desalters, 1));
        }

        public static float ProcessingTime(float crackers, float cokers, float desalters)
        {
            return ((1.5f * crackers) + 1f) * ((0.6f * cokers) + 1) * (float)Math.Pow(1.2, Math.Min(desalters, 1));
        }

        // Refineries have a Dangerous (G)as level
        // When G is above 50, the refinery takes damage
        // G increases by 0.8 * Efficiency every 2 seconds
        // Tilting the Refinery induces extra G, up to 20 at 90 degrees

        public static float DangerousGasGeneration(float currentGas, float efficiency, float flares, bool desalter = true)
        {
            return (
                (0.8f * efficiency) - 
                ((0.5f + (0.5f * (float)Math.Pow(currentGas, 0.5))) +
                    (flares * (desalter ? 1.5f : 0.5f))
                )
            );
        }
        
        public static float FlaresForSafe(float efficiency, bool desalter = true)
        {
            float flares = 0;
            while (true)
            {
                float gas = 0;
                float lastGas = 0;

                do
                {
                    lastGas = DangerousGasGeneration(gas, efficiency, flares, desalter);
                    gas += lastGas;
                } while (gas < 50f && lastGas > 0.125);

                if (gas < 50f)
                    return flares;
                ++flares;
            }
        }
    }
}
