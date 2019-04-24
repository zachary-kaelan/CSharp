using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jil;
using PostmanLib;
using PPLib;
using ZachLib;

namespace FixServiceCodes
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
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

            ServiceModel[] services = null;
            try
            {
                services = Utils.LoadCSV<ServiceModel>(dialog.FileName);
            }
            catch (Exception err)
            {
                if (err.Message.StartsWith("The process cannot access the file"))
                    Console.WriteLine("Please close the Excel file and try again.");
                else if (err.Message.StartsWith("No members are mapped"))
                    Console.WriteLine("Please make sure that the Excel file has 'LocationCode', 'ServiceCode', and 'SpecialProblems' fields.");
                else
                    Console.WriteLine(err.Message);

                Console.Write("Press Enter to exit...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("{0} services loaded", services.Length);

            List<ServiceModelErrored> failedServices = new List<ServiceModelErrored>();
            List<ServiceModelSuccessful> successfulServices = new List<ServiceModelSuccessful>();
            SortedDictionary<string, List<string>> codes = new SortedDictionary<string, List<string>>();
            Console.Write("Determining service codes... ");
            foreach(var service in services)
            {
                try
                {
                    ServiceCodeSelection.SelectCode("", service.SpecialProblems, "", out string code);
                    ServiceModelSuccessful successful = service;
                    successful.NewServiceCode = code;
                    successfulServices.Add(successful);
                    if (codes.TryGetValue(code, out List<string> locations))
                        locations.Add(service.LocationCode);
                    else
                        codes.Add(code, new List<string>() { service.LocationCode });
                }
                catch (Exception err)
                {
                    ServiceModelErrored errored = service;
                    errored.ErrorMessage = err.Message;
                }
            }
            Console.WriteLine("Done");

            Console.Write("Logging services... ");
            try
            {
                successfulServices.SaveCSV("Succeeded.csv");
                failedServices.SaveCSV("Failed.csv");
                if (!Directory.Exists("Lists"))
                    Directory.CreateDirectory("Lists");
            }
            catch(Exception err)
            {
                Console.WriteLine("An error has occurred:");
                Console.WriteLine(JSON.Serialize(err, Options.PrettyPrintExcludeNullsIncludeInherited));
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
                return;
            }

            foreach(var code in codes)
            {
                try
                {
                    File.WriteAllLines(@"Lists\" + code.Key + ".csv", code.Value.ToArray());
                }
                catch (Exception err)
                {
                    Console.WriteLine("An error has occurred:");
                    Console.WriteLine(JSON.Serialize(err, Options.PrettyPrintExcludeNullsIncludeInherited));
                    Console.WriteLine("Press enter to exit...");
                    Console.ReadLine();
                    return;
                }
            }
            Console.WriteLine("Done");

            Console.WriteLine("Creating empty PestPac lists and fetching new descriptions...");
            string dateString = DateTime.Now.ToString("dd.MM.yyyy");
            var serviceDescs = Postman.GetServiceDescriptions();
            string[] lines = new string[codes.Count];
            int i = 0;
            foreach(var code in codes.Keys.OrderBy())
            {
                Postman.CreateList(dateString + " " + code, ListVisibility.Public);
                string line = "\t" + code + " -:- " + serviceDescs[code];
                Console.WriteLine(line);
                lines[i] = line;
                ++i;
            }
            File.WriteAllLines("NewDescriptions.txt", lines);
            Console.WriteLine("Done");
            Console.WriteLine();

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }

    public class ServiceModel
    {
        public string LocationCode { get; set; }
        public string ServiceCode { get; set; }
        public string SpecialProblems { get; set; }
        public ServiceModel(ServiceModel model)
        {
            LocationCode = model.LocationCode;
            ServiceCode = model.ServiceCode;
            SpecialProblems = model.SpecialProblems;
        }
    }

    public class ServiceModelErrored : ServiceModel
    {
        public string ErrorMessage { get; set; }
    }

    public class ServiceModelSuccessful : ServiceModel
    {
        public string NewServiceCode { get; set; }
    }
}
