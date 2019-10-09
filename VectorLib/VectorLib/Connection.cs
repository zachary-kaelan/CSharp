using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Grpc;
using Grpc.Auth;
using Grpc.Core;

namespace VectorLib
{
    public class Connection
    {
        public string Name { get; private set; }
        public string Host { get; private set; }
        public string Cert_File { get; private set; }
        public string Guid { get; private set; }

        private Thread _thread = null;
        private Channel _channel = null;
        private SslStream _connection = null;

        public Connection(string name, string host, string cert_file, string guid)
        {
            Name = name;
            Host = host;
            Cert_File = cert_file;
            Guid = guid;
        }

        public void Connect()
        {

        }

        private void _connect()
        {
            try
            {
                var channel_credentials = new SslCredentials(File.ReadAllText(Cert_File));
                var call_credentials = GoogleGrpcCredentials.FromAccessToken(Guid);
                var credentials = ChannelCredentials.Create(channel_credentials, call_credentials);
                _channel = new Channel(Host, credentials, new ChannelOption[] { new ChannelOption(ChannelOptions.SslTargetNameOverride, Name) });
                
            }
            catch
            {

            }
        }
    }
}
