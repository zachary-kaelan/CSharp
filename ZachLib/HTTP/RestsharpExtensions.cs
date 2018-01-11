using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace ZachLib.HTTP
{
    public static class RestsharpExtensions
    {
        public static LoggableRestRequest Simplify(this RestRequest request)
        {
            bool temp = ((Int32.TryParse(
                request.Parameters.FirstOrDefault(
                    p => p.Name.Contains("Length")
                ).Value.ToString(), out int i
            ) ? i : 0) > 7500);

            Func<Parameter, bool> parameterSelector = null;
            if (temp)
                parameterSelector = p => p.Type != ParameterType.UrlSegment && p.Type != ParameterType.RequestBody;
            else
                parameterSelector = p => p.Type != ParameterType.UrlSegment;

            return new LoggableRestRequest()
            {
                Files = request.Files.ToDictionary(
                    f => f.FileName,
                    f => f.ContentType
                ),
                Method = request.Method,
                Paramters = request.Parameters.Where(parameterSelector).GroupBy(
                    p => p.Type,
                    p => p,
                    (k, g) => new KeyValuePair<string, Dictionary<string, string>>(
                        k.ToString(), g.ToDictionary(
                            p => p.Name,
                            p => p.Value.ToString()
                        )
                    )
                ).ToDictionary(),
                Resource = request.Resource,
                NumAttempts = request.Attempts
            };

            
        }
    }

    public struct LoggableRestRequest
    {
        public Dictionary<string, string> Files { get; set; }
        public Method Method { get; set; }
        public Dictionary<string, Dictionary<string, string>> Paramters { get; set; }
        public string Resource { get; set; }
        public int NumAttempts { get; set; }
    }
}
