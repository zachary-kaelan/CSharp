using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscreteProbLib
{
    public static class Trials
    {
        public static float BinomialDist(int successes, int trials, float probability)
        {
            if (successes == 0)
                return 0;
            else if (successes == 1 && trials == 1)
                return probability;

            int factorialSum = 1;
            int factorialMaxIndex = 3;

            int nFactorial = 1;
            int kFactorial = 1;
            int nKDifFactorial = 1;
            int failures = trials - successes;
            var q = 1f - probability; // probability of failure

            if (failures > successes)
            {
                
            }
        }
    }
}
