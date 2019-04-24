using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace VTWebLib.Models
{
    public struct VTFunctionStatus
    {
        [JilDirective("successful")]
        public bool Successful { get; private set; }
        [JilDirective("type")]
        public string Type { get; private set; }
        [JilDirective("message")]
        public string Message { get; private set; }
        [JilDirective("redirect")]
        public string Redirect { get; private set; }
    }
}
