using System;
using Robotics;
using Robotics.API;

namespace Example
{
	/// <summary>
	/// Example of a synchronous command executer
	/// </summary>
	public class SumCommandExecuter : SyncCommandExecuter
	{
		/// <summary>
		/// Default constructor calls base constructor and names the command
		/// </summary>
		public SumCommandExecuter() : base("sum") { }

		/// <summary>
		/// By setting ParametersRequired to true we make mandatory the use of parameters
		/// </summary>
		public override bool ParametersRequired
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// This is the method which executes the command.
		/// </summary>
		protected override Response SyncTask(Command command)
		{
			string[] parts = command.Parameters.Split(' ');
			double a = Double.Parse(parts[0]);
			double b = Double.Parse(parts[1]);
			Response r = Response.CreateFromCommand(command, true);
			r.Parameters = String.Format("{0}", a + b);
			return r;

		}
	}
}
