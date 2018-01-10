using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Third Party
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

// PestPac Old
using PPLib;
using RGX.PDF;
using RGX.PDF.INPC;
using RGX.PDF.SA;

// PestPac New
using PPRGX.FilenameAdvanced;
using PPRGX.SA;

namespace PostmanLib
{
    public enum Schedule
    {
        Q1_JAJO,
        Q2_FMAN,
        Q3_MJSD,
        MOSQUITOS
    };

    public static class PDFInfoExtractor
    {
        private static readonly Filename FILENAME_INFO = new Filename();
        private static readonly RGX.PDF.INPC.Info INVOICE_INFO = new RGX.PDF.INPC.Info();
        private static readonly VTNumbers VANTAGE_NUMBERS = new VTNumbers();
        private static readonly RGX.PDF.SA.Info SA_INFO = new RGX.PDF.SA.Info();
        private static readonly NewInfo NEW_SA_INFO = new NewInfo();
        private static readonly Schedule SA_SCHEDULE = new Schedule();
        private static readonly RGX.Utils.HasFilenameInfo HAS_FILENAME_INFO = new RGX.Utils.HasFilenameInfo();

        private static readonly FilenameInfoType FILENAME_INFO_TYPE = new FilenameInfoType();
        private static readonly INPC aINPC_FILENAME = new INPC();
        private static readonly SA aSA_FILENAME = new SA();
        private static readonly SA2 aSA2_FILENAME = new SA2();

        private static readonly Dictionary<Schedule, string[]> schedules = new Dictionary<Schedule, string[]>()
        {
            {Schedule.Q1_JAJO, new string[4] {"Jan", "Apr", "Jul", "Oct"} },
            {Schedule.Q2_FMAN, new string[4] {"Feb", "May", "Aug", "Nov"} },
            {Schedule.Q3_MJSD, new string[4] {"Mar", "Jun", "Sep", "Dec"} },
            {Schedule.MOSQUITOS, new string[4] {"Jun", "Jul", "Aug", "Sep"} }
        };

        public Customer ExtractPDF(ref string path)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            Customer customer = new Customer();
            
            if (FILENAME_INFO_TYPE.IsMatch(name))
            {
                Dictionary<string, string> type = FILENAME_INFO_TYPE.ToDictionary(name);
                customer.Advanced = type["Advanced"] == "a";
                customer.Type = Enum.TryParse<PDFType>(type["Type"], true, out PDFType pdftype) ? pdftype : PDFType.Unknown;

                if (customer.Advanced)
                {
                    switch(customer.Type)
                    {
                        case PDFType.INPC:
                            customer.Load(aINPC_FILENAME.ToDictionary(name));
                            break;

                        case PDFType.SA:
                            customer.Load(aSA_FILENAME.ToDictionary(name));
                            break;

                        case PDFType.SA2:
                            customer.Load(aSA2_FILENAME.ToDictionary(name));
                            break;

                        default:
                            var exception = new UnknownPDFTypeException(name);
                            exception.Data.Add("TypeDict", type);
                            throw exception;
                            
                    }
                }
                else if (HAS_FILENAME_INFO.IsMatch(name))
                {
                    customer.Load(FILENAME_INFO.ToDictionary(name));
                }
                else
                {
                    var exception = new UnknownPDFTypeException(name);
                    exception.Data.Add("TypeDict", type);
                    throw exception;
                }

                if (customer.MissingVTNumbers())
                {
                    PdfReader r = new PdfReader(path);
                    customer.VTNumbers.Load(
                        VANTAGE_NUMBERS.ToDictionary(
                            PdfTextExtractor.GetTextFromPage(
                                r, 1, new SimpleTextExtractionStrategy()
                            )
                        )
                    );
                    r.Close();
                    r = null;
                }
                type.Clear();
                type = null;
            }
            else
            {
                long length = new FileInfo(path).Length;

                PdfReader reader = new PdfReader(path);
                int numPages = reader.NumberOfPages;
                string page1 = PdfTextExtractor.GetTextFromPage(reader, 1, new SimpleTextExtractionStrategy());
                string page2 = numPages > 1 ? PdfTextExtractor.GetTextFromPage(reader, 2, new SimpleTextExtractionStrategy()) : "";
                reader.Close();
                reader = null;

                if ()
            }
        }
    }
}
