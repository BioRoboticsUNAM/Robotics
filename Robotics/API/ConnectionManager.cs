using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Robotics;
using Robotics.Sockets;

namespace Robotics.API
{
	/// <summary>
	/// Manages TCP connections.
	/// </summary>
	public class ConnectionManager : IService, IConnectionManager
	{
		private const int DEFAULT_PORT = 2000;
		private const string DEFAULT_MODULE_NAME = "MODULE";

		#region Variables

		/// <summary>
		/// Tcp Socket Server for input data
		/// </summary>
		protected readonly TcpServer tcpServer;
		/// <summary>
		/// Async thread timer for socket autoconnections
		/// </summary>
		protected Thread connectionThread;
		/// <summary>
		/// Represents the BidirectionalConnectionTask method
		/// </summary>
		protected readonly ThreadStart dlgConnectionThreadTask;
		/// <summary>
		/// Indicates if connection manager is running
		/// </summary>
		protected bool running;
		/// <summary>
		/// Flag that indicates if must restart server and reconnect client if disconnected
		/// </summary>
		protected bool autoReconnect;
		/// <summary>
		/// Stores the module name
		/// </summary>
		protected string moduleName;
		/// <summary>
		/// Command manager used to parse received data
		/// </summary>
		protected CommandManager cmdMan;
		/// <summary>
		/// Object used to lock start and stop methods and prevent undeterminate
		/// conditions by stoping the server while the connection thread is starting it
		/// </summary>
		private readonly object startLock;
		/// <summary>
		/// Parser for incomming Tcp packets
		/// </summary>
		private readonly TcpPacketParser parser;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of ConnectionManager. 
		/// </summary>
		public ConnectionManager()
			: this(DEFAULT_MODULE_NAME, DEFAULT_PORT, null) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager. 
		/// </summary>
		/// <param name="port">The I/O port for the Tcp Server</param>
		public ConnectionManager(int port)
			: this(DEFAULT_MODULE_NAME, port, null) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager. 
		/// </summary>
		/// <param name="port">The I/O port for the Tcp Server</param>
		/// <param name="commandManager">The CommandManager object which will manage incoming data</param>
		public ConnectionManager(int port, CommandManager commandManager)
			: this(DEFAULT_MODULE_NAME, port, commandManager) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager. 
		/// </summary>
		/// <param name="moduleName">The name of the module this object will manage</param>
		public ConnectionManager(string moduleName)
			: this(moduleName, DEFAULT_PORT, null) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager. 
		/// </summary>
		/// <param name="moduleName">The name of the module this object will manage</param>
		/// <param name="port">The I/O port for the Tcp Server</param>
		public ConnectionManager(string moduleName, int port)
			: this(moduleName, port, null) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager. 
		/// </summary>
		/// <param name="moduleName">The name of the module this object will manage</param>
		/// <param name="port">The I/O port for the Tcp Server</param>
		/// <param name="commandManager">The CommandManager object which will manage incoming data</param>
		public ConnectionManager(string moduleName, int port, CommandManager commandManager)
		{
			this.moduleName = moduleName;
			this.CommandManager = commandManager;
			this.dlgConnectionThreadTask = new ThreadStart(ConnectionThreadTask);
			this.tcpServer = new TcpServer(port);
			this.tcpServer.DataReceived += new EventHandler<TcpServer, TcpPacket>(TcpServer_DataReceived);
			this.tcpServer.ClientConnected += new EventHandler<TcpServer, IPEndPoint>(TcpServer_ClientConnected);
			this.tcpServer.ClientDisconnected += new EventHandler<TcpServer, IPEndPoint>(TcpServer_ClientDisconnected);
			this.parser = new TcpPacketParser(this);
			this.startLock = new Object();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the connector is connected to the message source
		/// </summary>
		bool IConnector.IsConnected { get { return ConnectedClientsCount > 0; } }

		/// <summary>
		/// Gets the number of clients connected to the local TCP Server
		/// </summary>
		public int ConnectedClientsCount
		{
			get { return tcpServer.ClientsConnected; }
		}

		/// <summary>
		/// Gets or sets the CommandManager object which will manage incoming data.
		/// The ConnectionManager must not be running when the CommandManager is set.
		/// </summary>
		public CommandManager CommandManager
		{
			get { return cmdMan; }
			internal set
			{
				//if() throw new Exception();
				//if() throw new ArgumentNullException();
				cmdMan = value;
				if ((cmdMan != null) && (cmdMan.ConnectionManager != this))
					cmdMan.ConnectionManager = this;
			}
		}

		/// <summary>
		/// Gets a value indicating if this instance of ConnectionManager has been started
		/// </summary>
		public bool IsRunning { get { return running; } }

		/// <summary>
		/// Gets or sets the name of the module that this ConnectionManager object interfaces.
		/// </summary>
		public string ModuleName
		{
			get { return moduleName; }
			set
			{
				if (IsRunning)
					throw new Exception("Cannot change the name of the module while the ConnectionManager is running");
				if (!IsValidModuleName(value))
					throw new ArgumentException("Invalid module name");
				moduleName = value;
			}
		}

		/// <summary>
		/// Gets or sets the Tcp port for incoming data used by Tcp Server.
		/// </summary>
		public int Port
		{
			get { return this.tcpServer.Port; }
			set
			{
				if (running) throw new Exception("Can not change the PortIn while the ConnectionManager is running");
				lock (tcpServer)
				{
					tcpServer.Port = value;
				}
			}
		}

		/// <summary>
		/// Gets the TCP Server
		/// </summary>
		internal TcpServer TcpServer { get { return this.tcpServer; } }

		#endregion

		#region Events
		/// <summary>
		/// Occurs when a remote client gets connected to local TCP Server
		/// </summary>
		public event EventHandler<IConnectionManager, IPEndPoint> ClientConnected;
		/// <summary>
		/// Occurs when a remote client disconnects from local TCP Server
		/// </summary>
		public event EventHandler<IConnectionManager, IPEndPoint> ClientDisconnected;
		/// <summary>
		/// Occurs when data is received
		/// </summary>
		public event EventHandler<IConnectionManager, TcpPacket> DataReceived;
		/// <summary>
		/// Occurs when the status of the ConnectionManager changes
		/// </summary>
		public event Action<IConnectionManager> StatusChanged;

		/// <summary>
		/// Occurs when the ConnectionManager is started
		/// </summary>
		public event Action<IConnectionManager> Started;

		/// <summary>
		/// Occurs when the ConnectionManager is stopped
		/// </summary>
		public event Action<IConnectionManager> Stopped;

		/// <summary>
		/// Occurs when a command is sent
		/// </summary>
		public event EventHandler<IConnectionManager, Command> CommandSent;

		/// <summary>
		/// Occurs when a response is sent
		/// </summary>
		public event EventHandler<IConnectionManager, Response> ResponseSent;

		/// <summary>
		/// Occurs when a command is received
		/// </summary>
		public event EventHandler<IConnector, Command> CommandReceived;

		/// <summary>
		/// Occurs when the connector gets connected to the message source
		/// </summary>
		public event Action<IConnector> Connected;

		/// <summary>
		/// Occurs when the connector gets disconnected from the message source
		/// </summary>
		public event Action<IConnector> Disconnected;

		/// <summary>
		/// Occurs when a response is received
		/// </summary>
		public event EventHandler<IConnector, Response> ResponseReceived;

		#endregion

		#region Methods

		/// <summary>
		/// Raises the ClientConnected event
		/// </summary>
		/// <param name="ep">Socket used for connection</param>
		/// <remarks>The OnClientConnected method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnClientConnected in a derived class, be sure to call the base class's OnClientConnected method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnClientConnected(IPEndPoint ep)
		{
			try
			{
				if (this.ClientConnected != null)
					ClientConnected(this, ep);
			}
			catch { }
		}

		/// <summary>
		/// Raises the ClientDisconnected event
		/// </summary>
		/// <param name="ep">Disconnection endpoint</param>
		/// <remarks>The OnClientDisconnected method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnClientDisconnected in a derived class, be sure to call the base class's OnClientDisconnected method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnClientDisconnected(IPEndPoint ep)
		{
			try
			{
				if (this.ClientDisconnected != null)
					ClientDisconnected(this, ep);
			}
			catch { }
		}

		/// <summary>
		/// Raises the DataReceived event
		/// </summary>
		/// <param name="p">Received data</param>
		/// <remarks>The OnDataReceived method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnDataReceived in a derived class, be sure to call the base class's OnDataReceived method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnDataReceived(TcpPacket p)
		{
			try
			{
				if (DataReceived != null)
					DataReceived(this, p);
			}
			catch { }
		}

		/// <summary>
		/// Raises the CommandSent event
		/// </summary>
		/// <param name="command">The sent command</param>
		protected virtual void OnCommandSent(Command command)
		{
			try
			{
				if (CommandSent != null)
					CommandSent(this, command);
			}
			catch { }
		}

		/// <summary>
		/// Raises the CommandReceived event
		/// </summary>
		/// <param name="command">The received command</param>
		protected internal virtual void OnCommandReceived(Command command)
		{
			try
			{
				if (CommandReceived != null)
					CommandReceived(this, command);
			}
			catch { }
		}
		
		/// <summary>
		/// Raises the Connected event
		/// </summary>
		protected virtual void OnConnected()
		{
			try
			{
				if (Connected != null)
					Connected(this);
			}
			catch { }
		}

		/// <summary>
		/// Raises the Connected event
		/// </summary>
		protected virtual void OnDisconnected()
		{
			try
			{
				if (Disconnected != null)
					Disconnected(this);
			}
			catch { }
		}

		/// <summary>
		/// Raises the ResponseSent event
		/// </summary>
		/// <param name="response">The sent response</param>
		protected virtual void OnResponseSent(Response response)
		{
			try
			{
				if (ResponseSent != null)
					ResponseSent(this, response);
			}
			catch { }
		}

		/// <summary>
		/// Raises the ResponseReceived event
		/// </summary>
		/// <param name="response">The received response</param>
		protected internal virtual void OnResponseReceived(Response response)
		{
			try
			{
				if (ResponseReceived != null)
					ResponseReceived(this, response);
			}
			catch { }
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
		/// Manages the Response objects for the Command objects generated by this IMessageSource object
		/// </summary>
		/// <param name="response">The Response for the Command generated by this IMessageSource object</param>
		public void ReceiveResponse(Response response)
		{
			if (response.MessageSource != this)
				return;
			Send(response);
		}

		/// <summary>
		/// Starts the Connection Manager
		/// </summary>
		public void Start()
		{
			lock (startLock)
			{
				if (running) return;
				this.running = true;
				lock (startLock)
				{
					try
					{
						tcpServer.Start();
					}
					catch {}
				}
				if(!tcpServer.Started)
					StartConnectionThread();
				OnStart();
				OnStatusChanged();
			}
		}

		/// <summary>
		/// Stops the Connection Manager
		/// </summary>
		public void Stop()
		{
			lock (startLock)
			{
				if(!running) return;
				this.running = false;
				if ((this.connectionThread != null) && this.connectionThread.IsAlive)
					this.connectionThread.Join();
				this.connectionThread = null;
				this.tcpServer.Stop();
				this.parser.Stop();
				OnStop();
				OnStatusChanged();
			}
		}

		#region Connection Thread Methods

		/// <summary>
		/// Starts the connection thread
		/// </summary>
		private void StartConnectionThread()
		{
			this.connectionThread = new Thread(new ThreadStart(dlgConnectionThreadTask));
			this.connectionThread.IsBackground = true;
			connectionThread.Start();
		}

		/// <summary>
		/// Starts the TCP server asynchronously
		/// </summary>
		protected void ConnectionThreadTask()
		{
			if (!running) return;

			while (running && !tcpServer.Started)
			{
				bool nap = false;
				lock (startLock)
				{
					if (!running) return;
					try
					{
						tcpServer.Start();
					}
					catch { nap = true; }
				}
				if(nap)
					Thread.Sleep(500);
			}

			OnStatusChanged();
		}

		#endregion

		#region Socket Methods

		/// <summary>
		/// Sends a Command
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if command was sent successfully, false otherwise</returns>
		public bool Send(Command command)
		{
			bool result;
			result = Send(command.StringToSend);
			if (result)
				OnCommandSent(command);
			return result;
		}

		/// <summary>
		/// Sends a Response
		/// </summary>
		/// <param name="response">Response to be sent</param>
		/// <returns>true if response was sent successfully, false otherwise</returns>
		public bool Send(Response response)
		{
			IPEndPoint ep;
			bool result;
			if ((response.MessageSource != this) && (response.MessageSource != this.CommandManager))
				return false;
			if (((ep = response.MessageSourceMetadata as IPEndPoint) != null) && tcpServer.IsConnected(ep))
				result = Send(response.StringToSend, ep);
			else
				result = Send(response.StringToSend);
			if (result)
				OnResponseSent(response);
			return result;
		}

		/// <summary>
		/// Sends data through server socket to specified endpoint
		/// </summary>
		/// <param name="s">String to send</param>\
		/// <param name="endPoint">Destination endpoint to snd data to</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		protected virtual bool Send(string s, IPEndPoint endPoint)
		{
			return tcpServer.SendTo(endPoint, s);
		}

		/// <summary>
		/// Sends data through server socket
		/// </summary>
		/// <param name="s">String to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		protected virtual bool Send(string s)
		{
			return tcpServer.SendToAll(s) > 0;
		}

		#endregion

		#region TCP Server events

		/// <summary>
		/// Manages the ClientConnected event of the input socket
		/// </summary>
		/// <param name="s">The TcpServer object which raises the event</param>
		/// <param name="ep">The Remote endpoint of the client</param>
		private void TcpServer_ClientConnected(TcpServer s, IPEndPoint ep)
		{
			OnClientConnected(ep);
			if (s.ClientsConnected == 1)
				OnConnected();
		}

		/// <summary>
		/// Manages the ClientDisconnected event of the input socket
		/// </summary>
		/// <param name="s">The TcpServer object which raises the event</param>
		/// <param name="ep">Disconnection endpoint</param>
		private void TcpServer_ClientDisconnected(TcpServer s, IPEndPoint ep)
		{
			parser.Stop(ep);
			OnClientDisconnected(ep);
			if (s.ClientsConnected < 1)
				OnDisconnected();
		}

		/// <summary>
		/// Manages the DataReceived event of the input socket
		/// </summary>
		/// <param name="s">The TcpServer object which raises the event</param>
		/// <param name="p">Received data</param>
		private void TcpServer_DataReceived(TcpServer s, TcpPacket p)
		{
			parser.Enqueue(p);
			OnDataReceived(p);
		}

		#endregion

		#endregion

		#region Console Methods

		#endregion

		#region Static Variables

		/// <summary>
		/// Regular expresion used to validate module names
		/// </summary>
		private static Regex rxModuleNameValidator = new Regex(@"^[A-Z][0-9A-Z\-]+[0-9A-Z]$", RegexOptions.Compiled);

		#endregion

		#region Static Methods

		/// <summary>
		/// Gets a value indicating if the provided string is valid as a name of a module
		/// </summary>
		/// <param name="name">string to validate</param>
		/// <returns>true if the provided string is a valid module name, false otherwise</returns>
		public static bool IsValidModuleName(string name)
		{
			return rxModuleNameValidator.IsMatch(name);
		}

		#endregion
	}
}