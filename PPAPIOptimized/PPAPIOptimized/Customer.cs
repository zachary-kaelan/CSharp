using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using RestSharp.Serializers;

namespace PPAPIOptimized
{
    public class LocationDocument
    {
        [JsonProperty("LocationID")]
        public string LocationID { get; set; }
        [JsonProperty("Date")]
        public string Date { get; set; }
        [JsonProperty("FileName")]
        public string FileName { get; set; }
        [JsonProperty("IncludeOn")]
        public IncludeOn IncludeOn { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Tags")]
        public string Tags { get; set; }
        [JsonProperty("URL")]
        public string URL { get; set; }

        public LocationDocument(string LocID, string name)
        {
            this.LocationID = LocID;
            this.Date = DateTime.Now.ToString("d");
            this.FileName = "tempName.pdf";
            this.IncludeOn = new IncludeOn();
            this.Name = name;
            this.Tags = "Postman";
            this.URL = "";
        }
    }

    public struct ReconModel
    {
        public string Location { get; set; }
        public string Name { get; set; }
        public string Office { get; set; }
        public string Phone { get; set; }
        public string Primaryemail { get; set; }
        public string StreetAddress { get; set; }
        public string SalesStatus { get; set; }
        public string SalesPerson { get; set; }
        public string NumberofServices { get; set; }
        public string InitialPrice { get; set; }
        public string RecurringPrice { get; set; }
        public string ContractValue { get; set; }
        public string StartDate { get; set; }
        public string Reconciled { get; set; }
        public string Initials { get; set; }
        public string ManagerQCofRecon { get; set; }
    }

    public struct CSVLocationModel
    {
        public string LocationID { get; set; }
        public string LocationCode { get; set; }
        public string Branch { get; set; }
        public string BillToID { get; set; }
        public string Company { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string Salutation { get; set; }
        public string SalutationName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string PhoneExtension { get; set; }
        public string AlternatePhone { get; set; }
        public string AlternatePhoneExtension { get; set; }
        public string Fax { get; set; }
        public string FaxExtension { get; set; }
        public string MobilePhone { get; set; }
        public string MobilePhoneExtension { get; set; }
        public string EMail { get; set; }
        public string Website { get; set; }
        public string Active { get; set; }
        public string IncludeInMailings { get; set; }
        public string Prospect { get; set; }
        public string EnteredDate { get; set; }
        public string ContactDate { get; set; }
        public string ContactCode { get; set; }
        public string County { get; set; }
        public string Division { get; set; }
        public string Source { get; set; }
        public string TaxCode { get; set; }
        public string TaxRate { get; set; }
        public string Type { get; set; }
        public string MapCode { get; set; }
        public string Comment { get; set; }
        public string Instructions { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string GLCode { get; set; }
        public string DoNotGeocode { get; set; }
        public string Builder { get; set; }
        public string Subdivision { get; set; }
        public string TaxExemptNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderExpirationDate { get; set; }
        public string InternalIdentifier { get; set; }
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

        public NoteModel(string locID, string note, string code = "GEN")
        {
            this.NoteDate = DateTime.Now.ToString("d");
            this.NoteCode = code;
            this.Note = note;
            this.CreatedByUser = "ADMN";
            this.Associations = new NoteAssociationModel();
            this.Associations.LocationID = Convert.ToInt32(locID);
        }
    }

    public class NoteAssociationModel
    {
        [JsonProperty("LocationID")]
        public int LocationID { get; set; }
    }

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

    [System.Serializable]
    class EmptyRecordException : Exception
    {
        public EmptyRecordException()
        {
            
        }

        public EmptyRecordException(string message) : base(message)
        {

        }

        public EmptyRecordException(string message, Exception inner) : base(message, inner)
        {

        }

        protected EmptyRecordException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {

        }
    }

    
    public struct Address
    {
        public string FirmName;
        public string Address1;
        public string Address2;
        public string City;
        public string State;
        public string Zip5;
        public string Zip4;
    }

    public struct VerifyResponse
    {
        public string Address2;
        public string City;
        public string State;
        public string Zip5;
        public string Zip4;
    }

    public struct ZipInfo
    {
        public string Zip5;
        public string City;
        public string State;
    }

    public struct PhoneVerifModel
    {
        //public string LocationCode { get; set; }
        public string Phone { get; set; }
        //public string PhoneExt { get; set; }
        public string LocationID { get; set; }
        public string Mobile { get; set; }
    }

    public struct PatchOperation
    {
        public string op;
        public string path;
        public string value;

        public PatchOperation(string operation, string fieldPath, string newValue)
        {
            op = operation;
            path = fieldPath;
            value = newValue;
        }
    }

    public struct VTNoteModel
    {
        public string PocomosID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
    }

    public struct BrokenDocumentsModel
    {
        public string LocationID { get; set; }
        public string Name { get; set; }
        public string DocumentName { get; set; }
        public string DocumentID { get; set; }
        public string DocumentFileName { get; set; }
        public string DocumentDate { get; set; }
    }

    public struct DocCountModel
    {
        public string LocationID { get; set; }
        public string Branch { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int DocCount { get; set; }
    }

    public struct PossiblyBrokenDocumentModel
    {
        public string LocationID { get; set; }
        public string Branch { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string FileName { get; set; }
        public string DocumentDate { get; set; }
    }

    public struct AdvancedQueryModel
    {
        public string Branch { get; set; }
        public string LocationID { get; set; }
        public string Zip { get; set; }
        public string ZipShort { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Schedule { get; set; }    // TODO : Remove
        public string VTID { get; set; }
        public string State { get; set; }
    }

    public struct FileFormatSkeleton
    {
        public string L_LocationCode { get; set; }
        public string L_BillToCode { get; set; }
        public string L_Company { get; set; }
        public string L_LName { get; set; }
        public string L_FName { get; set; }
        public string L_Title { get; set; }
        public string L_Address { get; set; }
        public string L_Address2 { get; set; }
        public string L_Zip { get; set; }
        public string L_City { get; set; }
        public string L_State { get; set; }
        public string L_Salutation { get; set; }
        public string L_SalutationName { get; set; }
        public string L_Phone { get; set; }
        public string L_PhoneExt { get; set; }
        public string L_AltPhone { get; set; }
        public string L_AltPhoneExt { get; set; }
        public string L_Fax { get; set; }
        public string L_FaxExt { get; set; }
        public string L_Mobile { get; set; }
        public string L_MobileExt { get; set; }
        public string L_EMail { get; set; }
        public string L_URL { get; set; }
        public string L_MapCode { get; set; }
        public string L_Division { get; set; }
        public string L_Type { get; set; }
        public string L_Builder { get; set; }
        public string L_Source { get; set; }
        public string L_County { get; set; }
        public string L_Subdivision { get; set; }
        public string L_ContactDate { get; set; }
        public string L_ContactCode { get; set; }
        public string L_Comment { get; set; }
        public string L_Directions { get; set; }
        public string L_Latitude { get; set; }
        public string L_Longitude { get; set; }
        public string L_Branch { get; set; }
        public string L_TaxCode { get; set; }
        public string L_Liaison1 { get; set; }
        public string L_Liaison2 { get; set; }
        public string SS_ServiceCode { get; set; }
        public string SS_Description { get; set; }
        public string SS_Quantity { get; set; }
        public string SS_UnitPrice { get; set; }
        public string SS_Schedule { get; set; }
        public string SS_WorkTime { get; set; }
        public string SS_TimeRange { get; set; }
        public string SS_Duration { get; set; }
        public string SS_StartDate { get; set; }
        public string SS_CancelDate { get; set; }
        public string SS_CancelReason { get; set; }
        public string SS_PriceIncrDate { get; set; }
        public string SS_ExpirationDate { get; set; }
        public string SS_PONumber { get; set; }
        public string SS_POExpirationDate { get; set; }
        public string SS_Source { get; set; }
        public string SS_Route { get; set; }
        public string SS_Division { get; set; }
        public string SS_Measurement { get; set; }
        public string SS_MeasurementType { get; set; }
        public string SS_CommissionStartDate { get; set; }
        public string SS_CommissionEndDate { get; set; }
        public string SS_Tech1 { get; set; }
        public string SS_Tech2 { get; set; }
        public string SS_Tech3 { get; set; }
        public string SS_Tech4 { get; set; }
        public string SS_Comment { get; set; }
        public string SS_NotificationDays { get; set; }
        public string SS_Terms { get; set; }
        public string SS_LastGeneratedDate { get; set; }
        public string SO_InitialServiceCode { get; set; }
        public string SO_Price { get; set; }
        public string SO_WorkDate { get; set; }
        public string SO_WorkTime { get; set; }
        public string SO_Duration { get; set; }
        public string SO_GLCode { get; set; }
        public string SO_Taxable { get; set; }
        public string SO_Tech1 { get; set; }
        public string SO_Route { get; set; }
        public string SO_Lock { get; set; }

        public static Dictionary<string, string> ReturnTemplate()
        {
            return new Dictionary<string, string>()
            {
                {"L_LocationCode", "" },
                {"L_BillToCode", "" },
                {"L_Company", "" },
                {"L_LName", "" },
                {"L_FName", "" },
                {"L_Title", "" },
                {"L_Address", "" },
                {"L_Address2", "" },
                {"L_Zip", "" },
                {"L_City", "" },
                {"L_State", "" },
                {"L_Salutation", "" },
                {"L_SalutationName", "" },
                {"L_Phone", "" },
                {"L_PhoneExt", "" },
                {"L_AltPhone", "" },
                {"L_AltPhoneExt", "" },
                {"L_Fax", "" },
                {"L_FaxExt", "" },
                {"L_Mobile", "" },
                {"L_MobileExt", "" },
                {"L_EMail", "" },
                {"L_URL", "" },
                {"L_MapCode", "" },
                {"L_Division", "" },
                {"L_Type", "" },
                {"L_Builder", "" },
                {"L_Source", "" },
                {"L_County", "" },
                {"L_Subdivision", "" },
                {"L_ContactDate", "" },
                {"L_ContactCode", "" },
                {"L_Comment", "" },
                {"L_Directions", "" },
                {"L_Latitude", "" },
                {"L_Longitude", "" },
                {"L_Branch", "" },
                {"L_TaxCode", "" },
                {"L_Liaison1", "" },
                {"L_UserDef1", "" },
                {"L_UserDef2", "" },
                {"L_UserDef3", "" },
                {"L_UserDef5", "" },
                {"L_UserDef8", "" },
                {"L_UserDef10", "" },
                {"L_UserDef11", "" },
                {"L_UserDef12", "" },
                {"L_UserDef13", "" },
                {"L_UserDef24", "" },
                {"L_UserDef25", "" },
                {"L_UserDef26", "" },
                {"L_UserDef27", "" },
                {"L_UserDef28", "" },
                {"L_UserDef29", "" },
                {"L_UserDef30", "" },
                {"L_UserDef31", "" },
                {"SS_ServiceCode", "" },
                {"SS_Description", "" },
                {"SS_Quantity", "" },
                {"SS_UnitPrice", "" },
                {"SS_Schedule", "" },
                {"SS_WorkTime", "" },
                {"SS_TimeRange", "" },
                {"SS_Duration", "" },
                {"SS_StartDate", "" },
                {"SS_CancelDate", "" },
                {"SS_CancelReason", "" },
                {"SS_PriceIncrDate", "" },
                {"SS_ExpirationDate", "" },
                {"SS_PONumber", "" },
                {"SS_POExpirationDate", "" },
                {"SS_Source", "" },
                {"SS_Route", "" },
                {"SS_Division", "" },
                {"SS_Measurement", "" },
                {"SS_MeasurementType", "" },
                {"SS_CommissionStartDate", "" },
                {"SS_CommissionEndDate", "" },
                {"SS_Tech1", "" },
                {"SS_Tech2", "" },
                {"SS_Tech3", "" },
                {"SS_Tech4", "" },
                {"SS_Comment", "" },
                {"SS_NotificationDays", "" },
                {"SS_Terms", "" },
                {"SS_LastGeneratedDate", "" },
                {"SO_InitialServiceCode", "" },
                {"SO_Price", "" },
                {"SO_WorkDate", "" },
                {"SO_WorkTime", "" },
                {"SO_Duration", "" },
                {"SO_GLCode", "" },
                {"SO_Taxable", "" },
                {"SO_Tech1", "" },
                {"SO_Route", "" },
                {"SO_Lock", "" }
            };
        }
    }
}
