namespace SharedVariableInspector
{
	partial class FrmInspector
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
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.lvSharedVariableList = new System.Windows.Forms.ListView();
			this.gbSharedVariableList = new System.Windows.Forms.GroupBox();
			this.gbSharedVariableProperties = new System.Windows.Forms.GroupBox();
			this.pgSharedVarProperties = new System.Windows.Forms.PropertyGrid();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.tsCommandBar = new System.Windows.Forms.ToolStrip();
			this.tsbRefreshSharedVariableList = new System.Windows.Forms.ToolStripButton();
			this.tsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsbUpdateSharedVariableInfo = new System.Windows.Forms.ToolStripButton();
			this.tsbUpdateSharedVariableValue = new System.Windows.Forms.ToolStripButton();
			this.tsbSharedVariableSubscriptionReport = new System.Windows.Forms.ToolStripButton();
			this.tsbSharedVariableSubscriptionContent = new System.Windows.Forms.ToolStripButton();
			this.statusStrip1.SuspendLayout();
			this.gbSharedVariableList.SuspendLayout();
			this.gbSharedVariableProperties.SuspendLayout();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.tsCommandBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 387);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(614, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// lblStatus
			// 
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(59, 17);
			this.lblStatus.Text = "Loading...";
			// 
			// lvSharedVariableList
			// 
			this.lvSharedVariableList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvSharedVariableList.Location = new System.Drawing.Point(3, 16);
			this.lvSharedVariableList.Name = "lvSharedVariableList";
			this.lvSharedVariableList.Size = new System.Drawing.Size(344, 343);
			this.lvSharedVariableList.TabIndex = 1;
			this.lvSharedVariableList.UseCompatibleStateImageBehavior = false;
			this.lvSharedVariableList.View = System.Windows.Forms.View.Details;
			this.lvSharedVariableList.SelectedIndexChanged += new System.EventHandler(this.lvSharedVariableList_SelectedIndexChanged);
			// 
			// gbSharedVariableList
			// 
			this.gbSharedVariableList.Controls.Add(this.lvSharedVariableList);
			this.gbSharedVariableList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSharedVariableList.Location = new System.Drawing.Point(0, 0);
			this.gbSharedVariableList.Name = "gbSharedVariableList";
			this.gbSharedVariableList.Size = new System.Drawing.Size(350, 362);
			this.gbSharedVariableList.TabIndex = 2;
			this.gbSharedVariableList.TabStop = false;
			this.gbSharedVariableList.Text = "Shared Variable List";
			// 
			// gbSharedVariableProperties
			// 
			this.gbSharedVariableProperties.Controls.Add(this.pgSharedVarProperties);
			this.gbSharedVariableProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSharedVariableProperties.Location = new System.Drawing.Point(0, 0);
			this.gbSharedVariableProperties.Name = "gbSharedVariableProperties";
			this.gbSharedVariableProperties.Size = new System.Drawing.Size(260, 362);
			this.gbSharedVariableProperties.TabIndex = 3;
			this.gbSharedVariableProperties.TabStop = false;
			this.gbSharedVariableProperties.Text = "Shared Variable Properties";
			// 
			// pgSharedVarProperties
			// 
			this.pgSharedVarProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pgSharedVarProperties.Location = new System.Drawing.Point(6, 19);
			this.pgSharedVarProperties.Name = "pgSharedVarProperties";
			this.pgSharedVarProperties.Size = new System.Drawing.Size(248, 337);
			this.pgSharedVarProperties.TabIndex = 0;
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 25);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.gbSharedVariableList);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.gbSharedVariableProperties);
			this.splitContainer.Size = new System.Drawing.Size(614, 362);
			this.splitContainer.SplitterDistance = 350;
			this.splitContainer.TabIndex = 4;
			// 
			// tsCommandBar
			// 
			this.tsCommandBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbRefreshSharedVariableList,
            this.tsSeparator1,
            this.tsbUpdateSharedVariableInfo,
            this.tsbUpdateSharedVariableValue,
            this.tsbSharedVariableSubscriptionReport,
            this.tsbSharedVariableSubscriptionContent});
			this.tsCommandBar.Location = new System.Drawing.Point(0, 0);
			this.tsCommandBar.Name = "tsCommandBar";
			this.tsCommandBar.Size = new System.Drawing.Size(614, 25);
			this.tsCommandBar.TabIndex = 2;
			this.tsCommandBar.Text = "toolStrip1";
			// 
			// tsbRefreshSharedVariableList
			// 
			this.tsbRefreshSharedVariableList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbRefreshSharedVariableList.Image = global::SharedVariableInspector.Properties.Resources.Recycle;
			this.tsbRefreshSharedVariableList.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbRefreshSharedVariableList.Name = "tsbRefreshSharedVariableList";
			this.tsbRefreshSharedVariableList.Size = new System.Drawing.Size(23, 22);
			this.tsbRefreshSharedVariableList.Text = "Refresh list";
			this.tsbRefreshSharedVariableList.ToolTipText = "Refresh shared variable list";
			this.tsbRefreshSharedVariableList.Click += new System.EventHandler(this.tsbRefreshSharedVariableList_Click);
			// 
			// tsSeparator1
			// 
			this.tsSeparator1.Name = "tsSeparator1";
			this.tsSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// tsbUpdateSharedVariableInfo
			// 
			this.tsbUpdateSharedVariableInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbUpdateSharedVariableInfo.Enabled = false;
			this.tsbUpdateSharedVariableInfo.Image = global::SharedVariableInspector.Properties.Resources.CircleInfo;
			this.tsbUpdateSharedVariableInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbUpdateSharedVariableInfo.Name = "tsbUpdateSharedVariableInfo";
			this.tsbUpdateSharedVariableInfo.Size = new System.Drawing.Size(23, 22);
			this.tsbUpdateSharedVariableInfo.Text = "Update SharedVariable Info";
			this.tsbUpdateSharedVariableInfo.ToolTipText = "Update SharedVariable Information";
			this.tsbUpdateSharedVariableInfo.Click += new System.EventHandler(this.tsbUpdateSharedVariableInfo_Click);
			// 
			// tsbUpdateSharedVariableValue
			// 
			this.tsbUpdateSharedVariableValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbUpdateSharedVariableValue.Enabled = false;
			this.tsbUpdateSharedVariableValue.Image = global::SharedVariableInspector.Properties.Resources.Refresh;
			this.tsbUpdateSharedVariableValue.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbUpdateSharedVariableValue.Name = "tsbUpdateSharedVariableValue";
			this.tsbUpdateSharedVariableValue.Size = new System.Drawing.Size(23, 22);
			this.tsbUpdateSharedVariableValue.Text = "Update variable value";
			this.tsbUpdateSharedVariableValue.ToolTipText = "Update variable value";
			this.tsbUpdateSharedVariableValue.Click += new System.EventHandler(this.tsbUpdateSharedVariableValue_Click);
			// 
			// tsbSharedVariableSubscriptionReport
			// 
			this.tsbSharedVariableSubscriptionReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSharedVariableSubscriptionReport.Enabled = false;
			this.tsbSharedVariableSubscriptionReport.Image = global::SharedVariableInspector.Properties.Resources.ShoppingBasket;
			this.tsbSharedVariableSubscriptionReport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSharedVariableSubscriptionReport.Name = "tsbSharedVariableSubscriptionReport";
			this.tsbSharedVariableSubscriptionReport.Size = new System.Drawing.Size(23, 22);
			this.tsbSharedVariableSubscriptionReport.Tag = "Subscription report mode";
			this.tsbSharedVariableSubscriptionReport.Text = "Subscription by send-report-only mode";
			this.tsbSharedVariableSubscriptionReport.Click += new System.EventHandler(this.tsbSharedVariableSubscriptionReport_Click);
			// 
			// tsbSharedVariableSubscriptionContent
			// 
			this.tsbSharedVariableSubscriptionContent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbSharedVariableSubscriptionContent.Enabled = false;
			this.tsbSharedVariableSubscriptionContent.Image = global::SharedVariableInspector.Properties.Resources.ShoppingBasketFull;
			this.tsbSharedVariableSubscriptionContent.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbSharedVariableSubscriptionContent.Name = "tsbSharedVariableSubscriptionContent";
			this.tsbSharedVariableSubscriptionContent.Size = new System.Drawing.Size(23, 22);
			this.tsbSharedVariableSubscriptionContent.Tag = "Subscription content mode";
			this.tsbSharedVariableSubscriptionContent.Text = "Subscription by send-content mode";
			this.tsbSharedVariableSubscriptionContent.Click += new System.EventHandler(this.tsbSharedVariableSubscriptionContent_Click);
			// 
			// FrmInspector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(614, 409);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.tsCommandBar);
			this.Controls.Add(this.statusStrip1);
			this.Icon = global::SharedVariableInspector.Properties.Resources.Inpector;
			this.Name = "FrmInspector";
			this.Text = "Shared Variable Inspector";
			this.Load += new System.EventHandler(this.FrmInspector_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.gbSharedVariableList.ResumeLayout(false);
			this.gbSharedVariableProperties.ResumeLayout(false);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.tsCommandBar.ResumeLayout(false);
			this.tsCommandBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel lblStatus;
		private System.Windows.Forms.ListView lvSharedVariableList;
		private System.Windows.Forms.GroupBox gbSharedVariableList;
		private System.Windows.Forms.GroupBox gbSharedVariableProperties;
		private System.Windows.Forms.PropertyGrid pgSharedVarProperties;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.ToolStrip tsCommandBar;
		private System.Windows.Forms.ToolStripButton tsbRefreshSharedVariableList;
		private System.Windows.Forms.ToolStripButton tsbUpdateSharedVariableValue;
		private System.Windows.Forms.ToolStripSeparator tsSeparator1;
		private System.Windows.Forms.ToolStripButton tsbSharedVariableSubscriptionReport;
		private System.Windows.Forms.ToolStripButton tsbSharedVariableSubscriptionContent;
		private System.Windows.Forms.ToolStripButton tsbUpdateSharedVariableInfo;
	}
}

