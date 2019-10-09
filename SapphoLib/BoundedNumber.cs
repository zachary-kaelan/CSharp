using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    public struct BoundedNumber : IEquatable<BoundedNumber>, IComparable<BoundedNumber>
    {
        internal const float MAX = 0.9999f;
        internal const float MIN = -0.9999f;

        // TODO: IsPositive field to avoid repeated checking
        private bool _isPositive;
        internal float WeightingFactor { get => (Number + 1.0f) / 2.0f; }
        private float _number;
        private float _unbounded;
        private bool _calculatedUnbounded;

        public float Number {
            get => _number;
            set
            {
                _number = _boundNumber(value);
                _unbounded = _getUnbounded(_number);
                _calculatedUnbounded = true;
            }
        }
        public float UnboundedNumber {
            get {
                if (!_calculatedUnbounded)
                {
                    _unbounded = _getUnbounded(_number);
                    _calculatedUnbounded = true;
                }
                return _unbounded;
            }
            set {
                _unbounded = value;
                _number = _fromUnbounded(_unbounded);
                _calculatedUnbounded = true;
            }
        }

        public BoundedNumber(UBoundedNumber uBounded)
        {
            _calculatedUnbounded = false;
            _number = (uBounded.Number * 2) - 1;
            _unbounded = 0;
            _isPositive = _number >= 0;
        }

        public BoundedNumber(float num)
        {
            _calculatedUnbounded = false;
            _number = _boundNumber(num);
            _unbounded = 0;
            _isPositive = num >= 0;
        }

        public BoundedNumber(float num, float unbounded)
        {
            _number = num;
            _isPositive = num >= 0;
            if (unbounded == 0 && num != 0)
            {
                _calculatedUnbounded = false;
                _unbounded = 0;
            }
            else
            {
                _calculatedUnbounded = true;
                _unbounded = unbounded;
            }
        }

        public static BoundedNumber FromUnboundedNumber(float unbounded) => 
            new BoundedNumber(_fromUnbounded(unbounded), unbounded);

        internal static float _fromUnbounded(float unbounded) =>
            unbounded > 0 ?
                1 - (1 / (1 + unbounded)) :
                (1 / (1 - unbounded)) - 1;

        private static float _getUnbounded(float bounded) => 
                bounded > 0 ?
                    (1 / (1 - bounded)) - 1 :
                    1 - (1 / (1 + bounded));

        private static float _boundNumber(float num)
        {
            if (num >= 1f)
                return MAX;
            else if (num <= -1f)
                return MIN;
            else
                return num;
        }

        public float Invert() =>
            !_isPositive ? 
                MIN - Number : 
                MAX - Number;

        public UBoundedNumber Significance() =>
            new UBoundedNumber(Math.Abs(Number));

        public UBoundedNumber UInvert() =>
            new UBoundedNumber(
                !_isPositive ?
                    MAX + Number :
                    MAX - Number
            );

        // Can use to blend three numbers together
        //     For example: Blend(
        //         Trustworthy_Mendacious, the speaker's trait
        //         Likely_Unlikely, the speaker's statement
        //         Gullible_Suspicious, the listener's trait
        //     )
        //
        // Can be used to scale down numbers
        //     Blend(0, Nice_Nasty, 0) scales to a place halfway toward zero
        //     A positive number scales it down more, a negative scales it down less
        // Can be used to scale up numbers
        //     If (num > 0) then Blend(num, 1, 0)
        //     If (num < 0) then Blend(num, -1, 0)
        // Other variables can be used in scaling to amplify or suppress a number by a variable degree
        // Bigger number favors other
        public float Blend(BoundedNumber other, BoundedNumber weight) =>
            BoundedHelpers.Blend(Number, other.Number, weight.WeightingFactor);

        public float Blend(BoundedNumber other, float weightingFactor) =>
            BoundedHelpers.Blend(Number, other.Number, weightingFactor);

        public BoundedNumber BlendToBounded(BoundedNumber other) =>
            new BoundedNumber((Number + other.Number) / 2, 0);

        public BoundedNumber BlendToBounded(BoundedNumber other, BoundedNumber weight) =>
            (other.Number * weight.WeightingFactor) +
            (Number * (1f - weight.WeightingFactor));

        public BoundedNumber BlendToBounded(BoundedNumber other, float weightingFactor) =>
            (other.Number * weightingFactor) +
            (Number * (1f - weightingFactor));

        public float Suppress() => Number * 0.5f;

        public BoundedNumber Suppress(float intensity) => 
            BoundedHelpers.Blend(0, Number, 1 - intensity);

        public float Amplify() =>
            (Number * 0.5f) + (Number > 0 ? 0.5f : -0.5f);

        public float Amplify(float intensity) =>
            BoundedHelpers.Blend(Number, Number > 0 ? 1 : -1, intensity);

        public override string ToString() => Number.ToString("#.00");

        public bool Equals(BoundedNumber other) => Number == other.Number;

        public int CompareTo(BoundedNumber other)
        {
            if (Equals(other))
                return 0;
            return WeightingFactor > other.WeightingFactor ? 1 : -1;
        }

        public string ToString(string format) => Number.ToString(format);

        public static implicit operator BoundedNumber(float num) => new BoundedNumber(num);
        public static implicit operator float(BoundedNumber bNum) => bNum.Number;
        public static BoundedNumber operator +(BoundedNumber n1, BoundedNumber n2) =>
            FromUnboundedNumber(n1.UnboundedNumber + n2.UnboundedNumber);
        public static BoundedNumber operator +(BoundedNumber n1, float n2) =>
            FromUnboundedNumber(n1.UnboundedNumber + n2);
        public static BoundedNumber operator -(BoundedNumber n1, BoundedNumber n2) =>
            FromUnboundedNumber(n1.UnboundedNumber - n2.UnboundedNumber);
        public static BoundedNumber operator -(BoundedNumber num) => num._calculatedUnbounded ? new BoundedNumber(-num.Number, -num._unbounded) : new BoundedNumber(-num.Number);
        public static bool operator !(BoundedNumber num) => num._number < 0;
        public static BoundedNumber operator /(BoundedNumber num, int integer) => FromUnboundedNumber(num.UnboundedNumber / integer);
        public static BoundedNumber operator *(BoundedNumber num, float other) => num.UnboundedNumber * other;
    }
}
