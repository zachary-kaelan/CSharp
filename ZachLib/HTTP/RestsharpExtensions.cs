using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RestSharp;
using ZachLib.Logging;

namespace ZachLib.HTTP
{
    public static class RestsharpExtensions
    {
        public static Parameter ToParameter(this HtmlNode node) => new Parameter()
        {
            Name = node.GetAttributeValue("name", node.Name),
            Value = (node.Name == "select" ? 
                node.SelectSingleNode(".//option[@selected] | .//option[1]") :
                node).GetAttributeValue("value", ""),
            Type = ParameterType.GetOrPost
        };

        public static string JoinHttpParameters(this IEnumerable<KeyValuePair<string, string>> dict, string title)
        {
            return " --:-- " + title + " --:--\r\n\t" + String.Join(
                "\r\n\t",
                dict.Select(kv => kv.Key + ": " + kv.Value)
            );
        }

        public static LoggableRestResponse Simplify(this IRestResponse response)
        {
            return new LoggableRestResponse()
            {
                Content = response.Content.Trim(),
                ContentEncoding = response.ContentEncoding,
                ContentLength = response.ContentLength,
                ContentType = response.ContentType,
                Cookies = response.Cookies.ToDictionary(
                    c => c.Path,
                    c => c.Value
                ),
                ErrorException = response.ErrorException,
                ErrorMessage = response.ErrorMessage,
                Headers = response.Headers.ToDictionary(
                    h => h.Name,
                    h => h.Value.ToString()
                ),
                Request = response.Request.Simplify(),
                ResponseStatus = response.ResponseStatus.ToString(),
                ResponseUri = response.ResponseUri.ToString(),
                StatusCode = response.StatusCode.ToString(),
                StatusDescription = response.StatusDescription.ToString()
            };
        }

        public static LoggableRestRequest Simplify(this IRestRequest request)
        {
            var index = request.Parameters.FindIndex(
                p => p.Name.Contains("Length") && p.Value != null
            );
            bool temp = index == -1 ? false : 
                (
                    (
                        Int32.TryParse(
                            request.Parameters[index].Value.ToString(), 
                            out int i
                        ) ? i : 0
                    ) > 7500
                );

            Func<Parameter, bool> parameterSelector = null;
            if (temp)
                parameterSelector = p => p.Type != ParameterType.UrlSegment && p.Type != ParameterType.RequestBody;
            else
                parameterSelector = p => p.Type != ParameterType.UrlSegment;

            var parametersTemp = request.Parameters.Where(
                parameterSelector
            ).GroupBy(
                p => p.Type,
                p => p,
                (k, g) => new KeyValuePair<string, Dictionary<string, string>>(
                    k.ToString(), 
                    g.ToDictionary(
                        p => p.Name,
                        p => p.Value.ToString()
                    )
                )
            ).ToDictionary();

            var loggableRequest = new LoggableRestRequest()
            {
                Files = request.Files.ToDictionary(
                    f => f.FileName, 
                    f => f.ContentLength
                ),
                Method = request.Method,
                Resource = request.Resource.Contains('&') ?
                        request.Resource.Substring(0, request.Resource.IndexOf('&')) :
                        request.Resource,
                NumAttempts = request.Attempts
            };

            loggableRequest.Initialize();
            bool hasGetOrPost = parametersTemp.TryGetValue("GetOrPost", out Dictionary<string, string> getOrPost);
            if (parametersTemp.TryGetValue("Cookie", out Dictionary<string, string> cookies))
                loggableRequest.Cookies = cookies;
            if (parametersTemp.TryGetValue("QueryString", out Dictionary<string, string> queries))
                loggableRequest.Parameters = queries.Concat(
                    request.Method == Method.GET && hasGetOrPost ?
                        getOrPost : Enumerable.Empty<KeyValuePair<string, string>>()
                ).ToDictionary();
            if (request.Method == Method.GET)
                loggableRequest.RequestBody = new Dictionary<string, string>();
            else if (hasGetOrPost)
                loggableRequest.RequestBody = getOrPost;
            if (parametersTemp.TryGetValue("Header", out Dictionary<string, string> headers))
                loggableRequest.Headers = headers;
            return loggableRequest;
        }
    }

    public class LoggableRestRequest : ILoggable
    {
        public Dictionary<string, string> Cookies { get; set; }
        public Dictionary<string, long> Files { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public Dictionary<string, string> RequestBody { get; set; }

        public Method Method { get; set; }
        public string Resource { get; set; }
        public int NumAttempts { get; set; }

        public void Initialize()
        {
            Cookies = new Dictionary<string, string>();
            Files = new Dictionary<string, long>();
            Headers = new Dictionary<string, string>();
            Parameters = new Dictionary<string, string>();
            RequestBody = new Dictionary<string, string>();
        }

        //private const string LOGENTRY_FORMAT = "{0} {1}, {2} Cookies, ";
        public object[] ToLogEntryString()
        {
            return new string[]
            {
                ToString(),
                Cookies.Count.ToString() + " Cookies, Content-Length: " + (
                    Files.Values.Sum() + RequestBody.Values.Sum(v => v.Length)
                ).ToString()
            };
        }

        public object[] ToFileString()
        {
            return new string[]
            {
                ToString(),
                Files.ToDictionary(
                    f => f.Key, 
                    f => f.Value.ToString()
                ).JoinHttpParameters("FILES"),
                Parameters.JoinHttpParameters("PARAMETERS"),
                RequestBody.JoinHttpParameters("BODY"),
                Cookies.JoinHttpParameters("COOKIES"),
                Headers.JoinHttpParameters("HEADERS")
            };
        }

        public override string ToString()
        {
            return Method.ToString() + " " + Resource + ", " + NumAttempts.ToString() + " Attempts";
        }

        public void Dispose()
        {
            Cookies.Clear();
            Files.Clear();
            Parameters.Clear();
            RequestBody.Clear();
            Headers.Clear();
            Resource = null;
        }
    }

    public class LoggableRestResponse : ILoggable
    {
        public string Content { get; set; }
        public string ContentEncoding { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
        public string ErrorMessage { get; set; }

        public string ResponseStatus { get; set; }
        public string ResponseUri { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public Dictionary<string, string> Cookies { get; set; }
        public Dictionary<string, string> Headers { get; set; }

        public bool HasError { get => ErrorException != null && !String.IsNullOrWhiteSpace(ErrorException.Message); }
        public Exception ErrorException { get; set; }
        public LoggableRestRequest Request { get; set; }

        public object[] ToLogEntryString()
        {
            if (HasError)
                return new string[]
                {
                    ErrorException.HResult.ToString() + " @ " + ErrorException.Source,
                    ErrorException.ToErrString(5)
                };

            return new string[]
            {
                ToString(),
                StatusCode + ": " + StatusDescription,
                ToStatusString(), 
                "REQUEST: " + Request.ToString()
            };
        }

        private const string INFO_FORMAT = "{0}\r\n{1} - {2}\r\n{3}\r\n{4}";
        private const string REQUEST_INFO_HEADER =
            "\t----------------------" +
            "\t---- REQUEST INFO ----" +
            "\t----------------------";
        public object[] ToFileString()
        {
            var requestContent = Request.ToFileString();
            if (ErrorException != null && !String.IsNullOrWhiteSpace(ErrorException.Message))
                return new object[]
                {
                    requestContent.First(),
                    ErrorException
                }.Concat(requestContent.Skip(1)).ToArray();

            return new object[]
            {
                String.Format(
                    INFO_FORMAT,
                    requestContent.First(),
                    StatusCode.ToString(),
                    StatusDescription,
                    ToString(),
                    ToStatusString()
                ),
                " --:-- CONTENT --:-- \r\n\r\n" + (ContentLength <= 24 && ContentLength >= 5 && !Content.Contains('\n') ? "Content Displayed at Top" : Content),
                Cookies.JoinHttpParameters("COOKIES"),
                Headers.JoinHttpParameters("HEADERS"),
                REQUEST_INFO_HEADER + "\r\n" + String.Join("\r\n", requestContent.Skip(1))
            };
        }

        public override string ToString()
        {
            return ContentLength <= 24 &&
                ContentLength >= 5 &&
                !Content.Contains('\n') ?
                    "Content: " + Content : (
                        (
                            String.IsNullOrWhiteSpace(ContentEncoding) ? "" : ContentEncoding + " "
                        ) + ContentType + ", " + ContentLength.ToString() + " bytes"
                    );
        }

        public string ToStatusString()
        {
            return "Response Status: " + ResponseStatus + " ~ " + (String.IsNullOrWhiteSpace(ErrorMessage) ? Cookies.Count + " Cookies" : ErrorMessage);
        }

        public void Dispose()
        {
            Cookies.Clear();
            Cookies = null;
            Headers.Clear();
            Headers = null;
            ErrorException = null;
            Request.Dispose();

            Content = null;
            ContentEncoding = null;
            ContentType = null;
            ResponseStatus = null;
            ResponseUri = null;
            StatusCode = null;
            StatusDescription = null;
        }
    }


}
