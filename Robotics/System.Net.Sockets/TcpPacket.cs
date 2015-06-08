using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace System.Net.Sockets
{
	/// <summary>
	/// Implementa un paquete TCP con la informacion relevante recibida
	/// </summary>
	[Obsolete]
	public class TcpPacket
	{
		#region Variables
		private IPEndPoint remoteEndPoint;
		private IPEndPoint localEndPoint;
		private Socket socket;
		private byte[] data;
		private string[] dataStrings;
		private bool isAnsi;
		private static char[] charZero = new char[] { (char)0 };

		#endregion

		#region Constructores

		[Obsolete]
		internal TcpPacket(Socket socket, byte[] data)
			: this(socket, data, 0, data.Length) { }

		[Obsolete]
		internal TcpPacket(Socket socket, byte[] data, int offset, int count)
		{
			int i;
			bool zeroFound = false;

			this.socket = socket;
			try
			{
				this.remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
				this.localEndPoint = (IPEndPoint)socket.LocalEndPoint;
			}
			catch { }
			if ((data != null) && (data.Length >= 0))
			{
				if (offset > data.Length) throw new Exception("Offset out of range");
				if ((offset + count) > data.Length) throw new Exception("Offset out of range");
				this.data = new byte[count];
				isAnsi = true;
				for (i = 0; i < count; ++i)
				{
					this.data[i] = data[offset + i];
					if (this.data[i] == 0) zeroFound = true;
					if (!zeroFound && (this.data[i] > 128)) isAnsi = false;
				}
				dataStrings = System.Text.ASCIIEncoding.Default.GetString(data).Split(charZero, StringSplitOptions.RemoveEmptyEntries);
			}
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets the data received in raw format
		/// </summary>
		[Obsolete]
		public byte[] Data
		{
			get { return data; }
		}

		/// <summary>
		/// Gets the data formatted as string
		/// </summary>
		[Obsolete]
		public string DataString
		{
			get
			{
				if (dataStrings.Length == 0) return "";
				return dataStrings[0];
			}
		}

		/// <summary>
		/// Gets the all strings stored in the data
		/// </summary>
		[Obsolete]
		public string[] DataStrings
		{
			get
			{
				return dataStrings;
			}
		}

		/// <summary>
		/// Gets a value indicating where the Tcp packet contains only characters between 0 and 127
		/// </summary>
		[Obsolete]
		public bool IsAnsi
		{
			get { return isAnsi; }
		}

		/// <summary>
		/// Gets the socket local endpoint 
		/// </summary>
		[Obsolete]
		public IPEndPoint LocalEndPoint
		{
			get { return localEndPoint; }
		}

		/// <summary>
		/// Gets the socket remote endpoint 
		/// </summary>
		[Obsolete]
		public IPEndPoint RemoteEndPoint
		{
			get { return remoteEndPoint; }
		}

		/// <summary>
		/// Gets the port where the sender sent the packet
		/// </summary>
		[Obsolete]
		public int Port
		{
			get { return ((IPEndPoint)socket.RemoteEndPoint).Port; }
		}

		/// <summary>
		/// Gets the IP Address of the sender of the packet
		/// </summary>
		[Obsolete]
		public IPAddress SenderIP
		{
			get { return ((IPEndPoint)socket.RemoteEndPoint).Address; }
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the i-th byte contained in the TcpPacket
		/// </summary>
		/// <param name="ix">Zero-based index of the data to get</param>
		/// <returns>The i-th byte contained in the TcpPacket</returns>
		[Obsolete]
		public byte this[int ix]
		{
			get
			{
				if ((ix < 0) || (ix >= this.data.Length)) throw new IndexOutOfRangeException();
				return this.data[ix];
			}
		}

		#endregion

		#region Métodos
		/// <summary>
		/// Returns a String that represents the current Object
		/// </summary>
		/// <returns>A String that represents the current Object</returns>
		[Obsolete]
		public override string ToString()
		{
			return "["+ SenderIP.ToString() +"]:" + DataString;
		}
		#endregion

		#region Operators

		/// <summary>
		/// Implicitly converts a TcpPacket into a String object
		/// </summary>
		/// <param name="p">TcpPacket to convert</param>
		/// <returns>String representation of data contained in the TcpPacket</returns>
		[Obsolete]
		public static explicit operator string(TcpPacket p)
		{
			return p.DataString;
		}

		/// <summary>
		/// Implicitly converts a TcpPacket into an array of bytes
		/// </summary>
		/// <param name="p">TcpPacket to convert</param>
		/// <returns>Array of bytes with the data contained in the TcpPacket</returns>
		[Obsolete]
		public static explicit operator byte[](TcpPacket p)
		{
			return p.data;
		}

		#endregion
	}
}
