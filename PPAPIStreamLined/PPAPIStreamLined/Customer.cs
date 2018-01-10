using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PPAPIStreamLined
{
    class LocationDocument
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

    class NoteModel
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
            this.Associations.LocationID = Convert.ToInt32(locID);
        }
    }

    class NoteAssociationModel
    {
        [JsonProperty("LocationID")]
        public int LocationID { get; set; }
    }

    class Document
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
        [JsonProperty("IsTechPhoto")] public string IsTechPhoto { get; set; }
    }
    class IncludeOn
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
}
