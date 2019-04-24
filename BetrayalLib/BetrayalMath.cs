using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace BetrayalLib
{
    public static class BetrayalMath
    {
        public const int NumDice = 8;

        static BetrayalMath()
        {
            DiceStats = new DiceRollStats[NumDice + 1];
            DiceStats[0] = new DiceRollStats(0, 0, 0, new int[] { 1 }, new double[] { 1 });
            int prevCount = 0;
            for (int d = 1; d < NumDice + 1; ++d)
            {
                int max = 2 * d;
                int[] chances = new int[max];
                int[] numOutcomes = new int[max];
                int count = ZachMath.IntPow(3, d);
                for (int i = 0; i < d; ++i)
                {
                    var prevDiceStats = DiceStats[d];
                    for (int n = 0; n < prevDiceStats.MaxRoll; ++n)
                    {
                        int num = prevDiceStats.NumOutcomes[n];
                        for (int j1 = 0; j1 <= 2; ++j1)
                        {
                            for (int j2 = j1; j2 <= 2; ++j2)
                            {
                                numOutcomes[n + j2] += j1;
                            }
                        }
                    }
                }
            }
        }

        public static DiceRollStats[] DiceStats { get; private set; }
    }
}
