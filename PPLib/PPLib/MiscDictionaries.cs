using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;
using ZachLib;
using PestPac.Model;

namespace PPLib
{
    public static class MiscDictionaries
    {
        /*public static readonly SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, ZipCode>>> ZipCodes = JSON.Deserialize<SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, ZipCode>>>>(
            File.ReadAllText(
                @"C:\Users\ZACH-GAMING\AppData\Local\ZachLogs\PPDictionaries\Zipcodes.txt"
            )
        );*/


        private static readonly Lazy<SortedDictionary<string, string>> _address_abbrvs = new Lazy<SortedDictionary<string, string>>(
            () => new SortedDictionary<string, string>(
                File.ReadAllLines(
                    @"E:\Insight Program Files\Address Abbreviations.txt"
                ).Select(a => a.Split(',')).GroupBy(
                    kv => kv[1],
                    kv => kv[0],
                    (k, g) => new KeyValuePair<string, string>(
                        k, g.GetByMin(a => a.Length)
                    )
                ).ToDictionary()
            )
        );
        public static SortedDictionary<string, string> ADDRESS_ABBRVS { get => _address_abbrvs.Value; }

        private static readonly Lazy<SortedDictionary<string, string>> _state_abbrvs = new Lazy<SortedDictionary<string, string>>(
            () => new SortedDictionary<string, string>(Utils.LoadDictionary(@"E:\Insight Program Files\State Abbreivations.txt"))
        );
        public static SortedDictionary<string, string> STATE_ABBRVS { get => _state_abbrvs.Value; }

        private static readonly Lazy<SortedDictionary<string, string>> _service_descs = new Lazy<SortedDictionary<string, string>>(
            () => new SortedDictionary<string, string>(
                Utils.LoadJSON<ServiceModel[]>(
                    @"E:\Insight Program Files\PPInternal\Services.txt"
                ).ToDictionary(s => s.Code, s => s.Description)
            )
        );

        public static SortedDictionary<string, string> SERVICE_DESCS { get => _service_descs.Value; }

        public static readonly SortedDictionary<string, string> VT_TO_PP = new SortedDictionary<string, string>(Utils.LoadDictionary(@"E:\Insight Program Files\VT to PP.txt"));
    }
}
