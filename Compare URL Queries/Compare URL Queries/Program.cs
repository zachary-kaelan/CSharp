using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using PPLib;

namespace Compare_URL_Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            var tests = File.ReadAllLines(@"E:\Temp\InvoiceInspectionReportFields.txt").Select(l => l.Split('&'))
                .Select(l => l.Select(p => p.Split('=')).OrderBy(p => p[0]).ToDictionary(p => HttpUtility.UrlDecode(p[0]), p => HttpUtility.UrlDecode(p[1]))).OrderBy(t => t.Count).ToList();
            tests.SaveAs(@"E:\Temp\Tests.txt");
            string[] allKeys = tests.SelectMany(t => t.Keys.ToArray()).Distinct().ToArray();
            Array.Sort(allKeys);
            Dictionary<string, string[]> variations = allKeys.ToDictionary(
                k => k,
                k => tests.FindAll(t => t.ContainsKey(k)).Select(t => t[k]).Distinct().OrderBy(v => v).ToArray()
            );
            variations = variations.ToList().FindAll(v => v.Value.Length != 1).ToDictionary(kv => kv.Key, kv => kv.Value);
            variations.SaveAs(@"E:\Temp\Variations.txt");

            //File.WriteAllLines(@"E:\Temp\Constants.txt", variations.ToList().FindAll(v => v.Value.Length == 1).Select(v => v.Key + " :=: " + v.Value.Single()));
            //Console.WriteLine(String.Join("\r\n", variations.ToList().FindAll(v => v.Value.Length != 1).Select(v => v.Key + " -:- " + v.Value.Length + (v.Value.Length <=4 ? "\r\n\t" + String.Join("\r\n\t", v.Value) : ""))));
            //Console.ReadLine();

            Regex keyValues = new Regex("(?:name|id)=(?<y>'|\")([^\"'" + @"\[\]]{3,45}?)\k<y>[^>]{0,350}?(?:(?:>\s" + "*?<option )?(?:value=(?<x>'|\"))|(?:>))([^\"']{0,155}?)(?:" + @"\k<x>|<\/textarea>)", RegexOptions.Compiled, Regex.InfiniteMatchTimeout);
            /*var duplicateKeys = keyValues.Matches(File.ReadAllText(@"E:\Temp\InvoiceFieldsTest.txt"))
                .Cast<Match>().GroupBy(m => m.Groups[1].Value, m => m.Groups[2].Value).ToList().FindAll(g => g.Count() >= 2);
            Console.WriteLine(String.Join("\r\n", duplicateKeys.Select(g => g.Key + ":\r\n\t" + String.Join("\r\n\t", g.ToArray()))));
            Console.ReadLine();*/

            var webpage = keyValues.Matches(File.ReadAllText(@"E:\Temp\InvoiceFieldsTest.txt"))
                .Cast<Match>().ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);
            var request = File.ReadAllLines(@"E:\Temp\Request.txt").Select(l => l.Split('='))
                .ToDictionary(l => HttpUtility.UrlDecode(l[0]), l => HttpUtility.UrlDecode(l[1]));

            /*string string1 = "browser-fp-data={\"language\":\"en-US\",\"color_depth\":24,\"resolution\":{\"w\":1920,\"h\":1080},\"available_resolution\":{\"w\":1920,\"h\":1040},\"timezone_offset\":240,\"session_storage\":1,\"local_storage\":1,\"indexed_db\":1,\"open_database\":1,\"cpu_class\":\"unknown\",\"navigator_platform\":\"Win32\",\"do_not_track\":\"1\",\"canvas\":\"canvas winding:yes~canvas\",\"webgl\":1,\"adblock\":0,\"has_lied_languages\":0,\"has_lied_resolution\":0,\"has_lied_os\":0,\"has_lied_browser\":0,\"touch_support\":{\"points\":0,\"event\":0,\"start\":0},\"plugins\":{\"count\":4,\"hash\":\"661c5820a590770da622de5b5297c0bd\"},\"fonts\":{\"count\":49,\"hash\":\"73a5ce890bdadb0295b20ba41e66f0ff\"},\"ts\":{\"serve\":1505396249520,\"render\":1505396248914}}&specId=yidReg&cacheStored=true&crumb=FhtevSboXsc&acrumb=kqjbooS2&c=&sessionIndex=&done=https://www.yahoo.com&googleIdToken=&authCode=&tos0=yahoo_freereg|us|en-US&tos1=yahoo_comms_atos|us|en-US&firstName=&lastName=&yid=neddy67&password=&shortCountryCode=US&phone=&mm=&dd=&yyyy=&freeformGender=";
            string string2 = "yid=neddy67&browser-fp-data={\"language\":\"en - US\",\"color_depth\":24,\"resolution\":{\"w\":1920,\"h\":1080},\"available_resolution\":{\"w\":1920,\"h\":1040},\"timezone_offset\":240,\"session_storage\":1,\"local_storage\":1,\"indexed_db\":1,\"open_database\":1,\"cpu_class\":\"unknown\",\"navigator_platform\":\"Win32\",\"do_not_track\":\"1\",\"canvas\":\"canvas winding: yes~canvas\",\"webgl\":1,\"adblock\":0,\"has_lied_languages\":0,\"has_lied_resolution\":0,\"has_lied_os\":0,\"has_lied_browser\":0,\"touch_support\":{\"points\":0,\"event\":0,\"start\":0},\"plugins\":{\"count\":4,\"hash\":\"661c5820a590770da622de5b5297c0bd\"},\"fonts\":{\"count\":49,\"hash\":\"73a5ce890bdadb0295b20ba41e66f0ff\"},\"ts\":{\"serve\":1505396249520,\"render\":1505396248914}}&specId=yidReg&cacheStored=true&crumb=FhtevSboXsc&acrumb=kqjbooS2&c=&sessionIndex=&done=https://www.yahoo.com&googleIdToken=&authCode=&tos0=yahoo_freereg|us|en-US&tos1=yahoo_comms_atos|us|en-US&firstName=&lastName=&password=&shortCountryCode=US&phone=&mm=&dd=&yyyy=&freeformGender=";
            var dict1 = string1.Split('&')
                .Select(s => s.Split('='))
                .ToDictionary(
                    url => HttpUtility.HtmlDecode(url[0]),
                    url => HttpUtility.HtmlDecode(url[1])
            );

            var dict2 = string2.Split('&')
                .Select(s => s.Split('='))
                .ToDictionary(
                    url => HttpUtility.HtmlDecode(url[0]),
                    url => HttpUtility.HtmlDecode(url[1])
            );*/

            var webpageOnly = webpage.ToList().FindAll(kv => !request.ContainsKey(kv.Key));
            var requestOnly = request.ToList().FindAll(kv => !webpage.ContainsKey(kv.Key));

            Console.WriteLine(
                "Webpage Only\r\n\t" + String.Join(
                    "\r\n\t",
                    webpageOnly.Select(
                        kv => kv.Key + " : " + kv.Value
                    ) 
            ));

            Console.WriteLine(
               "\r\nRequest Only\r\n\t" + String.Join(
                    "\r\n\t",
                    requestOnly.Select(
                        kv => kv.Key + " : " + kv.Value
                    ) 
            ));

            Console.WriteLine();

            webpage.Keys.ToArray().Select(k => webpage.Remove(k));
            request.Keys.ToArray().Select(k => request.Remove(k));

            Console.WriteLine(
                "Differing Values:\r\n\t" + String.Join(
                    "\r\n\t",
                    webpage.ToList().FindAll(
                        kv => request.TryGetValue(kv.Key, out string value) && kv.Value != value
                    ).Select(
                        kv => kv.Key + "\t - " + kv.Value + "\t : " + request[kv.Key]
                    )
                )
            );



            Console.ReadLine();
        }
    }
}
