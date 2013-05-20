using System;
using Robotics.Mathematics;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a surface delimited by a perimeter
	/// </summary>
	public interface IArea : IIntersectable
	{
		#region Properties

		/// <summary>
		/// Gets or sets the Vector2 object of the lower-left corner of the 
		/// rectangle that encloses the area represented by the IArea object
		/// </summary>
		Vector2 LowerLeftBound
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Vector2 object of the lower-right corner of the 
		/// rectangle that encloses the area represented by the IArea object
		/// </summary>
		Vector2 LowerRightBound
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Vector2 object of the upper-left corner of the 
		/// rectangle that encloses the area represented by the IArea object
		/// </summary>
		Vector2 UpperLeftBound
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the Vector2 object of the upper-right corner of the 
		/// rectangle that encloses the area represented by the IArea object
		/// </summary>
		Vector2 UpperRightBound
		{
			get;
			set;
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Gets or sets the n-th vertex of the polygon that delimites the IArea
		/// </summary>
		/// <param name="i">Index of desired node of te polygon</param>
		/// <returns>The n-th vertex of the polygon that delimites the IArea</returns>
		Vector2 this[int i]
		{
			get;
			set;
		}

		#endregion
	}
}
