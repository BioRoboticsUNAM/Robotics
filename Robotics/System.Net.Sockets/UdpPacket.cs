using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace System.Net.Sockets.Obsolete
{
	/// <summary>
	/// Implementa un paquete TCP con la informacion relevante recibida
	/// </summary>
	public class UdpPacket
	{
		#region Variables
		private Socket socket;
		private byte[] data;
		//private static char[] charZero = new char[] { (char)0, '\0', (char)128 };

		#endregion

		#region Constructores
		
		internal UdpPacket(Socket socket, byte[] data)
		{
			int i;
			this.socket = socket;
			if ((data != null) && (data.Length >= 0))
			{
				this.data = new byte[data.Length];
				for (i = 0; i < data.Length; ++i)
					this.data[i] = data[i];
			}
		}

		internal UdpPacket(Socket socket, byte[] data, int offset, int count)
		{
			int i;
			this.socket = socket;
			if ((data != null) && (data.Length >= 0))
			{
				if (offset > data.Length) throw new Exception("Offset out of range");
				if ((offset + count) > data.Length) count = data.Length - offset;
				this.data = new byte[count];
				for (i = 0; i < count; ++i)
					this.data[i] = data[offset + i];
			}
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets the data received in raw format
		/// </summary>
		public byte[] Data
		{
			get { return data; }
		}

		/// <summary>
		/// Gets the data formatted as string
		/// </summary>
		public string DataString
		{
			get
			{
				if (data.Length < 1) return "";
				try
				{
					string s = ASCIIEncoding.ASCII.GetString(data);
					s = s.Substring(0, s.IndexOf('\0'));
					s = s.Trim();
					return s;
				}catch{return "";}
			}
		}

		/// <summary>
		/// Gets the socket local endpoint 
		/// </summary>
		public IPEndPoint LocalEndPoint
		{
			get { return (IPEndPoint)socket.LocalEndPoint; }
		}

		/// <summary>
		/// Gets the socket remote endpoint 
		/// </summary>
		public IPEndPoint RemoteEndPoint
		{
			get { return (IPEndPoint)socket.RemoteEndPoint; }
		}

		/// <summary>
		/// Gets the port where the sender sent the packet
		/// </summary>
		public int Port
		{
			get { return ((IPEndPoint)socket.RemoteEndPoint).Port; }
		}

		/// <summary>
		/// Gets the IP Address of the sender of the packet
		/// </summary>
		public IPAddress SenderIP
		{
			get { return ((IPEndPoint)socket.RemoteEndPoint).Address; }
		}

		#endregion

		#region Métodos
		/// <summary>
		/// Returns a String that represents the current Object
		/// </summary>
		/// <returns>A String that represents the current Object</returns>
		public override string ToString()
		{
			return "["+ SenderIP.ToString() +"]:" + DataString;
		}
		#endregion
	}
}
