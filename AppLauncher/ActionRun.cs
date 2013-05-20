using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Robotics.AppLauncher
{
	/// <summary>
	/// Represents an action that executes a process
	/// </summary>
	class ActionRun : IAction
	{
		#region Variables

		/// <summary>
		/// The name of the file to run
		/// </summary>
		private string exePath;
		/// <summary>
		/// The arguments to provide to the executable
		/// </summary>
		private string arguments;
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
		/// Initiates a new instance of ActionRun
		/// </summary>
		/// <param name="exePath">The name of the executable file to run</param>
		/// <param name="arguments"> The arguments to provide to the executable</param>
		public ActionRun(string exePath, string arguments)
		{
			if (File.Exists(exePath))
			{
				this.exePath = (new FileInfo(exePath)).FullName;
			}
			this.arguments = arguments;
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
		/// Gets the type of this IAction instance
		/// </summary>
		public ActionType Type
		{
			get { return ActionType.Check; }
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
			ProcessStartInfo psi;
			Process p;
			bool result = false;
			if (!File.Exists(exePath)) return result;
			try
			{
				psi = new ProcessStartInfo(exePath, arguments);
				psi.WorkingDirectory = (new FileInfo(exePath)).DirectoryName;
				psi.UseShellExecute = true;
				psi.WindowStyle = ProcessWindowStyle.Normal;
				//if ((userName != null) && (userName.Length > 3))
				//{
				//	psi.UseShellExecute = false;
				//	psi.UserName = userName;
				//	if((password != null) &&(password.Length > 1))
				//		psi.Password = password;
				//}
				p = new Process();
				p.StartInfo = psi;
				result = p.Start();
				p.WaitForInputIdle(500);
				return result;
			}
			catch { return result; }
		}

		public override string ToString()
		{
			return "Execute \"" + exePath + "\"";
		}

		#endregion

		#region Static Variables

		private static Regex rxActionRunValidator = new Regex(
			@"action\s*=\s*\""run\""\s*programPath\s*=\s*\""(?<programPath>[^\""]+)\""(\s*programArgs\s*=\s*\""(?<programArgs>[^\""]*)\"")?",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		#endregion

		#region Static Methods

		/// <summary>
		/// Converts the String representation of a action to a ActionRun object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="sAction">A string containing the action to convert</param>
		/// <param name="action">When this method returns, contains the ActionRun equivalent to the action
		/// contained in the string, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string sAction, out ActionRun action)
		{
			Match m;
			string programPath;
			string programArgs;

			action = null;
			m = rxActionRunValidator.Match(sAction);
			if (!m.Success)
				return false;

			programPath = m.Result("${programPath}");
			programArgs = (m.Result("${programArgs}") == null) ? "" : m.Result("${programArgs}");
			action = new ActionRun(programPath, programArgs);
			return true;
		}

		/// <summary>
		/// Converts the String representation of a action to a ActionRun object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="sAction">A string containing the action to convert</param>
		/// <param name="action">When this method returns, contains the ActionRun equivalent to the action
		/// contained in the string, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string sAction, out IAction action)
		{
			bool result;
			ActionRun rAction;
			result = TryParse(sAction, out rAction);
			action = rAction;
			return result;
		}

		#endregion
	}
}
