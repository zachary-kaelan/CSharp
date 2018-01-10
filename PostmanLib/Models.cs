using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PPLib;
using RGX.Utils;

namespace PostmanLib
{
    public enum PDFType
    {
        INPC,
        SA,
        SA2,
        Unknown
    };

    public struct Customer
    {
        public PDFType Type { get; set; }
        public Names Names { get; set; }
        public LocInfo LocInfo { get; set; }

        public InvoiceNumbers VTNumbers { get; set; }
        public SA2Info SA2Info { get; set; }
        public SAInfo SAInfo { get; set; }

        public string LocationID { get; set; }
        public string Path { get; set; }
        public bool Advanced { get; set; }

        public void Load(Dictionary<string, string> dict)
        {
            Names = new Names(dict);
            LocInfo = new LocInfo(dict);

            switch (Type)
            {
                case PDFType.INPC:
                    VTNumbers.Load(dict);
                    break;

                case PDFType.SA2:
                    SA2Info.Load(dict);
                    break;
            }
        }

        public void LoadAll(Dictionary<string, string> dict)
        {
            this.Load(dict);
            LocationID = dict.Extract("LocationID");
            Path = dict.Extract("Path");
        }

        public bool MissingVTNumbers()
        {
            return Type == PDFType.INPC && String.IsNullOrWhiteSpace(VTNumbers.InvoiceID);
        }

        public override string ToString()
        {
            if (Type == PDFType.Unknown)
                throw new NotImplementedException();
            return String.Join(
                " - ",
                (Advanced ? "a" : "") + Type.ToString(),
                Names.ToString(),
                LocInfo.ToString(),
                (Type == PDFType.INPC ? 
                    VTNumbers.ToString() : (
                        Type == PDFType.SA ? 
                            SAInfo.ToString() : 
                            SA2Info.ToString()
                    )
                )
            );
        }
    }

    public struct Names
    {
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }
        public string SpouseName { get; set; }

        public Names(Dictionary<string, string> dict) : this()
        {
            Load(dict);
        }

        public void Load(Dictionary<string, string> dict)
        {
            FirstName = dict.Extract("FirstName");
            MiddleInitial = dict.Extract("MiddleInitial");
            LastName = dict.Extract("LastName");
            SpouseName = dict.Extract("SpouseName");
            dict = null;
        }
        
        public override string ToString()
        {
            return String.Concat(
                FirstName,
                String.IsNullOrWhiteSpace(MiddleInitial) ? "" : " " + MiddleInitial + " ",
                String.IsNullOrWhiteSpace(SpouseName) ? " " : (" & " + SpouseName + " "),
                LastName
            );
        }
    }

    public struct LocInfo
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        
        public LocInfo(Dictionary<string, string> dict) : this()
        {
            Load(dict);
        }

        public void Load(Dictionary<string, string> dict)
        {
            Address = dict.Extract("Address");
            City = dict.Extract("City");
            State = dict.Extract("State");
            Zip = dict.Extract("Zip");
            dict = null;
        }

        private const string FORMAT = "{0}, {1}, {2} {3}";
        public override string ToString()
        {
            return String.Format(FORMAT, Address, City, State, Zip);
        }
    }

    public struct InvoiceNumbers
    {
        public string VTID { get; set; }
        public string InvoiceID { get; set; }
        public double Balance { get; set; }

        public InvoiceNumbers(Dictionary<string, string> dict) : this()
        {
            Load(dict);
        }

        public void Load(Dictionary<string, string> dict)
        {
            VTID = dict.Extract("VTID");
            InvoiceID = dict.Extract("InvoiceID");
            Balance = Convert.ToDouble(dict.Extract("Balance"));
            dict = null;
        }
        
        public override string ToString()
        {
            return String.Join(
                ", ", 
                VTID, 
                InvoiceID, 
                Balance
            );
        }
    }

    public struct SA2Info
    {
        public string Schedule { get; set; }
        public double Total { get; set; }
        public string Email { get; set; }
        public string Phone { get => Phone; set => PPRGX.PHONE.Replace(value, "$1-$2-$3"); }

        public SA2Info(Dictionary<string, string> dict) : this()
        {
            Load(dict);
        }

        public void Load(Dictionary<string, string> dict)
        {
            Schedule = dict.Extract("Schedule");
            Total = double.TryParse(dict.Extract("Total"), out double total) ? total : 0.0;
            Email = dict.Extract("Email");
            Phone = dict.Extract("Phone");
            dict = null;
        }

        public override string ToString()
        {
            return String.Join(
                ", ",
                Schedule,
                Total.ToString(),
                Email,
                Phone
            );
        }
    }

    public struct SAInfo
    {
        public string Schedule { get; set; }
        public double Subtotal { get; set; }
        public double TaxRate { get; set; }

        public SAInfo(Dictionary<string,string> dict) : this()
        {
            Load(dict);
        }

        public void Load(Dictionary<string, string> dict)
        {
            Schedule = dict.Extract("Schedule");
            Subtotal = double.TryParse(dict.Extract("Subtotal"), out double subtotal) ? subtotal : 0.0;
            TaxRate = double.TryParse(dict.Extract("TaxRate"), out double taxrate) ? taxrate : 0.0;
            dict = null;
        }

        public override string ToString()
        {
            return String.Join(
                ", ",
                Schedule,
                Subtotal.ToString(),
                TaxRate.ToString()
            );
        }
    }
}
