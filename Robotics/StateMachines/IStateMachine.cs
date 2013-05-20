using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.StateMachines
{
	/// <summary>
	/// Represents a state machine
	/// </summary>
	public interface IStateMachine
	{
		/// <summary>
		/// Gets the number of states of the state machine
		/// </summary>
		int StateCount { get; }

		/// <summary>
		/// Gets the zero-based index of the current state of the state machine
		/// </summary>
		int CurrentStateIndex { get; }

		/// <summary>
		/// Gets a value indicating if the current state is an accept state
		/// </summary>
		bool CurrentStateIsAccept { get; } 

		/// <summary>
		/// Gets a value indicating if the current state is the final state
		/// </summary>
		bool CurrentStateIsFinal{get;}

		/// <summary>
		/// Gets a value indicating if the state machine reached its final state
		/// </summary>
		bool Finished { get; }

		/// <summary>
		/// Gets the index of the final state
		/// </summary>
		int FinalStateIndex { get; }

		/// <summary>
		/// Gets a value indicating if the state machine is currently executing a state
		/// </summary>
		bool ExecutingState { get; }

		/// <summary>
		/// Executes the current step and advances the state machine to the next step
		/// </summary>
		void RunNextStep();

		/// <summary>
		/// Executes the state machine untill it reaches an accept state
		/// </summary>
		void RunToNextAcceptState();

		/// <summary>
		/// Executes the state machine untill it reaches an accept state or a maximum number of steps
		/// are executed
		/// </summary>
		/// <param name="maxSteps">The maximum number of steps allowed for the state machine</param>
		void RunToNextAcceptState(int maxSteps);

		/// <summary>
		/// Executes the state machine untill it reaches the final state
		/// </summary>
		void RunUntillFinished();

		/// <summary>
		/// Executes the state machine untill it reaches the final state or a maximum number of steps
		/// are executed
		/// </summary>
		/// <param name="maxSteps">The maximum number of steps allowed for the state machine</param>
		void RunUntillFinished(int maxSteps);

		/// <summary>
		/// Resets the state machine
		/// </summary>
		void Reset();

		/// <summary>
		/// Sets the final state of the state machine. If the final state does not belong to the state machine it is added.
		/// </summary>
		/// <param name="state">The final state of the state machine.</param>
		void SetFinalState(IState state);

		/// <summary>
		/// Sets the final state of the state machine.
		/// </summary>
		/// <param name="stateIndex">The index of the final state of the state machine.</param>
		void SetFinalState(int stateIndex);
	}
}
