using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TinderRegex
{
    class Program
    {
        static void Main(string[] args)
        {
            AssemblyName name = new AssemblyName("TinderRGX, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            //name.KeyPair = new StrongNameKeyPair(File.OpenRead(@"C:\Users\ZACH-GAMING\Source\StrongKeys\ZachRGX.snk"));

            Regex.CompileToAssembly(
                new RegexCompilationInfo[]
                {
                    new RegexCompilationInfo(
                        @"(sc|snap(chat\s?|))\s*(is|me( at| here|)|)[^A-Za-z0-9]*(?<user>\S+)",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                        "Snapchat",
                        "TinderRGX",
                        true
                    ), new RegexCompilationInfo(
                        @"(ig|f?insta\S*)\s*(is|)[^A-Za-z0-9]*@?(?<user>\S+)",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                        "Instagram",
                        "TinderRGX",
                        true
                    ), new RegexCompilationInfo(
                        @"(venmo|cashapp)[\s\$]*(is|me( at| here|)|[^A-Za-z0-9]*)(?<user>\S+)",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                        "Venmo",
                        "TinderRGX",
                        true
                    )
                },
                name
            );
        }
    }
}
