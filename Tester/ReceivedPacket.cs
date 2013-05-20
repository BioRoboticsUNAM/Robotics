using System;
using System.Collections.Generic;
using System.Text;
using Robotics.API;
using System.Net.Sockets;

namespace Tester
{
	public class ReceivedPacket
	{
		#region Variables

		private ConnectionManager connectionManager;
		private TcpPacket packet;

		#endregion

		#region Constructors
		public ReceivedPacket(ConnectionManager connectionManager, TcpPacket packet)
		{
			this.connectionManager = connectionManager;
			this.packet = packet;
		}
		#endregion

		#region Events
		#endregion

		#region Properties

		public ConnectionManager ConnectionManager
		{
			get { return this.connectionManager; }
		}

		public TcpPacket TcpPacket
		{
			get { return this.packet; }
		}

		#endregion

		#region Methodos
		#endregion
	}
}
