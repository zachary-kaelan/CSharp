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

        public static double ResourceGenerationRate(double growthRate, double maxResources, double currentResources)
        {
            return (0.1 * growthRate * maxResources) / Math.Max(10.0, currentResources);
            // Resource generation is maximizd when current resources reaches 10.
            // Max rate is gR / 100
        }

        public static double GatheringRatePerComponent(double currentResources)
        {
            return 0.05 * Math.Pow(currentResources, 0.5);
        }

        public static double SteadyStateResourceLevel(double growthRate, double maxResources, double numberOfMachines)
        {
            return Math.Pow((2.0 * growthRate * maxResources) / numberOfMachines, 2.0 / 3.0);
        }

        // When below maximum
        public static double SteadyStateExtractionRate(double growthRate, double maxResources, double numberOfMachines)
        {
            return 0.5 * Math.Pow(numberOfMachines, 2.0 / 3.0) * Math.Pow(2.0 * growthRate * maxResources, 1.0 / 3.0);
        }

        public static double MachinesForMaxExtractionRate(double growthRate, double maxResources)
        {
            // Assuming continuous gathering
            return (2.0 * growthRate * maxResources) / Math.Pow(10.0, 1.5);
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
        public static double ProcessingEfficiency(double crackers, double cokers, double desalters, double altitude)
        {
            return ((10.0 * ((0.5 * crackers) + 1.0) * ((0.3 * cokers) + 1.0)) / Math.Max(altitude / 5.0, 1.0)) * Math.Pow(1.1, Math.Min(desalters, 1));
        }

        public static double ProcessingTime(double crackers, double cokers, double desalters)
        {
            return ((1.5 * crackers) + 1.0) * ((0.6 * cokers) + 1) * Math.Pow(1.2, Math.Min(desalters, 1));
        }

        // Refineries have a Dangerous (G)as level
        // When G is above 50, the refinery takes damage
        // G increases by 0.8 * Efficiency every 2 seconds
        // Tilting the Refinery induces extra G, up to 20 at 90 degrees

        public static double DangerousGasGeneration(double currentGas, double efficiency, double flares, bool desalter = true)
        {
            return (
                (0.8 * efficiency) - 
                ((0.5 + (0.5 * Math.Pow(currentGas, 0.5))) +
                    (flares * (desalter ? 1.5 : 0.5))
                )
            );
        }
        
        public static double FlaresForSafe(double efficiency, bool desalter = true)
        {
            double flares = 0;
            while (true)
            {
                double gas = 0;
                double lastGas = 0;

                do
                {
                    lastGas = DangerousGasGeneration(gas, efficiency, flares, desalter);
                    gas += lastGas;
                } while (gas < 50.0 && lastGas > 0.125);

                if (gas < 50.0)
                    return flares;
                ++flares;
            }
        }
    }
}
