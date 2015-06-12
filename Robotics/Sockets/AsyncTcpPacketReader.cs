using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Robotics.Sockets
{
	/// <summary>
	/// Implements an async reader for TcpPackets
	/// </summary>
	internal class AsyncTcpPacketReader
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
		public AsyncTcpPacketReader(IPEndPoint endpoint, int maxCapacity)
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
		/// Reads the next UTF8 character from the packet queue
		/// </summary>
		/// <returns>An UTF8 character if valid encoded data was found, -1 otherwise.</returns>
		public int ReadUTF8()
		{
			lock (oRead)
			{
				if (!CanContinue())
					return -1;

				int seqLength = CountHighOrder1s(currentPacket.Data[ix]);
				switch (seqLength)
				{
					// Bit pattern 0xxxxxxx for ASCII, just read.
					case 0: return currentPacket.Data[ix++];
					// Bit pattern 110xxxxx
					case 2:
					// Bit pattern 1110xxxx
					case 3:
					// Bit pattern 11110xxx
					case 4: return ReadXBytesUTF8(seqLength);
					// Non UTF8
					default: break;
				} // End Switch
			} // End lock
			return -1;
		}

		/// <summary>
		/// Count the number of 1s at the left on the given byte
		/// </summary>
		/// <param name="b">The byte on which left-most 1 bits will be counted</param>
		/// <returns>The number of high-order 1s</returns>
		private int CountHighOrder1s(byte b)
		{
			int count = 0;
			while ((b & 0x80) == 0x80)
			{
				++count;
				b <<= 1;
			}
			return count;
		}

		/// <summary>
		/// Reads an UTF8 character from the currentPacket data array at the ix position based on the
		/// given sequence length
		/// </summary>
		/// <param name="seqLength">The sequence length (number of high-order 1s in the byte pointed by ix)</param>
		/// <returns>An UTF8 Character if there is a valid sequence, or -1 if not</returns>
		private int ReadXBytesUTF8(int seqLength)
		{
			if ((ix + seqLength) >= currentPacket.Data.Length)
				return -1;
			for (byte i = 1; i < seqLength; ++i)
			{
				if ((currentPacket.Data[ix + i] & 0xC0) != 0x80)
					return -1;
			}
			return System.Text.UTF8Encoding.UTF8.GetChars(currentPacket.Data, ix, seqLength)[0];
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
