using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetrayalLib
{
    public class DiceRollStats
    {
        public double Average { get; private set; }
        public double[] Chances { get; private set; }
        public int[] NumOutcomes { get; private set; }
        public int NumDice { get; private set; }
        public int MaxRoll { get; private set; }

        public DiceRollStats(double avg, int dice, int maxRoll, int[] numOutcomes, double[] chances)
        {
            Average = avg;
            NumDice = dice;
            NumOutcomes = numOutcomes;
            Chances = chances;
            MaxRoll = maxRoll;
        }
    }
}
