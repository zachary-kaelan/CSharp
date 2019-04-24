using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace ZachLib
{
    public static class WebExtensions
    {
        public static IEnumerable<Parameter> ToParameters(this IEnumerable<KeyValuePair<string, object>> dict, ParameterType type = ParameterType.GetOrPost)
        {
            foreach(var kv in dict)
            {
                yield return new Parameter()
                {
                    Name = kv.Key,
                    Value = kv.Value,
                    Type = type
                };
            }
            yield break;
        }

        public static IEnumerable<Parameter> ToParameters(this IEnumerable<KeyValuePair<string, string>> dict, ParameterType type = ParameterType.GetOrPost)
        {
            foreach (var kv in dict)
            {
                yield return new Parameter()
                {
                    Name = kv.Key,
                    Value = kv.Value,
                    Type = type
                };
            }
            yield break;
        }

        #region AddParameters
        public static void AddParameters(this IRestRequest request, IEnumerable<Parameter> parameters)
        {
            foreach(Parameter param in parameters)
            {
                request.AddParameter(param);
            }
        }

        public static void AddParameters(this IRestRequest request, params Parameter[] parameters)
        {
            foreach (Parameter param in parameters)
            {
                request.AddParameter(param);
            }
        }

        public static void AddParameters(this IRestRequest request, IEnumerable<KeyValuePair<string, object>> parameters, ParameterType type = ParameterType.GetOrPost)
        {
            request.AddParameters(parameters.ToParameters(type));
        }

        public static void AddParameters(this IRestRequest request, IEnumerable<KeyValuePair<string, string>> parameters, ParameterType type = ParameterType.GetOrPost)
        {
            request.AddParameters(parameters.ToParameters(type));
        }
        #endregion

        #region AddOrUpdateParameters
        public static IRestRequest AddOrUpdateParameters(this IRestRequest request, IEnumerable<Parameter> parameters)
        {
            request.AddParameters(parameters);
            return request;
        }

        public static IRestRequest AddOrUpdateParameters(this IRestRequest request, params Parameter[] parameters)
        {
            request.AddParameters(parameters);
            return request;
        }

        public static IRestRequest AddOrUpdateParameters(this IRestRequest request, IEnumerable<KeyValuePair<string, object>> parameters, ParameterType type = ParameterType.GetOrPost)
        {
            request.AddParameters(parameters, type);
            return request;
        }

        public static IRestRequest AddOrUpdateParameters(this IRestRequest request, IEnumerable<KeyValuePair<string, string>> parameters, ParameterType type = ParameterType.GetOrPost)
        {
            request.AddParameters(parameters, type);
            return request;
        }
        #endregion
    }
}
