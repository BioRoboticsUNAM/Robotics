using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a physical prism-bounded object which has a position, orientation an dimensions
	/// </summary>
	public interface IBody: IIntersectable
	{
		#region Properties

		/// <summary>
		/// Gets or sets the pepth of the IBody object
		/// </summary>
		double Depth
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the orientation of the IBody object
		/// </summary>
		double Orientation
		{
			get;
			set;
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Checks if this instance of IBody intersects with another
		/// </summary>
		/// <param name="body">IBody object to check intersection with</param>
		/// <returns>true if there is intersection, false otherwise</returns>
		bool Intersects(IBody body);

		/// <summary>
		/// Checks if this instance of IBody contains the provided IBody instance
		/// </summary>
		/// <param name="body">IBody object to check contention with</param>
		/// <returns>true if the IBody object provided is completely contained within, false otherwise</returns>
		bool Contains(IBody body);

		#endregion
	}
}
