using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingLib
{
    public interface IPriorityItem<T> where T : IComparable<T>
    {
        T Priority { get; }
    }
}
