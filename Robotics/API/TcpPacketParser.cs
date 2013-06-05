using System;
using System.Net.Sockets;
using System.Threading;

namespace Robotics.API
{
	/// <summary>
	/// Implements a message parser for TCP packets received from a connection manager
	/// </summary>
	internal class TcpPacketParser : MessageParser<TcpPacket>
	{
		#region Variables

		/// <summary>
		/// Represents the ClientConnected method
		/// </summary>
		private readonly TcpClientConnectedEventHandler dlgClientConnected;
		/// <summary>
		/// Represents the ClientDisconnected method
		/// </summary>
		private readonly TcpClientDisconnectedEventHandler dlgClientDisconnected;
		/// <summary>
		/// Represents the DataReceived method
		/// </summary>
		private readonly ConnectionManagerDataReceivedEH dlgDataReceived;

		/// <summary>
		/// Stores the reference to the ConnectionManager object
		/// </summary>
		private IConnectionManager cnnMan;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of TcpPacketParser
		/// </summary>
		/// <param name="cnnMan">The ConnectionManager object which incomming data will be parsed</param>
		public TcpPacketParser(IConnectionManager cnnMan)
		{
			if (cnnMan == null)
				throw new ArgumentNullException();

			this.cnnMan = cnnMan;
			
			this.dlgClientConnected = new TcpClientConnectedEventHandler(ClientConnected);
			this.dlgClientDisconnected = new TcpClientDisconnectedEventHandler(ClientDisconnected);
			this.dlgDataReceived = new ConnectionManagerDataReceivedEH(DataReceived);

			this.cnnMan.DataReceived += dlgDataReceived;
			this.cnnMan.Connected += dlgClientConnected;
			this.cnnMan.ClientConnected += dlgClientConnected;
			this.cnnMan.Disconnected += dlgClientDisconnected;
			this.cnnMan.ClientDisconnected += dlgClientDisconnected;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the ConnectionManager object asociated to this CommandManager object
		/// </summary>
		public virtual IConnectionManager ConnectionManager
		{
			get { return cnnMan; }
		}

		/// <summary>
		/// Gets a value indicating if the connector is connected to the message source
		/// </summary>
		public override bool IsConnected
		{
			get { return cnnMan.IsConnected; }
		}

		/// <summary>
		/// Returns the ModuleName property of the asociated IConnectionManager object
		/// </summary>
		public override string ModuleName
		{
			get { return this.cnnMan.ModuleName; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Manages connections
		/// </summary>
		protected void ClientConnected(Socket socket)
		{
			//if (blackboardEndpoint == null)
			//	UpdateSharedVariableList();
			OnConnected();
		}

		/// <summary>
		/// Manages disconnections
		/// </summary>
		protected void ClientDisconnected(System.Net.EndPoint endpoint)
		{
			//if ((blackboardEndpoint != null) && (blackboardEndpoint == endpoint))
			//{
			//	initializationSyncEvent.Reset();
			//	blackboardEndpoint = null;
			//	shvLoaded = false;
			//}
			OnDisconnected();
		}

		/// <summary>
		/// Manages all received data
		/// </summary>
		/// <param name="cnnMan">The ConnectionManager object which provides the TcpPacket</param>
		/// <param name="packet">Tcp packet received</param>
		protected virtual void DataReceived(IConnectionManager cnnMan, TcpPacket packet)
		{
			if ((packet == null) || (packet.Data.Length < 1)) return;
			if (cnnMan != this.cnnMan) return;
			ProduceData(packet);
		}

		/// <summary>
		/// Parses a received string
		/// </summary>
		/// <param name="packet">String received</param>
		protected override void Parse(TcpPacket packet)
		{
			Command command;
			Response response;
			if ((packet.Data.Length > 1) && (packet.Data[0] == 0x7E))
				return;

			string stringToParse;
			for (int i = 0; running && (i < packet.DataStrings.Length); ++i)
			{
				stringToParse = packet.DataStrings[i];

				// Responses to manage
				if (Response.TryParse(stringToParse, out response))
				{
					response.MessageSource = cnnMan;
					response.MessageSourceMetadata = packet.RemoteEndPoint;
					OnResponseReceived(response);
					continue;
				}

				// Commands to parse
				if (Command.TryParse(stringToParse, out command))
				{
					command.MessageSource = cnnMan;
					command.MessageSourceMetadata = packet.RemoteEndPoint;

					OnCommandReceived(command);
					continue;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		public override void ReceiveResponse(Response response)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Sends a command through the IConnectionManager
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if the command was sent, false otherwise</returns>
		public override bool Send(Command command)
		{
			// 1. Set the origin of the command
			command.MessageSource = ConnectionManager;
			// 2. Send the command. If command cannot be sent, return false
			return ConnectionManager.Send(command);
		}

		/// <summary>
		/// Sends a response through the IConnectionManager
		/// </summary>
		/// <param name="response">Response to be sent</param>
		/// <returns>true if the response was sent, false otherwise</returns>
		public override bool Send(Response response)
		{
			if ((response.MessageSource == null) || response.MessageSource == this)
				return cnnMan.Send(response);
			try
			{
				response.MessageSource.ReceiveResponse(response);
				return true;
			}
			catch { return false; }
		}

		/// <summary>
		/// Starts the parser
		/// </summary>
		public override void Start()
		{
			if (!this.cnnMan.IsRunning)
				this.cnnMan.Start();
			base.Start();
		}

		/// <summary>
		/// Stops the parser
		/// </summary>
		public override void Stop()
		{
			if (!this.cnnMan.IsRunning)
				this.cnnMan.Stop();
			base.Stop();
		}

		#endregion
	}
}
