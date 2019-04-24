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

//using iTextSharp.text.pdf;
//using iTextSharp.text.pdf.parser;
using ZachLib;
using ZachLib.Statistics;
using ZachLib.Parsing;
using PPLib;
using ZillowLib;
using RDotNet;
using RDotNet.NativeLibrary;

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
        public static string[] inputNames = new string[]
        {
            "Bathrooms",
            "Bedrooms",
            "HomeValue",
            "LastPrice",
            "LastSoldOn",
            "LotSqFt",
            "MedianInc",
            "SquareFt",
            "TaxAssess",
            "YearBuilt",
            "TaxRate"
        };

        public static readonly string[] outputNames = new string[]
        {
            "Active",
            "ActiveDuration",
            "NumberOfInvoices"
        };

        public static readonly string[] areasInputNames = new string[]
        {
            "Population",
            "MedianAge",
            //"NumCompanies",
            "GraduationRate",
            //"NumGovernments",
            "HousingUnits",
            "MedianIncome",
            "ForeignBorn",
            "PovertyRate",
            "Veterans",
            "PopualtionDensity",
            "VeteransRate",
            "ForeignRate"
        };

        public static readonly string[] areasOutputNames = new string[]
        {
            "CurrentlyActive",
            "ActiveDuration"
        };

        static void Main(string[] args)
        {
            


            var areaStatsTemp = Utils.LoadCSV<AreaStats>(@"E:\Temp\Lists\AreaStats2.csv").ToDictionary(s => s.AreaName, s => s);
            var areasDataTemp = Utils.LoadCSV<AreaData>(@"E:\Temp\Lists\Zillow_Data_-_Areas.csv");
            int count = areasDataTemp.Length;
            List<double[]> areasDataList = new List<double[]>(count);
            List<double[]> areasStatsList = new List<double[]>(count);

            for (int i = 0; i < count; ++i)
            {
                var dataTemp = areasDataTemp[i];
                if (areaStatsTemp.TryGetValue(dataTemp.ZipCode.PadLeft(5, '0'), out AreaStats statsTemp))
                {
                    /*var vector = statsTemp.ToVector();
                    areasDataList.Add(vector.Take(1).ToArray());
                    areasStatsList.Add(vector.Skip(1).ToArray());*/
                    areasDataList.Add(dataTemp.ToVector());
                    areasStatsList.Add(statsTemp.ToVector());
                }
            }

            var areasData = new Matrix(areasDataList.ToArray());
            var areasStats = new Matrix(areasStatsList.ToArray());
            //var areasDataT = areasData.T();
            //var areasStatsT = areasStats.T();
            
            /*REngine.SetEnvironmentVariables();
            REngine engine = REngine.GetInstance();
            engine.Initialize();
            Stack<SymbolicExpression> results = new Stack<SymbolicExpression>();
            results.Push(engine.Evaluate("install.packages(\"energy\")"));
            for (int i = 0; i < areasInputNames.Length; ++i)
            {
                string xName = areasInputNames[i];
                Console.WriteLine(xName);
                engine.SetSymbol(
                    xName,
                    engine.CreateNumericVector(
                        areasStatsT[i]
                    )
                );
            }

            for (int o = 0; o < areasOutputNames.Length; ++o)
            {
                string yName = areasOutputNames[o];
                engine.SetSymbol(
                    yName,
                    engine.CreateNumericVector(
                        areasDataT[o]
                    )
                );
                //Console.WriteLine("\t" + areasOutputNames[o] + " : " + areasStatsT[i].SampleDistanceCorrelationTo(areasDataT[o]).ToString("#.0000"));
            }

            foreach (var input in areasInputNames)
            {
                Console.WriteLine(input);
                foreach(var output in areasOutputNames)
                {
                    results.Push(engine.Evaluate(String.Format("dcor.t({0}, {1}, distance=FALSE)", input, output)));
                    var numeric = results.Pop().AsNumeric();
                    var statistic = numeric.First();
                    Console.WriteLine("\t" + output + " : " + statistic.ToString("#.00"));
                }
            }
            engine.Dispose();
            Console.ReadLine();*/

            //var popCorrelations = new MultiCorrelationHelper(areasStatsT.Ro)

            var areaCorrelations = new MultiCorrelationHelper(areasStats, areasData);
            File.WriteAllText(
                @"E:\Temp\AreasCorrelations.csv",
                areaCorrelations.ToCSV(
                    areasInputNames, areasOutputNames, 4
                )
            );
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            /*File.WriteAllText(
                @"E:\Temp\AreasCorrelations.csv",
                areaCorrelations.ToCSV(
                    areasInputNames.Skip(1).ToArray(), areasInputNames.Take(1).ToArray(), 4
                )
            );
            var invertedInputs = areaCorrelations.InputsCorrelationsMatrix.Invert();
            Console.WriteLine("[" + String.Join("\t", invertedInputs.Dot(areaCorrelations.InputsOutputsCorrelationsMatrix[1])) + "]");
            Console.WriteLine();
            Console.WriteLine(invertedInputs.ToString(2, true));
            Console.ReadLine();*/
            

            /*var model = new XMLModelCreator(File.ReadAllText(@"E:\Gaming\Steam\steamapps\common\World of Tanks Blitz\Data\XML\item_defs\vehicles\uk\components\guns.xml"));
            model.PrintModel();
            Console.ReadLine();*/
            
            //var zillowData = Utils.LoadCSV<ZillowData>(@"E:\Temp\Zillow_Data_-_Setups.csv");
            var zillowData = Utils.LoadCSV<ZillowData>(@"E:\Temp\Zillow_Data_Exclude_Nulls.csv");

            var categorical = zillowData.Select(d => d.HomeType).ToArray();
            var data = zillowData.Select(
                d => d.ToNumbers()
            );
            var inputs = new Matrix(data.Select(n => n.Item1).ToArray());
            inputs.GlueTo(categorical.EncodeEnum("Unknown", out string[] namesOrder));
            inputs.SerializeToFile(@"E:\Zillow Networks", "Inputs", 2);
            var outputs = new Matrix(data.Select(n => n.Item2).ToArray());
            outputs.SerializeToFile(@"E:\Zillow Networks", "Outputs", 0);

            //Console.WriteLine(String.Join("\r\n", namesOrder));
            MultiCorrelationHelper helper = new MultiCorrelationHelper(inputs, outputs);
            inputNames = inputNames.Concat(namesOrder).ToArray();
            int numInputs = inputNames.Length;
            int numOutputs = outputNames.Length;
            int columns = Math.Max(numInputs, numOutputs) + 1;
            string separator = "\r\n" + new string(',', columns) + "\r\n";
            var split = new string[] { "\r\n" };
            string toStringFormat = "#.0000";
            File.WriteAllText(
                @"E:\Temp\Correlations.csv",
                helper.ToCSV(inputNames, outputNames, 4)
            );
            Console.WriteLine("FINISHED");
            Console.ReadLine();
            File.WriteAllText(
                @"E:\Temp\Correlations.csv",
                "," + String.Join(",", inputNames) + "\r\n" +
                String.Join(
                    "\r\n",
                    helper.InputsCorrelationsMatrix.ToCSV(4, numInputs).Split(
                        split,
                        StringSplitOptions.RemoveEmptyEntries
                    ).Select(
                        (l, i) => inputNames[i] + "," + l
                    )
                ) + separator + 
                String.Join(
                    "\r\n",
                    helper.InputsOutputsCorrelationsMatrix.ToCSV(4, numInputs).Split(
                        split,
                        StringSplitOptions.RemoveEmptyEntries
                    ).Select(
                        (l, i) => outputNames[i] + "," + l
                    )
                ) + separator + "," + String.Join(",", outputNames) + new string(',', numInputs - numOutputs) + "\r\n" +
                String.Join(
                    "\r\n",
                    helper.OutputsCorrelationsMatrix.ToCSV(4, numInputs).Split(
                        split,
                        StringSplitOptions.RemoveEmptyEntries
                    ).Select(
                        (l, i) => outputNames[i] + "," + l
                    )
                ) + separator + 
                "All Inputs," + String.Join(
                    ",",
                    helper.MultipleCorrelations.Select(n => n.ToString(toStringFormat))
                )
            );
            Console.WriteLine("FINISHED");
            Console.ReadLine();

            var categories = categorical.Distinct().ToArray();
            Console.WriteLine("\t" + String.Join("\t", categories));
            for (int i = 0; i < outputNames.Length; ++i)
            {
                var dict = categorical.ZipToKeyValues(outputs[i]).GetPearsonCorrelation();
                Console.WriteLine(outputNames[i] + "\t" + categories.Select(c => dict[c].ToString("#.00")));
            }
            Console.WriteLine("~~~");

            double[] totals = new double[outputs.NumCols];
            for(int i = 0; i < inputNames.Length; ++i)
            {
                for (int o = 0; o < outputNames.Length; ++o)
                {
                    double correlation = inputs[i].Zip(
                        outputs[o],
                        (input, output) => new KeyValuePair<double, double>(input, output)
                    ).GetPearsonCorrelation();

                    totals[o] += Math.Abs(correlation);
                    Console.WriteLine(
                        inputNames[i] + " --> " + outputNames[o] + " \t: " + 
                        correlation.ToString("#.0000")
                    );
                }
            }
            Console.WriteLine("~~~");
            Console.WriteLine("\t" + String.Join("\t", inputNames));
            /*for (int i = 0; i < inputNames.Length; ++i)
            {
                string str = inputNames[i];
                for (int j = 0; j < inputNames.Length; ++j)
                {
                    str += "\t" + (
                        i == j ? 1.0 : inputs[i].GetCorrelationTo(inputs[j])
                    ).ToString("#.00");
                }
                Console.WriteLine(str);
                str = null;
            }*/
            Console.WriteLine("~~~");

            for(int i = 0; i < totals.Length; ++i)
            {
                Console.WriteLine(outputNames[i] + " : " + totals[i]);
            }
            Console.WriteLine("Finished");
            Console.ReadLine();

            DateTime test = DateTime.Now;
            Object obj = test;
            Console.WriteLine(obj.ToString());
            Console.ReadLine();

            /*Test test = Test.Boogity | Test.Woogity;
            bool hasByTheBeat = test.HasFlag(Test.ByTheBeat);
            bool hasWoogity = test.HasFlag(Test.Woogity);
            var flags = test.GetFlags();

            Console.WriteLine("DONE");
            Console.ReadLine();*/

            /*string domainName = Environment.UserDomainName;
            string userName = Environment.UserName;

            string text = PdfTextExtractor.GetTextFromPage(
                new PdfReader(
                    Directory.GetFiles(
                        @"C:\DocUploads", "SA2 - *"
                    ).First()
                ), 2, new SimpleTextExtractionStrategy()
            );*/

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
