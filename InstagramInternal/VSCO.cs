using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Extensions;

namespace InstagramLib
{
    public static class VSCO
    {
        private static RestClient CLIENT_IMAGES = new RestClient("https://example.com");

        private static void DownloadImage(string url, string folder)
        {
            CLIENT_IMAGES.BaseUrl = new Uri(url);
            CLIENT_IMAGES.DownloadData(new RestRequest("#", Method.GET)).SaveAs(folder + CLIENT_IMAGES.BaseUrl.Segments.Last());
        }
    }

    public struct VSCO_IMAGE
    {
        public string _id;
        public string grid_name;
    }
}
