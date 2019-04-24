using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VTWebLib.Models
{
    public class CustomerServiceInfo
    {
        public string CustomerId { get; set; }
        public string CurrentContract { get; set; }
        public string SalesStatus { get; set; }
		public string ContractDates { get; set; }
		public string InitialJobDate { get; set; }
        public string Type { get; set; }
        public string ServiceType { get; set; }
		public string ServiceFrequency { get; set; }
        public string CreationDate { get; set; }
		public string InitialPrice { get; set; }
		public string RecurringPrice { get; set; }
		public string Salesperson { get; set; }
        public string QuotedInitialPrice { get; set; }
        public string MarketingType { get; set; }
		public string LastServiced { get; set; }
		public string RenewServices { get; set; }
		public string TaxCode { get; set; }

        public CustomerServiceInfo(HtmlNode node)
        {
            CustomerId = node.SelectSingleNode("./dl/dd").InnerText.Trim();
            node = node.SelectSingleNode("./dl[2]");
            var values = node.SelectNodes("./dd").Select(n => n.InnerText).ToArray();

            CurrentContract = values[0];
            SalesStatus = values[1];
            ContractDates = values[2];
            InitialJobDate = values[3];
            Type = values[4];
            ServiceType = values[5];
            ServiceFrequency = values[6];
            CreationDate = values[7];
            InitialPrice = values[8];
            RecurringPrice = values[9];
            Salesperson = values[10];
            QuotedInitialPrice = values[11];
            MarketingType = values[12];
            LastServiced = values[13];
            RenewServices = values[14];
            TaxCode = values[15];
        }
    }
}
