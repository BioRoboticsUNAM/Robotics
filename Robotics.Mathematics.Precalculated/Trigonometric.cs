using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Mathematics.Precalculated
{
	/// <summary>
	/// Stores precalculated values of trigonometric functions allowing queries in O(1)
	/// </summary>
	public class Trigonometric
	{
		#region Variables

		private Dictionary<int, double> sin;
		private readonly double precision;
		private readonly double factor;

		#endregion

		#region Constructor

		/// <summary>
		/// Generates a new storage engine of precalculated values with a precision of 1e-5
		/// </summary>
		public Trigonometric() : this(0.00001) { }

		/// <summary>
		/// Generates a new storage engine of precalculated values with the given precision
		/// </summary>
		/// <param name="precision">The precision used to build the precalculated values dictionary. Value must be between 1e-7 and 1e-2</param>
		public Trigonometric(double precision)
		{
			if ((precision < 0.0000001) || (precision > 0.01))
				throw new ArgumentOutOfRangeException("precision must be strictly between 1e-7 and 1e-2");
			this.precision = precision;

			this.factor = 1 / precision;
			int max = (int)(Constants.TwoPi * this.factor);
			this.sin = new Dictionary<int, double>(max);
			for (int i = 0; i < max; ++i)
				this.sin.Add(i, Math.Sin((double)i / factor));
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the precision of the precalculated values (differential of angle in radians).
		/// </summary>
		public double Precision
		{
			get { return this.precision; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns the sine of the specified angle.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="rad">An angle, measured in radians. </param>
		/// <returns>The sine of the provided angle. If a is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN</returns>
		public double Sin(double rad)
		{
			if (Double.IsNaN(rad) || Double.IsNegativeInfinity(rad) || Double.IsPositiveInfinity(rad)) return Double.NaN;

			Constants.FixRadians(ref rad);
			int index = (int)(rad * this.factor);
			return this.sin[index];
		}

		/// <summary>
		/// Returns the cosine of the specified angle.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="rad">An angle, measured in radians. </param>
		/// <returns>The cosine of  the provided angle. If a is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN</returns>
		public double Cos(double rad)
		{
			if (Double.IsNaN(rad) || Double.IsNegativeInfinity(rad) || Double.IsPositiveInfinity(rad)) return Double.NaN;

			rad += Constants.PiOver2;
			Constants.FixRadians(ref rad);
			int index = (int)(rad * this.factor);
			return this.sin[index];
		}

		/// <summary>
		/// Returns the tangent of the specified angle
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="rad">An angle, measured in radians. </param>
		/// <returns>The tangent of the provided angle. If a is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN</returns>
		public double Tan(double rad)
		{
			if (Double.IsNaN(rad) || Double.IsNegativeInfinity(rad) || Double.IsPositiveInfinity(rad)) return Double.NaN;
			double s = Sin(rad);
			double c = Cos(rad);
			if (c == 0)
			{
				if (s == 0) return Double.NaN;
				return s > 0 ? Double.PositiveInfinity : Double.NegativeInfinity;
			}
			return s / c;
		}

		#endregion
	}
}
