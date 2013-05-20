using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Information about a lasser measurement error
	/// </summary>
	public class LaserError :ISensorError
	{
		#region Variables

		/// <summary>
		/// Laser error code
		/// </summary>
		protected int errorCode;

		/// <summary>
		/// Error message
		/// </summary>
		protected string errorDescription;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of LaserError object
		/// </summary>
		public LaserError()
		{
			errorCode = -1;
			this.errorDescription = "None";
		}

		/// <summary>
		/// Initializes a new instance of LaserError object
		/// </summary>
		/// <param name="message">The error message</param>
		public LaserError(string message)
		{
			errorCode = -1;
			this.errorDescription = message;
		}

		/// <summary>
		/// Initializes a new instance of LaserError object
		/// </summary>
		/// <param name="errorCode">The code error</param>
		/// <param name="message">The error message</param>
		public LaserError(int errorCode, string message)
		{
			this.errorCode = errorCode;
			this.errorDescription = message;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the error code
		/// </summary>
		public virtual int ErrorCode
		{
			get { return errorCode; }
		}

		/// <summary>
		/// Gets the error description message
		/// </summary>
		public virtual string ErrorDescription
		{
			get { return errorDescription; }
		}

		#endregion
	}
}
