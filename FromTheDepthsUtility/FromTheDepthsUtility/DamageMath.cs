using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public class DamageMath
    {
        public static double DefaultDamageMultiplier(double shellAP, double combinedBlocksAC)
        {
            return Math.Min(0.05 + 0.45 * (shellAP / combinedBlocksAC), 1);
        }

        public static double CombinedLayersAC(params double[] layers)
        {
            double total = layers.First();
            double multiplier = 0.85;
            int count = Math.Min(layers.Length, 5);
            for (int i = 1; i < count; ++i)
            {
                total += layers[i] * multiplier;
                multiplier -= 0.15;
            }
            return total + (Convert.ToDouble(Math.Max(Math.Min(layers.Length, 8) - 6, 0)) * 0.1);
        }

        public static double CombinedLayersAC(params Block[] blocks)
        {
            return CombinedLayersAC(blocks.Select(b => b.ArmourClass).ToArray());
        }

        public static double Explosive(double remainingDamage, double distanceRadiusRatio, ref Block[] blocks)
        {
            for(int i = 0; i < blocks.Length; ++i)
            {

            }
        }
    }
}
