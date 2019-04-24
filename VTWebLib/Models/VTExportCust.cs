using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Jil;

namespace VTWebLib.Models
{
    public struct VTExportCust
    {
        public string ID { get; set; }
        public string URLID { get; set; }
        public string Name { get; set; }
        public string Office { get; set; }
        public bool OrderSucceeded { get; set; }
        public string ErrorMessage { get; set; }
        public string Status { get; set; }
        public string ErrorData { get; set; }

        private static readonly char[] FIND_CHARS = new char[] { ':', '/' };
        internal VTExportCust(VTExportCustTemp temp)
        {
            ID = temp.id;
            URLID = temp.customer_id;
            Name = temp.customer;
            Office = temp.office.Substring(10);
            Status = temp.status;
            OrderSucceeded = temp.status != "Failed";
            if (!OrderSucceeded)
            {
                ErrorMessage = temp.errors;
                if (!String.IsNullOrWhiteSpace(ErrorMessage))
                {
                    int lastChar = ErrorMessage.LastIndexOfAny(FIND_CHARS) + 1;
                    if (lastChar >= 0 && ErrorMessage.Length - lastChar >= 12)
                        ErrorMessage = ErrorMessage.Substring(lastChar).TrimStart();
                }
            }
            else
                ErrorMessage = null;
            ErrorData = "";
        }

        /*public VTExportCust(HtmlNode node)
        {
            ID = node.GetAttributeValue("data-id", "-1");
            OrderSucceeded = node.SelectSingleNode("./td[7]/a").InnerText.Trim() == "Yes";
            if (!OrderSucceeded)
            {
                ErrorMessage = null;
                var errNode = node.SelectSingleNode("./td[12]/div[2]/a");
                if (errNode != null)
                {
                    ErrorMessage = errNode.GetAttributeValue("data-content", null);
                    if (ErrorMessage != null)
                    {
                        ErrorMessage = HttpUtility.HtmlDecode(ErrorMessage).Trim();
                        int lastColon = ErrorMessage.LastIndexOf(':') + 1;
                        if (lastColon >= 0 && ErrorMessage.Length - lastColon >= 12)
                            ErrorMessage = ErrorMessage.Substring(lastColon).TrimStart();
                        lastColon = ErrorMessage.LastIndexOf('/') + 1;
                        if (lastColon >= 0)
                            ErrorMessage = ErrorMessage.Substring(lastColon).TrimStart();
                    }
                }
            }
            else
                ErrorMessage = null;
            var infoNode = node.SelectSingleNode("./td[2]/a");
            Name = HttpUtility.HtmlDecode(infoNode.InnerText).Trim();
            URLID = infoNode.GetAttributeValue("href", null).Substring(10, 6);
            Office = node.SelectSingleNode("./td[3]").InnerText.Substring(10);
            Status = node.SelectSingleNode("./td[11]").InnerText.Trim();
            ErrorData = "";
        }*/
    }

    public struct VTExportCustTemp
    {
        public string id { get; set; }
        public string customer_id { get; set; }
        public string customer { get; set; }
        public string office { get; set; }
        public string errors { get; set; }
        public string status { get; set; }
        public string contract { get; set; }
        public string location_id { get; set; }
    }
}
