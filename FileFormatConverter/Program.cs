using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileFormatConverter
{
    class Program
    {
        public static readonly KeyValuePair<double, double>[] MENTOR_EXP = new Dictionary<double, double>()
        {
            { 0, 1 },
            { 25000, 1.01 },
            { 50000, 1.02 },
            { 100000, 1.03 },
            { 200000, 1.04 },
            { 624000, 1.05 },
            { 1000000, 1.06 },
            { 1100000, 1.07 }
        }.ToArray();

        public const double OTHEREXP = 52530000;

        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(@"G:\Dead By Daylight", "*.avi", SearchOption.AllDirectories).Where(f => !f.ToLower().Contains("backup")).ToArray();

            foreach(string file in files)
            {
                Console.Write("Processing " + Path.GetFileNameWithoutExtension(file) + "...");
                var process = Process.Start(
                    new ProcessStartInfo(@"ffmpeg", "-i \"" + file + "\" \"" + file.Replace(".avi", ".mp4") + "\"")
                    {
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                );
                process.PriorityClass = ProcessPriorityClass.BelowNormal;
                process.WaitForExit();
                Console.WriteLine("\t Done");
                process.Close();
                process = null;
                File.Delete(file);
            }

            Console.WriteLine();
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }
    }
}
