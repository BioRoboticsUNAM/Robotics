using System;
using System.Xml.Serialization;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Represents a 3 dimension Vector
	/// </summary>
	[SerializableAttribute]
	[XmlRoot("Vector3")]
	public class Vector3 : Vector
	{

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Vector3
		/// </summary>
		public Vector3()
			: this(0, 0, 0)
		{ }

		/// <summary>
		/// Initializes a new instance of Vector3
		/// </summary>
		/// <param name="value">Value to initialize both components to</param>
		public Vector3(double value)
			: this(value, value, value)
		{ }

		/// <summary>
		/// Creates a Vector3 from a Vector2 with the z-axis value set to zero
		/// </summary>
		/// <param name="value">Two dimensions base vector</param>
		public Vector3(Vector2 value)
			: this(0, 0, 0)
		{
			if (value != null)
			{
				this.v[0] = value.v[0];
				this.v[1] = value.v[1];
			}
		}

		/// <summary>
		/// Creates a Vector3 from a Vector with first 3 elements of provided vector or 0 if vector is smaller
		/// </summary>
		/// <param name="value">N dimensions base vector</param>
		public Vector3(Vector value)
			: this(0, 0, 0)
		{
			if (value == null)
				return;
			for (int i = 0; i < 3; ++i)
			{
				this.v[i] = i < value.Dimension ? value[i] : 0;
			}
		}

		/// <summary>
		/// Initializes a new instance of Vector3
		/// </summary>
		/// <param name="x">Initial value for the x-component of the vector</param>
		/// <param name="y">Initial value for the y-component of the vector</param>
		/// <param name="z">Initial value for the z-component of the vector</param>
		public Vector3(double x, double y, double z)
			: base(3)
		{
			v[0] = x;
			v[1] = y;
			v[2] = z;
		}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Returns the unit vector for the x-axis
		/// </summary>
		public static Vector3 UnitX
		{
			get { return new Vector3(1, 0, 0); }
		}

		/// <summary>
		/// Returns the unit vector for the y-axis
		/// </summary>
		public static Vector3 UnitY
		{
			get { return new Vector3(0, 1, 0); }
		}

		/// <summary>
		/// Returns the unit vector for the z-axis
		/// </summary>
		public static Vector3 UnitZ
		{
			get { return new Vector3(0, 0, 1); }
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the unitary vector of the vector
		/// </summary>
		public new Vector3 Unitary
		{
			get
			{
				double magnitude = Magnitude;
				return new Vector3(this.X / magnitude, this.Y / magnitude, this.Z/magnitude);
			}
		}

		/// <summary>
		/// X coordinate of the vector (First dimension)
		/// </summary>
		public double X
		{
			get { return v[0]; }
			set { v[0] = value;
			OnComponentChanged();
		}
		}

		/// <summary>
		/// Y coordinate of the vector (Second dimension)
		/// </summary>
		public double Y
		{
			get { return v[1]; }
			set { v[1] = value; OnComponentChanged(); }
		}

		/// <summary>
		/// Z coordinate of the vector (Third dimension)
		/// </summary>
		public double Z
		{
			get { return v[2]; }
			set { v[2] = value; OnComponentChanged(); }
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises when the value of a component of the vector changes
		/// </summary>
		public new event VectorComponentChangedEH<Vector3> ComponentChanged;

		#endregion

		#region Methods

		/// <summary>
		/// Raises the ComponentChanged event
		/// </summary>
		protected override void OnComponentChanged()
		{
			if (ComponentChanged != null)
				ComponentChanged(this);
		}

		#endregion

		#region Public Static Mehods

		/// <summary>
		/// Returns a Vector3 with both of its components set to one
		/// </summary>
		public static new Vector3 One
		{
			get
			{
				return new Vector3(1);
			}
		}

		/// <summary>
		/// Returns a Vector3 with all of its components set to zero
		/// </summary>
		public static new Vector3 Zero
		{
			get
			{
				return new Vector3(0);
			}
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Vector3 Add(Vector3 value1, Vector3 value2)
		{
			return new Vector3(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
		}

		/// <summary>
		/// Calculates the cross product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Cross product of the source vectors</returns>
		public static Vector3 Cross(Vector3 value1, Vector3 value2)
		{
			return new Vector3(
				value1.Y * value2.Z - value1.Z * value2.Y,
				value1.Z * value2.X - value1.X * value2.Z,
				value1.X * value2.Y - value1.Y * value2.X);
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Vector3 Divide(Vector3 value, double divider)
		{
			return new Vector3(value.X / divider, value.Y / divider, value.Z/divider);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double Dot(Vector3 value1, Vector3 value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z;
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scaleFactor">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector3 Multiply(Vector3 value, double scaleFactor)
		{
			return new Vector3(value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Vector3 Negate(Vector3 value)
		{
			return new Vector3(-value.X, -value.Y, -value.Z);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Vector3 Substract(Vector3 value1, Vector3 value2)
		{
			return new Vector3(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Vector3 operator !(Vector3 value)
		{
			return new Vector3(-value.X, -value.Y, -value.Z);
		}

		/// <summary>
		/// Tests vectors for equality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are equal; false otherwise</returns>
		public static bool operator ==(Vector3 value1, Vector3 value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return true;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return false;

			return (value1.X == value2.X) && (value1.Y == value2.Y) && (value1.Z == value2.Z);
		}

		/// <summary>
		/// Tests vectors for inequality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are not equal; false otherwise</returns>
		public static bool operator !=(Vector3 value1, Vector3 value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return false;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return true;

			return (value1.X != value2.X) || (value1.Y != value2.Y) || (value1.Z != value2.Z);
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Vector3 operator +(Vector3 value1, Vector3 value2)
		{
			return new Vector3(value1.X + value2.X, value1.Y + value2.Y, value1.Z + value2.Z);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Vector3 operator -(Vector3 value1, Vector3 value2)
		{
			return new Vector3(value1.X - value2.X, value1.Y - value2.Y, value1.Z - value2.Z);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scaleFactor">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector3 operator *(Vector3 value, double scaleFactor)
		{
			return new Vector3(value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="scaleFactor">Scalar value</param>
		/// <param name="value">Source vector</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector3 operator *(double scaleFactor, Vector3 value)
		{
			return new Vector3(value.X * scaleFactor, value.Y * scaleFactor, value.Z * scaleFactor);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double operator *(Vector3 value1, Vector3 value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y + value1.Z * value2.Z;
		}

		/// <summary>
		/// Calculates the cross product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Cross product of the source vectors</returns>
		public static Vector3 operator ^(Vector3 value1, Vector3 value2)
		{
			return new Vector3(
				value1.Y * value2.Z - value1.Z * value2.Y,
				value1.Z * value2.X - value1.X * value2.Z,
				value1.X * value2.Y - value1.Y * value2.X);
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Vector3 operator /(Vector3 value1, double divider)
		{
			return new Vector3(value1.X / divider, value1.Y / divider, value1.Z / divider);
		}

		/// <summary>
		/// Divides the components of a vector by the components of another vector.
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Divisor vector</param>
		/// <returns>The result of dividing the vectors</returns>
		/// <remarks>Division of a vector by another vector is not mathematically defined.
		/// This method simply divides each component of a by the matching component of b.</remarks>
		public static Vector3 operator /(Vector3 value1, Vector3 value2)
		{
			return new Vector3(value1.X / value2.X, value1.Y / value2.Y, value1.Z / value2.Z);
		}

		/// <summary>
		/// Gets the equivalent R3 vector (sets the z-axis coordinate to Zero)
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 2-coordinate equivalent vector</returns>
		public static explicit operator Vector2(Vector3 value)
		{
			return new Vector2(value.X, value.Y);
		}

		/// <summary>
		/// Gets the equivalent R3 vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 3-coordinate equivalent vector</returns>
		public static implicit operator Vector3(Vector2 value)
		{
			return new Vector3(value.X, value.Y, 0);
		}

		/// <summary>
		/// Gets the lenght of the vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The length of the vector</returns>
		public static explicit operator double(Vector3 value)
		{
			return value.Length;
		}

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>String that represents the object</returns>
		public static explicit operator string(Vector3 value)
		{
			return "(" + value.X.ToString("0.0000") + "," + value.Y.ToString("0.0000") + "," + value.Z.ToString("0.0000") + ")";
		}

		/// <summary>
		/// Converts a Vector3 from cartesian representation to spherical representation
		/// </summary>
		/// <param name="value">Source value.</param>
		/// <returns>Vector3 in spherical representation</returns>
		/// <remarks>
		/// Cartesian	Cilindric	Spherical
		///		X	=	radius	 =	rho (radius)
		/// 	Y	=	theta	 =	theta (x-y angle)
		/// 	Z	=		Z	 =	phi
		/// </remarks>
		public static Vector3 CartesianToSpherical(Vector3 value)
		{
			return new Vector3(
				value.Length,
				Math.Atan2(value.Y, value.X),
				Math.Acos(value.Z/value.Length));
		}

		/// <summary>
		/// Converts a Vector3 from cartesian representation to cilindric representation
		/// </summary>
		/// <param name="value">Source value.</param>
		/// <returns>Vector3 in cilindric representation</returns>
		/// <remarks>
		/// Cartesian	Cilindric	Spherical
		///		X	=	radius	 =	rho (radius)
		/// 	Y	=	theta	 =	theta (x-y angle)
		/// 	Z	=		Z	 =	phi
		/// </remarks>
		public static Vector3 CartesianToCilindric(Vector3 value)
		{
			return new Vector3(
				Math.Sqrt(value.X * value.X+ value.Y * value.Y),
				Math.Atan2(value.Y,value.X),
				value.Z);
		}

		/// <summary>
		/// Converts a Vector3 from spherical representation to cartesian representation
		/// </summary>
		/// <param name="value">Source value.</param>
		/// <returns>Vector3 in spherical representation</returns>
		/// <remarks>
		/// Cartesian	Cilindric	Spherical
		///		X	=	radius	 =	rho (radius)
		/// 	Y	=	theta	 =	theta (x-y angle)
		/// 	Z	=		Z	 =	phi
		/// </remarks>
		public static Vector3 SphericalToCartesian(Vector3 value)
		{
			return new Vector3(
				value.X * Math.Cos(value.Z) * Math.Cos(value.Y),
				value.X * Math.Cos(value.Z) * Math.Sin(value.Y),
				value.X * Math.Sin(value.Z));
		}

		/// <summary>
		/// Converts a Vector3 from cilindric representation to cartesian representation
		/// </summary>
		/// <param name="value">Source value.</param>
		/// <returns>Vector3 in cartesian representation</returns>
		/// <remarks>
		/// Cartesian	Cilindric	Spherical
		///		X	=	radius	 =	rho (radius)
		/// 	Y	=	theta	 =	theta (x-y angle)
		/// 	Z	=		Z	 =	phi
		/// </remarks>
		public static Vector3 CilindricToCartesian(Vector3 value)
		{
			return new Vector3(
				value.X * Math.Cos(value.Y),
				value.X * Math.Sin(value.Y),
				value.Z);
		}

		/// <summary>
		/// Converts a Vector3 from cilindric representation to spherical representation
		/// </summary>
		/// <param name="value">Source value.</param>
		/// <returns>Vector3 in cilindric representation</returns>
		/// <remarks>
		/// Cartesian	Cilindric	Spherical
		///		X	=	radius	 =	rho (radius)
		/// 	Y	=	theta	 =	theta (x-y angle)
		/// 	Z	=		Z	 =	phi
		/// </remarks>
		public static Vector3 CilindricToSpherical(Vector3 value)
		{
			return new Vector3(
				Math.Sqrt(value.X * value.X + value.Z * value.Z),
				value.Y,
				Math.Atan2( value.X, value.Z));
		}

		/// <summary>
		/// Converts a Vector3 from spherical representation to cilindric representation
		/// </summary>
		/// <param name="value">Source value.</param>
		/// <returns>Vector3 in spherical representation</returns>
		/// <remarks>
		/// Cartesian	Cilindric	Spherical
		///		X	=	radius	 =	rho (radius)
		/// 	Y	=	theta	 =	theta (x-y angle)
		/// 	Z	=		Z	 =	phi
		/// </remarks>
		public static Vector3 SphericalToCilindric(Vector3 value)
		{
			return new Vector3(
				value.X * Math.Sin(value.Y),
				value.Y,
				value.X * Math.Cos(value.Y));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <returns>String that represents the object</returns>
		public override string ToString()
		{
			return "{ X: " + X.ToString("0.0000") + ", Y: " + Y.ToString("0.0000") + ", Z: " + Z.ToString("0.0000") + " }";
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

		#endregion
	}
}
