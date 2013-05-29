using System;
using Robotics.API;
using SampleModule.CommandExecuters;

namespace SampleModule
{
	class Program
	{
		static void Main(string[] args)
		{
			CommandManager cmdMan = new CommandManager();
			ConnectionManager cnnMan = new ConnectionManager(2052, cmdMan);

			Engine engine = new Engine();
			cmdMan.CommandExecuters.Add(new FactorialCommandExecuter(engine));
			cmdMan.CommandExecuters.Add(new SayCommandExecuter());
			cmdMan.CommandExecuters.Add(new StatusCommandExecuter(engine));
			cmdMan.SharedVariablesLoaded += new SharedVariablesLoadedEventHandler(cmdMan_SharedVariablesLoaded);
			cmdMan.Start();
			
			


			while (true)
				System.Threading.Thread.Sleep(100);
		}

		static void cmdMan_SharedVariablesLoaded(CommandManager cmdMan)
		{
			cmdMan.Ready = true;
		}
	}
}
