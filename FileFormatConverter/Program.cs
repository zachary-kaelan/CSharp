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
        static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(@"E:\Installations\Fraps\Movies\Dead By Daylight", "*.avi", SearchOption.AllDirectories);

            foreach(string file in files)
            {
                Console.Write("Processing " + Path.GetFileNameWithoutExtension(file) + "...");
                var process = Process.Start(@"E:\Installations\Programming_Frameworks\ffmpeg\ffmpeg_x64.exe", "-i \"" + file + "\" \"" + file.Replace(".avi", ".mp4") + "\"");
                process.PriorityClass = ProcessPriorityClass.AboveNormal;
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
