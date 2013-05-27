using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Robotics;

namespace Robotics.API
{
	/// <summary>
	/// Manages TCP connections.
	/// </summary>
	public class ConnectionManager : IConnectionManager
	{
		private const string DEFAULT_MODULE_NAME = "MODULE";
		/// <summary>
		/// The default buffer size for sockets
		/// </summary>
		public const int DEFAULT_BUFFER_SIZE = 16384;

		#region Variables

		/// <summary>
		/// Tcp Socket Server for input data
		/// </summary>
		protected SocketTcpServer tcpServer;
		/// <summary>
		/// Tcp Socket client for output data
		/// </summary>
		protected SocketTcpClient tcpClient;
		/// <summary>
		/// IP Address of the remote computer to connect using the socket client
		/// </summary>
		protected IPAddress remoteServerAddress;
		/// <summary>
		/// Port for incoming data used by Tcp Server
		/// </summary>
		protected int portIn;
		/// <summary>
		/// Port for outgoing data used by Tcp Client
		/// </summary>
		protected int portOut;
		/// <summary>
		/// Async thread timer for socket autoconnections
		/// </summary>
		protected Thread connectionThread;
		/// <summary>
		/// Represents the BidirectionalConnectionTask method
		/// </summary>
		protected ThreadStart dlgBidirectionalConnectionTask;
		/// <summary>
		/// Represents the UnidirectionalConnectionTask method
		/// </summary>
		protected ThreadStart dlgUnidirectionalConnectionTask;
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
		/// Stores the last received packet
		/// </summary>
		protected TcpPacket lastReceivedPacket;
		/// <summary>
		/// Enables or disables the reception of data from the output socket
		/// </summary>
		public bool outputSocketReceptionEnabled;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of ConnectionManager in bidirectional mode. 
		/// </summary>
		public ConnectionManager()
			: this(DEFAULT_MODULE_NAME) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager in bidirectional mode. 
		/// </summary>
		/// <param name="port">The I/O port for the Tcp Server</param>
		public ConnectionManager(int port)
			: this(DEFAULT_MODULE_NAME, port, port, IPAddress.Parse("127.0.0.1"), null) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager in bidirectional mode. 
		/// </summary>
		/// <param name="port">The I/O port for the Tcp Server</param>
		/// <param name="commandManager">The CommandManager object which will manage incoming data</param>
		public ConnectionManager(int port, CommandManager commandManager)
			: this(DEFAULT_MODULE_NAME, port, port, IPAddress.Parse("127.0.0.1"), commandManager) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager in unidirectional mode. 
		/// </summary>
		/// <param name="portIn">The input port for the Tcp Server</param>
		/// <param name="portOut">The output port for the Tcp client</param>
		/// <param name="remoteServerAddress">The IP Address of the remote tcp server where the client will connect to</param>
		public ConnectionManager(int portIn, int portOut, IPAddress remoteServerAddress)
			: this(DEFAULT_MODULE_NAME, portIn, portOut, remoteServerAddress, null) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager in unidirectional mode. 
		/// </summary>
		/// <param name="portIn">The input port for the Tcp Server</param>
		/// <param name="portOut">The output port for the Tcp client</param>
		/// <param name="remoteServerAddress">The IP Address of the remote tcp server where the client will connect to</param>
		/// <param name="commandManager">The CommandManager object which will manage incoming data</param>
		public ConnectionManager(int portIn, int portOut, IPAddress remoteServerAddress, CommandManager commandManager)
			: this(DEFAULT_MODULE_NAME, portIn, portOut, remoteServerAddress, commandManager) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager in bidirectional mode. 
		/// </summary>
		/// <param name="moduleName">The name of the module this object will manage</param>
		public ConnectionManager(string moduleName)
			: this(moduleName, 2000, 2000, IPAddress.Parse("127.0.0.1"), null)
		{
			this.portIn = 0;
			this.portOut = 0;
		}

		/// <summary>
		/// Initializes a new instance of ConnectionManager in bidirectional mode. 
		/// </summary>
		/// <param name="moduleName">The name of the module this object will manage</param>
		/// <param name="port">The I/O port for the Tcp Server</param>
		public ConnectionManager(string moduleName, int port)
			: this(moduleName, port, port, IPAddress.Parse("127.0.0.1"), null) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager in bidirectional mode. 
		/// </summary>
		/// <param name="moduleName">The name of the module this object will manage</param>
		/// <param name="port">The I/O port for the Tcp Server</param>
		/// <param name="commandManager">The CommandManager object which will manage incoming data</param>
		public ConnectionManager(string moduleName, int port, CommandManager commandManager)
			: this(moduleName, port, port, IPAddress.Parse("127.0.0.1"), commandManager) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager in unidirectional mode. 
		/// </summary>
		/// <param name="moduleName">The name of the module this object will manage</param>
		/// <param name="portIn">The input port for the Tcp Server</param>
		/// <param name="portOut">The output port for the Tcp client</param>
		/// <param name="remoteServerAddress">The IP Address of the remote tcp server where the client will connect to</param>
		public ConnectionManager(string moduleName, int portIn, int portOut, IPAddress remoteServerAddress)
			: this(moduleName, portIn, portOut, remoteServerAddress, null) { }

		/// <summary>
		/// Initializes a new instance of ConnectionManager in unidirectional mode. 
		/// </summary>
		/// <param name="moduleName">The name of the module this object will manage</param>
		/// <param name="portIn">The input port for the Tcp Server</param>
		/// <param name="portOut">The output port for the Tcp client</param>
		/// <param name="remoteServerAddress">The IP Address of the remote tcp server where the client will connect to</param>
		/// <param name="commandManager">The CommandManager object which will manage incoming data</param>
		public ConnectionManager(string moduleName, int portIn, int portOut, IPAddress remoteServerAddress, CommandManager commandManager)
		{
			this.moduleName = moduleName;
			this.PortIn = portIn;
			this.PortOut = portOut;
			this.TcpServerAddress = remoteServerAddress;
			this.CommandManager = commandManager;
			this.dlgBidirectionalConnectionTask = new ThreadStart(BidirectionalConnectionTask);
			this.dlgUnidirectionalConnectionTask = new ThreadStart(UnidirectionalConnectionTask);
			ConfigureSockets();
			ConfigureConnectionThread();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the ConnectionManager is woking in bidirectional mode
		/// </summary>
		public bool Bidirectional
		{
			get { return PortOut == PortIn; }
		}

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
				if((cmdMan!= null) && (cmdMan.ConnectionManager != this))
					cmdMan.ConnectionManager = this;
			}
		}

		/// <summary>
		/// Enables or disables the reception of data from the output socket
		/// </summary>
		public bool OutputSocketReceptionEnabled
		{
			get { return outputSocketReceptionEnabled; }
			set
			{
				//if (outputSocketReceptionEnabled == value) return;
				//if (running) throw new Exception("Can not change the value while the ConnectionManager is running");
				outputSocketReceptionEnabled = value;
			}
		}

		/// <summary>
		/// Gets a value indicating if local TCP client has connected to remote server
		/// </summary>
		/// <remarks>When working on bidirectional mode always return false</remarks>
		public bool IsConnected
		{
			get {
				if ((tcpClient == null) || Bidirectional)
					return false;
				else
					return tcpClient.IsConnected;
			}
		}

		/// <summary>
		/// Gets a value indicating if TCP Server has been started and is running
		/// </summary>
		public bool IsServerStarted
		{
			get {
				return tcpServer.Started;
			}
		}

		/// <summary>
		/// Gets a value indicating if this instance of ConnectionManager has been started
		/// </summary>
		public bool IsRunning
		{
			get { return running; }
		}

		/// <summary>
		/// Geths the last received packet
		/// </summary>
		protected TcpPacket LastReceivedPacket
		{
			get { return lastReceivedPacket; }
		}

		/// <summary>
		/// Gets a value indicating the mode of the current ConnectionManager instance
		/// </summary>
		public ConnectionManagerMode Mode
		{
			get { return PortOut == PortIn ? ConnectionManagerMode.Bidireactional : ConnectionManagerMode.Unidirectional; }
		}

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
		public int PortIn
		{
			get { return this.portIn; }
			set
			{
				if (portIn == value) return;
				if (running) throw new Exception("Can not change the PortIn while the ConnectionManager is running");
				if ((value < 1) || (value > 65535))
					throw new ArgumentOutOfRangeException("value");
				this.portIn = value;
				if(tcpServer != null)
					tcpServer.Port = this.portIn;
			}
		}

		/// <summary>
		/// Gets or sets the Tcp port for outgoing data used by Tcp Client.
		/// </summary>
		public int PortOut
		{
			get { return this.portOut; }
			set
			{
				if (portOut == value) return;
				if (running) throw new Exception("Can not change the PortOut while the ConnectionManager is running");
				if ((value < 1) || (value > 65535))
					throw new ArgumentOutOfRangeException("value");
				this.portOut = value;
				if (tcpClient != null)
					tcpClient.Port = this.portOut;
			}
		}

		/// <summary>
		/// Gets or sets the IP Address of the remote computer to connect using the socket client.
		/// </summary>
		public IPAddress TcpServerAddress
		{
			get { return this.remoteServerAddress; }
			set
			{
				if (remoteServerAddress == value) return;
				if (running) throw new Exception("Can not change the TcpServerAddress while the ConnectionManager is running");
				this.remoteServerAddress= value;
				if (tcpClient != null)
					tcpClient.ServerAddress = this.remoteServerAddress;
			}
		}

		#endregion

		#region Events
		/// <summary>
		/// Occurs when a remote client gets connected to local TCP Server
		/// </summary>
		public event TcpClientConnectedEventHandler ClientConnected;
		/// <summary>
		/// Occurs when a remote client disconnects from local TCP Server
		/// </summary>
		public event TcpClientDisconnectedEventHandler ClientDisconnected;
		/// <summary>
		/// Occurs when the local client connects to remote server.
		/// This event is rised only when the ConnectionManager works in Unidirectional mode.
		/// </summary>
		public event TcpClientConnectedEventHandler Connected;
		/// <summary>
		/// Occurs when the local client connects to remote server.
		/// This event is rised only when the ConnectionManager works in Unidirectional mode.
		/// </summary>
		public event TcpClientDisconnectedEventHandler Disconnected;
		/// <summary>
		/// Occurs when data is received
		/// </summary>
		public event ConnectionManagerDataReceivedEH DataReceived;
		/// <summary>
		/// Occurs when the status of the ConnectionManager changes
		/// </summary>
		public event ConnectionManagerStatusChangedEventHandler StatusChanged;

		/// <summary>
		/// Occurs when the ConnectionManager is started
		/// </summary>
		public event ConnectionManagerStatusChangedEventHandler Started;

		/// <summary>
		/// Occurs when the ConnectionManager is stopped
		/// </summary>
		public event ConnectionManagerStatusChangedEventHandler Stopped;

		/// <summary>
		/// Occurs when a command is sent
		/// </summary>
		public event CnnManCommandSentEventHandler CommandSent;

		/// <summary>
		/// Occurs when a response is sent
		/// </summary>
		public event CnnManResponseSentEventHandler ResponseSent;

		#endregion

		#region Methods

		/// <summary>
		/// Raises the ClientConnected event
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		/// <remarks>The OnClientConnected method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnClientConnected in a derived class, be sure to call the base class's OnClientConnected method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnClientConnected(Socket s)
		{
			if (this.ClientConnected != null)
				ClientConnected(s);
		}

		/// <summary>
		/// Raises the ClientDisconnected event
		/// </summary>
		/// <param name="ep">Disconnection endpoint</param>
		/// <remarks>The OnClientDisconnected method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnClientDisconnected in a derived class, be sure to call the base class's OnClientDisconnected method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnClientDisconnected(EndPoint ep)
		{
			if (this.ClientDisconnected != null)
				ClientDisconnected(ep);
		}

		/// <summary>
		/// Raises the Connected event
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		/// <remarks>The OnConnected method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnConnected in a derived class, be sure to call the base class's OnConnected method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnConnected(Socket s)
		{
			if (Connected != null)
				Connected(s);
		}

		/// <summary>
		/// Raises the CommandSent event
		/// </summary>
		/// <param name="command">The sent command</param>
		protected virtual void OnCommandSent(Command command)
		{
			if (CommandSent != null)
				CommandSent(this, command);
		}

		/// <summary>
		/// Raises the ResponseSent event
		/// </summary>
		/// <param name="response">The sent response</param>
		protected virtual void OnResponseSent(Response response)
		{
			if (ResponseSent != null)
				ResponseSent(this, response);
		}

		/// <summary>
		/// Raises the Disconnected event
		/// </summary>
		/// <param name="ep">Disconnection endpoint</param>
		/// <remarks>The OnDisconnected method also allows derived classes to handle the event without attaching a delegate.
		/// This is the preferred technique for handling the event in a derived class.
		/// When overriding OnDisconnected in a derived class, be sure to call the base class's OnDisconnected method so that registered
		/// delegates receive the event</remarks>
		protected virtual void OnDisconnected(EndPoint ep)
		{
			if (Disconnected != null)
				Disconnected(ep);
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
			if (DataReceived != null)
				DataReceived(this, p);
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
			if (StatusChanged != null)
				StatusChanged(this);
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
			if (Started != null)
				Started(this);
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
			if (Stopped != null)
				Stopped(this);
		}

		/// <summary>
		/// Starts the Connection Manager
		/// </summary>
		public void Start()
		{
			autoReconnect = true;
			running = true;
			if (portIn == 0)
				return;
			if (!Bidirectional && (portOut == 0))
				return;
			OnStart();
			OnStatusChanged();
			StartConnectionThread();
		}

		/// <summary>
		/// Stops the Connection Manager
		/// </summary>
		public void Stop()
		{
			autoReconnect = false;
			running = false;
			OnStop();
			OnStatusChanged();
			TerminateSockets();
		}

		#region Thread Methods

		/// <summary>
		/// Configures the connection thread
		/// </summary>
		private void ConfigureConnectionThread()
		{
			if(Bidirectional)
				connectionThread = new Thread(new ThreadStart(dlgBidirectionalConnectionTask));
			else
				connectionThread = new Thread(new ThreadStart(dlgUnidirectionalConnectionTask));
			connectionThread.IsBackground = true;
			connectionThread.Priority = ThreadPriority.BelowNormal;
		}

		/// <summary>
		/// Starts the connection thread
		/// </summary>
		private void StartConnectionThread()
		{
			if((connectionThread != null) && connectionThread.IsAlive) 
				return;

			if ((connectionThread == null) || (connectionThread.ThreadState != ThreadState.Unstarted))
				ConfigureConnectionThread();

			connectionThread.Start();
		}

		/// <summary>
		/// Starts the TCP server asynchronously
		/// </summary>
		protected void BidirectionalConnectionTask()
		{
			if (!running || !autoReconnect) return;

			if (tcpServer == null)
				ConfigureSockets();

			if (tcpServer.Port != portIn)
				ConfigureSockets();

			while (!tcpServer.Started)
			{
				try
				{
					tcpServer.Port = portIn;
					tcpServer.BufferSize = DEFAULT_BUFFER_SIZE;
					tcpServer.Start();
					//Console("TCP Server Started");
					OnStatusChanged();
				}
				catch
				{
					//Console("Can not start Tcp Server");
					Thread.Sleep(500);
				}
			}
		}

		/// <summary>
		/// Starts the TCP server and TCP client asynchronously
		/// </summary>
		protected void UnidirectionalConnectionTask()
		{
			if (!running || !autoReconnect) return;

			if ((tcpServer == null) || (tcpClient == null))
				ConfigureSockets();

			if ((tcpServer.Port != portIn) || tcpClient.Port != portOut)
				ConfigureSockets();

			while (!tcpServer.Started || !tcpClient.IsOpen)
			{
				if (!tcpServer.Started)
				{
					try
						{
						tcpServer.Port = portIn;
						tcpServer.BufferSize = DEFAULT_BUFFER_SIZE;
						tcpServer.Start();
						//Console("TCP Server Started");
						OnStatusChanged();
					}
					catch
					{
						//Console("Can not start Tcp Server");
					}
				}

				if (!tcpClient.IsOpen)
				{
					try
					{
						tcpClient.Connect();
						//Console("Local client connected to remote server");
					}
					catch
					{
						//Console("Cannot connect with remote server");
					}
				}

				if (!tcpServer.Started || !tcpClient.IsOpen) Thread.Sleep(500);
				//Thread.Sleep(500);
			}
		}

		#endregion

		#region Socket Methods

		/// <summary>
		/// Creates and configures the sockets by setting the ports, address and Event Handlers
		/// </summary>
		protected void ConfigureSockets()
		{
			//this.FormClosing += new FormClosingEventHandler(VisionForm_FormClosing);
			//PortIn = 2070;
			//PortOut = 2300;

			if ((tcpServer != null) && (tcpServer.Started))
				tcpServer.Stop();
			tcpServer = new SocketTcpServer(portIn);
			tcpServer.BufferSize = DEFAULT_BUFFER_SIZE;
			tcpServer.DataReceived += new TcpDataReceivedEventHandler(socketTCPIn_DataReceived);
			tcpServer.ClientConnected += new TcpClientConnectedEventHandler(socketTCPIn_ClientConnected);
			tcpServer.ClientDisconnected += new TcpClientDisconnectedEventHandler(socketTCPIn_ClientDisconnected);

			if ((tcpClient != null) && (tcpClient.IsConnected))
				tcpClient.Disconnect();
			if (portIn != portOut)
			{
				tcpClient = new SocketTcpClient(remoteServerAddress, portOut);
				tcpClient.BufferSize = DEFAULT_BUFFER_SIZE;

				tcpClient.Connected += new TcpClientConnectedEventHandler(socketTCPOut_Connected);
				tcpClient.DataReceived += new TcpDataReceivedEventHandler(socketTCPOut_DataReceived);
				tcpClient.Disconnected += new TcpClientDisconnectedEventHandler(socketTCPOut_Disconnected);
			}

			//mainThread.Start();
		}

		/// <summary>
		/// Stops all socket activity
		/// </summary>
		protected void TerminateSockets()
		{
			if ((tcpServer != null) && (tcpServer.Started)) tcpServer.Stop(); ;
			if ((tcpClient != null) && (tcpClient.IsOpen)) tcpClient.Disconnect();
		}

		/// <summary>
		/// Sends a Command
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if command was sent successfully, false otherwise</returns>
		public bool Send(Command command)
		{
			//if (Bidirectional && tcpServer.IsConnected(command.MessageSourceMetadata))
			//{
			//	return serverSend(command.StringToSend, command.MessageSourceMetadata);
			//}
			bool result;
			result = tcpSend(command.StringToSend);
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
			if (Bidirectional)
			{
				if (((ep = response.MessageSourceMetadata as IPEndPoint) != null) && tcpServer.IsConnected(ep))
					return serverSend(response.StringToSend, ep);
				else
					return serverSend(response.StringToSend);
			}
			result = tcpSend(response.StringToSend);
			if(result)
				OnResponseSent(response);
			return result;
		}

		/// <summary>
		/// Sends data through client socket
		/// </summary>
		/// <param name="s">String to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		protected bool clientSend(string s)
		{
			if (tcpClient.IsOpen)
			{
				tcpClient.Send(s.Trim());
				//Console("Sent to Client: " + s);
				return true;
			}
			else
			{
				//Console("Can`t Send through client [TCP]: " + (s.Length > 100 ? s.Substring(0, 100) : s));
				return false;
			}
		}

		/// <summary>
		/// Sends data through server socket
		/// </summary>
		/// <param name="s">String to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		protected bool serverSend(string s)
		{
			int count;
			if (tcpServer.ClientsConnected < 1) return false;
			if (tcpServer.Started)
			{
				lock (tcpServer)
				{
					count = tcpServer.SendToAll(s.Trim());
				}
				//Console("Sent to " + count.ToString() + " clients on server: " + s);
				return true;
			}
			else
			{
				//Console("Can`t Send through server [Client TCP]: " + (s.Length > 100 ? s.Substring(0, 100) : s));
				return false;
			}
		}

		/// <summary>
		/// Sends data through server socket to specified endpoint
		/// </summary>
		/// <param name="s">String to send</param>\
		/// <param name="endPoint">Destination endpoint to snd data to</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		protected bool serverSend(string s, IPEndPoint endPoint)
		{
			if (!tcpServer.Started || !tcpServer.IsConnected(endPoint))
			{
				//Console("Can`t Send through server to " + endPoint.ToString() + ": " + (s.Length > 100 ? s.Substring(0, 100) : s));
				return false;
			}
			tcpServer.SendTo(endPoint, s);
			//Console("Sent to " + endPoint.ToString() + " clients on server: " + s);
			return true;
			
		}

		/// <summary>
		/// Sends data through socket depending on the mode of the ConnectionManager
		/// </summary>
		/// <param name="s">String to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		protected bool tcpSend(string s)
		{
			if (Bidirectional)
			{
				return serverSend(s);
			}
			else
			{
				if (!s.StartsWith(moduleName)) return clientSend(moduleName + " " + s);
				else return clientSend(s);
			}
		}

		/// <summary>
		/// Sends data to all clients connected to the server
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="count">The offset in the byte array to begin sending</param>
		/// <param name="offset">The number of bytes to send</param>
		/// <returns>The number of clients to which the data was sent</returns>
		public int SendToAllClients(byte[] buffer, int offset, int count)
		{
			return tcpServer.SendToAll(buffer, offset, count);
		}

		/// <summary>
		/// Sends data to all clients connected to the server
		/// </summary>
		/// <param name="s">The string to send</param>
		/// <returns>The number of clients to which the string was sent</returns>
		public int SendToAllClients(string s)
		{
			return tcpServer.SendToAll(s);
		}

		/// <summary>
		/// Sends data through the tcp client
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="count">The offset in the byte array to begin sending</param>
		/// <param name="offset">The number of bytes to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		public bool SendTroughClient(byte[] buffer, int offset, int count)
		{
			try
			{
				if ((tcpClient != null) && tcpClient.IsConnected)
				{
					tcpClient.Send(buffer, offset, count);
					return true;
				}
				else return false;
			}
			catch { return false; }
		}

		/// <summary>
		/// Sends data through the tcp client
		/// </summary>
		/// <param name="s">The string to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		public bool SendTroughClient(string s)
		{
			try
			{
				if ((tcpClient != null) && tcpClient.IsConnected)
				{
					tcpClient.Send(s);
					return true;
				}
				else return false;
			}
			catch { return false; }
		}

		/// <summary>
		/// Sends data to the specified remote endpoint
		/// </summary>
		/// <param name="remoteEndPoint">The destination endpoint</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="count">The offset in the byte array to begin sending</param>
		/// <param name="offset">The number of bytes to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		public bool SendTo(IPEndPoint remoteEndPoint, byte[] buffer, int offset, int count)
		{

			try
			{
				if ((tcpClient != null) && tcpClient.IsConnected && (remoteEndPoint.Address == tcpClient.ServerAddress) && (remoteEndPoint.Port == tcpClient.Port))
				{
					tcpClient.Send(buffer, offset, count);
					return true;
				}
				else if ((tcpServer != null) && tcpServer.Started && tcpServer.IsConnected(remoteEndPoint))
				{
					tcpServer.SendTo(remoteEndPoint, buffer, offset, count);
					return true;
				}
				else
					return false;
			}
			catch { return false; }
		}

		/// <summary>
		/// Sends data to the specified remote endpoint
		/// </summary>
		/// <param name="remoteEndPoint">The destination endpoint</param>
		/// <param name="s">The string to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		public bool SendTo(IPEndPoint remoteEndPoint, string s)
		{

			try
			{
				if ((tcpClient != null) && tcpClient.IsConnected && (remoteEndPoint.Address == tcpClient.ServerAddress) && (remoteEndPoint.Port == tcpClient.Port))
				{
					tcpClient.Send(s);
					return true;
				}
				else if ((tcpServer != null) && tcpServer.Started && tcpServer.IsConnected(remoteEndPoint))
				{
					tcpServer.SendTo(remoteEndPoint, s);
					return true;
				}
				else
					return false;
			}
			catch { return false; }
		}

		#endregion

		#region ICommandSource Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		public void ReceiveResponse(Response response)
		{
			if (response.MessageSource != this)
				return;
			Send(response);
		}

		#endregion

		#region TCP Server events

		/// <summary>
		/// Manages the ClientConnected event of the input socket
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		private void socketTCPIn_ClientConnected(Socket s)
		{
			//Console(s.RemoteEndPoint.ToString() + " connected to local server");
			try
			{
				OnClientConnected(s);
			}
			//catch (Exception ex)
			//{
			//	Console("Exception wile managing ConnectionManager.ClientConnected event: [" + ex.ToString() + "]");
			//}
			catch { }
			
		}

		/// <summary>
		/// Manages the ClientDisconnected event of the input socket
		/// </summary>
		/// <param name="ep">Disconnection endpoint</param>
		private void socketTCPIn_ClientDisconnected(EndPoint ep)
		{
			//try
			//{
			//	Console("Client " + ep.ToString() + " disconnected from local server");
			//}
			//catch { Console("Client 0.0.0.0:0 disconnected from local server"); }
			try
			{
				OnClientDisconnected(ep);
			}
			//catch (Exception ex)
			//{
			//	Console("Exception wile managing ConnectionManager.ClientDisconnected event: [" + ex.ToString() + "]");
			//}
			catch { }
			
		}

		/// <summary>
		/// Manages the DataReceived event of the input socket
		/// </summary>
		/// <param name="p">Received data</param>
		private void socketTCPIn_DataReceived(TcpPacket p)
		{
			lastReceivedPacket = p;
			string stringReceived = p.DataString.Trim();
			//Console("Rcv: " + "[" + p.SenderIP.ToString() + "] " + stringReceived);
			try
			{
				OnDataReceived(p);
			}
			//catch (Exception ex)
			//{
			//	//Console("Exception wile managing ConnectionManager.DataReceived event: [" + ex.ToString() + "]");
			//}
			catch{}
		}

		#endregion

		#region TCP Client events

		/// <summary>
		/// Manages the Connected event of the output socket
		/// </summary>
		/// <param name="s">Socket used for connection</param>
		private void socketTCPOut_Connected(Socket s)
		{
			//Console("Client connected to " + s.RemoteEndPoint.ToString());
			try
			{
				OnConnected(s);
			}
			//catch (Exception ex)
			//{
			//	Console("Exception wile managing ConnectionManager.Connected event: [" + ex.ToString() + "]");
			//}
			catch { }
		}

		/// <summary>
		/// Manages the Disconnected event of the output socket
		/// </summary>
		/// <param name="ep">Disconnection endpoint</param>
		private void socketTCPOut_Disconnected(EndPoint ep)
		{
			//Console("Client disconnected");
			if (autoReconnect)
				StartConnectionThread();
			try
			{
				OnDisconnected(ep);
			}
			//catch (Exception ex)
			//{
			//	Console("Exception wile managing ConnectionManager.Disconnected event: ["+ex.ToString()+"]");
			//}
			catch { }
		}

		/// <summary>
		/// Manages the DataReceived event of the output socket
		/// </summary>
		/// <param name="p">Received data</param>
		private void socketTCPOut_DataReceived(TcpPacket p)
		{
			if (!outputSocketReceptionEnabled)
				return;

			lastReceivedPacket = p;
			string stringReceived = p.DataString.Trim();
			//Console("Rcv: " + "[" + p.SenderIP.ToString() + "] " + stringReceived);
			try
			{
				OnDataReceived(p);
			}
			//catch (Exception ex)
			//{
			//	//Console("Exception wile managing ConnectionManager.DataReceived event: [" + ex.ToString() + "]");
			//}
			catch { }
		}

		#endregion		

		#endregion

		#region Console Methods

		/// <summary>
		/// Appends text to the console
		/// </summary>
		/// <param name="text">Text to append</param>
		protected virtual void Console(string text)
		{
			return;
		}

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