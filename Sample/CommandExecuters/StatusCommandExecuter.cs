using System;
using Robotics.API;

namespace SampleModule.CommandExecuters
{
	public class StatusCommandExecuter : SyncCommandExecuter
	{
		private Engine engine;

		public StatusCommandExecuter(Engine engine)
			: base("status")
		{
			this.engine = engine;
		}

		public override bool ParametersRequired
		{
			get { return false; }
		}

		protected override Response SyncTask(Command command)
		{
			bool result = true;

			if (String.IsNullOrEmpty(command.Parameters))
				command.Parameters = engine.Enabled ? "enabled" : "disabled";
			else if (command.Parameters == "enabled")
				engine.Start();
			else if (command.Parameters == "enabled")
				engine.Stop();
			else
				result = false;
			return Response.CreateFromCommand(command, result);
		}
	}
}
