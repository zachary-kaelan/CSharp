using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PPRegex
{
    class Program
    {
        static void Main(string[] args)
        {
            TimeSpan ts10 = TimeSpan.FromMilliseconds(10);
            TimeSpan ts25 = TimeSpan.FromMilliseconds(25);
            TimeSpan ts50 = TimeSpan.FromMilliseconds(50);

            Regex.CompileToAssembly(
                new RegexCompilationInfo[] {

                    // ~~~~~ FILENAME ADVANCED ~~~~~
                    new RegexCompilationInfo(
                        @"^(?<Advanced>a?)(?<Type>[^ ]+) [A-Za-z]",
                        RegexOptions.None,
                        "FilenameInfoType",
                        "PPRGX.FilenameAdvanced",
                        true, ts10
                    ), new RegexCompilationInfo(
                        @"^aINPC - (?<FirstName>[a-z]+) (?:(?<MiddleInitial>[a-z]\.?) )?(?:& (?<SpouseName>[a-z]+) )?(?<LastName>[a-z]+) - (?<Address>[^,]+), (?<City>[^,]+), (?<State>[a-z]{2}) (?<Zip>[\d-]+)(?: - (?<VTID>\d*), (?<InvoiceID>\d*), \$?(?<Balance>[\d.]*))?",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, 
                        "INPC", "PPRGX.FilenameAdvanced", 
                        true, ts25 
                    ), new RegexCompilationInfo(
                        @"^aSA - (?<FirstName>[a-z]+) (?:(?<MiddleInitial>[a-z]\.?) )?(?:& (?<SpouseName>[a-z]+) )?(?<LastName>[a-z]+) - (?<Address>[^,]+), (?<City>[^,]+), (?<State>[a-z]{2}) (?<Zip>[\d-]+)(?: - (?<Schedule>[^,]*), \$?(?<Subtotal>[\d.]*), (?<TaxRate>[\d.]*))?",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                        "SA", "PPRGX.FilenameAdvanced",
                        true, ts25
                    ), new RegexCompilationInfo(
                        @"^aSA2 - (?<FirstName>[a-z]+) (?:(?<MiddleInitial>[a-z]\.?) )?(?:& (?<SpouseName>[a-z]+) )?(?<LastName>[a-z]+) - (?<Address>[^,]+), (?<City>[^,]+), (?<State>[a-z]{2}) (?<Zip>[\d-]+)(?: - (?<Schedule>[^,]*), \$?(?<Total>[\d.]*), (?<Email>[^,]*), (?<Phone>\d*))?",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
                        "SA2",
                        "PPRGX.FilenameAdvanced",
                        true, ts25
                    ),

                    // ~~~~~ PDF EXTRACTION ~~~~~
                    new RegexCompilationInfo(
                        @"\$(?<Price>[\d]+\.[\d]+)",
                        RegexOptions.None,
                        "SA2_Schedule_Prices",
                        "PPRGX.SA",
                        true, ts50
                    ), new RegexCompilationInfo(
                        @"(?<Month>[A-Z]{3}) (?:17|18)",
                        RegexOptions.None,
                        "SA2_Schedule_Months",
                        "PPRGX.SA",
                        true, ts50
                    ),

                    // ~~~~~ PESTPAC INTERNAL ~~~~~
                    new RegexCompilationInfo(
                        "^\t{8}<td>\n\t{8}<input (?:[^ ]+ ){2}value=\"(?<SessionUserID>[^\"]+)\">\n\t{8}<input (?:[^ ]+ ){2}value=\"(?<SessionID>[^\"]+)\">(?:\n[^\n]+){2}\n\t{8}<td>(?<Name>[^<]+)(?:<[^<]+){3}<td>(?<LastActivity>[^<]+)<" + @"\/td>[^>]+>(?<NumLicenses>\d*)",
                        RegexOptions.Multiline,
                        "LicenseUsers",
                        "PPRGX.PPI",
                        true, ts50
                    )
                }, new AssemblyName("PPRGX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")
            );
        }
    }
}
