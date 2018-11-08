using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayferCode
{
    public static class CharMatrixExtensions
    {
        public static (int, int) Find(this char[,] matrix, char c)
        {
            for (var i = 0; i < matrix.Length; i++)
            {
                var row = i / (matrix.GetUpperBound(0) + 1);
                var col = i % (matrix.GetUpperBound(1) + 1);
                if (matrix[row, col] == c)
                    return (row, col);
            }
            return (-1, -1);
        }
    }
}
