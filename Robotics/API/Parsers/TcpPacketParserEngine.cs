using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Robotics.Sockets;

namespace Robotics.API.Parsers
{
	/// <summary>
	/// Provides base functionality for asynchronously parse incoming TcpPackets
	/// </summary>
	public abstract partial class TcpPacketParserEngine
	{
		#region Variables

		/// <summary>
		/// Stores all the parser tasks accessible by endpoint
		/// </summary>
		private readonly Dictionary<IPEndPoint, AsyncTask> tasks;

		/// <summary>
		/// Lock for reading / writing the tasks dictionary
		/// </summary>
		private readonly ReaderWriterLock rwTasksLock;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of TcpPacketParser
		/// </summary>
		public TcpPacketParserEngine()
		{
			this.tasks = new Dictionary<IPEndPoint, AsyncTask>();
			this.rwTasksLock = new ReaderWriterLock();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating wether the ParserEngine is running.
		/// </summary>
		public abstract bool IsRunning { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Enqueues the given TcpPacket object into the parsing queue
		/// </summary>
		/// <param name="p">The packet to enqueue</param>
		public void Enqueue(TcpPacket p)
		{
			if ((p == null) || !IsRunning) return;
			
			this.rwTasksLock.AcquireReaderLock(-1);
			try
			{
				if (!this.tasks.ContainsKey(p.RemoteEndPoint))
					AddNewTask(p.RemoteEndPoint);
				AsyncTask task = this.tasks[p.RemoteEndPoint];
				task.Enqueue(p);
			}
			catch { }
			this.rwTasksLock.ReleaseReaderLock();
		}

		protected void AddNewTask(IPEndPoint ep)
		{
			bool reader;
			LockCookie coockie = new LockCookie();
			if (reader = this.rwTasksLock.IsReaderLockHeld)
				coockie = this.rwTasksLock.UpgradeToWriterLock(-1);
			else
				this.rwTasksLock.AcquireWriterLock(-1);

			try
			{
				AsyncTask task = CreateTask(ep);
				if (!this.tasks.ContainsKey(ep) && (task.EndPoint == ep))
				this.tasks.Add(ep, task);
			}
			catch { }
			if (reader)
				this.rwTasksLock.DowngradeFromWriterLock(ref coockie);
			else
				this.rwTasksLock.ReleaseWriterLock();
		}

		/// <summary>
		/// When overriden in a derived class, creates a new Task of the type required
		/// for this TcpPacketParserEngine
		/// </summary>
		/// <param name="ep">The endPoint to which the task will be bound</param>
		/// <returns>A parser task</returns>
		public abstract AsyncTask CreateTask(IPEndPoint ep);

		/// <summary>
		/// Stops parsing for the given endpoint 
		/// </summary>
		/// <param name="ep">The endpoint for which the parsing will be stopped</param>
		public void Stop(IPEndPoint ep)
		{
			AsyncTask task;
			this.rwTasksLock.AcquireWriterLock(-1);
			if (!this.tasks.ContainsKey(ep))
			{
				this.rwTasksLock.ReleaseWriterLock();
				return;
			}
			task = this.tasks[ep];
			this.tasks.Remove(ep);
			this.rwTasksLock.ReleaseWriterLock();
			task.Stop();
		}

		/// <summary>
		/// Stops parsing for all endpoints
		/// </summary>
		public void Stop()
		{
			this.rwTasksLock.AcquireWriterLock(-1);
			foreach (AsyncTask t in tasks.Values)
				t.BeginStop();
			foreach (AsyncTask t in tasks.Values)
				t.Stop();
			this.tasks.Clear();
			this.rwTasksLock.ReleaseWriterLock();
		}

		#endregion
	}
}
