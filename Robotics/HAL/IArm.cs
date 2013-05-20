using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a positionable arm
	/// </summary>
	public interface IArm
	{

		/// <summary>
		/// Gets the current position of the arm
		/// </summary>
		/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
		/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
		/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
		/// <returns>true if data acquisition was successfull. false otherwise</returns>
		bool GetAbsolutePosition(out double x, out double y, out double z);

		/// <summary>
		/// Request arm to move to specified predefined position
		/// </summary>
		/// <param name="position">Name of the predefined position to move at</param>
		/// <returns>true if arm moved to desired position. false otherwise</returns>
		bool GoTo(string position);

		/// <summary>
		/// Request arm to move to a specified position
		/// </summary>
		/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
		/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
		/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
		/// <returns>true if arm moved to specified position. false otherwise</returns>
		bool SetAbsolutePosition(ref double x, ref double y, ref double z);

		/// <summary>
		/// Request arm to move to the specified position relative to its current position
		/// </summary>
		/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
		/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
		/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
		/// <returns>true if arm moved to specified position. false otherwise</returns>
		bool SetRelativePosition(ref double x, ref double y, ref double z);

	}
}
