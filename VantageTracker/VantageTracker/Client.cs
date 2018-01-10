using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using RestSharp;
using Newtonsoft.Json;

namespace VantageTracker
{
    public class Client
    {
        public VTNav VT { get; set; }
        public IPPAPI PP { get; set; }
        public PPNav PPI { get; set; }
        public ReportWriter RW { get; set; }

        public Client(string initBranch, IPPAPI postman)
        {
            this.VT = new VTNav(initBranch);
            //this.PP = new PPAPI();
            this.PPI = new PPNav();
        }

        public void TransferNotes(string VTQuery)
        {
            (Dictionary<string, string> cust, List<NoteModel> notes) = this.VT.GetNotes(this.VT.FindCust(VTQuery));
            string id = this.PPI.FindCust(cust["Phone"], cust["VTID"])[0]["LocationID"];
            for (int i = 0; i < notes.Count; ++i)
            {
                /*notes[i].Associations = new NoteAssociationModel();
                notes[i].Associations.LocationID = id;
                notes[i].Note = notes[i].Note.Insert(0, "VTNotes : ");*/
                this.PP.UploadNote(id, notes[i].Note, "VTNotes : ", PPAPIOptimized.NoteCode.GEN);
            }
        }

        public void CreateReportWriterInstance()
        {
            string redirection = this.PPI.StartReportWriter();
            this.RW = new ReportWriter(this.PPI.Cookies, redirection);
        }

        public void GetInvoices(string path, string tempPath = null)
        {
            DateTime now = DateTime.Now;
            if (String.IsNullOrWhiteSpace(tempPath))
                tempPath = System.IO.Path.GetTempPath() + @"\VantageTracker\";

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            Console.WriteLine("Getting invoices...");
            int VT_REQUESTS = Convert.ToInt32(Environment.GetEnvironmentVariable("VT_REQUESTS", EnvironmentVariableTarget.Machine));
            Console.WriteLine("   {0} VT requests.", VT_REQUESTS);

            DateTime start = DateTime.Parse(Environment.GetEnvironmentVariable("INVOICES_DATE", EnvironmentVariableTarget.Machine));
            TimeSpan diff = (now - start);
            int weeks = diff.TotalDays > 7 ? Convert.ToInt32(diff.TotalDays / 7) : 1;

            if (VT_REQUESTS < 17 || (now - DateTime.Parse(Environment.GetEnvironmentVariable("EARLIEST_INVOICES", EnvironmentVariableTarget.Machine))).TotalHours >= 1.0)
            {
                if (diff.TotalHours >= 24)
                {
                    Environment.SetEnvironmentVariable("VT_REQUESTS", "0", EnvironmentVariableTarget.Machine);
                    this.VT.StartGetInvoices(start);
                    this.VT.DownloadSummaries(tempPath, weeks);
                }
            }
            else if (Convert.ToInt32(VT_REQUESTS) > 0)
                this.VT.DownloadSummaries(tempPath, weeks, true);

            VTNav.SplitInvoices(tempPath, path);
        }

        public void UpdateTechs(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            CsvReader cr = new CsvReader(sr);
            var records = cr.GetRecords<TechUpdateModel>().ToList();

            Parallel.ForEach(
                records,
                new ParallelOptions() { MaxDegreeOfParallelism = 12 },
                record =>
                {
                    this.PPI.UpdateSS(
                        record.Service,
                        techID: this.PP.GetTechID(record.INVTECH),
                        techName: record.INVTECH
                    );
                }
            );
        }
        public void DeleteDocument(string LocationID, string DocumentID)
        {
            Document doc = JsonConvert.DeserializeObject<Document>(
                this.PP.client.Execute(
                    new RestRequest(
                        String.Format("Documents/{0}?type=LocationDocument", DocumentID)
                    )
                ).Content
            );

            this.PPI.POST("delDoc",
                new Dictionary<string, string>() {
                    { "Mode", "Delete"},
                    { "LocationID", LocationID},
                    { "From", ""},
                    { "LocDocumentID", DocumentID},
                    { "SetupID", ""},
                    { "AWSDocumentID", DocumentID},
                    { "AWSFile", ""},
                    { "AWSFileURL", ""},
                    { "Name", doc.Name},
                    { "Date", doc.Date.Substring(0, doc.Date.IndexOf('T'))},
                    { "Tags", doc.Tags},
                    { "InclSO", ""},
                    { "InclWeb", ""},
                    { "DocType", "AWSFile"},
                    { "FileName", doc.FileName},
                    { "NewFile", ""}
                }, DocumentID, LocationID
            );
        }
    }

    public interface IPPAPI
    {
        RestClient client { get; set; }
        Dictionary<string, string> ExtractPDF(ref string filePath);
        //string[] FixBrokenInvoice(string accountID, string branch, ref string fileName);
        //string[] ExtractMatches(Match match);
        //string InfoToFileName(string type, string[] temp);
        void GetToken();
        List<Dictionary<string, string>> AdvSearch(Dictionary<string, string> query);
        bool NoteExists(string locID, string note);
        Task<List<Dictionary<string, string>>> GetLocID(Dictionary<string, string> query);
        //bool CheckForDuplicates(List<Dictionary<string, string>> res);
        List<Dictionary<string, string>> SearchResults(List<Dictionary<string, string>> res, Dictionary<string, string> query);
        string SetDocRecord(Dictionary<string, string> cust, out string docID, string name = "SA");
        //Document UploadDoc(string DocID, string filePath);
        //bool Patch(string locID, List<PatchOperation> ops);
        bool DocExists(string locID, out string docID, string name = "SA");
        void UploadNote(string locID, string note, string prefix, PPAPIOptimized.NoteCode code);
        void PatchNumber(string locID, string number);
        //bool CheckInternet();
        string GetTechID(string user);
    }

    public struct TechUpdateModel
    {
        public string LocationID { get; set; }
        public string LocationCode { get; set; }
        public string Service { get; set; }
        public string SSTECH { get; set; }
        public string INVService { get; set; }
        public string INVTECH { get; set; }
    }

    public interface PatchOperation
    {
        string op { get; set; }
        string path { get; set; }
        string value { get; set; }
    }
}
