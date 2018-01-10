using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Jil;

namespace PPAPIOptimized
{
    public class HouseCanaryClient
    {
        public RestClient client { get; set; }
        //public StreamWriter logOwnerNames = new StreamWriter(@"E:\PropertyInfo\OwnerInfo\", true) { AutoFlush=true};
        public HouseCanaryClient()
        {
            client = new RestClient("https://api.housecanary.com/v2/");
            client.AddDefaultHeader("Authorization", "Basic VURVV0hCRTVPNzhMTjc1TDQ5TFU6cFZTZWpSZFFpRE9RZ2JRV0lldzBFc2pTY2dlTzVMQ2I=");
        }

        public void GetPropertyDetails(AddrZip[] addresses)
        {
            string response = client.Execute(
                new RestRequest(
                    "property/details_enhanced", 
                    Method.POST
                ).AddJsonBody(addresses)
            ).Content;
        }

        public Dictionary<AddressInfo, MortgageLien[]> GetMortgageLien(AddrZip[] addresses, out string response)
        {
            response = client.Execute(
                new RestRequest(
                    "property/mortgage_lien",
                    Method.POST
                ).AddJsonBody(addresses)
            ).Content;

            return DeserializeMortgageLien(response);
        }

        public Dictionary<AddressInfo, MortgageLien[]> DeserializeMortgageLien(string response)
        {
            var obj = JSON.Deserialize<Dictionary<string, object>[]>(response, Options.ExcludeNulls);
            Dictionary<AddressInfo, MortgageLien[]> output = new Dictionary<AddressInfo, MortgageLien[]>();
            foreach(var dict in obj)
            {
                PropertyResponse propertyResponse = JSON.Deserialize<PropertyResponse>(
                    dict["property/mortgage_lien"].ToString(),
                    Options.ExcludeNulls
                );
                if (propertyResponse.api_code == 204)
                    continue;

                AddressInfo addr = JSON.Deserialize<AddressInfo>(dict["address_info"].ToString());
                output.Add(
                    addr, propertyResponse.result == null ? 
                        new MortgageLien[] {
                            new MortgageLien()
                        } : JSON.Deserialize<MortgageLien[]>(
                            propertyResponse.result.ToString(), 
                            Options.ExcludeNulls));
            }

            /*return obj.ToDictionary(
                p => JSON.Deserialize<AddressInfo>(p["address_info"].ToString()),
                p => JSON.Deserialize<MortgageLien[]>(
                    JSON.Deserialize<PropertyResponse>(
                        p["property/mortgage_lien"].ToString(),
                        Options.ExcludeNulls
                    ).result.ToString(),
                    Options.ExcludeNulls
                )
            );*/
            return output;
        }
    }

    public struct AddrZip
    {
        public string address { get; set; }
        public string zipcode { get; set; }
    }

    public struct AddressInfo
    {
        public string city { get; set; }
        public string county_fips { get; set; }
        public string geo_precision { get; set; }
        public string block_id { get; set; }
        public string zipcode { get; set; }
        public string blockgroup_id { get; set; }
        public string address_full { get; set; }
        public string state { get; set; }
        public string msa { get; set; }
        public string metrodiv { get; set; }
        public string unit { get; set; }
        public string address { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public string slug { get; set; }
        public string zipcode_plus4 { get; set; }
    }

    public struct PropertyResponse
    {
        public string api_code_description { get; set; }
        public int api_code { get; set; }
        public object result { get; set; }
    }

    public struct PropertyDetailsResult
    {
        
    }

    public struct PropertyRecord
    {
        public int year_built { get; set; }
        
    }

    public struct Assessment
    {
        public int tax_year { get; set; }
        public string owner_name { get; set; }
    }

    public struct MortgageLien
    {
        public string event_type { get; set; }
        public int? mortgage_years { get; set; }
        public string grantor_2 { get; set; }
        public string apn { get; set; }
        public string due_date { get; set; }
        public string record_book { get; set; }
        public string grantee_2_forenames { get; set; }
        public string grantee_1 { get; set; }
        public string grantee_2 { get; set; }
        public string grantee_1_forenames { get; set; }
        public string grantor_1 { get; set; }
        public string record_date { get; set; }
        public string record_page { get; set; }
        public string lien_type { get; set; }
        public string lender_type { get; set; }
        public string record_doc { get; set; }
    }

    public struct CheckOwnerNameModel
    {
        public string SalesRep { get; set; }
        public string Active { get; set; }
        public string LocationCode { get; set; }
        public string AccountBalance { get; set; }
        public string CustomersReportedName { get; set; }
        public string OwnerPerGIS { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string LAT { get; set; }
        public string LONG { get; set; }
        public string DaysPastDue { get; set; }
        public string CreditStatus { get; set; }
        public string CConFile { get; set; }
        public string CCMessage { get; set; }
    }

    public struct OwnerNameComparison
    {
        public string PestPacName { get; set; }
        public string PropertyName { get; set; }
    }
}
