using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Zipf_Analysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public const string path = @"C:\Zipf\";
        public const string corpiPath = path + @"Corpi\";
        public const string procPath = path + @"Processed\";
        public const string dataPath = path + @"Data\";
        public const string masterPath = dataPath + @"Master.txt";
        public string[] zipExts = new string[3] { "zip", "tar.gz", "rar" };

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Regex rgx = new Regex("[^a-zA-Z ]");
            
            string[] texts = Directory.GetFiles(corpiPath, "*.txt", SearchOption.AllDirectories);


            int count = texts.Length;
            for (int i = 0; i < count; ++i)
            {
                File.AppendAllText(masterPath, File.ReadAllText(texts[i]));
            }

            Parallel.For(0, count, (i) =>
            {

            });
        }
    }
}
