using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Mathematics.Precalculated
{
	/// <summary>
	/// Stores precalculated values of inverse trigonometric functions allowing queries in O(1)
	/// </summary>
	public class InverseTrigonometric
	{
		#region Variables

		private Dictionary<int, double> asin;
		private readonly double precision;
		private readonly double factor;

		#endregion

		#region Constructor

		/// <summary>
		/// Generates a new storage engine of precalculated values with a precision of 1e-5
		/// </summary>
		public InverseTrigonometric() : this(0.00001) { }

		/// <summary>
		/// Generates a new storage engine of precalculated values with the given precision
		/// </summary>
		/// <param name="precision">The precision used to build the precalculated values dictionary. Value must be between 1e-7 and 1e-2</param>
		public InverseTrigonometric(double precision)
		{
			if ((precision < 0.0000001) || (precision > 0.01))
				throw new ArgumentOutOfRangeException("precision must be strictly between 1e-7 and 1e-2");
			this.precision = precision;

			this.factor = 1 / precision;
			int max = (int)this.factor;
			this.asin = new Dictionary<int, double>(max);
			for (int i = 0; i <= max; ++i)
				this.asin.Add(i, Math.Asin((double)i / this.factor));
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
		/// Returns the arc-sine of the specified value.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="value">An angle, measured in radians. </param>
		/// <returns>The sine of the provided angle. If a is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN</returns>
		public double Asin(double value)
		{
			if (Double.IsNaN(value) || Double.IsNegativeInfinity(value) || Double.IsPositiveInfinity(value)) return Double.NaN;

			Constants.Clamp(ref value, -1, 1);
			if (value < 0) return -Asin(-value);
			int index = (int)(value * this.factor);
			if (index >= this.asin.Count) index = 0;
			return this.asin[index];
		}

		/// <summary>
		/// Returns the cosine of the specified angle.
		/// Values returned by this method are precalculated.
		/// </summary>
		/// <param name="value">An angle, measured in radians. </param>
		/// <returns>The cosine of  the provided angle. If a is equal to NaN, NegativeInfinity, or PositiveInfinity, this method returns NaN</returns>
		public double Acos(double value)
		{
			if (Double.IsNaN(value) || Double.IsNegativeInfinity(value) || Double.IsPositiveInfinity(value)) return Double.NaN;

			double result = Constants.PiOver2 - Asin(value);
			Constants.FixRadians(ref result);
			return result;
		}

		#endregion
	}
}
