using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace XmlLogViewer
{
	public partial class Form1 : Form
	{
		private XmlDocument logDocument;
		private XmlNodeList logNodes;
		private List<ListViewItem> logRecords;

		public Form1()
		{
			InitializeComponent();
			logRecords = new List<ListViewItem>(1000);
		}

		private void AddRecordsWithPriority(int priority)
		{
			int recordPriority;

			for (int i = 0; i < logRecords.Count; ++i)
			{
				recordPriority = Int32.Parse(logRecords[i].SubItems[2].Text);
				if ((recordPriority == priority) && !lvLog.Items.Contains(logRecords[i]))
					lvLog.Items.Add(logRecords[i]);
			}
			lvLog.Refresh();
		}

		private void AnalizeFile(string filePath)
		{
			logDocument = new XmlDocument();
			logDocument.Load(new XmlTextReader(filePath));
			CountLogsInFile();
			if (lbLogsInFile.Items.Count < 1)
			{
				lbLogsInFile.SelectedIndex = -1;
				MessageBox.Show("The specified file does not contain any log", "Log record not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			lbLogsInFile.SelectedIndex = 0;
		}

		private void BrowseFile()
		{
			if (dlgOpenLog.ShowDialog() != DialogResult.OK)
				return;
			txtLogFilePath.Text = dlgOpenLog.FileName;
		}

		private void ClearAll()
		{
			lvLog.Items.Clear();
			lbLogsInFile.Items.Clear();
			ClearLog();
		}

		private void ClearLog()
		{
			logRecords.Clear();
			lvLog.Items.Clear();
			cblVerbosityLevels.Items.Clear();
		}

		private void CountLogsInFile()
		{
			string date;
			XmlAttribute attribute;

			logNodes = logDocument.GetElementsByTagName("log");
			for (int i = 0; i < logNodes.Count; ++i)
			{
				attribute = logNodes[i].Attributes["startTime"];
				date = (attribute == null) ? "Unknown time" : attribute.Value;
				lbLogsInFile.Items.Add(date);
			}
		}

		private void Filter()
		{
			List<int> priorities;
			int priority;

			priorities = new List<int>();
			for (int i = 0; i < cblVerbosityLevels.CheckedIndices.Count; ++i)
				priorities.Add((int)cblVerbosityLevels.Items[(int)cblVerbosityLevels.CheckedIndices[i]]);

			lvLog.Items.Clear();
			for (int i = 0; i < logRecords.Count; ++i)
			{
				priority = Int32.Parse(logRecords[i].SubItems[2].Text);
				if (!priorities.Contains(priority))
					continue;
				lvLog.Items.Add(logRecords[i]);
			}
			lvLog.Refresh();
		}

		private void LoadLog(int index)
		{
			List<int> priorities;
			XmlAttribute attribute;
			int id;
			DateTime time;
			int priority;
			string value;
			int minId;
			int maxId;
			ListViewItem lvi;

			ClearLog();
			if (index < 0)
			{
				return;
			}

			priorities = new List<int>();
			minId = Int32.MaxValue;
			maxId = Int32.MinValue;
			foreach (XmlNode node in logNodes[index].ChildNodes)
			{
				if ((node == null) || (node.Name != "logRecord"))
					continue;
				attribute = node.Attributes["id"];
				if ((attribute == null) || !Int32.TryParse(attribute.Value, out id))
					continue;
				attribute = node.Attributes["time"];
				if((attribute == null) || !DateTime.TryParse(attribute.Value, out time))
					continue;
				attribute = node.Attributes["priority"];
				if ((attribute == null) || !Int32.TryParse(attribute.Value, out priority))
					continue;
				value = node.InnerText;

				if (!priorities.Contains(priority))
					priorities.Add(priority);
				if (minId > id) minId = id;
				if (maxId < id) maxId = id;
				lvi = new ListViewItem(new string[]{id.ToString(),
					time.ToString("HH:mm:ss.fff"),
					priority.ToString(), value});
				logRecords.Add(lvi);
				lvLog.Items.Add(lvi);
			}
			lvLog.Refresh();
			for (int i = 0; i < priorities.Count; ++i )
				cblVerbosityLevels.Items.Add(priorities[i], true);
			cblVerbosityLevels.Refresh();

		}

		private void OpenLogFile()
		{
			OpenLogFile(txtLogFilePath.Text);
		}

		private void OpenLogFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				ClearAll();
				return;
			}
			try
			{
				FileInfo fi = new FileInfo(filePath);
				txtLogFilePath.Text = fi.FullName;
				AnalizeFile(filePath);
			}
			catch(Exception ex) 
			{
				MessageBox.Show("Invalid data in the specified file\r\n\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				ClearAll();
			}
		}

		private void RemoveRecordsWithPriority(int priority)
		{
			int recordPriority;

			for (int i = 0; i < lvLog.Items.Count; ++i)
			{
				recordPriority = Int32.Parse(lvLog.Items[i].SubItems[2].Text);
				if (recordPriority == priority)
				{
					lvLog.Items.RemoveAt(i);
					--i;
				}
			}

			lvLog.Refresh();
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			BrowseFile();
		}

		private void btnLoadLogFile_Click(object sender, EventArgs e)
		{
			if (!File.Exists(txtLogFilePath.Text))
				BrowseFile();
			OpenLogFile();
		}

		private void lbLogsInFile_SelectedValueChanged(object sender, EventArgs e)
		{
			LoadLog(lbLogsInFile.SelectedIndex);
		}

		private void cblVerbosityLevels_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (e.NewValue == CheckState.Checked)
				AddRecordsWithPriority((int)cblVerbosityLevels.Items[e.Index]);
			else
				RemoveRecordsWithPriority((int)cblVerbosityLevels.Items[e.Index]);
		}
	}
}
