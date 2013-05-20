using System;
using System.Collections.Generic;
using System.Text;
using Robotics.API;

namespace Tester
{
	[SerializableAttribute]
	public class QuickCommand
	{
		#region Variables

		private string module;
		private string commandName;
		private string parameters;
		private int id;
		private int currentId;

		#endregion

		#region Constructors

		public QuickCommand(string moduleName, string commandName, string parameters, int id)
		{
			this.commandName = commandName;
			this.module = moduleName;
			this.parameters = parameters;
			this.id = id;
			if (id < -1)
				currentId = 0;
			else
				currentId = id;
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		public string Module { get { return module; } }
		public string CommandName { get { return commandName; } }
		public string Parameters { get { return parameters; } }
		public int Id { get { return id; } }
		public int CurrentId { get { return currentId; } }

		#endregion

		#region Methodos

		public bool Match(ConnectionManager cnnMan)
		{
			if (cnnMan == null)
				return false;
			return (module == cnnMan.ModuleName);
		}

		public Command GetCommand(ConnectionManager cnnMan)
		{
			if (!Match(cnnMan))
				return null;
			Command cmd = new Command(cnnMan, commandName, parameters, currentId);
			if (id < -1)
				++currentId;
			return cmd;
		}

		public override bool Equals(object obj)
		{
			QuickCommand other = obj as QuickCommand;
			if (other == null)
				return false;
			return (this.commandName == other.commandName) && (this.parameters == other.parameters) && (this.id == other.id) && (this.module == other.module);
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
			sb.Append(parameters);
			sb.Append('"');
			if (id < -1)
				sb.Append(" @#");
			else if (id != -1)
			{
				sb.Append(" @");
				sb.Append(id);
			}
			return sb.ToString();
		}

		#endregion

	}
}
