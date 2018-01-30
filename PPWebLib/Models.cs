using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

    internal struct TaxCode
    {
        public string County { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public string StateRate { get; set; }
        public string CountyRate { get; set; }
        public string CityRate { get; set; }
        public string TotalRate { get; set; }
    }
}
