using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Robotics;
using Robotics.API;
using Robotics.Controls;

namespace Tester
{
	public partial class FrmTester : Form
	{
		#region Variables

		private Regex rxModuleNameValidator = new Regex(@"[A-Z][A-Z\-]*", RegexOptions.Compiled);
		private Dictionary<string, ConnectionManager> modules;
		private ConnectionManager selectedConnectionManager;
		private StringEventHandler dlgConsole;
		private List<AutoResponder> autoResponders;
		private AutoResponder selectedAutoResponder;
		private List<Prompter> prompters;
		private Prompter selectedPrompter;
		private List<QuickCommand> quickCommands;
		private QuickCommand selectedQuickCommand;
		private ProducerConsumer<ReceivedPacket> dataReceived;
		private List<Response> receivedResponses;
		private Thread mainThread;
		private ThreadStart dlgMainThreadTask;
		private bool running;
		private bool performanceTestInProgress;
		private GetPromptDialogCaller dlgGetPromptDialog;
		private ShowPromptDialogCaller dlgShowPromptDialog;
		private Stopwatch stopwatch;
		private TextBoxStreamWriter log;
		
		#endregion
		
		#region Constructors

		public FrmTester()
		{
			InitializeComponent();
			log = new TextBoxStreamWriter(txtConsole);

			modules = new Dictionary<string, ConnectionManager>();
			autoResponders = new List<AutoResponder>();
			prompters = new List<Prompter>();
			dataReceived = new ProducerConsumer<ReceivedPacket>(100);
			receivedResponses = new List<Response>();
			quickCommands = new List<QuickCommand>();

			ValidateAddModuleButton();
			ValidateAddAutoResponderButton();
			ValidateAddPrompterButton();
			ValidateStartPerformanceTest();
			ValidateAddQuickCommandButton();
			SelectModule(null);
			SelectResponder(null);
			SelectPrompter(null);
			SelectQuickCommand(null);

			dlgConsole = new StringEventHandler(Console);
			dlgMainThreadTask = new ThreadStart(MainThreadTask);
			dlgGetPromptDialog = new GetPromptDialogCaller(GetPromptDialog);
			dlgShowPromptDialog = new ShowPromptDialogCaller(ShowPromptDialog);

			stopwatch = new Stopwatch();
			mainThread = new Thread(dlgMainThreadTask);
			mainThread.IsBackground = true;
			mainThread.Start();
		}

		#endregion

		#region Delegates

		internal delegate FrmPromptDialog GetPromptDialogCaller();
		internal delegate DialogResult ShowPromptDialogCaller(FrmPromptDialog promptDialog);

		#endregion

		#region Methods

		private void AddAutoResponder(string moduleName, string commandName, string parameters, int failureRate)
		{
			if (!modules.ContainsKey(moduleName) && (moduleName != "[ANY]"))
				return;
			if (failureRate < 0) failureRate = 0;
			if (failureRate > 100) failureRate = 100;
			AutoResponder ar = new AutoResponder(moduleName, commandName, parameters, failureRate);
			if (autoResponders.Contains(ar))
				return;
			autoResponders.Add(ar);
			lstAutoResponders.Items.Add(ar);
			if (lstAutoResponders.Items.Contains(ar))
				lstAutoResponders.SelectedItem = ar;
		}

		private void AddModule(string moduleName, int port)
		{
			if ((port <= 1024) || (port >= 65535) || !rxModuleNameValidator.IsMatch(moduleName) || modules.ContainsKey(moduleName))
				return;
			if (modules.ContainsKey(moduleName))
				return;
			ConnectionManager cnnMan = new ConnectionManager(moduleName, port);
			cnnMan.Started += new ConnectionManagerStatusChangedEventHandler(cnnMan_Started);
			cnnMan.Stopped += new ConnectionManagerStatusChangedEventHandler(cnnMan_Stopped);
			cnnMan.ClientConnected += new TcpClientConnectedEventHandler(cnnMan_ClientConnected);
			cnnMan.ClientDisconnected += new TcpClientDisconnectedEventHandler(cnnMan_ClientDisconnected);
			cnnMan.DataReceived += new ConnectionManagerDataReceivedEH(cnnMan_DataReceived);
			cnnMan.Start();
			modules.Add(moduleName, cnnMan);
			lstModules.Items.Add(moduleName);
			cmbARModule.Items.Add(moduleName);
			cmbPrmptModule.Items.Add(moduleName);
			cmbPerformanceTestModule.Items.Add(moduleName);
			cmbQCModule.Items.Add(moduleName);
		}

		private void AddPrompter(string moduleName, string commandName)
		{
			if (!modules.ContainsKey(moduleName) && (moduleName !="[ANY]"))
				return;
			Prompter prompter = new Prompter(this, moduleName, commandName);
			if (prompters.Contains(prompter))
				return;
			prompter.ResponseSent += new ResponseSentEventHandler(prompter_ResponseSent);
			prompters.Add(prompter);
			lstPrompter.Items.Add(prompter);
			if (lstPrompter.Items.Contains(prompter))
				lstPrompter.SelectedItem = prompter;
		}

		private void AddQuickCommand(string moduleName, string commandName, string parameters, int id)
		{
			if (!modules.ContainsKey(moduleName))
				return;
			if (id > 99) id = 99;
			QuickCommand qc = new QuickCommand(moduleName, commandName, parameters, id);
			if (quickCommands.Contains(qc))
				return;
			quickCommands.Add(qc);
			lstQuickCommands.Items.Add(qc);
			if (lstQuickCommands.Items.Contains(qc))
				lstQuickCommands.SelectedItem = qc;
		}

		private void Console(string text)
		{
			log.WriteLine(text);
			//if (txtConsole == null) return;

			//if (txtConsole.InvokeRequired)
			//{
			//	if (txtConsole.IsHandleCreated && !txtConsole.IsDisposed && !txtConsole.Disposing)
			//		txtConsole.BeginInvoke(dlgConsole, text);
			//	return;
			//}
			
			//lock (txtConsole)
			//{
			//	if (!txtConsole.IsHandleCreated || txtConsole.IsDisposed || txtConsole.Disposing)
			//		return;
			//	txtConsole.AppendText(text);
			//	txtConsole.AppendText("\r\n");
			//}
		}

		internal FrmPromptDialog GetPromptDialog()
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
					return null;
				return (FrmPromptDialog)this.Invoke(dlgGetPromptDialog);
			}
			FrmPromptDialog promptDialog = new FrmPromptDialog();
			promptDialog.Owner = this;
			promptDialog.StartPosition = FormStartPosition.CenterParent;
			return promptDialog;
		}

		private void LoadData()
		{
			int i;
			KeyValuePair<string, int> modulePortInfo;
			Prompter prompter;
			AutoResponder autoResponder;
			QuickCommand quickCommand;

			Config.Default.Load();

			for (i = 0; i < Config.Default.Modules.Count; ++i)
			{
				modulePortInfo = (KeyValuePair<string, int>)Config.Default.Modules[i];
				AddModule(modulePortInfo.Key, modulePortInfo.Value);
			}

			for (i = 0; i < Config.Default.AutoResponders.Count; ++i)
			{
				autoResponder = Config.Default.AutoResponders[i] as AutoResponder;
				if (autoResponder == null)
					continue;
				AddAutoResponder(autoResponder.Module, autoResponder.CommandName, autoResponder.Parameters, autoResponder.FailureRate);
			}

			for (i = 0; i < Config.Default.Prompters.Count; ++i)
			{
				prompter = Config.Default.Prompters[i] as Prompter;
				if (prompter == null)
					continue;
				AddPrompter(prompter.Module, prompter.CommandName);
			}

			for (i = 0; i < Config.Default.QuickCommands.Count; ++i)
			{
				quickCommand = Config.Default.QuickCommands[i] as QuickCommand;
				if (quickCommand == null)
					continue;
				AddQuickCommand(quickCommand.Module, quickCommand.CommandName, quickCommand.Parameters, quickCommand.Id);
			}
		}

		private void MainThreadTask()
		{
			running = true;
			int i;
			ReceivedPacket packet;
			Command command;
			Response response;
			string s;
			bool handled;

			while (running)
			{
				packet = dataReceived.Consume();
				if (running && (packet != null) && (packet.TcpPacket.IsAnsi))
				{
					for (i = 0; i < packet.TcpPacket.DataStrings.Length; ++i)
					{
						s = packet.TcpPacket.DataStrings[i];
						handled = false;
						if (Response.TryParse(packet.ConnectionManager, s, out response))
						{
							//response.SourceModule = packet.ConnectionManager.ModuleName;
							if (!performanceTestInProgress)
								Console("<=" + response.ToString());
							lock (receivedResponses)
							{
								receivedResponses.Add(response);
							}
							continue;
						}

						if (!Command.TryParse(packet.ConnectionManager, s, out command))
							continue;
						//command.SourceModule = packet.ConnectionManager.ModuleName;
						foreach (AutoResponder responder in autoResponders)
						{
							if (responder.Match(command))
							{
								response = responder.GetResponse(command);
								packet.ConnectionManager.Send(response);
								handled = true;
								Console("AutoResponder (" + command.SourceModule + "): [" + command.StringToSend + "] => [" + response.StringToSend + "]");
								break;
							}
						}

						foreach (Prompter prompter in prompters)
						{
							if (prompter.Match(command))
							{
								prompter.Prompt(packet.ConnectionManager, command);
								//Console("Prompter:" + command.StringToSend + " => " + response.StringToSend);
								handled = true;
								break;
							}
						}
						if (handled)
							continue;
						
					}
				}
			}

			foreach (ConnectionManager cnnMan in modules.Values) 
				cnnMan.Stop();
		}

		private void RemoveModule(string moduleName)
		{
			if ((moduleName == null) || !modules.ContainsKey(moduleName))
			{
				btnModuleStartStop.Enabled = false;
				btnModuleRemove.Enabled = false;
				return;
			}
			ConnectionManager cnnMan = modules[moduleName];
			int i =0;
			this.Enabled = false;
			modules.Remove(moduleName);
			if (cnnMan.IsRunning)
				cnnMan.Stop();
			lstModules.Items.Remove(moduleName);
			cmbARModule.Items.Remove(moduleName);
			cmbPrmptModule.Items.Remove(moduleName);
			cmbPerformanceTestModule.Items.Remove(moduleName);
			cmbQCModule.Items.Remove(moduleName);
			lstModules.SelectedIndex = -1;

			lock (autoResponders)
			{
				for (i = 0; i < autoResponders.Count; ++i)
				{
					if (autoResponders[i].Module == moduleName)
						RemoveAutoResponder(autoResponders[i--]);
				}
			}

			lock (prompters)
			{
				for (i = 0; i < prompters.Count; ++i)
				{
					if (prompters[i].Module == moduleName)
						RemovePrompter(prompters[i--]);
				}
			}

			lock (quickCommands)
			{
				for (i = 0; i < quickCommands.Count; ++i)
				{
					if (quickCommands[i].Module == moduleName)
						RemoveQuickCommand(quickCommands[i--]);
				}
			}

			this.Enabled = true;
		}

		private void RemoveAutoResponder(AutoResponder autoResponder)
		{
			if ((autoResponder == null) || !autoResponders.Contains(autoResponder))
			{
				btnARDelete.Enabled = false;
				return;
			}
			autoResponders.Remove(autoResponder);
			if (lstAutoResponders.Items.Contains(autoResponder))
				lstAutoResponders.Items.Remove(autoResponder);
			lstAutoResponders.SelectedIndex = -1;

		}

		private void RemovePrompter(Prompter prompter)
		{
			if ((prompter == null) || !prompters.Contains(prompter))
			{
				btnPrmptDelete.Enabled = false;
				return;
			}
			prompters.Remove(prompter);
			if (lstPrompter.Items.Contains(prompter))
				lstPrompter.Items.Remove(prompter);
			lstPrompter.SelectedIndex = -1;

		}

		private void RemoveQuickCommand(QuickCommand quickCommand)
		{
			if ((quickCommand == null) || !quickCommands.Contains(quickCommand))
			{
				btnQCDelete.Enabled = false;
				return;
			}
			quickCommands.Remove(quickCommand);
			if (lstQuickCommands.Items.Contains(quickCommand))
				lstQuickCommands.Items.Remove(quickCommand);
			lstQuickCommands.SelectedIndex = -1;

		}

		private void SaveData()
		{
			int i;
			Config.Default.Modules.Clear();
			Config.Default.AutoResponders.Clear();
			Config.Default.Prompters.Clear();

			foreach(ConnectionManager cnnMan in modules.Values)
				Config.Default.Modules.Add(new KeyValuePair<string, int>(cnnMan.ModuleName, cnnMan.PortIn));

			for (i = 0; i < autoResponders.Count; ++i)
				Config.Default.AutoResponders.Add(autoResponders[i]);
			for (i = 0; i < prompters.Count; ++i)
				Config.Default.Prompters.Add(prompters[i]);
			for (i = 0; i < quickCommands.Count; ++i)
				Config.Default.QuickCommands.Add(quickCommands[i]);
			Config.Default.Save();
		}

		private void SelectModule(string moduleName)
		{
			if (this.InvokeRequired)
				return;
			if ((moduleName==null)||!modules.ContainsKey(moduleName))
			{
				selectedConnectionManager = null;
				btnModuleStartStop.Enabled = false;
				btnModuleRemove.Enabled = false;
				return;
			}
			selectedConnectionManager = modules[moduleName];
			btnModuleStartStop.Text = selectedConnectionManager.IsRunning ? "Stop" : "Start";
			btnModuleRemove.Text = selectedConnectionManager.IsRunning ? "Stop and Remove" : "Remove";
			txtModuleName.Text = selectedConnectionManager.ModuleName;
			txtModulePort.Text = selectedConnectionManager.PortIn.ToString();
			btnModuleStartStop.Enabled = true;
			btnModuleRemove.Enabled = true;
		}

		private void SelectPrompter(Prompter prompter)
		{
			if (this.InvokeRequired)
				return;
			if ((prompter == null) || !prompters.Contains(prompter))
			{
				selectedPrompter = null;
				btnPrmptDelete.Enabled = false;
				return;
			}
			selectedPrompter = prompter;
			btnPrmptDelete.Enabled = true;
		}

		private void SelectQuickCommand(QuickCommand quickCommand)
		{
			if (this.InvokeRequired)
				return;
			if ((quickCommand == null) || !quickCommands.Contains(quickCommand))
			{
				selectedQuickCommand = null;
				btnQCDelete.Enabled = false;
				return;
			}
			selectedQuickCommand = quickCommand;
			btnQCDelete.Enabled = true;
		}

		private void SelectResponder(AutoResponder autoResponder)
		{
			if (this.InvokeRequired)
				return;
			if ((autoResponder == null) || !autoResponders.Contains(autoResponder))
			{
				selectedAutoResponder = null;
				btnARDelete.Enabled = false;
				return;
			}
			selectedAutoResponder = autoResponder;
			btnARDelete.Enabled = true;
		}

		private void SendQuickCommand(QuickCommand quickCommand)
		{
			if (!modules.ContainsKey(quickCommand.Module))
				return;
			ConnectionManager cnnMan = modules[quickCommand.Module];
			Command cmd = quickCommand.GetCommand(cnnMan);
			if (cmd == null)
				return;
			cnnMan.Send(cmd);
			Console("Sent: " + cmd.ToString());
		}

		internal DialogResult ShowPromptDialog(FrmPromptDialog promptDialog)
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.Disposing || this.IsDisposed)
					return DialogResult.None;
				return (DialogResult)this.Invoke(dlgShowPromptDialog, promptDialog);
			}
			return promptDialog.ShowDialog();
		}

		private void StartPerformanceTest(string moduleName, string commandName, string parameters, int executions, int commandTimeout)
		{
			if (!modules.ContainsKey(moduleName))
				return;
			if ((executions < 0)||(executions > 1000000))
				return;
			if (commandTimeout < 0) commandTimeout = 0;
			if (commandTimeout > 120000) commandTimeout = 120000;
			ConnectionManager cnnMan = modules[moduleName];
			PerformanceTestWork work = new PerformanceTestWork(cnnMan, commandName, parameters, executions, commandTimeout);
			bgwPerformanceTestWorker.RunWorkerAsync(work);
			btnStartStopPerformanceTest.Text = "Stop Performance Test";
			btnStartStopPerformanceTest.Enabled = true;
		}

		private void ValidateAddAutoResponderButton()
		{
			btnARAdd.Enabled = ((cmbARModule.Text == "[ANY]") || modules.ContainsKey(cmbARModule.Text)) && (txtARCommandName.Text.Trim().Length > 2);
		}

		private void ValidateAddModuleButton()
		{
			int port;
			btnAddModule.Enabled = (Int32.TryParse(txtModulePort.Text, out port) && (port > 1024) && (port < 65535))
			&& rxModuleNameValidator.IsMatch(txtModuleName.Text) && !modules.ContainsKey(txtModuleName.Text);
		}

		private void ValidateAddPrompterButton()
		{
			btnPrmptAdd.Enabled = ((cmbPrmptModule.Text == "[ANY]") || modules.ContainsKey(cmbPrmptModule.Text)) && (txtPrmptCommandName.Text.Trim().Length > 2);
		}

		private void ValidateAddQuickCommandButton()
		{
			btnQCAdd.Enabled = modules.ContainsKey(cmbQCModule.Text) && (txtQCCommandName.Text.Trim().Length > 2);
		}

		private void ValidateStartPerformanceTest()
		{
			btnStartStopPerformanceTest.Enabled = modules.ContainsKey(cmbPerformanceTestModule.Text) && (txtPerformanceTestCommandName.Text.Trim().Length > 2);
		}

		#endregion

		#region EventHandlers

		void prompter_ResponseSent(Command command, Response response)
		{
			if ((command == null) || (response == null))
				return;
			Console("Prompter (" + command.SourceModule + "): [" + command.StringToSend + "] => [" + response.StringToSend + "]");
		}

		#endregion

		#region Connection Manager Event Handlers

		private void cnnMan_Stopped(ConnectionManager connectionManager)
		{
			Console("Stopped " + connectionManager.ModuleName);
			if ((selectedConnectionManager != null) && (selectedConnectionManager.ModuleName == connectionManager.ModuleName))
			{
				SelectModule(selectedConnectionManager.ModuleName);
			}
		}

		private void cnnMan_Started(ConnectionManager connectionManager)
		{
			Console("Started " + connectionManager.ModuleName);
			if ((selectedConnectionManager != null) && (selectedConnectionManager.ModuleName == connectionManager.ModuleName))
			{
				SelectModule(selectedConnectionManager.ModuleName);
			}
		}

		private void cnnMan_ClientDisconnected(EndPoint ep)
		{
			//Console("Client " + ep.ToString() + " disconnected from " + connectionManager.ModuleName);
			Console("Client " + ep.ToString() + " disconnected");
		}

		private void cnnMan_ClientConnected(Socket s)
		{
			//Console("Client connected to " + connectionManager.ModuleName + " from " + ep.ToString());
			Console("Client " + s.RemoteEndPoint.ToString() + " connected");
		}

		private void cnnMan_DataReceived(ConnectionManager cnnMan, TcpPacket packet)
		{
			dataReceived.Produce(new ReceivedPacket(cnnMan, packet));
		}

		#endregion

		#region Windows Forms Event Handlers

		#region Module Controls

		private void txtModulePort_TextChanged(object sender, EventArgs e)
		{
			ValidateAddModuleButton();
		}

		private void txtModuleName_TextChanged(object sender, EventArgs e)
		{
			txtModuleName.Text = txtModuleName.Text.ToUpper();
			ValidateAddModuleButton();
		}

		private void btnAddModule_Click(object sender, EventArgs e)
		{
			string name;
			int port;

			if(!Int32.TryParse(txtModulePort.Text, out port) || (port <= 1024) || (port >= 65535))
				return;
			name = txtModuleName.Text.Trim();
			if(!rxModuleNameValidator.IsMatch(name) || modules.ContainsKey(name))
				return;
			
			AddModule(name, port);
		}

		private void lstModules_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((lstModules.SelectedIndex == -1) || (lstModules.SelectedItem == null))
			{
				SelectModule(null);
				return;
			}
			SelectModule((string)lstModules.SelectedItem);
		}

		private void btnModuleStartStop_Click(object sender, EventArgs e)
		{
			if (selectedConnectionManager.IsRunning)
				selectedConnectionManager.Stop();
			else
				selectedConnectionManager.Start();
		}

		private void txtModulePort_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (txtModuleName.Text.Length < 3)
					txtModuleName.Focus();
				else
					btnAddModule.PerformClick();
			}
		}

		private void txtModuleName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (txtModulePort.Text.Length < 3)
					txtModulePort.Focus();
				else
				btnAddModule.PerformClick();
			}
		}

		private void btnModuleRemove_Click(object sender, EventArgs e)
		{
			if (selectedConnectionManager.IsRunning)
				selectedConnectionManager.Stop();
			RemoveModule(selectedConnectionManager.ModuleName);
		}

		#endregion

		#region Autoresponder Controls

		private void btnARAdd_Click(object sender, EventArgs e)
		{
			string moduleName = cmbARModule.Text.ToUpper();
			string commandName = txtARCommandName.Text.Trim();
			string parameters = txtARParameters.Text.Trim();
			int failureRate = (int)nudARFailureRate.Value;
			AddAutoResponder(moduleName, commandName, parameters, failureRate);
		}

		private void btnARDelete_Click(object sender, EventArgs e)
		{
			RemoveAutoResponder(selectedAutoResponder);
		}

		private void lstAutoResponders_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((lstAutoResponders.SelectedIndex == -1) || (lstAutoResponders.SelectedItem == null))
			{
				SelectResponder(null);
				return;
			}
			SelectResponder((AutoResponder)lstAutoResponders.SelectedItem);
		}

		private void cmbARModule_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (txtARCommandName.Text.Length < 3)
					txtARCommandName.Focus();
				else if (txtARParameters.Text.Length < 3)
					txtARParameters.Focus();	
				else
					btnARAdd.PerformClick();
			}
		}

		private void txtARCommandName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (cmbARModule.Text.Length < 3)
					cmbARModule.Focus();
				else if (txtARParameters.Text.Length < 3)
					txtARParameters.Focus();
				else
					btnARAdd.PerformClick();
			}
		}

		private void txtARParameters_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (cmbARModule.Text.Length < 3)
					cmbARModule.Focus();
				else if (txtARCommandName.Text.Length < 3)
					txtARCommandName.Focus();
				else
					btnARAdd.PerformClick();
			}
		}

		private void cmbARModule_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValidateAddAutoResponderButton();
		}

		private void cmbARModule_TextChanged(object sender, EventArgs e)
		{
			ValidateAddAutoResponderButton();
		}

		private void txtARCommandName_TextChanged(object sender, EventArgs e)
		{
			ValidateAddAutoResponderButton();
		}

		private void txtARParameters_TextChanged(object sender, EventArgs e)
		{
			ValidateAddAutoResponderButton();
		}

		#endregion

		#region Prompter controls

		private void btnPrmptAdd_Click(object sender, EventArgs e)
		{
			string moduleName = cmbPrmptModule.Text.ToUpper();
			string commandName = txtPrmptCommandName.Text.Trim();
			//string parameters = txtPrmptParameters.Text.Trim();
			//bool success = true;
			AddPrompter(moduleName, commandName);
		}

		private void btnPrmptDelete_Click(object sender, EventArgs e)
		{
			RemovePrompter(selectedPrompter);
		}

		private void cmbPrmptModule_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (txtPrmptCommandName.Text.Length < 3)
					txtPrmptCommandName.Focus();
				//else if (txtPrmptParameters.Text.Length < 3)
				//	txtPrmptParameters.Focus();
				else
					btnPrmptAdd.PerformClick();
			}
		}

		private void txtPrmptCommandName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (cmbPrmptModule.Text.Length < 3)
					cmbPrmptModule.Focus();
				//else if (txtPrmptParameters.Text.Length < 3)
				//	txtPrmptParameters.Focus();
				else
					btnPrmptAdd.PerformClick();
			}
		}

		private void txtPrmptParameters_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (cmbPrmptModule.Text.Length < 3)
					cmbPrmptModule.Focus();
				//else if (txtPrmptCommandName.Text.Length < 3)
				//	txtPrmptCommandName.Focus();
				else
					btnPrmptAdd.PerformClick();
			}
		}

		private void cmbPrmptModule_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValidateAddPrompterButton();
		}

		private void cmbPrmptModule_TextChanged(object sender, EventArgs e)
		{
			ValidateAddPrompterButton();
		}

		private void txtPrmptCommandName_TextChanged(object sender, EventArgs e)
		{
			ValidateAddPrompterButton();
		}

		private void txtPrmptParameters_TextChanged(object sender, EventArgs e)
		{
			ValidateAddPrompterButton();
		}

		private void lstPrompter_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((lstPrompter.SelectedIndex == -1) || (lstPrompter.SelectedItem == null))
			{
				SelectPrompter(null);
				return;
			}
			SelectPrompter((Prompter)lstPrompter.SelectedItem);
		}

		#endregion

		#region PerformanceTest controls

		private void cmbPerformanceTestModule_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValidateStartPerformanceTest();
		}

		private void cmbPerformanceTestModule_TextChanged(object sender, EventArgs e)
		{
			ValidateStartPerformanceTest();
		}

		private void txtPerformanceTestCommandName_TextChanged(object sender, EventArgs e)
		{
			ValidateStartPerformanceTest();
		}

		private void cmbPerformanceTestModule_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (txtPerformanceTestCommandName.Text.Length < 3)
					txtPerformanceTestCommandName.Focus();
				else if (txtPerformanceTestParameters.Text.Length < 1)
					txtPerformanceTestParameters.Focus();
				else
					btnARAdd.PerformClick();
			}
		}

		private void txtPerformanceTestCommandName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (cmbPerformanceTestModule.Text.Length < 3)
					cmbPerformanceTestModule.Focus();
				else if (txtPerformanceTestParameters.Text.Length < 1)
					txtPerformanceTestParameters.Focus();
				else
					btnARAdd.PerformClick();
			}
		}

		private void txtPerformanceTestCmdParams_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (cmbPerformanceTestModule.Text.Length < 3)
					cmbPerformanceTestModule.Focus();
				else if (txtPerformanceTestCommandName.Text.Length < 3)
					txtPerformanceTestCommandName.Focus();
				else
					btnARAdd.PerformClick();
			}
		}

		private void txtPerformanceTestRspParams_KeyUp(object sender, KeyEventArgs e)
		{
			if (cmbPerformanceTestModule.Text.Length < 3)
				cmbPerformanceTestModule.Focus();
			else if (txtPerformanceTestCommandName.Text.Length < 3)
				txtPerformanceTestCommandName.Focus();
			else
				btnARAdd.PerformClick();
		}

		private void nudPerformanceTestExecutions_ValueChanged(object sender, EventArgs e)
		{
			if (nudPerformanceTestExecutions.Value < 10)
				nudPerformanceTestExecutions.Increment = 1;
			else if (nudPerformanceTestExecutions.Value < 100)
				nudPerformanceTestExecutions.Increment = 10;
			else if (nudPerformanceTestExecutions.Value < 1000)
				nudPerformanceTestExecutions.Increment = 100;
			else if (nudPerformanceTestExecutions.Value < 10000)
				nudPerformanceTestExecutions.Increment = 1000;
			else if (nudPerformanceTestExecutions.Value < 100000)
				nudPerformanceTestExecutions.Increment = 10000;
			else
				nudPerformanceTestExecutions.Increment = 100000;
		}

		private void nudPerformanceTestTimeout_ValueChanged(object sender, EventArgs e)
		{
			if (nudPerformanceTestExecutions.Value < 1000)
				nudPerformanceTestExecutions.Increment = 100;
			else if (nudPerformanceTestExecutions.Value < 10000)
				nudPerformanceTestExecutions.Increment = 1000;
			else
				nudPerformanceTestExecutions.Increment = 10000;
		}

		private void btnStartStopPerformanceTest_Click(object sender, EventArgs e)
		{
			btnStartStopPerformanceTest.Enabled = false;
			if (bgwPerformanceTestWorker.IsBusy)
			{
				bgwPerformanceTestWorker.CancelAsync();
				return;
			}
			string moduleName = cmbPerformanceTestModule.Text.ToUpper();
			string commandName = txtPerformanceTestCommandName.Text.Trim();
			string parameters = txtPerformanceTestParameters.Text.Trim();
			int executions = (int)nudPerformanceTestExecutions.Value;
			int commandTimeout = (int)nudPerformanceTestTimeout.Value;
			StartPerformanceTest(moduleName, commandName, parameters, executions, commandTimeout);
		}

		#endregion

		#region QuickCommand controls

		private void cmbQCModule_TextChanged(object sender, EventArgs e)
		{
			ValidateAddQuickCommandButton();
		}

		private void cmbQCModule_SelectedIndexChanged(object sender, EventArgs e)
		{
			ValidateAddQuickCommandButton();
		}

		private void cmbQCModule_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (txtQCCommandName.Text.Length < 3)
					txtQCCommandName.Focus();
				else if (txtQCParameters.Text.Length < 3)
					txtQCParameters.Focus();
				else
					btnQCAdd.PerformClick();
			}
		}

		private void txtQCCommandName_TextChanged(object sender, EventArgs e)
		{
			ValidateAddQuickCommandButton();
		}

		private void txtQCCommandName_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (cmbQCModule.Text.Length < 3)
					cmbQCModule.Focus();
				else if (txtQCParameters.Text.Length < 3)
					txtQCParameters.Focus();
				else
					btnQCAdd.PerformClick();
			}
		}

		private void txtQCParameters_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				if (cmbQCModule.Text.Length < 3)
					cmbQCModule.Focus();
				else if (txtQCCommandName.Text.Length < 3)
					txtQCCommandName.Focus();
				else
					btnQCAdd.PerformClick();
			}
		}

		private void txtQCParameters_TextChanged(object sender, EventArgs e)
		{
			ValidateAddQuickCommandButton();
		}

		private void btnQCAdd_Click(object sender, EventArgs e)
		{
			string moduleName = cmbQCModule.Text.ToUpper();
			string commandName = txtQCCommandName.Text.Trim();
			string parameters = txtQCParameters.Text.Trim();
			int id = (int)idbQCCommandId.Value;
			AddQuickCommand(moduleName, commandName, parameters, id);
		}

		private void btnQCDelete_Click(object sender, EventArgs e)
		{
			RemoveQuickCommand(selectedQuickCommand);
		}

		private void lstQuickCommands_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((lstQuickCommands.SelectedIndex == -1) || (lstQuickCommands.SelectedItem == null))
			{
				SelectQuickCommand(null);
				return;
			}
			SelectQuickCommand((QuickCommand)lstQuickCommands.SelectedItem);
		}

		private void lstQuickCommands_DoubleClick(object sender, EventArgs e)
		{
			lstQuickCommands.Enabled = false;
			SendQuickCommand(selectedQuickCommand);
			lstQuickCommands.Enabled = true;
		}

		#endregion

		private void FrmTester_FormClosing(object sender, FormClosingEventArgs e)
		{
			running = false;
			mainThread.Interrupt();
			this.gbAddModule.Enabled = false;
			this.gbAutoResponder.Enabled = false;
			this.gbModules.Enabled = false;
			this.gbPrompter.Enabled = false;

			Prompter.AbortAll();
			while (mainThread.IsAlive)
				Application.DoEvents();
			SaveData();
		}

		private void FrmTester_Load(object sender, EventArgs e)
		{
			LoadData();
			cmbARModule.Items.Add("[ANY]");
			cmbPrmptModule.Items.Add("[ANY]");
		}

		#endregion

		#region PerformanceTestWorker

		private void bgwPerformanceTestWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			PerformanceTestWork work = e.Argument as PerformanceTestWork;
			if (work == null)
				return;

			int step;
			Stopwatch stepStopwatch = new Stopwatch();
			Command command;
			Response candidate;
			bool found;

			performanceTestInProgress = true;
			stopwatch.Reset();
			stopwatch.Start();
			for (step = 0; running && !bgwPerformanceTestWorker.CancellationPending && (step < work.Executions); ++step)
			{
				work.Progress = 100 * step / work.Executions;
				bgwPerformanceTestWorker.ReportProgress(work.Progress, sender);
				
				command = new Command(work.ConnectionManager, work.CommandName, work.Parameters, step);

				// 1. Send the command. If command cannot be sent, return false
				lock (receivedResponses)
				{
					receivedResponses.Clear();
				}
				stepStopwatch.Reset();
				stepStopwatch.Start();
				if (!work.ConnectionManager.Send(command))
				{
					stepStopwatch.Stop();
					continue;
				}

				// 2. Wait for response to arrival
				while (running && (stepStopwatch.ElapsedMilliseconds < work.CommandTimeout))
				{
					found = false;
					lock (receivedResponses)
					{
						for (int i = 0; i < receivedResponses.Count; ++i)
						{
							candidate = receivedResponses[i];
							if (command.IsMatch(candidate))
							{
								receivedResponses.Remove(candidate);
								if(candidate.Success) ++work.Succeeded;
								found = true;
								break;
							}
						}
					}
					if (found)
						break;
					Thread.Sleep(1);
				}
				stepStopwatch.Stop();
				work.Elapsed = stopwatch.Elapsed;
			}
			stopwatch.Stop();
			e.Result = work;
		}

		private void bgwPerformanceTestWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			pbPerformanceTestProgress.Value = e.ProgressPercentage;
		}

		private void bgwPerformanceTestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			btnStartStopPerformanceTest.Text = "Start Performance Test";
			btnStartStopPerformanceTest.Enabled = true;
			performanceTestInProgress = false;
			PerformanceTestWork work = e.Result as PerformanceTestWork;
			if (work == null)
				return;
			txtPerformanceTestElapsed.Text = work.Elapsed.ToString();
			txtPerformanceTestSucceeded.Text = work.Succeeded.ToString();
		}

		#endregion

		private void tsiDeserialize_Click(object sender, EventArgs e)
		{
			if (this.contextMenu.SourceControl == null)
				return;
			this.contextMenu.SourceControl.Text = DeserializeDouble(this.contextMenu.SourceControl.Text);
		}

		private void tsiSerialize_Click(object sender, EventArgs e)
		{
			if (this.contextMenu.SourceControl == null)
				return;
			this.contextMenu.SourceControl.Text = SerializeDouble(this.contextMenu.SourceControl.Text);
		}

		private string DeserializeDouble(string p)
		{
			List<byte> dataList = new List<byte>();
			StringBuilder sb = new StringBuilder();
			byte[] data;
			byte b;

			if ((p.Length % 2) != 0) return p;
			for (int i = 0; i < p.Length - 1; i += 2)
			{
				if (!byte.TryParse(p.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, null, out b))
					return p;
				dataList.Add(b);
			}
			data = dataList.ToArray();
			if ((data.Length % 8) == 0)
				return DeserializeString(p);
			for (int i = 0; i < data.Length - 7; i += 8)
			{
				if(i > 0)
					sb.Append(' ');
				sb.Append(BitConverter.ToDouble(data, i));
			}
			return sb.ToString();
		}

		private string DeserializeString(string p)
		{
			List<byte> dataList = new List<byte>();
			StringBuilder sb = new StringBuilder();
			byte[] data;
			byte b;

			if ((p.Length % 2) != 0) return p;
			for (int i = 0; i < p.Length - 1; i += 2)
			{
				if (!byte.TryParse(p.Substring(i, 2), System.Globalization.NumberStyles.HexNumber, null, out b))
					return p;
				dataList.Add(b);
			}
			data = dataList.ToArray();

			return ASCIIEncoding.ASCII.GetString(data);
		}

		private string SerializeDouble(string p)
		{
			string[] parts = Regex.Split(p, @"\s+");
			List<byte[]> data = new List<byte[]>();
			StringBuilder sb = new StringBuilder();
			double d;
			foreach (string part in parts)
			{
				if (!Double.TryParse(part, out d))
					return SerializeString(p);
				data.Add(BitConverter.GetBytes(d));
			}

			for (int i = 0; i < data.Count; ++i)
			{
				if(data[i] == null)
					return SerializeString(p);
				for (int j = 0; j < data[i].Length; ++j)
					sb.Append(data[i][j].ToString("X2"));
			}
			return sb.ToString();
		}

		private string SerializeString(string p)
		{
			byte[] data = ASCIIEncoding.ASCII.GetBytes(p);
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < data.Length -1; ++i)
			{
				sb.Append(data[i++].ToString("X2"));
				sb.Append(data[i].ToString("X2"));
			}
			return sb.ToString();
		}

		
	}
}