using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    public static class TypesAndCastingExtensions
    {
        public static T CastTo<T>(this object obj)
        {
            return (T)obj;
        }

        public static Type GetAnyElementType(this Type type)
        {
            // Type is Array
            // short-circuit if you expect lots of arrays 
            if (type.IsArray)
                return type.GetElementType();

            // type is IEnumerable<T>;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // type implements/extends IEnumerable<T>;
            var enumType = type.GetInterfaces()
                                    .Where(t => t.IsGenericType &&
                                           t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                                    .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
            return enumType ?? type;
        }

        public static bool TryGetField(this Type type, string name, out FieldInfo field)
        {
            FieldInfo temp = null;
            try
            {
                temp = type.GetField(name);
                if (temp == null)
                {
                    field = null;
                    return false;
                }

                field = temp;
                return true;
            }
            catch
            {
                field = null;
                return false;
            }
        }

        public static bool TryGetProperty(this Type type, string name, out PropertyInfo prop)
        {
            PropertyInfo temp = null;
            try
            {
                temp = type.GetProperty(name);
                if (temp == null)
                {
                    prop = null;
                    return false;
                }

                prop = temp;
                return true;
            }
            catch
            {
                prop = null;
                return false;
            }
        }

        private static readonly Type strType = typeof(string);
        private static readonly Type intType = typeof(int);
        private static readonly Type dblType = typeof(double);
        private static readonly Type dateType = typeof(DateTime);
        internal static object HandleConversion<T>(this object obj)
        {
            return obj.HandleConversion(typeof(T));
        }

        internal static object HandleConversion(this object obj, Type type)
        {
            if (type == strType)
                return obj.ToString();
            else if (type == intType)
                return int.TryParse(obj.ToString(), out int num) ? num : 0;
            else if (type == dateType)
                return DateTime.TryParse(obj.ToString(), out DateTime date) ? date : new DateTime(0, 0, 0);
            else if (type == dblType)
                return double.TryParse(obj.ToString(), out double dbl) ? dbl : 0.0;
            return obj;
        }
    }

    public static class CommonTypes
    {
        public static readonly Type IntType = typeof(int);
        public static readonly Type DblType = typeof(double);
        public static readonly Type FltType = typeof(float);

        public static readonly Type StrType = typeof(string);
        public static readonly Type TSType = typeof(TimeSpan);
        public static readonly Type DttType = typeof(DateTime);
        public static readonly Type DtoType = typeof(DateTimeOffset);
    }
}
