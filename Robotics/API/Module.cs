using System;
using System.Collections.Generic;
using System.Threading;

namespace Robotics.API
{
	/// <summary>
	/// Encapsulates a CommandManager, ConnectionManager and other objects required
	/// to create a module for connection to Blackboard. This class is designed for 
	/// COM Interop
	/// </summary>
	public class Module : IService
	{
		#region Variables

		/// <summary>
		/// The command manager
		/// </summary>
		private ConnectionManager cnnMan;
		/// <summary>
		/// The connection manager
		/// </summary>
		private CommandManager cmdMan;
		/// <summary>
		/// Semaphore used to wait for the first client connected
		/// </summary>
		private AutoResetEvent clientConnectedEvent;
		/// <summary>
		/// Semaphore used to wait for the SharedVariablesLoaded event
		/// </summary>
		private ManualResetEvent sharedVarsLodadedEvent;
		/// <summary>
		/// Semaphore used to wait for the command manager to stop (Run method)
		/// </summary>
		private ManualResetEvent cmdManStoppedEvent;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Module
		/// </summary>
		public Module() : this(new CommandManager(), new ConnectionManager()) { }

		/// <summary>
		/// Initializes a new instance of Module
		/// <param name="port">Connection port of the module</param>
		/// </summary>
		public Module(int port) : this(new CommandManager(), new ConnectionManager(port)) { }

		/// <summary>
		/// Initializes a new instance of Module
		/// <param name="moduleName">The name of the module</param>
		/// <param name="port">Connection port of the module</param>
		/// </summary>
		public Module(string moduleName, int port) : this(new CommandManager(), new ConnectionManager(moduleName, port)) { }

		/// <summary>
		/// Initializes a new instance of Module
		/// <param name="commandManager">A command manager object used to manage commands</param>
		/// <param name="connectionManager">A connection manager object used to connect to Blackboard</param>
		/// </summary>
		public Module(CommandManager commandManager, ConnectionManager connectionManager)
		{
			if ((commandManager == null) || (connectionManager == null))
				throw new ArgumentNullException();
			this.cmdMan = commandManager;
			this.cnnMan = connectionManager;
			this.cnnMan.CommandManager = commandManager;
			this.cnnMan.ClientConnected += new System.Net.Sockets.TcpClientConnectedEventHandler(OnClientConnected);
			this.cmdMan.SharedVariablesLoaded += new SharedVariablesLoadedEventHandler(OnSharedVariablesLoaded);
			this.cmdMan.Started+=new CommandManagerStatusChangedEventHandler(OnCommandManagerStarted);
			this.cmdMan.Stopped += new CommandManagerStatusChangedEventHandler(OnCommandManagerStopped);
			this.clientConnectedEvent = new AutoResetEvent(false);
			this.sharedVarsLodadedEvent = new ManualResetEvent(false);
			this.cmdManStoppedEvent = new ManualResetEvent(false);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the busy state of the command manager
		/// </summary>
		public virtual bool Busy
		{
			get { return this.cmdMan.Busy; }
			set { this.cmdMan.Busy = value; }
		}

		/// <summary>
		/// Gets the ConnectionManager object that the module uses to manage connections
		/// </summary>
		public ConnectionManager ConnectionManager { get { return this.cnnMan; } }

		/// <summary>
		/// Gets the CommandManager object that the module uses to manage and execute commands
		/// </summary>
		public CommandManager CommandManager { get { return this.cmdMan; } }

		/// <summary>
		/// Gets a value indicating if the module is running
		/// </summary>
		public bool IsRunning { get { return this.cnnMan.IsRunning && this.cmdMan.IsRunning; } }

		/// <summary>
		/// Gets or sets the ready state of the command manager
		/// </summary>
		public virtual bool Ready
		{
			get { return this.cmdMan.Ready; }
			set { this.cmdMan.Ready = value; }
		}

		#endregion

		#region Methodos
		
		/// <summary>
		/// Adds a SharedVariable to the CommandManager of the module.
		/// If the shared variable is already registered, the reference is updated, otherwise a new shared variable of the type is created
		/// </summary>
		/// <param name="sharedVariable">A shared variable object used to create the new SharedVariable or update the reference</param>
		public void AddSharedVariable<T>(ref T sharedVariable) where T: SharedVariable
		{
			this.cmdMan.AddSharedVariable<T>(ref sharedVariable);
		}

		/// <summary>
		/// Adds a SharedVariable to the CommandManager of the module.
		/// If the shared variable is already registered, the reference is updated, otherwise a new shared variable of the type is created
		/// </summary>
		/// <param name="sharedVariable">A shared variable object used to create the new SharedVariable or update the reference</param>
		/// <param name="reportType">The type of report required for subscription</param>
		/// <param name="subscriptionType">The type of subscription</param>
		/// <param name="updateEventHandler">A delegate that represents the method that will handle the Updated event of the shared variable</param>
		public void AddSharedVariable<T>(ref T sharedVariable, SharedVariableReportType reportType, SharedVariableSubscriptionType subscriptionType, SharedVariableUpdatedEventHadler updateEventHandler) where T : SharedVariable
		{
			this.cmdMan.AddSharedVariable(ref sharedVariable, reportType, subscriptionType, updateEventHandler);
		}

		/// <summary>
		/// Handles the ClientConnected event of the connection manager to unlock the WaitForClientToConnect() method
		/// </summary>
		/// <param name="s">The connection socket. Unused.</param>
		protected virtual void OnClientConnected(System.Net.Sockets.Socket s)
		{
			if (this.cnnMan.ConnectedClientsCount > 0)
				this.clientConnectedEvent.Set();
		}

		/// <summary>
		/// Handles the Started event of the command manager to lock/unlock the Run() method
		/// </summary>
		/// <param name="commandManager">The command manager which rises the event</param>
		protected virtual void OnCommandManagerStarted(CommandManager commandManager)
		{
			if (commandManager != this.cmdMan) return;
			this.cmdManStoppedEvent.Reset();
		}

		/// <summary>
		/// Handles the Stopped event of the command manager to lock/unlock the Run() method
		/// </summary>
		/// <param name="commandManager">The command manager which rises the event</param>
		protected virtual void OnCommandManagerStopped(CommandManager commandManager)
		{
			if (commandManager != this.cmdMan) return;
			this.cmdManStoppedEvent.Set();
		}

		/// <summary>
		/// Handles the SharedVariablesLoaded event of the command manager to unlock the WaitSharedVariablesLoaded() method
		/// </summary>
		/// <param name="cmdMan">The command manager which rises the event</param>
		protected virtual void OnSharedVariablesLoaded(CommandManager cmdMan)
		{
			if (cmdMan != this.cmdMan) return;
			this.sharedVarsLodadedEvent.Set();
		}

		/// <summary>
		/// Blocks the Thread untill the Command Manager stops
		/// </summary>
		public void Run()
		{
			this.cmdManStoppedEvent.WaitOne();
		}

		/// <summary>
		/// Sends a command and waits for response
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <param name="timeOut">The timeout for command execution</param>
		/// <param name="response">The response received</param>
		/// <returns>true if command was sent and its response received. false otherwise</returns>
		public bool SendAndWait(Command command, int timeOut, out Response response)
		{
			return this.cmdMan.SendAndWait(command, timeOut, out response);
		}

		/// <summary>
		/// When overriden in a derived class, this method should load all the command executers to the CommandManager.
		/// This methos is called by the Start once method before starting the command manager.
		/// </summary>
		protected virtual void SetupCommandExecuters()
		{
		}

		/// <summary>
		/// Starts the module engine
		/// </summary>
		/// <returns>Zero if the method or function was started successfully, otherwise it returns the error number</returns>
		public void Start()
		{
			this.cnnMan.Start();
			this.SetupCommandExecuters();
			this.cmdMan.Start();
			this.WaitForClientToConnect();
			this.cmdMan.SharedVariables.LoadFromBlackboard();
			this.WaitSharedVariablesLoaded();
			if((this.cnnMan.ConnectedClientsCount > 0) && (this.cmdMan.CommandExecuters.Count > 0))
				this.cmdMan.Ready = true;
		}

		/// <summary>
		/// Stops the module engine
		/// </summary>
		/// <returns>Zero if the method or function was stopped successfully, otherwise it returns the error number</returns>
		public void Stop()
		{
			this.cmdMan.Stop();
			this.cnnMan.Stop();
		}

		/// <summary>
		/// Waits until a client connects to the TCP Server of the ConnectionManager
		/// </summary>
		public void WaitForClientToConnect()
		{
			if(cnnMan.ConnectedClientsCount < 1)
				this.clientConnectedEvent.WaitOne();
		}

		/// <summary>
		/// Waits until a client connects to the TCP Server of the ConnectionManager
		/// </summary>
		/// <param name="timeout">The maximum amount of time to wait for the first client to get connected</param>
		/// <returns>true if a client connected before the timeout occurs, false otherwise</returns>
		public bool WaitForClientToConnect(int timeout)
		{
			if (cnnMan.ConnectedClientsCount < 1)
				return this.clientConnectedEvent.WaitOne(timeout);
			return true;
		}

		/// <summary>
		/// Waits until the shared variables have been loaded by the Command Manager
		/// </summary>
		public void WaitSharedVariablesLoaded()
		{
			this.sharedVarsLodadedEvent.WaitOne();
		}

		/// <summary>
		/// Waits until the shared variables have been loaded by the Command Manager
		/// </summary>
		/// <param name="timeout">The maximum amount of time to wait for the Shared Variables to be loaded</param>
		/// <returns>true if the shared variables were loaded before the timeout occurs, false otherwise</returns>
		public bool WaitSharedVariablesLoaded(int timeout)
		{
			return this.sharedVarsLodadedEvent.WaitOne(timeout);
		}

		#endregion
	}
}
