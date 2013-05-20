using System;
using System.Runtime.Serialization;
using Robotics;

namespace Robotics.API
{
	#region Delegates

	/// <summary>
	/// Represents a methot that receives a command, executes it and generates a response
	/// </summary>
	/// <param name="command">Command object which contains the command to be executed</param>
	/// <returns>The Response object result of provided command execution. If no response is required, must return null</returns>
	public delegate Response CommandExecuterMethod(Command command);

	/// <summary>
	/// Represents the method that will handle the CommandExecuted event of a CommandManager object
	/// </summary>
	/// <param name="sender">The CommandManager object which rises the event</param>
	/// <param name="commandExecuter">The command executer used to execute the received command</param>
	/// <param name="executedCommand">The command which was executed</param>
	/// <param name="generatedResponse">The response generated due to command execution</param>
	public delegate void CommandExecutedEventHandler(CommandManager sender, CommandExecuter commandExecuter, Command executedCommand, Response generatedResponse);

	/// <summary>
	/// Represents the method that will handle the CommandSent event of a ConnectionManager object
	/// </summary>
	/// <param name="sender">The connection manager object which sends the command</param>
	/// <param name="command">The sent Command object</param>
	public delegate void CnnManCommandSentEventHandler(ConnectionManager sender, Command command);

	/// <summary>
	/// Represents the method that will handle the ResponseSent event of a ConnectionManager object
	/// </summary>
	/// <param name="sender">The connection manager object which sends the response</param>
	/// <param name="response">The sent Response object</param>
	public delegate void CnnManResponseSentEventHandler(ConnectionManager sender, Response response);

	/// <summary>
	/// Represents the method that will handle the CommandReceived event of a CommandManager object
	/// </summary>
	/// <param name="command">The Command object received</param>
	public delegate void CommandReceivedEventHandler(Command command);

	/// <summary>
	/// Represents the method that will handle the ResponseReceived event of a CommandManager object
	/// </summary>
	/// <param name="response">The Response object received</param>
	public delegate void ResponseReceivedEventHandler(Response response);

	/// <summary>
	/// Represents the method that will handle the ConsoleUpdated event
	/// </summary>
	/// <param name="sender">The object that raises the event</param>
	/// <param name="text">The text sent to the console</param>
	public delegate void ConsoleUpdatedEventHandler(object sender, string text);

	/// <summary>
	/// Represents the method that will handle the StatusChanged, Started and Stopped events of a CommandManager
	/// </summary>
	/// <param name="commandManager">The CommandManager object which rises the event</param>
	public delegate void CommandManagerStatusChangedEventHandler(CommandManager commandManager);

	/// <summary>
	/// Represents the method that will handle the DataReceived event of a SocketTcpServer object
	/// </summary>
	/// <param name="connectionManager">The ConnectionManager object which rises the event</param>
	/// <param name="packet">The Tcp Packet with the data received</param>
	public delegate void ConnectionManagerDataReceivedEH(ConnectionManager connectionManager, System.Net.Sockets.TcpPacket packet);

	/// <summary>
	/// Represents the method that will handle the StatusChanged, Started and Stopped events of a ConnectionManager
	/// </summary>
	/// <param name="connectionManager">The ConnectionManager object which rises the event</param>
	public delegate void ConnectionManagerStatusChangedEventHandler(ConnectionManager connectionManager);

	/// <summary>
	/// Represents the method that will handle the ExecutionStarted and ExecutionAborted of a CommandExecuter
	/// </summary>
	/// <param name="commandExecuter">The CommandExecuter object which rises the event</param>
	/// <param name="command">The Command object which execution status has changed</param>
	public delegate void ExecutionStatusEventHandler(CommandExecuter commandExecuter, Command command);

	/// <summary>
	/// Represents the method that will handle the ExecutionFinished of a CommandExecuter
	/// </summary>
	/// <param name="commandExecuter">The CommandExecuter object which rises the event</param>
	/// <param name="command">The Command object which execution status has changed</param>
	/// <param name="response">The Response object result of the command execution</param>
	public delegate void ExecutionFinishedEventHandler(CommandExecuter commandExecuter, Command command, Response response);

	/// <summary>
	/// Represents the method that will handle the ValueChanged and WriteNotification event of a SharedVariable (of T) object
	/// </summary>
	/// <param name="report">The SharedVariable object which raises the event</param>
	public delegate void SharedVariableSubscriptionReportEventHadler<T>(SharedVariableSubscriptionReport<T> report);

	/// <summary>
	/// Represents the method that will handle the Updated and of a SharedVariable object
	/// </summary>
	/// <param name="sharedVariable">The SharedVariable object which raises the event</param>
	public delegate void SharedVariableUpdatedEventHadler(SharedVariable sharedVariable);

	/// <summary>
	/// Represents the method that will handle the ReportReceived and of a SharedVariable object
	/// </summary>
	/// <param name="sharedVariable">The SharedVariable object which raises the event</param>
	/// <param name="report">The report data received from Blackboard</param>
	public delegate void SharedVariableReportReceivedEventHadler(SharedVariable sharedVariable, SharedVariableReport report);

	#endregion

	#region Enumerations

	/// <summary>
	/// Specifies the type of subscription to a SharedVariable object
	/// </summary>
	public enum SharedVariableSubscriptionType
	{
		/// <summary>
		/// The subscription status to the shared variable is unknown
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// There is no subscription to the shared variable
		/// </summary>
		None = 0,
		/// <summary>
		/// Subscribes to the creation of a variable by any module
		/// </summary>
		/// <remarks>After variable creation the subscription is deleted</remarks>
		Creation,
		/// <summary>
		/// Subscribes to the creation of a variable by any module
		/// </summary>
		WriteAny,
		/// <summary>
		/// Subscribes to the writing of a variable by any module different from the subscriber one
		/// </summary>
		/// <remarks>This is the default value</remarks>
		WriteOthers,
		/// <summary>
		/// Subscribes to the writing of a variable by th specified module
		/// </summary>
		WriteModule
	}

	/// <summary>
	/// Specifies how a subscription report of a shared variable change is made
	/// </summary>
	public enum SharedVariableReportType
	{
		/// <summary>
		/// The subscription status to the shared variable is unknown
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// There is no subscription to the shared variable
		/// </summary>
		None = 0,
		/// <summary>
		/// Sends a report that just notifies the change of the content of a shared variable
		/// </summary>
		/// <remarks>This is the default value</remarks>
		Notify,
		/// <summary>
		/// Sends a report that notifies the change of the content of a shared variable sending it's content
		/// </summary>
		SendContent
	}

	#endregion
}
