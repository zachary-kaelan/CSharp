using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampSchedules_v3.SimpleGeneticAlgorithm
{
    public struct SimpleGene : IEquatable<SimpleGene>, IComparable<SimpleGene>
    {
        public ushort ID { get; private set; }
        public byte Dorm { get; private set; }
        public byte OtherDorm { get; set; }

        public SimpleGene(ushort ID, byte dorm, byte otherDorm, bool isUnstable = true)
        {
            this.ID = ID;
            if (isUnstable)
            {
                if (dorm != 255 && dorm == otherDorm)
                {
                    Dorm = dorm;
                    OtherDorm = 255;
                    // if playing with yourself... then it's natural to clear OtherDorm
                }
                else if (dorm > otherDorm)
                {
                    Dorm = otherDorm;
                    OtherDorm = dorm;
                }
                else
                {
                    Dorm = dorm;
                    OtherDorm = otherDorm;
                }
            }
            else
            {
                Dorm = dorm;
                OtherDorm = otherDorm;
            }
        }

        public SimpleGene(ushort ID)
        {
            this.ID = ID;
            Dorm = SimpleGenePool.GetGeneParamRandomValue(3);
            if (Dorm == 255)
                OtherDorm = 255;
            else
            {
                do
                {
                    OtherDorm = SimpleGenePool.GetGeneParamRandomValue(4);
                } while (OtherDorm == Dorm);

                if (OtherDorm < Dorm)
                {
                    var temp = Dorm;
                    Dorm = OtherDorm;
                    OtherDorm = temp;
                }
                //Debug.Assert(Dorm != OtherDorm);
            }
        }

        public SimpleGene ChangeID(ushort newID) =>
            new SimpleGene()
            {
                ID = newID,
                Dorm = this.Dorm,
                OtherDorm = this.OtherDorm
            };

        public void SerializeWith(FileStream stream)
        {
            stream.Write(BitConverter.GetBytes(ID), 0, 2);
            /*if (IsConstant)
                stream.WriteByte(255);
            else
                stream.WriteByte(0);*/
            stream.WriteByte(Dorm);
            stream.WriteByte(OtherDorm);
        }

        public void Mutate()
        {
            if (
                (SimpleChromosome.GEN.NextDouble() > 0.03 || !SetBlank()) &&
                (SimpleChromosome.GEN.NextDouble() > 0.20 || !SetOtherNull())
            ){
                do
                {
                    Dorm = SimpleGenePool.GetGeneParamRandomValue(3);
                } while (Dorm == 255);

                do
                {
                    OtherDorm = SimpleGenePool.GetGeneParamRandomValue(4);
                } while (OtherDorm == Dorm || OtherDorm == 255);
                //OtherDorm = Constants.SURVIVAL_DORMINFO[Dorm].GetNewOtherDorm();

                if (OtherDorm < Dorm)
                {
                    var temp = Dorm;
                    Dorm = OtherDorm;
                    OtherDorm = temp;
                }

                //Debug.Assert(Dorm != OtherDorm);
            }
        }

        public void MutateBothExcl()
        {
            List<byte> excl = new List<byte>();
            excl.Add(Dorm);
            excl.Add(OtherDorm);
            do
            {
                Dorm = SimpleGenePool.GetGeneParamRandomValue(3);
            } while (excl.Contains(Dorm));

            if (Dorm == 255)
                OtherDorm = 255;
            else
            {
                excl.Add(Dorm);

                do
                {
                    OtherDorm = SimpleGenePool.GetGeneParamRandomValue(4);
                } while (excl.Contains(OtherDorm));

                if (OtherDorm < Dorm)
                {
                    var temp = Dorm;
                    Dorm = OtherDorm;
                    OtherDorm = temp;
                }

                //Debug.Assert(Dorm != OtherDorm);
            }
        }

        public void MutatePrimaryExcl()
        {
            if (OtherDorm != 255)
            {
                var initial = Dorm;
                do
                {
                    Dorm = SimpleGenePool.GetGeneParamRandomValue(3);
                } while (Dorm == initial || Dorm == OtherDorm);

                if (Dorm == 255)
                    OtherDorm = 255;
                else
                {
                    if (OtherDorm < Dorm)
                    {
                        var temp = Dorm;
                        Dorm = OtherDorm;
                        OtherDorm = temp;
                    }
                    //Debug.Assert(Dorm != OtherDorm);
                }
            }
            else
            {
                var initial = Dorm;
                do
                {
                    Dorm = SimpleGenePool.GetGeneParamRandomValue(3);
                } while (Dorm == initial);
            }
        }

        public void MutateOtherExcl()
        {
            var initial = OtherDorm;
            do
            {
                OtherDorm = SimpleGenePool.GetGeneParamRandomValue(4);
            } while (OtherDorm == initial || OtherDorm == Dorm);

            if (OtherDorm < Dorm)
            {
                var temp = Dorm;
                Dorm = OtherDorm;
                OtherDorm = temp;
            }
            //Debug.Assert(Dorm != OtherDorm);
        }

        public bool SetOtherNull()
        {
            if (OtherDorm == 255)
                return false;
            else
            {
                OtherDorm = 255;
                return true;
            }
        }

        public bool SetBlank()
        {
            if (Dorm == 255 && OtherDorm == 255)
                return false;
            else
            {
                Dorm = 255;
                OtherDorm = 255;
                return true;
            }
        }

        public byte Swap(SimpleGene other, bool isOtherDorm, out SimpleGene newOther)
        {
            if (isOtherDorm)
            {
                if (OtherDorm == other.OtherDorm)
                    return _swapSecondary(other, !isOtherDorm, out newOther);
                byte param = OtherDorm;
                OtherDorm = other.OtherDorm;
                other.OtherDorm = param;
            }
            else
            {
                if (Dorm == other.Dorm)
                    return _swapSecondary(other, !isOtherDorm, out newOther);
                byte param = Dorm;
                Dorm = other.Dorm;
                other.Dorm = param;
            }

            newOther = other;
            
            return 0;
        }

        private byte _swapSecondary(SimpleGene other, bool isOtherDorm, out SimpleGene newOther)
        {
            if (isOtherDorm)
            {
                if (OtherDorm == other.OtherDorm)
                {
                    newOther = other;
                    return 2;
                }
                byte param = OtherDorm;
                OtherDorm = other.OtherDorm;
                other.OtherDorm = param;
            }
            else
            {
                if (Dorm == other.Dorm)
                {
                    newOther = other;
                    return 2;
                }
                byte param = Dorm;
                Dorm = other.Dorm;
                other.Dorm = param;
            }

            newOther = other;

            return 1;
        }

        public bool Equals(SimpleGene other) =>
            ID == other.ID;

        public int CompareTo(SimpleGene other) =>
            ID - other.ID;
    }
}
