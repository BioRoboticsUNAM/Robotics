using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Robotics.Sockets;

namespace Robotics.API
{
	partial class TcpPacketParser
	{
		protected class Task
		{
			#region Variables

			/// <summary>
			/// Thread for asynchronous parsing
			/// </summary>
			private readonly Thread mainThread;
			/// <summary>
			/// reder used to fetch data from the Tcp packets
			/// </summary>
			private readonly AsyncTcpPacketReader reader;
			/// <summary>
			/// Indicates whether the task should keep running
			/// </summary>
			private bool running;
			/// <summary>
			/// Stores a reference to the ConnectionManager object which handles this parser
			/// </summary>
			private readonly ConnectionManager cnnMan;

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of Task
			/// </summary>
			/// <param name="endpoint">The endpoint this Task will be bounded to</param>
			public Task(IPEndPoint endpoint, ConnectionManager cnnMan)
			{
				this.cnnMan = cnnMan;
				this.reader = new AsyncTcpPacketReader(endpoint, 10);
				this.running = true;
				this.mainThread = new Thread(new ThreadStart(MainThreadTask));
				this.mainThread.IsBackground = true;
				this.mainThread.Start();
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets the endpoint this Task is bounded to
			/// </summary>
			public IPEndPoint EndPoint { get { return this.reader.EndPoint; } }

			#endregion

			#region Methods

			/// <summary>
			/// Request the thread to stop.
			/// </summary>
			public void BeginStop()
			{
				this.running = false;
			}

			/// <summary>
			/// Adds the provided TcpPacket to the parsing queue, blocking the thread when the queue is full.
			/// Packets from a different endpoint will not be added
			/// </summary>
			/// <param name="packet">The TcpPacket object to add to the queue. </param>
			public void Enqueue(TcpPacket packet)
			{
				this.reader.Enqueue(packet);
			}

			private void MainThreadTask()
			{
				StringBuilder sb = new StringBuilder(1024);
				int next = 0;
				while (running)
				{
					next = reader.Read();
					if (next <= 0)
					{
						Parse(sb.ToString());
						sb.Length = 0;
						do
						{
							next = reader.Read();
						}
						while (running && (next <= 0));
					}
					sb.Append((char)next);
				}
			}

			private void Parse(string s)
			{
				Command command;
				Response response;

				// Responses to manage
				if (Response.TryParse(s, out response))
				{
					response.MessageSource = cnnMan;
					response.MessageSourceMetadata = this.EndPoint;
					cnnMan.OnResponseReceived(response);
					return;
				}

				// Commands to parse
				if (Command.TryParse(s, out command))
				{
					command.MessageSource = cnnMan;
					command.MessageSourceMetadata = this.EndPoint;

					cnnMan.OnCommandReceived(command);
					return;
				}
			}

			/// <summary>
			/// Stops the task
			/// </summary>
			public void Stop()
			{
				BeginStop();
				if(this.mainThread.IsAlive)
					this.mainThread.Interrupt();
				this.mainThread.Join();
			}

			#endregion
		}
	}
}
