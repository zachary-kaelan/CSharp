using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstagramLib
{
    internal static class Helpers
    {
        public static long IDToUnixTimestamp(long id) =>
            Convert.ToInt64(
                Math.Round(
                    (
                        (id / 1000000000000) + 11024476.583915909500
                    ) / 0.008388608000
                )
            );

        public static long UnixTimestampToID(long timestamp) =>
            Convert.ToInt64(
                Math.Round(
                    (
                        ((timestamp * 0.008388608000) - 11024476.583915909500) * 1000000000000
                    )
                )
            );

        private const string SHORTCODE_MAP = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        private const int SHORTCODE_BASE = 64;
        public static string IDToShortCode(long id)
        {
            string shortCode = "";
            while (id > 0)
            {
                var r = id % SHORTCODE_BASE;
                id = (id - r) / SHORTCODE_BASE;
                shortCode += SHORTCODE_MAP[(int)r];
            }
            return shortCode;
        }
    }
}
