using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace SSH
{
    class SSHClient
    {
        public static SshClient client { get; set; }
        public static List<SftpFile> files { get; set; }
        private const string server = "servicepropest.com";
        private static ConnectionInfo ZacInfo = new ConnectionInfo(
                server,
                "zac.johnso",
                new PasswordAuthenticationMethod("zac.johnso", "I15Zac$0208")
            );
        private static ConnectionInfo SamInfo = new ConnectionInfo(
                server,
                "sam@insightpest.com",
                new PasswordAuthenticationMethod("sam@insightpest.com", "Sam@1218")
            );

        public static void Login()
        {
            client = new SshClient(SamInfo);
            client.Connect();
        }

        public static string[] ReturnDirectory()
        {
            List<string> output = new List<string>();
            foreach(var file in files)
            {
                string curFile = "";
                if (file.IsDirectory)
                    curFile = " + ";
                curFile += "\t" + file.Name;
                output.Add(curFile);
            }
            output.Sort();
            return output.ToArray();
        }
    }
}
