using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.Engines
{
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
