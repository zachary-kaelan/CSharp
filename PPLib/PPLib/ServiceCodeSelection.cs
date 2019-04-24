using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using ZachLib;

namespace PPLib
{
    public static class ServiceCodeSelection
    {
        [Flags]
        private enum Problems
        {
            None = 0,
            FleaTick = 1,
            Mole = 2,
            Vole = 4,
            Termon = 8,
            Yard = 16
        }

        private static readonly SortedDictionary<Problems, string> SERVICE_CODES = new SortedDictionary<Problems, string>()
        {
            { Problems.FleaTick, "QTPC-FLEA-TICK" },
            { Problems.FleaTick | Problems.Mole, "QTPC FT-MOLE" },
            { Problems.FleaTick | Problems.Mole | Problems.Vole, "QTPC FT-M-V" },
            { Problems.FleaTick | Problems.Mole | Problems.Vole | Problems.Termon, "QTPC-FT-M-V-TM" },
            { Problems.FleaTick | Problems.Mole | Problems.Termon, "QTPC-TM-FT-M" },
            { Problems.FleaTick | Problems.Vole, "QTPC-FT-VOLE" },
            { Problems.FleaTick | Problems.Termon, "QTPC-TM-FT" },
            { Problems.Mole, "QTPC-MOLE" },
            { Problems.Mole | Problems.Vole, "QTPC-MOLE-VOLE" },
            { Problems.Mole | Problems.Vole | Problems.Yard, "QTPC-YARD-M-V" },
            { Problems.Mole | Problems.Termon, "QTPC-TM-M" },
            { Problems.Mole | Problems.Termon | Problems.Yard, "QTPC-TM-Y-M" },
            { Problems.Mole | Problems.Yard, "QTPC-YARD-MOLE" },
            { Problems.Vole, "QTPC-VOLES" },
            { Problems.Vole | Problems.Termon, "QTPC-V-TM" },
            { Problems.Vole | Problems.Termon | Problems.Yard, "QTPC-TM-Y-V" },
            { Problems.Vole | Problems.Yard, "QTPC-YARD-VOLE" },
            { Problems.Termon, "QTPC-TERMON" },
            { Problems.Termon | Problems.Yard, "QTPC-TMORN-Y" },
            { Problems.Yard, "QTPC-YARD" },
        };

        public static bool SelectCode(string problems, string specialProblems, string serviceType, out string serviceCode)
        {
            string str = (problems + ", " + specialProblems).ToLower();
            Problems probs = Problems.None;

            if (str.Contains("flea") || str.Contains("tick"))
                probs |= Problems.FleaTick;
            else if (str.Contains("full yard granulation"))
                probs |= Problems.Yard;

            if (str.Contains("mole"))
                probs |= Problems.Mole;
            if (str.Contains("vole"))
                probs |= Problems.Vole;
            if (str.Contains("termite"))
                probs |= Problems.Termon;
            
            if (!SERVICE_CODES.TryGetValue(probs, out serviceCode))
                serviceCode = null;
            return serviceType.ToLower().Contains("mosquito") || specialProblems.Contains("mosquito");
        }


        private static ConcurrentDictionary<string, SortedDictionary<string, SortedDictionary<string, PPExportVTMatch>>> ppExportsDict = null;
        private static ConcurrentDictionary<string, SortedDictionary<string, PPExportVTMatch>> byStateDict = null;
        public static void FixServiceCodes(string vtExportsPath, string ppExportsPath)
        {
            var ppExports = Utils.LoadCSV<PPExportVTMatch>(ppExportsPath);

            var vtLessServices = new SortedDictionary<string, PPExportVTMatch>(
                ppExports.Where(s => String.IsNullOrWhiteSpace(s.VTCust)).GroupBy(
                    s => ZachRGX.PHONE.Replace(
                        String.IsNullOrWhiteSpace(s.Phone) ?
                            s.Mobile : s.Phone,
                        "$1$2$3"
                    )
                ).Where(g => g.Count() == 1 && !String.IsNullOrWhiteSpace(g.Key)).ToDictionary(
                    g => g.Key,
                    g => g.First()
                )
            );
            ppExports = ppExports.Where(s => !String.IsNullOrWhiteSpace(s.VTCust)).ToArray();

            var byState = ppExports.GroupBy(
                s => s.State.ToUpper()
            ).Where(g => !String.IsNullOrWhiteSpace(g.Key));
            ppExportsDict = new ConcurrentDictionary<string, SortedDictionary<string, SortedDictionary<string, PPExportVTMatch>>>(
                byState.ToDictionary(
                    state => state.Key,
                    state => new SortedDictionary<string, SortedDictionary<string, PPExportVTMatch>>(
                        state.GroupBy(s => s.City.ToUpper()).Where(
                            g => !String.IsNullOrWhiteSpace(g.Key)
                        ).ToDictionary(
                            city => city.Key,
                            city => new SortedDictionary<string, PPExportVTMatch>(
                                city.GroupBy(s => s.VTCust).Where(g => g.Count() == 1).Select(g => g.First()).ToDictionary(
                                    s => s.VTCust,
                                    s => s
                                )
                            )
                        )
                    )
                )
            );
            byStateDict = new ConcurrentDictionary<string, SortedDictionary<string, PPExportVTMatch>>(
                byState.ToDictionary(
                    state => state.Key,
                    state => new SortedDictionary<string, PPExportVTMatch>(
                        state.GroupBy(s => s.VTCust).Where(g => g.Count() == 1).Select(g => g.First()).ToDictionary(
                            s => s.VTCust,
                            s => s
                        )
                    )
                )
            );

            StreamWriter mosquitoWriter = new StreamWriter(@"E:\Work Programming\Insight Program Files\PPInternal\Exports\CombinedMosquitoServices.csv") { AutoFlush = true };
            CsvWriter mosquitoCsv = new CsvWriter(mosquitoWriter);
            mosquitoCsv.WriteHeader<CombinedServices>();
            mosquitoCsv.NextRecord();
            StreamWriter otherWriter = new StreamWriter(@"E:\Work Programming\Insight Program Files\PPInternal\Exports\CombinedServices.csv") { AutoFlush = true };
            CsvWriter otherCsv = new CsvWriter(otherWriter);
            otherCsv.WriteHeader<CombinedServices>();
            otherCsv.NextRecord();

            List<CombinedServices> combinedList = new List<CombinedServices>();
            var vtExports = new LoopManager<FixCodeVantageExport>(
                vtExportsPath, 
                @"E:\Work Programming\Insight Program Files\FinishedServices.txt", 
                @"E:\Work Programming\Insight Program Files\ErroredServices.txt", 
                s => s.CustomerID
            );
            vtExports.EndLoop(
                Parallel.ForEach(
                    vtExports.Updates,
                    LoopManager.PARALLEL_OPTS,
                    service =>
                    {
                        bool isMosquito = false;
                        string error = null;
                        var stateAbbrv = MiscDictionaries.STATE_ABBRVS.TryGetValue(service.StateProvince, out string abbrv) ? abbrv : service.StateProvince;
                        PPExportVTMatch ppExport = new PPExportVTMatch();
                        isMosquito = SelectCode(
                            service.Problems,
                            service.SpecialProblems,
                            service.Type,
                            out string code
                        );

                        if (String.IsNullOrWhiteSpace(code) && !isMosquito)
                        {
                            vtExports.LogUpdate(
                                service,
                                String.Format(
                                    "No matching problems: {0} - {1}\r\n\t{2}{3}", 
                                    service.CustomerID, 
                                    stateAbbrv, 
                                    service.Problems, 
                                    String.IsNullOrWhiteSpace(service.SpecialProblems) ? "" : "\r\n\t" + service.SpecialProblems
                                ), UpdateType.Nonexistant
                            );
                            return;
                        }

                        SortedDictionary<string, PPExportVTMatch> city = null;

                        if (String.IsNullOrWhiteSpace(service.City))
                            error = GetFromState(stateAbbrv, service, out ppExport);
                        else if (ppExportsDict.TryGetValue(stateAbbrv, out SortedDictionary<string, SortedDictionary<string, PPExportVTMatch>> state))
                        {
                            if (state.TryGetValue(service.City.ToUpper(), out city))
                                error = GetFromCity(city, service, out ppExport);
                            else
                            {
                                string serviceCity = service.City.ToUpper();
                                string serviceCityKey = state.Keys.FirstOrDefault(k => k.StartsWith(serviceCity));
                                if (!String.IsNullOrWhiteSpace(serviceCityKey) && state.TryGetValue(serviceCityKey, out city))
                                    error = GetFromCity(city, service, out ppExport);
                                else if (!String.IsNullOrWhiteSpace(GetFromState(stateAbbrv, service, out ppExport)))
                                    error = "City not found: " + service.City + ", " + service.StateProvince;
                            }
                        }
                        else
                            error = "State not found: " + service.StateProvince;

                        if (
                            !String.IsNullOrWhiteSpace(error) &&
                            (
                                (
                                    !String.IsNullOrWhiteSpace(service.Phone) && 
                                    vtLessServices.TryGetValue(ZachRGX.PHONE.Replace(service.Phone, "$1$2$3"), out ppExport)
                                ) 
                            )
                        )
                            error = null;

                        if (!String.IsNullOrWhiteSpace(error))
                            vtExports.LogUpdate(service, error, UpdateType.Error);
                        else
                        {
                            if (code == ppExport.Service)
                            {
                                vtExports.LogUpdate(service, ppExport.Location + " - Code already matches.");
                                return;
                            }
                            CombinedServices combined = new CombinedServices()
                            {
                                City = service.City,
                                State = stateAbbrv,
                                Street = ppExport.Address,
                                Email = String.IsNullOrWhiteSpace(ppExport.Email) ? service.PrimaryEmail : ppExport.Email,
                                Location = ppExport.Location,
                                Name = ppExport.Name,
                                Office = service.Office,
                                Phone = String.IsNullOrWhiteSpace(service.Phone) ? (String.IsNullOrWhiteSpace(ppExport.Phone) ? ppExport.Mobile : ppExport.Phone) : service.Phone,
                                Problems = service.Problems,
                                ServiceCode = code,
                                SpecialProblems = service.SpecialProblems,
                                Tags = service.Tags,
                                Type = service.Type,
                                VTCustomerID = service.CustomerID,
                                Zip = ppExport.ZipCode
                            };

                            lock (LoopManager.PARALLEL_LOCK)
                            {
                                if (isMosquito)
                                {
                                    mosquitoCsv.WriteRecord(combined);
                                    mosquitoCsv.NextRecord();
                                }

                                otherCsv.WriteRecord(combined);
                                otherCsv.NextRecord();
                                combinedList.Add(combined);
                            }
                            vtExports.LogUpdate(service, combined.Location + " - " + combined.ServiceCode);
                        }

                    }
                )
            );

            mosquitoCsv.Flush();
            mosquitoCsv.Dispose();
            mosquitoWriter.Close();
            otherCsv.Flush();
            otherCsv.Dispose();
            otherWriter.Close();
            Console.WriteLine("FILES SAVED\r\n");

            byStateDict = null;
            ppExportsDict = null;
            byState = null;
            vtLessServices = null;
            vtExports = null;
            ppExports = null;

            var combinedGroups = combinedList.GroupBy(
                s => s.ServiceCode,
                s => s.Location
            );
            string nowString = DateTime.Now.ToString("MM.dd.yyyy");
            Console.WriteLine(nowString);
            foreach (var group in combinedGroups)
            {
                var groupArray = group.Distinct().ToArray();
                Console.WriteLine("\t{0} - {1}", groupArray.Length, group.Key);
                File.WriteAllLines(
                    @"E:\Work Programming\Insight Program Files\PPInternal\ServiceCodeUpdates\" + nowString + " " + group.Key + ".csv", 
                    groupArray
                );
            }
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        private static string GetFromCity(SortedDictionary<string, PPExportVTMatch> city, FixCodeVantageExport service, out PPExportVTMatch ppExport) =>
            city.TryGetValue(service.CustomerID, out ppExport) ? null : String.Format(
                "ppExport not found: {0} - {1}, {2}",
                service.CustomerID,
                service.City,
                service.StateProvince
            );

        private static string GetFromState(string stateAbbrv, FixCodeVantageExport service, out PPExportVTMatch ppExport)
        {
            if (byStateDict.TryGetValue(stateAbbrv, out SortedDictionary<string, PPExportVTMatch> state))
            {
                if (!state.TryGetValue(service.CustomerID, out ppExport))
                    return "ppExport not found: " + service.CustomerID + " - " + service.StateProvince;
                else
                    return null;
            }
            else
            {
                ppExport = null;
                return "State not found: " + service.StateProvince;
            }
        }
    }
}
