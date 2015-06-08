namespace System.Net.Sockets
{
	#region Enumerations

	/// <summary>
	/// Enumerates the connection modes for a SocketTcpClient
	/// </summary>
	[Obsolete("Enum TcpClientConnectionMode is deprecated. Use Robotics.TcpClientConnectionMode instead")]
	public enum TcpClientConnectionMode
	{
		/// <summary>
		/// Performs a normal connection
		/// </summary>
		Normal,
		/// <summary>
		/// Performs a fast connection
		/// </summary>
		Fast
	}

	#endregion

	#region Delegados

	/// <summary>
	/// Represents the method that will handle the ClientConnected event of a SocketTcpServer object
	/// </summary>
	/// <param name="s">The Socket for the client connected</param>
	[Obsolete]
	public delegate void TcpClientConnectedEventHandler(Socket s);
	
	/// <summary>
	/// Represents the method that will handle the ClientConnected event of a SocketTcpServer object
	/// </summary>
	/// <param name="ep">The endpoint of the disconnected client</param>
	[Obsolete]
	public delegate void TcpClientDisconnectedEventHandler(EndPoint ep);
	
	/// <summary>
	/// Represents the method that will handle the DataReceived event of a SocketTcpServer object
	/// </summary>
	/// <param name="p">The Tcp Packet with the data received</param>
	[Obsolete]
	public delegate void TcpDataReceivedEventHandler(TcpPacket p);

	/// <summary>
	/// Represents the method that will handle the DataReceived event of a SocketUdp object
	/// </summary>
	/// <param name="e">A UdpDataReceivedEventArgs object that contains the event data</param>
	[Obsolete]
	public delegate void UdpDataReceivedEventHandler(UdpDataReceivedEventArgs e);
	
	/// <summary>
	/// Represents the method that will handle the ErrorReceived event of a SocketUdp object
	/// </summary>
	/// <param name="e">The SocketException produced by the error</param>
	[Obsolete]
	public delegate void UdpErrorReceivedEventHandler(SocketException e);

	#endregion

}