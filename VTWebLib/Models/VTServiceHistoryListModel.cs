using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VTWebLib.Models
{
    public struct VTServiceHistoryListModel
    {
        public string InvoiceNumber { get; private set; }
        public DateTime Date { get; private set; }
        public string Status { get; private set; }
        public string Type { get; private set; }
        public bool Paid { get; private set; }
        public string Technician { get; private set; }

        public VTServiceHistoryListModel(HtmlNode node)
        {
            InvoiceNumber = node.SelectSingleNode("td[1]").InnerText;
            Date = DateTime.Parse(node.SelectSingleNode("td[2]").InnerText.Trim());
            Status = node.SelectSingleNode("td[3]").InnerText;
            Type = node.SelectSingleNode("td[4]").InnerText;
            Paid = node.SelectSingleNode("td[7]").InnerText == "Yes";
            Technician = node.SelectSingleNode("td[8]").InnerText;
        }
    }
}
