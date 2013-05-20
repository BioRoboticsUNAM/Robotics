using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Robotics.Mathematics
{
	#region Delegates 

	/// <summary>
	/// Represents the function that handles the ComponentChanged event of a Vector
	/// </summary>
	/// <param name="v"></param>
	public delegate void VectorComponentChangedEH<T>(T v) where T : Vector;

	#endregion

	/// <summary>
	/// Represents a n dimension Vector
	/// </summary>
	[SerializableAttribute]
	[XmlRoot("Vector")]
	//public class Vector : ISerializable
	public class Vector
	{
		#region Fields
		/// <summary>
		/// Array that stores the data of the vector
		/// </summary>
		public readonly double[] v;
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Vector
		/// </summary>
		/// <param name="dimension">Dimension of the vector</param>
		public Vector(int dimension)
			: this(dimension, 0) { }

		/// <summary>
		/// Initializes a new instance of Vector
		/// </summary>
		/// <param name="dimension">Dimension of the vector</param>
		/// <param name="value">Value to initialize both components to</param>
		public Vector(int dimension, double value)
		{
			if (dimension < 0)
				throw new ArgumentOutOfRangeException();
			v = new double[dimension];
			for (int i = 0; i < v.Length; ++i)
				v[i] = value;
		}

		/// <summary>
		/// Creates a Vector of dimension 2 from a Vector2
		/// </summary>
		/// <param name="value">Two dimensions base vector</param>
		public Vector(Vector2 value)
			: this(2)
		{
			if (value == null) return;
			v[0] = value.X;
			v[1] = value.Y;
		}

		/// <summary>
		/// Creates a Vector of dimension 3 from a Vector3
		/// </summary>
		/// <param name="value">Three dimensions base vector</param>
		public Vector(Vector3 value)
			: this(3)
		{
			if (value == null) return;
			v[0] = value.X;
			v[1] = value.Y;
			v[2] = value.Z;
		}

		/// <summary>
		/// Creates a Vector of dimension 4 from a Vector4
		/// </summary>
		/// <param name="value">Four dimensions base vector</param>
		public Vector(Vector4 value)
			: this(4)
		{
			if (value == null) return;
			v[0] = value.X;
			v[1] = value.Y;
			v[2] = value.Z;
			v[3] = value.W;
		}

		/// <summary>
		/// Initializes a new instance of Vector
		/// </summary>
		/// <param name="value">Initial values for the vector</param>
		public Vector(params double[] value)
		{
			if (value == null)
				throw new ArgumentNullException();
			v = new double[value.Length];
			for (int i = 0; i < v.Length; ++i)
				v[i] = value[i];
		}

		/// <summary>
		/// Creates a Vector from another Vector
		/// This is a copy constructor
		/// </summary>
		/// <param name="value">Base vector</param>
		public Vector(Vector value)
		{
			if (value == null)
			{
				v = new double[0];
				return;
			}
			v = new double[value.Dimension];
			for (int i = 0; i < v.Length; ++i)
				v[i] = value[i];
		}

		#endregion

		#region Properties

		/// <summary>
		/// Calculates the dimension of the vector
		/// </summary>
		public int Dimension
		{
			get
			{
				return v.Length;
			}
		}

		/// <summary>
		/// Calculates the number of elements of the vector
		/// </summary>
		public int Length
		{
			get
			{
				return v.Length;
			}
		}

		/// <summary>
		/// Calculates the n-dimensional cartesian length of the vector
		/// </summary>
		public double Magnitude
		{
			get
			{
				double magnitude = 0;
				for (int i = 0; i < v.Length; ++i)
					magnitude += v[i] * v[i];
				return Math.Sqrt(magnitude);
			}
		}

		/// <summary>
		/// Calculates Norm1 (sum of the absolute values of each component) of the vector
		/// </summary>
		public double Norm1
		{
			get
			{
				double magnitude = 0;
				for (int i = 0; i < v.Length; ++i)
					magnitude += Math.Abs(v[i]);
				return magnitude;
			}
		}

		/// <summary>
		/// Calculates Norm2 (the n-dimensional cartesian length) of the vector
		/// </summary>
		public double Norm2
		{
			get
			{
				double magnitude = 0;
				for (int i = 0; i < v.Length; ++i)
					magnitude += v[i] * v[i];
				return Math.Sqrt(magnitude);
			}
		}

		/// <summary>
		/// Calculates Infinite Norm (maximum component value) of the vector
		/// </summary>
		public double NormInfinite
		{
			get
			{
				double magnitude = -1;
				double val;
				for (int i = 0; i < v.Length; ++i)
				{
					val = Math.Abs(v[i]);
					if (val > magnitude) magnitude = val;
				}
				return magnitude;
			}
		}

		/// <summary>
		/// Gets the unitary vector of the vector
		/// </summary>
		public virtual Vector Unitary
		{
			get
			{
				return this / Magnitude;
			}
		}

		/// <summary>
		/// Calculates the dimension of the vector
		/// </summary>
		public int Size
		{
			get
			{
				return v.Length;
			}
		}

		#endregion

		#region Inexers

		/// <summary>
		/// Gets or sets the value at specified location
		/// </summary>
		/// <param name="index">Index of desired element</param>
		/// <returns></returns>
		public double this[int index]
		{
			get
			{
				if ((index < 0) || (index >= v.Length))
					throw new ArgumentException("Index must be between 0 and " + v.Length, "index");
				return v[index];
			}
			set
			{
				if ((index < 0) || (index >= v.Length))
					throw new ArgumentException("Index must be between 0 and " + v.Length, "index");
				v[index] = value;
				OnComponentChanged();
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises when the value of a component of the vector changes
		/// </summary>
		public event VectorComponentChangedEH<Vector> ComponentChanged;

		#endregion

		#region Methods

		/// <summary>
		/// Raises the ComponentChanged event
		/// </summary>
		protected virtual void OnComponentChanged()
		{
			if (ComponentChanged != null)
				ComponentChanged(this);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Turns the current vector into a unit vector
		/// </summary>
		public void Normalize()
		{
			double magnitude = Magnitude;
			for (int i = 0; i < v.Length; ++i)
				v[i] = v[i] / magnitude;
		}

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <returns>String that represents the object</returns>
		public override string ToString()
		{
			int i;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			//sb.Append("{");
			//for (i = 0; i < v.Length- 1; ++i)
			//{
			//	sb.Append("{ v[");
			//	sb.Append(i);
			//	sb.Append("]: ");
			//	sb.Append(v[i].ToString("0.0000"));
			//	sb.Append(v[i].ToString(","));
			//}
			//sb.Append("{ v[");
			//sb.Append(i);
			//sb.Append("]: ");
			//sb.Append(v[i].ToString("0.0000"));
			//sb.Append("}");
			//return sb.ToString();

			sb.Append("{ ");
			for (i = 0; i < v.Length - 1; ++i)
			{
				sb.Append(v[i].ToString("0.0000"));
				sb.Append(", ");
			}
			if (i < v.Length)
				sb.Append(v[i].ToString("0.0000"));
			sb.Append(" }");
			return sb.ToString();
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
			return v.GetHashCode();
		}

		#region ISerializable Members
		/*
		/// <summary>
		/// Reads a SerializationInfo with the data needed to deserialize the object
		/// </summary>
		/// <param name="info">The SerializationInfo with data to populate the object.</param>
		/// <param name="context">The source (see StreamingContext) for this serialization</param>
		public Vector(SerializationInfo info, StreamingContext context)
		{
		}

		/// <summary>
		/// Populates a SerializationInfo with the data needed to serialize the target object
		/// </summary>
		/// <param name="info">The SerializationInfo to populate with data.</param>
		/// <param name="context">The destination (see StreamingContext) for this serialization</param>
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new Exception("The method or operation is not implemented.");
		}
		*/

		#endregion

		#endregion

		#region Public Static Mehods

		/// <summary>
		/// Returns a Vector with both of its components set to one
		/// </summary>
		/// <param name="dimension">Dimension of the vector</param>
		public static Vector One(int dimension)
		{
			return new Vector(dimension, 1);
		}

		/// <summary>
		/// Returns a Vector with all of its components set to zero
		/// </summary>
		/// <param name="dimension">Dimension of the vector</param>
		public static Vector Zero(int dimension)
		{
			return new Vector(dimension, 0);
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Vector Add(Vector value1, Vector value2)
		{
			if ((value1 == null) || (value2 == null))
				throw new ArgumentNullException();
			if (value1.Dimension != value2.Dimension)
				throw new ArgumentException("Incompatible vectors");
			double[] result = new double[value1.Dimension];
			for (int i = 0; i < value1.Dimension; ++i)
				result[i] = value1[i] + value2[i];
			return new Vector(result);
		}

		/// <summary>
		/// Calculates the Canberra Distance between two vectors.
		/// The Canberra distance between two vectors is the sum of abs(pi - qi)/(abs(pi) + abs(qi))
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The Canberra Distance between the vectors.</returns>
		public static double CanberraDistance(Vector value1, Vector value2)
		{
			if ((value1 == null) || (value2 == null))
				throw new ArgumentNullException();
			if (value1.Dimension != value2.Dimension)
				throw new ArgumentException("Incompatible vectors");
			double distance = 0;
			double diff;
			double sum;

			for (int i = 0; i < value1.Dimension; ++i)
			{
				diff = Math.Abs(value1[i] - value2[i]);
				sum = Math.Abs(value1[i]) + Math.Abs(value2[i]);
				distance+= diff / sum;
			}
			return distance;
		}

		/// <summary>
		/// Calculates the Chebyshev Distance between two vectors.
		/// The Chebyshev distance between two vectors is the greatest of their differences along any coordinate dimension.
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The Chebyshev Distance between the vectors.</returns>
		public static double ChebyshevDistance(Vector value1, Vector value2)
		{
			if ((value1 == null) || (value2 == null))
				throw new ArgumentNullException();
			if (value1.Dimension != value2.Dimension)
				throw new ArgumentException("Incompatible vectors");
			double distance = 0;
			double d;

			for (int i = 0; i < value1.Dimension; ++i)
			{
				d = Math.Abs(value1[i] - value2[i]);
				if (d > distance)
					distance = d;
			}
			return distance;
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scalar">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Vector Divide(Vector value, double scalar)
		{
			if (value == null)
				throw new ArgumentNullException();

			double[] result = new double[value.v.Length];
			for (int i = 0; i < value.v.Length; ++i)
				result[i] = value[i] / scalar;
			return new Vector(result);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double Dot(Vector value1, Vector value2)
		{
			if ((value1 == null) || (value2 == null))
				throw new ArgumentNullException();

			if (value1.Dimension != value2.Dimension)
				throw new ArgumentException("Incompatible vectors");

			double result = 0;
			for (int i = 0; i < value1.Dimension; ++i)
				result += value1[i] * value2[i];
			return result;
		}

		/// <summary>
		/// Calculates the Manhattan Distance between two vectors.
		/// The Manhattan distance between two vectors is the sum of the absolute differences of their coordinates.
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The Manhattan Distance between the vectors.</returns>
		public static double ManhattanDistance(Vector value1, Vector value2)
		{
			if ((value1 == null) || (value2 == null))
				throw new ArgumentNullException();
			if (value1.Dimension != value2.Dimension)
				throw new ArgumentException("Incompatible vectors");
			double distance = 0;

			for (int i = 0; i < value1.Dimension; ++i)
				distance += Math.Abs(value1[i] - value2[i]);
			return distance;
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scalar">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector Multiply(Vector value, double scalar)
		{
			if (value == null)
				throw new ArgumentNullException();

			double[] result = new double[value.v.Length];
			for (int i = 0; i < result.Length; ++i)
				result[i] = value[i] * scalar;
			return new Vector(result);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Vector Negate(Vector value)
		{
			if (value == null)
				throw new ArgumentNullException();

			double[] result = new double[value.Dimension];
			for (int i = 0; i < value.Dimension; ++i)
				result[i] = value[i] * -1;
			return new Vector(result);
		}

		/// <summary>
		/// Raises each component of a vector to the specified power
		/// </summary>
		/// <param name="baseVector">Source vector to be raised to a power</param>
		/// <param name="power">A double-precision floating-point number that specifies a power</param>
		/// <returns>A vector with each component raised to the power scalar</returns>
		public static Vector Pow(Vector baseVector, double power)
		{
			if (baseVector == null) return new Vector(new double[0]);
			double[] pv = new double[baseVector.Length];
			for (int i = 0; i < pv.Length; ++i)
			{
				pv[i] = Math.Pow(baseVector[i], power);
			}
			return new Vector(pv);
		}

		/// <summary>
		/// Raises a scalar to the specified power by each component of a vector
		/// </summary>
		/// <param name="baseScalar">A double-precision floating-point number to be raised to a power</param>
		/// <param name="power">A vector that specifies an array of powers</param>
		/// <returns>A vector with each component equals to he base scalar raised to the power of each vector component</returns>
		public static Vector Pow(double baseScalar, Vector power)
		{
			if (power == null) return new Vector(new double[0]);
			double[] pv = new double[power.Length];
			for (int i = 0; i < pv.Length; ++i)
			{
				pv[i] = Math.Pow(baseScalar, power[i]);
			}
			return new Vector(pv);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Vector Substract(Vector value1, Vector value2)
		{
			if ((value1 == null) || (value2 == null))
				throw new ArgumentNullException();

			if ((value1 == null) || (value2 == null))
				throw new ArgumentNullException();

			if (value1.Dimension != value2.Dimension)
				throw new ArgumentException("Incompatible vectors");
			double[] result = new double[value1.Dimension];
			for (int i = 0; i < value1.Dimension; ++i)
				result[i] = value1[i] - value2[i];
			return new Vector(result);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Vector operator !(Vector value)
		{
			return Negate(value);
		}

		/// <summary>
		/// Tests vectors for equality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are equal; false otherwise</returns>
		public static bool operator ==(Vector value1, Vector value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return true;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return false;

			if (value1.Dimension != value2.Dimension)
				return false;
			for (int i = 0; i < value1.Dimension; ++i)
				if (value1[i] != value2[i]) return false;
			return true;
		}

		/// <summary>
		/// Tests vectors for inequality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are not equal; false otherwise</returns>
		public static bool operator !=(Vector value1, Vector value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return false;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return true;

			if (value1.Dimension != value2.Dimension)
				return true;
			for (int i = 0; i < value1.Dimension; ++i)
				if (value1[i] != value2[i]) return true;
			return false;
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Vector operator +(Vector value1, Vector value2)
		{
			return Add(value1, value2);
		}

		/// <summary>
		/// Inverts the vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The source vector with each component multiplied by -1</returns>
		public static Vector operator -(Vector value)
		{
			if (value == null) return null;
			double[] v = new double[value.Length];
			for (int i = 0; i < v.Length; ++i)
				v[i] = -value[i];
			return new Vector(v);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Vector operator -(Vector value1, Vector value2)
		{
			return Substract(value1, value2);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scalar">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector operator *(Vector value, double scalar)
		{
			return Multiply(value, scalar);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="scalar">Scalar value</param>
		/// <param name="value">Source vector</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector operator *(double scalar, Vector value)
		{
			return Multiply(value, scalar);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double operator *(Vector value1, Vector value2)
		{
			return Dot(value1, value2);
		}

		/// <summary>
		/// Raises each component of a vector to the specified power
		/// </summary>
		/// <param name="baseVector">Source vector to be raised to a power</param>
		/// <param name="power">A double-precision floating-point number that specifies a power</param>
		/// <returns>A vector with each component raised to the power scalar</returns>
		public static Vector operator ^(Vector baseVector, double power)
		{
			return Pow(baseVector, power);
		}

		/// <summary>
		/// Raises a scalar to the specified power by each component of a vector
		/// </summary>
		/// <param name="baseScalar">A double-precision floating-point number to be raised to a power</param>
		/// <param name="power">A vector that specifies an array of powers</param>
		/// <returns>A vector with each component equals to he base scalar raised to the power of each vector component</returns>
		public static Vector operator ^(double baseScalar, Vector power)
		{
			return Pow(baseScalar, power);
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scalar">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Vector operator /(Vector value, double scalar)
		{
			return Divide(value, scalar);
		}

		#region Casts

		/*

		/// <summary>
		/// Gets the equivalent vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The N-coordinate equivalent vector</returns>
		public static implicit operator Vector(Vector2 value)
		{
			return new Vector(value);
		}

		/// <summary>
		/// Gets the equivalent vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The N-coordinate equivalent vector</returns>
		public static implicit operator Vector(Vector3 value)
		{
			return new Vector(value);
		}

		/// <summary>
		/// Gets the equivalent vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The N-coordinate equivalent vector</returns>
		public static implicit operator Vector(Vector4 value)
		{
			return new Vector(value);
		}

		/// <summary>
		/// Gets the equivalent R4 vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 4-coordinate equivalent vector</returns>
		public static implicit operator Vector4(Vector value)
		{
			if (value.Dimension < 4)
				throw new ArgumentException("Invalid conversion");
			return new Vector4(value[0], value[1], value[2], value[3]);
		}

		/// <summary>
		/// Gets the equivalent R3 vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 3-coordinate equivalent vector</returns>
		public static implicit operator Vector3(Vector value)
		{
			if (value.Dimension < 3)
				throw new ArgumentException("Invalid conversion");
			return new Vector3(value[0], value[1], value[2]);
		}

		/// <summary>
		/// Gets the equivalent R4 vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 2-coordinate equivalent vector</returns>
		public static implicit operator Vector2(Vector value)
		{
			if (value.Dimension < 2)
				throw new ArgumentException("Invalid conversion");
			return new Vector2(value[0], value[1]);
		}

		*/

		/// <summary>
		/// Gets the lenght of the vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The length of the vector</returns>
		public static explicit operator double(Vector value)
		{
			return value.Length;
		}

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>String that represents the object</returns>
		public static explicit operator string(Vector value)
		{
			int i;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("(");
			for (i = 0; i < value.Dimension - 1; ++i)
			{
				sb.Append(value.v[i].ToString("0.0000"));
				sb.Append(value.v[i].ToString(","));
			}
			sb.Append(value.v[i].ToString("0.0000"));
			sb.Append(")");
			return sb.ToString();
		}

		#endregion

		#endregion
	}
}