using System;
using System.ComponentModel;
using Robotics.Mathematics;

namespace Robotics.HAL
{
	/// <summary>
	/// Implements an oriented Bounding Box
	/// </summary>
	public abstract class OrientedBoundingBox : IIntersectable
	{
		#region Variables

		/// <summary>
		/// The depth of the BoundingBox object (Length over the y-axis)
		/// </summary>
		protected double depth;
		/// <summary>
		/// The height of the BoundingBox object (Length over the z-axis)
		/// </summary>
		protected double height;
		/// <summary>
		/// The width of the BoundingBox object (Length over the x-axis)
		/// </summary>
		protected double width;

		/// <summary>
		/// The roll of the BoundingBox object (rotation over the z-axis)
		/// </summary>
		protected double roll;
		/// <summary>
		/// The pitch of the BoundingBox object (rotation over the y-axis)
		/// </summary>
		protected double pitch;
		/// <summary>
		/// The yaw of the BoundingBox object (rotation over the x-axis)
		/// </summary>
		protected double yaw;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of BoundingBox
		/// </summary>
		public OrientedBoundingBox()
			: this(Double.Epsilon, Double.Epsilon, Double.Epsilon, 0, 0, 0)
		{
			this.roll = 0;
			this.pitch = 0;
			this.yaw = 0;
		}

		/// <summary>
		/// Initializes a new instance of BoundingBox
		/// </summary>
		/// <param name="depth">The roll of the BoundingBox object</param>
		/// <param name="height">The pitch of the BoundingBox object</param>
		/// <param name="width">The yaw of the BoundingBox object</param>
		public OrientedBoundingBox(double depth, double height, double width)
			: this(depth, height, width, 0, 0, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of BoundingBox
		/// </summary>
		/// <param name="depth">The depth of the BoundingBox object</param>
		/// <param name="height">The height of the BoundingBox object</param>
		/// <param name="width">The width of the BoundingBox object</param>
		/// <param name="roll">The roll of the BoundingBox object</param>
		/// <param name="pitch">The pitch of the BoundingBox object</param>
		/// <param name="yaw">The yaw of the BoundingBox object</param>
		public OrientedBoundingBox(double depth, double height, double width, double roll, double pitch, double yaw)
		{
			this.Depth = Double.Epsilon;
			this.Height = Double.Epsilon;
			this.Width = Double.Epsilon;
			this.Roll = roll;
			this.Pitch = pitch;
			this.Yaw = yaw;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the depth of the BoundingBox object (Length over the y-axis)
		/// </summary>
		[CategoryAttribute("Dimensions")]
		[DescriptionAttribute("Gets or sets the depth of the BoundingBox object (Length over the y-axis)")]
		public virtual double Depth
		{
			get { return this.depth; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException("Value must be greater than or equal to zero");
				this.depth = value;
			}
		}

		/// <summary>
		/// Gets or sets the height of the BoundingBox object (Length over the z-axis)
		/// </summary>
		[CategoryAttribute("Dimensions")]
		[DescriptionAttribute("Gets or sets the height of the BoundingBox object (Length over the z-axis)")]
		public virtual double Height
		{
			get { return this.height; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException("Value must be greater than or equal to zero");
				this.height = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the BoundingBox object (Length over the x-axis)
		/// </summary>
		[CategoryAttribute("Dimensions")]
		[DescriptionAttribute("Gets or sets the width of the BoundingBox object (Length over the x-axis)")]
		public virtual double Width
		{
			get { return this.width; }

			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException("Value must be greater than or equal to zero");
				this.width = value;
			}
		}

		/// <summary>
		/// Gets or sets the roll of the BoundingBox object (rotation over the z-axis)
		/// </summary>
		[CategoryAttribute("Orientation")]
		[DescriptionAttribute("Gets or sets the roll of the BoundingBox object (rotation over the z-axis)")]
		public virtual double Roll
		{
			get { return this.roll; }
			set
			{
				while (value < 0)
					value += Mathematics.MathUtil.TwoPi;
				while (value > Mathematics.MathUtil.TwoPi)
					value -= Mathematics.MathUtil.TwoPi;

				this.roll = value;
			}
		}

		/// <summary>
		/// Gets or sets the pitch of the BoundingBox object (rotation over the y-axis)
		/// </summary>
		[CategoryAttribute("Orientation")]
		[DescriptionAttribute("Gets or sets the pitch of the BoundingBox object (rotation over the y-axis)")]
		public virtual double Pitch
		{
			get { return this.pitch; }
			set
			{
				while (value < 0)
					value += Mathematics.MathUtil.TwoPi;
				while (value > Mathematics.MathUtil.TwoPi)
					value -= Mathematics.MathUtil.TwoPi;

				this.pitch = value;
			}
		}

		/// <summary>
		/// Gets or sets the yaw of the BoundingBox object (rotation over the x-axis)
		/// </summary>
		[CategoryAttribute("Orientation")]
		[DescriptionAttribute("Gets or sets the yaw of the BoundingBox object (rotation over the x-axis)")]
		public virtual double Yaw
		{
			get { return this.yaw; }
			set
			{
				while (value < 0)
					value += Mathematics.MathUtil.TwoPi;
				while (value > Mathematics.MathUtil.TwoPi)
					value -= Mathematics.MathUtil.TwoPi;

				this.yaw = value;
			}
		}

		/// <summary>
		/// Gets the 8 vertexes of the oriented bounding box
		/// </summary>
		[CategoryAttribute("Dimensions")]
		[DescriptionAttribute("Gets the 8 vertexes of the oriented bounding box")]
		public Vector3[] Vertexes
		{
			get
			{
				Vector3[] v = new Vector3[8];
				/*
				v[0] = new Vector3(BoundingBox.Width / 2, BoundingBox.Depth / 2, BoundingBox.Height / 2);
				v[1] = new Vector3(-BoundingBox.Width / 2, BoundingBox.Depth / 2, BoundingBox.Height / 2);
				v[2] = new Vector3(-BoundingBox.Width / 2, -BoundingBox.Depth / 2, BoundingBox.Height / 2);
				v[3] = new Vector3(BoundingBox.Width / 2, -BoundingBox.Depth / 2, BoundingBox.Height / 2);
				v[4] = new Vector3(BoundingBox.Width / 2, BoundingBox.Depth / 2, -BoundingBox.Height / 2);
				v[5] = new Vector3(-BoundingBox.Width / 2, BoundingBox.Depth / 2, -BoundingBox.Height / 2);
				v[6] = new Vector3(-BoundingBox.Width / 2, -BoundingBox.Depth / 2, -BoundingBox.Height / 2);
				v[7] = new Vector3(BoundingBox.Width / 2, -BoundingBox.Depth / 2, -BoundingBox.Height / 2);
				*/
				v[0] = new Vector3(Width / 2, Depth / 2, Height / 2);
				v[1] = new Vector3(-Width / 2, Depth / 2, Height / 2);
				v[2] = new Vector3(-Width / 2, -Depth / 2, Height / 2);
				v[3] = new Vector3(Width / 2, -Depth / 2, Height / 2);
				v[4] = new Vector3(Width / 2, Depth / 2, -Height / 2);
				v[5] = new Vector3(-Width / 2, Depth / 2, -Height / 2);
				v[6] = new Vector3(-Width / 2, -Depth / 2, -Height / 2);
				v[7] = new Vector3(Width / 2, -Depth / 2, -Height / 2);
				return v;
			}
		}

		/// <summary>
		/// Gets the homogeneous matrix which contains the position and orientation of the bounding box
		/// </summary>
		[CategoryAttribute("Transform Matrices")]
		[DescriptionAttribute("Gets the homogeneous matrix which contains the position and orientation of the bounding box")]
		public Matrix Homogeneous
		{
			get
			{
				Matrix om = this.OrientationMatrix3D;
				return new Matrix(4, 4,
					om[0, 0], om[0, 1], om[0, 2], this.Position.X,
					om[1, 0], om[1, 1], om[1, 2], this.Position.Y,
					om[2, 0], om[2, 1], om[2, 2], this.Position.Z,
					0, 0, 0, 1);
			}
		}

		/// <summary>
		/// Gets the 3-dimentional orientation matrix of the OrientedBoundingBox object
		/// </summary>
		[CategoryAttribute("Transform Matrices")]
		[DescriptionAttribute("Gets the 3-dimentional orientation matrix of the OrientedBoundingBox object")]
		public Matrix OrientationMatrix3D
		{
			get
			{
				return new Matrix(
					3, 3,
					Math.Cos(roll) * Math.Cos(pitch),
					-Math.Sin(roll) * Math.Cos(yaw) + Math.Cos(roll) * Math.Sin(pitch) * Math.Sin(yaw),
					Math.Sin(roll) * Math.Sin(yaw) + Math.Cos(roll) * Math.Sin(pitch) * Math.Cos(yaw),
					
					Math.Sin(roll) * Math.Cos(pitch),
					-Math.Cos(roll) * Math.Cos(yaw) + Math.Sin(roll) * Math.Sin(pitch) * Math.Sin(yaw),
					-Math.Cos(roll) * Math.Sin(yaw) + Math.Sin(roll) * Math.Sin(pitch) * Math.Cos(yaw),
					
					-Math.Sin(pitch),
					Math.Cos(pitch) * Math.Sin(yaw),
					Math.Cos(pitch) * Math.Cos(yaw));
			}
		}

		/// <summary>
		/// Gets the 4-dimentional orientation matrix of the OrientedBoundingBox object
		/// </summary>
		[CategoryAttribute("Transform Matrices")]
		[DescriptionAttribute("Gets the 4-dimentional orientation matrix of the OrientedBoundingBox object")]
		public Matrix OrientationMatrix4D
		{
			get
			{
				return new Matrix(
					4, 4,
					Math.Cos(roll) * Math.Cos(pitch),
					-Math.Sin(roll) * Math.Cos(yaw) + Math.Cos(roll) * Math.Sin(pitch) * Math.Sin(yaw),
					Math.Sin(roll) * Math.Sin(yaw) + Math.Cos(roll) * Math.Sin(pitch) * Math.Cos(yaw),
					0,
					
					Math.Sin(roll) * Math.Cos(pitch),
					-Math.Cos(roll) * Math.Cos(yaw) + Math.Sin(roll) * Math.Sin(pitch) * Math.Sin(yaw),
					-Math.Cos(roll) * Math.Sin(yaw) + Math.Sin(roll) * Math.Sin(pitch) * Math.Cos(yaw),
					0,
					
					-Math.Sin(pitch),
					Math.Cos(pitch) * Math.Sin(yaw),
					Math.Cos(pitch) * Math.Cos(yaw),
					0,

					0, 0, 0, 1);
			}
		}

		/// <summary>
		/// Gets the omogeneous matrix which contains the position and orientation of the bounding box
		/// This matrix is calculated using precalculated values of trigonometric functions
		/// </summary>
		[CategoryAttribute("Transform Matrices")]
		[DescriptionAttribute("Gets the omogeneous matrix which contains the position and orientation of the bounding box" + 
			"This matrix is calculated using precalculated values of trigonometric functions")]
		public Matrix QuickHomogeneous
		{
			get
			{
				Matrix om = this.QuickOrientation3D;
				return new Matrix(4, 4,
					om[0, 0], om[0, 1], om[0, 2], this.Position.X,
					om[1, 0], om[1, 1], om[1, 2], this.Position.Y,
					om[2, 0], om[2, 1], om[2, 2], this.Position.Z,
					0, 0, 0, 1);
			}
		}

		/// <summary>
		/// Gets the 3-dimentional orientation matrix of the OrientedBoundingBox object
		/// calculated using pre-calculated sin/cos values
		/// </summary>
		[CategoryAttribute("Transform Matrices")]
		[DescriptionAttribute("Gets the 3-dimentional orientation matrix of the OrientedBoundingBox object" +
			"calculated using pre-calculated sin/cos values")]
		public Matrix QuickOrientation3D
		{
			get
			{
				return new Matrix(
					3, 3,

					MathUtil.Cos(roll) * MathUtil.Cos(pitch),
					-MathUtil.Sin(roll) * MathUtil.Cos(yaw) + MathUtil.Cos(roll) * MathUtil.Sin(pitch) * MathUtil.Sin(yaw),
					MathUtil.Sin(roll) * MathUtil.Sin(yaw) + MathUtil.Cos(roll) * MathUtil.Sin(pitch) * MathUtil.Cos(yaw),

					MathUtil.Sin(roll) * MathUtil.Cos(pitch),
					-MathUtil.Cos(roll) * MathUtil.Cos(yaw) + MathUtil.Sin(roll) * MathUtil.Sin(pitch) * MathUtil.Sin(yaw),
					-MathUtil.Cos(roll) * MathUtil.Sin(yaw) + MathUtil.Sin(roll) * MathUtil.Sin(pitch) * MathUtil.Cos(yaw),

					-MathUtil.Sin(pitch),
					MathUtil.Cos(pitch) * MathUtil.Sin(yaw),
					MathUtil.Cos(pitch) * MathUtil.Cos(yaw));
			}
		}

		/// <summary>
		/// Gets the 4-dimentional orientation matrix of the OrientedBoundingBox object
		/// calculated using pre-calculated sin/cos values
		/// </summary>
		[CategoryAttribute("Transform Matrices")]
		[DescriptionAttribute("Gets the 4-dimentional orientation matrix of the OrientedBoundingBox object " +
			"calculated using pre-calculated sin/cos values")]
		public Matrix QuickOrientation4D
		{
			get
			{
				return new Matrix(
					4, 4,
					MathUtil.Cos(roll) * MathUtil.Cos(pitch),
					-MathUtil.Sin(roll) * MathUtil.Cos(yaw) + MathUtil.Cos(roll) * MathUtil.Sin(pitch) * MathUtil.Sin(yaw),
					MathUtil.Sin(roll) * MathUtil.Sin(yaw) + MathUtil.Cos(roll) * MathUtil.Sin(pitch) * MathUtil.Cos(yaw),
					0,

					MathUtil.Sin(roll) * MathUtil.Cos(pitch),
					-MathUtil.Cos(roll) * MathUtil.Cos(yaw) + MathUtil.Sin(roll) * MathUtil.Sin(pitch) * MathUtil.Sin(yaw),
					-MathUtil.Cos(roll) * MathUtil.Sin(yaw) + MathUtil.Sin(roll) * MathUtil.Sin(pitch) * MathUtil.Cos(yaw),
					0,

					-MathUtil.Sin(pitch),
					MathUtil.Cos(pitch) * MathUtil.Sin(yaw),
					MathUtil.Cos(pitch) * MathUtil.Cos(yaw),
					0,

					0, 0, 0, 1);
			}

		}

		#endregion

		#region IPositionable Members

		/// <summary>
		/// Gets or sets the position of the centroid of the Intersectable object
		/// </summary>
		[CategoryAttribute("Position")]
		[DescriptionAttribute("Gets or sets the position of the centroid of the OrientedBoundingBox object")]
		public abstract Vector3 Position
		{
			get;
			set;
		}

		/// <summary>
		/// Calculates the distance between this instance and another IPositionable object
		/// </summary>
		/// <param name="p">IPositionable object</param>
		/// <returns>Vector3 that represents the distance vector between two IPositionable objects</returns>
		public virtual Vector3 Distance(IPositionable p)
		{
			if (p == null) throw new ArgumentNullException();
			return this.Position - p.Position;
		}

		#endregion

		#region Methods



		#endregion

		#region IIntersectable Members

		/// <summary>
		/// Checks if this instance of IIntersectable intersects with another
		/// </summary>
		/// <param name="other">IIntersectable object to check intersection with</param>
		/// <returns>true if there is intersection, false otherwise</returns>
		public virtual bool Intersects(IIntersectable other)
		{
			throw new NotImplementedException();
			//// float ra, rb;
			//// Matrix33 R, AbsR;
			//int i, j;

			//// Compute rotation matrix expressing b in a's coordinate frame
			//for (i = 0; i < 3; i++)
			//{
			//	for (j = 0; j < 3; j++) ;
			//	//R[i][j] = Dot(a.u[i], b.u[j]);
			//}
			//
			/*
int TestOBBOBB(OBB &a, OBB &b)
{
   
   
   
   
   // Compute translation vector t
   Vector t = b.c - a.c;
   // Bring translation into a’s coordinate frame
   t = Vector(Dot(t, a.u[0]), Dot(t, a.u[2]), Dot(t, a.u[2]));
   // Compute common subexpressions. Add in an epsilon term to
   // counteract arithmetic errors when two edges are parallel and
   // their cross product is (near) null (see text for details)
   for (int i = 0; i < 3; i++)
	  for (int j = 0; j < 3; j++)
		 AbsR[i][j] = Abs(R[i][j]) + EPSILON;
   // Test axes L = A0, L = A1, L = A2
   for (int i = 0; i < 3; i++) {
	  ra = a.e[i];
	  rb = b.e[0] * AbsR[i][0] + b.e[1] * AbsR[i][1] + b.e[2] * AbsR[i][2];
	  if (Abs(t[i]) > ra + rb) return 0;
   }
   // Test axes L = B0, L = B1, L = B2
   for (int i = 0; i < 3; i++) {
	  ra = a.e[0] * AbsR[0][i] + a.e[1] * AbsR[1][i] + a.e[2] * AbsR[2][i];
	  rb = b.e[i];
	  if (Abs(t[0] * R[0][i] + t[1] * R[1][i] + t[2] * R[2][i]) > ra + rb) return 0;
   }
   // Test axis L = A0 x B0
   ra = a.e[1] * AbsR[2][0] + a.e[2] * AbsR[1][0];
   rb = b.e[1] * AbsR[0][2] + b.e[2] * AbsR[0][1];
   if (Abs(t[2] * R[1][0] - t[1] * R[2][0]) > ra + rb) return 0;
   // Test axis L = A0 x B1
   ra = a.e[1] * AbsR[2][1] + a.e[2] * AbsR[1][1];
   rb = b.e[0] * AbsR[0][2] + b.e[2] * AbsR[0][0];
   if (Abs(t[2] * R[1][1] - t[1] * R[2][1]) > ra + rb) return 0;
   // Test axis L = A0 x B2
   ra = a.e[1] * AbsR[2][2] + a.e[2] * AbsR[1][2];
   rb = b.e[0] * AbsR[0][1] + b.e[1] * AbsR[0][0];
   if (Abs(t[2] * R[1][2] - t[1] * R[2][2]) > ra + rb) return 0;
   // Test axis L = A1 x B0
   ra = a.e[0] * AbsR[2][0] + a.e[2] * AbsR[0][0];
   rb = b.e[1] * AbsR[1][2] + b.e[2] * AbsR[1][1];
   if (Abs(t[0] * R[2][0] - t[2] * R[0][0]) > ra + rb) return 0;
   // Test axis L = A1 x B1
   ra = a.e[0] * AbsR[2][1] + a.e[2] * AbsR[0][1];
   rb = b.e[0] * AbsR[1][2] + b.e[2] * AbsR[1][0];
   if (Abs(t[0] * R[2][1] - t[2] * R[0][1]) > ra + rb) return 0;
   // Test axis L = A1 x B2
   ra = a.e[0] * AbsR[2][2] + a.e[2] * AbsR[0][2];
   rb = b.e[0] * AbsR[1][1] + b.e[1] * AbsR[1][0];
   if (Abs(t[0] * R[2][2] - t[2] * R[0][2]) > ra + rb) return 0;
   // Test axis L = A2 x B0
   ra = a.e[0] * AbsR[1][0] + a.e[1] * AbsR[0][0];
   rb = b.e[1] * AbsR[2][2] + b.e[2] * AbsR[2][1];
   if (Abs(t[1] * R[0][0] - t[0] * R[1][0]) > ra + rb) return 0;
   // Test axis L = A2 x B1
   ra = a.e[0] * AbsR[1][1] + a.e[1] * AbsR[0][1];
   rb = b.e[0] * AbsR[2][2] + b.e[2] * AbsR[2][0];
   if (Abs(t[1] * R[0][1] - t[0] * R[1][1]) > ra + rb) return 0;
   // Test axis L = A2 x B2
   ra = a.e[0] * AbsR[1][2] + a.e[1] * AbsR[0][2];
   rb = b.e[0] * AbsR[2][1] + b.e[1] * AbsR[2][0];
   if (Abs(t[1] * R[0][2] - t[0] * R[1][2]) > ra + rb) return 0;
   // Since no separating axis is found, the OBBs must be intersecting
   return 1;
}
*/
		}

		/// <summary>
		/// Checks if this instance of IIntersectable contains the provided IIntersectable instance
		/// </summary>
		/// <param name="other">IIntersectable object to check contention with</param>
		/// <returns>true if the IIntersectable object provided is completely contained within, false otherwise</returns>
		public virtual bool Contains(IIntersectable other)
		{
			throw new NotImplementedException();
			//Vector3[] vertexes = new Vector3[]
			//{
			//	// Front
			//	// Upper right
			//	// Upper left
			//	// Lower left
			//	// Lower right

			//	// Back
			//	// Upper right
			//	// Upper left
			//	// Lower left
			//	// Lower right
			//};
		}

		/// <summary>
		/// Checks if this instance of IIntersectable contains the provided IPositionable instance
		/// </summary>
		/// <param name="position">IPositionable object to check contention with</param>
		/// <returns>true if the IPositionable object provided is inside this instance, false otherwise</returns>
		public virtual bool Contains(IPositionable position)
		{
			throw new NotImplementedException();

			//Matrix homogeneous;
			//homogeneous = Homogeneous;

			//for (int i = 0; i < 7; ++i)
			//	Vertexes[i] = (Vector3)(homogeneous * (Vector)Vertexes[i]);

		}

		#endregion
	}
}
/*
 * int TestOBBOBB(OBB &a, OBB &b)
{
   float ra, rb;
   Matrix33 R, AbsR;
   // Compute rotation matrix expressing b in a’s coordinate frame
   for (int i = 0; i < 3; i++)
	  for (int j = 0; j < 3; j++)
		 R[i][j] = Dot(a.u[i], b.u[j]);
   // Compute translation vector t
   Vector t = b.c - a.c;
   // Bring translation into a’s coordinate frame
   t = Vector(Dot(t, a.u[0]), Dot(t, a.u[2]), Dot(t, a.u[2]));
   // Compute common subexpressions. Add in an epsilon term to
   // counteract arithmetic errors when two edges are parallel and
   // their cross product is (near) null (see text for details)
   for (int i = 0; i < 3; i++)
	  for (int j = 0; j < 3; j++)
		 AbsR[i][j] = Abs(R[i][j]) + EPSILON;
   // Test axes L = A0, L = A1, L = A2
   for (int i = 0; i < 3; i++) {
	  ra = a.e[i];
	  rb = b.e[0] * AbsR[i][0] + b.e[1] * AbsR[i][1] + b.e[2] * AbsR[i][2];
	  if (Abs(t[i]) > ra + rb) return 0;
   }
   // Test axes L = B0, L = B1, L = B2
   for (int i = 0; i < 3; i++) {
	  ra = a.e[0] * AbsR[0][i] + a.e[1] * AbsR[1][i] + a.e[2] * AbsR[2][i];
	  rb = b.e[i];
	  if (Abs(t[0] * R[0][i] + t[1] * R[1][i] + t[2] * R[2][i]) > ra + rb) return 0;
   }
   // Test axis L = A0 x B0
   ra = a.e[1] * AbsR[2][0] + a.e[2] * AbsR[1][0];
   rb = b.e[1] * AbsR[0][2] + b.e[2] * AbsR[0][1];
   if (Abs(t[2] * R[1][0] - t[1] * R[2][0]) > ra + rb) return 0;
   // Test axis L = A0 x B1
   ra = a.e[1] * AbsR[2][1] + a.e[2] * AbsR[1][1];
   rb = b.e[0] * AbsR[0][2] + b.e[2] * AbsR[0][0];
   if (Abs(t[2] * R[1][1] - t[1] * R[2][1]) > ra + rb) return 0;
   // Test axis L = A0 x B2
   ra = a.e[1] * AbsR[2][2] + a.e[2] * AbsR[1][2];
   rb = b.e[0] * AbsR[0][1] + b.e[1] * AbsR[0][0];
   if (Abs(t[2] * R[1][2] - t[1] * R[2][2]) > ra + rb) return 0;
   // Test axis L = A1 x B0
   ra = a.e[0] * AbsR[2][0] + a.e[2] * AbsR[0][0];
   rb = b.e[1] * AbsR[1][2] + b.e[2] * AbsR[1][1];
   if (Abs(t[0] * R[2][0] - t[2] * R[0][0]) > ra + rb) return 0;
   // Test axis L = A1 x B1
   ra = a.e[0] * AbsR[2][1] + a.e[2] * AbsR[0][1];
   rb = b.e[0] * AbsR[1][2] + b.e[2] * AbsR[1][0];
   if (Abs(t[0] * R[2][1] - t[2] * R[0][1]) > ra + rb) return 0;
   // Test axis L = A1 x B2
   ra = a.e[0] * AbsR[2][2] + a.e[2] * AbsR[0][2];
   rb = b.e[0] * AbsR[1][1] + b.e[1] * AbsR[1][0];
   if (Abs(t[0] * R[2][2] - t[2] * R[0][2]) > ra + rb) return 0;
   // Test axis L = A2 x B0
   ra = a.e[0] * AbsR[1][0] + a.e[1] * AbsR[0][0];
   rb = b.e[1] * AbsR[2][2] + b.e[2] * AbsR[2][1];
   if (Abs(t[1] * R[0][0] - t[0] * R[1][0]) > ra + rb) return 0;
   // Test axis L = A2 x B1
   ra = a.e[0] * AbsR[1][1] + a.e[1] * AbsR[0][1];
   rb = b.e[0] * AbsR[2][2] + b.e[2] * AbsR[2][0];
   if (Abs(t[1] * R[0][1] - t[0] * R[1][1]) > ra + rb) return 0;
   // Test axis L = A2 x B2
   ra = a.e[0] * AbsR[1][2] + a.e[1] * AbsR[0][2];
   rb = b.e[0] * AbsR[2][1] + b.e[1] * AbsR[2][0];
   if (Abs(t[1] * R[0][2] - t[0] * R[1][2]) > ra + rb) return 0;
   // Since no separating axis is found, the OBBs must be intersecting
   return 1;
}
*/