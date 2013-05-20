namespace TestTcp
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
			this.btnServerStart = new System.Windows.Forms.Button();
			this.txtServerPort = new System.Windows.Forms.TextBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.lblServerPort = new System.Windows.Forms.Label();
			this.txtServerConsole = new System.Windows.Forms.TextBox();
			this.txtServerInput = new System.Windows.Forms.TextBox();
			this.btnServerSend = new System.Windows.Forms.Button();
			this.txtClientIp = new System.Windows.Forms.TextBox();
			this.lblClientIp = new System.Windows.Forms.Label();
			this.lblClientPort = new System.Windows.Forms.Label();
			this.txtClientPort = new System.Windows.Forms.TextBox();
			this.txtClientConsole = new System.Windows.Forms.TextBox();
			this.btnClientConnect = new System.Windows.Forms.Button();
			this.txtClientInput = new System.Windows.Forms.TextBox();
			this.btnClientSend = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnServerStart
			// 
			this.btnServerStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnServerStart.Location = new System.Drawing.Point(465, 3);
			this.btnServerStart.Name = "btnServerStart";
			this.btnServerStart.Size = new System.Drawing.Size(75, 23);
			this.btnServerStart.TabIndex = 0;
			this.btnServerStart.Text = "Start";
			this.btnServerStart.UseVisualStyleBackColor = true;
			this.btnServerStart.Click += new System.EventHandler(this.btnServerStart_Click);
			// 
			// txtServerPort
			// 
			this.txtServerPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerPort.AutoCompleteCustomSource.AddRange(new string[] {
			"2000",
			"2300"});
			this.txtServerPort.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtServerPort.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtServerPort.Location = new System.Drawing.Point(390, 5);
			this.txtServerPort.Name = "txtServerPort";
			this.txtServerPort.Size = new System.Drawing.Size(69, 20);
			this.txtServerPort.TabIndex = 1;
			this.txtServerPort.Text = "2000";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lblServerPort);
			this.splitContainer1.Panel1.Controls.Add(this.txtServerConsole);
			this.splitContainer1.Panel1.Controls.Add(this.txtServerInput);
			this.splitContainer1.Panel1.Controls.Add(this.txtServerPort);
			this.splitContainer1.Panel1.Controls.Add(this.btnServerSend);
			this.splitContainer1.Panel1.Controls.Add(this.btnServerStart);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.txtClientIp);
			this.splitContainer1.Panel2.Controls.Add(this.lblClientIp);
			this.splitContainer1.Panel2.Controls.Add(this.lblClientPort);
			this.splitContainer1.Panel2.Controls.Add(this.txtClientPort);
			this.splitContainer1.Panel2.Controls.Add(this.txtClientConsole);
			this.splitContainer1.Panel2.Controls.Add(this.btnClientConnect);
			this.splitContainer1.Panel2.Controls.Add(this.txtClientInput);
			this.splitContainer1.Panel2.Controls.Add(this.btnClientSend);
			this.splitContainer1.Size = new System.Drawing.Size(552, 504);
			this.splitContainer1.SplitterDistance = 252;
			this.splitContainer1.TabIndex = 2;
			// 
			// lblServerPort
			// 
			this.lblServerPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblServerPort.AutoSize = true;
			this.lblServerPort.Location = new System.Drawing.Point(324, 8);
			this.lblServerPort.Name = "lblServerPort";
			this.lblServerPort.Size = new System.Drawing.Size(57, 13);
			this.lblServerPort.TabIndex = 3;
			this.lblServerPort.Text = "Listen Port";
			// 
			// txtServerConsole
			// 
			this.txtServerConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerConsole.Enabled = false;
			this.txtServerConsole.Location = new System.Drawing.Point(13, 32);
			this.txtServerConsole.Multiline = true;
			this.txtServerConsole.Name = "txtServerConsole";
			this.txtServerConsole.Size = new System.Drawing.Size(527, 188);
			this.txtServerConsole.TabIndex = 2;
			// 
			// txtServerInput
			// 
			this.txtServerInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerInput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtServerInput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtServerInput.Enabled = false;
			this.txtServerInput.Location = new System.Drawing.Point(12, 228);
			this.txtServerInput.Name = "txtServerInput";
			this.txtServerInput.Size = new System.Drawing.Size(447, 20);
			this.txtServerInput.TabIndex = 1;
			this.txtServerInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
			// 
			// btnServerSend
			// 
			this.btnServerSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnServerSend.Enabled = false;
			this.btnServerSend.Location = new System.Drawing.Point(465, 226);
			this.btnServerSend.Name = "btnServerSend";
			this.btnServerSend.Size = new System.Drawing.Size(75, 23);
			this.btnServerSend.TabIndex = 0;
			this.btnServerSend.Text = "Send";
			this.btnServerSend.UseVisualStyleBackColor = true;
			this.btnServerSend.Click += new System.EventHandler(this.btnServerSend_Click);
			// 
			// txtClientIp
			// 
			this.txtClientIp.AutoCompleteCustomSource.AddRange(new string[] {
			"127.0.0.1",
			"192.168.1.1",
			"192.168.190.100"});
			this.txtClientIp.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtClientIp.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtClientIp.Location = new System.Drawing.Point(69, 5);
			this.txtClientIp.Name = "txtClientIp";
			this.txtClientIp.Size = new System.Drawing.Size(226, 20);
			this.txtClientIp.TabIndex = 4;
			this.txtClientIp.Text = "127.0.0.1";
			// 
			// lblClientIp
			// 
			this.lblClientIp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblClientIp.AutoSize = true;
			this.lblClientIp.Location = new System.Drawing.Point(12, 8);
			this.lblClientIp.Name = "lblClientIp";
			this.lblClientIp.Size = new System.Drawing.Size(51, 13);
			this.lblClientIp.TabIndex = 3;
			this.lblClientIp.Text = "Server IP";
			// 
			// lblClientPort
			// 
			this.lblClientPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblClientPort.AutoSize = true;
			this.lblClientPort.Location = new System.Drawing.Point(301, 8);
			this.lblClientPort.Name = "lblClientPort";
			this.lblClientPort.Size = new System.Drawing.Size(83, 13);
			this.lblClientPort.TabIndex = 3;
			this.lblClientPort.Text = "Connection Port";
			// 
			// txtClientPort
			// 
			this.txtClientPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtClientPort.AutoCompleteCustomSource.AddRange(new string[] {
			"2000",
			"2011",
			"2052",
			"2080",
			"2090",
			"2300"});
			this.txtClientPort.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtClientPort.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtClientPort.Location = new System.Drawing.Point(390, 5);
			this.txtClientPort.Name = "txtClientPort";
			this.txtClientPort.Size = new System.Drawing.Size(69, 20);
			this.txtClientPort.TabIndex = 1;
			this.txtClientPort.Text = "2000";
			// 
			// txtClientConsole
			// 
			this.txtClientConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtClientConsole.Enabled = false;
			this.txtClientConsole.Location = new System.Drawing.Point(13, 31);
			this.txtClientConsole.Multiline = true;
			this.txtClientConsole.Name = "txtClientConsole";
			this.txtClientConsole.Size = new System.Drawing.Size(527, 176);
			this.txtClientConsole.TabIndex = 2;
			// 
			// btnClientConnect
			// 
			this.btnClientConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClientConnect.Location = new System.Drawing.Point(465, 3);
			this.btnClientConnect.Name = "btnClientConnect";
			this.btnClientConnect.Size = new System.Drawing.Size(75, 23);
			this.btnClientConnect.TabIndex = 0;
			this.btnClientConnect.Text = "Connect";
			this.btnClientConnect.UseVisualStyleBackColor = true;
			this.btnClientConnect.Click += new System.EventHandler(this.btnClientConnect_Click);
			// 
			// txtClientInput
			// 
			this.txtClientInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtClientInput.AutoCompleteCustomSource.AddRange(new string[] {
			"find_object \"all\" @1"});
			this.txtClientInput.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtClientInput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtClientInput.Enabled = false;
			this.txtClientInput.Location = new System.Drawing.Point(13, 215);
			this.txtClientInput.Name = "txtClientInput";
			this.txtClientInput.Size = new System.Drawing.Size(447, 20);
			this.txtClientInput.TabIndex = 1;
			this.txtClientInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
			// 
			// btnClientSend
			// 
			this.btnClientSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClientSend.Enabled = false;
			this.btnClientSend.Location = new System.Drawing.Point(465, 213);
			this.btnClientSend.Name = "btnClientSend";
			this.btnClientSend.Size = new System.Drawing.Size(75, 23);
			this.btnClientSend.TabIndex = 0;
			this.btnClientSend.Text = "Send";
			this.btnClientSend.UseVisualStyleBackColor = true;
			this.btnClientSend.Click += new System.EventHandler(this.btnClientSend_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(552, 504);
			this.Controls.Add(this.splitContainer1);
			this.Name = "Form1";
			this.Text = "TestTCP";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnServerStart;
		private System.Windows.Forms.TextBox txtServerPort;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TextBox txtServerConsole;
		private System.Windows.Forms.TextBox txtServerInput;
		private System.Windows.Forms.Button btnServerSend;
		private System.Windows.Forms.Label lblServerPort;
		private System.Windows.Forms.TextBox txtClientIp;
		private System.Windows.Forms.Label lblClientIp;
		private System.Windows.Forms.Label lblClientPort;
		private System.Windows.Forms.TextBox txtClientPort;
		private System.Windows.Forms.TextBox txtClientConsole;
		private System.Windows.Forms.Button btnClientConnect;
		private System.Windows.Forms.TextBox txtClientInput;
		private System.Windows.Forms.Button btnClientSend;
	}
}

