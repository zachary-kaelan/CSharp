using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace DiscreteProbLib
{
    internal class Helpers
    {
        public static int Factorial(int n, int fromIndex = 3)
        {
            System.Runtime.Caching.
            if (n == 1)
                return 1;
            else if (n == 2)
                return 2;
            else if (n == fromIndex)
                return n;

            int product = fromIndex == 3 ? 2 : 1;
            for (int i = fromIndex; i <= n; ++i)
            {
                product *= i;
            }

            return product;
        }
    }
}
