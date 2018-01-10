using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using VantageTracker.Properties;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PPAPIOptimized;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;
using Jil;
using PPLib;

namespace VantageTracker
{
    class Discrepancy
    {
        public long startPos { get; set; }  // inclusive
        public long endPos { get; set; }    // exclusive
        public byte[] bytes { get; set; }
        public string encoded { get; set; }
        //public string utf8 { get; set; }

        public Discrepancy(long curPos)
        {
            this.startPos = curPos;
        }
    }

    

    class Program
    {
        static volatile double[] successCounts = new double[2] { 0, 0 };
        static volatile double[] totalCounts = new double[2] { 0, 0 };
        const string KeyValuePat = "(?:<span id=\"[^\"']*?\".{15,285}?)?(?:name|id)=(?<y>'|\")([^\"'" + @"\[\]]{3,45}?)\k<y>.{0,350}?(?:(?:\s" + "*?<option )?(?:value=(?<x>'|\"))|(?:>))([^\"']{0,155}?)(?:" + @"\k<x>|<\/textarea>)";
        const string CustIDPat = @"<dd>\s+(\d+)\s+<\/dd>";
        public const string logsPath = @"E:\VTLogs\";
        public const string curOperation = "ChargeBack";

        public static readonly CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        public const CompareOptions opts = CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols;

        static void Main(string[] args)
        {
            Process ghostScript = new Process();
            ghostScript.StartInfo.WorkingDirectory = @"E:\Temp\";
            ghostScript.StartInfo.FileName = @"C:\Program Files\gs\gs9.22\bin\gswin64c.exe";
            ghostScript.StartInfo.Arguments = "-dSAFER -dBATCH -dNOPAUSE -sDEVICE=jpeg -r150 -dTextAlphaBits=4 -dGraphicsAlphaBits=4 -dMaxStripSize=8192 -sOutputFile=image_%d.jpg PostmanPdf_1.pdf";
            ghostScript.Start();
            ghostScript.WaitForExit();

            //PPNav ppClient = new PPNav();
            CookieContainer cookies = new CookieContainer();
            RestClient client = new RestClient("http://app.pestpac.com/.NET/reports/");
            client.CookieContainer = cookies;
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
            //client.CookieContainer = ppClient.Cookies;

            client.AddDefaultParameter("ShowPrices", "1", ParameterType.QueryString);
            client.AddDefaultParameter("SummaryDetail", "D", ParameterType.QueryString);
            client.AddDefaultParameter("CompanyKey", "323480", ParameterType.QueryString);

            RestRequest request = new RestRequest("Report.aspx", Method.POST);
            /*string[] constants = File.ReadAllLines(@"E:\Temp\Constants.txt").Select(c => {
                string[] kv = c.Split(new string[] { " :=: " }, StringSplitOptions.None);
                request.AddParameter(kv[0], kv[1], ParameterType.GetOrPost);
                return kv[0];
            }).ToArray();*/
            request.AddParameter("Assem", "PestPac.Report.SERVICEPRO123480", ParameterType.GetOrPost);
            request.AddParameter("CompanyID", "1", ParameterType.GetOrPost);
            request.AddParameter("Mode", "", ParameterType.GetOrPost);
            request.AddParameter("ReportName", "Inspection", ParameterType.GetOrPost);
            request.AddParameter("UserID", "1", ParameterType.GetOrPost);

            request.AddParameter("InvoiceID", "1206363", ParameterType.GetOrPost);
            request.AddParameter("RouteID", null, ParameterType.GetOrPost);
            request.AddParameter("TaxCodeID", "1", ParameterType.GetOrPost);
            request.AddParameter("DivisionID", "2", ParameterType.GetOrPost);

            client.DownloadData(request).SaveAs(@"E:\Temp\PostmanPdf.pdf");

            /*
            Parameter[] constantParams = request.Parameters.ToArray();

            Parameter[] parameters = File.ReadAllLines(@"E:\Temp\TestRequest.txt").Select(l => l.Split('\t'))
                .Select(kv => new Parameter() { Name = kv[0], Value = kv[1], Type = ParameterType.GetOrPost }).ToList()
                .FindAll(p => !constants.Contains(p.Name)).ToArray();
            request.Parameters.AddRange(parameters);
            var response = client.Execute(request);
            response = client.Execute(request);
            response.RawBytes.SaveAs(@"E:\Temp\TestPdf.pdf");
            PdfReader pr = new PdfReader(@"E:\Temp\TestPdf.pdf");
            long length = response.ContentLength;
            string text = PdfTextExtractor.GetTextFromPage(pr, 1, new SimpleTextExtractionStrategy());
            pr.Dispose();
            pr.Close();
            pr = null;
            response = null;
            List<Parameter> ignoredParams = new List<Parameter>();
            StreamWriter sw = new StreamWriter(@"E:\Temp\IgnoredParams.txt", true);
            string gibberish = Convert.ToBase64String(Encoding.Default.GetBytes("iuh45457fuHDU12312IHFI987990GYGIOfew33277ryoeofYYDFndN5432fudFU079098DSFINdufsFJka3123jsadk8769aSALDHfr8932949p"));
            foreach(Parameter param in constantParams)
            {
                int index = request.Parameters.FindIndex(p => p.Name == param.Name);
                string tempValue = request.Parameters[index].Value.ToString();
                request.Parameters[index].Value = gibberish;
                response = client.Execute(request);
                if (Math.Abs(response.ContentLength - length) <= 1000)
                {
                    response.RawBytes.SaveAs(@"E:\Temp\RequestPdf.pdf");
                    using (pr = new PdfReader(response.RawBytes))
                    {
                        if (PdfTextExtractor.GetTextFromPage(pr, 1, new SimpleTextExtractionStrategy()) == text)
                        {
                            sw.WriteLine(param.Name);
                            ignoredParams.Add(param);
                        }
                    }
                    pr = null;
                }
                response = null;

                request.Parameters[index].Value = tempValue;
                tempValue = null;
            }*/

            Random gen = new Random();
            StreamReader sr = new StreamReader(@"C:\Users\ZACH-GAMING\Downloads\Invoices.csv");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            List<SimpleInvoice> invoices = cr.GetRecords<SimpleInvoice>().ToList();
            cr.Dispose();
            cr = null;
            sr.Dispose();
            sr.Close();
            sr = null;

            invoices = invoices.GroupBy(i => i.LocationID).Select(g => g.First()).ToList();
            List<SimpleInvoice> tempInvoices = new List<SimpleInvoice>();
            for (int i = 0; i < 16; ++i)
            {
                int index = gen.Next(invoices.Count);
                tempInvoices.Add(invoices[index]);
                invoices.RemoveAt(index);
            }
            invoices = null;
            Console.WriteLine(String.Join("\r\n", tempInvoices.Select(i => "http://app.pestpac.com/invoice/detail.asp?Mode=NewWindow&InvoiceID=" + i.InvoiceID + "&LookIn=C&From=L")));
            Console.ReadLine();

            /*VTNav client = new VTNav("MIN");
            Dictionary<string, string> query = new Dictionary<string, string>() { { "salesPerson", "208987" }, { "customerStatus", "Active" }, { "initialJobStatus", "Complete" } };
            var custs = client.AdvancedSearch(query, "MIN")
                .ToDictionary(
                    c => c,
                    c => {
                        Invoice invoice = client.GetInvoice(c.id);
                        invoice.Name = c.name;
                        invoice.Address = c.address;
                        return invoice;
                    }
                );

            custs.Values.ToArray().SaveAs(@"E:\Temp\Customers.txt");*/


            /*Dictionary<string, string[]> query = new Dictionary<string, string[]>(VTNav.VTRequests["advSearch"]);
            query["searchtype[salesPerson]"] = new string[] { "208987" };
            query["searchtype[customerStatus][]"] = new string[] { "Active" };*/


            /*StreamWriter sw = new StreamWriter(@"E:\Temp\Invoices.csv");
            CsvWriter cw = new CsvWriter(sw);
            cw.WriteRecords(Newtonsoft.Json.JsonConvert.DeserializeObject<Invoice[]>(File.ReadAllText(@"E:\Temp\Customers.txt")));
            sw.Flush();
            cw.Dispose();
            cw = null;
            sw.Dispose();
            sw.Close();
            sw = null;

            Console.WriteLine("DONE.");
            Console.ReadLine();

            DateTime daysStart = new DateTime(1899, 12, 30);*/

            /*
            string LAST_NOTE_DELETED = Environment.GetEnvironmentVariable("LAST_NOTE_DELETED", EnvironmentVariableTarget.Machine);
            //Console.ReadKey();
            //Client client = new Client("MIN");
            PPNav client = new PPNav();
            var records = new CsvReader(
                new StreamReader(
                    @"C:\DocUploads\Notes\Notes.csv")
            ).GetRecords<DeleteNotesModel>().OrderBy(n => n.NoteID).ToList();//.SkipWhile(n => n.NoteID != LAST_NOTE_DELETED).ToList();

            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            Parallel.For(0, records.Count, new ParallelOptions() { MaxDegreeOfParallelism=8, TaskScheduler=TaskScheduler.Current},i => {
                DeleteNotesModel n = records[i];
                client.DeleteNote(n.LocationID, n.NoteID, n.Note);*/
                /*if (i % 50 == 0)
                    try
                    {
                        Environment.SetEnvironmentVariable("LAST_NOTE_DELETED", n.NoteID, EnvironmentVariableTarget.Machine);
                    }
                    catch
                    {

                    }*/
            //});
            //client.DeleteNote("152306", "2720794", "TEST NOTE");

            Stopwatch timer = new Stopwatch();

            /*
            Card card = client.ExtractCard(client.FindCust("Mark Gierach"));
            foreach(var prop in card.GetType().GetProperties())
            {
                Console.WriteLine(prop.Name + " - " + prop.GetValue(card, null));
            }
            */

            /*
            (Dictionary<string, string> cust, List<NoteModel> notes) = client.GetNotes(client.FindCust("Mark Gierach"));
            foreach(var entry in cust)
            {
                Console.WriteLine(entry.Key + " = " + entry.Value);
            }
            Console.WriteLine();

            foreach(NoteModel note in notes)
            {
                Console.WriteLine(note.NoteDate + "\r\n" + note.Note + "\r\n");
            }
            */
            //client.TransferNotes("Mark Gierach");


            /*
            ParallelOptions opts = new ParallelOptions();
            opts.TaskScheduler = TaskScheduler.Current;
            opts.MaxDegreeOfParallelism = 4;

            List<string> skip = new List<string>(File.ReadAllLines(@"C:\Users\ZACH-GAMING\Documents\LocIDs_SUCCESS.txt"));
            skip.AddRange(File.ReadAllLines(@"C:\Users\ZACH-GAMING\Documents\LocIDs_DUPLICATES.txt"));
            skip = skip.Select(s =>
                s.Length < 10 ? "" : s.Split('\t')[3]
            ).ToList();

            FileStream fs = new FileStream(@"C:\Users\ZACH-GAMING\Documents\LocIDs_SUCCESS.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            FileStream fs2 = new FileStream(@"C:\Users\ZACH-GAMING\Documents\LocIDs_FAILED.txt", FileMode.Create, FileAccess.Write);
            StreamWriter sw2 = new StreamWriter(fs2);
            FileStream fs3 = new FileStream(@"C:\Users\ZACH-GAMING\Documents\LocIDs_DUPLICATES.txt", FileMode.Append, FileAccess.Write);
            StreamWriter sw3 = new StreamWriter(fs3);

            Random gen = new Random(DateTime.Now.Millisecond);
                 
            var lines = File.ReadAllLines(@"C:\Users\ZACH-GAMING\Documents\Accounts that need PP Account Numbers.txt").ToList();
            Console.WriteLine(lines.RemoveAll(l => skip.Contains(l.Split('\t')[3])));

            //sw.WriteLine(lines[0] + "\tLocationID\tLocationCode");
            //sw2.WriteLine(lines[0]);

            Console.Write("Done!");
            Console.ReadKey();*/
        }

        public static void SplitePDF(string filepath)
        {
            iTextSharp.text.pdf.PdfReader reader = null;
            int currentPage = 1;
            int pageCount = 0;
            //string filepath_New = filepath + "\\PDFDestination\\";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            //byte[] arrayofPassword = encoding.GetBytes(ExistingFilePassword);
            reader = new iTextSharp.text.pdf.PdfReader(filepath);
            reader.RemoveUnusedObjects();
            pageCount = reader.NumberOfPages;
            string ext = System.IO.Path.GetExtension(filepath);
            for (int i = 1; i <= pageCount; i++)
            {
                iTextSharp.text.pdf.PdfReader reader1 = new iTextSharp.text.pdf.PdfReader(filepath);
                string outfile = filepath.Replace((System.IO.Path.GetFileName(filepath)), (System.IO.Path.GetFileName(filepath).Replace(".pdf", "") + "_" + i.ToString()) + ext);
                reader1.RemoveUnusedObjects();
                iTextSharp.text.Document doc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(currentPage));
                iTextSharp.text.pdf.PdfCopy pdfCpy = new iTextSharp.text.pdf.PdfCopy(doc, new System.IO.FileStream(outfile, System.IO.FileMode.Create));
                pdfCpy.SetFullCompression();
                doc.Open();
                for (int j = 1; j <= 1; j++)
                {
                    iTextSharp.text.pdf.PdfImportedPage page = pdfCpy.GetImportedPage(reader1, currentPage);
                    pdfCpy.AddPage(page);
                    currentPage += 1;
                }
                doc.Close();
                pdfCpy.Close();
                reader1.Close();
                reader.Close();

            }
        }
    }
}
