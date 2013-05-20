using System;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Represents a 1 dimension Vector
	/// </summary>
	[SerializableAttribute]
	public class Vector2 : Vector
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of Vector2
		/// </summary>
		public Vector2()
			: this(0, 0)
		{ }

		/// <summary>
		/// Initializes a new instance of Vector2
		/// </summary>
		/// <param name="value">Value to initialize both components to</param>
		public Vector2(double value)
			: this(value, value)
		{}

		/// <summary>
		/// Creates a Vector2 from a Vector with first 2 elements of provided vector or 0 if vector is smaller
		/// </summary>
		/// <param name="value">N dimensions base vector</param>
		public Vector2(Vector value)
			: this(0, 0)
		{
			if (value == null)
				return;
			for (int i = 0; i < 2; ++i)
			{
				this.v[i] = i < value.Dimension ? value[i] : 0;
			}
		}

		/// <summary>
		/// Initializes a new instance of Vector2
		/// </summary>
		/// <param name="x">Initial value for the x-component of the vector</param>
		/// <param name="y">Initial value for the y-component of the vector</param>
		public Vector2(double x, double y)
			: base(2)
		{
			v[0] = x;
			v[1] = y;
		}
		#endregion

		#region Public Static Properties

		/// <summary>
		/// Returns the unit vector for the x-axis
		/// </summary>
		public static Vector2 UnitX
		{
			get { return new Vector2(1, 0); }
		}

		/// <summary>
		/// Returns the unit vector for the y-axis
		/// </summary>
		public static Vector2 UnitY
		{
			get { return new Vector2(0, 1); }
		}
		#endregion

		#region Properties

		/// <summary>
		/// Gets the unitary vector of the vector
		/// </summary>
		public new Vector2 Unitary
		{
			get
			{
				double magnitude = Magnitude;
				return new Vector2(this.X / magnitude, this.Y / magnitude);
			}
		}

		/// <summary>
		/// X coordinate of the vector (First dimension)
		/// </summary>
		public double X
		{
			get { return v[0]; }
			set { v[0] = value; OnComponentChanged(); }
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
		/// Returns the inclination angle of the vector
		/// </summary>
		public double Angle
		{
			get { return Math.Atan2(Y, X); }
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises when the value of a component of the vector changes
		/// </summary>
		public new event VectorComponentChangedEH<Vector2> ComponentChanged;

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
		/// Returns a Vector2 with both of its components set to one
		/// </summary>
		public static new Vector2 One
		{
			get
			{
				return new Vector2(1);
			}
		}

		/// <summary>
		/// Returns a Vector2 with all of its components set to zero
		/// </summary>
		public static new Vector2 Zero
		{
			get
			{
				return new Vector2(0);
			}
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Vector2 Add(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X + value2.X, value1.Y + value2.Y);
		}

		/// <summary>
		/// Calculates the cross product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Cross product of the source vectors</returns>
		public static Vector3 Cross(Vector2 value1, Vector2 value2)
		{
			return new Vector3(0, 0, value1.X * value2.Y - value1.Y * value2.X);
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Vector2 Divide(Vector2 value1, double divider)
		{
			return new Vector2(value1.X / divider, value1.Y / divider);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double Dot(Vector2 value1, Vector2 value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y;
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scaleFactor">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector2 Multiply(Vector2 value, double scaleFactor)
		{
			return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Vector2 Negate(Vector2 value)
		{
			return new Vector2(-value.X, -value.Y);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Vector2 Substract(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X - value2.X, value1.Y - value2.Y);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Vector2 operator !(Vector2 value)
		{
			return new Vector2(-value.X, -value.Y);
		}

		/// <summary>
		/// Tests vectors for equality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are equal; false otherwise</returns>
		public static bool operator ==(Vector2 value1, Vector2 value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return true;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return false;

			return (value1.X == value2.X) && (value1.Y == value2.Y);
		}

		/// <summary>
		/// Tests vectors for inequality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are not equal; false otherwise</returns>
		public static bool operator !=(Vector2 value1, Vector2 value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return false;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return true;

			return (value1.X != value2.X) || (value1.Y != value2.Y);
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Vector2 operator +(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X + value2.X, value1.Y + value2.Y);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Vector2 operator -(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X - value2.X, value1.Y - value2.Y);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scaleFactor">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector2 operator *(Vector2 value, double scaleFactor)
		{
			return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double operator *(Vector2 value1, Vector2 value2)
		{
			return value1.X * value2.X + value1.Y * value2.Y;
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="scaleFactor">Scalar value</param>
		/// <param name="value">Source vector</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector2 operator *(double scaleFactor, Vector2 value)
		{
			return new Vector2(value.X * scaleFactor, value.Y * scaleFactor);
		}

		/// <summary>
		/// Calculates the cross product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Cross product of the source vectors</returns>
		public static Vector3 operator ^(Vector2 value1, Vector2 value2)
		{
			return new Vector3(0, 0, value1.X * value2.Y - value1.Y * value2.X);
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Vector2 operator /(Vector2 value1, double divider)
		{
			return new Vector2(value1.X / divider, value1.Y / divider);
		}

		/// <summary>
		/// Divides the components of a vector by the components of another vector.
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Divisor vector</param>
		/// <returns>The result of dividing the vectors</returns>
		/// <remarks>Division of a vector by another vector is not mathematically defined.
		/// This method simply divides each component of a by the matching component of b.</remarks>
		public static Vector2 operator /(Vector2 value1, Vector2 value2)
		{
			return new Vector2(value1.X / value2.X, value1.Y / value2.Y);
		}

		/// <summary>
		/// Converts the Vector2 to a System.Drawing.PointF structure
		/// </summary>
		/// <param name="value">Vector2 to convert</param>
		/// <returns>System.Drawing.PointF structure</returns>
		public static implicit operator System.Drawing.PointF(Vector2 value)
		{
			return new System.Drawing.PointF((float)value.X, (float)value.Y);
		}

		/// <summary>
		/// Gets the lenght of the vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The length of the vector</returns>
		public static explicit operator double(Vector2 value)
		{
			return value.Magnitude;
		}

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>String that represents the object</returns>
		public static explicit operator string(Vector2 value)
		{
			return "(" + value.X.ToString("0.0000") + "," + value.Y.ToString("0.0000") + ")";
		}

		/// <summary>
		/// Converts a Vector2 to its Polar representation
		/// </summary>
		/// <param name="value">Source value.</param>
		/// <returns>Vector in radius (X) angle (Y) representation</returns>
		public static Vector2 Polar(Vector2 value)
		{
			return new Vector2(value.Magnitude, value.Angle);
		}

		/// <summary>
		/// Converts a Vector2 to its Cartesian representation
		/// </summary>
		/// <param name="value">Source value.</param>
		/// <returns>Vector in cartesian representation</returns>
		public static Vector2 Cartesian(Vector2 value)
		{
			return new Vector2(value.X * Math.Cos(value.Y), value.X * Math.Sin(value.Y));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <returns>String that represents the object</returns>
		public override string ToString()
		{
			return "{ X: " + X.ToString("0.0000") + ", Y: " + Y.ToString("0.0000") + " }";
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
