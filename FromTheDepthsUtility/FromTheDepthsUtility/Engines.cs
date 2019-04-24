﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public static class FuelEngineMath
    {
        public static float HeatFactor(float heat)
        {
            return 1f - (Math.Pow(heat, 3f) / 2f);
        }

        public static (float[], float) CoolingRate(Cylinder[] cylinders, float h, float NumberOfRadiators = 0)
        {
            // h = heat generated by 1 fuel per second
            float radiatorCooling = 2f * Math.Pow((NumberOfRadiators / (float)Convert.ToDouble(cylinders.Length)) * h, 0.5);
            return (
                cylinders.Select(
                    c => (4f * h * c.Exhausts) + 
                        (0.4 * h) +
                        (2 * c.Turbochargers) +
                        radiatorCooling
                ).ToArray(), 
                (1f + (0.01 * Math.Pow(NumberOfRadiators, 0.5)))
            );
        }

        // RPM = Relative RPM

        public static float CylinderBurnRate(float RPM, float injectors = 0, float carburetors = 0)
        {
            // Liters per second
            float BasicEngineCurve = 0.8 + (0.4 * RPM);

            if (injectors == 0 && carburetors == 0)
                return (10f * RPM) / (100f * 1.2 * BasicEngineCurve);
            else
                return ((injectors * RPM * 200f) / (100f * 1.1 * BasicEngineCurve)) +
                    (RPM * BasicEngineCurve * carburetors);
        }

        public struct Cylinder
        {
            public float Exhausts;
            public float Turbochargers;
        }
    }

    public static class ElectricEngineMath
    {
        // Each cubic meter of battery can store 1000 kJ

        public static float MaxChargeConsumption(float charge, float powerOutput)
        {
            // kW / m^3
            return 0.4 * charge * powerOutput;
            // 40 at full charge.
        }

        public static float Efficiency(float powerOutput)
        {
            return 2f / (1f + powerOutput);
        }

        public static float MaxPowerOutput(float charge, float powerOutput)
        {
            return MaxChargeConsumption(charge, powerOutput) * Efficiency(powerOutput);
        }

        // RTGs cost 375 resources
        // Produce 25 battery power per second per cubic metre

        // b = volume ratio of batteries to RTGs
        // M is the factor between the maximum power output and the sustainable output
        // c = cost ratio of RTGs to batteries

        public static float MaxSustainableOutput(float powerOutput)
        {
            // Per cubic metre of RTG
            return 50f / (1f + powerOutput);
        }

        // Maximize ratio of power output to cost

        public static float CostRatio(float powerOutput, float headroom)
        {
            return (1.25 * (1f + headroom)) / Math.Pow(powerOutput, 2f);
        }

        public static float PowerOutputForRatio(float headroom, float costRatio)
        {
            return Math.Pow((1.25 * (1f + headroom)) / costRatio, 0.5);
        }

        // For M = 1.2, and c = 18.75; powerOutput = 0.28 and battery ratio = 4.2
    }

    public static class SteamEngineMath
    {
        // Steam engine produces engine power + energy directly from resources
        // steam = m (mass in kilograms)
        // Pressure = P; production of power and energy, steam movement
        // Too much pressure can make things explode
        // Volume = V; steam containment

        // Steam is an ideal gas; (PV)/T = constant
        // Temperature = T (seems to be constant)
        // Pressure is directly proportional to density
        // PV = constant * m
        // (PV) / m = constant

        // PV = m in part, seems to work
        // Can set constant to 1 (velocity squared) for now

        // Boilers produce steam at a constant rate, as long as you have resources
        // Steam pipes have a volume of 0.2 m^3
        // A continuous pipe assembly has a common valume of the sum of its pipes
        // Pistons, turbines, open pipes, and pressure release valves consume steam
        // Gears and generators produce energy and power

        // Movement of steam is due to pressure differences
        // kg/s = pressure difference
        // Small boiler has a volume of 0.8 m^3 and produces 320 steam per second
        // Turbines have a volume of 0.8m^3 and consumed whatever they are given

        // Assume a simple engine of a boiler, combact turbine, and pipes
        // Pipe pressure = Turbine pressure + 320
        // Boiler pressure = Turbine pressure + 640
        // Add a small piston
        // Pipe pressure = Turbine pressure + 45
        // Turbine consumes 45 steam per second and piston consumes 275
        // Larger boilers need more time and pressure to push out steam

        // In a turbines-only setup, boiler pressure is triple its steam generation
        // Turbines produce energy and pistons produce engine power
        // Pistons don't have any volume or pressure, as they don't contain steam
        // Pistons produce power equal to half the steam consumed
        // Piston minimum RPM is proportional to pipe steam pressure
        // Pressure release valves always has a set perssure
    }
    
}
