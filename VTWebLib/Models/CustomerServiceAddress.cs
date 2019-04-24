using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace VTWebLib.Models
{
    public struct CustomerServiceAddress
    {
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public CustomerServiceAddress(HtmlNode node)
        {
            Name = HttpUtility.HtmlDecode(node.SelectSingleNode("./div/span").InnerText).Trim();
            var addressNodes = node.SelectNodes("./div/a/span/br");
            AddressLine1 = addressNodes[0].PreviousSibling.InnerText.Trim(VantageTracker.TRIM_CHARS);
            AddressLine2 = addressNodes[1].PreviousSibling.InnerText.Trim(VantageTracker.TRIM_CHARS);
            Email = node.SelectSingleNode("./div[2]/a").InnerText;
            Phone = node.SelectSingleNode("./div[2]/br[2]").InnerText.Trim(VantageTracker.TRIM_CHARS);
        }
    }
}
