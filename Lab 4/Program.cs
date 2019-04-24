using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_4
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        public static void Question1()
        {
            Console.Write("How many credit hours do you have? ");
            int creditHours = Convert.ToInt32(Console.ReadLine());
            Console.Write("What is your GPA? ");
            double GPA = Convert.ToDouble(Console.ReadLine());

            if (creditHours >= 72 && GPA >= 2.0)
                Console.WriteLine("You can graduate!");
            else
                Console.WriteLine("You can't graduate.");
        }

        public static void Question2()
        {
            Console.Write("What is the age of the patron? ");
            Console.WriteLine(
                "The fee is $" + 
                (Convert.ToInt32(Console.ReadLine()) >= 16 ? "12" : "7")
            );
        }
    }
}
