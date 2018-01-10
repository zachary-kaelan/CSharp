using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASCV4EXTMENULib;

namespace LowLevelTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    if (process.Modules.OfType<ProcessModule>().Any(m => m.ModuleName.Equals("mscoree.dll", StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine("{0} is a .NET process", process.ProcessName);
                    }
                }
                catch
                {
                    Console.WriteLine("{0} cannot be accessed.", process.ProcessName);
                }
                
            }
            Console.ReadLine();
        }
    }
}
