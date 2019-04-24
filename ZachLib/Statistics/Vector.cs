using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Statistics
{
    public struct Vector : IReadOnlyList<double>
    {
        private double[] _vector { get; set; }
        public int Count { get; private set; }

        public double this[int index] { get => _vector[index]; }

        public Vector(double[] vector)
        {
            _vector = vector == null ? Array.Empty<double>() : vector;
            Count = _vector.Length;
        }

        public Vector(int length)
        {
            Count = Math.Max(0, length);
            _vector = Count == 0 ? Array.Empty<double>() : new double[Count];
        }

        public static Vector OnesVector(int length)
        {
            double[] vector = new double[length];
            for (int i = 0; i < length; ++i)
            {
                vector[i] = 1;
            }
            return vector;
        }

        #region VectorFunctions
        #region Dot
        public double Dot(Vector other)
        {
            if (Count != other.Count)
                throw new IncorrectMatrixSizeException("Both vectors must be of equal length.");
            double sum = 0;
            for (int i = 0; i < Count; ++i)
            {
                sum += _vector[i] * other[i];
            }
            return sum;
        }

        public Vector Dot(Matrix other)
        {
            int rows = other.NumRows;
            if (rows == Count)
            {
                int cols = other.NumCols;
                double[] vectorProduct = new double[cols];
                for (int j = 0; j < cols; ++j)
                {
                    for (int i = 0; i < other.NumRows; ++i)
                    {
                        vectorProduct[j] += _vector[i] * other[i, j];
                    }
                }
                return vectorProduct;
            }
            else
                throw new IncorrectMatrixSizeException(String.Format("Vector is size {0} and Matrix has {1} rows. The two need to be equal.", Count, other.NumRows));
        }

        public Matrix DotToMatrix(Vector other)
        {
            int otherLength = other.Count;
            Matrix mat = new Matrix(Count, otherLength);
            for (int i = 0; i < Count; ++i)
            {
                double element = _vector[i];
                double[] row = new double[otherLength];
                for (int j = 0; j < otherLength; ++j)
                {
                    row[j] = element * other[j];
                }
                mat[i] = row;
            }
            return mat;
        }
        #endregion

        #region Covariance
        public double Covariance(Vector other)
        {
            if (Count != other.Count)
                throw new InvalidVectorLengthException(other.Count);

            double sum = 0;
            double otherSum = 0;
            double productSum = 0;
            for (int i = 0; i < Count; ++i)
            {
                double value = _vector[i];
                double otherValue = other._vector[i];
                sum += value;
                otherSum += otherValue;
                productSum += value * otherValue;
            }
            return (productSum / Count) - ((sum * otherSum) / (Count * Count));
        }

        public double Covariance(Vector other, out double[] means)
        {
            if (Count != other.Count)
                throw new InvalidVectorLengthException(other.Count);

            double sum = 0;
            double otherSum = 0;
            double productSum = 0;
            for (int i = 0; i < Count; ++i)
            {
                double value = _vector[i];
                double otherValue = other._vector[i];
                sum += value;
                otherSum += otherValue;
                productSum += value * otherValue;
            }
            means = new double[] { productSum / Count, sum / Count, otherSum / Count };
            return means[0] - (means[1] * means[2]);
        }
        #endregion

        public double Variance()
        {
            double sum = 0;
            for (int i = 0; i < Count; ++i)
            {
                double val = _vector[i];
                sum += val * val;
            }
            return sum / Count;
        }

        public Vector ElementWiseChange(Func<double, double> op)
        {
            double[] newVector = new double[Count];
            for(int i = 0; i < Count; ++i)
            {
                newVector[i] = op(_vector[i]);
            }
            return newVector;
        }

        public Vector ElementWiseOp(Vector other, Func<double, double, double> op)
        {
            if (Count != other.Count)
                throw new InvalidVectorLengthException(other.Count);

            double[] newVector = new double[Count];
            for (int i = 0; i < Count; ++i)
            {
                newVector[i] = op(_vector[i], other[i]);
            }
            return newVector;
        }

        public double ElementWiseAggregate(Vector other, Func<double, double, double, double> agg)
        {
            if (Count != other.Count)
                throw new InvalidVectorLengthException(other.Count);

            double runningSum = 0;
            for (int i = 0; i < Count; ++i)
            {
                runningSum = agg(runningSum, _vector[i], other._vector[i]);
            }
            return runningSum;
        }
        #endregion

        public void SerializeToFile(string folderPath, string vectorName) =>
            File.WriteAllBytes(
                folderPath + (folderPath.EndsWith("\\") ? "\\" : "") + vectorName + ".vec", 
                _vector.SelectMany(e => BitConverter.GetBytes((float)e)).ToArray()
            );
        
        public static Vector LoadFile(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            var length = stream.Length;
            double[] vector = new double[length / 4];
            byte[] bytes = new byte[4];
            int iter = 0;
            for (int i = 0; i < length; i += 4)
            {
                stream.Read(bytes, i, 4);
                vector[iter] = BitConverter.ToSingle(bytes, 0);
            }
            return vector;
        }

        public IEnumerator<double> GetEnumerator()
        {
            return ((IReadOnlyList<double>)_vector).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyList<double>)_vector).GetEnumerator();
        }

        public static implicit operator double[](Vector vector) => vector._vector;
        public static implicit operator Vector(double[] array) => new Vector(array);
        public static explicit operator double(Vector vector)
        {
            double sum = 0;
            for (int i = 0; i < vector.Count; ++i)
            {
                sum += vector[i];
            }
            return sum;
        }

        public static Vector operator +(Vector vector, Vector other) => vector.ElementWiseOp(other, (e1, e2) => e1 + e2);
        public static Vector operator -(Vector vector, Vector other) => vector.ElementWiseOp(other, (e1, e2) => e1 - e2);
        public static Vector operator *(Vector vector, Vector other) => vector.ElementWiseOp(other, (e1, e2) => e1 * e2);
        public static Vector operator /(Vector vector, Vector other) => vector.ElementWiseOp(other, (e1, e2) => e1 / e2);

        public static Vector operator +(Vector vector, double other) => vector.ElementWiseChange(e => e + other);
        public static Vector operator -(Vector vector, double other) => vector.ElementWiseChange(e => e - other);
        public static Vector operator *(Vector vector, double other) => vector.ElementWiseChange(e => e * other);
        public static Vector operator /(Vector vector, double other) => vector.ElementWiseChange(e => e / other);
    }

    public class InvalidVectorLengthException : Exception
    {
        private const string MESSAGE_FORMAT = "Vector of length {0} is invalid.";
        public InvalidVectorLengthException(int vectorLength) : base(string.Format(MESSAGE_FORMAT, vectorLength)) { }
    }
}
