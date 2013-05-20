using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Represents the method that will handle the ThresholdExceeded event of a TelemetricSensor object
	/// </summary>
	/// <param name="sensor">The TelemetricSensor object which rises the event</param>
	public delegate void TelemetricSensorThresholdExceededEventHandler<TSensor>(TSensor sensor)
		where TSensor : ITelemetricSensor;

	/// <summary>
	/// Serves as base interface for TelemetricSensor class
	/// </summary>
	//public interface ITelemetricSensor : ISensor<ITelemetricSensor, ITelemetricReading>
	public interface ITelemetricSensor : ISensor
	{
		#region Properties

		/// <summary>
		/// Gets the number of mistaken readings in the last read
		/// </summary>
		int ErrorCount { get; }

		/// <summary>
		/// Gets the minumim distance the sensor can detect
		/// </summary>
		double MinimumDistance { get; }

		/// <summary>
		/// Gets the maximum distance the sensor can detect
		/// </summary>
		double MaximumDistance { get; }

		/// <summary>
		/// Gets or sets the proximity threshold used to determinate when the ThresholdExcedeed event is rised
		/// This value is the distance between the sensor and the closest object
		/// </summary>
		double Treshold { get; set; }

		#endregion
	}	
}
