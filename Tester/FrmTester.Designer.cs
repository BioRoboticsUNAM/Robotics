namespace Tester
{
	partial class FrmTester
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
			this.components = new System.ComponentModel.Container();
			this.gbModules = new System.Windows.Forms.GroupBox();
			this.btnModuleRemove = new System.Windows.Forms.Button();
			this.btnModuleStartStop = new System.Windows.Forms.Button();
			this.lstModules = new System.Windows.Forms.ListBox();
			this.gbAddModule = new System.Windows.Forms.GroupBox();
			this.btnAddModule = new System.Windows.Forms.Button();
			this.lblModuleName = new System.Windows.Forms.Label();
			this.txtModuleName = new System.Windows.Forms.TextBox();
			this.txtModulePort = new System.Windows.Forms.TextBox();
			this.lblModulePort = new System.Windows.Forms.Label();
			this.gbAutoResponder = new System.Windows.Forms.GroupBox();
			this.lblARFailureRate = new System.Windows.Forms.Label();
			this.nudARFailureRate = new System.Windows.Forms.NumericUpDown();
			this.lstAutoResponders = new System.Windows.Forms.ListBox();
			this.btnARDelete = new System.Windows.Forms.Button();
			this.btnARAdd = new System.Windows.Forms.Button();
			this.txtARParameters = new System.Windows.Forms.TextBox();
			this.lblARParameters = new System.Windows.Forms.Label();
			this.txtARCommandName = new System.Windows.Forms.TextBox();
			this.lblARCommandName = new System.Windows.Forms.Label();
			this.lblARModule = new System.Windows.Forms.Label();
			this.cmbARModule = new System.Windows.Forms.ComboBox();
			this.gbPrompter = new System.Windows.Forms.GroupBox();
			this.lstPrompter = new System.Windows.Forms.ListBox();
			this.btnPrmptDelete = new System.Windows.Forms.Button();
			this.btnPrmptAdd = new System.Windows.Forms.Button();
			this.txtPrmptParameters = new System.Windows.Forms.TextBox();
			this.lblPrmptParameters = new System.Windows.Forms.Label();
			this.txtPrmptCommandName = new System.Windows.Forms.TextBox();
			this.lblPrmptCommandName = new System.Windows.Forms.Label();
			this.lblPrmptModule = new System.Windows.Forms.Label();
			this.cmbPrmptModule = new System.Windows.Forms.ComboBox();
			this.gbConsole = new System.Windows.Forms.GroupBox();
			this.txtConsole = new System.Windows.Forms.TextBox();
			this.bgwPerformanceTestWorker = new System.ComponentModel.BackgroundWorker();
			this.tcTabs = new System.Windows.Forms.TabControl();
			this.tpAutoRespondersAndPrompters = new System.Windows.Forms.TabPage();
			this.scSplitter1 = new System.Windows.Forms.SplitContainer();
			this.tpCommandAndPerformanceTest = new System.Windows.Forms.TabPage();
			this.scSplitter2 = new System.Windows.Forms.SplitContainer();
			this.gbQC = new System.Windows.Forms.GroupBox();
			this.lblQCId = new System.Windows.Forms.Label();
			this.idbQCCommandId = new Robotics.Controls.IdBox();
			this.lstQuickCommands = new System.Windows.Forms.ListBox();
			this.btnQCDelete = new System.Windows.Forms.Button();
			this.btnQCAdd = new System.Windows.Forms.Button();
			this.txtQCParameters = new System.Windows.Forms.TextBox();
			this.lblQCParameters = new System.Windows.Forms.Label();
			this.txtQCCommandName = new System.Windows.Forms.TextBox();
			this.lblQCCommandName = new System.Windows.Forms.Label();
			this.lblQCModule = new System.Windows.Forms.Label();
			this.cmbQCModule = new System.Windows.Forms.ComboBox();
			this.gbPerformanceTest = new System.Windows.Forms.GroupBox();
			this.txtPerformanceTestSucceeded = new System.Windows.Forms.TextBox();
			this.lblPerformanceTestSucceeded = new System.Windows.Forms.Label();
			this.txtPerformanceTestElapsed = new System.Windows.Forms.TextBox();
			this.lblPerformanceTestElapsed = new System.Windows.Forms.Label();
			this.pbPerformanceTestProgress = new System.Windows.Forms.ProgressBar();
			this.lblPerformanceTestTimeout = new System.Windows.Forms.Label();
			this.lblPerformanceTestExecutions = new System.Windows.Forms.Label();
			this.nudPerformanceTestTimeout = new System.Windows.Forms.NumericUpDown();
			this.nudPerformanceTestExecutions = new System.Windows.Forms.NumericUpDown();
			this.btnStartStopPerformanceTest = new System.Windows.Forms.Button();
			this.txtPerformanceTestParameters = new System.Windows.Forms.TextBox();
			this.lblPerformanceTestParameters = new System.Windows.Forms.Label();
			this.txtPerformanceTestCommandName = new System.Windows.Forms.TextBox();
			this.lblPerformanceTest = new System.Windows.Forms.Label();
			this.lblPerformanceTestModule = new System.Windows.Forms.Label();
			this.cmbPerformanceTestModule = new System.Windows.Forms.ComboBox();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsiSerialize = new System.Windows.Forms.ToolStripMenuItem();
			this.tsiDeserialize = new System.Windows.Forms.ToolStripMenuItem();
			this.gbModules.SuspendLayout();
			this.gbAddModule.SuspendLayout();
			this.gbAutoResponder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudARFailureRate)).BeginInit();
			this.gbPrompter.SuspendLayout();
			this.gbConsole.SuspendLayout();
			this.tcTabs.SuspendLayout();
			this.tpAutoRespondersAndPrompters.SuspendLayout();
			this.scSplitter1.Panel1.SuspendLayout();
			this.scSplitter1.Panel2.SuspendLayout();
			this.scSplitter1.SuspendLayout();
			this.tpCommandAndPerformanceTest.SuspendLayout();
			this.scSplitter2.Panel1.SuspendLayout();
			this.scSplitter2.Panel2.SuspendLayout();
			this.scSplitter2.SuspendLayout();
			this.gbQC.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.idbQCCommandId)).BeginInit();
			this.gbPerformanceTest.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPerformanceTestTimeout)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPerformanceTestExecutions)).BeginInit();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbModules
			// 
			this.gbModules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.gbModules.Controls.Add(this.btnModuleRemove);
			this.gbModules.Controls.Add(this.btnModuleStartStop);
			this.gbModules.Controls.Add(this.lstModules);
			this.gbModules.Location = new System.Drawing.Point(13, 28);
			this.gbModules.Name = "gbModules";
			this.gbModules.Size = new System.Drawing.Size(175, 370);
			this.gbModules.TabIndex = 0;
			this.gbModules.TabStop = false;
			this.gbModules.Text = "Modules";
			// 
			// btnModuleRemove
			// 
			this.btnModuleRemove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnModuleRemove.Location = new System.Drawing.Point(6, 336);
			this.btnModuleRemove.Name = "btnModuleRemove";
			this.btnModuleRemove.Size = new System.Drawing.Size(163, 23);
			this.btnModuleRemove.TabIndex = 2;
			this.btnModuleRemove.Text = "Stop and Remove";
			this.btnModuleRemove.UseVisualStyleBackColor = true;
			this.btnModuleRemove.Click += new System.EventHandler(this.btnModuleRemove_Click);
			// 
			// btnModuleStartStop
			// 
			this.btnModuleStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnModuleStartStop.Location = new System.Drawing.Point(6, 307);
			this.btnModuleStartStop.Name = "btnModuleStartStop";
			this.btnModuleStartStop.Size = new System.Drawing.Size(163, 23);
			this.btnModuleStartStop.TabIndex = 1;
			this.btnModuleStartStop.Text = "Stop";
			this.btnModuleStartStop.UseVisualStyleBackColor = true;
			this.btnModuleStartStop.Click += new System.EventHandler(this.btnModuleStartStop_Click);
			// 
			// lstModules
			// 
			this.lstModules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstModules.FormattingEnabled = true;
			this.lstModules.Location = new System.Drawing.Point(6, 19);
			this.lstModules.Name = "lstModules";
			this.lstModules.Size = new System.Drawing.Size(163, 238);
			this.lstModules.TabIndex = 0;
			this.lstModules.SelectedIndexChanged += new System.EventHandler(this.lstModules_SelectedIndexChanged);
			// 
			// gbAddModule
			// 
			this.gbAddModule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.gbAddModule.Controls.Add(this.btnAddModule);
			this.gbAddModule.Controls.Add(this.lblModuleName);
			this.gbAddModule.Controls.Add(this.txtModuleName);
			this.gbAddModule.Controls.Add(this.txtModulePort);
			this.gbAddModule.Controls.Add(this.lblModulePort);
			this.gbAddModule.Location = new System.Drawing.Point(13, 404);
			this.gbAddModule.Name = "gbAddModule";
			this.gbAddModule.Size = new System.Drawing.Size(175, 102);
			this.gbAddModule.TabIndex = 1;
			this.gbAddModule.TabStop = false;
			this.gbAddModule.Text = "Add Module";
			// 
			// btnAddModule
			// 
			this.btnAddModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddModule.Location = new System.Drawing.Point(9, 73);
			this.btnAddModule.Name = "btnAddModule";
			this.btnAddModule.Size = new System.Drawing.Size(160, 23);
			this.btnAddModule.TabIndex = 5;
			this.btnAddModule.Text = "Add Module";
			this.btnAddModule.UseVisualStyleBackColor = true;
			this.btnAddModule.Click += new System.EventHandler(this.btnAddModule_Click);
			// 
			// lblModuleName
			// 
			this.lblModuleName.AutoSize = true;
			this.lblModuleName.Location = new System.Drawing.Point(6, 23);
			this.lblModuleName.Name = "lblModuleName";
			this.lblModuleName.Size = new System.Drawing.Size(38, 13);
			this.lblModuleName.TabIndex = 2;
			this.lblModuleName.Text = "Name:";
			// 
			// txtModuleName
			// 
			this.txtModuleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.txtModuleName.Location = new System.Drawing.Point(50, 20);
			this.txtModuleName.Name = "txtModuleName";
			this.txtModuleName.Size = new System.Drawing.Size(119, 20);
			this.txtModuleName.TabIndex = 3;
			this.txtModuleName.TextChanged += new System.EventHandler(this.txtModuleName_TextChanged);
			this.txtModuleName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtModuleName_KeyUp);
			// 
			// txtModulePort
			// 
			this.txtModulePort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtModulePort.Location = new System.Drawing.Point(50, 46);
			this.txtModulePort.Name = "txtModulePort";
			this.txtModulePort.Size = new System.Drawing.Size(119, 20);
			this.txtModulePort.TabIndex = 4;
			this.txtModulePort.TextChanged += new System.EventHandler(this.txtModulePort_TextChanged);
			this.txtModulePort.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtModulePort_KeyUp);
			// 
			// lblModulePort
			// 
			this.lblModulePort.AutoSize = true;
			this.lblModulePort.Location = new System.Drawing.Point(6, 49);
			this.lblModulePort.Name = "lblModulePort";
			this.lblModulePort.Size = new System.Drawing.Size(29, 13);
			this.lblModulePort.TabIndex = 0;
			this.lblModulePort.Text = "Port:";
			// 
			// gbAutoResponder
			// 
			this.gbAutoResponder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbAutoResponder.Controls.Add(this.lblARFailureRate);
			this.gbAutoResponder.Controls.Add(this.nudARFailureRate);
			this.gbAutoResponder.Controls.Add(this.lstAutoResponders);
			this.gbAutoResponder.Controls.Add(this.btnARDelete);
			this.gbAutoResponder.Controls.Add(this.btnARAdd);
			this.gbAutoResponder.Controls.Add(this.txtARParameters);
			this.gbAutoResponder.Controls.Add(this.lblARParameters);
			this.gbAutoResponder.Controls.Add(this.txtARCommandName);
			this.gbAutoResponder.Controls.Add(this.lblARCommandName);
			this.gbAutoResponder.Controls.Add(this.lblARModule);
			this.gbAutoResponder.Controls.Add(this.cmbARModule);
			this.gbAutoResponder.Location = new System.Drawing.Point(3, 3);
			this.gbAutoResponder.Name = "gbAutoResponder";
			this.gbAutoResponder.Size = new System.Drawing.Size(175, 367);
			this.gbAutoResponder.TabIndex = 1;
			this.gbAutoResponder.TabStop = false;
			this.gbAutoResponder.Text = "Auto Responders";
			// 
			// lblARFailureRate
			// 
			this.lblARFailureRate.AutoSize = true;
			this.lblARFailureRate.Location = new System.Drawing.Point(6, 101);
			this.lblARFailureRate.Name = "lblARFailureRate";
			this.lblARFailureRate.Size = new System.Drawing.Size(67, 13);
			this.lblARFailureRate.TabIndex = 12;
			this.lblARFailureRate.Text = "Failure Rate:";
			// 
			// nudARFailureRate
			// 
			this.nudARFailureRate.Increment = new decimal(new int[] {
			50,
			0,
			0,
			0});
			this.nudARFailureRate.Location = new System.Drawing.Point(75, 99);
			this.nudARFailureRate.Name = "nudARFailureRate";
			this.nudARFailureRate.Size = new System.Drawing.Size(94, 20);
			this.nudARFailureRate.TabIndex = 13;
			// 
			// lstAutoResponders
			// 
			this.lstAutoResponders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstAutoResponders.FormattingEnabled = true;
			this.lstAutoResponders.Location = new System.Drawing.Point(6, 183);
			this.lstAutoResponders.Name = "lstAutoResponders";
			this.lstAutoResponders.Size = new System.Drawing.Size(163, 147);
			this.lstAutoResponders.TabIndex = 11;
			this.lstAutoResponders.SelectedIndexChanged += new System.EventHandler(this.lstAutoResponders_SelectedIndexChanged);
			// 
			// btnARDelete
			// 
			this.btnARDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnARDelete.Location = new System.Drawing.Point(6, 154);
			this.btnARDelete.Name = "btnARDelete";
			this.btnARDelete.Size = new System.Drawing.Size(163, 23);
			this.btnARDelete.TabIndex = 15;
			this.btnARDelete.Text = "Delete AutoResponder";
			this.btnARDelete.UseVisualStyleBackColor = true;
			this.btnARDelete.Click += new System.EventHandler(this.btnARDelete_Click);
			// 
			// btnARAdd
			// 
			this.btnARAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnARAdd.Location = new System.Drawing.Point(7, 125);
			this.btnARAdd.Name = "btnARAdd";
			this.btnARAdd.Size = new System.Drawing.Size(163, 23);
			this.btnARAdd.TabIndex = 14;
			this.btnARAdd.Text = "Add AutoResponder";
			this.btnARAdd.UseVisualStyleBackColor = true;
			this.btnARAdd.Click += new System.EventHandler(this.btnARAdd_Click);
			// 
			// txtARParameters
			// 
			this.txtARParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtARParameters.ContextMenuStrip = this.contextMenu;
			this.txtARParameters.Location = new System.Drawing.Point(75, 73);
			this.txtARParameters.Name = "txtARParameters";
			this.txtARParameters.Size = new System.Drawing.Size(94, 20);
			this.txtARParameters.TabIndex = 12;
			this.txtARParameters.TextChanged += new System.EventHandler(this.txtARParameters_TextChanged);
			this.txtARParameters.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtARParameters_KeyUp);
			// 
			// lblARParameters
			// 
			this.lblARParameters.AutoSize = true;
			this.lblARParameters.Location = new System.Drawing.Point(6, 76);
			this.lblARParameters.Name = "lblARParameters";
			this.lblARParameters.Size = new System.Drawing.Size(63, 13);
			this.lblARParameters.TabIndex = 1;
			this.lblARParameters.Text = "Parameters:";
			// 
			// txtARCommandName
			// 
			this.txtARCommandName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtARCommandName.Location = new System.Drawing.Point(75, 47);
			this.txtARCommandName.Name = "txtARCommandName";
			this.txtARCommandName.Size = new System.Drawing.Size(94, 20);
			this.txtARCommandName.TabIndex = 11;
			this.txtARCommandName.TextChanged += new System.EventHandler(this.txtARCommandName_TextChanged);
			this.txtARCommandName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtARCommandName_KeyUp);
			// 
			// lblARCommandName
			// 
			this.lblARCommandName.AutoSize = true;
			this.lblARCommandName.Location = new System.Drawing.Point(6, 50);
			this.lblARCommandName.Name = "lblARCommandName";
			this.lblARCommandName.Size = new System.Drawing.Size(57, 13);
			this.lblARCommandName.TabIndex = 1;
			this.lblARCommandName.Text = "Command:";
			// 
			// lblARModule
			// 
			this.lblARModule.AutoSize = true;
			this.lblARModule.Location = new System.Drawing.Point(6, 23);
			this.lblARModule.Name = "lblARModule";
			this.lblARModule.Size = new System.Drawing.Size(45, 13);
			this.lblARModule.TabIndex = 1;
			this.lblARModule.Text = "Module:";
			// 
			// cmbARModule
			// 
			this.cmbARModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbARModule.FormattingEnabled = true;
			this.cmbARModule.Location = new System.Drawing.Point(75, 20);
			this.cmbARModule.Name = "cmbARModule";
			this.cmbARModule.Size = new System.Drawing.Size(94, 21);
			this.cmbARModule.TabIndex = 10;
			this.cmbARModule.SelectedIndexChanged += new System.EventHandler(this.cmbARModule_SelectedIndexChanged);
			this.cmbARModule.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmbARModule_KeyUp);
			this.cmbARModule.TextChanged += new System.EventHandler(this.cmbARModule_TextChanged);
			// 
			// gbPrompter
			// 
			this.gbPrompter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbPrompter.Controls.Add(this.lstPrompter);
			this.gbPrompter.Controls.Add(this.btnPrmptDelete);
			this.gbPrompter.Controls.Add(this.btnPrmptAdd);
			this.gbPrompter.Controls.Add(this.txtPrmptParameters);
			this.gbPrompter.Controls.Add(this.lblPrmptParameters);
			this.gbPrompter.Controls.Add(this.txtPrmptCommandName);
			this.gbPrompter.Controls.Add(this.lblPrmptCommandName);
			this.gbPrompter.Controls.Add(this.lblPrmptModule);
			this.gbPrompter.Controls.Add(this.cmbPrmptModule);
			this.gbPrompter.Location = new System.Drawing.Point(3, 3);
			this.gbPrompter.Name = "gbPrompter";
			this.gbPrompter.Size = new System.Drawing.Size(170, 367);
			this.gbPrompter.TabIndex = 2;
			this.gbPrompter.TabStop = false;
			this.gbPrompter.Text = "Prompters";
			// 
			// lstPrompter
			// 
			this.lstPrompter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstPrompter.FormattingEnabled = true;
			this.lstPrompter.Location = new System.Drawing.Point(6, 183);
			this.lstPrompter.Name = "lstPrompter";
			this.lstPrompter.Size = new System.Drawing.Size(158, 147);
			this.lstPrompter.TabIndex = 17;
			this.lstPrompter.SelectedIndexChanged += new System.EventHandler(this.lstPrompter_SelectedIndexChanged);
			// 
			// btnPrmptDelete
			// 
			this.btnPrmptDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnPrmptDelete.Location = new System.Drawing.Point(6, 154);
			this.btnPrmptDelete.Name = "btnPrmptDelete";
			this.btnPrmptDelete.Size = new System.Drawing.Size(158, 23);
			this.btnPrmptDelete.TabIndex = 24;
			this.btnPrmptDelete.Text = "Delete Prompter";
			this.btnPrmptDelete.UseVisualStyleBackColor = true;
			this.btnPrmptDelete.Click += new System.EventHandler(this.btnPrmptDelete_Click);
			// 
			// btnPrmptAdd
			// 
			this.btnPrmptAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnPrmptAdd.Location = new System.Drawing.Point(6, 125);
			this.btnPrmptAdd.Name = "btnPrmptAdd";
			this.btnPrmptAdd.Size = new System.Drawing.Size(158, 23);
			this.btnPrmptAdd.TabIndex = 23;
			this.btnPrmptAdd.Text = "Add Prompter";
			this.btnPrmptAdd.UseVisualStyleBackColor = true;
			this.btnPrmptAdd.Click += new System.EventHandler(this.btnPrmptAdd_Click);
			// 
			// txtPrmptParameters
			// 
			this.txtPrmptParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPrmptParameters.ContextMenuStrip = this.contextMenu;
			this.txtPrmptParameters.Enabled = false;
			this.txtPrmptParameters.Location = new System.Drawing.Point(75, 73);
			this.txtPrmptParameters.Name = "txtPrmptParameters";
			this.txtPrmptParameters.Size = new System.Drawing.Size(89, 20);
			this.txtPrmptParameters.TabIndex = 22;
			this.txtPrmptParameters.Visible = false;
			this.txtPrmptParameters.TextChanged += new System.EventHandler(this.txtPrmptParameters_TextChanged);
			this.txtPrmptParameters.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPrmptParameters_KeyUp);
			// 
			// lblPrmptParameters
			// 
			this.lblPrmptParameters.AutoSize = true;
			this.lblPrmptParameters.Enabled = false;
			this.lblPrmptParameters.Location = new System.Drawing.Point(6, 76);
			this.lblPrmptParameters.Name = "lblPrmptParameters";
			this.lblPrmptParameters.Size = new System.Drawing.Size(63, 13);
			this.lblPrmptParameters.TabIndex = 1;
			this.lblPrmptParameters.Text = "Parameters:";
			this.lblPrmptParameters.Visible = false;
			// 
			// txtPrmptCommandName
			// 
			this.txtPrmptCommandName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPrmptCommandName.Location = new System.Drawing.Point(75, 47);
			this.txtPrmptCommandName.Name = "txtPrmptCommandName";
			this.txtPrmptCommandName.Size = new System.Drawing.Size(89, 20);
			this.txtPrmptCommandName.TabIndex = 21;
			this.txtPrmptCommandName.TextChanged += new System.EventHandler(this.txtPrmptCommandName_TextChanged);
			this.txtPrmptCommandName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPrmptCommandName_KeyUp);
			// 
			// lblPrmptCommandName
			// 
			this.lblPrmptCommandName.AutoSize = true;
			this.lblPrmptCommandName.Location = new System.Drawing.Point(6, 50);
			this.lblPrmptCommandName.Name = "lblPrmptCommandName";
			this.lblPrmptCommandName.Size = new System.Drawing.Size(57, 13);
			this.lblPrmptCommandName.TabIndex = 1;
			this.lblPrmptCommandName.Text = "Command:";
			// 
			// lblPrmptModule
			// 
			this.lblPrmptModule.AutoSize = true;
			this.lblPrmptModule.Location = new System.Drawing.Point(6, 23);
			this.lblPrmptModule.Name = "lblPrmptModule";
			this.lblPrmptModule.Size = new System.Drawing.Size(45, 13);
			this.lblPrmptModule.TabIndex = 1;
			this.lblPrmptModule.Text = "Module:";
			// 
			// cmbPrmptModule
			// 
			this.cmbPrmptModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbPrmptModule.FormattingEnabled = true;
			this.cmbPrmptModule.Location = new System.Drawing.Point(75, 20);
			this.cmbPrmptModule.Name = "cmbPrmptModule";
			this.cmbPrmptModule.Size = new System.Drawing.Size(89, 21);
			this.cmbPrmptModule.TabIndex = 20;
			this.cmbPrmptModule.SelectedIndexChanged += new System.EventHandler(this.cmbPrmptModule_SelectedIndexChanged);
			this.cmbPrmptModule.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmbPrmptModule_KeyUp);
			this.cmbPrmptModule.TextChanged += new System.EventHandler(this.cmbPrmptModule_TextChanged);
			// 
			// gbConsole
			// 
			this.gbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbConsole.Controls.Add(this.txtConsole);
			this.gbConsole.Location = new System.Drawing.Point(193, 404);
			this.gbConsole.Name = "gbConsole";
			this.gbConsole.Size = new System.Drawing.Size(355, 103);
			this.gbConsole.TabIndex = 3;
			this.gbConsole.TabStop = false;
			this.gbConsole.Text = "Console";
			// 
			// txtConsole
			// 
			this.txtConsole.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtConsole.Location = new System.Drawing.Point(3, 16);
			this.txtConsole.Multiline = true;
			this.txtConsole.Name = "txtConsole";
			this.txtConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtConsole.Size = new System.Drawing.Size(349, 84);
			this.txtConsole.TabIndex = 100;
			this.txtConsole.TabStop = false;
			// 
			// bgwPerformanceTestWorker
			// 
			this.bgwPerformanceTestWorker.WorkerReportsProgress = true;
			this.bgwPerformanceTestWorker.WorkerSupportsCancellation = true;
			this.bgwPerformanceTestWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwPerformanceTestWorker_DoWork);
			this.bgwPerformanceTestWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgwPerformanceTestWorker_RunWorkerCompleted);
			this.bgwPerformanceTestWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgwPerformanceTestWorker_ProgressChanged);
			// 
			// tcTabs
			// 
			this.tcTabs.Controls.Add(this.tpAutoRespondersAndPrompters);
			this.tcTabs.Controls.Add(this.tpCommandAndPerformanceTest);
			this.tcTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcTabs.Location = new System.Drawing.Point(0, 0);
			this.tcTabs.Name = "tcTabs";
			this.tcTabs.SelectedIndex = 0;
			this.tcTabs.Size = new System.Drawing.Size(562, 516);
			this.tcTabs.TabIndex = 4;
			// 
			// tpAutoRespondersAndPrompters
			// 
			this.tpAutoRespondersAndPrompters.Controls.Add(this.scSplitter1);
			this.tpAutoRespondersAndPrompters.Location = new System.Drawing.Point(4, 22);
			this.tpAutoRespondersAndPrompters.Name = "tpAutoRespondersAndPrompters";
			this.tpAutoRespondersAndPrompters.Padding = new System.Windows.Forms.Padding(3);
			this.tpAutoRespondersAndPrompters.Size = new System.Drawing.Size(554, 490);
			this.tpAutoRespondersAndPrompters.TabIndex = 0;
			this.tpAutoRespondersAndPrompters.Text = "AutoResponders / Prompters";
			// 
			// scSplitter1
			// 
			this.scSplitter1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scSplitter1.Location = new System.Drawing.Point(187, 3);
			this.scSplitter1.Margin = new System.Windows.Forms.Padding(0);
			this.scSplitter1.Name = "scSplitter1";
			// 
			// scSplitter1.Panel1
			// 
			this.scSplitter1.Panel1.Controls.Add(this.gbAutoResponder);
			// 
			// scSplitter1.Panel2
			// 
			this.scSplitter1.Panel2.Controls.Add(this.gbPrompter);
			this.scSplitter1.Size = new System.Drawing.Size(362, 373);
			this.scSplitter1.SplitterDistance = 180;
			this.scSplitter1.TabIndex = 3;
			// 
			// tpCommandAndPerformanceTest
			// 
			this.tpCommandAndPerformanceTest.Controls.Add(this.scSplitter2);
			this.tpCommandAndPerformanceTest.Location = new System.Drawing.Point(4, 22);
			this.tpCommandAndPerformanceTest.Name = "tpCommandAndPerformanceTest";
			this.tpCommandAndPerformanceTest.Padding = new System.Windows.Forms.Padding(3);
			this.tpCommandAndPerformanceTest.Size = new System.Drawing.Size(554, 490);
			this.tpCommandAndPerformanceTest.TabIndex = 1;
			this.tpCommandAndPerformanceTest.Text = "Quick Commands / Performance Test";
			// 
			// scSplitter2
			// 
			this.scSplitter2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scSplitter2.Location = new System.Drawing.Point(187, 2);
			this.scSplitter2.Margin = new System.Windows.Forms.Padding(0);
			this.scSplitter2.Name = "scSplitter2";
			// 
			// scSplitter2.Panel1
			// 
			this.scSplitter2.Panel1.Controls.Add(this.gbQC);
			// 
			// scSplitter2.Panel2
			// 
			this.scSplitter2.Panel2.Controls.Add(this.gbPerformanceTest);
			this.scSplitter2.Size = new System.Drawing.Size(362, 374);
			this.scSplitter2.SplitterDistance = 180;
			this.scSplitter2.TabIndex = 5;
			// 
			// gbQC
			// 
			this.gbQC.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbQC.Controls.Add(this.lblQCId);
			this.gbQC.Controls.Add(this.idbQCCommandId);
			this.gbQC.Controls.Add(this.lstQuickCommands);
			this.gbQC.Controls.Add(this.btnQCDelete);
			this.gbQC.Controls.Add(this.btnQCAdd);
			this.gbQC.Controls.Add(this.txtQCParameters);
			this.gbQC.Controls.Add(this.lblQCParameters);
			this.gbQC.Controls.Add(this.txtQCCommandName);
			this.gbQC.Controls.Add(this.lblQCCommandName);
			this.gbQC.Controls.Add(this.lblQCModule);
			this.gbQC.Controls.Add(this.cmbQCModule);
			this.gbQC.Location = new System.Drawing.Point(3, 4);
			this.gbQC.Name = "gbQC";
			this.gbQC.Size = new System.Drawing.Size(174, 367);
			this.gbQC.TabIndex = 5;
			this.gbQC.TabStop = false;
			this.gbQC.Text = "Quick Commands";
			// 
			// lblQCId
			// 
			this.lblQCId.AutoSize = true;
			this.lblQCId.Location = new System.Drawing.Point(6, 101);
			this.lblQCId.Name = "lblQCId";
			this.lblQCId.Size = new System.Drawing.Size(69, 13);
			this.lblQCId.TabIndex = 12;
			this.lblQCId.Text = "Command Id:";
			// 
			// idbQCCommandId
			// 
			this.idbQCCommandId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.idbQCCommandId.Location = new System.Drawing.Point(75, 99);
			this.idbQCCommandId.Maximum = new decimal(new int[] {
			99,
			0,
			0,
			0});
			this.idbQCCommandId.Minimum = new decimal(new int[] {
			2,
			0,
			0,
			-2147483648});
			this.idbQCCommandId.Name = "idbQCCommandId";
			this.idbQCCommandId.Size = new System.Drawing.Size(93, 20);
			this.idbQCCommandId.TabIndex = 13;
			// 
			// lstQuickCommands
			// 
			this.lstQuickCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lstQuickCommands.FormattingEnabled = true;
			this.lstQuickCommands.Location = new System.Drawing.Point(6, 183);
			this.lstQuickCommands.Name = "lstQuickCommands";
			this.lstQuickCommands.Size = new System.Drawing.Size(162, 160);
			this.lstQuickCommands.TabIndex = 11;
			this.lstQuickCommands.SelectedIndexChanged += new System.EventHandler(this.lstQuickCommands_SelectedIndexChanged);
			this.lstQuickCommands.DoubleClick += new System.EventHandler(this.lstQuickCommands_DoubleClick);
			// 
			// btnQCDelete
			// 
			this.btnQCDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnQCDelete.Location = new System.Drawing.Point(6, 154);
			this.btnQCDelete.Name = "btnQCDelete";
			this.btnQCDelete.Size = new System.Drawing.Size(162, 23);
			this.btnQCDelete.TabIndex = 15;
			this.btnQCDelete.Text = "Delete Quick Command";
			this.btnQCDelete.UseVisualStyleBackColor = true;
			this.btnQCDelete.Click += new System.EventHandler(this.btnQCDelete_Click);
			// 
			// btnQCAdd
			// 
			this.btnQCAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnQCAdd.Location = new System.Drawing.Point(7, 125);
			this.btnQCAdd.Name = "btnQCAdd";
			this.btnQCAdd.Size = new System.Drawing.Size(162, 23);
			this.btnQCAdd.TabIndex = 14;
			this.btnQCAdd.Text = "Add Quick Command";
			this.btnQCAdd.UseVisualStyleBackColor = true;
			this.btnQCAdd.Click += new System.EventHandler(this.btnQCAdd_Click);
			// 
			// txtQCParameters
			// 
			this.txtQCParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtQCParameters.ContextMenuStrip = this.contextMenu;
			this.txtQCParameters.Location = new System.Drawing.Point(75, 73);
			this.txtQCParameters.Name = "txtQCParameters";
			this.txtQCParameters.Size = new System.Drawing.Size(93, 20);
			this.txtQCParameters.TabIndex = 12;
			this.txtQCParameters.TextChanged += new System.EventHandler(this.txtQCParameters_TextChanged);
			this.txtQCParameters.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtQCParameters_KeyUp);
			// 
			// lblQCParameters
			// 
			this.lblQCParameters.AutoSize = true;
			this.lblQCParameters.Location = new System.Drawing.Point(6, 76);
			this.lblQCParameters.Name = "lblQCParameters";
			this.lblQCParameters.Size = new System.Drawing.Size(63, 13);
			this.lblQCParameters.TabIndex = 1;
			this.lblQCParameters.Text = "Parameters:";
			// 
			// txtQCCommandName
			// 
			this.txtQCCommandName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtQCCommandName.Location = new System.Drawing.Point(75, 47);
			this.txtQCCommandName.Name = "txtQCCommandName";
			this.txtQCCommandName.Size = new System.Drawing.Size(93, 20);
			this.txtQCCommandName.TabIndex = 11;
			this.txtQCCommandName.TextChanged += new System.EventHandler(this.txtQCCommandName_TextChanged);
			this.txtQCCommandName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtQCCommandName_KeyUp);
			// 
			// lblQCCommandName
			// 
			this.lblQCCommandName.AutoSize = true;
			this.lblQCCommandName.Location = new System.Drawing.Point(6, 50);
			this.lblQCCommandName.Name = "lblQCCommandName";
			this.lblQCCommandName.Size = new System.Drawing.Size(57, 13);
			this.lblQCCommandName.TabIndex = 1;
			this.lblQCCommandName.Text = "Command:";
			// 
			// lblQCModule
			// 
			this.lblQCModule.AutoSize = true;
			this.lblQCModule.Location = new System.Drawing.Point(6, 23);
			this.lblQCModule.Name = "lblQCModule";
			this.lblQCModule.Size = new System.Drawing.Size(45, 13);
			this.lblQCModule.TabIndex = 1;
			this.lblQCModule.Text = "Module:";
			// 
			// cmbQCModule
			// 
			this.cmbQCModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbQCModule.FormattingEnabled = true;
			this.cmbQCModule.Location = new System.Drawing.Point(75, 20);
			this.cmbQCModule.Name = "cmbQCModule";
			this.cmbQCModule.Size = new System.Drawing.Size(93, 21);
			this.cmbQCModule.TabIndex = 10;
			this.cmbQCModule.SelectedIndexChanged += new System.EventHandler(this.cmbQCModule_SelectedIndexChanged);
			this.cmbQCModule.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmbQCModule_KeyUp);
			this.cmbQCModule.TextChanged += new System.EventHandler(this.cmbQCModule_TextChanged);
			// 
			// gbPerformanceTest
			// 
			this.gbPerformanceTest.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbPerformanceTest.Controls.Add(this.txtPerformanceTestSucceeded);
			this.gbPerformanceTest.Controls.Add(this.lblPerformanceTestSucceeded);
			this.gbPerformanceTest.Controls.Add(this.txtPerformanceTestElapsed);
			this.gbPerformanceTest.Controls.Add(this.lblPerformanceTestElapsed);
			this.gbPerformanceTest.Controls.Add(this.pbPerformanceTestProgress);
			this.gbPerformanceTest.Controls.Add(this.lblPerformanceTestTimeout);
			this.gbPerformanceTest.Controls.Add(this.lblPerformanceTestExecutions);
			this.gbPerformanceTest.Controls.Add(this.nudPerformanceTestTimeout);
			this.gbPerformanceTest.Controls.Add(this.nudPerformanceTestExecutions);
			this.gbPerformanceTest.Controls.Add(this.btnStartStopPerformanceTest);
			this.gbPerformanceTest.Controls.Add(this.txtPerformanceTestParameters);
			this.gbPerformanceTest.Controls.Add(this.lblPerformanceTestParameters);
			this.gbPerformanceTest.Controls.Add(this.txtPerformanceTestCommandName);
			this.gbPerformanceTest.Controls.Add(this.lblPerformanceTest);
			this.gbPerformanceTest.Controls.Add(this.lblPerformanceTestModule);
			this.gbPerformanceTest.Controls.Add(this.cmbPerformanceTestModule);
			this.gbPerformanceTest.Location = new System.Drawing.Point(3, 4);
			this.gbPerformanceTest.Name = "gbPerformanceTest";
			this.gbPerformanceTest.Size = new System.Drawing.Size(167, 367);
			this.gbPerformanceTest.TabIndex = 4;
			this.gbPerformanceTest.TabStop = false;
			this.gbPerformanceTest.Text = "PerformanceTest";
			// 
			// txtPerformanceTestSucceeded
			// 
			this.txtPerformanceTestSucceeded.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPerformanceTestSucceeded.Location = new System.Drawing.Point(75, 238);
			this.txtPerformanceTestSucceeded.Name = "txtPerformanceTestSucceeded";
			this.txtPerformanceTestSucceeded.ReadOnly = true;
			this.txtPerformanceTestSucceeded.Size = new System.Drawing.Size(86, 20);
			this.txtPerformanceTestSucceeded.TabIndex = 39;
			// 
			// lblPerformanceTestSucceeded
			// 
			this.lblPerformanceTestSucceeded.AutoSize = true;
			this.lblPerformanceTestSucceeded.Location = new System.Drawing.Point(6, 241);
			this.lblPerformanceTestSucceeded.Name = "lblPerformanceTestSucceeded";
			this.lblPerformanceTestSucceeded.Size = new System.Drawing.Size(65, 13);
			this.lblPerformanceTestSucceeded.TabIndex = 37;
			this.lblPerformanceTestSucceeded.Text = "Succeeded:";
			// 
			// txtPerformanceTestElapsed
			// 
			this.txtPerformanceTestElapsed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPerformanceTestElapsed.Location = new System.Drawing.Point(75, 212);
			this.txtPerformanceTestElapsed.Name = "txtPerformanceTestElapsed";
			this.txtPerformanceTestElapsed.ReadOnly = true;
			this.txtPerformanceTestElapsed.Size = new System.Drawing.Size(86, 20);
			this.txtPerformanceTestElapsed.TabIndex = 38;
			// 
			// lblPerformanceTestElapsed
			// 
			this.lblPerformanceTestElapsed.AutoSize = true;
			this.lblPerformanceTestElapsed.Location = new System.Drawing.Point(6, 215);
			this.lblPerformanceTestElapsed.Name = "lblPerformanceTestElapsed";
			this.lblPerformanceTestElapsed.Size = new System.Drawing.Size(48, 13);
			this.lblPerformanceTestElapsed.TabIndex = 37;
			this.lblPerformanceTestElapsed.Text = "Elapsed:";
			// 
			// pbPerformanceTestProgress
			// 
			this.pbPerformanceTestProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pbPerformanceTestProgress.Location = new System.Drawing.Point(6, 183);
			this.pbPerformanceTestProgress.Name = "pbPerformanceTestProgress";
			this.pbPerformanceTestProgress.Size = new System.Drawing.Size(155, 23);
			this.pbPerformanceTestProgress.TabIndex = 36;
			// 
			// lblPerformanceTestTimeout
			// 
			this.lblPerformanceTestTimeout.AutoSize = true;
			this.lblPerformanceTestTimeout.Location = new System.Drawing.Point(6, 127);
			this.lblPerformanceTestTimeout.Name = "lblPerformanceTestTimeout";
			this.lblPerformanceTestTimeout.Size = new System.Drawing.Size(48, 13);
			this.lblPerformanceTestTimeout.TabIndex = 17;
			this.lblPerformanceTestTimeout.Text = "Timeout:";
			// 
			// lblPerformanceTestExecutions
			// 
			this.lblPerformanceTestExecutions.AutoSize = true;
			this.lblPerformanceTestExecutions.Location = new System.Drawing.Point(6, 101);
			this.lblPerformanceTestExecutions.Name = "lblPerformanceTestExecutions";
			this.lblPerformanceTestExecutions.Size = new System.Drawing.Size(62, 13);
			this.lblPerformanceTestExecutions.TabIndex = 17;
			this.lblPerformanceTestExecutions.Text = "Executions:";
			// 
			// nudPerformanceTestTimeout
			// 
			this.nudPerformanceTestTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nudPerformanceTestTimeout.Location = new System.Drawing.Point(75, 125);
			this.nudPerformanceTestTimeout.Maximum = new decimal(new int[] {
			6000,
			0,
			0,
			0});
			this.nudPerformanceTestTimeout.Name = "nudPerformanceTestTimeout";
			this.nudPerformanceTestTimeout.Size = new System.Drawing.Size(86, 20);
			this.nudPerformanceTestTimeout.TabIndex = 34;
			this.nudPerformanceTestTimeout.Value = new decimal(new int[] {
			100,
			0,
			0,
			0});
			this.nudPerformanceTestTimeout.ValueChanged += new System.EventHandler(this.nudPerformanceTestTimeout_ValueChanged);
			// 
			// nudPerformanceTestExecutions
			// 
			this.nudPerformanceTestExecutions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.nudPerformanceTestExecutions.Location = new System.Drawing.Point(75, 99);
			this.nudPerformanceTestExecutions.Maximum = new decimal(new int[] {
			1000000,
			0,
			0,
			0});
			this.nudPerformanceTestExecutions.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.nudPerformanceTestExecutions.Name = "nudPerformanceTestExecutions";
			this.nudPerformanceTestExecutions.Size = new System.Drawing.Size(86, 20);
			this.nudPerformanceTestExecutions.TabIndex = 34;
			this.nudPerformanceTestExecutions.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.nudPerformanceTestExecutions.ValueChanged += new System.EventHandler(this.nudPerformanceTestExecutions_ValueChanged);
			// 
			// btnStartStopPerformanceTest
			// 
			this.btnStartStopPerformanceTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnStartStopPerformanceTest.Location = new System.Drawing.Point(6, 154);
			this.btnStartStopPerformanceTest.Name = "btnStartStopPerformanceTest";
			this.btnStartStopPerformanceTest.Size = new System.Drawing.Size(155, 23);
			this.btnStartStopPerformanceTest.TabIndex = 35;
			this.btnStartStopPerformanceTest.Text = "Start PerformanceTest";
			this.btnStartStopPerformanceTest.UseVisualStyleBackColor = true;
			this.btnStartStopPerformanceTest.Click += new System.EventHandler(this.btnStartStopPerformanceTest_Click);
			// 
			// txtPerformanceTestParameters
			// 
			this.txtPerformanceTestParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPerformanceTestParameters.ContextMenuStrip = this.contextMenu;
			this.txtPerformanceTestParameters.Location = new System.Drawing.Point(75, 73);
			this.txtPerformanceTestParameters.Name = "txtPerformanceTestParameters";
			this.txtPerformanceTestParameters.Size = new System.Drawing.Size(86, 20);
			this.txtPerformanceTestParameters.TabIndex = 32;
			this.txtPerformanceTestParameters.TextChanged += new System.EventHandler(this.txtPerformanceTestCommandName_TextChanged);
			this.txtPerformanceTestParameters.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPerformanceTestCmdParams_KeyUp);
			// 
			// lblPerformanceTestParameters
			// 
			this.lblPerformanceTestParameters.AutoSize = true;
			this.lblPerformanceTestParameters.Location = new System.Drawing.Point(6, 76);
			this.lblPerformanceTestParameters.Name = "lblPerformanceTestParameters";
			this.lblPerformanceTestParameters.Size = new System.Drawing.Size(63, 13);
			this.lblPerformanceTestParameters.TabIndex = 1;
			this.lblPerformanceTestParameters.Text = "Parameters:";
			// 
			// txtPerformanceTestCommandName
			// 
			this.txtPerformanceTestCommandName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtPerformanceTestCommandName.Location = new System.Drawing.Point(75, 47);
			this.txtPerformanceTestCommandName.Name = "txtPerformanceTestCommandName";
			this.txtPerformanceTestCommandName.Size = new System.Drawing.Size(86, 20);
			this.txtPerformanceTestCommandName.TabIndex = 31;
			this.txtPerformanceTestCommandName.TextChanged += new System.EventHandler(this.txtPerformanceTestCommandName_TextChanged);
			this.txtPerformanceTestCommandName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPerformanceTestCommandName_KeyUp);
			// 
			// lblPerformanceTest
			// 
			this.lblPerformanceTest.AutoSize = true;
			this.lblPerformanceTest.Location = new System.Drawing.Point(6, 50);
			this.lblPerformanceTest.Name = "lblPerformanceTest";
			this.lblPerformanceTest.Size = new System.Drawing.Size(57, 13);
			this.lblPerformanceTest.TabIndex = 1;
			this.lblPerformanceTest.Text = "Command:";
			// 
			// lblPerformanceTestModule
			// 
			this.lblPerformanceTestModule.AutoSize = true;
			this.lblPerformanceTestModule.Location = new System.Drawing.Point(6, 23);
			this.lblPerformanceTestModule.Name = "lblPerformanceTestModule";
			this.lblPerformanceTestModule.Size = new System.Drawing.Size(45, 13);
			this.lblPerformanceTestModule.TabIndex = 1;
			this.lblPerformanceTestModule.Text = "Module:";
			// 
			// cmbPerformanceTestModule
			// 
			this.cmbPerformanceTestModule.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbPerformanceTestModule.FormattingEnabled = true;
			this.cmbPerformanceTestModule.Location = new System.Drawing.Point(75, 20);
			this.cmbPerformanceTestModule.Name = "cmbPerformanceTestModule";
			this.cmbPerformanceTestModule.Size = new System.Drawing.Size(86, 21);
			this.cmbPerformanceTestModule.TabIndex = 30;
			this.cmbPerformanceTestModule.SelectedIndexChanged += new System.EventHandler(this.cmbPerformanceTestModule_SelectedIndexChanged);
			this.cmbPerformanceTestModule.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cmbPerformanceTestModule_KeyUp);
			this.cmbPerformanceTestModule.TextChanged += new System.EventHandler(this.cmbPerformanceTestModule_TextChanged);
			// 
			// toolStripContainer1
			// 
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.gbModules);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.gbAddModule);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.gbConsole);
			this.toolStripContainer1.ContentPanel.Controls.Add(this.tcTabs);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(562, 516);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(562, 516);
			this.toolStripContainer1.TabIndex = 4;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsiSerialize,
			this.tsiDeserialize});
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size(153, 70);
			// 
			// tsiSerialize
			// 
			this.tsiSerialize.Name = "tsiSerialize";
			this.tsiSerialize.Size = new System.Drawing.Size(152, 22);
			this.tsiSerialize.Text = "Serialize";
			this.tsiSerialize.Click += new System.EventHandler(this.tsiSerialize_Click);
			// 
			// tsiDeserialize
			// 
			this.tsiDeserialize.Name = "tsiDeserialize";
			this.tsiDeserialize.Size = new System.Drawing.Size(152, 22);
			this.tsiDeserialize.Text = "Deserialize";
			this.tsiDeserialize.Click += new System.EventHandler(this.tsiDeserialize_Click);
			// 
			// FrmTester
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(562, 516);
			this.Controls.Add(this.toolStripContainer1);
			this.MinimumSize = new System.Drawing.Size(489, 520);
			this.Name = "FrmTester";
			this.Text = "Tester";
			this.Load += new System.EventHandler(this.FrmTester_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTester_FormClosing);
			this.gbModules.ResumeLayout(false);
			this.gbAddModule.ResumeLayout(false);
			this.gbAddModule.PerformLayout();
			this.gbAutoResponder.ResumeLayout(false);
			this.gbAutoResponder.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudARFailureRate)).EndInit();
			this.gbPrompter.ResumeLayout(false);
			this.gbPrompter.PerformLayout();
			this.gbConsole.ResumeLayout(false);
			this.gbConsole.PerformLayout();
			this.tcTabs.ResumeLayout(false);
			this.tpAutoRespondersAndPrompters.ResumeLayout(false);
			this.scSplitter1.Panel1.ResumeLayout(false);
			this.scSplitter1.Panel2.ResumeLayout(false);
			this.scSplitter1.ResumeLayout(false);
			this.tpCommandAndPerformanceTest.ResumeLayout(false);
			this.scSplitter2.Panel1.ResumeLayout(false);
			this.scSplitter2.Panel2.ResumeLayout(false);
			this.scSplitter2.ResumeLayout(false);
			this.gbQC.ResumeLayout(false);
			this.gbQC.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.idbQCCommandId)).EndInit();
			this.gbPerformanceTest.ResumeLayout(false);
			this.gbPerformanceTest.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudPerformanceTestTimeout)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPerformanceTestExecutions)).EndInit();
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			this.contextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbModules;
		private System.Windows.Forms.ListBox lstModules;
		private System.Windows.Forms.Button btnModuleRemove;
		private System.Windows.Forms.Button btnModuleStartStop;
		private System.Windows.Forms.GroupBox gbAddModule;
		private System.Windows.Forms.TextBox txtModulePort;
		private System.Windows.Forms.Label lblModulePort;
		private System.Windows.Forms.Button btnAddModule;
		private System.Windows.Forms.Label lblModuleName;
		private System.Windows.Forms.TextBox txtModuleName;
		private System.Windows.Forms.GroupBox gbAutoResponder;
		private System.Windows.Forms.ComboBox cmbARModule;
		private System.Windows.Forms.TextBox txtARParameters;
		private System.Windows.Forms.Label lblARParameters;
		private System.Windows.Forms.TextBox txtARCommandName;
		private System.Windows.Forms.Label lblARCommandName;
		private System.Windows.Forms.Label lblARModule;
		private System.Windows.Forms.Button btnARAdd;
		private System.Windows.Forms.ListBox lstAutoResponders;
		private System.Windows.Forms.Button btnARDelete;
		private System.Windows.Forms.GroupBox gbPrompter;
		private System.Windows.Forms.ListBox lstPrompter;
		private System.Windows.Forms.Button btnPrmptDelete;
		private System.Windows.Forms.Button btnPrmptAdd;
		private System.Windows.Forms.TextBox txtPrmptParameters;
		private System.Windows.Forms.Label lblPrmptParameters;
		private System.Windows.Forms.TextBox txtPrmptCommandName;
		private System.Windows.Forms.Label lblPrmptCommandName;
		private System.Windows.Forms.Label lblPrmptModule;
		private System.Windows.Forms.ComboBox cmbPrmptModule;
		private System.Windows.Forms.GroupBox gbConsole;
		private System.Windows.Forms.TextBox txtConsole;
		private System.Windows.Forms.Label lblARFailureRate;
		private System.Windows.Forms.NumericUpDown nudARFailureRate;
		private System.ComponentModel.BackgroundWorker bgwPerformanceTestWorker;
		private System.Windows.Forms.TabControl tcTabs;
		private System.Windows.Forms.TabPage tpAutoRespondersAndPrompters;
		private System.Windows.Forms.TabPage tpCommandAndPerformanceTest;
		private System.Windows.Forms.SplitContainer scSplitter2;
		private System.Windows.Forms.SplitContainer scSplitter1;
		private System.Windows.Forms.GroupBox gbQC;
		private System.Windows.Forms.Label lblQCId;
		private Robotics.Controls.IdBox idbQCCommandId;
		private System.Windows.Forms.ListBox lstQuickCommands;
		private System.Windows.Forms.Button btnQCDelete;
		private System.Windows.Forms.Button btnQCAdd;
		private System.Windows.Forms.TextBox txtQCParameters;
		private System.Windows.Forms.Label lblQCParameters;
		private System.Windows.Forms.TextBox txtQCCommandName;
		private System.Windows.Forms.Label lblQCCommandName;
		private System.Windows.Forms.Label lblQCModule;
		private System.Windows.Forms.ComboBox cmbQCModule;
		private System.Windows.Forms.GroupBox gbPerformanceTest;
		private System.Windows.Forms.TextBox txtPerformanceTestSucceeded;
		private System.Windows.Forms.Label lblPerformanceTestSucceeded;
		private System.Windows.Forms.TextBox txtPerformanceTestElapsed;
		private System.Windows.Forms.Label lblPerformanceTestElapsed;
		private System.Windows.Forms.ProgressBar pbPerformanceTestProgress;
		private System.Windows.Forms.Label lblPerformanceTestTimeout;
		private System.Windows.Forms.Label lblPerformanceTestExecutions;
		private System.Windows.Forms.NumericUpDown nudPerformanceTestTimeout;
		private System.Windows.Forms.NumericUpDown nudPerformanceTestExecutions;
		private System.Windows.Forms.Button btnStartStopPerformanceTest;
		private System.Windows.Forms.TextBox txtPerformanceTestParameters;
		private System.Windows.Forms.Label lblPerformanceTestParameters;
		private System.Windows.Forms.TextBox txtPerformanceTestCommandName;
		private System.Windows.Forms.Label lblPerformanceTest;
		private System.Windows.Forms.Label lblPerformanceTestModule;
		private System.Windows.Forms.ComboBox cmbPerformanceTestModule;
		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem tsiSerialize;
		private System.Windows.Forms.ToolStripMenuItem tsiDeserialize;
	}
}

