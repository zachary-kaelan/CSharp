using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignedUnsigned
{
    class Program
    {
        public const string DocsPath = @"E:\Docs\";
        static void Main(string[] args)
        {
            string[] signed = Directory.GetFiles(DocsPath, "*signed*");
            Queue<string> qSigned = new Queue<string>(signed);
            string[] unsigned = Directory.GetFiles(DocsPath).Except(signed).ToArray();
            Queue<string> qUnsigned = new Queue<string>(unsigned);
            List<string> lines = new List<string>();
            lines.Add("Signed\tUnsigned");
            bool outofunsigned = false;
            while (true)
            {
                try
                {
                    StringBuilder sb = new StringBuilder("");
                    sb.Append(new FileInfo(qSigned.Dequeue()).Name);
                    if (!outofunsigned)
                    {
                        sb.Append("\t");
                        try
                        {
                            sb.Append(new FileInfo(qUnsigned.Dequeue()).Name);
                        }
                        catch
                        {
                            outofunsigned = true;
                        }
                    }
                    lines.Add(sb.ToString());
                }
                catch
                {
                    break;
                }
            }

            File.WriteAllLines(@"E:\Signed_Unsigned.txt", lines);
        }
    }
}
