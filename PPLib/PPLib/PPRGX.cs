using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using PPRGX;
using PPRGX.FilenameAdvanced;
using PPRGX.SA;
using PPRGX.PPI;

namespace PPLib
{
    public static class RGX
    {
        public static readonly FilenameInfoType FILENAME_INFO_TYPE = new FilenameInfoType();
        public static readonly INPC FILENAME_INPC = new INPC();
        public static readonly SA FILENAME_SA = new SA();
        public static readonly SA2 FILENAME_SA2 = new SA2();

        public static readonly SA2_Schedule_Prices SCHEDULE_PRICES_SA2 = new SA2_Schedule_Prices();
        public static readonly SA2_Schedule_Months SCHEDULE_MONTHS_SA2 = new SA2_Schedule_Months();

        public static readonly LicenseUsers LICENSE_USERS = new LicenseUsers();
    }
}
