using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PDFTesting
{
    [Flags]
    public enum Test
    {
        Boogity,
        Woogity,
        ByTheBeat
    };

    class Program
    {
        static void Main(string[] args)
        {
            string domainName = Environment.UserDomainName;
            string userName = Environment.UserName;

            Test test = Test.Boogity | Test.Woogity;
            bool hasByTheBeat = test.HasFlag(Test.ByTheBeat);
            bool hasWoogity = test.HasFlag(Test.Woogity);
            var flags = test.GetFlags();
            
            Console.WriteLine("DONE");
            Console.ReadLine();

            string text = PdfTextExtractor.GetTextFromPage(
                new PdfReader(
                    Directory.GetFiles(
                        @"C:\DocUploads", "SA2 - *"
                    ).First()
                ), 2, new SimpleTextExtractionStrategy()
            );

            Console.ReadLine();
        }


    }

    public static class TypesEnumsExtensions
    {
        private static void CheckIsEnum<T>(bool withFlags)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format("Type '{0}' is not an enum", typeof(T).FullName));
            if (withFlags && !Attribute.IsDefined(typeof(T), typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Type '{0}' doesn't have the 'Flags' attribute", typeof(T).FullName));
        }

        public static bool IsFlagSet<Enum>(this Enum value, Enum flag)
        {
            //CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        public static IEnumerable<Enum> GetFlags(this Enum value)
        {
            foreach (Enum flag in Enum.GetValues(typeof(Enum)).Cast<Enum>())
            {
                if (value.IsFlagSet(flag))
                    yield return flag;
            }
        }

        /*public static string ToMultiString<T>(this T value) where T : struct
        {
            return String.Join(" | ", value.GetFlags());
        }

        public static T SetFlags<T>(this T value, T flags, bool on = false) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flags);
            if (on)
            {
                lValue |= lFlag;
            }
            else
            {
                lValue &= (~lFlag);
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        public static T ClearFlags<T>(this T value, T flags) where T : struct
        {
            return value.SetFlags(flags, false);
        }

        public static T CombineFlags<T>(this IEnumerable<T> flags) where T : struct
        {
            CheckIsEnum<T>(true);
            long lValue = 0;
            foreach (T flag in flags)
            {
                long lFlag = Convert.ToInt64(flag);
                lValue |= lFlag;
            }
            return (T)Enum.ToObject(typeof(T), lValue);
        }

        public static string GetDescription<T>(this T value) where T : struct
        {
            CheckIsEnum<T>(false);
            string name = Enum.GetName(typeof(T), value);
            if (name != null)
            {
                FieldInfo field = typeof(T).GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }*/
    }
}
