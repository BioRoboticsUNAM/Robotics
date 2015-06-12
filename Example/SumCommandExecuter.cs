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
		public SumCommandExecuter() : base("sum") { }

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
