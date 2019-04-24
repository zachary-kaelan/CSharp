using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib.Models
{
    public class RequestData<T>
    {
        public T data { get; private set; }
        public string status { get; private set; }
    }

    public struct PageInfo
    {
        public string end_cursor { get; private set; }
        public bool has_next_page { get; private set; }
    }

    public class UserTemp<T>
    {
        public T user { get; private set; }
    }

    public struct UserBase
    {
        public string username { get; private set; }
    }

    public class EdgesTemp<T>
    {
        public KeyValuePair<string, T>[] edges { get; private set; }
    }
    
    public class Stories
    {
        public string status { get; private set; }
        public StoriesTray[] tray { get; private set; }
    }

}
