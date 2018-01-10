using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSH
{
    static class Program
    {
        static FileStream logStream = new FileStream("Log.txt", FileMode.Create, FileAccess.ReadWrite);
        static void Main()
        {
            

            SSHClient.Login();
            SSHClient.client.ErrorOccurred += Client_ErrorOccurred;
            SSHClient.client.CreateShell(
                Console.OpenStandardInput(),
                Console.OpenStandardOutput(),
                logStream
            );
        }

        private static void Client_ErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            logStream.Flush();
            logStream.Dispose();
            logStream.Close();
        }
        

        /*
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        */
    }
}
