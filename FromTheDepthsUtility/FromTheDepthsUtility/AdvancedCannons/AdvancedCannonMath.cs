using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FromTheDepthsUtility.AdvancedCannons
{
    public static class AdvancedCannonMath
    {
        public static float TraverseSpeed(float length, int numBarrels, float diameter) =>
            1 / (float)Math.Pow(
                0.1 + (0.25 * Math.PI * diameter * diameter * numBarrels * length), 0.7
            );
    }
}
