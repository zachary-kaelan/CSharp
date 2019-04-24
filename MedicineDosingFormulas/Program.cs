using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicineDosingFormulas
{
    class Program
    {
        public static double HALF_LIFE1 = 10;
        public static double HALF_LIFE2 = 12.5;

        static void Main(string[] args)
        {
            double quantity = 0;
            for (int i = 0; i < 120; ++i)
            {
                Console.WriteLine("Day {0} - {1} mg", i, quantity.ToString("#.00"));
                for (int j = 0; j < 4; ++j)
                {
                    quantity = HalfLife(quantity, 11, 3);
                    quantity += 10;
                }
                quantity = HalfLife(quantity, 11, 12);
            }-
            Console.ReadLine();
        }

        public static double HalfLife(double initialQuantity, double halfLife, double time)
        {
            return initialQuantity * Math.Pow(0.5, time / halfLife);
        }
    }
}
