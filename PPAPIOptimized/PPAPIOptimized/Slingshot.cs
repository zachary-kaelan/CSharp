using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;
//using Jil;
using PestPac.Model;
using PPLib;
using RestSharp;

namespace PPAPIOptimized
{
    public class Slingshot
    {
        public const string ModifiedLocInfoPat = @"(.+?)?, ?([A-Z]+)?, ?([A-Z]{1}[a-z]+)? ?(\d{5})?";
        public static Dictionary<string, string> stateAbbrvs = File.ReadAllLines(
            @"C:\Users\ZACH-GAMING\Documents\StateAbbreviations.txt")
            .Select(s => s.Split(new string[] { " :=: " }, StringSplitOptions.None))
            .ToDictionary(s => s[0], s => s[1]);
        public static Postman client { get; set; }

        public static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatString = "o",
            DateTimeZoneHandling = DateTimeZoneHandling.Local
        };

        public static LocationModel[] UploadCustomers(string JSON, bool escaped = false)
        {
            JSON = ASCIIDecode(JSON);
            if (escaped)
                JSON = Regex.Unescape(JSON);
            Lead[] leads = JsonConvert.DeserializeObject<SlingshotJSON>(JSON, settings).leads;
            JSON = null;

            return leads.Select(l => SendPOST(l)).ToArray();
        }

        public static string ASCIIDecode(string value)
        {
            return Regex.Replace(
                value,
                @"\\u(?<Value>[a-zA-Z0-9]{4})",
                m =>
                {
                    return (
                        (char)int.Parse(
                            m.Groups["Value"].Value, NumberStyles.HexNumber
                        )
                    ).ToString();
                }
            );
        }

        public static LocationModel SendPOST(Lead customer)
        {
            LocationInputModel location = new LocationInputModel();
            location.Branch = customer.location.name.Substring(8);

            string[] names = Postman.ExtractMatches(PPRGX.NAMES.Match(customer.name));
            location.FirstName = names[0];
            location.LastName = names[1];

            string[] locinfo = Postman.ExtractMatches(Regex.Match(customer.address, ModifiedLocInfoPat));
            location.State = stateAbbrvs[locinfo[2]];
            location.Address = locinfo[0];
            location.City = locinfo[1];
            location.Zip = locinfo[3];
            location.TaxCode = PestPacGeoCoding.GetTaxCode(new Params(locinfo[0], locinfo[1], location.State, locinfo[3]));

            location.Phone = customer.phone;
            location.EMail = customer.email;
            location.EnteredDate = customer.created_at;

            location.UserDefinedFields = new List<UserDefinedField>()
            {
                new UserDefinedField("# Services", customer.services.ToString())
            };

            names = null;
            locinfo = null;

            return client.CreateLocation(location);
        }
    }

    public struct SlingshotJSON
    {
        public Lead[] leads { get; set; }
        public Company[] companies { get; set; }
        public Pagination pagination { get; set; }
    }

    public struct Lead
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public DateTime created_at { get; set; }

        public string name { get; set; }
        public string email { get; set; }
        public string email_link { get; set; }
        public string phone { get; set; }
        public string phone_link { get; set; }
        public string address { get; set; }
        public string lead_type { get; set; }

        public bool is_current_customer { get; set; }
        public bool is_miscellaneous { get; set; }
        public bool is_new_customer { get; set; }
        public bool is_quotable_service { get; set; }
        public bool has_viewed { get; set; }
        public bool is_accurate { get; set; }
        public bool is_archived { get; set; }
        public bool is_canceled { get; set; }
        public bool is_correct { get; set; }
        public bool is_deferred { get; set; }
        public bool is_locked { get; set; }

        public string canceled_note { get; set; }
        public string canceled_note_addl { get; set; }
        public string corrected_note { get; set; }
        public string corrected_note_addl { get; set; }
        public string price_initial { get; set; }
        public string price_ongoing { get; set; }

        public int services { get; set; }
        public string price_initial_specialty { get; set; }
        public string price_ongoing_specialty { get; set; }
        public string services_specialty { get; set; }
        public string value { get; set; }

        public int cp_friendly { get; set; }
        public int cp_effort { get; set; }
        public int cp_energy { get; set; }
        public int cp_tone { get; set; }

        public string cp_notes { get; set; }
        public bool has_call_recordings { get; set; }
        public string commission { get; set; }
        public string commission_rate_display { get; set; }

        public IDName location { get; set; }
        public Name representative { get; set; }
        public Name[] channels { get; set; }
        public Note[] notes { get; set; }
        public IDName[] recordings { get; set; }
        public Name[] sources { get; set; }
        public CustomerItem[] items { get; set; }
    }

    public struct Pagination
    {
        public int curr { get; set; }
        public Nullable<int> next { get; set; }
        public Nullable<int> prev { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public bool first { get; set; }
        public bool last { get; set; }
    }

    public struct IDName
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public struct Name
    {
        public string name { get; set; }
    }

    public struct Note
    {
        public int id { get; set; }
        public string user_name { get; set; }
        public char user_letter { get; set; }
        public string message { get; set; }
        public string message_formatted { get; set; }
    }

    public struct DownloadName
    {
        public string download_name { get; set; }
    }

    public struct CustomerItem
    {
        public int id { get; set; }
        public string label { get; set; }
        public string value { get; set; }
        public string value_formatted { get; set; }
    }

    public struct Company
    {
        public int id { get; set; }
        public string name { get; set; }
        public IDName[] locations { get; set; }
    }
}
