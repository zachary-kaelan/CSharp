using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Jil;
using PestPac.Model;
using PostmanLib.Properties;
using PPLib;
using RestSharp;
using RestSharp.Extensions;
using ZachLib;
using ZachLib.EventsAndExceptions;
using ZachLib.HTTP;
using ZachLib.Logging;

namespace PostmanLib
{
    public enum NoteCode
    {
        GEN,
        FEEDBACK
    };

    public enum EntityType
    {
        BillTos,
        Locations,
        ServiceOrders
    }

    public enum ListVisibility
    {
        Private,
        Public,
        PublicToBranch
    }

    public enum EmployeeType
    {
        All,
        User,
        Technician
    }

    public enum ActiveStatus
    {
        All,
        Yes,
        No
    }

    public enum OrderType
    {
        ServiceOrder,
        Production,
        CallBack,
        Estimate,
        CreditMemo,
        NotServiced,
        Other,
        None
    }

    public static class Postman
    {
        private static SortedDictionary<string, int> BranchIDs;
        public const string LOG_NAME = "Postman";

        #region Setup
        public static bool IsInitialized { get; private set; }

        private static RestClient client { get; set; }
        private static RestClient tokenClient { get; set; }
        private static IRestRequest tokenPasswordRequest = new RestRequest(Method.POST).AddOrUpdateParameters(
            new Parameter("grant_type", "password", ParameterType.GetOrPost),
            new Parameter("username", "pestpacapi@insightpest.com", ParameterType.GetOrPost),
            new Parameter("password", "!Pest6547!", ParameterType.GetOrPost)
        );
        private static IRestRequest tokenRefreshRequest = new RestRequest(Method.POST).AddOrUpdateParameters(
            new Parameter("grant_type", "refresh_token", ParameterType.GetOrPost),
            new Parameter("refresh_token", Settings.Default.refresh_token, ParameterType.GetOrPost)
        );

        static Postman()
        {
            LogManager.AddLog(
                LOG_NAME,
                LogType.FolderFilesByDate
            );

            tokenClient = new RestClient("https://is.workwave.com/oauth2/token?scope=openid");
            tokenClient.AddDefaultHeader("Authorization", "Bearer N2JWMU9wRjFmT1FDSVRNam1fWmpsNjJkcFFZYTpjdXJueTNXb3g0ZUdpREdKTWhWdUI3OVhSSVlh");
            tokenClient.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
            /*tokenClient.AddDefaultParameter("grant_type", "password", ParameterType.RequestBody);
            tokenClient.AddDefaultParameter("username", "pestpacapi@insightpest.com", ParameterType.RequestBody);
            tokenClient.AddDefaultParameter("password", "!Pest6547!", ParameterType.RequestBody);*/

            client = new RestClient("https://api.workwave.com/pestpac/v1/");
            client.AddDefaultHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            client.AddDefaultHeader("tenant-id", "323480");
            client.AddDefaultHeader("Authorization", "");

            HTTPUtils.WaitForInternet();
            DateTime now = DateTime.Now;

            if (String.IsNullOrWhiteSpace(Settings.Default.access_token) || DateTime.Compare(Settings.Default.expires_in, now) <= 0)
                GetToken(now);
            else
            {
                client.RemoveDefaultParameter("Authorization");
                client.AddDefaultHeader("Authorization", "Bearer " + Settings.Default.access_token);
            }

            if (!File.Exists("BranchIDs.txt"))
            {
                if (TryGetBranches(out IDictionary<string, int> branchIDs))
                {
                    BranchIDs = new SortedDictionary<string, int>(branchIDs);
                    BranchIDs.SaveDictAs("BranchIDs.txt");
                }
                else
                {
                    LogManager.Enqueue(
                        LOG_NAME,
                        EntryType.ERROR,
                        "Initialization",
                        "Branches update failed"
                    );
                }
            }
            else
                BranchIDs = new SortedDictionary<string, int>(Utils.LoadIntDictionary("BranchIDs.txt"));

            IsInitialized = true;
            client.Proxy = null;
        }

        public static void GetToken(DateTime now)
        {
            TokenResponse tokens = new TokenResponse();
            try
            {
                var response = tokenClient.Execute(tokenRefreshRequest);
                if (!response.StatusCode.IsOK())
                    throw response.ErrorException;
                tokens = JSON.Deserialize<TokenResponse>(response.Content);
                LogManager.Enqueue(
                    new LogUpdate(
                        "Postman",
                        EntryType.DEBUG,
                        "Getting new token",
                        "Type: Refresh"
                    )
                );
            }
            catch
            {
                tokens = JSON.Deserialize<TokenResponse>(
                    tokenClient.Execute(tokenPasswordRequest).Content
                );

                Settings.Default.refresh_token = tokens.refresh_token;
                LogManager.Enqueue(
                    new LogUpdate(
                        "Postman",
                        EntryType.DEBUG,
                        "Getting new token",
                        "Type: Password"
                    )
                );
            }

            Settings.Default.access_token = tokens.access_token;
            Settings.Default.expires_in = now.AddSeconds(tokens.expires_in - 5);
            Settings.Default.Save();

            client.RemoveDefaultParameter("Authorization");
            client.AddDefaultHeader("Authorization", "Bearer " + tokens.access_token);
        }

        private static void GetToken()
        {
            GetToken(DateTime.Now);
        }

        private struct TokenResponse
        {
            public string scope { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public string id_token { get; set; }
            public string access_token { get; set; }
        }

        private static bool TryGetBranches(out IDictionary<string, int> branchIDs)
        {
            IRestResponse response = null;
            try
            {
                response = client.Execute(new RestRequest("lookups/Branches"));
                var branches = JSON.Deserialize<BranchListModel[]>(response.Content);
                branchIDs = branches.ToDictionary(b => b.Name, b => b.BranchID.Value);
                LogManager.Enqueue(
                    LOG_NAME,
                    EntryType.DEBUG,
                    "Branches successfully updated",
                    (
                        BranchIDs != null && BranchIDs.Any() ?
                            branchIDs.Count - BranchIDs.Count :
                            branchIDs.Count
                    ).ToString() + " added"
                );
                return true;
            }
            catch (Exception e)
            {
                LogManager.Enqueue(
                    LOG_NAME,
                    "BranchIDsResponse",
                    response,
                    e
                );
                branchIDs = null;
                return false;
            }
        }

        public static bool TryGetBranch(string branch, out int branchID)
        {
            if (BranchIDs != null && BranchIDs.TryGetValue(branch, out branchID))
                return true;
            else if (TryGetBranches(out IDictionary<string, int> branchIDs))
            {
                if (branchIDs.TryGetValue(branch, out branchID))
                    return true;
                else
                {
                    LogManager.Enqueue(
                        LOG_NAME,
                        EntryType.ERROR,
                        "Invalid branch name",
                        "\"" + branch + "\" could not be found in lookup"
                    );
                    throw new InvalidBranchException(branch);
                }
            }
            else
            {
                LogManager.Enqueue(
                    LOG_NAME,
                    EntryType.ERROR,
                    "Employee Access",
                    "Branches update failed"
                );
                branchID = -1;
                return false;
            }
        }
        #endregion

        #region Employees
        public static EmployeeListModel GetEmployee(string username) =>
            client.TryExecute<EmployeeListModel[]>(
                new RestRequest(
                    "lookups/Employees"
                ).AddOrUpdateParameter(
                    "username",
                    username,
                    ParameterType.GetOrPost
                ),
                LOG_NAME,
                GetToken,
                out EmployeeListModel[] content
            ).IsOK() ? content.Single() : null;

        public static EmployeeListModel[] GetEmployees() =>
            client.TryExecute<EmployeeListModel[]>(
                new RestRequest(
                    "lookups/Employees"
                ),
                LOG_NAME,
                GetToken,
                out EmployeeListModel[] content
            ).IsOK() ? content : Array.Empty<EmployeeListModel>();

        public static EmployeeListModel[] GetEmployees(bool active, bool isTech, string username) =>
            client.TryExecute<EmployeeListModel[]>(
                new RestRequest(
                    "lookups/Employees"
                ).AddOrUpdateParameters(
                    new Dictionary<string, object>()
                    {
                        { "active", active },
                        { "type", isTech ? "technician" : "user" },
                        { "username", username }
                    }
                ), 
                LOG_NAME,
                GetToken,
                out EmployeeListModel[] content
            ).IsOK() ? content : Array.Empty<EmployeeListModel>();

        public static EmployeeListModel[] GetEmployees(ActiveStatus active, EmployeeType type, string username) =>
            client.TryExecute<EmployeeListModel[]>(
                new RestRequest(
                    "lookups/Employees"
                ).AddOrUpdateParameters(
                    new Dictionary<string, object>()
                    {
                        { "active", active == ActiveStatus.All ? "" : (active == ActiveStatus.Yes ? "true" : "false") },
                        { "type", type == EmployeeType.All ? "" : type.ToString() },
                        { "username", username }
                    }
                ),
                LOG_NAME,
                GetToken,
                out EmployeeListModel[] content
            ).IsOK() ? content : Array.Empty<EmployeeListModel>();

        public static EmployeeListModel[] GetEmployees(ActiveStatus active, EmployeeType type) =>
            client.TryExecute<EmployeeListModel[]>(
                new RestRequest(
                    "lookups/Employees"
                ).AddOrUpdateParameters(
                    new Dictionary<string, object>()
                    {
                        { "active", active == ActiveStatus.All ? "" : (active == ActiveStatus.Yes ? "true" : "false") },
                        { "type", type == EmployeeType.All ? "" : type.ToString() }
                    }
                ),
                LOG_NAME,
                GetToken,
                out EmployeeListModel[] content
            ).IsOK() ? content : Array.Empty<EmployeeListModel>();

        public static bool PatchEmployee(string id, params PatchOperation[] ops)
        {
            return client.TryExecute(
                new RestRequest(
                    "lookups/Employees/" + id,
                    Method.PATCH
                ).AddJsonBody(ops),
                LOG_NAME,
                GetToken
            ).IsOK();
        }

        public static bool UpdateEmployeeAccess(string id, string branch, string newAccess)
        {
            if (TryGetBranch(branch, out int branchID))
                return UpdateEmployeeAccess(id, branchID, newAccess);
            return false;
        }

        public static bool UpdateEmployeeAccess(string id, int branchID, string newAccess) =>
            PatchEmployee(id, new PatchOperation("replace", "/branchAccess/" + branchID.ToString(), newAccess));

        public static bool DeactivateEmployee(string id) =>
            PatchEmployee(
                id,
                new PatchOperation("replace", "/Active", false),
                new PatchOperation("replace", "/TerminationDate", DateTime.Now.ToString("s"))
            );
        #endregion

        #region GetLocation
        public static int GetLocationIDFromCode(int code)
        {
            return client.TryExecute(
                new RestRequest("locations/code/" + code.ToString()),
                LOG_NAME,
                GetToken,
                out LocationIDModel model
            ).IsOK() ?
                model.LocationID :
                -1;
            /*var model = ;
            return model != null && model.LocationID.HasValue ? model.LocationID.Value.ToString() : null;*/
        }

        public static string GetLocationIDFromCode(string code)
        {
            return client.TryExecute(
                new RestRequest("locations/code/" + code),
                LOG_NAME,
                GetToken,
                out LocationIDModel model
            ).IsOK() ?
                model.LocationID.ToString() :
                null;
            /*var model = ;
            return model != null && model.LocationID.HasValue ? model.LocationID.Value.ToString() : null;*/
        }

        public static LocationModel GetLocationFromCode(string code)
        {
            return client.TryExecute<LocationModel>(
                new RestRequest("locations/code/" + code, Method.GET),
                LOG_NAME,
                GetToken,
                out LocationModel model
            ).IsOK() ?
                model :
                null;
        }

        public static LocationModel GetLocation(string id)
        {
            return client.TryExecute<LocationModel>(
                new RestRequest("locations/" + id, Method.GET),
                LOG_NAME,
                GetToken,
                out LocationModel model
            ).IsOK() ?
                model :
                null;
        }
        #endregion

        #region Search
        public static bool Search(string query, out IEnumerable<LocationListModel> results)
        {
            return client.TryExecute<IEnumerable<LocationListModel>>(
                new RestRequest(
                    "locations", 
                    Method.GET
                ).AddOrUpdateParameter(
                    "q", query, ParameterType.QueryString
                ),
                LOG_NAME,
                GetToken, 
                out results
            ).IsOK();
        }
        #endregion

        #region UploadNote
        public static bool UploadNote(NoteModel note)
        {
            return client.TryExecute(
                new RestRequest(
                    "locations/" + 
                    note.Associations.LocationID + 
                    "/notes", Method.POST
                ).AddJsonBody(note),
                LOG_NAME,
                GetToken
            ).IsOK();
        }

        public static bool UploadNote(int locID, string note, string prefix = "VTNotes : ", string code = "GEN")
        {
            return UploadNote(
                new NoteModel(
                    NoteCode:code,
                    Note:note,
                    CreatedByUser:"ADMN",
                    Associations:new NoteAssociationModel(
                        LocationID:Convert.ToInt32(locID)
                    )
                )
            );
        }

        public static bool UploadNote(int locID, string note, string prefix = "VTNotes : ", NoteCode code = NoteCode.GEN)
        {
            return UploadNote(locID, note, prefix, code.ToString());
        }
        #endregion

        #region GetDocuments
        public static bool GetDocuments(int locID, out IEnumerable<DocumentListModel> docs)
        {
            if (client.TryExecute<IEnumerable<DocumentListModel>>(
                new RestRequest(
                    "locations/" + locID + "/documents",
                    Method.GET
                ),
                LOG_NAME,
                GetToken, 
                out IEnumerable<DocumentListModel> docsTemp
            ).IsOK()) {
                docs = docsTemp;
                return docs.Any();
            }
            docs = Enumerable.Empty<DocumentListModel>();
            return false;
        }

        public static bool GetDocuments(int locID, string name, out IEnumerable<DocumentListModel> docs)
        {
            if (GetDocuments(locID, out docs))
            {
                docs = docs.Where(
                    d => d.DocumentType == DocumentListModel.DocumentTypeEnum.Locationdocument &&
                        Utils.COMPARE_INFO.IsPrefix(d.Name, name, Utils.IGNORE_CASE_AND_SYMBOLS) ||
                        Utils.COMPARE_INFO.IsSuffix(d.Name, name, Utils.IGNORE_CASE_AND_SYMBOLS) ||
                        d.Name.Contains(" " + name + " ")
                );
                return docs.Any();
            }
            
            return false;    
        }

        public static bool GetDocuments(int locID, DateTime startingDate, out IEnumerable<DocumentListModel> docs)
        {
            return GetDocuments(locID, startingDate, null, out docs);
        }

        public static bool GetDocuments(int locID, DateTime startingDate, string name, out IEnumerable<DocumentListModel> docs)
        {
            if (String.IsNullOrWhiteSpace(name) ? GetDocuments(locID, out docs) : GetDocuments(locID, name, out docs))
            {
                docs = docs.Where(
                    d => d.Date.HasValue &&
                        d.Date.Value.CompareTo(startingDate) > 0
                );

                return docs.Any();
            }

            return false;
        }
        #endregion

        #region DocExists
        public static bool DocExists(int locID, DateTime comparisonDate, out IEnumerable<DocumentListModel> duplicates)
        {
            return DocExists(locID, comparisonDate, null, out duplicates);
        }

        public static bool DocExists(int locID, DateTime comparisonDate, string name, out IEnumerable<DocumentListModel> duplicates)
        {
            if (GetDocuments(locID, comparisonDate, name, out duplicates))
            {
                duplicates = duplicates.Where(
                    d => !Utils.COMPARE_INFO.IsPrefix(d.FileName, "temp", Utils.IGNORE_CASE_AND_SYMBOLS)
                );

                return duplicates.Any();
            }
            
            return false;
        }

        public static bool DocExists(int locID, out IEnumerable<DocumentListModel> duplicates)
        {
            return DocExists(locID, null, out duplicates);
        }

        public static bool DocExists(int locID, string name, out IEnumerable<DocumentListModel> duplicates)
        {
            return DocExists(locID, DateTime.Now, name, out duplicates);
        }
        #endregion

        #region SetDocRecord
        public static bool SetDocRecord(int locID, string name, out IEnumerable<DocumentListModel> duplicates)
        {
            if (!DocExists(locID, name, out duplicates))
            {   
                if (
                    !client.TryExecute(
                        new RestRequest(
                            "documents", 
                            Method.POST
                        ).AddJsonBody(
                            new DocumentModel(
                                DocumentModel.DocumentTypeEnum.Locationdocument,
                                Name: name,
                                Date: DateTime.Now,
                                Tags: "Postman",
                                FileName: "tempName.pdf",
                                OrderID: Convert.ToInt32(locID)
                            )
                        ),
                        LOG_NAME,
                        GetToken, 
                        out string content
                ).IsOK()) {
                    throw new Exception(content);
                }

                return true;
            }

            return false;
        }

        public static bool SetDocRecord(int locID, out IEnumerable<DocumentListModel> duplicates)
        {
            return SetDocRecord(locID, null, out duplicates);
        }

        public static bool SetDocRecord(Customer cust, out IEnumerable<DocumentListModel> duplicates)
        {
            return SetDocRecord(Convert.ToInt32(cust.LocationID), cust.Type.ToString(), out duplicates);
        }
        #endregion

        #region UploadDoc
        private const string CONTENT_TYPE_FORMAT = "multipart/form-data; boundary=----WebKitFormBoundary{0}";
        private const string FORM_DATA_FORMAT = 
            "------WebKitFormBoundary{0}\r\n" +
            "Content-Disposition: form-data; name=\"file\"; filename=\"{1}\"\r\n" +
            "Content-Type: application/pdf\r\n\r\n\r\n" +
            "------WebKitFormBoundary{0}--";
        public static bool UploadDoc(string docID, string path, out DocumentModel doc)
        {
            string boundaryCode = Utils.GetRandomString(15);
            string name = Path.GetFileName(path);

            var request = new RestRequest(
                "documents/" + docID + "/upload",
                Method.POST
            ){
                AlwaysMultipartFormData = true,
                Timeout = 15000
            }.AddOrUpdateParameter(
                String.Format(
                    CONTENT_TYPE_FORMAT,
                    boundaryCode
                ), String.Format(
                    FORM_DATA_FORMAT,
                    boundaryCode,
                    name
                ), ParameterType.RequestBody
            ).AddFile(
                "file", File.ReadAllBytes(path),
                name, "multipart/form-data"
            );
            boundaryCode = null;
            name = null;

            request.IncreaseNumAttempts();
            request.IncreaseNumAttempts();

            bool check = client.TryExecute<DocumentModel>(request, LOG_NAME, GetToken, out doc).IsOK();
            request = null;
            return check;
        }
        #endregion

        #region GetTaxCodes
        public static bool GetTaxCodes(out IEnumerable<TaxCode> taxcodes)
        {
            return GetTaxCodes(false, out taxcodes);
        }

        public static bool GetTaxCodes(bool active, out IEnumerable<TaxCode> taxcodes)
        {
            return client.TryExecute<IEnumerable<TaxCode>>(
                new RestRequest("lookups/TaxCodes", Method.GET).AddOrUpdateParameter("active", active, ParameterType.GetOrPost),
                LOG_NAME,
                GetToken,
                out taxcodes
            ).IsOK();
        }
        #endregion

        #region GetTaxCodesAutoFill
        public static bool GetTaxCodesAutoFill(out IEnumerable<TaxCode> taxcodes)
        {
            return client.TryExecute<IEnumerable<TaxCode>>(
                new RestRequest("lookups/TaxCodes/autoFill", Method.GET),
                LOG_NAME,
                GetToken,
                out taxcodes
            ).IsOK();
        }

        public static bool GetTaxCodesAutoFill(out IEnumerable<TaxCode> taxcodes, string state)
        {
            return client.TryExecute<IEnumerable<TaxCode>>(
                new RestRequest(
                    "lookups/TaxCodes/autoFill", 
                    Method.GET
                ).AddOrUpdateParameter(
                    "state",
                    state,
                    ParameterType.GetOrPost
                ),
                LOG_NAME,
                GetToken,
                out taxcodes
            ).IsOK();
        }

        public static bool GetTaxCodesAutoFill(out IEnumerable<TaxCode> taxcodes, string state, string county)
        {
            return client.TryExecute<IEnumerable<TaxCode>>(
                new RestRequest(
                    "lookups/TaxCodes/autoFill",
                    Method.GET
                ).AddOrUpdateParameter(
                    "state",
                    state,
                    ParameterType.GetOrPost
                ).AddOrUpdateParameter(
                    "county",
                    county,
                    ParameterType.GetOrPost
                ),
                LOG_NAME,
                GetToken,
                out taxcodes
            ).IsOK();
        }

        public static bool GetTaxCodesAutoFill(out IEnumerable<TaxCode> taxcodes, string state, string county, string city)
        {
            return client.TryExecute<IEnumerable<TaxCode>>(
                new RestRequest(
                    "lookups/TaxCodes/autoFill",
                    Method.GET
                ).AddOrUpdateParameter(
                    "state",
                    state,
                    ParameterType.GetOrPost
                ).AddOrUpdateParameter(
                    "county",
                    county,
                    ParameterType.GetOrPost
                ).AddOrUpdateParameter(
                    "city",
                    city,
                    ParameterType.GetOrPost
                ),
                LOG_NAME,
                GetToken,
                out taxcodes
            ).IsOK();
        }
        #endregion

        #region PaymentAccounts
        public static CardOnFileListModel[] GetPaymentAccounts(int billToId) =>
            client.TryExecute(
                new RestRequest(
                    "BillTos/" + billToId.ToString() + "/paymentAccounts",
                    Method.GET
                ),
                LOG_NAME,
                GetToken, 
                Options.ISO8601,
                out CardOnFileListModel[] cards
            ).IsOK() ? cards : Array.Empty<CardOnFileListModel>();

        public static PaymentProcessorTransactionModel Charge(int cardID, string amount, bool webPayment) =>
            client.TryExecute(
                new RestRequest(
                    "PaymentAccounts/" + cardID.ToString() + "/charge",
                    Method.POST
                ).AddOrUpdateParameters(
                    new Parameter()
                    {
                        Name = "amount",
                        Value = amount,
                        Type = ParameterType.QueryString
                    }, new Parameter()
                    {
                        Name = "origin",
                        Value = webPayment ? "WebPayment" : "Other",
                        Type = ParameterType.QueryString
                    }
                ),
                LOG_NAME,
                GetToken, 
                out PaymentProcessorTransactionModel model
            ).IsOK() ? model : new PaymentProcessorTransactionModel();
        #endregion

        #region Services
        public static ServiceOrderModel GetServiceOrder(string orderID)
        {
            return client.TryExecute<ServiceOrderModel>(
                new RestRequest(
                    "ServiceOrders/" + orderID,
                    Method.GET
                ),
                LOG_NAME,
                GetToken, 
                out ServiceOrderModel model
            ).IsOK() ? model : new ServiceOrderModel();
        }

        #region GetLocationServiceSetup
        public static ServiceSetupListModel[] GetLocationServiceSetups(int locID)
        {
            return client.TryExecute<ServiceSetupListModel[]>(
                new RestRequest(
                    "locations/" + locID + "/serviceSetups",
                    Method.GET
                ),
                LOG_NAME,
                GetToken, 
                out ServiceSetupListModel[] setups
            ).IsOK() ?
                setups :
                Array.Empty<ServiceSetupListModel>();
        }

        public static ServiceSetupListModel GetLocationLatestServiceSetup(int locID)
        {
            return GetLocationServiceSetup(locID, s => s.Active.HasValue && s.Active.Value);
        }

        public static ServiceSetupListModel GetLocationServiceSetup(int locID, Func<ServiceSetupListModel, bool> predicate)
        {
            var setups = GetLocationServiceSetups(locID);
            return setups.Any() ?
                setups.Single(predicate) :
                new ServiceSetupListModel();
        }

        public static ServiceSetupModel GetServiceSetup(string setupID)
        {
            return client.TryExecute<ServiceSetupModel>(
                new RestRequest(
                    "ServiceSetups/" + setupID,
                    Method.GET
                ),
                LOG_NAME,
                GetToken, 
                out ServiceSetupModel setup
            ).IsOK() ?
                setup : new ServiceSetupModel();
        }
        #endregion

        #region GetLatestServiceOrder
        public static bool GetLatestServiceOrder(int locID, out ServiceOrderListModel model)
        {
            var models = GetServiceOrders(locID);
            if (models.Any())
            {
                if (models.Length == 1)
                    model = models[0];
                else
                {
                    var ordered = models.OrderByDescending(o => o.WorkDate.HasValue).ThenByDescending(o => o.WorkDate.HasValue ? o.WorkDate.Value : DateTime.MaxValue);
                    model = ordered.First();
                }
                return true;
            }
            model = new ServiceOrderListModel();
            return false;
        }

        public static bool GetLatestServiceOrder(int locID, string setupID, out ServiceOrderListModel model)
        {
            var models = GetServiceOrders(locID, setupID);
            if (models.Any())
            {
                if (models.Length == 1)
                    model = models[0];
                else
                {
                    var ordered = models.OrderByDescending(o => o.WorkDate.HasValue).ThenByDescending(o => o.WorkDate.HasValue ? o.WorkDate.Value : DateTime.MaxValue);
                    model = ordered.First();
                }
                return true;
            }
            model = new ServiceOrderListModel();
            return false;
        }
        #endregion

        #region GetServiceOrders
        public static ServiceOrderListModel[] GetServiceOrders(int locationID)
        {
            return client.TryExecute<ServiceOrderListModel[]>(
                new RestRequest(
                    "Locations/" + locationID.ToString() + "/serviceOrders",
                    Method.GET
                ),
                LOG_NAME,
                GetToken,
                out ServiceOrderListModel[] models
            ).IsOK() ? models : new ServiceOrderListModel[0];
        }

        public static ServiceOrderListModel[] GetServiceOrders(int locationID, string setupID)
        {
            int temp = Convert.ToInt32(setupID);
            return GetServiceOrders(locationID).Where(o => o.SetupID.Value == temp).ToArray();
        }

        public static ServiceOrderListModel GetServiceOrderByNumber(int locID, int orderNumber)
        {
            string num = orderNumber.ToString();
            return GetServiceOrders(locID).First(o => o.OrderNumber == num);
        }
        #endregion

        public static HttpStatusCode PostServiceOrder(string orderID, string tech, int batchNumber) => client.TryExecute<ServiceOrderModel>(
            new RestRequest(
                "ServiceOrders/" + orderID + "/post",
                Method.POST
            ).AddJsonBody(
                new PPPostedServiceOrder()
                {
                    Posted = true,
                    BatchNumber = batchNumber,
                    TechnicianCode = tech
                }
            ),
            LOG_NAME,
            GetToken, 
            out _
        );
        #endregion

        #region Invoices
        public static InvoiceListModel[] GetServiceHistory(int locID)
        {
            return client.TryExecute(
                new RestRequest(
                    "Locations/" + locID + "/serviceHistory",
                    Method.GET
                ),
                LOG_NAME,
                GetToken,
                out InvoiceListModel[] models
            ).IsOK() ? models : new InvoiceListModel[] { };
        }

        public static string GetInvoiceIDFromNumber(int locID, string invoiceNumber)
        {
            return client.TryExecute(
                new RestRequest(
                    "Locations/" + locID + "/serviceHistory",
                    Method.GET
                ),
                LOG_NAME,
                GetToken,
                out InvoiceIDModel[] models
            ).IsOK() ?
                models.First(
                    i => i.InvoiceNumber == invoiceNumber &&
                         (i.InvoiceType == InvoiceListModel.InvoiceTypeEnum.Invoice ||
                          i.InvoiceType == InvoiceListModel.InvoiceTypeEnum.Callback)
                ).InvoiceID.ToString() : null;
        }

        public static void SaveInvoicePDF(string id, string path)
        {
            client.DownloadData(
                new RestRequest(
                    "Invoices/" + id + "/inspectionReport?format=DetailWithPrices&customForm=true"
                ), true
            ).SaveAs(path);
        }
        #endregion

        #region Patch
        public static bool Patch(int locID, params PatchOperation[] ops)
        {
            return client.TryExecute(
                new RestRequest(
                    "locations/" + locID,
                    Method.PATCH
                ).AddJsonBody(ops),
                LOG_NAME,
                GetToken
            ).IsOK();
        }

        public static bool Patch(EntityType type, string id, params PatchOperation[] ops)
        {
            return client.TryExecute(
                new RestRequest(
                    type.ToString() + "/" + id,
                    Method.PATCH
                ).AddJsonBody(ops),
                LOG_NAME,
                GetToken
            ).IsOK();
        }
        #endregion

        #region CreateList
        public static PPList CreateList(string name, ListVisibility visibility, bool isStatic = true) => client.TryExecute<PPList>(
            new RestRequest(
                "lists/locations",
                Method.POST
            ).AddJsonBody(
                new PPList(name, visibility, isStatic)
            ),
            LOG_NAME,
            GetToken, 
            out PPList content
        ).IsOK() ? content : new PPList();

        public static bool CreateList(string name, ListVisibility visibility, IEnumerable<int> locationIds, bool isStatic = true)
        {
            var list = CreateList(name, visibility, isStatic);
            if (list.Equals(new PPList()))
                return false;
            string resource = "lists/locations/" + list.ListId;
            foreach (var location in locationIds)
            {
                client.TryExecute(
                    new RestRequest(
                        resource,
                        Method.PUT
                    ).AddParameter(
                        new Parameter()
                        {
                            Name = "locationId",
                            Value = location,
                            Type = ParameterType.QueryString
                        }
                    ),
                    LOG_NAME,
                    GetToken
                );
            }
            return true;
        }
        #endregion

        public static ServiceModel[] GetServices() =>
            client.TryExecute(
                new RestRequest(
                    "lookups/Services",
                    Method.GET
                ).AddParameter("active", true, ParameterType.QueryString),
                LOG_NAME,
                GetToken,
                out ServiceModel[] services
            ).IsOK() ? services : null;

        public static SortedDictionary<string, string> GetServiceDescriptions() =>
            new SortedDictionary<string, string>(
                GetServices().ToDictionary(
                    s => s.Code,
                    s => s.Description
                )
            );
    }
}
