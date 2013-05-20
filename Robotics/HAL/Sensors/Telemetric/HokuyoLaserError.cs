using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Information about a Hokuyo Laser measurement error
	/// </summary>
	class HokuyoLaserError : LaserError
	{
		#region variables

		/// <summary>
		/// The Laser status code
		/// </summary>
		private int statusCode;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of HokuyoLaserError
		/// </summary>
		/// <param name="statusCode">The error code</param>
		protected internal HokuyoLaserError(int statusCode)
		{
			this.statusCode = statusCode;
			this.errorDescription = "none";
		}

		/// <summary>
		/// Initializes a new instance of HokuyoLaserError
		/// </summary>
		/// <param name="errorDescription">The error message</param>
		protected internal HokuyoLaserError(string errorDescription) 
			:base(errorDescription)
		{
			this.statusCode = -1;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The status returned by the lasser
		/// </summary>
		public int StatusCode
		{
			get { return statusCode; }
		}

		/// <summary>
		/// Gets the error description bases on the status code
		/// </summary>
		public override string ErrorDescription
		{
			get
			{
				switch (statusCode)
				{
					case -1:
						return errorDescription;
					case 99:
					case 0:
						return "None";

					case 1: return "Starting step is non-numeric";
					case 2: return "End step is non-numeric";
					case 3: return "Cluster count non-numeric";
					case 4: return "End step out of range";
					case 5: return "End step < Start step";
					case 6: return "Scan Interval is non-numeric";
					case 7: return "Number of Scan is non-numeric";

					default:
						if ((errorCode >= 21) && (errorCode <= 49))
							return "Processing stopped to verify error";
						if ((errorCode >= 50) && (errorCode <= 97))
							return "Hardware trouble!!! Laser or motor malfunction";
						if (errorCode == 98)
							return "Resumption of process after confirming normal laser operation";
						break;
				}
				return "Unknown error";
			}
		}

		#endregion
	}
}