using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Robotics.API;
using Robotics.API.PrimitiveSharedVariables;

namespace SharedVariableInspector
{
	public enum InspectorStatus
	{
		Starting,
		WaitingForConnection,
		LoadingSharedVariables,
		Idle,
		Stopping,
		Stopped
	}

	public delegate void InspectorStatusChangedEH(Inspector inspector);
	public delegate void InspectorSharedVariableAddedEH(Inspector inspector, SharedVariable sharedVariable);

	public class Inspector
	{
		#region Variables

		private CommandManager cmdMan;
		private ConnectionManager cnnMan;
		private StringSharedVariable shVariableList;
		private InspectorStatus status;
		private List<string> subscribedVariables;

		#endregion

		#region Constructors

		public Inspector()
		{
			this.status = InspectorStatus.Stopped;
			this.subscribedVariables = new List<string>(1000);
			SetupCommandManager();
			SetupConnectionManager();
			SetupSharedVariables();
		}

		#endregion

		#region Events

		public event InspectorStatusChangedEH StatusChanged;

		public event SharedVariableUpdatedEventHadler SharedVariableUpdated;

		public event InspectorSharedVariableAddedEH SharedVariableAdded;

		#endregion

		#region Properties

		public CommandManager CommandManager
		{
			get { return this.cmdMan;}
		}

		public ConnectionManager ConnectionManager
		{
			get { return this.cnnMan; }
		}

		public StringSharedVariable SharedVariableList
		{
			get { return this.shVariableList; }
		}

		public InspectorStatus Status
		{
			get { return this.status; }
		}

		#endregion

		#region Methodos

		protected void OnStatusChanged()
		{
			if(this.StatusChanged !=null)
				StatusChanged(this);
		}

		protected void OnSharedVariableUpdated(SharedVariable sharedVariable)
		{
			if (this.SharedVariableUpdated != null)
				this.SharedVariableUpdated(sharedVariable);
		}

		protected void OnSharedVariableAdded(SharedVariable sharedVariable)
		{
			if (this.SharedVariableAdded != null)
				this.SharedVariableAdded(this, sharedVariable);
		}

		private void SetupCommandManager()
		{
			this.cmdMan = new CommandManager();
			this.cmdMan.Started+=new CommandManagerStatusChangedEventHandler(cmdMan_Started);
			this.cmdMan.Stopped += new CommandManagerStatusChangedEventHandler(cmdMan_Stopped);
			this.cmdMan.SharedVariablesLoaded += new SharedVariablesLoadedEventHandler(cmdMan_SharedVariablesLoaded);
		}

		private void SetupConnectionManager()
		{
			this.cnnMan = new ConnectionManager(2500, cmdMan);
			this.cnnMan.ClientConnected +=
				new System.Net.Sockets.TcpClientConnectedEventHandler(cnnMan_ClientConnected);
			this.cnnMan.ClientDisconnected
				+= new System.Net.Sockets.TcpClientDisconnectedEventHandler(cnnMan_ClientDisconnected);
		}

		private void SetupSharedVariables()
		{
			this.shVariableList = new StringSharedVariable("vars");
		}

		public void Start()
		{
			if (cmdMan.IsRunning)
				return;
			this.status = InspectorStatus.Starting;
			OnStatusChanged();
			subscribedVariables.Clear();
			cmdMan.Start();
		}

		public void Stop()
		{
			if (!cmdMan.IsRunning)
				return;
			this.status = InspectorStatus.Stopping;
			OnStatusChanged();
			subscribedVariables.Clear();
			cmdMan.Stop();
		}

		private void SubscribeToAll()
		{
			foreach (SharedVariable sv in this.cmdMan.SharedVariables)
			{
				if (subscribedVariables.Contains(sv.Name))
					continue;
				try
				{
					sv.Subscribe(SharedVariableReportType.Notify, SharedVariableSubscriptionType.WriteAny);
					subscribedVariables.Add(sv.Name);
					sv.Updated += new SharedVariableUpdatedEventHadler(sv_Updated);
					OnSharedVariableAdded(sv);
				}
				catch { continue; }
			}
		}

		public void UpdateSharedVariableList()
		{
			string message;
			if (this.cmdMan.SharedVariables.LoadFromBlackboard(1000, out message) > 0)
				SubscribeToAll();
		}

		#endregion

		#region Command Manager Event Handlers

		private void cmdMan_Stopped(CommandManager commandManager)
		{
			this.status = InspectorStatus.Stopped;
			OnStatusChanged();
		}

		private void cmdMan_Started(CommandManager commandManager)
		{
			this.status = InspectorStatus.WaitingForConnection;
			OnStatusChanged();
		}

		private void cmdMan_SharedVariablesLoaded(CommandManager cmdMan)
		{
			if (this.cmdMan.SharedVariables.Contains(this.shVariableList.Name))
				this.shVariableList = (StringSharedVariable)this.cmdMan.SharedVariables[this.shVariableList.Name];
			else
				this.cmdMan.SharedVariables.Add(shVariableList);

			this.shVariableList.Subscribe(SharedVariableReportType.Notify, SharedVariableSubscriptionType.WriteAny);
			this.shVariableList.WriteNotification += new SharedVariableSubscriptionReportEventHadler<string>(shVariableList_WriteNotification);
			SubscribeToAll();
			this.status = InspectorStatus.Idle;
			OnStatusChanged();
			this.CommandManager.Ready = true;
		}

		#endregion

		#region Connection Manager Event Handlers

		private void cnnMan_ClientDisconnected(System.Net.EndPoint ep)
		{
			SetupSharedVariables();
			this.status = InspectorStatus.WaitingForConnection;
			OnStatusChanged();
		}

		private void cnnMan_ClientConnected(System.Net.Sockets.Socket s)
		{
			this.status = InspectorStatus.LoadingSharedVariables;
			OnStatusChanged();
		}

		#endregion

		#region Shared Variables Event Handlers

		private void shVariableList_WriteNotification(SharedVariableSubscriptionReport<string> report)
		{
			SubscribeToAll();
		}

		private void sv_Updated(SharedVariable sharedVariable)
		{
			OnSharedVariableUpdated(sharedVariable);
		}

		#endregion
	}
}
