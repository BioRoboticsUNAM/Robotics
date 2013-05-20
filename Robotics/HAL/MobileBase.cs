using System;
using System.Collections.Generic;
using System.ComponentModel;
using Robotics;
using Robotics.Mathematics;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a robot's mobile base
	/// </summary>
	public abstract class MobileBase :OrientedBoundingBox, IPositionable
	{
		#region Variables

		/// <summary>
		/// Stores the robot diameter, in meters
		/// </summary>
		protected double robotDiameter;
		/// <summary>
		/// Stores the maximum forward speed of the robot, in radians per second
		/// </summary>
		protected double maxForwardSpeed;
		/// <summary>
		/// Stores the minimum forward speed of the robot, in radians per second
		/// </summary>
		protected double maxBackwardSpeed;

		/// <summary>
		/// Stores the position of the robot, in meters
		/// </summary>
		protected Vector3 position;
		/// <summary>
		/// Stores the orientation of the robot, in radians between [-Pi, Pi]
		/// </summary>
		protected double orientation;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Robot
		/// </summary>
		public MobileBase()
			: this(new Vector3(), 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of Robot
		/// </summary>
		/// <param name="position">Initial position of the robot</param>
		public MobileBase(Vector3 position)
			: this(position, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of Robot
		/// </summary>
		/// <param name="position">Initial position of the robot</param>
		/// <param name="orientation">Initial orientation of the robot</param>
		public MobileBase(Vector3 position, double orientation)
		{
			Orientation = orientation;
			Position = new Vector3(position.X, position.Y, position.Z);
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the robot diameter, in meters
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the robot diameter, in meters")]
		public virtual double RobotDiameter
		{
			get { return this.robotDiameter; }
			set
			{
				if (value <= 0) throw new ArgumentOutOfRangeException("value must be greater than zero");
				this.robotDiameter = value;
			}
		}

		/// <summary>
		/// Gets the maximum forward speed of the robot, in radians per second
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the maximum forward speed of the robot, in radians per second")]
		public virtual double MaxForwardSpeed
		{
			get { return this.maxForwardSpeed; }
			set
			{
				if (value <= 0) throw new ArgumentOutOfRangeException("value must be greater than zero");
				this.maxForwardSpeed = value;
			}
		}

		/// <summary>
		/// Gets the minimum forward speed of the robot, in radians per second
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the minimum forward speed of the robot, in radians per second")]
		public virtual double MaxBackwardSpeed
		{
			get { return this.maxBackwardSpeed; }
			set
			{
				if (value <= 0) throw new ArgumentOutOfRangeException("value must be greater than zero");
				this.maxBackwardSpeed = value;
			}
		}

		/// <summary>
		/// Gets or sets the position of the robot, in meters
		/// </summary>
		[CategoryAttribute("Status")]
		[DescriptionAttribute("Gets or sets the position of the robot, in meters")]
		public override Vector3 Position
		{
			get { return position; }
			set
			{
				//if(value == null) throw new ArgumentNullException();
				this.position = value;
			}
		}

		/// <summary>
		/// Gets or sets the orientation of the robot, in radians between [-Pi, Pi]
		/// </summary>
		[CategoryAttribute("Status")]
		[DescriptionAttribute("Gets or sets the orientation of the robot, in radians between [-Pi, Pi]")]
		public virtual double Orientation
		{
			get { return this.orientation; }
			set
			{
				while (value > Math.PI) value -= 2*Math.PI;
				while (value < -Math.PI) value += 2*Math.PI;
				this.orientation = value;
			}
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Immediatly stops the robot and aplies the break if any.
		/// </summary>
		public abstract void Break();

		/// <summary>
		/// Request to the robot to execute a MV command
		/// </summary>
		/// <param name="distance">Distance in meters the robot must advance.
		/// This parameter is passed by reference, its value will be modified with the total distance advanced by the robot</param>
		/// <param name="angle">Angle in radians the robot must turn.
		/// This parameter is passed by reference, its value will be modified with the total angle turned by the robot</param>
		/// <param name="time">Time of command execution.
		/// This parameter is passed by reference, its value will be modified with the total execution time</param>
		/// <returns>true if command is supported by the robot, false otherwise</returns>
		public abstract bool ExecuteLowLevelMV(ref double distance, ref double angle, ref double time);

		/// <summary>
		/// Resets the base of the robot to its initial state.
		/// </summary>
		/// <remarks>This command stops the robot and clear the encoder values.</remarks>
		public abstract void ResetRobotBase();

		/// <summary>
		/// Sets the speed of the robot to zero
		/// </summary>
		public abstract void Stop();

		#endregion
	}
}
