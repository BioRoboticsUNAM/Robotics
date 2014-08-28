using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace Robotics.API
{
	/// <summary>
	/// Provides the baase methods for basic commands and it's responses
	/// </summary>
	[ComVisible(true)]
	[Guid("ED7AE349-7209-4698-9A64-C6B8D8068B41")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IBaseMessage : IComparable, IComparable<BaseMessage>
	{

		/// <summary>
		/// Gets the parameters string with quotes escaped
		/// </summary>
		string EscapedParameters
		{
			get;
		}

		/// <summary>
		/// Gets the object source of the message, like a ConnectionManager or a Form
		/// </summary>
		IMessageSource MessageSource
		{
			get;
		}

		/// <summary>
		/// Gets or sets the aditional data provided by the source of the message, like an IPEndPoint or a Delegate
		/// </summary>
		object MessageSourceMetadata
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or Sets the source module of the command
		/// </summary>
		string SourceModule
		{
			get;
		}

		/// <summary>
		/// Gets or Sets the destination module of the command
		/// </summary>
		string DestinationModule
		{
			get;

		}

		/// <summary>
		/// Gets or Sets the command name
		/// </summary>
		string CommandName
		{
			get;
		}

		/// <summary>
		/// Gets or Sets the command paramenters
		/// </summary>
		string Parameters
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or Sets the command id
		/// </summary>
		int Id
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating id the Command contains params
		/// </summary>
		bool HasParams
		{
			get;
		}

		/// <summary>
		/// Gets a string which can be sent to a module
		/// </summary>
		string StringToSend
		{
			get;
		}
	}

	/// <summary>
	/// Represents a Command to be executed
	/// </summary>
	[ComVisible(true)]
	[Guid("C0FFDACE-05B0-4ec9-A20B-F9D484DD1BE3")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface ICommand : IBaseMessage
	{

		/// <summary>
		/// Gets a value indicating if the Command object represents a system command
		/// </summary>
		bool IsSystemCommand
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating if provided Response is a response for current command
		/// </summary>
		/// <param name="response">Response to check</param>
		/// <returns>true if provided Response is a response for command, false otherwise</returns>
		bool IsMatch(Response response);

	}

	/// <summary>
	/// Serves as base class for classes which allow execute a Command
	/// </summary>
	[ComVisible(true)]
	[Guid("6E0FB807-FA8B-4831-9588-7FE093A6A927")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface ICommandExecuter
	{

		#region Events

		/// <summary>
		/// Occurs when text has been writted to the console
		/// </summary>
		event ConsoleUpdatedEventHandler ConsoleUpdated;

		/// <summary>
		/// Occurs when the execution of the command has started by the CommandManager
		/// but before the method that manages the command execution be executed
		/// </summary>
		event ExecutionStatusEventHandler ExecutionStarted;

		/// <summary>
		/// Occurs when the execution of the command has been aborted and a failure response has been sent
		/// </summary>
		event ExecutionStatusEventHandler ExecutionAborted;

		/// <summary>
		/// Occurs when the execution of the command has finished its execution and the response has been sent
		/// </summary>
		event ExecutionFinishedEventHandler ExecutionFinished;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value that indicates if the CommandExecuter is busy
		/// </summary>
		bool Busy { get; }

		/// <summary>
		/// Gets the CommandManager object to which this CommandExecuter is bound to
		/// </summary>
		CommandManager CommandManager
		{
			get;
		}

		/// <summary>
		/// Gets the name of the command that the CommandExecuter will execute
		/// </summary>
		string CommandName { get; }

		/// <summary>
		/// Gets a value that indicates if the CommandExecuter is running
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Gets a value indicating if the executer requires parameters
		/// </summary>
		bool ParametersRequired
		{
			get;
		}

		/// <summary>
		/// Stores a value that indicates if the executer must generate and send a response
		/// </summary>
		bool ResponseRequired
		{
			get;
			set;
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Aborts the command execution
		/// If a response is required, a failure response will be generated and sent automatically
		/// </summary>
		/// <returns>true if command was aborted successfully, false otherwise</returns>
		/// <remarks>Not all implementations derived of CommandExecuter may be abortables</remarks>
		bool Abort();

		/// <summary>
		/// When overriden, receives the parameters of an analyzed command by the default Signature object as an array of strings
		/// </summary>
		/// <param name="parameters">Array of strings containing the parameters of the command</param>
		void DefaultParameterParser(string[] parameters);

		/// <summary>
		/// Executes the provided Command
		/// </summary>
		/// <param name="command">Command to execute</param>
		void Execute(Command command);

		#endregion

	}

	/// <summary>
	/// Represents the response of a command
	/// </summary>
	[ComVisible(true)]
	[Guid("E48F0045-A7B5-4e17-B814-0F56A133E403")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IResponse : IBaseMessage
	{
		/// <summary>
		/// Gets the result contained in response
		/// </summary>
		bool Success
		{
			get;
		}
	}
}
