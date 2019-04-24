using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DatamuseLib;
using DatamuseLib.Models;
using DatamuseLib.Models.Songs;
using ParodyWriter.Properties;
using ZachLib;

namespace ParodyWriter
{
    public partial class SongAnalysis : Form
    {
        public SongAnalysis()
        {
            InitializeComponent();
        }

        public string PATH_SONG { get; set; }
        public int[] LineLengths { get; set; }
        public int MaxLineLength { get; set; }
        public int MaxLineMeasuredWidth { get; set; }
        public const int LISTBOX_COL_HEIGHT = 13;
        public int ListBoxScrollRemainder = 0;
        public string[] Words { get; set; }
        public SortedDictionary<string, int> WordCounts { get; set; }
        public string SongName { get; set; }
        public List<string> SplitStrings { get; set; }
        public string[] Stanzas { get; private set; }

        private void SongAnalysis_Load(object sender, EventArgs e)
        {
            

            txtSong.MouseWheel += TxtSong_MouseWheel;
            //MouseWheel += TxtSong_MouseWheel;

            /*for (int i = 0; i < 256; ++i)
            {
                string str = Convert.ToString(i);
                txtSong.AppendText(str + "\r\n");
                lstAnalysis.Items.Add(str);
            }*/
        }

        private void TxtSong_MouseWheel(object sender, MouseEventArgs e)
        {
            lstAnalysis.TopIndex = Math.Min(lstAnalysis.Items.Count - 42, Math.Max(0, lstAnalysis.TopIndex - (e.Delta / 60)));
        }

        private void txtSong_TextChanged(object sender, EventArgs e)
        {
            var lines = txtSong.Lines.Select(l => l.Trim());
            LineLengths = lines.Select(l => l.Length).ToArray();
            MaxLineLength = LineLengths.Max();
            txtSong.Lines = lines.ToArray();
            MaxLineMeasuredWidth = lines.Max(l => TextRenderer.MeasureText(l, txtSong.Font).Width);
            int prevWidth = txtSong.Width;
            int widthModifier = MaxLineMeasuredWidth - prevWidth;

        }

        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            txtSong.ReadOnly = true;
            SongName = txtSongName.Text;
            PATH_SONG = Form1.PATH_MAIN + SongName + @"\";
            Words = txtSong.Text.ToLower().Split(
                new char[] { '\r', '\n', ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries
            ).Select(
                w => ZachRGX.NON_ALPHA_NUMERIC.Replace(w, "")
            ).Where(
                w => !String.IsNullOrWhiteSpace(w)
            ).ToArray();

            WordCounts = new SortedDictionary<string, int>(
                Words.GroupBy(w => w).ToDictionary(
                    g => g.Key,
                    g => g.Count()
                )
            );
            WordCounts.SaveDictAs(PATH_SONG + "WordCounts.txt");

            var countsList = WordCounts.ToList().OrderByDescending(w => w.Value);
            int grandTotal = countsList.Sum(l => l.Value);
            var percentagesList = countsList.Select(
                kv => new KeyValuePair<string, double>(
                    kv.Key,
                    (double)kv.Value / grandTotal
                )
            ).ToDictionary(
                w => w.Key,
                w => w.Value.ToString("##.00")
            );
            percentagesList.SaveDictAs(PATH_SONG + "WordPercentages.txt");
            lstPercentages.Items.AddRange(
                percentagesList.Select(
                    kv => kv.Key + " - " + kv.Value
                ).ToArray()
            );

            foreach(var word in countsList)
            {
                var fullLexical = API.GetWordInfo(
                    word.Key,
                    ExtraLexicalKnowledge.Definitions |
                    ExtraLexicalKnowledge.PartsOfSpeech |
                    ExtraLexicalKnowledge.Pronunciation |
                    ExtraLexicalKnowledge.SyllableCount |
                    ExtraLexicalKnowledge.WordFrequency
                );
                var wordModel = new SongWordModel(
                    fullLexical,
                    word.Value,
                    
                );
            }

            SplitStrings = new List<string>();
            foreach (var line in txtSong.Lines)
            {
                if (String.IsNullOrWhiteSpace(ZachRGX.NON_ALPHA_NUMERIC.Replace(line, "")))
                {
                    lstAnalysis.Items.Add(line);
                    if (!SplitStrings.Contains(line))
                        SplitStrings.Add(line);
                }
                else
                {
                    int syllables = line.CountSyllables(true);
                    lstAnalysis.Items.Add(syllables);
                }
            }

            
        }
    }
}
