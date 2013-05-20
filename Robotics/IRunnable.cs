using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics
{
	/// <summary>
	/// Represents the method that will handle the ExecutionAborted, ExecutionCompleted and ExecutionStarted events
	/// </summary>
	/// <param name="runnableObject">The object that raises the event</param>
	public delegate void IRunnableObjectExecutionStateChanged(IRunnable runnableObject);

	/// <summary>
	/// Implements an object which executes in a separate thread
	/// </summary>
	public interface IRunnable
	{

		#region Events

		/// <summary>
		/// Occurs when the execution of the IRunnable object is aborted
		/// </summary>
		event IRunnableObjectExecutionStateChanged ExecutionAborted;

		/// <summary>
		/// Occurs when the execution of the IRunnable object is completed
		/// </summary>
		event IRunnableObjectExecutionStateChanged ExecutionCompleted;

		/// <summary>
		/// Occurs when the execution of the IRunnable object is started
		/// </summary>
		event IRunnableObjectExecutionStateChanged ExecutionStarted;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether the IRunnable object is running
		/// </summary>
		bool IsRuning{ get; }

		/// <summary>
		/// Gets or sets a value indicating whether the IRunnable object executes or not in a background thread
		/// </summary>
		bool RunInBackground{ get; set; }

		#endregion

		#region Methodos

		/// <summary>
		/// Aborts the execution of the asynchronous task of the IRunnable object
		/// </summary>
		void Abort();

		/// <summary>
		/// Executes the asynchronous task of the IRunnable object
		/// </summary>
		void Run();

		/// <summary>
		/// Unblocks the thread that is waiting for a signal. It is commonly used to synchronize threads
		/// </summary>
		void Signal();

		#endregion
	}
}
