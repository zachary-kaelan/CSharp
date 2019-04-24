using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PPLib;
using ZachLib;
using ZachLib.Logging;
using PostmanLib;
using PestPac.Model;

namespace BatchPayments
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var proc = Process.GetCurrentProcess();
            proc.PriorityBoostEnabled = true;
            proc.PriorityClass = ProcessPriorityClass.AboveNormal;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            LogManager.AddLog(new Log(LogType.FolderFilesByDate, "BatchPayments") { LogFileEntries = true });
            LogManager.Start(ThreadPriority.AboveNormal, false);

            OpenFileDialog dialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "csv",
                AddExtension = true,
                Filter = "CSV UTF-8 (Comma delimited) (*.csv)|*.csv|Text files (*.prn;*.txt;*.csv)|*.prn;*.txt;*.csv|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads",
                SupportMultiDottedExtensions = true,
                Title = "PostServiceOrders - Open File"
            };
            Console.Write("Pick a file: ");
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            Console.WriteLine(dialog.FileName);

            var loop = new LoopManager<BillToPaymentModel>(
                dialog.FileName,
                "FinishedBatchPayments.txt",
                "ErroredBatchPayments.txt",
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
                                string loopLogKey = billto.BillToID;

                                switch (transaction.TransactionResponse.TransactionResult.Value)
                                {
                                    case TransactionResponseModel.TransactionResultEnum.Error:
                                        loop.LogUpdate(loopLogKey, loopLogEntry, UpdateType.Error);
                                        LogManager.Enqueue(
                                            "BatchPayments",
                                            loopLogKey,
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
                                            loopLogKey + " has been charged " + billto.Balance.ToString(), 
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
                                billto.BillToID,
                                true,
                                "Code - " + billto.BillToID,
                                e, true
                            );
                        }
                    }
                )
            );
        }
    }
}
