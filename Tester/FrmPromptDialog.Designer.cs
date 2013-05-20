namespace Tester
{
	partial class FrmPromptDialog
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
			this.lblCaption = new System.Windows.Forms.Label();
			this.txtParams = new System.Windows.Forms.TextBox();
			this.lblCommand = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSucceeded = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnFailed = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblCaption
			// 
			this.lblCaption.AutoSize = true;
			this.lblCaption.Location = new System.Drawing.Point(12, 9);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.Size = new System.Drawing.Size(196, 13);
			this.lblCaption.TabIndex = 0;
			this.lblCaption.Text = "A response is required for the command:";
			// 
			// txtParams
			// 
			this.txtParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtParams.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtParams.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.txtParams.Location = new System.Drawing.Point(12, 67);
			this.txtParams.Name = "txtParams";
			this.txtParams.Size = new System.Drawing.Size(318, 20);
			this.txtParams.TabIndex = 1;
			// 
			// lblCommand
			// 
			this.lblCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblCommand.Location = new System.Drawing.Point(12, 25);
			this.lblCommand.Margin = new System.Windows.Forms.Padding(3);
			this.lblCommand.Name = "lblCommand";
			this.lblCommand.Size = new System.Drawing.Size(318, 23);
			this.lblCommand.TabIndex = 2;
			this.lblCommand.Text = "command \"parameters\" @1";
			this.lblCommand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(247, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Please enter the parameters used for the response:";
			// 
			// btnSucceeded
			// 
			this.btnSucceeded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSucceeded.Location = new System.Drawing.Point(230, 93);
			this.btnSucceeded.Name = "btnSucceeded";
			this.btnSucceeded.Size = new System.Drawing.Size(100, 23);
			this.btnSucceeded.TabIndex = 3;
			this.btnSucceeded.Text = "Succeeded";
			this.btnSucceeded.UseVisualStyleBackColor = true;
			this.btnSucceeded.Click += new System.EventHandler(this.btnSucceeded_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(12, 93);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnFailed
			// 
			this.btnFailed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFailed.Location = new System.Drawing.Point(149, 93);
			this.btnFailed.Name = "btnFailed";
			this.btnFailed.Size = new System.Drawing.Size(75, 23);
			this.btnFailed.TabIndex = 3;
			this.btnFailed.Text = "Failed";
			this.btnFailed.UseVisualStyleBackColor = true;
			this.btnFailed.Click += new System.EventHandler(this.btnFailed_Click);
			// 
			// FrmPromptDialog
			// 
			this.AcceptButton = this.btnSucceeded;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(342, 126);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnFailed);
			this.Controls.Add(this.btnSucceeded);
			this.Controls.Add(this.lblCommand);
			this.Controls.Add(this.txtParams);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblCaption);
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(350, 160);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(350, 160);
			this.Name = "FrmPromptDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Command Arrived";
			this.TopMost = true;
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FrmPromptDialog_KeyUp);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblCaption;
		private System.Windows.Forms.TextBox txtParams;
		private System.Windows.Forms.Label lblCommand;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSucceeded;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnFailed;
	}
}