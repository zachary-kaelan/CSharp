using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicineDosingFormulas
{
    public static class Formulas
    {
        // D - Dosage
        // 

        //public static float BloodVolume(float kg) => kg * Constants.BloodVolumePerKg;

        public static double Concentration(double dosage, double volumeD) => dosage / volumeD;

        public static double EliminationRateConstant(double eliminationHalfLife) => Math.Log(2) / eliminationHalfLife;
    }
}
