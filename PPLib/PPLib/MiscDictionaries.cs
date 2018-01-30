using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;
using ZachLib;

namespace PPLib
{
    public static class MiscDictionaries
    {
        public static readonly SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, ZipCode>>> ZipCodes = JSON.Deserialize<SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, ZipCode>>>>(
            File.ReadAllText(
                @"C:\Users\ZACH-GAMING\AppData\Local\ZachLogs\PPDictionaries\Zipcodes.txt"
            )
        );
    }
}
