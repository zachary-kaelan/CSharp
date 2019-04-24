using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNALib.GeneticAlgorithm
{
    public class Evolution
    {
        private float Bountifulness { get; set; }
        public int PopulationSize { get; private set; }
        private Func<byte[], bool> SurvivalFunction;
        private Func<byte[], float> FertilityFunction;
        private Func<byte[], float> FitnessFunction;

        public Evolution(
            Func<byte[], bool> survivalFunction,    // hard limits 
            Func<byte[], float> fertilityFunction,  // number of kids
            Func<byte[], float> fitnessFunction     // attractiveness
        ) {
            SurvivalFunction = survivalFunction;
            FertilityFunction = fertilityFunction;
            SurvivalFunction = survivalFunction;
        }
    }
}
