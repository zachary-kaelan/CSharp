using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PPRegex
{
    class Program
    {
        public const string NAMES_PAT = @"(?<FirstName>[a-z]+) (?:(?<MiddleInitial>[a-z]\.?) )?(?:& (?<SpouseName>[a-z]+) )?(?<LastName>[a-z]+)";
        public const string ADDRESS_LINES_PAT = @"(?<Address>[^,]+), (?<City>[^,]+), (?<State>[a-z]{2}) (?<Zip>[\d-]+)";

        static void Main(string[] args)
        {
            TimeSpan ts10 = TimeSpan.FromMilliseconds(10);
            TimeSpan ts25 = TimeSpan.FromMilliseconds(25);
            TimeSpan ts50 = TimeSpan.FromMilliseconds(50);

            AssemblyName name = new AssemblyName("PPRGX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c072c81d02f16673");
            name.KeyPair = new StrongNameKeyPair(File.OpenRead(@"C:\Users\ZACH-GAMING\Source\StrongKeys\PPRGX.snk"));

            Regex.CompileToAssembly(
                new RegexCompilationInfo[] {
                    // ~~~~~ COMPONENTS ~~~~
                    new RegexCompilationInfo(
                        NAMES_PAT,
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                        "Names", "RGX.Components",
                        true, ts10
                    ),
                    new RegexCompilationInfo(
                        ADDRESS_LINES_PAT,
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                        "AddressInfo", "RGX.Components",
                        true, ts10
                    ),

                    // ~~~~~ FILENAME ADVANCED ~~~~~
                    new RegexCompilationInfo(
                        @"^(?<Advanced>a?)(?<Type>[^ ]+) [A-Za-z]",
                        RegexOptions.None,
                        "FilenameInfoType",
                        "RGX.FilenameAdvanced",
                        true, ts10
                    ), new RegexCompilationInfo(
                        @"^aINPC - " + NAMES_PAT + " - " + ADDRESS_LINES_PAT + @"(?: - (?<VTID>\d*), (?<InvoiceID>\d*), \$?(?<Balance>[\d.]*))?",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, 
                        "INPC", "RGX.FilenameAdvanced", 
                        true, ts25 
                    ), new RegexCompilationInfo(
                        @"^aSA - " + NAMES_PAT + " - " + ADDRESS_LINES_PAT + @"(?: - (?<Schedule>[^,]*), \$?(?<Subtotal>[\d.]*), (?<TaxRate>[\d.]*))?",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                        "SA", "RGX.FilenameAdvanced",
                        true, ts25
                    ), new RegexCompilationInfo(
                        @"^aSA2 - " + NAMES_PAT + " - " + ADDRESS_LINES_PAT + @"(?: - (?<Schedule>[^,]*), \$?(?<Total>[\d.]*), (?<Email>[^,]*), (?<Phone>\d*))?",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                        "SA2",
                        "RGX.FilenameAdvanced",
                        true, ts25
                    ),
                    new RegexCompilationInfo(
                        @"^(?<Type>[^ ]+) - " + NAMES_PAT + " - " + ADDRESS_LINES_PAT,
                        RegexOptions.IgnoreCase,
                        "Filename",
                        "RGX",
                        true,
                        ts10
                    ),

                    // ~~~~~ PDF EXTRACTION ~~~~~
                    new RegexCompilationInfo(
                        @"\$(?<Price>[\d]+\.[\d]+)",
                        RegexOptions.None,
                        "SA2_Schedule_Prices",
                        "RGX.SA",
                        true, ts50
                    ), new RegexCompilationInfo(
                        @"(?<Month>[A-Z]{3}) (?:17|18)",
                        RegexOptions.None,
                        "SA2_Schedule_Months",
                        "RGX.SA",
                        true, ts50
                    ),
                    new RegexCompilationInfo(
                        @"^.+\n0*(?<VTID>\d+) 0*(?<InvoiceID>\d+) Due\n.+\n\$(?<Balance>[^ ]+)", 
                        RegexOptions.None, 
                        "VTNumbers", 
                        "RGX.INPC", 
                        true, ts50
                    ),

                    // ~~~~~ PESTPAC INTERNAL ~~~~~
                    new RegexCompilationInfo(
                        "^\t{8}<td>\n\t{8}<input (?:[^ ]+ ){2}value=\"(?<SessionUserID>[^\"]+)\">\n\t{8}<input (?:[^ ]+ ){2}value=\"(?<SessionID>[^\"]+)\">(?:\n[^\n]+){2}\n\t{8}<td>(?<Name>[^<]+)(?:<[^<]+){3}<td>(?<LastActivity>[^<]+)<" + @"\/td>[^>]+>(?<NumLicenses>\d*)",
                        RegexOptions.Multiline,
                        "LicenseUsers",
                        "RGX.PPI",
                        true, ts50
                    ), new RegexCompilationInfo(
                        "<locationstate>(.{2})<",
                        RegexOptions.None,
                        "LocationState",
                        "RGX.PPI",
                        true, ts50
                    ),

                    // ~~~~~ ZILLOW ~~~~~
                    new RegexCompilationInfo(
                        "^ {8}data-region-id=\"([^\"]+)",
                        RegexOptions.Multiline,
                        "RegionID",
                        "RGX.Zillow",
                        true, ts25
                    )
                }, name
            );
        }
    }
}
