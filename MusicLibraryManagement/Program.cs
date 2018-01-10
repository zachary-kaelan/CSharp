using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MusicLibraryManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = @"E:\Music\(2016) Stranger Things, Vol. 1\";
            string[] files = Directory.GetFiles(dir, "*ST*");
            int index = dir.Length;
            int length = "04 - Kyle Dixon & Michael Stein".Length;
            foreach(string file in files)
            {
                /*if (Char.IsDigit(Path.GetFileName(file)[0]))
                    File.Move(file, file.Remove(index, length).Insert(index, "ST2"));*/
                if (file.Contains('_'))
                    File.Move(file, file.Replace("_", ""));
            }

            /*FileInfo info = new FileInfo(@"E:\Music\(2016) Stranger Things, Vol. 1\01 - Kyle Dixon & Michael Stein - Stranger Things.mp3");
            var file = StorageFile.GetFileFromPathAsync(@"E:\Music\(2016) Stranger Things, Vol. 1\01 - Kyle Dixon & Michael Stein - Stranger Things.mp3").GetResults();
            var props = file.Properties;
            var musicprops = props.GetMusicPropertiesAsync().GetResults();
            var docProps = props.GetDocumentPropertiesAsync().GetResults();
            var basicProps = file.GetBasicPropertiesAsync().GetResults();*/

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
