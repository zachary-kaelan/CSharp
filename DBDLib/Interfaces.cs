using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBDLib
{
    public interface IDBDObject
    {
        string Condense();
        void LoadCondensed(string condensed);
        string ToString(bool shortened);
    }
}
