using System;
using System.Collections.Generic;
using Robotics.Mathematics;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a physical which has a position
	/// </summary>
	public interface IPositionable
	{
		#region Properties
		
		/// <summary>
		/// Gets or sets the position of the centroid of the Ipositionable object
		/// </summary>
		Vector3 Position { get; set; }

		#endregion

		#region Methodos

		/// <summary>
		/// Calculates the distance between this instance and another IPositionable object
		/// </summary>
		/// <param name="p">IPositionable object</param>
		/// <returns>Vector3 that represents the distance vector between two IPositionable objects</returns>
		Vector3 Distance(IPositionable p);

		#endregion
	}
}
