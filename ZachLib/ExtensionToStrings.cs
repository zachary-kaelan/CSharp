using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    public static class ExtensionToStrings
    {
        public static string ToArrayString<T>(this IEnumerable<T> array) =>
            "[" + String.Join(", ", array) + "]";

        public static string ToArrayString<T>(this IEnumerable<T> array, string format) where T : IFormattable =>
            "[" + String.Join(", ", array.Select(a => a.ToString(format, null))) + "]";

        public static string ToArrayString<T>(this IEnumerable<T> array, string format, IFormatProvider formatProvider) where T : IFormattable =>
            "[" + String.Join(", ", array.Select(a => a.ToString(format, formatProvider))) + "]";

        public static string ToErrString(this Exception e, int tabCount = 0)
        {
            StringBuilder sb = new StringBuilder("");

            sb.Append("MESSAGE:\t\t");
            if (!String.IsNullOrWhiteSpace(e.Message))
                sb.AppendLine(e.Message);
            else if (e.InnerException != null && !String.IsNullOrWhiteSpace(e.InnerException.Message))
                sb.AppendLine(e.InnerException.Message);
            else
                sb.AppendLine("None.");

            sb.Append(new string('\t', tabCount));
            sb.Append("SOURCE:\t\t");
            sb.AppendLine(e.Source);

            sb.Append(new string('\t', tabCount));
            sb.Append("STACKTRACE:\t");
            sb.AppendLine(e.StackTrace.Replace("\n", "\n" + new string('\t', tabCount + 3)));

            sb.Append(new string('\t', tabCount));
            sb.Append("TARGETSITE:\t");
            sb.AppendLine(e.TargetSite.Name);

            return sb.ToString();
        }

        /*public static string ToPropertiesString<T>(this T obj, bool displayName = false, bool publicOnly = true)
        {
            return obj.ToPropertiesString(typeof(T), displayName, publicOnly);
        }*/

        private const string PROPERTY_STRING_FORMAT = "{0} : {1}";
        public static string ToPropertiesString(this object obj, bool displayName = false, bool publicOnly = true)
        {
            string strFormat = null;
            string title = "";
            var type = obj.GetType();
            if (displayName)
            {
                strFormat = "\t" + PROPERTY_STRING_FORMAT;
                title = type.Name.ToConsoleTitle();
            }
            else
                strFormat = PROPERTY_STRING_FORMAT;

            return title +
                String.Join(
                    "\r\n",
                    type.GetProperties(
                        publicOnly ?
                            (BindingFlags.Instance | BindingFlags.Public) :
                            (BindingFlags.Instance)
                    ).Select(
                        prop => String.Format(
                            strFormat,
                            prop.Name,
                            prop.GetValue(obj)
                        )
                    )
                );
        }
    }
}
