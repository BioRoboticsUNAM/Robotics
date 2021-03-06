using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Contains commonly used precalculated values
	/// </summary>
	public static partial class MathUtil
	{
		#region Constants
		/// <summary>
		/// Represents the mathematical constant e.
		/// </summary>
		public const double E =			2.718281828459045235360287471352662497757247093699959574966967628;
		/// <summary>
		/// Represents the log base ten of e. 
		/// </summary>
		public const double Log10E =	0.4342944819032518276511289189166050822943970058036665661144537832;
		/// <summary>
		/// Represents the log base two of e. 
		/// </summary>
		public const double Log2E =		1.442695040888963407359924681001892137426645954152985934135449407;
		/// <summary>
		/// Represents the value of pi. 
		/// </summary>
		public const double Pi =		3.141592653589793238462643383279502884197169399375105820974944592;
		/// <summary>
		/// Represents the value of pi divided by two. 
		/// </summary>
		public const double PiOver2 =	1.570796326794896619231321691639751442098584699687552910487472296;
		/// <summary>
		/// Represents the value of pi divided by four. 
		/// </summary>
		public const double PiOver4 =	0.7853981633974483096156608458198757210492923498437764552437361481;
		/// <summary>
		/// Represents the value of pi divided by six. 
		/// </summary>
		public const double PiOver6 =	0.5235987755982988730771072305465838140328615665625176368291574321;
		/// <summary>
		/// Represents the value of pi times two. 
		/// </summary>
		public const double TwoPi =		6.283185307179586476925286766559005768394338798750211641949889185;
		
		#endregion

		#region Methods

		#region Abs Methods
		/// <summary>
		/// Returns the absolute value of a Decimal number. 
		/// </summary>
		/// <param name="value">A number in the range System.Decimal.MinValue ≤ value ≤ System.Decimal.MaxValue. </param>
		/// <returns>A Decimal, x, such that 0 ≤ x ≤ System.Decimal.MaxValue. </returns>
		public static decimal Abs(decimal value)
		{
			return value < 0 ? -value : value;
		}

		/// <summary>
		/// Returns the absolute value of a Double number. 
		/// </summary>
		/// <param name="value">A number in the range System.Double.MinValue = value = System.Double.MaxValue. </param>
		/// <returns>A Double, x, such that 0 = x = System.Double.MaxValue. </returns>
		public static double Abs(double value)
		{
			return value < 0 ? -value : value;
		}

		/// <summary>
		/// Returns the absolute value of a Int16 number. 
		/// </summary>
		/// <param name="value">A number in the range System.Int16.MinValue = value = System.Int16.MaxValue. </param>
		/// <returns>A Double, x, such that 0 = x = System.Int16.MaxValue. </returns>
		public static short Abs(short value)
		{
			return value < 0 ? (short)-value : value;
		}

		/// <summary>
		/// Returns the absolute value of a Int32 number. 
		/// </summary>
		/// <param name="value">A number in the range System.Int32.MinValue = value = System.Int32.MaxValue. </param>
		/// <returns>A Double, x, such that 0 = x = System.Int32.MaxValue. </returns>
		public static int Abs(int value)
		{
			return value < 0 ? -value : value;
		}

		/// <summary>
		/// Returns the absolute value of a Int64 number. 
		/// </summary>
		/// <param name="value">A number in the range System.Int64.MinValue = value = System.Int64.MaxValue. </param>
		/// <returns>A Double, x, such that 0 = x = System.Int64.MaxValue. </returns>
		public static long Abs(long value)
		{
			return value < 0 ? -value : value;
		}

		/// <summary>
		/// Returns the absolute value of a SByte number. 
		/// </summary>
		/// <param name="value">A number in the range System.SByte.MinValue = value = System.SByte.MaxValue. </param>
		/// <returns>A Double, x, such that 0 = x = System.SByte.MaxValue. </returns>
		public static sbyte Abs(sbyte value)
		{
			return value < 0 ? (sbyte)-value : value;
		}

		/// <summary>
		/// Returns the absolute value of a Single number. 
		/// </summary>
		/// <param name="value">A number in the range System.Single.MinValue = value = System.Single.MaxValue. </param>
		/// <returns>A Double, x, such that 0 = x = System.Single.MaxValue. </returns>
		public static float Abs(float value)
		{
			return value < 0 ? -value : value;
		}

		#endregion

		/// <summary>
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped value.</returns>
		public static double Clamp(double value, double min, double max)
		{
			if (value < min) return min;
			if (value > max) return max;
			return value;
		}

		/// <summary>
		/// Calculates the absolute value of the difference of two values. 
		/// </summary>
		/// <param name="value1">Source value.</param>
		/// <param name="value2">Source value.</param>
		/// <returns>Distance between the two values.</returns>
		public static double Distance(double value1, double value2)
		{
			return Math.Abs(value1 - value2);
		}

		/// <summary>
		/// Returns the equivalent angle in the range [0, 360)
		/// </summary>
		/// <param name="angle">The angle to fix in degrees</param>
		/// <returns>The equivalent angle in the range [0, 360)</returns>
		public static double FixDegrees(double angle)
		{
			FixDegrees(ref angle);
			return angle;
		}

		/// <summary>
		/// Returns the equivalent angle in the range [0, 360)
		/// </summary>
		/// <param name="angle">The angle to fix in degrees</param>
		/// <returns>The equivalent angle in the range [0, 360)</returns>
		public static void FixDegrees(ref double angle)
		{
			int factor;
			if (angle < 0)
			{
				factor = (int)(angle / 360) - 1;
				angle -= (factor * 360);
			}
			else if (angle >= 360)
			{
				factor = (int)(angle / 360.0);
				angle -= (factor * 360);
			}
		}

		/// <summary>
		/// Returns the equivalent angle in the range [0, 2pi)
		/// </summary>
		/// <param name="angle">The angle to fix in radians</param>
		/// <returns>The equivalent angle in the range [0, 2pi)</returns>
		public static void FixRadians(ref double angle)
		{
			int factor;
			if (angle < 0)
			{
				factor = (int)(angle / TwoPi) - 1;
				angle -= (factor * TwoPi);
			}
			else if (angle >= TwoPi)
			{
				factor = (int)(angle / TwoPi);
				angle -= (factor * TwoPi);
			}
		}

		/// <summary>
		/// Returns the equivalent angle in the range [0, 2pi)
		/// </summary>
		/// <param name="angle">The angle to fix in radians</param>
		/// <returns>The equivalent angle in the range [0, 2pi)</returns>
		public static double FixRadians(double angle)
		{
			FixRadians(ref angle);
			return angle;
		}

		/// <summary>
		/// Linearly interpolates between two values
		/// </summary>
		/// <param name="value1">Source value</param>
		/// <param name="value2">Source value</param>
		/// <param name="amount">Value between 0 and 1 indicating the weight of value2</param>
		/// <returns>Interpolated value</returns>
		/// <remarks>This method performs the linear interpolation based on the following formula.
		/// value1 + (value2 - value1) * amount
		/// Passing amount a value of 0 will cause value1 to be returned, a value of 1 will cause value2 to be returned.
		/// </remarks>
		public static double Lerp(double value1, double value2, double amount)
		{
			return value1 + (value2 - value1) * amount;
		}

		#region Max Methods

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static double Max(params double[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			double max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static byte Max(params byte[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			byte max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static decimal Max(params decimal[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			decimal max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static float Max(params float[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			float max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static int Max(params int[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			int max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static long Max(params long[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			long max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static sbyte Max(params sbyte[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			sbyte max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static short Max(params short[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			short max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static uint Max(params uint[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			uint max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		/// <summary>
		/// Returns the greater of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The greater value</returns>
		public static ushort Max(params ushort[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			ushort max = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] > max) max = values[i];
			return max;
		}

		#endregion

		#region Min Methods

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static double Min(params double[] values)
		{
			if ((values == null) || (values.Length < 1)) return Double.NaN;
			double min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static byte Min(params byte[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			byte min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static decimal Min(params decimal[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			decimal min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static float Min(params float[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			float min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static int Min(params int[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			int min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static long Min(params long[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			long min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static sbyte Min(params sbyte[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			sbyte min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static short Min(params short[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			short min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static uint Min(params uint[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			uint min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		/// <summary>
		/// Returns the lesser of n values. 
		/// </summary>
		/// <param name="values">List of values</param>
		/// <returns>The lesser value</returns>
		public static ushort Min(params ushort[] values)
		{
			if ((values == null) || (values.Length < 1)) throw new ArgumentNullException("values");
			ushort min = values[0];
			for (int i = 1; i < values.Length; ++i)
				if (values[i] < min) min = values[i];
			return min;
		}

		#endregion

		/// <summary>
		/// Converts radians to degrees. 
		/// </summary>
		/// <param name="radians">The angle in radians.</param>
		/// <returns>The angle in degrees.</returns>
		public static double ToDegrees(double radians)
		{
			return 180 * radians / Pi;
		}

		/// <summary>
		/// Converts degrees to radians.
		/// </summary>
		/// <param name="degrees">The angle in degrees.</param>
		/// <returns>The angle in radians.</returns>	
		public static double ToRadians(double degrees)
		{
			return Pi * degrees / 180;
		}

		#region Trigonometric Functions

		/// <summary>
		/// Returns the angle whose cosine is the specified number.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="d">A number representing a cosine, where -1 ≤ d ≤ 1.</param>
		/// <returns>An angle, θ, measured in radians, such that 0 ≤ θ ≤ π -or- NaN if d &lt; -1 or d &gt; 1.</returns>
		public static double Acos(double d)
		{
			return ACOS[(int)((1 + d) * 5000)];
		}

		/// <summary>
		/// Returns the angle whose sine is the specified number.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="d">A number representing a sine, where -1 ≤ d ≤ 1. </param>
		/// <returns>An angle, θ, measured in radians, such that -π/2 ≤ θ ≤π /2 -or- NaN if d &lt; -1 or d &gt; 1.</returns>
		public static double Asin(double d)
		{
			return ASIN[(int)((1 + d) * 5000)];
		}

		/*
		/// <summary>
		/// Returns the angle whose tangent is the quotient of two specified numbers.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="y">The y coordinate of a point</param>
		/// <param name="x">The x coordinate of a point</param>
		/// <returns>An angle, θ, measured in radians, such that -π ≤θ ≤ π, and tan(θ) = y / x, where (x, y) is a point in the Cartesian plane. Observe the following: 
		/// For (x, y) in quadrant 1, 0 < θ < π/2.
		/// For (x, y) in quadrant 2, π/2 < θ≤π.
		/// For (x, y) in quadrant 3, -π < θ < -π/2.
		/// For (x, y) in quadrant 4, -π/2 < θ < 0.
		///</returns>
		public static double Atan2(double y, double x)
		{
			if (Double.IsNaN(x) || Double.IsNaN(y)) return Double.NaN;
			if(x == 0)
			{
				if(y == 0) return Double.NaN;
				if(y > 0) return PiOver2;
				if(y < 0) return -PiOver2;
			}

			double atan = ATAN[(int)Math.Round(Math.Abs(y/x) * 10000, 0)];
			if(x < 0)
				return atan + (y >= 0 ? Pi : -Pi);
			return atan;
		}
		*/

		/// <summary>
		/// Returns the cosine of the specified angle.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="radians">An angle, measured in radians. </param>
		/// <returns>The cosine of  the provided angle. If a is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN</returns>
		public static double Cos(double radians)
		{
			if (Double.IsNaN(radians) || Double.IsNegativeInfinity(radians) || Double.IsPositiveInfinity(radians)) return Double.NaN;
			radians += PiOver2;
			FixRadians(ref radians);
			int index = (int)Math.Round(radians * 10000, 0);
			if(index > SIN.Length)index = 0;
			//int ix = Math.Round(radians * 10000, 0) + Math.Round(PiOver2 * 10000, 0);
			//int ix = (int)Math.Round(radians * 10000, 0);
			return SIN[index];
		}

		/// <summary>
		/// Returns the sine of the specified angle.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="radians">An angle, measured in radians. </param>
		/// <returns>The sine of the provided angle. If a is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN</returns>
		public static double Sin(double radians)
		{
			if (Double.IsNaN(radians) || Double.IsNegativeInfinity(radians) || Double.IsPositiveInfinity(radians)) return Double.NaN;
			FixRadians(ref radians);
			int index = (int)Math.Round(radians * 10000, 0);
			if (index > SIN.Length) index = 0;
			return SIN[index];
		}

		/// <summary>
		/// Returns the tangent of the specified angle
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="radians">An angle, measured in radians. </param>
		/// <returns>The sine of the provided angle. If a is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN</returns>
		public static double Tan(double radians)
		{
			if (Double.IsNaN(radians) || Double.IsNegativeInfinity(radians) || Double.IsPositiveInfinity(radians)) return Double.NaN;
			FixRadians(ref radians);
			double sin = Sin(radians);
			double cos = Cos(radians);
			if (cos == 0) return sin > 0 ? Double.PositiveInfinity : Double.NegativeInfinity;
			return sin / cos;
		}

		#endregion

		#endregion
	}
}
