using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API
{
	/// <summary>
	/// Encapsulates shared variable information
	/// </summary>
	public interface ISharedVariableInfo
	{
		#region Properties

		/// <summary>
		/// Gets an array containing the list of all module names with write permission on the shared variable.
		/// A null value represents that all modules may write the shared variable
		/// </summary>
		string[] AllowedWriters { get; }

		/// <summary>
		/// Gets the creation time of the shared variable
		/// </summary>
		DateTime CreationTime { get; }

		/// <summary>
		/// Gets a value indicating if the variable is an array
		/// </summary>
		bool IsArray
		{
			get;
		}

		/// <summary>
		/// If the variable is an array gets the length of the array, else returns -1
		/// </summary>
		int Length { get; }

		/// <summary>
		/// Gets the name of the SharedVariable
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the information about the subscriptions to the shared variable
		/// </summary>
		ISharedVariableSubscriptionInfo[] Subscriptions { get; }

		/// <summary>
		/// Gets the global name of the type of the SharedVariable
		/// </summary>
		string TypeName { get; }

		#endregion
	}
}
