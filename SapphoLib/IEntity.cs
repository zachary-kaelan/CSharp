using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    public interface IEntity
    {
        ushort EntityID { get; }
    }

    public interface IEntity<T> : IEntity
        where T : PersonalityTraits
    {
        // EntityID 0 refers to Accordance values
        SortedDictionary<ushort, T> Perceptions { get; }
    }
}
