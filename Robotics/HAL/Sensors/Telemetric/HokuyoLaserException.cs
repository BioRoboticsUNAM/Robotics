using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Represents errors that occur during hokuyo laser operation
	/// </summary>
	public class HokuyoLaserException : Exception
	{
		/// <summary>
		/// Status of the Hokuyo device
		/// </summary>
		private int status = -1;

		/// <summary>
		/// Initializes a new instance of the HokuyoLaserException class
		/// </summary>
		public HokuyoLaserException() : base() { }

		/// <summary>
		/// Initializes a new instance of the HokuyoLaserException class with a specified error message
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public HokuyoLaserException(string message)
			: base(message) { }

		/// <summary>
		/// Initializes a new instance of the HokuyoLaserException class with a specified error message and a reference to the inner exception that is the cause of this exception. 
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public HokuyoLaserException(string message, Exception innerException)
			: base(message, innerException) { }

		/// <summary>
		/// Initializes a new instance of the HokuyoLaserException class with a specified error message and a reference to the inner exception that is the cause of this exception. 
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="status">The status reported by the hokuyo device.</param>
		public HokuyoLaserException(string message, int status)
			: base(message)
		{
			this.status = status;
		}

		/// <summary>
		/// Gets the status reported by the hokuyo device.
		/// </summary>
		public int Status
		{
			get { return status; }
		}
	}
}
