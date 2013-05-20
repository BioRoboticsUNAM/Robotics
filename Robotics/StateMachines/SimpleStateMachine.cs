using System;
using System.Collections.Generic;
using System.Threading;

namespace Robotics.StateMachines
{
	/// <summary>
	/// Implements a simple state machine
	/// </summary>
	public class SimpleStateMachine<T> : StateMachineBase where T : IState
	{
		#region Variables

		/// <summary>
		/// Stores the a list of states to be executed by the state machine
		/// </summary>
		private List<T> states;

		/// <summary>
		/// Stores the index of the current state of the state machine
		/// </summary>
		private int currentStateIx;

		/// <summary>
		/// Object used to synchronize the access to the state machine
		/// </summary>
		private object oLock;

		/// <summary>
		/// Indicates if the state machine is executing a state
		/// </summary>
		private bool executingState;

		/// <summary>
		/// The index of the final state
		/// </summary>
		private int finalStateIndex;

		/// <summary>
		/// Indicates if the state machine is being executed
		/// </summary>
		private bool running;

		/// <summary>
		/// Proivides Synchrnonized access to the running variable
		/// </summary>
		private ReaderWriterLock rwRunning;

		/// <summary>
		/// Event used to synchronize the asynchronous execution of a state machine
		/// </summary>
		private ManualResetEvent executionFinishedEvent;

		/// <summary>
		/// Thread used to execute the state machine asynchronously
		/// </summary>
		private Thread executionThread;

		/// <summary>
		/// Flag used to abort the execution of a thread.
		/// </summary>
		private bool abortAsyncExecution;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of FunctionBasedStateMachine
		/// </summary>
		public SimpleStateMachine()
		{
			this.states = new List<T>(100);
			this.currentStateIx = 0;
			this.oLock = new Object();
			this.executingState = false;
			this.finalStateIndex = -1;
			this.rwRunning = new ReaderWriterLock();
			this.executionFinishedEvent = new ManualResetEvent(true);
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of states of the state machine
		/// </summary>
		public override int StateCount
		{
			get { return states.Count; }
		}

		/// <summary>
		/// Gets the zero-based index of the current state of the state machine
		/// </summary>
		public override int CurrentStateIndex
		{
			get { return this.currentStateIx; }
			set
			{
				lock (oLock)
				{
					if ((value < 0) || (value >= this.states.Count))
						this.currentStateIx = -1;
					this.currentStateIx = value;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating if the current state is an accept state
		/// </summary>
		public override bool CurrentStateIsAccept
		{
			get { return this.CurrentState.IsAcceptState; }
		}

		/// <summary>
		/// Gets a value indicating if the current state is the final state
		/// </summary>
		public override bool CurrentStateIsFinal
		{
			get { return this.CurrentStateIndex == finalStateIndex; }
		}

		/// <summary>
		/// Gets the object that encapsulate the current state of the state machine
		/// </summary>
		public T CurrentState{
			get{return (CurrentStateIndex == -1) ? default(T) : states[CurrentStateIndex];}
		}

		/// <summary>
		/// Gets a value indicating if the state machine reached its final state
		/// </summary>
		public override bool Finished
		{
			get
			{
				return (CurrentStateIndex == -1) || (this.CurrentStateIndex == this.finalStateIndex);
			}
		}

		/// <summary>
		/// Gets the index of the final state
		/// </summary>
		public override int FinalStateIndex
		{
			get { return this.finalStateIndex; }
		}

		/// <summary>
		/// Gets a value indicating if the state machine is currently executing a state
		/// </summary>
		public override bool ExecutingState
		{
			get { return this.executingState; }
		}

		/// <summary>
		/// Gets a vbalue indicating if the state machine is being executed
		/// </summary>
		public bool IsRunning {
			get
			{
				rwRunning.AcquireReaderLock(-1);
				bool value = this.running;
				rwRunning.ReleaseReaderLock();
				return value;
			}
			protected set
			{
				rwRunning.AcquireWriterLock(-1);
				if (running && value)
				{
					rwRunning.ReleaseWriterLock();
					throw new Exception("State machine is already running");
				}
				running = value;
				rwRunning.ReleaseWriterLock();
			}
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Aborts the async execution of the state machine
		/// </summary>
		public void AbortAsyncExecution()
		{
			lock (oLock)
			{
				if (this.executionThread == null)
					throw new Exception("The state machine is not running asynchronously");
				abortAsyncExecution = true;
			}
			executionFinishedEvent.WaitOne();
		}

		/// <summary>
		/// Adds a state to the state machine
		/// </summary>
		/// <param name="state">State to be added to the step machine</param>
		public void AddState(T state)
		{
			lock (oLock)
			{
				if (state == null)
					throw new ArgumentNullException();
				if (state.StateNumber < 0)
					throw new ArgumentException("A state can not have a negative index");
				if (state.StateNumber >= this.states.Count)
				{
					if (state.StateNumber >= this.states.Capacity)
						this.states.Capacity = state.StateNumber + 10;
					for (int i = this.states.Count; i <= state.StateNumber; ++i)
						this.states.Add(default(T));
				}
				this.states[state.StateNumber] = state;
			}
		}

		/// <summary>
		/// Begins to executes the state machine untill it reaches a final state in an asynchronous thread
		/// </summary>
		public void BeginRunUntillFinished()
		{
			lock (oLock)
			{
				this.IsRunning = true;
				this.abortAsyncExecution = false;
				this.executionFinishedEvent.Reset();
				this.executionThread = new Thread(new ThreadStart(ExecutionThreadTask));
				this.executionThread.IsBackground = true;
				this.executionThread.Start();
			}
		}

		/// <summary>
		/// Executes the state machine in an asynchronous thread
		/// </summary>
		private void ExecutionThreadTask()
		{
			try
			{
				// Advance untill the next state (unexecuted) is the final state
				while (!abortAsyncExecution &&!this.Finished && (this.CurrentStateIndex != -1))
					RunNextStep();
				// Now execute the final state
				if (!abortAsyncExecution && (this.CurrentStateIndex != -1))
					RunNextStep();
			}
			catch{}
			finally
			{
				this.IsRunning = false;
				this.executionFinishedEvent.Set();
				this.executionThread = null;
			}
		}

		/// <summary>
		/// Waits until the execution of
		/// the the state machine in an asynchronous thread is finished
		/// </summary>
		public void EndRunUntilFinished()
		{
			executionFinishedEvent.WaitOne();
		}

		/// <summary>
		/// Removes a state from the state machine
		/// </summary>
		/// <param name="state">State to be removed from the step machine</param>
		public void RemoveState(T state)
		{
			lock (oLock)
			{
				int index;
				index = this.states.IndexOf(state);
				if(index != -1)
					this.states[index] = default(T);
			}
		}

		/// <summary>
		/// Removes a state from the state machine
		/// </summary>
		/// <param name="stateIndex">Index of the state to be removed the step machine</param>
		public void RemoveState(int stateIndex)
		{
			lock (oLock)
			{
				if (stateIndex < this.states.Count)
					this.states[stateIndex] = default(T);
			}
		}

		/// <summary>
		/// Executes the state machine to it's next step
		/// </summary>
		public override void RunNextStep()
		{
			lock (oLock)
			{
				executingState = true;
				try
				{
					if ((states[CurrentStateIndex] == null)||(CurrentStateIndex == -1))
					{
						CurrentStateIndex = -1;
						executingState = false;
						return;
					}
					CurrentStateIndex = states[CurrentStateIndex].Execute();
				}
				catch { CurrentStateIndex = -1; }
				executingState = false;
			}
		}

		/// <summary>
		/// Executes the state machine untill it reaches a final state
		/// </summary>
		public override void RunUntillFinished()
		{
			this.IsRunning = true;
			base.RunUntillFinished();
			this.IsRunning = false;
		}

		/// <summary>
		/// Executes the state machine untill it reaches a final state or a maximum number of steps
		/// are executed
		/// </summary>
		/// <param name="maxSteps">The maximum number of steps allowed for the state machine</param>
		public override void RunUntillFinished(int maxSteps)
		{
			this.IsRunning = true;
			base.RunUntillFinished();
			this.IsRunning = false;
		}

		/// <summary>
		/// Executes the state machine untill it reaches a final state or a maximum number of steps
		/// are executed
		/// </summary>
		/// <param name="timeout">The maximum amount of time that the state machine is allowed to run</param>
		public void RunUntillFinished(TimeSpan timeout)
		{
			BeginRunUntillFinished();
			if (!executionFinishedEvent.WaitOne(timeout))
				AbortAsyncExecution();
			EndRunUntilFinished();
		}

		/// <summary>
		/// Resets the state machine
		/// </summary>
		public override void Reset()
		{
			lock (oLock)
			{
				this.CurrentStateIndex = 0;
			}
		}

		/// <summary>
		/// Sets the final state of the state machine. If the final state does not belong to the state machine it is added.
		/// </summary>
		/// <param name="state">The final state of the state machine.</param>
		public override void SetFinalState(IState state)
		{
			this.SetFinalState((T)state);
		}

		/// <summary>
		/// Sets the final state of the state machine. If the final state does not belong to the state machine it is added.
		/// </summary>
		/// <param name="state">The final state of the state machine.</param>
		public void SetFinalState(T state)
		{
			lock (oLock)
			{
				if (!this.states.Contains(state))
					this.AddState(state);
				SetFinalState(this.states.IndexOf(state));
			}
		}

		/// <summary>
		/// Sets the final state of the state machine.
		/// </summary>
		/// <param name="stateIndex">The index of the final state of the state machine.</param>
		public override void SetFinalState(int stateIndex)
		{
			if ((stateIndex < 0) || (stateIndex > this.states.Count))
				throw new ArgumentOutOfRangeException();
			finalStateIndex = stateIndex;
		}

		#endregion
	}
}
