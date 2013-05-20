using System;

namespace Robotics.StateMachines
{
	/// <summary>
	/// Represents a state function of a state machine
	/// </summary>
	/// <param name="currentState">The current state of the state machine</param>
	/// <param name="o">Additional parameters used by the function</param>
	/// <returns>The number of the next state of the state machine</returns>
	public delegate int SMStateFuncion(int currentState, object o);

	/// <summary>
	/// Represents a state function of a state machine
	/// </summary>
	/// <param name="currentState">The current state of the state machine</param>
	/// <param name="o">Additional parameters used by the function</param>
	/// <returns>The number of the next state of the state machine</returns>
	public delegate T SMEnumeratedStateFuncion<T>(T currentState, object o)
	where T : struct, IComparable, IConvertible, IFormattable;
}