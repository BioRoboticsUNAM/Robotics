using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Paralelism
{
	/// <summary>
	/// Represents a Filter for the Pipes and Filters pattern
	/// </summary>
	public interface IFilter
	{
		#region Variables

		#endregion

		#region Constructors
		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the IFilter input data type.
		/// </summary>
		Type InputType { get; }

		/// <summary>
		/// Gets a value indicating whether the IFilter is running asynchronously
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Gets the IFilter output data type
		/// </summary>
		Type OutputType { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the IFilter object will execute
		/// or not in a background thread while running asynchronously
		/// </summary>
		bool RunInBackground { get; set; }

		#endregion

		#region Methodos

		/// <summary>
		/// Executes the Filter task over the data provided by the input pipe
		/// and writes the result to the output pipe.
		/// Is est, executes the operation: OutputPipe.Write(Filter(InputPipe.Read()));
		/// </summary>
		void FilterNext();

		#endregion
	}
}
