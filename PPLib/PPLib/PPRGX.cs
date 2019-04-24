using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

using RGX;
using RGX.Components;
using RGX.FilenameAdvanced;
using RGX.INPC;
using RGX.SA;
using RGX.PPI;
using RGX.Zillow;
using ZachLib;

namespace PPLib
{
    public static class PPRGX
    {
        static PPRGX()
        {
            Assembly rgx = Assembly.Load(
                new AssemblyName("PPRGX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=c072c81d02f16673")
            );

            COMP_NAMES = (Regex)rgx.CreateInstance("RGX.Components.Names");
            COMP_ADDRESS_INFO = (Regex)rgx.CreateInstance("RGX.Components.AddressInfo");

            FILENAME_INFO = (Regex)rgx.CreateInstance("RGX.Filename");
            FILENAME_INFO_TYPE = (Regex)rgx.CreateInstance("RGX.FilenameAdvanced.FilenameInfoType");
            FILENAME_INPC = (Regex)rgx.CreateInstance("RGX.FilenameAdvanced.INPC");
            FILENAME_SA = (Regex)rgx.CreateInstance("RGX.FilenameAdvanced.SA");
            FILENAME_SA2 = (Regex)rgx.CreateInstance("RGX.FilenameAdvanced.SA2");

            PDF_SA2_SCHEDULE_PRICES = (Regex)rgx.CreateInstance("RGX.FilenameAdvanced.SA2_Schedule_Prices");
            PDF_SA2_SCHEDULE_MONTHS = (Regex)rgx.CreateInstance("RGX.FilenameAdvanced.SA2_Schedule_Months");
            PDF_INPC_VTNUMBERS = (Regex)rgx.CreateInstance("RGX.INPC.VTNumbers");

            PPI_LICENSE_USERS = (Regex)rgx.CreateInstance("RGX.PPI.LicenseUsers");
            PPI_LOCATION_STATE = (Regex)rgx.CreateInstance("RGX.PPI.LocationState");

            ZILLOW_REGIONID = (Regex)rgx.CreateInstance("RGX.Zillow.RegionID");
        }

        public static Regex COMP_NAMES { get; private set; }
        public static Regex COMP_ADDRESS_INFO { get; private set; }

        public static Regex FILENAME_INFO { get; private set; }
        public static Regex FILENAME_INFO_TYPE { get; private set; }
        public static Regex FILENAME_INPC { get; private set; }
        public static Regex FILENAME_SA { get; private set; }
        public static Regex FILENAME_SA2 { get; private set; }

        public static Regex PDF_SA2_SCHEDULE_PRICES { get; private set; }
        public static Regex PDF_SA2_SCHEDULE_MONTHS { get; private set; }

        public static Regex PPI_LICENSE_USERS { get; private set; }
        public static Regex PPI_LOCATION_STATE { get; private set; }

        public static Regex PDF_INPC_VTNUMBERS { get; private set; }

        public static Regex ZILLOW_REGIONID { get; private set; }
    }
}
