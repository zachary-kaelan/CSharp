using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VTWebLib.Models
{
    public struct VTEmployee
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string UserType { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string ID { get; set; }

        public VTEmployee(HtmlNode node)
        {
            ID = node.GetAttributeValue("data-id", null);
            Name = node.SelectSingleNode("td[1]").InnerText;
            Username = node.SelectSingleNode("td[2]").InnerText;
            UserType = node.SelectSingleNode("td[3]").InnerText;
            Email = node.SelectSingleNode("td[4]").InnerText;
            Status = node.SelectSingleNode("td[5]").InnerText;
        }
    }
}
