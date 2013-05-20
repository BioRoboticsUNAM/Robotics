using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Robotics.AppLauncher
{
	/// <summary>
	/// Represents an action that checks if a process is running.
	/// If not running then the process is launched.
	/// </summary>
	internal class ActionCheck : IAction
	{
		#region Variables

		/// <summary>
		/// The name of the process for which check
		/// </summary>
		private string processName;
		/// <summary>
		/// The name of the file to run
		/// </summary>
		private string exePath;
		/// <summary>
		/// The arguments to provide to the executable
		/// </summary>
		private string arguments;
		/// <summary>
		/// The priority to start the process with
		/// </summary>
		private ProcessPriorityClass priority;
		/// <summary>
		/// Username used to launch the application
		/// </summary>
		private string userName;
		/// <summary>
		/// Password of the user used to launch application
		/// </summary>
		private System.Security.SecureString password;

		#endregion

		#region Constructors

		/// <summary>
		/// Initiates a new instance of ActionCheck
		/// </summary>
		/// <param name="processName">The name of the process for which check</param>
		/// <param name="exePath">The name of the executable file to run</param>
		/// <param name="arguments"> The arguments to provide to the executable</param>
		public ActionCheck(string processName, string exePath, string arguments)
		{
			this.processName = processName;
			if (File.Exists(exePath))
			{
				this.exePath = (new FileInfo(exePath)).FullName;
			}
			this.arguments = arguments;
		}

		/// <summary>
		/// Initiates a new instance of ActionCheck
		/// </summary>
		/// <param name="processName">The name of the process for which check</param>
		/// <param name="exePath">The name of the executable file to run</param>
		/// <param name="arguments"> The arguments to provide to the executable</param>
		/// <param name="priority">The priority to start the process with</param>
		public ActionCheck(string processName, string exePath, string arguments, ProcessPriorityClass priority) : this(processName, exePath, arguments)
		{
			this.priority = priority;
		}

		#endregion

		#region Properties


		/// <summary>
		/// Gets the argument string to provide to the executable
		/// </summary>
		public string Arguments
		{
			get { return arguments; }
		}

		/// <summary>
		/// Gets the path of the file to run
		/// </summary>
		public string ExePath
		{
			get { return exePath; }
		}

		/// <summary>
		/// Gets the name of the process to check for
		/// </summary>
		public string ProcessName
		{
			get { return processName; }
		}		

		/// <summary>
		/// Gets the type of this IAction instance
		/// </summary>
		public ActionType Type
		{
			get { return ActionType.Check; }
		}

		/// <summary>
		/// Gets the priority to start the process with
		/// </summary>
		public ProcessPriorityClass Priority
		{
			get { return priority; }
		}
		
		/// <summary>
		/// Gets or sets the Username used to launch the application
		/// </summary>
		public string UserName
		{
			get { return userName; }
			set { userName = value; }
		}
		/// <summary>
		/// Gets or sets the Password of the user used to launch application
		/// </summary>
		public System.Security.SecureString Password
		{
			get { return password; }
			set { password = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks if a process is running.
		/// If not running then the process is launched.
		/// </summary>
		/// <returns>true if the action was executed successfully. false otherwise</returns>
		public bool Execute()
		{
			//bool running;
			Process[] processList = Process.GetProcesses();
			foreach (Process p in processList)
			{
				if (p.ProcessName == processName)
					return true;//running = true;
			}
			if (!File.Exists(exePath)) return false;
			try
			{
				Process process;
				ProcessStartInfo psi;
				psi = new ProcessStartInfo(exePath, arguments);
				psi.WorkingDirectory = (new FileInfo(exePath)).DirectoryName;
				process = new Process();
				process.StartInfo = psi;
				//if ((userName != null) && (userName.Length > 3))
				//{
				//	psi.UserName = userName;
				//	if ((password != null) && (password.Length > 1))
				//		psi.Password = password;
				//}
				//process.PriorityClass = priority;
				process.Start();
				process.PriorityClass = priority;
				
				return true;
			}
			catch
			{
				return false;
			}
		}

		public override string ToString()
		{
			return "Check for process [" + processName + "] if not running execute \"" + exePath + " " + arguments + "\"";
		}

		#endregion

		#region Static Variables

		private static Regex rxActionCheckValidator = new Regex(
			@"action=action\s*=\s*\""checkProcess\""\s*processName\s*=\s*\""(?<pName>[^\""]+)\""\s*programPath\s*=\s*\""(?<programPath>[^\""]+)\""(\s*programArgs\s*=\s*\""(?<programArgs>[^\""]*)\"")?",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		#endregion

		#region Static Methods

		/// <summary>
		/// Converts the String representation of a action to a ActionCheck object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="sAction">A string containing the action to convert</param>
		/// <param name="action">When this method returns, contains the ActionCheck equivalent to the action
		/// contained in the string, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string sAction, out ActionCheck action)
		{
			Match m;
			string pName;
			string programPath;
			string programArgs;

			action = null;
			m = rxActionCheckValidator.Match(sAction);
			if (!m.Success)
				return false;

			pName = m.Result("${pName}");
			programPath = m.Result("${programPath}");
			programArgs = (m.Result("${programArgs}") == null) ? "" : m.Result("${programArgs}");
			
			action = new ActionCheck(pName, programPath, programArgs);
			return true;
		}

		/// <summary>
		/// Converts the String representation of a action to a ActionCheck object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="sAction">A string containing the action to convert</param>
		/// <param name="action">When this method returns, contains the ActionCheck equivalent to the action
		/// contained in the string, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string sAction, out IAction action)
		{
			bool result;
			ActionCheck kAction;
			result = TryParse(sAction, out kAction);
			action = kAction;
			return result;
		}

		#endregion
	}
}
