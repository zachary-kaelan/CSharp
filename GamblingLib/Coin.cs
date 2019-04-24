using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamblingLib
{
    public enum Currency
    {
        BTC,
        ETH,
        LTC,
        DOGE,
        BCH,
        BTX,
        BLK,
        DASH,
        PPC,
        XPM,
        POT
    };

    public struct Coin : IEquatable<Coin>, IConvertible
    {
        public const double BITS_PER_COIN = 1000000.0;
        public const double SATOSHI_PER_BIT = 100.0;
        public const double SATOSHI_PER_COIN = BITS_PER_COIN * SATOSHI_PER_BIT;
        private const double BITS_IN_COINS = 1.0 / BITS_PER_COIN;
        private const double SATOSHI_IN_COINS = 1.0 / SATOSHI_PER_COIN;

        public double Amount { get; set; }
        public double Bits
        {
            get => Amount / BITS_PER_COIN;
            set
            {
                Amount = value * BITS_PER_COIN;
            }
        }
        public double Satoshi
        {
            get => Amount / SATOSHI_PER_COIN;
            set
            {
                Amount = value * SATOSHI_PER_COIN;
            }
        }

        #region ToString
        public override string ToString() => ToString(4);

        public string ToString(int satoshiDigits)
        {
            StringBuilder sb = new StringBuilder();
            double amount = Amount;

            if (amount >= 1.0)
            {
                int coins = Convert.ToInt32(amount);
                sb.Append(coins.ToString() + " Coins");
                amount -= coins;
                if (amount == 0)
                    return sb.ToString();
                else
                    sb.Append(", ");
            }

            if (amount >= BITS_IN_COINS)
            {
                int bits = Convert.ToInt32(amount * BITS_PER_COIN);
                sb.Append(bits.ToString() + " Bits");
                amount -= Convert.ToDouble(bits) * BITS_IN_COINS;
                if (amount == 0)
                    return sb.ToString();
                else
                    sb.Append(", ");
            }

            if (amount >= 0)
            {
                double satoshi = amount * SATOSHI_PER_COIN;
                sb.Append(
                    satoshiDigits == 0 ?
                        Convert.ToInt32(satoshi).ToString() :
                        satoshi.ToString("#." + new string('0', satoshiDigits))
                );
                return sb.ToString();
            }
            else
                return "0";
        }
        #endregion

        public bool Equals(Coin other) => Amount == other.Amount;

        public object ToType(Type t, IFormatProvider provider)
        {

        }
    }
}
