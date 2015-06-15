using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;


namespace Robotics.Sockets
{

	/// <summary>
	/// A TCP Server with support for multiple clients
	/// </summary>
	public class TcpServer
	{
		/// <summary>
		/// Default buffer size
		/// </summary>
		public const int DEFAULT_BUFFER_SIZE = 8192;

		#region Variables

		/// <summary>
		/// Stores the size of the input buffer
		/// </summary>
		protected int bufferSize;

		/// <summary>
		/// Represents the dataReceived Method. Used for async callback
		/// </summary>
		protected readonly AsyncCallback dlgDataReceived;

		private TcpListener listener;

		/// <summary>
		/// Object used to perform a lock over the listener
		/// </summary>
		private readonly Object oListener;

		/// <summary>
		/// Stores the Sockets for the clients, accessible by remote endpoint
		/// </summary>
		private readonly Dictionary<IPEndPoint, Socket> clients;

		/// <summary>
		/// Synchronization method for the clients dictionary
		/// </summary>
		private readonly ReaderWriterLock rwClients;

		/// <summary>
		/// Points at the SocketAccepted method
		/// </summary>
		private readonly AsyncCallback dlgSocketAccepted;

		/// <summary>
		/// Stores the connection port
		/// </summary>
		protected int port;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpServer class
		/// </summary>
		public TcpServer()
			: this(2000) { }

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpServer class
		/// </summary>
		public TcpServer(int port)
		{
			this.bufferSize = DEFAULT_BUFFER_SIZE;
			this.oListener = new Object();
			this.Port = port;
			this.dlgSocketAccepted = new AsyncCallback(SocketAccepted);
			this.dlgDataReceived = new AsyncCallback(Socket_DataReceived);
			this.clients = new Dictionary<IPEndPoint, Socket>(10);
			this.rwClients = new ReaderWriterLock();
		}


		/// <summary>
		/// Destructor closes all connections and release resources
		/// </summary>
		~TcpServer()
		{
			this.rwClients.AcquireWriterLock(-1);
			foreach (Socket s in clients.Values)
			{
				try
				{
					s.Shutdown(SocketShutdown.Both);
					s.Close(0);
				}
				catch { }
			}
			clients.Clear();
			this.rwClients.ReleaseWriterLock();
			try
			{
				lock (oListener)
				{
					if (listener != null)
						listener.Stop();
					listener = null;
				}
			}
			catch { }
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the size of the buffer for incoming/outgoing data
		/// </summary>
		public int BufferSize
		{
			get { return bufferSize; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException("Size of buffer must be greater than zero");
				lock (oListener)
				{
					if (Started)
						throw new Exception("Cannot change the size of the buffer while the client is connected");
					bufferSize = value;
				}
			}
		}

		/// <summary>
		/// Gets the number of clients connected to the TCP Server.
		/// </summary>
		public int ClientsConnected
		{
			get
			{
				this.rwClients.AcquireReaderLock(-1);
				int count = clients.Count;
				this.rwClients.ReleaseReaderLock();
				return count;
			}
		}

		/// <summary>
		/// Gets or sets the connection port for the socket.
		/// </summary>
		public int Port
		{
			get { return port; }
			set
			{
				if ((value < 1) || (value > 65535))
					throw new Exception("Port must be between 1 and 65535");

				lock (oListener)
				{
					if (Started)
						throw new Exception("Cannot change Port while the server is running");
					port = value;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating if the Tcp server is running
		/// </summary>
		public bool Started
		{
			get
			{
				lock (oListener)
				{
					return (listener != null);
				}
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a client connects to the server
		/// </summary>
		public event EventHandler<TcpServer, IPEndPoint> ClientConnected;

		/// <summary>
		/// Occurs when a client disconnects from the server
		/// </summary>
		public event EventHandler<TcpServer, IPEndPoint> ClientDisconnected;

		/// <summary>
		/// Occurs when data is received from a client
		/// </summary>
		public event EventHandler<TcpServer, TcpPacket> DataReceived;

		#endregion

		#region Methods

		/// <summary>
		/// Begins a (safe) receive operation with a socket.
		/// If the operation fails, it retries automatically while the socket is connected
		/// </summary>
		private bool BeginReceive(AsyncStateObject aso)
		{
			if ((aso == null) || (aso.Socket == null))
				return false;

			lock (aso.Socket)
			{
				if (!aso.Socket.Connected)
				{
					DisconnectAndRemoveClient(aso.Socket);
					return false;
				}

				try
				{
					aso.Received = 0;
					aso.Socket.BeginReceive(aso.Buffer,
						0,
						bufferSize,
						SocketFlags.None,
						dlgDataReceived,
						aso);
				}
				catch
				{
					DisconnectAndRemoveClient(aso.Socket);
					return false;
				}
			} // end lock
			return true;
		}

		/// <summary>
		/// Adds a client to the client list
		/// </summary>
		/// <param name="client">The client to add to the list</param>
		private void AddClient(Socket client)
		{
			IPEndPoint ep = (IPEndPoint)client.RemoteEndPoint;
			if (this.rwClients.IsReaderLockHeld)
				this.rwClients.UpgradeToWriterLock(-1);
			else
				this.rwClients.AcquireWriterLock(-1);
			if (!this.clients.ContainsKey(ep))
				this.clients.Add(ep, client);
			else
			{
				try
				{
					this.clients[ep].Close();
				}
				catch { }
				this.clients[ep] = client;
			}
			this.rwClients.ReleaseWriterLock();
		}

		/// <summary>
		/// Disconnects all clients and removes them from the client list
		/// </summary>
		private void DisconnectAndRemoveAll()
		{
			Queue<Socket> sockets;
			this.rwClients.AcquireWriterLock(-1);
			sockets = new Queue<Socket>(clients.Count);
			foreach (Socket client in clients.Values)
				sockets.Enqueue(client);
			clients.Clear();
			this.rwClients.ReleaseWriterLock();
			while (sockets.Count > 0)
			{
				IPEndPoint ep;
				Socket s = sockets.Dequeue();
				if ((s == null) || !s.Connected) continue;
				lock (s)
				{
					try
					{
						ep = (IPEndPoint)s.RemoteEndPoint;
						s.Close();
					}
					catch { continue; }
				}
				OnClientDisconnected(ep);
			}
		}

		/// <summary>
		/// Disconnects the given socket (client) and removes it from the client list
		/// </summary>
		/// <param name="socket">The socket to disconnect</param>
		private void DisconnectAndRemoveClient(Socket socket)
		{
			if ((socket == null) || !socket.Connected) return;
			IPEndPoint ep = (IPEndPoint)socket.RemoteEndPoint;
			if (this.rwClients.IsReaderLockHeld)
				this.rwClients.UpgradeToWriterLock(-1);
			else
				this.rwClients.AcquireWriterLock(-1);
			clients.Remove(ep);
			this.rwClients.ReleaseWriterLock();
			lock (socket)
			{
				try
				{
					socket.Close();
				}
				catch { return; }
			}
			OnClientDisconnected(ep);
		}

		/// <summary>
		/// Check if a client with the specified remote endpoint is connected to the server
		/// </summary>
		/// <param name="ep">The remote endpoint of the client</param>
		/// <returns>True if connected</returns>
		public bool IsConnected(IPEndPoint ep)
		{
			if (!Started) return false;
			this.rwClients.AcquireReaderLock(-1);
			bool connected = this.clients.ContainsKey(ep);
			this.rwClients.ReleaseReaderLock();
			return connected;
		}

		/// <summary>
		/// Check if a client with the specified IP address is connected to the server
		/// </summary>
		/// <param name="ip">The IP Address of the client</param>
		/// <returns>True if connected</returns>
		public bool IsConnected(IPAddress ip)
		{
			if (!Started) return false;
			bool connected = false;
			this.rwClients.AcquireReaderLock(-1);
			foreach (IPEndPoint ep in clients.Keys)
			{
				if (ep.Address == ip)
				{
					connected = true;
					break;
				}
			}
			this.rwClients.ReleaseReaderLock();
			return connected;
		}

		/// <summary>
		/// Raises de ClientConnected event
		/// </summary>
		/// <param name="ep">Remote Endpoint of the connected client</param>
		private void OnClientConnected(IPEndPoint ep)
		{
			try
			{
				if (this.ClientConnected != null)
					this.ClientConnected(this, ep);
			}
			catch { }
		}

		/// <summary>
		/// Raises de ClientDisconnected event
		/// </summary>
		/// <param name="ep">Remote Endpoint of the disconnected client</param>
		private void OnClientDisconnected(IPEndPoint ep)
		{
			try
			{
				if (this.ClientDisconnected != null)
					this.ClientDisconnected(this, ep);
			}
			catch { }
		}

		/// <summary>
		/// Raises the DataReceived event
		/// </summary>
		/// <param name="packet">TcpPacket containing the data received</param>
		protected virtual void OnDataReceived(TcpPacket packet)
		{
			try
			{
				if (DataReceived != null)
					DataReceived(this, packet);
			}
			catch { }
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		public bool SendTo(IPEndPoint destination, byte[] buffer)
		{
			return SendTo(destination, buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public bool SendTo(IPEndPoint destination, byte[] buffer, int offset, int count)
		{
			Socket client = null;
			// if (!Started) throw new Exception("Tcp Server is not runing");
			if (!Started) return false;
			this.rwClients.AcquireReaderLock(-1);
			if (this.clients.ContainsKey(destination))
				client = this.clients[destination];
			this.rwClients.ReleaseReaderLock();
			// if(client == null)
			//	throw new Exception("Client is not connected");
			if (client == null)
				return false;
			IAsyncResult result;
			try
			{
				lock (client)
				{
					result = client.BeginSend(buffer, offset, count, SocketFlags.None, null, null);
				}
				result.AsyncWaitHandle.WaitOne();
			}
			catch { return false; }
			return true;
		}

		/// <summary>
		/// Sends a specified number of bytes to the client with specified remote IPEndPoint
		/// </summary>
		/// <param name="destination">The client's IP address</param>
		/// <param name="s">The string to send to the output buffer</param>
		public bool SendTo(IPEndPoint destination, string s)
		{
			if (!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.UTF8.GetBytes(s);
			return SendTo(destination, data, 0, data.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all connected Clients
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <returns>The number of clients to which the packet was sent.  If server is not running returns -1</returns>
		public int SendToAll(byte[] buffer)
		{
			return SendToAll(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends a specified number of bytes to all connected Clients
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		/// <returns>The number of clients to which the packet was sent. If server is not running returns -1</returns>
		public int SendToAll(byte[] buffer, int offset, int count)
		{
			if (!Started) throw new Exception("Tcp Server is not runing");
			int cnt = 0;
			Queue<WaitHandle> handles = new Queue<WaitHandle>();
			this.rwClients.AcquireReaderLock(-1);
			foreach (Socket client in clients.Values)
			{
				lock (client)
				{
					try
					{
						IAsyncResult result = client.BeginSend(buffer, offset, count, SocketFlags.None, null, null);
						handles.Enqueue(result.AsyncWaitHandle);
					}
					catch { continue; }
					++cnt;
				}
			}
			this.rwClients.ReleaseReaderLock();
			while (handles.Count > 0)
				handles.Dequeue().WaitOne();
			return cnt;
		}

		/// <summary>
		/// Sends the specifiec string to all connected Clients
		/// </summary>
		/// <param name="s">The string to send to the output buffer</param>
		/// <returns>The number of clients to which the packet was sent.  If server is not running returns -1</returns>
		public int SendToAll(string s)
		{
			if (!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.UTF8.GetBytes(s);
			return SendToAll(data, 0, data.Length);
		}

		/// <summary>
		/// Starts the TCP server and begin to listen incomming connections
		/// </summary>
		public void Start()
		{
			if (port <= 0) throw new Exception("Port is not configured");
			lock (oListener)
			{
				if (listener != null)
					return;
				DisconnectAndRemoveAll();
				listener = new TcpListener(IPAddress.Any, port);
				listener.Server.ReceiveBufferSize = this.bufferSize;
				listener.Server.SendBufferSize = this.bufferSize;
				listener.Start();
				listener.BeginAcceptSocket(dlgSocketAccepted, listener);
			}
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
			lock (oListener)
			{
				if (listener == null) return;
				DisconnectAndRemoveAll();
				TcpListener oldListener = listener;
				listener = null;
				oldListener.Stop();
			} // lock(oListener)
		}

		#endregion

		#region Event Handler Functions

		private void SocketAccepted(IAsyncResult result)
		{
			Socket s;
			lock (oListener)
			{
				if (listener == null)
					return;
				try
				{
					s = listener.EndAcceptSocket(result);
				}
				catch
				{
					s = null;
				}
				listener.BeginAcceptSocket(dlgSocketAccepted, listener);
			}

			if ((s != null) && s.Connected)
			{
				lock (s)
				{
					AddClient(s);
					AsyncStateObject aso = new AsyncStateObject(s, bufferSize);
					if (!BeginReceive(aso))
					{
						DisconnectAndRemoveClient(aso.Socket);
						return;
					}
					OnClientConnected((IPEndPoint)s.RemoteEndPoint);
				}
			}

		}

		/// <summary>
		/// Manages the data received async callback
		/// </summary>
		/// <param name="result">Result of async operation</param>
		protected void Socket_DataReceived(IAsyncResult result)
		{
			AsyncStateObject aso = (AsyncStateObject)result.AsyncState;
			Socket socket = aso.Socket;

			TcpPacket packet = null;
			if (socket == null) return;
			lock (socket)
			{
				if (!socket.Connected)
					return;
				try
				{
					aso.Received = socket.EndReceive(result);
				}
				catch (SocketException)
				{
					if (!BeginReceive(aso))
						DisconnectAndRemoveClient(socket);
					return;
				}
				catch (ObjectDisposedException)
				{
					return;
				}

				if (socket.Connected && (aso.Received > 0))
				{
					packet = new TcpPacket(aso.Socket, aso.Buffer, 0, aso.Received);
					BeginReceive(aso);
				}
				else
					DisconnectAndRemoveClient(socket);
			} // end lock
			if (packet != null)
				OnDataReceived(packet);
		}

		#endregion
	}
}