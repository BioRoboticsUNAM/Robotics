using System;
using System.Xml.Serialization;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Represents a NxM bidimentional matrix
	/// </summary>
	[SerializableAttribute]
	[XmlRoot("Matrix")]
	public class Matrix
	{
		#region Fields
		private int rows;
		private int cols;
		private double[,] m;
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		private Matrix()
		{
			rows = 0;
			cols = 0;
		}

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="m">A source to take values from</param>
		public Matrix(Matrix m)
		{
			rows = m.Rows;
			cols = m.Columns;
			this.m = new double[rows, cols];
			for (int i = 0; i < m.Rows; ++i)
				for (int j = 0;j< m.Columns; ++j)
					this.m[i, j] = m.m[i, j];
		}

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="m">Number of rows</param>
		/// <param name="n">Number of columns</param>
		public Matrix(int m, int n)
		{
			rows = m;
			cols = n;
			this.m = new double[rows, cols];
			this.m.Initialize();
		}

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="m">A source to take values from</param>
		/// <param name="rows">Number of rows</param>
		/// <param name="columns">Number of columns</param>
		public Matrix(Matrix m, int rows, int columns)
		{
			if (rows < 0)
				throw new ArgumentException("Rows must be greater than 0", "rows");
			if (columns < 0)
				throw new ArgumentException("Columns must be greater than 0", "columns");

			int r, c;

			this.rows = rows;
			this.cols = columns;
			this.m = new double[rows, cols];
			this.m.Initialize();

			r = Math.Min(this.rows, m.rows);
			c = Math.Min(this.cols, m.cols);
			for (int i = 0; i < r; ++i)
				for (int j = 0; j < c; ++j)
					this.m[i, j] = m.m[i, j];
		}

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="rows">Number of rows</param>
		/// <param name="columns">Number of columns</param>
		/// <param name="array">The array of values to fill the matrix with. The number of provided values must match the number of values the matrix can store</param>
		public Matrix(double[] array, int rows, int columns)
		{
			if (rows < 0)
				throw new ArgumentException("Rows must be greater than 0", "rows");
			if (columns < 0)
				throw new ArgumentException("Columns must be greater than 0", "columns");
			if (array.Length != rows * columns)
				throw new ArgumentException("The number of provided values must match the number of values the matrix can store", "list");
			this.rows = rows;
			this.cols = columns;
			m = new double[this.rows, this.cols];
			for (int i = 0; i < array.Length; ++i)
				m[i / cols, i % cols] = array[i];
		}

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="rows">Number of rows</param>
		/// <param name="columns">Number of columns</param>
		/// <param name="list">The list of values to fill the matrix with. The number of provided values must match the number of values the matrix can store</param>
		public Matrix(int rows, int columns, params double[] list)
		{
			if (rows < 0)
				throw new ArgumentException("Rows must be greater than 0", "rows");
			if (columns < 0)
				throw new ArgumentException("Columns must be greater than 0", "columns");
			if (list.Length != rows * columns)
				throw new ArgumentException("The number of provided values must match the number of values the matrix can store", "list");
			this.rows = rows;
			this.cols = columns;
			m = new double[this.rows, this.cols];
			for (int i = 0; i < list.Length; ++i)
				m[i/cols, i % cols] = list[i];
		}

		#region Constructors from Vectors

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="v">A source vector to take values from</param>
		/// <param name="column">Indicates if a column matrix must be created from the input vector, use false to create a row matrix</param>
		public Matrix(Vector2 v, bool column)
		{
			if (column)
			{
				rows = 2;
				cols = 1;
				this.m = new double[rows, cols];
				this.m[0, 0] = v.X;
				this.m[1, 0] = v.Y;
			}
			else
			{
				rows = 1;
				cols = 2;
				this.m = new double[rows, cols];
				this.m[0, 0] = v.X;
				this.m[0, 1] = v.Y;
			}
		}

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="v">A source vector to take values from</param>
		/// <param name="column">Indicates if a column matrix must be created from the input vector, use false to create a row matrix</param>
		public Matrix(Vector3 v, bool column)
		{
			if (column)
			{
				rows = 3;
				cols = 1;
				this.m = new double[rows, cols];
				this.m[0, 0] = v.X;
				this.m[1, 0] = v.Y;
				this.m[2, 0] = v.Z;
			}
			else
			{
				rows = 1;
				cols = 3;
				this.m = new double[rows, cols];
				this.m[0, 0] = v.X;
				this.m[0, 1] = v.Y;
				this.m[0, 2] = v.Z;
			}
		}

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="v">A source vector to take values from</param>
		/// <param name="column">Indicates if a column matrix must be created from the input vector, use false to create a row matrix</param>
		public Matrix(Vector4 v, bool column)
		{
			if (column)
			{
				rows = 4;
				cols = 1;
				this.m = new double[rows, cols];
				for (int i = 0; i < 4; ++i)
					this.m[i, 0] = v.v[i];
			}
			else
			{
				rows = 1;
				cols = 4;
				this.m = new double[rows, cols];
				for (int i = 0; i < 4; ++i)
					this.m[0, i] = v.v[i];
			}
		}

		/// <summary>
		/// Initializes a new instance of Matrix
		/// </summary>
		/// <param name="v">A source vector to take values from</param>
		/// <param name="column">Indicates if a column matrix must be created from the input vector, use false to create a row matrix</param>
		public Matrix(Vector v, bool column)
		{
			if (column)
			{
				rows = v.Dimension;
				cols = 1;
				this.m = new double[rows, cols];
				for (int i = 0; i < v.Dimension; ++i)
					this.m[i, 0] = v.v[i];
			}
			else
			{
				rows = 1;
				cols = v.Dimension;
				this.m = new double[rows, cols];
				for (int i = 0; i < v.Dimension; ++i)
					this.m[0, i] = v.v[i];
			}
		}

		#endregion

		#endregion

		#region Public Static Methods

		/// <summary>
		/// Returns an instance of the identity matrix
		/// </summary>
		/// <param name="rank">rank of the square identity matrix</param>
		public static Matrix Identity(int rank)
		{

			Matrix i = new Matrix(rank, rank);
			for (int j = 0; j < rank; ++j)
				i.m[j, j] = 1;
			return i;
		}

		/// <summary>
		///  Returns a Zero-dimension matrix
		/// </summary>
		public static Matrix Null
		{
			get { return new Matrix(); }
		}

		/// <summary>
		/// Returns a matrix with all its values set to zero
		/// </summary>
		/// <param name="rows">Number of rows</param>
		/// <param name="columns">Number of columns</param>
		public static Matrix Zero(int rows, int columns)
		{
			return new Matrix(rows, columns);
		}

		/// <summary>
		/// Returns a matrix with all its values set to one
		/// </summary>
		/// <param name="rows">Number of rows</param>
		/// <param name="columns">Number of columns</param>
		public static Matrix One(int rows, int columns)
		{
			Matrix one = new Matrix(rows, columns);
			for (int i = 0; i < rows; ++i)
				for (int j = 0; j < columns; ++j)
					one[i, j] = i;
			return one;
		}

		#endregion

		#region Properties

		/*
		/// <summary>
		/// Calculates the adjunt (cofactor) matrix
		/// </summary>
		private Matrix Adjunt
		{
			get { return Cofactor; }
		}
		*/

		/// <summary>
		/// Calculates the transpose of the matrix of cofactors
		/// </summary>
		public Matrix Adjugate
		{
			get
			{
				if (!IsSquare) return Null;
				int n = rows;
				Matrix c = Matrix.Zero(n, n);
				int row, col;

				for (row = 0; row < n; ++row)
				{
					for (col = 0; col < n; ++col)
					{
						c[col, row] = Minor(row, col);
						if (((col + row) % 2) != 0)
							c[row, col] *= -1;
					}
				}
				return c;
			}
		}

		/// <summary>
		/// Gets the number of rows in the matrix
		/// </summary>
		public int Rows
		{
			get { return rows; }
		}

		/// <summary>
		/// Calculates the cofactor (adjunt) matrix
		/// </summary>
		public Matrix Cofactor
		{
			get
			{
				if (!IsSquare) return Null;
				int n = rows;
				Matrix c = Matrix.Zero(n, n);
				int row, col;

				for (row = 0; row < n; ++row)
				{
					for (col = 0; col < n; ++col)
					{
						c[row, col] = Minor(row, col);
						if (((col + row) % 2) != 0)
							c[row, col] *= -1;
					}
				}
				return c;
			}
		}

		/// <summary>
		/// Gets the number of columns in the matrix
		/// </summary>
		public int Columns
		{
			get { return cols; }
		}

		/// <summary>
		/// Gets a value indicating if matrix is square
		/// </summary>
		public bool IsSquare
		{
			get { return cols == rows; }
		}

		/// <summary>
		/// Calculates the determinant of the matrix
		/// </summary>
		public virtual double Determinant
		{
			get
			{
				if (!IsSquare)
					return double.NaN;
				switch (rows)
				{
					case 0:
						return double.NaN;

					case 1:
						return m[0, 0];

					case 2:
						return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];

					case 3:
						return
							m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1])
							- m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
							+ m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
				}

				double det = 0;
				for (int col = 0; col < cols; ++col)
				{
					double minor = Minor(0, col);
					if ((col % 2) == 0)
						det += m[0, col] * minor;
					else
						det -= m[0, col] * minor;
				}
				return det;
			}
		}

		/// <summary>
		/// Calculates the determinant of the matrix.
		/// It computes the determinant by reducing the matrix to reduced echelon form using row operations.
		/// The function is very fast and efficient but may oerflow in some cases returning NaN.
		/// In such cases use the Matrix.Determinant property which computes determinent using minors
		/// </summary>
		public double FastDeterminant
		{
			get
			{
				if (!IsSquare || (rows == 0))
					return double.NaN;

				// Reduced Echelon Matrix
				Matrix rem;
				double det;
				int i, j;

				det = 1;
				try
				{
					rem = new Matrix(this);
					for (i = 0; i < this.Rows; ++i)
					{
						if (rem[i, i] == 0)
						{
						// If diagonal entry is zero, check if some below entry is non-zero
						for (j = i + 1; j < rem.Rows; ++j)
						{
							// if below entry is non-zero
							if (rem[j, i] != 0)
							{
								// interchange the two rows
								rem.SwitchRow(i, j);
								//interchanging two rows negates the determinent
								det *= -1;
							}
						}
						}

						det *= rem[i, i];
						rem.MultiplyRow(i, 1/rem[i, i]);

						for (j = i + 1; j < rem.Rows; ++j)
							rem.AddRows(j, i, - rem[j, i]);
						for (j = i - 1; j >= 0; --j)
							rem.AddRows(j, i ,- rem[j, i]);
					}
					return det;
				}
				catch
				{
					throw new Exception("Determinent of the given matrix could not be calculated");
				}
			}
		}

		/// <summary>
		/// Gets the inverse matrix
		/// The function returns the inverse of the current matrix using Reduced Echelon Form method
		/// The function is very fast and efficient but may cause overflow in some cases returning Null
		/// In such cases use the Inverse property which computes inverse using adjugate / determinant
		/// </summary>
		public Matrix FastInverse
		{
			get
			{
				if (!IsSquare) return Null;

				// Reduced Echelon Matrix
				Matrix rem;
				Matrix inverse;
				double det, aux;
				int i, j;

				det = FastDeterminant;
				if (det == 0) return Null;
				inverse = Matrix.Identity(this.rows);
				rem = new Matrix(this);

				try
				{
					for (i = 0; i < this.Rows; ++i)
					{
						if (rem[i, i] == 0)
						{
							// If diagonal entry is zero, check if some below entry is non-zero
							for (j = i + 1; j < rem.Rows; ++j)
							{
								// if below entry is non-zero
								if (rem[j, i] != 0)
								{
									// interchange the two rows of both matrix
									rem.SwitchRow(i, j);
									inverse.SwitchRow(i, j);
								}
							}
						}
						aux = 1 / rem[i, i];
						inverse.MultiplyRow(i, aux);
						rem.MultiplyRow(i, aux);

						for (j = i + 1; j < rem.Rows; ++j)
						{
							aux = -rem[j, i];
							inverse.AddRows(j, i, aux);
							rem.AddRows(j, i, aux);
						}
						for (j = i - 1; j >= 0; --j)
						{
							aux = -rem[j, i];
							inverse.AddRows(j, i, aux);
							rem.AddRows(j, i, aux);
						}
					}
					return inverse;
				}
				catch
				{
					throw new Exception("Inverse of the given matrix could not be calculated");
				}
			}
		}

		/// <summary>
		/// Returns the minors matrix
		/// </summary>
		public Matrix Minors
		{
			get
			{
				if (!IsSquare) return Null;
				int n = rows;
				Matrix c = Matrix.Zero(n, n);
				int row, col;

				for (row = 0; row < n; ++row)
				{
					for (col = 0; col < n; ++col)
						c[row, col] = Minor(row, col);
				}
				return c;
			}
		}

		/// <summary>
		/// Gets the transpose matrix 
		/// </summary>
		public virtual Matrix Transpose
		{
			get
			{
				Matrix t = new Matrix(cols, rows);
				for (int i = 0; i < cols; ++i)
					for (int j = 0; j < rows; ++j)
						t.m[i, j] = m[j, i];
				return t;
			}
		}

		/// <summary>
		/// Gets the inverse matrix
		/// </summary>
		public virtual Matrix Inverse
		{
			get
			{
				if (!IsSquare) return Null;
				double det = Determinant;

				if (det == 0) return Null;
				return this.Adjugate / det;

				/*
				int n = rows;
				Matrix a = new Matrix(this);
				Matrix b = Matrix.Identity(n);
				Matrix c = Matrix.Zero(n, n);

				for (int k = 0; k < n - 1; k++)
				{
					for (int i = k + 1; i < n; i++)
					{
						for (int s = 0; s < n; s++)
							b.m[i, s] -= a.m[i, k] * b.m[k, s] / a.m[k, k];
						for (int j = k + 1; j < n; j++)
							a.m[i,j] -= a.m[i,k] * a.m[k,j] / a.m[k,k];
					}
				}

				for (int s = 0; s < n; s++)
				{
					c.m[n - 1, s] = b.m[n - 1, s] / a.m[n - 1, n - 1];
					for (int i = n - 2; i >= 0; i--)
					{
						c.m[i, s] = b.m[i, s] / a.m[i, i];
						for (int k = n - 1; k > i; k--)
							c.m[i, s] -= a.m[i, k] * c.m[k, s] / a.m[i, i];
					}
				}
				return c;
				*/
			}
		}

		#endregion

		#region Inexers

		/// <summary>
		/// Gets or sets the value at specified location
		/// </summary>
		/// <param name="row">Zero-based Row of the desired value</param>
		/// <param name="col">Zero-based Column of the desired value</param>
		/// <returns></returns>
		public double this[int row, int col]
		{
			get
			{
				if ((row < 0) || (row > rows))
					throw new ArgumentException("Row must be between 0 and " + rows.ToString(), "row");
				if ((col < 0) || (col > cols))
					throw new ArgumentException("Col must be between 0 and " + col.ToString(), "col");
				return m[row, col];
			}
			set
			{
				if ((row < 0) || (row > rows))
					throw new ArgumentException("Row must be between 0 and " + rows.ToString(), "row");
				if ((col < 0) || (col > cols))
					throw new ArgumentException("Col must be between 0 and " + col.ToString(), "col");
				m[row, col] = value;
			}
		}

		/// <summary>
		/// Gets or sets the specified row
		/// </summary>
		/// <param name="row">Zero-based index of the Row to get</param>
		/// <returns></returns>
		public double[] this[int row]
		{
			get
			{
				if ((row < 0) || (row >= rows))
					throw new ArgumentException("Row must be between 0 and " + (rows-1).ToString(), "row");
				double[] r = new double[cols];
				for (int i = 0; i < cols; ++i)
					r[i] = m[row, i];
				return r;
			}
			set
			{
				if ((row < 0) || (row >= rows))
					throw new ArgumentException("Row must be between 0 and " + (rows-1).ToString(), "row");
				if(value.Length != cols)
					throw new ArgumentException("The array must have the same number of elements than colmumns in the matrix");
				for (int i = 0; i < cols; ++i)
					m[row, i] = value[i];
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds one row to another
		/// targetRow = targetRow + sourceRow
		/// </summary>
		/// <param name="targetRow">The row to which the source row will be added</param>
		/// <param name="sourceRow">Source row which will be added to the target row</param>
		public void AddRows(int targetRow, int sourceRow)
		{
			for (int col = 0; col < this.cols; ++col)
			{
				m[targetRow, col] += m[sourceRow, col];
			}
		}

		/// <summary>
		/// Adds one scaled row to another
		/// targetRow = targetRow + sourceRow * scalar
		/// </summary>
		/// <param name="targetRow">The row to which the source row will be added</param>
		/// <param name="sourceRow">Source row which multiple will be added to the target row</param>
		/// <param name="scalar">Scale factor for the source row</param>
		public void AddRows(int targetRow, int sourceRow, double scalar)
		{
			for (int col = 0; col < this.cols; ++col)
			{
				m[targetRow, col] += m[sourceRow, col] *scalar;
			}
		}

		/// <summary>
		/// Gets the specified Column as an Mx1 matrix
		/// </summary>
		/// <param name="col">Index of column to get</param>
		/// <returns>The specified column as an array of doubles</returns>
		public Matrix GetColumn(int col)
		{
			if ((col < 0) || (col >= cols))
				throw new ArgumentException("Col must be between 0 and " + (rows - 1).ToString(), "col");
			Matrix m = new Matrix(rows, 1);

			for (int i = 0; i < rows; ++i)
				m.m[i, 0] = m[i, col];
			return m;
		}

		/// <summary>
		/// Gets the specified Column as an array of doubles
		/// </summary>
		/// <param name="col">Index of column to get</param>
		/// <returns>The specified column as an array of doubles</returns>
		public double[] GetColumnArray(int col)
		{
			if ((col < 0) || (col >= cols))
				throw new ArgumentException("Col must be between 0 and " + (rows - 1).ToString(), "col");
			double[] c = new double[rows];
			for (int i = 0; i < rows; ++i)
				c[i] = m[i, col];
			return c;
		}

		/// <summary>
		/// Gets the specified Row as an 1xN matrix
		/// </summary>
		/// <param name="row">Index of row to get</param>
		/// <returns>The specified row as an array of doubles</returns>
		public Matrix GetRow(int row)
		{
			if ((row < 0) || (row >= rows))
				throw new ArgumentException("Row must be between 0 and " + (rows - 1).ToString(), "row");

			Matrix m = new Matrix(1, cols);

			for (int i = 0; i < cols; ++i)
				m.m[0, i] = m[row, i];
			return m;
		}

		/// <summary>
		/// Gets the specified Row as an array of doubles
		/// </summary>
		/// <param name="row">Index of row to get</param>
		/// <returns>The specified row as an array of doubles</returns>
		public double[] GetRowArray(int row)
		{
			if ((row < 0) || (row >= rows))
				throw new ArgumentException("Row must be between 0 and " + (rows - 1).ToString(), "row");
			double[] r = new double[cols];
			for (int i = 0; i < cols; ++i)
				r[i] = m[row, i];
			return r;
		}

		/// <summary>
		/// Gets the Minor value for a given element
		/// </summary>
		/// <param name="column">Column of the element for which calculate the minor</param>
		/// <param name="row">Row of the element for which calculate the minor</param>
		/// <returns>Minor value</returns>
		public double Minor(int row, int column)
		{
			if (!IsSquare)
				throw new Exception("Invalid operation");
			if (row >= this.rows)
				throw new ArgumentOutOfRangeException();
			if (column >= this.cols)
				throw new ArgumentOutOfRangeException();

			Matrix sub;
			switch (this.rows)
			{
				case 0:
				case 1:
					return Double.NaN;

				case 2:
					sub = SubMatrix(row, column);
					return sub[0, 0];

				default:
					sub = SubMatrix(row, column);
					return sub.Determinant;
			}
			//Matrix aux = Matrix.Zero(n-1, n-1);
			//int i, j;
			//int ai = -1;
			//int aj = -1;
			
			//for (i = 0; i < n; ++i)
			//{
			//	if (i == row)
			//		continue;
			//	else
			//		++ai;
			//	for (j = 0, aj=-1; j < n; ++j)
			//	{
			//		if (j == column)
			//			continue;
			//		else
			//			++aj;
			//		aux[ai, aj] = m[i, j];
			//	}
			//}

			//return aux.Determinant;
		}

		/// <summary>
		/// Multiplies the specified row of this Matrix by a scalar value
		/// </summary>
		/// <param name="row">Row to multiply by provided scalar value</param>
		/// <param name="scalar">Scalar value to multiply by</param>
		public void MultiplyRow(int row, double scalar)
		{
			for (int col = 0; col < this.cols; ++col)
			{
				m[row, col] *= scalar;
			}
		}

		/// <summary>
		/// Gets the sumbatrix obtained from remove the i-th row and j-th column of this Matrix
		/// </summary>
		/// <param name="column">Column to remove</param>
		/// <param name="row">Row to remove</param>
		/// <returns>Sumbatrix obtained from remove the i-th row and j-th column of this Matrix</returns>
		public Matrix SubMatrix(int row, int column)
		{
			if (row >= this.rows)
				throw new ArgumentOutOfRangeException();
			if (column >= this.cols)
				throw new ArgumentOutOfRangeException();

			Matrix aux;
			int i, j, ai, aj;

			aux = Matrix.Zero(this.rows - 1, this.cols - 1);
			for (i = 0, ai = 0; i < this.rows; ++i)
			{
				if (i == row)
					continue;
				for (j = 0, aj = 0; j < this.cols; ++j)
				{
					if (j == column)
						continue;
					aux[ai, aj] = m[i, j];
					++aj;
				}
				++ai;
			}

			return aux;
		}

		/// <summary>
		/// Switches two rows in the matrix
		/// </summary>
		/// <param name="i">Index of row to switch</param>
		/// <param name="j">Index of row to switch</param>
		public void SwitchRow(int i, int j)
		{
			if ((i < 0) || (i >= rows))
				throw new ArgumentException("i must be between 0 and " + (rows-1).ToString(), "i");
			if ((j < 0) || (j >= rows))
				throw new ArgumentException("j must be between 0 and " + (rows-1).ToString(), "j");
			double tmp;
			for (int k = 0; k < cols; ++k)
			{
				tmp = m[j, k];
				m[j, k] = m[i, k];
				m[i, k] = tmp;
			}
		}

		/// <summary>
		/// Switches two columns in the matrix
		/// </summary>
		/// <param name="i">Index of column to switch</param>
		/// <param name="j">Index of column to switch</param>
		public void SwitchColumn(int i, int j)
		{
			if ((i < 0) || (i >= cols))
				throw new ArgumentException("i must be between 0 and " + (cols-1).ToString(), "i");
			if ((j < 0) || (j >= cols))
				throw new ArgumentException("j must be between 0 and " + (cols-1).ToString(), "j");
			double tmp;
			for (int k = 0; k < rows; ++k)
			{
				tmp = m[k, j];
				m[k, j] = m[k, i];
				m[k, i] = tmp;
			}
		}

		/// <summary>
		/// Calculates the lower triangle matrix
		/// </summary>
		/// <returns>A triangular Matrix with all the entries above the main diagonal are zero</returns>
		public Matrix TriangularU()
		{
			int limit = Math.Min(cols, rows);
			Matrix a = new Matrix(this);
			for (int k = 0; k < limit - 1; k++)
				for (int i = k; i < limit; i++)
					for (int j = k + 1; j < limit; j++)
						a.m[i, j] -= a.m[i, k] * a.m[k, j] / a.m[k, k];
			return a;
		}

		/// <summary>
		/// Calculates the lower triangle matrix
		/// </summary>
		/// <returns>A triangular Matrix with all the entries below the main diagonal are zero</returns>
		public Matrix TriangularL()
		{
			int limit = Math.Min(cols, rows);
			Matrix a = new Matrix(this);
			for (int k = 0; k < limit - 1; k++)
				for (int i = k; i < limit; i++)
					for (int j = k + 1; j < limit; j++)
						a.m[i, j] -= a.m[i, k] * a.m[k, j] / a.m[k, k];

			return a;
		}

		/// <summary>
		/// Compares equality with other objects
		/// </summary>
		/// <param name="obj">Object to compare</param>
		/// <returns>true if objects are equal, false otherwise</returns>
		public override bool Equals(object obj)
		{
			Matrix mobj = obj as Matrix;
			if (mobj == null) return false;
			return mobj == this;
		}

		/// <summary>
		/// Gets the hash code for the matrix object
		/// </summary>
		/// <returns>Hash code</returns>
		public override int GetHashCode()
		{
			return m.GetHashCode();
		}

		/// <summary>
		/// Gets the string representation of this matrix
		/// </summary>
		/// <returns>string representation of the matrix</returns>
		public override string ToString()
		{
			string s = "";
			for (int i = 0; i < rows; ++i)
			{
				s += "{ ";
				for (int j = 0; j < cols; ++j)
				{
					//s+= this.m[i, j].ToString("0.00") + (j==cols-1 ? "\r\n" : " " );
					s += this.m[i, j].ToString("0.00") + " ";
				}
				s += (i == rows - 1 ? "}" : "} ");
			}
			return s;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Adds two matrices
		/// </summary>
		/// <param name="value1">Source matrix</param>
		/// <param name="value2">Source matrix</param>
		/// <returns>Sum of the source matrices</returns>
		public static Matrix Add(Matrix value1, Matrix value2)
		{
			return value1 + value2;
		}

		/// <summary>
		/// Divides a matrix by a scalar value
		/// </summary>
		/// <param name="value">Source matrix</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source matrix divided by divider</returns>
		public static Matrix Divide(Matrix value, double divider)
		{
			return value / divider;
		}

		/// <summary>
		/// Calculates the product of a matrix by the inverse of the other
		/// </summary>
		/// <param name="matrix">Source matrix</param>
		/// <param name="inverted">Source matrix</param>
		/// <returns>The product of the two matrices</returns>
		public static double Divide(Matrix matrix, Matrix inverted)
		{
			return matrix * inverted.Inverse;
		}

		/// <summary>
		/// Calculates the product of two matrices
		/// </summary>
		/// <param name="value1">Source matrix</param>
		/// <param name="value2">Source matrix</param>
		/// <returns>The dot product of the two matrices</returns>
		public static double Multiply(Matrix value1, Matrix value2)
		{
			return value1 * value2;
		}

		/// <summary>
		/// Multiplies a matrix by a scalar value
		/// </summary>
		/// <param name="matrix">Source matrix</param>
		/// <param name="scalar">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Matrix Multiply(Matrix matrix, double scalar)
		{
			return scalar * matrix;
		}

		/// <summary>
		/// Subtracts a matrix from a matrix
		/// </summary>
		/// <param name="value1">Source matrix</param>
		/// <param name="value2">Source matrix</param>
		/// <returns>Result of the subtraction</returns>
		public static Matrix Substract(Matrix value1, Matrix value2)
		{
			return value1 - value2;
		}

		/// <summary>
		/// Solves a NxN linear equation system 
		/// </summary>
		/// <param name="coefficients">Coefficient matrix</param>
		/// <param name="independentTerms">Vector of independent terms</param>
		/// <returns>Vector solution or null if system has no solution</returns>
		public static double[] Solve(Matrix coefficients, double[] independentTerms)
		{
			if ((coefficients.Inverse == null) || (coefficients.Inverse == Matrix.Null)) return null;
			if (coefficients.Columns != independentTerms.Length) return null;
			Matrix m = (coefficients.Inverse.Transpose * new Matrix(independentTerms.Length, 1, independentTerms));
			double[] result = new double[independentTerms.Length];
			for (int i = 0; i < independentTerms.Length; ++i)
				result[i] = m.m[i, 0];
			return result;
		}

		/// <summary>
		/// Solves a NxN linear equation system 
		/// </summary>
		/// <param name="coefficients">Coefficient matrix</param>
		/// <param name="independentTerms">Vector of independent terms</param>
		/// <returns>Matrix with solution or Matrix.Null if system has no solution</returns>
		public static Matrix Solve(Matrix coefficients, Matrix independentTerms)
		{
			if ((coefficients.Inverse == null) || (coefficients.Inverse == Matrix.Null)) return Matrix.Null;
			if (coefficients.Columns != independentTerms.Rows) return Matrix.Null;
			return (coefficients.Inverse.Transpose * independentTerms);
		}

		#region Matrix from vectors

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector2 value1, Vector2 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector2 value1, Vector3 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector2 value1, Vector4 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector3 value1, Vector2 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector3 value1, Vector3 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector3 value1, Vector4 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector4 value1, Vector2 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector4 value1, Vector3 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector4 value1, Vector4 value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		/// <summary>
		/// Calculates the product of two vectors as a matrix
		/// </summary>
		/// <param name="value1">Source vector (column vector)</param>
		/// <param name="value2">Source vector (row vector)</param>
		/// <returns>Matrix generated by the product of the vectors</returns>
		public static Matrix Multiply(Vector value1, Vector value2)
		{
			Matrix m1 = new Matrix(value1, true);
			Matrix m2 = new Matrix(value2, false);
			return m1 * m2;
		}

		#endregion

		#endregion

		#region Operator Overload

		/// <summary>
		/// Negates the matrix
		/// </summary>
		/// <param name="a">Matrix to substract</param>
		/// <returns>Matrix result of difference</returns>
		public static Matrix operator -(Matrix a)
		{
			Matrix result = Matrix.Zero(a.rows, a.cols);
			for (int i = 0; i < a.rows; ++i)
				for (int j = 0; j < a.cols; ++j)
					result.m[i, j] = -a[i, j];
			return result;
		}

		/// <summary>
		/// Add two Matrices
		/// </summary>
		/// <param name="a">Matrix to add</param>
		/// <param name="b">Matrix to add</param>
		/// <returns>Matrix result of sum</returns>
		public static Matrix operator +(Matrix a, Matrix b)
		{
			if ((a.rows != b.rows) || (a.cols != b.cols))
				throw new ArgumentException("Incompatible matrices");
			Matrix result = Matrix.Zero(a.rows, a.cols);
			for (int i = 0; i < a.rows; ++i)
				for (int j = 0; j < a.cols; ++j)
					result.m[i, j] = a[i, j] + b[i, j];
			return result;
		}

		/// <summary>
		/// Substract two Matrices
		/// </summary>
		/// <param name="a">Matrix to substract</param>
		/// <param name="b">Matrix to substract</param>
		/// <returns>Matrix result of difference</returns>
		public static Matrix operator -(Matrix a, Matrix b)
		{
			if ((a.rows != b.rows) || (a.cols != b.cols))
				throw new ArgumentException("Incompatible matrices");
			Matrix result = Matrix.Zero(a.rows, a.cols);
			for (int i = 0; i < a.rows; ++i)
				for (int j = 0; j < a.cols; ++j)
					result.m[i, j] = a[i, j] - b[i, j];
			return result;
		}

		/// <summary>
		/// Multiplicates a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix result of multiplication</returns>
		public static Matrix operator *(Matrix matrix, double scalar)
		{
			Matrix result  = new Matrix(matrix);
			for (int i = 0; i < matrix.rows; ++i)
				for (int j = 0; j < matrix.cols; ++j)
					result.m[i, j] *= scalar;
			return result;
		}

		/// <summary>
		/// Multiplicates a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix result of multiplication</returns>
		public static Matrix operator *(double scalar, Matrix matrix)
		{
			Matrix result = new Matrix(matrix);
			for (int i = 0; i < matrix.rows; ++i)
				for (int j = 0; j < matrix.cols; ++j)
					result.m[i, j] *= scalar;
			return result;
		}

		/// <summary>
		/// Multiplicates two Matrices
		/// </summary>
		/// <param name="a">Matrix to multiplicate</param>
		/// <param name="b">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Matrix operator *(Matrix a, Matrix b)
		{
			if (a.cols != b.rows)
				throw new ArgumentException("Incompatible matrices");
			Matrix result = Matrix.Zero(a.rows, b.cols);
			for (int i = 0; i < a.rows; ++i)
				for (int j = 0; j < b.cols; ++j)
					for (int k = 0; k < a.cols; ++k)
						result.m[i, j] += a.m[i, k] * b.m[k, j];
			return result;
		}

		/// <summary>
		/// Divides a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix result of division</returns>
		public static Matrix operator /(Matrix matrix, double scalar)
		{
			Matrix result = new Matrix(matrix);
			for (int i = 0; i < matrix.rows; ++i)
				for (int j = 0; j < matrix.cols; ++j)
					result.m[i, j] /= scalar;
			return result;
		}

		/// <summary>
		/// Divides a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix result of division</returns>
		public static Matrix operator /(double scalar, Matrix matrix)
		{
			Matrix result = new Matrix(matrix);
			for (int i = 0; i < matrix.rows; ++i)
				for (int j = 0; j < matrix.cols; ++j)
					result.m[i, j] /= scalar;
			return result;
		}

		/// <summary>
		/// Returns a the inverse matrix
		/// </summary>
		/// <param name="value">Source matrix</param>
		/// <returns>Inverse matrix</returns>
		public static Matrix operator !(Matrix value)
		{
			return value.Inverse;
		}

		/// <summary>
		/// Tests matrices for equality
		/// </summary>
		/// <param name="value1">Source matrix</param>
		/// <param name="value2">Source matrix</param>
		/// <returns>true if the matrices are equal; false otherwise</returns>
		public static bool operator ==(Matrix value1, Matrix value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return true;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return false;

			if ((value1.rows == value2.rows) && (value1.cols == value2.cols))
			{
				for (int i = 0; i < value1.rows; ++i)
				{
					for (int j = 0; j < value1.cols; ++j)
					{
						if (value1[i, j] != value2[i, j])
							return false;
					}
				}
			}
			else return false;
			return true;
		}

		/// <summary>
		/// Tests matrices for inequality
		/// </summary>
		/// <param name="value1">Source matrix</param>
		/// <param name="value2">Source matrix</param>
		/// <returns>true if the matrices are not equal; false otherwise</returns>
		public static bool operator !=(Matrix value1, Matrix value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return false;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return true;

			if ((value1.rows != value2.rows) || (value1.cols != value2.cols))
				return true;

			for (int i = 0; i < value1.rows; ++i)
			{
				for (int j = 0; j < value1.cols; ++j)
				{
					if (value1[i, j] != value2[i, j])
						return true;
				}
			}
			return false;
		}

		#region Vector Operator Overload

		/// <summary>
		/// Multiplicates a row vector with a matrix
		/// </summary>
		/// <param name="v">Vector2 to multiplicate</param>
		/// <param name="m">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Matrix operator *(Vector2 v, Matrix m)
		{
			Matrix row = new Matrix(v, false);
			return row * m;
		}

		/// <summary>
		/// Multiplicates a row vector with a matrix
		/// </summary>
		/// <param name="v">Vector3 to multiplicate</param>
		/// <param name="m">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Matrix operator *(Vector3 v, Matrix m)
		{
			Matrix row = new Matrix(v, false);
			return row * m;
		}

		/// <summary>
		/// Multiplicates a row vector with a matrix
		/// </summary>
		/// <param name="v">Vector4 to multiplicate</param>
		/// <param name="m">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Matrix operator *(Vector4 v, Matrix m)
		{
			Matrix row = new Matrix(v, false);
			return row * m;
		}

		/// <summary>
		/// Multiplicates a Matrix with a column vector
		/// </summary>
		/// <param name="v">Vector2 to multiplicate</param>
		/// <param name="m">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Matrix operator *(Matrix m, Vector2 v)
		{
			Matrix column = new Matrix(v, true);
			return m * column;
		}

		/// <summary>
		/// Multiplicates a Matrix with a column vector
		/// </summary>
		/// <param name="v">Vector3 to multiplicate</param>
		/// <param name="m">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Matrix operator *(Matrix m, Vector3 v)
		{
			Matrix column = new Matrix(v, true);
			return m * column;
		}

		/// <summary>
		/// Multiplicates a Matrix with a column vector
		/// </summary>
		/// <param name="v">Vector4 to multiplicate</param>
		/// <param name="m">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Matrix operator *(Matrix m, Vector4 v)
		{
			Matrix column = new Matrix(v, true);
			return m * column;
		}

		/// <summary>
		/// Multiplicates a Matrix with a column vector
		/// </summary>
		/// <param name="v">Vector to multiplicate</param>
		/// <param name="m">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Vector operator *(Matrix m, Vector v)
		{
			Matrix m1 = new Matrix(v, true);
			Matrix resultM = m * m1;
			Vector result = new Vector(resultM.rows);
			for(int i =0; i < result.Length; ++i)
				result[i] = resultM[i, 0];
			return result;
		}

		/// <summary>
		/// Multiplicates a row vector with a matrix
		/// </summary>
		/// <param name="v">Vector to multiplicate</param>
		/// <param name="m">Matrix to miltiplicate</param>
		/// <returns>Matrix product</returns>
		public static Vector operator *(Vector v, Matrix m)
		{
			Matrix m1 = new Matrix(v, false);
			Matrix resultM = m1 * m;
			Vector result = new Vector(v.v.Length);
			for(int i =0; i < result.Length; ++i)
				result[i] = resultM[0, i];
			return result;
		}

		/// <summary>
		/// Cast a matrix to a Vector2
		/// </summary>
		/// <param name="m">Matrix to cast</param>
		/// <returns>A Vector2 containing the first 2 elements of the casted Matrix</returns>
		public static explicit operator Vector2(Matrix m)
		{
			if ((m.rows * m.cols) < 2)
				throw new InvalidCastException();
			if (m.cols == 1)
				return new Vector2(m[0, 0], m[1, 0]);
			else
				return new Vector2(m[0, 0], m[0, 1]);
		}

		/// <summary>
		/// Cast a matrix to a Vector3
		/// </summary>
		/// <param name="m">Matrix to cast</param>
		/// <returns>A Vector3 containing the first 3 elements of the casted Matrix</returns>
		public static explicit operator Vector3(Matrix m)
		{
			int i, j, k;
			double[] v = new double[3];

			k = 0;
			for (i = 0; (i < m.Rows) && (k < v.Length); ++i)
			{
				for (j = 0; (j < m.Columns) && (k < v.Length); ++j)
				{
					v[k] = m[i, j];
					++k;
				}
			}
			return new Vector3(v[0], v[1], v[2]);
		}

		/// <summary>
		/// Cast a matrix to a Vector4
		/// </summary>
		/// <param name="m">Matrix to cast</param>
		/// <returns>A Vector4 containing the first 4 elements of the casted Matrix</returns>
		public static explicit operator Vector4(Matrix m)
		{
			int i, j, k;
			double[] v = new double[4];

			k = 0;
			for (i = 0; (i < m.Rows) && (k < v.Length); ++i)
			{
				for (j = 0; (j < m.Columns) && (k < v.Length); ++j)
				{
					v[k] = m[i, j];
					++k;
				}
			}
			return new Vector4(v[0], v[1], v[2], v[3]);
		}

		/// <summary>
		/// Cast a matrix to a Vector
		/// </summary>
		/// <param name="m">Matrix to cast</param>
		/// <returns>A Vector containing elements of the casted Matrix</returns>
		public static explicit operator Vector(Matrix m)
		{
			int i, j, k;
			double[] v = new double[m.rows * m.cols];

			k = 0;
			for (i = 0; (i < m.Rows) && (k < v.Length); ++i)
			{
				for (j = 0; (j < m.Columns) && (k < v.Length); ++j)
				{
					v[k] = m[i, j];
					++k;
				}
			}
			return new Vector(v);
		}

		#endregion

		#endregion

		#region Casting Overload

		/// <summary>
		/// Casts the Matrix object to a int
		/// If the Matrix object is a square matrix, the determinant is returned.
		/// If the Matrix object is NOT a square matrix, the element located at the first row and first column is returned
		/// </summary>
		/// <param name="matrix">Matrix to cast</param>
		/// <returns>32 bit integer that represents the determinant or the first element of the matrix object</returns>
		public static implicit operator int(Matrix matrix)
		{
			if (matrix.IsSquare)
				return (int)matrix.Determinant;
			else
				return (int)matrix.m[0, 0];
		}

		/// <summary>
		/// Casts the Matrix object to a double
		/// If the Matrix object is a square matrix, the determinant is returned.
		/// If the Matrix object is NOT a square matrix, the element located at the first row and first column is returned
		/// </summary>
		/// <param name="matrix">Matrix to cast</param>
		/// <returns>double that represents the determinant or the first element of the matrix object</returns>
		public static implicit operator double(Matrix matrix)
		{
			if (matrix.IsSquare)
				return matrix.Determinant;
			else
				return matrix.m[0, 0];
		}

		/// <summary>
		/// Casts the Matrix object to an array of doubles
		/// The elements of the matrix are stored in the array as a merge of its rows
		/// </summary>
		/// <param name="matrix">Matrix to cast</param>
		/// <returns>double[] containing all elements of the matrix</returns>
		public static explicit operator double[](Matrix matrix)
		{
			if((matrix.rows < 1) || (matrix.cols < 1)) return (new double[]{});
			double[] vector = new double[matrix.rows * matrix.cols];
			
			int row = 0;
			int col = 0;
			int i = 0;
			for(row = 0; row < matrix.rows; ++row)
			{
				for (col = 0; col < matrix.cols; ++col)
				{
					vector[i] = matrix.m[row, col];
					++i;
				}
			}
			return vector;
		}

		#endregion
	}
}