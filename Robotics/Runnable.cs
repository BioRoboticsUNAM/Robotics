using System;
using System.Threading;
using System.Text;

namespace Robotics
{
	/// <summary>
	/// Represents a class which executes in a separate thread
	/// </summary>
	public abstract class Runnable : IRunnable
	{
		#region Variables

		/// <summary>
		/// The thread that executes the async task
		/// </summary>
		private Thread mainThread;

		/// <summary>
		/// Represents the Task method
		/// </summary>
		private ThreadStart mainThreadTask;

		/// <summary>
		/// Event used to synchronize threads
		/// </summary>
		private AutoResetEvent autoEvent;

		/// <summary>
		/// Indicaties whether the Runnable object executes or not in a background thread
		/// </summary>
		private bool runInBackground;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of a Runnable class
		/// </summary>
		public Runnable()
		{
			this.mainThreadTask = new ThreadStart(this.MainThreadTask);
			this.runInBackground = true;
			this.autoEvent = new AutoResetEvent(false);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the execution of the Runnable object is aborted
		/// </summary>
		public event IRunnableObjectExecutionStateChanged ExecutionAborted;

		/// <summary>
		/// Occurs when the execution of the Runnable object is completed
		/// </summary>
		public event IRunnableObjectExecutionStateChanged ExecutionCompleted;

		/// <summary>
		/// Occurs when the execution of the Runnable object is started
		/// </summary>
		public event IRunnableObjectExecutionStateChanged ExecutionStarted;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the thread that executes the async task
		/// </summary>
		protected Thread MainThread
		{
			get { return this.mainThread; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Runnable object executes or not in a background thread
		/// </summary>
		public bool RunInBackground
		{
			get { return runInBackground; }
			set
			{
				if ((this.mainThread != null) && this.mainThread.IsAlive)
					return;
				runInBackground = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the Runnable object is running
		/// </summary>
		public bool IsRuning
		{
			get { return ((mainThread != null) && mainThread.IsAlive); }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Aborts the execution of the asynchronous task of the Runnable object
		/// </summary>
		public void Abort()
		{
			if ((this.mainThread == null) || !this.mainThread.IsAlive)
				return;
			this.mainThread.Abort();
			this.mainThread.Join();
			this.mainThread = null;
		}

		/// <summary>
		/// Executes the asynchronous task of the Runnable object
		/// </summary>
		public void Run()
		{
			if ((this.mainThread != null) && this.mainThread.IsAlive)
				return;
			this.mainThread = new Thread(mainThreadTask);
			mainThread.IsBackground = runInBackground;
			mainThread.Start();
		}

		/// <summary>
		/// Raises events and executes the Task method
		/// </summary>
		private void MainThreadTask()
		{
			OnExecutionStarted();
			try
			{
				Task();
			}
			catch(ThreadAbortException)
			{
				OnExecutionAborted();
				return;
			}
			OnExecutionCompleted();
			this.mainThread = null;
		}

		/// <summary>
		/// Raises the ExecutionAborted event.
		/// When overriding OnExecutionAborted in a derived class, be sure to call the base class's OnExecutionAborted method so that registered delegates receive the event. 
		/// </summary>
		protected void OnExecutionAborted()
		{
			try
			{
				if (this.ExecutionAborted != null)
					this.ExecutionAborted(this);
			}
			catch { }
		}

		/// <summary>
		/// Raises the ExecutionCompleted event.
		/// When overriding OnExecutionCompleted in a derived class, be sure to call the base class's OnExecutionCompleted method so that registered delegates receive the event. 
		/// </summary>
		protected void OnExecutionCompleted()
		{
			try
			{
				if (this.ExecutionCompleted != null)
					this.ExecutionCompleted(this);
			}
			catch { }
		}

		/// <summary>
		/// Raises the ExecutionStarted event.
		/// When overriding OnExecutionStarted in a derived class, be sure to call the base class's OnExecutionStarted method so that registered delegates receive the event. 
		/// </summary>
		protected void OnExecutionStarted()
		{
			try
			{
				if (this.ExecutionStarted != null)
					this.ExecutionStarted(this);
			}
			catch { }
		}		

		/// <summary>
		/// Unblocks the thread that is waiting for a signal. It is commonly used to synchronize threads
		/// </summary>
		public void Signal()
		{
			autoEvent.Set();
		}

		/// <summary>
		/// Blocks the thread until a signal is received (another thread calls the Signal method)
		/// </summary>
		protected void Wait()
		{
			autoEvent.WaitOne();
		}

		/// <summary>
		/// Blocks the thread until a signal is received (another thread calls the Signal method) or the specified time elapses
		/// </summary>
		/// <param name="millisecondsTimeout">The number of milliseconds to wait, or -1 to wait indefinitely</param>
		protected void Wait(int millisecondsTimeout)
		{
			autoEvent.WaitOne(millisecondsTimeout);
		}

		/// <summary>
		/// Blocks the thread until a signal is received (another thread calls the Signal method) or the specified time elapses
		/// </summary>
		/// <param name="timeout">A TimeSpan that represents the number of milliseconds to wait, or a TimeSpan that represents -1 milliseconds to wait indefinitely</param>
		protected void Wait(TimeSpan timeout)
		{
			autoEvent.WaitOne(timeout);
		}

		/// <summary>
		/// When overriden, executes the asynchronous task
		/// </summary>
		protected abstract void Task();

		#endregion
	}
}
