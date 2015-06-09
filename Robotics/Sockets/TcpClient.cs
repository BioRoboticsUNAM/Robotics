using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;


namespace Robotics.Sockets
{
	/// <summary>
	/// Implements a simple TCP client
	/// </summary>
	public class TcpClient
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
		protected AsyncCallback dlgDataReceived;

		/// <summary>
		/// Stores the connection port
		/// </summary>
		protected int port;

		/// <summary>
		/// the socket used for connection
		/// </summary>
		private System.Net.Sockets.TcpClient client;

		/// <summary>
		/// Object used to lock the access to client
		/// </summary>
		private object oClient;

		/// <summary>
		/// Stores the ip address of the server
		/// </summary>
		private IPAddress serverIP;

		/// <summary>
		/// Stores the maximum amount of time to wait for establishing a connection
		/// </summary>
		private TimeSpan connectionTimeout = TimeSpan.FromSeconds(3);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of TcpClient class
		/// </summary>
		public TcpClient() : this(IPAddress.Loopback, 2000) { }

		/// <summary>
		/// Initializes a new instance of TcpClient class
		/// </summary>
		/// <param name="serverAddress">The server IP Address</param>
		/// <param name="port">The connection port for the socket</param>
		public TcpClient(IPAddress serverAddress, int port)
		{
			oClient = new Object();
			ServerAddress = serverAddress;
			Port = port;
			bufferSize = DEFAULT_BUFFER_SIZE;
			dlgDataReceived = new AsyncCallback(Socket_DataReceived);
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
				lock (oClient)
				{
					if (IsConnected)
						throw new Exception("Cannot change the size of the buffer while the client is connected");
					bufferSize = value;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating the Connected or Disconnected status of the socket
		/// </summary>
		public bool IsConnected
		{
			get
			{
				lock (oClient)
				{
					return (client != null) && client.Connected;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating the Connected (open) or Disconnected (closed) status of the socket
		/// </summary>
		public bool IsOpen { get { return IsConnected; } }

		/// <summary>
		/// Gets or sets the server IP Address
		/// </summary>
		public IPAddress ServerAddress
		{
			get { return serverIP; }
			set
			{
				if (value == null) throw new Exception("Parameter serverAddress cannot be null");
				lock (oClient)
				{
					if (IsConnected)
						throw new Exception("Cannot change Server IP Address while the client is connected");
					serverIP = value;
				}
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

				lock (oClient)
				{
					if (IsConnected)
						throw new Exception("Cannot change Port while the client is connected");
					port = value;
				}
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the client connects to a server
		/// </summary>
		public event EventHandler<TcpClient, IPEndPoint> Connected;

		/// <summary>
		/// Occurs when the client disconnects from the server
		/// </summary>
		public event EventHandler<TcpClient, IPEndPoint> Disconnected;

		/// <summary>
		/// Occurs when data is received
		/// </summary>
		public event EventHandler<TcpClient, TcpPacket> DataReceived;

		#endregion

		#region Methods

		/// <summary>
		/// Begins a (safe) receive operation with a socket.
		/// If the operation fails, it retries automatically while the socket is connected
		/// </summary>
		private void BeginReceive(AsyncStateObject aso)
		{
			if (aso == null)
				return;

			lock (oClient)
			{
				if (!client.Connected)
				{
					Disconnect();
					return;
				}

				try
				{
					aso.Received = 0;
					client.Client.BeginReceive(aso.Buffer,
						0,
						aso.BufferSize,
						SocketFlags.None,
						dlgDataReceived,
						aso);
				}
				catch
				{
					Disconnect();
					return;
				}
			} // end lock
		}

		/// <summary>
		/// Opens a new Tco socket connection
		/// </summary>
		/// <param name="address">IP Address of the server to connect to</param>
		/// <param name="port">Port to connect</param>
		public void Connect(IPAddress address, int port)
		{
			ServerAddress = address;
			Port = port;
			Connect();
		}

		/// <summary>
		/// Opens a new Tcp socket connection
		/// </summary>
		public void Connect()
		{
			Exception ex;
			if (!TryConnect(out ex))
				throw ex;
		}

		/// <summary>
		/// Closes the port connection and sets the System.Net.Sockets.SocketTcpClient.IsOpen property to false
		/// </summary>
		public void Disconnect()
		{
			lock (oClient)
			{
				if (client == null) return;
				System.Net.Sockets.TcpClient oldClient = client;
				client = null;
				try
				{
					if (oldClient.Connected)
					{
						oldClient.Client.Disconnect(true);
						oldClient.Client.Close();
						oldClient.Close();
					}
				}
				catch (ObjectDisposedException){ return; }
			} // end lock
			OnDisconnected();
		}

		/// <summary>
		/// Rises the Connected event
		/// </summary>
		protected void OnConnected()
		{
			try
			{
				if (this.Connected != null)
					this.Connected(this, new IPEndPoint(serverIP, port));
			}
			catch { }
		}

		/// <summary>
		/// Rises the DataReceived event
		/// </summary>
		/// <param name="packet">TcpPacket containing the data received</param>
		protected void OnDataReceived(TcpPacket packet)
		{
			try
			{
				if (DataReceived != null)
					DataReceived(this, packet);
			}
			catch { }
		}

		/// <summary>
		/// Rises the Disconnected event
		/// </summary>
		protected void OnDisconnected()
		{
			try
			{
				if (this.Disconnected != null)
					this.Disconnected(this, new IPEndPoint(serverIP, port));
			}
			catch { }
		}

		/// <summary>
		/// Sends a specified number of bytes to the server
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void Send(byte[] buffer, int offset, int count)
		{
			// client.Send(buffer, offset, count, SocketFlags.None);
			IAsyncResult result;
			lock (oClient)
			{
				if (!IsConnected) throw new Exception("Client is not connected");
				result = client.Client.BeginSend(buffer, offset, count, SocketFlags.None, null, null);
			}
			result.AsyncWaitHandle.WaitOne();
		}

		/// <summary>
		/// Sends a byte array to the server
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		public void Send(byte[] buffer)
		{
			Send(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Sends the specified string to the server
		/// </summary>
		/// <param name="s">The string to send to the output buffer</param>
		public void Send(string s)
		{
			if (!s.EndsWith("\0")) s = s + "\0";
			byte[] data = Encoding.UTF8.GetBytes(s);
			Send(data, 0, data.Length);
		}

		/// <summary>
		/// Configures the connection parameters for the client
		/// </summary>
		private void SetupSocket()
		{
			client = new System.Net.Sockets.TcpClient();
			// client.ExclusiveAddressUse = false;
			client.Client.NoDelay = true;
			client.ReceiveBufferSize = bufferSize;
			client.SendBufferSize = bufferSize;
		}

		private bool TryConnect(out Exception ex)
		{
			ex = null;
			lock (oClient)
			{
				if (client != null) return true;

				if (port < 1)
				{
					ex = new Exception("Invalid port");
					return false;
				}
				if (serverIP == null)
				{
					ex = new Exception("Set a server IP Address first");
					return false;
				}

				SetupSocket();
				IAsyncResult result = client.BeginConnect(serverIP, port, null, null);
				result.AsyncWaitHandle.WaitOne(connectionTimeout);
				if (!client.Connected)
				{
					client.Close();
					client = null;
					ex = new TimeoutException("Connection timeout");
					return false;
				}
				client.EndConnect(result);
				BeginReceive(new AsyncStateObject(client.Client, bufferSize));
			} // end lock
			OnConnected();
			return true;
		}

		/// <summary>
		/// Tries to open a new Tcp socket connection
		/// </summary>
		/// <param name="address">IP Address of the server to connect to</param>
		/// <param name="port">Port to connect</param>
		/// <returns>True if connection was established</returns>
		public bool TryConnect(IPAddress address, int port)
		{
			ServerAddress = address;
			Port = port;
			return TryConnect();
		}

		/// <summary>
		/// Tries to open a new Tcp socket connection
		/// </summary>
		/// <returns>True if connection was established</returns>
		public bool TryConnect()
		{
			Exception ex;
			return TryConnect(out ex);
		}

		#endregion

		#region Event Handler Functions

		/// <summary>
		/// Manages the data received async callback
		/// </summary>
		/// <param name="result">Result of async operation</param>
		protected void Socket_DataReceived(IAsyncResult result)
		{
			TcpPacket packet = null;
			AsyncStateObject aso = (AsyncStateObject)result.AsyncState;

			lock (oClient)
			{
				if ((client == null) || !client.Client.Connected)
				{
					Disconnect();
					return;
				}

				try
				{
					aso.Received = client.Client.EndReceive(result);
				}
				catch (SocketException)
				{
					BeginReceive(aso);
					return;
				}
				
				if (aso.Received > 0)
				{
					packet = new TcpPacket(aso.Socket, aso.Buffer, 0, aso.Received);
					BeginReceive(aso);
				}
				else
					Disconnect();
			} // end lock
			if (packet != null)
				OnDataReceived(packet);
		}

		#endregion
	}
}
