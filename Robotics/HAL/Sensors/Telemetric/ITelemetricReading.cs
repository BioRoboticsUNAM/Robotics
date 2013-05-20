using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors.Telemetric
{

	/// <summary>
	/// Represents a single reading from a telemetry device
	/// </summary>
	public interface ITelemetricReading : ISensorReading<ITelemetricSensor>
	{
		#region Properties

		/// <summary>
		/// Indicates if an obstacle was detected by this reading
		/// </summary>
		bool ObstacleDetected { get; }

		/// <summary>
		/// Gets the x-coordinate of the cartesian transform of this reading in meters.
		/// </summary>
		double X { get; }

		/// <summary>
		/// Gets the y-coordinate of the cartesian transform of this reading in meters
		/// </summary>
		double Y { get; }

		#endregion

	}
}

/*
namespace Robotics.HAL.Sensors.Telemetric
{

	/// <summary>
	/// Represents a single reading from a telemetry device
	/// </summary>
	public interface ITelemetricReading : ISensorReading<TelemetricSensor>
	{
		#region Properties

		/// <summary>
		/// Indicates if an obstacle was detected by this reading
		/// </summary>
		bool ObstacleDetected { get; }

		/// <summary>
		/// Gets the x-coordinate of the cartesian transform of this reading in meters.
		/// </summary>
		double X { get; }

		/// <summary>
		/// Gets the y-coordinate of the cartesian transform of this reading in meters
		/// </summary>
		double Y { get; }

		#endregion

	}
}
*/
