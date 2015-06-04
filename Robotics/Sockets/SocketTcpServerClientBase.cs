using System;
using System.Collections.Generic;
using System.Text;

namespace System.Net.Sockets
{
	/// <summary>
	/// Base class for SocketTcpClient and SocketTcpServer implementations
	/// </summary>
	public abstract class SocketTcpServerClientBase
	{
		/// <summary>
		/// Default buffer size
		/// </summary>
		public const int DEFAULT_BUFFER_SIZE = 16384;

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
		/// Indicates if the Server is started or the client is open
		/// </summary>
		protected bool running;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the variables of the base class
		/// </summary>
		public SocketTcpServerClientBase(int port)
		{
			running = false;
			bufferSize = DEFAULT_BUFFER_SIZE;
			Port = port;

			dlgDataReceived = new AsyncCallback(dataReceived);
		}

		#endregion

		#region Events

		/// <summary>
		/// Represents the method that will handle the DataReceived event
		/// </summary>
		public event TcpDataReceivedEventHandler DataReceived;

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
				if (value < 0) throw new Exception("Size of buffer must be greater than zero");
				if (running) throw new Exception("Can not change buffer size while client is connected");
				bufferSize = value;
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
		/// Gets the underlying socket used for connection
		/// </summary>
		public abstract Socket Socket
		{
			get;
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Parses data received trough socket spliting merged packets
		/// </summary>
		/// <param name="s">socket which received the data</param>
		/// <param name="data">Received data buffer</param>
		protected virtual TcpPacket ParseReceivedData(Socket s, byte[] data)
		{
			TcpPacket packet = new TcpPacket(s, data, 0, data.Length);
			if (DataReceived != null)
				DataReceived(packet);
			return packet;
			/*
			int i;
			bool binaryPakage = false;
			TcpPacket packet = null;

			// Check if is string suitable
			i = 0;
			while ((i < data.Length) && (data[i] != 0))
			{
				if (data[i] > 127)
				{
					binaryPakage = true;
					break;
				}
				++i;
			}

			if (binaryPakage)
			{
				packet = new TcpPacket(s, data, 0, data.Length);
				if (DataReceived != null)
					DataReceived(packet);
				//lastString = packet.DataString;
			}
			else
			{
				int count = 0;
				byte[] segment;

				System.IO.MemoryStream ms = new System.IO.MemoryStream(data.Length);
				for (i = 0; (i < data.Length) && (data[i] < 127); ++i)
				{
					if ((data[i] == 0) && (ms.Position > 0))
					{
						segment = ms.ToArray();
						packet = new TcpPacket(s, segment, 0, segment.Length);
						if (DataReceived != null) DataReceived(packet);
						ms.Close();
						ms = new System.IO.MemoryStream(data.Length);
						count = 0;
						continue;
					}
					ms.WriteByte(data[i]);
					count++;
					data[i] = 0;
				}
				//if (ms.Position == received)
				//{
				//	segment = ms.ToArray();
				//	packet = new TcpPacket(s, segment, 0, segment.Length);
				//	if (DataReceived != null) DataReceived(packet);
				//}
				//if (packet != null) lastString = packet.DataString;
			}

			// Clear buffer
			return packet;
			*/

		}

		/// <summary>
		/// Begins a (safe) receive operation with a socket.
		/// If the operation fails, it retries automatically while the socket is connected
		/// </summary>
		internal virtual bool BeginReceive(AsyncStateObject aso)
		{
			if((aso == null) || (aso.Socket == null))
				return false;

			//while (aso.Socket.Connected)
			if (aso.Socket.Connected)
			{
				try
				{
					aso.Socket.BeginReceive(aso.Buffer, 0, bufferSize, SocketFlags.None, dlgDataReceived, aso);
					return true;
				}
				catch
				{
					System.Threading.Thread.Sleep(0);
					//continue;
				}
			}
			return false;
		}

		#endregion

		#region Event Handler Functions

		/// <summary>
		/// Manages the data received async callback
		/// </summary>
		/// <param name="result">Result of async operation</param>
		protected virtual void dataReceived(IAsyncResult result)
		{
			int received;
			AsyncStateObject aso = (AsyncStateObject)result.AsyncState;
			Socket socket = aso.Socket;

			try
			{
				received = socket.EndReceive(result);
			}

			//catch (SocketException ex)
			catch (SocketException)
			{
				//if (ex.ErrorCode == 10054)
				//	throw ex;
				BeginReceive(aso);
				return;
			}
			if (socket.Connected)
			{
				if((received == 0) && (aso.Length == 0))
					throw new Exception("Disconnected");
				aso.Flush(received);
				// There is no pending data
				if ((received < aso.BufferSize) || (socket.Available < 1))
				{
					ParseReceivedData(socket, aso.DataReceived);
					aso = new AsyncStateObject(socket, bufferSize);
				}
				
				BeginReceive(aso);
				//if (socket.Receive(cnnBuff, SocketFlags.Peek) == 0)
				//	throw new Exception("Disconnected");
			}
		}

		#endregion
	}
}
