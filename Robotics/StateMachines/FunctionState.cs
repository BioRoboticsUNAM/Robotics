using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.StateMachines
{
	/// <summary>
	/// Encapsulates information to use a class method as a state of a state machine
	/// </summary>
	public class FunctionState : IState
	{
		#region Variables

		/// <summary>
		/// Indicates if the state is an accept state of the state machine
		/// </summary>
		protected readonly bool isAcceptState;

		/// <summary>
		/// SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine
		/// </summary>
		protected readonly SMStateFuncion stateFunction;

		/// <summary>
		/// The parameters to be pased to the state function
		/// </summary>
		protected object stateFunctionParameters;

		/// <summary>
		/// The zero-based index of the state
		/// </summary>
		protected readonly int stateNumber;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of FunctionState
		/// </summary>
		/// <param name="stateNumber">The zero-based index of the state</param>
		/// <param name="stateFunction">SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine</param>
		public FunctionState(int stateNumber, SMStateFuncion stateFunction)
			:this(stateNumber, stateFunction, false, null){}

			/// <summary>
		/// Initializes a new instance of FunctionState
		/// </summary>
		/// <param name="stateNumber">The zero-based index of the state</param>
		/// <param name="isAccept">Indicates if the state is an accept state of the state machine</param>
		/// <param name="stateFunction">SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine</param>
		public FunctionState(int stateNumber, SMStateFuncion stateFunction, bool isAccept)
			: this(stateNumber, stateFunction, isAccept, null) { }

			/// <summary>
		/// Initializes a new instance of FunctionState
		/// </summary>
		/// <param name="stateNumber">The zero-based index of the state</param>
		/// <param name="stateFunction">SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine</param>
		/// <param name="stateFunctionParameters">The parameters to be pased to the state function</param>
		public FunctionState(int stateNumber, SMStateFuncion stateFunction, object stateFunctionParameters)
			: this(stateNumber, stateFunction, false, stateFunctionParameters) { }

		/// <summary>
		/// Initializes a new instance of FunctionState
		/// </summary>
		/// <param name="stateNumber">The zero-based index of the state</param>
		/// <param name="isAccept">Indicates if the state is an accept state of the state machine</param>
		/// <param name="stateFunction">SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine</param>
		/// <param name="stateFunctionParameters">The parameters to be pased to the state function</param>
		public FunctionState(int stateNumber, SMStateFuncion stateFunction, bool isAccept, object stateFunctionParameters)
		{
			this.isAcceptState = isAccept;
			if (stateNumber < 0)
				throw new ArgumentOutOfRangeException("Parameter stateNumber must be greater or equal than zero");
			this.stateNumber = stateNumber;
			if (stateFunction == null)
				throw new ArgumentNullException();
			this.stateFunction = stateFunction;
			this.stateFunctionParameters = stateFunctionParameters;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the state is an accept state of the state machine
		/// </summary>
		public bool IsAcceptState { get { return this.isAcceptState; } }

		/// <summary>
		/// Gets the SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine
		/// </summary>
		public SMStateFuncion StateFunction
		{
			get { return this.stateFunction; }
		}

		/// <summary>
		/// Gets or sets the parameters to be pased to the state function
		/// </summary>
		public object StateFunctionParameters
		{
			get { return this.stateFunctionParameters; }
			set { this.stateFunctionParameters = value; }
		}

		/// <summary>
		/// Gets the zero-based index of the state
		/// </summary>
		/// <returns>The zero-based index of the state</returns>
		public int StateNumber
		{
			get { return this.stateNumber; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Executes the current state of the state machine
		/// </summary>
		/// <returns>The zero-based index of the next state to be executed</returns>
		public int Execute()
		{
			try
			{
				return stateFunction(this.stateNumber, stateFunctionParameters);
			}
			catch { return -1; }
		}

		#endregion
	}
}
