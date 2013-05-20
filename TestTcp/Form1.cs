//#define BENCHMARK
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;

namespace TestTcp
{
	public partial class Form1 : Form
	{
		SocketTcpServer server;
		SocketTcpClient client;
		EventHandler updateServerConsoleEH;
		EventHandler updateClientConsoleEH;
		string textForServerConsole;
		string textForClientConsole;
		Stopwatch stopwatch = new Stopwatch();

		public Form1()
		{
			InitializeComponent();
			server = new SocketTcpServer();
			client = new SocketTcpClient();
			client.ConnectionTimeOut = 1000;
			updateServerConsoleEH = new EventHandler(updateServerConsole);
			updateClientConsoleEH = new EventHandler(updateClientConsole);

			server.ClientConnected += new TcpClientConnectedEventHandler(server_ClientConnected);
			server.ClientDisconnected += new TcpClientDisconnectedEventHandler(server_ClientDisconnected);
			server.DataReceived += new TcpDataReceivedEventHandler(server_DataReceived);

			client.Connected += new TcpClientConnectedEventHandler(client_Connected);
			client.DataReceived += new TcpDataReceivedEventHandler(client_DataReceived);
			client.Disconnected += new TcpClientDisconnectedEventHandler(client_Disconnected);
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void btnServerStart_Click(object sender, EventArgs e)
		{
			if (!server.Started)
			{
				try
				{
					int port;

					if (!Int32.TryParse(txtServerPort.Text, out port))
					{
						MessageBox.Show("Invalid port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					server.Port = port;
					server.Start();
				}
				catch
				{
					return;
				}
				txtServerPort.AutoCompleteCustomSource.Add(txtServerPort.Text);
				btnServerStart.Text = "Stop";
				txtServerPort.Enabled = false;
				txtServerInput.Enabled = true;
				txtServerConsole.Enabled = true;
				btnServerSend.Enabled = true;
			}
			else
			{
				try
				{
					server.Stop();
				}
				catch
				{
					return;
				}
				btnServerStart.Text = "Start";
				txtServerPort.Enabled = true;
				txtServerInput.Enabled = false;
				txtServerConsole.Enabled = false;
				btnServerSend.Enabled = false;
			}

		}

		private void server_ClientDisconnected(EndPoint ep)
		{
			textForServerConsole = "Disconnected client " + ep;
			this.Invoke(updateServerConsoleEH);
		}

		private void server_ClientConnected(Socket s)
		{
			textForServerConsole = "Connected client " + s.RemoteEndPoint;
			this.Invoke(updateServerConsoleEH);
		}

		private void server_DataReceived(TcpPacket p)
		{
			string text;
#if BENCHMARK
			stopwatch.Stop();
			textForServerConsole = stopwatch.Elapsed.ToString();
			this.Invoke(updateServerConsoleEH);
#endif
			if (!p.IsAnsi)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(p.Data.Length + 10);
				for (int i = 0; i < p.Data.Length; ++i)
				{
					sb.Append(' ');
					sb.Append(p.Data[i].ToString("X2"));
				}
				text = sb.ToString();
				textForServerConsole = "[" + p.SenderIP.ToString() + "] (" + p.Data.Length + "bytes): " + text;
				this.Invoke(updateServerConsoleEH);
			}
			else
			{
				textForServerConsole = "[" + p.SenderIP.ToString() + "] (" + p.Data.Length + "bytes): ";
				this.Invoke(updateServerConsoleEH);
				for (int i = 0; i < p.DataStrings.Length; ++i)
				{
					text = p.DataStrings[i];
					textForServerConsole = "\t[" + p.SenderIP.ToString() + "] (Part " + i + "): " + text;
					this.Invoke(updateServerConsoleEH);
				}
			}

			/*
			text = p.DataString;
			if ((text.Length < 1) || ((p.Data.Length != text.Length) && (p.Data.Length != text.Length + 1)))
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(p.Data.Length + 10);
				//sb.Append(p.Data.Length.ToString());
				//sb.Append("bytes");
				for (int i = 0; i < p.Data.Length; ++i)
				{
					sb.Append(' ');
					sb.Append(p.Data[i].ToString("X2"));
				}
				text = sb.ToString();
			}
			textForServerConsole = "[" + p.SenderIP.ToString() + "] ("+ p.Data.Length +"bytes): " + text;
			this.Invoke(updateServerConsoleEH);
			*/
		}

		private void client_Disconnected(EndPoint ep)
		{
			if (this.InvokeRequired)
			{
				if (!this.IsHandleCreated || this.IsDisposed || this.Disposing)
					return;
				this.BeginInvoke(new TcpClientDisconnectedEventHandler(client_Disconnected), ep);
				return;
			}
			textForClientConsole = "[Disconnected]";
			this.Invoke(updateClientConsoleEH);
			btnClientConnect.Text = "Connect";
			txtClientPort.Enabled = true;
			txtClientInput.Enabled = false;
			btnClientSend.Enabled = false;
			txtClientIp.Enabled = true;
			txtClientConsole.Enabled = false;
		}

		private void client_DataReceived(TcpPacket p)
		{
			string text;
#if BENCHMARK
			stopwatch.Stop();
			textForClientConsole = stopwatch.Elapsed.ToString();
			this.Invoke(updateClientConsoleEH);
#endif
			if (!p.IsAnsi)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(p.Data.Length + 10);
				for (int i = 0; i < p.Data.Length; ++i)
				{
					sb.Append(' ');
					sb.Append(p.Data[i].ToString("X2"));
				}
				text = sb.ToString();
				textForClientConsole = "[" + p.SenderIP.ToString() + "] (" + p.Data.Length + "bytes): " + text;
				this.Invoke(updateClientConsoleEH);
			}
			else
			{
				textForClientConsole = "[" + p.SenderIP.ToString() + "] (" + p.Data.Length + "bytes): ";
				this.Invoke(updateClientConsoleEH);
				for (int i = 0; i < p.DataStrings.Length; ++i)
				{
					text = p.DataStrings[i];
					textForClientConsole = "\t[" + p.SenderIP.ToString() + "] (Part " + i + "): " + text;
					this.Invoke(updateClientConsoleEH);
				}
			}

			/*
			text = p.DataString;
			if ((text.Length < 1) || ((p.Data.Length != text.Length) && (p.Data.Length != text.Length +1)))
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(p.Data.Length + 10);
				//sb.Append(p.Data.Length.ToString());
				//sb.Append("bytes");
				for (int i = 0; i < p.Data.Length; ++i)
				{
					sb.Append(' ');
					sb.Append(p.Data[i].ToString("X2"));
				}
				text = sb.ToString();
			}
			textForClientConsole = "[" + p.SenderIP.ToString() + "] (" + p.Data.Length + "bytes): " + text;
			this.Invoke(updateClientConsoleEH);
			*/
		}

		private void client_Connected(Socket s)
		{
			textForClientConsole = "Connected";
			this.Invoke(updateClientConsoleEH);
		}

		private void updateServerConsole(object sender, EventArgs e)
		{
			txtServerConsole.AppendText(textForServerConsole + "\r\n");
			textForServerConsole = "";
		}

		private void updateClientConsole(object sender, EventArgs e)
		{
			txtClientConsole.AppendText(textForClientConsole + "\r\n");
			textForClientConsole = "";
		}

		private void btnClientConnect_Click(object sender, EventArgs e)
		{
			if (!client.IsOpen)
			{
				try
				{
					int port;
					IPAddress address;

					if (!Int32.TryParse(txtClientPort.Text, out port))
					{
						MessageBox.Show("Invalid port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					if (!IPAddress.TryParse(txtClientIp.Text, out address))
					{
						MessageBox.Show("Invalid server ip address", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
					client.Port = port;
					client.ServerAddress = address;
					client.Connect();
				}
				catch
				{
					return;
				}
				txtClientIp.AutoCompleteCustomSource.Add(txtClientIp.Text);
				txtClientPort.AutoCompleteCustomSource.Add(txtClientPort.Text);
				btnClientConnect.Text = "Disconnect";
				txtClientPort.Enabled = false;
				txtClientInput.Enabled = true;
				btnClientSend.Enabled = true;
				txtClientIp.Enabled = false;
				txtClientConsole.Enabled = true;
			}
			else
			{
				try
				{
					client.Disconnect();
				}
				catch
				{
					return;
				}
				btnClientConnect.Text = "Connect";
				txtClientPort.Enabled = true;
				txtClientInput.Enabled = false;
				btnClientSend.Enabled = false;
				txtClientIp.Enabled = true;
				txtClientConsole.Enabled = false;
			}
		}

		private void btnServerSend_Click(object sender, EventArgs e)
		{
			if (txtServerInput.Text.Length > 0)
			{
#if BENCHMARK
				stopwatch.Reset();
				stopwatch.Start();
#endif
				server.SendToAll(txtServerInput.Text);
				textForServerConsole = "[Sent: " + txtServerInput.Text.Substring(0, Math.Min(txtServerInput.Text.Length, 50)) + "]";
				this.Invoke(updateServerConsoleEH);
				if (!txtServerInput.AutoCompleteCustomSource.Contains(txtServerInput.Text))
					txtServerInput.AutoCompleteCustomSource.Add(txtServerInput.Text);
				txtServerInput.Text = "";
			}
			txtServerInput.Focus();
		}

		private void btnClientSend_Click(object sender, EventArgs e)
		{
			if (txtClientInput.Text.Length > 0)
			{
#if BENCHMARK
				stopwatch.Reset();
				stopwatch.Start();
#endif
				client.Send(txtClientInput.Text);
				textForClientConsole = "[Sent: " + txtClientInput.Text.Substring(0, Math.Min(txtClientInput.Text.Length, 50)) + "]";
				this.Invoke(updateClientConsoleEH);
				if (!txtClientInput.AutoCompleteCustomSource.Contains(txtClientInput.Text))
					txtClientInput.AutoCompleteCustomSource.Add(txtClientInput.Text);
				txtClientInput.Text = "";

			}
			txtClientInput.Focus();
		}

		private void txtInput_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				TextBox t = (TextBox)sender;
				if (t == txtClientInput)
					btnClientSend_Click(sender, e);
				if (t == txtServerInput)
					btnServerSend_Click(sender, e);

			}
		}
	}
}