using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnDLib.Models
{
    public class Language
    {
        public string Name { get; set; }
        public byte Range { get; set; }
        public string Note { get; set; }
        public bool Granted { get; set; }
    }
}
