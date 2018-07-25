using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuralNetworks.LinearAlgebra
{
    public class Matrix
    {
        #region Fields

        private double[,] values;

        #endregion

        #region Properties

        public int Rows { get; private set; }
        public int Columns { get; private set; }

        #endregion

        #region Constructors

        public Matrix(int rows, int columns)
        {
            SetDimensions(rows, columns);
            values = new double[rows, columns];
        }

        public Matrix(double[,] values)
        {
            var rows = values.GetLength(0);
            var columns = values.GetLength(1);
            SetDimensions(rows, columns);
            this.values = values;
        }

        #endregion

        #region Row / Column Iterators

        private IEnumerable<double> GetRowUnchecked(int i)
        {
            for (var j = 0; j < Columns; ++j)
            {
                yield return values[i, j]; 
            }
        } 
        
        private IEnumerable<double> GetColumnUnchecked(int j)
        {
            for (var i = 0; i < Rows; ++i)
            {
                yield return values[i, j]; 
            }
        }
        
        public IEnumerable<double> GetRow(int row)
        {
            EnsureValidRowIndex(row);
            return GetRowUnchecked(row - 1);
        }

        public IEnumerable<double> GetColumn(int column)
        {
            EnsureValidColumnIndex(column);
            return GetColumnUnchecked(column - 1);
        }
        
        public IEnumerable<IEnumerable<double>> GetRows()
        {
            for (var i = 0; i < Rows; ++i)
            {
                yield return GetRowUnchecked(i);
            }
        }
        
        public IEnumerable<IEnumerable<double>> GetColumns()
        {
            for (var j = 0; j < Columns; ++j)
            {
                yield return GetColumnUnchecked(j);
            }
        }

        #endregion

        #region Matrix Operations

        public Matrix AddInplace(Matrix other)
        {
            EnsureEqualDimensions(other);
            return SetValues((i, j) => values[i, j] + other.values[i, j]); 
        }
        
        public Matrix SubtractInplace(Matrix other)
        {
            EnsureEqualDimensions(other);
            return SetValues((i, j) => values[i, j] - other.values[i, j]);  
        }
        
        public Matrix Add(Matrix other)
        {
            EnsureEqualDimensions(other);
            return CreateNewAndSetValues((i, j) => values[i, j] + other.values[i, j]); 
        }
        
        public Matrix Subtract(Matrix other)
        {
            EnsureEqualDimensions(other);
            return CreateNewAndSetValues((i, j) => values[i, j] - other.values[i, j]); 
        }
        
        public Matrix Multiply(Matrix other)
        {
            EnsureValidDimensionsForMatrixMultiplication(this, other);
            return CreateNewAndSetValues(Rows, other.Columns, (i, j) => VectorDotProduct(GetRowUnchecked(i), other.GetColumnUnchecked(j)));
        }
        
        public Matrix Transpose()
        {
            return CreateNewAndSetValues(Columns, Rows, (i, j) => values[j, i]);
        }
        
        public Matrix Copy()
        {
            return CreateNewAndSetValues((i, j) => values[i, j]);
        }
        
        #endregion

        #region Scalar Operations

        public Matrix AddInplace(double value)
        {
            return SetValues((i, j) => values[i, j] + value);
        }
        
        public Matrix SubtractInplace(double value)
        {
            return SetValues((i, j) => values[i, j] - value);
        }
        
        public Matrix MultiplyInplace(double value)
        {
            return SetValues((i, j) => values[i, j] * value);
        }
        
        public Matrix DivideInplace(double value)
        {
            return SetValues((i, j) => values[i, j] / value);
        }
        
        public Matrix Add(double value)
        {
            return CreateNewAndSetValues((i, j) => values[i, j] + value);
        }
        
        public Matrix Subtract(double value)
        {
            return CreateNewAndSetValues((i, j) => values[i, j] - value);
        }
        
        public Matrix Multiply(double value)
        {
            return CreateNewAndSetValues((i, j) => values[i, j] * value);
        }
        
        public Matrix Divide(double value)
        {
            return CreateNewAndSetValues((i, j) => values[i, j] / value);
        }

        #endregion
        
        #region Operator Overloads

        public double this[int row, int column]
        {
            get
            {
                EnsureValidIndices(row, column);
                return values[row - 1, column - 1];
            }
            set
            {
                EnsureValidIndices(row, column);
                values[row - 1, column - 1] = value;
            }
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            return a.Add(b);
        }
        
        public static Matrix operator -(Matrix a, Matrix b)
        {
            return a.Subtract(b);
        }
        
        public static Matrix operator *(Matrix a, Matrix b)
        {
            return a.Multiply(b);
        }
        
        public static Matrix operator +(Matrix m, double value)
        {
            return m.Add(value);
        }
        
        public static Matrix operator -(Matrix m, double value)
        {
            return m.Subtract(value);
        }
        
        public static Matrix operator *(Matrix m, double value)
        {
            return m.Multiply(value);
        }
        
        public static Matrix operator /(Matrix m, double value)
        {
            return m.Divide(value);
        }
        
        public static Matrix operator +(double value, Matrix m)
        {
            return m.Add(value);
        }
        
        public static Matrix operator -(double value, Matrix m)
        {
            return m.Subtract(value);
        }
        
        public static Matrix operator *(double value, Matrix m)
        {
            return m.Multiply(value);
        }
        
        public static Matrix operator /(double value, Matrix m)
        {
            return m.Divide(value);
        }
        
        #endregion

        #region Utilities

        private void SetDimensions(int rows, int columns)
        {
            EnsureValidDimensions(rows, columns);
            Rows = rows;
            Columns = columns;
        }

        private static double VectorDotProduct(IEnumerable<double> v1, IEnumerable<double> v2)
        {
            return v1.Zip(v2, (a, b) => a * b).Sum();
        }
        
        private Matrix SetValues(Func<int, int, double> valueSource)
        {
            Parallel.For(0, Columns, j =>
            {
                for (var i = 0; i < Rows; ++i)
                {
                    values[i, j] = valueSource(i, j);
                }
            });
            return this;
        }
        
        private static Matrix CreateNewAndSetValues(int rows, int columns, Func<int, int, double> valueSource)
        {
            return new Matrix(rows, columns).SetValues(valueSource);
        }
        
        private Matrix CreateNewAndSetValues(Func<int, int, double> valueSource)
        {
            return CreateNewAndSetValues(Rows, Columns, valueSource);
        }
        
        #endregion
        
        #region Method Overrides

        public override string ToString()
        {
            return $"{{{string.Join(", ", GetRows().Select(r => $"{{{(string.Join(", ", r))}}}"))}}}";
        }

        #endregion
        
        #region Validation

        private static bool IsRowDimensionValid(int rows)
        {
            return rows > 0;
        }
        
        private static bool IsColumnDimensionValid(int columns)
        {
            return columns > 0;
        }

        private bool IsRowIndexValid(int row)
        {
            return IsRowDimensionValid(row) && row <= Rows;
        }
        
        private bool IsColumnIndexValid(int column)
        {
            return IsColumnDimensionValid(column) && column <= Columns;
        }

        private bool AreDimensionsEqual(Matrix other)
        {
            return Rows == other.Rows && Columns == other.Columns;
        }
        
        private static bool AreDimensionsValidForMatrixMultiplication(Matrix a, Matrix b)
        {
            return a.Columns == b.Rows;
        }

        private void EnsureValidDimensions(int rows, int columns)
        {
            if (!IsRowDimensionValid(rows))
            {
                throw new ArgumentOutOfRangeException($"Invalid {nameof(rows)} dimension.");
            }
            if (!IsColumnDimensionValid(columns))
            {
                throw new ArgumentOutOfRangeException($"Invalid {nameof(columns)} dimension.");
            }
        }

        private void EnsureValidRowIndex(int row)
        {
            if (!IsRowIndexValid(row))
            {
                throw new ArgumentOutOfRangeException($"Invalid {nameof(row)} index.");
            }
        }
        
        private void EnsureValidColumnIndex(int column)
        {
            if (!IsColumnIndexValid(column))
            {
                throw new ArgumentOutOfRangeException($"Invalid {nameof(column)} index.");
            }
        }

        private void EnsureValidIndices(int row, int column)
        {
            EnsureValidRowIndex(row);
            EnsureValidColumnIndex(column);
        }

        private void EnsureEqualDimensions(Matrix other)
        {
            if (!AreDimensionsEqual(other))
            {
                throw new ArgumentOutOfRangeException($"Dimensions of this matrix and {nameof(other)} matrix are not equal.");
            }
        }

        private void EnsureValidDimensionsForMatrixMultiplication(Matrix a, Matrix b)
        {
            if (!AreDimensionsValidForMatrixMultiplication(a, b))
            {
                throw new InvalidOperationException($"Cannot multiply a ({a.Rows}, {a.Columns}) matrix with a ({b.Rows}, {b.Columns}) matrix.");
            }
        }

        #endregion
    }
}