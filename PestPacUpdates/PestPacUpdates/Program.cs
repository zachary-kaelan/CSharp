using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using PestPac.Model;
using PestPac = PPWebLib.PestPac;
using PostmanLib;
using ZachLib;
using ZachLib.Logging;
using PPLib;

namespace PestPacUpdates
{
    class Program
    {
        private static Stopwatch timer = Stopwatch.StartNew();

        private static readonly ParallelOptions PARALLEL_OPTS = new ParallelOptions() { MaxDegreeOfParallelism = 16 };

        private static string PATH_LOGGING =
            Path.GetDirectoryName(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData
                )
            ) + @"\Local\Temp\PestPacUpdates\";

        private static string[,] PROCEDURES = null;

        [STAThread]
        static void Main(string[] args)
        {
            int selected = -1;
            string path = null;

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            var proc = Process.GetCurrentProcess();
            proc.PriorityClass = ProcessPriorityClass.AboveNormal;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //Console.WriteLine(ServicePointManager.DefaultConnectionLimit);
            ServicePointManager.DefaultConnectionLimit = 1000000;

            LogManager.AddLog(new Log(LogType.FolderFilesByDate, "PestPacUpdates") { LogFileEntries = true });
            LogManager.Start(ThreadPriority.AboveNormal, false);

            timer.Stop();
            LogManager.Enqueue("PestPacUpdates", EntryType.DEBUG, "Initialization completed in " + timer.ElapsedMilliseconds.ToString() + " ms");
            timer.Restart();

#if !DEBUG
            PROCEDURES = new string[6, 5]
            {
                { "", "NAME", "FIELDS", "DATE CREATED", "INPUT TYPE" },
                { "1 - ", "RemoveSkipMonths", "Location", "01/22/2019", "csv" },
                { "2 - ", "FixTimeRanges", "Location, Order", "10/18/2018", "csv" },
                { "3 - ", "BatchPayments", "BillToID, Balance, LocationCode", "01/07/2019", "csv" },
                { "4 - ", "UpdateEmployeeAccess", "Username", "2/26/2019", "txt" },
                { "5 - ", "PostServiceOrders", "OrderID, Tech", "6/5/2018", "csv" }
            };

            if (!Directory.Exists(PATH_LOGGING))
                Directory.CreateDirectory(PATH_LOGGING);

            StringExtensions.ConsoleWriteTable(PROCEDURES);

            do
            {
                Console.WriteLine();
                Console.Write("Select an update procedure: ");
                try
                {
                    selected = Convert.ToInt32(Char.GetNumericValue(Console.ReadKey().KeyChar));
                    Console.WriteLine();
                    Console.Write("You selected {0}. Is this correct? (y/n): ", PROCEDURES[selected, 1]);
                    var yesNo = Console.ReadKey().KeyChar;
                    if (!(yesNo == 'Y' || yesNo == 'y'))
                        selected = -1;
                    Console.WriteLine();
                }
                catch
                {
                    Console.WriteLine("Please enter a valid procedure ID.");
                    selected = -1;
                }
            } while (selected <= 0);

            timer.Stop();
            LogManager.Enqueue("PestPacUpdates", EntryType.DEBUG, "Procedure selection completed in " + timer.ElapsedMilliseconds.ToString() + " ms");
            timer.Restart();

            OpenFileDialog dialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = PROCEDURES[selected, 4],
                AddExtension = true,
                Filter = PROCEDURES[selected, 4] == "csv" ? 
                    "CSV UTF-8 (Comma delimited) (*.csv)|*.csv|Text files (*.prn;*.txt;*.csv)|*.prn;*.txt;*.csv|All files (*.*)|*.*" :
                    "Text files (*.prn;*.txt;*.csv)|*.prn;*.txt;*.csv|CSV UTF-8 (Comma delimited) (*.csv)|*.csv|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads",
                SupportMultiDottedExtensions = true,
                Title = PROCEDURES[selected, 1] + " - Open File"
            };
            Console.Write("Pick a file: ");
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            path = dialog.FileName;
            Console.WriteLine(path);
            Console.WriteLine();

            timer.Stop();
            LogManager.Enqueue("PestPacUpdates", EntryType.DEBUG, "File selection completed in " + timer.ElapsedMilliseconds.ToString() + " ms");

            LogManager.Enqueue(
                new LogUpdate(
                    "PestPacUpdates", 
                    new string[] {
                        "Procedure Started",
                        null,
                        "Procedure Name:    " + PROCEDURES[selected, 1],
                        "File Name:         " + dialog.FileName,
                        "File Size:         " + new FileInfo(dialog.FileName).Length
                    }
                )
            );
#else
            UpdateEmployeeAccess(@"C:\Users\ZACH-GAMING\Downloads\Main Branch Username List.txt");
#endif

            timer.Restart();
            bool tryAgain = false;
            do
            {
                tryAgain = !TryRunProcedure(selected, path);
            } while (tryAgain);
            timer.Stop();

            LogManager.Enqueue("PestPacUpdates", EntryType.DEBUG, "Procedure execution completed in " + timer.ElapsedMilliseconds.ToString() + " ms");
            Console.WriteLine();
            Console.WriteLine("Procedure execution completed in {0}.", timer.Elapsed);
            Console.ReadLine();
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            LogManager.Enqueue("PestPacUpdates", EntryType.DEBUG, "Process exited");
        }

        public static bool TryRunProcedure(int selected, string file)
        {
            try
            {
                timer.Restart();
                switch (selected)
                {
                    case 1:
                        SkipMonths(file, true);
                        break;

                    case 2:
                        TimeRanges(file);
                        break;

                    case 3:
                        BatchPayments(file);
                        break;

                    case 4:
                        UpdateEmployeeAccess(file);
                        break;

                    case 5:
                        PostServiceOrders(file);
                        break;

                    default:
                        Console.WriteLine("Invalid procedure selection");
                        break;
                }
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine("Please close the Excel file before continuing.");
                Console.WriteLine("Press enter to continue... ");
                Console.ReadLine();
                return false;
            }
            catch (Exception e)
            {
                if (e.Message.StartsWith("No members are mapped"))
                    Console.WriteLine("Make sure the file has the right columns: " + PROCEDURES[selected, 2]);
                else
                {
                    LogManager.Enqueue(
                        "LoopManager",
                        PROCEDURES[selected, 1],
                        true,
                        e
                    );
                    Console.WriteLine(e.GetType().Name + " - " + e.Message);
                    if (e.InnerException != null)
                        Console.WriteLine(e.InnerException.GetType().Name + " - " + e.InnerException.Message);
                }
                Console.WriteLine("Press enter to continue... ");
                Console.ReadLine();
                return false;
            }
        }

        public static void SkipMonths(string path, bool clearErrors = false)
        {
            var loop = new LoopManager<Location_CodeOnly>(
                path,
                PATH_LOGGING + "SetupSkipMonthsFinished.txt",
                PATH_LOGGING + "SetupSkipMonthsErrored.txt",
                l => l.Location.ToString(),
                clearErrors
            );
            var skipFields = Enumerable.Range(1, 12).Select(m => "Skip" + m.ToString()).ToArray();
            loop.EndLoop(
                Parallel.ForEach(
                    loop.Updates,
                    PARALLEL_OPTS,
                    location =>
                    {
                        try
                        {
                            int locID = Postman.GetLocationIDFromCode(location.Location);
                            var setupID = Postman.GetLocationServiceSetup(locID, s => s.ServiceCode == "MONTHLY MOSQ").SetupID.Value.ToString();
                            PPWebLib.PestPac.EditServiceSetup(setupID, null, skipFields);
                            loop.LogUpdate(location.Location.ToString(), setupID);
                        }
                        catch (Exception e)
                        {
                            loop.LogUpdate(location.Location, e.Message, UpdateType.Error);
                        }
                    }
                ),
                false
            );
        }

        public static void TimeRanges(string path)
        {
            var loop = new LoopManager<Location_CodeAndOrder>(
                @"C:\Users\ZACH-GAMING\Downloads\Time Range Updates Oct.18.2018.csv",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\TimerangeChangesFinished.txt",
                @"E:\Work Programming\Insight Program Files\PPInternal\Logging\TimerangeChangesErrored.txt",
                t => t.Location
            );
            loop.EndLoop(
                Parallel.ForEach(
                    loop.Updates,
                    LoopManager.PARALLEL_OPTS,
                    l =>
                    {
                        string location = l.Location.ToString();
                        try
                        {
                            int id = Postman.GetLocationIDFromCode(l.Location);
                            var serviceSetups = Postman.GetLocationServiceSetups(id).Where(s => !String.IsNullOrWhiteSpace(s.TimeRange));
                            var serviceOrder = Postman.GetServiceOrderByNumber(id, l.Order);

                            foreach (var serviceSetup in serviceSetups)
                            {
                                PPWebLib.PestPac.EditServiceSetup(serviceSetup.SetupID.Value.ToString(), new Dictionary<string, string>() { { "TimeRange", "" } });
                            }

                            if (serviceOrder == null || !serviceOrder.OrderID.HasValue)
                                loop.LogUpdate(l.Location, l.Location + " has no matching orders.", UpdateType.Nonexistant);
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

                            loop.LogUpdate(l.Location, null);
                        }
                        catch (Exception e)
                        {
                            loop.LogUpdate(
                                l.Location, 
                                l.Location + " met with error: " + e.Message, 
                                UpdateType.Error
                            );
                        }
                    }
                ),
                false
            );
        }

        public static void BatchPayments(string path)
        {
            var loop = new LoopManager<BillToPaymentModel>(
                path,
                PATH_LOGGING + "FinishedBatchPayments.txt",
                PATH_LOGGING + "ErroredBatchPayments.txt",
                b => b.LocationCode
            );

            loop.EndLoop(
                Parallel.ForEach(
                    loop.Updates,
                    new ParallelOptions() { MaxDegreeOfParallelism = 32 },
                    billto =>
                    {
                        CardOnFileListModel[] accounts = null;
                        CardOnFileListModel account = null;
                        PaymentProcessorTransactionModel transaction = null;
                        try
                        {

                            accounts = Postman.GetPaymentAccounts(billto.BillToID);
                            if (accounts.Length == 0)
                                loop.LogUpdate(billto, billto.LocationCode + ", " + billto.BillToID + " - No payment accounts found", UpdateType.Nonexistant);
                            else
                            {
                                if (accounts.Count() > 1)
                                {
                                    accounts = accounts.Where(a => a.ExpirationDate.Value > Utils.Now).ToArray();
                                    if (accounts.Count() > 1)
                                        accounts = accounts.Where(a => a.AutoBilledServices.Any()).ToArray();
                                }
                                account = accounts.Count() == 1 ?
                                    accounts[0] :
                                    accounts.First(a => a.IsPrimary.Value);

                                transaction = Postman.Charge(
                                    account.CardID.Value,
                                    billto.Balance.StartsWith("$") ?
                                        billto.Balance.Substring(1) :
                                        billto.Balance,
                                    false
                                );

                                string loopLogEntry = (transaction.Payment != null ? transaction.Payment.BatchNumber + " - " : "") +
                                    (transaction.TransactionResponse.TransactionResult.HasValue ?
                                        transaction.TransactionResponse.TransactionResult.Value.ToString() + ": " : "") +
                                        transaction.TransactionResponse.Message;
                                int loopLogKey = billto.BillToID;

                                switch (transaction.TransactionResponse.TransactionResult.Value)
                                {
                                    case TransactionResponseModel.TransactionResultEnum.Error:
                                        loop.LogUpdate(loopLogKey, loopLogEntry, UpdateType.Error);
                                        LogManager.Enqueue(
                                            "BatchPayments",
                                            loopLogKey.ToString(),
                                            new object[] {
                                                loopLogKey,
                                                transaction.TransactionResponse.Message
                                            },
                                            new object[]
                                            {
                                                billto,
                                                account,
                                                transaction.TransactionResponse
                                            },
                                            true
                                        );
                                        break;

                                    case TransactionResponseModel.TransactionResultEnum.Approved:
                                        loop.LogUpdate(loopLogKey, loopLogEntry, UpdateType.Finished);
                                        LogManager.Enqueue(
                                            "BatchPayments",
                                            EntryType.DEBUG,
                                            loopLogKey + " has been charged " + billto.Balance,
                                            "Batch " + transaction.Payment.BatchNumber.Value.ToString()
                                        );

                                        break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            loop.LogUpdate(billto, e.Message, UpdateType.Error);
                            LogManager.Enqueue(
                                "BatchPayments",
                                billto.BillToID.ToString(),
                                true,
                                "Code - " + billto.BillToID,
                                e, true
                            );
                        }
                    }
                ),
                false
            );
        }

        public static void UpdateEmployeeAccess(string path)
        {
            List<int> branchIDs = new List<int>();
            do
            {
                Console.Write("Enter branch name(s): ");
                var branchesStr = Console.ReadLine().Trim();
                string[] branches = branchesStr.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var branch in branches)
                {
                    if (Postman.TryGetBranch(branch, out int branchID))
                        branchIDs.Add(branchID);
                    else
                        Console.WriteLine("\"" + branch + "\" is an invalid branch");
                }

                if (branchIDs.Count > 0)
                {
                    if (branchIDs.Count < branches.Length)
                    {
                        Console.Write("Would you like to add more branches? (y/n)");
                        var key = Console.ReadKey();
                        if (key.KeyChar == 'N' || key.KeyChar == 'n')
                            break;
                    }
                    else
                        break;
                }
                else
                    Console.WriteLine("Try again, fat-fingers.");
            } while (true);

            string access = null;
            Console.Write("Enter new access template: ");
            access = Console.ReadLine().Trim();

            var loop = new LoopManager(
                path,
                PATH_LOGGING + "FinishedEmployeeAccessUpdates.txt",
                PATH_LOGGING + "ErroredEmployeeAccessUpdates.txt",
                s => !s.Contains('.')
#if DEBUG
                , true
#endif
            );

            loop.EndLoop(
                Parallel.ForEach(
                    loop.Updates,
                    PARALLEL_OPTS,
                    username =>
                    {
                        try
                        {
                            var employee = Postman.GetEmployee(username);
                            var id = employee.EmployeeID.Value.ToString();
                            List<int> erroredBranchIDs = new List<int>();
                            foreach(var branchID in branchIDs)
                            {
                                if (!Postman.UpdateEmployeeAccess(id, branchID, access))
                                    erroredBranchIDs.Add(branchID);
                            }
                            if (erroredBranchIDs.Any())
                                loop.LogUpdate(username, "Failed branches: " + erroredBranchIDs.ToArrayString(), UpdateType.Error);
                            else
                                loop.LogUpdate(username, id);
                        }
                        catch (Exception e)
                        {
                            loop.LogUpdate(username, e.Message, UpdateType.Error);
                        }
                    }
                )
            );
        }

        public static void PostServiceOrders(string path)
        {
            Console.Write("Enter the number of the batch you want to post to: ");
            int batchNum = Convert.ToInt32(Console.ReadLine().Trim());
            var loop = new LoopManager<PostedOrder>(
                path,
                "FinishedPostedOrders.txt",
                "ErroredPostedOrders.txt",
                o => o.OrderID
            );

            loop.EndLoop(
                Parallel.ForEach(
                    loop.Updates, LoopManager.PARALLEL_OPTS, postedOrder =>
                    {
                        try
                        {
                            var responseCode = Postman.PostServiceOrder(postedOrder.OrderID, postedOrder.Tech, batchNum);
                            if (responseCode.IsOK())
                                loop.LogUpdate(postedOrder.OrderID, "OK", UpdateType.Finished);
                            else
                                loop.LogUpdate(postedOrder.OrderID, responseCode.ToString(), UpdateType.Error);
                        }
                        catch (Exception e)
                        {
                            loop.LogUpdate(postedOrder.OrderID, e.Message, UpdateType.Error);
                        }
                    }
                )
            );
        }
    }
}
