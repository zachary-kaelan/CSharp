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

// PestPac New
//using PPPPRGX.FilenameAdvanced;
//using PPPPRGX.SA;
using ZachLib;

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
        /*private static readonly Filename FILENAME_INFO = new Filename();
        private static readonly PPRGX.PDF.INPC.Info INVOICE_INFO = new PPRGX.PDF.INPC.Info();
        private static readonly VTNumbers VANTAGE_NUMBERS = new VTNumbers();
        private static readonly PPRGX.PDF.SA.Info SA_INFO = new PPRGX.PDF.SA.Info();
        private static readonly NewInfo NEW_SA_INFO = new NewInfo();
        private static readonly Schedule SA_SCHEDULE = new Schedule();
        private static readonly PPRGX.Utils.HasFilenameInfo HAS_FILENAME_INFO = new PPRGX.Utils.HasFilenameInfo();

        private static readonly FilenameInfoType FILENAME_INFO_TYPE = new FilenameInfoType();
        private static readonly INPC aINPC_FILENAME = new INPC();
        private static readonly SA aSA_FILENAME = new SA();
        private static readonly SA2 aSA2_FILENAME = new SA2();*/

        private static readonly Dictionary<Schedule, string[]> schedules = new Dictionary<Schedule, string[]>()
        {
            {Schedule.Q1_JAJO, new string[4] {"Jan", "Apr", "Jul", "Oct"} },
            {Schedule.Q2_FMAN, new string[4] {"Feb", "May", "Aug", "Nov"} },
            {Schedule.Q3_MJSD, new string[4] {"Mar", "Jun", "Sep", "Dec"} },
            {Schedule.MOSQUITOS, new string[4] {"Jun", "Jul", "Aug", "Sep"} }
        };

        public static Customer ExtractPDF(ref string path)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            Customer customer = new Customer();
            
            if (PPRGX.FILENAME_INFO_TYPE.IsMatch(name))
            {
                Dictionary<string, string> type = PPRGX.FILENAME_INFO_TYPE.ToDictionary(name);
                customer.Advanced = type["Advanced"] == "a";
                customer.Type = Enum.TryParse<PDFType>(type["Type"], true, out PDFType pdftype) ? pdftype : PDFType.Unknown;

                if (customer.Advanced)
                {
                    switch(customer.Type)
                    {
                        case PDFType.INPC:
                            customer.Load(PPRGX.FILENAME_INPC.ToDictionary(name));
                            break;

                        case PDFType.SA:
                            customer.Load(PPRGX.FILENAME_SA.ToDictionary(name));
                            break;

                        case PDFType.SA2:
                            customer.Load(PPRGX.FILENAME_SA2.ToDictionary(name));
                            break;

                        default:
                            var exception = new UnknownPDFTypeException(name);
                            exception.Data.Add("TypeDict", type);
                            throw exception;
                            
                    }
                }
                else if (PPRGX.FILENAME_INFO_TYPE.IsMatch(name))
                {
                    customer.Load(PPRGX.FILENAME_INFO.ToDictionary(name));
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
                        PPRGX.PDF_INPC_VTNUMBERS.ToDictionary(
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

                
            }

            return new Customer();
        }
    }
}
