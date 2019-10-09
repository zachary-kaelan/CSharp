using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphoLib
{
    public struct UBoundedNumber
    {
        public float Number
        {
            get => _number;
            set
            {
                _number = _boundNumber(value);
                _unbounded = _getUnbounded(_number);
                _calculatedUnbounded = true;
            }
        }
        public float UnboundedNumber
        {
            get
            {
                if (!_calculatedUnbounded)
                {
                    _unbounded = _getUnbounded(_number);
                    _calculatedUnbounded = true;
                }
                return _unbounded;
            }
            set
            {
                _unbounded = Math.Abs(value);
                _number = _fromUnbounded(_unbounded);
                _calculatedUnbounded = true;
            }
        }
        private float _number;
        private float _unbounded;
        private bool _calculatedUnbounded;

        public UBoundedNumber(float num)
        {
            _calculatedUnbounded = false;
            _number = _boundNumber(num);
            _unbounded = 0;
        }

        public UBoundedNumber(float num, float unbounded)
        {
            _calculatedUnbounded = true;
            _number = _boundNumber(num);
            _unbounded = unbounded;
        }

        public UBoundedNumber(BoundedNumber num)
        {
            _number = num.WeightingFactor;
            _unbounded = 0;
            _calculatedUnbounded = false;
        }

        public static UBoundedNumber FromUnbounded(float unbounded) =>
            new UBoundedNumber(_fromUnbounded(unbounded), unbounded);

        private static float _boundNumber(float num) =>
            Math.Min(0.99f, Math.Abs(num));

        private static float _fromUnbounded(float unbounded) =>
            1 - (1 / (1 + unbounded));

        private static float _getUnbounded(float bounded) =>
            (1 / (1 - bounded)) - 1;

        public UBoundedNumber Suppress(float intensity) =>
            new UBoundedNumber(_number * intensity);

        public float Amplify() =>
            (Number * 0.5f) + 0.5f;

        public float Amplify(float weight) =>
            (Number * (1 - weight)) + weight;

        public UBoundedNumber Blend(UBoundedNumber other, float weight) =>
            new UBoundedNumber(
                (other.Number * weight) +
                (Number * (1f - weight))
            );

        public string ToString(string format) => Number.ToString(format);

        public static UBoundedNumber operator +(UBoundedNumber num1, UBoundedNumber num2) => FromUnbounded(num1.UnboundedNumber + num2.UnboundedNumber);

        public static UBoundedNumber operator +(UBoundedNumber uNum, BoundedNumber num)
        {
            float unbounded = uNum.UnboundedNumber + num.UnboundedNumber;
            if (unbounded < 0)
                throw new OverflowException("BoundedNumber " + num.ToString("#.00") + " cannot be added to UBoundedNumber " + uNum.ToString("#.00") + ".");
            return FromUnbounded(unbounded);
        }

        public static UBoundedNumber operator -(UBoundedNumber uNum, BoundedNumber num)
        {
            float unbounded = uNum.UnboundedNumber - num.UnboundedNumber;
            if (unbounded < 0)
                throw new OverflowException("BoundedNumber " + num.ToString("#.00") + " cannot be added to UBoundedNumber " + uNum.ToString("#.00") + ".");
            return FromUnbounded(unbounded);
        }

        public static BoundedNumber operator -(UBoundedNumber num) => new BoundedNumber(-num.Number, -num.UnboundedNumber);
        public static UBoundedNumber operator -(UBoundedNumber num, UBoundedNumber other) => new UBoundedNumber(Math.Max(num.Number - other.Number, 0));
        public static UBoundedNumber operator *(UBoundedNumber num, float multiplier) => new UBoundedNumber(num.Number * multiplier);
        public static UBoundedNumber operator /(UBoundedNumber num, float divisor) => new UBoundedNumber(num.Number / divisor);
        public static explicit operator BoundedNumber(UBoundedNumber num) => new BoundedNumber((num.Number * 2f) - 1);

        public static bool operator >(UBoundedNumber num, UBoundedNumber otherNum) => num.Number > otherNum.Number;
        public static bool operator <(UBoundedNumber num, UBoundedNumber otherNum) => num.Number < otherNum.Number;
        public static bool operator ==(BoundedNumber num, UBoundedNumber otherNum) => num.Number == otherNum.Number;
        public static bool operator !=(BoundedNumber num, UBoundedNumber otherNum) => num.Number != otherNum.Number;

        public static bool operator >(UBoundedNumber num, float otherNum) => num.Number > otherNum;
        public static bool operator <(UBoundedNumber num, float otherNum) => num.Number < otherNum;
    }
}
