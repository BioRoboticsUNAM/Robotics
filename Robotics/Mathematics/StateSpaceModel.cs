using System;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Serves as base class for implement State Space Models
	/// dx = f(x, t, u)
	/// y  = f(x, t, u)
	/// Where:
	///		x	Vector of states of the space model
	///		t	Time
	///		u	External signal
	/// </summary>
	public abstract class StateSpaceModel : IStateSpaceModel
	{
		#region Variables

		/// <summary>
		/// Vector of states of the space model
		/// </summary>
		protected Vector state = new Vector(0);

		#endregion

		#region Porperties

		/// <summary>
		/// Gets a value indicating if the State Space Model is stable
		/// </summary>
		public abstract bool IsStable
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating if the State Space Model is asymptotically stable
		/// </summary>
		public abstract bool IsAsymptoticallyStable
		{
			get;
		}

		/// <summary>
		/// Gets or sets the vector of states of the space model
		/// </summary>
		public virtual Vector State
		{
			get { return this.state; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				this.state = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Calculates the derivative of the State Space Model for a given time and input signal
		/// </summary>
		/// <param name="time">The time for which the derivative will be calculated.</param>
		/// <returns>The derivative of the State Space Model</returns>
		public virtual Vector Derivative(double time)
		{
			return Derivative(time, Vector.Zero(this.state.Dimension));
		}

		/// <summary>
		/// Calculates the derivative of the State Space Model for a given time and input signal
		/// </summary>
		/// <param name="time">The time for which the derivative will be calculated.</param>
		/// <param name="signal">Signal vector used to calculate the derivative.</param>
		/// <returns>The derivative of the State Space Model</returns>
		public abstract Vector Derivative(double time, Vector signal);

		/// <summary>
		/// Calculates the output of the State Space Model for a given time and input signal
		/// </summary>
		/// <param name="time">The time for which the output will be calculated.</param>
		/// <returns>The output of the State Space Model</returns>
		public virtual Vector Output(double time)
		{
			return Output(time, Vector.Zero(this.state.Dimension));
		}

		/// <summary>
		/// Calculates the output of the State Space Model for a given time and input signal
		/// </summary>
		/// <param name="time">The time for which the output will be calculated.</param>
		/// <param name="signal">Signal vector used to calculate the output.</param>
		/// <returns>The output of the State Space Model</returns>
		public abstract Vector Output(double time, Vector signal);

		#endregion
	}
}
