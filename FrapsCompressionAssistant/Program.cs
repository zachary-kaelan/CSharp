using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrapsCompressionAssistant
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(10000);
            while (true)
            {
                IEnumerable<FileInfo> files = Directory.GetFiles(@"N:\Recordings", "*.avi").Select(f => new FileInfo(f)).OrderBy(f => f.CreationTime);
                if (files.Any())
                {
                    Thread.Sleep(500);
                    if (files.Last().Length == 4227858432)
                        files = files.Take(files.Count() - 1);
                    Thread.Sleep(500);
                    foreach (var file in files)
                    {
                        var path = file.FullName;
                        Console.Write("Processing " + Path.GetFileNameWithoutExtension(path) + " - " + file.Length + " bytes ... \t");
                        var process = Process.Start(
                            new ProcessStartInfo(@"ffmpeg", "-i \"" + path + "\" \"N:\\" + file.Name.Replace(".avi", ".mp4") + "\"")
                            {
                                UseShellExecute = false,
                                WindowStyle = ProcessWindowStyle.Hidden
                            }
                        );
                        process.PriorityClass = ProcessPriorityClass.Normal;
                        process.WaitForExit();
                        Console.WriteLine(" Done");
                        process.Close();
                        process = null;
                        File.Delete(path);
                    }
                }
                else
                    Thread.Sleep(30000);
            }
        }
    }
}
