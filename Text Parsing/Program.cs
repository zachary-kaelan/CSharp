using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Parsing
{
    class Program
    {
        static void Main(string[] args)
        {
            var Killers = File.ReadAllText(
                @"E:\Dead By Daylight\Programming Stuff\Killers Numerical Overview.txt"
            ).Split(
                new string[] { "\r\n\r\n" }, 
                StringSplitOptions.RemoveEmptyEntries
            ).Select(
                k => k.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            ).ToDictionary(
                k => new string(k.First().TakeWhile(c => c != ' ').ToArray()),
                k => k.Skip(1).Select(
                    t => t.Split(
                        new string[] { ": " }, 
                        StringSplitOptions.None
                    )
                ).ToDictionary(
                    t => t.First(),
                    t => Convert.ToDouble(t.Last())
                )
            );

            var traits = Killers.SelectMany(k => k.Value).GroupBy(
                t => t.Key,
                t => t.Value
            ).ToDictionary(
                t => t.Key,
                t => t.ToArray()
            );

            foreach(var trait in traits)
            {
                var values = trait.Value;
                Array.Sort(values);
                Console.WriteLine(" ~ " + trait.Key.ToUpper() + " ~ ");
                double mean = values.Average();
                Console.WriteLine("  Mean: " + mean.ToString("##.0"));
                Console.WriteLine("  Standard Deviation: " + Math.Pow(values.Average(v => Math.Pow(v - mean, 2)), 0.5).ToString("##.0"));
                Console.WriteLine(
                    "  Median: " + values.Skip(4).Take(2).Average().ToString("##.0")
                );
                var mode = values.GroupBy(v => Convert.ToInt32(Math.Round(v)), v => 0, (v, g) => new KeyValuePair<int, int>(v, g.Count())).OrderByDescending(kv => kv.Value).First();
                Console.WriteLine("  Mode: " + (mode.Value > 1 ? mode.Key.ToString() + " x" + mode.Value.ToString() : "None"));
                Console.WriteLine();
            }

            Console.WriteLine(" ~~~~~~~~~~ ");
            Console.ReadLine();
        }
    }
}
