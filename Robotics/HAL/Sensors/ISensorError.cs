using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors
{
	/// <summary>
	/// Represents an object that contains information about an error of a sensor
	/// </summary>
	public interface ISensorError
	{
		/// <summary>
		/// Gets the error code returned by the sensor
		/// </summary>
		int ErrorCode
		{
			get;
		}

		/// <summary>
		/// Gets a description of the error
		/// </summary>
		string ErrorDescription
		{
			get;
		}
	}
}
