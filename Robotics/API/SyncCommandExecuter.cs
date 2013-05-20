using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API
{
	/// <summary>
	/// Serves as base class for classes which allow execute a Command synchronously
	/// </summary>
	/// <remarks>A SyncCommandExecuter blocks the CommandManager trhead until the execution of the command
	/// has been finished; therefore no other commands will be parsed or executed. If the execution of the command
	/// requires to wait for an event to occur (like incoming data) or takes more than a few lines of code
	/// or will take more than a few microseconds to execute, use instead an AsyncCommandExecuter.</remarks>
	public abstract class SyncCommandExecuter : CommandExecuter
	{
		#region Variables

		/// <summary>
		/// Indicates if the executer is busy
		/// </summary>
		private bool busy;

		/// <summary>
		/// Flag that indicates if the asyncExecutionThread is running
		/// </summary>
		private bool running;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SyncCommandExecuter
		/// </summary>
		/// <param name="signature">The Signature object for the command that the CommandExecuter will execute</param>
		public SyncCommandExecuter(Signature signature) : this(signature, null) { }

		/// <summary>
		/// Initializes a new instance of SyncCommandExecuter
		/// </summary>
		/// <param name="signature">The Signature object for the command that the CommandExecuter will execute</param>
		/// <param name="commandManager">The CommandManager object that will handle the command executed by this CommandExecuter instance</param>
		public SyncCommandExecuter(Signature signature, CommandManager commandManager)
			: base(signature, commandManager)
		{
			running = false;
			busy = false;
		}

		/// <summary>
		/// Initializes a new instance of SyncCommandExecuter for the synchronous execution of a command
		/// </summary>
		/// <param name="commandName">The name of the command that the SyncCommandExecuter will execute</param>
		public SyncCommandExecuter(string commandName) : this(commandName, null) { }

		/// <summary>
		/// Initializes a new instance of SyncCommandExecuter for the aynchronous execution of a command
		/// </summary>
		/// <param name="commandName">The name of the command that the SyncCommandExecuter will execute</param>
		/// <param name="commandManager">The CommandManager object that will handle the command executed by this CommandExecuter instance</param>
		public SyncCommandExecuter(string commandName, CommandManager commandManager)
			:base(commandName, commandManager)
		{
			running = false;
			busy = false;
		}

		#endregion

		#region Events



		#endregion

		#region Properties

		/// <summary>
		/// Gets a value that indicates if the SyncCommandExecuter is busy
		/// </summary>
		public override bool Busy
		{
			get { return busy; }
		}

		/// <summary>
		/// Gets a value that indicates if the CommandExecuter is running
		/// </summary>
		public override bool IsRunning
		{
			get { return running; }
		}

		/// <summary>
		/// Gets a value indicating if the executer requires parameters
		/// </summary>
		public override bool ParametersRequired
		{
			get { return false; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Aborts the command execution
		/// </summary>
		/// <returns>false, since SyncCommandExecuter objects usually cannot be aborted</returns>
		public override bool Abort()
		{
			//running = false;
			return false;
		}

		/// <summary>
		/// When overriden executes the provided command
		/// </summary>
		/// <param name="command">Command object which contains the command to be executed</param>
		/// <returns>The Response object result of provided command execution. If no response is required, must return null</returns>
		/// <remarks>If the command execution is aborted the execution of this method is
		/// canceled and a failure response is sent if required</remarks>
		protected abstract Response SyncTask(Command command);

		

		#endregion

		#region Inherited Methodos

		/// <summary>
		/// Executes the provided command
		/// </summary>
		/// <param name="command">Command object which contains the command to be executed</param>
		/// <remarks>If the command execution is aborted a failure response is sent if required</remarks>
		public sealed override void Execute(Command command)
		{
			// 1. Check busy state and parameters
			#region Busy check
			if (Busy)
			{
				if (ResponseRequired)
					SendResponse(false, command);
				return;
			}
			#endregion

			#region No params provided
			if (ParametersRequired && !command.HasParams)
			{
				SendResponse(false, command);
				return;
			}
			#endregion

			// 2. Set execution flags
			busy = true;
			running = true;

			// 3. Create default failed response
			Response response = Response.CreateFromCommand(command, false);

			// 4. Raise ExecutionStarted event
			OnExecutionStarted(command);

			// 5. Perform command execution
			response = SyncTask(command);

			// 6. If a null response is provided (and required), generate failure response
			if (ResponseRequired && (response == null))
				response = Response.CreateFromCommand(command, false);

			// 7. Send the result of command execution (if any)
			if (response != null) SendResponse(response);

			// 8. Reset execution flags
			busy = false;
			running = false;

			// 9. Raise ExecutionFinished event
			OnExecutionFinished(command, response);
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
