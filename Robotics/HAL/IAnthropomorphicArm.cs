using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a 7DOF antropomorphic arm
	/// </summary>
	public interface IAnthropomorphicArm : IArm, IManipulator
	{
		/// <summary>
		/// Gets the current position and orientation of the anthropomorphic arm
		/// </summary>
		/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
		/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
		/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
		/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
		/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
		/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
		/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
		/// <returns>true if data acquisition was successfull. false otherwise</returns>
		bool GetAbsolutePosition(out double x, out double y, out double z, out double roll, out double pitch, out double yaw, out double elbow);

		/// <summary>
		/// Gets the current orientation of the anthropomorphic arm
		/// </summary>
		/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
		/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
		/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
		/// <returns>true if arm moved to the specified orientation. false otherwise</returns>
		bool GetOrientation(out double roll, out double pitch, out double yaw);

		/// <summary>
		/// Request arm to move to the specified position and orientation
		/// </summary>
		/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
		/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
		/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
		/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
		/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
		/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
		/// <returns>true if arm moved to specified position. false otherwise</returns>
		bool SetAbsolutePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw);

		/// <summary>
		/// Request arm to move to the specified position and orientation
		/// </summary>
		/// <param name="x">Absolute x coordinate position of the actuator of the arm.</param>
		/// <param name="y">Absolute y coordinate position of the actuator of the arm.</param>
		/// <param name="z">Absolute z coordinate position of the actuator of the arm.</param>
		/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
		/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
		/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
		/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
		/// <returns>true if arm moved to specified position. false otherwise</returns>
		bool SetAbsolutePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw, ref double elbow);

		/// <summary>
		/// Request arm to move to the specified orientation
		/// </summary>
		/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
		/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
		/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
		/// <returns>true if arm moved to the specified orientation. false otherwise</returns>
		bool SetOrientation(ref double roll, ref double pitch, ref double yaw);

		/// <summary>
		/// Request arm to move to the specified position and orientation relative to its current position and orientation
		/// </summary>
		/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
		/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
		/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
		/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
		/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
		/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
		/// <returns>true if arm moved to specified position. false otherwise</returns>
		bool SetRelativePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw);

		/// <summary>
		/// Request arm to move to the specified position and orientation relative to its current position and orientation
		/// </summary>
		/// <param name="x">Relative x coordinate position of the actuator of the arm.</param>
		/// <param name="y">Relative y coordinate position of the actuator of the arm.</param>
		/// <param name="z">Relative z coordinate position of the actuator of the arm.</param>
		/// <param name="roll">Angle of rotation about the X-axis of the actuator of the arm.</param>
		/// <param name="pitch">Angle of rotation about the Y-axis of the actuator of the arm.</param>
		/// <param name="yaw">Angle of rotation about the Z-axis of the actuator of the arm.</param>
		/// <param name="elbow">Angle in radians of the elbow of the arm.</param>
		/// <returns>true if arm moved to specified position. false otherwise</returns>
		bool SetRelativePosition(ref double x, ref double y, ref double z, ref double roll, ref double pitch, ref double yaw, ref double elbow);
	}
}
