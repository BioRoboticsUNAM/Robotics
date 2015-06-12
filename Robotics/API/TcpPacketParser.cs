using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Robotics.Sockets;

namespace Robotics.API
{
	/// <summary>
	/// Asynchronously parses incomming TcpPackets extracting command and responses
	/// </summary>
	internal partial class TcpPacketParser
	{
		#region Variables

		/// <summary>
		/// Stores all the parser tasks accessible by endpoint
		/// </summary>
		private readonly Dictionary<IPEndPoint, Task> tasks;
		/// <summary>
		/// Stores a reference to the ConnectionManager object which handles this parser
		/// </summary>
		private readonly ConnectionManager cnnMan;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of TcpPacketParser
		/// </summary>
		public TcpPacketParser(ConnectionManager cnnMan)
		{
			this.tasks = new Dictionary<IPEndPoint, Task>();
			this.cnnMan = cnnMan;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Enqueues the given TcpPacket object into the parsing queue
		/// </summary>
		/// <param name="p">The packet to enqueue</param>
		public void Enqueue(TcpPacket p)
		{
			if ((p == null) || !cnnMan.IsRunning) return;
			Task task;
			lock (this.tasks)
			{
				if (!this.tasks.ContainsKey(p.RemoteEndPoint))
					CreateTask(p.RemoteEndPoint);
				task = this.tasks[p.RemoteEndPoint];
			}
			task.Enqueue(p);
		}

		private void CreateTask(IPEndPoint ep)
		{
			if(!cnnMan.IsRunning) return;
			lock (this.tasks)
			{
				if (this.tasks.ContainsKey(ep))
					return;
				this.tasks.Add(ep, new Task(ep, cnnMan));
			}
		}

		/// <summary>
		/// Stops parsing for the given endpoint 
		/// </summary>
		/// <param name="ep">The endpoint for which the parsing will be stopped</param>
		public void Stop(IPEndPoint ep)
		{
			Task task;
			lock (this.tasks)
			{
				if (!this.tasks.ContainsKey(ep))
					return;
				task = this.tasks[ep];
				this.tasks.Remove(ep);
			}
			task.Stop();
		}

		/// <summary>
		/// Stops parsing for all endpoints
		/// </summary>
		public void Stop()
		{
			lock (this.tasks)
			{
				foreach (Task t in tasks.Values)
					t.BeginStop();
				foreach (Task t in tasks.Values)
					t.Stop();
				this.tasks.Clear();
			}
		}

		#endregion

		#region Subclasses
		#endregion
	}
}
