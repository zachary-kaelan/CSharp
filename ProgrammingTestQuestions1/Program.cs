using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammingTestQuestions1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter an amount in US Dollars: ");
            moneyConverter converter = new moneyConverter(Convert.ToDecimal(Console.ReadLine()));
            Console.WriteLine("Equivalent amount in Euro: " + converter.ToEuro().ToString());
            Console.WriteLine("Equivalent amount in Pound: " + converter.ToPound().ToString());

            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }
    }

    public class moneyConverter
    {
        public decimal Dollars {
            get
            {
                return Dollars;
            }
            set
            {
                if (value >= 0)
                    Dollars = value;
            }
        }

        public moneyConverter(decimal dollars)
        {
            Dollars = dollars;
        }

        public decimal ToEuro()
        {
            return Dollars * 0.74m;
        }

        public decimal ToPound()
        {
            return Dollars * 0.63m;
        }
    }
}
