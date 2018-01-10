using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiotAPI
{
    public enum HttpStatusCode
    {
        BAD_REQUEST = 400,
        UNAUTHORIZED = 401,
        FORBIDDEN = 403,
        DATA_NOT_FOUND = 404,
        METHOD_NOT_ALLOWED = 405,
        UNSUPPORTED_MEDIA_TYPE = 415,
        RATE_LIMIT_EXCEEDED = 429,
        INTERNAL_SERVER_ERROR = 500,
        BAD_GATEWAY = 502,
        SERVICE_UNAVAILABLE = 503,
        GATEWAY_TIMEOUT = 504
    }
    public static class Calculator
    {
        public static const double MAX_FIRST_SOFT_CAP = 0.8 * (490 - 415);
        public double MoveSpeed(int baseSpeed, int[] flatBonuses = null, double[] percentBonuses = null, double[] slows = null, double[] multiplicative = null)
        {
            double totalSpeed = (Convert.ToDouble(baseSpeed) +
                (flatBonuses == null ? 1 : flatBonuses.Sum(b => Convert.ToDouble(b)))) *
                (percentBonuses == null ? 1 : 1 + percentBonuses.Sum(b => b)) *
                (slows == null ? 1 : 1 - slows.Max()) * 
                (multiplicative == null ? 1 : 1 + multiplicative.Sum(d => d));
            
        }
    }
}
