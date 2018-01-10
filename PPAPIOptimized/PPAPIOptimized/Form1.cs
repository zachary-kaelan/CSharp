using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net.Mail;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Outlook = Microsoft.Office.Interop.Outlook;
using iTextSharp.text.pdf;
using VantageTracker;
using CsvHelper;
using CsvHelper.Configuration;
using System.Net;
using PestPac.Model;
using RestSharp;
using RGX.Utils;
using PPLib;

namespace PPAPIOptimized
{
    public enum ItemType {LBL, PRG, DIA, CBO};
    public enum CBO { GHOST, CODE, FAIL, ERROR}

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public OpenFileDialog filog { get; set; }

        public const string VTAddressPat = @"^((?:\d{1,7} )?.{4,25})  ([a-zA-Z .]{2,17}) (\d{5})$";

        public const string DocPath = @"C:\DocUploads\";
        public const string SummariesPath = DocPath + @"Summaries\";
        public const string ErrorPath = DocPath + @"Erroneous Docs\";
        public const string CodeErrorPath = DocPath + @"Code Errors\";
        public const string DupesPath = DocPath + @"Duplicates\";
        public const string ProgramFilesPath = DocPath + @"Program Files\";
        public const string LogsPath = DocPath + @"Logs\";
        public const string PostmanLogsPath = LogsPath + @"Postman\";
        public const string ZillowLogsPath = LogsPath + @"Zillow\";
        public const string VTLogsPath = LogsPath + @"VantageTracker\";
        public const string CodeLogsPath = LogsPath + @"Code Errors\";
        public const string FailedLogsPath = LogsPath + @"Failed Uploads\";
        public const string GhostLogsPath = LogsPath + @"Missing Customers\";
        public const string PatchLogsPath = LogsPath + @"Patch Notes\";
        public const string ErrorLogsPath = LogsPath + @"Search Errors\";
        public const string PatchErrorsLogsPath = PatchLogsPath + @"Errors\";
        public const string UnsignedPath = DocPath + @"Unsigned\";
        public const string SourcePath = DocPath + @"Docs\";
        public const string GhostsPath = DocPath + @"Ghosts\";
        public const string FailedPath = DocPath + @"Failed\";
        public const string NotesPath = DocPath + @"Notes\";
        public const string TooLargePath = DocPath + @"TooLarge\";
        public const string EmptyRecsPath = DocPath + @"EmptyRecords\";
        public const string VTExportsPath = DocPath + @"VTExports\";
        public const string FailedNotesPath = NotesPath + @"Failed\";
        public const string SuccessNotesPath = NotesPath + @"Success\";
        public const string SourceNotesPath = NotesPath + @"Docs\";
        public const string DupeNotesPath = NotesPath + @"Dupes\";

        public const string ZillowDictionariesPath = ProgramFilesPath + @"ZillowDictionaries\";
        public const string AbbrvsPath = ProgramFilesPath + "Abbreviations.txt";
        public const string DupeLog = ProgramFilesPath + "Duplicates.txt";
        public const string ErrorLog = ProgramFilesPath + "UploadErrors.txt";
        public const string CodeLog = ProgramFilesPath + "CodeErrors.txt";
        public const string GhostLog = ProgramFilesPath + "Ghosts.txt";
        public const string NotesLog = ProgramFilesPath + "Notes.txt";
        public const string MissingZipsLog = ZillowLogsPath + "Missing Zips.txt";
        public const string ReconLog = ProgramFilesPath + "Recon.csv";

        //Dictionary<string, string> abbrvs = new Dictionary<string, string>();
        //Dictionary<string, string> abbrvs2 = new Dictionary<string, string>();

        List<Dictionary<string, string>> emptyNotes = new List<Dictionary<string, string>>();

        //JsonSerializer _jsonWriter = new JsonSerializer { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
        
        Dictionary<string, Dictionary<string, List<AdvancedQueryModel>>> branchLookup = null;
        Dictionary<string, List<AdvancedQueryModel>> noScheduleLookup = null;
        public JsonSerializerSettings settings = new JsonSerializerSettings() {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.Indented
        };

        MailAddress myAddr = new MailAddress("z.johnson@insightpest.com", "ZJ@ISP");

        private readonly Dictionary<string, string> secondarySchedule = new Dictionary<string, string>()
        {
            {"Q1 JAJO", "BILLQ1 1ST"},
            {"Q2 FMAN", "BILLQ2 1ST"},
            {"Q3 MJSD", "BILLQ3 1ST"},
            {"MONTHLY", "MOSQUITOS"},
            {"MOSQUITOS", "MONTHLY"}
        };

        private readonly Dictionary<string, string> stateAbbrvs = new Dictionary<string, string>()
        {
            {"CT", "Connecticut, CT"},
            {"RI", "Rhode Island, RI"}
        };

        private readonly Dictionary<string, string> pptoINPC = File.ReadAllLines(
            @"C:\Users\ZACH-GAMING\Documents\PestPac to Invoices.txt"
        ).ToDictionary(
            b => b.Split(new string[] { " :=: " }, StringSplitOptions.None)[1],
            b => b.Split(new string[] { " :=: " }, StringSplitOptions.None)[0]
        );

        public static Dictionary<string, List<VTExportModel>> VTExports = new Dictionary<string, List<VTExportModel>>();
        public static Dictionary<string, Dictionary<string, string>> branchZipCodes = null;

        /*public static Dictionary<string, Thread> threads = new Dictionary<string, Thread>(
        );
        Thread docsThread = null;
        Thread downloadThread = null;
        Thread phoneThread = null;
        Thread patchThread = null;
        Thread reconThread = null;
        Thread addCustsThread = null;*/

        public static readonly FileDictionary RGX_FILE_DICT = new FileDictionary();

        public const string LOG_FORMAT = "[{0}] [{1}]:\t{2}";
        public const string LOG_DATE_FORMAT = "HH:mm:ss";
        public const string FILENAME_DATE_FORMAT = "MM.dd.yyyy";
        //public static StreamWriter logMain = new StreamWriter(LogsPath + DateTime.Now.ToString(FILENAME_DATE_FORMAT) + ".txt", true) { AutoFlush = true };
        //public static StreamWriter logExecute = new StreamWriter(PostmanLogsPath + DateTime.Now.ToString(FILENAME_DATE_FORMAT) + ".txt", true) { AutoFlush = true };

        //public static ConcurrentQueue<Item> _queue = new ConcurrentQueue<Item>();
        //public static AutoResetEvent _signal = new AutoResetEvent(true);
        
        public static BackgroundWorker lblThread = null;
        public static BackgroundWorker cboThread = null;
        public static BackgroundWorker prgThread = null;
        public static BackgroundWorker lstThread = null;
        public static ConcurrentQueue<LabelUpdate> lblQueue = new ConcurrentQueue<LabelUpdate>();
        public static ConcurrentQueue<ComboBoxUpdate> cboQueue = new ConcurrentQueue<ComboBoxUpdate>();
        public static ConcurrentQueue<int> prgQueue = new ConcurrentQueue<int>();
        public static int prgCounter = 0;
        public static ConcurrentQueue<string> lstQueue = new ConcurrentQueue<string>();

        public static Thread UIUpdateThread = null;
        public static Dispatcher UIThread = null;
        public static LogManager Logger = new LogManager(
            "[{0}] [{1}]:\t{2} ~ {3}",
            LOG_DATE_FORMAT,
            LogsPath
        );

        Stopwatch timer = new Stopwatch();
        public static Client universalClient = null;
        public static Postman client = null;
        public static Random gen = new Random(DateTime.Now.Millisecond);

        public static Thread producer = null;
        public CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        public CompareOptions options = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase;

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new Postman(Logger);

            PidjClient pidj = new PidjClient(client);
            pidj.Debug();
            //client.DownloadDoc("1177672", @"C:\Users\ZACH-GAMING\Downloads\ServiceReportDoc.pdf");
            client.DownloadServiceReport("171421", @"C:\Users\ZACH-GAMING\Downloads\ServiceReport.pdf");

            /*Graphics gfx = CreateGraphics();
            Font font = new Font("Verdana", 11, FontStyle.Regular);
            float tabSize = gfx.MeasureString("\t", font).Width;
            float spaceSize = gfx.MeasureString(" ", font).Width;
            float emptySize = gfx.MeasureString("", font).Width;
            string[] lines = File.ReadAllLines(@"E:\Temp\TabPaddingTest.txt");
            List<string> paddedLines = new List<string>();
            float max = (lines.Max(l => gfx.MeasureString(l, font).Width) + gfx.MeasureString("    \t", font).Width);
            foreach(string line in lines)
            {
                float lineLength = gfx.MeasureString(line, font).Width;
                float numTabsF = (max - lineLength) / spaceSize;
                int numTabs = (int)Math.Round(numTabsF);

                string newLine = line + new string(' ', numTabs);
                float newLineLength = gfx.MeasureString(newLine, font).Width;
                float diff = max - newLineLength;
                if (diff < 0 && Math.Abs(diff) >= spaceSize)
                    newLine += " ";
                else if (diff > 0 && diff >= spaceSize)
                    newLine = newLine.Remove(newLine.Length - 1);
                if (newLineLength > 575.0)
                    newLine += "\t";
                float newLineLength2 = gfx.MeasureString(newLine, font).Width;
                paddedLines.Add(newLine + "\tTest");
            }
            paddedLines.Add(new string(' ', (int)(max / spaceSize)) + "Test");*/

            //lines = lines.Select(l => l + new string(' ', (int)((max - gfx.MeasureString(l, font).Width) / spaceSize)) + "Test").ToArray();
            //File.WriteAllLines(@"E:\Temp\TabsPadded.txt", paddedLines);

            /*PhoneClient phoneClient = new PhoneClient();
            string[] numbers = File.ReadAllLines(ProgramFilesPath + "FakeLandlines.txt").Take(25).ToArray();
            //List<string> fakes = new List<string>();
            PhoneInfo[][] tests = Enumerable.Range(0, 25)
                .Select(
                    i => Enumerable.Range(0, 25)
                        .Select(
                            j => phoneClient.GetInfo(numbers[j])
                        ).ToArray()
                ).ToArray();
            tests.SaveAs(ProgramFilesPath + "PhoneTests.txt");
            int[] nonduplicates = Enumerable.Range(0, 25).Select(i => 0).ToArray();
            for(int i = 0; i < 25; ++i)
            {
                List<PhoneInfo> info = new List<PhoneInfo>() { tests[0][i] };
                for (int j = 1; j < 25; ++j)
                {
                    PhoneInfo curInfo = tests[j][i];
                    if (info.All(p => p.PhoneCompany != curInfo.PhoneCompany || 
                        p.PhoneLocation != curInfo.PhoneLocation ||
                        p.PhoneNumber != curInfo.PhoneNumber ||
                        p.PhoneType != curInfo.PhoneType))
                    {
                        ++nonduplicates[i];
                        info.Add(curInfo);
                    }
                }
            }*/

            foreach (string path in new string[] {
                DocPath,
                ErrorPath,
                DupesPath,
                ProgramFilesPath,
                ZillowDictionariesPath,
                UnsignedPath,
                GhostsPath,
                FailedPath,
                SourcePath,
                LogsPath,
                VTLogsPath,
                PatchLogsPath,
                PatchErrorsLogsPath,
                PostmanLogsPath,
                ErrorLogsPath,
                CodeErrorPath,
                FailedLogsPath,
                GhostLogsPath,
                VTExportsPath,
                NotesPath,
                SourceNotesPath,
                FailedNotesPath,
                SuccessNotesPath,
                DupeNotesPath,
                SummariesPath })
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            UIThread = Dispatcher.CurrentDispatcher;
            Logger.OnEntry += Logger_OnEntry;
            Logger.Start();

            lblThread = new BackgroundWorker();
            lblThread.DoWork += lblThread_DoWork;
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => UIUpdater<LabelUpdate>(lblThread, lblQueue, 3000)));
            cboThread = new BackgroundWorker();
            cboThread.DoWork += cboThread_DoWork;
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => UIUpdater<ComboBoxUpdate>(cboThread, cboQueue, 150)));
            prgThread = new BackgroundWorker();
            prgThread.DoWork += prgThread_DoWork;
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => UIUpdater<int>(prgThread, prgQueue, 25)));
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => UIUpdater(prgThread, ref prgCounter, 250)));
            lstThread = new BackgroundWorker();
            lstThread.DoWork += lstThread_DoWork;
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) => UIUpdater<string>(lstThread, lstQueue, 250)));

            Console.SetOut(new ConsoleWriter());
            //CheckRecordsForBrokenDocuments();

            pptoINPC.SaveAs(@"E:\Temp\SaveAsTest.txt");

            /*producer = new Thread(() => Producer());
            producer.IsBackground = true;
            producer.Start();

            updThread = new BackgroundWorker();
            updThread.DoWork += UpdThread_DoWork;
            updThread.RunWorkerCompleted += UpdThread_RunWorkerCompleted;*/
        }

        private void Logger_OnEntry(object sender, string logName, string entry)
        {
            lstConsole.Items.Add(logName + " - " + entry);
        }

        private void UIUpdater<T>(BackgroundWorker worker, ConcurrentQueue<T> queue, int sleepTime)
        {
            while(true)
            {
                while (queue.TryDequeue(out T lblResult))
                {
                    lock(worker)
                    {
                        SpinWait.SpinUntil(() => !worker.IsBusy);
                        worker.RunWorkerAsync(lblResult);
                    }
                }

                Thread.Sleep(sleepTime);
            }
        }

        private void UIUpdater(BackgroundWorker worker, ref int counter, int sleepTime)
        {
            while (true)
            {
                if (counter > 0 && !worker.IsBusy)
                {
                    lock(worker)
                    {
                        int temp = counter;
                        worker.RunWorkerAsync(-1 * temp);
                        counter -= temp;
                    }
                }
                
                Thread.Sleep(sleepTime);
            }
        }

        private void DocUploadSetup()
        {
            /*Socket sock = new Socket(SocketType.Rdm, ProtocolType.IPv4);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("192.168.1.104"), 55700);
            sock.SendTo(Encoding.Default.GetBytes("GetUpdates"), endpoint);*/

            string[] vtexportfiles = Directory.GetFiles(VTExportsPath);
            foreach (string file in vtexportfiles)
            {
                FileInfo fileinf = new FileInfo(file);
                StreamReader reader = new StreamReader(file);
                CsvReader creader = new CsvReader(reader, Utils.csvConfig);
                VTExports.Add(fileinf.Name.Substring(0, fileinf.Name.Length - 4), creader.GetRecords<VTExportModel>().ToList());
                creader.Dispose();
                creader = null;
                reader.Dispose();
                reader.Close();
                reader = null;
                fileinf = null;
            }
            vtexportfiles = null;

            StreamReader sr = new StreamReader(ProgramFilesPath + "Advanced_Queries.csv");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            /*branchLookup = (Lookup<string, Lookup<string, AdvancedQueryModel>>)
                cr.GetRecords<AdvancedQueryModel>().ToLookup(
                    q => q.Branch
                ).ToLookup(
                    ql => ql.Key,
                    ql => (Lookup<string, AdvancedQueryModel>)ql.ToLookup(
                        q => q.Schedule
                    )
                );*/
            AdvancedQueryModel[] records = cr.GetRecords<AdvancedQueryModel>().ToArray();

            cr.Dispose();
            cr = null;
            sr.Dispose();
            sr.Close();
            sr = null;

            var tempGrps = records
                .GroupBy(
                    q => q.Branch + ", " + q.State
                );

            branchZipCodes = records.GroupBy(
                r => r.State
            ).ToDictionary(
                g => g.Key,
                g => g.GroupBy(
                    r => r.ZipShort.PadLeft(5, '0'),
                    r => r.Branch + ", " + r.State
                ).ToDictionary(
                    grp => grp.Key,
                    grp => grp.First()
                )
            );

            records = null;

            branchLookup = tempGrps.ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(
                        q => q.Schedule// + (secondarySchedule.TryGetValue(q.Schedule, out string altSch) ? " - " + altSch : "")
                    ).ToDictionary(
                        g2 => g2.Key,
                        g2 => g2.ToList()
                    )
                );
            noScheduleLookup = tempGrps.ToDictionary(
                g => g.Key,
                g => g.ToList()
            );

            tempGrps = null;

            client = new Postman(Logger);
            //client.OnExecute += Client_OnExecute;
            universalClient = new Client("BOS", client);
            client.universalClient = universalClient;
        }

        public static void Client_OnExecute(object sender, ExecutionEventArgs e)
        {
            /*
            string message = " [" + e.URL + "] " + (String.IsNullOrEmpty(e.Message) ? "" : e.Message);
            
            logExecute.TryWriteLineAsync(
                String.Format(
                    LOG_FORMAT,
                    DateTime.Now.ToString(LOG_DATE_FORMAT),
                    e.ResponseCode,
                    message
                )
            );

            lstQueue.Enqueue(
                String.Format(
                    LOG_FORMAT,
                    DateTime.Now.ToString(LOG_DATE_FORMAT),
                    "POSTMAN][" + e.ResponseCode,
                    message
                )
            );*/
        }

        private void cboErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            //_queue.Enqueue(new Item(ItemType.CBO, false, new KeyValuePair<CBO, object>(CBO.ERR, null)));
        }

        private void cboNameErrors_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FindMissingCustomers()
        {
            StreamReader sr = new StreamReader(ProgramFilesPath + "Customers_Without_Documents.csv");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            List<DocCountModel> locDocCounts = cr.GetRecords<DocCountModel>().ToList();
            cr.Dispose();
            cr = null;
            sr.Dispose();
            sr.Close();
            sr = null;
        }

        private void CheckRecordsForBrokenDocuments()
        {
            StreamReader sr = new StreamReader(ProgramFilesPath + "All_Postman_Docs.csv");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            List<PossiblyBrokenDocumentModel> docs = cr.GetRecords<PossiblyBrokenDocumentModel>().ToList();
            cr.Dispose();
            cr = null;
            sr.Dispose();
            sr.Close();
            sr = null;

            List<PossiblyBrokenDocumentModel> obscureFilename = docs.FindAll(
                d => Regex.IsMatch(d.FileName, @"INPC - [\d_ -]+\.pdf") ||
                    d.FileName == "tempName.pdf"
            );
            docs = docs.Except(obscureFilename).ToList();

            List<PossiblyBrokenDocumentModel> possiblyBroken = docs.FindAll(
                d => !d.FileName.Contains(d.FirstName) || !d.FileName.Contains(d.LastName)
            );
            docs = docs.Except(possiblyBroken).ToList();

            List<PossiblyBrokenDocumentModel> definitelyNotBroken = docs.FindAll(
                d => d.FileName.Contains(d.FirstName) && d.FileName.Contains(d.LastName)
            );
            docs = docs.Except(definitelyNotBroken).ToList();

            /*List<PossiblyBrokenDocumentModel> probablyNotBroken = docs.FindAll(
                d => d.FileName.Contains(d.FirstName) || d.FileName.Contains(d.LastName)
            );
            docs = docs.Except(probablyNotBroken).ToList();*/

            StreamWriter sw = new StreamWriter(ProgramFilesPath + "possiblyBroken.csv");
            CsvWriter cw = new CsvWriter(sw);
            cw.WriteRecords(possiblyBroken);
            cw.Dispose();
            cw = null;
            sw.Flush();
            sw.Dispose();
            sw.Close();
            sw = null;

            sw = new StreamWriter(ProgramFilesPath + "definitelyNotBroken.csv");
            cw = new CsvWriter(sw);
            cw.WriteRecords(definitelyNotBroken);
            cw.Dispose();
            cw = null;
            sw.Flush();
            sw.Dispose();
            sw.Close();
            sw = null;

            sw = new StreamWriter(ProgramFilesPath + "obscureFileName.csv");
            cw = new CsvWriter(sw);
            cw.WriteRecords(obscureFilename);
            cw.Dispose();
            cw = null;
            sw.Flush();
            sw.Dispose();
            sw.Close();
            sw = null;

            sw = new StreamWriter(ProgramFilesPath + "broken.csv");
            cw = new CsvWriter(sw);
            cw.WriteRecords(docs);
            cw.Dispose();
            cw = null;
            sw.Flush();
            sw.Dispose();
            sw.Close();
            sw = null;
        }

        private void SplitInvoices(string sumsPath)
        {
            string[] files = Directory.GetFiles(SourcePath, "INPC*");
            int fileName = 0;
            if (files.Length > 0)
                fileName = files.Select(f => Convert.ToInt32(Regex.Match(f, @"INPC - (\d{1,5})\.pdf").Groups[1].Value)).OrderBy(d => d).Last();
            string[] billSums = Directory.GetFiles(sumsPath).ToList().FindAll(b => !b.Contains("SPLIT - SPLIT")).ToArray();
            for (int i = 0; i < billSums.Length; ++i)
            {
                string path = billSums[i];
                PdfReader pr = new PdfReader(path);
                int count = pr.NumberOfPages;
                for (int j = 1; j <= count; ++j)
                {
                    ++fileName;
                    //byteLengths.Add(fileName, pr.GetPageContent(j).Length);
                    FileStream file = new FileStream(SourcePath + "INPC - " + fileName.ToString() + ".pdf", FileMode.Create);
                    var doc = new iTextSharp.text.Document(pr.GetPageSizeWithRotation(j));
                    var copy = new PdfCopy(doc, file);
                    doc.Open();
                    copy.AddPage(copy.GetImportedPage(pr, j));
                    if (j < count && pr.GetPageContent(j + 1).Length < 20000)
                    {
                        copy.AddPage(copy.GetImportedPage(pr, j + 1));
                        ++j;
                    }
                    doc.Close();
                    doc = null;
                    file.Close();
                    file = null;
                    //File.WriteAllBytes(SourcePath + "INPC - " + fileName.ToString() + ".pdf", pr.GetPageContent(j));
                }
                pr.Close();
                pr = null;
                //byteLengths = byteLengths.OrderBy(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
                File.Move(path, SummariesPath + "SPLIT - " + new FileInfo(path).Name);
            }
        }

        public void TryMove(string src, string dest)
        {
            /*dest = dest.Substring(0, dest.Length - 4) + " ";
            while (File.Exists(dest + ".pdf"))
                dest += "-";*/

            for (int i = 0; i < 5; ++i)
            {
                try
                {
                    if (File.Exists(src))
                    {
                        if (File.Exists(dest))
                            TryDelete(dest);
                        File.Move(src, dest/*.Trim() + ".pdf"*/);
                    }
                }
                catch
                {
                    Thread.Sleep(gen.Next(50, 150));
                }
            }
        }

        public void TryDelete(string path)
        {
            for (int i = 0; i < 5; ++i)
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                    break;
                }
                catch
                {
                    Thread.Sleep(gen.Next(50, 150));
                }
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            object value = null;
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(
                    (val) =>
                    {
                        DocUploadSetup();
                        DownloadDocuments(val);
                        ProcessDocuments(val);
                    }
                ), value
            );

        }

        private void DownloadAllDocuments()
        {
            Outlook.Application oApp = new Outlook.Application();
            Outlook.NameSpace oNS = oApp.GetNamespace("mapi");
            oNS.Logon("vantagecontracts@insightpest.com", "!Pest6547!", true, true);
            Outlook.MAPIFolder oInbox = oNS.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            Outlook.Items oItems = oInbox.Items;

            List<Outlook.MailItem> oMsgs = oItems.Cast<Outlook.MailItem>().ToList();
            Parallel.ForEach(oMsgs, new ParallelOptions() { MaxDegreeOfParallelism = 16 }, oMsg =>
                   {
                       if (oMsg.Attachments.Count > 0)
                           oMsg.Attachments[1].SaveAsFile(@"E:\Docs\" + oMsg.Attachments[1].FileName);
                   }
            );
        }

        private void DownloadDocuments(object value)
        {
            DateTime now = DateTime.Now;
            DateTime earDate = DateTime.Parse(Environment.GetEnvironmentVariable("PDF_DATE", EnvironmentVariableTarget.Machine));
            TimeSpan dif = now - earDate;

            if (dif.TotalHours >= 24)
            {
                int fileCount = Directory.GetFiles(SourcePath).Length;

                //Outlook.Application oApp = new Outlook.Application();
                Outlook.NameSpace oNS = new Outlook.Application().GetNamespace("mapi");
                oNS.Logon("vantagecontracts@insightpest.com", "!Pest6547!", false, true);
                //Outlook.MAPIFolder oInbox = oNS.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);

                /*Outlook.Table oTable = oInbox.GetTable();
                oTable.Columns.Add("SentOn");
                oTable.Restrict("( [SentOn] >= '" + earDate.ToString("MM/dd/yyyy") + "' )");*/
                Outlook.Items oItems = oNS.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox).Items;
                oItems.Sort("SentOn", true);

                /*oNS.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox)
                    .Items.Cast<Outlook.MailItem>().ToList().FindAll(
                        m => m.SentOn.Date.CompareTo(earDate) >= 0
                    ).FindAll(m => m.Attachments.Count > 0)
                    .Select(m => m.Attachments[1])
                    .ToList().ForEach(
                        a => {
                            if (!File.Exists(SourcePath + a.FileName))
                                a.SaveAsFile(SourcePath + a.FileName);
                        }
                );*/

                //earDate.ToString("m/d/yy h:mm tt")
                //oItems = oItems.Restrict("( [SentOn] >= '" + earDate.ToString("MM/dd/yyyy") + "' )");

                /*List<Outlook.MailItem> messages = Enumerable.Range(0, oTable.GetRowCount())
                    .Select(i => (Outlook.MailItem)oNS.GetItemFromID(oTable.GetNextRow()[1])).ToList();
                oTable = null;*/
                /*List<Outlook.MailItem> messages = oItems.Cast<Outlook.MailItem>().ToList();
                oItems = null;
                messages = messages.FindAll(
                    m => m.SentOn.Date.CompareTo(earDate) >= 0
                ).Select(m => (Outlook.MailItem)oNS.GetItemFromID(m.EntryID)).ToList();*/

                /*List<Outlook.Attachment> oAttachments = messages.FindAll(m => m.Attachments != null && m.Attachments.Count > 0)
                    .Select(m => m.Attachments[1]).ToList()
                    .FindAll(a => a.FileName.Contains("signed"));
                messages = null;
                oAttachments.ForEach(a => {
                    if (!File.Exists(SourcePath + a.FileName))
                        a.SaveAsFile(SourcePath + a.FileName);
                });
                oAttachments = null;*/

                //var table = (Outlook.Table)oItems.RawTable;
                //var row = table.GetNextRow();
                //var msg = oItems.GetFirst();

                lblQueue.Enqueue(new LabelUpdate("Downloading Attachments... ", true));

                /*int count = messages.Count;
                prgQueue.Enqueue(count);
                for(int i = 0; i < count; ++i)
                {
                    Outlook.Attachment oAttach = messages[i].Attachments[1];
                    if (oAttach.FileName.Contains("signed") && !File.Exists(SourcePath + oAttach.FileName))
                        oAttach.SaveAsFile(SourcePath + oAttach.FileName);
                    oAttach = null;
                    messages[i] = null;
                    prgQueue.Enqueue(0);
                }*/

                int count = oItems.Count;
                Outlook.MailItem oMsg = (Outlook.MailItem)oItems.GetFirst();
                int counter = 0;
                while (oMsg != null)
                {
                    if (oMsg.Attachments.Count > 0 && oMsg.Attachments[1].FileName.Contains("signed"))
                    {
                        if (!File.Exists(SourcePath + oMsg.Attachments[1].FileName))
                            oMsg.Attachments[1].SaveAsFile(SourcePath + oMsg.Attachments[1].FileName);
                    }
                    oMsg.Close(Outlook.OlInspectorClose.olDiscard);
                    oMsg = null;
                    oMsg = (Outlook.MailItem)oItems.GetNext();
                    if (DateTime.Compare(earDate, oMsg.SentOn) >= 0)
                        break;

                    ++counter;
                    if (counter % 500 == 0)
                        GC.Collect();
                }
                //oMsg = null;
                oItems = null;
                //oInbox = null;
                oNS = null;
                //oApp = null;

                if (Directory.GetFiles(SourcePath).Length != fileCount)
                    Environment.SetEnvironmentVariable("PDF_Date", DateTime.Now.ToString("d"), EnvironmentVariableTarget.Machine);
            }

            lblQueue.Enqueue(new LabelUpdate("Completed.\r\n"));
            
            if (
                (now - DateTime.Parse(
                    Environment.GetEnvironmentVariable(
                        "INVOICES_DATE",
                        EnvironmentVariableTarget.Machine
                    )
                )).TotalDays > 1
            )
            {
                universalClient.GetInvoices(SourcePath);
                Environment.SetEnvironmentVariable("INVOICES_DATE", now.ToString("d"), EnvironmentVariableTarget.Machine);
                Environment.SetEnvironmentVariable("VT_REQUESTS", "0", EnvironmentVariableTarget.Machine);
            }


            /*var duplicates = pdfs.FindAll(p => p.Contains("INPC - "))
                .GroupBy(p => new FileInfo(p).Length)
                .ToList().FindAll(g => g.Count() > 1)
                .Select(
                    g => g.Select((p, i) => new {File = p, Index = i})
                    .SkipWhile(p => p.Index == 0).Select(p => p.File)
                ).SelectMany(e => e).ToList();
            pdfs = pdfs.Except(duplicates)
                .Select(p => new FileInfo(p).Name).ToList();
            duplicates.ForEach(
                d => TryDelete(d)
            );*/
        }

        private void ProcessDocuments(object value)
        {
            List<string> pdfs = Directory.GetFiles(SourcePath, "*.pdf").Select(p => new FileInfo(p).Name).ToList();
            List<string> unsigned = pdfs.FindAll(p => !p.ToLower().Contains("signed") && !p.ToLower().Contains("service") && !p.ToLower().Contains("inpc") && !p.ToLower().Contains("sa-") && !p.ToLower().Contains("invoice"));
            pdfs.RemoveAll(p => !p.ToLower().Contains("signed") && !p.ToLower().Contains("service") && !p.ToLower().Contains("inpc") && !p.ToLower().Contains("sa-") && !p.ToLower().Contains("invoice"));
            foreach (string pdf in unsigned)
            {
                TryMove(SourcePath + pdf, UnsignedPath + pdf);
            }
            unsigned = null;

            int counter = 0;
            prgQueue.Enqueue(pdfs.Count);
            lblQueue.Enqueue(new LabelUpdate("Uploading PDFs... "));

            //FixDocuments(Directory.GetFiles(ProgramFilesPath, "Broken*").FirstOrDefault());

            ParallelOptions opts = new ParallelOptions();
            opts.MaxDegreeOfParallelism = 12;
            opts.TaskScheduler = TaskScheduler.Current;

            List<AdvancedQueryModel> AdvancedSearch(AdvancedQueryModel advQuery, List<AdvancedQueryModel> results, bool brokenZip = false)
            {
                return results.FindAll(
                    q => (
                        (brokenZip ? q.Zip.StartsWith(advQuery.Zip) : q.Zip == advQuery.Zip) ||
                        (advQuery.VTID != "" && q.VTID == advQuery.VTID) ||
                        (advQuery.Mobile != "" && (q.Mobile == advQuery.Mobile ||
                        q.Phone == advQuery.Mobile)) ||
                        (advQuery.Email != "" && q.Email == advQuery.Email)
                    )
                );
            }

            ParallelLoopResult result = Parallel.ForEach(pdfs, opts, (pdf) =>
            {
                string srcPDF = SourcePath + pdf;
                string srcPath = srcPDF; 
                string destPDF = DocPath + pdf;
                string destPath = destPDF;
                string fileName = "tempFile.pdf";

                List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                Dictionary<string, string> cust = null;
                bool brokenZip = false;
                bool broken = false;

                AdvancedQueryModel advQuery;
                List<AdvancedQueryModel> queryResults = null;
                List<AdvancedQueryModel> advResults = new List<AdvancedQueryModel>();
                string branch = null;
                string altSchedule = null;
                List<AdvancedQueryModel> alt = null;

                try
                {
                    if (!File.Exists(srcPDF))
                        return;
                    if (File.Exists(destPDF))
                    {
                        File.Delete(srcPDF);
                        return;
                    }
                        
                    

                    //++total;
                    cust = client.ExtractPDF(ref srcPath);

                    fileName = cust["FileName"];
                    destPath = DocPath + fileName;
                    if (File.Exists(destPath))
                    {
                        TryDelete(destPath);
                        return;
                    }

                    srcPath = SourcePath + fileName;
                    if ((File.Exists(srcPDF) && File.Exists(srcPath) && pdf != fileName) || 
                        (File.Exists(destPDF) || File.Exists(destPath)))
                    {
                        TryDelete(pdf);
                        return;
                    }
                    else
                        TryMove(srcPDF, srcPath);
                    cust.Remove("FileName");

                    broken = Convert.ToBoolean(cust["Broken"]);
                    cust.Remove("Broken");

                    advQuery = new AdvancedQueryModel()
                    {
                        Branch = cust["Branch"],
                        Email = cust["Email"],
                        Mobile = cust["Phone"],
                        Phone = cust["Phone"],
                        Schedule = cust["Schedule"],
                        VTID = cust["VTID"],
                        Zip = cust["Zip"]
                    };

                    queryResults = new List<AdvancedQueryModel>();

                    try
                    {
                        branch = branchLookup.Keys.Single(
                            k => k.StartsWith(cust["Branch"]) ||
                                 k.EndsWith(cust["State"]));
                    }
                    catch
                    {
                        if(!stateAbbrvs.TryGetValue(cust["State"], out branch))
                            branch = branchZipCodes[cust["State"]][cust["Zip"].Split('-')[0]];   
                    }

                    advQuery.Branch = branch;

                    brokenZip = !advQuery.Zip.Contains("-");

                    //string altSchedule = secondarySchedule[advQuery.Schedule];
                    if (!String.IsNullOrEmpty(advQuery.Schedule))
                    {
                        if (branchLookup.TryGetValue(branch, out Dictionary<string, List<AdvancedQueryModel>> primary) &&
                            primary.TryGetValue(advQuery.Schedule, out List<AdvancedQueryModel> matches)
                        )
                            advResults = AdvancedSearch(advQuery, branchLookup[branch][advQuery.Schedule], brokenZip);
                        else
                            advResults = AdvancedSearch(advQuery, noScheduleLookup[branch], brokenZip);

                        /*.ToList().FindAll(
                            s => s.Key == advQuery.Schedule ||
                                s.Key == altSchedule ||
                                s.Key == "QUARTERLY"
                        ).SelectMany(
                            kv => kv.Value
                        )*/

                        if ((advResults == null || advResults.Count == 0) && secondarySchedule.TryGetValue(
                            advQuery.Schedule, out altSchedule
                        ) && branchLookup[branch].TryGetValue(
                            altSchedule, out alt
                        ))
                            advResults = AdvancedSearch(advQuery, alt, brokenZip);
                    }
                    else
                        advResults = AdvancedSearch(advQuery, noScheduleLookup[branch], brokenZip);

                    if (advResults.Count > 1)
                        advResults.RemoveAll(
                            r => (r.VTID != "" && r.VTID != advQuery.VTID) &&
                                (r.Phone != "" && r.Phone != advQuery.Mobile) &&
                                (r.Mobile != "" && r.Mobile != advQuery.Phone)
                        );

                    if (advResults.Count == 1)
                        cust.Add("LocationID", advResults.Single().LocationID);                    

                    if (!cust.TryGetValue("LocationID", out _))
                        results = client.GetLocID(cust).Result;
                    else
                        results.Add(cust);

                    if (results == null)
                    {
                        TryMove(srcPath, FailedPath + fileName);
                        cboQueue.Enqueue(
                            new ComboBoxUpdate(
                                CBO.FAIL, false, 
                                srcPath, cust, 
                                cust["FirstName"] + " " + cust["LastName"]));
                    }
                    else if (results.Count == 1)
                    {
                        //++success;
                        cust = results[0];
                        string docID = client.SetDocRecord(
                            cust, out string dupDocID, 
                            fileName.Contains("INPC") ? "INPC" : "SA");

                        if (docID != "Doc Exists")
                            client.UploadDoc(docID, srcPath);
                        else if (broken)
                        {
                            if (dupDocID != null)
                            {
                                universalClient.DeleteDocument(cust["LocationID"], dupDocID);
                                if (docID != "Doc Exists")
                                    client.UploadDoc(docID, srcPath);
                            }
                        }
                        TryMove(srcPath, destPath);
                    }
                    else if (results[0].TryGetValue("Missing", out _))
                    {
                        results[0].Remove("Missing");
                        TryMove(srcPath, GhostsPath + fileName);
                        cboQueue.Enqueue(
                            new ComboBoxUpdate(
                                CBO.GHOST, false, srcPath,
                                new KeyValuePair<Dictionary<string, string>, List<Dictionary<string, string>>>(cust, results),
                                cust["FirstName"] + " " + cust["LastName"]));
                    }
                    else
                    {
                        TryMove(srcPath, ErrorPath + fileName);
                        cust["SearchString"] = results[0]["SearchString"];
                        results.RemoveAt(0);
                        cboQueue.Enqueue(
                            new ComboBoxUpdate(
                                CBO.ERROR, false, srcPath, 
                                new KeyValuePair<Dictionary<string, string>, List<Dictionary<string, string>>>(cust, results), 
                                cust["FirstName"] + " " + cust["LastName"]));
                    }
                }
                catch (DuplicateWaitObjectException err2)
                {
                    TryMove(srcPath, DupesPath + fileName);
                }
                catch (EmptyRecordException e)
                {
                    TryMove(srcPath, EmptyRecsPath + e.Message + " - " + pdf);
                }
                catch (System.Net.Http.HttpRequestException e)
                {
                    TryMove(srcPath, TooLargePath + fileName);
                }
                catch (Exception err)
                {
                    if (err.Message.Contains("One or more"))
                        TryMove(srcPath, DupesPath + fileName);
                    else
                    {
                        cboQueue.Enqueue(new ComboBoxUpdate(CBO.CODE, false, pdf, err));
                        //_queue.Enqueue(new Item(ItemType.CBO, true, new KeyValuePair<CBO, object>(CBO.CODE, new KeyValuePair<object, object>(pdf, newErr))));
                    }
                }
                finally
                {
                    srcPDF = null;
                    srcPath = null;
                    destPDF = null;
                    destPath = null;
                    fileName = null;

                    results = null;
                    cust = null;

                    queryResults = null;
                    advResults = null;
                    branch = null;
                    altSchedule = null;
                    alt = null;

                    ++counter;
                    if (counter % 125 == 0)
                        GC.Collect();
                    prgQueue.Enqueue(0);

                    /*if (File.Exists(path))
                        TryDelete(path);

                    if (File.Exists(pdf))
                        TryDelete(pdf);*/

                    /*if (total % 50 == 0 && client.responseTimes.All(t => t.Value.Count > 5))
                    {
                        client.overallTimes = new List<long>();
                        List<string> vars = new List<string>();
                        vars.Add(success.ToString());
                        vars.Add(total.ToString());
                        vars.Add((success / total).ToString("##.00%"));
                        vars.Add("");
                        foreach (var responseTimeList in client.responseTimes)
                        {
                            var avg = responseTimeList.Value.Average();
                            client.overallTimes.Add((long)avg);
                            vars.Add(avg.ToString("##.0"));
                        }
                        vars.Add(client.overallTimes.Average().ToString("##.0"));
                        timer.Stop();
                        vars[3] = (client.overallTimes.Count / timer.Elapsed.TotalSeconds).ToString("##.00");

                        _queue.Enqueue(new Item(ItemType.DIA, false, String.Join(", ", vars)));
                    }*/
                }
            });

            SpinWait.SpinUntil(() => result.IsCompleted);
            lblQueue.Enqueue(new LabelUpdate("Completed.\r\n"));
        }

        private void FixDocuments(string path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return;

            ParallelOptions opts = new ParallelOptions();
            opts.MaxDegreeOfParallelism = 8;
            opts.TaskScheduler = TaskScheduler.Current;

            StreamReader sr = new StreamReader(path);
            CsvReader cr = new CsvReader(
                sr,
                Utils.csvConfig
            );
            var brokenDates = cr.GetRecords<BrokenDocumentsModel>()
            .GroupBy(d => DateTime.Parse(d.DocumentDate))
            .OrderByDescending(g => g.Key).ToArray();
            List<FileInfo> files = Directory.GetFiles(
                DocPath, "*", SearchOption.AllDirectories
            ).Select(f => new FileInfo(f)).ToList()
            .FindAll(f => f.Directory.Name != "TooLarge");

            foreach (var brokenDate in brokenDates)
            {
                files.RemoveAll(f => f.CreationTime.Date.CompareTo(brokenDate.Key) > 0);
                foreach (var document in brokenDate)
                {
                    FileInfo file = null;
                    try
                    {
                        if (client.DocExists(document.LocationID, out _, document.DocumentName))
                            universalClient.DeleteDocument(document.LocationID, document.DocumentID);
                        else
                        {
                            List<FileInfo> matches = files.FindAll(f => f.Name.Contains(document.Name));
                            string[] names = document.Name.Split(' ');
                            if (matches.Count == 0)
                                matches = files.FindAll(
                                    f => f.Name.StartsWith(document.DocumentName) &&
                                    (f.Name.Contains(names[0]) || f.Name.Contains(names[0]))
                                );

                            if (matches.Count == 1)
                            {
                                file = matches.Single();
                                client.UploadDoc(document.DocumentID, matches.Single().FullName);
                            }


                        }
                    }
                    catch (System.Net.Http.HttpRequestException e)
                    {
                        TryMove(file.FullName, TooLargePath + file.Name);
                    }
                }
            }

            cr.Dispose();
            sr.Dispose();
            sr.Close();
            TryDelete(path);
        }

        private void lblProcess_Click(object sender, EventArgs e)
        {
            
        }

        private void btnNotes_Click(object sender, EventArgs e)
        {
            //ThreadPool.QueueUserWorkItem(new WaitCallback((o) => UploadNotes()));

            /*docsThread = new Thread(() => UploadNotes());
            docsThread.IsBackground = true;
            docsThread.Priority = ThreadPriority.AboveNormal;
            docsThread.Start();

            return;*/
        }

        /*private void UploadNotes()
        {
            StreamReader sr = new StreamReader(NotesPath + "Vantage Corporate notes - 2017-08-31.csv");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            List<VTNoteModel> notes = cr.GetRecords<VTNoteModel>().ToList();
            List<KeyValuePair<VTNoteModel, string[]>> customers = notes.GroupBy(n => n.PocomosID)
            .Select(
                g => new KeyValuePair<VTNoteModel, string[]>(
                    g.First(), g.Select(n => n.Note).ToArray()
                )
            ).ToList();

            // Testing purposes
            //File.WriteAllLines(NotesPath + "addresses.txt", customers.Select(c => c.Key.Address));

            Parallel.ForEach(
                customers,
                new ParallelOptions() { MaxDegreeOfParallelism = 16 },
                customer =>
                {
                    string[] names = null;
                    string[] locInfo = null;
                    Dictionary<string, string> cust = null;

                    try
                    {
                        names = Postman.ExtractMatches(Regex.Match(customer.Key.Name, Postman.NamesPat));
                        locInfo = Postman.ExtractMatches(Regex.Match(customer.Key.Address, VTAddressPat));
                        cust = new Dictionary<string, string>()
                        {
                            {"FirstName", names[0]},
                            {"SpouseName", names[1]},
                            {"LastName", names[2]}
                        };

                        if (!String.IsNullOrWhiteSpace(locInfo[2]))
                        {
                            if (!String.IsNullOrWhiteSpace(locInfo[0]))
                            {
                                VerifyResponse fullInfo = client.upsClient.AddressValidation(locInfo[0], locInfo[2]);
                                cust.Add("City", fullInfo.City);
                                cust.Add("State", fullInfo.State);
                                cust.Add("Zip", fullInfo.Zip5 + "-" + fullInfo.Zip4);
                            }
                            else
                            {
                                KeyValuePair<string, string> cityState = client.upsClient.FixInfo(locInfo[2]);
                                cust.Add("City", cityState.Key);
                                cust.Add("State", cityState.Value);
                                cust.Add("Zip", locInfo[2]);
                                cust["Zip"] = client.upsClient.GetZipExt(cust);
                            }
                        }


                    }
                    catch (Exception e)
                    {

                    }
                }
            );
        }*/

        private void filog_fileOk(object sender, CancelEventArgs e)
        {
            this.Activate();
            //string[] files = 
        }

        public static string ExceptionToString(Exception e, int tabCount = 0)
        {
            StringBuilder sb = new StringBuilder("");

            sb.Append("MESSAGE:\t\t");
            if (!String.IsNullOrWhiteSpace(e.Message))
                sb.AppendLine(e.Message);
            else if (e.InnerException != null && !String.IsNullOrWhiteSpace(e.InnerException.Message))
                sb.AppendLine(e.InnerException.Message);
            else
                sb.AppendLine("None.");

            sb.Append(new string('\t', tabCount));
            sb.Append("SOURCE:\t\t");
            sb.AppendLine(e.Source);

            sb.Append(new string('\t', tabCount));
            sb.Append("STACKTRACE:\t");
            sb.AppendLine(e.StackTrace.Replace("\n", "\n" + new string('\t', tabCount + 3)));

            sb.Append(new string('\t', tabCount));
            sb.Append("TARGETSITE:\t");
            sb.AppendLine(e.TargetSite.Name);

            return sb.ToString();
        }

        #region lblThread
        private void lblThread_DoWork(object sender, DoWorkEventArgs e)
        {
            LabelUpdate update = (LabelUpdate)e.Argument;
            string time = DateTime.Now.ToString(LOG_DATE_FORMAT);
            string logEntry = null;
            if (update.reset)
            {
                logEntry = String.Format(
                    LOG_FORMAT, time, "UICHANGE",
                    "Resetting lblProcess... ~ \"" +
                    update.update + "\""
                );
                UIThread.Invoke(() => lblProcess.Text = update.update);
            }
            else
            {
                logEntry = String.Format(
                    LOG_FORMAT, time, "STATUS",
                    update.update
                );
                UIThread.Invoke(() => lblProcess.Text += update.update);
            }

            while (true)
            {
                try
                {
                    //logMain.WriteLineAsync(logEntry);
                }
                catch
                {
                    Thread.Sleep(gen.Next(50, 150));
                }
            }
            
        }

        public struct LabelUpdate
        {
            public bool reset { get; set; }
            public string update { get; set; }

            public LabelUpdate(string u, bool r = false)
            {
                reset = r;
                update = u;
            }
        }
        #endregion

        #region cboThread
        private void cboThread_DoWork(object sender, DoWorkEventArgs e)
        {
            ComboBoxUpdate update = (ComboBoxUpdate)e.Argument;
            string time = DateTime.Now.ToString(LOG_DATE_FORMAT);
            string logEntry = null;

            string pdf = new FileInfo(update.pdf).Name;
            pdf = pdf.Substring(0, pdf.Length - 4);
            string fileName = null;
            string fileContent = null;

            switch (update.type)
            {
                case CBO.CODE:
                    Exception error = (Exception)update.update;
                    fileName = CodeLogsPath + pdf + ".txt";
                    logEntry = String.Format(
                        LOG_FORMAT, time, "ERROR",
                        update.pdf + " ~ " + 
                        fileName.Replace(LogsPath, @"\") + 
                        "\r\n" + new string('\t', 5) + 
                        ExceptionToString(error, 5)
                    );
                    fileContent = JsonConvert.SerializeObject(error, Formatting.Indented);
                    break;

                case CBO.FAIL:
                    Dictionary<string, string> customer = (Dictionary<string, string>)update.update;
                    fileName = FailedLogsPath + update.selected + ".txt";
                    logEntry = String.Format(
                        LOG_FORMAT, time, "DEBUG",
                        update.pdf + " ~ " +
                        fileName.Replace(LogsPath, @"\") +
                        "\r\n" + new string('\t', 5) + 
                        String.Join(
                            "\r\n" + new string('\t', 5),
                            customer.Select(kv => kv.Key + " :=: " + kv.Value)
                        )
                    );
                    fileContent = JsonConvert.SerializeObject(customer, Formatting.Indented);
                    break;

                case CBO.GHOST:
                    KeyValuePair<Dictionary<string, string>, List<Dictionary<string, string>>> results = (KeyValuePair<Dictionary<string, string>, List<Dictionary<string, string>>>)update.update;
                    fileName = GhostLogsPath + update.selected + ".txt";
                    logEntry = String.Format(
                        LOG_FORMAT, time, "DEBUG",
                        update.pdf + " ~ " + 
                        fileName.Replace(LogsPath, @"\") +
                        "\n" + new string('\t', 5) +
                        String.Join(
                            "\r\n" + new string('\t', 5),
                            results.Key.Select(kv => kv.Key + " :=: " + kv.Value)
                        )
                    );
                    fileContent = JsonConvert.SerializeObject(results.Key, Formatting.Indented) + "\r\n\r\n" + JsonConvert.SerializeObject(results.Value, Formatting.Indented);
                    break;

                case CBO.ERROR:
                    KeyValuePair<Dictionary<string, string>, List<Dictionary<string, string>>> erroredResults = (KeyValuePair<Dictionary<string, string>, List<Dictionary<string, string>>>)update.update;
                    string searchString = erroredResults.Key["SearchString"];
                    erroredResults.Key.Remove("SearchString");
                    fileName = ErrorLogsPath + update.selected + ".txt";
                    logEntry = String.Format(
                        LOG_FORMAT, time, "DEBUG",
                        update.pdf + " ~ " + 
                        fileName.Replace(LogsPath, @"\") + 
                        " ~ " + searchString + "\n" + new string('\t', 5) +
                        String.Join(
                            "\n" + new string('\t', 5),
                            erroredResults.Key.Select(kv => kv.Key + " :=: " + kv.Value)
                        )
                    );
                    fileContent = "Search String:\t" + searchString + "\r\n" + JsonConvert.SerializeObject(erroredResults.Key, Formatting.Indented) + "\r\n\r\n" + JsonConvert.SerializeObject(erroredResults.Value, Formatting.Indented);
                    break;
            }

            fileContent = pdf + "\r\n" + fileContent;

            if (!File.Exists(fileName))
                File.WriteAllText(fileName, fileContent);
            else
                File.AppendAllText(fileName, "\r\n\r\n" + new string('~', 16) + "\r\n\r\n" + fileContent);
            //logMain.TryWriteLineAsync(logEntry);
        }
        

        public struct ComboBoxUpdate
        {
            public CBO type { get; set; }
            public object update { get; set; }
            public bool textbox { get; set; }
            public string selected { get; set; }
            public string pdf { get; set; }

            public ComboBoxUpdate(CBO t, bool txt, string file, object upd = null, string select = null)
            {
                type = t;
                textbox = txt;
                pdf = file;
                update = upd;
                selected = select;
            }
        }

        private void cboCodeErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCurLog.Text = File.ReadAllText(CodeLogsPath + cboCodeErrors.Text + ".txt");
        }

        private void cboGhosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCurLog.Text = File.ReadAllText(GhostLogsPath + cboGhosts.Text + ".txt");
        }

        private void cboFailed_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCurLog.Text = File.ReadAllText(FailedLogsPath + cboFailed.Text + ".txt");
        }

        #endregion

        private void prgThread_DoWork(object sender, DoWorkEventArgs e)
        {
            int arg = (int)e.Argument;
            if (arg == 0)
                UIThread.Invoke(() => prgBar.Increment(1));
            else if (arg < 0)
                UIThread.Invoke(() => prgBar.Increment(Math.Abs(arg)));
            else
            {
                UIThread.Invoke(() => prgBar.Value = 0);
                UIThread.Invoke(() => prgBar.Maximum = arg);
            }
        }

        private void lstThread_DoWork(object sender, DoWorkEventArgs e)
        {
            string arg = (string)e.Argument;
            if (arg == "RESET")
                UIThread.Invoke(() => lstConsole.Items.Clear());
            else
                UIThread.Invoke(() => lstConsole.Items.Add(arg));
        }

        private void btnCheckNums_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => CheckPhoneNumbers()));
        }

        private void CheckPhoneNumbers()
        {
            List<PhoneVerifModel> phones = null;
            using (StreamReader sr = new StreamReader(@"C:\Users\ZACH-GAMING\Downloads\Phone_Number_Verification.csv"))
            {
                using (CsvReader cr = new CsvReader(sr, Utils.csvConfig))
                {
                    phones = cr.GetRecords<PhoneVerifModel>().ToList();
                }
            }
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ofdPatch.InitialDirectory = @"C:\Users\ZACH-GAMING\Downloads";
            SpinWait.SpinUntil(() => ofdPatch.ShowDialog() == DialogResult.OK);
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => RunPatch(ofdPatch.FileName)));
        }

        private static void RunPatch(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            CsvReader cr = new CsvReader(sr);
            List<FileFormatSkeleton> ffs = cr.GetRecords<FileFormatSkeleton>().ToList();

            Parallel.ForEach(ffs, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, skel =>
                {

                }
            );
        }

        private void btnCheckEmails_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => CheckEmails()));

            /*Parallel.ForEach(emails, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, email =>
               {
                   client.Patch(
                       email.LocationID,
                       new List<PatchOperation>()
                       { new PatchOperation(
                           "replace", "/userdefinedfields/Email?",
                            EmailValidatorSimple.Validate(email.Email)
                       )}
                    );
               });*/

            /*SmtpClient mailClient = new SmtpClient();
            mailClient.Port = 25;
            mailClient.Credentials = new MailCredentials();
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.UseDefaultCredentials = false;
            mailClient.Host = "smtp.insightpest.com";
            mailClient.EnableSsl = true;

            MailMessage testMsg = new MailMessage("z.johnson@insightpest.com", "zachary99johnson912@gmail.com", "test", "test");
            testMsg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            mailClient.Send(testMsg);*/
        }

        private void CheckEmails()
        {
            StreamReader sr = new StreamReader(ProgramFilesPath + "Email_Verification.csv");
            CsvReader cr = new CsvReader(sr);

            SimpleEmailModel[] emails = cr.GetRecords<SimpleEmailModel>().ToArray();
            cr.Dispose();
            sr.Dispose();
            sr.Close();

            if (client == null)
                client = new Postman(Logger);

            SimpleEmailModel[] gmailEmails = emails.ToList().FindAll(a => a.Email.EndsWith("gmail.com")).ToArray();

            Parallel.ForEach(gmailEmails, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, gmailEmail =>
            {
                string result = EmailValidator.VerifyEmail(gmailEmail.Email).Result.ToString();
                client.Patch(
                    gmailEmail.LocationID,
                    new List<PatchOperation>()
                    {
                        new PatchOperation(
                            "replace", "/userdefinedfields/Email?",
                            result
                        )
                    }
                );
            });
        }

        private List<CSVLocationModel> ExtractRecon()
        {
            List<CSVLocationModel> locations = null;
            if (!File.Exists(ReconLog))
            {
                if (client == null)
                    client = new Postman(Logger);

                StreamReader reader = new StreamReader(@"C:\Users\ZACH-GAMING\Downloads\Recon Report 8-30-17.xlsx - Recon 3-1 to 8\15.csv");
                CsvReader creader = new CsvReader(reader, Utils.csvConfig);
                List<ReconModel> customers = creader.GetRecords<ReconModel>().ToList();
                creader.Dispose();
                reader.Dispose();
                reader.Close();

                customers.RemoveAll(c => !String.IsNullOrWhiteSpace(c.Reconciled));

                List<LocationModel> locationsTemp = customers.Select(c => client.GetInfo(c.Location).Result).ToList();
                List<LocationModel> failed = locationsTemp.FindAll(l => l.LocationID == null || l.LocationID < 1000);
                locationsTemp = locationsTemp.Except(failed).ToList();
                File.WriteAllLines(ProgramFilesPath + "FailedRecon.txt", failed.Select(l => l.LocationCode));

                StreamWriter sw = new StreamWriter(ReconLog);
                CsvWriter cw = new CsvWriter(sw, Utils.csvConfig);
                cw.WriteRecords(locations);

                cw.Dispose();
                sw.Dispose();
                sw.Close();
            }

            StreamReader sr = new StreamReader(ReconLog);
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            locations = cr.GetRecords<CSVLocationModel>().ToList();
            cr.Dispose();
            sr.Dispose();
            sr.Close();
            
            
            return locations;
        }

        private void RunRecon(List<CSVLocationModel> locations)
        {
            if (universalClient == null)
                universalClient = new Client("MIN", client);

            StreamReader sr = new StreamReader(@"C:\Users\ZACH-GAMING\Downloads\Recon Report 8-30-17.xlsx - Recon 3-1 to 8\15.csv");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            Dictionary<string, ReconModel> customers = cr.GetRecords<ReconModel>().Distinct(new ReconComparer())
                .ToList().FindAll(c => String.IsNullOrWhiteSpace(c.Reconciled))
                .ToDictionary(c => c.Location, c => c);
            cr.Dispose();
            sr.Dispose();
            sr.Close();

            //customers.RemoveAll(c => !String.IsNullOrWhiteSpace(c.Reconciled));
            Dictionary<string, string> reconciled = new Dictionary<string, string>();
            List<ReconModel> failedRecons = new List<ReconModel>();

            int count = 0;
            foreach(CSVLocationModel location in locations)
            {
                try
                {
                    ReconModel recon = new ReconModel();
                    recon = customers[location.LocationCode];
                    if (
                        ((!recon.Name.Contains(location.FirstName) ||
                            !recon.Name.Contains(location.LastName)) && !recon.Name.ToUpper().Contains("AND") && !location.FirstName.Contains("/"))
                        )
                    {
                        if ((compInf.Compare(location.Phone, recon.Phone, options) != 0 &&
                            compInf.Compare(location.MobilePhone, recon.Phone, options) != 0) &&
                            compInf.Compare(location.EMail, recon.Primaryemail, options) != 0)
                        {
                            reconciled.Add(location.LocationCode, "Location Code Incorrect");
                        }
                    }
                    ++count;
                }
                catch
                {

                }
            }

            Parallel.ForEach(locations, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, location =>
                {
                    ReconModel recon = new ReconModel(); 
                    try
                    {
                        recon = customers[location.LocationCode];
                        if (
                            (!recon.Name.Contains(location.FirstName) ||
                                !recon.Name.Contains(location.LastName)) &&
                            (compInf.Compare(location.Phone, recon.Phone, options) != 0 &&
                                compInf.Compare(location.MobilePhone, recon.Phone, options) != 0) &&
                                compInf.Compare(location.EMail, recon.Primaryemail, options) != 0
                            )
                        {
                            reconciled.Add(location.LocationCode, "Location Code Incorrect");
                            return;
                        }

                        string VTID = "";// location.UserDefinedFields.Single(f => f.Caption.Contains("VT Cust")).Value;
                        List<VTExportModel> matches = VTExports[location.Branch].FindAll(e => e.CustomerID == VTID);

                        if (matches.Count == 0)
                        {
                            matches = VTExports[location.Branch].FindAll(
                            e => (compInf.Compare(recon.Name, e.Name, options) == 0 ||
                                compInf.Compare(recon.Name, location.LastName, options) == 0) &&
                                (compInf.Compare(e.Phone, recon.Phone, options) == 0 ||
                                compInf.Compare(recon.Primaryemail, e.Primaryemail, options) == 0)
                            );
                        }
                        else if (matches.Count > 1)
                            throw new DuplicateNameException();
                        

                        if (matches.Count == 1)
                        {
                            VTExportModel vt = matches.Single();

                            List<InvoiceListModel> invoices = JsonConvert.DeserializeObject<List<InvoiceListModel>>(
                                client.client.Execute(
                                    new RestRequest(
                                        String.Format(
                                            "Locations/{0}/serviceHistory",
                                            location.LocationID
                                        ), Method.GET
                                    )
                                ).Content
                            );

                            Dictionary<string, string> vtpatch = new Dictionary<string, string>();

                            if (compInf.Compare(vt.Phone, location.Phone, options) != 0 && compInf.Compare(vt.Phone, location.MobilePhone, options) != 0)
                                vtpatch.Add("service_address[contactAddress][phone]", String.IsNullOrWhiteSpace(location.MobilePhone) ? location.Phone : location.MobilePhone);


                        }

                        
                    }
                    catch
                    {
                        failedRecons.Add(recon);
                    }
                    

                    /*string VTNum = location.UserDefinedFields.Single(f => f.Caption.Contains("VT Cust")).Value;
                    string VTID = "";*/

                    //Customer cust = new Customer();
                    /*universalClient.VT.GetCustomer();
                    if (!String.IsNullOrWhiteSpace(VTNum))
                        VTID = universalClient.VT.AdvancedSearch(
                            new Dictionary<string, string>()
                            { {"accountid", VTID} }, location.Branch
                        ).Single().id;*/

                    
                }
            );
        }

        private void btnAddCusts_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => AddCusts()));
        }

        private void AddCusts()
        {
            if (client == null)
                client = new Postman(Logger);
            Slingshot.client = client;
            PestPacGeoCoding.client.CookieContainer = universalClient.PPI.Cookies;
            string JSON = File.ReadAllText(ProgramFilesPath + "SlingshotJSONTest.txt");
            LocationModel[] locations = Slingshot.UploadCustomers(JSON);
            JSON = null;
            string[] codes = locations.Select(l => l.LocationCode).ToArray();
        }

        private void btnZillow_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => RunZillow()));
        }

        private static PatchEntry[] ZillowFields = new PatchEntry[]
        {
            new PatchEntry("Lot SqFt", "Lot", true),
            new PatchEntry("Bedrooms", "Beds"),
            new PatchEntry("Square Ft", "Floorsize", true),
            new PatchEntry("Roof Type", "Rooftype"),
            new PatchEntry("Bathrooms", "Baths"),
            new PatchEntry("Flooring", "Flooring"),
            new PatchEntry("Materials", "Exteriormaterial"),
            new PatchEntry("Year Built", "YearBuilt"),
            new PatchEntry("Home Value", "HomeValue")
        };

        

        private void RunZillow()
        {
            Dictionary<string, Dictionary<string, string>> ZillowDictionaries = Directory.GetFiles(ZillowDictionariesPath)
                .ToDictionary(
                    f => new string(new FileInfo(f).Name.TakeWhile(c => c != '.').ToArray()),
                    f => File.ReadAllLines(f)
                    .Select(l => l.Split(new string[] { " :=: "}, StringSplitOptions.None))
                    .ToDictionary(
                        l => l[0],
                        l => l[1]
                    )
                );

            if (client == null)
                client = new Postman(Logger);

            StreamReader sr = new StreamReader(ProgramFilesPath + "MedianHouseholdIncomes.csv");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            Dictionary<string, KeyValuePair<string, string>> incomes = cr.GetRecords<ZipCodeIncome>()
                .ToDictionary(
                    i => ZillowClient.rgxNumeric.Replace(i.ZipCode, ""), 
                    i => new KeyValuePair<string, string>(
                        i.MedianHouseholdIncome, 
                        i.MedianHousingValue
                    )
                );

            cr.Dispose();
            cr = null;
            sr.Dispose();
            sr.Close();
            sr = null;

            sr = new StreamReader(ProgramFilesPath + "ZIllow_To-Dos.csv");
            cr = new CsvReader(sr);
            List<ZillowUpdateModel> customers = cr.GetRecords<ZillowUpdateModel>().ToList();

            //TODO : FIX
            /*if (File.Exists(ZillowLogsPath + "MissingAddresses.txt") && File.Exists(MissingZipsPath))
            {
                string[] missingZips = File.ReadAllLines(MissingZipsPath);
                string[] errors = File.ReadAllLines(ZillowLogsPath + "MissingAddresses.txt");
                customers.RemoveAll(c => missingZips.Contains(c.Zip) || errors.Contains(c.Address + ", " + c.Zip));
                errors = null;
                missingZips = null;
            }*/
            StreamWriter sw = new StreamWriter(ZillowLogsPath + "ErroneousAddresses.txt", true);
            ZillowClient zillow = new ZillowClient(Logger);

            KeyValuePair<string, ZillowUpdateModel[]>[] zipCodeGroups = customers.GroupBy(
                c => c.Zip
            ).Select(
                g => new KeyValuePair<string, ZillowUpdateModel[]>(
                    g.Key, g.ToArray()
                )
            ).ToArray();
            customers = null;

            cr.Dispose();
            cr = null;
            sr.Dispose();
            sr.Close();
            sr = null;

            System.Reflection.PropertyInfo[] props = typeof(HomeFacts).GetProperties();
            HomeFacts defaultDetails = new HomeFacts();

            Parallel.ForEach(zipCodeGroups, new ParallelOptions() { MaxDegreeOfParallelism = 8 }, zipCodeGroup =>
            //foreach (var zipCodeGroup in zipCodeGroups)
            {
                if (!incomes.TryGetValue(zipCodeGroup.Key, out KeyValuePair<string,string> income) || income.Key == "-1")
                {
                    //TryAppend(MissingZipsPath, zipCodeGroup.Key + "\n");
                    return;
                }

                int count = zipCodeGroup.Value.Length;
                for (int i = 0; i < count; ++i)
                {
                    ZillowUpdateModel customer = zipCodeGroup.Value[i];
                    /*Dictionary<string, string[]> details = zillow.GetDetails(customer.Address, customer.Zip);
                    if (details == null)
                        client.Patch(customer.OrderID, new List<PatchOperation>() { new PatchOperation("replace", "/userdefinedfields/Median Inc", income.Key) }, "ServiceOrders");
                    else
                        client.Patch(
                            customer.OrderID,
                            Postman.CreatePatch(
                                ZillowFields,
                                details,
                                new Dictionary<string, string>()
                                {{"Median Inc", income.Key } }
                            ), "ServiceOrders"
                        );
                    */

                    /*System.Xml.Linq.XDocument doc = zillow.GetSearchResults(
                        customer.Address,
                        customer.Zip
                    );

                    if (doc == null)
                    {
                        sw.WriteLineAsync(customer.OrderID);
                        continue;
                    }

                    string zpid = doc.Descendants("zpid").First().Value;
                    HomeFacts details = zillow.GetUpdatedPropertyDetails(zpid);
                    if (details.Equals(defDetails))
                        details = zillow.GetDeepSearchResults(customer.Address, customer.Zip);*/

                    HomeFacts details = zillow.GetDeepSearchResults(customer.Address, customer.Zip);
                    if (details.Equals(defaultDetails))
                    {
                        sw.WriteLineAsync(customer.Address + ", " + customer.Zip);
                        continue;
                    }

                    List<PatchOperation> ops = new List<PatchOperation>(); 
                    for(int j = 0; j < 11; ++j)
                    {
                        string name = ZillowClient.UserFields[props[j].Name];
                        object val = props[j].GetValue(details);
                        if (val == null)
                            continue;
                        string value = val.ToString();
                        val = null;

                        if (value.Length > 10)
                        {
                            if (ZillowDictionaries.TryGetValue(name, out Dictionary<string, string> dict) && dict.TryGetValue(value, out string newValue))
                            {
                                value = newValue;
                                newValue = null;
                                dict = null;
                            }
                            else
                            {
                                TryAppend(ZillowLogsPath + name + ".txt", value + "\r\n");
                                value = value.Substring(0, 9) + ".";
                            }
                        }

                        ops.Add(new PatchOperation(
                            "replace",
                            "/userdefinedfields/" + name,
                            value));

                        name = null;
                        value = null;
                    }

                    ops.Add(new PatchOperation("replace", "/userdefinedfields/Median Inc", income.Key));
                    client.Patch(customer.OrderID, ops, "ServiceOrders");
                    ops = null;
                }
            });
        }

        public void TryAppend(string fileName, string content)
        {
            int numtries = 0;
            while(true)
            {
                try
                {
                    File.AppendAllText(fileName, content);
                }
                catch
                {
                    ++numtries;
                    Thread.Sleep(gen.Next(50, 150));
                    if (numtries > 5)
                        break;
                }
            }
        }

        public struct ZipCodeIncome
        {
            public string ZipCode { get; set; }
            public string TotalPopulation { get; set; }
            public string MedianHouseholdIncome { get; set; }
            public string MedianHousingValue { get; set; }
            public string CityName { get; set; }
            public string State { get; set; }
        }

        public struct ZillowUpdateModel
        {
            public string OrderID { get; set; }
            public string Address { get; set; }
            public string Zip { get; set; }
        }

        public class ConsoleWriter : TextWriter
        {
            public override Encoding Encoding { get; }

            public override void WriteLine(string value)
            {
                lstQueue.Enqueue(value);
            }

            public override void WriteLine(string format, params object[] arg)
            {
                lstQueue.Enqueue(String.Format(format, arg));
            }
        }

        private void btnSurvey_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(
                    path => UploadSurveyResults(path)
                ), @"C:\DocUploads\Program Files\NeuralNetworks\Surveys\QTPC.csv"
            );
        }

        private void UploadSurveyResults(object path)
        {
            SogoClient sogo = new SogoClient(Logger);
            sogo.LoadSurvey(path.ToString());
            sogo.UploadNotes();
            sogo.Dispose();
        }
    }

    public struct MailCredentials : System.Net.ICredentialsByHost
    {
        public NetworkCredential GetCredential(string host, int port, string authenticationType)
        {
            return new NetworkCredential("z.johnson@insightpest.com", "@iw9ba1ul2sfb?", "insightpest.com");
        }
    }

    public struct ReconComparer : IEqualityComparer<ReconModel>
    {
        public bool Equals(ReconModel x, ReconModel y)
        {
            return x.Location == y.Location;
        }

        public int GetHashCode(ReconModel obj)
        {
            return obj.GetHashCode();
        }
    }

    

}
