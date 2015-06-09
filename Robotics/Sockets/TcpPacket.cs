using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Robotics.Sockets
{
	/// <summary>
	/// Encapsulates received data through a TCP socket
	/// </summary>
	public class TcpPacket
	{
		#region Variables
		/// <summary>
		/// The remote endpoint (where the data comes from)
		/// </summary>
		private IPEndPoint remoteEndPoint;
		
		/// /// <summary>
		/// The local endpoint (where the data arrived)
		/// </summary>
		private IPEndPoint localEndPoint;

		/// <summary>
		/// The received data
		/// </summary>
		private byte[] data;

		#endregion

		#region Constructores

		/// <summary>
		/// Initalizes a new instance of TcpPacket
		/// </summary>
		/// <param name="socket">The socket where the data comes from</param>
		/// <param name="data">The received data</param>
		internal TcpPacket(Socket socket, byte[] data)
			: this(socket, data, 0, data.Length) { }

		/// <summary>
		/// Initalizes a new instance of TcpPacket
		/// </summary>
		/// <param name="socket">The socket where the data comes from</param>
		/// <param name="data">The received data</param>
		/// <param name="offset">The offset where the received data starts within the buffer</param>
		/// <param name="count">The number of bytes to copy within the buffer starting from the offset</param>
		internal TcpPacket(Socket socket, byte[] data, int offset, int count)
		{
			if (count < 0) throw new Exception("Count must be equal or greater than zero");
			if (offset < 0) throw new Exception("Offset must be equal or greater than zero");
			if ((offset + count) > data.Length) throw new Exception("Offset/count out of range");
			try
			{
				this.remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
				this.localEndPoint = (IPEndPoint)socket.LocalEndPoint;
			}
			catch { }
			if ((data != null) && (data.Length >= 0))
			{
				this.data = new byte[count];
				for (int i = 0; i < count; ++i)
					this.data[i] = data[offset + i];
			}
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets the received data
		/// </summary>
		public byte[] Data
		{
			get { return data; }
		}

		/// <summary>
		/// Gets the local endpoint (where the data arrived)
		/// </summary>
		public IPEndPoint LocalEndPoint
		{
			get { return localEndPoint; }
		}

		/// <summary>
		/// Gets the remote endpoint (where the data comes from)
		/// </summary>
		public IPEndPoint RemoteEndPoint
		{
			get { return remoteEndPoint; }
		}
		
		/// <summary>
		/// Gets the IP Address of the sender of the packet
		/// </summary>
		public IPAddress SenderIP
		{
			get { return RemoteEndPoint.Address; }
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the i-th byte contained in the TcpPacket
		/// </summary>
		/// <param name="ix">Zero-based index of the data to get</param>
		/// <returns>The i-th byte contained in the TcpPacket</returns>
		public byte this[int ix] { get { return this.data[ix]; } }

		#endregion

		#region Métodos
		/// <summary>
		/// Returns a String that represents the current Object
		/// </summary>
		/// <returns>A String that represents the current Object</returns>
		public override string ToString()
		{
			string[] sizes = {"B", "k", "M", "G"};
			double size = this.data.LongLength;
			int ix;
			for(ix = 0; (ix < 3) && (size > 1024); ++ix)
				size/= 1024.0;
			string cut = UTF8Encoding.UTF8.GetString(data, 0, (int)Math.Min(50, data.LongLength));
			return String.Format("[{0}] {1}{2} Data={3}{4}",
				remoteEndPoint.Address,
				(int)size,
				sizes[ix],
				cut,
				data.LongLength > cut.Length ? "..." : String.Empty);
		}
		#endregion
	}
}
