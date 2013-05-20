using System;
using Robotics.API;

namespace Tester
{
	public class PerformanceTestWork
	{
		#region Variables
		
		private ConnectionManager connectionManager;
		private string commandName;
		private string parameters;
		private int executions;
		private int commandTimeout;
		private int progress;
		private TimeSpan elapsedTime;
		private int succeeded;

		#endregion

		#region Constructors
		
		public PerformanceTestWork(ConnectionManager connectionManager, string commandName, string parameters, int executions, int commandTimeout)
		{
			this.commandName = commandName;
			this.commandTimeout = commandTimeout;
			this.connectionManager = connectionManager;
			this.executions = executions;
			this.parameters = parameters;
			this.progress = 0;
			this.succeeded = 0;
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		public ConnectionManager ConnectionManager { get { return connectionManager; } }
		public string CommandName { get { return commandName; } }
		public string Parameters { get { return parameters; } }
		public int Executions { get { return executions; } }
		public int CommandTimeout { get { return commandTimeout; } }
		public int Progress {
			get {
				return progress;
			}
			set
			{
				if ((value < 0) || (value > 100)) throw new ArgumentOutOfRangeException();
				progress = value;
			}
		}
		public int Succeeded
		{
			get
			{
				return succeeded;
			}
			set
			{
				if ((value < 0) || (value > executions)) throw new ArgumentOutOfRangeException();
				succeeded = value;
			}
		}
		public TimeSpan Elapsed
		{
			get {
				return elapsedTime;
			}
			set
			{
				elapsedTime = value;
			}
		}

		#endregion

		#region Methodos
		#endregion
	}
}
