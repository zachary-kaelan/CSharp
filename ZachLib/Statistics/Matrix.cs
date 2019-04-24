using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Jil;

namespace ZachLib.Statistics
{
    public class Matrix : IEnumerable<double>, IEquatable<Matrix>, ICloneable
    {
        public int NumRows { get; protected set; }
        public int NumCols { get; protected set; }
        public int Count { get; protected set; }
        public double[] _compactedMatrix { get; protected set; }

        public double this[int i, int j]
        {
            get => _compactedMatrix[i * NumCols + j];
            set => _compactedMatrix[i * NumCols + j] = value;
        }

        public double[] this[int i]
        {
            get
            {
                double[] row = new double[NumCols];
                int start = i * NumCols;
                for (int j = 0; j < NumCols; ++j)
                {
                    row[j] = _compactedMatrix[start + j];
                }
                return row;
            }
            set
            {
                if (value.Length != NumCols)
                    throw new IncorrectMatrixSizeException("Vector size needs to be the same as the number of columns in the matrix.");
                int max = (i + 1) * NumCols;
                int index = 0;
                for (int j = i * NumCols; j < max; ++j)
                {
                    _compactedMatrix[j] = value[index];
                    ++index;
                }
            }
        }

        #region Initialization
        public Matrix(int n) : this(n, n) { }

        public Matrix(int rows, int cols)
        {
            NumRows = rows;
            NumCols = cols;
            Count = rows * cols;
            _compactedMatrix = new double[Count];
        }

        public Matrix(double[] compacted, int cols)
        {
            NumCols = cols;
            Count = compacted.Length;
            NumRows = Count / NumCols;
            _compactedMatrix = compacted;
        }

        public Matrix(double[][] mat)
        {
            NumRows = mat.Length;
            NumCols = mat[0].Length;
            Count = NumRows * NumCols;
            _compactedMatrix = new double[Count];
            for (int i = 0; i < NumRows; ++i)
            {
                for (int j = 0; j < NumCols; ++j)
                {
                    _compactedMatrix[i * NumCols + j] = mat[i][j];
                }
            }
        }

        public Matrix(double[][] mat, int rowsCapacity, int colsCapacity)
        {
            NumRows = rowsCapacity;
            NumCols = colsCapacity;
            Count = NumRows * NumCols;
            _compactedMatrix = new double[Count];

            int rows = mat.Length;
            for (int i = 0; i < rows; ++i)
            {
                var row = mat[i];
                int cols = row.Length;
                for (int j = 0; j < cols; ++j)
                {
                    _compactedMatrix[i * NumCols + j] = mat[i][j];
                }
            }
        }

        public Matrix(double[] compacted, int numCols, int rowsCapacity, int colsCapacity)
        {
            NumRows = rowsCapacity;
            NumCols = colsCapacity;
            Count = NumRows * NumCols;
            _compactedMatrix = new double[Count];

            int count = compacted.Length;
            int rows = count % numCols;
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < numCols; ++j)
                {
                    _compactedMatrix[i * NumCols + j] = compacted[i * numCols + j];
                }
            }
        }

        public static Matrix Identity(int n)
        {
            Matrix mat = new Matrix(n);
            for (int i = 0; i < n; ++i)
            {
                mat[i, i] = 1.0;
            }
            return mat;
        }

        public static Matrix GenerateRandom(int rows, int cols)
        {
            return GenerateRandom(rows, cols, -1.0, 1.0);
        }

        public static Matrix GenerateRandom(int rows, int cols, double minVal, double maxVal)
        {
            var range = maxVal - minVal;
            return new Matrix(rows, cols).ElementwiseChange(
                n => range * Utils.RANDOM.NextDouble() + minVal
            );
        }

        public static Matrix GenerateRandom(int rows, int cols, Func<double> generator) => new Matrix(rows, cols).ElementwiseChange(e => generator());
        #endregion

        #region Helpers
        public Matrix ElementwiseChange(Func<double, double> op)
        {
            Matrix mat = new Matrix(NumRows, NumCols);
            for (int e = 0; e < Count; ++e)
            {
                mat._compactedMatrix[e] = op(_compactedMatrix[e]);
            }
            return mat;
        }

        public Matrix ElementWiseOperation(Matrix other, Func<double, double, double> op)
        {
            if (NumRows != other.NumRows || NumCols != other.NumCols)
                throw new IncorrectMatrixSizeException(String.Format("Matrix A is {0}x{1} and Matrix B is {2}x{3}, when they should be the same size.", NumRows, NumCols, other.NumRows, other.NumCols));
            Matrix mat = new Matrix(NumRows, NumCols);
            for (int e = 0; e < Count; ++e)
            {
                mat._compactedMatrix[e] = op(_compactedMatrix[e], other._compactedMatrix[e]);
            }
            return mat;
        }

        public double GetElement(int index) => _compactedMatrix[index];

        public void SetElement(int index, double value) => _compactedMatrix[index] = value;
        #endregion

        #region Operators
        #region NonMatrix
        public static Matrix operator +(Matrix mat1, double num) => mat1.ElementwiseChange(e => e + num);

        public static Matrix operator -(Matrix mat1, double num) => mat1.ElementwiseChange(e => e - num);

        public static Matrix operator *(Matrix mat1, double num) => mat1.ElementwiseChange(e => e * num);
        #endregion

        public static Matrix operator -(Matrix mat1, Matrix mat2) => mat1.ElementWiseOperation(mat2, (e1, e2) => e1 - e2);

        public static Matrix operator +(Matrix mat1, Matrix mat2) => mat1.ElementWiseOperation(mat2, (e1, e2) => e1 + e2);

        public static Matrix operator *(Matrix mat1, Matrix mat2) => mat1.ElementWiseOperation(mat2, (e1, e2) => e1 * e2);
        #endregion

        #region Matrix Functions
        #region Dot
        /*public unsafe Matrix TiledDot(Matrix mat2)
        {
            if (NumCols != mat2.NumRows)
                throw new IncorrectMatrixSizeException(String.Format("Matrix A has {0} columns and Matrix B has {1} rows, when they should be equal.", NumCols, mat2.NumRows));
            Matrix output = new Matrix(NumRows, mat2.NumCols);

            int ib = 20;
            int kb = 20;
            double acc00 = 0;
            double acc01 = 0;
            double acc10 = 0;
            double acc11 = 0;
            for (int ii = 0; ii < NumRows; ii += ib)
            {
                for (int kk = 1; kk < NumRows; kk += kb)
                {
                    for (int j = 0; j < NumRows; j += 2)
                    {
                        int mat1Index = 0;
                        for (int i = ii; i < ii + ib; i += 2)
                        {
                            for (int k = kk; k < kk + kb; ++k)
                            {
                                double temp10 = _compactedMatrix[mat1Index + j];
                                double temp11 = _compactedMatrix[mat1Index + NumCols + j];
                                double temp20 = mat2[k, j];
                                double temp21 = mat2[k, j + 1];
                                acc00 += temp10 * temp20;
                                acc01 += temp10 * temp21;
                                acc10 += temp11 * temp20;
                                acc11 += temp11 * temp21;
                            }
                            output._compactedMatrix[mat1Index + j] += acc00;
                            output._compactedMatrix[mat1Index + j + 1] += acc01;
                            mat1Index += NumCols;
                            output._compactedMatrix[mat1Index + j] += acc10;
                            output._compactedMatrix[mat1Index + j + 1] += acc11;

                            acc00 = 0;
                            acc01 = 0;
                            acc10 = 0;
                            acc11 = 0;
                        }
                    }
                }
            }
            return output;
        }*/

        public Matrix Dot(Matrix mat2)
        {
            if (NumCols != mat2.NumRows)
                throw new IncorrectMatrixSizeException(String.Format("Matrix A has {0} columns and Matrix B has {1} rows, when they should be equal.", NumCols, mat2.NumRows));
            Matrix output = new Matrix(NumRows, mat2.NumCols);
            int mat1Index = 0;
            for (int i = 0; i < NumRows; ++i)
            {
                for (int j = 0; j < mat2.NumCols; ++j)
                {
                    double acc = 0;
                    for (int k = 0; k < NumCols; ++k)
                    {
                        acc += _compactedMatrix[mat1Index + k] * mat2[k, j];
                    }
                    output[i, j] = acc;
                }
                mat1Index += NumCols;
            }
            return output;
        }

        public Vector Dot(Vector vector)
        {
            int vectorLength = vector.Count;

            if (NumCols == vectorLength)
            {
                double[] vectorProduct = new double[vectorLength];
                for (int e = 0; e < Count; ++e)
                {
                    int index = e % vectorLength;
                    vectorProduct[index] += _compactedMatrix[e] * vector[index];
                }
                return vectorProduct;
            }
            else
                throw new IncorrectMatrixSizeException(String.Format("Matrix has {0} columns, and Vector is size {1}. The two need to be equal.", NumCols, vectorLength));    
        }
        #endregion

        public bool CheckSymmetry()
        {
            bool symmetrical = true;
            int rowStart = 0;
            for (int i = 0; symmetrical && (i < NumRows); ++i)
            {
                for (int j = 0; symmetrical && (j < NumCols); ++j)
                {
                    symmetrical = (_compactedMatrix[rowStart + j] == this[j][i]);
                }
            }
            return symmetrical;
        }

        public Matrix T()
        {
            Matrix output = new Matrix(NumCols, NumRows);
            for (int i = 0; i < NumRows; ++i)
            {
                for (int j = 0; j < NumCols; ++j)
                {
                    output[j, i] = this[i, j];
                }
            }
            return output;
        }

        public Matrix Decompose(out int[] perm, out int toggle)
        {
            if (NumRows != NumCols)
                throw new IncorrectMatrixSizeException("Trying to decompose a non-square matrix.");

            Matrix output = Duplicate();
            var permTemp = new int[NumRows];

            for (int i = 0; i < NumRows; ++i)
            {
                permTemp[i] = i;
            }

            int toggleTemp = 1; 
            // toggle tracks row swaps
            // +1 - greater-than even
            // -1 - greater-than odd

            void RowSwap(int row, int col)
            {
                double[] rowPtr = output[row];
                output[row] = output[col];
                output[col] = rowPtr;

                // swap perm info
                int tmp = permTemp[row];
                permTemp[row] = permTemp[col];
                permTemp[row] = tmp;

                // adjust the row-swap toggle
                toggleTemp = -toggleTemp;
            }

            for (int j = 0; j < NumRows - 1; ++j)
            {
                double colMax = Math.Abs(output[j, j]);
                int pRow = j;

                for (int i = j + 1; i < NumRows; ++i)
                {
                    double num = Math.Abs(output[i, j]);
                    if (num > colMax)
                    {
                        colMax = num;
                        pRow = i;
                    }
                }

                // if largest value not on pivot, swap rows
                if (pRow != j)
                    RowSwap(pRow, j);

                if (output[j][j] == 0.0)
                {
                    // find a good row to swap
                    int goodRow = -1;
                    for (int i = j + 1; i < NumRows; ++i)
                    {
                        if (output[i][j] != 0.0)
                            goodRow = i;
                    }

                    if (goodRow == -1)
                        throw new MatrixInoperableException("No good row, can't use Doolittle's method.");

                    // swap rows so 0.0 no longer on diagonal
                    RowSwap(goodRow, j);
                }

                for (int i = j + 1; i < NumRows; ++i)
                {
                    output[i][j] /= output[j][j];
                    for (int k = j + 1; k < NumRows; ++k)
                    {
                        output[i][k] -= output[i][j] * output[j][k];
                    }
                }
            }

            perm = permTemp;
            toggle = toggleTemp;
            return output;
        }

        public Matrix Invert()
        {
            if (NumRows != NumCols)
                throw new IncorrectMatrixSizeException("Trying to invert a non-square matrix.");

            // Cramer's rule
            if (NumRows == 2)
                return new Matrix(
                    new double[][]
                    {
                         new double[]
                         {
                             _compactedMatrix[3],
                             -_compactedMatrix[1]
                         }, new double[]
                         {
                             -_compactedMatrix[2],
                             _compactedMatrix[0]
                         }
                    }
                ) * (1.0 / Determinant());
            else if (NumRows == 3)
            {
                double a = _compactedMatrix[0];
                double b = _compactedMatrix[1];
                double c = _compactedMatrix[2];
                double d = _compactedMatrix[3];
                double e = _compactedMatrix[4];
                double f = _compactedMatrix[5];
                double g = _compactedMatrix[6];
                double h = _compactedMatrix[7];
                double i = _compactedMatrix[8];

                double A = (e * i) - (f * h);
                double B = -1 * ((d * i) - (f * g));
                double C = (d * h) - (e * g);

                // rule of Sarrus
                double determinant = (a * A) + (b * B) + (c * C);

                return new Matrix(
                    new double[][]
                    {
                        new double[]
                        {
                            A,
                            -((b * i) - (c * h)),
                            (b * f) - (c * e)
                        }, new double[]
                        {
                            B,
                            (a * i) - (c * g),
                            -((a * f) - (c * d))
                        }, new double[]
                        {
                            C,
                            -((a * h) - (b * g)),
                            (a * e) - (b * d)
                        }
                    }
                ) * (1.0 / determinant);
            }

            Matrix lum = null;
            int[] perm = null;
            int toggle = 1;
            try
            {
                lum = Decompose(out perm, out toggle);
            }
            catch
            {
                throw new MatrixInoperableException("Matrix cannot be inverted using supported methods.");
            }

            Matrix output = Duplicate();
            double[] bTemp = new double[NumRows];
            for (int i = 0; i < NumRows; ++i)
            {
                for (int j = 0; j < NumCols; ++j)
                {
                    if (i == perm[j])
                        bTemp[j] = 1.0;
                    else
                        bTemp[j] = 0.0;
                }

                double[] x = lum.HelperSolve(bTemp);

                for (int j = 0; j < NumRows; ++j)
                {
                    output[j][i] = x[j];
                }
            }

            return output;
        }

        public double Determinant()
        {
            if (NumRows != NumRows)
                throw new MatrixInoperableException("Cannot get determinant of non-square matrix.");
            if (NumRows == 2)
                return (_compactedMatrix[0] * _compactedMatrix[3]) -
                       (_compactedMatrix[1] * _compactedMatrix[2]);
            else if (NumRows == 3)
                return (_compactedMatrix[0] * _compactedMatrix[4] * _compactedMatrix[8]) +
                       (_compactedMatrix[1] * _compactedMatrix[5] * _compactedMatrix[6]) +
                       (_compactedMatrix[2] * _compactedMatrix[3] * _compactedMatrix[7]) -
                       (_compactedMatrix[2] * _compactedMatrix[4] * _compactedMatrix[6]) -
                       (_compactedMatrix[1] * _compactedMatrix[3] * _compactedMatrix[8]) -
                       (_compactedMatrix[0] * _compactedMatrix[5] * _compactedMatrix[7]);

            int[] perm = null;
            int toggle = 1;
            Matrix lum = Decompose(out perm, out toggle);
            if (lum == null)
                throw new MatrixInoperableException("Unable to compute determinant");
            double result = toggle;
            for (int i = 0; i < lum.NumRows; ++i)
            {
                result *= lum[i][i];
            }
            return result;
        }

        private double[] HelperSolve(double[] b)
        {
            // before calling, permute b using the perm array
            double[] output = new double[NumRows];
            b.CopyTo(output, 0);
            
            for (int i = 1; i < NumRows; ++i)
            {
                double sum = output[i];
                for (int j = 0; j < i; ++j)
                {
                    sum -= this[i,j] * output[j];
                }
                output[i] = sum / this[i,i];
            }

            return output;
        }

        private double[] SystemSolve(double[] b)
        {
            // 1. decompose A
            int[] perm = null;
            int toggle = 1;
            Matrix lum = Decompose(out perm, out toggle);
            if (lum == null)
                return null;

            // 2. permute b according to perm[] into bp
            double[] bp = new double[b.Length];
            for (int i = 0; i < NumRows; ++i)
            {
                bp[i] = b[perm[i]];
            }

            // 3. call helper
            return lum.HelperSolve(bp);
        }
        #endregion

        #region StatisticsFunctions
        public double[] GetColumnMeans()
        {
            double[] sums = new double[NumCols];
            int rowStart = 0;
            for (int i = 0; i < NumRows; ++i)
            {
                for (int j = 0; j < NumCols; ++j)
                {
                    sums[j] += _compactedMatrix[rowStart + j];
                }
                rowStart += NumCols;
            }
            return sums.Select(s => s / NumRows).ToArray();
        }

        public Matrix GetCovarianceMatrix()
        {
            Matrix covarMat = new Matrix(NumCols);
            Vector[] vectors = Enumerable.Range(0, NumCols).Select(j => new Vector(GetColumn(j))).ToArray();
            for (int j = 0; j < NumCols - 1; ++j)
            {
                Vector vec = GetColumn(j);
                covarMat[j, j] = vec.Variance();
                for (int k = j + 1; k < NumCols; ++k)
                {
                    double covariance = vec.ElementWiseAggregate(vectors[k], (sum, jVal, kVal) => sum + (jVal * kVal)) / Count;
                    covarMat[j, k] = covariance;
                    covarMat[k, j] = covariance;
                }
            }
            covarMat._compactedMatrix[covarMat.Count - 1] = vectors.Last().Variance();
            return covarMat;
        }

        public EigenvalueDecomposition PCA()
        {
            var means = GetColumnMeans();

            Matrix normalized = new Matrix(_compactedMatrix, NumCols);
            int rowStart = 0;
            for (int i = 0; i < NumRows; ++i)
            {
                for (int j = 0; j < NumCols; ++j)
                {
                    normalized._compactedMatrix[rowStart + j] -= means[j];
                }
                rowStart += NumCols;
            }

            Matrix covarMat = normalized.GetCovarianceMatrix();
            return new EigenvalueDecomposition(covarMat, true);
            /*return eigen.RealEigenvalues.Select(
                (v, i) => new KeyValuePair<int, double>(i, v)
            ).Zip(
                eigen.GetV(),
                (v, ev) => new Tuple<int, double, Vector>(v.Key, v.Value, ev)
            ).OrderByDescending(e => e.Item2).ToArray();*/
        }
        #endregion

        #region Interface Implementations
        public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)_compactedMatrix).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<double>)_compactedMatrix).GetEnumerator();

        public bool Equals(Matrix other) => Equals(other, 0.0);

        public bool Equals(Matrix other, double epsilon)
        {
            if (other == null || other.NumRows != NumRows || other.NumCols != NumCols)
                return false;

            for (int e = 0; e < Count; ++e)
            {
                if (Math.Abs(_compactedMatrix[e] - other._compactedMatrix[e]) > epsilon)
                    return false;
            }

            return true;
        }

        public object Clone() => new Matrix(_compactedMatrix, NumCols);
        #endregion

        public Matrix Duplicate() => new Matrix((double[])_compactedMatrix.Clone(), NumCols);

        public double[][] ToArray()
        {
            double[][] output = new double[NumRows][];
            for (int i = 0; i < NumRows; ++i)
            {
                output[i] = new double[NumCols];
                for (int j = 0; j < NumCols; ++j)
                {
                    output[i][j] = this[i, j];
                }
            }
            return output;
        }

        public void AddRow(double[] row)
        {
            _compactedMatrix = _compactedMatrix.Concat(row).ToArray();
            ++NumRows;
            Count += NumCols;
        }

        #region Resize
        public void Resize(int newRowCapacity)
        {
            if (newRowCapacity == 0)
                return;
            else if (newRowCapacity - NumRows == 1)
            {
                AddRow(new double[NumCols]);
                return;
            }
            int rowsChange = newRowCapacity - NumRows;
            NumRows += rowsChange;
            Count += NumCols * rowsChange;
            _compactedMatrix = (
                rowsChange > 0 ?
                    _compactedMatrix.Concat(new double[rowsChange * NumCols]) :
                    _compactedMatrix.Take(Count)
            ).ToArray();
        }

        public void Resize(int newRowCapacity, int newColCapacity)
        {
            if (newColCapacity == NumCols)
            {
                Resize(newRowCapacity);
                return;
            }
            else if (newRowCapacity == NumRows && newColCapacity - NumCols == 1)
            {
                AddColumn(new double[NumRows]);
                return;
            }
            Count = newRowCapacity * newColCapacity;
            double[] newCompactedMatrix = new double[Count];
            NumRows = Math.Min(NumRows, newRowCapacity);
            NumCols = Math.Min(NumCols, newColCapacity);
            int oldCols = 0;
            int newCols = 0;
            for (int i = 0; i < NumRows; ++i)
            {
                for (int j = 0; j < NumCols; ++j)
                {
                    newCompactedMatrix[newCols + j] = _compactedMatrix[oldCols + j];
                }
                oldCols += NumCols;
                newCols += newColCapacity;
            }
            NumRows = newRowCapacity;
            NumCols = newColCapacity;
            _compactedMatrix = newCompactedMatrix;
        }
        #endregion

        public void GlueTo(Matrix mat2)
        {
            if (mat2.NumCols == NumCols)
            {
                NumRows += mat2.NumRows;
                Count += mat2.Count;
                _compactedMatrix = _compactedMatrix.Concat(mat2._compactedMatrix).ToArray();
            }
            else if (mat2.NumRows == NumRows)
            {
                int newCols = NumCols + mat2.NumCols;
                int newCount = Count + mat2.Count;
                double[] newCompactedMatrix = new double[newCount];
                int oldRowStart = 0;
                int mat2RowStart = 0;
                int newRowStart = 0;

                for (int i = 0; i < NumRows; ++i)
                {
                    for (int j = 0; j < NumCols; ++j)
                    {
                        newCompactedMatrix[newRowStart + j] = _compactedMatrix[oldRowStart + j];
                    }

                    int tempCols = newRowStart + NumCols;
                    for (int j = 0; j < mat2.NumCols; ++j)
                    {
                        newCompactedMatrix[tempCols + j] = mat2._compactedMatrix[mat2RowStart + j];
                    }

                    oldRowStart += NumCols;
                    newRowStart += newCols;
                    mat2RowStart += mat2.NumCols;
                }

                NumCols = newCols;
                Count = newCount;
                _compactedMatrix = newCompactedMatrix;
            }
        }

        public void AddColumn(double[] column)
        {
            int newCols = NumCols + 1;
            int newCount = Count + NumRows;
            double[] newCompactedMatrix = new double[newCount];
            int oldRowStart = 0;
            int newRowStart = 0;

            for (int i = 0; i < NumRows; ++i)
            {
                for (int j = 0; j < NumCols; ++j)
                {
                    newCompactedMatrix[newRowStart + j] = _compactedMatrix[oldRowStart + j];
                }
                newCompactedMatrix[newRowStart + NumCols] = column[i];
                oldRowStart += NumCols;
                newRowStart += newCols;
            }

            NumCols = newCols;
            Count = newCount;
            _compactedMatrix = newCompactedMatrix;
        }

        #region ColumnProperty
        public void SetColumn(int columnIndex, double[] column)
        {
            int num = columnIndex;
            for (int i = 0; i < NumRows; ++i)
            {
                _compactedMatrix[num] = column[i];
                num += NumCols;
            }
        }

        public double[] GetColumn(int columnIndex)
        {
            double[] column = new double[NumRows];
            int num = columnIndex;
            for (int i = 0; i < NumRows; ++i)
            {
                column[i] = _compactedMatrix[num];
                num += NumCols;
            }
            return column;
        }
        #endregion

        #region ToString
        public override string ToString() => ToString(2);

        public string ToString(int decimalPlaces) => ToString(decimalPlaces, true);

        public string ToString(int decimalPlaces, bool rowsLines)
        {
            string toStringFormat = decimalPlaces > 0 ? ("#." + new string('0', decimalPlaces)) : ("#");
            return "[" + String.Join(
                "]" + (rowsLines ? "\r\n" : "") + "[",
                Enumerable.Range(0, NumRows).Select(
                    i => String.Join(
                        new string('\t', Math.Max(1, decimalPlaces / 2)),
                        this[i].Select(
                            e => e.ToString(toStringFormat)
                        )
                    )
                )
            ) + "]";
        }

        public string ToCSV() => ToCSV(4);

        public string ToCSV(int decimalPlaces) => ToCSV(decimalPlaces, NumCols);

        public string ToCSV(int decimalPlaces, int paddedColumns)
        {
            string toStringFormat = decimalPlaces > 0 ? ("#." + new string('0', decimalPlaces)) : ("#");
            string padding = new string(',', paddedColumns - NumCols);
            return String.Join(
                "\r\n",
                Enumerable.Range(0, NumRows).Select(
                    i => String.Join(
                        ",",
                        this[i].Select(
                            e => e.ToString(toStringFormat)
                        )
                    ) + padding
                )
            );
        }
        #endregion

        #region SaveAs
        public void SaveAs(string folder, string matrixName)
        {
            SaveAs(folder, matrixName, 4);
        }

        private const string MATRIX_FILE_HEADER_FORMAT = "{0} {1}x{2} {3}\r\n";
        public void SaveAs(string folder, string matrixName, int decimalPlaces)
        {
            StringBuilder sb = null;
            try
            {
                var average = _compactedMatrix.Average();
                int digits = Convert.ToInt32(Math.Ceiling(Math.Log10(Convert.ToInt32(average)) + 1));
                int maxCapacity = ((
                    (
                        NumCols * (decimalPlaces == 0 ? 1 : decimalPlaces + 2)
                    ) + (NumCols * digits) + 2
                ) * NumRows) + 16;

                sb = new StringBuilder(
                    maxCapacity - 32, maxCapacity
                );
            }
            catch (Exception e)
            {
                (double min, double max) = _compactedMatrix.GetMinAndMax();
                var minDigits = Convert.ToInt32(min.GetDigitsCount());
                var maxDigits = Convert.ToInt32(max.GetDigitsCount());
                if (decimalPlaces == 0)
                {
                    sb = new StringBuilder(
                        ((
                            (NumCols * (decimalPlaces + 2 + minDigits)) + 2
                        ) * NumRows) + 16,
                        ((
                            (NumCols * (decimalPlaces + 2 + maxDigits)) + 2
                        ) * NumRows) + 16
                    );
                }
                else
                {
                    sb = new StringBuilder(
                        ((
                            (NumCols * minDigits) + 2
                        ) * NumRows) + 16,
                        ((
                            (NumCols * maxDigits) + 2
                        ) * NumRows) + 16
                    );
                }
            }
            
            sb.AppendFormat(MATRIX_FILE_HEADER_FORMAT, Count, NumRows, NumCols, decimalPlaces.ToString());

            if (decimalPlaces <= 0)
            {
                for (int i = 0; i < NumRows; ++i)
                {
                    int max = (i + 1) * NumCols;
                    for (int j = i * NumCols; j < max; ++j)
                    {
                        sb.Append((int)_compactedMatrix[j]);
                        sb.Append(' ');
                    }
                    sb.AppendLine();
                }
            }
            else
            {
                string toStringFormat = "#." + new string('0', decimalPlaces) + " ";

                for (int i = 0; i < NumRows; ++i)
                {
                    int max = (i + 1) * NumCols;
                    for (int j = i * NumCols; j < max; ++j)
                    {
                        sb.Append(_compactedMatrix[j].ToString(toStringFormat));
                    }
                    sb.AppendLine();
                }
            }

            File.WriteAllText(folder + (folder.EndsWith("\\") ? "" : "\\") + matrixName + ".mtx", sb.ToString());
            sb.Clear();
            sb = null;
        }

        public long Serialize(string folderPath, string matrixName, int numDecimalPlaces = 3)
        {
            (double min, double max, bool hasNegatives) = _compactedMatrix.GetAbsMinAndMax();
            var minDigits = (byte)Convert.ToInt32(min.GetDigitsCount());
            var maxDigits = (byte)Convert.ToInt32(max.GetDigitsCount());

            byte[] header = new byte[16];
            var bytes = BitConverter.GetBytes(Count);
            Array.Copy(bytes, header, 4);
            bytes = BitConverter.GetBytes(NumRows);
            Array.Copy(bytes, 0, header, 4, 4);
            bytes = BitConverter.GetBytes(NumCols);
            Array.Copy(bytes, 0, header, 8, 4);
            header[12] = (byte)numDecimalPlaces;
            header[13] = minDigits;
            header[14] = maxDigits;
            if (hasNegatives)
                header[15] = 255;
            else
                header[15] = 0;

            FileStream stream = new FileStream(
                folderPath.TrimEnd('\\') + "\\" + matrixName + ".mtx",
                FileMode.Create
            );
            stream.Write(header, 0, 16);
            if (numDecimalPlaces < 0 || numDecimalPlaces >= 7)
            {
                foreach (var element in _compactedMatrix)
                {
                    stream.Write(BitConverter.GetBytes((float)element), 0, 4);
                }
            }
            else if (numDecimalPlaces == 0)
            {
                foreach (var element in _compactedMatrix)
                {
                    stream.Write(BitConverter.GetBytes((int)element), 0, 4);
                }
            }
            else
            {
                foreach (var element in _compactedMatrix)
                {
                    stream.Write(BitConverter.GetBytes((float)Math.Round(element, numDecimalPlaces)), 0, 4);
                }
            }
            
            stream.Close();
            return 16 + ((long)4 * Count);
        }
        #endregion

        #region LoadFile
        public static Matrix LoadFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader reader = new StreamReader(fs);
            int[] header = reader.ReadLine().Split(' ', 'x').Select(n => Convert.ToInt32(n)).ToArray();
            int Count = header[0];
            int NumRows = header[1];
            int NumCols = header[2];
            int NumDecimals = header[3];
            header = null;

            double[] compacted = new double[Count];
            int start = 0;
            for (int i = 0; i < NumRows; ++i)
            {
                int end = start + NumCols;
                int index = 0;
                string[] line = reader.ReadLine().Split(' ');
                for(int j = start; j < end; ++j)
                {
                    compacted[j] = Convert.ToDouble(line[index]);
                    ++index;
                }
                start = end;
                line = null;
            }
            reader.Close();
            fs.Close();
            reader = null;
            fs = null;
            return new Matrix(compacted, NumCols);
        }

        public static Matrix Deserialize(string path) => Deserialize(path, out _);

        public static Matrix Deserialize(string path, out MatrixFileHeader fileHeader)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] header = new byte[16];
            fs.Read(header, 0, 16);
            fileHeader = new MatrixFileHeader(header);
            var compactedMatrix = new double[fileHeader.Count];

            byte[] bytes = new byte[4];
            if (fileHeader.NumDecimalPlaces == 0)
            {
                for (int i = 0; i < fileHeader.Count; ++i)
                {
                    fs.Read(bytes, 0, 4);
                    compactedMatrix[i] = BitConverter.ToInt32(bytes, 0);
                }
            }
            else
            {
                for (int i = 0; i < fileHeader.Count; ++i)
                {
                    fs.Read(bytes, 0, 4);
                    compactedMatrix[i] = Math.Round(BitConverter.ToSingle(bytes, 0), fileHeader.NumDecimalPlaces);
                }
            }
            fs.Close();
            return new Matrix(compactedMatrix, fileHeader.NumCols);
        }

        public struct MatrixFileHeader
        {
            public int Count { get; private set; }
            public int NumRows { get; private set; }
            public int NumCols { get; private set; }
            public int NumDecimalPlaces { get; private set; }
            public int MinDigits { get; private set; }
            public int MaxDigits { get; private set; }
            public bool HasNegatives { get; private set; }

            internal MatrixFileHeader(byte[] header)
            {
                Count = BitConverter.ToInt32(header, 0);
                NumRows = BitConverter.ToInt32(header, 4);
                NumCols = BitConverter.ToInt32(header, 8);
                NumDecimalPlaces = header[12];
                MinDigits = header[13];
                MaxDigits = header[14];
                HasNegatives = header[15] == 255;
            }
        }
        #endregion
    }
    public class IncorrectMatrixSizeException : Exception
    {
        public IncorrectMatrixSizeException(string message) : base(message)
        {

        }
    }

    public class MatrixInoperableException : Exception
    {
        public MatrixInoperableException(string message) : base(message)
        {

        }
    }
}
