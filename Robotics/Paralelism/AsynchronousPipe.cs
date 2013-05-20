using System;
using System.Collections.Generic;
using System.Threading;

namespace Robotics.Paralelism
{
	/// <summary>
	/// Implements an asynchronous pipe which can be used to communicate two Filter objects.
	/// It uses a FIFO buffer to store data
	/// </summary>
	/// <typeparam name="T">The type of data transmited through the pipe</typeparam>
	public sealed class AsynchronousPipe<T> : IPipe<T>
	{
		#region Variables

		/// <summary>
		/// Temporary stores data
		/// </summary>
		private Queue<T> buffer;

		/// <summary>
		/// The capacity of the pipe
		/// </summary>
		private int capacity;

		/// <summary>
		/// Object used for synchronous access to the read method
		/// </summary>
		private object rLock;

		/// <summary>
		/// Event used for synchronization when queue is full
		/// </summary>
		private ManualResetEvent queueHasData;

		/// <summary>
		/// Event used for synchronization when queue is empty
		/// </summary>
		private ManualResetEvent queueHasSpace;

		/// <summary>
		/// Lock used to allow only one thread modify the queue at a time
		/// </summary>
		private ReaderWriterLock queueLock;

		/// <summary>
		/// Object used for synchronous access to the write method
		/// </summary>
		private object wLock;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of AsynchronousPipe
		/// </summary>
		/// <param name="capacity">The maximum number of elements the pipe can store in its internal queue</param>
		public AsynchronousPipe(int capacity)
		{
			if (capacity < 1)
				throw new ArgumentOutOfRangeException("Capacity can not be less than 1");
			this.rLock = new Object();
			this.wLock = new Object();
			this.queueHasData = new ManualResetEvent(false);
			this.queueHasSpace = new ManualResetEvent(true);
			this.capacity = capacity;
			this.queueLock = new ReaderWriterLock();
			this.buffer = new Queue<T>(capacity + 1);
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the maximum capacity of the pipe
		/// </summary>
		public int Capacity
		{
			get { return this.capacity; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Reads data from the pipe without removing it
		/// If the queue of the pipe is empty, it blocks the thread until another thread writes data
		/// </summary>
		/// <returns>The data written on the other side of the pipe</returns>
		public T Peek()
		{
			T data;

			// 1. Wait until there is data in the queue
			queueHasData.WaitOne();

			// 2. Fetch data from the queue
			queueLock.AcquireReaderLock(-1);
			data = buffer.Peek();
			queueLock.ReleaseReaderLock();

			// 3. Return peeked data
			return data;
		}

		/// <summary>
		/// Reads data from the pipe.
		/// If the queue of the pipe is empty, it blocks the thread until another thread writes data
		/// </summary>
		/// <returns>The data written on the other side of the pipe</returns>
		public T Read()
		{
			T data;

			// 1. Only one thread can access read methods at a time
			Monitor.Enter(rLock);

			// 2. Wait until there is data in the queue
			queueHasData.WaitOne();

			// 3. Fetch data from the queue
			queueLock.AcquireWriterLock(-1);
			data = buffer.Dequeue();
			queueLock.ReleaseWriterLock();

			// 4. If there is no more data in queue, reset event
			if (buffer.Count == 0)
				queueHasData.Reset();

			// 5. Notify there is space in the queue
			queueHasSpace.Set();

			// 6. Release lock. Allow other threads to read.
			Monitor.PulseAll(rLock);
			Monitor.Exit(rLock);
			// 7. Return readed data
			return data;
		}

		/// <summary>
		/// Writes data to the queue of the pipe.
		/// If the queue of pipe is full, it blocks the thread until another thread reads data
		/// </summary>
		/// <param name="data">The data to be read on the other side of the pipe</param>
		public void Write(T data)
		{
			// 1. Only one thread can access this method at a time
			Monitor.Enter(wLock);
			
			// 2. Wait until there is space in the queue
			queueHasSpace.WaitOne();

			// 3. Add data to queue
			queueLock.AcquireWriterLock(-1);
			buffer.Enqueue(data);
			queueLock.ReleaseWriterLock();

			// 4. If there is no more space in queue, reset event
			if (buffer.Count == capacity)
				queueHasSpace.Reset();

			// 5. Notify there is data in queue
			queueHasData.Set();

			// 6. Release lock. Allow other threads to write.
			Monitor.PulseAll(wLock);
			Monitor.Exit(wLock);
		}

		/// <summary>
		/// Tries to read data from the pipe before the timeout elapses without removing it.
		/// If the queue of the pipe is empty, it blocks the thread until another thread writes data
		/// </summary>
		/// <param name="timeout">The maximum amount of time in milliseconds to wait for available data</param>
		/// <param name="data">When this method returns contains the data written on the other side of the pipe
		/// if the read succeded, or the default value of T if the timeout elapses</param>
		/// <returns>true if the read operation suceeded, false otherwise</returns>
		public bool TryPeek(int timeout, out T data)
		{
			// 1. Wait until there is data in the queue
			if (!queueHasData.WaitOne())
			{
				// 1.1. No data arrived before timeout. Return default
				data = default(T);
				return false;
			}

			// 2. Fetch data from the queue
			queueLock.AcquireReaderLock(-1);
			data = buffer.Peek();
			queueLock.ReleaseReaderLock();

			// 3. Return
			return true;
		}

		/// <summary>
		/// Tries to read data from the pipe before the timeout elapses.
		/// If the queue of the pipe is empty, it blocks the thread until another thread writes data
		/// </summary>
		/// <param name="timeout">The maximum amount of time in milliseconds to wait for available data</param>
		/// <param name="data">When this method returns contains the data written on the other side of the pipe
		/// if the read succeded, or the default value of T if the timeout elapses</param>
		/// <returns>true if the read operation suceeded, false otherwise</returns>
		public bool TryRead(int timeout, out T data)
		{
			// 1. Only one thread can access read methods at a time
			Monitor.Enter(rLock);

			// 2. Wait until there is data in the queue
			if (!queueHasData.WaitOne())
			{
				// 2.1. No data arrived before timeout. Return default
				data = default(T);
				Monitor.PulseAll(rLock);
				Monitor.Exit(rLock);
				return false;
			}

			// 3. Fetch data from the queue
			queueLock.AcquireWriterLock(-1);
			data = buffer.Dequeue();
			queueLock.ReleaseWriterLock();

			// 4. If there is no more data in queue, reset event
			if (buffer.Count == 0)
				queueHasData.Reset();

			// 5. Notify there is space in the queue
			queueHasSpace.Set();

			// 6. Release lock. Allow other threads to read.
			Monitor.PulseAll(rLock);
			Monitor.Exit(rLock);

			// 7. Return
			return true;
		}

		/// <summary>
		/// Tries to write data to the pipe before the timeout elapses.
		/// If the queue of pipe is full, it blocks the thread until another thread reads data
		/// </summary>
		/// <param name="timeout">The maximum amount of time in milliseconds to wait for available space in the queue</param>
		/// <param name="data">The data to be readed on the other side of the pipe</param>
		/// <returns>true if the write operation suceeded, false otherwise</returns>
		public bool TryWrite(T data, int timeout)
		{
			//bool result;

			// 1. Only one thread can access this method at a time
			Monitor.Enter(wLock);

			// 2. Wait until there is space in the queue
			if (!queueHasSpace.WaitOne(timeout))
			{
				// 2.1. No free space availiable before timeout. Release lock and return false
				Monitor.PulseAll(wLock);
				Monitor.Exit(wLock);
				return false;
			}

			// 3. Add data to queue
			queueLock.AcquireWriterLock(-1);
			buffer.Enqueue(data);
			queueLock.ReleaseWriterLock();

			// 4. If there is no more space in queue, reset event
			if (buffer.Count == capacity)
				queueHasSpace.Reset();

			// 5. Notify there is data in queue
			queueHasData.Set();

			// 6. Release lock. Allow other threads to write.
			Monitor.PulseAll(wLock);
			Monitor.Exit(wLock);
			return true;
		}

		#endregion
	}
}
