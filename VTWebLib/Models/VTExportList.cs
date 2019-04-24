using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTWebLib.Models
{
    public class VTExportList
    {
        public VTExportCustTemp[] aaData { get; private set; }
        public int iTotalDisplayRecords { get; private set; }
        public int iTotalRecords { get; private set; }
        public int sEcho { get; private set; }

        public VTExportCust[] GetCustomers() => aaData.Select(c => new VTExportCust(c)).ToArray();
    }
}
