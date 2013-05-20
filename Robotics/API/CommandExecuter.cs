using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API
{
	/// <summary>
	/// Serves as base class for classes which allow execute a Command
	/// </summary>
	public abstract class CommandExecuter : ICommandExecuter
	{
		#region Variables

		/// <summary>
		/// Stores the CommandManager object to which this CommandExecuter is bound to
		/// </summary>
		private CommandManager cmdMan;

		/// <summary>
		/// Stores the name of the command that the CommandExecuter will execute
		/// </summary>
		private string commandName;

		/// <summary>
		/// Stores a value that indicates if the executer must generate and send a response
		/// </summary>
		private bool responseRequired;

		/// <summary>
		/// Stores the signature asociated to this command
		/// </summary>
		private Signature signature;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of CommandExecuter
		/// </summary>
		/// <param name="signature">The Signature object for the command that the CommandExecuter will execute</param>
		public CommandExecuter(Signature signature) : this(signature, null) { }

		/// <summary>
		/// Initializes a new instance of CommandExecuter
		/// </summary>
		/// <param name="signature">The Signature object for the command that the CommandExecuter will execute</param>
		/// <param name="commandManager">The CommandManager object that will handle the command executed by this CommandExecuter instance</param>
		public CommandExecuter(Signature signature, CommandManager commandManager)
		{
			this.commandName = signature.CommandName;
			this.responseRequired = true;
			this.signature = signature;
		}

		/// <summary>
		/// Initializes a new instance of CommandExecuter
		/// </summary>
		/// <param name="commandName">The name of the command that the CommandExecuter will execute</param>
		public CommandExecuter(string commandName) : this(commandName, null) { }

		/// <summary>
		/// Initializes a new instance of CommandExecuter
		/// </summary>
		/// <param name="commandName">The name of the command that the CommandExecuter will execute</param>
		/// <param name="commandManager">The CommandManager object that will handle the command executed by this CommandExecuter instance</param>
		public CommandExecuter(string commandName, CommandManager commandManager)
		{
			this.commandName = commandName;
			this.responseRequired = true;
			SignatureBuilder sb = new SignatureBuilder();
			sb.AddNewFromDelegate(new StringArrayEventHandler(this.DefaultParameterParser));
			this.signature = sb.GenerateSignature(this.commandName);
			//if (commandManager != null)
			//	commandManager.CommandExecuters.Add(this);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when text has been writted to the console
		/// </summary>
		public event ConsoleUpdatedEventHandler ConsoleUpdated;

		/// <summary>
		/// Occurs when the execution of the command has started by the CommandManager
		/// but before the method that manages the command execution be executed
		/// </summary>
		public event ExecutionStatusEventHandler ExecutionStarted;

		/// <summary>
		/// Occurs when the execution of the command has been aborted and a failure response has been sent
		/// </summary>
		public event ExecutionStatusEventHandler ExecutionAborted;

		/// <summary>
		/// Occurs when the execution of the command has finished its execution and the response has been sent
		/// </summary>
		public event ExecutionFinishedEventHandler ExecutionFinished;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value that indicates if the CommandExecuter is busy
		/// </summary>
		public abstract bool Busy { get; }

		/// <summary>
		/// Gets the CommandManager object to which this CommandExecuter is bound to
		/// </summary>
		public CommandManager CommandManager
		{
			get { return cmdMan; }
			protected internal set { cmdMan = value; }
		}

		/// <summary>
		/// Gets the name of the command that the CommandExecuter will execute
		/// </summary>
		public string CommandName {
			get { return commandName; }
		}

		/// <summary>
		/// Gets a value that indicates if the CommandExecuter is running
		/// </summary>
		public abstract bool IsRunning { get; }

		/// <summary>
		/// Gets a value indicating if the executer requires parameters
		/// </summary>
		public abstract bool ParametersRequired
		{
			get;
		}
		
		/// <summary>
		/// Stores a value that indicates if the executer must generate and send a response
		/// </summary>
		public bool ResponseRequired
		{
			get { return responseRequired; }
			set { responseRequired = value; }
		}

		/// <summary>
		/// Gets or sets the signature used to parse the command arguments
		/// </summary>
		protected Signature Signature
		{
			get { return signature; }
			set
			{
				if (this.signature == null)
					throw new ArgumentNullException();
				if (signature.CommandName != this.commandName)
					throw new ArgumentException("Signature.CommandName does not match CommandExecuter.CommandName for this object", "value");
				signature = value;
			}
		}

		#endregion

		#region Methodos

		#region Abstract Methodos

		/// <summary>
		/// Aborts the command execution
		/// If a response is required, a failure response will be generated and sent automatically
		/// </summary>
		/// <returns>true if command was aborted successfully, false otherwise</returns>
		/// <remarks>Not all implementations derived of CommandExecuter may be abortables</remarks>
		public abstract bool Abort();

		/// <summary>
		/// When overriden, receives the parameters of an analyzed command by the default Signature object as an array of strings
		/// </summary>
		/// <param name="parameters">Array of strings containing the parameters of the command</param>
		public abstract void DefaultParameterParser(string[] parameters);

		/// <summary>
		/// Executes the provided Command
		/// </summary>
		/// <param name="command">Command to execute</param>
		public abstract void Execute(Command command);

		#endregion

		/// <summary>
		/// Output console
		/// </summary>
		/// <param name="text">The text to be written</param>
		protected virtual void Console(string text)
		{
		}

		/// <summary>
		/// Raises the OnConsoleUpdated Event
		/// </summary>
		/// <param name="text">The text sent to the Console</param>
		/// <remarks>The OnConsoleUpdated method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnConsoleUpdated in a derived class, be sure to call the base class's OnConsoleUpdated method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnConsoleUpdated(string text)
		{
			if ((text == null) || (text.Length < 0)) return;
			if (ConsoleUpdated != null)
				ConsoleUpdated(this, text);
		}

		/// <summary>
		/// Raises the ExecutionAborted Event and sends a failure response
		/// </summary>
		/// <param name="command">The command which execution was aborted</param>
		/// <remarks>The OnExecutionAborted method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnExecutionAborted in a derived class, be sure to call the base class's OnExecutionAborted method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnExecutionAborted(Command command)
		{
			if (command == null) return;
			SendResponse(false, command);
			if (ExecutionAborted != null) ExecutionAborted(this, command);
		}

		/// <summary>
		/// Raises the ExecutionFinished Event
		/// </summary>
		/// <param name="command">The command which execution was finished</param>
		/// <param name="response">The response generated by the execution of the command (if any)</param>
		/// <remarks>The OnExecutionFinished method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnExecutionFinished in a derived class, be sure to call the base class's OnExecutionFinished method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnExecutionFinished(Command command, Response response)
		{
			if (command == null) return;
			if (ExecutionFinished != null)
				ExecutionFinished(this, command, response);
			if (this.cmdMan != null)
				this.cmdMan.OnCommandExecuted(this, command, response);
		}

		/// <summary>
		/// Raises the ExecutionStarted event
		/// </summary>
		/// <param name="command">The command which execution has been started</param>
		/// <remarks>The OnExecutionStarted method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnExecutionStarted in a derived class, be sure to call the base class's OnExecutionStarted method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnExecutionStarted(Command command)
		{
			if (command == null) return;
			if (ExecutionStarted != null)
				ExecutionStarted(this, command);
		}

		/// <summary>
		/// Uses the asociated Signature object to parse the provided Command object parameters and execute the corresponding method
		/// </summary>
		/// <param name="command">The Command object which parameters will be parsed</param>
		/// <returns>true if parameters was parsed successfully and the corresponding method executed, false otherwise</returns>
		protected virtual bool ParseParameters(Command command)
		{
			return Signature.CallIfMatch(command);
		}

		#region Outgoing Data Management

		/// <summary>
		/// Sends a command through the asociated ConnectionManager of the CommandExecuter object that this instance of CommandExecuter is bound to
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if command was sent and its response received. false otherwise</returns>
		protected bool SendCommand(Command command)
		{
			if (cmdMan == null) return false;
			return cmdMan.SendCommand(command);
		}

		/// <summary>
		/// Creates a response and sends it through the asociated ConnectionManager of the CommandExecuter object that this instance of CommandExecuter is bound to
		/// </summary>
		/// <param name="success">Indicates if command succeded</param>
		/// <param name="command">Command to respond</param>
		protected internal void SendResponse(bool success, Command command)
		{
			if (cmdMan != null)
				cmdMan.SendResponse(success, command);
		}

		/// <summary>
		/// Creates a response and sends it through the asociated ConnectionManager of the CommandExecuter object that this instance of CommandExecuter is bound to
		/// </summary>
		/// <param name="success">Indicates if command succeded</param>
		/// <param name="command">Command to respond</param>
		/// <param name="response">Generated response</param>
		protected internal void SendResponse(bool success, Command command, out Response response)
		{
			response = null;
			if (cmdMan != null)
				cmdMan.SendResponse(success, command, out response);
		}

		/// <summary>
		/// Sends a Response through the asociated ConnectionManager of the CommandExecuter object that this instance of CommandExecuter is bound to
		/// </summary>
		/// <param name="response">Response object to be sent</param>
		protected internal void SendResponse(Response response)
		{
			if (cmdMan != null)
				cmdMan.SendResponse(response);
		}

		#endregion

		#endregion

	}
}
