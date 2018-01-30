using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using ZachLib;
using PostmanLib;
using PestPac.Model;
using PPLib;

namespace TaxRatesInKansas
{
    class Program
    {
        static void Main(string[] args)
        {
            var zipcodes = new SortedDictionary<string, PPLib.ZipCode>(
                MiscDictionaries.ZipCodes["KS"].SelectMany(s => s.Value).ToDictionary()
            );

            Postman.GetTaxCodes(true, out IEnumerable<TaxCode> taxcodes);
            var newCodes = taxcodes.Where(c => c.Code.StartsWith("KS") && c.Description.Contains("County")).ToArray();
            taxcodes = taxcodes.Where(t => zipcodes.ContainsKey(t.Code));

            StreamReader sr = new StreamReader(@"C:\Users\ZACH-GAMING\Downloads\Tax_Rates_in_Kansas.csv");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            var customers = cr.GetRecords<Customer>().ToArray();
            cr.Dispose();
            cr = null;
            sr.Close();
            sr = null;

            foreach(TaxCode taxcode in taxcodes)
            {
                string[] ids = customers.Where(c => c.TaxCode == taxcode.Code).Select(c => c.LocationID).ToArray();
                if (ids.Any())
                {
                    ZipCode zip = zipcodes[taxcode.Code];
                    string newCode = newCodes.Single(
                        c => c.Code.Contains(zip.County.ToUpper()) &&
                            (c.Code.Contains(zip.PrimaryCity.ToUpper()) ||
                             zip.AcceptableCities.Any(city => c.Code.Contains(city.ToUpper())))
                    ).Code;

                    foreach(string id in ids)
                    {
                        Postman.Patch(id, new PatchOperation("replace", "/TaxCode", newCode));
                    }

                    if (PPWebLib.PestPac.CheckIfTaxCodeInUse(taxcode.TaxCodeID.ToString()))
                        continue;
                }

                PPWebLib.PestPac.DeleteTaxCode(taxcode.TaxCodeID.ToString());
            }

            Console.WriteLine("DONE");
            Console.ReadLine();

            /*Stopwatch timer = new Stopwatch();
            timer.Start();
            StreamReader sr = new StreamReader(@"C:\Users\ZACH-GAMING\Downloads\zip_code_database.csv");
            CsvReader cr = new CsvReader(sr);
            ZipCodeComplex[] zipcodesTemp = cr.GetRecords<ZipCodeComplex>().ToArray();
            cr.Dispose();
            cr = null;
            sr.Close();
            sr = null;
            timer.Stop();
            long setup = timer.ElapsedMilliseconds;
            timer.Restart();

            var zipcodes = new SortedDictionary<string, SortedDictionary<string, SortedDictionary<string, ZipCode>>>(
                zipcodesTemp.Where(
                    z => z.decommissioned == 0 && 
                         z.irs_estimated_population_2015 > 0
                ).GroupBy(
                    z => z.state
                ).ToDictionary(
                    s => s.Key,
                    s => new SortedDictionary<string, SortedDictionary<string, ZipCode>>(
                        s.GroupBy(
                            z => z.county,
                            z => new ZipCode()
                            {
                                AcceptableCities = z.acceptable_cities.Split(new string[] { ", "}, StringSplitOptions.RemoveEmptyEntries),
                                County = z.county,
                                PrimaryCity = z.primary_city,
                                Type = z.type,
                                UnacceptableCities = z.unacceptable_cities.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries),
                                Zip = z.zip
                            }
                        ).ToDictionary(
                            c => c.Key,
                            c => new SortedDictionary<string, ZipCode>(
                                c.ToDictionary(
                                    z => z.Zip,
                                    z => z
                                )
                            )
                        )
                    )
                )
            );
            timer.Stop();
            long processing = timer.ElapsedMilliseconds;
            timer.Restart();
            zipcodes.SaveAs(@"C:\Users\ZACH-GAMING\AppData\Local\ZachLogs\PPDictionaries\Zipcodes.txt");
            timer.Stop();
            Console.WriteLine("Setup: " + setup.ToString());
            Console.WriteLine("Processing: " + processing.ToString());
            Console.WriteLine("IO: " + timer.ElapsedMilliseconds.ToString());

            Console.WriteLine("DONE");
            Console.ReadLine();*/

            /*PPWebLib.PestPac.UpdateStateTaxCodes(@"E:\Temp\TaxRatesFinal.csv", "Kansas");
            Console.WriteLine("DONE");
            Console.ReadLine();*/

            /*StreamReader reader = new StreamReader(@"E:\Temp\LocationsTaxRates.csv");
            CsvReader csvReader = new CsvReader(reader, Utils.csvConfig);
            Customer[] customers = csvReader.GetRecords<Customer>().ToArray();
            csvReader.Dispose();
            csvReader = null;
            reader.Close();
            reader = null;

            foreach(var customer in customers)
            {
                if (!Postman.Patch(
                    customer.LocationID,
                    new PatchOperation("replace", "/taxcode", "KS" + customer.County.Substring(0, 3) + customer.City.Substring(0, 3)),
                    new PatchOperation("replace", "/county", customer.County)
                ))
                    Console.WriteLine(customer.ToString());
            }

            Console.WriteLine("DONE");
            Console.ReadLine();

            var counties = customers.GroupBy(
                c => new Customer() {
                    City = c.City.ToUpper(),
                    County = c.County.ToUpper(),
                    TaxRate = c.TaxRate,
                    ZipCode = c.ZipCode
                },
                c => c.LocationID,
                (k, g) => new KeyValuePair<Customer, string[]>(k, g.OrderBy().ToArray())
            ).OrderBy(
                c => c.Key.County
            ).ThenBy(
                c => c.Key.City
            ).ThenBy(
                c => c.Key.ZipCode
            ).ToArray();

            Console.WriteLine(
                String.Join(
                    "\r\n", 
                    counties.Select(
                        c => c.Key.ToString()
                    )
                )
            );

            StreamWriter writer = new StreamWriter(@"E:\Temp\TaxRatesInKansas.csv");
            CsvWriter csvWriter = new CsvWriter(writer);
            csvWriter.WriteRecords<Customer>(counties.Select(kv => kv.Key));
            csvWriter.Dispose();
            csvWriter = null;
            writer.Close();
            writer = null;

            Console.ReadLine();*/
        }
    }

    public struct Customer
    {
        public string LocationID { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string ZipCode { get; set; }
        public string TaxCode { get; set; }
        public double TaxRate { get; set; }

        public override string ToString()
        {
            return County + " - " + City + " - " + ZipCode + " - " + TaxRate.ToString();
        }
    }

    public struct ZipCodeComplex
    {
        public string zip { get; set; }
        public string type { get; set; }
        public int decommissioned { get; set; }
        public string primary_city { get; set; }
        public string acceptable_cities { get; set; }
        public string unacceptable_cities { get; set; }
        public string state { get; set; }
        public string county { get; set; }
        public string timezone { get; set; }
        public string area_codes { get; set; }
        public string world_region { get; set; }
        public string country { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int irs_estimated_population_2015 { get; set; }
    }
}
