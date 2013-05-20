using System;
using System.Collections.Generic;
using System.Text;
using Robotics.API;

namespace Tester
{
	[SerializableAttribute]
	public class AutoResponder
	{
		#region Variables

		private string module;
		private string commandName;
		private string parameters;
		private int failureRate;

		[NonSerializedAttribute]
		private static Random rnd = new Random();

		#endregion

		#region Constructors

		public AutoResponder(string module, string commandName, string parameters, int failureRate)
		{
			this.commandName = commandName;
			this.module = module;
			this.parameters = parameters;
			this.failureRate = failureRate;
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		public string Module { get { return module; } }
		public string CommandName { get { return commandName; } }
		public string Parameters { get { return parameters; } }
		public int FailureRate { get { return failureRate; } }

		#endregion

		#region Methodos

		public bool Match(Command command)
		{
			if(command == null)
				return false;
			return (((module == "[ANY]") || (module == command.SourceModule)) && (commandName == command.CommandName));
		}

		public Response GetResponse(Command command)
		{
			bool success;
			if (!Match(command))
				return null;
			success = failureRate < rnd.Next(1, 100);
			Response response = Response.CreateFromCommand(command, success);
			if(parameters.Length != 0)
				response.Parameters = parameters;
			return response;
		}

		public override bool Equals(object obj)
		{
			AutoResponder other = obj as AutoResponder;
			if (other == null)
				return false;
			return (this.commandName == other.commandName) && (this.module == other.module);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(module);
			sb.Append(' ');
			sb.Append(commandName);
			sb.Append(' ');
			sb.Append('"');
			if (parameters.Length > 10)
			{
				sb.Append(parameters.Substring(0, 8));
				sb.Append("...");
			}
			else
				sb.Append(parameters);
			sb.Append('"');
			sb.Append(' ');
			if (failureRate <= 0)
				sb.Append('1');
			else
			{
				sb.Append('0');
				if (failureRate < 100)
				{
					sb.Append(" (");
					sb.Append(failureRate);
					sb.Append("%)");
				}
			}
			return sb.ToString();
		}

		#endregion
	}
}
