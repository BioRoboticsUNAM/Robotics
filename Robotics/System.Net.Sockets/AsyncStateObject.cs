using System;
using System.IO;
using System.Net.Sockets;

namespace System.Net.Sockets.Obsolete
{
	/// <summary>
	/// Represents an object used to receive data asynchronously
	/// </summary>
	[Obsolete("Class AsyncStateObject is deprecated. Use Robotics.AsyncStateObject instead")]
	internal class AsyncStateObject
	{

		#region Variables

		/// <summary>
		/// Stores the size of the buffer
		/// </summary>
		protected readonly byte[] buffer;

		/// <summary>
		/// Stores the size of the buffer
		/// </summary>
		protected readonly int bufferSize;

		/// <summary>
		/// Stores the data source Socket object
		/// </summary>
		protected Socket socket = null;
		
		/// <summary>
		/// Stores the received data
		/// </summary>
		protected MemoryStream dataReceived;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of AsyncStateObject
		/// </summary>
		/// <param name="socket">The data source Socket object</param>
		/// <param name="bufferSize">The size of the buffer</param>
		[Obsolete("Class AsyncStateObject is deprecated. Use Robotics.AsyncStateObject instead")]
		public AsyncStateObject(Socket socket, int bufferSize)
		{
			this.socket = socket;
			this.bufferSize = bufferSize;
			buffer = new byte[bufferSize];
			dataReceived = new MemoryStream(bufferSize);
		}

		/// <summary>
		/// Destroy the object
		/// </summary>
		~AsyncStateObject()
		{
			this.socket = null;
			dataReceived.Close();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the buffer to store temporary data
		/// </summary>
		[Obsolete("Class AsyncStateObject is deprecated. Use Robotics.AsyncStateObject instead")]
		public byte[] Buffer
		{
			get
			{
				return buffer;
			}
		}

		/// <summary>
		/// Stores the size of the buffer
		/// </summary>
		[Obsolete("Class AsyncStateObject is deprecated. Use Robotics.AsyncStateObject instead")]
		public int BufferSize
		{
			get { return bufferSize; }
		}

		/// <summary>
		/// Gets the data received
		/// </summary>
		[Obsolete("Class AsyncStateObject is deprecated. Use Robotics.AsyncStateObject instead")]
		public byte[] DataReceived
		{
			get
			{
				byte[] received = new byte[dataReceived.Length];
				dataReceived.Position = 0;
				dataReceived.Read(received, 0, (int)dataReceived.Length);
				return received;
			}
		}

		/// <summary>
		/// Gets the length of received data
		/// </summary>
		[Obsolete("Class AsyncStateObject is deprecated. Use Robotics.AsyncStateObject instead")]
		public int Length
		{
			get { return (int)dataReceived.Length; }
		}

		/// <summary>
		/// Gets the data source Socket object
		/// </summary>
		[Obsolete("Class AsyncStateObject is deprecated. Use Robotics.AsyncStateObject instead")]
		public Socket Socket
		{
			get { return this.socket; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Flushes out the buffer to the DataReceived data storage to allow receive more data 
		/// </summary>
		/// <param name="count">Number of bytes in the temporary buffer to flush out</param>
		[Obsolete("Class AsyncStateObject is deprecated. Use Robotics.AsyncStateObject instead")]
		public void Flush(int count)
		{
			dataReceived.Write(buffer, 0, count);
		}

		#endregion

	}
}
