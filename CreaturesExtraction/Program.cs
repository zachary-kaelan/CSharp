using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreaturesExtraction
{
    

    class Program
    {
        public const string MAIN_PATH = @"E:\Gaming\GOG Galaxy\Creatures 1\";
        public const string GENES_PATH = MAIN_PATH + @"Genetics\";

        static void Main(string[] args)
        {


            foreach(var geneFile in Directory.GetFiles(GENES_PATH, "*gen"))
            {
                string[] genes = File.ReadAllText(geneFile, Encoding.ASCII).Split(new string[] { "gene", "gend" }, StringSplitOptions.RemoveEmptyEntries);
                foreach(var gene in genes)
                {

                }
            }
        }

        private static void DecodeFile(string path)
        {
            // http://double.nz/creatures/creatures2/c2genfile.txt

            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte firstByte = (byte)file.ReadByte();
            if (firstByte == 100)
            {
                // Creatures 2 or Creatures 3 file format
                file.ReadByte();
                file.ReadByte();
                firstByte = (byte)file.ReadByte();
                if (firstByte == 50)
                {
                    // Creatures 2 file format

                }
            }
        }
    }
}
