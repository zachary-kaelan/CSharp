using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChemicalsLib.Models
{
    public class Molecule : IEnumerable<Element>
    {
        private Element[] _elements;

        public bool Organic { get; set; }
        public uint ID { get; set; }
        public sbyte Charge { get; set; }
        public ChemicalState State { get; set; }
        public byte Count { get; set; }

        #region IReadOnlyList<Element> Implementation
        public IEnumerator<Element> GetEnumerator()
        {
            return ((IReadOnlyList<Element>)_elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _elements.GetEnumerator();
        }
        #endregion
    }
}
