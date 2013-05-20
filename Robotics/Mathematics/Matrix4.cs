using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Represents a 4x4 bidimentional matrix
	/// </summary>
	[SerializableAttribute]
	[XmlRoot("Matrix4")]
	public class Matrix4 : Matrix
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of Matrix4
		/// </summary>
		/// <param name="m">A source to take values from</param>
		public Matrix4(Matrix4 m) : base(m)
		{}

		/// <summary>
		/// Initializes a new instance of Matrix4
		/// </summary>
		/// <param name="m11">Value to initialize m11 to</param>
		/// <param name="m12">Value to initialize m12 to</param>
		/// <param name="m13">Value to initialize m13 to</param>
		/// <param name="m14">Value to initialize m14 to</param>
		/// <param name="m21">Value to initialize m21 to</param>
		/// <param name="m22">Value to initialize m22 to</param>
		/// <param name="m23">Value to initialize m23 to</param>
		/// <param name="m24">Value to initialize m24 to</param>
		/// <param name="m31">Value to initialize m31 to</param>
		/// <param name="m32">Value to initialize m32 to</param>
		/// <param name="m33">Value to initialize m33 to</param>
		/// <param name="m34">Value to initialize m34 to</param>
		/// <param name="m41">Value to initialize m41 to</param>
		/// <param name="m42">Value to initialize m42 to</param>
		/// <param name="m43">Value to initialize m43 to</param>
		/// <param name="m44">Value to initialize m44 to</param>
		public Matrix4(double m11, double m12, double m13, double m14,
			double m21, double m22, double m23, double m24,
			double m31, double m32, double m33, double m34,
			double m41, double m42, double m43, double m44)
			: base(4, 4, m11, m12, m13, m14,
					m21, m22, m23, m24,
					m31, m32, m33, m34,
					m41, m42, m43, m44)
		{}

		/// <summary>
		/// Initializes a new instance of Matrix4
		/// </summary>
		/// <param name="row1">Value to initialize first row of the matrix to</param>
		/// <param name="row2">Value to initialize second row of the matrix to</param>
		/// <param name="row3">Value to initialize third row of the matrix to</param>
		/// <param name="row4">Value to initialize fourth row of the matrix to</param>
		public Matrix4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4) : base(4, 4)
		{
			if ((row1 == null) || (row2 == null) || (row3 == null) || (row4 == null))
				throw new ArgumentNullException();
			this[0, 0] = row1.X;
			this[0, 1] = row1.Y;
			this[0, 2] = row1.Z;
			this[0, 3] = row1.W;

			this[1, 0] = row2.X;
			this[1, 1] = row2.Y;
			this[1, 2] = row2.Z;
			this[1, 3] = row2.W;

			this[2, 0] = row3.X;
			this[2, 1] = row3.Y;
			this[2, 2] = row3.Z;
			this[2, 3] = row3.W;

			this[3, 0] = row4.X;
			this[3, 1] = row4.Y;
			this[3, 2] = row4.Z;
			this[3, 3] = row4.W;
		}

		/// <summary>
		/// Initializes a new instance of Matrix4
		/// </summary>
		private Matrix4():base(4, 4)
		{}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Returns an instance of the identity matrix
		/// </summary>
		new public static Matrix4 Identity
		{
			get { return new Matrix4(
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1); }
		}

		/// <summary>
		/// Returns a matrix with all its values set to zero
		/// </summary>
		new public static Matrix4 Zero
		{
			get
			{
				return new Matrix4(
			  0, 0, 0, 0,
			  0, 0, 0, 0,
			  0, 0, 0, 0,
			  0, 0, 0, 0);
			}
		}

		/// <summary>
		/// Returns a matrix with all its values set to one
		/// </summary>
		new public static Matrix4 One
		{
			get
			{
				return new Matrix4(
			  1, 1, 1, 1,
			  1, 1, 1, 1,
			  1, 1, 1, 1,
			  1, 1, 1, 1);
			}
		}
		
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the value at row 1 column 1 of the matrix.
		/// </summary>
		public double M11
		{
			get { return this[0, 0]; }
			set { this[0, 0] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 1 column 2 of the matrix.
		/// </summary>
		public double M12
		{
			get { return this[0, 1]; }
			set { this[0, 1] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 1 column 3 of the matrix.
		/// </summary>
		public double M13
		{
			get { return this[0, 2]; }
			set { this[0, 2] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 1 column 4 of the matrix.
		/// </summary>
		public double M14
		{
			get { return this[0, 3]; }
			set { this[0, 3] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 2 column 1 of the matrix.
		/// </summary>
		public double M21
		{
			get { return this[1, 0]; }
			set { this[1, 0] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 2 column 2 of the matrix.
		/// </summary>
		public double M22
		{
			get { return this[1, 1]; }
			set { this[1, 1] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 2 column 3 of the matrix.
		/// </summary>
		public double M23
		{
			get { return this[1, 2]; }
			set { this[1, 2] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 2 column 4 of the matrix.
		/// </summary>
		public double M24
		{
			get { return this[1, 3]; }
			set { this[1, 3] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 3 column 1 of the matrix.
		/// </summary>
		public double M31
		{
			get { return this[2, 0]; }
			set { this[2, 0] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 3 column 2 of the matrix.
		/// </summary>
		public double M32
		{
			get { return this[2, 1]; }
			set { this[2, 1] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 3 column 3 of the matrix.
		/// </summary>
		public double M33
		{
			get { return this[2, 2]; }
			set { this[2, 2] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 3 column 4 of the matrix.
		/// </summary>
		public double M34
		{
			get { return this[2, 3]; }
			set { this[2, 3] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 4 column 1 of the matrix.
		/// </summary>
		public double M41
		{
			get { return this[3, 0]; }
			set { this[3, 0] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 4 column 2 of the matrix.
		/// </summary>
		public double M42
		{
			get { return this[3, 1]; }
			set { this[3, 1] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 4 column 3 of the matrix.
		/// </summary>
		public double M43
		{
			get { return this[3, 2]; }
			set { this[3, 2] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 4 column 4 of the matrix.
		/// </summary>
		public double M44
		{
			get { return this[3, 3]; }
			set { this[3, 3] = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the string representation of this matrix
		/// </summary>
		/// <returns>string representation of the matrix</returns>
		public override string ToString()
		{
			string s = "";
			for (int i = 0; i < 4; ++i)
			{
				for (int j = 0; j < 4; ++j)
					s+= this[i, j].ToString("0.00") + (j==3 ? "\r\n" : "\t" );	
			}
			return s;
		}

		#endregion

		#region Operator Overload

		/// <summary>
		/// Add two Matrices
		/// </summary>
		/// <param name="a">Matrix4 to add</param>
		/// <param name="b">Matrix4 to add</param>
		/// <returns>Matrix4 result of sum</returns>
		public static Matrix4 operator +(Matrix4 a, Matrix4 b)
		{
			return (Matrix4)((Matrix)a + (Matrix)b);
		}

		/// <summary>
		/// Substract two Matrices
		/// </summary>
		/// <param name="a">Matrix4 to substract</param>
		/// <param name="b">Matrix4 to substract</param>
		/// <returns>Matrix4 result of difference</returns>
		public static Matrix4 operator -(Matrix4 a, Matrix4 b)
		{
			return (Matrix4)((Matrix)a - (Matrix)b);
		}

		/// <summary>
		/// Multiplicates a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix4 to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix4 result of multiplication</returns>
		public static Matrix4 operator *(Matrix4 matrix, double scalar)
		{
			return (Matrix4)((Matrix)matrix * scalar);
		}

		/// <summary>
		/// Multiplicates a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix4 to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix4 result of multiplication</returns>
		public static Matrix4 operator *(double scalar, Matrix4 matrix)
		{
			return (Matrix4)(scalar * (Matrix)matrix);
		}

		/// <summary>
		/// Multiplicates two Matrices
		/// </summary>
		/// <param name="a">Matrix4 to multiplicate</param>
		/// <param name="b">Matrix4 to miltiplicate</param>
		/// <returns>Matrix4 product</returns>
		public static Matrix4 operator *(Matrix4 a, Matrix4 b)
		{
			return (Matrix4)((Matrix)a * (Matrix)b);
		}

		/// <summary>
		/// Divides a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix4 to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix4 result of division</returns>
		public static Matrix4 operator /(Matrix4 matrix, double scalar)
		{
			return (Matrix4)((Matrix)matrix / scalar);
		}

		/// <summary>
		/// Divides a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix4 to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix4 result of division</returns>
		public static Matrix4 operator /(double scalar, Matrix4 matrix)
		{
			return (Matrix4)(scalar / (Matrix)matrix);
		}

		#endregion

	/*
	/// <summary>
	/// Represents a 4x4 bidimentional matrix
	/// </summary>
	public class Matrix4
	{
		#region Fields
		private double[,] m;
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Matrix4
		/// </summary>
		/// <param name="m">A source to take values from</param>
		public Matrix4(Matrix4 m) : this()
		{
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					this.m[i, j] = m.m[i, j];
		}

		/// <summary>
		/// Initializes a new instance of Matrix4
		/// </summary>
		/// <param name="m11">Value to initialize m11 to</param>
		/// <param name="m12">Value to initialize m12 to</param>
		/// <param name="m13">Value to initialize m13 to</param>
		/// <param name="m14">Value to initialize m14 to</param>
		/// <param name="m21">Value to initialize m21 to</param>
		/// <param name="m22">Value to initialize m22 to</param>
		/// <param name="m23">Value to initialize m23 to</param>
		/// <param name="m24">Value to initialize m24 to</param>
		/// <param name="m31">Value to initialize m31 to</param>
		/// <param name="m32">Value to initialize m32 to</param>
		/// <param name="m33">Value to initialize m33 to</param>
		/// <param name="m34">Value to initialize m34 to</param>
		/// <param name="m41">Value to initialize m41 to</param>
		/// <param name="m42">Value to initialize m42 to</param>
		/// <param name="m43">Value to initialize m43 to</param>
		/// <param name="m44">Value to initialize m44 to</param>
		public Matrix4(double m11, double m12, double m13, double m14,
			double m21, double m22, double m23, double m24,
			double m31, double m32, double m33, double m34,
			double m41, double m42, double m43, double m44) : this()
		{
			this.m[0, 0] = m11;
			this.m[0, 1] = m12;
			this.m[0, 2] = m13;
			this.m[0, 3] = m14;

			this.m[1, 0] = m21;
			this.m[1, 1] = m22;
			this.m[1, 2] = m23;
			this.m[1, 3] = m24;

			this.m[2, 0] = m31;
			this.m[2, 1] = m32;
			this.m[2, 2] = m33;
			this.m[2, 3] = m34;

			this.m[3, 0] = m41;
			this.m[3, 1] = m42;
			this.m[3, 2] = m43;
			this.m[3, 3] = m44;
		}

		/// <summary>
		/// Initializes a new instance of Matrix4
		/// </summary>
		/// <param name="col1">Value to initialize first colum of the matrix to</param>
		/// <param name="col2">Value to initialize second colum of the matrix to</param>
		/// <param name="col3">Value to initialize third colum of the matrix to</param>
		/// <param name="col4">Value to initialize fourth colum of the matrix to</param>
		public Matrix4(Vector4 col1, Vector4 col2, Vector4 col3, Vector4 col4) : this()
		{
			this.m[0, 0] = col1.X;
			this.m[0, 1] = col1.Y;
			this.m[0, 2] = col1.Z;
			this.m[0, 3] = col1.W;

			this.m[1, 0] = col2.X;
			this.m[1, 1] = col2.Y;
			this.m[1, 2] = col2.Z;
			this.m[1, 3] = col2.W;

			this.m[2, 0] = col3.X;
			this.m[2, 1] = col3.Y;
			this.m[2, 2] = col3.Z;
			this.m[2, 3] = col3.W;

			this.m[3, 0] = col4.X;
			this.m[3, 1] = col4.Y;
			this.m[3, 2] = col4.Z;
			this.m[3, 3] = col4.W;
		}

		/// <summary>
		/// Initializes a new instance of Matrix4
		/// </summary>
		private Matrix4()
		{
			m = new double[4,4];
		}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Returns an instance of the identity matrix
		/// </summary>
		public static Matrix4 Identity
		{
			get { return new Matrix4(
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1); }
		}

		/// <summary>
		/// Returns a matrix with all its values set to zero
		/// </summary>
		public static Matrix4 Zero
		{
			get
			{
				return new Matrix4(
			  0, 0, 0, 0,
			  0, 0, 0, 0,
			  0, 0, 0, 0,
			  0, 0, 0, 0);
			}
		}

		/// <summary>
		/// Returns a matrix with all its values set to one
		/// </summary>
		public static Matrix4 One
		{
			get
			{
				return new Matrix4(
			  1, 1, 1, 1,
			  1, 1, 1, 1,
			  1, 1, 1, 1,
			  1, 1, 1, 1);
			}
		}
		
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the value at row 1 column 1 of the matrix.
		/// </summary>
		public double M11
		{
			get { return m[0, 0]; }
			set { m[0, 0] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 1 column 2 of the matrix.
		/// </summary>
		public double M12
		{
			get { return m[0, 1]; }
			set { m[0, 1] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 1 column 3 of the matrix.
		/// </summary>
		public double M13
		{
			get { return m[0, 2]; }
			set { m[0, 2] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 1 column 4 of the matrix.
		/// </summary>
		public double M14
		{
			get { return m[0, 3]; }
			set { m[0, 3] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 2 column 1 of the matrix.
		/// </summary>
		public double M21
		{
			get { return m[1, 0]; }
			set { m[1, 0] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 2 column 2 of the matrix.
		/// </summary>
		public double M22
		{
			get { return m[1, 1]; }
			set { m[1, 1] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 2 column 3 of the matrix.
		/// </summary>
		public double M23
		{
			get { return m[1, 2]; }
			set { m[1, 2] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 2 column 4 of the matrix.
		/// </summary>
		public double M24
		{
			get { return m[1, 3]; }
			set { m[1, 3] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 3 column 1 of the matrix.
		/// </summary>
		public double M31
		{
			get { return m[2, 0]; }
			set { m[2, 0] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 3 column 2 of the matrix.
		/// </summary>
		public double M32
		{
			get { return m[2, 1]; }
			set { m[2, 1] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 3 column 3 of the matrix.
		/// </summary>
		public double M33
		{
			get { return m[2, 2]; }
			set { m[2, 2] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 3 column 4 of the matrix.
		/// </summary>
		public double M34
		{
			get { return m[2, 3]; }
			set { m[2, 3] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 4 column 1 of the matrix.
		/// </summary>
		public double M41
		{
			get { return m[3, 0]; }
			set { m[3, 0] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 4 column 2 of the matrix.
		/// </summary>
		public double M42
		{
			get { return m[3, 1]; }
			set { m[3, 1] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 4 column 3 of the matrix.
		/// </summary>
		public double M43
		{
			get { return m[3, 2]; }
			set { m[3, 2] = value; }
		}

		/// <summary>
		/// Gets or sets the value at row 4 column 4 of the matrix.
		/// </summary>
		public double M44
		{
			get { return m[3, 3]; }
			set { m[3, 3] = value; }
		}

		/// <summary>
		/// Calculates the determinant of the matrix
		/// </summary>
		public double Determinant
		{
			get
			{
				double det = 0;
				for (int col = 0; col < 4; ++col)
					det += Minor(0, col);
				return det;
			}
		}

		/// <summary>
		/// Gets the transpose matrix 
		/// </summary>
		public Matrix4 Transpose
		{
			get { return new Matrix4(
				m[0, 0], m[1, 0], m[2, 0], m[3, 0],
				m[0, 1], m[1, 1], m[2, 1], m[3, 1],
				m[0, 2], m[1, 2], m[2, 2], m[3, 2],
				m[0, 3], m[1, 3], m[2, 3], m[3, 3]
				); }
		}

		/// <summary>
		/// Gets the inverse matrix
		/// </summary>
		public Matrix4 Inverse
		{
			get
			{
				int n = 4;
				Matrix4 a = new Matrix4(this);
				Matrix4 b = Matrix4.Identity;
				Matrix4 c = Matrix4.Zero;

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
			}
		}

		#endregion

		#region Inexers

		/// <summary>
		/// Gets or sets the value at specified location
		/// </summary>
		/// <param name="row">Row of desired value</param>
		/// <param name="col">Column of desired value</param>
		/// <returns></returns>
		public double this[int row, int col]
		{
			get
			{
				if ((row < 0) || (row > 3))
					throw new ArgumentException("Row must be between 0 and 3", "row");
				if ((col < 0) || (col > 3))
					throw new ArgumentException("Col must be between 0 and 3", "col");
				return m[row, col];
			}
			set
			{
				if ((row < 0) || (row > 3))
					throw new ArgumentException("Row must be between 0 and 3", "row");
				if ((col < 0) || (col > 3))
					throw new ArgumentException("Col must be between 0 and 3", "col");
				m[row, col] = value;
			}
		}

		/// <summary>
		/// Gets or sets the specified row
		/// </summary>
		/// <param name="row">Row to get</param>
		/// <returns></returns>
		public Vector4 this[int row]
		{
			get
			{
				if ((row < 0) || (row > 3))
					throw new ArgumentException("Row must be between 0 and 3", "row");
				return new Vector4(m[row, 0], m[row, 1], m[row, 2], m[row, 3]);
			}
			set
			{
				if ((row < 0) || (row > 3))
					throw new ArgumentException("Row must be between 0 and 3", "row");
				for (int i = 0; i < 4; ++i )
					m[row, i] = value[i];
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets the Minor value for a given element
		/// </summary>
		/// <param name="column">Column of the element for which calculate the minor</param>
		/// <param name="row">Row of the element for which calculate the minor</param>
		/// <returns>Minor value</returns>
		public double Minor(int row, int column)
		{
			Matrix aux = Matrix.Zero(3, 3);
			int i, j;
			int ai = -1;
			int aj = -1;

			for (i = 0; i < 4; ++i)
			{
				if (i == row)
					continue;
				else
					++ai;
				for (j = 0, aj = -1; j < 4; ++j)
				{
					if (j == column)
						continue;
					else
						++aj;
					aux[ai, aj] = m[i, j];
				}
			}

			return aux.Determinant;
		}

		/// <summary>
		/// Switches two rows in the matrix
		/// </summary>
		/// <param name="i">Index of row to switch</param>
		/// <param name="j">Index of row to switch</param>
		public void SwitchRow(int i, int j)
		{
			if ((i < 0) || (i > 3))
				throw new ArgumentException("i must be between 0 and 3", "i");
			if ((j < 0) || (j > 3))
				throw new ArgumentException("j must be between 0 and 3", "j");
			double tmp;
			for (int k = 0; k < 4; ++k)
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
			if ((i < 0) || (i > 3))
				throw new ArgumentException("i must be between 0 and 3", "i");
			if ((j < 0) || (j > 3))
				throw new ArgumentException("j must be between 0 and 3", "j");
			double tmp;
			for (int k = 0; k < 4; ++k)
			{
				tmp = m[k, j];
				m[k, j] = m[k, i];
				m[k, i] = tmp;
			}
		}

		/// <summary>
		/// Calculates the lower triangle matrix
		/// </summary>
		/// <returns>A triangular Matrix4 with all the entries above the main diagonal are zero</returns>
		public Matrix4 TriangularU()
		{
			Matrix4 a = new Matrix4(this);
			for (int k = 0; k < 4 - 1; k++)
				for (int i = k; i < 4; i++)
					for (int j = k + 1; j < 4; j++)
						a.m[i, j] -= a.m[i, k] * a.m[k, j] / a.m[k, k];
			return a;
		}

		/// <summary>
		/// Calculates the lower triangle matrix
		/// </summary>
		/// <returns>A triangular Matrix4 with all the entries below the main diagonal are zero</returns>
		public Matrix4 TriangularL()
		{
			Matrix4 a = new Matrix4(this);
			for (int k = 0; k < 4 - 1; k++)
				for (int i = k; i < 4; i++)
					for (int j = k + 1; j < 4; j++)
						a.m[i, j] -= a.m[i, k] * a.m[k, j] / a.m[k, k];

			return a;
		}

		/// <summary>
		/// Gets the string representation of this matrix
		/// </summary>
		/// <returns>string representation of the matrix</returns>
		public override string ToString()
		{
			string s = "";
			for (int i = 0; i < 4; ++i)
			{
				for (int j = 0; j < 4; ++j)
					s+= this.m[i, j].ToString("0.00") + (j==3 ? "\r\n" : "\t" );	
			}
			return s;
		}

		#endregion

		#region Operator Overload

		/// <summary>
		/// Add two Matrices
		/// </summary>
		/// <param name="a">Matrix4 to add</param>
		/// <param name="b">Matrix4 to add</param>
		/// <returns>Matrix4 result of sum</returns>
		public static Matrix4 operator +(Matrix4 a, Matrix4 b)
		{
			Matrix4 result = Matrix4.Zero;
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					result.m[i, j] = a[i, j] + b[i, j];
			return result;
		}

		/// <summary>
		/// Substract two Matrices
		/// </summary>
		/// <param name="a">Matrix4 to substract</param>
		/// <param name="b">Matrix4 to substract</param>
		/// <returns>Matrix4 result of difference</returns>
		public static Matrix4 operator -(Matrix4 a, Matrix4 b)
		{
			Matrix4 result = Matrix4.Zero;
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					result.m[i, j] = a[i, j] - b[i, j];
			return result;
		}

		/// <summary>
		/// Multiplicates a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix4 to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix4 result of multiplication</returns>
		public static Matrix4 operator *(Matrix4 matrix, double scalar)
		{
			Matrix4 result  = new Matrix4(matrix);
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					result.m[i, j] *= scalar;
			return result;
		}

		/// <summary>
		/// Multiplicates a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix4 to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix4 result of multiplication</returns>
		public static Matrix4 operator *(double scalar, Matrix4 matrix)
		{
			Matrix4 result = new Matrix4(matrix);
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					result.m[i, j] *= scalar;
			return result;
		}

		/// <summary>
		/// Multiplicates two Matrices
		/// </summary>
		/// <param name="a">Matrix4 to multiplicate</param>
		/// <param name="b">Matrix4 to miltiplicate</param>
		/// <returns>Matrix4 product</returns>
		public static Matrix4 operator *(Matrix4 a, Matrix4 b)
		{
			Matrix4 result = Matrix4.Zero;
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					for (int k = 0; k < 4; ++k)
						result.m[i, j] += a.m[i, k] * b.m[k, j];
			return result;
		}

		/// <summary>
		/// Divides a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix4 to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix4 result of division</returns>
		public static Matrix4 operator /(Matrix4 matrix, double scalar)
		{
			Matrix4 result = new Matrix4(matrix);
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					result.m[i, j] /= scalar;
			return result;
		}

		/// <summary>
		/// Divides a matrix for a scalar
		/// </summary>
		/// <param name="matrix">Matrix4 to multiplicate</param>
		/// <param name="scalar">Scalar value to multiplicate the matrix with</param>
		/// <returns>Matrix4 result of division</returns>
		public static Matrix4 operator /(double scalar, Matrix4 matrix)
		{
			Matrix4 result = new Matrix4(matrix);
			for (int i = 0; i < 4; ++i)
				for (int j = 0; j < 4; ++j)
					result.m[i, j] /= scalar;
			return result;
		}

		#endregion
		#endregion

		*/

		#region WTF is this?
		/*

		#region Public Static Mehods

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Matrix4 Add(Matrix4 value1, Matrix4 value2)
		{
			return new Matrix4(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z, value1.W + value2.W);
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Matrix4 Divide(Matrix4 value, double divider)
		{
			return new Matrix4(value.X / divider, value.Y / divider, value.Z / divider, value.W / divider);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double Dot(Matrix4 value1, Matrix4 value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z + value1.W * value2.W;
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scaleFactor">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Matrix4 Multiply(Matrix4 value, double scaleFactor)
		{
			return new Matrix4(value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor, value.W * scaleFactor);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Matrix4 Negate(Matrix4 value)
		{
			return new Matrix4(-value.X, -value.Y, -value.Z, -value.W);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Matrix4 Substract(Matrix4 value1, Matrix4 value2)
		{
			return new Matrix4(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z, value1.W - value2.W);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Matrix4 operator !(Matrix4 value)
		{
			return new Matrix4(-value.X, -value.Y, -value.Z, -value.W);
		}

		/// <summary>
		/// Tests vectors for equality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are equal; false otherwise</returns>
		public static bool operator ==(Matrix4 value1, Matrix4 value2)
		{
			return (value1.X == value2.X) && (value1.Y == value2.Y) && (value1.Z == value2.Z) && (value1.W == value2.W);
		}

		/// <summary>
		/// Tests vectors for inequality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are not equal; false otherwise</returns>
		public static bool operator !=(Matrix4 value1, Matrix4 value2)
		{
			return (value1.X != value2.X) || (value1.Y != value2.Y) || (value1.Z != value2.Z) || (value1.W != value2.W);
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Matrix4 operator +(Matrix4 value1, Matrix4 value2)
		{
			return new Matrix4(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z, value1.W + value2.W);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Matrix4 operator -(Matrix4 value1, Matrix4 value2)
		{
			return new Matrix4(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z, value1.W - value2.W);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scaleFactor">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Matrix4 operator *(Matrix4 value, double scaleFactor)
		{
			return new Matrix4(value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor, value.W * scaleFactor);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="scaleFactor">Scalar value</param>
		/// <param name="value">Source vector</param>
		/// <returns>Result of the multiplication</returns>
		public static Matrix4 operator *(double scaleFactor, Matrix4 value)
		{
			return new Matrix4(value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor, value.W * scaleFactor);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double operator *(Matrix4 value1, Matrix4 value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z + value1.W * value2.W;
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Matrix4 operator /(Matrix4 value1, double divider)
		{
			return new Matrix4(value1.X / divider, value1.Y / divider, value1.Z / divider, value1.W / divider);
		}

		/// <summary>
		/// Divides the components of a vector by the components of another vector.
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="divider">Divisor vector</param>
		/// <returns>The result of dividing the vectors</returns>
		/// <remarks>Division of a vector by another vector is not mathematically defined.
		/// This method simply divides each component of a by the matching component of b.</remarks>
		public static Matrix4 operator /(Matrix4 value1, Matrix4 value2)
		{
			return new Matrix4(value1.X / value2.X, value1.Y / value2.Y, value1.Z / value2.Z, value1.W / value2.W);
		}

		/// <summary>
		/// Gets the equivalent R3 vector (sets the z-axis and w-axis coordinates to Zero)
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 2-coordinate equivalent vector</returns>
		public static implicit operator Vector2(Matrix4 value)
		{
			return new Matrix4(value.X, value.Y, 0, 0);
		}

		/// <summary>
		/// Gets the equivalent R3 vector (sets the w-axis coordinate to Zero)
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 2-coordinate equivalent vector</returns>
		public static implicit operator Vector3(Matrix4 value)
		{
			return new Matrix4(value.X, value.Y, value.Z, 0);
		}

		/// <summary>
		/// Gets the lenght of the vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The length of the vector</returns>
		public static explicit operator double(Matrix4 value)
		{
			return value.Length;
		}

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>String that represents the object</returns>
		public static explicit operator string(Matrix4 value)
		{
			return "(" + value.X.ToString("0.0000") + "," + value.Y.ToString("0.0000") + "," + value.Z.ToString("0.0000") + "," + value.W.ToString("0.0000") + ")";
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Turns the current vector into a unit vector
		/// </summary>
		public void Normalize()
		{
			double length = Length;
			X /= length;
			Y /= length;
			Z /= length;
		}

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <returns>String that represents the object</returns>
		public override string ToString()
		{
			return "{ X: " + X.ToString("0.0000") + ", Y: " + Y.ToString("0.0000") + ", Z: " + Z.ToString("0.0000") + ", Z: " + W.ToString("0.0000") + " }";
		}

		/// <summary>
		/// Returns a value that indicates whether the current instance is equal to a specified object
		/// </summary>
		/// <param name="obj">Object to make the comparison with</param>
		/// <returns>true if the current instance is equal to the specified object; false otherwise</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		/// <summary>
		/// Gets the hash code of the vector object
		/// </summary>
		/// <returns>Hash code of the vector object</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		*/

		#endregion
	}
}
