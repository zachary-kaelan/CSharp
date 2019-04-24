using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib
{
    /// <summary> Enum Extension Methods </summary>
    /// <typeparam name="T"> Enum being boxed </typeparam>
    public static class Enum<T> where T : struct, IConvertible
    {
        private static readonly Type t = typeof(T);
        public static readonly string[] Names = Enum.GetNames(t);
        public static readonly int Count = Names.Length;
        public static readonly bool Flags = Attribute.IsDefined(t, typeof(FlagsAttribute));
        public static readonly int[] Values = Enum.GetValues(t).Cast<int>().ToArray();

        static Enum()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", t.FullName));
        }

        /*protected static void Setup(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
            if (Flags != withFlags)
                ThrowFlagsException();

            Names = Enum.GetNames(typeof(T));
            Count = Names.Length;
        }*/

        public static int GetValue(string name)
        {
            return (int)Enum.Parse(t, name, true);
        }

        public static string GetName(int value)
        {
            return Names[Array.IndexOf<int>(Values, value)];
        }

        private static void ThrowFlagsException()
        {
            throw new ArgumentException(string.Format("Type '{0}' " + (Flags ? "has" : "doesn't have") + " the 'Flags' attribute", typeof(T).FullName));
        }
    }
}
