using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;


namespace System.Net.Sockets.Obsolete
{
	/// <summary>
	/// Implements a simple TCP client
	/// </summary>
	[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
	public class SocketTcpClient : SocketTcpServerClientBase
	{
		#region Variables

		/// <summary>
		/// the socket used for connection
		/// </summary>
		private Socket socket;

		/// <summary>
		/// Stores the ip address of the server
		/// </summary>
		private IPAddress serverIP;

		private IPEndPoint endPoint;
		/// <summary>
		/// Last received TCP Packet
		/// </summary>
		private TcpPacket lastPacket;
		/// <summary>
		/// Specifies whether the stream Socket is using the Nagle algorithm
		/// </summary>
		private bool noDelay;
		/// <summary>
		/// The remote endpoint to whre the socket is connected
		/// </summary>
		private EndPoint remoteEndPoint;
		/// <summary>
		/// The connection tieout in milliseconds
		/// </summary>
		private int connectionTimeOut;

		/// <summary>
		/// Selects the connection mode for the SocketTCPClient
		/// </summary>
		private TcpClientConnectionMode connectionMode;
		#endregion

		#region Constructores

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpClient class
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public SocketTcpClient()
			: this(2000)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpClient class
		/// </summary>
		/// <param name="port">the connection port for the socket</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public SocketTcpClient(int port)
			: base(port)
		{
			this.connectionTimeOut = 2000;
			this.ConnectionMode = TcpClientConnectionMode.Fast;
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpClient class
		/// </summary>
		/// <param name="serverAddress">The server IP Address</param>
		/// <param name="port">The connection port for the socket</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public SocketTcpClient(IPAddress serverAddress, int port)
			: this(port)
		{
			ServerAddress = serverAddress;
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketTcpClient class
		/// </summary>
		/// <param name="serverAddress">An string representing the server IP Address</param>
		/// <param name="port">the connection port for the socket</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public SocketTcpClient(string serverAddress, int port)
			: this(IPAddress.Parse(serverAddress), port){}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets or sets the connection timeout in milliseconds.
		/// Requires the ConnectionMode be set to Fast
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public int ConnectionTimeOut
		{
			get { return this.connectionTimeOut; }
			set
			{
				if (value < -1) throw new ArgumentOutOfRangeException();
				this.connectionTimeOut = value;
			}
		}

		/// <summary>
		/// Gets or sets the connection mode for the SocketTCPClient
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public TcpClientConnectionMode ConnectionMode
		{
			get { return this.connectionMode; }
			set { this.connectionMode = value; }
		}

		/// <summary>
		/// Gets a value indicating the Connected or Disconnected status of the socket
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public bool IsConnected
		{
			get
			{
				if (socket != null)
					return running & socket.Connected;
				else return running;
			}
		}

		/// <summary>
		/// Gets a value indicating the Connected (open) or Disconnected (closed) status of the socket
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public bool IsOpen
		{
			get
			{
				if (socket != null)
					return running & socket.Connected;
				else return running;
			}
		}

		/// <summary>
		/// Gets the last packet received
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public TcpPacket LastPacketReceived
		{
			get { return lastPacket; }
		}

		/// <summary>
		/// Gets the last packet received formatted as string
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public string LastStringReceived
		{
			get{return lastPacket.DataString;}
		}

		/// <summary>
		/// Gets or sets a Boolean value that specifies whether the stream Socket is using the Nagle algorithm
		/// </summary>
		/// <remarks>
		/// The Nagle algorithm reduces network traffic by causing the socket to buffer packets for up to 200 milliseconds and then combines and sends them in one packet
		/// The majority of network applications should use the Nagle algorithm.
		/// </remarks>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public bool NoDelay
		{
			get { return noDelay; }
			set { noDelay = value;}

		}

		/// <summary>
		/// Gets or sets the server IP Address
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public IPAddress ServerAddress
		{
			get { return serverIP; }
			set
			{
				if (value == null) throw new Exception("Parameter serverAddress cannot be null");
				serverIP = value;
			}
		}

		/// <summary>
		/// Gets the underlying socket used for connection
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public override Socket Socket
		{
			get { return this.socket; }
		}

		#endregion

		#region Eventos

		/// <summary>
		/// Represents the method that will handle the client connected event of a SocketTcpClient object
		/// </summary>
		public event TcpClientConnectedEventHandler Connected;
		/// <summary>
		/// Represents the method that will handle the client disconnected event of a SocketTcpClient object
		/// </summary>
		public event TcpClientDisconnectedEventHandler Disconnected;

		#endregion

		#region Metodos

		/// <summary>
		/// Begins a (safe) receive operation with a socket.
		/// If the operation fails, it retries automatically while the socket is connected
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		internal override bool BeginReceive(AsyncStateObject aso)
		{
			bool result;
			if (!(result = base.BeginReceive(aso)))
				Disconnect();
			return result;
		}

		/// <summary>
		/// Opens a new Tco socket connection
		/// </summary>
		/// <param name="port">Port to connect</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public void Connect(int port)
		{
			Port = port;
			Connect();
		}

		/// <summary>
		/// Opens a new Tcp socket connection
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public void Connect()
		{
			AsyncStateObject aso;
			int timeOut;

			if (port < 1) throw new Exception("Unknown port");
			if (serverIP == null) throw new Exception("Set a server IP Address first");
			if (running) throw new Exception("Socket is already open");
			SetupSocket(out timeOut);
			switch (this.connectionMode)
			{
				case TcpClientConnectionMode.Fast:
					ConnectFast(socket);
					break;

				case TcpClientConnectionMode.Normal:
					ConnectNormal(socket);
					break;
			}
			
			socket.SendTimeout = timeOut;
			remoteEndPoint = socket.RemoteEndPoint;
			aso = new AsyncStateObject(socket, bufferSize);

			socket.BeginReceive(
				aso.Buffer,
				0,
				bufferSize,
				SocketFlags.None,
				dlgDataReceived,
				aso);
			running = true;
			if (Connected != null)
				Connected(socket);
		}

		#region Connection Modes

		private void ConnectFast(Socket socket)
		{
			IAsyncResult cnnResult;

			cnnResult = socket.BeginConnect(endPoint, null, null);
			cnnResult.AsyncWaitHandle.WaitOne(connectionTimeOut, true);
			if (!socket.Connected)
			{
				try { socket.Shutdown(SocketShutdown.Both); }
				catch { }
				try { socket.Disconnect(false); }
				catch { }
				try { socket.Close(); }
				catch { }
				throw new TimeoutException("Connection to remote server timed out");
			}
			socket.EndConnect(cnnResult);
		}

		private void ConnectNormal(Socket socket)
		{
			socket.Connect(endPoint);
		}

		#endregion

		/// <summary>
		/// Closes the port connection and sets the System.Net.Sockets.SocketTcpClient.IsOpen property to false
		/// </summary>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public void Disconnect()
		{
			if (!running) throw new Exception("Socket is not open");
			if (socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Disconnect(false);
				socket.Close();
			}
			//socket.Close(0);
			running = false;
			if (Disconnected != null)
				Disconnected(remoteEndPoint);
		}

		/// <summary>
		/// Parses data received trough socket spliting merged packets
		/// </summary>
		/// <param name="s">socket which received the data</param>
		/// <param name="data">Received data</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		protected sealed override TcpPacket ParseReceivedData(Socket s, byte[] data)
		{
			lastPacket = base.ParseReceivedData(s, data);
			return lastPacket;
		}

		/// <summary>
		/// Sends a specified number of bytes to the server
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public void Send(byte[] buffer, int offset, int count)
		{
			if (!running || !socket.Connected) throw new Exception("Client is not connected");
			// socket.Send(buffer, offset, count, SocketFlags.None);
			IAsyncResult result = socket.BeginSend(buffer, offset, count, SocketFlags.None, null, null);
			result.AsyncWaitHandle.WaitOne();
		}

		/// <summary>
		/// Sends a byte array to the server
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public void Send(byte[] buffer)
		{
			if (!running || !socket.Connected) throw new Exception("Client is not connected");
			Send(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends the specified string to the server
		/// </summary>
		/// <param name="s">The string to send to the output buffer</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public void Send(string s)
		{
			if (!running || !socket.Connected) throw new Exception("Client is not connected");
			if (!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.ASCII.GetBytes(s);
			Send(data, 0, data.Length);
		}

		/// <summary>
		/// Configures the connection parameters for the socket
		/// </summary>
		private void SetupSocket(out int socketSendTimeout)
		{
			endPoint = new IPEndPoint(serverIP, port);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			socket.NoDelay = noDelay;
			socket.LingerState = new LingerOption(true, 10);
			socketSendTimeout = socket.SendTimeout;
			socket.SendTimeout = 10;
			socket.ReceiveBufferSize = bufferSize;
			socket.SendBufferSize = bufferSize;
		}

		/// <summary>
		/// Tries to open a new Tcp socket connection
		/// </summary>
		/// <param name="port">Port to connect</param>
		/// <returns>True if connection was established</returns>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		public bool TryConnect(int port)
		{
			Port = port;
			return TryConnect();
		}

		/// <summary>
		/// Tries to open a new Tcp socket connection
		/// </summary>
		/// <returns>True if connection was established</returns>
		public bool TryConnect()
		{
			try
			{
				Connect();
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		#region Event Handler Functions

		/// <summary>
		/// Manages the data received async callback
		/// </summary>
		/// <param name="result">Result of async operation</param>
		[Obsolete("Class SocketTcpClient is deprecated. Use Robotics.TcpClient instead")]
		protected sealed override void dataReceived(IAsyncResult result)
		{
			try
			{
				if (socket.Connected)
					base.dataReceived(result);
				else
				{
					if (running)
						Disconnect();
				}
			}
			catch
			{
				if (running)
					Disconnect();
			}
		}

		#endregion
	}
}