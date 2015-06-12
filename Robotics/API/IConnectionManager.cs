using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Robotics;
using Robotics.Sockets;

namespace Robotics.API
{
	/// <summary>
	/// Represents an object that manages TCP connections.
	/// </summary>
	public interface IConnectionManager : IConnector, IService, IMessageSource
	{
		#region Properties

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
		/// Gets or sets the Tcp port for incoming data used by Tcp Server.
		/// </summary>
		int Port { get; set; }

		#endregion

		#region Events
		
		/// <summary>
		/// Occurs when a remote client gets connected to local TCP Server
		/// </summary>
		event EventHandler<IConnectionManager, IPEndPoint> ClientConnected;
		
		/// <summary>
		/// Occurs when a remote client disconnects from local TCP Server
		/// </summary>
		event EventHandler<IConnectionManager, IPEndPoint> ClientDisconnected;
		
		/// <summary>
		/// Occurs when data is received
		/// </summary>
		event EventHandler<IConnectionManager, Robotics.Sockets.TcpPacket> DataReceived;
		
		/// <summary>
		/// Occurs when the status of the IConnectionManager changes
		/// </summary>
		event Action<IConnectionManager> StatusChanged;

		/// <summary>
		/// Occurs when the IConnectionManager is started
		/// </summary>
		event Action<IConnectionManager> Started;

		/// <summary>
		/// Occurs when the IConnectionManager is stopped
		/// </summary>
		event Action<IConnectionManager> Stopped;

		/// <summary>
		/// Occurs when a command is sent
		/// </summary>
		event EventHandler<IConnectionManager, Command> CommandSent;

		/// <summary>
		/// Occurs when a response is sent
		/// </summary>
		event EventHandler<IConnectionManager, Response> ResponseSent;

		#endregion

		#region Methods

		#endregion
	}
}