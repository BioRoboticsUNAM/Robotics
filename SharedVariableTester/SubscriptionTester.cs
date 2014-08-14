using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Robotics.API;
using Robotics.API.PrimitiveSharedVariables;
using Robotics.API.MiscSharedVariables;

namespace SharedVariableTester
{
	class SubscriptionTester : ITester
	{
		CommandManager cmdMan;
		ConnectionManager cnnMan;
		IntSharedVariable svInteger;
		VectorSharedVariable svVector;
		MatrixSharedVariable svMatrix;
		ByteArraySharedVariable svBytes;
		DoubleArraySharedVariable svDoubles;
		StringSharedVariable svString;
		//StringSharedVariable svConnected;
		StringSharedVariable svRecognized;
		RecognizedSpeechSharedVariable svSpeech;
		AutoResetEvent e = new AutoResetEvent(false);

		public SubscriptionTester()
		{
			cmdMan = new CommandManager();
			cmdMan.Started += new CommandManagerStatusChangedEventHandler(cmdMan_Started);
			cmdMan.Stopped += new CommandManagerStatusChangedEventHandler(cmdMan_Stopped);
			cmdMan.SharedVariablesLoaded += new SharedVariablesLoadedEventHandler(cmdMan_SharedVariablesLoaded);
			
			cnnMan = new ConnectionManager(2020, cmdMan);
			cnnMan.DataReceived += new ConnectionManagerDataReceivedEH(cnnMan_DataReceived);
			cnnMan.ClientConnected += new System.Net.Sockets.TcpClientConnectedEventHandler(cnnMan_ClientConnected);
			cnnMan.Started += new ConnectionManagerStatusChangedEventHandler(cnnMan_Started);
			cnnMan.Stopped += new ConnectionManagerStatusChangedEventHandler(cnnMan_Stopped);

			svSpeech = new RecognizedSpeechSharedVariable("recognizedSpeech");
			svInteger = new IntSharedVariable("svInteger");
			svInteger.ValueChanged += new SharedVariableSubscriptionReportEventHadler<int>(svInteger_ValueChanged);
			svDoubles = new DoubleArraySharedVariable("svDoubles");
			svDoubles.ValueChanged += new SharedVariableSubscriptionReportEventHadler<double[]>(svDoubles_ValueChanged);
			svBytes = new ByteArraySharedVariable("svBytes");
			svBytes.ValueChanged += new SharedVariableSubscriptionReportEventHadler<byte[]>(svBytes_ValueChanged);
			svMatrix = new MatrixSharedVariable("svMatrix");
			svMatrix.ValueChanged += new SharedVariableSubscriptionReportEventHadler<Robotics.Mathematics.Matrix>(svMatrix_ValueChanged);
			svVector = new VectorSharedVariable("svVector");
			svVector.ValueChanged += new SharedVariableSubscriptionReportEventHadler<Robotics.Mathematics.Vector>(svVector_ValueChanged);
			svString = new StringSharedVariable("svString");
			svString.WriteNotification += new SharedVariableSubscriptionReportEventHadler<string>(svString_WriteNotification);
			svRecognized = new StringSharedVariable("recognized");
			svRecognized.WriteNotification += new SharedVariableSubscriptionReportEventHadler<string>(svString_WriteNotification);
		}

		void svBytes_ValueChanged(SharedVariableSubscriptionReport<byte[]> report)
		{
			Console.WriteLine(report);
		}

		void cnnMan_DataReceived(ConnectionManager connectionManager, TcpPacket packet)
		{
			//foreach (string s in packet.DataStrings)
			//{
			//	Console.WriteLine("Received " + s);
			//}
		}

		void cmdMan_SharedVariablesLoaded(CommandManager cmdMan)
		{
			Console.WriteLine(cmdMan.SharedVariables.Count.ToString() + " shared variables was loaded automatically");
			if (cmdMan.SharedVariables.Contains("connected"))
			{
				cmdMan.SharedVariables["connected"].Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteOthers);
				((StringSharedVariable)cmdMan.SharedVariables["connected"]).ValueChanged += new SharedVariableSubscriptionReportEventHadler<string>(svConnected_ValueChanged);
			}
			e.Set();
		}

		void svConnected_ValueChanged(SharedVariableSubscriptionReport<string> report)
		{
			Console.WriteLine("Connected modules:" + ((StringSharedVariable)report.Variable).BufferedData);
		}

		void svInteger_ValueChanged(SharedVariableSubscriptionReport<int> report)
		{
			Console.WriteLine(report.ToString());
		}

		void svString_WriteNotification(SharedVariableSubscriptionReport<string> report)
		{
			Console.WriteLine(report.ToString());
		}

		void svVector_ValueChanged(SharedVariableSubscriptionReport<Robotics.Mathematics.Vector> report)
		{
			Console.WriteLine(report.ToString());
		}


		void svMatrix_ValueChanged(SharedVariableSubscriptionReport<Robotics.Mathematics.Matrix> report)
		{
			Console.WriteLine(report.ToString());
		}

		void svDoubles_ValueChanged(SharedVariableSubscriptionReport<double[]> report)
		{
			Console.WriteLine(report.ToString());
		}



		void svBytes_WriteNotification(SharedVariableSubscriptionReport<byte[]> report)
		{
			string s = "0x";
			for (int i = 0; i < report.Value.Length; ++i)
				s += report.Value[i].ToString("X2");
			Console.WriteLine(s);
		}

		void svDoubles_WriteNotification(SharedVariableSubscriptionReport<double[]> report)
		{
			string s = "0x";
			for (int i = 0; i < report.Value.Length; ++i)
				s += report.Value[i].ToString() + ",";
			Console.WriteLine(s);
		}

		private void var1_WriteNotification(SharedVariableSubscriptionReport<string> report)
		{
			Console.WriteLine(report.ToString());
		}

		void cnnMan_ClientConnected(Socket s)
		{
			Console.WriteLine("Connected client " + s.RemoteEndPoint.ToString());
			cmdMan.SharedVariables.LoadFromBlackboard();
		}

		void cnnMan_Stopped(ConnectionManager connectionManager)
		{
			Console.WriteLine("Connection manager stopped");
		}

		void cnnMan_Started(ConnectionManager connectionManager)
		{
			Console.WriteLine("Connection manager started");
		}

		void cmdMan_Stopped(CommandManager commandManager)
		{
			Console.WriteLine("Command manager stopped");
		}

		void cmdMan_Started(CommandManager commandManager)
		{
			Console.WriteLine("Command manager started");
		}

		#region ITester Members

		public void Setup(string moduleName, int port)
		{
			cnnMan.ModuleName = "MVN-PLN1";
		}

		public void Run()
		{
			int svCount;
			cmdMan.Start();
			Response rsp;
			IAsyncResult result = 
			cmdMan.BeginSendCommand(new Command("stop", String.Empty), 100);
			cmdMan.EndSendCommand(result, out rsp);
			while (cnnMan.ConnectedClientsCount < 1)
				Thread.Sleep(100);

			e.WaitOne();
			Console.WriteLine("Loading shared variables from blackboard");
			svCount = cmdMan.SharedVariables.LoadFromBlackboard();
			cmdMan.Ready = true;
			Console.WriteLine("Loaded " + svCount.ToString() + " shared variables.");
			if (svCount > 0)
			{
				Console.WriteLine("Enumerating:");
				foreach (SharedVariable shv in cmdMan.SharedVariables)
				{
					Console.WriteLine("\t" + shv.ToString());
				}
			}
			Console.WriteLine();
			if (!cmdMan.SharedVariables.Contains(svSpeech))
				cmdMan.SharedVariables.Add(svSpeech);
			else
				svSpeech = (RecognizedSpeechSharedVariable)cmdMan.SharedVariables["recognizedSpeech"];
			if (!cmdMan.SharedVariables.Contains(svInteger))
				cmdMan.SharedVariables.Add(svInteger);
			else
				svInteger = (IntSharedVariable)cmdMan.SharedVariables["svInteger"];
			if (!cmdMan.SharedVariables.Contains(svDoubles))
				cmdMan.SharedVariables.Add(svDoubles);
			else
				svDoubles = (DoubleArraySharedVariable)cmdMan.SharedVariables["svDoubles"];
			if (!cmdMan.SharedVariables.Contains(svBytes))
				cmdMan.SharedVariables.Add(svBytes);
			else
				svBytes = (ByteArraySharedVariable)cmdMan.SharedVariables["svBytes"];
			if (!cmdMan.SharedVariables.Contains(svMatrix))
				cmdMan.SharedVariables.Add(svMatrix);
			else
				svMatrix = (MatrixSharedVariable)cmdMan.SharedVariables["svMatrix"];
			if (!cmdMan.SharedVariables.Contains(svVector))
				cmdMan.SharedVariables.Add(svVector);
			else
				svVector = (VectorSharedVariable)cmdMan.SharedVariables["svVector"];
			if (!cmdMan.SharedVariables.Contains(svString))
				cmdMan.SharedVariables.Add(svString);
			else
				svString = (StringSharedVariable)cmdMan.SharedVariables["svString"];

			if (!cmdMan.SharedVariables.Contains(svRecognized))
				cmdMan.SharedVariables.Add(svRecognized);
			else
				svRecognized = (StringSharedVariable)cmdMan.SharedVariables["recognized"];

			Console.Write("Subscribing...");
			try
			{
				svSpeech.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				svSpeech.WriteNotification += new SharedVariableSubscriptionReportEventHadler<Robotics.HAL.Sensors.RecognizedSpeech>(svSpeech_WriteNotification);
				svInteger.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				svDoubles.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				svDoubles.WriteNotification += new SharedVariableSubscriptionReportEventHadler<double[]>(svDoubles_WriteNotification);
				svMatrix.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				svVector.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				svString.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				svRecognized.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				svBytes.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				svBytes.WriteNotification += new SharedVariableSubscriptionReportEventHadler<byte[]>(svBytes_WriteNotification);
			}
			catch (Exception ex){ Console.WriteLine(ex.Message);}
			Console.Write("\b\b\b");
			Console.WriteLine();
			Console.WriteLine("Done!");

			if (cmdMan.SharedVariables.Contains("svBytes"))
			{
				byte[] data;
				Random rnd = new Random();

				for (int i = 0; i < 10; ++i)
				{
					data = new byte[rnd.Next(1, 21)];
					for (int j = 0; j < data.Length; ++j)
						data[j] = (byte)rnd.Next(1, 220);
					svBytes.TryWrite(data);
					Thread.Sleep(100);
				}
			}

			/*
			if (cmdMan.SharedVariables.Contains("svDoubles"))
			{
				double[] data;
				Random rnd = new Random();

				for (int i = 0; i < 10; ++i)
				{
					data = new double[rnd.Next(1, 21)];
					for (int j = 0; j < data.Length; ++j)
						data[j] = rnd.Next(1, 1000);
					svDoubles.TryWrite(data);
					Thread.Sleep(100);
				}
			}
			*/

			for (int i = 0; i < 500; ++i)
			{
				svSpeech.TryWrite(new Robotics.HAL.Sensors.RecognizedSpeech(
					new Robotics.HAL.Sensors.RecognizedSpeechAlternate[] { 
						new Robotics.HAL.Sensors.RecognizedSpeechAlternate(i.ToString(), 1),
						new Robotics.HAL.Sensors.RecognizedSpeechAlternate("Hello", 0)
					}
				));
				//Console.ReadLine();
			}

			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

			/*
			sw.Start();
			System.Text.StringBuilder sb = new System.Text.StringBuilder(65536);
			for (int i = 0; i < 65536; ++i)
			{
				sb.Append((char)('a' + i % 25));
			}
			string longStr = sb.ToString();
			sw.Stop();
			sw.Reset();
			//svString.TryWrite(longStr);
			sw.Start();
			for (int i = 0; i < 1000; ++i )
				svString.TryWrite(longStr, 0);
			sw.Stop();
			Console.WriteLine("Written 1000 times a 64k string > " + sw.ElapsedMilliseconds.ToString() + "ms elapsed.");
			Console.ReadLine();
			*/

			//Thread.Sleep(1000);
			svInteger.TryWrite(1);
			svDoubles.TryWrite(new double[] { 0, 0 });
			svMatrix.Write(Robotics.Mathematics.Matrix.Identity(2));
			svVector.Write(Robotics.Mathematics.Vector.Zero(4));
			svString.TryWrite("Realmente dijo \"Hola mundo\"?");
			Console.ReadLine();
			Console.Clear();
			//while (true)
			//	Thread.Sleep(10);

			if (cmdMan.SharedVariables.Contains("var1"))
			{
				VarSharedVariable var1 = (VarSharedVariable)cmdMan.SharedVariables["var1"];
				var1.WriteNotification += new SharedVariableSubscriptionReportEventHadler<string>(var1_WriteNotification);
				var1.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
				for (int i = 0; i < 10; ++i)
				{
					var1.Write(i.ToString());
					Thread.Sleep(100);
				}
			}
		}

		void svSpeech_WriteNotification(SharedVariableSubscriptionReport<Robotics.HAL.Sensors.RecognizedSpeech> report)
		{
			Console.WriteLine("Said: " + report.Value.Text);
		}

		#endregion
	}
}
