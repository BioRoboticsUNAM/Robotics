using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Represents a complex number
	/// </summary>
	[SerializableAttribute]
	public struct Complex
	{
		#region Variables

		/// <summary>
		/// The real part of the complex number
		/// </summary>
		public double Real;

		/// <summary>
		/// The imaginary part of the complex number
		/// </summary>
		public double Imaginary;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of complex
		/// </summary>
		/// <param name="real">Initial value for the real part of the complex number</param>
		/// <param name="imaginary">Initial value for the imaginary part of the complex number</param>
		public Complex(double real, double imaginary)
		{
			this.Real = real;
			this.Imaginary = imaginary;
		}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Returns the imaginary number i
		/// </summary>
		public static Complex i
		{
			get { return new Complex(0, 1); }
		}

		/// <summary>
		/// Returns the a complex number with real and imaginary part set to zero
		/// </summary>
		public static Complex Zero
		{
			get { return new Complex(0, 0); }
		}

		/// <summary>
		/// Represents a value that is not a number
		/// </summary>
		public static Complex NaN
		{
			get { return new Complex(Double.NaN, Double.NaN); }
		}
		
		#endregion

		#region Properties

		/// <summary>
		/// Returns the inclination angle of the complex number
		/// </summary>
		public double Angle
		{
			get { return Math.Atan2(Imaginary, Real); }
		}

		/// <summary>
		/// Calculates the magnitude of the complex number
		/// </summary>
		public double Magnitude
		{
			get
			{
				return Math.Sqrt(Real*Real + Imaginary * Imaginary);
			}
		}

		#endregion

		#region Public Static Mehods

		/// <summary>
		/// Adds two complex numbers
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>Sum of the source complex numbers</returns>
		public static Complex Add(Complex value1, Complex value2)
		{
			return new Complex(value1.Real + value2.Real, value1.Imaginary + value2.Imaginary);
		}

		/// <summary>
		/// Returns the conjugate of a complex number
		/// </summary>
		/// <param name="value">Source complex number</param>
		/// <returns>Conjugate of the source complex number</returns>
		public static Complex Conjugate(Complex value)
		{
			return new Complex(value.Real, -value.Imaginary);
		}

		/// <summary>
		/// Divides two complex numbers
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>The quotient of the two provided complex numbers</returns>
		public static Complex Divide(Complex value1, Complex value2)
		{
			double div = value2.Real * value2.Real + value2.Imaginary * value2.Imaginary;
			return new Complex(
				(value1.Real * value2.Real + value1.Imaginary * value2.Imaginary) / div,
				(value1.Imaginary * value2.Real - value1.Real * value2.Imaginary) / div);
		}

		/// <summary>
		/// Multiplies two complex numbers
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>The product of the two provided complex numbers</returns>
		public static Complex Multiply(Complex value1, Complex value2)
		{
			return new Complex(value1.Real * value2.Real - value1.Imaginary * value2.Imaginary,
				value1.Real * value2.Imaginary + value1.Imaginary * value2.Real);
		}

		/// <summary>
		/// Subtracts a complex number from a complex number
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>Result of the subtraction</returns>
		public static Complex Substract(Complex value1, Complex value2)
		{
			return new Complex(value1.Real - value2.Real, value1.Imaginary - value2.Imaginary);
		}

		/// <summary>
		/// Scales a complex number
		/// </summary>
		/// <param name="value">Source complex number</param>
		/// <param name="scaleFactor">Real number</param>
		/// <returns>Scaled complex number</returns>
		public static Complex Scale(Complex value, double scaleFactor)
		{
			return new Complex(value.Real * scaleFactor, value.Imaginary * scaleFactor);
		}

		/// <summary>
		/// Calculates the square root of a complex number
		/// </summary>
		/// <param name="value">Source complex number</param>
		/// <returns>square root of the source complex number</returns>
		public static Complex Sqrt(Complex value)
		{
			if (value.Imaginary == 0) return new Complex(Math.Sqrt(value.Real), 0);
			double magnitude = value.Magnitude;
			return new Complex(
				Math.Sqrt((value.Real + magnitude)/2 ),
				Math.Sign(value.Imaginary) * Math.Sqrt((-value.Real + magnitude)/2 ));
		}

		/// <summary>
		/// Returns the conjugate of a complex number
		/// </summary>
		/// <param name="value">Source complex number</param>
		/// <returns>Conjugate of the source complex number</returns>
		public static Complex operator !(Complex value)
		{
			return new Complex(value.Real, -value.Imaginary);
		}

		/// <summary>
		/// Tests complex numbers for equality
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>true if the complex numbers are equal; false otherwise</returns>
		public static bool operator ==(Complex value1, Complex value2)
		{
			return (value1.Real == value2.Real) && (value1.Imaginary == value2.Imaginary);
		}

		/// <summary>
		/// Tests complex numbers for inequality
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>true if the complex numbers are not equal; false otherwise</returns>
		public static bool operator !=(Complex value1, Complex value2)
		{
			return (value1.Real != value2.Real) || (value1.Imaginary != value2.Imaginary);
		}

		/// <summary>
		/// Adds two complex numbers
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>Sum of the source complex numbers</returns>
		public static Complex operator +(Complex value1, Complex value2)
		{
			return new Complex(value1.Real + value2.Real, value1.Imaginary + value2.Imaginary);
		}

		/// <summary>
		/// Subtracts a complex number from a complex number
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>Result of the subtraction</returns>
		public static Complex operator -(Complex value1, Complex value2)
		{
			return new Complex(value1.Real - value2.Real, value1.Imaginary - value2.Imaginary);
		}

		/// <summary>
		/// Multiplies two complex numbers
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>The product of the two provided complex numbers</returns>
		public static Complex operator *(Complex value1, Complex value2)
		{
			return new Complex(value1.Real * value2.Real - value1.Imaginary * value2.Imaginary,
				value1.Real * value2.Imaginary + value1.Imaginary * value2.Real);
		}

		/// <summary>
		/// Divides two complex numbers
		/// </summary>
		/// <param name="value1">Source complex number</param>
		/// <param name="value2">Source complex number</param>
		/// <returns>The quotient of the two provided complex numbers</returns>
		public static Complex operator /(Complex value1, Complex value2)
		{
			double div = value2.Real * value2.Real + value2.Imaginary * value2.Imaginary;
			return new Complex(
				(value1.Real * value2.Real + value1.Imaginary * value2.Imaginary) / div,
				(value1.Imaginary * value2.Real - value1.Real * value2.Imaginary) / div);
		}

		/// <summary>
		/// Gets the real part of the complex number
		/// </summary>
		/// <param name="value">Source complex number</param>
		/// <returns>The real part of the complex number</returns>
		public static explicit operator double(Complex value)
		{
			return value.Real;
		}

		/// <summary>
		/// Cast a double as a complex number
		/// </summary>
		/// <param name="value">Source number</param>
		/// <returns>Complex representation of source number</returns>
		public static implicit operator Complex(int value)
		{
			return new Complex(value, 0);
		}

		/// <summary>
		/// Cast a double as a complex number
		/// </summary>
		/// <param name="value">Source number</param>
		/// <returns>Complex representation of source number</returns>
		public static implicit operator Complex(long value)
		{
			return new Complex(value, 0);
		}

		/// <summary>
		/// Cast a double as a complex number
		/// </summary>
		/// <param name="value">Source number</param>
		/// <returns>Complex representation of source number</returns>
		public static implicit operator Complex(float value)
		{
			return new Complex(value, 0);
		}

		/// <summary>
		/// Cast a double as a complex number
		/// </summary>
		/// <param name="value">Source number</param>
		/// <returns>Complex representation of source number</returns>
		public static implicit operator Complex(double value)
		{
			return new Complex(value, 0);
		}

		/// <summary>
		/// Converts the complex to a System.Drawing.PointF structure
		/// </summary>
		/// <param name="value">complex to convert</param>
		/// <returns>System.Drawing.PointF structure</returns>
		public static implicit operator System.Drawing.PointF(Complex value)
		{
			return new System.Drawing.PointF((float)value.Real, (float)value.Imaginary);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Retrieves a string representation of the current object
		/// </summary>
		/// <returns>String that represents the object</returns>
		public override string ToString()
		{
			return "{ Re: " + Real.ToString("0.0000") + ", Im: " + Imaginary.ToString("0.0000") + " }";
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
		/// Gets the hash code of the complex number object
		/// </summary>
		/// <returns>Hash code of the complex number object</returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return Real.GetHashCode() + Imaginary.GetHashCode();
			}
		}

		#endregion
	}
}
