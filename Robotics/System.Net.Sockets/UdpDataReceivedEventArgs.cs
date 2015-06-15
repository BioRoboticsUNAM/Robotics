using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace System.Net.Sockets.Obsolete
{
	/// <summary>
	/// Provides data for the System.Net.Sockets.UdpDataReceived event
	/// </summary>
	public class UdpDataReceivedEventArgs
	{
		#region Variables

		private byte[] data;
		private IPAddress senderAddress;
		private bool truncated;
		// private static char[] charZero = new char[] { (char)0, (char)128 };

		#endregion

		#region Constructores

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="senderAddress">The IP Address of the message sender</param>
		/// <param name="data">The message</param>
		/// <param name="truncated">True if message was received incomplete</param>
		internal UdpDataReceivedEventArgs(IPAddress senderAddress, byte[] data, bool truncated)
		{
			this.senderAddress = new IPAddress(senderAddress.GetAddressBytes());
			this.data = data;
			this.truncated = truncated;
		}

		#endregion

		#region Propiedades

		/// <summary>
		/// Gets the IP Address of the message sender
		/// </summary>
		public IPAddress SenderIPAddress
		{
			get { return senderAddress; }
		}

		/// <summary>
		/// Gets the byte array received
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
				string s = ASCIIEncoding.ASCII.GetString(data);
				s = s.Substring(0, s.IndexOf('\0'));
				s = s.Trim();
				return s;
			}
		}

		/// <summary>
		/// True if message was truncated or received incomplete
		/// </summary>
		public bool Truncated
		{
			get { return truncated; }
		}

		/// <summary>
		/// Returns a string representation of the current instance
		/// </summary>
		/// <returns>String that represents the object</returns>
		public override string ToString()
		{
			int i = 0;
			while (data[i] != 0) ++i;
			if (i == 0) return "";
			//new String(
			return ASCIIEncoding.Default.GetString(data, 0, i);
		}
		#endregion
	}
}