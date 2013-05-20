using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.StateMachines
{
	/// <summary>
	/// Encapsulates information to use a class method as a state of a state machine
	/// </summary>
	public class EnumeratedFunctionState<T> : IState where T : struct, IComparable, IConvertible, IFormattable
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
		protected readonly SMEnumeratedStateFuncion<T> stateFunction;

		/// <summary>
		/// The parameters to be pased to the state function
		/// </summary>
		protected object stateFunctionParameters;

		/// <summary>
		/// The zero-based index of the state
		/// </summary>
		protected readonly T stateName;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of FunctionState
		/// </summary>
		/// <param name="stateName">The zero-based index of the state</param>
		/// <param name="stateFunction">SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine</param>
		public EnumeratedFunctionState(T stateName, SMEnumeratedStateFuncion<T> stateFunction)
			: this(stateName, stateFunction, false, null) { }

		/// <summary>
		/// Initializes a new instance of FunctionState
		/// </summary>
		/// <param name="stateName">The zero-based index of the state</param>
		/// <param name="isAccept">Indicates if the state is an accept state of the state machine</param>
		/// <param name="stateFunction">SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine</param>
		public EnumeratedFunctionState(T stateName, SMEnumeratedStateFuncion<T> stateFunction, bool isAccept)
			: this(stateName, stateFunction, isAccept, null) { }

		/// <summary>
		/// Initializes a new instance of FunctionState
		/// </summary>
		/// <param name="stateName">The zero-based index of the state</param>
		/// <param name="stateFunction">SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine</param>
		/// <param name="stateFunctionParameters">The parameters to be pased to the state function</param>
		public EnumeratedFunctionState(T stateName, SMEnumeratedStateFuncion<T> stateFunction, object stateFunctionParameters)
			: this(stateName, stateFunction, false, stateFunctionParameters) { }

		/// <summary>
		/// Initializes a new instance of FunctionState
		/// </summary>
		/// <param name="stateName">The zero-based index of the state</param>
		/// <param name="isAccept">Indicates if the state is an accept state of the state machine</param>
		/// <param name="stateFunction">SMStateFuncion delegate which points to the method
		/// to be executed as state function of the state machine</param>
		/// <param name="stateFunctionParameters">The parameters to be pased to the state function</param>
		public EnumeratedFunctionState(T stateName, SMEnumeratedStateFuncion<T> stateFunction, bool isAccept, object stateFunctionParameters)
		{
			if (!typeof(T).IsEnum)
				throw new Exception("This class must be implemented only with enumerations");
			this.isAcceptState = isAccept;
			if (this.StateNumber < 0)
				throw new ArgumentOutOfRangeException("Parameter stateNumber must be greater or equal than zero");
			this.stateName = stateName;
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
		public SMEnumeratedStateFuncion<T> StateFunction
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
		/// Gets the name of the state
		/// </summary>
		/// <returns>The name the state</returns>
		public T StateName
		{
			get { return this.stateName; }
		}

		/// <summary>
		/// Gets the zero-based index of the state
		/// </summary>
		/// <returns>The zero-based index of the state</returns>
		public int StateNumber
		{
			get { return (int)(object)this.stateName; }
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
				return (int)(object)stateFunction(this.stateName, stateFunctionParameters);
			}
			catch { return -1; }
		}

		#endregion
	}
}
