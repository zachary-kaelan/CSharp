using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            var gen = new Random();
            TestClass[] clsArr = new TestClass[4];
            for (int i = 0; i < 4; ++i)
            {
                clsArr[i] = new TestClass(gen.Next(256));
            }

            var cls = clsArr[2];
            var clsArr2 = new TestClass[4];
            clsArr2[1] = cls;
            clsArr2[3] = clsArr[0];
            clsArr2[1].TestInt = -1;
            clsArr2[3].TestInt = -1;

            SortedDictionary<int, TestStruct> strDict = new SortedDictionary<int, TestStruct>();
            var str = new TestStruct();
            strDict.Add(0, str);
            for (int i = 0; i < 10; ++i)
            {
                Console.WriteLine(strDict[0].GetPointer());
            }
            Console.ReadLine();

            var sorted = new SortedDictionary<int, TestClass>(
                Enumerable.Range(0, 9).ToDictionary(
                    i => i,
                    i => new TestClass(i)
                )
            );

            var where = sorted.Values.Where(t => t.TestInt > 20);
            var test = where.ElementAt(2);
            Console.WriteLine(test.ToString());
            test.TestInt = 23;
            Console.WriteLine(sorted[test.TestKey].ToString());
            Console.ReadLine();
        }
    }

    public class TestClass
    {
        public int TestKey { get; set; }
        public int TestInt { get; set; }

        public TestClass(int testInt)
        {
            TestKey = testInt;
            TestInt = testInt * testInt;
        }

        public override string ToString()
        {
            return TestKey.ToString() + ", " + TestInt.ToString();
        }
    }

    public struct TestStruct
    {
        public int Pointer { get; set; }

        public int GetPointer() => ++Pointer;
    }
}
