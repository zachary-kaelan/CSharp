using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DNSChangerService
{
    public partial class DNSService : ServiceBase
    {
        public DNSService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnStop()
        {
        }
    }
}
