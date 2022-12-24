using System;
using System.Data;
using System.Threading.Tasks;

namespace MatrixOperations.Models
{
    public class Matrix
    {
        private double[][] Values;

        public Matrix()
        {

        }

        public Matrix(int rows, int columns)
        {
            Values = new double[rows][];

            for (int i = 0; i < rows; i++)
            {
                Values[i] = new double[columns];
            }
        }

        public Matrix(double[][] values)
        {
            Values = values;
        }

        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A != null && B != null)
            {
                if (A.Values[0].Length != B.Values.Length)
                {
                    throw new ArithmeticException("It is impossible to multiply matrices.");
                }

                int newMatrixRows = A.Values.Length;
                int newMatrixColumns = B.Values[0].Length;

                Matrix C = new Matrix(newMatrixRows, newMatrixColumns);

                int bMatrixRows = B.Values.Length;
                Parallel.For(0, newMatrixRows, (i) =>
                {
                    Parallel.For(0, newMatrixColumns, (j) =>
                    {
                        Parallel.For(0, bMatrixRows, (k) =>
                        {
                            C.Values[i][j] += A.Values[i][k] * B.Values[k][j];
                        });
                    });
                });

                return C;
            }

            return new Matrix();
        }

        public DataTable ToDataTable()
        {
            DataTable resultTable = new DataTable();

            if (Values != null)
            {
                for (int i = 0; i < Values[0].Length; i++)
                {
                    resultTable.Columns.Add();
                }

                for (int i = 0; i < Values.Length; i++)
                {
                    DataRow row = resultTable.NewRow();

                    for (int j = 0; j < Values[0].Length; j++)
                    {
                        row[j] = Values[i][j];
                    }

                    resultTable.Rows.Add(row);
                }
            }

            return resultTable;
        }

        public double this[int row, int column]
        {
            get { return Values[row][column]; }
            set { Values[row][column] = value; }
        }
    }
}
