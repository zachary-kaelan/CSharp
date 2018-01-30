using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CsvHelper;
using RestSharp;
using PPRGX.PPI;
using ZachLib;
using ZachLib.Logging;
using PostmanLib;
using PestPac.Model;

namespace PPWebLib
{
    public static class PestPac
    {
        private static CookieContainer cookies = new CookieContainer();
        private static RestClient client = new RestClient("http://app.pestpac.com")
        {
            CookieContainer = cookies,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36",
            FollowRedirects = false
        };
        private static RestRequest loginRequest = new RestRequest("/default.asp?Mode=Login", Method.POST);
        private static readonly LicenseUsers RGX_LICENSES = new LicenseUsers();
        private static Thread LicenseCleanupThread = new Thread(() => LicenseCleanup());
        internal static Dictionary<string, string> CountyIDs = File.ReadAllLines(
            @"C:\DocUploads\Program Files\PPDictionaries\CountyIDs.txt"
        ).Select(
            l => l.Split(
                new string[] { " :=: "}, 
                StringSplitOptions.None
            )
        ).ToDictionary(
            l => l[0],
            l => l[1]
        );

        static PestPac()
        {
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.AddDefaultHeader("X-Requested-With", "XMLHttpRequest");

            loginRequest.AddParameter("CompanyKey", "323480", ParameterType.GetOrPost);
            loginRequest.AddParameter("Password", "I15Zac$0208", ParameterType.GetOrPost);
            loginRequest.AddParameter("RememberMe", "1", ParameterType.GetOrPost);
            loginRequest.AddParameter("RememberedAuth", "0", ParameterType.GetOrPost);
            loginRequest.AddParameter("SavePassword", "1", ParameterType.GetOrPost);
            loginRequest.AddParameter("Username", "zac.johnso", ParameterType.GetOrPost);

            LicenseCleanupThread.Start();
            Thread.Sleep(1250);
            Login();
        }

        private static void Login()
        {
            client.FollowRedirects = true;
            client.Get(new RestRequest("/"));
            client.Execute(loginRequest);
            client.FollowRedirects = false;
        }

        public static void UpdateStateTaxCodes(string path, string state)
        {
            Dictionary<string, string> stateAbbreviations = File.ReadAllLines(
                @"C:\DocUploads\Program Files\PPDictionaries\StateAbbreviations.txt"
            ).Select(
                l => l.Split(
                    new string[] { " - " },
                    StringSplitOptions.None
                )
            ).ToDictionary(
                l => l[0].ToUpper(),
                l => l[1]
            );
            Postman.GetTaxCodes(true, out IEnumerable<PostmanLib.TaxCode> existingCodes);

            StreamReader reader = new StreamReader(path);
            CsvReader csvReader = new CsvReader(reader, Utils.csvConfig);
            var taxcodes = csvReader.GetRecords<TaxCode>();
            

            string abbreviation = stateAbbreviations[state.ToUpper()];
            foreach(var taxcode in taxcodes)
            {
                string code = abbreviation + taxcode.County.Replace(" ", "").Substring(0, 3).ToUpper() + taxcode.City.Replace(" ", "").Substring(0, 3).ToUpper();
                if (!existingCodes.Any(c => c.Code == code))
                {
                    CreateTaxCode(
                        code,
                        abbreviation + " - " + taxcode.County.TitleCapitalization() + (taxcode.County.ToUpper().EndsWith("COUNTY") ? "" : " County") + " - " + taxcode.City.TitleCapitalization(),
                        Convert.ToDouble(taxcode.TotalRate) / 100.0,
                        Convert.ToDouble(taxcode.CityRate) / 100.0,
                        Convert.ToDouble(taxcode.CountyRate) / 100.0,
                        Convert.ToDouble(taxcode.StateRate) / 100.0
                    );

                    CreateTaxCodeAutoFill(abbreviation, taxcode.County, taxcode.City, code);
                }
            }

            csvReader.Dispose();
            csvReader = null;
            reader.Close();
            reader = null;
        }

        private const string XML = "<params><database></database><id>{0}</id><table>TaxCode</table></params>";
        public static bool CheckIfTaxCodeInUse(string id)
        {
            var request = new RestRequest("/xml/DeleteLookup.asp", Method.POST);
            request.AddParameter("application/x-www-form-urlencoded", String.Format(XML, id), ParameterType.RequestBody);
            return client.Execute(request).Content.Contains(">1<");
        }

        public static void DeleteTaxCode(string id)
        {
            client.Execute(
                new RestRequest(
                    "/lookup/taxCode/detail.asp",
                    Method.POST
                ).AddOrUpdateParameter(
                    "Mode",
                    "Delete",
                    ParameterType.GetOrPost
                ).AddOrUpdateParameter(
                    "TaxCodeID",
                    id,
                    ParameterType.GetOrPost
                )
            );
        }

        public static void CreateTaxCode(string code, string desc, double taxrate, double city, double county, double state)
        {
            RestRequest request = new RestRequest("/lookup/taxCode/insert.asp", Method.POST);
            request.AddParameter("OldTaxRate", 0.0, ParameterType.GetOrPost);
            request.AddParameter("TaxCodeID", "", ParameterType.GetOrPost);
            request.AddParameter("ID_5", "0.0", ParameterType.GetOrPost);
            request.AddParameter("GST", "0.0", ParameterType.GetOrPost);
            request.AddParameter("PST", "0.0", ParameterType.GetOrPost);

            request.AddParameter("Code", code, ParameterType.GetOrPost);
            request.AddParameter("Description", desc, ParameterType.GetOrPost);
            request.AddParameter("TaxRate", taxrate, ParameterType.GetOrPost);
            request.AddParameter("ID_1", city, ParameterType.GetOrPost);
            request.AddParameter("ID_2", county, ParameterType.GetOrPost);
            request.AddParameter("ID_3", state, ParameterType.GetOrPost);

            client.Execute(request);
        }

        public static void CreateTaxCodeAutoFill(string state, string county, string city, string code)
        {
            RestRequest request = new RestRequest("/lookup/taxCodeAutoFill/insert.asp", Method.POST);
            request.AddParameter("AutoFillID", "", ParameterType.GetOrPost);
            request.AddParameter("Zip", "", ParameterType.GetOrPost);

            request.AddParameter("State", code.Substring(0, 2), ParameterType.GetOrPost);
            request.AddParameter("County", county, ParameterType.GetOrPost);
            request.AddParameter("City", city, ParameterType.GetOrPost);
            request.AddParameter("TaxCode", code, ParameterType.GetOrPost);

            Postman.GetTaxCodes(true, out IEnumerable<PostmanLib.TaxCode> taxcodes);
            request.AddParameter("TaxCodeID", taxcodes.Single(t => t.Code == code).TaxCodeID, ParameterType.GetOrPost);
            taxcodes = null;
            request.AddParameter("CountyID", CountyIDs[county.ToUpper()], ParameterType.GetOrPost);

            client.Execute(request);
        }

        private const int SLEEP_TIME = 1000 * 60 * 15;
        private static void LicenseCleanup()
        {
            while(true)
            {
                var users = RGX_LICENSES.ToObjects<LicenseUser>(
                    client.Get(
                        new RestRequest("license.asp")
                    ).Content
                ).ToArray();

                DateTime now = DateTime.Now;
                if (users.Any(u => u.LogOut(now)))
                {
                    int count = users.Length;
                    RestRequest request = new RestRequest("/license.asp", Method.POST);
                    request.AddParameter("Mode", "Logout", ParameterType.GetOrPost);
                    request.AddParameter("Users", count, ParameterType.GetOrPost);
                    request.AddParameter("OverrideKey", "", ParameterType.GetOrPost);

                    for (int i = 1; i <= count; ++i)
                    {
                        LicenseUser user = users[i - 1];
                        string numString = i.ToString();

                        request.AddParameter(
                            "SessionUserID" + numString, 
                            user.SessionUserID, 
                            ParameterType.GetOrPost
                        );

                        request.AddParameter(
                            "SessionID" + numString, 
                            user.SessionID, 
                            ParameterType.GetOrPost
                        );

                        if (user.LogOut(now))
                            request.AddParameter("LogOutUser" + numString, 1, ParameterType.GetOrPost);
                    }
                }

                Thread.Sleep(SLEEP_TIME);
            }
        }
    }
}
