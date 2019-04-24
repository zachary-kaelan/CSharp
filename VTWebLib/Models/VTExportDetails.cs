using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace VTWebLib.Models
{
    public struct VTExportDetails
    {
        public string ID { get; private set; }
        public string Status { get; private set; }
        public string Errors { get; private set; }
        public DateTime DateCreated { get; private set; }
        public DateTime DateExportAttempted { get; private set; }

        public VTExportDetails(string id, string text)
        {
            ID = id;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(text);
            var nodes = doc.DocumentNode.SelectNodes("./div[2]/table/tbody[position()>7]");
            var dict = nodes.ToDictionary(n => n.FirstChild.InnerText, n => n.LastChild.InnerText);

            Status = dict["Status"];
            Errors = dict["Errors"];
            Errors = Errors.Substring(Errors.LastIndexOf(':') + 1).Trim(' ', '\t', '\r', '\n', '"', '\'');
            DateCreated = DateTime.Parse(dict["Date Created"]);
            DateExportAttempted = DateTime.Parse(dict["Date Export Attempted"]);
        }
    }
}
