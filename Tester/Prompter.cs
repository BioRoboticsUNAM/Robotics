using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Robotics.API;
using System.Runtime.Serialization;

namespace Tester
{
	public delegate void ResponseSentEventHandler (Command command, Response response);

	[SerializableAttribute]
	public class Prompter : ISerializable
	{
		#region Serializable Variables

		private string module;
		private string commandName;
		private List<string> usedParamList;

		#endregion

		#region Non Serializable Variables
		
		[NonSerializedAttribute]
		private static List<Thread> threadList = new List<Thread>();
		
		[NonSerializedAttribute]
		private ParameterizedThreadStart dlgPromptTask;

		[NonSerializedAttribute]
		private FrmTester ownerForm;

		#endregion

		#region Constructors

		public Prompter(FrmTester ownerForm, string module, string commandName)
		{
			this.commandName = commandName;
			this.module = module;
			this.usedParamList = new List<string>();
			this.ownerForm = ownerForm;
			dlgPromptTask = new ParameterizedThreadStart(PromptTask);
		}

		public Prompter(SerializationInfo info, StreamingContext context)
		{
			module = (string)info.GetValue("module", typeof(string));
			commandName = (string)info.GetValue("commandName", typeof(string));
			usedParamList = (List<string>)info.GetValue("usedParamList", typeof(List<string>));
		}

		#endregion

		#region Events

		public event ResponseSentEventHandler ResponseSent;

		#endregion

		#region Properties

		public string Module { get { return module; } }

		public string CommandName { get { return commandName; } }

		public List<string> UsedParamList
		{
			get { return usedParamList; }
		}

		public FrmTester OwnerForm
		{
			get { return ownerForm; }
		}

		#endregion

		#region Methodos

		public bool Match(Command command)
		{
			if(command == null)
				return false;
			return (((module == "[ANY]") || (module == command.SourceModule)) && (commandName == command.CommandName));
		}

		public void Prompt(ConnectionManager cnnMan, Command command)
		{
			if (!Match(command))
				return;

			Thread promptThread = new Thread(dlgPromptTask);
			PromptTaskParam startParams;
			lock (threadList)
			{
				threadList.Add(promptThread);
			}
			promptThread.IsBackground = true;
			promptThread.SetApartmentState(ApartmentState.STA);
			startParams = new PromptTaskParam(promptThread, cnnMan, command);
			promptThread.Start(startParams);
		}

		private void PromptTask(object oArgs)
		{
			ConnectionManager cnnMan;
			Command command;
			Thread thisThread;
			FrmPromptDialog promptDialog = null;
			Response response;
			PromptTaskParam args;
			DialogResult dialogResult;

			args = oArgs as PromptTaskParam;
			if (args == null)
				return;

			thisThread = args.Thread;
			cnnMan = args.ConnectionManager;
			command = args.Command;

			try
			{
				promptDialog = ownerForm.GetPromptDialog();
				promptDialog.Command = command;
				promptDialog.AddAutoComplete(usedParamList.ToArray());
				dialogResult = ownerForm.ShowPromptDialog(promptDialog);
				if ((dialogResult != DialogResult.OK) || (promptDialog.Response == null))
				{
					lock (threadList)
					{
						if (threadList.Contains(thisThread))
							threadList.Remove(thisThread);
					}
					return;
				}
				response = promptDialog.Response;
				if(cnnMan.Send(response) && (ResponseSent !=null))
				{
					try
					{
						ResponseSent(command, response);
					}
					catch { }
				}
				lock (usedParamList)
				{
					if (!usedParamList.Contains(response.Parameters))
						usedParamList.Add(response.Parameters);
				}
				lock (threadList)
				{
					if (threadList.Contains(thisThread))
						threadList.Remove(thisThread);
				}
			}
			catch
			{
				if ((promptDialog != null) && !promptDialog.Disposing && !promptDialog.IsDisposed)
				{
					promptDialog.Close();
					if (!promptDialog.Disposing && !promptDialog.IsDisposed)
						promptDialog.Dispose();
				}
				lock (threadList)
				{
					if (threadList.Contains(thisThread))
						threadList.Remove(thisThread);
				}
			}
		}

		public override bool Equals(object obj)
		{
			Prompter other = obj as Prompter;
			if(other == null)
				return false;
			return (this.commandName == other.commandName) && (this.module == other.module);
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
			sb.Append("...");
			sb.Append('"');
			sb.Append(' ');
			sb.Append('?');
			return sb.ToString();
		}

		public static void AbortAll()
		{
			for (int i = 0; i < threadList.Count; ++i)
			{
				lock (threadList[i])
				{
					try
					{
						if (threadList[i].IsAlive)
							threadList[i].Abort();
					}
					catch { }
				}
			}
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("module", module, typeof(string));
			info.AddValue("commandName", commandName, typeof(string));
			info.AddValue("usedParamList", usedParamList, typeof(List<string>));
		}

		#endregion

		#endregion

		#region Subclases

		public class PromptTaskParam
		{
			private Thread thread;
			private ConnectionManager cnnMan;
			private Command command;

			public PromptTaskParam(Thread thread, ConnectionManager cnnMan, Command command)
			{
				this.thread = thread;
				this.cnnMan = cnnMan;
				this.command = command;
			}

			public Thread Thread { get { return thread; } }
			public ConnectionManager ConnectionManager { get { return cnnMan; } }
			public Command Command { get { return command; } }
		}

		#endregion
	}
}
