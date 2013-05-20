using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Paralelism
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TInput">The type of the input data</typeparam>
	/// <typeparam name="TOutput">The type of the output data</typeparam>
	public class ParallelPipesAndFilters<TInput, TOutput>
	{
		#region Variables

		/// <summary>
		/// The pipe for data input
		/// </summary>
		public IPipe<TInput> inputPipe;

		/// <summary>
		/// The pipe for data output
		/// </summary>
		public IPipe<TOutput> outputPipe;

		//private List<Filter> filters;
		//private List<IPipe> pipes;

		#endregion

		#region Constructors
		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the pipe for data input
		/// </summary>
		public IPipe<TInput> InputPipe
		{
			get { return this.inputPipe; }
			set { }
		}

		/// <summary>
		/// Gets or sets the pipe for data output
		/// </summary>
		public IPipe<TOutput> OutputPipe
		{
			get { return this.outputPipe; }
			set { }
		}
		#endregion

		#region Methodos
		#endregion
	}
}
