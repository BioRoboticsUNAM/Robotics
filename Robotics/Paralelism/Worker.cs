using System;
using System.Collections.Generic;
using System.Threading;

namespace Robotics.Paralelism
{
	public partial class ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult>
	{
		/// <summary>
		/// A worker for the Manager-Worker class
		/// </summary>
		internal class Worker
		{
			#region Variables

			/// <summary>
			/// Event used to synchronize threads
			/// </summary>
			private AutoResetEvent autoEvent;

			/// <summary>
			/// The ManagerWorkers object which controls this worker
			/// </summary>
			private readonly ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult> owner;

			/// <summary>
			/// Indicaties whether the Runnable object executes or not in a background thread
			/// </summary>
			private bool runInBackground;

			/// <summary>
			/// The number of the worker
			/// </summary>
			private readonly int workerNumber;

			/// <summary>
			/// The thread used to perform the worker task
			/// </summary>
			private Thread workerThread;

			/// <summary>
			/// Represents the WorkerThreadTask method
			/// </summary>
			private ParameterizedThreadStart workerThreadTask;

			#endregion

			/// <summary>
			/// Initializes a new instance of Worker
			/// </summary>
			/// <param name="owner">The ManagerWorkers object which controls this worker</param>
			/// <param name="workerNumber">The number of the worker</param>
			public Worker(ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult> owner, int workerNumber)
			{
				this.owner = owner;
				this.workerNumber = workerNumber;
				this.autoEvent = new AutoResetEvent(false);
				this.workerThreadTask = new ParameterizedThreadStart(this.WorkerThreadTask);
			}

			#region Properties

			/// <summary>
			/// Gets or sets a value indicating whether the Runnable object is running
			/// </summary>
			public bool IsRuning
			{
				get { return ((this.workerThread != null) && this.workerThread.IsAlive); }
			}

			/// <summary>
			/// Gets the ManagerWorkers object which controls this worker
			/// </summary>
			public ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult> Owner
			{
				get { return owner; }
			}

			/// <summary>
			/// Gets or sets a value indicating whether the Worker object executes or not in a background thread
			/// </summary>
			public bool RunInBackground
			{
				get { return runInBackground; }
				set
				{
					runInBackground = value;
				}
			}

			/// <summary>
			/// Gets the type o the data used by this object
			/// </summary>
			public Type WorkerDataType
			{
				get { return typeof(TWorkerData); }
			}

			/// <summary>
			/// Gets the number of the worker
			/// </summary>
			public int WorkerNumber
			{
				get { return this.workerNumber; }
			}

			/// <summary>
			/// Gets the thread that executes the async task
			/// </summary>
			public Thread WorkerThread
			{
				get { return this.workerThread; }
			}

			#endregion

			#region Methods

			/// <summary>
			/// Configures the thread and begins the execution of the Owner.WorkerTask method.
			/// </summary>
			/// <param name="taskObject">Object that contains the task data to be used during the worker execution</param>
			public void DoWork(WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult> taskObject)
			{
				this.workerThread = new Thread(this.workerThreadTask);
				this.workerThread.IsBackground = this.runInBackground;
				this.workerThread.Start(taskObject);
			}

			/// <summary>
			/// Performs the worker task by calling the Owner.WorkerTask method.
			/// When it completes, enqueues the worker in the Owner.idleWorkers queue.
			/// </summary>
			/// <param name="o">Object that contains the task data to be used during the worker execution</param>
			private void WorkerThreadTask(object o)
			{
				try
				{
					WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult> taskObject =
						o as WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult>;
					if (taskObject == null)
						return;
					owner.WorkerTask(taskObject);
				}
				catch
				{
					// Nothing to do here, just ensure the finally block will be executed
				}
				finally
				{
					owner.FreeWorker(this);
				}
			}

			#endregion

		}
	}
}
