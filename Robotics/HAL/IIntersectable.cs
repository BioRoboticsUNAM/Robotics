using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a intersectable object
	/// </summary>
	public interface IIntersectable : IPositionable
	{
		#region Methodos

		/// <summary>
		/// Checks if this instance of IIntersectable intersects with another
		/// </summary>
		/// <param name="other">IIntersectable object to check intersection with</param>
		/// <returns>true if there is intersection, false otherwise</returns>
		bool Intersects(IIntersectable other);

		/// <summary>
		/// Checks if this instance of IIntersectable contains the provided IIntersectable instance
		/// </summary>
		/// <param name="other">IIntersectable object to check contention with</param>
		/// <returns>true if the IIntersectable object provided is completely contained within, false otherwise</returns>
		bool Contains(IIntersectable other);

		/// <summary>
		/// Checks if this instance of IIntersectable contains the provided IPositionable instance
		/// </summary>
		/// <param name="position">IPositionable object to check contention with</param>
		/// <returns>true if the IPositionable object provided is inside this instance, false otherwise</returns>
		bool Contains(IPositionable position);

		#endregion
	}
}
