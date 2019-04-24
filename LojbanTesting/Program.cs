using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LojbanLib;

namespace LojbanTesting
{
    class Program
    {
        public static string[][] RAFSI = new string[][]
        {
            new string[] { "xek", "xe'i", "xekr", "xekri" },
            new string[] { "jev", "jve" },
            new string[] { "bla", "blan", "blanu" }
        };

        static void Main(string[] args)
        {
            var lujvoOptions = LojbanAnalyzer.GetLujvoVariants(RAFSI);
            foreach(var option in lujvoOptions)
            {
                Console.WriteLine(option.Item2 + "\t|\t" + option.Item1 + "\t|\t" + option.Item3);
            }
            Console.ReadLine();
        }
    }
}
