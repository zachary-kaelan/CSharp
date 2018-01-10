using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PPReportAnalyzer
{
    class Program
    {
        public const string DocsPath = @"E:\Docs\";

        public const string MainPath = @"E:\Report Analyzer\";
        public const string ReportsPath = MainPath + @"Reports\";
        public const string BranchesPath = MainPath + @"Branches\";
        public const string PdfDataPath = MainPath + @"Pdf Size Data\";
        public static readonly CsvConfiguration cfg = new CsvConfiguration()
        {
            IgnoreHeaderWhiteSpace = true,
            IgnoreBlankLines = true,
            DetectColumnCountChanges = false,
            IsHeaderCaseSensitive = false,
            SkipEmptyRecords = true,
            WillThrowOnMissingField = false
        };
        public static readonly CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        public static readonly CompareOptions opts = CompareOptions.IgnoreCase |
                                                     CompareOptions.IgnoreSymbols;

        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(PdfDataPath + "Signatures_Full.csv");
            CsvReader cr = new CsvReader(sr, new CsvConfiguration() { IgnoreHeaderWhiteSpace = true });
            Dictionary<string, SignatureModel> sigs = cr.GetRecords<SignatureModel>().ToDictionary(
                s => s.FileName, s => s
            );
            cr.Dispose();
            cr = null;
            sr.Dispose();
            sr.Close();
            sr = null;

            Dictionary<KeyValuePair<string, int>, Dictionary<Tuple<int, int>, Dictionary<string, int>>> correlations = new Dictionary<KeyValuePair<string, int>, Dictionary<Tuple<int, int>, Dictionary<string, int>>>();

            /*Dictionary<string, int> domainCounts = File.ReadAllLines(@"E:\Report Analyzer\Email Domains.txt").ToDictionary(
                d => d.Split('\t')[0], d => Convert.ToInt32(d.Split('\t')[1])
            ).OrderBy(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);*/

            //List<FileInfo> files = Directory.GetFiles(@"E:\Docs").Select(f => new FileInfo(f)).ToList();

            string[] files = Directory.GetFiles(@"E:\Docs")
                .Select(f => new FileInfo(f).Name).Intersect(sigs.Keys).ToArray();

            int count = files.Length;
            for (int i = 0; i < count; ++i)
            {
                string file = files[i];
                (int pageCount, List<Tuple<int, int>> pdfObjects) = PdfImages.ExtractDebug(DocsPath + file);
                KeyValuePair<string, int> info = new KeyValuePair<string, int>(file.Contains("signed") ? "Signed" : "Unsigned", pageCount);

                foreach(Tuple<int, int> pdfObject in pdfObjects)
                {
                    if (!correlations.Keys.Contains(info))
                    {
                        correlations.Add(info, new Dictionary<Tuple<int, int>, Dictionary<string, int>>());
                        correlations[info].Add(pdfObject, new Dictionary<string, int>()
                        {
                            {"APAY", 0},
                            {"SignedSA", 0}
                        });
                    }
                    else if (!correlations[info].Keys.Contains(pdfObject))
                        correlations[info].Add(pdfObject, new Dictionary<string, int>()
                        {
                            {"APAY", 0},
                            {"SignedSA", 0}
                        });

                    if (sigs[file].APAY == "1")
                        ++correlations[info][pdfObject]["APAY"];
                    else
                        --correlations[info][pdfObject]["APAY"];

                    if (sigs[file].SignedSA == "1")
                        ++correlations[info][pdfObject]["SignedSA"];
                    else
                        --correlations[info][pdfObject]["SignedSA"];
                }
                
            }

            //string[] files = Directory.GetFiles(@"E:\Docs");

            /*var imageGroups = signed_unsigned.Select(
                s => s.GroupBy(
                    f => PdfImages.ExtractDebug(f.Key)
                ).ToArray()
            ).ToArray();*/

            var imageGroups =
                files.GroupBy(
                    f => f.Contains("signed")
                ).Select(
                    g => new KeyValuePair<string, KeyValuePair<string, Tuple<int, List<Tuple<int, int>>>>[]>(
                        g.Key ? "Signed" : "Unsigned",
                        g.Select(
                            f => new KeyValuePair<string, Tuple<int, List<Tuple<int, int>>>>(
                                f, PdfImages.ExtractDebug(f).ToTuple()
                            )
                        ).ToArray()
                    )
                ).Select(
                    g => new KeyValuePair<string, KeyValuePair<KeyValuePair<int, Tuple<int, int>>, string[]>[]>(
                        g.Key,
                        g.Value.SelectMany(
                            f => f.Value.Item2.Select(
                                o => new KeyValuePair<string, KeyValuePair<int, Tuple<int, int>>>(
                                    f.Key,
                                    new KeyValuePair<int, Tuple<int, int>>(f.Value.Item1, o)
                                )
                            )
                        ).GroupBy(
                            f => f.Value,
                            f => f.Key
                        ).Select(
                            g2 => new KeyValuePair<KeyValuePair<int, Tuple<int, int>>, string[]>(
                                g2.Key,
                                g2.OrderBy(f => f).ToArray()
                            )
                        ).OrderBy(g2 => g2.Key.Key)
                        .ThenBy(g2 => g2.Key.Value.Item1)
                        .ThenBy(g2 => g2.Key.Value.Item2).ToArray()
                    )
                ).ToArray();


                /*.GroupBy(
                f => f.Value.Key,
                f => new KeyValuePair<string, Tuple<int, List<Tuple<int, int>>>>(
                    f.Key, f.Value.Value
            )
            ).ToArray();*/

            /*for (int i = 0; i < 50; ++i)
            {
                PdfImages.ExtractDebug(signed_unsigned[0][i].Key);
                PdfImages.ExtractDebug(signed_unsigned[1][i].Key);
            }*/

            //PdfImages.Extract(@"E:\Docs\Nara Mattevi_signed_agreement.pdf", @"E:\PdfImages", 2);
            //PdfImages.Extract(@"E:\Docs\Aaron Burns_agreement.pdf", @"E:\PdfImages", 2);

            PdfReader pr = new PdfReader(@"E:\Docs\Nara Mattevi_signed_agreement.pdf");
            var page = pr.GetPageN(2);
            PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
            PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            var objects = xobj.Keys.Select(
                k => xobj.Get(k)
            ).ToList();


            /*KeyValuePair<string, long>[][] signed_unsigned = files.Select(f => new KeyValuePair<string, long>(f.FullName, f.Length))
                .GroupBy(f => f.Key.Contains("signed_agreement"))
                .Select(g => g.OrderBy(f => f.Value).ToArray())
                .ToArray();

            KeyValuePair<long, string[]>[] signed = signed_unsigned.Single(g => g.First().Key.Contains("signed_agreement"))
                .GroupBy(f => f.Value, f => f.Key)
                .Select(g => new KeyValuePair<long, string[]>(g.Key, g.OrderBy(f => f).ToArray()))
                .ToArray();

            KeyValuePair<long, string[]>[] unsigned = signed_unsigned.Single(g => !g.First().Key.Contains("signed_agreement"))
                .GroupBy(f => f.Value, f => f.Key)
                .Select(g => new KeyValuePair<long, string[]>(g.Key, g.OrderBy(f => f).ToArray()))
                .ToArray();

            signed_unsigned = null;*/

            Dictionary<string, List<KeyValuePair<KeyValuePair<string, long>, Dictionary<int, long>>>> pdfData = new Dictionary<string, List<KeyValuePair<KeyValuePair<string, long>, Dictionary<int, long>>>>()
            {
                {"Signed New", new List<KeyValuePair<KeyValuePair<string, long>, Dictionary<int, long>>>()},
                {"Signed Old", new List<KeyValuePair<KeyValuePair<string, long>, Dictionary<int, long>>>()},
                {"Unsigned New", new List<KeyValuePair<KeyValuePair<string, long>, Dictionary<int, long>>>()},
                {"Unsigned Old", new List<KeyValuePair<KeyValuePair<string, long>, Dictionary<int, long>>>()}
            };

            /*foreach (var file in files)
            {

                PdfReader r = null;
                try
                {
                    r = new PdfReader(file.FullName);
                    KeyValuePair<KeyValuePair<string, long>, Dictionary<int, long>> info = new KeyValuePair<KeyValuePair<string, long>, Dictionary<int, long>>(
                        new KeyValuePair<string, long>(file.Name, file.Length),
                        new Dictionary<int, long>()
                    );

                    for (int i = 1; i <= r.NumberOfPages; ++i)
                    {
                        info.Value.Add(i, r.GetPageContent(i).LongLength);
                    }

                    pdfData[
                        (file.Name.Contains("signed") ? "Signed" : "Unsigned") +
                        " " + (r.NumberOfPages == 4 ? "New" : (r.NumberOfPages == 3 ? "Old" : "Other"))
                    ].Add(info);
                }
                catch
                {
                }
                finally
                {
                    if (r != null)
                    {
                        r.Dispose();
                        r.Close();
                        r = null;
                    }
                }
            }*/

            files = null;

            /*
             * List<
             *     KeyValuePair<
             *         Dictionary<
             *             int,         page number
             *             long         bytecount of page content
             *         >,
             *         KeyValuePair<
             *             string,      file name
             *             long         file size
             *         >
             *     >
             * >
             */

            /*Dictionary<string, KeyValuePair<Dictionary<int, long>, KeyValuePair<string, long>[]>[]> pdfSizes =
                pdfData.
                Select(
                    kv => new
                    {
                        key = kv.Key,
                        value = kv.Value.GroupBy(f => f.Value, f => f.Key)
                    }
                ).ToDictionary(
                    kv => kv.key,
                    kv => 
                        kv.value.Select(
                            g => new KeyValuePair<Dictionary<int, long>, KeyValuePair<string, long>[]>(
                                g.Key, g.OrderBy(f => f.Value).ToArray()
                            )
                        ).ToArray()
                );*/

            Dictionary<string, Dictionary<int, KeyValuePair<long, KeyValuePair<string, long>[]>[]>> groupedByPages = pdfData.ToDictionary(
                kv => kv.Key,
                kv => (kv.Key.Contains("New") ? new int[] {1, 2, 3, 4} : (kv.Key.Contains("Old") ? new int[] {1, 2, 3} : new int[] { 1, 2 })).ToDictionary(
                    n => n,
                    n => kv.Value.GroupBy(
                        s => s.Value[n],
                        s => s.Key
                    ).Select(g => new KeyValuePair<long, KeyValuePair<string, long>[]>(g.Key, g.OrderBy(f => f.Value).ToArray())).OrderBy(s => s.Key).ToArray()
                )
            );

            pdfData = null;

            /*
             * Dictionary<
             *     string,              type of pdf
             *     KeyValuePair<        
             *         Dictionary<
             *             int,         page number
             *             long         bytecount of page content
             *         >,
             *         KeyValuePair<
             *             string,      file name
             *             long         file size
             *         >[]
             *     >[]
             * >
             */

            /*foreach(var pdfSize in pdfSizes)
            {
                File.WriteAllLines(
                    PdfDataPath + (pdfSize.Key + " sizes.txt").ToLower().Replace(' ', '_'),
                    pdfSize.Value.Select(
                        s => String.Join("\n", s.Key.Select(p => p.Key.ToString() + "\t :=: \t" + p.Value.ToString())) + "\n\t" +
                            String.Join("\n\t", s.Value.Select(f => f.Key + "\t :=: \t" + f.Value.ToString()))
                    )
                );
            }*/
        }

        public static void CleanReport(List<Customer> report)
        {
            report.RemoveAll(
                c => compInf.IndexOf(c.FirstName, "test", opts) != -1 ||
                    compInf.IndexOf(c.LastName, "test", opts) != -1
            );
        }
    }
}
