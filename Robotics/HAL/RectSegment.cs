using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL
{
	/// <summary>
	/// A segment of rect defined by two points in the space
	/// </summary>
	public class RectSegment : IIntersectable
	{
		#region IIntersectable Members

		/// <summary>
		/// Checks if this instance of RectSegment intersects with another IIntersectable object
		/// </summary>
		/// <param name="other">IIntersectable object to check intersection with</param>
		/// <returns>true if there is intersection, false otherwise</returns>
		public bool Intersects(IIntersectable other)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Checks if this instance of RectSegment contains the provided IIntersectable instance
		/// </summary>
		/// <param name="other">IIntersectable object to check contention with</param>
		/// <returns>true if the IIntersectable object provided is completely contained within, false otherwise</returns>
		public bool Contains(IIntersectable other)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Checks if this instance of RectSegment contains the point provided by the IPositionable instance
		/// </summary>
		/// <param name="position">IPositionable object to check contention with</param>
		/// <returns>true if the IPositionable object provided is inside this instance, false otherwise</returns>
		public bool Contains(IPositionable position)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IPositionable Members

		/// <summary>
		/// Gets or sets the position (centroid) of the rect segment
		/// </summary>
		public Robotics.Mathematics.Vector3 Position
		{
			get
			{
				throw new Exception("The method or operation is not implemented.");
			}
			set
			{
				throw new Exception("The method or operation is not implemented.");
			}
		}

		/// <summary>
		/// Calculates the distance between the rect segment and a IPositionable object
		/// </summary>
		/// <param name="p">IPositionable object to calculate the distance to</param>
		/// <returns>Distance between the rect segment and IPositionable object</returns>
		public Robotics.Mathematics.Vector3 Distance(IPositionable p)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
