using System;
using System.Drawing;
using System.Windows.Forms;

namespace Robotics.Controls
{
	/// <summary>
	/// Implements a simple control to show video in a WinForm
	/// </summary>
	public partial class VideoControl : System.Windows.Forms.Panel
	{

		#region Variables

		/// <summary>
		/// Image to display
		/// </summary>
		protected Bitmap image;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a nre instance of VideoControl
		/// </summary>
		public VideoControl()
		{
			InitializeComponent();
			this.image = null;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the image displayed by the control
		/// </summary>
		public Bitmap Image
		{
			get { return image; }
			set
			{
				if (value == null)
					return;
				Bitmap lastImage= this.image;
				this.image = value;
				this.Invalidate();
				
				if (lastImage != null)
					lastImage.Dispose();
			}
		}

		#endregion

		#region Methods

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer, true);
			//this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}

		#endregion

		#region Overided Methods

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

		/// <summary>
		/// Overrided. Draws the control contents
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains the event data</param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			if (image != null)
			{
				g.DrawImage(image, 0, 0, this.Width, this.Height);
				if ((image.Height > this.Width) || (image.Width > this.Height))
					g.ScaleTransform((float)this.Width / (float)image.Width, (float)this.Height / (float)image.Height);
			}

			base.OnPaint(e);
		}

		/// <summary>
		/// Overrided. Paints the backgrund of the control
		/// </summary>
		/// <param name="e">A PaintEventArgs that contains the event data</param>
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			Rectangle rec = e.ClipRectangle;
			if (image != null)
			{
				Rectangle rec1 = new Rectangle(image.Width, 0, rec.Width - image.Width, rec.Height);
				Rectangle rec2 = new Rectangle(0, image.Height, image.Width, rec.Height - image.Height);
				using (Brush br = new SolidBrush(this.BackColor))
				{
					e.Graphics.FillRectangle(br, rec1);
					e.Graphics.FillRectangle(br, rec2);
				}
			}
			else
			{
				base.OnPaintBackground(e);
			}
		}

		#endregion
	}
}
