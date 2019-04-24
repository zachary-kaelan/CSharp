using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redzen.Random;

namespace DNALib.GeneticAlgorithm
{
    internal enum Mutation
    {
        Bitwise,
        Complementary,
        Boundary,
        NonUniform,
        Uniform,
        Gaussian,
        Shrink,
        FrameShift
    }

    public class GenePool
    {
        public bool Frozen { get; private set; }
        private SortedDictionary<string, GeneParameter> GeneParameters { get; set; }
        private byte ChromosomeLength { get; set; }
        private KeyValuePair<float, Mutation>[] MutationChances { get; set; }
        private ulong MaxChromosomeValue { get; set; }

        public GenePool()
        {
            GeneParameters = new SortedDictionary<string, GeneParameter>();
            Frozen = false;
        }

        public bool AddParameterType(string name, bool mutable, params int[] ids)
        {
            if (Frozen || GeneParameters.ContainsKey(name))
                return false;
            GeneParameters.Add(name, new GeneParameter(ids));
            return true;
        }

        // switches to read-only
        public byte FreezeGenePool()
        {
            Frozen = true;

            var byMaxValue = GeneParameters.Values.OrderBy(o => o.MaxValue);
            KeyValuePair<int, int>[] sums = new KeyValuePair<int, int>[GeneParameters.Count];
            ulong chromosomeMaxValue = 0;
            byte prevPowerOfTwoIndex = 0;
            int prevBitsOffSum = 0;
            int prevBitsOnSum = 0;
            byte prevMaxValue = 0;
            byte prevBits = byMaxValue.First().Bits;
            int index = 0;
            foreach(var operon in byMaxValue)
            {
                ChromosomeLength += operon.Bits;
                int bitsOffSum = prevBitsOffSum;
                int bitsOnSum = prevBitsOnSum;
                if (operon.Bits > prevBits)
                {
                    bitsOffSum += prevMaxValue;
                    prevBits = operon.Bits;
                }
                chromosomeMaxValue += Constants.ALL_POWERS_OF_2_REVERSE[prevPowerOfTwoIndex] * operon.MaxValue;
                prevPowerOfTwoIndex += operon.Bits;

                for (byte bits = prevMaxValue; bits <= operon.MaxValue; ++bits)
                {
                    byte temp = bits;
                    for (int bit = operon.Bits; bit >= 0; --bit)
                    {
                        var currentPower = Constants.POWERS_OF_2_REVERSE[bit];
                        if (temp > currentPower)
                        {
                            temp -= currentPower;
                            ++bitsOnSum;
                        }
                        else if (temp < currentPower)
                        {
                            ++bitsOffSum;
                            if (temp == currentPower - 1)
                            {
                                bitsOnSum += bit;
                                break;
                            }
                        }
                        else
                        {
                            ++bitsOnSum;
                            bitsOffSum += bit;
                            break;
                        }
                    }
                }

                prevBitsOffSum = bitsOffSum;
                prevBitsOnSum = bitsOnSum;
                prevMaxValue = operon.MaxValue;
                sums[index] = new KeyValuePair<int, int>(bitsOffSum, bitsOnSum);
            }
            MaxChromosomeValue = chromosomeMaxValue;
            float bitwiseChance = 1f / ChromosomeLength;
            float bitsTotal = prevBitsOffSum + prevBitsOnSum;
            float bitsOffChance = prevBitsOffSum / bitsTotal;
            float bitsOnChance = prevBitsOnSum / bitsTotal;
            float avgOperonLength = prevPowerOfTwoIndex / (float)GeneParameters.Count;

            byte maxValueBitsOn = 0;
            byte maxValueBitsOff = 0;
            for (int bit = ChromosomeLength; bit >= 0; --bit)
            {
                var currentPower = Constants.ALL_POWERS_OF_2_REVERSE[bit];
                if (chromosomeMaxValue >= currentPower)
                {
                    chromosomeMaxValue -= currentPower;
                    ++maxValueBitsOn;
                }
                else
                    ++maxValueBitsOff;
            }
                       
            float[] chances = new float[8];
            chances[0] = bitwiseChance;
            chances[1] = bitwiseChance * bitwiseChance;
            chances[2] = bitwiseChance / (
                (
                    (
                        (maxValueBitsOn * bitsOffChance) + (maxValueBitsOff * bitsOnChance)
                    ) / 2
                ) + (
                    (ChromosomeLength * bitsOnChance) / 2   // for the lower boundary, which is all zeroes
                )
            );
            chances[3] = 0;
            chances[4] = bitwiseChance / (bitsOffChance * bitsOnChance * 2 * avgOperonLength);
            chances[5] = chances[4];
            chances[6] = chances[5];
            chances[7] = (
                (
                    bitwiseChance / (bitsOffChance * bitsOnChance * 2 * ChromosomeLength)
                ) + chances[1]
            ) / 2;

            var sum = chances.Sum();

            MutationChances = new KeyValuePair<float, Mutation>[8];
            for (int i = 0; i < 8; ++i)
            {
                float chance = chances[i];
                MutationChances[i] = new KeyValuePair<float, Mutation>(
                    bitwiseChance * (chance / sum),
                    (Mutation)i
                );
            }

            return ChromosomeLength;
        }
    }
}
