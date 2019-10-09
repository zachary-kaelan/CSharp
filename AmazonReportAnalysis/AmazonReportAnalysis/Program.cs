using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib;

namespace AmazonReportAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            var orders = Utils.LoadCSV<Order>(@"C:\Users\ZACH-GAMING\Downloads\Amazon Report.csv").Where(o => o.ItemTotal > 0);
            var categories = orders.GroupBy(o => o.Category, o => o.ItemTotal, (k, g) => new KeyValuePair<string, double>(k, g.Sum())).OrderByDescending(c => c.Value);
            foreach(var category in categories)
            {
                Console.WriteLine("{0} - ${1}", category.Key, category.Value);
            }
            Console.ReadLine();
        }
    }

    public class Order
    {
        public DateTime OrderDate { get; set; }
        public string OrderID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Seller { get; set; }
        public double ItemTotal { get; set; }
    }
}
