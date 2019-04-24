using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace GetYouTubeAudio
{
    class Program
    {
        private const string DIRECTORY = @"C:\Users\ZACH-GAMING\YouTubeDownloads\";
        static void Main(string[] args)
        {
            string url = null;
            Console.Write("URL: ");
            url = Console.ReadLine();
            do
            {
                if (!Directory.Exists(DIRECTORY))
                    Directory.CreateDirectory(DIRECTORY);

                var process = Process.Start(
                    new ProcessStartInfo("youtube-dl.exe", "-f bestaudio --extract-audio --audio-format mp3 " + url + " -o \"" + DIRECTORY + "%(title)s.%(ext)s\"")
                    {
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                );
                process.PriorityClass = ProcessPriorityClass.High;
                process.WaitForExit();
                process.Close();
                process = null;
                Console.WriteLine();

                Console.Write("URL: ");
                url = Console.ReadLine();
            } while (url.StartsWith("http"));
        }
    }
}
