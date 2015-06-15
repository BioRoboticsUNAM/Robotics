using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;


namespace System.Net.Sockets.Obsolete
{
	/// <summary>
	/// Implements a simple UDP client
	/// </summary>
	[Obsolete]
	public class SocketUdpClient
	{
		#region Variables

		private int port;
		private bool isOpen;
		private Socket socket;
		private int bufferSize;
		private byte[] rxBuffer;
		private IPAddress serverIP;
		private IPEndPoint endPoint;
		private string lastString;
		#endregion

		#region Constructores

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketUdpClient class
		/// </summary>
		public SocketUdpClient()
		{
			port = 0;
			bufferSize = 1024;
			isOpen = false;
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketUdpClient class
		/// </summary>
		/// <param name="port">the connection port for the socket</param>
		public SocketUdpClient(int port)
			: this()
		{
			Port = port;
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketUdpClient class
		/// </summary>
		/// <param name="serverAddress">The server IP Address</param>
		/// <param name="port">The connection port for the socket</param>
		public SocketUdpClient(IPAddress serverAddress, int port)
			: this()
		{
			ServerAddress = serverAddress;
			Port = port;
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketUdpClient class
		/// </summary>
		/// <param name="serverAddress">An string representing the server IP Address</param>
		/// <param name="port">the connection port for the socket</param>
		public SocketUdpClient(string serverAddress, int port)
			: this()
		{
			this.serverIP = IPAddress.Parse(serverAddress);
			Port = port;
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets a value indicating the open or closed status of the socket
		/// </summary>
		public bool Connected
		{
			get
			{
				if (socket != null)
					return isOpen & socket.Connected;
				else return isOpen;
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
				if ((value >= 1) && (value <= 65535))
					port = value;
				else throw new Exception("Port must be between 1 and 65535");
			}
		}

		/// <summary>
		/// Gets or sets the server IP Address
		/// </summary>
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
		/// Gets the last packet received formatted as string
		/// </summary>
		public string LastStringReceived
		{
			get { return lastString; }
		}

		/// <summary>
		/// Gets or sets the size of the buffer for incoming data
		/// </summary>
		public int BufferSize
		{
			get { return bufferSize; }
			set
			{
				if (value < 0) throw new Exception("Size of buffer must be greater than zero");
				if (isOpen) throw new Exception("Can not change buffer size while client is connected");
				bufferSize = value;
			}
		}

		#endregion

		#region Eventos

		/*
		/// <summary>
		/// Represents the method that will handle the client connected event of a SocketUdpClient object
		/// </summary>
		public event TcpClientConnectedEventHandler Connected;
		/// <summary>
		/// Represents the method that will handle the client disconnected event of a SocketUdpClient object
		/// </summary>
		public event TcpClientConnectedEventHandler Disconnected;
		*/

		/// <summary>
		/// Represents the method that will handle the data received event of a SocketUdpClient object
		/// </summary>
		public event UdpDataReceivedEventHandler DataReceived;

		#endregion

		#region Metodos

		/// <summary>
		/// Opens a new Tco socket connection
		/// </summary>
		/// <param name="port">Port to connect</param>
		public void Connect(int port)
		{
			Port = port;
			Connect();
		}

		/// <summary>
		/// Opens a new Tcp socket connection
		/// </summary>
		public void Connect()
		{
			if (port < 1) throw new Exception("Unknown port");
			if (serverIP == null) throw new Exception("Set a server IP Address first");
			if (isOpen) throw new Exception("Socket is already open");
			endPoint = new IPEndPoint(serverIP, port);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			//socket.Bind(new IPEndPoint(IPAddress.Any, port));
			socket.Connect(endPoint);
			EndPoint ep = (EndPoint)endPoint;

			rxBuffer = new byte[bufferSize];
			socket.BeginReceiveFrom(
				rxBuffer,
				0,
				bufferSize,
				SocketFlags.None,
				ref ep,
				new AsyncCallback(dataReceived),
				socket);
			isOpen = true;
			//if (Connected != null)
			//	Connected(socket);
		}

		/// <summary>
		/// Closes the port connection and sets the System.Net.Sockets.SocketUdpClient.IsOpen property to false
		/// </summary>
		public void Disconnect()
		{
			if (!isOpen) throw new Exception("Socket is not open");
			//hilo.Abort();
			if (socket.Connected)
			{
				socket.Shutdown(SocketShutdown.Both);
				socket.Disconnect(false);
				socket.Close();
			}
			//socket.Close(0);
			isOpen = false;
			//if (Disconnected != null)
			//	Disconnected(socket);
		}

		/// <summary>
		/// Sends a specified number of bytes to the server
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void Send(byte[] buffer, int offset, int count)
		{
			if (!isOpen || !socket.Connected) throw new Exception("Client is not connected");
			socket.Send(buffer, offset, count, SocketFlags.None);
		}

		/// <summary>
		/// Sends the specified string to the server
		/// </summary>
		/// <param name="s">The string to send to the output buffer</param>
		public void Send(string s)
		{
			if (!isOpen || !socket.Connected) throw new Exception("Client is not connected");
			byte[] data = Encoding.ASCII.GetBytes(s + "\0\0");
			socket.Send(data, SocketFlags.None);
		}

		/// <summary>
		/// Tries to open a new Tcp socket connection
		/// </summary>
		/// <param name="port">Port to connect</param>
		/// <returns>True if connection was established</returns>
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

		#region Send

		#region SendTo (byte[])

		/// <summary>
		/// Sends a specified number of bytes to specified endpoint
		/// </summary>
		/// <param name="destination">Destination endpoint</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void SendTo(IPEndPoint destination, byte[] buffer, int offset, int count)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			socket.SendBufferSize = count - offset + 1;
			socket.SendTo(buffer, offset, count, SocketFlags.None, destination);
		}

		/// <summary>
		/// Sends a specified number of bytes to specified IP address in the network
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void SendTo(IPAddress destination, byte[] buffer, int offset, int count)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			IPEndPoint ep = new IPEndPoint(destination, port);
			socket.SendBufferSize = count - offset + 1;
			socket.SendTo(buffer, offset, count, SocketFlags.None, ep);
		}

		/// <summary>
		/// Sends a specified number of bytes to specified IP address over the specified port in the network
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="port">The port where the packet will arrive</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void SendTo(IPAddress destination, ushort port, byte[] buffer, int offset, int count)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			IPEndPoint ep = new IPEndPoint(destination, port);
			socket.SendBufferSize = count - offset + 1;
			socket.SendTo(buffer, offset, count, SocketFlags.None, ep);
		}

		#endregion

		#region SendTo (char[])

		/// <summary>
		/// Sends a specified number of characters to specified endpoint
		/// </summary>
		/// <param name="destination">Destination endpoint</param>
		/// <param name="buffer">The char array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void SendTo(IPEndPoint destination, char[] buffer, int offset, int count)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			SendTo(destination, new String(buffer, offset, count));
		}

		/// <summary>
		/// Sends a specified number of characters to specified IP address in the network
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="buffer">The char array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void SendTo(IPAddress destination, char[] buffer, int offset, int count)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			SendTo(destination, new String(buffer, offset, count));
		}

		/// <summary>
		/// Sends a specified number of characters to specified IP address over the specified port in the network
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="port">The port where the packet will arrive</param>
		/// <param name="buffer">The char array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void SendTo(IPAddress destination, ushort port, char[] buffer, int offset, int count)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			SendTo(destination, port, new String(buffer, offset, count));
		}

		#endregion

		#region SendTo (string)

		/// <summary>
		/// Sends the parameter string to specified endpoint
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendTo(IPEndPoint destination, string s)
		{
			if (!isOpen) throw new Exception("Socket is not open");

			byte[] txBuffer;
			txBuffer = Encoding.Default.GetBytes(s + "\0");
			socket.SendTo(txBuffer, txBuffer.Length, SocketFlags.None, destination);
		}

		/// <summary>
		/// Sends the parameter string to specified IP address in the network
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendTo(IPAddress destination, string s)
		{
			if (!isOpen) throw new Exception("Socket is not open");

			byte[] txBuffer;
			IPEndPoint ep = new IPEndPoint(destination, port);
			txBuffer = Encoding.Default.GetBytes(s + "\0");
			socket.SendTo(txBuffer, txBuffer.Length, SocketFlags.None, ep);
		}

		/// <summary>
		/// Sends the parameter string to specified IP address in the network
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="port">The port where the packet will arrive</param>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendTo(IPAddress destination, ushort port, string s)
		{
			if (!isOpen) throw new Exception("Socket is not open");

			byte[] txBuffer;
			IPEndPoint ep = new IPEndPoint(destination, port);
			txBuffer = Encoding.Default.GetBytes(s + "\0");
			socket.SendTo(txBuffer, txBuffer.Length, SocketFlags.None, ep);
		}

		#endregion

		#endregion

		#endregion

		#region Event Handler Functions

		private void dataReceived(IAsyncResult result)
		{
			UdpDataReceivedEventArgs e;
			int received;
			byte[] data;
			try
			{
				received = socket.EndReceive(result);
				if (socket.Connected && (received != 0))
				{
					data = new byte[received];
					for (int i = 0; i < received; ++i) data[i] = rxBuffer[i];
					e = new UdpDataReceivedEventArgs(((IPEndPoint)socket.RemoteEndPoint).Address, data, false);
					if (DataReceived != null)
						DataReceived(e);
					lastString = e.DataString;
					for (int i = 0; i < rxBuffer.Length; ++i) rxBuffer[i] = 0;
					socket.BeginReceive(
						rxBuffer,
						0,
						bufferSize,
						SocketFlags.None,
						new AsyncCallback(dataReceived),
						socket);
				}
				else
				{
					if (isOpen)
						Disconnect();
				}
			}
			catch
			{
				if (isOpen)
					Disconnect();
			}
		}

		#endregion
	}
}

