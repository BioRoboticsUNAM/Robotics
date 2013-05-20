using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a manipulator capable of take an release objects
	/// </summary>
	public interface IManipulator
	{
		/// <summary>
		/// Request the IManipulator to close the grip
		/// </summary>
		/// <returns>true if the IManipulator closed the grip. false otherwise</returns>
		bool CloseGrip();

		/// <summary>
		/// Request IManipulator to close the grip
		/// </summary>
		/// <param name="percentage">Percentage aperture of the grip</param>
		/// <returns>true if the IManipulator closed the grip. false otherwise</returns>
		bool CloseGrip(int percentage);

		/// <summary>
		/// Request IManipulator to open the grip
		/// </summary>
		/// <returns>true if IManipulator opened the grip. false otherwise</returns>
		bool OpenGrip();

		/// <summary>
		/// Request IManipulator to open the grip
		/// </summary>
		/// <param name="percentage">Percentage aperture of the grip</param>
		/// <returns>true if IManipulator opened the grip. false otherwise</returns>
		bool OpenGrip(int percentage);
	}
}
