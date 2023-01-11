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

        public Matrix(double[][] values)
        {
            Values = values;
        }

        public Matrix(int rows, int columns)
        {
            Values = new double[rows][];

            for (int i = 0; i < rows; i++)
            {
                Values[i] = new double[columns];
            }
        }


        public static Matrix operator *(Matrix firstMatrix, Matrix secondMatrix)
        {
            if (firstMatrix != null && secondMatrix != null)
            {
                if (!CanMultiply(secondMatrix.Values.Length, firstMatrix.Values[0].Length))
                {
                    throw new ArithmeticException("It is impossible to multiply matrices.");
                }

                int newMatrixRows = firstMatrix.Values.Length;
                int newMatrixColumns = secondMatrix.Values[0].Length;
                int secondMatrixRows = secondMatrix.Values.Length;
                Matrix newMatrix = new Matrix(newMatrixRows, newMatrixColumns);

                Parallel.For(0, newMatrixRows, (i) =>
                {
                    Parallel.For(0, newMatrixColumns, (j) =>
                    {
                        Parallel.For(0, secondMatrixRows, (k) =>
                        {
                            newMatrix.Values[i][j] += firstMatrix.Values[i][k] * secondMatrix.Values[k][j];
                        });
                    });
                });

                return newMatrix;
            }

            throw new ArgumentNullException();
        }

        public DataTable ToDataTable()
        {
            DataTable dataTable = new DataTable();

            if (Values != null)
            {
                for (int i = 0; i < Values[0].Length; i++)
                {
                    dataTable.Columns.Add();
                }

                for (int i = 0; i < Values.Length; i++)
                {
                    DataRow row = dataTable.NewRow();

                    for (int j = 0; j < Values[0].Length; j++)
                    {
                        row[j] = Values[i][j];
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }

        public double this[int row, int column]
        {
            get { return Values[row][column]; }
            set { Values[row][column] = value; }
        }

        private static bool CanMultiply(int row, int column)
        {
            return row == column ? true : false;
        }
    }
}
