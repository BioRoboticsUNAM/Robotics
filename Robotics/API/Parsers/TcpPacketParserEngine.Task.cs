using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Robotics.Sockets;

namespace Robotics.API.Parsers
{
	partial class TcpPacketParserEngine
	{
		/// <summary>
		/// Serves a base class for asynchonous parsing of incomming data
		/// </summary>
		public abstract class AsyncTask
		{
			#region Variables

			/// <summary>
			/// Thread for asynchronous parsing
			/// </summary>
			private readonly Thread mainThread;
			/// <summary>
			/// reder used to fetch data from the Tcp packets
			/// </summary>
			private readonly Queue reader;
			/// <summary>
			/// Indicates whether the task should keep running
			/// </summary>
			private bool running;

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of AsyncTask
			/// </summary>
			/// <param name="endpoint">The endpoint this Task will be bounded to</param>
			public AsyncTask(IPEndPoint endpoint)
			{
				this.reader = new Queue(endpoint, 10);
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

			/// <summary>
			/// Gets a value indicating wether the Task is running.
			/// </summary>
			public bool IsRunning { get { return this.running; } }

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
				while (running)
					ParseNext();				
			}

			/// <summary>
			/// When overriden in a derived class, performs the parsing operation.
			/// This method will be called in a separated thread while the task is running.
			/// </summary>
			protected abstract void ParseNext();

			/// <summary>
			/// Reads the next byte from the received TcpPacket queue
			/// </summary>
			/// <returns>The next byte available, or -1 if there was an error.</returns>
			public int Read()
			{
				return this.reader.Read();
			}

			/// <summary>
			/// Reads the next UTF8 character from the packet queue
			/// </summary>
			/// <returns>An UTF8 character if valid encoded data was found, -1 otherwise.</returns>
			public int ReadUTF8()
			{	
				byte[] data;
				return ReadUTF8(out data);
			}

			/// <summary>
			/// Reads the next UTF8 character from the packet queue
			/// </summary>
			/// <param name="data">The data which was fetched from the input while attempting to read an UTF8 character</param>
			/// <returns>An UTF8 character if valid encoded data was found, -1 otherwise.</returns>
			public int ReadUTF8(out byte[] data)
			{
				data = null;
				int next = Read();
				if(next < 0) return -1;
					int seqLength = CountHighOrder1s((byte)next);
					switch (seqLength)
					{
						// Bit pattern 0xxxxxxx for ASCII, just read.
						case 0:
							data = new byte[1] { (byte)next };
							return next;
						// Bit pattern 110xxxxx
						case 2:
						// Bit pattern 1110xxxx
						case 3:
						// Bit pattern 11110xxx
						case 4:
							data = new byte[seqLength];
							data[0] = (byte)next;
							return ReadXBytesUTF8(seqLength, data);
						// Non UTF8
						default: break;
					} // End Switch
				return -1;
			}

			/// <summary>
			/// Reads an UTF8 character from the currentPacket data array at the ix position based on the
			/// given sequence length
			/// </summary>
			/// <param name="seqLength">The sequence length (number of high-order 1s in the byte pointed by ix)</param>
			/// <param name="data">The data which was fetched from the input while attempting to read an UTF8 character</param>
			/// <returns>An UTF8 Character if there is a valid sequence, or -1 if not</returns>
			private int ReadXBytesUTF8(int seqLength, byte[] data)
			{
				int next;
				for (byte i = 1; i < seqLength; ++i)
				{
					if((next = Read()) < 0)return -1;
					data[i] = (byte)next;
					if ((data[i] & 0xC0) != 0x80)
						return -1;
				}
				return System.Text.UTF8Encoding.UTF8.GetChars(data)[0];
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
