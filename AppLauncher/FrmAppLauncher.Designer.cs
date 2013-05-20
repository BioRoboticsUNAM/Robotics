namespace Robotics.AppLauncher
{
	partial class FrmAppLauncher
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
			this.icon = new System.Windows.Forms.NotifyIcon(this.components);
			this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.showHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menu.SuspendLayout();
			this.SuspendLayout();
			// 
			// icon
			// 
			this.icon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.icon.BalloonTipTitle = "AppLauncher Help";
			this.icon.ContextMenuStrip = this.menu;
			this.icon.Text = "AppLauncher";
			this.icon.Visible = true;
			// 
			// menu
			// 
			this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.showHelpToolStripMenuItem,
			this.toolStripMenuItem1,
			this.exitToolStripMenuItem});
			this.menu.Name = "menu";
			this.menu.Size = new System.Drawing.Size(125, 54);
			this.menu.Text = "AppLauncher";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(89, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
			this.exitToolStripMenuItem.Text = "&Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// toolTip
			// 
			this.toolTip.AutoPopDelay = 20000;
			this.toolTip.InitialDelay = 500;
			this.toolTip.ReshowDelay = 100;
			this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.toolTip.ToolTipTitle = "AppLauncher Help";
			// 
			// showHelpToolStripMenuItem
			// 
			this.showHelpToolStripMenuItem.Name = "showHelpToolStripMenuItem";
			this.showHelpToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.showHelpToolStripMenuItem.Text = "Show &Help";
			this.showHelpToolStripMenuItem.Click += new System.EventHandler(this.showHelpToolStripMenuItem_Click);
			// 
			// FrmAppLauncher
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.MinimizeBox = false;
			this.Name = "FrmAppLauncher";
			this.ShowInTaskbar = false;
			this.Text = "AppLauncher";
			this.Load += new System.EventHandler(this.FrmAppLauncher_Load);
			this.Shown += new System.EventHandler(this.FrmAppLauncher_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAppLauncher_FormClosing);
			this.Resize += new System.EventHandler(this.FrmAppLauncher_Resize);
			this.menu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon icon;
		private System.Windows.Forms.ContextMenuStrip menu;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ToolStripMenuItem showHelpToolStripMenuItem;
	}
}

