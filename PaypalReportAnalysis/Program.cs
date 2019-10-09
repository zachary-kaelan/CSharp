using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZachLib;

namespace PaypalReportAnalysis
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var payments = Utils.LoadCSV<Payment>(@"C:\Users\ZACH-GAMING\Downloads\Paypal Report.CSV").Where(p => p.GetAmount() < 0);

            var groupedTypes = payments.GroupBy(p => p.Type).OrderBy(g => g.Sum(p => p.GetAmount()));
            foreach(var type in groupedTypes)
            {
                Console.WriteLine("{0} - ${1}", type.Key, type.Sum(p => p.GetAmount()));
                var groupedNames = type.GroupBy(p => p.Name, p => p.GetAmount(), (k, g) => new KeyValuePair<string, double>(k, g.Sum())).OrderBy(n => n.Value);
                foreach(var name in groupedNames)
                {
                    Console.WriteLine("\t{0} - ${1}", name.Key, name.Value);
                }
                Console.WriteLine();
            }
            Console.ReadLine();

            var names = payments.GroupBy(
                p => p.Name,
                p => p.GetAmount(),
                (k, g) => new KeyValuePair<string, double>(
                    k, g.Sum()
                )
            ).Where(kv => kv.Value < 0).OrderBy(kv => kv.Value);
            var excludedNames = names.Take(5).Select(n => n.Key);

            var types = payments.Where(
                p => !excludedNames.Contains(p.Name)
            ).GroupBy(p => p.Type).OrderBy(
                g => g.Sum(
                    p => p.GetAmount()
                )
            ).ToArray();

            Console.WriteLine(
                String.Join(
                    "\r\n\r\n",
                    types.Select(
                        t => t.Key + " - " + t.Sum(
                            p => p.GetAmount()
                        ) + "\r\n\t" + String.Join(
                            "\r\n\t",
                            t.GroupBy(
                                p => p.Name,
                                p => p.GetAmount(),
                                (k, g) => new KeyValuePair<string, double>(k, g.Sum())
                            ).Where(kv => kv.Value < 0)
                        )
                    )
                )
            );

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
