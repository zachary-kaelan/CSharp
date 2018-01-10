using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VantageTracker
{
    public abstract class IVTUpdateModel
    {
        public const string kvPat = "(?:input type=\"([^\"]+)\"|select) (?:id=\"[^\"]+\" )?name=(?<x>['\"])([^\"'>]*?)" + @"\k<x>[^>]*?(?:[^>]+>(?:(?:(?:<optgroup label=" + "\"[^\"]+\">)?<option value=\"" + @"\d+" + "\"" + @"\s?>[^<]+<\/option>(?:<\/optgroup>)?)*)?<option )?value=(?<y>['" + "\"])([^'\">]*?)" + @"\k<y>(?: (?:checked=" + "\"(checked)\")|selected=\"(selected)\")?";
        public const string CustIDPat = @"<dd>\s+(\d+)\s+<\/dd>";
        public string urlName;
        public string htmlName;
        public string[] defaults;
        //public abstract Dictionary<string, string[]> ConvertToDictionary(List<Match> matches);
        public Dictionary<string, string[]> FilterMatches(string html, out string custid)
        {
            custid = Regex.Match(html, CustIDPat).Groups[1].Value;

            List<Match> matches = Regex.Matches(html, kvPat, RegexOptions.None, VTNav.rgxTimeout).Cast<Match>().ToList();
            matches.RemoveAll(
                m => (m.Groups[1].Value == "checkbox" && m.Groups[4].Value != "checked")
            );

            Dictionary<string, string[]> dict = matches.GroupBy(
                m => m.Groups[2].Value, m => m.Groups[3].Value
            ).ToDictionary(
                g => g.Key,
                g => g.ToArray()
            );

            foreach(string def in this.defaults)
            {
                if (!dict.TryGetValue(def, out _))
                    dict.Add(def, new string[] { "" });
            }

            return dict;
        }
    }

    public class VTServiceInfoModel : IVTUpdateModel
    {
        public VTServiceInfoModel()
        {
            this.urlName = "service-information";
            this.htmlName = "service-info";
            this.defaults = new string[]
            {
                "service_info[county]",
                "service_info[rescheduleInitialJob][timeScheduled]",
                "service_info[rescheduleInitialJob][futureJobs][futureTime]",
                "service_info[contract][taxCode]",
                "service_info[mapCode]",
                "service_info[contract][purchaseOrderNumber]",
                "service_info[technician]",
                "service_info[weekOfTheMonth]",
                "service_info[dayOfTheWeek]",
                "service_info[preferredTime]"
            };
        }
    }

    public class VTServiceAddrModel : IVTUpdateModel
    {
        public VTServiceAddrModel()
        {
            this.urlName = "service-address";
            this.htmlName = "service_address";
            this.defaults = new string[]
            {
                "service_address[companyName]",
                "service_address[contactAddress][altPhone]",
                "service_address[secondaryEmailAddresses]"
            };
        }
    }

    public struct VTServiceInfo
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string postalCode { get; set; }
        public string phone { get; set; }
        public bool deliverEmail { get; set; }
        public string emailAddress { get; set; }
        public string accountType { get; set; }
        public bool subscribed { get; set; }

        public Dictionary<string, string> pests { get; set; }
        public Dictionary<string, string> specialtyPests { get; set; }
        public Dictionary<string, string> tags { get; set; }
        public string county { get; set; }
        public string dateStart { get; set; }
        public bool rescheduleInitial { get; set; }
        public DateTime dateScheduled { get; set; }
        public bool applyToFuture { get; set; }
        public bool futureOptionsOffset { get; set; }
        public int futureWeek { get; set; }
        public int futureDay { get; set; }
        public string futureTime { get; set; }
        public TimeSpan defaultJobDuration { get; set; }
        public KeyValuePair<string, double> taxcode { get; set; }
        public string salesStatus { get; set; }
        public string salesperson { get; set; }
        public bool autoRenew { get; set; }
        public bool customColor { get; set; }
        public string contractColor { get; set; }
        public string foundByType { get; set; }
        public bool redirectToRouting { get; set; }
    }
}
