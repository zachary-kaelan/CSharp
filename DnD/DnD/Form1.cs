using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace DnD
{
    public class Listener : IRenderListener
    {
        public void BeginTextBlock()
        {
            throw new NotImplementedException();
        }

        public void EndTextBlock()
        {
            throw new NotImplementedException();
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            throw new NotImplementedException();
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            throw new NotImplementedException();
        }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        const string basePath = @"E:\Installations\Programming_Frameworks\DnD\";
        const string DataPath = basePath + @"Data\";
        const string FifthEditionPath = DataPath + @"5e\";
        const string FifthEditionTreasurePath = FifthEditionPath + @"Treasure\";
        const string DMGPath = basePath + "DMG.pdf";
        const string DMG5ePath = basePath + @"DnD 5e Dungeon Masters Guide.pdf";
        const string MatchesPath = basePath + "Matches.txt";
        const string SimpleTextPat = @"([0-9]+,?[0-9]*) GP (.+?)\n.+\n((?:[0-9A-Z]{1,2}[A-Za-z\s~'-]+?\n)+?)(?=,?[0-9]+,?[0-9]* GP|[A-Z ]{2,}\s)";

        private void btnTables_Click(object sender, EventArgs e)
        {
            PdfReader reader = new PdfReader(DMG5ePath);
            string simpleText = PdfTextExtractor.GetTextFromPage(reader, 135, new SimpleTextExtractionStrategy());
            //string locationText = PdfTextExtractor.GetTextFromPage(reader, 135, new LocationTextExtractionStrategy());

            RollTable[] tables = Regex.Matches(simpleText, SimpleTextPat)
                .Cast<Match>().Select(m => new RollTable(m)).ToArray();
            File.WriteAllText(
                FifthEditionTreasurePath + "ArtObjects.txt",
                String.Join("\n\n", tables.Select(t => t.ToString()))
            );

            /*Dictionary<string[], string[]> matches = File.ReadAllLines(MatchesPath)
            .Select(l => l.Split(new string[] { " :=: " }, StringSplitOptions.None))
            .ToDictionary(
                l => l[0].Split(new string[] { ", ", "," }, StringSplitOptions.None),
                l => l[1].Split(new string[] { ", ", "," }, StringSplitOptions.None)
            );

            PdfReader pr = new PdfReader(DMGPath);
            string pdf = PdfTextExtractor.GetTextFromPage(pr, 110, new SimpleTextExtractionStrategy());

            //var page = pr.GetPageN(110);
            
            var tableNames = Regex.Matches(
                pdf, @"Table \d.\d:"
            ).Cast<Match>().Select(
                m => new KeyValuePair<int, string>(m.Index, m.Value)
            ).ToArray();

            var lines = pdf.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList().Select(s => s.Split(' ').ToList()).ToList();
            List<DataTable> tables = new List<DataTable>(tableNames.Length);*/
            
            /*for (int i = 0; i < lines.Count; ++i)
            {
                //table.LoadDataRow(lines[i].ToArray(), true);
                List<string> line = lines[i];
                for (int j = 0; j < line.Count; ++j)
                {
                    string element = line[j];
                    if (Regex.IsMatch(element, @"01.\d\d"))
                    {
                        DataTable table = new DataTable();
                        for (int k = j; j < line.Count; ++j)
                        {
                            element = line[i];
                            string lbl = lines[i - 1][j];
                            if (Int32.TryParse(element, out _))
                            DataColumn col = new DataColumn(, );
                        } 
                    }
                }
            }*/
            
            /*
            for (int i = 0; i < table.Rows.Count; ++i)
            {
                var row = table.Rows[i].ItemArray.Cast<string>().ToList();
                for (int j = 0; j < row.Count; ++j)
                {
                    var item = row[j];
                    
                }
            }
            */
        }

        
    }

    public class ContentOperator : IContentOperator
    {
        public void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands)
        {
            
        }
    }

    public class TextExtractionStrategy : ITextExtractionStrategy
    {
        private StringBuilder result = new StringBuilder();
        private Vector lastBaseLine;
        private string lastFont;
        private float lastFontSize;

        private enum TextRenderMode
        {
            FillText = 0,
            StrokeText = 1,
            FillThenStrokeText = 2,
            Invisible = 3,
            FillTextAndAddToPathForClipping = 4,
            StrokeTextAndAddToPathForClipping = 5,
            FillThenStrokeTextAndAddToPathForClipping = 6,
            AddTextToPaddForClipping = 7
        }

        public void BeginTextBlock()
        {
            throw new NotImplementedException();
        }

        public void EndTextBlock()
        {
            throw new NotImplementedException();
        }

        public string GetResultantText()
        {
            throw new NotImplementedException();
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            throw new NotImplementedException();
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            Vector bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
            Vector topRight = renderInfo.GetAscentLine().GetEndPoint();
            
        }
    }
}
