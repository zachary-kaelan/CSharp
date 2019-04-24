using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using CsvHelper;
using HtmlAgilityPack;
using NeuralNetworksLib;
using NeuralNetworksLib.NetworkModels;
using PostmanLib;
using PestPac.Model;
using PPLib;
using RestSharp;
using VTWebLib;
using VTWebLib.Models;
using ZachLib;
using ZachLib.HTTP;
using ZachLib.Logging;
using ZachLib.Statistics;
using ZillowLib;

namespace TaxRatesInKansas
{
    class Program
    {
        public static KeyValuePair<string, string>[] FIELDS = new Dictionary<string, string>()
        {
            { "ACTIVE", "Active" },
            { "AUTOBILL", "AutoBill" },
            { "CALLNOTIFY_1", "CallNotify1" },
            { "EMAILNOTIFY_1", "EmailNotify1" },
            { "NOTIFICATIONDAYS_1", "NotificationDays1" },
            { "TEXTNOTIFY_1", "" }
        }.ToArray();

        public static Dictionary<string, string> SERVICE_CODES = new Dictionary<string, string>()
        {
            { "INPC", "Initial Pest Control Service" },
            { "QTPC", "Quarterly Pest Control Service" }
        };

        public static readonly object PARALLEL_LOCK = new object();
        public static readonly ParallelOptions PARALLEL_OPTS = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 8
        };

        public static int updatesCount = 0;
        public static string[] completed = null;
        public static StreamWriter finishedWriter = null;
        public static StreamWriter erroredWriter = null;
        public static ConcurrentBag<string> finished = null;
        public static ConcurrentBag<string> errored = null;
        public static int updatesIter = 0;
        public static int erroredCount = 0;

        public const string PATH_TEMP = @"E:\Work Programming\Insight Temp Files\";
        public const string PATH_LOGGING = @"E:\Work Programming\Insight Program Files\PPInternal\Logging\";

        static void Main(string[] args)
        {
            var proc = Process.GetCurrentProcess();
            proc.PriorityClass = ProcessPriorityClass.High;
            //Console.WriteLine(ServicePointManager.DefaultConnectionLimit);
            ServicePointManager.DefaultConnectionLimit = 1000000;

            var employees = Postman.GetEmployees(ActiveStatus.Yes, EmployeeType.All).Where(e => e.Username.StartsWith("18"));
            Parallel.ForEach(
                employees,
                new ParallelOptions() { MaxDegreeOfParallelism = 16 },
                employee =>
                {
                    try
                    {
                        if (!Postman.DeactivateEmployee(employee.EmployeeID.Value.ToString()))
                        {
                            lock(PARALLEL_LOCK)
                            {
                                Console.WriteLine("Request failed for {0}-{1}.", employee.Username, employee.EmployeeID);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("{0}: {1}\r\n\t{2}-{3}", e.GetType().Name, e.Message, employee.Username, employee.EmployeeID);
                    }
                }
            );

            Console.WriteLine("FINISHED");
            Console.ReadLine();

            var setupSkipMonthsLoop = new LoopManager<SetupSkipMonths>(
                PATH_TEMP + "MOSQ Services with Skip Months.csv",
                PATH_LOGGING + "SetupSkipMonthsFinished.txt",
                PATH_LOGGING + "SetupSkipMonthsErrored.txt",
                s => s.Location.ToString(),
                true
            );
            var skipFields = Enumerable.Range(1, 12).Select(m => "Skip" + m.ToString()).ToArray();
            setupSkipMonthsLoop.EndLoop(
                Parallel.ForEach(
                    setupSkipMonthsLoop.Updates,
                    new ParallelOptions() { MaxDegreeOfParallelism = 16 },
                    setup =>
                    {
                        try
                        {
                            int locID = Postman.GetLocationIDFromCode(setup.Location);
                            var setupID = Postman.GetLocationServiceSetup(locID, s => s.ServiceCode == "MONTHLY MOSQ").SetupID.Value.ToString();
                            PPWebLib.PestPac.EditServiceSetup(setupID, null, skipFields);
                            setupSkipMonthsLoop.LogUpdate(setup.Location.ToString(), setupID);
                        }
                        catch (Exception e)
                        {
                            setupSkipMonthsLoop.LogUpdate(setup.Location.ToString(), e.Message, UpdateType.Error);
                        }
                    }
                )
            );

            var invoicePDFsLoop = new LoopManager<CSVKeyValuePair<int, string>>(
                PATH_TEMP + "InvoicesToDownload.csv",
                PATH_LOGGING + "InvoicePDFsFinished.txt",
                PATH_LOGGING + "InvoicePDFsErrored.txt",
                i => i.Key
            );
            invoicePDFsLoop.EndLoop(
                Parallel.ForEach(
                    invoicePDFsLoop.Updates,
                    new ParallelOptions() { MaxDegreeOfParallelism = 16 },
                    invoice =>
                    {
                        string invoiceID = "NO ID";
                        try
                        {
                            invoiceID = Postman.GetInvoiceIDFromNumber(
                                Postman.GetLocationIDFromCode(invoice.Key),
                                invoice.Value
                            );
                            Postman.SaveInvoicePDF(invoiceID, @"E:\Work Programming\Insight Program Files\Invoices\" + invoice.Key + " - " + invoice.Value + ".pdf");
                            invoicePDFsLoop.LogUpdate(invoice.Key, invoiceID);
                        }
                        catch (Exception e)
                        {
                            invoicePDFsLoop.LogUpdate(invoice.Key, invoice.Value + ", " + e.Message, invoiceID == "NO ID" ? UpdateType.Nonexistant : UpdateType.Error);
                        }
                    }
                )
            );

            ConcurrentBag<IDictionary<string, string>> setupDicts = new ConcurrentBag<IDictionary<string, string>>();
            var serviceSetupsTestLoop = new LoopManager<ServiceSetupTest>(
                @"E:\Work Programming\Insight Temp Files\ServiceSetups\Test_Service_Setups.csv",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\ServiceSetupTestFinished.txt",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\ServiceSetupTestErrored.txt",
                t => t.LocationCode
            );
            Stopwatch loopTotalTime = Stopwatch.StartNew();
            serviceSetupsTestLoop.EndLoop(
                Parallel.ForEach(
                    serviceSetupsTestLoop.Updates.GroupBy(s => s.LocationCode).Take(32).Select(g => g.First()),
                    new ParallelOptions() {MaxDegreeOfParallelism = 16},
                    s =>
                    {
                        try
                        {
                            setupDicts.Add(
                                PPWebLib.PestPac.EditServiceSetup(
                                    s.SetupID, 
                                    new Dictionary<string, string>()
                                )
                            );

                            serviceSetupsTestLoop.LogUpdate(s, null);
                        }
                        catch (Exception e)
                        {
                            serviceSetupsTestLoop.LogUpdate(s, s.LocationCode + " met with error: " + e.Message, UpdateType.Error);
                        }
                    }
                ), false
            );
            loopTotalTime.Stop();

            var dictsComparison = new DictionaryListComparisonResults(setupDicts);
            Console.WriteLine();
            Console.WriteLine(dictsComparison.ToString());
            Console.WriteLine();
            Console.WriteLine("Ticks: " + loopTotalTime.ElapsedTicks.ToString());
            Console.WriteLine("MS: " + loopTotalTime.ElapsedMilliseconds.ToString());
            Console.WriteLine();
            Console.WriteLine(PPWebLib.PestPac.GetTimesString());
            Console.ReadLine();

            var timerTemp = PPWebLib.PestPac.TIMER;
            
            LogManager.Start(true);
            SpinWait.SpinUntil(() => Postman.IsInitialized);
            Postman.GetToken(DateTime.Now);

            var timeRangesLoop = new LoopManager<TimeRangeChangesModel>(
                @"C:\Users\ZACH-GAMING\Downloads\Time Range Updates Oct.18.2018.csv",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\TimerangeChangesFinished.txt",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\TimerangeChangesErrored.txt",
                t => t.Location
            );
            timeRangesLoop.EndLoop(
                Parallel.ForEach(
                    timeRangesLoop.Updates,
                    LoopManager.PARALLEL_OPTS,
                    l =>
                    {
                        try
                        {
                            int id = Postman.GetLocationIDFromCode(l.Location);
                            var serviceSetups = Postman.GetLocationServiceSetups(id).Where(s => !String.IsNullOrWhiteSpace(s.TimeRange));
                            var serviceOrder = Postman.GetServiceOrderByNumber(id, l.Order);

                            foreach(var serviceSetup in serviceSetups)
                            {
                                PPWebLib.PestPac.EditServiceSetup(serviceSetup.SetupID.Value.ToString(), new Dictionary<string, string>() { { "TimeRange", "" } });
                            }

                            if (serviceOrder == null || !serviceOrder.OrderID.HasValue)
                                timeRangesLoop.LogUpdate(l.Location, l.Location + " has no matching orders.", UpdateType.Nonexistant);
                            else
                            {
                                Postman.Patch(
                                    EntityType.ServiceOrders,
                                    serviceOrder.OrderID.Value.ToString(),
                                    new PatchOperation(
                                        "replace",
                                        "/constraints/latesttime",
                                        "07:00 PM"
                                    )
                                );
                            }

                            timeRangesLoop.LogUpdate(l.Location, null);
                        }
                        catch (Exception e)
                        {
                            timeRangesLoop.LogUpdate(l.Location, l.Location + " met with error: " + e.Message, UpdateType.Error);
                        }
                    }
                )
            );

            int[] locationCodes = new int[]
            {
                10179,
                10268,
                10551,
                10620,
                10640
            };

            foreach(var locationCode in locationCodes)
            {
                Console.Write(locationCode);
                int id = Postman.GetLocationIDFromCode(locationCode);
                Console.WriteLine(" - " + id.ToString());
                var setup = Postman.GetLocationLatestServiceSetup(id);
                var setupID = setup.SetupID.Value.ToString();
                Console.WriteLine("\tSetup ID: " + setupID);
                var serviceOrders = Postman.GetServiceOrders(id, setupID);
                Console.Write("\tEditing Setup... ");
                PPWebLib.PestPac.EditServiceSetup(setupID, new Dictionary<string, string>() { { "TimeRange", "" } });
                Console.WriteLine("Done");
                foreach(var order in serviceOrders)
                {
                    var orderID = order.OrderID.Value.ToString();
                    Console.Write("\t\tEditing Order, ID " + orderID + " &  # " + order.OrderNumber + "... ");
                    Postman.Patch(
                        EntityType.ServiceOrders, 
                        orderID, 
                        new PatchOperation(
                            "replace", 
                            "/constraints/latesttime", 
                            "07:00 PM"
                        )/*, new PatchOperation(
                            "replace", 
                            "/constraints/earliesttime", 
                            "07:00 AM"
                        )*/
                    );
                    Console.WriteLine("Done");
                }
            }
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            var setupBranchChangeLoop = new LoopManager<SetupBranchChangeModel>(
                @"C:\Users\ZACH-GAMING\Downloads\Service_Setup_Branch_Mismatches (1).csv",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\ServiceBranchChangeFinished.txt",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\ServiceBranchChangeErrored.txt",
                s => s.SetupID
            );
            setupBranchChangeLoop.EndLoop(
                Parallel.ForEach(
                    setupBranchChangeLoop.Updates,
                    LoopManager.PARALLEL_OPTS,
                    s =>
                    {
                        string display = String.Format("{0} - {1} to {2}", s.SetupID, s.SetupBranch, s.LocationBranch);
                        try
                        {
                            PPWebLib.PestPac.EditServiceSetup(
                                s.SetupID,
                                new Dictionary<string, string>()
                                {
                                    { "BranchID", s.LocationBranchID },
                                    { "SetupBranchID", s.LocationBranchID }
                                }
                            );

                            setupBranchChangeLoop.LogUpdate(
                                s.SetupID, display
                            );
                        }
                        catch (Exception e)
                        {
                            setupBranchChangeLoop.LogUpdate(
                                s.SetupID, display + " - " + e.Message, UpdateType.Error
                            );
                        }
                    }
                )
            );

            GetActiveZipCodeStats();

            var mtx = Matrix.Deserialize(@"E:\Work Programming\Insight Program Files\Neural Network\AreaStats2.mtx");
            int rowsFixed = 0;
            for (int i = 0; i < mtx.NumRows; ++i)
            {
                var row = mtx[i];
                bool changed = false;
                if (row[0] <= 0)
                {
                    row[8] = -1;
                    row[9] = -1;
                    row[10] = -1;
                    changed = true;
                }
                else
                {
                    if (row[3] <= 0)
                    {
                        row[8] = -1;
                        changed |= true;
                    }
                    if (row[7] <= 0)
                    {
                        row[9] = -1;
                        changed |= true;
                    }
                    if (row[5] <= 0)
                    {
                        row[10] = -1;
                        changed |= true;
                    }
                }
                if (changed)
                {
                    mtx[i] = row;
                    ++rowsFixed;
                }
            }
            mtx.Serialize(@"E:\Work Programming\Insight Program Files\Neural Network", "AreaStats2");

            //var matrix = Matrix.LoadFile(@"E:\Work Programming\Insight Program Files\Neural Network\AreaStats.mtx");
            
            Console.WriteLine("FINISHED");
            Console.WriteLine("Rows Fixed: " + rowsFixed);
            Console.ReadLine();

            NeuralNetwork network = new NeuralNetwork(0.9, 0.25, 0.3, true, new int[] { 11, 7, 3 });

            /*var correlations = new MultiCorrelationHelper(
                Matrix.LoadFile(
                    @"E:\Work Programming\Insight Temp Files\Lists\AreaStats.mtx"
                ), 
                Matrix.LoadFile(
                    @"E:\Work Programming\Insight Temp Files\Lists\OutputsMatrix.mtx"
                )
            );
            Console.WriteLine("Calculated correlations.");
            File.WriteAllText(
                @"E:\Work Programming\Insight Temp Files\Lists\AreaCorrelations.csv",
                correlations.ToCSV(
                    new string[]
                    {
                        "Population",
                        "MedianAge",
                        "GraduationRate",
                        "HousingUnits",
                        "MedianIncome",
                        "ForeignBorn",
                        "PovertyRate",
                        "Veterans",
                        "PeoplePerHousingUnit",
                        "VeteransPercentage",
                        "ForeignersPercentage"
                    },
                    new string[]
                    {
                        "Active",
                        "Count",
                        "Months"
                    },
                    5
                )
            );
            Console.WriteLine("Correlations saved.");
            Console.ReadLine();*/

            var areaStatsTemp = Utils.LoadCSV<AreaStats>(
                @"E:\Work Programming\Insight Temp Files\Lists\AreaStats2.csv"
            ).ToDictionary(a => a.AreaName);
            List<double[]> inputVectors = new List<double[]>();
            List<double[]> outputVectors = new List<double[]>();
            var ppAreaStats = Utils.LoadCSV<AreaData>(@"E:\Work Programming\Insight Temp Files\Lists\Area_Output_Fields.csv");
            var missingZipsArr = File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\Lists\MissingZipcodes.txt");
            //StreamWriter missingZips = new StreamWriter(@"E:\Work Programming\Insight Temp Files\Lists\MissingZipcodes.txt", true) { AutoFlush = true };
            foreach(var area in ppAreaStats)
            {
                if (missingZipsArr.Contains(area.ZipCode))
                    continue;
                AreaStats stats = null;
                double[] inputVector = null;
                if (!areaStatsTemp.TryGetValue(area.ZipCode, out stats))
                {
                    Console.WriteLine("Missing Zipcode - " + area.ZipCode);
                    Console.WriteLine("\tCount: \t" + area.Count);
                    Console.WriteLine("\tActive: \t" + area.Active);
                    Console.WriteLine("\tMonths: \t" + area.Months);
                    Console.Write("Status . . . ");

                    stats = FactFinder.GetStats(area.ZipCode.PadLeft(5, '0'));
                    if (stats == null)
                        Console.WriteLine("FAILED");
                    else
                    {
                        Console.WriteLine("SUCCESS");
                        inputVector = stats.ToVector();
                    }
                }
                else
                    inputVector = stats.ToVector();

                if (inputVector != null)
                {
                    if (inputVector.Any(e => e < 0))


                    inputVectors.Add(inputVector);
                    var outputVector = area.ToVector();
                    outputVector[1] /= stats.Population;
                    outputVectors.Add(outputVector);
                }
                /*else
                    missingZips.WriteLine(area.ZipCode);*/
            }
            Console.WriteLine();

            Console.WriteLine("NUMBER OF RECORDS: " + inputVectors.Count);
            var areaStats = new Matrix(inputVectors.ToArray());
            //var outputsMatrix = Matrix.LoadFile(@"E:\Work Programming\Insight Temp Files\Lists\OutputsMatrix.mtx");   
            var outputsMatrix = new Matrix(outputVectors.ToArray());
            areaStats.Serialize(@"E:\Work Programming\Insight Temp Files\Lists", "AreaStats");
            outputsMatrix.Serialize(@"E:\Work Programming\Insight Temp Files\Lists", "OutputsMatrix");
            inputVectors = null;
            outputVectors = null;
            Console.WriteLine();

            var PCA = areaStats.PCA();
            foreach(var potentialLValue in PCA.PotentialLValues)
            {
                Console.WriteLine(potentialLValue);
            }
            Console.WriteLine();
            Console.Write("Pick an L value: ");
            var transformed = PCA.GetTransformedData(Convert.ToInt32(Console.ReadLine()));

            transformed.Value.Serialize(@"E:\Work Programming\Insight Temp Files\Lists", "PCA-Transformed Area Data", 4);
            Console.WriteLine("Serialized.");
            Console.WriteLine();
            areaStats = null;

            var correlations2 = new MultiCorrelationHelper(transformed.Value, outputsMatrix);
            Console.WriteLine("Calculated correlations.");
            File.WriteAllText(
                @"E:\Work Programming\Insight Temp Files\Lists\AreaCorrelations.csv",
                correlations2.ToCSV(
                    new string[]
                    {
                        "Population",
                        "MedianAge",
                        "GraduationRate",
                        "HousingUnits",
                        "MedianIncome",
                        "ForeignBorn",
                        "PovertyRate",
                        "Veterans"
                    }.Where((n, i) => Array.BinarySearch(transformed.Key, i) >= 0).ToArray(),
                    new string[]
                    {
                        "Active",
                        "Count",
                        "Months"
                    },
                    5
                )
            );
            Console.WriteLine("Correlations saved.");

            //missingZips.Close();
            Console.ReadLine();

            

            var billtos = new LoopManager<BillToPaymentModel>(
                @"C:\Users\ZACH-GAMING\Downloads\APAY_Accounts_with_Balance.csv",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\BillToPaymentsFinished.txt",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\BillToPaymentsErrored.txt",
                b => b.BillToID
            );
            var now = DateTime.Now;
            billtos.EndLoop(
                Parallel.ForEach(
                    billtos.Updates.Skip(125).Take(100), LoopManager.PARALLEL_OPTS, billto =>
                    {
                        CardOnFileListModel[] accounts = null;
                        CardOnFileListModel account = null;
                        PaymentProcessorTransactionModel transaction = null;
                        try
                        {
                            
                            accounts = Postman.GetPaymentAccounts(billto.BillToID);
                            if (accounts.Length == 0)
                                billtos.LogUpdate(billto, billto.BillToID + " - No payment accounts found", UpdateType.Nonexistant);
                            else
                            {
                                if (accounts.Count() > 1)
                                {
                                    accounts = accounts.Where(a => a.ExpirationDate.Value > now).ToArray();
                                    if (accounts.Count() > 1)
                                        accounts = accounts.Where(a => a.AutoBilledServices.Any()).ToArray();
                                }
                                    
                                account = accounts.Count() == 1 ? accounts[0] : accounts.First(a => a.IsPrimary.Value);
                                transaction = Postman.Charge(account.CardID.Value, billto.Balance.Substring(1), false);
                                billtos.LogUpdate(
                                    billto,
                                    billto.BillToID + " - " +
                                    (transaction.Payment != null ? transaction.Payment.BatchNumber + " - " : "") +
                                    (transaction.TransactionResponse.TransactionResult.HasValue ?
                                        transaction.TransactionResponse.TransactionResult.Value.ToString() + ": " : "") +
                                        transaction.TransactionResponse.Message
                                );
                            }
                        }
                        catch(Exception e)
                        {
                            billtos.LogUpdate(billto, billto.BillToID + " - " + e.Message, UpdateType.Error);
                        }
                    }
                )
            );

            //VantageTracker.TryExportAllBranches(true);
            //Console.ReadLine();
            //var failedExportsRGX = new Regex(@"^(?<Key>\d+) :=: (?<Value>(?:(?!\d+)[^\r\n]+[\r\n]+)+)", RegexOptions.Multiline);

            //VantageTracker.FixVTTechPPUsernames();
            //Console.ReadLine();

            var setups = new LoopManager<Setup>(
                @"C:\Users\ZACH-GAMING\Downloads\SS_Timerange_Updates_by_Timerange (3).csv",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\OrdersTimerangeFinished.txt",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\OrdersTimerangeErrored.txt",
                s => s.SetupID
            );
            setups.EndLoop(
                Parallel.ForEach(
                    setups.Updates, LoopManager.PARALLEL_OPTS, setup =>
                    {
                        try
                        {
                            var setupModel = Postman.GetServiceSetup(setup.SetupID);
                            if (setupModel.Constraints.LatestTime.Value.TimeOfDay.Hours == 7)
                            {
                                PPWebLib.PestPac.EditServiceSetup(
                                    setup.SetupID, new Dictionary<string, string>()
                                    {
                                        { "RouteOptTime1Beg", "7:00" },
                                        { "RouteOptTime1BegAmPm", "AM" },
                                        { "RouteOptTime1End", "7:00" },
                                        { "RouteOptTime1EndAmPm", "PM" },
                                        { "TimeRange", "7:00A-7:00P" },
                                        { "AnytimeStartTime", "07:00" },
                                        { "AnytimeStartAmPm", "AM" },
                                        { "AnytimeEndTime", "07:00" },
                                        { "AnytimeEndAmPm", "PM" }
                                    }
                                );

                                var serviceOrders = Postman.GetServiceOrders(
                                    setupModel.LocationID.Value, 
                                    setup.SetupID
                                ).Where(
                                    o => !o.TimeRange.EndsWith("P") && 
                                         !o.TimeRange.EndsWith("PM")
                                );

                                foreach (var order in serviceOrders)
                                {
                                    Postman.Patch(
                                        EntityType.ServiceOrders,
                                        order.OrderID.Value.ToString(),
                                        new PatchOperation("replace", "/timerange", "7:00A-7:00P"),
                                        new PatchOperation("replace", "/constraints/earliesttime", "1899-12-30T07:00:00"),
                                        new PatchOperation("replace", "/constraints/latesttime", "1899-12-30T19:00:00")
                                    );
                                }

                                setups.LogUpdate(setup, setup.SetupID + " - Success");
                            }
                        }
                        catch (Exception err)
                        {
                            setups.LogUpdate(setup, setup.SetupID + " - " + err.Message, UpdateType.Error);
                        }
                    }
                )
            );

            /*LoopManager.EndLoop(
                Parallel.ForEach(
                    setups, LoopManager.PARALLEL_OPTS, setup =>
                    {
                        try
                        {
                            PPWebLib.PestPac.EditServiceSetup(
                                setup.SetupID, new Dictionary<string, string>()
                                {
                                    { "RouteOptTime1Beg", "7:00" },
                                    { "RouteOptTime1BegAmPm", "AM" },
                                    { "RouteOptTime1End", "7:00" },
                                    { "RouteOptTime1EndAmPm", "PM" },
                                    { "TimeRange", "7:00A-7:00P" },
                                    { "AnytimeStartTime", "07:00" },
                                    { "AnytimeStartAmPm", "AM" },
                                    { "AnytimeEndTime", "07:00" },
                                    { "AnytimeEndAmPm", "PM" }
                                }
                            );
                            LoopManager.LogUpdate(setup.SetupID + " - Success");
                        }
                        catch(Exception err)
                        {
                            LoopManager.LogUpdate(setup.SetupID + " - " + err.Message, UpdateType.Error);
                        }
                    }
                )
            );*/

            ServiceCodeSelection.FixServiceCodes(
                @"C:\Users\ZACH-GAMING\Downloads\search_results (4).csv", 
                @"C:\Users\ZACH-GAMING\Downloads\report (48).csv"
            );

            /*var files = Directory.GetFiles(VantageTracker.PATH_FAILED_EXPORTS).Where(f => !f.EndsWith("AllBranches.txt"));
            var allBranches = files.SelectMany(f => Utils.LoadJSON<VTExportCust[]>(f));
            allBranches.SaveAs(VantageTracker.PATH_FAILED_EXPORTS + "AllBranches.txt");
            var errorMessages = allBranches.GroupBy(e => e.ErrorMessage, e => e).OrderByDescending(e => e.Count());
            foreach(var errorMessage in errorMessages)
            {
                Console.WriteLine(errorMessage.Count() + " - " + errorMessage.Key);
                foreach(var example in errorMessage.Take(3))
                {
                    Console.WriteLine("\t * {0} - {1} - {2}", example.Name, example.Office, example.ID);
                    Console.WriteLine();
                }
            }
            Console.ReadLine();*/

            //var proc = Process.GetCurrentProcess();
            //proc.PriorityClass = ProcessPriorityClass.High;
            
            VantageTracker.Login(true);

            /*var vtExportLines = File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\VantagePPExportIDs.txt");
            foreach(var line in vtExportLines)
            {
                if (!Char.IsDigit(line[0]))
                {
                    VantageTracker.SwitchBranch(line);
                    Console.WriteLine();
                    Console.WriteLine(line);
                }
                else
                {
                    var split = line.Split(new string[] { " :=: " }, StringSplitOptions.None);
                    Console.Write("\t(\"{0}\", {1})", split[1], split[0]);
                    var status = VantageTracker.AddCustomerToPPExport(split[0]);
                    Console.WriteLine(" - " + (status.Successful ? "Successful" : status.Message));
                }
            }
            Console.WriteLine("FINISHED");
            Console.ReadLine();*/

            /*foreach(var branch in VantageTracker.BRANCH_IDS.Value)
            {
                Console.WriteLine(branch.Value);
                VantageTracker.SwitchBranch(branch.Key);
            }

            var addExports = Utils.LoadCSV<VTAddExport>(@"C:\Users\ZACH-GAMING\Downloads\Customers to be Added to PP Export 6.14.2018.csv", Encoding.UTF8);
            var exportGroups = addExports.GroupBy(e => e.Branch);
            foreach(var exportGroup in exportGroups)
            {
                string branchName = VantageTracker.BRANCH_IDS.Value[VantageTracker.BRANCH_ID_DICTIONARY.Value[exportGroup.Key]];

                List<VTAddExport> branchFailed = new List<VTAddExport>();
                Console.WriteLine(exportGroup.Count() + " - " + branchName);
                VantageTracker.SwitchBranch(exportGroup.Key);
                var prevExports = VantageTracker.GetPestpacExports();
                prevExports.SaveAs(VantageTracker.PATH_ALL_EXPORTS + branchName + ".txt");
                var prevExportsNames = prevExports.Select(e => e.Name).ToArray();
                Array.Sort(prevExportsNames);
                var prevExportsIDs = prevExports.Select(e => e.URLID).ToArray();
                Array.Sort(prevExportsIDs);
                var exports = exportGroup.Where(e => Array.BinarySearch(prevExportsNames, e.Name) < 0);

                foreach(var export in exportGroup)
                {
                    Console.Write("\tSearching for \"{0}\" ... ", export.Name);
                    KeyValuePair<string, string> match = new KeyValuePair<string, string>();
                    SortedDictionary<string, string> nameResults = null;
                    if (!TryGetResults(export.Name, out nameResults))
                    {
                        branchFailed.Add(export);
                        Console.WriteLine("Failed on 'Name'.");
                        continue;
                    }

                    var names = export.Name.Split(new char[] { ' ', '/', '\\', '&' });
                    int numNames = names.Length;
                    int namesPointer = 0;
                    while (nameResults.Count == 0 && namesPointer < numNames)
                    {
                        if (!TryGetResults(export.FName, out nameResults))
                        {
                            branchFailed.Add(export);
                            Console.WriteLine("Failed on name {0}.", namesPointer);
                            break;
                        }
                        ++namesPointer;
                    }

                    if (nameResults.Count == 0)
                    {
                        branchFailed.Add(export);
                        Console.WriteLine("No results.");
                        continue;
                    }

                    string urlID = null;
                    if (nameResults.Count == 1)
                        match = nameResults.First();
                    else if (nameResults.TryGetValue(export.Name, out urlID))
                        match = new KeyValuePair<string, string>(export.Name, urlID);
                    else
                    {
                        var noSymbols = ZachRGX.SYMBOLS.Replace(export.Name, "");
                        if (nameResults.TryGetValue(noSymbols, out urlID))
                            match = new KeyValuePair<string, string>(export.Name, urlID);
                        else
                        {
                            branchFailed.Add(export);
                            Console.WriteLine("Failed to find a match.");
                            continue;
                        }
                    }

                    if (Array.BinarySearch(prevExportsIDs, match.Value) >= 0)
                        continue;
                    Console.WriteLine("(\"{0}\", {1})", match.Key, match.Value);
                }

                branchFailed.SaveAs(VantageTracker.PATH_EXPORT_ERROR_MESSAGES + branchName + ".txt", Jil.Options.PrettyPrint, Encoding.UTF8);
            }

            Console.WriteLine("FINISHED");
            Console.ReadLine();*/

            /*var voleServices = Utils.LoadCSV<CombinedServices>(@"C:\Users\ZACH-GAMING\Downloads\VoleServices.csv");
            StreamWriter volesWriter = new StreamWriter(@"E:\Work Programming\Insight Temp Files\ServiceCodes.csv");
            foreach(var service in voleServices)
            {
                bool isMosq = ServiceCodeSelection.SelectCode(service.Problems, service.SpecialProblems, service.Type, out string code);
                volesWriter.WriteLine(service.Location + "," + code);
            }
            volesWriter.Close();
            volesWriter = null;
            Console.WriteLine("FINISHED");
            Console.ReadLine();*/

            /*var postedOrders = LoopManager.StartLoop<PostedOrder>(
                @"E:\Work Programming\Insight Temp Files\PostServiceOrders.csv",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\FinishedPostedOrders.txt",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\ErroredPostedOrders.txt",
                o => o.OrderID
            );

            LoopManager.EndLoop(
                Parallel.ForEach(
                    postedOrders, PARALLEL_OPTS, postedOrder =>
                    {
                        try
                        {
                            if (Postman.PostServiceOrder(postedOrder.OrderID, postedOrder.Tech, 303779))
                                LoopManager.LogUpdate(postedOrder.OrderID + " - true", UpdateType.Finished);
                            else
                                LoopManager.LogUpdate(postedOrder.OrderID + " - false", UpdateType.Error);
                        }
                        catch
                        {
                            LoopManager.LogUpdate(postedOrder.OrderID + " - ERROR");
                        }
                    }
                )
            );

            return;

            for (int i = 0; i < postedOrders.Length; i += 100)
            {
                int end = Math.Min(postedOrders.Length, i + 100);
                Console.WriteLine("Posting Orders {0} to {1}.", i, end);
                for (int j = 0; j < end; ++j)
                {
                    var postedOrder = postedOrders[j];
                    Console.WriteLine("\t{0} - {1}", postedOrder.OrderID, Postman.PostServiceOrder(postedOrder.OrderID, postedOrder.Tech, 303779));
                }
                Console.WriteLine();
            }
            Console.WriteLine("FINISHED");
            Console.ReadLine();*/

            VantageTracker.Login();

            VantageTracker.TryExportAllBranches();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            //var tableRows = VantageTracker.GetFailedPestpacExports();
            //File.WriteAllLines(@"E:\Work Programming\Insight Temp Files\VantagePPExports.txt", tableRows);
            //var tableRows = File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\VantagePPExports.txt");
            //Console.WriteLine("FINISHED");
            //Console.ReadLine();

            var stateAbbrvs = new ConcurrentDictionary<string, string>(Utils.LoadDictionary(@"E:\Work Programming\Insight Program Files\State Abbreivations.txt"));

            var services = Utils.LoadCSV<SetupExportVTMatch>(@"E:\Work Programming\Insight Temp Files\Exports\PestpacServicesExport.csv");
            var vtLessServices = new SortedDictionary<string, SetupExportVTMatch>(
                services.Where(s => String.IsNullOrWhiteSpace(s.VTCust)).ToDictionary(
                    s => s.ZipCode.Substring(0, 5),
                    s => s
                )
            );
            services = services.Where(s => !String.IsNullOrWhiteSpace(s.VTCust)).ToArray();
            var byState = services.GroupBy(
                s => s.State.ToUpper()
            );
            var servicesDict = new ConcurrentDictionary<string, SortedDictionary<string, SortedDictionary<string, SetupExportVTMatch>>>(
                byState.ToDictionary(
                    state => state.Key,
                    state => new SortedDictionary<string, SortedDictionary<string, SetupExportVTMatch>>(
                        state.GroupBy(
                            s => s.City.ToUpper()
                        ).Where(c => !String.IsNullOrWhiteSpace(c.Key)).ToDictionary(
                            city => city.Key,
                            city => new SortedDictionary<string, SetupExportVTMatch>(
                                city.ToDictionary(
                                    s => s.VTCust,
                                    s => s
                                )
                            )
                        )
                    )
                )
            );
            var byStateDict = new ConcurrentDictionary<string, SortedDictionary<string, SetupExportVTMatch>>(
                byState.ToDictionary(
                    state => state.Key,
                    state => new SortedDictionary<string, SetupExportVTMatch>(
                        state.GroupBy(s => s.VTCust).Where(g => g.Count() == 1).Select(g => g.First()).ToDictionary(
                            s => s.VTCust,
                            s => s
                        )
                    )
                )
            );
            
            StreamWriter mosquitoWriter = new StreamWriter(@"E:\Work Programming\Insight Temp Files\Exports\CombinedMosquitoServices.csv") { AutoFlush = true };
            CsvWriter mosquitoCsv = new CsvWriter(mosquitoWriter);
            mosquitoCsv.WriteHeader<CombinedServices>();
            mosquitoCsv.NextRecord();
            StreamWriter otherWriter = new StreamWriter(@"E:\Work Programming\Insight Temp Files\Exports\CombinedServices.csv") { AutoFlush = true };
            CsvWriter otherCsv = new CsvWriter(otherWriter);
            otherCsv.WriteHeader<CombinedServices>();
            otherCsv.NextRecord();

            //var vtServices = StartLoop<VTServiceExport>(@"E:\Work Programming\Insight Temp Files\Exports\VantageServicesExports2.csv", @"E:\Work Programming\Insight Program Files\FinishedServices.txt", @"E:\Work Programming\Insight Program Files\ErroredServices.txt", s => s.CustomerID);
            /*foreach (var service in vtServices)
            {
                if (service.CustomerID != "602115")
                    continue;
                bool isMosquito = ServiceCodeSelection.SelectCode(service.Problems, service.SpecialProblems, service.Type, out string code);
                if (String.IsNullOrWhiteSpace(code) && !isMosquito)
                {
                    isMosquito = ServiceCodeSelection.SelectCode(service.Problems, service.SpecialProblems, service.Type, out code);
                    continue;
                }
            }*/
            /*EndLoop(
                Parallel.ForEach(vtServices, PARALLEL_OPTS, service =>
                {
                    bool isMosquito = false;
                    string error = null;
                    var stateAbbrv = stateAbbrvs[service.State];
                    SetupExportVTMatch customer = new SetupExportVTMatch();
                    isMosquito = ServiceCodeSelection.SelectCode(service.Problems, service.SpecialProblems, service.Type, out string code);

                    if (String.IsNullOrWhiteSpace(code) && !isMosquito)
                    {
                        LogUpdate(String.Format("No matching problems: {0} - {1}\r\n\t{2}{3}", service.CustomerID, stateAbbrvs, service.Problems, String.IsNullOrWhiteSpace(service.SpecialProblems) ? "" : "\r\n\t" + service.SpecialProblems), UpdateType.Nonexistant);
                        return;
                    }

                    if (String.IsNullOrWhiteSpace(service.City))
                    {
                        if (byStateDict.TryGetValue(stateAbbrv, out SortedDictionary<string, SetupExportVTMatch> state))
                        {
                            if (!state.TryGetValue(service.CustomerID, out customer))
                                error = "Customer not found: " + service.CustomerID + " - " + service.State;
                        }
                        else
                            error = "State not found: " + service.State;
                    }
                    else if (servicesDict.TryGetValue(stateAbbrv, out SortedDictionary<string, SortedDictionary<string, SetupExportVTMatch>> state))
                    {
                        if (state.TryGetValue(service.City.ToUpper(), out SortedDictionary<string, SetupExportVTMatch> city))
                        {
                            if (!city.TryGetValue(service.CustomerID, out customer))
                                error = String.Format("Customer not found: {0} - {1}, {2}", service.CustomerID, service.City, service.State);
                        }
                        else
                            error = "City not found: " + service.City + ", " + service.State;
                    }
                    else
                        error = "State not found: " + service.State;

                    if (!String.IsNullOrWhiteSpace(error) && vtLessServices.TryGetValue(service.Zip, out customer))
                        error = null;

                    if (!String.IsNullOrWhiteSpace(error))
                        LogUpdate(error, UpdateType.Error);
                    else
                    {
                        CombinedServices combined = new CombinedServices()
                        {
                            City = service.City,
                            State = stateAbbrv,
                            Street = customer.Address,
                            Email = String.IsNullOrWhiteSpace(customer.Email) ? service.PrimaryEmail : customer.Email,
                            Location = customer.Location,
                            Name = customer.Name,
                            Office = service.Office,
                            Phone = String.IsNullOrWhiteSpace(service.Phone) ? (String.IsNullOrWhiteSpace(customer.Phone) ? customer.Mobile : customer.Phone) : service.Phone,
                            Problems = service.Problems,
                            ServiceCode = code,
                            SpecialProblems = service.SpecialProblems,
                            Tags = service.Tags,
                            Type = service.Type,
                            VTCustomerID = service.CustomerID,
                            Zip = customer.ZipCode
                        };

                        lock (PARALLEL_LOCK)
                        {
                            if (isMosquito)
                            {
                                mosquitoCsv.WriteRecord(combined);
                                mosquitoCsv.NextRecord();
                            }
                            otherCsv.WriteRecord(combined);
                            otherCsv.NextRecord();
                        }
                        LogUpdate(combined.Location + " - " + combined.ServiceCode);
                    }
                })
            );
            mosquitoCsv.Flush();
            mosquitoCsv.Dispose();
            mosquitoWriter.Close();
            otherCsv.Flush();
            otherCsv.Dispose();
            otherWriter.Close();
            Console.WriteLine("FILES SAVED");
            Console.ReadLine();*/

            var words = new Regex(@"[^\w ]", RegexOptions.Compiled, TimeSpan.FromMilliseconds(25));
            var locations = Utils.LoadCSV<Customer>(@"E:\Work Programming\Insight Temp Files\Minnesota\Locations.csv");
            var countyCityCombos = locations.GroupBy(
                l => new KeyValuePair<string, string>(
                    words.Replace(l.City.ToUpper(), ""),
                    words.Replace(l.County.ToUpper(), "")
                ),
                l => l.LocationID
            ).OrderBy(c => c.Key.Value).ThenBy(c => c.Key.Key);
            List<CSVKeyValuePair<string, string>> combos = new List<CSVKeyValuePair<string, string>>();
            foreach(var combo in countyCityCombos)
            {
                Console.WriteLine(combo.Key.Key + " - " + combo.Key.Value + " COUNTY - " + combo.Count());
                combos.Add(new CSVKeyValuePair<string, string>(combo.Key.Value, combo.Key.Key));
            }
            combos.SaveCSV(@"E:\Work Programming\Insight Temp Files\Minnesota\CombinedRates.csv");
            Console.ReadLine();

            var cities = locations.Select(
                l => words.Replace(l.City.ToUpper(), "")
            ).Distinct().ToArray();

            /*var taxUpdates = StartLoop(@"E:\Work Programming\Insight Temp Files\Exports\TaxCodeUpdates.csv", @"E:\Work Programming\Insight Program Files\PPInternal\Logging\FinishedTaxCodes.txt", @"E:\Work Programming\Insight Program Files\PPInternal\Logging\ErroredTaxCodes.txt");
            EndLoop(
                Parallel.ForEach(
                    taxUpdates, PARALLEL_OPTS, update =>
                    {
                        try
                        {
                            LogUpdate(
                                update.Key,
                                Postman.Patch(
                                    update.Key, 
                                    new PatchOperation(
                                        "replace", 
                                        "/taxcode", 
                                        update.Value
                                    )
                                ) ? UpdateType.Finished : UpdateType.Error
                            );
                                
                        }
                        catch
                        {
                            LogUpdate(update.Key, UpdateType.Error);
                        }
                    }
                )
            );*/

            /*VantageTracker.Login();
            var serviceTemplates = Utils.LoadCSV<VTService>(@"E:\Work Programming\Insight Temp Files\Exports\ServiceSetupTypes.csv");
            foreach (var branch in VantageTracker.ID_BRANCH_DICTIONARY.Value.Keys)
            {
                if (branch != "113" && branch != "599")
                {
                    Console.WriteLine(branch);
                    VantageTracker.SwitchBranch(branch);
                    var services = VantageTracker.GetVTServices();
                    foreach(var service in serviceTemplates)
                    {
                        string serviceType = service.serviceType == "1133" ? "Comprehensive Residential Treatment" : service.serviceSetupType;
                        Console.Write("\t" + serviceType);
                        if (!services.Any(s => s.serviceType == serviceType))
                        {
                            var obj = VantageTracker.AddVTServiceType(service);
                            if (obj.GetType() == typeof(VTFunctionStatus) && ((VTFunctionStatus)obj).Successful)
                                Console.WriteLine("\t - Added");
                            else
                                Console.WriteLine("\t - Error");
                                
                        }
                        else
                            Console.WriteLine("\t - Already Exists");
                            
                    }
                }
            }
            Console.WriteLine("FINISHED");
            Console.ReadLine();*/

            /*
            var descUpdates = StartLoop<InvoiceDescUpdate>(
                @"E:\Work Programming\Insight Temp Files\Exports\Invoices_No_Description.csv", 
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\DescriptionsFinished.txt", 
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\DescriptionsErrored.txt",
                u => u.LocationCode
            );

            EndLoop(
                Parallel.ForEach(
                    descUpdates, PARALLEL_OPTS, update =>
                    {
                        try
                        {
                            var invoiceParams = PPWebLib.PestPac.GetParameters(
                                "invoice/detail.asp", 
                                "InvoiceID", 
                                update.InvoiceID
                            ).ToDictionary();
                            bool succeeded = PPWebLib.PestPac.EditEntity(
                                "invoice/detail.asp",
                                invoiceParams,
                                new Dictionary<string, string>()
                                {
                                    { "Description1", "Initial Pest Control Service" }
                                },
                                out IRestResponse invoiceResponse
                            );

                            if (succeeded)
                                LogUpdate(update.LocationCode);
                            else
                                LogUpdate(update.LocationCode, UpdateType.Error);
                        }
                        catch
                        {
                            LogUpdate(update.LocationCode, UpdateType.Error);
                        }
                    }
                )
            );*/

            var finished = File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\ServiceOrders\Finished.txt").Concat(File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\ServiceOrders\Broken.txt"));
            //var zillowData = Utils.LoadCSV<ZillowData>(@"E:\Work Programming\Insight Temp Files\ZillowInvoices.csv").Where(z => !String.IsNullOrWhiteSpace(z.SetupID) && !finished.Contains(z.SetupID));
            var orders = Utils.LoadCSV<BrokenServiceOrder>(@"E:\Work Programming\Insight Temp Files\ServiceOrders\ServiceOrders.csv").Where(o => !finished.Contains(o.OrderID.ToString())).ToArray();
            int count = orders.Count();
            /*var testCode = "83045";
            Console.WriteLine("CODE: " + testCode);
            string testID = Postman.GetLocationIDFromCode(testCode);
            Console.WriteLine("ID: " + testID);
            Postman.GetLatestServiceOrder(testID, out ServiceOrderListModel testOrder);
            Console.WriteLine("ORDER ID: " + testOrder.OrderID.Value.ToString());
            PPWebLib.PestPac.EditServiceOrder(
                testOrder.OrderID.Value.ToString(),
                orderFieldsDict
            );
            Console.WriteLine("FINISHED");
            Console.ReadLine();*/

            var process = Process.GetCurrentProcess();
            process.PriorityClass = ProcessPriorityClass.AboveNormal;
            object bagLock = new object();
            StreamWriter finishedOrders = new StreamWriter(@"E:\Work Programming\Insight Temp Files\ServiceOrders\Finished.txt", true) { AutoFlush = true };
            StreamWriter brokenOrders = new StreamWriter(@"E:\Work Programming\Insight Temp Files\ServiceOrders\Broken.txt", true) { AutoFlush = true };
            int iter = 0;
            ConcurrentBag<int> bag = new ConcurrentBag<int>();
            ConcurrentBag<int> bag2 = new ConcurrentBag<int>();
            var loop = Parallel.ForEach(
                orders,
                PARALLEL_OPTS,
                order => {

                    Dictionary<string, string> dictFix = new Dictionary<string, string>();
                    if (!order.RouteOpStartEligibleTime.StartsWith("7:00"))
                        dictFix.Add("RouteOptTime1Beg", "7:00");
                    if (!order.RouteOpStartEligibleTime.EndsWith("AM"))
                        dictFix.Add("RouteOptTime1BegAmPm", "AM");
                    if (!order.RouteOpEndEligibleTime.StartsWith("7:00"))
                        dictFix.Add("RouteOptTime1End", "7:00");
                    if (!order.RouteOpEndEligibleTime.EndsWith("PM"))
                        dictFix.Add("RouteOptTime1EndAmPm", "PM");
                    if (order.TimeRange != "7:00A-7:00P")
                        dictFix.Add("TimeRange", "7:00A-7:00P");
                    if (dictFix.Any())
                    {
                        if (!PPWebLib.PestPac.EditServiceOrder(order.OrderID.ToString(), dictFix, out IRestResponse response))
                            bag2.Add(order.OrderID);
                        else
                            bag.Add(order.OrderID);
                    }
                    else
                        bag.Add(order.OrderID);

                    /*string locationID = Postman.GetLocationIDFromCode(model.Or);
                    if (Postman.GetLatestServiceOrder(locationID, out ServiceOrderListModel model) && model.OrderID.HasValue)
                        PPWebLib.PestPac.EditServiceOrder(
                            model.OrderID.Value.ToString(),
                            orderFieldsDict.MakeSerializedCopy()
                        );
                    else
                        bag2.Add(setupID);*/
                    lock (bagLock)
                    {
                        ++iter;
                        if (iter % 25 == 0)
                        {
                            string text = "";
                            text = String.Join("\r\n", bag);
                            bag = new ConcurrentBag<int>();
                            Console.WriteLine(text);
                            finishedOrders.WriteLine(text);

                            if (bag2.Any())
                            {
                                text = String.Join("\r\n\t", bag2);
                                Console.Write("~~ BROKEN:\r\n\t");
                                Console.WriteLine(text);
                                brokenOrders.WriteLine(String.Join("\r\n", bag2));
                                bag2 = new ConcurrentBag<int>();
                            }

                            Console.WriteLine(" -=~~~~~=- ");
                            Console.WriteLine();
                            Console.WriteLine("{0} down, {1} to go...", iter, count - iter);
                            Console.WriteLine();
                            Console.WriteLine(" -=~~~~~=- ");
                        }
                    }
                });
            SpinWait.SpinUntil(() => loop.IsCompleted);
            string text2 = String.Join("\r\n", bag);
            bag = new ConcurrentBag<int>();
            Console.WriteLine(text2);
            finishedOrders.WriteLine(text2);
            finishedOrders.Close();
            if (bag2.Any())
            {
                text2 = String.Join("\r\n\t", bag2);
                Console.Write("~~ BROKEN:\r\n\t");
                Console.WriteLine(text2);
                brokenOrders.WriteLine(String.Join("\r\n", bag2));
                bag2 = new ConcurrentBag<int>();
            }
            Console.WriteLine("Finished");
            Console.ReadLine();

            var boxWhiskerDataset = File.ReadAllLines(@"C:\Users\ZACH-GAMING\Downloads\BoxWhiskerTest.txt").Select(n => Convert.ToDouble(n)).ToArray();
            var boxWhiskerPlot = new BoxWhiskerPlot(boxWhiskerDataset);

            var zipcodes = File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\Lists\ZipCodes.csv");
            //List<AreaStats> stats = new List<AreaStats>(zipcodes.Length);
            StreamWriter writer = new StreamWriter(@"E:\Work Programming\Insight Temp Files\Lists\AreaStats.csv") { AutoFlush = true };
            CsvWriter csvWriter = new CsvWriter(writer);
            csvWriter.WriteHeader<AreaStats>();
            foreach (var zipcode in zipcodes)
            {
                csvWriter.NextRecord();
                csvWriter.WriteRecord(FactFinder.GetStats(zipcode.PadLeft(5, '0')));
                Console.WriteLine(zipcode);
            }
            csvWriter.Dispose();
            writer.Close();
            csvWriter = null;
            writer = null;
            Console.WriteLine("FINISHED");
            Console.ReadLine();


            

            
            /*var cityAbbrvs = File.ReadAllLines(
                @"C:\Users\ZACH-GAMING\Downloads\Cities (4).csv"
            ).Select(
                c => ZachRGX.SYMBOLS.Replace(c, "")
            ).Distinct().Select(
                c => new KeyValuePair<string, string>(c, c.AbbreviatePhrase(5))
            ).GroupBy(
                c => c.Value,
                c => c.Key
            ).Where(
                g => g.Count() > 1
            ).Select(
                g => g.Key + ":\t" + String.Join(" - ", g)
            );

            foreach(string abbrv in cityAbbrvs)
            {
                Console.WriteLine(abbrv);
            }

            Console.WriteLine();
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            var inter = NetworkInterface.GetAllNetworkInterfaces().Where(
                n => n.OperationalStatus == OperationalStatus.Up
            ).ToDictionary(
                n => n.Description,
                n => n.GetIPv4Statistics()
            );*/

            

            
            /*StreamWriter writerTest = new StreamWriter(@"E:\Work Programming\Insight Temp Files\AbbrvsTest.csv");
            writerTest.WriteLine("County,CountyAbbrv,JoinedCountyAbbrv,CountySyllables,City,CityAbbrv,JoinedCityAbbrv,CitySyllables");
            string format = "{0},{1},{2},{3},{4},{5},{6},{7}";
            foreach(var taxrate in taxrates)
            {
                string cityAbbrv = taxrate.City.AbbreviatePhrase(5);
                string joinedAbbrv = taxrate.City.Replace(" ", "").Abbreviate(5);
                if (problems.Contains(taxrate.City))
                    Console.WriteLine(taxrate.City + " - " + cityAbbrv + " - " + joinedAbbrv);
                writerTest.WriteLine(
                    String.Format(
                        format, 
                        taxrate.County,
                        taxrate.County.AbbreviatePhrase(3),
                        taxrate.County.Replace(" ", "").Abbreviate(3),
                        taxrate.County.CountSyllables(true),
                        taxrate.City,
                        cityAbbrv,
                        joinedAbbrv,
                        taxrate.City.CountSyllables(true)
                    )
                );
            }
            writerTest.Flush();
            writerTest.Close();*/
            Console.WriteLine("DONE");
            Console.ReadLine();

            PPWebLib.PestPac.UpdateStateTaxCodes(@"E:\Work Programming\Insight Temp Files\Ohio\Final Tax Rates 2.csv", "Ohio");
            Console.WriteLine("DONE");
            Console.ReadLine();

            Utils.LoadCSV<Customer>(@"C:\Users\ZACH-GAMING\Downloads\Tax_Rates_in_Kansas (1).csv").GroupBy(
                c => new KeyValuePair<string, string>(c.County.ToUpper().Replace(" COUNTY", ""), c.City.ToUpper()),
                c => c.LocationID,
                (k, g) => new CSVKeyValuePair<string, string>(k.Key, k.Value)
            ).OrderBy(
                kv => kv.Key
            ).ThenBy(
                kv => kv.Value
            ).SaveCSV(@"E:\Work Programming\Insight Temp Files\Ohio\City County Combos.csv");
            Console.WriteLine("DONE");
            Console.ReadLine();
            
        }

        private static void GetActiveCityStats()
        {
            var activeCities = File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\Lists\VirginiaWilmingtonActiveCities.txt");
            var citiesCount = activeCities.Length;
            var citiesCompleted = new CityHomeValueAndIncome[citiesCount];
            for (int i = 0; i < citiesCount; ++i)
            {
                var city = activeCities[i];
                int divider = city.LastIndexOf(' ');
                var income = FactFinder.GetMedianIncome(city);
                var regionID = ZillowWeb.GetRegionID(city);
                var timeseries = ZillowWeb.GetTimeseries(regionID);
                int homevalue = -1;
                try
                {
                    var timestamp = DateTime.Parse(timeseries.update_date).ToUnixTimestamp(true);
                    homevalue = timeseries.data.GetByMin(p => Math.Abs(p.x - timestamp)).y;
                }
                catch
                {

                }
                citiesCompleted[i] = new CityHomeValueAndIncome()
                {
                    City = city.Substring(0, divider),
                    State = city.Substring(divider + 1),
                    HomeValue = homevalue,
                    Income = income
                };
                Console.WriteLine(city + ", " + homevalue.ToString() + ", " + income.ToString());
            }
            citiesCompleted.SaveCSV(@"E:\Work Programming\Insight Temp Files\Lists\Virginia & Wilmington - Median Income & Home Value.csv");
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        private static void GetActiveZipCodeStats()
        {
            var activeZips = File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\Lists\FTL Zipcodes.csv");
            var zipsCount = activeZips.Length;
            var zipsCompleted = new List<ZipHomeValueAndIncome>();
            for (int i = 0; i < zipsCount; ++i)
            {
                var zip = activeZips[i];
                int divider = zip.LastIndexOf(' ');
                var income = FactFinder.GetMedianIncome(zip);
                var regionID = ZillowWeb.GetRegionID(zip);
                var timeseries = ZillowWeb.GetTimeseries(regionID);
                int homevalue = -1;
                try
                {
                    var timestamp = DateTime.Parse(timeseries.update_date).ToUnixTimestamp(true);
                    homevalue = timeseries.data.GetByMin(p => Math.Abs(p.x - timestamp)).y;
                }
                catch
                {

                }
                zipsCompleted.Add(
                    new ZipHomeValueAndIncome()
                    {
                        Zip = zip,
                        HomeValue = homevalue,
                        Income = income
                    }
                );
                Console.WriteLine(zip + ", " + homevalue.ToString() + ", " + income.ToString());
            }
            zipsCompleted.SaveCSV(@"E:\Work Programming\Insight Temp Files\Lists\FTL - Median Income & Home Value.csv");
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        private static void ServiceOrderCodeUpdates()
        {
            var serviceCodeUpdates = new LoopManager<CombinedServices>(
                @"E:\Work Programming\Insight Temp Files\Exports\CombinedServices.csv", 
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\FinishedServiceCodes.txt", 
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\ErroredServiceCodes.txt", 
                s => s.Location
            );
            serviceCodeUpdates.EndLoop(
                Parallel.ForEach(
                    serviceCodeUpdates.Updates, PARALLEL_OPTS, update =>
                    {
                        var locationID = Postman.GetLocationIDFromCode(update.Location);
                        var nonINPCs = Postman.GetServiceOrders(locationID).Where(o => !o.ServiceCode.StartsWith("INPC"));
                        List<string> setupIDs = new List<string>();

                        foreach (var order in nonINPCs)
                        {
                            Postman.Patch(EntityType.ServiceOrders, order.OrderID.Value.ToString(), new PatchOperation("replace", "/ServiceCode", update.ServiceCode));
                            setupIDs.Add(order.SetupID.Value.ToString());
                        }

                        foreach(var setup in setupIDs.Distinct())
                        {
                            PPWebLib.PestPac.EditServiceSetup(setup, new Dictionary<string, string>());
                        }
                    }
                )
            );
        }

        private static void LogManager_OnEntry(object sender, string logName, string entry)
        {
            Console.WriteLine(entry);
        }

        public static readonly Dictionary<string, string> orderFieldsDict = new Dictionary<string, string>()
        {
            { "RouteOptTime1Beg", "07:00" },
            { "RouteOptTime1BegAmPm", "AM" },
            { "RouteOptTime1End", "07:00" },
            { "RouteOptTime1EndAmPm", "PM" },
            { "TimeRange", "7:00A-7:00P" }
        };
        public static readonly Dictionary<string, string> fieldsDict = new Dictionary<string, string>()
        {
            { "AnytimeStartTime", "07:00" },
            { "AnytimeStartAmPm", "AM" },
            { "AnytimeEndTime", "07:00" },
            { "AnytimeEndAmPm", "PM" },
            { "RouteOptTime1Beg", "07:00" },
            { "RouteOptTime1BegAmPm", "AM" },
            { "RouteOptTime2BegAmPm", "AM" },
            { "RouteOptTime1EndAmPm", "PM" },
            { "RouteOptTime1End", "07:00" },
            { "RouteOptTime2EndAmPm", "PM" },
            { "TimeRange", "7:00A-7:00P" }
        };

        public static readonly SortedDictionary<string, Dictionary<string, KeyValuePair<string, string>>> setupsFields = new SortedDictionary<string, Dictionary<string, KeyValuePair<string, string>>>(
            Utils.LoadCSV<ChangeLogField>(
                @"E:\Work Programming\Insight Temp Files\Change_Log_-_Mass_Export.csv"
            ).GroupBy(
                f => f.SetupID,
                (k, g) =>
                {
                    return new KeyValuePair<string, Dictionary<string, KeyValuePair<string, string>>>(
                        k,
                        g.GroupBy(
                            f => f.FieldName.TrimMiddle().ToUpper(),
                            (fKey, fGrp) =>
                            {
                                var ordered = fGrp.OrderByDescending(f => f.UpdateDateTime).TakeWhile(f => f.UpdateUser == "ZAC.JOHNSO");
                                if (ordered.Count() == 0)
                                    return new KeyValuePair<string, KeyValuePair<string, string>>(null, new KeyValuePair<string, string>());
                                var last = ordered.Last();
                                return new KeyValuePair<string, KeyValuePair<string, string>>(
                                    fKey, new KeyValuePair<string, string>(
                                        last.NewValue.ToUpper(),
                                        last.OldValue.ToUpper()
                                    )
                                );
                            }
                        ).Where(f => f.Key != null).ToDictionary()
                    );
                }
            ).Where(s => s.Value.Any()).ToDictionary()
        );

        public static readonly Tuple<string, string, string, string>[] SetupFieldValueRanges = new Tuple<string, string, string, string>[]
        {
            new Tuple<string, string, string, string>("ACTIVE", "Active", "0", "FALSE"),
            new Tuple<string, string, string, string>("AUTOBILL", "AutoBill", "0", "FALSE"),
            new Tuple<string, string, string, string>("AUTOBILLTYPE", "AutoBillType", null, null),
            new Tuple<string, string, string, string>("SERVICEDETAILEDDESCRIPTION1", "Description1", null, null),

            new Tuple<string, string, string, string>("CALLNOTIFY_1", "CallNotify1", "1", "TRUE"),
            new Tuple<string, string, string, string>("EMAILNOTIFY_1", "EmailNotify1", "1", "TRUE"),
            new Tuple<string, string, string, string>("PRINTNOTIFY_1", "PrintNotify1", "1", "TRUE"),
            new Tuple<string, string, string, string>("TEXTNOTIFY_1", "TextNotify1", "1", "TRUE"),
            new Tuple<string, string, string, string>("WHOTONOTIFY_1", "WhoToNotify1", null, null)
        };
        public static readonly List<string> RouteOptDays = new List<string>()
        {
            "",
            "MON",
            "TUE",
            "WED",
            "THU",
            "FRI",
            "SAT",
            "SUN"
        };

        public (string, string) UpdateTimeRange(int locationCode)
        {
            int locationID = Postman.GetLocationIDFromCode(locationCode);
            var setup = Postman.GetLocationLatestServiceSetup(locationID);
            var setupID = setup.SetupID.Value.ToString();

            Dictionary<string, string> setupDict = new Dictionary<string, string>(fieldsDict);
            List<string> removal = new List<string>();
            if (setupsFields.TryGetValue(setupID, out Dictionary<string, KeyValuePair<string, string>> setupFields))
            {
                KeyValuePair<string, string> fieldValues = new KeyValuePair<string, string>();
                if (setupFields.TryGetValue("ROUTEOPTINCLUDEDAYS", out fieldValues))
                {
                    setupFields.Remove("ROUTEOPTINCLUDEDAYS");
                    for (int i = 1; i < 8; ++i)
                    {
                        string day = RouteOptDays[i];
                        string name = "RouteOptIncludeDay" + i;
                        if (fieldValues.Value.Contains(day))
                            setupDict.Add(name, "1");
                        else
                            removal.Add(name);

                    }
                }

                for (int i = 1; i < 13; ++i)
                {
                    string name = "Skip" + i;
                    if (setupFields.TryGetValue(name.ToUpper(), out fieldValues))
                    {
                        
                    }
                }

                foreach (var setupField in SetupFieldValueRanges)
                {
                    if (setupFields.TryGetValue(setupField.Item1, out fieldValues))
                    {
                        if (
                            setupField.Item4 == null ?
                            !String.IsNullOrWhiteSpace(fieldValues.Value) :
                            setupField.Item4 == fieldValues.Value
                        )
                            setupDict.Add(setupField.Item2, setupField.Item3);
                        else
                            removal.Add(setupField.Item2);
                    }
                }

                /*if (setupFields.TryGetValue("ACTIVE", out fieldValues) && fieldValues.Value == "FALSE")
                    setupDict.Add("Active", "0");
                if (setupFields.TryGetValue("AUTOBILL", out fieldValues) && fieldValues.Value == "FALSE")
                    setupDict.Add("AutoBill", "0");
                if (setupFields.TryGetValue("CALLNOTIFY_1", out fieldValues) && fieldValues.Value == "TRUE")
                    setupDict.Add("CallNotify1", "1");
                if (setupFields.TryGetValue("EMAILNOTIFY_1", out fieldValues) && fieldValues.Value == "TRUE")
                    setupDict.Add("EmailNotify1", "1");
                if (setupFields.TryGetValue("PRINTNOTIFY_1", out fieldValues) && fieldValues.Value == "TRUE")
                    setupDict.Add("PrintNotify1", "1");
                if (setupFields.TryGetValue("TEXTNOTIFY_1", out fieldValues) && fieldValues.Value == "TRUE")
                    setupDict.Add("TextNotify1", "1");
                if (setupFields.TryGetValue("WHOTONOTIFY_1", out fieldValues) && (fieldValues.Value == "B" || fieldValues.Value == "L"))
                    setupDict.Add("WhoToNotify1", fieldValues.Value);

                if (setupFields.TryGetValue("AUTOBILLTYPE", out fieldValues) && !String.IsNullOrWhiteSpace(fieldValues.Value))
                    setupDict.Add("AutoBillType", fieldValues.Value);
                if (setupFields.TryGetValue("ROUTEOPTINCLUDEDAYS", out fieldValues) && fieldValues.Value != "1111111")
                    setupDict.Add("RouteOptIncludeDays", fieldValues.Value);
                if (setupFields.TryGetValue("SERVICEDETAILEDDESCRIPTION1", out fieldValues) && !String.IsNullOrWhiteSpace(fieldValues.Value))
                    setupDict.Add("Description1", fieldValues.Value);*/
            }

            PPWebLib.PestPac.EditServiceSetup(
                setupID,
                setupDict
            );

            string orderID = null;
            if (Postman.GetLatestServiceOrder(locationID, setupID, out ServiceOrderListModel order) && order.OrderID.HasValue)
            {
                orderID = order.OrderID.Value.ToString();

                PPWebLib.PestPac.EditServiceOrder(
                    orderID,
                    orderFieldsDict.MakeSerializedCopy(),
                    out IRestResponse response
                );
            }
            
            return (setupID, orderID);
        }

        public void CreateTechs()
        {
            var employees = Utils.LoadJSON<InsightRep[]>(@"E:\Work Programming\Insight Temp Files\Employees\Existing.txt").Select(e => e.Username).ToArray();//.Concat(File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\Employees\Finished.txt")).Concat(File.ReadAllLines(@"E:\Work Programming\Insight Temp Files\Employees\Broken.txt")).ToArray();
            var reps = Utils.LoadCSV<InsightRep>(@"E:\Work Programming\Insight Temp Files\Employees\Converted Insight Reps.csv").Where(e => !employees.Contains(e.Username)).ToArray();
            int repIter = 0;
            ConcurrentBag<string> brokenReps = new ConcurrentBag<string>();
            ConcurrentBag<string> finishedReps = new ConcurrentBag<string>();
            var parallel = Parallel.ForEach(
                reps, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 8
                },
                rep =>
                {
                    if (!PPWebLib.PestPac.CreateTech(rep.FirstName, rep.LastName, rep.Username, out IRestResponse response))
                        brokenReps.Add(rep.Username);
                    else
                        finishedReps.Add(rep.Username);

                    lock (PARALLEL_LOCK)
                    {
                        ++repIter;
                        if (repIter % 25 == 0)
                        {
                            Console.WriteLine(String.Join("\r\n", finishedReps));
                            File.AppendAllLines(@"E:\Work Programming\Insight Temp Files\Employees\Finished.txt", finishedReps);
                            finishedReps = new ConcurrentBag<string>();
                            if (brokenReps.Any())
                            {
                                Console.Write(" ~~ BROKEN:\r\n\t");
                                Console.WriteLine(String.Join("\r\n\t", brokenReps));
                                File.AppendAllLines(@"E:\Work Programming\Insight Temp Files\Employees\Broken.txt", brokenReps);
                                brokenReps = new ConcurrentBag<string>();
                            }
                        }
                    }
                }
            );
            SpinWait.SpinUntil(() => parallel.IsCompleted);
            Console.WriteLine(String.Join("\r\n", finishedReps));
            File.AppendAllLines(@"E:\Work Programming\Insight Temp Files\Employees\Finished.txt", finishedReps);
            finishedReps = new ConcurrentBag<string>();
            if (brokenReps.Any())
            {
                Console.Write(" ~~ BROKEN:\r\n\t");
                Console.WriteLine(String.Join("\r\n\t", brokenReps));
                File.AppendAllLines(@"E:\Work Programming\Insight Temp Files\Employees\Broken.txt", brokenReps);
                brokenReps = new ConcurrentBag<string>();
            }
            Console.WriteLine("FINISHED");
            Console.ReadLine();
        }

        public static bool TryGetResults(string query, out SortedDictionary<string, string> results)
        {
            try
            {
                results = VantageTracker.SearchCustomers(query);
                return true;
            }
            catch
            {
                results = new SortedDictionary<string, string>();
                return false;
            }
        }
    }

    
}
