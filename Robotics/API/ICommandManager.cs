using System;
using System.Runtime.InteropServices;

namespace Robotics.API
{
	/// <summary>
	/// Represents a command manager
	/// </summary>
	[ComVisible(true)]
	[Guid("316D90CA-C1BC-4cf1-B8A5-B18294E0DE5C")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface ICommandManager : IService, IMessageSource
	{
		#region Events

		/*

		/// <summary>
		/// Occurs when a command is executed
		/// </summary>
		event CommandExecutedEventHandler CommandExecuted;

		/// <summary>
		/// Occurs when a command is received
		/// </summary>
		event CommandReceivedEventHandler CommandReceived;

		/// <summary>
		/// Occurs when a response is received
		/// </summary>
		event ResponseReceivedEventHandler ResponseReceived;

		/// <summary>
		/// Occurs when the status of the CommandManager changes
		/// </summary>
		event CommandManagerStatusChangedEventHandler StatusChanged;

		/// <summary>
		/// Occurs when the CommandManager is started
		/// </summary>
		event CommandManagerStatusChangedEventHandler Started;

		/// <summary>
		/// Occurs when the CommandManager is stopped
		/// </summary>
		event CommandManagerStatusChangedEventHandler Stopped;

		/// <summary>
		/// Occurs when new shared variables has been imported from blackboard
		/// </summary>
		event SharedVariablesLoadedEventHandler SharedVariablesLoaded;

		*/

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Id for commands
		/// </summary>
		int AutoId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value that indicates if the CommandManager is globaly busy.
		/// </summary>
		bool Busy
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ConnectionManager object asociated to this CommandManager object
		/// </summary>
		IConnectionManager ConnectionManager
		{
			get;
			//set;
		}

		/// <summary>
		/// Gets the collection of CommandExecuters contained within the CommandManager
		/// </summary>
		CommandExecuterCollection CommandExecuters
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating if a parallel send-and-wait operation has been started
		/// </summary>
		bool ParallelSendAndWaitStarted
		{
			get;
		}

		/// <summary>
		/// Gets or sets the ready state of the module that controls the command manager
		/// </summary>
		bool Ready
		{
			get;
			set;
		}

		/*

		/// <summary>
		/// Gets the list of shared variables
		/// </summary>
		/// <remarks>shared variables are stored in blackboard</remarks>
		SharedVariableList SharedVariables
		{
			get;
		}

		*/

		#endregion

		#region Methodos

		/*

		/// <summary>
		/// Begins to asynchronously send a command and wait from a response
		/// </summary>
		/// <param name="command">The command to send.</param>
		/// <param name="timeOut">The timeout for command execution</param>
		/// <returns>An IAsyncResult that references the asynchronous operation.</returns>
		IAsyncResult BeginSendCommand(Command command, int timeOut)
		{
			return BeginSendCommand(command, timeOut, null, null);
		}

		/// <summary>
		/// Begins to asynchronously send a command and wait from a response
		/// </summary>
		/// <param name="command">The command to send.</param>
		/// <param name="timeOut">The timeout for command execution</param>
		/// <param name="callback">An AsyncCallback delegate that references the method to invoke when the operation is complete.</param>
		/// <param name="state">A user-defined object that contains information about the operation. This object is passed to the EndReceive delegate when the operation is complete.</param>
		/// <returns>An IAsyncResult that references the asynchronous operation.</returns>
		IAsyncResult BeginSendCommand(Command command, int timeOut, AsyncCallback callback, object state)
		{
			Response response;
			if (command == null)
				throw new ArgumentNullException();
			AsyncSendAndWaitCaller caller = new AsyncSendAndWaitCaller(SendAndWait);
			IAsyncResult result = caller.BeginInvoke(command, timeOut, out response, callback, state);
			return result;
		}

		*/

		/// <summary>
		/// Ends a pending asynchronous wait from response
		/// </summary>
		/// <param name="asyncResult">An IAsyncResult that stores state information and any user defined data for this asynchronous operation.</param>
		/// <param name="response">The response received during the asynchronous operation</param>
		/// <returns>true if a response has been received, false otherwise.</returns>
		bool EndSendCommand(IAsyncResult asyncResult, out Response response);

		#region Command Injection

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="command">The Command object to be executed</param>
		void BeginCommandExecution(Command command);

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="command">The Command object to be executed</param>
		/// <param name="source">The IMessageSource object which will receive the Response object generated after the command execution</param>
		void BeginCommandExecution(Command command, IMessageSource source);

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="command">The Command object to be executed</param>
		/// <param name="source">The IMessageSource object which will receive the Response object generated after the command execution</param>
		/// <param name="metaData">The object which will be attached to the command and it's response as MetaData</param>
		void BeginCommandExecution(Command command, IMessageSource source, object metaData);

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="commandName">The name of the command to be executed</param>
		/// <param name="parameters">The parameters for the command to be executed</param>
		void BeginCommandExecution(string commandName, string parameters);

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="commandName">The name of the command to be executed</param>
		/// <param name="parameters">The parameters for the command to be executed</param>
		/// <param name="source">The IMessageSource object which will receive the Response object generated after the command execution</param>
		void BeginCommandExecution(string commandName, string parameters, IMessageSource source);

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="commandName">The name of the command to be executed</param>
		/// <param name="parameters">The parameters for the command to be executed</param>
		/// <param name="source">The IMessageSource object which will receive the Response object generated after the command execution</param>
		/// <param name="metaData">The object which will be attached to the command and it's response as MetaData</param>
		void BeginCommandExecution(string commandName, string parameters, IMessageSource source, object metaData);

		#endregion

		#endregion
	}
}
