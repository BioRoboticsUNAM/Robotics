using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace System.Net.Sockets.Obsolete
{
	/// <summary>
	/// Implementa un Socket UDP en modo Broadcast para 
	/// manejarlo como si fuera puerto serie
	/// </summary>
	[Obsolete]
	public class SocketUdp
	{
		#region Variables
		
		private ushort port;
		private bool isOpen;
		private Socket socket;
		private Thread hilo;
		private string newLine;
		private int bufferSize;
		private IPAddress lastSender;
		private byte[] lastPacket;
		private bool ignoreSent = false;

		#endregion

		#region Constructores
		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketUDP class
		/// </summary>
		public SocketUdp()
		{
			port = 0;
			isOpen = false;
			bufferSize = 1024;
			newLine = "\r\n";
		}

		/// <summary>
		/// Initializes a new instance of System.Net.Sockets.SocketUDP class
		/// </summary>
		/// <param name="port">Connection port</param>
		public SocketUdp(int port)
			: this()
		{
			Port = port;
		}

		#endregion

		#region Eventos
		/// <summary>
		/// Represents the method that will handle the data received event of a SocketUdp object
		/// </summary>
		public event UdpDataReceivedEventHandler DataReceived;
		/// <summary>
		/// Represents the method that will handle the error received event of a SocketUdp object
		/// </summary>
		public event UdpErrorReceivedEventHandler ErrorReceived;

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets or sets the size of the buffer for incoming data
		/// </summary>
		public int BufferSize
		{
			get { return bufferSize; }
			set
			{
				if (value < 0) throw new Exception("Size of buffer must be greater than zero");
				bufferSize = value;
			}
		}

		/// <summary>
		/// Gets a value indicating the open or closed status of the socket
		/// </summary>
		public bool IsOpen
		{
			get
			{
				return isOpen;
			}
		}

		/// <summary>
		/// Returns the last packet arrived the udp port
		/// </summary>
		public byte[] LastPacketReceived
		{
			get
			{
				return lastPacket;
			}
		}

		/// <summary>
		/// Returns the last packet arrived the udp port encoded as ASCII string
		/// </summary>
		public string LastStringReceived
		{
			get
			{
				return ASCIIEncoding.ASCII.GetString(lastPacket);
			}
		}

		/// <summary>
		/// Returns the IP address of the sender of the last packet arrived the udp port
		/// </summary>
		public IPAddress LastPacketSender
		{
			get
			{
				return lastSender;
			}
		}

		/// <summary>
		/// Gets or sets the value used to interpret the end of a call to the System.Net.Sockets.SocketUDP.SendLine method
		/// </summary>
		public string NewLine
		{
			get { return NewLine; }
			set
			{
				newLine = value;
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
					port = (ushort)value;
				//else throw new Exception("Port must be between 1 and 65535");
				else unchecked { port = (ushort)value; }
			}
		}

		#endregion

		#region Metodos

		/// <summary>
		/// Opens a new UDP socket connection
		/// </summary>
		/// <param name="port">Port to connect</param>
		public void Open(int port)
		{
			Port = port;
			Open();
		}

		/// <summary>
		/// Opens a new UDP socket connection
		/// </summary>
		public void Open()
		{
			if (port < 1) throw new Exception("Unknown port");
			if (isOpen) throw new Exception("Socket is already open");
			hilo = new Thread(receiveDataPoll);
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			//socket.ReceiveTimeout
			socket.EnableBroadcast = true;
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			socket.Bind(new IPEndPoint(IPAddress.Any, port));
			hilo.Start();
			isOpen = true;
		}

		/// <summary>
		/// Closes the port connection and sets the System.Net.Sockets.SocketUDP.IsOpen property to false
		/// </summary>
		public void Close()
		{
			if (!isOpen) throw new Exception("Socket is not open");
			hilo.Abort();
			socket.Shutdown(SocketShutdown.Both);
			socket.Close(0);
			isOpen = false;
		}

		/// <summary>
		/// Asynchronusly receives data
		/// </summary>
		private void receiveDataPoll()
		{
			byte[] rxBuffer;
			EndPoint senderIP;
			//IPPacketInformation packetInfo;
			//SocketFlags flags;
			bool truncated;
			int i;

			while (isOpen)
			{
				rxBuffer = new byte[bufferSize];
				rxBuffer.Initialize();
				for (i = 0; i < bufferSize; ++i)
					rxBuffer[i] = 0;
				senderIP = new IPEndPoint(IPAddress.Any, 0);
				//flags = SocketFlags.None;
				truncated = false;
				try
				{
					socket.ReceiveFrom(rxBuffer, bufferSize, SocketFlags.None, ref senderIP);
					if (ignoreSent) continue;
					//socket.ReceiveMessageFrom(rxBuffer, 0, bufferSize, ref flags, ref senderIP, out packetInfo);
				}
				catch (SocketException ex)
				{
					if (!isOpen) return;
					if (ex.ErrorCode == 10040) //Datos muy largos
					{
						truncated = true;
					}
					if (ErrorReceived != null) ErrorReceived(ex);
				}
				lastPacket = new byte[rxBuffer.Length];
				lastPacket.Initialize();
				Array.Copy(rxBuffer, lastPacket, rxBuffer.Length);
				lastSender = ((IPEndPoint)senderIP).Address;
				if (DataReceived != null) DataReceived(new UdpDataReceivedEventArgs(lastSender, rxBuffer, truncated));
			}
		}

		#region Send

		/// <summary>
		/// Sends a specified number of bytes to all IP address in the network
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void Send(byte[] buffer, int offset, int count)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			socket.SendBufferSize = count - offset + 1;
			IPEndPoint destination = new IPEndPoint(IPAddress.Broadcast, port);
			ignoreSent = true;
			socket.SendTo(buffer, offset, count, SocketFlags.None, destination);
			Thread.Sleep(0);
			ignoreSent = false;
		}

		/// <summary>
		/// Sends a specified number of characters to all IP address in the network
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="offset">The offset in the buffer array to begin sending</param>
		/// <param name="count">The number of bytes to send</param>
		public void Send(char[] buffer, int offset, int count)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			Send(new String(buffer, offset, count));
		}

		/// <summary>
		/// Sends the parameter string to all IP address in the network
		/// </summary>
		/// <param name="s">The string to send to the output buffer</param>
		public void Send(string s)
		{
			if (!isOpen) throw new Exception("Socket is not open");

			byte[] txBuffer;
			IPEndPoint destination = new IPEndPoint(IPAddress.Broadcast, port);
			txBuffer = Encoding.Default.GetBytes(s + "\0");
			ignoreSent = true;
			socket.SendTo(txBuffer, txBuffer.Length, SocketFlags.None, destination);
			Thread.Sleep(0);
			ignoreSent = false;
		}

		/// <summary>
		/// Sends the specified string and the System.Net.Sockets.SocketUDP.NewLineto all IP address in the network
		/// </summary>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendLine(string s)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			Send(s + this.newLine + "\0");
		}

		#endregion

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
			socket.SendBufferSize = txBuffer.Length;
			ignoreSent = true;
			socket.SendTo(txBuffer, txBuffer.Length, SocketFlags.None, destination);
			Thread.Sleep(0);
			ignoreSent = false;
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
			socket.SendBufferSize = txBuffer.Length;
			ignoreSent = true;
			socket.SendTo(txBuffer, txBuffer.Length, SocketFlags.None, ep);
			Thread.Sleep(0);
			ignoreSent = false;
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
			socket.SendBufferSize = txBuffer.Length;
			ignoreSent = true;
			socket.SendTo(txBuffer, txBuffer.Length, SocketFlags.None, ep);
			Thread.Sleep(0);
			ignoreSent = false;
		}

		#endregion

		#region SendLineTo

		/// <summary>
		/// Sends the specified string and the System.Net.Sockets.SocketUDP.NewLine to specified endpoint
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendLineTo(IPEndPoint destination, string s)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			SendTo(destination, s + this.newLine + "\0");
		}

		/// <summary>
		/// Sends the specified string and the System.Net.Sockets.SocketUDP.NewLineto specified IP address in the network
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendLineTo(IPAddress destination, string s)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			SendTo(destination, s + this.newLine + "\0");
		}

		/// <summary>
		/// Sends the specified string and the System.Net.Sockets.SocketUDP.NewLineto specified IP address in the network
		/// </summary>
		/// <param name="destination">The IP address of the destiny</param>
		/// <param name="port">The port where the packet will arrive</param>
		/// <param name="s">The string to send to the output buffer</param>
		public void SendLineTo(IPAddress destination, ushort port, string s)
		{
			if (!isOpen) throw new Exception("Socket is not open");
			SendTo(destination, port, s + this.newLine + "\0");
		}

		#endregion

		#endregion

	}
}