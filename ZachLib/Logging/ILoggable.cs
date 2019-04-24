using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Logging
{
    public interface ILoggable : IDisposable
    {
        object[] ToFileString();
        object[] ToLogEntryString();
    }
}
