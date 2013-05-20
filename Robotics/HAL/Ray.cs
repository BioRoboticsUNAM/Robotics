using System;
using Robotics.Mathematics;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a inifinite length ray
	/// </summary>
	public class Ray : IIntersectable
	{

		#region Variables

		/// <summary>
		/// The position of the ray
		/// </summary>
		protected Vector3 position;
		/// <summary>
		/// The director vector of the ray
		/// </summary>
		protected Vector3 director;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Ray
		/// </summary>
		public Ray() : this(Vector3.Zero, Vector3.UnitX)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of Ray
		/// </summary>
		/// <param name="rayPosition">The posotion of the ray</param>
		/// <param name="rayDirection">The direction of the ray</param>
		public Ray(Vector3 rayPosition, Vector3 rayDirection)
		{
			position = new Vector3(rayPosition);
			Director = director;
		}

		#endregion

		#region Events
		#endregion

		#region Properties
		#endregion

		#region Methodos
		#endregion

		#region Inherited Methodos
		#endregion

		#region IIntersectable Members

		/// <summary>
		/// Checks if this instance of Ray intersects with another
		/// </summary>
		/// <param name="other">IIntersectable object to check intersection with</param>
		/// <returns>true if there is intersection, false otherwise</returns>
		public bool Intersects(IIntersectable other)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Checks if this instance of Ray contains the provided IIntersectable instance
		/// </summary>
		/// <param name="other">IIntersectable object to check contention with</param>
		/// <returns>true if the IIntersectable object provided is completely contained within, false otherwise</returns>
		public bool Contains(IIntersectable other)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Checks if this instance of Ray contains the point provided by the IPositionable instance
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
		/// Gets or sets the position vector of the ray
		/// </summary>
		public Vector3 Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		/// <summary>
		/// Gets or sets the director vector of the ray
		/// </summary>
		public Vector3 Director
		{
			get
			{
				return director;
			}
			set
			{
				director = value.Unitary;
			}
		}

		/// <summary>
		/// Calculates the distance between this instance and another IPositionable object
		/// </summary>
		/// <param name="p">IPositionable object</param>
		/// <returns>Vector3 that represents the distance vector between two IPositionable objects</returns>
		public Vector3 Distance(IPositionable p)
		{
			if (p == null)
				return null;
			if (p is Ray)
				return Distance((Ray)p);
			Vector3 difference = p.Position - this.position;
			double proy = Vector3.Dot(director, difference);
			Vector3 a = this.position + proy * director;
			return p.Position - a;
		}

		/// <summary>
		/// Calculates the distance vector between two rays
		/// </summary>
		/// <param name="other">Ray to calculate the distance with</param>
		/// <returns>Distance between two rays</returns>
		public Vector3 Distance(Ray other)
		{
			if (other == null)
				return null;

			// Gets the closest point of this ray to the other ray
			Vector3 difference = other.Position - this.position;
			Vector3 closestPoint = this.position + Vector3.Dot(director, difference) * director;
			Vector3 distanceV = other.Position - closestPoint;

			// Distance between paralel rays
			if (this.director == other.director)
				return distanceV;



			return distanceV;
		}

		/// <summary>
		/// Calculates the angle between two rays
		/// </summary>
		/// <param name="other">Ray to calculate the angle with</param>
		/// <returns>Angle between two rays</returns>
		public double Angle(Ray other)
		{
			if(other == null)
				return Double.NaN;

			return Math.Acos(Math.Abs(Vector3.Dot(Director, other.Director))/(Director.Magnitude * other.Director.Magnitude));
		}

		#endregion
	}
}
