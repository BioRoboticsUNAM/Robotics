using System;

namespace Robotics.HAL.Sensors
{
	/// <summary>
	/// Represents a ISensor object read
	/// </summary>
	public interface ISensorReading<TSensor> where TSensor : ISensor
	{
		/// <summary>
		/// ISensor object which generated this ISensorRead object
		/// </summary>
		TSensor Sensor { get; }

		/// <summary>
		/// Indicates if the reading was mistaken
		/// </summary>
		bool Mistaken { get; }
	}
}
