using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility
{
    public class DamageMath
    {
        public static float DefaultDamageMultiplier(float shellAP, float combinedBlocksAC)
        {
            //return Math.Min(0.05 + 0.45 * (shellAP / combinedBlocksAC), 1);
            return Math.Min(0.5f * (shellAP / combinedBlocksAC), 1);
        }

        // Structural blocks behind a block being damaged will contribute part of their armor
        // Layer 0 is the layer being hit
        // Layers 1-6 contribute 100% to 25% of their armor
        // Layers 7 and 8 contribute 10% of their armor, against kinetic only
        public static (float, float) CombinedLayersAC(params float[] layers)
        {
            float total = layers.First();
            float multiplier = 1;
            int count = Math.Min(layers.Length, 7);
            for (int i = 1; i < count; ++i)
            {
                total += layers[i] * multiplier;
                multiplier -= 0.15f;
            }
            return (total, total + ((float)Convert.ToDouble(Math.Max(Math.Min(layers.Length, 9) - 6, 0)) * 0.1f));
        }

        public static (float, float) CombinedLayersAC(params Block[] blocks)
        {
            return CombinedLayersAC(blocks.Select(b => b.ArmourClass).ToArray());
        }

        public static KineticRichocet Kinetic(float angle, float shellAP, float combinedBlocksAC)
        {
            var cosTheta = (float)Math.Cos(angle);
            return new KineticRichocet(cosTheta, DefaultDamageMultiplier(shellAP, combinedBlocksAC))
            {
                RichocetChance = (float)Math.Pow(1 - cosTheta, (2 * shellAP) / combinedBlocksAC)
            };
        }

        public static float Explosive(float remainingDamage, float distanceRadiusRatio, ref Block[] blocks)
        {
            throw new NotImplementedException();
            for(int i = 0; i < blocks.Length; ++i)
            {

            }
        }
    }
}
