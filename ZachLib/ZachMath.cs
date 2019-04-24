using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    public static class ZachMath
    {
        public static int IntPow(int x, int y)
        {
            if (y == 0)
                return 1;
            else if (y == 1)
                return x;
            int total = x;
            for (int i = 0; i < y - 1; ++i)
            {
                total *= x;
            }
            return total;
        }

        public static int IntLog(int z, int x)
        {
            if (z == 1)
                return 0;
            int max = z / 2;
            for (int y = 1; y <= max; ++y)
            {
                if (IntPow(x, y) == z)
                    return y;
            }
            return -1;
        }
    }
}
