using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics
{
	/// <summary>
	/// Represents a Service
	/// </summary>
	public interface IService
	{
		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether the IService object is running
		/// </summary>
		bool IsRunning { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Starts the service
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the service
		/// </summary>
		void Stop();

		#endregion
	}
}
