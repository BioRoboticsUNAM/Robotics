using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;


namespace System.Net.Sockets
{

	/// <summary>
	/// Implementa un Servidor TCP con soporte para multiples clientes
	/// </summary>
	public class SocketTcpServer : SocketTcpServerClientBase
	{
		#region Variables

		private TcpListener listener;
		private List<Socket> clients;
		private AsyncCallback dlgSocketAccepted;

		#endregion

		#region Constructores

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpServer class
		/// </summary>
		public SocketTcpServer(): this(2000)

		{
			
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpServer class
		/// </summary>
		public SocketTcpServer(int port)
			: base(port)
		{
			dlgSocketAccepted = new AsyncCallback(SocketAccepted);
			clients = new List<Socket>(10);
		}


		/// <summary>
		/// Destructor closes all connections and release resources
		/// </summary>
		~SocketTcpServer()
		{
			int i;
			for (i = 0; i < clients.Count; ++i)
			{
				try
				{
					clients[i].Shutdown(SocketShutdown.Both);
					clients[i].BeginDisconnect(false, null, null);
					//clients[i].Disconnect(true);
					clients[i].Close();
				}
				catch { }
			}

			try
			{
				listener.Stop();
			}
			catch { }
			running = false;
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets the number of clients connected to the TCP Server.
		/// </summary>
		public int ClientsConnected
		{
			get { return clients.Count; }
		}

		/// <summary>
		/// Gets an array with the IP Addresses of the sockets connected to this TCP Server
		/// </summary>
		public IPEndPoint[] Clients
		{
			get
			{
				IPEndPoint[] endPoints = new IPEndPoint[clients.Count];
				for (int i = 0; i < clients.Count; ++i)
					endPoints[i] = (IPEndPoint)clients[i].RemoteEndPoint;
				return endPoints;
			}
		}

		/// <summary>
		/// Gets a list containing the sockets connected to this TCP Server
		/// </summary>
		public Socket[] ClientList
		{
			get { return clients.ToArray(); }
		}

		/// <summary>
		/// Gets the underlying socket used for connection
		/// </summary>
		public override Socket Socket
		{
			get { return this.listener.Server; }
		}

		/// <summary>
		/// Gets a value indicating if the Tcp server is running
		/// </summary>
		public bool Started
		{
			get
			{
				return running;
			}
		}

		#endregion

		#region Eventos
		/// <summary>
		/// Represents the method that will handle the client connected event of a SocketTcpServer object
		/// </summary>
		public event TcpClientConnectedEventHandler ClientConnected;
		/// <summary>
		/// Represents the method that will handle the client disconnected event of a SocketTcpServer object
		/// </summary>
		public event TcpClientDisconnectedEventHandler ClientDisconnected;
		#endregion

		#region Metodos

		/// <summary>
		/// Check if a client with the specified remote endpoint is connected to the server
		/// </summary>
		/// <param name="ep">The remote endpoint of the client</param>
		/// <returns>True if connected</returns>
		public bool IsConnected(IPEndPoint ep)
		{
			if (!running) return false;
			int i;
			for (i = 0; i < clients.Count; ++i)
			{
				if ((IPEndPoint)clients[i].RemoteEndPoint == ep)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Check if a client with the specified IP address is connected to the server
		/// </summary>
		/// <param name="ip">The IP Address of the client</param>
		/// <returns>True if connected</returns>
		public bool IsConnected(IPAddress ip)
		{
			if (!running) return false;
			int i;
			for (i = 0; i < clients.Count; ++i)
			{
				if (((IPEndPoint)clients[i].RemoteEndPoint).Address == ip)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		public void SendTo(IPEndPoint destination, byte[] buffer)
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			SendTo(destination, buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void SendTo(IPEndPoint destination, byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			int i, j = 0;
			for (i = 0; i < clients.Count; ++i)
			{
				if (destination == (IPEndPoint)clients[i].RemoteEndPoint)
				{
					++j;
					clients[i].Send(buffer, offset, count, SocketFlags.None);
					return;
				}
			}
			if (j == 0) throw new Exception("Client is not connected");
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendTo(IPEndPoint destination, string s)
		{
			if (!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.ASCII.GetBytes(s);
			SendTo(destination, data, 0, data.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all the clients with specified IP adress
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendTo(IPAddress destination, byte[] buffer)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			return SendTo(destination, buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all the clients with specified IP adress
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendTo(IPAddress destination, byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			int i, j = 0;
			for (i = 0; i < clients.Count; ++i)
			{
				if (destination == ((IPEndPoint)clients[i].RemoteEndPoint).Address)
				{
					++j;
					clients[i].Send(buffer, offset, count, SocketFlags.None);
				}
			}
			return j;
		}

		/// <summary>
		/// Sends a specified number of bytes to all the clients with specified IP adress
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="s">The string to send to the output buffer</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendTo(IPAddress destination, string s)
		{
			if (!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.ASCII.GetBytes(s);
			return SendTo(destination, data, 0, data.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all connected Clients
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendToAll(byte[] buffer)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			return SendToAll(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all connected Clients
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendToAll(byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			int cnt = 0;
			for (int i = 0; i < clients.Count; ++i)
			{
				try
				{
					clients[i].Send(buffer, offset, count, SocketFlags.None);
					++cnt;
				}
				catch { }
			}
			return cnt;
		}

		/// <summary>
		/// Sends the specifiec string to all connected Clients
		/// </summary>
		/// <param name="s">The string to send to the output buffer</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendToAll(string s)
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			
			if(!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.ASCII.GetBytes(s);
			return SendToAll(data, 0, data.Length);
		}

		/// <summary>
		/// Starts the TCP server and begin to listen incomming connections
		/// </summary>
		public void Start()
		{
			if (running) throw new Exception("Tcp Server already running");
			if (port <= 0) throw new Exception("Port is not configured");
			//listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
			listener = new TcpListener(IPAddress.Any, port);
			//listener.ExclusiveAddressUse
			
			listener.Server.ReceiveBufferSize = this.bufferSize;
			listener.Server.SendBufferSize = this.bufferSize;
			listener.Start();
			listener.BeginAcceptSocket(dlgSocketAccepted, listener);
			running = true;
		}

		/// <summary>
		/// Starts the TCP server and begin to listen incomming connections
		/// </summary>
		/// <param name="port">Port to connect</param>
		public void Start(int port)
		{
			Port = port;
			Start();
		}

		/// <summary>
		/// Stop waiting for incoming connections and disconnect all connected clients
		/// </summary>
		public void Stop()
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			for (int i = 0; i < clients.Count; ++i)
			{
				try
				{
					clients[i].Disconnect(false);
					clients[i].Close();
				}
				catch { }
			}
			clients.Clear();
			listener.Stop();
			running = false;
		}

		#endregion

		#region Event Handler Functions

		private void SocketAccepted(IAsyncResult result)
		{
			Socket s;
			AsyncStateObject aso;
			try
			{
				s = listener.EndAcceptSocket(result);
			}
			catch { return; }
			
				if (s.Connected)
				{
					clients.Add(s);
					aso = new AsyncStateObject(s, bufferSize);
					BeginReceive(aso);
					if (ClientConnected != null) ClientConnected(s);
				}
				listener.BeginAcceptSocket(dlgSocketAccepted, listener);
		}

		/// <summary>
		/// Manages the data received async callback
		/// </summary>
		/// <param name="result">Result of async operation</param>
		protected override void dataReceived(IAsyncResult result)
		{
			int ix;
			AsyncStateObject aso = (AsyncStateObject)result.AsyncState;
			Socket socket = aso.Socket;
			EndPoint ep = null;
			
			ix = clients.IndexOf(socket);
			if (ix < 0)
				return;
			try
			{
				ep = socket.RemoteEndPoint;
				if (socket.Connected)
					base.dataReceived(result);
				else
				//if (!socket.Connected || !socket.Poll(1, SelectMode.SelectRead) || !socket.Poll(1, SelectMode.SelectWrite))
				{
					clients.RemoveAt(ix);
					if (ClientDisconnected != null)
						ClientDisconnected(ep);
				}
				
			}
			catch
			{
				if (clients.Count > ix)
					clients.RemoveAt(ix);
				if (ClientDisconnected != null)
					ClientDisconnected(ep);
			}

		}

		#endregion
	}

	#region OldCode

	/*
	 * 
	/// <summary>
	/// Implementa un Servidor TCP con soporte para multiples clientes
	/// </summary>
	public class SocketTcpServer
	{
		#region Variables

		private int port;
		private TcpListener listener;
		private bool running = false;
		private List<Socket> clients;
		private List<byte[]> buffers;
		private int bufferSize;
		private AsyncCallback dlgDataReceived;
		private AsyncCallback dlgSocketAccepted;

		#endregion

		#region Constructores

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpServer class
		/// </summary>
		public SocketTcpServer()
		{
			dlgDataReceived = new AsyncCallback(dataReceived);
			dlgSocketAccepted = new AsyncCallback(SocketAccepted);
			clients = new List<Socket>(10);
			buffers = new List<byte[]>(10);
			bufferSize = 1024;
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpServer class
		/// </summary>
		public SocketTcpServer(int port)
			: this()
		{
			Port = port;
		}


		/// <summary>
		/// Destructor closes all connections and release resources
		/// </summary>
		~SocketTcpServer()
		{
			int i;
			for (i = 0; i < clients.Count; ++i)
			{
				try
				{
					clients[i].Disconnect(true);
					clients[i].Close();
				}
				catch { }
			}

			try
			{
				listener.Stop();
			}
			catch { }
			buffers.Clear();
			running = false;
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets or sets the connection port for the socket.
		/// </summary>
		public int Port
		{
			get { return port; }
			set
			{
				if ((value >= 1) && (value <= 65535))
					port = value;
				else throw new Exception("Port must be between 1 and 65535");
			}
		}

		/// <summary>
		/// Gets a value indicating if the Tcp server is running
		/// </summary>
		public bool Started
		{
			get
			{
				return running;
			}
		}

		/// <summary>
		/// Gets or sets the size of the buffer per client for incoming data
		/// </summary>
		public int BufferSize
		{
			get { return bufferSize; }
			set
			{
				if (value < 0) throw new Exception("Size of buffer must be greater than zero");
				if (running) throw new Exception("Can not change buffer size while server is running");
				bufferSize = value;
			}
		}

		/// <summary>
		/// Gets the number of clients connected to the TCP Server.
		/// </summary>
		public int ClientsConnected
		{
			get { return clients.Count; }
		}

		/// <summary>
		/// Gets an array with the IP Addresses of the sockets connected to this TCP Server
		/// </summary>
		public IPEndPoint[] Clients
		{
			get
			{
				IPEndPoint[] endPoints = new IPEndPoint[clients.Count];
				for (int i = 0; i < clients.Count; ++i)
					endPoints[i] = (IPEndPoint)clients[i].RemoteEndPoint;
				return endPoints;
			}
		}

		/// <summary>
		/// Gets a list containing the sockets connected to this TCP Server
		/// </summary>
		public Socket[] ClientList
		{
			get { return clients.ToArray(); }
		}

		#endregion

		#region Eventos
		/// <summary>
		/// Represents the method that will handle the client connected event of a SocketTcpServer object
		/// </summary>
		public event TcpClientConnectedEventHandler ClientConnected;
		/// <summary>
		/// Represents the method that will handle the client disconnected event of a SocketTcpServer object
		/// </summary>
		public event TcpClientDisconnectedEventHandler ClientDisconnected;
		/// <summary>
		/// Represents the method that will handle the data received event of a SocketTcpServer object
		/// </summary>
		public event TcpDataReceivedEventHandler DataReceived;
		#endregion

		#region Metodos

		/// <summary>
		/// Check if a client with the specified remote endpoint is connected to the server
		/// </summary>
		/// <param name="ep">The remote endpoint of the client</param>
		/// <returns>True if connected</returns>
		public bool IsConnected(IPEndPoint ep)
		{
			if (!running) return false;
			int i;
			for (i = 0; i < clients.Count; ++i)
			{
				if ((IPEndPoint)clients[i].RemoteEndPoint == ep)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Check if a client with the specified IP address is connected to the server
		/// </summary>
		/// <param name="ip">The IP Address of the client</param>
		/// <returns>True if connected</returns>
		public bool IsConnected(IPAddress ip)
		{
			if (!running) return false;
			int i;
			for (i = 0; i < clients.Count; ++i)
			{
				if (((IPEndPoint)clients[i].RemoteEndPoint).Address == ip)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Parses data received trough socket spliting merged packets
		/// </summary>
		/// <param name="s">socket which received the data</param>
		/// <param name="buffer">Received data buffer</param>
		/// <param name="received">Number of bytes received</param>
		private void ParseReceivedData(Socket s, ref byte[] buffer, int received)
		{
			int i;
			bool binaryPakage = false;
			TcpPacket packet;

			// Check if is string suitable
			i = 0;
			while ((i < received) && (buffer[i] != 0))
			{
				if (buffer[i] > 127)
				{
					binaryPakage = true;
					break;
				}
				++i;
			}

			if (binaryPakage)
			{
				packet = new TcpPacket(s, buffer, 0, received);
				for (i = 0; i < buffer.Length; ++i) buffer[i] = 0;
				if (DataReceived != null)
				DataReceived(packet);
				//lastString = packet.DataString;
				return;
			}
			else
			{
				int count = 0;
				byte[] segment;

				System.IO.MemoryStream ms = new System.IO.MemoryStream(received);
				for (i = 0; (i < received) && (buffer[i] < 127); ++i)
				{
					if ((buffer[i] == 0) && (ms.Position > 0))
					{
						segment = ms.ToArray();
						packet = new TcpPacket(s, segment, 0, segment.Length);
						if (DataReceived != null)DataReceived(packet);
						ms.Close();
						ms = new System.IO.MemoryStream(received);
						count = 0;
						continue;
					}
					ms.WriteByte(buffer[i]);
					count++;
					buffer[i] = 0;
				}
				//if (packet != null) lastString = packet.DataString;
			}
			
			// Clear buffer
			
			
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		public void SendTo(IPEndPoint destination, byte[] buffer)
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			SendTo(destination, buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void SendTo(IPEndPoint destination, byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			int i, j = 0;
			for (i = 0; i < clients.Count; ++i)
			{
				if (destination == (IPEndPoint)clients[i].RemoteEndPoint)
				{
					++j;
					clients[i].SendBufferSize = (count - offset);
					clients[i].Send(buffer, offset, count, SocketFlags.None);
					return;
				}
			}
			if (j == 0) throw new Exception("Client is not connected");
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendTo(IPEndPoint destination, string s)
		{
			if (!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.ASCII.GetBytes(s);
			SendTo(destination, data, 0, data.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all the clients with specified IP adress
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendTo(IPAddress destination, byte[] buffer)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			return SendTo(destination, buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all the clients with specified IP adress
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendTo(IPAddress destination, byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			int i, j = 0;
			for (i = 0; i < clients.Count; ++i)
			{
				if (destination == ((IPEndPoint)clients[i].RemoteEndPoint).Address)
				{
					++j;
					clients[i].SendBufferSize = (count - offset); ;
					clients[i].Send(buffer, offset, count, SocketFlags.None);
				}
			}
			return j;
		}

		/// <summary>
		/// Sends a specified number of bytes to all the clients with specified IP adress
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="s">The string to send to the output buffer</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendTo(IPAddress destination, string s)
		{
			if (!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.ASCII.GetBytes(s);
			return SendTo(destination, data, 0, data.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all connected Clients
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendToAll(byte[] buffer)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			return SendToAll(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all connected Clients
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendToAll(byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			int cnt = 0;
			for (int i = 0; i < clients.Count; ++i)
			{
				try
				{
					clients[i].SendBufferSize = (count - offset);
					clients[i].Send(buffer, offset, count, SocketFlags.None);
					++cnt;
				}
				catch { }
			}
			return cnt;
		}

		/// <summary>
		/// Sends the specifiec string to all connected Clients
		/// </summary>
		/// <param name="s">The string to send to the output buffer</param>
		/// <returns>The number of clients to which the packet was sent</returns>
		public int SendToAll(string s)
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			
			if(!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.ASCII.GetBytes(s);
			return SendToAll(data, 0, data.Length);
		}

		/// <summary>
		/// Starts the TCP server and begin to listen incomming connections
		/// </summary>
		public void Start()
		{
			if (running) throw new Exception("Tcp Server already running");
			if (port <= 0) throw new Exception("Port is not configured");
			//listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
			listener = new TcpListener(IPAddress.Any, port);
			//listener.ExclusiveAddressUse
			listener.Start();
			listener.BeginAcceptSocket(dlgSocketAccepted, listener);
			running = true;
		}

		/// <summary>
		/// Starts the TCP server and begin to listen incomming connections
		/// </summary>
		/// <param name="port">Port to connect</param>
		public void Start(int port)
		{
			Port = port;
			Start();
		}

		/// <summary>
		/// Stop waiting for incoming connections and disconnect all connected clients
		/// </summary>
		public void Stop()
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			for (int i = 0; i < clients.Count; ++i)
			{
				try
				{
					clients[i].Disconnect(false);
					clients[i].Close();
				}
				catch { }
			}
			clients.Clear();
			buffers.Clear();
			listener.Stop();
			running = false;
		}

		#endregion

		#region Event Handler Functions

		private void SocketAccepted(IAsyncResult result)
		{
			Socket s;
			try
			{
				s = listener.EndAcceptSocket(result);
			}
			catch { return; }
			byte[] buffer;
			
				if (s.Connected)
				{
					buffer = new byte[bufferSize];
					clients.Add(s);
					buffers.Add(buffer);
					s.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, dlgDataReceived, s);
					if (ClientConnected != null) ClientConnected(s);
				}
				listener.BeginAcceptSocket(dlgSocketAccepted, listener);
		}

		private void dataReceived(IAsyncResult result)
		{
			int ix, received;
			byte[] buffer;
			Socket s = (Socket)result.AsyncState;
			ix = clients.IndexOf(s);
			//TcpPacket packet;
			try
			{
				if (ix < 0) return;
				received = s.EndReceive(result);
				if (s.Connected && (received != 0))
				{
					buffer =buffers[ix];
					//packet = new TcpPacket(s, buffer, 0, received);
					//if (DataReceived != null)
					//	DataReceived(packet);
					//for (int i = 0; i < buffer.Length; ++i) buffer[i] = 0;
					ParseReceivedData(s, ref buffer, received);
					s.BeginReceive(buffer, 0, bufferSize, SocketFlags.None, dlgDataReceived, s);
				}
				else
				{
					clients.RemoveAt(ix);
					buffers.RemoveAt(ix);
					if (ClientDisconnected != null)
						ClientDisconnected(s.RemoteEndPoint);
				}
			}
			catch
			{
				if (clients.Count > ix)
				{
					clients.RemoveAt(ix);
					buffers.RemoveAt(ix);
				}
				if (ClientDisconnected != null)
					ClientDisconnected(s.RemoteEndPoint);
			}

		}

		#endregion
	}
	 * 
	*/

	#endregion
}
