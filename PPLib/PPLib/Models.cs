using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPLib
{
    public struct ZipCode
    {
        public string Zip { get; set; }
        public string Type { get; set; }
        public string PrimaryCity { get; set; }
        public string County { get; set; }
        public string[] AcceptableCities { get; set; }
        public string[] UnacceptableCities { get; set; }
    }

    public struct TaxRates
    {
        //public static Dictionary<string, double> CityRates { get; set; }
        public static Dictionary<string, double> CountyRates { get; set; }
        public static Dictionary<KeyValuePair<string, string>, double> TransitRates { get; set; }
        public static double _StateRate { get; set; }

        public string County { get; set; }
        public string City { get; set; }
        
        public double StateRate { get; set; }
        public double CountyRate { get; set; }
        public double CityRate { get; set; }
        public double TransitRate { get; set; }
        public double TotalRate { get; set; }

        public TaxRates(string city, string county) : this()
        {
            City = city;
            County = county;

            StateRate = _StateRate;
            CountyRate = CountyRates[county];
            CityRate = 0.0;
            TransitRate = TransitRates.TryGetValue(
                new KeyValuePair<string, string>(county, ""), 
                out double transitrate
            ) ? transitrate : (
                TransitRates.TryGetValue(
                    new KeyValuePair<string, string>(county, city),
                    out double transitrate2
                ) ? transitrate2 : 0.0
            );
            TotalRate = StateRate + CountyRate + CityRate + TransitRate;
        }
    }

    public struct LocationZillowInfo
    {
        public string LocationID { get; set; }
        public string Bathrooms { get; set; }
        public string Bedrooms { get; set; }
        public string HomeValue { get; set; }
        public string LastPrice { get; set; }
        public string LastSoldOn { get; set; }
        public string LotSqFt { get; set; }
        public string MedianInc { get; set; }
        public string SquareFt { get; set; }
        public string TaxAssess { get; set; }
        public string YearBuilt { get; set; }
        public string ZillowPID { get; set; }

        public string Cancelled { get; set; }
        public string TotalBilled { get; set; }
        public string NumberOfInvoices { get; set; }
        public string CallbackPercentage { get; set; }
    }

    public struct ZillowData
    {
        public string InvoiceID { get; set; }
        public string SetupID { get; set; }
        public string LocationID { get; set; }
        public string ZillowPID { get; set; }
        public string Zip { get; set; }

        public string Bathrooms { get; set; }
        public string Bedrooms { get; set; }
        public string HomeValue { get; set; }
        public string LastPrice { get; set; }
        public string LastSoldOn { get; set; }
        public string LotSqFt { get; set; }
        public string MedianInc { get; set; }
        public string SquareFt { get; set; }
        public string TaxAssess { get; set; }
        public string YearBuilt { get; set; }
        public string HomeType { get; set; }
        public string TaxRate { get; set; }

        public string Active { get; set; }
        //public string TotalBilled { get; set; }
        public string NumberOfInvoices { get; set; }
        //public string CallbackPercentage { get; set; }
        public string ActiveDuration { get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("UserDef21", ZillowPID);

            dict.Add("UserDef18", String.IsNullOrWhiteSpace(Bathrooms) ? "-1" : Bathrooms);
            dict.Add("UserDef19", String.IsNullOrWhiteSpace(Bedrooms) ? "-1" : Bedrooms);
            dict.Add("UserDef17", String.IsNullOrWhiteSpace(HomeValue) ? "-1" : HomeValue);
            dict.Add("UserDef56", String.IsNullOrWhiteSpace(LastPrice) ? "-1" : LastPrice);
            dict.Add("UserDef23", String.IsNullOrWhiteSpace(LastSoldOn) ? "-1" : LastSoldOn);
            dict.Add("UserDef15", String.IsNullOrWhiteSpace(LotSqFt) ? "-1" : LotSqFt);
            dict.Add("UserDef20", String.IsNullOrWhiteSpace(MedianInc) ? "-1" : MedianInc);
            dict.Add("UserDef14", String.IsNullOrWhiteSpace(SquareFt) ? "-1" : SquareFt);
            dict.Add("UserDef22", String.IsNullOrWhiteSpace(TaxAssess) ? "-1" : TaxAssess);
            dict.Add("UserDef16", String.IsNullOrWhiteSpace(YearBuilt) ? "-1" : YearBuilt);
            dict.Add("UserDef57", String.IsNullOrWhiteSpace(HomeType) ? "-1" : HomeType);
            return dict;
        }

        private static readonly NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands;
        private static readonly IFormatProvider formatProvider = CultureInfo.CurrentCulture;
        public (double[], double[]) ToNumbers()
        {
            double result = -1.0;
            return (
                new double[]
                {
                    Double.TryParse(Bathrooms, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(Bedrooms, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(HomeValue, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(LastPrice, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(LastSoldOn/*.Length > 4 ? LastSoldOn.Substring(6) : "0"*/, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(LotSqFt, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(MedianInc, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(SquareFt, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(TaxAssess, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(YearBuilt, style, formatProvider, out result) ? result : -1.0,
                    Double.TryParse(TaxRate, style, formatProvider, out result) ? result : -1.0
                }.Select(n => Math.Max(0, n)).ToArray(),
                new double[]
                {
                    Double.Parse(Active, NumberStyles.Integer),//(bool.TryParse(Active, out bool boolResult) ? boolResult : Double.Parse(Active) == 1.0) ? 1.0 : -1.0,
                    //Double.Parse(TotalBilled, style),
                    Double.Parse(NumberOfInvoices, NumberStyles.Integer),
                    //Double.Parse(CallbackPercentage, style)
                    Double.Parse(ActiveDuration, NumberStyles.Integer)
                }
            );
        }
    }

    public struct AreaData
    {
        public string ZipCode { get; set; }
        public int Count { get; set; }
        public double Active { get; set; }
        public double Months { get; set; }

        public double[] ToVector()
        {
            return new double[]
            {
                Active,
                Count,
                Months
            };
        }
    }

    public struct BillToPaymentModel
    {
        //public string Branch { get; set; }
        public int BillToID { get; set; }
        public string Balance { get; set; }
        public int LocationCode { get; set; }
        //public string CCAutoBill { get; set; }
    }

    public struct PostedOrder
    {
        public string OrderID { get; set; }
        public string Tech { get; set; }
    }

    public class BaseVantageExport
    {
        public string CustomerID { get; set; }
        public string Office { get; set; }
        public string Name { get; set; }
    }

    public class FixCodeVantageExport : BaseVantageExport
    {
        public string Phone { get; set; }
        public string PrimaryEmail { get; set; }
        public string Zip { get; set; }
        public string Status { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string SalesStatus { get; set; }
        public string Type { get; set; }
        public string ServiceType { get; set; }
        public string ServiceFrequency { get; set; }
        public string Problems { get; set; }
        public string SpecialProblems { get; set; }
        public string Tags { get; set; }
        public DateTime InitialJobDate { get; set; }
    }

    public class PPExportVTMatch
    {
        public string Location { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string VTCust { get; set; }
        public string Service { get; set; }
        public string ServiceDescription { get; set; }
        public string Schedule { get; set; }
        public string Tech { get; set; }
        public string Targets { get; set; }
    }

    public struct CombinedServices
    {
        public string Location { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Office { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string VTCustomerID { get; set; }
        public string Type { get; set; }
        public string Problems { get; set; }
        public string SpecialProblems { get; set; }
        public string Tags { get; set; }
        public string ServiceCode { get; set; }
    }
}
