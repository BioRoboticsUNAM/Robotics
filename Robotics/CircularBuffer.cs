using System;
using System.Collections.Generic;
using System.Threading;

namespace Robotics
{
	/// <summary>
	/// Implements a thread-safe circular buffer
	/// The CircularBuffer may be used as shared resource for Producer/Consumer processes
	/// </summary>
	internal sealed class CircularBuffer<T>
	{
		#region Variables

		/// <summary>
		/// Stores the size of the circular buffer
		/// </summary>
		private int bufferSize;
		/// <summary>
		/// Buffer used to store data
		/// </summary>
		private T[] buffer;
		/// <summary>
		/// Stores the number of (active) elements in the buffer
		/// </summary>
		private int elementCount;
		/// <summary>
		/// Stores the index for read operations
		/// </summary>
		private int readIndex;
		/// <summary>
		/// Stores the index for write operations
		/// </summary>
		private int writeIndex;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of ProducerConsumerMonitor with an empty buffer of two elements
		/// </summary>
		public CircularBuffer()
			:this(2)
		{
		}

		/// <summary>
		/// Creates a new instance of ProducerConsumerMonitor with an empty buffer of two elements
		/// </summary>
		/// <param name="bufferSize">The size of the circular buffer</param>
		public CircularBuffer(int bufferSize)
		{
			if (bufferSize <= 1) throw new ArgumentOutOfRangeException("bufferSize");
			this.bufferSize = bufferSize;
			this.buffer = new T[bufferSize];
			this.readIndex = 0;
			this.writeIndex = 0;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the size of the CircularBuffer
		/// </summary>
		public int BufferSize
		{
			get { return bufferSize; }
		}

		/// <summary>
		/// Gets the number of availiable in the CircularBuffer
		/// </summary>
		public int Count
		{
			get { return elementCount; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Adds an element to the CircularBuffer
		/// </summary>
		/// <remarks>This method returns only when there is space availiable in the buffer to put in</remarks>
		/// <param name="value">Element to add</param>
		public void Put(T value)
		{
			// Locks the current object
			Monitor.Enter(buffer);
			// If buffer is full, wait
			while(elementCount == bufferSize)
				Monitor.Wait(buffer);
			// Releases the thread until the 
			buffer[writeIndex] = value;
			// Increment element count
			++elementCount;
			// Update write index
			writeIndex = (writeIndex + 1) % bufferSize;
			// Unlock the current object
			Monitor.PulseAll(buffer);
		}

		/// <summary>
		/// Retrieves an element from the CircularBuffer
		/// </summary>
		/// <remarks>This method returns only when there is data availiable in the buffer to take from</remarks>
		/// <returns>Element retrieved</returns>
		public T Take()
		{
			T data;

			// Locks the current object
			Monitor.Enter(buffer);
			// Check if there are elements. If not, sleep the thread
			while(elementCount < 1)
				Monitor.Wait(buffer);
			// Retrieve the data
			data = buffer[readIndex];
			// Reduce element count
			--elementCount;
			// Update read index
			readIndex = (readIndex + 1) % bufferSize;
			// Unlock the current object
			Monitor.PulseAll(buffer);
			// Return data
			return data;
		}

		#endregion

		#region Inherited Methodos
		#endregion

	}
}
