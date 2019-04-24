using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RedditSharpLib
{
    internal static class ExtensionMethods
    {
        public static Dictionary<string, object> ConvertToRecursiveJSON<T>(this T obj)
        {
            if (obj == null)
                return null;
            return typeof(T).GetProperties(
                BindingFlags.Instance | 
                BindingFlags.Public
            ).ToDictionary(
                p => p.Name,
                p => p.GetValue(obj)
            );
        }
    }
}
