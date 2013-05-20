using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using Robotics;
using Robotics.API;

namespace Robotics.AppLauncher
{
	/// <summary>
	/// Implements an application that launches applications
	/// </summary>
	public partial class FrmAppLauncher : Form
	{
		#region Variables

		private ConnectionManager cnnMan;
		private Thread mainThread;
		private ProducerConsumer<TcpPacket> dataReceived;
		private bool running;
		private bool paused;
		private object oPause;
		private ParameterizedThreadStart dlgExecute;
		private Regex rxAction;

		#endregion

		#region Constructors

		/// <summary>
		/// initializes a new instance of FrmAppLauncher
		/// </summary>
		public FrmAppLauncher()
		{
			InitializeComponent();
			icon.Icon = Properties.Resources.icoOrange;
			icon.BalloonTipText = @"Use following:

* action=""check"" processName=""Name of process to kill"" programPath=""Path of program to run"" [programArgs=""Program arguments""]

* action=""kill"" processName=""Name of process to kill""

* action=""run"" programPath=""Path of program to run"" [programArgs=""Program arguments""]
";
			oPause = new object();
			cnnMan = new ConnectionManager("AppLauncherSvc", 2300);
			cnnMan.Started += new ConnectionManagerStatusChangedEventHandler(cnnMan_Started);
			cnnMan.StatusChanged += new ConnectionManagerStatusChangedEventHandler(cnnMan_StatusChanged);
			cnnMan.Stopped += new ConnectionManagerStatusChangedEventHandler(cnnMan_Stopped);
			cnnMan.DataReceived += new ConnectionManagerDataReceivedEH(cnnMan_DataReceived);
			dataReceived = new ProducerConsumer<TcpPacket>(10);
			dlgExecute = new ParameterizedThreadStart(Execute);
			rxAction = new Regex(@"action\s*=\s*\""(?<action>[A-Za-z]+)\""", RegexOptions.Compiled);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the paused status of the current service
		/// </summary>
		public bool Paused
		{
			get { return paused; }
			protected set
			{
				if (paused == value)
					return;
				paused = value;
				if ((mainThread == null) || !mainThread.IsAlive)
					return;

				if (paused)
				{
					if (mainThread.ThreadState == ThreadState.WaitSleepJoin)
						mainThread.Interrupt();
				}
				else
				{
					if (mainThread.ThreadState == ThreadState.WaitSleepJoin)
					{
						Monitor.Enter(oPause);
						Monitor.PulseAll(oPause);
						Monitor.Exit(oPause);
					}
				}
			}
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Executes the provided action
		/// </summary>
		/// <param name="oAction">Action to execute</param>
		private void Execute(object oAction)
		{
			IAction action = oAction as IAction;
			if (action == null)
				return;
			action.Execute();
		}

		/// <summary>
		/// Executes the main tasks of the service
		/// </summary>
		private void MainThreadTask()
		{
			TcpPacket packet;
			int i;
			IAction action;
			//Thread thread;

			if (!cnnMan.IsRunning)
				cnnMan.Start();

			running = true;
			while (running)
			{
				try
				{
					if (Paused && Monitor.TryEnter(oPause, 1))
					{
						while (Paused) Monitor.Wait(oPause);
						Monitor.Exit(oPause);
					}
					packet = dataReceived.Consume();
					if ((packet != null) && packet.IsAnsi)
					{
						for (i = 0; i < packet.DataStrings.Length; ++i)
						{
							if (Parse(packet.DataStrings[i], out action))
							{
								//thread = new Thread(dlgExecute);
								//thread.IsBackground = true;
								//thread.Start(action);
								icon.Icon = Properties.Resources.icoGreen;
								action.Execute();
								icon.Icon = Properties.Resources.icoBlue;
							}
						}
					}
				}
				catch (ThreadInterruptedException tiex)
				{
					tiex.GetType();
					continue;
				}
				catch (ThreadAbortException taex)
				{
					running = false;
					if (cnnMan.IsRunning)
						cnnMan.Stop();
					taex.GetType();
					return;
				}
				catch
				{
					continue;
				}
			}

			if (cnnMan.IsRunning)
				cnnMan.Stop();
		}

		/// <summary>
		/// Parses an string and gets the action contained in it
		/// </summary>
		/// <param name="sAction">string containing the action to parse</param>
		/// <param name="action">The action extracted from string</param>
		/// <returns>true if action parsed successfully, false otherwise</returns>
		private bool Parse(string sAction, out IAction action)
		{
			Match m = rxAction.Match(sAction);
			string actionName;
			action = null;

			if (!m.Success)
				return false;
			actionName = m.Result("${action}");

			switch (actionName.ToLower())
			{
				case "checkprocess":
					if (ActionCheck.TryParse(sAction, out action))
					{
						return true;
					}
					break;
				case "kill":
					if (ActionKill.TryParse(sAction, out action))
						return true;
					break;

				case "run":
					if (ActionRun.TryParse(sAction, out action))
					{
						((ActionRun)action).UserName = "Robocup";
						return true;
					}
					break;
			}
			return false;
		}

		/// <summary>
		/// Configures the Main Thread
		/// </summary>
		private void SetupMainThread()
		{
			StopMainThread();
			running = true;
			Paused = false;
			mainThread = new Thread(new ThreadStart(MainThreadTask));
			mainThread.IsBackground = true;
		}

		/// <summary>
		/// Stops the Main thread
		/// </summary>
		private void StopMainThread()
		{
			if ((mainThread != null) && (mainThread.IsAlive))
			{
				if (Paused)
				{
					Paused = false;
					if (mainThread.ThreadState == ThreadState.WaitSleepJoin)
						mainThread.Interrupt();
				}

				running = false;
				if (mainThread.ThreadState == ThreadState.WaitSleepJoin)
					mainThread.Interrupt();
				Thread.Sleep(10);
				if (mainThread.IsAlive)
					mainThread.Abort();
				mainThread.Join(10);
			}
		}

		#endregion

		#region Connection Manager Event Handlers

		/// <summary>
		/// Stores the received packet in a ProducerConsumer queue
		/// </summary>
		/// <param name="cnnMan">The ConnectionManager source of the data</param>
		/// <param name="packet">The received TCP packet</param>
		private void cnnMan_DataReceived(ConnectionManager cnnMan, TcpPacket packet)
		{
			if (!packet.IsAnsi)
				return;

			dataReceived.Produce(packet);
		}

		private void cnnMan_StatusChanged(ConnectionManager connectionManager)
		{
			if (cnnMan.IsServerStarted)
			{
				icon.Icon = Properties.Resources.icoBlue;
			}
			else
			{
				icon.Icon = Properties.Resources.icoOrange;
			}
		}

		private void cnnMan_Stopped(ConnectionManager connectionManager)
		{
			icon.Icon = Properties.Resources.icoOrange;
		}

		private void cnnMan_Started(ConnectionManager connectionManager)
		{
			if (cnnMan.IsServerStarted)
			{
				icon.Icon = Properties.Resources.icoBlue;
			}
			else
			{
				icon.Icon = Properties.Resources.icoOrange;
			}
		}

		#endregion

		#region Form Eventhandlers

		private void FrmAppLauncher_Resize(object sender, EventArgs e)
		{
			this.Hide();
			if (this.WindowState == FormWindowState.Minimized)
			{
				icon.Visible = true;
				//icon.ShowBalloonTip(500);
				//this.Hide();
			}
			else
				this.WindowState = FormWindowState.Minimized;
		}

		private void FrmAppLauncher_Load(object sender, EventArgs e)
		{
			this.Hide();
			this.WindowState = FormWindowState.Minimized;
			this.Hide();
			SetupMainThread();
			mainThread.Start();
		}

		private void FrmAppLauncher_Shown(object sender, EventArgs e)
		{
			this.Hide();
		}

		private void FrmAppLauncher_FormClosing(object sender, FormClosingEventArgs e)
		{
			StopMainThread();
			if (cnnMan.IsRunning)
				cnnMan.Stop();
			icon.Visible = false;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void showHelpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			icon.ShowBalloonTip(20000);
		}

		#endregion
	}
}