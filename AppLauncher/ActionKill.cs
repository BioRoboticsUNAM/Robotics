using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Robotics.AppLauncher
{
	/// <summary>
	/// Represents an action that checks if a process is running and kills it
	/// </summary>
	internal class ActionKill : IAction
	{
		#region Variables

		/// <summary>
		/// The name of the process for which check
		/// </summary>
		private string processName;
		/// <summary>
		/// The amount of time to wait for a process to terminate
		/// </summary>
		private int timeOut;

		#endregion

		#region Constructor

		/// <summary>
		/// Initiates a new instance of ActionKill
		/// </summary>
		/// <param name="processName">The name of the process to kill</param>
		/// <param name="timeOut"> The maximum amount of time to wait for the process to end</param>
		public ActionKill(string processName, int timeOut)
		{
			this.processName = processName;
			this.timeOut = timeOut;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the process to kill
		/// </summary>
		public string ProcessName
		{
			get { return processName; }
		}

		/// <summary>
		/// Gets the amount of time to wait for a process to terminate
		/// </summary>
		public int TimeOut
		{
			get { return timeOut; }
		}

		/// <summary>
		/// Gets the type of this IAction instance
		/// </summary>
		public ActionType Type
		{
			get { return ActionType.Kill; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks if a process is running and kills it.
		/// </summary>
		/// <returns>true if the action was executed successfully. false otherwise</returns>
		public bool Execute()
		{
			Process[] processesToKill;
			int timeLeft;

			processesToKill = Process.GetProcessesByName(processName);
			if (processesToKill.Length < 1)
				return false;
			// Request close
			foreach (Process p in processesToKill)
			{
				try
				{
					p.CloseMainWindow();
					//p.Close();
				}
				catch { }
			}
			timeLeft = timeOut;

			// Wait for processes to close
			while (timeLeft > 0)
			{
				processesToKill = Process.GetProcessesByName(processName);
				if (processesToKill.Length == 0) return true;
				System.Threading.Thread.Sleep(10);
				timeLeft -= 10;
			}

			// Kill alive process
			foreach (Process p in processesToKill)
			{
				try
				{
					p.Close();
					System.Threading.Thread.Sleep(10);
					p.Kill();
				}
				catch { }
			}
			processesToKill = Process.GetProcessesByName(processName);
			return (processesToKill.Length == 0);
		}

		public override string ToString()
		{
			return "Kill [" + processName + "]";
		}

		#endregion

		#region Static Variables

		private static Regex rxActionKillValidator = new Regex(
			@"action\s*=\s*\""kill\""\s*processName\s*=\s*\""(?<pName>[^\""]+)\""(\s*timeOut\s*=\s*\""(?<timeOut>\d+)\"")?",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		#endregion

		#region Static Methods

		/// <summary>
		/// Converts the String representation of a action to a ActionKill object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="sAction">A string containing the action to convert</param>
		/// <param name="action">When this method returns, contains the ActionKill equivalent to the action
		/// contained in the string, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string sAction, out ActionKill action)
		{
			Match m;
			string pName;
			int timeOut;

			action = null;
			m = rxActionKillValidator.Match(sAction);
			if (!m.Success)
				return false;

			pName = m.Result("${pName}");
			if(!Int32.TryParse(m.Result("${timeOut}"), out timeOut))
				timeOut = 1000;
			action = new ActionKill(pName, timeOut);
			return true;
		}

		/// <summary>
		/// Converts the String representation of a action to a ActionKill object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="sAction">A string containing the action to convert</param>
		/// <param name="action">When this method returns, contains the ActionKill equivalent to the action
		/// contained in the string, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the node parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string sAction, out IAction action)
		{
			bool result;
			ActionKill kAction;
			result = TryParse(sAction, out kAction);
			action = kAction;
			return result;
		}

		#endregion
	}
}
