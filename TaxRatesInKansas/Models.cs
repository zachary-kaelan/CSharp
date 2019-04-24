using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TaxRatesInKansas
{
    public struct Customer
    {
        public int LocationID { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string ZipCode { get; set; }
        public string TaxCode { get; set; }
        public double TaxRate { get; set; }

        public override string ToString()
        {
            return County + " - " + City + " - " + ZipCode + " - " + TaxRate.ToString();
        }
    }

    public struct ZipCodeComplex
    {
        public string zip { get; set; }
        public string type { get; set; }
        public int decommissioned { get; set; }
        public string primary_city { get; set; }
        public string acceptable_cities { get; set; }
        public string unacceptable_cities { get; set; }
        public string state { get; set; }
        public string county { get; set; }
        public string timezone { get; set; }
        public string area_codes { get; set; }
        public string world_region { get; set; }
        public string country { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int irs_estimated_population_2015 { get; set; }
    }

    public struct TempWikipedia
    {
        public string County { get; set; }
        public string City { get; set; }
    }

    public struct ChangeLogField
    {
        public string SetupID { get; set; }
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string UpdateUser { get; set; }
    }

    public struct InsightRep
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }

    public struct Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }

    public struct BrokenServiceOrder
    {
        public int OrderID { get; set; }
        public string RouteOpStartEligibleTime { get; set; }
        public string RouteOpEndEligibleTime { get; set; }
        public string TimeRange { get; set; }
    }

    public struct BrokenServiceSetup
    {
        public int LocationID { get; set; }
        public string SetupID { get; set; }
        public string CCAutoBill { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    public struct VantageEmployee
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
    }

    public struct PestPacTotalUpdate
    {
        public int LocationCode { get; set; }
        public string ServiceCode { get; set; }
        public string Schedule { get; set; }
        public string OldTotal { get; set; }
        public string NewTotal { get; set; }
        public string NewTotal2 { get; set; }
    }

    public struct InvoiceDescUpdate
    {
        public int LocationCode { get; set; }
        public int InvoiceID { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string ServiceCode { get; set; }
        public DateTime WorkDate { get; set; }
    }

    public struct SetupExportVTMatch
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
        public string Schedule { get; set; }
        public string Tech { get; set; }
        public string Targets { get; set; }

    }

    public struct VTServiceExport
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string PrimaryEmail { get; set; }
        public string Zip { get; set; }
        public string CustomerID { get; set; }
        public string Office { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string Problems { get; set; }
        public string SpecialProblems { get; set; }
        public string Tags { get; set; }
    }

    public struct CombinedServices
    {
        public int Location { get; set; }
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

    public struct FailedVTExport
    {
        public string Office { get; set; }
        public string ID { get; set; }
        public string ErrorMessage { get; set; }

        public FailedVTExport(string id, string error, string office)
        {
            Office = office;
            ID = id;
            ErrorMessage = HttpUtility.HtmlDecode(error).Trim();
            int lastColon = ErrorMessage.LastIndexOf(':');
            if (lastColon >= 0 && ErrorMessage.Length - lastColon >= 12)
                ErrorMessage = ErrorMessage.Substring(lastColon).TrimStart();
        }
    }

    public struct VTAddExport
    {
        public string Branch { get; set; }
        public string ID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }
    }
    
    public struct Setup
    {
        public string SetupID { get; set; }
    }

    public class CityHomeValueAndIncome
    {
        public int Income { get; set; }
        public int HomeValue { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }

    public class ZipHomeValueAndIncome
    {
        public int Income { get; set; }
        public int HomeValue { get; set; }
        public string Zip { get; set; }
    }

    public class SetupBranchChangeModel
    {
        public string SetupBranch { get; set; }
        public string LocationBranch { get; set; }
        public string LocationBranchID { get; set; }
        public string SetupID { get; set; }
    }

    public class TimeRangeChangesModel
    {
        public int Location { get; set; }
        public int Order { get; set; }
    }

    public class ServiceSetupTest
    {
        public int LocationCode { get; set; }
        public int LocationID { get; set; }
        public string SetupID { get; set; }
    }

    public struct SetupSkipMonths
    {
        public int Location { get; set; }
        /*public string Schedule { get; set; }
        public string Tech { get; set; }
        public double Total { get; set; }*/
    }
}
