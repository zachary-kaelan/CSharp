using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DNSChangerService.Properties;

namespace DNSChangerService
{
    public partial class DNSService : ServiceBase
    {
        private static WebClient client = new WebClient();

        public DNSService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (String.IsNullOrWhiteSpace(Settings.Default.IP))
            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            
        }

        public static string GetCurrentIP()
        {
            return client.DownloadString("http://icanhazip.com");
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
