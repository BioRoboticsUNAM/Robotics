using System;
using Robotics.API;

namespace SampleModule
{
	class SayCommandExecuter : AsyncCommandExecuter
	{
		public SayCommandExecuter() : base ("spg_say"){ }

		protected override Response AsyncTask(Command command)
		{
			return Response.CreateFromCommand(command, true);
		}

		public override void DefaultParameterParser(string[] parameters)
		{
			return;
		}
	}
}
