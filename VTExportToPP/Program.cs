using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTWebLib;

namespace VTExportToPP
{
    [Flags]
    public enum Choice
    {
        None = 0,
        Sweep = 1,
        FixErrors = 2
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Pick a choice:\r\n 1. Sweep\r\n 2. Fix Errors\r\n 3. Both\r\n 0. Neither\r\n:");
            var choice = (Choice)Convert.ToInt32(
                Char.GetNumericValue(
                    Console.ReadLine().First(
                        c => char.IsDigit(c)
                    )
                )
            );
            if (choice.HasFlag(Choice.Sweep))
            {
                Console.Write("Would you like to retry previously errored exports? (y/n) ");
                VantageTracker.TryExportAllBranches(Console.ReadKey().KeyChar == 'y');
            }
            if (choice.HasFlag(Choice.FixErrors))
                VantageTracker.FixVTTechPPUsernames();

            Console.WriteLine("FINISHED ALL OPERATIONS, press Enter to exit...");
            Console.ReadLine();
            Console.ReadLine();
        }
    }
}
