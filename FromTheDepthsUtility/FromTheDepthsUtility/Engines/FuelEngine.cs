using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.Engines
{
    public class FuelEngine
    {
        private Cylinder[] Cylinders { get; set; }

        public FuelEngine(CylinderInput[] cylinders, int numRadiators)
        {
            //Cylinders = cylinders.Select()
        }
    }

    public struct CylinderInput
    {
        public float Exhausts;
        public float Turbochargers;
    }

    public struct Cylinder
    {
        public float CoolingRate;
        public float HeatPercentage;
        public float PowerGen;
    }

    public struct Carburetor
    {
        public int NumSuperChargers;
        public int NumTurboChargers;
    }
}
