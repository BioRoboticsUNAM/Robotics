using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a device which can detect or move over a line (X-axis)
	/// </summary>
	public interface LinearDevice
	{
		#region Properties

		/// <summary>
		/// Gets the Absolute last step the device can reach
		/// </summary>
		int AbsoluteMaximumLinearStep { get;}

		/// <summary>
		/// Gets the smallest distance the device can detect or move
		/// </summary>
		double LinearResolution { get; }

		/// <summary>
		/// Gets the linear resolution bits
		/// </summary>
		int LinearResolutionBits { get; }

		/// <summary>
		/// Step in which the Zero is located
		/// </summary>
		int LinearStepZero { get; }

		/// <summary>
		/// Gets the number of steps per meter
		/// </summary>
		int StepsPerMeter { get; }

		/// <summary>
		/// Gets the First Step of the Measurement Range 
		/// </summary>
		int ValidMinimumLinearStep { get; }

		/// <summary>
		/// Gets the Last Step of the Measurement Range 
		/// </summary>
		int ValidMaximumLinearStep { get; }

		#endregion

		#region Methodos

		#endregion
	

	}
}
