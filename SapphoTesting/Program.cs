using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SapphoLib;

namespace SapphoTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            float x = 0.85f;
            float fx = Activation(x);
            float deriv = Deriv(x);
            Console.WriteLine(fx);
            Console.WriteLine(deriv);
            float fxPlusD = Activation(0.85f + deriv);
            Console.WriteLine(fxPlusD);
            Console.WriteLine((fxPlusD - fx) / deriv);

            Console.ReadLine();
        }

        public static float Activation(float x) => (1 / (1 - x)) - 1;

        public static float Deriv(float x)
        {
            var bottom = (1 + x);
            return 1 / (bottom * bottom);
        }
    }
}
