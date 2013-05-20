using System;
using System.Threading;

namespace Robotics.Paralelism
{
	/// <summary>
	/// Encapsulates the data to be used by a Worker thread of a ManagerWorkers object
	/// </summary>
	///<typeparam name="TManagerData">The type of data used by the manager</typeparam>
	///<typeparam name="TManagerResult">The type of data returned by the manager</typeparam>
	/// <typeparam name="TWorkerData">The type of data used by the workers</typeparam>
	/// <typeparam name="TWorkerResult">The type of data returned by the workers</typeparam>
	public class WorkerTaskObject<TManagerData, TManagerResult, TWorkerData, TWorkerResult>
	{
		#region Variables

		/// <summary>
		/// The data for the worker thread
		/// </summary>
		private TWorkerData data;

		/// <summary>
		/// The number of the worker
		/// </summary>
		private readonly int workerNumber;

		/// <summary>
		/// The thread used to perform the worker task
		/// </summary>
		private readonly Thread workerThread;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the WorkerTaskObject
		/// </summary>
		/// <param name="worker">The worker used to initialize the object</param>
		/// <param name="data">The data to be used by the worker</param>
		internal WorkerTaskObject(ManagerWorkers<TManagerData, TManagerResult, TWorkerData, TWorkerResult>.Worker worker, TWorkerData data)
		{
			this.workerNumber = worker.WorkerNumber;
			this.workerThread = worker.WorkerThread;
			this.data = data;
		}

		#endregion

		#region Properties
		
		/// <summary>
		/// Gets the data for the worker thread
		/// </summary>
		private TWorkerData Data
		{
			get { return this.data; }
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
		protected Thread WorkerThread
		{
			get { return this.workerThread; }
		}

		#endregion
	}

	/// <summary>
	/// Encapsulates the data to be used by a Worker thread of a ManagerWorkers object
	/// </summary>
	public class WorkerTaskObject : WorkerTaskObject<object, object, object, object>
	{

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the WorkerTaskObject
		/// </summary>
		/// <param name="worker">The worker used to initialize the object</param>
		/// <param name="data">The data to be used by the worker</param>
		internal WorkerTaskObject(ManagerWorkers<object, object, object, object>.Worker worker, object data)
			:base(worker, data)
		{
		}

		#endregion
	}
}
