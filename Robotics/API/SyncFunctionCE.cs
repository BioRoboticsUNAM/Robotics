using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API
{
	/// <summary>
	/// Synchronously executes a function or method as a command executer
	/// </summary>
	public class SyncFunctionCE : SyncCommandExecuter
	{
		
		#region Variables

		/// <summary>
		/// Represents the method/function that will perform the command execution
		/// </summary>
		CommandExecuterMethod executerMethod;

		/// <summary>
		/// Indicates if the executer requires parameters
		/// </summary>
		private bool parametersRequired;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of AsyncFunctionCE for the asynchronous execution of a command
		/// </summary>
		/// <param name="commandName">The name of the command that the AsyncFunctionCE will execute</param>
		/// <param name="executerMethod">The method/function that will perform the command execution</param>
		/// <param name="parametersRequired">Indicates if the executer requires parameters</param>
		public SyncFunctionCE(string commandName, CommandExecuterMethod executerMethod, bool parametersRequired)
			: base(commandName)
		{
			if (executerMethod == null)
				throw new ArgumentNullException();
			this.executerMethod = executerMethod;
			this.parametersRequired = parametersRequired;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the executer requires parameters
		/// </summary>
		public override bool ParametersRequired
		{
			get { return this.parametersRequired; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Calls the executer method
		/// </summary>
		/// <param name="command">Command object which contains the command to be executed</param>
		/// <returns>The Response object result of provided command execution. If no response is required, must return null</returns>
		/// <remarks>If the command execution is aborted the execution of this method is
		/// canceled and a failure response is sent if required</remarks>
		protected override Response SyncTask(Command command)
		{
			return executerMethod(command);
		}

		/// <summary>
		/// When overriden, receives the parameters of an analyzed command by the default Signature object as an array of strings
		/// </summary>
		/// <param name="parameters">Array of strings containing the parameters of the command</param>
		public override void DefaultParameterParser(string[] parameters)
		{
			return;
		}

		#endregion
	}
}
