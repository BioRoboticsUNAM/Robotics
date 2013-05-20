using System;
using System.Threading;

namespace Robotics
{
	/// <summary>
	/// Enumerates the status of a service
	/// </summary>
	public enum ServiceStatus
	{
		/// <summary>
		/// The service is Idle
		/// </summary>
		Idle,
		/// <summary>
		/// The service is starting but not running yet
		/// </summary>
		Starting,
		/// <summary>
		/// The service is running
		/// </summary>
		Running,
		/// <summary>
		/// The execution of the service has been paused
		/// </summary>
		Paused,
		/// <summary>
		/// The service has stopped but not idle yet
		/// </summary>
		Stopping
	}

	/// <summary>
	/// Serves as base class for instances which provides a service which
	/// run in background and can be started and stopped
	/// </summary>
	public abstract class ServiceBase
	{
		#region Variables

		private object oLock;
		private ServiceStatus status;
		private Thread mainThread;
		private AutoResetEvent taskStarted;

		#endregion

		#region Constructor

		/// <summary>
		/// Initialize a new instance of ServiceBase 
		/// </summary>
		public ServiceBase()
		{
			oLock = new object();
			taskStarted = new AutoResetEvent(false);
			status = ServiceStatus.Idle;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether the ServiceBase object is running
		/// </summary>
		public bool IsRunning { get { return this.status != ServiceStatus.Idle; } }

		/// <summary>
		/// Gets the status of the ServiceBase object
		/// </summary>
		public ServiceStatus Status { get { return this.status; } }

		#endregion

		#region Methods

		/// <summary>
		/// Begins the execution of the service withot waiting for the background thread to start running
		/// </summary>
		public void BeginStart()
		{
			// If we can not acquire the lock, then there is a Start or Stop operation 
			// in progress, so better leave. Also leave if already running.
			if ((status != ServiceStatus.Idle) || !Monitor.TryEnter(oLock))
				return;

			// Set running flag to prevent other threads to start service again
			status = ServiceStatus.Starting;

			// Create and start the thread
			mainThread = new Thread(new ThreadStart(Task));
			mainThread.IsBackground = true;
			mainThread.Start();

			// Release the lock
			Monitor.PulseAll(oLock);
			Monitor.Exit(oLock);
		}

		/// <summary>
		/// Starts the execution of the service
		/// </summary>
		public void Start()
		{
			// If we can not acquire the lock, then there is a Start or Stop operation 
			// in progress, so better leave. Also leave if already running.
			if ((status != ServiceStatus.Idle) || !Monitor.TryEnter(oLock))
				return;

			// Set running flag to prevent other threads to start service again
			status = ServiceStatus.Starting;

			// Create and start the thread
			mainThread = new Thread(new ThreadStart(Task));
			mainThread.IsBackground = true;
			mainThread.Start();

			// Wait until the task execution begins. This is optional
			taskStarted.WaitOne();

			// Release the lock
			Monitor.PulseAll(oLock);
			Monitor.Exit(oLock);
		}

		/// <summary>
		/// Stops the execution of the service
		/// </summary>
		public void Stop()
		{
			// If we can not acquire the lock, then there is a Start or Stop operation 
			// in progress, so better leave. Also leave if not running.
			if ((status != ServiceStatus.Running) || !Monitor.TryEnter(oLock))
				return;

			// Clear the running task to prevent reentrancy
			status = ServiceStatus.Stopping;

			// Here we can abort the thread. This is optional an depends on task
			mainThread.Abort();

			// Wait until the thrad finish
			mainThread.Join();

			// Clear
			mainThread = null;

			// Release the lock
			Monitor.PulseAll(oLock);
			Monitor.Exit(oLock);
		}

		/// <summary>
		/// 
		/// </summary>
		private void MainThreadTask()
		{
			status = ServiceStatus.Running;
			taskStarted.Set();
			Task();
		}

		/// <summary>
		/// When overriden in a derived class, executes the task which brings the service
		/// </summary>
		protected abstract void Task();

		#endregion
	}
}
