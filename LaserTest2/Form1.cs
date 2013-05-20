using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Robotics.HAL.Sensors;
using Robotics.HAL.Sensors.Telemetric;

namespace LaserLib
{
	public partial class Form1 : Form
	{
		byte[] rxBuffer;
		//int ix = 0;
		Laser laser;
		int lecturesPerSecond = 0;
		bool started;
		//PerformanceCounter cpuCounter;
		bool tresholdExceded;

		public Form1()
		{
			InitializeComponent();
			rxBuffer = new byte[1024];
			Bitmap b = new Bitmap(500, 500);
			laser = HokuyoLaser.FindUSBLasers[0];
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//laser = Laser.FindLaser;
			if (laser == null)
			{
				MessageBox.Show("Laser is not connected");
				Application.Exit();
				Application.ExitThread();
			}
			else
			{
				laser.ReadCompleted += new Robotics.HAL.Sensors.SensorReadingCompletedEventHandler<Laser, LaserReading>(laser_ReadCompleted);
				laser.TresholdExceeded += new TelemetricSensorThresholdExceededEventHandler<Laser>(laser_TresholdExceeded);
				laser.Error += new Robotics.HAL.Sensors.SensorErrorEventHandler<Laser>(laser_Error);
				//laser.Density = 32;

				laser.Connect();
				//laser.MotorSpeed = 2;
				laser.Treshold = 300;
				VersionInformation();
				this.Text = "Laser" + laser.MinimumAngle.ToString() + " / " + laser.MaximumAngle.ToString();
				laser.Start();
				timer.Enabled = true;
				timer.Start();
				started = true;
			}
		}

		void laser_Error(Laser sensor, ISensorError error)
		{
			this.BeginInvoke(new EventHandler(delegate(object sender, EventArgs e1)
			{
				txtRaw.AppendText(error.ErrorDescription);
			}));
		}

		void laser_TresholdExceeded(Laser l)
		{
			tresholdExceded = true;
		}

		void laser_ReadCompleted(Laser sensor, LaserReading[] read)
		{
			tresholdExceded = false;
			try
			{
				++lecturesPerSecond;
				//return;
				if (read != null)
					this.BeginInvoke(new EventHandler(updatePB), new object[] { read, null });
			}
			catch { }		
		}

		private void updatePB(object sender, EventArgs e)
		{
			Bitmap b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			Graphics g = Graphics.FromImage(b);
			LaserReading[] reading = (LaserReading[]) sender;
			PointF origin = new Point(b.Width / 2, b.Height / 2);
			PointF destiny;
			int i;
			float x, y, xScale, yScale;
			StringBuilder sb = new StringBuilder(4096);
			List<int> mistaken = new List<int>(300);

			//xmax = 7 * b.Width / 16;
			//ymax = 7 * b.Height / 16;
			xScale = ((b.Width / 2) / (float)laser.MaximumDistance);
			yScale = ((b.Height / 2) / (float)laser.MaximumDistance);
			g.Clear(Color.White);
			
			for (i = 0; i < reading.Length; ++i)
			{
				if (reading[i].Mistaken)
				{
					mistaken.Add(i);
					//sb.Append("Lecture ");
					//sb.Append(i);
					//sb.Append(" [");
					//sb.Append(lecture[i].Degrees.ToString("0.00"));
					//sb.Append("]: ");
					//sb.Append(lecture[i].Error.ToString());
					//sb.Append(" (error ");
					//sb.Append((int)lecture[i].Error);
					//sb.Append(")\r\n");
					if (!chkShowErr.Checked) continue;
				}
				
				x = (float)(Math.Cos(reading[i].Angle + Math.PI/2) * reading[i].Distance/laser.MaximumDistance);
				x = origin.X + x * xScale;
				y = (float)(Math.Sin(reading[i].Angle + Math.PI / 2) * -reading[i].Distance / laser.MaximumDistance);
				y = origin.X + y * yScale;
				destiny = new PointF(x, y);
				if (reading[i].Angle>Math.PI/4)
				g.DrawLine(Pens.Red, origin, destiny);
				else if(reading[i].Angle<-Math.PI/4)
					g.DrawLine(Pens.Blue, origin, destiny);
				else
					g.DrawLine(Pens.Green, origin, destiny);
			}
			pictureBox1.Image = b;
			return;
			sb.Append(laser.ErrorCount);
			sb.Append(" errors found\r\n\r\n\r\n");
			for (i = 0; i < mistaken.Count; ++i)
			{
				sb.Append(mistaken[i]);
				sb.Append(",");
			}
			txtErrors.Clear();
			txtErrors.AppendText(sb.ToString());
			//txtRaw.AppendText(laser.LastLectureRaw.Replace('\n', ' ').Trim() + "\r\n");

		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (started)
			{
				started = false;
				laser.Stop();
				laser.Disconnect();
				//laser = null;
				return;
			}
			//if (laser != null)
			else
			{
				laser.Connect();
				//laser.MotorSpeed = 2;
				laser.Treshold = 300;
				VersionInformation();
				this.Text = "Laser" + laser.MinimumAngle.ToString() + " / " + laser.MaximumAngle.ToString();
				LaserReading[] reading;
				laser.Read(out reading);
				laser_ReadCompleted(laser, reading);
				laser.Start();
				timer.Enabled = true;
				timer.Start();
				started = true;
			}
		}

		private void VersionInformation()
		{
			TextBox textBox2 = this.txtRaw;
			DeviceInfo di = laser.Information;
			textBox2.AppendText("Firmware: " + di.FirmwareVersion + "\r\n");
			textBox2.AppendText("Product: " +di.ProductName+ "\r\n");
			textBox2.AppendText("Protocol:" +di.ProtocolVersion+ "\r\n");
			textBox2.AppendText("S/N: " +di.SerialNumber+ "\r\n");
			textBox2.AppendText("Vendor:" +di.VendorName+ "\r\n");
		}

		private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{
			this.Invoke(new EventHandler(updateConsole));
		}

		private void updateConsole(object sender, EventArgs e)
		{
			//textBox1.AppendText("[Received]\t" + format(rxBuffer, 0, ix-1));
			//ix = 0;
			byte[] rx = new byte[1];
			textBox2.AppendText(format(rx, 0, 1) + " ");
		}

		private string format(byte[] data)
		{
			return format(data, 0, data.Length);
		}

		private string format(byte[] data, int offset, int length)
		{
			string s = "";
			int i;

			for(i = offset; i < offset + length; ++i)
			{
				if (
					((data[i] >= 'a') && (data[i] <= 'z')) ||
					((data[i] >= 'A') && (data[i] <= 'Z')) ||
					((data[i] >= '0') && (data[i] <= '9'))
					)
					s += Convert.ToString((char)data[i]) + " ";
				else
					s += "0x0" + Convert.ToString(data[i], 16).ToUpper() + " ";
			}
			return s;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(laser != null)
				laser.Stop();
		}

		static int ix;

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
			return;
			
			Laser laser;
			System.Threading.Timer t = new System.Threading.Timer(new System.Threading.TimerCallback(t1));

			laser = HokuyoLaser.FindUSBLasers[0];
			if (laser == null)
			{
				Console.WriteLine("Laser is not connected");
				Console.ReadKey();
				return;
			}
			laser.Connect();
			t.Change(0, 2);
			string s = "";
			DateTime ini = DateTime.Now;
			for (int i = 0; i < 10; ++i)
			{
				ix = 0;
				LaserReading[] readings;
				laser.Read(out readings);
				s+=ix.ToString() + "\r\n";
			}
			DateTime fin = DateTime.Now;
			Console.WriteLine(s + (fin -ini));
			Console.ReadKey();
			//laser.Interval = 500;
			//laser.Density = 32;
		}

		static void t1(object o)
		{
			ix+=2;
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			this.Text = "Laser Test - " + lecturesPerSecond.ToString() + "lps" + (tresholdExceded ? " T" : "");
			lecturesPerSecond = 0;
		}
	}
}