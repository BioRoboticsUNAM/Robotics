using System;
using System.Xml.Serialization;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Represents a 4 dimension Vector
	/// </summary>
	[SerializableAttribute]
	[XmlRoot("Vector4")]
	public class Vector4 : Vector
	{

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Vector4
		/// </summary>
		public Vector4()
			: this(0, 0, 0, 0)
		{ }

		/// <summary>
		/// Initializes a new instance of Vector4
		/// </summary>
		/// <param name="value">Value to initialize both components to</param>
		public Vector4(double value)
			: this(value, value, value, value)
		{ }

		/// <summary>
		/// Creates a Vector4 from a Vector2 with the z-axis and w-axis value set to zero
		/// </summary>
		/// <param name="value">Two dimensions base vector</param>
		public Vector4(Vector2 value)
			: this(0, 0, 0, 0)
		{
			if (value != null)
			{
				this.v[0] = value.v[0];
				this.v[1] = value.v[1];
			}
		}

		/// <summary>
		/// Creates a Vector4 from a Vector3 with the w-axis value set to zero
		/// </summary>
		/// <param name="value">Three dimensions base vector</param>
		public Vector4(Vector3 value)
			: this(0, 0, 0, 0)
		{
			if (value != null)
			{
				this.v[0] = value.v[0];
				this.v[1] = value.v[1];
				this.v[2] = value.v[2];
			}
		}

		/// <summary>
		/// Creates a Vector4 from a Vector with first 4 elements of provided vector or 0 if vector is smaller
		/// </summary>
		/// <param name="value">N dimensions base vector</param>
		public Vector4(Vector value)
			: this(0, 0, 0, 0)
		{
			if (value == null)
				return;
			for (int i = 0; i < 4; ++i)
			{
				this.v[i] = i < value.Dimension ? value[i] : 0;
			}
		}

		/// <summary>
		/// Initializes a new instance of Vector4
		/// </summary>
		/// <param name="x">Initial value for the x-component of the vector</param>
		/// <param name="y">Initial value for the y-component of the vector</param>
		/// <param name="z">Initial value for the z-component of the vector</param>
		/// <param name="w">Initial value for the w-component of the vector</param>
		public Vector4(double x, double y, double z, double w)
			: base(x, y, z, w)
		{
			v[0] = x;
			v[1] = y;
			v[2] = z;
			v[3] = w;
		}
		#endregion

		#region Public Static Properties

		/// <summary>
		/// Returns the unit vector for the x-axis
		/// </summary>
		public static Vector4 UnitX
		{
			get { return new Vector4(1, 0, 0, 0); }
		}

		/// <summary>
		/// Returns the unit vector for the y-axis
		/// </summary>
		public static Vector4 UnitY
		{
			get { return new Vector4(0, 1, 0, 0); }
		}

		/// <summary>
		/// Returns the unit vector for the z-axis
		/// </summary>
		public static Vector4 UnitZ
		{
			get { return new Vector4(0, 0, 1, 0); }
		}

		/// <summary>
		/// Returns the unit vector for the z-axis
		/// </summary>
		public static Vector4 UnitW
		{
			get { return new Vector4(0, 0, 0, 1); }
		}
		#endregion

		#region Properties

		/// <summary>
		/// Gets the unitary vector of the vector
		/// </summary>
		public new Vector4 Unitary
		{
			get
			{
				double magnitude = Magnitude;
				Vector4 u = new Vector4();
				u.v[0] = this.v[0] / magnitude;
				u.v[1] = this.v[1] / magnitude;
				u.v[2] = this.v[2] / magnitude;
				u.v[3] = this.v[3] / magnitude;
				return u;
			}
		}

		/// <summary>
		/// Gets or sets the W coordinate of the vector (Fourth dimension)
		/// </summary>
		public double W { get { return v[3]; } set { v[3] = value; OnComponentChanged(); } }
		/// <summary>
		/// Gets or sets the X coordinate of the vector (First dimension)
		/// </summary>
		public double X { get { return v[0]; } set { v[0] = value; OnComponentChanged(); } }
		/// <summary>
		/// Gets or sets Y coordinate of the vector (Second dimension)
		/// </summary>
		public double Y { get { return v[1]; } set { v[1] = value; OnComponentChanged(); } }
		/// <summary>
		/// Gets or sets Z coordinate of the vector (Third dimension)
		/// </summary>
		public double Z { get { return v[2]; } set { v[2] = value; OnComponentChanged(); } }

		#endregion

		#region Events

		/// <summary>
		/// Raises when the value of a component of the vector changes
		/// </summary>
		public new event VectorComponentChangedEH<Vector4> ComponentChanged;

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
		/// Returns a Vector with both of its components set to one
		/// </summary>
		public static new Vector4 One
		{
			get{
			return new Vector4(1);
			}
		}

		/// <summary>
		/// Returns a Vector with all of its components set to zero
		/// </summary>
		public static new Vector4 Zero
		{
			get
			{
				return new Vector4(0);
			}
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Vector4 Add(Vector4 value1, Vector4 value2)
		{
			return new Vector4(value1.v[0] + value2.v[0], value1.v[1] + value2.v[1], value1.v[2] + value2.v[2], value1.v[3] + value2.v[3]);
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Vector4 Divide(Vector4 value, double divider)
		{
			return new Vector4(value.v[0] / divider, value.v[1] / divider, value.v[2] / divider, value.v[3] / divider);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double Dot(Vector4 value1, Vector4 value2)
		{
			return value1.v[0] * value2.v[0] + value1.v[1] * value2.v[1] + value1.v[2] * value2.v[2] + value1.v[3] * value2.v[3];
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scaleFactor">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector4 Multiply(Vector4 value, double scaleFactor)
		{
			return new Vector4(value.v[0] * scaleFactor, value.v[1] * scaleFactor, value.v[2] * scaleFactor, value.v[3] * scaleFactor);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Vector4 Negate(Vector4 value)
		{
			return new Vector4(-value.v[0], -value.v[1], -value.v[2], -value.v[3]);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Vector4 Substract(Vector4 value1, Vector4 value2)
		{
			return new Vector4(value1.v[0] - value2.v[0], value1.v[1] - value2.v[1], value1.v[2] - value2.v[2], value1.v[3] - value2.v[3]);
		}

		/// <summary>
		/// Returns a vector pointing in the opposite direction
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>Vector pointing in the opposite direction</returns>
		public static Vector4 operator !(Vector4 value)
		{
			return new Vector4(-value.v[0], -value.v[1], -value.v[2], -value.v[3]);
		}

		/// <summary>
		/// Tests vectors for equality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are equal; false otherwise</returns>
		public static bool operator ==(Vector4 value1, Vector4 value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return true;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return false;

			return (value1.v[0] == value2.v[0]) && (value1.v[1] == value2.v[1]) && (value1.v[2] == value2.v[2]) && (value1.v[3] == value2.v[3]);
		}

		/// <summary>
		/// Tests vectors for inequality
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>true if the vectors are not equal; false otherwise</returns>
		public static bool operator !=(Vector4 value1, Vector4 value2)
		{
			if (Object.Equals(value1, null) && Object.Equals(value2, null))
				return false;
			if (Object.Equals(value1, null) | Object.Equals(value2, null))
				return true;

			return (value1.v[0] != value2.v[0]) || (value1.v[1] != value2.v[1]) || (value1.v[2] != value2.v[2]) || (value1.v[3] != value2.v[3]);
		}

		/// <summary>
		/// Adds two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Sum of the source vectors</returns>
		public static Vector4 operator +(Vector4 value1, Vector4 value2)
		{
			return new Vector4(value1.v[0] + value2.v[0], value1.v[1] + value2.v[1], value1.v[2] + value2.v[2], value1.v[3] + value2.v[3]);
		}

		/// <summary>
		/// Subtracts a vector from a vector
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>Result of the subtraction</returns>
		public static Vector4 operator -(Vector4 value1, Vector4 value2)
		{
			return new Vector4(value1.v[0] - value2.v[0], value1.v[1] - value2.v[1], value1.v[2] - value2.v[2], value1.v[3] - value2.v[3]);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <param name="scaleFactor">Scalar value</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector4 operator *(Vector4 value, double scaleFactor)
		{
			return new Vector4(value.v[0] * scaleFactor, value.v[1] * scaleFactor, value.v[2] * scaleFactor, value.v[3] * scaleFactor);
		}

		/// <summary>
		/// Multiplies a vector by a scalar value
		/// </summary>
		/// <param name="scaleFactor">Scalar value</param>
		/// <param name="value">Source vector</param>
		/// <returns>Result of the multiplication</returns>
		public static Vector4 operator *(double scaleFactor, Vector4 value)
		{
			return new Vector4(value.v[0] * scaleFactor, value.v[1] * scaleFactor, value.v[2] * scaleFactor, value.v[3] * scaleFactor);
		}

		/// <summary>
		/// Calculates the dot product of two vectors
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Source vector</param>
		/// <returns>The dot product of the two vectors</returns>
		public static double operator *(Vector4 value1, Vector4 value2)
		{
			return value1.v[0] * value2.v[0] + value1.v[1] * value2.v[1] + value1.v[2] * value2.v[2] + value1.v[3] * value2.v[3];
		}

		/// <summary>
		/// Divides a vector by a scalar value
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="divider">The divisor</param>
		/// <returns>The source vector divided by b</returns>
		public static Vector4 operator /(Vector4 value1, double divider)
		{
			return new Vector4(value1.v[0] / divider, value1.v[1] / divider, value1.v[2] / divider, value1.v[3] / divider);
		}

		/// <summary>
		/// Divides the components of a vector by the components of another vector.
		/// </summary>
		/// <param name="value1">Source vector</param>
		/// <param name="value2">Divisor vector</param>
		/// <returns>The result of dividing the vectors</returns>
		/// <remarks>Division of a vector by another vector is not mathematically defined.
		/// This method simply divides each component of a by the matching component of b.</remarks>
		public static Vector4 operator /(Vector4 value1, Vector4 value2)
		{
			return new Vector4(value1.v[0] / value2.v[0], value1.v[1] / value2.v[1], value1.v[2] / value2.v[2], value1.v[3] / value2.v[3]);
		}

		#region Casts

		/// <summary>
		/// Gets the equivalent R3 vector (sets the z-axis and w-axis coordinates to v[2]ero)
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 2-coordinate equivalent vector</returns>
		public static explicit operator Vector2(Vector4 value)
		{
			return new Vector2(value.v[0], value.v[1]);
		}

		/// <summary>
		/// Gets the equivalent R3 vector (sets the w-axis coordinate to v[2]ero)
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 2-coordinate equivalent vector</returns>
		public static explicit operator Vector3(Vector4 value)
		{
			return new Vector3(value.v[0], value.v[1], value.v[2]);
		}

		/// <summary>
		/// Gets the equivalent R4 vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 4-coordinate equivalent vector</returns>
		public static implicit operator Vector4(Vector2 value)
		{
			return new Vector4(value);
		}

		/// <summary>
		/// Gets the equivalent R4 vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The 4-coordinate equivalent vector</returns>
		public static implicit operator Vector4(Vector3 value)
		{
			return new Vector4(value);
		}

		/// <summary>
		/// Gets the lenght of the vector
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>The length of the vector</returns>
		public static explicit operator double(Vector4 value)
		{
			return value.Length;
		}

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <param name="value">Source vector</param>
		/// <returns>String that represents the object</returns>
		public static explicit operator string(Vector4 value)
		{
			return "(" + value.v[0].ToString("0.0000") + "," + value.v[1].ToString("0.0000") + "," + value.v[2].ToString("0.0000") + "," + value.v[3].ToString("0.0000") + ")";
		}

		#endregion

		#endregion

		#region Public Methods

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <returns>String that represents the object</returns>
		public override string ToString()
		{
			return "{ v[0]: " + v[0].ToString("0.0000") + ", v[1]: " + v[1].ToString("0.0000") + ", v[2]: " + v[2].ToString("0.0000") + ", v[3]: " + v[3].ToString("0.0000") + " }";
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