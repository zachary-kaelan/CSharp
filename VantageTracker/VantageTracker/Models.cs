using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace VantageTracker
{
    public struct PastDueModel
    {
        public string Branch { get; set; }
        public string LocationID { get; set; }
        public string PPLocationCode { get; set; }
        public double Balance { get; set; }
        public string Email { get; set; }
        public string VantageCustomer { get; set; }
        public string Phone { get; set; }
        public string DaysOld { get; set; }
        public string initialJobDate { get; set; }
    }

    public struct ChargeBackModel
    {
        public string Phone { get; set; }
        public string PrimaryEmail { get; set; }
        public string CustomerID { get; set; }
        public string Office { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
    }

    public class NoteModel
    {
        [JsonProperty("NoteDate")]
        public string NoteDate { get; set; }
        [JsonProperty("NoteCode")]
        public string NoteCode { get; set; }
        [JsonProperty("Note")]
        public string Note { get; set; }
        [JsonProperty("CreatedByUser")]
        public string CreatedByUser { get; set; }
        [JsonProperty("Assocations")]
        public NoteAssociationModel Associations { get; set; }

        public NoteModel(string locID, string note)
        {
            this.NoteDate = DateTime.Now.ToString("d");
            this.NoteCode = "GEN";
            this.Note = note;
            this.CreatedByUser = "Postman";
            this.Associations = new NoteAssociationModel();
            this.Associations.LocationID = locID;
        }

        public NoteModel()
        {
            this.NoteCode = "GEN";
            this.CreatedByUser = "Postman";
        }

    }

    public class NoteAssociationModel
    {
        [JsonProperty("LocationID")]
        public string LocationID { get; set; }
    }

    public struct DeleteNotesModel
    {
        public string NoteID { get; set; }
        public string LocationID { get; set; }
        public string Note { get; set; }
    }

    /*public struct VTServiceInfoModel
    {
        public string _token;
        public string[] pests;
        public string[] specialtyPests;

        public string contractColor;
        public string contract_autoRenew;
        public string contract_dateStart;
        public string contract_foundByType;
        public string contract_purchaseOrderNumber;
        public string contract_salesperson;
        public string contract_salesStatus;
        public string contract_taxcode;

        public string county;
        public string dayOfTheWeek;
        public string defaultJobDuration;
        public string mapCode;
        public string preferredTime;

        public string rescheduleInitialJob_dateScheduled;
        public string rescheduleInitialJob_timeScheduled;
        public string rescheduleInitialJob_futureOptions;
        public string rescheduleInitialJob_futureJobs_futureWeek;
        public string rescheduleInitialJob_futureJobs_futureDay;
        public string rescheduleInitialJob_futureJobs_futureTime;

        public string technician;
        public string weekOfTheMonth;

    }*/

    /*public struct VTServiceAddrModel
    {
        public string companyName;
        public string firstName;
        public string lastName;

        public string contactAddress_street;
        public string contactAddress_suite;
        public string contactAddress_city;
        public string contactAddress_region;
        public string contactAddress_postalCode;
        public string contactAddress_phone;
        public string contactAddress_altPhone;

        public string deliverEmail;
        public string emailAddress;
        public string secondaryEmailAddresses;
        public string accountType;
        public string _token;
    }*/

    public class Document
    {
        [JsonProperty("DocumentType")] public string DocumentType { get; set; }
        [JsonProperty("DocumentID")] public string DocumentID { get; set; }
        [JsonProperty("Name")] public string Name { get; set; }
        [JsonProperty("Date")] public string Date { get; set; }
        [JsonProperty("FileName")] public string FileName { get; set; }
        [JsonProperty("URL")] public string URL { get; set; }
        [JsonProperty("StartingEffectiveDate")] public string StartingEffectiveDate { get; set; }
        [JsonProperty("EndingEffectiveDate")] public string EndingEffectiveDate { get; set; }
        [JsonProperty("OrderID")] public string OrderID { get; set; }
        [JsonProperty("IsTechPhoto")] public bool IsTechPhoto { get; set; }
        [JsonProperty("FormData")] public string FormData { get; set; }
        [JsonProperty("IncludeOn")] public IncludeOn IncludeOn { get; set; }
        [JsonProperty("Tags")] public string Tags { get; set; }
    }
    public class IncludeOn
    {
        [JsonProperty("AutomatedInspectionReports")]
        public bool AutomatedInspectionReports { get; set; }
        [JsonProperty("LogIt")]
        public bool LogIt { get; set; }
        [JsonProperty("MobileApp")]
        public bool MobileApp { get; set; }
        [JsonProperty("Portal")]
        public bool Portal { get; set; }
        [JsonProperty("SeviceOrders")]
        public bool SeviceOrders { get; set; }

        public IncludeOn()
        {
            this.AutomatedInspectionReports = true;
            this.LogIt = true;
        }
    }

    public struct VTSearchResults
    {
        public Customer[] aaData;
        public int iTotalDisplayRecords;
        public int iTotalRecords;
        public int sEcho;
    }

    public struct Customer
    {
        public string address;
        public string contract_id;
        public string email;
        public string id;
        public string last_service_date;
        public string name;
        public string next_service_date;
        public string phone;
        public string postal_code;
        public string status;
        public string balance;
    }

    public struct Invoice
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public string AccountID { get; set; }
        public string JobID { get; set; }
        public string InvoiceID { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Total { get; set; }
        public string Balance { get; set; }
        public string Paid { get; set; }
        public string Technician { get; set; }

        public string TechNote { get; set; }
    }

    public struct SimpleInvoice
    {
        public string LocationID { get; set; }
        public string InvoiceID { get; set; }
    }

    public struct ReportExecutionArgs
    {
        ReportExecutionArg[] args;
        public static Dictionary<string, object> ReportExecutionSettings = new Dictionary<string, object>()
        {
            {"BrowserType", "Chrome"},
            {"Language", "en-us,en-us-getting-started,en-us-tooltips"},
            {"SessionNum", "1"},
            {"ShowErrorDetail", true},
            {"IsAdmin", false},
            {"DevicePixelRatio", 1},
            {"IsExceptionServerEvent", false}
        };

        public ReportExecutionArgs(string reportData)
        {
            args = new ReportExecutionArg[]
            {
                new ReportExecutionArg(reportData),
                new ReportExecutionArg(),
                new ReportExecutionArg(),
                new ReportExecutionArg(null, false, "Client.ClientParameterValueList"),
                new ReportExecutionArg("csv"),
                new ReportExecutionArg(false),
                new ReportExecutionArg(null, false, "Client.ClientReportAdjustments"),
                new ReportExecutionArg(JsonConvert.SerializeObject(ReportExecutionSettings), false, "Settings")
            };
        }

        public ReportExecutionArgs(string reportID, string reportPath)
        {
            args = new ReportExecutionArg[]
            {
                new ReportExecutionArg(reportPath),
                new ReportExecutionArg("csv"),
                new ReportExecutionArg(false),
                new ReportExecutionArg(false),
                new ReportExecutionArg(),
                new ReportExecutionArg(null, false, "Client.ClientParameterValueList"),
                new ReportExecutionArg("csv"),
                new ReportExecutionArg(false),
                new ReportExecutionArg(null, false, "Client.ClientReportAdjustments"),
                new ReportExecutionArg(JsonConvert.SerializeObject(ReportExecutionSettings), false, "Settings")
            };
        }
    }

    public struct ReportExecutionArg
    {
        string className;
        bool isArray;
        object value;

        public ReportExecutionArg(object val = null, bool arr = false, string name = "")
        {
            className = name;
            isArray = arr;
            value = val;
        }
    }

    public struct ClientExpressReport
    {
        public string Id;
        public string Theme;
        public string Options;
    }

    public static class ReportOptions
    {
        public static string ExportType = "default";
        public static string IncludeSetupInfo = "No";
        public static bool ShowExecuteForm = false;
        public static string NoDataRenderType = "ShowMessage";
        public static bool ShowGrid = true;
        public static bool SuppressFormatting = false;
        public static string FilterExecutionWindow = "default";
    }

    public struct DefaultExportModel
    {
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string PrimaryEmail { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string Status { get; set; }
        public string LastServiceDate { get; set; }
        public string NextServiceDate { get; set; }
    }

    public struct VTExportModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Primaryemail { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        /*public string Status { get; set; }
        public string LastServiceDate { get; set; }
        public string NextServiceDate { get; set; }*/
        public string CustomerID { get; set; }
        public string Office { get; set; }
        //public string CompanyName { get; set; }
        //public string BillingName { get; set; }
        public string SecondaryEmails { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        /*public string BillingAddress { get; set; }
        public string BillingZip { get; set; }
        public string BillingCity { get; set; }
        public string BillingStateProvince { get; set; }
        public string SalesStatus { get; set; }
        public string ContractDates { get; set; }
        public string Salesperson { get; set; }
        public string MarketingType { get; set; }
        public string TaxCode { get; set; }
        public string MapCode { get; set; }
        public string Type { get; set; }
        public string Autopay { get; set; }*/
        public string ServiceType { get; set; }
        public string ServiceFrequency { get; set; }
        /*public string CreationDate { get; set; }
        public string SignUpDate { get; set; }
        public string InitialPrice { get; set; }
        public string RecurringPrice { get; set; }
        public string QuotedInitialPrice { get; set; }
        public string PreferredTech { get; set; }
        public string Problems { get; set; }
        public string SpecialProblems { get; set; }
        public string Tags { get; set; }
        public string Balance { get; set; }
        public string CCorACHonfile { get; set; }
        public string CurrentContracts { get; set; }
        public string InitialJobDate { get; set; }
        public string LastTechnician { get; set; }
        public string Isparentlocation { get; set; }
        public string Ischildlocation { get; set; }*/

    }
}
