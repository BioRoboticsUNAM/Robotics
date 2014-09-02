using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using Robotics;

namespace Robotics.API
{
	/// <summary>
	/// Specifies how a IConnectionManager will manage connections
	/// </summary>
	public enum ConnectionManagerMode
	{
		/// <summary>
		/// Works in bidirectional mode. Only one socket is used, same port for incomming an outgoing transmissions
		/// </summary>
		Bidireactional,
		/// <summary>
		/// Works in unirectional mode. Two sockets are used, one port for incomming transmissions and another for outgoing transmissions
		/// </summary>
		Unidirectional
	};

	/// <summary>
	/// Represents an object that manages TCP connections.
	/// </summary>
	public interface IConnectionManager : IService, IMessageSource
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating if the IConnectionManager is woking in bidirectional mode
		/// </summary>
		bool Bidirectional { get; }

		/// <summary>
		/// Gets the number of clients connected to the local TCP Server
		/// </summary>
		int ConnectedClientsCount { get; }

		/// <summary>
		/// Gets or sets the CommandManager object which will manage incoming data.
		/// The IConnectionManager must not be running when the CommandManager is set.
		/// </summary>
		CommandManager CommandManager { get; }

		/// <summary>
		/// Enables or disables the reception of data from the output socket
		/// </summary>
		bool OutputSocketReceptionEnabled { get; set; }

		/// <summary>
		/// Gets a value indicating if local TCP client has connected to remote server
		/// </summary>
		/// <remarks>When working on bidirectional mode always return false</remarks>
		bool IsConnected { get; }

		/// <summary>
		/// Gets a value indicating if TCP Server has been started and is running
		/// </summary>
		bool IsServerStarted { get; }

		/// <summary>
		/// Gets a value indicating the mode of the current IConnectionManager instance
		/// </summary>
		ConnectionManagerMode Mode { get; }

		/// <summary>
		/// Gets or sets the Tcp port for incoming data used by Tcp Server.
		/// </summary>
		int PortIn { get; set; }

		/// <summary>
		/// Gets or sets the Tcp port for outgoing data used by Tcp Client.
		/// </summary>
		int PortOut { get; set; }

		/// <summary>
		/// Gets or sets the IP Address of the remote computer to connect using the socket client.
		/// </summary>
		IPAddress TcpServerAddress { get; set; }

		#endregion

		#region Events
		
		/// <summary>
		/// Occurs when a remote client gets connected to local TCP Server
		/// </summary>
		event TcpClientConnectedEventHandler ClientConnected;
		
		/// <summary>
		/// Occurs when a remote client disconnects from local TCP Server
		/// </summary>
		event TcpClientDisconnectedEventHandler ClientDisconnected;
		
		/// <summary>
		/// Occurs when the local client connects to remote server.
		/// This event is rised only when the IConnectionManager works in Unidirectional mode.
		/// </summary>
		event TcpClientConnectedEventHandler Connected;
		
		/// <summary>
		/// Occurs when the local client connects to remote server.
		/// This event is rised only when the IConnectionManager works in Unidirectional mode.
		/// </summary>
		event TcpClientDisconnectedEventHandler Disconnected;
		
		/// <summary>
		/// Occurs when data is received
		/// </summary>
		event ConnectionManagerDataReceivedEH DataReceived;
		
		/// <summary>
		/// Occurs when the status of the IConnectionManager changes
		/// </summary>
		event ConnectionManagerStatusChangedEventHandler StatusChanged;

		/// <summary>
		/// Occurs when the IConnectionManager is started
		/// </summary>
		event ConnectionManagerStatusChangedEventHandler Started;

		/// <summary>
		/// Occurs when the IConnectionManager is stopped
		/// </summary>
		event ConnectionManagerStatusChangedEventHandler Stopped;

		/// <summary>
		/// Occurs when a command is sent
		/// </summary>
		event CnnManCommandSentEventHandler CommandSent;

		/// <summary>
		/// Occurs when a response is sent
		/// </summary>
		event CnnManResponseSentEventHandler ResponseSent;

		#endregion

		#region Methods

		#region Socket Methods

		/// <summary>
		/// Sends a Command
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if command was sent successfully, false otherwise</returns>
		bool Send(Command command);

		/// <summary>
		/// Sends a Response
		/// </summary>
		/// <param name="response">Response to be sent</param>
		/// <returns>true if response was sent successfully, false otherwise</returns>
		bool Send(Response response);

		/// <summary>
		/// Sends data to all clients connected to the server
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="count">The offset in the byte array to begin sending</param>
		/// <param name="offset">The number of bytes to send</param>
		/// <returns>The number of clients to which the data was sent</returns>
		int SendToAllClients(byte[] buffer, int offset, int count);

		/// <summary>
		/// Sends data to all clients connected to the server
		/// </summary>
		/// <param name="s">The string to send</param>
		/// <returns>The number of clients to which the string was sent</returns>
		int SendToAllClients(string s);

		/// <summary>
		/// Sends data through the tcp client
		/// </summary>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="count">The offset in the byte array to begin sending</param>
		/// <param name="offset">The number of bytes to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		bool SendTroughClient(byte[] buffer, int offset, int count);

		/// <summary>
		/// Sends data through the tcp client
		/// </summary>
		/// <param name="s">The string to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		bool SendTroughClient(string s);

		/// <summary>
		/// Sends data to the specified remote endpoint
		/// </summary>
		/// <param name="remoteEndPoint">The destination endpoint</param>
		/// <param name="buffer">The byte array to send</param>
		/// <param name="count">The offset in the byte array to begin sending</param>
		/// <param name="offset">The number of bytes to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		bool SendTo(IPEndPoint remoteEndPoint, byte[] buffer, int offset, int count);

		/// <summary>
		/// Sends data to the specified remote endpoint
		/// </summary>
		/// <param name="remoteEndPoint">The destination endpoint</param>
		/// <param name="s">The string to send</param>
		/// <returns>true if data was sent successfully, false otherwise</returns>
		bool SendTo(IPEndPoint remoteEndPoint, string s);

		#endregion

		#endregion
	}
}