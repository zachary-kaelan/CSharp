using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jil;
using RestSharp;
using ZachLib.EventsAndExceptions;
using ZachLib.Logging;

namespace ZachLib.HTTP
{
    public static class HTTPUtils
    {
        static HTTPUtils()
        {
            OnTryExecute += HTTPUtils_OnTryExecute;
        }

        public delegate void ExecutionHandler(object sender, ExecutionEventArgs e);
        public static event HTTPUtils.ExecutionHandler OnTryExecute;

        public static bool WaitForInternet()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                int tries = 0;
                while (true)
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            using (client.OpenRead("http://clients3.google.com/generate_204"))
                            {
                                return true;
                            }
                        }
                    }
                    catch
                    {
                        ++tries;
                        Thread.Sleep(50 * tries);
                    }
                }
            }
        }

        private static void HTTPUtils_OnTryExecute(object sender, ExecutionEventArgs e)
        {
            switch (e.ResponseCode)
            {
                case HttpStatusCode.NotImplemented:
                    LogManager.Enqueue(
                        e.LogName,
                        ZachRGX.FILENAME_DISALLOWED_CHARACTERS.Replace(e.Message, ""),
                        true,
                        ((RestRequest)sender).Simplify(),
                        (Exception)e.Data
                    );
                    break;

                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.Unauthorized:
                    LogManager.Enqueue(
                        e.LogName,
                        EntryType.HTTP,
                        e.Message
                    );
                    break;

                case HttpStatusCode.ServiceUnavailable:
                    LogManager.Enqueue(
                        e.LogName,
                        EntryType.NTWRK,
                        e.Message
                    );
                    break;

                default:
                    if (e.ResponseCode.IsOK())
                        LogManager.Enqueue(
                            e.LogName,
                            EntryType.DEBUG,
                            ((RestRequest)sender).Simplify().ToLogEntryString()
                        );
                    else
                    {
                        var fileName = ZachRGX.FILENAME_DISALLOWED_CHARACTERS.Replace(e.Message, "");

                        if (e.HasResponse)
                        {
                            var response = ((RestResponse)sender).Simplify();
                            LogManager.Enqueue(
                                e.LogName,
                                fileName,
                                response.ToLogEntryString(),
                                response.ToFileString()
                            );
                        }
                        else
                        {
                            var request = ((RestRequest)sender).Simplify();
                            LogManager.Enqueue(
                                e.LogName,
                                fileName,
                                request.ToLogEntryString(),
                                e.Data != null ?
                                    request.ToFileString().Prepend(e.Data).ToArray() :
                                    request.ToFileString()
                            );
                        }
                    }
                    break;
            }
        }

        public static HttpStatusCode TryExecute(this RestClient client, IRestRequest request, string logName, Action getToken, out string content)
        {
            var eventargs = new ExecutionEventArgs();
            eventargs.URL = request.Resource;
            eventargs.LogName = logName;
            try
            {
                IRestResponse response = client.Execute(request);
                eventargs.ResponseCode = response.StatusCode;

                content = response.Content;
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    getToken();
                    eventargs.Message = "Getting new token...";
                    OnTryExecute?.Invoke(request, eventargs);
                    return client.TryExecute(request, logName, getToken, out content);
                }
                else if ((int)response.StatusCode >= 400)
                {
                    eventargs.Message = response.Content;
                    OnTryExecute?.Invoke(response, eventargs);
                    return response.StatusCode;
                }
                else
                {
                    eventargs.Message = "Request successful! " + response.ContentLength.ToString() + " bytes received.";
                    OnTryExecute?.Invoke(request, eventargs);
                    return response.StatusCode;
                }
            }
            catch (Exception e)
            {
                if (WaitForInternet())
                {
                    eventargs.ResponseCode = HttpStatusCode.ServiceUnavailable;
                    eventargs.Message = "Internet was down. Retrying...";
                    OnTryExecute?.Invoke(request, eventargs);
                    return client.TryExecute(request, logName, getToken, out content);
                }

                content = null;
                while (e.InnerException != null && !String.IsNullOrWhiteSpace(e.InnerException.Message))
                    e = e.InnerException;
                eventargs.ResponseCode = HttpStatusCode.NotImplemented;
                eventargs.Data = e;
                eventargs.Message = e.Message;
                OnTryExecute?.Invoke(request, eventargs);

                return HttpStatusCode.NotImplemented;
            }
        }

        public static HttpStatusCode TryExecute<T>(this RestClient client, IRestRequest request, string logName, Action getToken, Options opts, out T content)
        {
            var code = TryExecute(client, request, logName, getToken, out string str);
            if (code.IsOK() && !String.IsNullOrWhiteSpace(str))
            {
                content = JSON.Deserialize<T>(str, opts);
                return code;
            }

            content = default(T);
            return code;
        }

        public static HttpStatusCode TryExecute<T>(this RestClient client, IRestRequest request, string logName, Action getToken, out T content) =>
            client.TryExecute(request, logName, getToken, Options.ISO8601, out content);

        public static HttpStatusCode TryExecute(this RestClient client, IRestRequest request, string logName, Action getToken)
        {
            return TryExecute(client, request, logName, getToken, out _);
        }
    }
}
