using System;
using System.Collections.Generic;
using System.Threading;

namespace Robotics
{
	/// <summary>
	/// Implements a producer/consumer. 
	/// </summary>
	/// <typeparam name="T">Specifies the type of elements in the queue</typeparam>
	public class ProducerConsumer<T>
	{
		#region Variables

		/// <summary>
		/// The Queue object used to store data
		/// </summary>
		private readonly Queue<T> queue;

		/// <summary>
		/// Number of elements contained in the ProducerConsumer. It allows read the number of elements in the queue asynchronously
		/// </summary>
		private int count;

		/// <summary>
		///  The number of elements that the ProducerConsumer can store
		/// </summary>
		private int capacity;

		/// <summary>
		/// Indicates if the ProducerConsumer has fixed size
		/// </summary>
		private readonly bool fixedSize;

		/// <summary>
		/// Indicates if the the last element of the queue will be discarded when a new element is stored in a fixed size ProducerConsumer object.
		/// </summary>
		private readonly bool discardExcedent;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ProducerConsumer class.
		/// </summary>
		public ProducerConsumer()
			: this(10) { }

		/// <summary>
		/// Initializes a new instance of the ProducerConsumer class.
		/// </summary>
		/// <param name="capacity">The maximum number of elements that the ProducerConsumer can contain</param>
		public ProducerConsumer(int capacity):
			this(capacity < 1 ? 10 : capacity, capacity >= 1, true)	{ }

		/// <summary>
		/// Initializes a new instance of the ProducerConsumer class.
		/// </summary>
		/// <param name="capacity">The maximum number of elements that the ProducerConsumer can contain</param>
		/// <param name="fixedSize">Indicates if the ProducerConsumer has fixed size</param>
		/// <param name="discardExcedent">Indicates if the the last element of the queue will be discarded when a new element is stored in a fixed size ProducerConsumer object.</param>
		public ProducerConsumer(int capacity, bool fixedSize, bool discardExcedent)
		{
			if (fixedSize && (capacity < 1))
				throw new ArgumentOutOfRangeException();
			this.capacity = capacity;
			this.queue = new Queue<T>(capacity);
			this.fixedSize = fixedSize;
			this.discardExcedent = discardExcedent;
			count = 0;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of elements contained in the ProducerConsumer
		/// </summary>
		public int Count
		{
			get { return count; }
		}

		/// <summary>
		/// Gets the number of elements that the Queue can store
		/// </summary>
		public int Capacity
		{
			get { return capacity; }
		}

		/// <summary>
		/// Gets a value indicating if the the last element of the queue will be discarded when a new element is stored in a fixed size ProducerConsumer object.
		/// If the ProducerConsumer has fixed size and this value is set to false, the Produce method will block untill there is space in the queue
		/// </summary>
		/// <remarks>This value has not effect on non fixed size ProducerConsumer objects</remarks>
		public bool DiscardExcedent
		{
			get { return discardExcedent; }
		}

		/// <summary>
		/// Gets a value indicating if the ProducerConsumer has fixed size
		/// </summary>
		/// <remarks>A fixed size ProducerConsumer object will not increase the size of its queue.
		/// The exceding contained objects may be discarded depending on the value of the DiscardExcedent property</remarks>
		public bool FixedSize
		{
			get { return fixedSize; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Removes all objects from the ProducerConsumer queue
		/// </summary>
		public void Clear()
		{
			lock (queue)
			{
				queue.Clear();
				count = 0;
				if (!fixedSize)
					queue.TrimExcess();
				else if(!discardExcedent)
					Monitor.PulseAll(queue);
			}
		}

		/// <summary>
		/// Adds an object to the end of the queue of the ProducerConsumer
		/// </summary>
		/// <param name="item">The object to add to the ProducerConsumer. The value can be a null reference (Nothing in Visual Basic) for reference types</param>
		public void Produce(T item)
		{
			T discarded = default(T);
			lock (queue)
			{
				if (fixedSize && (queue.Count >= capacity))
				{
					// If the queue cannot be resized and excedent
					// will be discarded, then dequeue and discard
					if (discardExcedent)
						discarded = queue.Dequeue();
					// If elements will not be discarded te producer must wait until
					// there is space availiable in the queue
					else
					{
						while (queue.Count >= capacity)
						{
							// This releases the queue, only reacquiring it
							// after being woken up by a call to Pulse
							try
							{
								Monitor.Wait(queue);
							}
							catch (ThreadInterruptedException) { return; }
						}
					}
				}
				else
					++count;
				queue.Enqueue(item);
				if (queue.Count > capacity)
					capacity = queue.Count;
				// We always need to pulse, even if the queue wasn't
				// empty before. Otherwise, if we add several items
				// in quick succession, we may only pulse once, waking
				// a single thread up, even if there are multiple threads
				// waiting for items.
				if (discardExcedent)
					Monitor.Pulse(queue);
				else
					Monitor.PulseAll(queue);
			}
			if (discarded is IDisposable)
			{
				try
				{
					((IDisposable)discarded).Dispose();
				}
				catch { }
			}
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the queue of the ProducerConsumer
		/// </summary>
		/// <returns>The object that is removed from the beginning of the queue of the ProducerConsumer.</returns>
		public T Consume()
		{
			T item;
			lock (queue)
			{
				// If the queue is empty, wait for an item to be added
				// Note that this is a while loop, as we may be pulsed
				// but not wake up before another thread has come in and
				// consumed the newly added object. In that case, we'll
				// have to wait for another pulse.
				while (queue.Count == 0)
				{
					// This releases queue, only reacquiring it
					// after being woken up by a call to Pulse
					try
					{
						Monitor.Wait(queue);
					}
					catch (ThreadInterruptedException) { return default(T); }
					catch (ThreadAbortException) { return default(T); }
				}
				--count;
				item =  queue.Dequeue();
				// If there is a producer waiting, notify the count has changed
				if (fixedSize && !discardExcedent)
					Monitor.PulseAll(queue);
			}
			return item;
		}

		/// <summary>
		/// Removes and returns the object at the beginning of the queue of the ProducerConsumer
		/// If the specified time-out interval elapses, the thread enters the ready queue and returns the default value of the type. 
		/// </summary>
		/// <param name="timeOut">The number of milliseconds to wait for an element</param>
		/// <returns>The object that is removed from the beginning of the queue of the ProducerConsumer.</returns>
		public T Consume(int timeOut)
		{
			T item;
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Reset();
			int remaining = timeOut;
			lock (queue)
			{
				stopwatch.Start();
				// If the queue is empty, wait for an item to be added
				// Note that this is a while loop, as we may be pulsed
				// but not wake up before another thread has come in and
				// consumed the newly added object. In that case, we'll
				// have to wait for another pulse.
				while (queue.Count == 0)
				{
					// This releases queue, only reacquiring it
					// after being woken up by a call to Pulse
					try
					{
						if (!Monitor.Wait(queue, remaining))
						{
							stopwatch.Stop();
							return default(T);
						}
					}
					catch (ThreadInterruptedException)
					{
						stopwatch.Stop();
						return default(T);
					}
					catch (ThreadAbortException)
					{
						stopwatch.Stop();
						return default(T);
					}
					remaining-=(int)stopwatch.ElapsedMilliseconds;
					if(remaining <= 0)
					{
						stopwatch.Stop();
						return default(T);
					}
				}
				stopwatch.Stop();
				--count;
				item = queue.Dequeue();
				// If there is a producer waiting, notify the count has changed
				if (fixedSize && !discardExcedent)
					Monitor.PulseAll(queue);
			}
			return item;
		}

		/// <summary>
		/// Copies the ProducerConsumer&lt;T&gt; elements to a new array.
		/// </summary>
		/// <returns>A new array containing elements copied from the ProducerConsumer</returns>
		public T[] ToArray()
		{
			T[] a;
			lock (queue)
			{
				a = (T[])queue.ToArray();
			}
			return a;
		}

		#endregion
	}
}
