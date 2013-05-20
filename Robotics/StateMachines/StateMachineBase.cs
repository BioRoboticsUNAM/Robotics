using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.StateMachines
{
	/// <summary>
	/// Serves as base class to execute state machines
	/// </summary>
	public abstract class StateMachineBase : IStateMachine
	{
		#region Variables

		#endregion

		#region Constructors
		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of states of the state machine
		/// </summary>
		public abstract int StateCount { get; }

		/// <summary>
		/// Gets or sets the zero-based index of the current state of the state machine
		/// </summary>
		public abstract int CurrentStateIndex
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating if the current state is an accept state
		/// </summary>
		public abstract bool CurrentStateIsAccept { get; }

		/// <summary>
		/// Gets a value indicating if the current state is the final state
		/// </summary>
		public abstract bool CurrentStateIsFinal { get; }

		/// <summary>
		/// Gets a value indicating if the state machine is currently executing a state
		/// </summary>
		public abstract bool ExecutingState { get; }

		/// <summary>
		/// Gets the index of the final state
		/// </summary>
		public abstract int FinalStateIndex { get; }

		/// <summary>
		/// Gets a value indicating if the state machine reached its final state
		/// </summary>
		public abstract bool Finished { get; }

		#endregion

		#region Methodos

		/// <summary>
		/// Executes the current step and advances the state machine to the next step
		/// </summary>
		public abstract void RunNextStep();

		/// <summary>
		/// Executes the state machine untill it reaches an accept state (the accept state is NOT executed)
		/// </summary>
		public virtual void RunToNextAcceptState()
		{
			// Advance untill the next state (unexecuted) is reached
			while (!this.CurrentStateIsAccept && (this.CurrentStateIndex != -1))
				RunNextStep();
		}

		/// <summary>
		/// Executes the state machine untill it reaches an accept state or a maximum number of steps
		/// are executed (the accept state is NOT executed)
		/// </summary>
		/// <param name="maxSteps">The maximum number of steps allowed for the state machine</param>
		public virtual void RunToNextAcceptState(int maxSteps)
		{
			for (int i = 0; (i < maxSteps) && !this.CurrentStateIsAccept && (this.CurrentStateIndex != -1); ++i)
				RunNextStep();
		}

		/// <summary>
		/// Executes the state machine untill it reaches a final state
		/// </summary>
		public virtual void RunUntillFinished()
		{
			// Advance untill the next state (unexecuted) is the final state
			while (!this.Finished && (this.CurrentStateIndex != -1))
				RunNextStep();
			// Now execute the final state
			if(this.CurrentStateIndex != -1)
				RunNextStep();
		}

		/// <summary>
		/// Executes the state machine untill it reaches a final state or a maximum number of steps
		/// are executed
		/// </summary>
		/// <param name="maxSteps">The maximum number of steps allowed for the state machine</param>
		public virtual void RunUntillFinished(int maxSteps)
		{
			// Advance untill the next state (unexecuted) is the final state or maxSteps -1 is reached
			for (int i = 0; (i < maxSteps - 1) && !this.CurrentStateIsFinal && (this.CurrentStateIndex != -1); ++i)
				RunNextStep();
			// Now execute the final state
			if(this.CurrentStateIndex != -1)
				RunNextStep();
		}

		/// <summary>
		/// Resets the state machine
		/// </summary>
		public virtual void Reset()
		{
			this.CurrentStateIndex = 0;
		}

		/// <summary>
		/// Sets the final state of the state machine. If the final state does not belong to the state machine it is added.
		/// </summary>
		/// <param name="state">The final state of the state machine.</param>
		public abstract void SetFinalState(IState state);

		/// <summary>
		/// Sets the final state of the state machine.
		/// </summary>
		/// <param name="stateIndex">The index of the final state of the state machine.</param>
		public abstract void SetFinalState(int stateIndex);

		#endregion
	}
}
