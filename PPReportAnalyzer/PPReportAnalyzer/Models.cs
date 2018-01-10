using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;

namespace PPReportAnalyzer
{
    public interface Customer
    {
        string LocationID { get; set; }
        string LocationCode { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Street { get; set; }
        string City { get; set; }
        string State { get; set; }
        string Zip { get; set; }
        string Active { get; set; }
    }

    public struct SignatureModel
    {
        public string FileName { get; set; }
        public string APAY { get; set; }
        public string SignedSA { get; set; }
        public string Branch { get; set; }
    }

    public static class PdfImages
    {
        //private static ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageEncoders().Single(e => e.CodecName.Contains("JPEG"));
        //private static EncoderParameters prms = new EncoderParameters(1) { Param = new EncoderParameter[] { new EncoderParameter(Encoder.Compression, 0) } };

        private static List<PdfObject> Find(PdfDictionary page)
        {
            PdfDictionary xobj = ((PdfDictionary)PdfReader.GetPdfObject(
                ((PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES)))
                .Get(PdfName.XOBJECT)
            ));

            List<PdfObject> output = new List<PdfObject>();

            foreach(PdfName name in xobj.Keys)
            {
                PdfObject obj = xobj.Get(name);
                if (obj.IsIndirect())
                {
                    PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                    PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                    //PdfName filter = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.FILTER));

                    if (PdfName.IMAGE.Equals(type))
                        output.Add(obj);
                    else if (PdfName.FORM.Equals(type) || PdfName.GROUP.Equals(type))
                        output = output.Concat(Find(tg)).ToList();
                }
            }

            return output;

            //return (null, null);
        }

        public static (int, List<Tuple<int, int/*, int, int*/>>) ExtractDebug(string src = null)
        {
            try
            {
                if (src == null)
                    return (0, new List<Tuple<int, int>>());

                PdfReader pr = new PdfReader(src);
                /*(PdfObject obj, PdfObject filter)*/
                List<PdfObject> objects = Find(pr.GetPageN(
                    pr.NumberOfPages > 3 ? 1 : 2
                ));

                /*bool[] whatIsObj = new bool[]
                {
                    obj.IsArray(),
                    obj.IsBoolean(),
                    obj.IsDictionary(),
                    obj.IsIndirect(),
                    obj.IsName(),
                    obj.IsNull(),
                    obj.IsNumber(),
                    obj.IsStream(),
                    obj.IsString()
                };*/
                int n = pr.NumberOfPages;

                pr.Dispose();
                pr.Close();
                pr = null;

                return (n,
                    objects.Select(
                    o => (PdfIndirectReference)o
                ).OrderBy(o => o.Length).ThenBy(o => o.Number).Select(
                    o => new Tuple<int, int/*, int, int*/>(
                        /*o.Generation, */o.Length, o.Number//, o.Type
                    )
                ).ToList());

                /*PdfIndirectReference objref = (PdfIndirectReference)obj;
                return new Tuple<int, int, int, int>(
                    objref.Generation,
                    objref.Length,
                    objref.Number,
                    objref.Type
                );*/
            }
            catch
            {
                return (0, new List<Tuple<int, int>>());
            }
            
        }

        public static void Extract(string src, string outputPath, int page = 0)
        {
            PdfReader pr = new PdfReader(src);
            RandomAccessFileOrArray file = new RandomAccessFileOrArray(src);


            int max = page > 0 ? page : pr.NumberOfPages;
            for (int i = page > 0 ? page : 1; i <= max; ++i)
            {
                try
                {
                    //(PdfObject obj, PdfObject filter) = Find(pr.GetPageN(i));
                    PdfObject obj = null;
                    if (obj != null)
                    {
                        /*int xrefIndex = Convert.ToInt32(
                            (
                                (PRIndirectReference)obj
                            ).Number.ToString(
                                CultureInfo.InvariantCulture
                            )
                        );

                        PdfObject pdfImg = pr.GetPdfObject(xrefIndex);
                        PdfStream stream = (PdfStream)pdfImg;
                        byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)stream);*/
                        byte[] bytes = PdfReader.GetStreamBytesRaw(
                                    (PRStream)((PdfStream)pr.GetPdfObject(
                                        Convert.ToInt32(
                                            (
                                                (PRIndirectReference)obj
                                            ).Number.ToString(
                                                CultureInfo.InvariantCulture
                                            )
                                        )
                                    ))
                                );

                        if (bytes != null)
                        {
                            using (MemoryStream mem = new MemoryStream(bytes))
                            {
                                mem.Position = 0;
                                var img = Image.FromStream(mem, true, true);
                                if (!Directory.Exists(outputPath))
                                    Directory.CreateDirectory(outputPath);

                                ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().Single(e => e.FormatID.Equals(img.RawFormat.Guid));
                                EncoderParameters prms = new EncoderParameters(1);
                                prms.Param[0] = new EncoderParameter(Encoder.Compression, 0);

                                img.Save(
                                    Path.Combine(outputPath, String.Format(@"{0}.{1}", i, codec.FilenameExtension.Split(';')[0])),
                                    codec, prms
                                );
                            }
                        }
                    }
                }
                catch
                {
                    
                }

                pr.Dispose();
                pr.Close();
                file.Close();
            }            
        }
    }
}
