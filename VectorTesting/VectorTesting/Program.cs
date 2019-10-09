using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Anki.Vector.ExternalInterface;
using Anki.Vector;

namespace VectorTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            /*SslCredentials creds = new SslCredentials(File.ReadAllText(@"E:\Programming\Vector\Vector-X9H1-00703bbf.cert"));
            Channel channel = new Channel(
                "192.168.1.97", 443, creds,
                new ChannelOption[] { new ChannelOption("grpc.ssl_target_name_override", "Vector-X9H1") }
            );
            var task = channel.ConnectAsync(DateTime.Now.AddSeconds(15));
            task.Wait();
            Console.WriteLine(task.Status);

            var interfaceStub = new ExternalInterface.ExternalInterfaceClient(channel);
            var request = new UserAuthenticationRequest();
            request.ClientName = ByteString.CopyFromUtf8(System.Environment.MachineName);
            request.UserSessionId = ByteString.CopyFromUtf8("2hCXtGMZRtCJqwqe6QHkHP2");
            var response = interfaceStub.UserAuthentication(request);
            Console.WriteLine(response.Code);*/

            Console.ReadLine();
        }
    }
}
