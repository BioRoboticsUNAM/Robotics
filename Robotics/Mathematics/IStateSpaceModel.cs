using System;

namespace Robotics.Mathematics
{
	/// <summary>
	/// Represents a State Space Model
	/// dx = f(x, t, u)
	/// y  = f(x, t, u)
	/// Where:
	///		x	Vector of states of the space model
	///		t	Time
	///		u	External signal
	/// </summary>
	public interface IStateSpaceModel
	{
		#region Porperties

		/// <summary>
		/// Gets a value indicating if the State Space Model is stable
		/// </summary>
		bool IsStable
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating if the State Space Model is asymptotically stable
		/// </summary>
		bool IsAsymptoticallyStable
		{
			get;
		}

		/// <summary>
		/// Gets or sets the vector of states of the space model
		/// </summary>
		Vector State
		{
			get;
			set;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Calculates the derivative of the State Space Model for a given time and input signal
		/// </summary>
		/// <param name="time">The time for which the derivative will be calculated.</param>
		/// <returns>The derivative of the State Space Model</returns>
		Vector Derivative(double time);

		/// <summary>
		/// Calculates the derivative of the State Space Model for a given time and input signal
		/// </summary>
		/// <param name="time">The time for which the derivative will be calculated.</param>
		/// <param name="signal">Signal vector used to calculate the derivative.</param>
		/// <returns>The derivative of the State Space Model</returns>
		Vector Derivative(double time, Vector signal);

		/// <summary>
		/// Calculates the output of the State Space Model for a given time and input signal
		/// </summary>
		/// <param name="time">The time for which the output will be calculated.</param>
		/// <returns>The output of the State Space Model</returns>
		Vector Output(double time);

		/// <summary>
		/// Calculates the output of the State Space Model for a given time and input signal
		/// </summary>
		/// <param name="time">The time for which the output will be calculated.</param>
		/// <param name="signal">Signal vector used to calculate the output.</param>
		/// <returns>The output of the State Space Model</returns>
		Vector Output(double time, Vector signal);

		#endregion
	}
}
