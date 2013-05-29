using System;
using Robotics.API;

namespace SampleModule.CommandExecuters
{
	public class FactorialCommandExecuter : AsyncCommandExecuter
	{
		private Engine engine;

		public FactorialCommandExecuter(Engine engine)
			: base("factorial")
		{
			this.engine = engine;
		}

		protected override Response AsyncTask(Command command)
		{
			int num;
			Response response;

			if (!Int32.TryParse(command.Parameters, out num))
				return Response.CreateFromCommand(command, false);

			response = Response.CreateFromCommand(command, true);
			response.Parameters = engine.Factorial(num).ToString();
			return response;
		}

		public override void DefaultParameterParser(string[] parameters)
		{
			return;
		}
	}
}
