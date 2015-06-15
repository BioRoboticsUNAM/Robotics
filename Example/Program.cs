using System;
using Robotics;
using Robotics.API;
using Robotics.API.PrimitiveSharedVariables;

namespace Example
{
	/// <summary>
	/// This class ejemplifies how to build a Module
	/// </summary>
	public class Example
	{
		static void Main(string[] args)
		{
			// STEP 1. Create a new Module object named EXAMPLE and running on port 2011
			Module module = new Module("EXAMPLE", 2011);
			// STEP 2. Register command executers
			module.AddCommandExecuter(new SumCommandExecuter());
			// STEP 3. Start the module
			Console.WriteLine("Module {0} runing on port {1}.", module.Name, module.ConnectionManager.Port);
			Console.WriteLine("Waiting connection from Blackboard");
			module.Start();
			Console.WriteLine("Connected.");
			// STEP 4. Create the echo shared variable
			StringSharedVariable echo = new StringSharedVariable("echo");
			// STEP 5. Register the shared variable
			module.AddSharedVariable(ref echo);
			// STEP 6. Subscribe to the shared variable and create a listener
			echo.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
			echo.WriteNotification += new SharedVariableSubscriptionReportEventHadler<string>(echo_WriteNotification);
			Console.WriteLine("Shared variable echo successfully created");

			// STEP 8 Write into echo the console input.
			do
			{
				Console.Write("Enter some text: ");
				echo.TryWrite(Console.ReadLine());
			} while (!String.IsNullOrEmpty(echo.BufferedData));

			// STEP 8. Let the module to run
			module.Run();
		}

		static void echo_WriteNotification(SharedVariableSubscriptionReport<string> report)
		{
			Console.WriteLine("{0} = {1}", report.Variable.Name, report.Value);
		}
	}
}
