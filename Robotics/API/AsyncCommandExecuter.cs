using System;
using System.Collections.Generic;
using System.Threading;

namespace Robotics.API
{

	/// <summary>
	/// Serves as base class for classes which allow execute a Command asynchronously
	/// </summary>
	public abstract class AsyncCommandExecuter : CommandExecuter
	{
		#region Variables

		/// <summary>
		/// Indicates if the executer is busy
		/// </summary>
		private bool busy;

		/// <summary>
		/// Thread for async execution of the command
		/// </summary>
		protected Thread asyncExecutionThread;

		/// <summary>
		/// Flag that indicates if the asyncExecutionThread is running
		/// </summary>
		private bool running;

		/// <summary>
		/// Indicates if the executer requires parameters
		/// </summary>
		private readonly bool parametersRequired;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of AsyncCommandExecuter
		/// </summary>
		/// <param name="signature">The Signature object for the command that the CommandExecuter will execute</param>
		public AsyncCommandExecuter(Signature signature) : this(signature, null) { }

		/// <summary>
		/// Initializes a new instance of AsyncCommandExecuter
		/// </summary>
		/// <param name="signature">The Signature object for the command that the CommandExecuter will execute</param>
		/// <param name="commandManager">The CommandManager object that will handle the command executed by this CommandExecuter instance</param>
		public AsyncCommandExecuter(Signature signature, CommandManager commandManager)
			: base(signature, commandManager)
		{
			parametersRequired = true;
			asyncExecutionThread = null;
			running = false;
			busy = false;
		}

		/// <summary>
		/// Initializes a new instance of AsyncCommandExecuter for the asynchronous execution of a command
		/// </summary>
		/// <param name="commandName">The name of the command that the AsyncCommandExecuter will execute</param>
		public AsyncCommandExecuter(string commandName) : this(commandName, null) { }

		/// <summary>
		/// Initializes a new instance of AsyncCommandExecuter for the asynchronous execution of a command
		/// </summary>
		/// <param name="commandName">The name of the command that the AsyncCommandExecuter will execute</param>
		/// <param name="commandManager">The CommandManager object that will handle the command executed by this CommandExecuter instance</param>
		public AsyncCommandExecuter(string commandName, CommandManager commandManager)
			:base(commandName, commandManager)
		{
			parametersRequired = true;
			asyncExecutionThread = null;
			running = false;
			busy = false;
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets a value that indicates if the AsyncCommandExecuter is busy
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
			get {return parametersRequired;}
		}

		/// <summary>
		/// Gets the priority of this command. Valid values are between 0 (higest) and 255 (lowest)
		/// This value is used by the blackboard and has no effect on the execution of this CommandExecuter object
		/// </summary>
		public override int Priority { get { return 1; } }

		/// <summary>
		/// Gets the maxium amount of time the blackboard will wait for the execution of this command.
		/// Valid values are positive integers and given in milliseconds, it is advised to use values greater than 100
		/// This value is used by the blackboard and has no effect on the execution of this CommandExecuter object
		/// </summary>
		public override int Timeout { get { return 5000; } }

		#endregion

		#region Methodos

		/// <summary>
		/// Aborts the command execution
		/// If a response is required, a failure response will be generated and sent automatically
		/// </summary>
		/// <returns>true if command was aborted successfully, false otherwise</returns>
		public override bool Abort()
		{
			//int count = 1000;
			running = false;
			asyncExecutionThread.Abort();
			/*
			while (count-- > 0)
			{
				if (!asyncExecutionThread.IsAlive) return true;
				Thread.Sleep(0);
			}
			*/
			asyncExecutionThread.Join(100);
			return true;
		}

		/// <summary>
		/// Asynchronously executes the provided command
		/// </summary>
		/// <param name="oCommand">Object which contains the command to be executed</param>
		/// <remarks>This method is provided to validate the casting and execution of a valid command object by the AsyncTask method</remarks>
		private void AsyncExecutionThreadTask(object oCommand)
		{
			Command command = null;
			Response response = null;
			bool aborted = false;
			try
			{
				// 3. Retrieve and verify command object
				command = oCommand as Command;
				if (oCommand == null)
				{
					// Failed, reset flags
					busy = false;
					running = false;
					return;
				}

				// 4. Set execution flag
				aborted = false;
				running = true;

				// 5. Begin execution.
				// 6 to 11. See AsyncExecutionThreadTask(Command command)
				AsyncExecutionThreadTask(command, out response);

			}
			catch (Exception ex)
			{
				// Reset execution flags to allow thread remain alive while executing Abort events
				busy = false;
				running = false;

				// If execution was aborted before the command parsed, handle it
				if (command == null)
				{
					command = oCommand as Command;
					if (oCommand == null)
						return;
				}

				// If command execution was aborted, raise the event
				if (ex is ThreadAbortException)
				{
					aborted = true;
					OnExecutionAborted(command);
					asyncExecutionThread.Abort();
				}
			}
			finally
			{
				// 12. Raise ExecutionFinished event
				if(!aborted && command != null)
				OnExecutionFinished(command, response);
			}
		}

		/// <summary>
		/// Asynchronously executes the provided command
		/// </summary>
		/// <param name="command">Command object which contains the command to be executed</param>
		/// <param name="response">Response sent as result of command execution</param>
		private void AsyncExecutionThreadTask(Command command, out Response response)
		{
			// The running state is checked in all steps to discard execution on command abort

			//6. Default failed response
			response = Response.CreateFromCommand(command, false);

			// 7. Raise ExecutionStarted event
			if (running)
				OnExecutionStarted(command);

			// 8. Perform command execution
			if (running)
				response = AsyncTask(command);

			// 9. If a null response is provided (and required) generate failure response
			if (ResponseRequired && (response == null))
				response = Response.CreateFromCommand(command, false);

			// 10. Send the result of command execution (if any)
			if (response != null)
				SendResponse(response);

			// 11. Reset execution flags
			busy = false;
			running = false;
		}

		/// <summary>
		/// When overriden executes the provided command
		/// </summary>
		/// <param name="command">Command object which contains the command to be executed</param>
		/// <returns>The Response object result of provided command execution. If no response is required, must return null</returns>
		/// <remarks>If the command execution is aborted the execution of this method is
		/// canceled and a failure response is sent if required</remarks>
		protected abstract Response AsyncTask(Command command);

		/// <summary>
		/// Prepares the async command executer to perform multiple send-and-wait operations
		/// </summary>
		protected void BeginParallelSendAndWait()
		{
			CommandManager.BeginParallelSendAndWait();
		}

		/// <summary>
		/// Executes all pending send-and-wait operations
		/// </summary>
		/// <param name="timeOut">The overall timeout for parallel command execution</param>
		/// <param name="results">Array of Command/Response pairs result of parallel execution</param>
		protected internal bool CommitParallelSendAndWait(int timeOut, out CommandResponsePair[] results)
		{
			return CommandManager.CommitParallelSendAndWait(timeOut, out results);
		}

		/// <summary>
		/// Enqueues a command for a parallel send-and-wait operation
		/// </summary>
		/// <param name="command">Command to enqueue</param>
		protected internal void EnqueueCommand(Command command)
		{
			CommandManager.EnqueueCommand(command);
		}

		/// <summary>
		/// Sends a Command and waits for response
		/// The Command is sent through the asociated AsyncConnectionManager of the CommandExecuter object that this instance of CommandExecuter is bound to
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <param name="timeOut">The timeout for command execution</param>
		/// <param name="response">The response received</param>
		/// <returns>true if command was sent and its response received. false otherwise</returns>
		protected bool SendAndWait(Command command, int timeOut, out Response response)
		{
			response = null;
			if (base.CommandManager == null) return false;
			return base.CommandManager.SendAndWait(command, timeOut, out response);
		}

		/// <summary>
		/// Sends multiple commands and waits for its response
		/// The Command is sent through the asociated AsyncConnectionManager of the CommandExecuter object that this instance of CommandExecuter is bound to
		/// </summary>
		/// <param name="commands">Array of Command objects which contains commands to be sent</param>
		/// <param name="timeOut">The overall timeout for command execution</param>
		/// <param name="results">Array of Response objects generated from responses received</param>
		/// <returns>true if at least one command was sent and its response received. false otherwise</returns>
		protected internal bool SendAndWait(Command[] commands, int timeOut, out CommandResponsePair[] results)
		{
			results = null;
			if (base.CommandManager == null) return false;
			return base.CommandManager.SendAndWait(commands, timeOut, out results);
		}

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
				if(ResponseRequired)
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

			// 2. Set busy flag and configure thread
			busy = true;
			asyncExecutionThread = new Thread(new ParameterizedThreadStart(AsyncExecutionThreadTask));
			asyncExecutionThread.IsBackground = true;
			asyncExecutionThread.Start(command);

			// 3. See AsyncExecutionThreadTask(object oCommand)
		}

		#endregion
	}
}
