using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Statistics
{
    public enum EncodingType
    {
        Binary,
        Categorical,
        CategoricalWithOtherOrUnknown,
        Numeric
    }

    public class MultiCorrelationHelper
    {
        private static string[] CONFIRMATIONS = new string[]
        {
            "true",
            "yes",
            "t",
            "1",
            "y"
        };

        private static string[] DECLINATIONS = new string[]
        {
            "false",
            "no",
            "f",
            "0",
            "n"
        };

        public Matrix InputsCorrelationsMatrix { get; protected set; }
        public Matrix InputsOutputsCorrelationsMatrix { get; protected set; }
        public Matrix OutputsCorrelationsMatrix { get; protected set; }

        /*public Matrix NormalizedInputs { get; protected set; }
        public EncodingType[] InputEncodings { get; protected set; }
        public Matrix NormalizedOutputs { get; protected set; }
        public EncodingType[] OutputEncodings { get; protected set; }

        private string[][] InputCategories { get; set; }
        private string[][] OutputCategories { get; set; }
        private int NumInputCategories { get; set; }
        private int NumOutputCategories { get; set; }*/

        public double[] MultipleCorrelations { get; protected set; }
        public int NumInputs { get; protected set; }
        public int NumOutputs { get; protected set; }
        public int NumRows { get; protected set; }

        public TempHelper Helper { get; protected set; }
        public struct TempHelper
        {
            public double[] InputSums { get; set; }
            public double[] InputSquaredSums { get; set; }
            public double[] InputMeans { get; set; }
            public double[] InputStdDevs { get; set; }

            public double[] OutputSums { get; set; }
            public double[] OutputSquaredSums { get; set; }
            public double[] OutputMeans { get; set; }
            public double[] OutputStdDevs { get; set; }
        }

        public MultiCorrelationHelper(Matrix inputsMatrix, Matrix outputsMatrix)
        {
                                                // In comments...
            NumInputs = inputsMatrix.NumCols;       // Represented by X
            NumOutputs = outputsMatrix.NumCols;     // Represented by Y
            NumRows = outputsMatrix.NumRows;        // Represented by N

            // Correlation between a dataset and itself will always be 1
            // This conveniently translates to the identity of the matrix making a perfect base
            InputsCorrelationsMatrix = Matrix.Identity(NumInputs);
            OutputsCorrelationsMatrix = Matrix.Identity(NumOutputs);

            InputsOutputsCorrelationsMatrix = new Matrix(NumOutputs, NumInputs);

            int jaggedNumInputs = NumInputs - 1;
            int jaggedNumOutputs = NumOutputs - 1;

            // The Helper is to give the user some of the values potentially used in other calculations
            // InputSums and InputSquaredSums technically aren't necessary after calculating meand and StdDev
            var Helper = new TempHelper();
            Helper.InputSums = new double[NumInputs];
            Helper.InputSquaredSums = new double[NumInputs];
            Helper.InputMeans = new double[NumInputs];
            Helper.InputStdDevs = new double[NumInputs];

            Helper.OutputSums = new double[NumOutputs];
            Helper.OutputSquaredSums = new double[NumOutputs];
            Helper.OutputMeans = new double[NumOutputs];
            Helper.OutputStdDevs = new double[NumOutputs];

            // Prep and fill the jagged arrays for sums between values
            // This is later used in the correlation calculations
            double[][] InputInputSums = new double[jaggedNumInputs][];
            double[][] OutputOutputSums = new double[jaggedNumOutputs][];
            double[][] InputOutputSums = new double[NumInputs][];

            for (int x = 0; x < jaggedNumInputs; ++x)
            {
                InputInputSums[x] = new double[jaggedNumInputs - x];
                InputOutputSums[x] = new double[NumOutputs];
            }
            InputOutputSums[jaggedNumInputs] = new double[NumOutputs];

            for (int y = 0; y < jaggedNumOutputs; ++y)
            {
                OutputOutputSums[y] = new double[jaggedNumOutputs - y];
            }

            // The rows of the dataset are iterated through only once
            // Given how many rows are typically in a dataset of this size, this is much more efficient
            for (int i = 0; i < NumRows; ++i)
            {
                // Setup to reduce number of read operations
                double[] inputs = inputsMatrix[i];
                double[] outputs = outputsMatrix[i];

                // Handle input sums, as well as the input * output sums
                for (int j = 0; j < jaggedNumInputs; ++j)
                {
                    double input = inputs[j];

                    Helper.InputSums[j] += input;
                    Helper.InputSquaredSums[j] += input * input;
                    for (int k = 0; k < NumOutputs; ++k)
                    {
                        InputOutputSums[j][k] += input * outputs[k];
                    }

                    // Efficiently fill in triangular matrix
                    int index = 0;
                    for (int k = jaggedNumInputs; k > j; --k)
                    {
                        InputInputSums[j][index] += input * inputs[k];
                        ++index;
                    }
                }

                // The last input has no work in the triangular matrix
                // To avoid excessive boolean operators, that input does their processing here
                double jaggedInput = inputs[jaggedNumInputs];
                Helper.InputSums[jaggedNumInputs] += jaggedInput;
                Helper.InputSquaredSums[jaggedNumInputs] += jaggedInput * jaggedInput;
                for (int k = 0; k < NumOutputs; ++k)
                {
                    InputOutputSums[jaggedNumInputs][k] += jaggedInput * outputs[k];
                }

                // Handle output sums
                for (int j = 0; j < jaggedNumOutputs; ++j)
                {
                    double output = outputs[j];

                    Helper.OutputSums[j] += output;
                    Helper.OutputSquaredSums[j] += output * output;

                    int index = 0;
                    for (int k = jaggedNumOutputs; k > j; --k)
                    {
                        OutputOutputSums[j][index] += output * outputs[k];
                        ++index;
                    }
                }

                // Same as above, with jaggedInput
                double jaggedOutput = outputs[jaggedNumOutputs];
                Helper.OutputSums[jaggedNumOutputs] += jaggedOutput;
                Helper.OutputSquaredSums[jaggedNumOutputs] += jaggedOutput * jaggedOutput;

                inputs = null;
                outputs = null;
            }

            int sampleNumRows = NumRows - 1;
            // Get correlations between inputs and other inputs, as well as inputs and outputs
            for (int j = 0; j < NumInputs; ++j)
            {
                // Pre-fetch array values to avoid excessive read operations
                var sums = InputOutputSums[j];
                var sum = Helper.InputSums[j];
                var squaredSum = Helper.InputSquaredSums[j];

                // Calculate and store standard deviation
                // Not entirely necessary, but could be useful to the user
                // Normalization and graph-checking the correlation both come to mind
                double mean = sum / NumRows;
                Helper.InputMeans[j] = mean;
                Helper.InputStdDevs[j] = StdDev(sampleNumRows, mean, squaredSum);

                // Calculate correlation between each input and each output
                for (int k = 0; k < NumOutputs; ++k)
                {
                    InputsOutputsCorrelationsMatrix[k, j] = StatisticsExtensions.PearsonCorrelationFormula(
                        NumRows,
                        sum,
                        Helper.OutputSums[k],
                        squaredSum,
                        Helper.OutputSquaredSums[k],
                        sums[k]
                    );
                }

                // Here we use a boolean simply because of all the excess code above
                // Also, the number of boolean operations is X, while above the number would be N * X
                if (j != jaggedNumInputs)
                {
                    // Calculate the correlation between inputs, for Multiple Correlation
                    // Using the triangular matrix code, so that it is an extra write operation instead of an extra calculation
                    var inputSums = InputInputSums[j];
                    int index = 0;
                    for (int k = jaggedNumInputs; k > j; --k)
                    {
                        double correlation = StatisticsExtensions.PearsonCorrelationFormula(
                            NumRows,
                            sum,
                            Helper.InputSums[k],
                            squaredSum,
                            Helper.InputSquaredSums[k],
                            inputSums[index]
                        );
                        InputsCorrelationsMatrix[j, k] = correlation;
                        InputsCorrelationsMatrix[k, j] = correlation;
                        ++index;
                    }
                }
            }

            // Prep for calculating Multiple Correlation
            MultipleCorrelations = new double[NumOutputs];
            var invertedInputs = InputsCorrelationsMatrix.Invert();

            // Get correlations between outputs and other outputs
            // Technically not a use for these correlations
            // Simply might be interesting to see, while we have the values
            for (int j = 0; j < NumOutputs; ++j)
            {
                var sum = Helper.OutputSums[j];
                var squaredSum = Helper.OutputSquaredSums[j];

                double mean = sum / NumRows;
                Helper.OutputMeans[j] = mean;
                Helper.OutputStdDevs[j] = StdDev(NumRows, mean, squaredSum);

                if (j != jaggedNumOutputs)
                {
                    var outputSums = OutputOutputSums[j];
                    int index = 0;
                    for (int k = jaggedNumOutputs; k > j; --k)
                    {
                        double correlation = StatisticsExtensions.PearsonCorrelationFormula(
                            NumRows,
                            sum,
                            Helper.OutputSums[k],
                            squaredSum,
                            Helper.OutputSquaredSums[k],
                            outputSums[index]
                        );
                        OutputsCorrelationsMatrix[j, k] = correlation;
                        OutputsCorrelationsMatrix[k, j] = correlation;
                        ++index;
                    }

                    outputSums = null;
                }
                
                var inputCorrelations = InputsOutputsCorrelationsMatrix[j];
                MultipleCorrelations[j] = Math.Sqrt(invertedInputs.Dot(inputCorrelations).Dot(inputCorrelations));
                inputCorrelations = null;
            }
            invertedInputs = null;
        }

        #region ToString()
        public override string ToString() => ToString(true);

        public string ToString(int numDecimalPlaces) => ToString(true, true, numDecimalPlaces);

        public string ToString(bool includeInputs) => ToString(includeInputs, 2);

        public string ToString(bool includeInputs, int numDecimalPlaces) => ToString(includeInputs, true, numDecimalPlaces);

        public string ToString(bool includeInputs, bool includeOutputs) => ToString(includeInputs, includeInputs, 2);

        public string ToString(bool includeInputs, bool includeOutputs, int numDecimalPlaces)
        {
            bool decimals = numDecimalPlaces > 0;
            string decimalsString = decimals ? "#." + new string('0', numDecimalPlaces) : "";
            return (includeInputs ? " ~~ Input-Input Correlations:\r\n" + InputsCorrelationsMatrix.ToString(numDecimalPlaces) + "\r\n\r\n" : "") +
                   (includeOutputs ? " ~~ Output-Output Correlations:\r\n" + OutputsCorrelationsMatrix.ToString(numDecimalPlaces) + "\r\n\r\n" : "") +
                   " ~~ Input-Output Correlations:\r\n" + InputsOutputsCorrelationsMatrix.ToString(numDecimalPlaces) + "\r\n\r\n" +
                   " ~~ Multiple Correlations:\r\n[" + String.Join(
                       new string('\t', Math.Max(1, numDecimalPlaces / 2)),
                       MultipleCorrelations.Select(
                           c => decimals ? c.ToString(decimalsString) : Convert.ToInt32(c).ToString()
                       )
                   ) + "]";
        }
        #endregion

        #region ToCSV
        public string ToCSV(string[] inputNames, string[] outputNames, int numDecimalPlaces)
        {
            string toStringFormat = numDecimalPlaces > 0 ? "#." + new string('0', numDecimalPlaces) : "#";
            StringBuilder inputsHeader1 = new StringBuilder(150, 300);
            StringBuilder inputsHeader2 = new StringBuilder(150, 300);
            StringBuilder sbInputsInputs = new StringBuilder(1500, 3000);
            StringBuilder sbInputsOutputs = new StringBuilder(250, 500);
            StringBuilder outputsHeader1 = new StringBuilder(75, 150);
            StringBuilder outputsHeader2 = new StringBuilder(75, 150);
            StringBuilder sbOutputsOutputs = new StringBuilder(125, 250);
            StringBuilder sbMultiCorrelations = new StringBuilder(25, 50);
            int jaggedInputs = NumInputs - 1;
            int jaggedOutputs = NumOutputs - 1;
            sbMultiCorrelations.Append("All Inputs");

            for (int j = 0; j < NumInputs; ++j)
            {
                var name = inputNames[j];
                if (j != 0)
                    inputsHeader1.Insert(0, ',' + name);
                inputsHeader2.Append(',');
                inputsHeader2.Append(name);
                
                if (j != jaggedInputs)
                {
                    sbInputsInputs.Append(name);
                    for (int k = jaggedInputs; k > j; --k)
                    {
                        sbInputsInputs.Append(',');
                        sbInputsInputs.Append(InputsCorrelationsMatrix[j, k].ToString(toStringFormat));
                    }
                    sbInputsInputs.AppendLine();
                }
            }
            inputsHeader1.AppendLine();
            inputsHeader2.AppendLine();

            for (int j = 0; j < NumOutputs; ++j)
            {
                sbMultiCorrelations.Append(',');
                sbMultiCorrelations.Append(MultipleCorrelations[j].ToString(toStringFormat));

                var name = outputNames[j];
                if (j != 0)
                    outputsHeader1.Insert(0, ',' + name);
                outputsHeader2.Append(',');
                outputsHeader2.Append(name);

                if (j != jaggedOutputs)
                {
                    sbOutputsOutputs.Append(name);
                    for (int k = jaggedOutputs; k > j; --k)
                    {
                        sbOutputsOutputs.Append(',');
                        sbOutputsOutputs.Append(OutputsCorrelationsMatrix[j, k].ToString(toStringFormat));
                    }
                    sbOutputsOutputs.AppendLine();
                }

                sbInputsOutputs.Append(name);
                for (int k = 0; k < NumInputs; ++k)
                {
                    sbInputsOutputs.Append(',');
                    sbInputsOutputs.Append(InputsOutputsCorrelationsMatrix[j, k].ToString(toStringFormat));
                }
                sbInputsOutputs.AppendLine();
            }
            outputsHeader1.AppendLine();
            outputsHeader2.AppendLine();

            return inputsHeader1.ToString() +
                sbInputsInputs.ToString() + "\r\n" +
                inputsHeader2.ToString() +
                sbInputsOutputs.ToString() + "\r\n" +
                outputsHeader1.ToString() +
                sbOutputsOutputs.ToString() + "\r\n" +
                outputsHeader2.ToString() +
                sbMultiCorrelations.ToString();

        }
        #endregion

        /*
        public MultiCorrelationHelper(object[][] inputsMatrix, object[][] outputsMatrix, EncodingType[] inputEncodings, EncodingType[] outputEncodings)
        {
            NumInputs = inputsMatrix[0].Length;
            NumOutputs = outputsMatrix[0].Length;
            NumRows = outputsMatrix.Length;

            NumInputCategories = inputEncodings.Count(i => i != EncodingType.Numeric);
            NumOutputCategories = outputEncodings.Count(o => o != EncodingType.Numeric);
            int category = 0;

            InputCategories = new string[NumInputCategories][];
            OutputCategories = new string[NumOutputCategories][];

            if (NumInputCategories > 0)
            {
                for (int j = 0; j < NumInputs; ++j)
                {
                    switch (inputEncodings[j])
                    {
                        case EncodingType.Binary:
                            string cat1 = (string)inputsMatrix[0][j];
                            string cat2 = null;
                            int i = 1;

                            do
                            {
                                cat2 = (string)inputsMatrix[i][j];
                                ++i;
                            } while (cat2 == cat1);

                            InputCategories[category] = new string[2];
                            string cat1Temp = cat1.ToLower().Trim();
                            string cat2Temp = cat2.ToLower().Trim();

                            if (CONFIRMATIONS.Contains(cat1Temp) && DECLINATIONS.Contains(cat2Temp))
                            {
                                InputCategories[category][1] = cat1Temp;
                                InputCategories[category][0] = cat2Temp;
                            }
                            else
                            {
                                InputCategories[category][0] = cat1Temp;
                                InputCategories[category][1] = cat2Temp;
                            }

                            ++category;
                            break;

                        case EncodingType.Categorical:
                            var categories = inputsMatrix.GetCol(j).Cast<string>().Distinct().ToArray();
                            Array.Sort(categories);

                            break;
                    }
                }
            }
            

            var names = categoricalInputs.Distinct().ToArray();
            Array.Sort(names);
            int numCategories = names.Length;

            if (String.IsNullOrWhiteSpace(leastInterestCategory))
            {
                --numCategories;
                names = names.Where(n => n != leastInterestCategory).ToArray();
                Array.Sort(names);
                
            }

            for (int i = 0; i < count; ++i)
                {
                    string name = names[i];
                    if (name == leastInterestName)
                        mat[i] = leastInterestEncoded;
                    else
                        mat[i, Array.BinarySearch(names, name)] = 1.0;
                }
                Matrix mat = new Matrix(count, distinct);
                for (int i = 0; i < count; ++i)
                {
                    mat[i, Array.BinarySearch(names, xValues[i])] = 1.0;
                }

            double[] leastInterestEncoded = Enumerable.Repeat<double>(-1.0, numCategories).ToArray();

            NumInputs = inputsMatrix.NumCols + numCategories;
            NumOutputs = outputsMatrix.NumCols;
            NumRows = outputsMatrix.NumRows;

            InputsCorrelationsMatrix = Matrix.Identity(NumInputs);
            InputsOutputsCorrelationsMatrix = new Matrix(NumOutputs, NumInputs);
            OutputsCorrelationsMatrix = Matrix.Identity(NumOutputs);

            double[] InputSums = new double[NumInputs];
            double[] InputSquaredSums = new double[NumInputs];
            double[] OutputSums = new double[NumOutputs];
            double[] OutputSquaredSums = new double[NumOutputs];
            double[][] InputInputSums = new double[NumInputs - 1][];
            double[][] OutputOutputSums = new double[NumOutputs - 1][];
            Matrix InputOutputSums = new Matrix(NumInputs, NumOutputs);

            int jaggedNumInputs = NumInputs - 1;
            int jaggedNumOutputs = NumOutputs - 1;

            for (int j = 0; j < jaggedNumInputs; ++j)
            {
                InputInputSums[j] = new double[jaggedNumInputs - j];
            }

            for (int j = 0; j < jaggedNumOutputs; ++j)
            {
                OutputOutputSums[j] = new double[jaggedNumOutputs - j];
            }

            for (int i = 0; i < NumRows; ++i)
            {
                double[] inputs = inputsMatrix[i];
                double[] outputs = outputsMatrix[i];

                for (int j = 0; j < NumInputs; ++j)
                {
                    double input = inputs[j];

                    InputSums[j] += input;
                    InputSquaredSums[j] += Math.Pow(input, 2.0);
                    for (int k = 0; k < NumOutputs; ++k)
                    {
                        InputOutputSums[j][k] += input * outputs[k];
                    }

                    if (j != jaggedNumInputs)
                    {
                        for (int k = j + 1; k < NumInputs; ++k)
                        {
                            InputInputSums[j][k - 1] += input * inputs[k];
                        }
                    }
                }

                for (int j = 0; j < NumOutputs; ++j)
                {
                    double output = outputs[j];

                    OutputSums[j] += output;
                    OutputSquaredSums[j] += Math.Pow(output, 2.0);

                    if (j != jaggedNumOutputs)
                    {
                        for (int k = j + 1; k < NumOutputs; ++k)
                        {
                            OutputOutputSums[j][k - 1] += output * outputs[k];
                        }
                    }
                }

                inputs = null;
                outputs = null;
            }

            for (int j = 0; j < NumInputs; ++j)
            {
                var sums = InputOutputSums[j];
                var sum = InputSums[j];
                var squaredSum = InputSquaredSums[j];
                for (int k = 0; k < NumOutputs; ++k)
                {
                    InputsOutputsCorrelationsMatrix[k, j] = StatisticsExtensions.PearsonCorrelationFormula(
                        NumRows,
                        sum,
                        OutputSums[k],
                        squaredSum,
                        OutputSquaredSums[k],
                        sums[k]
                    );
                }

                if (j != jaggedNumInputs)
                {
                    var inputSums = InputInputSums[j];
                    for (int k = j + 1; k < NumInputs; ++k)
                    {
                        double correlation = StatisticsExtensions.PearsonCorrelationFormula(
                            NumRows,
                            sum,
                            InputSums[k],
                            squaredSum,
                            InputSquaredSums[k],
                            inputSums[k - 1]
                        );
                        InputsCorrelationsMatrix[j, k] = correlation;
                        InputsCorrelationsMatrix[k, j] = correlation;
                    }
                }
            }

            MultipleCorrelations = new double[NumOutputs];
            var invertedInputs = InputsCorrelationsMatrix.Invert();
            for (int j = 0; j < NumOutputs; ++j)
            {
                if (j != jaggedNumOutputs)
                {
                    var sum = OutputSums[j];
                    var squaredSum = OutputSquaredSums[j];
                    var outputSums = OutputOutputSums[j];

                    for (int k = j + 1; k < NumOutputs; ++k)
                    {
                        double correlation = StatisticsExtensions.PearsonCorrelationFormula(
                            NumRows,
                            sum,
                            OutputSums[k],
                            squaredSum,
                            OutputSquaredSums[k],
                            outputSums[k - 1]
                        );
                        OutputsCorrelationsMatrix[j, k] = correlation;
                        OutputsCorrelationsMatrix[k, j] = correlation;
                    }

                    outputSums = null;
                }

                var inputCorrelations = InputsOutputsCorrelationsMatrix[j];
                MultipleCorrelations[j] = invertedInputs.Dot(inputCorrelations).Dot(inputCorrelations);
                inputCorrelations = null;
            }
            invertedInputs = null;
        }

        public object[] DenormalizeOutputs(double[] outputs)
        {
            object[] denormalized = new object[NumOutputs];
            int category = 0;
            int column = 0;
            for (int j = 0; j < NumOutputs; ++j)
            {
                switch (OutputEncodings[j])
                {
                    case EncodingType.Binary:
                        denormalized[j] = outputs[column] >= 0.5 ? OutputCategories[category][1] : OutputCategories[category][0];
                        ++category;
                        ++column;
                        break;

                    case EncodingType.Categorical:
                        string[] categories = OutputCategories[category];
                        int numCategories = categories.Length;
                        int maxIndex = 0;
                        double curMax = 0;
                        for (int c = 0; c < numCategories; ++c)
                        {
                            double cur = outputs[column + c];
                            if (cur > curMax)
                            {
                                maxIndex = c;
                                curMax = cur;
                            }
                        }
                        denormalized[j] = categories[maxIndex];
                        column += numCategories;
                        ++category;
                        break;

                    case EncodingType.Numeric:
                        denormalized[j] = (outputs[column] * Helper.OutputStdDevs[j]) + Helper.OutputMeans[j];
                        ++column;
                        break;
                }
            }
            return denormalized;
        }
        */

        private static double StdDev(double N, double mean, double squaredSum) => Math.Sqrt((squaredSum - N * mean * mean) / (N - 1));
    }
}
