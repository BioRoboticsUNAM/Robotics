using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;

namespace Robotics.AppLauncher
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1) return;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FrmAppLauncher());
		}
	}
}