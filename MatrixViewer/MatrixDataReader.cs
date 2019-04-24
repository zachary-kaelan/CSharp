using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZachLib.Statistics;

namespace MatrixViewer
{
    public class MatrixDataReader : IDataReader
    {
        #region IDataReader Properties
        public object this[int i] => GetDouble(i);
        public object this[string name] => throw new NotImplementedException();
        public int Depth => 1;
        public bool IsClosed { get; private set; }
        public int RecordsAffected => throw new NotImplementedException();
        public int FieldCount => Matrix.NumCols;
        #endregion

        private double[] CurrentRow { get; set; }
        private int CurrentRowIndex { get; set; }
        private int CurrentColumnIndex { get; set; }
        private Matrix Matrix { get; set; }

        public MatrixDataReader(Matrix matrix)
        {
            Matrix = matrix;
            IsClosed = false;
            CurrentColumnIndex = 0;
            CurrentRowIndex = 0;
        }

        #region IDataReader Methods
        public void Close()
        {
            Matrix = null;
            CurrentRow = null;
            IsClosed = true;
        }

        public bool GetBoolean(int i) => CurrentRow[i] > 0;

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            double element = CurrentRow[i];
            var bytes = BitConverter.GetBytes(element);
            if (fieldOffset == 0 && length <= 8 && bufferoffset + length < buffer.Length)
            {
                Array.Copy(bytes, fieldOffset, buffer, bufferoffset, length);
                return length;
            }

            int bytesCopied = 0;
            int bufferIter = bufferoffset;
            for (int pos = (int)fieldOffset; pos < bytes.Length && bufferIter < buffer.Length && bytesCopied < length; ++pos)
            {
                buffer[bufferIter] = bytes[pos];
                ++bytesCopied;
            }
            return bytesCopied;
        }

        public double GetDouble(int i) => CurrentRow[i];

        public float GetFloat(int i) => (float)CurrentRow[i];

        public int GetInt32(int i) => (int)CurrentRow[i];

        #region Unnecessary Methods
        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }
        #endregion

        public bool Read()
        {
            ++CurrentRowIndex;
            if (CurrentRowIndex == Matrix.NumRows)
                return false;
            CurrentColumnIndex = 0;
            CurrentRow = Matrix[CurrentRowIndex];
            return true;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Matrix = null;
                    CurrentRow = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MatrixDataReader() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
