using System;
using System.Threading;

namespace Robotics
{
	/// <summary>
	/// Implements a base class with resource management control to prevent two processes access the same shared resource at the same time
	/// </summary>
	public abstract class SharedResource
	{
		#region Variables
		
		/// <summary>
		/// Stores a value indicating if the resource is free
		/// </summary>
		private bool busy;

		/// <summary>
		/// The resource object for lock
		/// </summary>
		private object resourceObject;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of ShredResourceAccessControl
		/// </summary>
		public SharedResource()
		{
			this.busy = false;
			resourceObject = new object();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the ShredResource is busy
		/// The ShredResource is busy when the resource is not free
		/// </summary>
		public bool Busy
		{
			get { return busy; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Tries to get exclusive acces of the resource
		/// </summary>
		/// <returns></returns>
		public bool GetResource()
		{
			if (busy)
				return false;
			if (Monitor.TryEnter(resourceObject))
			{
				busy = true;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Releases the exclusive acces of the resource
		/// </summary>
		/// <returns></returns>
		public void FreeResource()
		{
			if (!busy)
				return;
			busy = false;
			Monitor.Exit(resourceObject);
		}

		#endregion
	}
}
