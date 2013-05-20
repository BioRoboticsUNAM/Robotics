using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.StateMachines
{
	/// <summary>
	/// Represents a state of a state machine
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// Gets the zero-based index of the state
		/// </summary>
		/// <returns></returns>
		int StateNumber { get; }

		/// <summary>
		/// Gets a value indicating if the state is an accept state in the state machine
		/// </summary>
		bool IsAcceptState { get; }

		/// <summary>
		/// Executes the current state of the state machine
		/// </summary>
		/// <returns>The zero-based index of the next state to be executed</returns>
		int Execute();
	}
}
