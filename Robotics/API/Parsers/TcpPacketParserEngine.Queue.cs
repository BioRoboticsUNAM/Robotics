using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Robotics.Sockets;

namespace Robotics.API.Parsers
{
	partial class TcpPacketParserEngine
	{
		/// <summary>
		/// Implements a thread-safe queue for TcpPackets which allows synchronous read from them one char at a time.
		/// </summary>
		public class Queue
		{
			#region Variables

			/// <summary>
			/// The maximum capacity of the internal queue for storing TCP packets
			/// </summary>
			private readonly int maxCapacity;

			/// <summary>
			/// Stores the endpoint this AsyncTcpPacketReader is bounded to
			/// </summary>
			private readonly IPEndPoint endpoint;

			/// <summary>
			/// Stores all incomming packets
			/// </summary>
			private readonly Queue<TcpPacket> packets;

			/// <summary>
			/// Stores the current packet (last dequeued)
			/// </summary>
			private TcpPacket currentPacket;

			/// <summary>
			/// Read index over the data in the current packet
			/// </summary>
			private int ix;

			/// <summary>
			/// Object for synchronous access to the Read method
			/// </summary>
			private readonly object oRead;

			#endregion

			#region Properties

			/// <summary>
			/// Gets the maximum capacity of the internal queue for storing TCP packets
			/// </summary>
			public int MaxCapacity { get { return this.maxCapacity; } }

			/// <summary>
			/// Gets the endpoint this AsyncTcpPacketReader is bounded to
			/// </summary>
			public IPEndPoint EndPoint { get { return this.endpoint; } }

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of AsyncTcpPacketReader
			/// </summary>
			/// <param name="endpoint">Stores the endpoint this AsyncTcpPacketReader is bounded to</param>
			/// <param name="maxCapacity">The maximum capacity of the internal queue for storing TCP packets</param>
			public Queue(IPEndPoint endpoint, int maxCapacity)
			{
				if (endpoint == null)
					throw new ArgumentNullException();
				if (maxCapacity < 1)
					throw new ArgumentOutOfRangeException("maxCapacity", "Maximum capacity cannot be less than or equal to zero");
				this.maxCapacity = maxCapacity;
				this.oRead = new Object();
				this.endpoint = endpoint;
				this.packets = new Queue<TcpPacket>(maxCapacity);
				this.currentPacket = null;
				this.ix = 0;
			}

			#endregion

			#region Methods

			/// <summary>
			/// Adds the provided TcpPacket to the queue, blocking the thread when the queue is full. Packets from a
			/// different endpoint will not be added
			/// </summary>
			/// <param name="packet">The TcpPacket object to add to the queue. </param>
			public void Enqueue(TcpPacket packet)
			{
				if ((packet == null) || (packet.RemoteEndPoint == null) || (packet.RemoteEndPoint != this.endpoint))
					return;
				lock (packets)
				{
					while (packets.Count >= maxCapacity)
						Monitor.Wait(packets);
					packets.Enqueue(packet);
					// Notify threads waiting due to empty queue
					if (packets.Count == 1)
						Monitor.PulseAll(packets);
				}
			}

			/// <summary>
			/// Dequeues a packet from the queue, putting it into the currentPacket variable,
			/// blocking the thread when the queue is empty
			/// </summary>
			private void NextPacket()
			{
				lock (packets)
				{
					while (packets.Count < 1)
						Monitor.Wait(packets);
					currentPacket = packets.Dequeue();
					// Notify threads waiting due to full queue
					if (packets.Count == maxCapacity - 1)
						Monitor.PulseAll(packets);
				}
			}

			/// <summary>
			/// Reads the next byte from the received TcpPacket queue
			/// </summary>
			/// <returns>The next byte available, or -1 if there was an error.</returns>
			public int Read()
			{
				lock (oRead)
				{
					try
					{
						if (!CanContinue())
							return -1;
						return currentPacket.Data[ix++];
					}
					catch (ThreadAbortException) { return -1; }
					catch (ThreadInterruptedException) { return -1; }
					catch { return -1; }
				} // End lock
			}

			/// <summary>
			/// Validates whether the read operation can continue or not,
			/// changing of packet and reseting the ix counter when the end of the packet is reached.
			/// </summary>
			/// <returns>A value indicating if the read operation can continue</returns>
			private bool CanContinue()
			{
				if ((currentPacket == null) || (ix >= currentPacket.Data.Length))
				{
					ix = 0;
					NextPacket();
					if (currentPacket == null)
						return false;
				}
				return true;
			}

			#endregion
		}
	}
}