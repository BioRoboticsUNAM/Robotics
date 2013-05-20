using System;
using System.Threading;

namespace Robotics.Paralelism
{
	/// <summary>
	/// Implements a synchronous pipe which can be used to communicate two Filter objects.
	/// A synchronous pipe will synchronize two filters passing the written data directly to the reader.
	/// </summary>
	/// <typeparam name="T">The type of data transmited through the pipe</typeparam>
	public sealed class SynchronousPipe<T> : IPipe<T>
	{
		#region Variables

		/// <summary>
		/// Temporary stores data
		/// </summary>
		private T buffer;

		/// <summary>
		/// Event used for synchronization on write
		/// </summary>
		private ManualResetEvent readEvent;

		/// <summary>
		/// Object used for synchronous access to the read method
		/// </summary>
		private object rLock;

		/// <summary>
		/// Event used for synchronization on write
		/// </summary>
		private ManualResetEvent writeEvent;

		/// <summary>
		/// Object used for synchronous access to the write method
		/// </summary>
		private object wLock;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SynchronousPipe
		/// </summary>
		public SynchronousPipe()
		{
			rLock = new Object();
			wLock = new Object();
			readEvent = new ManualResetEvent(false);
			writeEvent = new ManualResetEvent(false);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the maximum capacity of the pipe
		/// </summary>
		public int Capacity
		{
			get { return 0; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Reads data from the pipe.
		/// It blocks the thread until another thread writes data
		/// </summary>
		/// <returns>The data written on the other side of the pipe</returns>
		public T Read()
		{
			T data;
			// 1. Only one thread can read at a time
			Monitor.Enter(rLock);
			// 2. Notify there is a thread reading
			readEvent.Set();
			// 3. Wait until data is written
			writeEvent.WaitOne();
			// 4. Read the data
			data = this.buffer;
			// 5. Thread is no longer reading
			readEvent.Reset();
			// 6. Release lock. Allow other threads to read.
			Monitor.PulseAll(rLock);
			Monitor.Exit(rLock);
			// 7. Return readed data
			return data;
		}

		/// <summary>
		/// Writes data to the pipe.
		/// It blocks the thread until another thread reads data
		/// </summary>
		/// <param name="data">The data to be read on the other side of the pipe</param>
		public void Write(T data)
		{
			// 1. Only one thread can write at a time
			Monitor.Enter(wLock);
			// 2. Write data
			this.buffer = data;
			// 3. Notify a thread has written
			writeEvent.Set();
			// 4. Wait until a thread is reading
			readEvent.WaitOne();
			// 5. Data is available no more
			writeEvent.Reset();
			// 6. Release lock. Allow other threads to write.
			Monitor.PulseAll(wLock);
			Monitor.Exit(wLock);
		}

		/// <summary>
		/// Tries to read data from the pipe before the timeout elapses.
		/// It blocks the thread until another thread writes data
		/// </summary>
		/// <param name="timeout">The maximum amount of time in milliseconds to wait for a writer
		/// on the other side of the pipe</param>
		/// <param name="data">When this method returns contains the data written on the other side of the pipe
		/// if the read succeded, or the default value of T if the timeout elapses</param>
		/// <returns>true if the read operation suceeded, false otherwise</returns>
		public bool TryRead(int timeout, out T data)
		{
			bool result;

			// 1. Only one thread can read at a time
			Monitor.Enter(rLock);
			// 2. Notify there is a thread reading
			readEvent.Set();
			// 3. Wait until data is written
			result = writeEvent.WaitOne(timeout);
			// 4. Read the data
			data = result ? this.buffer : default(T);
			// 5. Thread is no longer reading
			readEvent.Reset();
			// 6. Release lock. Allow other threads to read.
			Monitor.PulseAll(rLock);
			Monitor.Exit(rLock);

			// 7. Return value indicating if write operation succeeded
			return result;
		}

		/// <summary>
		/// Tries to write data to the pipe before the timeout elapses.
		/// It blocks the thread until another thread reads data
		/// </summary>
		/// <param name="timeout">The maximum amount of time in milliseconds to wait for a reader
		/// on the other side of the pipe</param>
		/// <param name="data">The data to be readed on the other side of the pipe</param>
		/// <returns>true if the write operation suceeded, false otherwise</returns>
		public bool TryWrite(T data, int timeout)
		{
			bool result;
			// 1. Only one thread can write at a time
			Monitor.Enter(wLock);
			// 2. Write data
			this.buffer = data;
			// 3. Notify a thread has written
			writeEvent.Set();
			// 4. Wait until a thread is reading
			result = readEvent.WaitOne(timeout);
			// 5. Data is available no more
			writeEvent.Reset();
			// 6. Release lock. Allow other threads to write.
			Monitor.PulseAll(wLock);
			Monitor.Exit(wLock);

			// 7. Return value indicating if write operation succeeded
			return result;
		}

		#endregion
	}
}
