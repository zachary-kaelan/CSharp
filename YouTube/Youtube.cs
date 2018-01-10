using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Jil;
using RestSharp;
using RestSharp.Extensions;

namespace YouTube
{
    public class Youtube
    {
        private static readonly CookieContainer cookies = new CookieContainer();
        private static readonly RestClient client = new RestClient("https://www.youtube.com/") {
            CookieContainer = cookies,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36",
            BaseHost = "www.youtube.com"
        };
        
        static Youtube()
        {
            client.AddDefaultHeader("X-YouTube-Client-Name", "1");
            client.AddDefaultHeader("X-YouTube-Client-Version", "2.20171218");
            client.AddDefaultHeader("X-YouTube-Identity-Token", "QUFFLUhqbnRfUUhHbVBTTjhlRGg1Vnc3Zks0cUM1ZW1Id3w=");
            client.AddDefaultHeader("X-SPF-Previous", "https://www.youtube.com/feed/history");
            client.AddDefaultHeader("X-SPF-Referer", "https://www.youtube.com/feed/history");

            client.AddDefaultHeader("Accept-Encoding", "gzip, deflate, br");
            client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.9");
            client.AddDefaultHeader("Referer", "https://www.youtube.com/feed/history");

            cookies.Add(new Cookie("VISITOR_INFO1_LIVE", "C8ho5Rgx1w8", "/", ".youtube.com"));
            cookies.Add(new Cookie("CONSENT", "YES+US.en+20171029-08-0", "/", ".youtube.com"));
            cookies.Add(new Cookie("LOGIN_INFO", "AICDL0QwRQIga73rgfKxkomWu8ZjTDWtHJNQIxvtvHxt6RpLJcRvDGUCIQDhzBa8ewL_t9kzeYaEDLzYyDxYLG-cgtnr6FN_Gj9nzg:QUpCWWozcFY2NklWWklPZlZlZEFjMlJWVnVQeE5LUTNteFZtRWYtZEl0NFNoampuZXB0OUpOb3dXZFhKX0Z6WHhYXzlqOEZCdDAxQ2o2bGc0X2xrVXBTWG8yUzZOWmMzTEhTRlo0WmY1b1YyLXNBSGk1WWdQdVQwSk11QUIwVlVMNGNoSDVXRHZETGwtSF9sUDlRdzlXUVBDMV9xVk5xaEFhejlQYTNZcmF6aXNwWEdlSGJyZnA0", "/", ".youtube.com"));
            cookies.Add(new Cookie("SID", "jgUhvM6e6cJWWLyD-H0ArsDsInHzSWXSz9-lXdplVF5UGNF8p4ACIHQ6upLwvq4NWbqInQ.", "/", ".youtube.com"));
            cookies.Add(new Cookie("HSID", "ArCDlaCh4e-3gRAxu", "/", ".youtube.com"));
            cookies.Add(new Cookie("SSID", "AuIVoovXOvTI_wj3v", "/", ".youtube.com"));
            cookies.Add(new Cookie("APISID", "RJuzAexoNsBNiuPh/AdP9dz31CbNGX3_NL", "/", ".youtube.com"));
            cookies.Add(new Cookie("SAPISID", "S1EfX6a8_IQKyk1L/AdHvW_E4A3FBgkDL3", "/", ".youtube.com"));
            cookies.Add(new Cookie("YSC", "3HkioFqWu7Y", "/", ".youtube.com"));
            cookies.Add(new Cookie("PREF", "al=en", "/", ".youtube.com"));
            cookies.Add(new Cookie("VISITOR_INFO1_LIVE", "C8ho5Rgx1w8", "/", ".youtube.com"));
            cookies.Add(new Cookie("CONSENT", "YES+US.en+20171029-08-0", "/", ".youtube.com"));
            cookies.Add(new Cookie("VISITOR_INFO1_LIVE", "C8ho5Rgx1w8", "/", ".youtube.com"));
            cookies.Add(new Cookie("CONSENT", "YES+US.en+20171029-08-0", "/", ".youtube.com"));
            cookies.Add(new Cookie("LOGIN_INFO", "AICDL0QwRQIga73rgfKxkomWu8ZjTDWtHJNQIxvtvHxt6RpLJcRvDGUCIQDhzBa8ewL_t9kzeYaEDLzYyDxYLG-cgtnr6FN_Gj9nzg:QUpCWWozcFY2NklWWklPZlZlZEFjMlJWVnVQeE5LUTNteFZtRWYtZEl0NFNoampuZXB0OUpOb3dXZFhKX0Z6WHhYXzlqOEZCdDAxQ2o2bGc0X2xrVXBTWG8yUzZOWmMzTEhTRlo0WmY1b1YyLXNBSGk1WWdQdVQwSk11QUIwVlVMNGNoSDVXRHZETGwtSF9sUDlRdzlXUVBDMV9xVk5xaEFhejlQYTNZcmF6aXNwWEdlSGJyZnA0", "/", ".youtube.com"));
            cookies.Add(new Cookie("SID", "jgUhvM6e6cJWWLyD-H0ArsDsInHzSWXSz9-lXdplVF5UGNF8p4ACIHQ6upLwvq4NWbqInQ.", "/", ".youtube.com"));
            cookies.Add(new Cookie("HSID", "ArCDlaCh4e-3gRAxu", "/", ".youtube.com"));
            cookies.Add(new Cookie("SSID", "AuIVoovXOvTI_wj3v", "/", ".youtube.com"));
            cookies.Add(new Cookie("APISID", "RJuzAexoNsBNiuPh/AdP9dz31CbNGX3_NL", "/", ".youtube.com"));
            cookies.Add(new Cookie("SAPISID", "S1EfX6a8_IQKyk1L/AdHvW_E4A3FBgkDL3", "/", ".youtube.com"));
            cookies.Add(new Cookie("YSC", "3HkioFqWu7Y", "/", ".youtube.com"));
            cookies.Add(new Cookie("PREF", "al=en", "/", ".youtube.com"));
            cookies.Add(new Cookie("gsScrollPos-1757", "0", "/", ".youtube.com"));
            cookies.Add(new Cookie("gsScrollPos-1751", "0", "/", ".youtube.com"));
            cookies.Add(new Cookie("gsScrollPos-1788", "0", "/", ".youtube.com"));
            cookies.Add(new Cookie("SID", "jwUhvKH_q2-5Oze547iG2cpj-HDQVCBLlXORxOpZfadD58GWW49EigOgVQWrYqlHmsRb6A.", "/", ".youtube.com"));
            cookies.Add(new Cookie("HSID", "AXbjN34W8sytPinmU", "/", ".youtube.com"));
            cookies.Add(new Cookie("SSID", "A5te-pXu3u3jJLJyT", "/", ".youtube.com"));
            cookies.Add(new Cookie("APISID", "7tvpKsKMjUoqN7b3/ALDMKK0OCTl0WDASy", "/", ".youtube.com"));
            cookies.Add(new Cookie("SAPISID", "XItHiPntvAGkTfqD/AH-_byhQS83FhcxZA", "/", ".youtube.com"));
            cookies.Add(new Cookie("LOGIN_INFO", "AICDL0QwRQIgOWaK0FQ-H93WhMeZ3stK43xg-Wesev2C4e0rGk7DiuACIQCH5tXV18pUNblY6DWFo7_Rn4Yize02mE_uWh-hm187Mg:QUpCWWozcjVzVWhlWS1SdGFjTVpZalZRTFhiUlpTcEgtV1FwcTJaaXVKY21sbk16X0MzSE1yYkdNXzJBTjUyUlZWVTduZ2V0a0pvOHBrNXZ0WTNBRGxiWVlRVjEtSkVyYXJxcHB2LUR2QnBDay01OExoajhTSE1MWERFT2I1eEhXM1pLX2pFZWVsNDFpVEpZT3RFQ2dVWmRHOWV5aEFzNkZiS3dyZjBUOWgyOHFBbW43U004THpJ", "/", ".youtube.com"));
            cookies.Add(new Cookie("PREF", "al=en&f1=50000000", "/", ".youtube.com"));
            cookies.Add(new Cookie("ST-cv3kr6", "itct=CLkBELUsGAAiEwiv1_-q66DYAhXOwKoKHZBbCS4yCmctcGVyc29uYWw%3D&csn=gao-WuOXMc7TqQWZ55OQCg", "/", ".youtube.com"));

        }

        public Youtube()
        {

        }

        public List<VideoListModel> GetWatchHistory()
        {
            Page page = JSON.Deserialize<IEnumerable<Page>>(
                client.Execute(
                    new RestRequest(
                        "feed/history?pbj=1", 
                        Method.GET
                    )
                ).Content
            ).Last();
            client.AddDefaultParameter("session_token", page.xsrf_token, ParameterType.GetOrPost);

            ItemSectionRenderer temp = page.response.contents.twoColumnBrowserResultsRenderer.tabs.tabRenderer.content.sectionListRenderer.contents.itemSectionRenderer;
            List<VideoListModel> videos = new List<VideoListModel>(
                temp.contents.Select(v => v.Simplify())
            );
            page = new Page();

            ItemSectionRenderer GetItemSectionRenderer(RestRequest request)
            {
                return JSON.Deserialize<IEnumerable<Page>>(
                    client.Execute(request).Content
                ).Last().response.contents.twoColumnBrowserResultsRenderer.tabs.tabRenderer.content.sectionListRenderer.contents.itemSectionRenderer;
            }

            bool check = true;
            do
            {
                try
                {
                    RestRequest request = new RestRequest("browse_ajax", Method.POST);
                    request.AddParameter("ctoken", temp.continuations.nextContinuationData.continuation, ParameterType.QueryString);
                    request.AddParameter("itct", temp.continuations.nextContinuationData.clickTrackingParams, ParameterType.QueryString);
                    temp = GetItemSectionRenderer(request);
                    videos.AddRange(temp.contents.Select(v => v.Simplify()));
                }
                catch (Exception e)
                {
                    check = false;
                }
            } while (check && temp.contents != null && temp.contents.Any());

            return videos;
        }
    }
}
