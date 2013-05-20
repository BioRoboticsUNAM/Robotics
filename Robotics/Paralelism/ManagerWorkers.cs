using System;
using System.Collections.Generic;
using System.Threading;

namespace Robotics.Paralelism
{
	/// <summary>
	/// Enumerates the execution result of a ManagerWorkers object
	/// </summary>
	public enum MWExecutionStatus
	{
		/// <summary>
		/// Execution was aborted
		/// </summary>
		ExecutionAborted = 1,
		/// <summary>
		/// Execution was canceled
		/// </summary>
		ExecutionCanceled = 2,
		/// <summary>
		/// Execution completed normally
		/// </summary>
		ExecutionCompleted = 0,
		/// <summary>
		/// Execution is still in progress
		/// </summary>
		ExecutionInProgress = -1,
		/// <summary>
		/// Execution result is Unknown
		/// </summary>
		Unknown = -2
	}

	/// <summary>
	/// Serves as base class for a Manager/Workers implementation
	/// </summary>
	/// <typeparam name="TManagerData">The type of data used by the manager</typeparam>
	/// <typeparam name="TManagerResult">The type of data produced by the manager</typeparam>
	/// <typeparam name="TWorkerData">The type of data used by the workers</typeparam>
	/// <typeparam name="TWorkerResult">The type of data produced by the workers</typeparam>
	public abstract partial class ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult>
	{
		#region Constants
		#endregion

		#region Variables

		/// <summary>
		/// Event used to notify there is at leas one worker availiable
		/// </summary>
		private ManualResetEvent availiableWorker;

		/// <summary>
		/// Event used to notify when all workers become free
		/// </summary>
		private ManualResetEvent allWorkersFree;

		/// <summary>
		/// Indicates whether an abort of the work has been requested
		/// </summary>
		private bool abortRequested;

		/// <summary>
		/// Indicates whether the manager/workers has been requested to cancel the current operation
		/// </summary>
		private bool cancelRequested;

		/// <summary>
		/// The status of the last execution
		/// </summary>
		private MWExecutionStatus lastExecutionStatus;

		/// <summary>
		/// The result of the last execution
		/// </summary>
		private TManagerResult lastExecutionResult;

		/// <summary>
		/// Number of availiable workers
		/// </summary>
		private int numWorkers;

		/// <summary>
		/// Object used to synchronize the acces to the start, cancel and abort methods
		/// </summary>
		private object oLock;

		/// <summary>
		/// Indicates if the manager and worker threads will be executed as background threads
		/// </summary>
		private bool runInBackground;

		/// <summary>
		/// Indicates whether the manager/workers are running
		/// </summary>
		private bool running;

		/// <summary>
		/// Used to synchronize the completition of the execution
		/// </summary>
		private ManualResetEvent workFinished;

		#region Manager related

		/// <summary>
		/// Represents the ManagerThreadTask method
		/// </summary>
		private ParameterizedThreadStart managerThreadTask;

		/// <summary>
		/// The manager thread
		/// </summary>
		private Thread managerThread;

		#endregion

		#region Workers related

		/// <summary>
		/// Stores the busy workers
		/// </summary>
		private List<Worker> busyWorkers;

		/// <summary>
		/// Stores the idle workers
		/// </summary>
		private Queue<Worker> idleWorkers;

		/// <summary>
		/// Lock used to access the busyWorkers and idleWorkers objects
		/// </summary>
		private ReaderWriterLock workersLock;

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of ManagerWorkers class.
		/// </summary>
		public ManagerWorkers():this(9)
		{}

		/// <summary>
		/// Initializes a new instance of ManagerWorkers class
		/// </summary>
		/// <param name="numWorkers">Number of workers to use</param>
		public ManagerWorkers(int numWorkers)
		{
			this.abortRequested = true;
			this.NumWorkers = numWorkers;
			this.running = false;
			this.runInBackground = true;
			this.abortRequested = false;
			this.oLock = new Object();

			this.workersLock = new ReaderWriterLock();
			this.allWorkersFree = new ManualResetEvent(true);
			this.availiableWorker = new ManualResetEvent(true);
			this.workFinished = new ManualResetEvent(false);
			this.managerThreadTask = new ParameterizedThreadStart(this.ManagerThreadTask);
			this.lastExecutionStatus = MWExecutionStatus.Unknown;
			this.lastExecutionResult = default(TManagerResult);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the execution of manager/workers is canceled or aborted
		/// </summary>
		public event EventHandler ExecutionAborted;

		/// <summary>
		/// Occurs when the execution of manager/workers completes
		/// </summary>
		public event EventHandler ExecutionCompleted;

		/// <summary>
		/// Occurs when the execution of manager/workers starts
		/// </summary>
		public event EventHandler ExecutionStarted;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether an abort of the work has been requested
		/// </summary>
		public bool AbortRequested
		{
			get { return this.abortRequested; }
		}

		/// <summary>
		/// Gets a value indicating whether the manager/workers has been requested to cancel the current operation
		/// </summary>
		public bool CancelRequested
		{
			get { return this.cancelRequested; }
		}

		/// <summary>
		/// Gets a value indicating whether manager/workers are running
		/// </summary>
		public bool IsRunning
		{
			get { return this.running; }
		}

		/// <summary>
		/// Gets the status of the last execution 
		/// </summary>
		public MWExecutionStatus LastExecutionStatus
		{
			get { return this.lastExecutionStatus; }
		}

		/// <summary>
		/// Gets the result of the last execution 
		/// </summary>
		public TManagerResult LastExecutionResult
		{
			get { return this.lastExecutionResult; }
		}

		/// <summary>
		/// Gets or sets the number of workers.
		/// If the manager/workers has been started, it has no effect
		/// </summary>
		public int NumWorkers
		{
			get { return numWorkers; }
			set
			{
				if((value < 1) || (value > 255))
					throw new ArgumentOutOfRangeException("The nuber of workers must be between 1 and 255");
				Monitor.Enter(oLock);
				if (running)
				{
					Monitor.PulseAll(oLock);
					Monitor.Exit(oLock);
					return;
				}
				numWorkers = value;
				Monitor.PulseAll(oLock);
				Monitor.Exit(oLock);
			}
		}

		/// <summary>
		/// Gts a value indicating if the manager and worker threads will be executed as background threads.
		/// If the manager/workers has been started, it has no effect
		/// </summary>
		private bool RunInBackground
		{
			get { return runInBackground; }
			set {
				Monitor.Enter(oLock);
				if (running)
				{
					Monitor.PulseAll(oLock);
					Monitor.Exit(oLock);
					return;
				}
				runInBackground = value;
				Monitor.PulseAll(oLock);
				Monitor.Exit(oLock);
			}
		}

		#endregion

		#region Public Methodos

		/// <summary>
		/// Aborts the manager/workers execution.
		/// It calls the Abort method for all, manager and worker threads.
		/// If the execution has not been started yet it has no effect.
		/// </summary>
		public void Abort()
		{
			Monitor.Enter(oLock);
			if (!running || abortRequested)
			{
				Monitor.PulseAll(oLock);
				Monitor.Exit(oLock);
				return;
			}
			
			abortRequested = true;
			Monitor.PulseAll(oLock);
			Monitor.Exit(oLock);

			this.workersLock.AcquireReaderLock(-1);
			for (int i = 0; i < this.busyWorkers.Count; ++i)
				busyWorkers[i].WorkerThread.Abort();
			this.workersLock.ReleaseReaderLock();
			this.managerThread.Abort();

			WaitFinished();
		}

		/// <summary>
		/// Stops the manager/workers execution.
		/// If the execution has not been started yet it has no effect.
		/// </summary>
		public void Cancel()
		{
			Monitor.Enter(oLock);
			if (!running || abortRequested || cancelRequested)
			{
				Monitor.PulseAll(oLock);
				Monitor.Exit(oLock);
				return;
			}

			cancelRequested = true;

			Monitor.PulseAll(oLock);
			Monitor.Exit(oLock);

			WaitFinished();
		}

		/// <summary>
		/// Starts the manager/workers execution.
		/// If the execution has been started it has no effect.
		/// </summary>
		/// <param name="managerData">The data to be used by the manager thread during its execution</param>
		public void Start(TManagerData managerData)
		{
			Monitor.Enter(oLock);
			if (running)
			{
				Monitor.PulseAll(oLock);
				Monitor.Exit(oLock);
				return;
			}

			this.running = true;
			this.abortRequested = false;
			this.cancelRequested = false;
			this.SetupLists();
			this.workFinished.Reset();
			this.lastExecutionStatus = MWExecutionStatus.ExecutionInProgress;
			this.lastExecutionResult = default(TManagerResult);
			
			this.managerThread = new Thread(managerThreadTask);
			this.managerThread.IsBackground = this.runInBackground;
			this.managerThread.Start(managerData);

			Monitor.PulseAll(oLock);
			Monitor.Exit(oLock);
		}

		/// <summary>
		/// Blocks the thread until the execution is finished due to work completition, cancelation of abort.
		/// If the execution has not been started, it returns immediately.
		/// </summary>
		public TManagerResult WaitFinished()
		{
			if (!running)
				return default(TManagerResult);
			workFinished.WaitOne();
			return this.lastExecutionResult;
		}

		/// <summary>
		/// Blocks the thread until the execution is finished due to work completition, cancelation of abort or the specified time elapses.
		/// If the execution has not been started, it returns immediately.
		/// </summary>
		/// <param name="timeout">The number of milliseconds to wait, or Timeout.Infinite (-1) to wait indefinitely.</param>
		public TManagerResult WaitFinished(int timeout)
		{
			if (!running)
				return default(TManagerResult);
			if(workFinished.WaitOne(timeout))
				return this.lastExecutionResult;
			return default(TManagerResult);
		}

		#endregion

		#region Protected and Private Methodos

		/// <summary>
		/// Assigns work to a free worker. If there is no free workers will block the thread until one becomes availiable
		/// </summary>
		/// <param name="data">The data to be used by the worker</param>
		protected void AssignWork(TWorkerData data)
		{
			Worker worker;
			WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult> taskObject;

			if(abortRequested || cancelRequested)
				return;

			// 1. Assure only the Manager Thread access this method
			if ((this.managerThread == null) || (this.managerThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId))
				throw new AccessViolationException("Only Manager Thread can access this method");

			// 2. Wait for a worker to be released.
			// This doesn't guarantee that only one thread will cross the synchronization event so
			// in principle many threads can reach the lock and the Q may be empty while dequeuing.
			// However, this will never happen because previously we limited the access to this function
			// to the Manager thread.
			this.availiableWorker.WaitOne();

			// 3. Acquire Lock. Synchronized access to the idleWorker queue and busyWorkers list.
			this.workersLock.AcquireWriterLock(-1);
			Thread.BeginCriticalRegion();

			// 4. Get a worker from the idleWorkers queue.
			// Main thread can not reach this point if the availiableWorker event was not set
			// which occurs only at initialization or when a woker is released
			worker = this.idleWorkers.Dequeue();
			// 5. Update availiableWorker synchronization event
			// Notice that availiableWorker event is only set/reset within the workersLock context
			if (idleWorkers.Count > 0)
				this.availiableWorker.Set();
			else
				this.availiableWorker.Reset();
			// 6. Reset allWorkersFree synchronization event
			// Notice that allWorkersFree event is only set/reset within the workersLock context
			this.allWorkersFree.Reset();
			// 7. Add the worker to the list in its corresponding position
			this.busyWorkers.Insert(worker.WorkerNumber, worker);
			// 8. Release the lock
			this.workersLock.ReleaseWriterLock();
			Thread.EndCriticalRegion();

			// 9. Create the data object to be passed to the worker
			taskObject = new WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult>(worker, data);
			// 10. Start the worker
			worker.DoWork(taskObject);
		}

		/// <summary>
		/// Assigns work to a free worker.
		/// If there is no free workers will block the thread until one becomes availiable or the specified time elapses.
		/// </summary>
		/// <param name="data">The data to be used by the worker</param>
		/// <param name="timeout">The number of milliseconds to wait for a worker to become availiable.
		/// Use zero to return immediately. Use -1 to wait indefinitely.</param>
		/// <returns>true if the work was assigned, false otherwise</returns>
		protected bool AssignWorkWait(TWorkerData data, int timeout)
		{
			Worker worker;
			WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult> taskObject;

			if (abortRequested || cancelRequested)
				return false;

			// 1. Assure only the Manager Thread access this method
			if ((this.managerThread == null) || (this.managerThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId))
				throw new AccessViolationException("Only Manager Thread can access this method");

			// 2. Wait for a worker to be released, or timeout elapses.
			// This doesn't guarantee that only one thread will cross the synchronization event so
			// in principle many threads can reach the lock and the Q may be empty while dequeuing.
			// However, this will never happen because previously we limited the access to this function
			// to the Manager thread.
			if (!this.availiableWorker.WaitOne(timeout))
				return false;

			// 3. Acquire Lock. Synchronized access to the idleWorker queue and busyWorkers list.
			this.workersLock.AcquireWriterLock(-1);
			Thread.BeginCriticalRegion();

			// 4. Get a worker from the idleWorkers queue.
			// Main thread can not reach this point if the availiableWorker event was not set
			// which occurs only at initialization or when a woker is released
			worker = this.idleWorkers.Dequeue();
			// 5. Update availiableWorker synchronization event
			// Notice that availiableWorker event is only set/reset within the workersLock context
			if (idleWorkers.Count > 0)
				this.availiableWorker.Set();
			else
				this.availiableWorker.Reset();
			// 6. Reset allWorkersFree synchronization event
			// Notice that allWorkersFree event is only set/reset within the workersLock context
			this.allWorkersFree.Reset();
			// 7. Add the worker to the busyWorkers list in its corresponding position
			this.busyWorkers.Insert(worker.WorkerNumber, worker);
			// 8. Release the lock
			this.workersLock.ReleaseWriterLock();
			Thread.EndCriticalRegion();

			// 9. Create the data object to be passed to the worker
			taskObject = new WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult>(worker, data);
			// 10. Start the worker
			worker.DoWork(taskObject);

			return true;
		}

		/// <summary>
		/// Moves the worker from the busyWorkers list to the idleWorkers queue
		/// </summary>
		/// <param name="worker">The worker to move</param>
		private void FreeWorker(Worker worker)
		{
			// 1. Assure only an authorized worker can access this method
			if ((worker == null) || (worker.Owner != this) ||
				(worker.WorkerThread == null) ||
				(worker.WorkerThread.ManagedThreadId != Thread.CurrentThread.ManagedThreadId))
				throw new AccessViolationException("Only Worker Threads created by this Manager/Workers inscance can access this method");

			// 2. Acquire Lock. Synchronized access to the idleWorker queue and busyWorkers list.
			this.workersLock.AcquireWriterLock(-1);
			Thread.BeginCriticalRegion();
			// 3. Remove the worker from the busyWorkers list
			if (!this.busyWorkers.Remove(worker))
			{
				// Error! Houston, we had a problem.
				// It seems the worker hs been already removed
				this.workersLock.ReleaseWriterLock();
				throw new AccessViolationException("This worker is already freed");
			}
			// 4. Add the worker to the idleWorkers queue
			this.idleWorkers.Enqueue(worker);
			// 5. Update availiableWorker synchronization event
			// Notice that availiableWorker event is only set/reset within the workersLock context
			if (idleWorkers.Count > 0)
				this.availiableWorker.Set();
			else
				this.availiableWorker.Reset();
			// 6. If all workers have been freed, reset allWorkersFree synchronization event
			// Notice that allWorkersFree event is only set/reset within the workersLock context
			if (idleWorkers.Count == this.numWorkers)
				this.allWorkersFree.Set();

			// 7. Release the lock
			this.workersLock.ReleaseWriterLock();
			Thread.BeginCriticalRegion();
		}

		/// <summary>
		/// Performs the manager task by calling the ManagerTask method.
		/// When it completes, enqueues the worker in the Owner.idleWorkers queue.
		/// </summary>
		/// <param name="o">Object that contains the task data to be used by the manager thread</param>
		private void ManagerThreadTask(object o)
		{
			TManagerResult executionResult;

			if (!(o is TManagerData))
			{
				OnExecutionAborted();
				return;
			}
			try
			{
				OnExecutionStarted();
				executionResult = ManagerTask((TManagerData)o);

				if (cancelRequested)
				{
					this.lastExecutionStatus = MWExecutionStatus.ExecutionCanceled;
				}
				else
				{
					this.lastExecutionStatus = MWExecutionStatus.ExecutionCompleted;
				}
				OnExecutionCompleted();
			}
			catch(ThreadAbortException)
			{
				// Rise the ExecutionAborted event and ensure the finally block will be executed
				this.OnExecutionAborted();
				this.lastExecutionResult = default(TManagerResult);
				this.lastExecutionStatus = MWExecutionStatus.ExecutionAborted;
			}
			finally
			{
				this.workersLock.AcquireWriterLock(-1);
				Thread.BeginCriticalRegion();
				this.idleWorkers.Clear();
				this.busyWorkers.Clear();
				this.availiableWorker.Set();
				this.allWorkersFree.Set();
				this.workersLock.ReleaseWriterLock();
				Thread.EndCriticalRegion();
				this.workFinished.Set();
				this.running = false;
			}
		}

		/// <summary>
		/// Raises the ExecutionAborted event. 
		/// When overriding in a derived class, be sure to call the base class's method so that registered delegates receive the event. 
		/// </summary>
		protected virtual void OnExecutionAborted()
		{
			if (ExecutionAborted != null)
				ExecutionAborted(this, null);
		}

		/// <summary>
		/// Raises the ExecutionCompleted event. 
		/// When overriding in a derived class, be sure to call the base class's method so that registered delegates receive the event. 
		/// </summary>
		protected virtual void OnExecutionCompleted()
		{
			if (ExecutionCompleted != null)
				ExecutionCompleted(this, null);
		}

		/// <summary>
		/// Raises the ExecutionStarted event. 
		/// When overriding in a derived class, be sure to call the base class's method so that registered delegates receive the event. 
		/// </summary>
		protected virtual void OnExecutionStarted()
		{
			if (ExecutionStarted != null)
				ExecutionStarted(this, null);
		}

		/// <summary>
		/// Sets up the busyWorkers and idleWorkers lists
		/// </summary>
		private void SetupLists()
		{
			int i;

			// 1. Acquire Lock. Synchronized access to the idleWorker queue and busyWorkers list.
			this.workersLock.AcquireWriterLock(-1);

			// 2. Clear both busyWorkers list and idleWorkers queue
			if (this.busyWorkers != null)
				this.busyWorkers.Clear();
			if (this.idleWorkers != null)
				this.idleWorkers.Clear();

			// 3. Create a new ones.
			// It can be replaced by aditional verification and also expand their capacities
			// but create a new ones is easier.
			this.busyWorkers = new List<ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult>.Worker>(numWorkers);
			this.idleWorkers = new Queue<ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult>.Worker>(numWorkers);
			this.allWorkersFree.Set();
			this.availiableWorker.Set();

			// 4. Fill the idleWorkers queue with new workers.
			// Again the old workers (if any) can be reused, but this provides a faster initialization
			// than a clean up (especially when Joins are involved) and we don't want the GC become lazy
			for (i = 0; i < numWorkers; ++i)
				idleWorkers.Enqueue(new ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult>.Worker(this, i));

			// 5. Release the lock
			this.workersLock.ReleaseWriterLock();
		}

		/// <summary>
		/// Blocks the thread until all workers finish their tasks.
		/// </summary>
		protected void WaitAllWorkers()
		{
			this.allWorkersFree.WaitOne();
		}

		/// <summary>
		/// Blocks the thread until a worker finish its task and becomes availiable.
		/// </summary>
		protected void WaitWorker()
		{
			if (!running)
				return;
			this.availiableWorker.WaitOne();
		}

		/// <summary>
		/// Blocks the thread until a worker finish its task becoming availiable or the specified time elapses.
		/// </summary>
		/// <param name="timeout">The number of milliseconds to wait for a worker to become availiable.
		/// Use zero to return immediately. Use -1 to wait indefinitely.</param>
		/// <returns>true if a worker becomes availiable before the timeout, false otherwise</returns>
		protected bool WaitWorker(int timeout)
		{
			if (!running)
				return false;
			this.availiableWorker.WaitOne(timeout);
			return true;
		}

		#endregion

		#region Abstract Methodos

		/// <summary>
		/// When overriden in a derived class, performs the manager task
		/// </summary>
		/// <param name="managerData">The data to be used by the manager</param>
		protected abstract TManagerResult ManagerTask(TManagerData managerData);

		/// <summary>
		/// When overriden in a derived class, performs the worker task
		/// </summary>
		/// <param name="taskObject">Object that contains the task data with which execute the task</param>
		protected abstract TWorkerResult WorkerTask(WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult> taskObject);

		#endregion
	}

	/// <summary>
	/// Serves as base class for a Manager/Workers implementation
	/// </summary>
	public abstract class ManagerWorkers : ManagerWorkers<object, object, object, object>
	{}
}
