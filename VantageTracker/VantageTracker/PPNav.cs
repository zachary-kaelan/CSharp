using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using RestSharp.Extensions;
using RestSharp.Deserializers;

namespace VantageTracker
{
    public class RequestState
    {
        const int BUFFER_SIZE = 1024;
        public Stopwatch timer;
        public StringBuilder requestData;
        //public string requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public RequestState()
        {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
            timer = new Stopwatch();
        }
    }
    public class PPNav
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        const int DEFAULT_TIMEOUT = 2 * 60 * 1000; // 2 minutes

        const string FindCustPat = @"detail.asp\?LocationID=(.+?)" + "\"";
        const string LogOutPat = "name=\"" + @"(SessionUserID\d+?)" + "\" value=\"(.+?)\">" + @"(?:\s|.)*?name=" + "\"(.+?)\" value=\"(.+?)\">" + @"\s+?(\S+?)\s+?<\/td>\s+?<td>(.+?)<\/td>\s+?<td>(.+?)<\/td>\s+?<td>(.+?)<\/td>(?:\s|.)*?>(\d)<(?:\s|.)*?name=" + "\"(.+?)\"";
        const string FindNumMatchesPat = @"Search Results \((\d*?) matches\)";
        const string KeyValuePat = "(?:<span id=\"[^\"']*?\".{15,285}?)?(?:name|id)=(?<y>'|\")([^\"'" + @"\[\]]{3,45}?)\k<y>.{0,350}?(?:(?:\s" + "*?<option )?(?:value=(?<x>'|\"))|(?:>))([^\"']{0,155}?)(?:" + @"\k<x>|<\/textarea>)";
        const string IsActivePat = "\"Active\" onclick=\"active_onClick..\" value=\"1\" checked";

        const int NUM_RETRIES = 3;
        private Stopwatch timer { get; set; }
        public List<long> responseTimes { get; set; }
        public RestClient pageClient { get; set; }
        public RestRequest pageRequest { get; set; }

        private Thread licenseCleanupThread { get; set; }

        private readonly Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"ACHAccountNumber", ""},
                {"ACHAutoBill", "1"},
                {"ACHAutoBillEndDate", ""},
                {"ACHRoutingNumber", ""},
                {"ActUpgrade", "0"},
                {"Active", "1"},
                {"AddDevices", "0"},
                {"AddDiagrams", "]:"},
                {"AnnualPrepayDiscount", "0.000"},
                {"AnnualPrepayDiscountPayment", "0.00"},
                {"Anytime", "0"},
                {"AnytimeEndAmPm", "AM"},
                {"AnytimeEndTime", ""},
                {"AnytimeStartAmPm", "AM"},
                {"AnytimeStartTime", ""},
                {"ApplicationEquipmentCode1", ""},
                {"ApplicationEquipmentID1", ""},
                {"ApplicationRateCode1", ""},
                {"ApplicationRateID1", ""},
                {"AttachToSalesOppID", ""},
                {"AutoBill", "1"},
                {"AutoBillEndDate", ""},
                {"AutoBillType", "CC"},
                {"BASquareFootage1", ""},
                {"BillToMobile", ""},
                {"BillToNumber", "708-238-7696"},
                {"BillToPhone", "708-238-7696"},
                {"BillingAmount", "0.00"},
                {"BillingInterval", "0"},
                {"BillingLastGeneratedDate", ""},
                {"BillingSchedule", ""},
                {"BillingScheduleID", ""},
                {"BranchID", "16"},
                {"CalendarSchedule", ""},
                {"CallNotify1", "1"},
                {"CanadianTax", "0"},
                {"CancelDate", ""},
                {"CancelReason", ""},
                {"CancelReasonID", ""},
                {"CancelSSOrdersNSReason", ""},
                {"CardExpMonth", "04"},
                {"CardExpYear", "2019"},
                {"CardID", "-2"},
                {"CardName", "Johnathon Luna"},
                {"CardNumber", "************4082"},
                {"CardPointer", "bda57ff0-f742-4403-9acf-4b68638fd000"},
                {"CardSourceProgramID", ""},
                {"CardSourceSetupID", ""},
                {"CardType", "Visa"},
                {"ChemGroupID1", ""},
                {"ChemIndex", "516"},
                {"ChemLines", "1"},
                {"ChemicalCalculatedEdited1", ""},
                {"ChemicalCode1", ""},
                {"ChemicalID1", ""},
                {"ChemicalLineChanged1", ""},
                {"Color", "000099"},
                {"Comment", "**Do not touch treated areas until dry** **Keep pets and children away from treated areas**"},
                {"CommentCode", "MISC"},
                {"CommentID", "0"},
                {"CommissionEndDate", ""},
                {"CommissionStartDate", ""},
                {"CompanyIDForNotifications", "1"},
                {"CopesanGUID", ""},
                {"Cost1", "0.00"},
                {"DatabaseForNotifications", "Provider=SQLOLEDB.1;Password=XGkLXvRzP!3PP647*5UBQMjYw#c3tBgCS;Persist Security Info=True;User ID=sa;Initial Catalog=PestPac6542;Data Source=PP-AWS-E-SQL19"},
                {"Days", "0"},
                {"DaysToFloat", ""},
                {"Description1", "Quarterly Pest Control Service"},
                {"DilutionCalcType1", ""},
                {"DilutionFactor1_1", ""},
                {"DilutionFactor2_1", ""},
                {"DilutionInputMethod1", "T"},
                {"Discount", "0.000"},
                {"Division", "PEST"},
                {"DivisionID", "2"},
                {"DoNotSendTech", ""},
                {"DoNotSendTechID", ""},
                {"Duration", "00:30"},
                {"EmailNotify1", "1"},
                {"ExpirationDate", ""},
                {"ExtendedPrice1", "99.00"},
                {"FName1", ""},
                {"From", ""},
                {"GLCodeID", ""},
                {"GLCodeID1", "0"},
                {"HasDefaultAttribute1", "0"},
                {"HiddenDescription1", "Quarterly Pest Control Service"},
                {"HiddenDetailedDescription1", "Quarterly Pest Control Service"},
                {"ISSHFMID", "0"},
                {"ISSReasonID", "0"},
                {"ISSRequireReasonOnSave", "0"},
                {"IncreasePrice", "1"},
                {"InitialDuration", "00:15"},
                {"InitialServiceCode", "INPC"},
                {"InitialServiceDate", "06/21/2017"},
                {"InitialServicePrice", "99.00"},
                {"InitialServiceTime", "12:58"},
                {"InitialTaxable", "1"},
                {"InitialTech", "MIC.WROBLE"},
                {"LName1", ""},
                {"LastGeneratedDate", "06/28/2017"},
                {"LastServiceDate", "06/21/2017"},
                {"LineIndex", "104"},
                {"LinearFootage1", ""},
                {"Lines", "1"},
                {"LocationID", "152306"},
                {"LocationMobile", ""},
                {"LocationNumber", "708-238-7696"},
                {"LocationPhone", "708-238-7696"},
                {"Measurement", ""},
                {"MeasurementType", ""},
                {"MeasurementTypeID", "0"},
                {"MethodID1", ""},
                {"MethodOfAppl1", ""},
                {"Mode", "Save"},
                {"MultTargetsChanged", ""},
                {"Net", "30"},
                {"NewMultTargets", ""},
                {"NewMultTargetsService", ""},
                {"NextChemRow", "9"},
                {"NextGeneratedDate", "09/24/2017"},
                {"NextServiceDate", ""},
                {"NotificationCount", "1"},
                {"NotificationDays1", "0"},
                {"NotificationTime1", ""},
                {"NotificationTimeID1", ""},
                {"OChemicalCode1", ""},
                {"OInitialServiceCode", "INPC"},
                {"OSchedule", "Q3 MJSD"},
                {"OServiceCode1", "QTPC"},
                {"OfServiceCode1", "QTPC"},
                {"OldActive", "1"},
                {"OldBranchID", "16"},
                {"OldCalTechID1", ""},
                {"OldCancel", ""},
                {"OldComment", "**Do not touch treated areas until dry** **Keep pets and children away from treated areas**"},
                {"OldRouteOptExcludeTimeBeg", ""},
                {"OldRouteOptExcludeTimeBegAmPm", ""},
                {"OldRouteOptExcludeTimeEnd", ""},
                {"OldRouteOptExcludeTimeEndAmPm", ""},
                {"OldRouteOptTime1Beg", ""},
                {"OldRouteOptTime1BegAmPm", ""},
                {"OldRouteOptTime1End", ""},
                {"OldRouteOptTime1EndAmPm", ""},
                {"OldRouteOptTime2Beg", ""},
                {"OldRouteOptTime2BegAmPm", ""},
                {"OldRouteOptTime2End", ""},
                {"OldRouteOptTime2EndAmPm", ""},
                {"OldScheduleID", "205"},
                {"OldSetupBranchID", "16"},
                {"OldSetupBranchIDLoaded", "16"},
                {"OldTimeRange", "7:00A-7:00P"},
                {"OldUserDef14", "0"},
                {"OrderReasonID1", ""},
                {"OriginalACHAccountNumber", ""},
                {"OriginalACHRoutingNumber", ""},
                {"OriginalAutoBillType", "CC"},
                {"OriginalCardExpMonth", "04"},
                {"OriginalCardExpYear", "2019"},
                {"OriginalCardNumber", "************4082"},
                {"OriginalCardPointer", "bda57ff0-f742-4403-9acf-4b68638fd000"},
                {"OriginalCardType", "Visa"},
                {"POExpirationDate", ""},
                {"PONumber", ""},
                {"PST", "0"},
                {"PSTonGST", "0"},
                {"Phone1", ""},
                {"PreferredTechID", ""},
                {"PreserveCardNumber", "0"},
                {"PriceByMeasurementBasePrice1", ""},
                {"PriceByMeasurementComment1", ""},
                {"PriceByMeasurementCustomerDiscountDollars1", ""},
                {"PriceByMeasurementCustomerDiscountPercent1", ""},
                {"PriceByMeasurementDelete1", "1"},
                {"PriceByMeasurementDiscountExpirationDate1", ""},
                {"PriceByMeasurementLineMeasurementID1", ""},
                {"PriceByMeasurementLocation1", ""},
                {"PriceByMeasurementMinPrice1", ""},
                {"PriceByMeasurementOverride1", "0"},
                {"PriceByMeasurementPriceAdjustor1", ""},
                {"PriceByMeasurementPriceAdjustorReasonID1", ""},
                {"PriceByMeasurementPriceDate1", ""},
                {"PriceByMeasurementPrintDetail1", "0"},
                {"PriceByMeasurementQuantity1", ""},
                {"PriceByMeasurementTypeID1", ""},
                {"PriceByMeasurementUnitPrice1", ""},
                {"PriceIncrDate", ""},
                {"PriceIncreaseMonths", "0"},
                {"ProgramGeneratePrompt", ""},
                {"Qty1", ""},
                {"Quantity1", "1.00"},
                {"Reason11", ""},
                {"Reason21", ""},
                {"ReasonID11", ""},
                {"ReasonID21", ""},
                {"RenewalDate", ""},
                {"RenewalOrSetup", "S"},
                {"Reserved", ""},
                {"Route", "CHI-01"},
                {"RouteID", "39"},
                {"RouteOptDaysFromLastService", ""},
                {"RouteOptEDayOfMonth", ""},
                {"RouteOptEDayOfYear", ""},
                {"RouteOptExcludeTimeBeg", ""},
                {"RouteOptExcludeTimeBegAmPm", ""},
                {"RouteOptExcludeTimeEnd", ""},
                {"RouteOptExcludeTimeEndAmPm", ""},
                {"RouteOptIncludeDay1", "1"},
                {"RouteOptIncludeDay2", "1"},
                {"RouteOptIncludeDay3", "1"},
                {"RouteOptIncludeDay4", "1"},
                {"RouteOptIncludeDay5", "1"},
                {"RouteOptIncludeDay6", "1"},
                {"RouteOptIncludeDay7", "1"},
                {"RouteOptSDayOfMonth", ""},
                {"RouteOptSDayOfYear", ""},
                {"RouteOptTime1Beg", ""},
                {"RouteOptTime1BegAmPm", "AM"},
                {"RouteOptTime1End", ""},
                {"RouteOptTime1EndAmPm", "AM"},
                {"RouteOptTime2Beg", ""},
                {"RouteOptTime2BegAmPm", "AM"},
                {"RouteOptTime2End", ""},
                {"RouteOptTime2EndAmPm", "AM"},
                {"STCubicFootage1", ""},
                {"SalesOppReasonID1", ""},
                {"Schedule", "Q3 MJSD"},
                {"ScheduleChanged", "NO"},
                {"ScheduleID", "205"},
                {"ScheduleInterval", "0"},
                {"ServiceCode1", "QTPC"},
                {"ServiceDuration1", ""},
                {"ServiceID1", "20"},
                {"SetupBranchID", "16"},
                {"SetupChemLineID1", "0"},
                {"SetupID", "150367"},
                {"SetupLineID1", "151024"},
                {"SetupReasonID1", "547984"},
                {"SetupSelectedCardID", "bda57ff0-f742-4403-9acf-4b68638fd000"},
                {"SetupType", "SO"},
                {"Source", "SSALES"},
                {"SourceID", "3"},
                {"StartDate", "06/21/2017"},
                {"StationSetupID", ""},
                {"SubTotal", "99.00"},
                {"Target1", ""},
                {"TargetID1", ""},
                {"Tax", "0.00"},
                {"TaxRate", "0"},
                {"Taxable1", "1"},
                {"Tech1", "MIC.WROBLE"},
                {"Tech2", ""},
                {"Tech3", "17A.SAMAD"},
                {"Tech4", ""},
                {"Tech5", ""},
                {"TechBonus1", "0.00"},
                {"TechBonus2", "0.00"},
                {"TechBonus3", "0.00"},
                {"TechBonus4", "0.00"},
                {"TechBonus5", "0.00"},
                {"TechComm1", "0.00"},
                {"TechComm2", "0.00"},
                {"TechComm3", "0.00"},
                {"TechComm4", "0.00"},
                {"TechComm5", "0.00"},
                {"TechID1", "3870"},
                {"TechID2", ""},
                {"TechID3", "3342"},
                {"TechID4", ""},
                {"TechID5", ""},
                {"TechNoCommission1", ""},
                {"TechNoCommission2", ""},
                {"TechNoCommission3", ""},
                {"TechNoCommission4", ""},
                {"TechNoCommission5", ""},
                {"TechShare1", "1"},
                {"TechShare2", "1"},
                {"TechShare3", "1"},
                {"TechShare4", "1"},
                {"TechShare5", "1"},
                {"TechShow1", "1"},
                {"TechShow2", "1"},
                {"TechShow3", "1"},
                {"TechShow4", "1"},
                {"TechShow5", "1"},
                {"Terms", "NET 30"},
                {"TermsID", "4"},
                {"TimeRange", "7:00A-7:00P"},
                {"Total", "99.00"},
                {"UndilutedQty1", ""},
                {"UndilutedUOM1", ""},
                {"UndilutedUOMID1", ""},
                {"UnitOfMeasure1", ""},
                {"UnitOfMeasureID1", ""},
                {"UnitPrice1", "99.00"},
                {"UpdateCCProgramIDs", ""},
                {"UpdateCCSetupAutoBill", ""},
                {"UpdateCCSetupIDs", ""},
                {"UserDef14", ""},
                {"WhoToNotify1", "L"},
                {"WorkAmPm", "AM"},
                {"WorkTime", "07:00"}
            };

        private readonly KeyValuePair<string,string>[] Defaults = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("ApplicationEquipmentCode1", ""),
            new KeyValuePair<string, string>("ApplicationEquipmentID1", ""),
            new KeyValuePair<string, string>("ApplicationRateCode1", ""),
            new KeyValuePair<string, string>("ApplicationRateID1", ""),
            new KeyValuePair<string, string>("BASquareFootage1", ""),
            new KeyValuePair<string, string>("ChemicalCode1", ""),
            new KeyValuePair<string, string>("DilutionFactor2_1", ""),
            new KeyValuePair<string, string>("LinearFootage1", ""),
            new KeyValuePair<string, string>("MethodOfAppl1", ""),
            new KeyValuePair<string, string>("Qty1", ""),
            new KeyValuePair<string, string>("STCubicFootage1", ""),
            new KeyValuePair<string, string>("Target1", ""),
            new KeyValuePair<string, string>("UndilutedQty1", ""),
            new KeyValuePair<string, string>("UndilutedUOM1", ""),
            new KeyValuePair<string, string>("UnitOfMeasure1", "")
        };

        public readonly Dictionary<string, string> urls = new Dictionary<string, string>()
        {
            {"base", "http://app.pestpac.com"},
            {"login", "http://app.pestpac.com/default.asp?Mode=Login"},
            {"getUser", "http://app.pestpac.com/userAuthenticatedEmails/ajax/getUser.asp?UserID="},
            {"search", "http://app.pestpac.com/location/search.asp"},
            {"getTok", "https://is.workwave.com/oauth2/token?scope=openid"},
            {"lgnAPI", "https://loginapi.pestpac.com/api/tenants/323480"},
            {"altigen", "http://app.pestpac.com/xml/getAltigenCredentials.asp"},
            {"globals", "http://app.pestpac.com/loadGlobals.asp"},
            {"locDefault", "http://app.pestpac.com/location/default.asp"},
            {"elmWarning", "http://app.pestpac.com/elementWarning.asp"},
            {"userAck", "http://app.pestpac.com/useracknowledgments/ajax/getAcknowledgment.asp?UserID=4084&Type=user.forced-redirect"},
            {"proxy", "webpoxy/api/sessions"},
            {"license", "http://app.pestpac.com/license.asp"},
            {"delDoc", "http://app.pestpac.com/letters/document/detail.asp"}
        };

        public readonly Dictionary<string, string> refs = new Dictionary<string, string>()
        {
            {"login", "http://app.pestpac.com/default.asp?Mode=Logout"},
            {"lgnAPI", "http://app.pestpac.com/"},
            {"getUser", "http://app.pestpac.com/default.asp"},
            {"search", "http://app.pestpac.com/location/"},     //default.asp
            {"license", "http://app.pestpac.com/license.asp"},
            {"delDoc", "http://app.pestpac.com/letters/document/detail.asp?Mode=Edit&LocDocumentID={0}&LocationID={1}"}
        };

        public readonly CookieContainer Cookies = new CookieContainer();

        private readonly Dictionary<string, string> creds = new Dictionary<string, string>()
        {
            {"CompanyKey", "323480"},
            {"Password", "I15Zac$0208"},
            {"RememberMe", "1"},
            {"RememberedAuth", "0"},
            {"SavePassword", "1"},
            {"Username", "zac.johnso"}
        };

        public string referer { get; set; }

        public PPNav()
        {
            ServicePointManager.UseNagleAlgorithm = false;
            //WebProxy proxy = new WebProxy();
            timer = new Stopwatch();
            responseTimes = new List<long>();

            this.GET("base", "text/html, application/xhtml+xml, image/jxr, */*");
            //this.referer = "http://app.pestpac.com/default.asp?Mode=Logout";
            this.referer = urls["base"];
            //this.GET("altigen", "application/xml, text/xml, */*; q=0.01");
            this.GET("lgnAPI");
            this.POST("login", creds);
            //t.Wait();
            this.GET("globals", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            this.GET("elmWarning", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            this.GET("locDefault", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            this.referer = "http://app.pestpac.com/location/default.asp";
            this.GET("altigen", "application/xml, text/xml, */*; q=0.01");
            this.GET("getUser");
            this.GET("userAck", "application/json, text/javascript, */*; q=0.01");
            //this.SetProxy();

            pageClient = new RestClient("http://app.pestpac.com/");
            pageClient.FollowRedirects = false;
            pageClient.CookieContainer = this.Cookies;
            pageClient.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            pageClient.AddDefaultHeader("Accept-Encoding", "gzip, deflate");
            pageClient.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8");
            pageClient.AddDefaultHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36");
            pageClient.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            pageClient.AddDefaultHeader("Host", "app.pestpac.com");

            pageRequest = new RestRequest("location/search.asp", Method.GET);
            pageRequest.AddHeader("Referer", "http://app.pestpac.com/location/search.asp");
            pageRequest.AddQueryParameter("Start", "");
            pageRequest.AddQueryParameter("Sort", "LName");
            pageRequest.AddQueryParameter("SortDesc", "0");
            pageRequest.AddQueryParameter("Locationsln", "");
            

            this.licenseCleanupThread = new Thread(this.LicenseCleanup);
            this.licenseCleanupThread.Start();
        }

        public HttpWebRequest defaultReq(string url)
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            //request.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US, en;q=0.8");
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            request.Headers.Add("upgrade-insecure-requests", "1");
            //request.Headers.Add("x-devtools-emulate-network-conditions-client-id", "df7cd306-a36f-4bed-b988-d37174767c4a");
            //request.Headers.Add("x-devtools-request-id", "25464.962");
            request.Headers.Add("Origin", urls["base"]);
            //request.Host = "https://myvantagetracker.com";
            request.AllowAutoRedirect = false;
            request.CookieContainer = this.Cookies;
            request.KeepAlive = true;
            request.Host = Regex.Match(url, @"https?:\/\/(.+?).com").Groups[1].Value + ".com";
            //request.Connection = "keep-alive";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
            request.Headers.Add("x-requested-with", "XMLHttpRequest");

            return request;
            //request.Headers.Add(HttpRequestHeader.Connection, "keep-alive");
        }

        public void SetProxy()
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(urls["proxy"]);
            request.Method = "OPTIONS";
            request.Accept = "*/*";
            request.Headers.Add("Origin", urls["base"]);
            request.Referer = this.referer;
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063";
            request.Headers.Add("Access-Control-Request-Headers", "Authorization");
            request.Headers.Add("Access-Control-Request-Method", "POST");
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Add("Host", "webproxy");
            request.ContentLength = 0;
            request.Headers.Add(HttpRequestHeader.Pragma, "no-cache");
            request.KeepAlive = true;
            request.CookieContainer = this.Cookies;
            request.GetResponse();
        }

        public Dictionary<string, string> CreateQuery(Dictionary<string,string> query)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"Active", ""},
                {"Address", ""},
                {"Address2", ""},
                {"Attributes", ""},
                {"AutoBillType", ""},
                {"BBalance", ""},
                {"BCCExpirationDate", ""},
                {"BCancelDate", ""},
                {"BCollectionChangeDate", ""},
                {"BContactDate", ""},
                {"BDaysPastDue", ""},
                {"BEnteredDate", ""},
                {"BExpirationDate", ""},
                {"BNextSentriconDate", ""},
                {"BPOExpirationDate", ""},
                {"BPriceIncreaseDate", ""},
                {"BRenewalDate", ""},
                {"BRenewedThrough", ""},
                {"BSales", ""},
                {"BSalesDate", ""},
                {"BServiceDate", ""},
                {"BStartDate", ""},
                {"Balance", "B"},
                {"BillToCode", ""},
                {"BranchID", "0"},
                {"Builder", ""},
                {"BuilderID", ""},
                {"CMType", ""},
                {"CallLeadID", ""},
                {"CancelReason", ""},
                {"City", ""},
                {"Company", ""},
                {"ContactCode", ""},
                {"ContactCodeID", ""},
                {"CopesanLocationNum", ""},
                {"CopesanTicketNum", ""},
                {"CorporationCode", ""},
                {"CorporationID", ""},
                {"CorporationRegionID", ""},
                {"County", ""},
                {"CreditAlert", "1"},
                {"CreditCollection", "1"},
                {"CreditHold", "1"},
                {"CreditNormal", "1"},
                {"DefaultAreaCode", ""},
                {"Division", ""},
                {"EBalance", ""},
                {"ECCExpirationDate", ""},
                {"ECancelDate", ""},
                {"ECollectionChangeDate", ""},
                {"EContactDate", ""},
                {"EDaysPastDue", ""},
                {"EEnteredDate", ""},
                {"EExpirationDate", ""},
                {"ENextSentriconDate", ""},
                {"EPOExpirationDate", ""},
                {"EPriceIncreaseDate", ""},
                {"ERenewalDate", ""},
                {"ERenewedThrough", ""},
                {"ESales", ""},
                {"ESalesDate", ""},
                {"EServiceDate", ""},
                {"EStartDate", ""},
                {"Email", ""},
                {"EmailAddress", ""},
                {"FName", ""},
                {"FinanceCharge", ""},
                {"Frequency", ""},
                {"Header1", ""},
                {"Header2", ""},
                {"InternalIdentifier", ""},
                {"InvoiceNum", ""},
                {"InvoiceServiceCode", ""},
                {"IsCopesan", ""},
                {"LName", ""},
                {"Liaison1", ""},
                {"Liaison2", ""},
                {"LocType", ""},
                {"LocationActive", "1"},
                {"LocationCode", ""},
                {"LocationDirection", ""},
                {"Mailings", ""},
                {"MapCode", ""},
                {"Mode", ""},
                {"OrderNum", ""},
                {"POExpirationDateSearchType", "S"},
                {"Phone", ""},
                {"PhoneExt", ""},
                {"ProgramActive", ""},
                {"ProgramType", ""},
                {"Prospect", ""},
                {"Route", ""},
                {"ScheduleCode", ""},
                {"SearchInFields", ""},
                {"SentriconSetup", ""},
                {"ServiceClass", ""},
                {"ServiceClassIDs", ""},
                {"ServiceCode", ""},
                {"ServiceCodeIDs", ""},
                {"ServiceCodeNot", ""},
                {"ServiceDivision", ""},
                {"SetupComment", ""},
                {"SetupType", ""},
                {"Sort", "UserDef10"},
                {"Source", ""},
                {"SourceID", ""},
                {"State", ""},
                {"Street", ""},
                {"Subdivision", ""},
                {"SubdivisionID", ""},
                {"TargetPest", ""},
                {"TargetPestID", ""},
                {"TaxCode", ""},
                {"Tech2", ""},
                {"Tech3", ""},
                {"Tech4", ""},
                {"Tech5", ""},
                {"Technician", ""},
                {"Terms", ""},
                {"Type", ""},
                {"UseAdvanced", "0"},
                {"UseAnnualPrepayDiscount", ""},
                {"UserDef1", ""},
                {"UserDef10", ""},
                {"UserDef11", ""},
                {"UserDef12", ""},
                {"UserDef13", ""},
                {"UserDef2", ""},
                {"UserDef24", ""},
                {"UserDef25", ""},
                {"UserDef26", ""},
                {"UserDef27", ""},
                {"UserDef28", ""},
                {"UserDef29", ""},
                {"UserDef30", ""},
                {"UserDef31", ""},
                {"UserDef5", ""},
                {"UserDef8", ""},
                {"WDOReportNum", ""},
                {"WebAccess", ""},
                {"Zip", ""}
            };

            foreach(var entry in query)
            {
                try
                {
                    dict[entry.Key] = entry.Value;
                }
                catch
                {
                    continue;
                }
            }

            return dict;
        }

        public string POST(string URL, Dictionary<string,string> form, params string[] urlformat)
        {
            HttpWebRequest request = this.defaultReq(urls[URL]);
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 10 * 60 * 1000;
            
            request.Referer = refs.TryGetValue(URL, out string r) ? String.Format(r, urlformat) : this.referer;
            StringBuilder sb = new StringBuilder();
            int count = form.Keys.Count;
            for(int i = 0; i < count; ++i)
            {
                if (sb.Length != 0)
                    sb.Append("&");
                sb.Append(HttpUtility.UrlEncode(form.Keys.ElementAt(i)));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(form[form.Keys.ElementAt(i)]));
            }

            var data = Encoding.ASCII.GetBytes(sb.ToString());
            request.ContentLength = data.Length;


            var stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Dispose();
            stream.Close();

            RequestState state = null;
            string returnString = "FAILED";
            for (int i = 0; i < NUM_RETRIES; ++i)
            {
                try
                {
                    state = new RequestState();
                    state.request = request;
                    state.timer.Reset();
                    state.timer.Start();
                    IAsyncResult result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(RespCallback), state);
                    ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), request, DEFAULT_TIMEOUT, true);
                    allDone.WaitOne();

                    //state.response.Close();
                    var endState = (RequestState)result.AsyncState;
                    returnString = endState.requestData.ToString();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nPOST Exception raised!");
                    Console.WriteLine("\nMessage:{0}", e.Message);
                    if (e.GetType() == typeof(WebException))
                    {
                        var webE = (WebException)e;
                        Console.WriteLine("\nStatus:{0}", webE.Status);
                    }
                }
            }
            //this.timer.Stop();
            this.responseTimes.Add(state.timer.ElapsedMilliseconds);
            //this.timer.Reset();
            //Console.WriteLine("FAILED");
            return returnString;
            //var t = new TaskCompletionSource<string>();
        }

        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                    request.Abort();
            }
        }

        private static void RespCallback (IAsyncResult result)
        {
            try
            {
                RequestState state = (RequestState)result.AsyncState;
                HttpWebRequest request = state.request;
                state.response = (HttpWebResponse)request.EndGetResponse(result);
                state.timer.Stop();

                state.streamResponse = state.response.GetResponseStream();
                //var sw = new StreamReader(state.streamResponse);

                //var resResult = state.streamResponse.BeginRead(state.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallback), state);
                int read = 0;
                do
                {
                    var t = state.streamResponse.ReadAsync(state.BufferRead, 0, BUFFER_SIZE);
                    t.Wait();
                    read = t.Result;
                    state.requestData.Append(Encoding.ASCII.GetString(state.BufferRead, 0, read));
                } while (read > 0);
                allDone.Set();
                return;
            }
            catch (WebException e)
            {
                Console.WriteLine("\nRespCallback Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }
        }

        /*
        private static void ReadCallback(IAsyncResult result)
        {
            try
            {
                RequestState state = (RequestState)result.AsyncState;
                int read = state.streamResponse.EndRead(result);

                if (read > 0)
                {
                    state.requestData.Append(Encoding.ASCII.GetString(state.BufferRead, 0, read));
                    IAsyncResult res4 = state.streamResponse.BeginRead(state.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallback), state);
                    return;
                }
                else
                {
                    //state.streamResponse.Close();
                }
            }
            catch (WebException e)
            {
                Console.WriteLine("\nReadCallback Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();
        }
        */

        public string GET(string URL, string Accept = "*/*")
        {
            HttpWebRequest request = this.defaultReq(urls[URL]);
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Accept = Accept;
            request.Referer = refs.TryGetValue(URL, out string r) ? r : this.referer;
            return new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd();
        }


        public List<Dictionary<string,string>> FindCust(Dictionary<string, string> query)
        {
            int start = 1;
            int numMatches = 0;
            List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            string response = null;
            
                
            void AddMatchesToResults()
            {
                Match[] matches = Regex.Matches(response, @"(?:(?:<td nowrap>(.+?)&nbsp;&nbsp;<\/td>))").Cast<Match>().ToArray();
                string[] locIDs = Regex.Matches(response, "LocationID=(.+?)\"").Cast<Match>().Select(m => m.Groups[1].Value).ToArray();
                int count = locIDs.Length;
                int index = 5;

                for (int i = 0; i < count; ++i)
                {
                    //int start = (7 * i) + 1;
                    string[] info = matches.TakeWhile((m, ind) => ind <= index).Select(m => m.Groups[1].Value).ToArray();
                    index += 6;
                    string[] names = info[0].Split(' ');
                    Dictionary<string, string> result = new Dictionary<string, string>()
                    {
                        {"FirstName",  names[0]},
                        {"LastName", names.Last()},
                        {"Address", info[1]},
                        {"City", info[2]},
                        {"State", info[3]},
                        {"Phone", info[4]},
                        {"Branch", info[5]},
                        {"LocationID", locIDs[i]}
                    };

                    results.Add(result);
                }
            }

            bool GetNextPage()
            {
                start += 10;
                if (start > numMatches)
                    return false;
                this.pageRequest.Parameters.Single(p => p.Name == "Start").Value = start;
                response = this.pageClient.Execute(this.pageRequest).Content;
                return true;
            }

            response = this.POST("search", this.CreateQuery(query));
            
            if (!response.Contains("This object"))
            {
                numMatches = Convert.ToInt32(Regex.Match(response, FindNumMatchesPat).Groups[1].Value);
                if (numMatches == 0 || numMatches > 50)
                    return new List<Dictionary<string, string>>();
                do
                {
                    AddMatchesToResults();
                } while (GetNextPage());
                return results;
            }
            else
            {
                return new List<Dictionary<string, string>>() {
                    new Dictionary<string, string>() {
                    {
                        "URL", Regex.Match(response, FindCustPat).Groups[1].Value }
                    }
                };
            }
        }

        public void DeleteNote(string LocID, object NoteID, string note)
        {
            RestRequest request = new RestRequest("notes/detail.asp", Method.POST);
            request.Parameters.AddRange(new Parameter[] {
                new Parameter() { Name = "Mode", Value = "Delete", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "NoteID", Value = NoteID, Type = ParameterType.GetOrPost },
                new Parameter() { Name = "NextPage", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "LocationID", Value = LocID, Type = ParameterType.GetOrPost },
                new Parameter() { Name = "CallID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "CallLeadID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "IsQuestion", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "TaskID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "SalesOppID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "NonCycle", Value = "False", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "PMIDetailID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "IssueID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "OrderID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "InvoiceID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "IsEmail", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "Date", Value = "08/10/2017", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "Time", Value = "10:54", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "AmPm", Value = "AM", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "NoteCode", Value = "GEN", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "NoteCodeID", Value = "", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "Duration", Value = "0", Type = ParameterType.GetOrPost },
                new Parameter() { Name = "Note", Value = note, Type = ParameterType.GetOrPost },
                new Parameter() { Name = "ExpirationDate", Value = "", Type = ParameterType.GetOrPost }
            });

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            this.pageClient.Execute(request);
        }

        public string StartReportWriter()
        {
            RestRequest request = new RestRequest("PestPacReportWriter.asp", Method.GET);
            request.AddParameter("Action", "CreateReport", ParameterType.GetOrPost);
            var response = pageClient.Execute(request);
            return response.Headers.Single(h => h.Name == "Location").Value.ToString();
        }

        public List<Dictionary<string,string>> FindCust(string phone, string VTID = "")
        {
            return this.FindCust(new Dictionary<string, string>()
            {
                {"Phone", phone},
                {"UserDef10", VTID}
            });
        }

        public void UpdateSS(string setupid, Dictionary<string, string> dict = null, string techID = null, string techName = null)
        {
            RestRequest request = new RestRequest("serviceSetup/detail.asp", Method.GET);
            request.AddQueryParameter("Mode", "Edit");
            request.AddQueryParameter("SetupID", setupid);
            string content = pageClient.Execute(request).Content;
            if (Regex.IsMatch(content, IsActivePat))
                return;

            var postData = Regex.Matches(
                content,
                KeyValuePat
            ).Cast<Match>().Select(
                m => new KeyValuePair<string, string>(
                    m.Groups[1].Value,
                    m.Groups[2].Value
                )
            ).ToList();
            content = null;

            postData.RemoveAll(
                kv => !this.dict.ContainsKey(kv.Key)
            );
            postData.AddRange(Defaults);

            /*postData.FindAll(
                kv => postData.FindAll(
                    _kv => _kv.Key == kv.Key
                ).Count > 1
            ).Distinct().ToList().ForEach(
                kv => postData.RemoveAt(
                    postData.IndexOf(kv)
                )
            );*/

            var postDict = postData.
                GroupBy(x => x.Key)
                .Select(g => g.First())
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value
            );
            
            if (techID != null && techName != null)
            {
                postDict["Tech1"] = techName;
                postDict["TechID1"] = techID;
            }

            string addDiagrams = postDict["AddDiagrams"];
            postDict.Remove("AddDiagrams");
            postDict.Add("AddDiagrams[]", addDiagrams);
            postDict.Remove("Active");
            postDict.Remove("Taxable1");
            postDict.Remove("InitialTaxable");
            postDict.Remove("AutoBill");

            RestRequest post = new RestRequest("serviceSetup/detail.asp", Method.POST);
            //post.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            post.AddHeader("Referer", "http://app.pestpac.com/serviceSetup/detail.asp?Mode=Edit&SetupID=" + setupid);
            post.Parameters.AddRange(
                postDict.Select(
                    kv => new Parameter()
                    {
                        Name=kv.Key,
                        Value=kv.Value,
                        Type=ParameterType.GetOrPost
                    }
                )
            );

            pageClient.Execute(post);
        }

        public void LicenseCleanup()
        {
            const int SLEEP_TIME = 1000 * 60 * 15;
            while (true)
            {
                try
                {
                    HttpWebRequest request = HttpWebRequest.CreateHttp(urls["license"]);
                    request.Method = "GET";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                    request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                    request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                    request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                    request.CookieContainer = this.Cookies;
                    request.Headers.Add("Upgrade-Insecure-Requests", "1");

                    List<User> users = Regex.Matches(
                        new StreamReader(
                            ((HttpWebResponse)request.GetResponse())
                            .GetResponseStream()
                        ).ReadToEnd(),
                        LogOutPat
                    ).Cast<Match>().Select(m => new User(m)).ToList();

                    int count = users.Count;

                    if (users.Count > 0)
                    {
                        Dictionary<string, string> post = new Dictionary<string, string>()
                        {
                            {"Mode", "Logout"},
                            {"OverrideKey", ""},
                            {"Users", count.ToString()}
                        };

                        var postData = post.ToList();

                        int purged = 0;

                        for (int i = 0; i < count; ++i)
                        {
                            postData.Add(users[i].SessID);
                            postData.Add(users[i].SessUserID);
                            if (users[i].LogOut.Value == "1")
                            {
                                postData.Add(users[i].LogOut);
                                ++purged;
                            }
                        }

                        File.WriteAllLines(@"C:\Users\ZACH-GAMING\Documents\LICENSE_POST.txt",
                            postData.OrderBy(kv => kv.Key).Select(kv => kv.Key + "\t|\t" + kv.Value));
                        postData.RemoveAll(e => e.Key.Contains('"') || e.Value.Contains('"'));
                        post = postData.ToDictionary(e => e.Key, e => e.Value);

                        Console.WriteLine("Purging {0} users...", purged);
                        Console.WriteLine(this.POST("license", post));
                    }
                    

                    /*
                    request = HttpWebRequest.CreateHttp("");
                    request.Method = "POST";

                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
                    request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                    request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                    request.CookieContainer = this.Cookies;
                    request.Headers.Add("Upgrade-Insecure-Requests", "1");
                    request.Headers.Add(HttpRequestHeader.)
                    */
                    //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";

                    Thread.Sleep(SLEEP_TIME);
                }
                catch (Exception e)
                {
                    Console.WriteLine("\nLicense Exception raised!");
                    Console.WriteLine("\nMessage:{0}", e.Message);
                    if (e.GetType() == typeof(WebException))
                    {
                        var webE = (WebException)e;
                        Console.WriteLine("\nStatus:{0}", webE.Status);
                    }
                }
            }
        }
    }

    class User
    {
        public KeyValuePair<string, string> SessUserID { get; set; }
        public KeyValuePair<string, string> SessID { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string branch { get; set; }
        public int minsAbsent { get; set; }
        public KeyValuePair<string, string> LogOut { get; set; }
        private CultureInfo inf = new CultureInfo("en-US");
        private readonly string[] highPriority = new string[] 
        {
            "Sam Smith",
            "Janice Robinson",
            "Janice Cornish",
            "Jeffrey Allen"
        };

        public User(Match user)
        {
            string[] vals = user.Groups.Cast<Group>().Select(g => g.Value).ToArray();
            this.SessUserID = new KeyValuePair<string, string>(vals[1], vals[2]);
            this.SessID = new KeyValuePair<string, string>(vals[3], vals[4]);
            this.username = vals[5];
            this.name = vals[6];
            this.branch = vals[7];
            this.minsAbsent = Convert.ToInt32((DateTime.Now - DateTime.ParseExact(vals[8], "g", inf)).TotalMinutes) * Convert.ToInt32(vals[9]);
            string LogOutQ = "0";
            if (this.minsAbsent > 30 && !highPriority.Contains(this.name))
                LogOutQ = "1";
            else if (this.minsAbsent > 5 && this.username == "ZAC.JOHNSO")
                LogOutQ = "1";

            this.LogOut = new KeyValuePair<string, string>(vals[10], LogOutQ);
        }
    }
}
