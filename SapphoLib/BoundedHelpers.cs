using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    public static class BoundedHelpers
    {
        public static float Blend(float num1, float num2, float weightingFactor) =>
            (num2 * weightingFactor) +
            (num1 * (1f - weightingFactor));

        public static float BlendUnbounded(float num1, float num2, float weightingFactor) =>
            (BoundedNumber._fromUnbounded(num2) * weightingFactor) +
            (BoundedNumber._fromUnbounded(num1) * (1f - weightingFactor));
    }
}
