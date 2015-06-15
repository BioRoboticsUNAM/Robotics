using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;


namespace System.Net.Sockets.Obsolete
{

	/// <summary>
	/// Implementa un Servidor TCP con soporte para multiples clientes
	/// </summary>
	[Obsolete]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public SocketTcpServer()
			: this(2000)

		{
			
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpServer class
		/// </summary>
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public int ClientsConnected
		{
			get { return clients.Count; }
		}

		/// <summary>
		/// Gets an array with the IP Addresses of the sockets connected to this TCP Server
		/// </summary>
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public Socket[] ClientList
		{
			get { return clients.ToArray(); }
		}

		/// <summary>
		/// Gets the underlying socket used for connection
		/// </summary>
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public override Socket Socket
		{
			get { return this.listener.Server; }
		}

		/// <summary>
		/// Gets a value indicating if the Tcp server is running
		/// </summary>
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public event TcpClientConnectedEventHandler ClientConnected;
		/// <summary>
		/// Represents the method that will handle the client disconnected event of a SocketTcpServer object
		/// </summary>
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public event TcpClientDisconnectedEventHandler ClientDisconnected;
		#endregion

		#region Metodos

		/// <summary>
		/// Check if a client with the specified remote endpoint is connected to the server
		/// </summary>
		/// <param name="ep">The remote endpoint of the client</param>
		/// <returns>True if connected</returns>
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public void SendTo(IPEndPoint destination, byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			int i, j = 0;
			for (i = 0; i < clients.Count; ++i)
			{
				if (destination == (IPEndPoint)clients[i].RemoteEndPoint)
				{
					++j;
					// clients[i].Send(buffer, offset, count, SocketFlags.None);
					IAsyncResult result = clients[i].BeginSend(buffer, offset, count, SocketFlags.None, null, null);
					result.AsyncWaitHandle.WaitOne();
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public int SendTo(IPAddress destination, byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");

			int i, j = 0;
			for (i = 0; i < clients.Count; ++i)
			{
				if (destination == ((IPEndPoint)clients[i].RemoteEndPoint).Address)
				{
					++j;
					// clients[i].Send(buffer, offset, count, SocketFlags.None);
					IAsyncResult result = clients[i].BeginSend(buffer, offset, count, SocketFlags.None, null, null);
					result.AsyncWaitHandle.WaitOne();
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public int SendToAll(byte[] buffer, int offset, int count)
		{
			if (!running) throw new Exception("Tcp Server is not runing");
			int cnt = 0;
			for (int i = 0; i < clients.Count; ++i)
			{
				try
				{
					// clients[i].Send(buffer, offset, count, SocketFlags.None);
					IAsyncResult result = clients[i].BeginSend(buffer, offset, count, SocketFlags.None, null, null);
					result.AsyncWaitHandle.WaitOne();
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
		public void Start(int port)
		{
			Port = port;
			Start();
		}

		/// <summary>
		/// Stop waiting for incoming connections and disconnect all connected clients
		/// </summary>
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
		[Obsolete("Class SocketTcpServer is deprecated. Use Robotics.TcpServer instead")]
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
}
