using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using System.Globalization;

namespace PPAPIStreamLined
{
    class Postman
    {
        public string accessToken { get; set; }
        public const string compkey = "323480";
        public Dictionary<string, string> abbrvs { get; set; }
        public Dictionary<string, string> abbrvs2 { get; set; }
        public const string find = "Billing Information";
        public const string find2 = "SERVICE AGREEMENT TERMS";
        public CompareInfo compInf = CultureInfo.CurrentCulture.CompareInfo;
        public CompareOptions options = CompareOptions.IgnoreSymbols | CompareOptions.IgnoreCase;


        public Postman()
        {
            this.GetToken();
        }

        public Dictionary<string, string> ExtractPDF(string filePath)
        {
            string fileName = new FileInfo(filePath).Name;
            string text;
            if (compInf.IndexOf(fileName, "SERVICE", options) != -1 || compInf.IndexOf(fileName, "INPC", options) != -1 || compInf.IndexOf(fileName, "SA-", options) != -1 || compInf.IndexOf(fileName, ".txt", options) != -1)
            {
                text = fileName.Substring(fileName.IndexOf('-') + 2).Trim();
                text = text.Substring(0, text.Length - 4).Trim();
                int lastSpace = text.LastIndexOf(' ');
                if (text[lastSpace + 1] == '-')
                    text = text.Remove(lastSpace);
            }
            else
            {
                string find = "Billing Information";
                string find2 = "SERVICE AGREEMENT TERMS";
                PdfReader r = new PdfReader(filePath);
                text = PdfTextExtractor.GetTextFromPage(r, 2);
                if (text.Substring(0, 11) == "APPOINTMENT")
                {
                    text = PdfTextExtractor.GetTextFromPage(r, 1);
                    string findTemp = "CUSTOMER & SERVICE LOCATION CUSTOMER & BILLING ADDRESS";
                    string find2Temp = "COVERED PESTS";
                    int where = text.IndexOf(findTemp) + 54;
                    List<string> texts = new List<string>(text.Substring(where, text.IndexOf(find2Temp) - where).Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));
                    texts = texts.Select(t => t.Substring(0, t.Length / 2).Trim()).ToList();
                    text = texts[0] + " - " + texts[3];
                }
                else
                {   
                    int where = text.IndexOf(find) + 19;
                    string[] texts = text.Substring(where, text.IndexOf(find2) - where).Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                    text = texts[0].Substring(0, texts[0].Length / 2).Trim().ToUpper();
                    if (texts.Length > 1 && texts[1].Length >= 8 && texts[1].Length <= 14)
                        text += " " + texts[1].Substring(0, texts[1].Length / 2).Trim();
                }
                r.Close();
                r.Dispose();

            }

            List<string> temp = new List<string>(text.Split(new char[] { ',', '-' }, StringSplitOptions.None));
            temp.AddRange(nullCheck(temp[3]).Split(' '));
            temp.RemoveAt(3);

            string nullCheck(string str = "")
            {
                try
                {
                    if (str == null || str == " ")
                        return "";
                    else
                        return str.Trim();
                }
                catch
                {
                    return "";
                }
            }

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (temp[i] == null)
                        temp.Insert(i, "");
                }
                catch
                {
                    temp.Insert(i, "");
                }
            }

            Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"Address", nullCheck(temp[1])},
                {"City", nullCheck(temp[2])},
                {"State", nullCheck(temp[3])},
                {"Zip", nullCheck(temp[4])}
            };

            if (Int32.TryParse(dict["State"], out _))
            {
                dict["Zip"] = dict["State"];
                dict["State"] = String.Empty;
            }

            temp = new List<string>(temp[0].Replace("/", " & ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            if (temp.Count == 2)
            {
                dict.Add("FirstName", temp[0]);
                dict.Add("LastName", temp[1]);
            }
            else if (temp.Count == 1)
            {
                dict.Add("LastName", temp[0]);
                dict.Add("FirstName", temp[0]);
            }
            else
            {
                string[] ands = new string[] { "AND", "&" };
                int and = temp.FindIndex(n => ands.Contains(n));
                if (and != -1)
                {
                    dict.Add("FirstName", nullCheck(temp[and - 1]));
                    dict.Add("SpouseName", nullCheck(temp[and + 1]));
                    dict.Add("LastName", nullCheck(temp[temp.Count - 1]));
                }
                else
                {
                    dict.Add("FirstName", nullCheck(temp[0]));
                    dict.Add("LastName", nullCheck(temp[temp.Count - 1]));
                }
            }
            return dict;
        }

        public void GetToken()
        {
            var client = new RestClient("https://is.workwave.com/oauth2/token?scope=openid");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("authorization", "Bearer N2JWMU9wRjFmT1FDSVRNam1fWmpsNjJkcFFZYTpjdXJueTNXb3g0ZUdpREdKTWhWdUI3OVhSSVlh");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&username=pestpacapi%40insightpest.com&password=!Pest6547!", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            this.accessToken = JsonConvert.DeserializeObject<dynamic>(response.Content).access_token;
            //this.accessToken = JsonConvert.DeserializeObject<dynamic>(client.Execute(request).Content).access_token;
        }

        public List<Dictionary<string, string>> GetLocID(Dictionary<string, string> query)
        {
            var client = new RestClient("https://api.workwave.com/pestpac/v1/locations?q=" + query["LastName"]);
            var request = new RestRequest(Method.GET);
            request.AddHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            request.AddHeader("tenant-id", compkey);
            request.AddHeader("authorization", "Bearer " + this.accessToken);
            
            List<Dictionary<string,string>> res;
            IRestResponse response;
            try
            {
                //IRestResponse response = client.Execute(request);
                //res = JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(response.Content);
                response = client.Execute(request);
                res = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(response.Content);
            }
            catch
            {
                if (query["LastName"].Any(c => !char.IsLetter(c)))
                    query["LastName"] = query["FirstName"];
                else
                    this.GetToken();
                return this.GetLocID(query);
            }

            client = null;
            request = null;
            

            string[] keys = query.Select(q => q.Key).ToArray();

            /*
            foreach(string key in keys)
            {
                query[key] = query[key].ToUpper().Trim();
            }
            res.RemoveAll(c => c["LastName"].ToUpper() != query["LastName"]);
            */
            res.RemoveAll(c => compInf.Compare(c["LastName"], query["LastName"], options) != 0 && compInf.Compare(c["FirstName"], query["LastName"], options) != 0);

            if (res.Count == 1)
                return res;

            foreach(Dictionary<string, string> r in res)
            {
                r["Zip"] = r["Zip"].Split('-')[0];
            }

            List<Dictionary<string, string>> results;
            List<List<Dictionary<string, string>>> resultsList = new List<List<Dictionary<string, string>>>();
            List<string> keyOrder = new List<string> { "FirstName", "LastName", "State", "City", "Zip", "Address" };
            for (int i = 0; i < keyOrder.Count; i++)
            {
                if (query[keyOrder[i]] == "")
                {
                    keyOrder.RemoveAt(i);
                    --i;
                }
            }

            double count = Math.Pow(2, keyOrder.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                results = res;
                List<string> searchString = new List<string>();
                string str = Convert.ToString(i, 2).PadLeft(keyOrder.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        //results = results.FindAll(c => c[keyOrder[j]].ToUpper().Trim() == query[keyOrder[j]]);
                        results = results.FindAll(c => compInf.Compare(c[keyOrder[j]], query[keyOrder[j]], options) == 0);
                        searchString.Add(keyOrder[j]);
                    }

                    if (results.Count == 0)
                        break;
                }

                if (results.Count == 0)
                {
                    if (searchString.Count == 1)
                    {
                        keyOrder.Remove(searchString[0]);
                        count /= 2;
                        i -= ((Convert.ToInt32(count) % i) + 1);
                    }
                }
                else if (results.Count == 1)
                    return results;
                else if (results.Count <= 10 && results.Count >= 2 && results.All(r => compInf.Compare(results[0]["Email"], r["Email"], options) == 0 || compInf.Compare(results[0]["Phone"], r["Phone"], options) == 0 || /*compInf.Compare(results[0]["Zip"], r["Zip"], options) == 0 || */compInf.Compare(results[0]["Address"], r["Address"], options) == 0))
                    throw new DuplicateWaitObjectException();
                else if (results.Count < 25)
                {
                    query["SearchString"] = String.Join(", ", searchString);
                    results.Insert(0, query);
                    resultsList.Add(results);
                }
                else
                    continue;
            }

            if (query.TryGetValue("SpouseName", out _))
            {
                if (compInf.Compare(query["SpouseName"], query["FirstName"], options) != 0 && query["SpouseName"] != "")
                {
                    string bak = query["FirstName"];
                    query["FirstName"] = query["SpouseName"];
                    query["SpouseName"] = "";
                    List<Dictionary<string, string>> results2 = this.GetLocID(query);
                    if (results2 != null)
                    {
                        results2.RemoveAt(0);
                        if (results2.Count == 1)
                            return results2;
                        else if (results2.Count <= 10 && results2.Count >= 2 && results2.All(r => compInf.Compare(results2[0]["Email"], r["Email"], options) == 0 || compInf.Compare(results2[0]["Phone"], r["Phone"], options) == 0 || /*compInf.Compare(results2[0]["Zip"], r["Zip"], options) == 0 || */compInf.Compare(results2[0]["Address"], r["Address"], options) == 0))
                            throw new DuplicateWaitObjectException();
                    }
                    query["FirstName"] = bak;
                }
                else
                    query["SpouseName"] = "";
            }

            string[] temp = query["Address"].Split(' ');
            List<KeyValuePair<string, string>> exts = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < temp.Length; ++i)
            {
                string BigExt;
                string BigExt2;
                if (this.abbrvs.TryGetValue(temp[i].ToUpper(), out BigExt))
                    exts.Add(new KeyValuePair<string, string>(temp[i], BigExt));
                else if (this.abbrvs2.TryGetValue(temp[i].ToUpper(), out BigExt2))
                    exts.Add(new KeyValuePair<string, string>(temp[i], BigExt2));
            }

            count = Math.Pow(2, exts.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                string vaar = query["Address"];
                string str = Convert.ToString(i, 2).PadLeft(exts.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                        vaar.Replace(exts[j].Key, exts[j].Value);
                }
                results = res.FindAll(c => compInf.Compare(c["Address"], vaar, options) == 0);
                if (results.Count == 1)
                    return results;
                else if (results.Count <= 10 && results.Count >= 2 && results.All(r => compInf.Compare(results[0]["Email"], r["Email"], options) == 0 || compInf.Compare(results[0]["Phone"], r["Phone"], options) == 0 || /*compInf.Compare(results[0]["Zip"], r["Zip"], options) == 0 || */compInf.Compare(results[0]["Address"], r["Address"], options) == 0))
                    throw new DuplicateWaitObjectException();
            }

            if (resultsList.Count == 0)
            {
                if (res.Count == 0)
                    return null;
                res[0].Add("Missing", "True");
                return res;
            }
            return resultsList.OrderBy(lcs => lcs.Count).ToList()[0];
        }

        public string SetDocRecord(Dictionary<string, string> cust, string name = "SA")
        {
            if (this.DocExists(cust["LocationID"], name))
                return "Doc Exists";
            var client = new RestClient("https://api.workwave.com/pestpac/v1/documents?type=LocationDocument");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            request.AddHeader("tenant-id", compkey);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer " + this.accessToken);

            LocationDocument locDoc = new LocationDocument(cust["LocationID"], name);

            request.AddParameter("application/json", JsonConvert.SerializeObject(locDoc), ParameterType.RequestBody);

            try
            {
                return JsonConvert.DeserializeObject<dynamic>(client.Execute(request).Content).DocumentID;
            }
            catch
            {
                this.GetToken();
                return this.SetDocRecord(cust, name);
            }
        }

        public string UploadDoc(string DocID, string filePath)
        {
            var client = new RestClient("https://api.workwave.com/pestpac/v1/documents/" + DocID + "/upload?type=LocationDocument");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            request.AddHeader("tenant-id", compkey);
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            request.AddHeader("authorization", "Bearer " + this.accessToken);
            request.AddParameter("multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW", "------WebKitFormBoundary7MA4YWxkTrZu0gW\r\nContent-Disposition: form-data; name=\"file\"; filename=\"" + new FileInfo(filePath).Name + "\"\r\nContent-Type: application/pdf\r\n\r\n\r\n------WebKitFormBoundary7MA4YWxkTrZu0gW--", ParameterType.RequestBody);
            request.AddFile("file", File.ReadAllBytes(filePath), new FileInfo(filePath).Name, "multipart/form-data");

            try
            {
                return client.Execute(request).Content;
            }
            catch
            {
                this.GetToken();
                return this.UploadDoc(DocID, filePath);
            }
        }

        public bool DocExists(string locID, string name = "SA")
        {
            var client = new RestClient("https://api.workwave.com/pestpac/v1/Locations/" + locID + @"/documents");
            var request = new RestRequest(Method.GET);
            request.AddHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            request.AddHeader("tenant-id", compkey);
            request.AddHeader("authorization", "Bearer " + this.accessToken);
            request.AddHeader("Accept", "text/plain, application/json");

            List<Document> docs;
            try
            {
                docs = JsonConvert.DeserializeObject<List<Document>>(client.Execute(request).Content);
            }
            catch
            {
                this.GetToken();
                return this.DocExists(locID, name);
            }
            foreach (Document doc in docs)
            {
                doc.Name = doc.Name.ToUpper();
            }
            if (name == "SA")
                return docs.Any(d => d.Name.Contains("SA") || (d.Name.Contains("SERVICE") && d.Name.Contains("AGREEMENT")));
            else
                return docs.Any(d => d.Name.Contains("INPC") || d.Name.Contains("INVOICE"));
        }

        public string UploadNote(string locID, string note)
        {
            var client = new RestClient("https://api.workwave.com/pestpac/v1/Locations/" + locID + @"/notes");
            var request = new RestRequest(Method.POST);
            request.AddHeader("apikey", "Ac1jfgugSAmy6mpj1AGnYzrAdV9HfLPc");
            request.AddHeader("tenant-id", compkey);
            request.AddHeader("authorization", "Bearer " + this.accessToken);
            request.AddHeader("Accept", "text/plain, application/json, text/json");
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", JsonConvert.SerializeObject(new NoteModel(locID, note)), ParameterType.RequestBody);

            try
            {
                return client.Execute(request).Content;
            }
            catch
            {
                this.GetToken();
                return this.UploadNote(locID, note);
            }
        }
    }
}
