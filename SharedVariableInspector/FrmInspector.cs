using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Robotics.API;

namespace SharedVariableInspector
{
	public partial class FrmInspector : Form
	{
		#region Variables

		private Inspector inspector;

		#endregion

		#region Constructors

		public FrmInspector()
		{
			InitializeComponent();
			//AddButtonsToPropertyGrid();
			lvSharedVariableList.View = View.Details;
			lvSharedVariableList.Columns.Add("Name", "Name", 100);
			lvSharedVariableList.Columns.Add("Type", "Type", 75);
			lvSharedVariableList.Columns.Add("LastUpdated", "Last updated", 75);
			lvSharedVariableList.Columns.Add("LastWriter", "Last writer", 150);
			lvSharedVariableList.FullRowSelect = true;
			inspector = new Inspector();
			inspector.StatusChanged += new InspectorStatusChangedEH(inspector_StatusChanged);
			inspector.SharedVariableAdded += new InspectorSharedVariableAddedEH(inspector_SharedVariableAdded);
			inspector.SharedVariableUpdated += new SharedVariableUpdatedEventHadler(inspector_SharedVariableUpdated);
		}

		#endregion

		#region Properties
		#endregion

		#region Methodos

		private void AddButtonsToPropertyGrid()
		{
			/*
			
			ToolStrip pgToolStrip = (ToolStrip)typeof(PropertyGrid).InvokeMember("toolStrip",
				System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
				null,
				pgSharedVarProperties,
				null);
			ToolStripButton button = new ToolStripButton("Hello");
			pgToolStrip.Items.Add(button);
			*/
		}

		private void UpdateSharedVariableInfo()
		{
			if ((lvSharedVariableList.SelectedIndices.Count == 0) || (lvSharedVariableList.SelectedIndices[0] == -1))
			{
				tsbSharedVariableSubscriptionReport.Enabled = false;
				tsbSharedVariableSubscriptionContent.Enabled = false;
				tsbUpdateSharedVariableInfo.Enabled = false;
				tsbUpdateSharedVariableValue.Enabled = false;
				pgSharedVarProperties.SelectedObject = null;
				pgSharedVarProperties.Refresh();
				return;
			}

			SharedVariable sv = (SharedVariable)lvSharedVariableList.SelectedItems[0].Tag;
			if (sv == null)
				return;
			tsbSharedVariableSubscriptionContent.Enabled = true;
			tsbSharedVariableSubscriptionContent.Checked = (sv.SubscriptionReportType == SharedVariableReportType.SendContent);
			tsbSharedVariableSubscriptionReport.Enabled = true;
			tsbSharedVariableSubscriptionReport.Checked = (sv.SubscriptionReportType == SharedVariableReportType.Notify);
			tsbUpdateSharedVariableInfo.Enabled = true;
			tsbUpdateSharedVariableValue.Enabled = (sv.SubscriptionReportType == SharedVariableReportType.Notify);
			pgSharedVarProperties.SelectedObject = new SharedVariableInspectorWrapper(sv);
			pgSharedVarProperties.Refresh();
		}

		#endregion

		#region Event Handlers

		private void FrmInspector_Load(object sender, EventArgs e)
		{
			inspector.Start();
		}

		private void lvSharedVariableList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateSharedVariableInfo();	
		}

		private void tsbRefreshSharedVariableList_Click(object sender, EventArgs e)
		{
			this.inspector.UpdateSharedVariableList();
		}

		private void tsbUpdateSharedVariableInfo_Click(object sender, EventArgs e)
		{
			SharedVariable sv = (SharedVariable)lvSharedVariableList.SelectedItems[0].Tag;
			if (sv == null)
				return;
			try
			{
				sv.UpdateInfo();
			}
			catch { }
			UpdateSharedVariableInfo();
		}

		private void tsbUpdateSharedVariableValue_Click(object sender, EventArgs e)
		{
			SharedVariable sv = (SharedVariable)lvSharedVariableList.SelectedItems[0].Tag;
			if (sv == null)
				return;
			try
			{
				sv.UpdateBufferedData();
			}
			catch { }
			UpdateSharedVariableInfo();
		}

		private void tsbSharedVariableSubscriptionReport_Click(object sender, EventArgs e)
		{
			SharedVariable sv = (SharedVariable)lvSharedVariableList.SelectedItems[0].Tag;
			if (sv == null)
				return;
			try
			{
				sv.Subscribe(SharedVariableReportType.Notify, SharedVariableSubscriptionType.WriteAny);

			}
			catch { }
			UpdateSharedVariableInfo();
		}

		private void tsbSharedVariableSubscriptionContent_Click(object sender, EventArgs e)
		{
			SharedVariable sv = (SharedVariable)lvSharedVariableList.SelectedItems[0].Tag;
			if (sv == null)
				return;
			try
			{
				sv.Subscribe(SharedVariableReportType.SendContent, SharedVariableSubscriptionType.WriteAny);
			}
			catch { }
			UpdateSharedVariableInfo();
		}
		
		private void inspector_StatusChanged(Inspector inspector)
		{
			this.lblStatus.Text = inspector.Status.ToString();
		}

		private void inspector_SharedVariableAdded(Inspector inspector, SharedVariable sharedVariable)
		{
			this.BeginInvoke(new EventHandler (delegate(object o, EventArgs e)
			{
				string[] row = { sharedVariable.Name,
								   sharedVariable.TypeName + (sharedVariable.IsArray ? "[]" : String.Empty),
								   sharedVariable.LastUpdated.ToString("hh:mm:ss"),
								   String.IsNullOrEmpty(sharedVariable.LastWriter) ? "(Unknown)" : sharedVariable.LastWriter
							   };
				ListViewItem item = new ListViewItem(row);
				item.Name = sharedVariable.Name;
				item.Tag = sharedVariable;
				lvSharedVariableList.Items.Add(item);
				item.BackColor = (item.Index % 2 == 0) ? Color.White : Color.LightGray;
				lvSharedVariableList.Refresh();
			}));
		}

		private void inspector_SharedVariableUpdated(SharedVariable sharedVariable)
		{
			this.BeginInvoke(new EventHandler(delegate(object o, EventArgs e)
			{
				ListViewItem item = lvSharedVariableList.Items[sharedVariable.Name];
				int ixLU = lvSharedVariableList.Columns["LastUpdated"].Index;
				int ixLW = lvSharedVariableList.Columns["LastWriter"].Index;
				item.SubItems[ixLU].Text = sharedVariable.LastUpdated.ToString("hh:mm:ss");
				item.SubItems[ixLW].Text =
					String.IsNullOrEmpty(sharedVariable.LastWriter) ? "(Unknown)" : sharedVariable.LastWriter;
				lvSharedVariableList.Refresh();
				pgSharedVarProperties.Refresh();
			}));
		}

		#endregion
	}
}
