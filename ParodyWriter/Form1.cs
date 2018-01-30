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
using ZachLib;

namespace ParodyWriter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var lines = File.ReadAllLines(@"H:\Notes\Zach\Parodies\Temp.txt");
            var wordCounts = File.ReadAllText(
                @"H:\Notes\Zach\Parodies\Temp.txt"
            ).ToLower().Split(
                new char[] { '\r', '\n', ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries
            ).GroupBy(w => w).Where(
                w => w.Key
            ).ToDictionary(
                g => g.Key, 
                g => g.Count()
            );

            var countsList = wordCounts.ToList().OrderByDescending(w => w.Value);
            int grandTotal = countsList.Sum(l => l.Value);
            var percentagesList = countsList.Select(
                kv => new KeyValuePair<string, double>(
                    kv.Key, 
                    (double)kv.Value / grandTotal
                )
            ).Select(
                w => new KeyValuePair<string, string>(
                    w.Key, 
                    w.Value.ToString("##.00")
                )
            );
            
            
        }
    }
}
