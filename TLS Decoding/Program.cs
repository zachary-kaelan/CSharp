using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CryptSharp;

namespace TLS_Decoding
{
    class Program
    {

        static void Main(string[] args)
        {
            byte[] serverPubKeyBytes = new byte[] {
                0x04, 0x91, 0xaf, 0xd4, 0xfe, 0x61, 0x5f, 0xe2,
                0xf9, 0x20, 0x7a, 0xc4, 0x2c, 0x57, 0x1a, 0xcd,
                0x50, 0x2b, 0x3d, 0x1b, 0x16, 0xf8, 0xcb, 0x4d,
                0x2c, 0xc9, 0x54, 0xea, 0x09, 0xe7, 0xee, 0xc5,
                0xf5, 0x14, 0xb4, 0x10, 0x95, 0x2f, 0x21, 0xa6,
                0x2e, 0xd0, 0x9f, 0x97, 0xc1, 0x53, 0xd4, 0x31,
                0x98, 0xa3, 0x6f, 0x01, 0xd7, 0x78, 0xe0, 0x5a,
                0xb8, 0x0c, 0x97, 0x40, 0x24, 0x93, 0x67, 0x56,
                0xfa
            };

            RSAParameters certPubKey = new RSAParameters();

            var pubKey = "AAAAB3NzaC1yc2EAAAABJQAAAQEAu9jasDhZYbu2/80PecTTCowKNzFY6Jr4QP9inEA32dSTpdOil3oHSi1wVekNFqJduelnhvhhT2lqXrpLODemJUZ67lHvA8/rpDiPdI8cFeHTWhN2dBw8GzVHuTrDvKVLqmeFX7IL4pH6OA8w/FG+KGpoUrzzNFM8G0KO5x7CRxfgBdke23oq5vW3Rd3yjneyqaBXVGmZE3RTLM2ODQ2piAprxMuh/dJGUUjYgWgUvNvWkl4UqScdWpjXej64UVcvNbop8+/V9SFlduEFEkF2kPWWoVXPyedDeiz/xafCOlgQFRq2zrIfMVBIy9pLkCFa0PBbszGhbfWZV5shLzodJw==";
            byte[] bytes = Encoding.ASCII.GetBytes(pubKey);
            byte[] publicExponent = 

            string server = "205.251.207.78";
            TcpClient client = new TcpClient(server, 443);
            using (SslStream ssl = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null))
            {
                ssl.AuthenticateAsClient(, )
                ssl.AuthenticateAsClient(server);
            }
        }

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            /*if (sslPolicyErrors != SslPolicyErrors.None)
            {
                Console.WriteLine(“SSL Certificate Validation Error!”);
                Console.WriteLine(sslPolicyErrors.ToString());
                return false;
            }
            else*/
                return true;
        }
    }
}
