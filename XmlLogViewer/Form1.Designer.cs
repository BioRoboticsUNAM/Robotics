namespace XmlLogViewer
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtLogFilePath = new System.Windows.Forms.TextBox();
			this.lblLogFile = new System.Windows.Forms.Label();
			this.gbLogsInFile = new System.Windows.Forms.GroupBox();
			this.lbLogsInFile = new System.Windows.Forms.ListBox();
			this.dlgOpenLog = new System.Windows.Forms.OpenFileDialog();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.btnLoadLogFile = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.gbVerbosityLevels = new System.Windows.Forms.GroupBox();
			this.cblVerbosityLevels = new System.Windows.Forms.CheckedListBox();
			this.lvLog = new System.Windows.Forms.ListView();
			this.Time = new System.Windows.Forms.ColumnHeader();
			this.Priority = new System.Windows.Forms.ColumnHeader();
			this.Message = new System.Windows.Forms.ColumnHeader();
			this.Id = new System.Windows.Forms.ColumnHeader();
			this.gbLogsInFile.SuspendLayout();
			this.panel1.SuspendLayout();
			this.gbVerbosityLevels.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtLogFilePath
			// 
			this.txtLogFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLogFilePath.Location = new System.Drawing.Point(65, 14);
			this.txtLogFilePath.Name = "txtLogFilePath";
			this.txtLogFilePath.Size = new System.Drawing.Size(396, 20);
			this.txtLogFilePath.TabIndex = 0;
			// 
			// lblLogFile
			// 
			this.lblLogFile.AutoSize = true;
			this.lblLogFile.Location = new System.Drawing.Point(12, 17);
			this.lblLogFile.Name = "lblLogFile";
			this.lblLogFile.Size = new System.Drawing.Size(47, 13);
			this.lblLogFile.TabIndex = 4;
			this.lblLogFile.Text = "Log File:";
			// 
			// gbLogsInFile
			// 
			this.gbLogsInFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbLogsInFile.Controls.Add(this.lbLogsInFile);
			this.gbLogsInFile.Location = new System.Drawing.Point(3, 3);
			this.gbLogsInFile.Name = "gbLogsInFile";
			this.gbLogsInFile.Size = new System.Drawing.Size(200, 114);
			this.gbLogsInFile.TabIndex = 5;
			this.gbLogsInFile.TabStop = false;
			this.gbLogsInFile.Text = "Logs in File";
			// 
			// lbLogsInFile
			// 
			this.lbLogsInFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbLogsInFile.FormattingEnabled = true;
			this.lbLogsInFile.Location = new System.Drawing.Point(3, 16);
			this.lbLogsInFile.Name = "lbLogsInFile";
			this.lbLogsInFile.Size = new System.Drawing.Size(194, 95);
			this.lbLogsInFile.TabIndex = 4;
			this.lbLogsInFile.SelectedValueChanged += new System.EventHandler(this.lbLogsInFile_SelectedValueChanged);
			// 
			// dlgOpenLog
			// 
			this.dlgOpenLog.Title = "Open log file";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnBrowse.Image = global::XmlLogViewer.Properties.Resources.SearchFolderHS;
			this.btnBrowse.Location = new System.Drawing.Point(467, 12);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(23, 23);
			this.btnBrowse.TabIndex = 6;
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// btnLoadLogFile
			// 
			this.btnLoadLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLoadLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnLoadLogFile.Image = global::XmlLogViewer.Properties.Resources.openHS;
			this.btnLoadLogFile.Location = new System.Drawing.Point(496, 12);
			this.btnLoadLogFile.Name = "btnLoadLogFile";
			this.btnLoadLogFile.Size = new System.Drawing.Size(23, 23);
			this.btnLoadLogFile.TabIndex = 6;
			this.btnLoadLogFile.UseVisualStyleBackColor = true;
			this.btnLoadLogFile.Click += new System.EventHandler(this.btnLoadLogFile_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.gbVerbosityLevels);
			this.panel1.Controls.Add(this.gbLogsInFile);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 350);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(531, 120);
			this.panel1.TabIndex = 7;
			// 
			// gbVerbosityLevels
			// 
			this.gbVerbosityLevels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbVerbosityLevels.Controls.Add(this.cblVerbosityLevels);
			this.gbVerbosityLevels.Location = new System.Drawing.Point(209, 3);
			this.gbVerbosityLevels.Name = "gbVerbosityLevels";
			this.gbVerbosityLevels.Size = new System.Drawing.Size(200, 114);
			this.gbVerbosityLevels.TabIndex = 5;
			this.gbVerbosityLevels.TabStop = false;
			this.gbVerbosityLevels.Text = "Verbosity levels";
			// 
			// cblVerbosityLevels
			// 
			this.cblVerbosityLevels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cblVerbosityLevels.FormattingEnabled = true;
			this.cblVerbosityLevels.Location = new System.Drawing.Point(3, 16);
			this.cblVerbosityLevels.Name = "cblVerbosityLevels";
			this.cblVerbosityLevels.Size = new System.Drawing.Size(194, 94);
			this.cblVerbosityLevels.TabIndex = 0;
			this.cblVerbosityLevels.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.cblVerbosityLevels_ItemCheck);
			// 
			// lvLog
			// 
			this.lvLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Id,
            this.Time,
            this.Priority,
            this.Message});
			this.lvLog.Location = new System.Drawing.Point(12, 40);
			this.lvLog.Name = "lvLog";
			this.lvLog.Size = new System.Drawing.Size(507, 307);
			this.lvLog.TabIndex = 8;
			this.lvLog.UseCompatibleStateImageBehavior = false;
			this.lvLog.View = System.Windows.Forms.View.Details;
			// 
			// Time
			// 
			this.Time.Text = "Time";
			this.Time.Width = 100;
			// 
			// Priority
			// 
			this.Priority.Text = "Priority";
			this.Priority.Width = 50;
			// 
			// Message
			// 
			this.Message.Text = "Message";
			this.Message.Width = 400;
			// 
			// Id
			// 
			this.Id.Text = "Id";
			this.Id.Width = 50;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(531, 470);
			this.Controls.Add(this.lvLog);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.btnLoadLogFile);
			this.Controls.Add(this.txtLogFilePath);
			this.Controls.Add(this.lblLogFile);
			this.Name = "Form1";
			this.Text = "Log Viewer";
			this.gbLogsInFile.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.gbVerbosityLevels.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtLogFilePath;
		private System.Windows.Forms.Label lblLogFile;
		private System.Windows.Forms.GroupBox gbLogsInFile;
		private System.Windows.Forms.Button btnLoadLogFile;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.OpenFileDialog dlgOpenLog;
		private System.Windows.Forms.ListBox lbLogsInFile;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox gbVerbosityLevels;
		private System.Windows.Forms.CheckedListBox cblVerbosityLevels;
		private System.Windows.Forms.ListView lvLog;
		private System.Windows.Forms.ColumnHeader Time;
		private System.Windows.Forms.ColumnHeader Priority;
		private System.Windows.Forms.ColumnHeader Message;
		private System.Windows.Forms.ColumnHeader Id;
	}
}

