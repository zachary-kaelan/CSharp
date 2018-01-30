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
using CsvHelper;
using CsvHelper.Configuration;
using ZachLib;

namespace PaypalReportAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(@"C:\Users\ZACH-GAMING\Downloads\Download.CSV");
            CsvReader cr = new CsvReader(sr, Utils.csvConfig);
            IEnumerable<Payment> payments = cr.GetRecords<Payment>().ToArray();
            cr.Dispose();
            cr = null;
            sr.Close();
            sr = null;

            var names = payments.GroupBy(
                p => p.Name,
                p => Convert.ToDouble(p.Amount.Replace(",", "")),
                (k, g) => new KeyValuePair<string, double>(
                    k, g.Sum()
                )
            ).Where(kv => kv.Value < 0).OrderBy(kv => kv.Value);
            var excludedNames = names.Take(5).Select(n => n.Key);

            var types = payments.Where(
                p => !excludedNames.Contains(p.Name)
            ).GroupBy(p => p.Type).OrderBy(
                g => g.Sum(
                    p => Convert.ToDouble(p.Amount.Replace(",", ""))
                )
            ).ToArray();

            

            /*var types = payments.Where(p => !excludedNames.Contains(p.Name)).GroupBy(
                p => p.Type,
                p => Convert.ToDouble(p.Amount.Replace(",", "")),
                (k, g) => new KeyValuePair<string, double>(
                    k, g.Sum()
                )
            ).Where(kv => kv.Value < 0).OrderBy(kv => kv.Value);*/

        }
    }

    public struct Payment
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string TimeZone { get; set; }
        private double dblAmount { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string ReceiptID { get; set; }
        public string Balance { get; set; }

        public double GetAmount()
        {
            if (dblAmount == double.NaN || dblAmount == 0)
                dblAmount = Convert.ToDouble(Amount.Replace(",", ""));
            return dblAmount;
        }
    }
}
