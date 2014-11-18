using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Mathematics.Precalculated
{
	/// <summary>
	/// Stores high precision constants
	/// </summary>
	public static class Constants
	{
		#region Constants
		/// <summary>
		/// Represents the mathematical constant e.
		/// </summary>
		public const double E = 2.718281828459045235360287471352662497757247093699959574966967628;
		/// <summary>
		/// Represents the log base ten of e. 
		/// </summary>
		public const double Log10E = 0.4342944819032518276511289189166050822943970058036665661144537832;
		/// <summary>
		/// Represents the log base two of e. 
		/// </summary>
		public const double Log2E = 1.442695040888963407359924681001892137426645954152985934135449407;
		/// <summary>
		/// Represents the value of pi. 
		/// </summary>
		public const double Pi = 3.141592653589793238462643383279502884197169399375105820974944592;
		/// <summary>
		/// Represents the value of pi divided by two. 
		/// </summary>
		public const double PiOver2 = 1.570796326794896619231321691639751442098584699687552910487472296;
		/// <summary>
		/// Represents the value of pi divided by four. 
		/// </summary>
		public const double PiOver4 = 0.7853981633974483096156608458198757210492923498437764552437361481;
		/// <summary>
		/// Represents the value of pi divided by six. 
		/// </summary>
		public const double PiOver6 = 0.5235987755982988730771072305465838140328615665625176368291574321;
		/// <summary>
		/// Represents the value of pi times two. 
		/// </summary>
		public const double TwoPi = 6.283185307179586476925286766559005768394338798750211641949889185;

		#endregion

		#region Methods

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
				factor = (int)(angle / Constants.TwoPi) - 1;
				angle -= (factor * Constants.TwoPi);
			}
			else if (angle >= Constants.TwoPi)
			{
				factor = (int)(angle / Constants.TwoPi);
				angle -= (factor * Constants.TwoPi);
			}
		}

		/// <summary>
		/// Returns the equivalent angle in the range [0, 2pi)
		/// </summary>
		/// <param name="angle">The angle to fix in radians</param>
		/// <param name="error">The precision factor when comparing with 2pi.
		/// The angle and 2pi are considered equal when the difference between them is less than error</param>
		/// <returns>The equivalent angle in the range [0, 2pi)</returns>
		public static void FixRadians(ref double angle, double error)
		{
			int factor;
			if (angle < 0)
			{
				factor = (int)(angle / Constants.TwoPi) - 1;
				angle -= (factor * Constants.TwoPi);
			}
			else if (angle >= TwoPi)
			{
				factor = (int)(angle / Constants.TwoPi);
				angle -= (factor * Constants.TwoPi);
			}
			if (Math.Abs(Constants.TwoPi - angle) < error)
				angle = 0;
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
		/// Restricts a value to be within a specified range.
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		/// <returns>The clamped value.</returns>
		internal static void Clamp(ref double value, double min, double max)
		{
			if (value < min) value = min;
			if (value > max) value = max;
		}

		#endregion
	}
}
