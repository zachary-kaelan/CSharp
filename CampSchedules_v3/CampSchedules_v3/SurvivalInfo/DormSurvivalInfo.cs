using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3
{
    public struct DormSurvivalInfo
    {
        internal static UnboundedByteGenerator GEN = new UnboundedByteGenerator();

        public SortedSet<byte> AllowedExclusives;
        public byte[] AllowedOtherDorms;
        private byte _numOtherDorms;

        public DormSurvivalInfo(byte[] exclusives, byte[] otherDorms)
        {
            AllowedExclusives = new SortedSet<byte>(exclusives);
            AllowedOtherDorms = otherDorms;
            _numOtherDorms = (byte)AllowedOtherDorms.Length;
        }

        internal byte GetNewOtherDorm() =>
            AllowedOtherDorms[GEN.GetNext(_numOtherDorms)];
    }
}
