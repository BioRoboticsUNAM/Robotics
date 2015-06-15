using System;
using System.IO;
using System.Net.Sockets;

namespace Robotics.Sockets
{
	/// <summary>
	/// Represents an object used to receive data asynchronously
	/// </summary>
	internal class AsyncStateObject
	{

		#region Variables

		/// <summary>
		/// Stores the size of the buffer
		/// </summary>
		protected readonly byte[] buffer;

		/// <summary>
		/// Stores the data source Socket object
		/// </summary>
		protected Socket socket = null;

		/// <summary>
		/// Stores the number of bytes received
		/// </summary>
		protected int received;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of AsyncStateObject
		/// </summary>
		/// <param name="socket">The data source Socket object</param>
		/// <param name="bufferSize">The size of the buffer</param>
		public AsyncStateObject(Socket socket, int bufferSize)
		{
			this.socket = socket;
			buffer = new byte[bufferSize];
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the buffer to store temporary data
		/// </summary>
		public byte[] Buffer { get { return buffer; } }

		/// <summary>
		/// Gets the size of the buffer
		/// </summary>
		public int BufferSize { get { return buffer.Length; } }

		/// <summary>
		/// Gets or sets the number of bytes received
		/// </summary>
		public int Received
		{
			get { return received; }
			set
			{
				if ((value < 0) || (value > buffer.Length))
					throw new ArgumentOutOfRangeException();
				received = value;
			}
		}

		/// <summary>
		/// Gets the data source Socket object
		/// </summary>
		public Socket Socket
		{
			get { return this.socket; }
		}

		#endregion

	}
}
