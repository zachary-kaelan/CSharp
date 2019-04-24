using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZachLib.Statistics
{
    public class TriangleMatrix : Matrix
    {
        // vector multiplying itself
        public TriangleMatrix(Vector vec1, Func<double, double> elementCalculation) : base(0, 0)
        {
            int count = vec1.Count;
            _compactedMatrix = new double[
                (count - 1) * ((2 * count - 1) - (0))
            ];
            for (int i = 0; i < count - 1; ++i)
            {
                for(int j = count - 1; j > i; --i)
                {
                    
                }
            }
        }
    }
}
