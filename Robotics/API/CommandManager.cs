using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Robotics.Utilities;

namespace Robotics.API
{
	/// <summary>
	/// Represent the method that will handle the SharedVariablesLoaded of a CommandManager
	/// </summary>
	/// <param name="cmdMan">The CommandManager object which raises the event</param>
	public delegate void SharedVariablesLoadedEventHandler(CommandManager cmdMan);

	/// <summary>
	/// Handles asynchronous calls to the SendAndWait Method
	/// </summary>
	/// <param name="command">Command to be sent</param>
	/// <param name="timeOut">The timeout for command execution</param>
	/// <param name="response">The response received</param>
	/// <returns>true if command was sent and its response received. false otherwise</returns>
	public delegate bool AsyncSendAndWaitCaller(Command command, int timeOut, out Response response);

	/// <summary>
	/// Manages incoming commands and its responses
	/// </summary>
	/// <remarks>This class is incomplete and should not be used</remarks>
	[ComVisible(true)]
	[Guid("E4DE45D7-8E79-4c5e-877C-3F65D2CE17F5")]
	[ClassInterface(ClassInterfaceType.None)]
	public partial class CommandManager : IService, ICommandManager
	{

		#region Variables
		/// <summary>
		/// Represents the Connector_CommandReceived method
		/// </summary>
		private readonly EventHandler<IConnector, Command> dlgCommandReceived;
		/// <summary>
		/// Represents the Connector_ResponseReceived method
		/// </summary>
		private readonly EventHandler<IConnector, Response> dlgResponseReceived;
		/// <summary>
		/// Represents the Connector_Connected method
		/// </summary>
		private readonly Action<IConnector> dlgConnectorConnected;
		/// <summary>
		/// Represents the Connector_Disconnected method
		/// </summary>
		private readonly Action<IConnector> dlgConnectorDisconnected;
		/// <summary>
		/// Stores the reference to the ConnectionManager object
		/// </summary>
		private ConnectionManager cnnMan;
		/// <summary>
		/// The default connector object used to send and receive messages
		/// </summary>
		private IConnector connector;
		/// <summary>
		/// Queue of received commands.
		/// </summary>
		private readonly ProducerConsumer<Command> commandsReceived;
		/// <summary>
		/// Queue of received responses.
		/// </summary>
		private readonly ProducerConsumer<Response> responsesReceived;
		/// <summary>
		/// List of received responses which has not been paired with a sent command
		/// </summary>
		private readonly List<Response> unpairedResponses;
		/// <summary>
		/// Stores the CommandExecuter objects which the CommandManager instance manages
		/// </summary>
		private readonly CommandExecuterCollection executers;
		/// <summary>
		/// List of shared variables
		/// </summary>
		/// <remarks>shared variables are stored in blackboard</remarks>
		private SharedVariableList sharedVariables;
		/// <summary>
		/// Stores an autoId for commands
		/// </summary>
		private int autoId = 0;
		/// <summary>
		/// Indicates that the Connector has just connected
		/// </summary>
		private bool firstConnected;
		/// <summary>
		/// Flag that indicates if the shared variable list has been retrieved
		/// </summary>
		private bool shvLoaded;

		#region Control Variables

		/// <summary>
		/// Stores the busy state of the module that controls the command manager
		/// </summary>
		private bool busy;

		/// <summary>
		/// ResetEvent used to initialization sync tasks
		/// </summary>
		private AutoResetEvent initializationSyncEvent;

		/// <summary>
		/// Stores the ready state of the module that controls the command manager
		/// </summary>
		private bool ready;

		#endregion

		#region ParallelSW Variables

		/// <summary>
		/// Queue for parallel send-and-wait operations
		/// </summary>
		private readonly Queue<Command> parallelSendAndWaitQueue;

		/// <summary>
		/// Indicates if a Parallel Send and Wait operation has been started
		/// </summary>
		private bool parallelSendStarted;

		/// <summary>
		/// Stores the Id of the thread that performed a lock over the parallelSendAndWaitQueue object
		/// </summary>
		private int pswOwner;

		#endregion

		#region Thread Variables

		/// <summary>
		/// Indicates if main thread is running
		/// </summary>
		protected bool running;
		/// <summary>
		/// The main thread, it parses commands
		/// </summary>
		protected Thread mainThread;
		/// <summary>
		/// Used to asyncrhonously parse responses in order to improve performance.
		/// </summary>
		private Thread responseParserThread;
		/// <summary>
		/// Thread used to update the shared variable list
		/// </summary>
		protected Thread sharedVariableListUpdaterThread;
		/// <summary>
		/// Thread used to send prototypes
		/// </summary>
		protected Thread prototypeSenderThread;
		/// <summary>
		/// Represents the MainThreadTask method
		/// </summary>
		private readonly ThreadStart dlgMainThreadTask;
		/// <summary>
		/// Represents the ResponseParserThreadTask method
		/// </summary>
		private readonly ThreadStart dlgResponseParserThreadTask;
		/// <summary>
		/// Represents the UpdateSharedVariableListTask method
		/// </summary>
		private readonly ThreadStart dlgUpdateSharedVariableListTask;
		/// <summary>
		/// Represents the SendPrototypesListTask method
		/// </summary>
		private readonly ThreadStart dlgSendPrototypesListTask;

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of CommandManager
		/// </summary>
		public CommandManager()
		{
			
			commandsReceived = new ProducerConsumer<Command>();
			responsesReceived = new ProducerConsumer<Response>();
			unpairedResponses = new List<Response>();
			parallelSendAndWaitQueue = new Queue<Command>();
			sharedVariables = new SharedVariableList(this);
			executers = new CommandExecuterCollection(this);

			dlgCommandReceived = new EventHandler<IConnector, Command>(Connector_CommandReceived);
			dlgResponseReceived = new EventHandler<IConnector, Response>(Connector_ResponseReceived);
			dlgConnectorConnected = new Action<IConnector>(Connector_Connected);
			dlgConnectorDisconnected = new Action<IConnector>(Connector_Disconnected);
			dlgMainThreadTask = new ThreadStart(MainThreadTask);
			dlgResponseParserThreadTask = new ThreadStart(ResponseParserThreadTask);
			dlgUpdateSharedVariableListTask = new ThreadStart(UpdateSharedVariableListTask);
			dlgSendPrototypesListTask = new ThreadStart(SendPrototypesListTask);
			initializationSyncEvent = new AutoResetEvent(false);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a command is executed
		/// </summary>
		public event CommandExecutedEventHandler CommandExecuted;

		/// <summary>
		/// Occurs when a command is received
		/// </summary>
		public event CommandReceivedEventHandler CommandReceived;

		/// <summary>
		/// Occurs when a response is received
		/// </summary>
		public event ResponseReceivedEventHandler ResponseReceived;

		/// <summary>
		/// Occurs when the status of the CommandManager changes
		/// </summary>
		public event CommandManagerStatusChangedEventHandler StatusChanged;

		/// <summary>
		/// Occurs when the CommandManager is started
		/// </summary>
		public event CommandManagerStatusChangedEventHandler Started;

		/// <summary>
		/// Occurs when the CommandManager is stopped
		/// </summary>
		public event CommandManagerStatusChangedEventHandler Stopped;

		/// <summary>
		/// Occurs when new shared variables has been imported from blackboard
		/// </summary>
		public event SharedVariablesLoadedEventHandler SharedVariablesLoaded;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Id for commands
		/// </summary>
		public virtual int AutoId
		{
			get { return autoId; }
			set
			{
				if (value > 99) autoId = 0;
				else autoId = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the CommandManager is globaly busy.
		/// </summary>
		public virtual bool Busy
		{
			get { return this.busy; }
			set
			{
				if (this.busy == value)
					return;

				busy = value;

				if ((connector != null) && connector.IsRunning)
					Send(new Response(this, "busy", "", busy));
				OnStatusChanged();
			}
		}

		/// <summary>
		/// Gets or sets the ConnectionManager object asociated to this CommandManager object
		/// </summary>
		public virtual ConnectionManager ConnectionManager
		{
			get { return cnnMan; }
			set
			{
				if (value == null) throw new ArgumentNullException();
				// If asociated ConnectionManager and new ConnectionManager are the same, do nothing
				if (cnnMan == value) return;
				// Free asociated ConnectionManager (if any)
				if ((cnnMan != null) && (cnnMan.CommandManager != this))
					cnnMan.CommandManager = null;
				// Set new ConnectionManager
				cnnMan = value;
				cnnMan.CommandManager = this;
				Connector = cnnMan;
			}
		}

		/// <summary>
		/// Gets or sets the IConnectionManager object asociated to this CommandManager object
		/// </summary>
		IConnectionManager ICommandManager.ConnectionManager
		{
			get { return this.cnnMan; }
		}

		/// <summary>
		/// Gets or sets the default IConnector object used to send and receive messages
		/// </summary>
		public IConnector Connector
		{
			get { return this.connector; }
			set
			{
				if (value == null) throw new ArgumentNullException();
				// If asociated IConnector and new IConnector are the same, do nothing
				if (connector == value) return;
				// Free asociated IConnector (if any)
				if (connector != null)
				{
					connector.Connected -= dlgConnectorConnected;
					connector.Disconnected -= dlgConnectorDisconnected;
					connector.CommandReceived -= dlgCommandReceived;
					connector.ResponseReceived -= dlgResponseReceived;
				}
				// Set new ConnectionManager
				connector = value;
				connector.Connected += dlgConnectorConnected;
				connector.Disconnected += dlgConnectorDisconnected;
				connector.CommandReceived += dlgCommandReceived;
				connector.ResponseReceived += dlgResponseReceived;
			}
		}

		/// <summary>
		/// Gets the collection of CommandExecuters contained within the CommandManager
		/// </summary>
		public CommandExecuterCollection CommandExecuters
		{
			get { return executers; }
		}

		/// <summary>
		/// Gets the name of the module that this CommandManager object interfaces.
		/// </summary>
		public virtual string ModuleName
		{
			get
			{
				if (connector == null) return "CommandManager";
				return connector.ModuleName;
			}
		}

		/// <summary>
		/// Gets a value indicating if this instance of CommandManager has been started
		/// </summary>
		public bool IsRunning
		{
			get { return running; }
		}

		/// <summary>
		/// Gets a value indicating if a parallel send-and-wait operation has been started
		/// </summary>
		public bool ParallelSendAndWaitStarted
		{
			get { return parallelSendStarted; }
		}

		/// <summary>
		/// Gets or sets the ready state of the module that controls the command manager
		/// </summary>
		public virtual bool Ready
		{
			get { return this.ready; }
			set
			{
				if (this.ready == value)
					return;

				ready = value;

				if ((connector != null) && connector.IsRunning)
					Send(new Response(this, "ready", "", ready));
			}
		}

		/// <summary>
		/// Gets the list of shared variables
		/// </summary>
		/// <remarks>shared variables are stored in blackboard</remarks>
		public SharedVariableList SharedVariables
		{
			get { return this.sharedVariables; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Adds a SharedVariable to the CommandManager.
		/// If the shared variable is already registered, the reference is updated, otherwise a new shared variable of the type is created
		/// </summary>
		/// <param name="sharedVariable">A shared variable object used to create the new SharedVariable or update the reference</param>
		public void AddSharedVariable<T>(ref T sharedVariable) where T : SharedVariable
		{
			if (sharedVariable == null)
				throw new ArgumentNullException();
			if (this.sharedVariables.Contains(sharedVariable.Name))
			{
				sharedVariable = (T)this.sharedVariables[sharedVariable.Name];
				return;
			}
			this.sharedVariables.Add(sharedVariable);
		}

		/// <summary>
		/// Adds a SharedVariable to the CommandManager.
		/// If the shared variable is already registered, the reference is updated, otherwise a new shared variable of the type is created
		/// </summary>
		/// <param name="sharedVariable">A shared variable object used to create the new SharedVariable or update the reference</param>
		/// <param name="reportType">The type of report required for subscription</param>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <param name="updateEventHandler">A delegate that represents the method that will handle the Updated event of the shared variable</param>
		public void AddSharedVariable<T>(ref T sharedVariable, SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, SharedVariableUpdatedEventHadler updateEventHandler) where T : SharedVariable
		{
			if (sharedVariable == null)
				throw new ArgumentNullException();
			if (this.sharedVariables.Contains(sharedVariable.Name))
			{
				sharedVariable = (T)this.sharedVariables[sharedVariable.Name];
				return;
			}
			this.sharedVariables.Add(sharedVariable);
			sharedVariable.Subscribe(reportType, subscriptionType);
			sharedVariable.Updated+= updateEventHandler;
		}

		/// <summary>
		/// Prepares the async command executer to perform multiple send-and-wait operations in parallel
		/// </summary>
		protected internal void BeginParallelSendAndWait()
		{
			if(!Monitor.TryEnter(parallelSendAndWaitQueue, 1))
				throw new Exception("Only one parallel send-and-wait operation can be performed at a time");
			// Check if current thread is the owner of the lock
			pswOwner = Thread.CurrentThread.ManagedThreadId;
			parallelSendStarted = true;
			// Clear the parallelSendAndWaitQueue. It should not be necesary since the object is synchronized
			// and should BeginCommandExecution empty, however, lets prevent bugs
			parallelSendAndWaitQueue.Clear();
		}

		/// <summary>
		/// Begins to asynchronously send a command and wait from a response
		/// </summary>
		/// <param name="command">The command to send.</param>
		/// <param name="timeOut">The timeout for command execution</param>
		/// <returns>An IAsyncResult that references the asynchronous operation.</returns>
		public IAsyncResult BeginSendCommand(Command command, int timeOut)
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
		public IAsyncResult BeginSendCommand(Command command, int timeOut, AsyncCallback callback, object state)
		{
			Response response;
			if (command == null)
				throw new ArgumentNullException();
			AsyncSendAndWaitCaller caller = new AsyncSendAndWaitCaller(SendAndWait);
			IAsyncResult result = caller.BeginInvoke(command, timeOut, out response, callback, state);
			return result;
		}

		/// <summary>
		/// Ends a pending asynchronous wait from response
		/// </summary>
		/// <param name="asyncResult">An IAsyncResult that stores state information and any user defined data for this asynchronous operation.</param>
		/// <param name="response">The response received during the asynchronous operation</param>
		/// <returns>true if a response has been received, false otherwise.</returns>
		public bool EndSendCommand(IAsyncResult asyncResult, out Response response)
		{
			System.Runtime.Remoting.Messaging.AsyncResult result;
			AsyncSendAndWaitCaller caller;

			if (asyncResult == null)
				throw new ArgumentNullException();
			result = asyncResult as System.Runtime.Remoting.Messaging.AsyncResult;

			if (result == null)
				throw new ArgumentException("asyncResult was not returned by a call to the BeginReceive method. ");
			caller = result.AsyncDelegate as AsyncSendAndWaitCaller;
			if (caller == null)
				throw new ArgumentException("asyncResult was not returned by a call to the BeginReceive method. ");
			return caller.EndInvoke(out response, asyncResult);
		}

		/// <summary>
		/// Executes all pending send-and-wait operations
		/// </summary>
		/// <param name="timeOut">The overall timeout for parallel command execution</param>
		/// <param name="results">Array of Command/Response pairs result of parallel execution</param>
		protected internal bool CommitParallelSendAndWait(int timeOut, out CommandResponsePair[] results)
		{
			if (!parallelSendStarted)
				throw new Exception("Parallel send-and-wait has not been initialized");

			if (pswOwner != Thread.CurrentThread.ManagedThreadId)
				throw new Exception("Parallel send-and-wait was not initialized for this thread context");

			// Stores the commands from the queue
			Command[] commands;
			// Result of the parallel operation
			bool result;
			// General counter
			int i;

			commands = new Command[parallelSendAndWaitQueue.Count];
			for(i = 0; i < commands.Length; ++i)
				commands[i] = parallelSendAndWaitQueue.Dequeue();
			result = MultipleSendAndWait(commands, timeOut, out results);

			// Clear the parallelSendAndWaitQueue. It should not be necesary since the object is synchronized;
			// however, lets prevent bugs
			parallelSendAndWaitQueue.Clear();
			parallelSendStarted = false;
			Monitor.PulseAll(parallelSendAndWaitQueue);
			Monitor.Exit(parallelSendAndWaitQueue);

			return result;
		}

		/// <summary>
		/// Clears out all the contents of dataReceived, commandsReceived, responsesReceived and unpairedResponses Queues/Lists
		/// </summary>
		protected void CleanBuffers()
		{
			commandsReceived.Clear();
			responsesReceived.Clear();
			lock (unpairedResponses)
				unpairedResponses.Clear();
			parallelSendAndWaitQueue.Clear();
		}

		/// <summary>
		/// Raises the CommandExecuted event
		/// </summary>
		/// <param name="commandExecuter">The command executer used to execute the received command</param>
		/// <param name="executedCommand">The command which was executed</param>
		/// <param name="generatedResponse">The response generated due to command execution</param>
		protected internal virtual void OnCommandExecuted(CommandExecuter commandExecuter, Command executedCommand, Response generatedResponse)
		{
			if (executedCommand == null)
				return;
			if (this.CommandExecuted != null)
			{
				try { this.CommandExecuted(this, commandExecuter, executedCommand, generatedResponse); }
				catch { }
			}
		}

		/// <summary>
		/// Enqueues a command for a parallel send-and-wait operation
		/// </summary>
		/// <param name="command">Command to enqueue</param>
		protected internal void EnqueueCommand(Command command)
		{
			if (command == null)
				throw new ArgumentNullException();

			if (!parallelSendStarted)
				throw new Exception("Parallel send-and-wait has not been initialized");

			if (pswOwner != Thread.CurrentThread.ManagedThreadId)
				throw new Exception("Parallel send-and-wait was not initialized for this thread context");

			parallelSendAndWaitQueue.Enqueue(command);
		}

		/// <summary>
		/// Raises the CommandReceived event
		/// </summary>
		/// <param name="command">The received command</param>
		protected virtual void OnCommandReceived(Command command)
		{
			try
			{
				if (this.CommandReceived != null)
					CommandReceived(command);
			}
			catch { }
				//CommandReceived(this, command);
		}

		/// <summary>
		/// Raises the ResponseReceived event
		/// </summary>
		/// <param name="response">the received response</param>
		protected virtual void OnResponseReceived(Response response)
		{
			try
			{
				if (this.ResponseReceived != null)
					ResponseReceived(response);
			}
			catch { }
				//ResponseReceived(this, response);
		}

		/// <summary>
		/// Raises the SharedVariablesLoaded event
		/// </summary>
		protected virtual void OnSharedVariablesLoaded()
		{
			try
			{
				if (this.SharedVariablesLoaded != null)
					SharedVariablesLoaded(this);
			}
			catch { }

			//if (this.SharedVariablesLoaded == null)
			//	return;
			//if (Thread.CurrentThread.ManagedThreadId == mainThread.ManagedThreadId)
			//{
			//	Thread eventThread = new Thread(new ThreadStart(OnSharedVariablesLoaded));
			//	eventThread.IsBackground = true;
			//	return;
			//}
			//else 
			//	SharedVariablesLoaded(this);
		}

		/// <summary>
		/// Raises the StatusChanged event
		/// </summary>
		/// <remarks>The OnStatusChanged method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnStatusChanged in a derived class, be sure to call the base class's OnStatusChanged method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnStatusChanged()
		{
			try
			{
				if (StatusChanged != null)
					StatusChanged(this);
			}
			catch { }
		}

		/// <summary>
		/// Raises the Started event
		/// </summary>
		/// <remarks>The OnStart method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnStart in a derived class, be sure to call the base class's OnStart method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnStart()
		{
			try
			{
				if (Started != null)
					Started(this);
			}
			catch { }
		}

		/// <summary>
		/// Raises the Stopped event
		/// </summary>
		/// <remarks>The OnStop method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnStop in a derived class, be sure to call the base class's OnStop method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnStop()
		{
			try
			{
				if (Stopped != null)
					Stopped(this);
			}
			catch { }
		}

		/// <summary>
		/// Starts the Connection Manager
		/// </summary>
		public void Start()
		{
			if (running) return;
			CleanBuffers();
			shvLoaded = false;
			mainThread = new Thread(dlgMainThreadTask);
			mainThread.IsBackground = true;
			responseParserThread = new Thread(dlgResponseParserThreadTask);
			responseParserThread.IsBackground = true;
			mainThread.Start();
		}

		/// <summary>
		/// Stops the Connection Manager
		/// </summary>
		public void Stop()
		{
			running = false;
			CleanBuffers();
		}
		
		/// <summary>
		/// Executes async tasks
		/// </summary>
		protected virtual void MainThreadTask()
		{
			Command cmd;

			running = true;
			//int i = 0;
			if (responseParserThread != null)
				responseParserThread.Start();
			if ((connector != null) && (!connector.IsRunning))
				connector.Start();

			if (running)
			{
				OnStart();
				OnStatusChanged();
			}
			while (running)
			{
				//shvRetElapsed = DateTime.Now - lastShvRetTime;
				//// Update shared variable list
				//if (!shvlUpdateRequested && (connector.ConnectedClientsCount > 0) && (!shvLoaded || ((shvRetInterval != -1) && (shvRetElapsed.TotalMilliseconds > shvRetInterval))))
				//{
				//	//shvlUpdateRequested = true;
				//	////shvLoaded = true;
				//	//SharedVariableListRequestUpdate();
				//	UpdateSharedVariableList();
				//}

				cmd = commandsReceived.Consume(100);
				if (cmd != null)
					ParseCommand(cmd);
			}

			OnStop();
			OnStatusChanged();
			if ((connector != null) && connector.IsRunning)
				connector.Stop();

			JoinResponseParserThread();
			JoinSharedVariableThread();
		}

		/// <summary>
		/// Asyncrhonously manages the incomming responses.
		/// It can be done by the MainThread but is implemented to
		/// increase performance
		/// </summary>
		private void ResponseParserThreadTask()
		{
			Response rsp;

			while (running)
			{
				try
				{
					rsp = responsesReceived.Consume();
					if (rsp != null)
						ParseResponse(rsp);
				}
				catch (ThreadInterruptedException) { continue; }
				catch (ThreadAbortException) { return; }
				catch { continue; }
			}
		}

		/// <summary>
		/// Waits for the sharedVariableListUpdaterThread to finish and join
		/// </summary>
		private void JoinSharedVariableThread()
		{
			if ((sharedVariableListUpdaterThread != null) && sharedVariableListUpdaterThread.IsAlive)
			{
				sharedVariableListUpdaterThread.Join(10);
				if (sharedVariableListUpdaterThread.IsAlive)
					sharedVariableListUpdaterThread.Abort();
			}
		}

		/// <summary>
		/// Waits for the responseParserThread to finish and join
		/// </summary>
		private void JoinResponseParserThread()
		{
			if ((responseParserThread != null) && responseParserThread.IsAlive)
			{
				responseParserThread.Join(100);
				if (responseParserThread.IsAlive)
					responseParserThread.Abort();
			}
		}

		/// <summary>
		/// Request the list of all shared variables stored in bloackboard.
		/// It waits 5ms for an answer
		/// </summary>
		private void SharedVariableListRequestUpdate()
		{
			//Response rsp;
			Command cmdListVars;
			

			// 1. Send the list_vars command
			cmdListVars = new Command("list_vars", "");
			this.connector.Send(cmdListVars);
			return;

			//// 2. Wait up to 5 milliseconds for a response arrival
			//rsp = responsesReceived.Consume(15);
			//// 2.1. If no response has arrived, return
			//if (rsp == null) 
			//	return;
			//// 2.2 If the response was not the expected, return it to the queue and quit 
			//if (rsp.CommandName != "list_vars")
			//{
			//	responsesReceived.Produce(rsp);
			//	return;
			//}
			//// 3. The response was the expected one, check if there are new variables
		}

		/// <summary>
		/// Updates the shared variable list from the blackboard
		/// </summary>
		private void UpdateSharedVariableList()
		{
			if((sharedVariableListUpdaterThread != null) && sharedVariableListUpdaterThread.IsAlive)
				return;
			initializationSyncEvent.Reset();
			sharedVariableListUpdaterThread = new Thread(dlgUpdateSharedVariableListTask);
			sharedVariableListUpdaterThread.IsBackground = true;
			sharedVariableListUpdaterThread.Start();
		}

		/// <summary>
		/// Updates the shared variable list from the blackboard
		/// </summary>
		private void UpdateSharedVariableListTask()
		{
			Response response;
			Command cmdListVars;
			Command cmdReadVars;
			string[] varNames;
			List<string> unknownVars;
			int count;
			int tries = 0;

			while (running)
			{
				if(initializationSyncEvent.WaitOne(100))
					break;
				if (tries > 100)
					return;
				++tries;
			}

			tries = 0;
			do
			{
				if (tries++ > 5)
					Thread.Sleep(1000);
				else if (tries > 1)
					Thread.Sleep(100);

				// 1. Send the list_vars command and wait up to 300 milliseconds for a response arrival
				cmdListVars = new Command("list_vars", "");
				if (!SendAndWait(cmdListVars, 300, out response))
					continue;

				// 2. Get the unknown vars
				// 2.1. Split the variable name array
				varNames = response.Parameters.Split(' ');
				unknownVars = new List<string>(varNames.Length);
				// 2.3. Add to the list the unknown variables
				for (int i = 0; i < varNames.Length; ++i)
				{
					if (String.IsNullOrEmpty(varNames[i]) || sharedVariables.Contains(varNames[i]))
						continue;
					unknownVars.Add(varNames[i]);
				}
				// 2.4. If no variables ar unknown, quit.
				if (unknownVars.Count == 0)
					continue;

				// 2.5. Create the command and send it
				cmdReadVars = new Command("read_vars", String.Join(" ", unknownVars.ToArray()));
				if (!SendAndWait(cmdReadVars, 500, out response))
					continue;

				// 3. Update the list of variables using the received response
				// 3.1. Update
				count = sharedVariables.UpdateFromBlackboard(response);
				// 3.2. If new variables has been added, rise the event
				if ((!shvLoaded) && (count > 0))
					OnSharedVariablesLoaded();
				// 3.3 Set the flag
				shvLoaded = true;

			} while (running && !shvLoaded);
		}

		/// <summary>
		/// Sends the list of supported commands to the blackboard
		/// </summary>
		private void SendPrototypesList()
		{
			if ((prototypeSenderThread != null) && prototypeSenderThread.IsAlive)
				return;
			prototypeSenderThread = new Thread(dlgSendPrototypesListTask);
			prototypeSenderThread.IsBackground = true;
			prototypeSenderThread.Start();
		}

		/// <summary>
		/// Sends the list of supported commands to the blackboard
		/// </summary>
		private void SendPrototypesListTask()
		{
			ushort flags;
			StringBuilder sb = new StringBuilder();
			CommandExecuter[] acex = this.CommandExecuters.ToArray();
			foreach (CommandExecuter ce in acex)
			{
				sb.Append(ce.CommandName);
				sb.Append(' ');
				flags = (ushort)(ce.Priority & 0xFF);
				if (ce.ResponseRequired) flags |= 0x0100;
				if (ce.ParametersRequired) flags |= 0x0200;
				sb.Append(flags);
				sb.Append(' ');
				sb.Append(ce.Timeout);
				sb.Append(' ');
			}
			if (sb.Length > 0)
				--sb.Length;

			Response rspReady = new Response("ready", String.Empty, Ready);
			Send(rspReady);
			Response rspBusy = new Response("busy", String.Empty, Busy);
			Send(rspBusy);
			Command cmdProto = new Command("prototypes", sb.ToString());
			Send(cmdProto);
		}

		#region Command Injection

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="command">The Command object to be executed</param>
		public void BeginCommandExecution(Command command)
		{
			BeginCommandExecution(command, this, null);
		}

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="command">The Command object to be executed</param>
		/// <param name="source">The IMessageSource object which will receive the Response object generated after the command execution</param>
		public void BeginCommandExecution(Command command, IMessageSource source)
		{
			BeginCommandExecution(command, source, null);
		}

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="command">The Command object to be executed</param>
		/// <param name="source">The IMessageSource object which will receive the Response object generated after the command execution</param>
		/// <param name="metaData">The object which will be attached to the command and it's response as MetaData</param>
		public void BeginCommandExecution(Command command, IMessageSource source, object metaData)
		{
			if (command.MessageSource == null)
			{
				if (source == null)
					command.MessageSource = this;
				else
					command.MessageSource = source;
			}
			else if (command.MessageSource != source)
				command.MessageSource = source;
			command.MessageSourceMetadata = metaData;

			commandsReceived.Produce(command);
		}

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="commandName">The name of the command to be executed</param>
		/// <param name="parameters">The parameters for the command to be executed</param>
		public void BeginCommandExecution(string commandName, string parameters)
		{
			BeginCommandExecution(commandName, parameters, this, null);
		}

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="commandName">The name of the command to be executed</param>
		/// <param name="parameters">The parameters for the command to be executed</param>
		/// <param name="source">The IMessageSource object which will receive the Response object generated after the command execution</param>
		public void BeginCommandExecution(string commandName, string parameters, IMessageSource source)
		{
			BeginCommandExecution(commandName, parameters, source, null);
		}

		/// <summary>
		/// Adds a command to the execution Queue
		/// </summary>
		/// <param name="commandName">The name of the command to be executed</param>
		/// <param name="parameters">The parameters for the command to be executed</param>
		/// <param name="source">The IMessageSource object which will receive the Response object generated after the command execution</param>
		/// <param name="metaData">The object which will be attached to the command and it's response as MetaData</param>
		public void BeginCommandExecution(string commandName, string parameters, IMessageSource source, object metaData)
		{
			Command cmd = new Command(commandName, parameters);
			cmd.MessageSource = source;
			BeginCommandExecution(cmd, source, metaData);
		}

		#endregion

		#region Incomming Data Management

		/// <summary>
		/// Manages incomming commands from a IConnector source
		/// </summary>
		/// <param name="sender">The IConnector object which sends the command</param>
		/// <param name="command">The command received</param>
		protected void Connector_CommandReceived(IConnector sender, Command command)
		{
			if (command == null)
				return;
			if (command.MessageSource == null)
				command.MessageSource = sender;
			this.commandsReceived.Produce(command);
			OnCommandReceived(command);
		}

		/// <summary>
		/// Manages incomming responses from a IConnector source
		/// </summary>
		/// <param name="sender">The IConnector object which sends the command</param>
		/// <param name="response">The response received</param>
		protected void Connector_ResponseReceived(IConnector sender, Response response)
		{
			if (response == null)
				return;
			if (response.MessageSource == null)
				response.MessageSource = sender;
			this.responsesReceived.Produce(response);
			OnResponseReceived(response);
		}

		/// <summary>
		/// Manages the Connected event of a IConnector source
		/// </summary>
		/// <param name="sender">The IConnector object which raises the event</param>
		protected void Connector_Connected(IConnector sender)
		{
			if (!firstConnected)
			{
				firstConnected = true;
				UpdateSharedVariableList();
				initializationSyncEvent.Set();
			}
			SendPrototypesList();
		}

		/// <summary>
		/// Manages the Connected event of a IConnector source
		/// </summary>
		/// <param name="sender">The IConnector object which raises the event</param>
		protected void Connector_Disconnected(IConnector sender) 
		{
			initializationSyncEvent.Reset();
			shvLoaded = false;
		}

		/// <summary>
		/// Manages updates of the shared variable list
		/// </summary>
		/// <param name="response">Response which contains a the shared vaiable data</param>
		protected bool ManageSharedVariableList(Response response)
		{
			// 2.1. If no adequate response, return false
			if ((response == null) || (response.Parameters.Length < 1))
				return false;

			if (response.CommandName == "list_vars")
			{
				string[] varNames = response.Parameters.Split(' ');
				List<string> unknownVars = new List<string>(varNames.Length);
				for (int i = 0; i < varNames.Length; ++i)
				{
					if (String.IsNullOrEmpty(varNames[i]) || sharedVariables.Contains(varNames[i]))
						continue;
					unknownVars.Add(varNames[i]);
				}

				if (unknownVars.Count == 0)
					return true;
				Command cmdReadVars = new Command("read_vars", String.Join(" ", unknownVars.ToArray()));
				this.connector.Send(cmdReadVars);
				return true;
			}
			else if (response.CommandName == "read_vars")
			{
				
			}
			return false;
		}

		/// <summary>
		/// Manages the events of the shared variables
		/// </summary>
		/// <param name="response">Response which contains a the shared vaiable data</param>
		protected bool ManageSubscription(Response response)
		{
			/*
			string parameters;
			int pos;
			string type;
			bool isArray;
			ushort usize;
			int size;
			string name;
			string data;
			string sReportType;
			string sSubscriptionType;
			string writer;
			int cc;
			SharedVariableReportType reportType;
			SharedVariableSubscriptionType subscriptionType;
			Exception ex;

			parameters = response.Parameters;
			// 1. Get writer
			pos = parameters.LastIndexOf('%');
			if(pos == -1) return false;
			writer = parameters.Substring(pos+1).Trim();
			parameters = parameters.Substring(0, pos);
			
			// 2. Get subscription type
			pos = parameters.LastIndexOf('%');
			if (pos == -1) return false;
			sSubscriptionType = parameters.Substring(pos + 1).Trim();
			parameters = parameters.Substring(0, pos);

			// 3. Get report type.
			pos = Math.Max(parameters.LastIndexOf('%'), parameters.LastIndexOf('}'));
			if (pos == -1) return false;
			sReportType = parameters.Substring(pos + 1).Trim();
			parameters = parameters.Substring(0, pos);
			
			// 4. Get variable type
			cc = 0;
			if (parameters[cc] == '{')
				parameters = parameters.Substring(1, parameters.Length - 1);
			parameters = parameters.Trim();
			if(!Parser.XtractIdentifier(parameters, ref cc, out type))
				return false;
			isArray = false;
			size = -1;
			if (Scanner.ReadChar('[', parameters, ref cc))
			{
				if (Scanner.XtractUInt16(parameters, ref cc, out usize))
					size = usize;
				if (!Scanner.ReadChar(']', parameters, ref cc))
					return false;
				isArray = true;
			}
			
			// 5. Get variable name
			Scanner.SkipSpaces(parameters, ref cc);
			if (!Parser.XtractIdentifier(parameters, ref cc, out name))
				return false;

			// 6. Get variable data
			Scanner.SkipSpaces(parameters, ref cc);
			data = parameters.Substring(cc);

			// Update variable
			if (!sharedVariables.Contains(name))
				return true;

			try { reportType = (SharedVariableReportType)Enum.Parse(typeof(SharedVariableReportType), sReportType, true); }
			catch { return true; }
			try { subscriptionType = (SharedVariableSubscriptionType)Enum.Parse(typeof(SharedVariableSubscriptionType), sSubscriptionType, true); }
			catch { return true; }

			sharedVariables[name].Update(type, isArray, size, name, data, reportType, subscriptionType, writer, out ex);
			return true;
			*/

			// 1. Get report info
			SharedVariableReport report;
			Exception ex;
			string name;
			if (!SharedVariableReport.CreateFromResponse(response, out report, out ex))
				return false;
			name = report.VariableInfo.Name;

			// 2. Update variable
			if (!sharedVariables.Contains(name))
				return true;

			sharedVariables[name].Update(report, out ex);
			return true;
		}

		/// <summary>
		/// Parses a received Command
		/// </summary>
		/// <param name="command">Command received</param>
		/// <remarks>This method is incomplete and should not be used</remarks>
		protected virtual void ParseCommand(Command command)
		{
			if (ParseSystemCommand(command))
				return;

			if (executers.Contains(command.CommandName))
			{
				CommandExecuter executer = executers[command.CommandName];
				//if(!executer.MatchSignature(command))
				//	SendResponse(false, command);
				executer.Execute(command);
			}
		}

		/// <summary>
		/// Parses a received system command
		/// </summary>
		/// <param name="command">Command that represents the system command received</param>
		/// <returns>true if provided command is a system command and has been parsed, false otherwise</returns>
		protected virtual bool ParseSystemCommand(Command command)
		{
			if (!command.IsSystemCommand)
				return false;

			switch (command.CommandName)
			{
				case "alive":
					if (Busy)
						Send(new Response(this, "busy", "", Busy));
					else if(!Ready)
						Send(new Response(this, "ready", "", Ready));
					else
						SendResponse(true, command);
					break;

				case "bin":
					SendResponse(false, command);
					break;

				case "busy":
					SendResponse(Busy, command);
					break;

				case "ready":
					SendResponse(Ready, command);
					break;
			}

			if (this.firstConnected)
			{
				this.firstConnected = false;
				initializationSyncEvent.Set();
			}
			return true;
		}

		/// <summary>
		/// Parses a received Response
		/// </summary>
		/// <param name="response">Response received</param>
		protected virtual void ParseResponse(Response response)
		{
			//ManageSharedVariableList(response);
			if (ManageSubscription(response))
				return;
			lock (unpairedResponses)
			{
				unpairedResponses.Add(response);
			}
		}

		#endregion

		#region Outgoing Data Management

		/// <summary>
		/// Sends multiple commands and waits for its response
		/// </summary>
		/// <param name="commands">Array of Command objects which contains commands to be sent</param>
		/// <param name="timeOut">The overall timeout for command execution</param>
		/// <param name="results">Array of Response objects generated from responses received</param>
		/// <returns>true if at least one command was sent and its response received. false otherwise</returns>
		private bool MultipleSendAndWait(Command[] commands, int timeOut, out CommandResponsePair[] results)
		{
			// Time elapsed for operation
			Stopwatch swElapsed = new Stopwatch();
			// Array of flags that indicates if a response has been found
			bool[] found;
			// Number of responses found
			int foundCount;
			// Array of responses for command matching
			Response[] responses;
			// Response candidate while lookingfor
			Response candidate = null;
			// Number of sent commands
			int commandsSentCount;
			// Array that indicates if command was sent
			bool[] commandsSent;
			// Counter variables
			int i, j;

			if (commands == null)
				throw new ArgumentNullException();
			if (commands.Length < 1)
				throw new ArgumentException("At least one command is required");

			// Initialize arrays
			if (timeOut <= 0) timeOut = Int32.MaxValue;
			found = new bool[commands.Length];
			commandsSent = new bool[commands.Length];
			results = new CommandResponsePair[commands.Length];
			responses = new Response[commands.Length];
			Array.Sort<Command>(commands, BaseMessage.CompareByName);

			// 1. Send the commands and initialize responses.
			commandsSentCount = 0;
			for (i = 0; i < commands.Length; ++i)
			{
				found[i] = false;
				// Congruence check. A command cannot be sent twice on a same packet
				if ((i > 0) && (commands[i - 1].CommandName == commands[i].CommandName))
					throw new Exception("Naming incongruence. A command cannot be sent twice on a same packet");
				if (commandsSent[i] = SendCommand(commands[i]))
					++commandsSentCount;
			}

			// 2. If no command can be sent, return false
			if (commandsSentCount < 1)
				return false;

			// 3. Wait for responses to arrival
			swElapsed.Start();
			swElapsed.Reset();
			foundCount = 0;
			// 3.1 Wait for
			while (running && (swElapsed.ElapsedMilliseconds < timeOut))
			{
				// Check if there is candidates availiable
				if (unpairedResponses.Count < 1)
				{
					if (foundCount >= commandsSentCount) break;
					Thread.Sleep(10);
					continue;
				}

				// Lock to avoid interference with other threads
				lock (unpairedResponses)
				{
					// 3.1 Sort unpaired responses to speed-up search
					unpairedResponses.Sort(BaseMessage.CompareByName);
					// 3.3 For each sent command...
					for (i = 0; running && (i < commands.Length); ++i)
					{
						if (!commandsSent[i] || found[i])
							continue;

						// 3.4 ...find a candidate in received unpaired responses
						for (j = 0; running && (j < unpairedResponses.Count); ++j)
						{
							candidate = unpairedResponses[j];
							// Both, command and responses are sorted, so, if the first character of a response command name
							// is greater than the first first character of command name being tested, then all subsequent
							// responses will not match. So, break and test next command
							if (candidate.CommandName[0] > commands[i].CommandName[0])
								break;
							if (commands[i].IsMatch(candidate))
							{
								// Since command and candidate response matches, the candidate is removed from candidate list
								unpairedResponses.Remove(candidate);
								// The list item count has changed, so a decrement is necesary
								--j;
								// Mark the command as found-response-command
								found[i] = true;
								// Increment the found responses counter
								++foundCount;
								// Asign the candidate.
								responses[i] = candidate;
								break;
							}
						}
						candidate = null;
						if (foundCount >= commandsSentCount) break;
					}
				}
				if (foundCount >= commandsSentCount) break;
				Thread.Sleep(10);
			}
			swElapsed.Stop();

			// 3. Fill output array
			for (i = 0; i < commands.Length; ++i)
			{
				if(found[i])
					results[i] = new CommandResponsePair(commands[i], responses[i]);
				else
					results[i] = new CommandResponsePair(commands[i], Response.CreateFromCommand(commands[i], false));
			}

			// 4. No response arrival. Return false
			if (foundCount == 0)
				return false;

			// Success
			return true;
		}

		/// <summary>
		/// Sends a command through the Connector
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if command was sent, false otherwise</returns>
		private bool Send(Command command)
		{
			if (command == null)
				return false;
			// 1. Set the origin of the command
			command.MessageSource = connector;
			// 2. Send the command. If command cannot be sent, return false
			try
			{
				return connector.Send(command);
			}
			catch { return false; }
		}

		/// <summary>
		/// Sends a response through the Connector
		/// </summary>
		/// <param name="response">Response to be sent</param>
		/// <returns>true if response was sent, false otherwise</returns>
		private bool Send(Response response)
		{
			if (response == null)
				return false;
			// 1. Set the origin of the command
			response.MessageSource = connector;
			// 2. Send the command. If command cannot be sent, return false
			try
			{
				return connector.Send(response);
			}
			catch { return false; }
		}

		/// <summary>
		/// Sends a command through the Connector
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if command was sent and its response received. false otherwise</returns>
		protected internal bool SendCommand(Command command)
		{
			return Send(command);
		}

		/// <summary>
		/// Sends a command and waits for response
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <param name="timeOut">The timeout for command execution</param>
		/// <param name="response">The response received</param>
		/// <returns>true if command was sent and its response received. false otherwise</returns>
		protected internal bool SendAndWait(Command command, int timeOut, out Response response)
		{
			// Time elapsed for operation
			Stopwatch swElapsed = new Stopwatch();
			// Flag that indicates if a response has been found
			bool found = false;
			// Response candidate while lookingfor
			Response candidate = null;

			if (command == null)
				throw new ArgumentNullException();

			// Initialize response
			response = Response.CreateFromCommand(command, false);
			if (timeOut <= 0) timeOut = Int32.MaxValue;

			// Parallel execution check
			if (parallelSendStarted)
			{
				// Check if current thread is the owner of the lock
				if (pswOwner != Thread.CurrentThread.ManagedThreadId)
					return false;
				parallelSendAndWaitQueue.Enqueue(command);
				return true;
			}

			// 1. Send the command. If command cannot be sent, return false
			if (!SendCommand(command))
				return false;

			// 2. Wait for response to arrival
			swElapsed.Reset();
			swElapsed.Start();
			while (running && (swElapsed.ElapsedMilliseconds < timeOut))
			{
				lock (unpairedResponses)
				{
					for (int i = 0; running && (i < unpairedResponses.Count); ++i)
					{
						candidate = unpairedResponses[i];
						if (command.IsMatch(candidate))
						{
							unpairedResponses.Remove(candidate);
							found = true;
							break;
						}
					}
				}
				if (found) break;
				candidate = null;
				Thread.Sleep(10);
			}
			swElapsed.Stop();

			// 3. No response arrival. Return false
			if (!found || (candidate == null))
				return false;

			// Success
			response = candidate;
			return true;
		}

		/// <summary>
		/// Sends multiple commands and waits for its response
		/// </summary>
		/// <param name="commands">Array of Command objects which contains commands to be sent</param>
		/// <param name="timeOut">The overall timeout for command execution</param>
		/// <param name="results">Array of Response objects generated from responses received</param>
		/// <returns>true if at least one command was sent and its response received. false otherwise</returns>
		protected internal bool SendAndWait(Command[] commands, int timeOut, out CommandResponsePair[] results)
		{
			if (!parallelSendStarted)
				return MultipleSendAndWait(commands, timeOut, out results);

			if (commands == null)
				throw new ArgumentNullException();
			if (commands.Length < 1)
				throw new ArgumentException("At least one command is required");

			// Initialize arrays
			results = new CommandResponsePair[commands.Length];
			for (int i = 0; i < commands.Length; ++i)
			{
				results[i] = new CommandResponsePair(commands[i], Response.CreateFromCommand(commands[i], false));
				// Check if current thread is the owner of the lock
				if (pswOwner == Thread.CurrentThread.ManagedThreadId)
					parallelSendAndWaitQueue.Enqueue(commands[i]);
			}
			return false;

			#region Deprecated, Consider revision
			/*

			// Check if current thread is the owner of the lock
			if (pswOwner != Thread.CurrentThread.ManagedThreadId)
			{
				for (int i = 0; i < commands.Length; ++i)
					results[i] = new CommandResponsePair(commands[i], Response.CreateFromCommand(commands[i], false));
				return false;
			}
			for (int i = 0; i < commands.Length; ++i)
				parallelSendAndWaitQueue.Enqueue(commands[i]);
			results = null;
			return true;

			*/
			#endregion
		}

		/// <summary>
		/// Creates a response and sends it through the asociated Connector
		/// </summary>
		/// <param name="success">Indicates if command succeded</param>
		/// <param name="command">Command to respond</param>
		protected internal virtual void SendResponse(bool success, Command command)
		{
			Response response;
			SendResponse(success, command, out response);
		}

		/// <summary>
		/// Creates a response and sends it through the asociated Connector
		/// </summary>
		/// <param name="success">Indicates if command succeded</param>
		/// <param name="command">Command to respond</param>
		/// <param name="response">Generated response</param>
		protected internal virtual void SendResponse(bool success, Command command, out Response response)
		{
			response = Response.CreateFromCommand(command, success);
			SendResponse(response);
		}

		/// <summary>
		/// Sends a Response through the asociated Connectonr
		/// </summary>
		/// <param name="response">Response object to be sent</param>
		protected internal virtual void SendResponse(Response response)
		{
			if ((response.MessageSource == null) || response.MessageSource == this)
				connector.Send(response);
			response.MessageSource.ReceiveResponse(response);
		}

		/// <summary>
		/// Waits for a response for a sent command
		/// </summary>
		/// <param name="commandSent">Command object that represents the command sent</param>
		/// <param name="timeOut">The amount of time to wait for a response arrival</param>
		/// <returns>Response object that represents the response received or null if no response received and timed out</returns>
		protected Response WaitForResponse(Command commandSent, int timeOut)
		{
			// Time elapsed for operation
			Stopwatch swElapsed = new Stopwatch();

			// Response candidate while lookingfor
			Response candidate = null;

			if (commandSent == null)
				throw new ArgumentNullException();

			// Wait for response to arrival
			swElapsed.Reset();
			swElapsed.Start();
			while (running && (swElapsed.ElapsedMilliseconds < timeOut))
			{
				lock (unpairedResponses)
				{
					for (int i = 0; running && (i < unpairedResponses.Count); ++i)
					{
						candidate = unpairedResponses[i];
						if (commandSent.IsMatch(candidate))
						{
							unpairedResponses.Remove(candidate);
							swElapsed.Stop();
							return candidate;
						}
					}
				}
				candidate = null;
				Thread.Sleep(10);
			}

			// Timed out
			swElapsed.Stop();
			return null;
		}

		#endregion

		#endregion

		#region Inherited Methodos
		#endregion

		#region Static Properties
		#endregion

		#region Static Methodos
		#endregion

		#region ICommandSource Members

		/// <summary>
		/// Manages a response for a command sent by the CommandManager module
		/// Since responses has been managed at this point, this method does nothing
		/// </summary>
		/// <param name="response">Response to discard</param>
		public void ReceiveResponse(Response response)
		{
			return;
		}

		#endregion
	}
}
