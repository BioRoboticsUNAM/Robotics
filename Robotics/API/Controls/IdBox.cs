using System;
using System.Windows.Forms;
using System.Text;

namespace Robotics.Controls
{
	/// <summary>
	/// Implements the basic functionality required by a spin box (also known as an up-down control)modified for use with numeric Id
	/// </summary>
	public class IdBox : NumericUpDown
	{
		/// <summary>
		/// Initializes a new instance of the IdBox class.
		/// </summary>
		public IdBox() : base()
		{
			base.Maximum = 99;
			base.Minimum = -2;
		}

		/// <summary>
		/// Raises the KeyDown event. 
		/// </summary>
		/// <param name="e">A KeyEventArgs that contains the event data.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if ((e.KeyValue == 'A') || (e.KeyValue == 'a'))
			{
				Value = -2;
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyValue == 'N') || (e.KeyValue == 'n'))
			{
				Value = -1;
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			else
				base.OnKeyDown(e);
		}

		/// <summary>
		/// When overridden in a derived class, updates the text displayed in the spin box (also known as an up-down control).
		/// </summary>
		protected override void UpdateEditText()
		{
			if (Value < -1)
				Text = "Auto";
			else if (Value < 0)
				Text = "None";
			else
				base.UpdateEditText();
		}
	}
}
