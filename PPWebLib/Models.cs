using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PPWebLib
{
    internal struct LicenseUser
    {
        public string SessionUserID { get; set; }
        public string SessionID { get; set; }
        public string Name { get; set; }
        public string LastActivity { get; set; }
        public string NumLicenses { get; set; }
        private static CultureInfo inf = new CultureInfo("en-US");
        private static readonly string[] highPriority = new string[]
        {
            "Sam Smith",
            "Janice Robinson",
            "Janice Cornish",
            "Jeffrey Allen"
        };
        private int logOut { get; set; }

        public bool LogOut(DateTime now)
        {
            if (logOut == 1)
                return false;
            else if (logOut == 2)
                return true;

            bool temp = !highPriority.Contains(Name) &&
                (
                    (
                        now - DateTime.Parse(LastActivity)
                    ).TotalMinutes *
                    Convert.ToInt32(NumLicenses)
                ) >= 30;

            if (temp)
                logOut = 2;
            else
                logOut = 1;

            return temp;
        }
    }

    public class ServiceCodeLookup
    {
        [XmlAttribute]
        public string id { get; set; }
        [XmlAttribute]
        public string code { get; set; }
        [XmlAttribute]
        public string description { get; set; }

        public string price { get; set; }
        public string defaultproductionvalue { get; set; }
        public string taxable { get; set; }
        public string cost { get; set; }
        public string duration { get; set; }
        public string measurementtypeid { get; set; }
        public string color { get; set; }
        public string glcode { get; set; }
        public string glcodeid { get; set; }
        public string initialglcode { get; set; }
        public string type { get; set; }
        public string division { get; set; }
        public string divisionid { get; set; }
        public string defaultsetuptype { get; set; }
        public string defaultordertype { get; set; }
        public string measurementunit { get; set; }
        public string unitofmeasureid { get; set; }
        public string pricebymeasurement { get; set; }
        public string iseligibleforprepay { get; set; }
        public string issentricon { get; set; }
        public string schedule { get; set; }
        public string scheduleid { get; set; }
        public string starteligibilityday { get; set; }
        public string endeligibilityday { get; set; }
        public string frequency { get; set; }
        public string frequencyid { get; set; }
        public string daystofloat { get; set; }
        public string daysfromlastservice { get; set; }
        public string hasdefaultattribute { get; set; }
        public string priceincreasemonths { get; set; }
        public string iscopesan { get; set; }
    }

    public class QuickBillTo
    {
        public int status { get; set; }
        public string billtocode { get; set; }
        public string billtoid { get; set; }
        public string branchid { get; set; }
        public string branch { get; set; }
        public string glsuffix { get; set; }
        public string company { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string name { get; set; }
        public char creditstatus { get; set; }
        public int collectionstagenum { get; set; }
        public double balance { get; set; }
        public Double credit { get; set; }
        public DateTime lastpaymentdate { get; set; }
        public double lastpaymentamount { get; set; }
        public string defaultglcode { get; set; }
        public string defaultglcodeid { get; set; }
        public string creditmemos { get; set; }
        public string alertnote { get; set; }
        public int orphan { get; set; }
    }
}
