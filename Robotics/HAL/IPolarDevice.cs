using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a device which can detect or move rotating
	/// </summary>
	public interface IPolarDevice
	{
		#region Properties

		/// <summary>
		/// Gets the Absolute last step the device can reach
		/// </summary>
		int AbsoluteMaximumAngularStep { get;}

		/// <summary>
		/// Gets the smallest angle change the device can detect or rotate
		/// </summary>
		double AngularResolution {get; }

		/// <summary>
		/// Gets the angle resolution bits
		/// </summary>
		int AngularResolutionBits { get; }

		/// <summary>
		/// Step in which the Zero is located
		/// </summary>
		int AngularStepZero { get; }

		/// <summary>
		/// Gets the number of steps in a complete revolution (360º or two pi radians)
		/// </summary>
		int StepsPerRevolution { get; }

		/// <summary>
		/// Gets the First Step of the Measurement Range 
		/// </summary>
		int ValidMinimumAngularStep { get; }

		/// <summary>
		/// Gets the Last Step of the Measurement Range 
		/// </summary>
		int ValidMaximumAngularStep { get; }

		#endregion

		#region Methodos

		/// <summary>
		/// Gets the cosine value of the angle at provided step.
		/// </summary>
		/// <param name="step">The step for which angle the cosine is desired</param>
		/// <returns>The cosine value of the angle at provided step.</returns>
		double GetCosFromStep(int step);

		/// <summary>
		/// Gets the sine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the sine is desired</param>
		/// <returns>The sine value of the angle at provided step.</returns>
		double GetSinFromStep(int step);

		#endregion
	}
}
