using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace PestPac_Utility
{
    public class NoteModel
    {
        public string NoteDate { get; set; }
        public string NoteCode { get; set; }
        public string Note { get; set; }
        public string CreatedByUser { get; set; }
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
        public int LocationID { get; set; }
    }
}
