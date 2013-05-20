using System;
using System.Collections.Generic;
using System.ComponentModel;
using Robotics;
using Robotics.Mathematics;

namespace Robotics.HAL
{
	/// <summary>
	/// Represents a robot's mobile base which works with differential pair
	/// </summary>
	public abstract class DiferentialPairMobileBase:MobileBase
	{
		#region Constants

		#endregion

		#region Variables

		/// <summary>
		/// Stores the distance advanced by the robot, for each encoder pulse, in meters
		/// </summary>
		[Obsolete("This attribute shall not be used and will be removed in further versions", true)]
		protected double metersPerPulse;

		/// <summary>
		/// Stores the distance advanced by the left wheel of the robot, for each left encoder pulse, in meters
		/// </summary>
		protected double metersPerPulseLeft;

		/// <summary>
		/// Stores the distance advanced by the right wheel of the robot, for each right encoder pulse, in meters
		/// </summary>
		protected double metersPerPulseRight;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Robot
		/// </summary>
		public DiferentialPairMobileBase()
			: this(new Vector3(), 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of Robot
		/// </summary>
		/// <param name="position">Initial position of the robot</param>
		public DiferentialPairMobileBase(Vector3 position)
			: this(position, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of Robot
		/// </summary>
		/// <param name="position">Initial position of the robot</param>
		/// <param name="orientation">Initial orientation of the robot</param>
		public DiferentialPairMobileBase(Vector3 position, double orientation)
		{
			Orientation = orientation;
			Position = new Vector3(position.X, position.Y, position.Z);
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the average distance advanced by the robot, for each encoder pulse, in meters
		/// </summary>
		[Obsolete("This property shall not be used and will be removed in further versions")]
		[Browsable(false)]
		public virtual double MetersPerPulse
		{
			get { return (this.metersPerPulseLeft + this.metersPerPulseRight)/2; }
			set { }
		}

		/// <summary>
		/// Gets the distance advanced by the left wheel of the robot, for each left encoder pulse, in meters
		/// </summary>
		[CategoryAttribute("Hardware Information")]
		[DescriptionAttribute("Gets the distance advanced by the left wheel of the robot, for each left encoder pulse, in meters")]
		public virtual double MetersPerPulseLeft
		{
			get { return this.metersPerPulseLeft; }
			set
			{
				if ((value < 0) || (value > 1)) throw new ArgumentOutOfRangeException("Value must be between zero and 1");
				this.metersPerPulseLeft = value;
			}
		}

		/// <summary>
		/// Gets the distance advanced by the right wheel of the robot, for each right encoder pulse, in meters
		/// </summary>
		[CategoryAttribute("Hardware Information")]
		[DescriptionAttribute("Gets the distance advanced by the right wheel of the robot, for each right encoder pulse, in meters")]
		public virtual double MetersPerPulseRight
		{
			get { return this.metersPerPulseRight; }
			set
			{
				if ((value < 0) || (value > 1)) throw new ArgumentOutOfRangeException("Value must be between zero and 1");
				this.metersPerPulseRight = value;
			}
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Resets the value of the encoders of the robot to zero
		/// </summary>
		public abstract bool ClearEncoders();

		/// <summary>
		/// Read the encoder values of the robot
		/// </summary>
		/// <param name="leftEncoderValue">The value of the left wheel encoder in pulses</param>
		/// <param name="rightEncoderValue">The value of the right wheel encoder in pulses</param>
		public abstract bool ReadEncoders(out int leftEncoderValue, out int rightEncoderValue);

		/// <summary>
		/// Read the encoder values of the robot
		/// </summary>
		/// <param name="leftEncoderValue">The value of the left wheel encoder in meters</param>
		/// <param name="rightEncoderValue">The value of the right wheel encoder in meters</param>
		public abstract bool ReadEncoders(out double leftEncoderValue, out double rightEncoderValue);

		/// <summary>
		/// Resets the base of the robot to its initial state.
		/// </summary>
		/// <remarks>This command stops the robot and clear the encoder values.</remarks>
		public override void ResetRobotBase()
		{
			Stop();
			ClearEncoders();
		}

		/// <summary>
		/// Sets the angular speed of both wheels of the robot
		/// </summary>
		/// <param name="left">Left angular speed in radians per second</param>
		/// <param name="right">Right angular speed in radians per second</param>
		public abstract bool SetAngularSpeeds(double left, double right);

		/// <summary>
		/// Sets the torque of both motor's wheels of the robot
		/// </summary>
		/// <param name="left">Torque for left motor wheel</param>
		/// <param name="right">Torque for right motor wheel</param>
		public abstract bool SetTorques(double left, double right);

		/// <summary>
		/// Sets the angular speed of both wheels of the robot
		/// </summary>
		/// <param name="left">Left angular speed in percentage</param>
		/// <param name="right">Right angular speed in percentage</param>
		/// <remarks>Angular speeds must be in the interval [-100, 100]</remarks>
		public virtual bool SetAngularSpeeds(int left, int right)
		{
			double ls, rs;

			ls = (double)left / 100.0;
			ls *= (left > 0) ? maxForwardSpeed : maxBackwardSpeed;
			rs = (double)right / 100.0;
			rs *= (right > 0) ? maxForwardSpeed : maxBackwardSpeed;
			return SetAngularSpeeds(ls, rs);
		}

		/// <summary>
		/// Sets the angular speed of both wheels of the robot
		/// </summary>
		/// <param name="left">Left angular speed as a 8bit integer</param>
		/// <param name="right">Right angular speed as a 8bit integer</param>
		/// <remarks>Angular speeds must be in the interval [-100, 100]</remarks>
		public virtual bool SetAngularSpeeds(byte left, byte right)
		{
			double ls, rs;

			//v(b)=(b-127)*(1/128)
			ls = 0.0078125 * (left - 127);
			rs = 0.0078125 * (right - 127);
			ls*= (left >= 127) ? maxForwardSpeed : maxBackwardSpeed;
			rs*= (right >= 127) ? maxForwardSpeed : maxBackwardSpeed;
			return SetAngularSpeeds(ls, rs);
		}

		/// <summary>
		/// Sets the speed of the robot to zero
		/// </summary>
		public override void Stop()
		{
			SetAngularSpeeds(0, 0);
		}

		#endregion

	}
}