using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Robotics.API;

namespace Tester
{
	public partial class FrmPromptDialog : Form
	{
		private Command command;
		private bool result;

		public FrmPromptDialog()
		{
			InitializeComponent();
			result =false;
		}

		public Command Command
		{
			get { return command; }
			set
			{
				if (value == null) throw new ArgumentNullException();
				command = value;
				txtParams.Text = command.Parameters;
				lblCommand.Text = command.ToString();
			}
		}

		public Response Response
		{
			get
			{
				Response response;
				if (this.DialogResult == DialogResult.OK)
				{
					response = Response.CreateFromCommand(command, result);
					response.Parameters = txtParams.Text;
				}
				else
					response = Response.CreateFromCommand(command, false);
				return response;
			}
		}

		public void AddAutoComplete(string[] items)
		{
			if(items == null)
				return;
			for (int i = 0; i < items.Length; ++i)
			{
				if (txtParams.AutoCompleteCustomSource.Contains(items[i]))
					continue;
				txtParams.AutoCompleteCustomSource.Add(items[i]);
			}
		}

		public void AddAutoComplete(string item)
		{
			if (txtParams.AutoCompleteCustomSource.Contains(item))
				return;
			txtParams.AutoCompleteCustomSource.Add(item);
		}

		private void FrmPromptDialog_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					btnSucceeded.PerformClick();
					break;

				case Keys.Escape:
					btnCancel.PerformClick();
					break;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnSucceeded_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.result = true;
			this.Close();
		}

		private void btnFailed_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.result = false;
			this.Close();
		}
	}
}