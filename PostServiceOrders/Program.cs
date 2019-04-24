using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PPLib;
using PostmanLib;

namespace PostServiceOrders
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Write("Enter the number of the batch you want to post to: ");
            int batchNum = Convert.ToInt32(Console.ReadLine().Trim());

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

            PostedOrder[] postedOrders = null;
            try
            {
                postedOrders = LoopManager.StartLoop<PostedOrder>(
                    dialog.FileName,
                    "FinishedPostedOrders.txt",
                    "ErroredPostedOrders.txt",
                    o => o.OrderID
                );
            }
            catch(Exception err)
            {
                if (err.Message.StartsWith("The process cannot access the file"))
                    Console.WriteLine("Please close the Excel file and try again.");
                else if (err.Message.StartsWith("No members are mapped"))
                    Console.WriteLine("Please make sure that the Excel file has 'OrderID' and 'Tech' fields.");
                else
                    Console.WriteLine(err.Message);
                Console.Write("Press Enter to exit...");
                Console.ReadLine();
                return;
            }

            LoopManager.EndLoop(
                Parallel.ForEach(
                    postedOrders, LoopManager.PARALLEL_OPTS, postedOrder =>
                    {
                        try
                        {
                            if (Postman.PostServiceOrder(postedOrder.OrderID, postedOrder.Tech, batchNum))
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
        }
    }
}
