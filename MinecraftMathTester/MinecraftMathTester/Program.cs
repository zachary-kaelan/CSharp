using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fNbt;

namespace MinecraftMathTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = new NbtFile(@"C:\Users\ZACH-GAMING\Downloads\madiaval-lamp.schematic");
            Console.WriteLine(file.ToString("\t"));
            Console.ReadLine();
        }
    }
}
