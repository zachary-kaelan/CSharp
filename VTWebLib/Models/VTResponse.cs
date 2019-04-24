using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace VTWebLib.Models
{
    public class VTResponse
    {
        public IRestResponse Response { get; set; }
        public object Content { get; set; }
        public bool IsError { get; set; }
    }
}
