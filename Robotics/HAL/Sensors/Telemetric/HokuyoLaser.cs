using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Threading;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Enumerates sensivity modes of the laser
	/// </summary>
	public enum HokuyoLaserSensitivity
	{
		/// <summary>
		/// Hight Laser sensivity
		/// </summary>
		HighSensitivity = 1,
		/// <summary>
		/// Normal Laser sensivity
		/// </summary>
		NormalSensitivity = 0
	};

	/// <summary>
	/// Enumerates comunication baudrates for comunicate with the laser
	/// </summary>
	public enum HokuyoLaserComSpeed
	{
		/// <summary>
		/// 19200bps
		/// </summary>
		B019K2,
		/// <summary>
		/// 57600bps
		/// </summary>
		B057K6,
		/// <summary>
		/// 115200bps
		/// </summary>
		B115K2,
		/// <summary>
		/// 250kbps
		/// </summary>
		B250K,
		/// <summary>
		/// 500kbps
		/// </summary>
		B500K,
		/// <summary>
		/// 750kbps
		/// </summary>
		B750K
	};

	/// <summary>
	/// Interfaces with a Hokuyo Laser range finder
	/// </summary>
	public class HokuyoLaser : Laser
	{
		#region Variables

		/// <summary>
		/// The serial port where the Laser Device is connected
		/// </summary>
		protected SerialPort serialPort;

		/// <summary>
		/// Sensitivity of the Laser device
		/// </summary>
		protected HokuyoLaserSensitivity sensivity = HokuyoLaserSensitivity.NormalSensitivity;

		/// <summary>
		/// Proximity treshold
		/// </summary>
		protected int threshold;

		/// <summary>
		/// Flag that indicates that there is data in the SerialPort
		/// </summary>
		private bool dataAvailiable;

		/// <summary>
		/// Speed of the serial port
		/// </summary>
		protected HokuyoLaserComSpeed comSpeed = HokuyoLaserComSpeed.B019K2;

		/// <summary>
		/// Last lecture in RAW
		/// </summary>
		protected string lastRaw = "";

		/// <summary>
		/// Last taken reading
		/// </summary>
		protected LaserReading[] lastReading;

		/// <summary>
		/// Laser device motor speed
		/// </summary>
		protected int motorSpeed;

		/// <summary>
		/// Stores the Cos value for the angle at specified measurement step
		/// </summary>
		private double[] cosByStep;

		/// <summary>
		/// Stores the Sin value for the angle at specified measurement step
		/// </summary>
		private double[] sinByStep;


		#region Laser specs

		/// <summary>
		/// Absolute first step
		/// </summary>
		protected int step0 = 0;
		/// <summary>
		/// First Step of the Measurement Range
		/// </summary>
		protected int stepA = 44;			// First Step of the Measurement Range 
		/// <summary>
		/// Step number on the sensor's front axis
		/// </summary>
		protected int stepB = 384;			// Step number on the sensor’s front axis
		/// <summary>
		/// Last Step of the Measurement Range
		/// </summary>
		protected int stepC = 725;			// Last Step of the Measurement Range
		/// <summary>
		/// Absolute last step
		/// </summary>
		protected int stepD = 768;			// Absolute last step
		/// <summary>
		/// Minimum Measurement [mm]
		/// </summary>
		protected int minMeasurement;		// Minimum Measurement [mm]
		/// <summary>
		/// Maximum Measurement [mm]
		/// </summary>
		protected int maxMeasurement;		// Maximum Measurement [mm]
		/// <summary>
		/// Total Number of Steps in 360º range
		/// </summary>
		protected int ares;					// Total Number of Steps in 360º range
		/// <summary>
		/// Standard motor speed [rpm]
		/// </summary>
		protected int standardSpeed;		// Standard motor speed [rpm]

		#endregion

		#endregion

		#region Constructors

		/// <summary>
		/// Retrieves a laser object attached o the first laser device found
		/// </summary>
		public HokuyoLaser()
			: base()
		{
			running = false;
			serialPort = new SerialPort();
			serialPort.BaudRate = 19200;
			serialPort.Handshake = Handshake.None;
			serialPort.Parity = Parity.None;
			serialPort.NewLine = "\n";
			serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
			density = stepC - stepA;
			threshold = -1;
		}

		/// <summary>
		/// Creates a new instance of Laser class
		/// </summary>
		/// <param name="PortName">COM port where the laser is attached</param>
		public HokuyoLaser(string PortName)
			: this()
		{
			serialPort.PortName = PortName;
		}

		/// <summary>
		/// Destructor. Releases resources and closes the serial port if open
		/// </summary>
		~HokuyoLaser()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}

		#endregion

		#region Events

		#endregion

		#region Properties

		/// <summary>
		/// Gets the Absolute last step the device can reach.
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the Absolute last step the device can reach")]
		public override int AbsoluteMaximumAngularStep
		{
			get { return this.stepD; }
		}

		/// <summary>
		/// Gets the smallest angle change the device can detect or rotate. Returns 2Pi / 1024
		/// </summary>
		[CategoryAttribute("Hardware Information")]
		[DescriptionAttribute("Gets the smallest angle change the device can detect or rotate. Returns 2Pi / 1024")]
		public override double AngularResolution
		{
			get
			{
				// Returns 2 * Pi / 1024
				return 0.0061359231515425649188723503579678;
			}
		}

		/// <summary>
		/// Gets the angle resollution bits. Always returns 10
		/// </summary>
		[CategoryAttribute("Hardware Information")]
		[DescriptionAttribute("Gets the angle resollution bits. Always returns 10")]
		public override int AngularResolutionBits
		{
			get { return 10; }
		}

		/// <summary>
		/// Step number on the sensor's front axis
		/// </summary>
		[CategoryAttribute("Hardware Information")]
		[DescriptionAttribute("Step number on the sensor's front axis")]
		public override int AngularStepZero
		{
			get { return this.stepB; }
		}

		/// <summary>
		/// Gets a value indicating the open or close status of the Laser port
		/// </summary>
		[CategoryAttribute("Device Status")]
		[DescriptionAttribute("Gets a value indicating the open or close status of the Laser port")]
		public override bool IsOpen
		{
			get { return serialPort.IsOpen; }

		}

		/// <summary>
		/// Gets the last reading array obtained from the sensor
		/// </summary>
		[CategoryAttribute("Device Status")]
		[DescriptionAttribute("Gets the last reading array obtained from the sensor")]
		public override LaserReading[] LastReadings
		{
			get { return this.lastReading; }
		}

		/// <summary>
		/// Gets the maximum angle in radians the sensor can detect measured from the front of the sensor
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the maximum angle in radians the sensor can detect measured from the front of the sensor")]
		public override double MaximumAngle
		{
			get { return (stepB - stepC) * -2 * Math.PI / ares; }
		}

		/// <summary>
		/// Gets the maximum distance the sensor can detect
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the maximum distance the sensor can detect")]
		public override double MaximumDistance
		{
			get { return this.maxMeasurement / 1000.0; }
		}

		/// <summary>
		/// Gets the minumim distance the sensor can detect
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the minumim distance the sensor can detect")]
		public override double MinimumDistance
		{
			get { return this.minMeasurement / 1000.0; }
		}

		/// <summary>
		/// Gets the minumim angle in radians the sensor can detect measured from the front of the sensor
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the minumim angle in radians the sensor can detect measured from the front of the sensor")]
		public override double MinimumAngle
		{
			get { return (stepB - stepA) * -2 * Math.PI / ares; }
		}

		/// <summary>
		/// Gets the name of the port where the Hokuyo Laser is connected
		/// </summary>
		[CategoryAttribute("Device Status")]
		[DescriptionAttribute("Gets the name of the port where the Hokuyo Laser is connected")]
		public string PortName
		{
			get { return serialPort.PortName; }
		}

		/// <summary>
		/// Gets a value indicating if the continous asynchronous read operation of the sensor has been started
		/// </summary>
		[CategoryAttribute("Device Status")]
		[DescriptionAttribute("Gets a value indicating if the continous asynchronous read operation of the sensor has been started")]
		public override bool Started
		{
			get { return this.running; }
		}

		/// <summary>
		/// Gets the number of steps in a complete revolution (360º or two pi radians).
		/// </summary>
		[CategoryAttribute("Hardware Information")]
		[DescriptionAttribute("Gets the number of steps in a complete revolution (360º or two pi radians).")]
		public override int StepsPerRevolution
		{
			get { return this.ares; }
		}

		/// <summary>
		/// Gets the First Step of the Measurement Range 
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the First Step of the Measurement Range")]
		public override int ValidMinimumAngularStep
		{
			get { return this.stepA; }
		}

		/// <summary>
		/// Gets the Last Step of the Measurement Range 
		/// </summary>
		[CategoryAttribute("Hardware Capabilities")]
		[DescriptionAttribute("Gets the Last Step of the Measurement Range")]
		public override int ValidMaximumAngularStep
		{
			get { return this.stepC; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Checks if port exists and is availiable for reading
		/// </summary>
		/// <returns>true if port exists and can be opened, false otherwise</returns>
		protected virtual bool CheckPortAvailiability()
		{
			bool flag = false;
			if (serialPort == null)
				return false;

			List<string> portNames = new List<string>(SerialPort.GetPortNames());
			if (!portNames.Contains(serialPort.PortName))
				return false;

			flag = true;
			try
			{
				if (!serialPort.IsOpen)
					serialPort.Open();
			}
			catch { flag = false; }

			try
			{
				if (serialPort.IsOpen)
					serialPort.Close();
			}
			catch { flag = false; }
			return flag;
		}

		/// <summary>
		/// Starts communication with the laser
		/// </summary>
		public override void Connect()
		{
			if (serialPort.IsOpen) return;
			if (!serialPort.IsOpen)
				serialPort.Open();

			int stat;
			// Wait untill device stop sending data
			do
			{
				serialPort.DiscardInBuffer();
				serialPort.DiscardOutBuffer();
				Thread.Sleep(10);
			}
			while (serialPort.BytesToRead > 0);
			sendCommand("SCIP2.0", "", out stat);
			if ((stat != 0) && (stat != 14))
			{
				// Try again but stopping the device first
				sendCommand("RS", "", out stat);
				serialPort.DiscardInBuffer();
				serialPort.DiscardOutBuffer();
				sendCommand("SCIP2.0", "", out stat);
				if ((stat != 0) && (stat != 14))
				{
					serialPort.Close();
					throw new Exception("Unsupported device");
					//return;
				}
			}
			VersionInformation();
		}

		/// <summary>
		/// When overriden release allocated resources.
		/// </summary>
		/// <param name="disposing">Indicates if Dispose() method (true) was called or it is called by the Garbage Collector (false)</param>
		protected override void Dispose(bool disposing)
		{
			lock (this)
			{
				Disposing = true;
				try
				{
					sendCommand("QT", "");
				}
				catch { }
				running = false;
				try
				{
					if (mainThread != null)
					{
						mainThread.Join(100);
						if (mainThread.IsAlive)
							mainThread.Abort();
						mainThread = null;
					}
				}
				catch { }
				if (serialPort != null)
				{
					lock (serialPort)
					{
						try
						{
							if (serialPort.IsOpen)
							{
								serialPort.DiscardInBuffer();
								serialPort.DiscardOutBuffer();
								serialPort.Close();
							}
							serialPort.Dispose();
							serialPort = null;
						}
						catch { }
					}
				}
				IsDisposed = true;
				Disposing = false;
			}
		}

		/// <summary>
		/// Stops communication with the laser
		/// </summary>
		public override void Disconnect()
		{
			if (running)
				base.Stop();
			if (serialPort.IsOpen)
				serialPort.Close();
		}

		/// <summary>
		/// Decrypts an integer from a byte array
		/// </summary>
		/// <param name="data">Array of data to decript</param>
		/// <returns>Integer value represented by the encrypted data</returns>
		protected int Decrypt(byte[] data)
		{
			int result = 0;
			int i;

			if (data.Length > 4) return -1;
			for (i = 0; i < data.Length; ++i)
			{
				result *= 64;
				result += data[i] - 0x30;
			}
			return result;
		}

		/// <summary>
		/// Decrypts an integer from a byte array
		/// </summary>
		/// <param name="data">Array of data to decript</param>
		/// <param name="offset">a zero-based offset where to start the decryption</param>
		/// <param name="count">Number of bytes to decrypt</param>
		/// <returns>Integer value represented by the encrypted data</returns>
		protected int Decrypt(byte[] data, int offset, int count)
		{
			int result = 0;
			int i;

			if (count > 4) return -1;
			for (i = 0; i < count; ++i)
			{
				result *= 64;
				result += data[offset + i] - 0x30;
			}
			return result;
		}

		/// <summary>
		/// Gets the cosine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the cosine is desired</param>
		/// <returns>The cosine value of the angle at provided step.</returns>
		public override double GetCosFromStep(int step)
		{
			if ((step < 0) || (step > ares))
				throw new ArgumentOutOfRangeException();
			if (cosByStep == null)
				throw new HokuyoLaserException("Uninitialized device. Make sure device is connected and Connect() method has been called.");
			return cosByStep[step];
		}

		/// <summary>
		/// Gets the response for an MD command
		/// </summary>
		/// <param name="remainingScans">The number of scans remaining of the MD command requested</param>
		/// <returns>The data readed from the laser. Null if the data is not congruent</returns>
		protected byte[] GetMDResponse(ref int remainingScans)
		{
			string result;
			string[] lines;
			MemoryStream buffer;
			byte[] sBuffer;
			int i, j;
			Int16 sum;
			byte[] data = null;

			// Get the result

			try
			{
				result = spReadLine();
				lastRaw = (string)result.Clone();
				lines = result.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				// get remaining
				if ((lines[0].Length == 15) && (lines[0].StartsWith("MD")))
					remainingScans = Int32.Parse(lines[0].Substring(13));
				if ((lines.Length < 4) || (lines[1] != "99b"))
					return null;
			}
			catch { return null; }

			// Fill the buffer and check sums
			buffer = new MemoryStream(result.Length);
			sBuffer = new byte[1];
			for (i = 3; i < lines.Length; ++i)
			{
				sum = 0;
				for (j = 0; j < lines[i].Length - 1; ++j)
				{
					sBuffer[0] = (byte)lines[i][j];
					buffer.Write(sBuffer, 0, 1);
					unchecked { sum += sBuffer[0]; }
				}
				sum &= 0x00003F;
				sum += 0x30;
				if (sum != lines[i][j]) return null;
			}
			// Check if the data is congruent
			if ((buffer.Length % 3) != 0)
				return null;

			// Get data from buffer
			data = buffer.ToArray();
			buffer.Close();
			return data;
		}

		/// <summary>
		/// Gets the sine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the sine is desired</param>
		/// <returns>The sine value of the angle at provided step.</returns>
		public override double GetSinFromStep(int step)
		{
			if ((step < 0) || (step > ares))
				throw new ArgumentOutOfRangeException();
			if (sinByStep == null)
				throw new HokuyoLaserException("Uninitialized device. Make sure device is connected and Connect() method has been called.");
			return sinByStep[step];
		}

		/// <summary>
		/// Checks if the provided command is valid
		/// </summary>
		/// <param name="command">The command to check</param>
		/// <returns>true if the command is recognized by the device, false otherwise</returns>
		protected bool IsValidCommand(string command)
		{
			//if (command.Length != 2) return false;
			switch (command)
			{
				case "SCIP2.0":
				case "MD":
				case "MS":
				case "GD":
				case "GS":
				case "BM":
				case "QT":
				case "RS":
				case "TM":
				case "SS":
				case "CR":
				case "HS":
				case "DB":
				case "VV":
				case "PP":
				case "II":
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Performs the asynchronous device reading operation
		/// </summary>
		protected override void MainThreadTask()
		{
			bool validatePort = false;
			int stat = -1;
			int tries = 0;
			int remaining;
			LaserReading[] readings;
			short clusterCount = 0;
			int hangCounter = 0;
			double minDistance = MaximumDistance;
			double maxDistance = MinimumDistance;

			running = true;

			// Request reset the laser
			#region Request reset the laser
			do
			{
				++tries;
				try
				{
					sendCommand("QT", "", out stat);
					if (stat == 1)
						OnError(new HokuyoLaserError("Laser malfunction during initialization stop (" + tries.ToString() + " of 3"));
					sendCommand("RS", "", out stat);
					if (stat == 1)
						OnError(new HokuyoLaserError("Laser malfunction during initialization reset (" + tries.ToString() + " of 3"));
				}
				catch { OnError(new HokuyoLaserError("Laser malfunction while initialization reset (" + tries.ToString() + " of 3")); }
			} while (running && (tries < 3) && (stat != 0) && (stat != 2));
			#endregion

			// Request turn on laser
			#region Request turn on laser
			do
			{
				++tries;
				try
				{
					sendCommand("BM", "", out stat);
					if (stat == 1)
						OnError(new HokuyoLaserError("Laser malfunction while turning on (" + tries.ToString() + " of 3"));
				}
				catch { OnError(new HokuyoLaserError("Laser malfunction while turning on (" + tries.ToString() + " of 3")); }
			} while (running && (tries < 3) && (stat != 0) && (stat != 2));
			#endregion

			#region Start Event Thread
			//if((eventThread != null) && eventThread.IsAlive)
			//	eventThread.Abort();
			//eventThread = new Thread(new ThreadStart(EventThreadTask));
			//eventThread.IsBackground = true;
			////eventThread.Priority = ThreadPriority.BelowNormal;
			//eventThread.Start();

			#endregion

			#region Scan
			remaining = 0;
			while (running)
			{
				if (validatePort)
				{
					while (running && !(validatePort = CheckPortAvailiability()))
						Thread.Sleep(1000);
					try
					{
						sendCommand("RS", "", out stat);
						sendCommand("BM", "", out stat);
					}
					catch { }
				}
				try
				{
					// Request continous (99) measuring
					if (((hangCounter > 10) || (remaining <= 0)) && !SendCointinousMD(99, out clusterCount))
						OnError(new HokuyoLaserError("Cannot send MD command"));

					minDistance = MaximumDistance;
					maxDistance = MinimumDistance;
					if (ParseMDResponse(clusterCount, out readings, out minDistance, out maxDistance, ref remaining))
					{
						hangCounter = 0;
						this.lastReading = readings;
						OnReadCompleted(readings);
						if (minDistance <= threshold)
							OnTresholdExceeded();
					}
					else
					{
						++hangCounter;
						Thread.Sleep(10);
					}
				}
				catch (InvalidOperationException)
				{
					validatePort = CheckPortAvailiability();
					if (validatePort)
						RequestStopLaser();
					continue;
				}
				catch (TimeoutException)
				{
					validatePort = CheckPortAvailiability();
					if (validatePort)
						RequestStopLaser();
					continue;
				}
				catch (ThreadAbortException taex)
				{
					taex.ToString();
					RequestStopLaser();
					break;
				}
				catch
				{
					continue;
				}
				//while (running) Thread.Sleep(1);
			}

			#endregion

			// Request stop laser
			#region Request stop laser
			RequestStopLaser();
			#endregion

			running = false;
		}

		/// <summary>
		/// Extract the readings of a MD response sent by the Hokuyo Laser device
		/// </summary>
		/// <param name="clusterCount">The number of cluster count</param>
		/// <param name="readings">When this method returns contains the array of laser readings</param>
		/// <param name="remainingScans">When this method returns contains the number of remaining scans</param>
		/// <returns>true if data was parsed successfully, false otherwise</returns>
		protected bool ParseMDResponse(short clusterCount, out LaserReading[] readings, ref int remainingScans)
		{
			double minDistance;
			double maxDistance;
			return ParseMDResponse(clusterCount, out readings, out minDistance, out maxDistance, ref remainingScans);
		}

		/// <summary>
		/// Extract the readings of a MD response sent by the Hokuyo Laser device
		/// </summary>
		/// <param name="clusterCount">The number of cluster count</param>
		/// <param name="readings">When this method returns contains the array of laser readings</param>
		/// <param name="minDistance">When this method returns contains the minimum distance measured</param>
		/// <param name="maxDistance">When this method returns contains the maximum distance measured</param>
		/// <param name="remainingScans">When this method returns contains the number of remaining scans</param>
		/// <returns>true if data was parsed successfully, false otherwise</returns>
		protected bool ParseMDResponse(short clusterCount, out LaserReading[] readings, out double minDistance, out double maxDistance, ref int remainingScans)
		{
			int i, j;
			byte[] data;
			double distance;
			double angle;
			double angleStep;

			// First assign
			readings = null;
			minDistance = Int32.MaxValue;
			maxDistance = Int32.MinValue;

			// Get the result
			data = GetMDResponse(ref remainingScans);
			if (data == null)
				return false;
			readings = new LaserReading[data.Length / 3];

			// Prepare conversion
			if (clusterCount != 0)
				angleStep = clusterCount * this.AngularResolution;
			else
				angleStep = this.AngularResolution;


			for (i = 0, j = 0; j < data.Length; ++i, j += 3)
			{
				angle = (step0 + i - stepB) * angleStep;
				distance = Decrypt(data, j, 3) / 1000.0;
				if ((distance < this.MinimumDistance) || (distance > this.MaximumDistance))
					readings[i] = new LaserReading(this, angle, this.MaximumDistance, true);
				else
				{
					readings[i] = new LaserReading(this, angle, distance, false);
					if (distance < minDistance)
						minDistance = distance;
					if (distance > maxDistance)
						maxDistance = distance;
				}
			}

			return true;
		}

		/// <summary>
		/// Syncronusly reads the Hokuyo Laser sensor
		/// </summary>
		/// <param name="readings">When this method returns contains the array of sensor readings if the sensor was readed successfully, null otherwise</param>
		/// <returns>true if read from the sensor was completed successfully, false otherwise</returns>
		public override bool Read(out ITelemetricReading[] readings)
		{
			LaserReading[] laserReadings;
			bool result = Read(out laserReadings);
			readings = laserReadings;
			return result;
		}

		/// <summary>
		/// Syncronusly reads the Hokuyo Laser sensor
		/// </summary>
		/// <param name="readings">When this method returns contains the array of Hokuyo Laser sensor readings if the sensor was readed successfully, null otherwise</param>
		/// <returns>true if read from the sensor was completed successfully, false otherwise</returns>
		public override bool Read(out LaserReading[] readings)
		{
			int errors;
			readings = Scan(out errors);
			return errors == 0;
		}

		/// <summary>
		/// Requests the laser to stop reading
		/// </summary>
		/// <returns>true if laser ws stopped, false otherwise</returns>
		protected virtual bool RequestStopLaser()
		{
			int stat = -1, tries = 0;
			do
			{
				++tries;
				try
				{
					sendCommand("QT", "", out stat);
					if ((stat != 0) && (stat != 2))
						return true;
				}
				catch { }
			} while (tries < 10);
			return false;
		}

		/// <summary>
		/// Performs a laser scan
		/// </summary>
		/// <returns>Array of laser readings</returns>
		protected virtual LaserReading[] Scan()
		{
			int errors;
			return Scan(out errors);
		}

		/// <summary>
		/// Performs a laser scan
		/// </summary>
		/// <param name="errors">Number of errors found during scan</param>
		/// <returns>Array of laser readings</returns>
		protected virtual LaserReading[] Scan(out int errors)
		{
			string result;
			string[] lines;
			int stat;
			string p;
			int startingStep, endStep;
			short clusterCount, NumberOfScans;
			byte scanInterval;

			MemoryStream buffer;
			byte[] data;
			int i, j;
			int distance;
			int step;
			double angle, angleStep;
			LaserReading[] lectures;
			errors = 0;

			// El barrido se realiza de A a C, donde B es el frente (0 grados)
			// Crear el parametro
			// El parametro tiene la forma:
			// Starting Step (4B) | End Step (4B) | Cluster Count (2B) | Scan Interval (1B) | Number of Scans (2B)
			// Starting Step = A, End Step = C
			startingStep = stepA;
			endStep = stepC;
			// Cluster Count:	Numero de pasos adyacentes que seran fusionados como una sola lectura
			//					se toma la lectura mas cercana. Valores de 0 a 99
			clusterCount = Math.Min((short)((endStep - startingStep) / density), (short)99);
			// Scans Interval:	Numero de lecturas descartadas entre cada lectura tomada
			//					Default: 0
			scanInterval = 0;
			// Number of Scans:	Numero de escaneos antes de apagar el laser. Default a 1
			//					00 -> Lecturas infinitas
			NumberOfScans = 1;

			// Formar el parametro
			p = startingStep.ToString("0000");
			p += endStep.ToString("0000");
			p += clusterCount.ToString("00");
			p += scanInterval.ToString("0");
			p += NumberOfScans.ToString("00");


			// Comando MD
			// Primero prende el laser
			// Realiza N barridos
			// Apaga el laser
			result = sendCommand("MD", p, out stat);
			if ((stat != 0) && (stat != 99))
			{
				throw new HokuyoLaserException("MD command execution error", stat);
				//return null;
			}

			buffer = new MemoryStream(result.Length * 2);
			lines = result.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			// Meto todos los datos en un buffer de memoria
			for (i = 1; i < lines.Length; ++i)
			{
				data = ASCIIEncoding.ASCII.GetBytes(lines[i]);
				buffer.Write(data, 0, data.Length - 1);
			}

			// Transformo los datos a enteros y genero el arreglo
			if ((buffer.Length % 3) != 0)
				return null;
			data = buffer.ToArray();
			lectures = new LaserReading[buffer.Length / 3];
			//angleStep = clusterCount * 360 / ares;
			angleStep = clusterCount * AngularResolution;

			errors = 0;
			angle = (stepA - stepB) * angleStep;
			for (i = 0, j = 0; j < buffer.Length; ++i, j += 3)
			{
				//try
				//{
				step = stepA + i;
				//angle = (stepA + i - stepB) * angleStep;
				angle += angleStep;
				distance = Decrypt(data, j, 3);
				if (distance < 20)
				{
					lectures[i] = new LaserReading(this, step, angle, distance, distance);
					++errors;
				}
				else if (distance < minMeasurement)
				{
					lectures[i] = new LaserReading(this, step, angle, this.maxMeasurement, 1);
					++errors;
				}
				else if (distance > maxMeasurement)
				{
					lectures[i] = new LaserReading(this, step, angle, this.maxMeasurement, 3);
					++errors;
				}
				else
				{
					lectures[i] = new LaserReading(this, step, angle, distance, false);
				}
				//}
				//catch { }
			}

			//if((errors > 0) && this.Error != null)
			//Error(this, new HokuyoLaserError(err.Count.ToString() + " lectures presents error"));
			return lectures;
		}

		/// <summary>
		/// Sends a MD command for continous reading
		/// </summary>
		/// <param name="clusterCount">Count of neighbours lectures to merge</param>
		/// <returns>true if command was sent successfully, false otherwise</returns>
		protected bool SendCointinousMD(out short clusterCount)
		{
			return SendCointinousMD(0, out clusterCount);
		}

		/// <summary>
		/// Sends a MD command for continous reading
		/// </summary>
		/// <param name="NumberOfScans">Number of scans to perform</param>
		/// <param name="clusterCount">Count of neighbours lectures to merge</param>
		/// <returns>true if command was sent successfully, false otherwise</returns>
		protected bool SendCointinousMD(int NumberOfScans, out short clusterCount)
		{
			string cmd;
			string oldNewLine;
			int startingStep;
			int endStep;
			//short clusterCount;
			//short NumberOfScans;
			byte scanInterval;
			string[] result;
			int status;
			int sum;

			// El barrido se realiza de A a C, donde B es el frente (0 grados)
			// Crear el parametro
			// El parametro tiene la forma:
			// Starting Step (4B) | End Step (4B) | Cluster Count (2B) | Scan Interval (1B) | Number of Scans (2B)
			// Starting Step = A, End Step = C
			startingStep = step0;//startingStep = stepA;
			endStep = stepD;//endStep = stepC;
			// Cluster Count:	Numero de pasos adyacentes que seran fusionados como una sola lectura
			//					se toma la lectura mas cercana. Valores de 0 a 99
			//clusterCount = (short)Math.Min((endStep - startingStep) / density, 99); // siempre devuelve 1
			clusterCount = 0;
			// Scans Interval:	Numero de lecturas descartadas entre cada lectura tomada
			//					Default: 0
			scanInterval = 0;
			// Number of Scans:	Numero de escaneos antes de apagar el laser. Default a 1
			//					00 -> Lecturas infinitas
			//NumberOfScans = 00;

			// Pongo el comando (MD)
			cmd = "MD";
			// Formar el parametro
			cmd += startingStep.ToString("0000");
			cmd += endStep.ToString("0000");
			cmd += clusterCount.ToString("00");
			cmd += scanInterval.ToString("0");
			cmd += NumberOfScans.ToString("00");

			// Configuro puerto serie
			serialPort.ReadTimeout = 200;
			oldNewLine = serialPort.NewLine;
			serialPort.NewLine = "\n\n";

			try
			{
				// Envio comando
				serialPort.Write(cmd + "\n");

				// Leo respuesta
				status = -1;
				sum = -1;

				//result = serialPort.ReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				result = spReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				// Verifico status y Checksum
				status = Int16.Parse(result[1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				sum = (int)result[1][2];
				if ((status != 0) || (sum != 'P')) return false;
			}
			catch (InvalidOperationException ioex) { throw ioex; }
			catch (TimeoutException tex) { throw tex; }
			return true;
		}

		#region SendCommand

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param)
		{
			int status, sum;
			string text = "";
			return sendCommand(command, param, text, out status, out sum);
		}

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <param name="text">Aditional text to include with the command</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param, string text)
		{
			int status, sum;
			return sendCommand(command, param, text, out status, out sum);
		}

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <param name="status">When this method returns contains the status returned by the Hikuyo Laser device</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param, out int status)
		{
			int sum;
			string text = "";
			return sendCommand(command, param, text, out status, out sum);
		}

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <param name="status">When this method returns contains the status returned by the Hikuyo Laser device</param>
		/// <param name="sum">When this method returns contains the checksum returned by the Hikuyo Laser device</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param, out int status, out int sum)
		{
			string text = "";
			return sendCommand(command, param, text, out status, out sum);
		}

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <param name="text">Aditional text to include with the command</param>
		/// <param name="status">When this method returns contains the status returned by the Hikuyo Laser device</param>
		/// <param name="sum">When this method returns contains the checksum returned by the Hikuyo Laser device</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param, string text, out int status, out int sum)
		{
			if (!IsValidCommand(command)) throw new Exception("Invalid command");
			//if (param.Length >= 2) throw new Exception("Param _miVariable too long");
			if (text.Length >= 15)
			{
				text = ";" + text;
				if (text.Length > 16) throw new Exception("Text _miVariable too long");
			}

			string cmd = command + param + text;
			string oldNewLine;
			string[] result;
			status = -1;
			sum = -1;

			try
			{
				serialPort.ReadTimeout = 200;
				oldNewLine = serialPort.NewLine;
				serialPort.NewLine = "\n\n";
				if (!serialPort.IsOpen)
					serialPort.Open();
				serialPort.Write(cmd + "\n");

				//result = serialPort.ReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				result = spReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				if (((command == "MD") || (command == "MS")) && (result.Length >= 2))
				{
					status = Int16.Parse(result[1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					sum = (int)result[1][2];
					if ((status != 0) || (sum != 'P')) return "";
					//result = serialPort.ReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
					result = spReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

					return String.Join("\n", result, 2, result.Length - 2);
				}
				serialPort.NewLine = oldNewLine;

				if ((result[0] != cmd) || (result.Length < 2))
					return "";
				status = Int16.Parse(result[1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				sum = (int)result[1][2];
				if (result.Length < 3)
					return "";
				if (result.Length > 3)
					return String.Join("\n", result, 2, result.Length - 2);
				else
					return result[2];
			}
			catch (TimeoutException)
			{
				status = -2;
			}
			catch
			{
				status = -1;
			}
			return "";
		}

		#endregion

		/// <summary>
		/// Sets the laser communication speed
		/// </summary>
		/// <param name="speed">Speed required</param>
		/// <returns>True if command executed correctly</returns>
		public bool SetComSpeed(HokuyoLaserComSpeed speed)
		{
			int status;
			int baudrate, oldBaudrate;
			string strSpeed;
			switch (speed)
			{
				default:
					strSpeed = "019200";
					baudrate = 19200;
					break;

				case HokuyoLaserComSpeed.B057K6:
					strSpeed = "057600";
					baudrate = 57600;
					break;
				case HokuyoLaserComSpeed.B115K2:
					strSpeed = "115200";
					baudrate = 115200;
					break;
				case HokuyoLaserComSpeed.B250K:
					strSpeed = "250000";
					baudrate = 250000;
					break;
				case HokuyoLaserComSpeed.B500K:
					strSpeed = "500000";
					baudrate = 500000;
					break;
				case HokuyoLaserComSpeed.B750K:
					strSpeed = "750000";
					baudrate = 750000;
					break;
			}

			oldBaudrate = serialPort.BaudRate;
			try
			{
				if (serialPort.IsOpen)
				{
					serialPort.Close();
					serialPort.BaudRate = baudrate;
				}
				else
					serialPort.BaudRate = baudrate;
			}
			catch
			{
				serialPort.BaudRate = oldBaudrate;
				return false;
			}

			serialPort.BaudRate = oldBaudrate;
			sendCommand("SS", strSpeed, out status);
			if (status != 0) return false;
			if (serialPort.IsOpen)
			{
				serialPort.Close();
				serialPort.BaudRate = baudrate;
			}
			else
				serialPort.BaudRate = baudrate;
			return true;
		}

		/// <summary>
		/// Reads a line from the Hokuyo Laser Device
		/// </summary>
		/// <returns>The readed line</returns>
		protected string spReadLine()
		{
			return spReadLine(serialPort.ReadTimeout);
		}

		/// <summary>
		/// Reads a line from the Hokuyo Laser Device
		/// </summary>
		/// <param name="timeOut">The number of milliseconds before a time-out occurs when the read operation does not finish</param>
		/// <returns>The readed line</returns>
		protected string spReadLine(int timeOut)
		{
			StringBuilder rxBuffer;
			bool eol = false;
			int i = 0;
			int j = 0;
			int offset;
			int count;
			int elapsed = 0;
			string nl;
			string readed;

			rxBuffer = new StringBuilder(1024);
			//MemoryStream ms;

			nl = serialPort.NewLine;

			while (!eol)
			{
				Thread.Sleep(1);
				if (++elapsed > timeOut)
					throw new TimeoutException();
				if (!dataAvailiable)
					continue;
				lock (serialPort)
				{
					dataAvailiable = false;
				}

				readed = serialPort.ReadExisting();
				offset = rxBuffer.Length - nl.Length;
				if (offset < 0) offset = 0;
				rxBuffer.Append(readed);
				count = rxBuffer.Length - offset;

				for (i = offset; i < count; ++i)
				{
					for (j = 0; j < nl.Length; ++j)
					{
						if (rxBuffer[i + j] != nl[j])
							break;
					}
					if (j == nl.Length)
					{
						eol = true;
						break;
					}
				}
			}
			return rxBuffer.ToString();
		}

		/// <summary>
		/// Starts to take asynchronous readings from the Hokuyo Laser
		/// </summary>
		public override void Start()
		{
			try
			{
				Connect();
			}
			catch { return; }
			base.Start();
		}

		/// <summary>
		/// Stops the Hokuyo Laser
		/// </summary>
		public override void Stop()
		{
			base.Stop();
			try
			{
				Disconnect();
			}
			catch { return; }
		}

		/// <summary>
		/// Gets the Hokuyo Laser device version information
		/// </summary>
		protected void VersionInformation()
		{
			try
			{
				serialPort.ReadTimeout = 200;
				string model, vendor, product, firmware, protocol, sn;
				string[] lines;
				char[] nlSplit = { '\n' };
				char[] delimiters = { ':', ';' };

				if (!serialPort.IsOpen)
					serialPort.Open();

				// Envio comando VV: Solicitud de info del laser
				lines = sendCommand("VV", "").Split(nlSplit);

				//Obtengo la informacion del HW
				vendor = lines[0].Split(delimiters)[1].Trim();
				product = lines[1].Split(delimiters)[1].Trim();
				firmware = lines[2].Split(delimiters)[1].Trim();
				protocol = lines[3].Split(delimiters)[1].Trim();
				sn = lines[4].Split(delimiters)[1].Trim();

				// Envio comando PP: Solicitud de especificaciones del laser
				lines = sendCommand("PP", "").Split(nlSplit);
				//Obtengo la especificacion del laser
				model = lines[0].Split(delimiters)[1].Trim();
				minMeasurement = Convert.ToInt32(lines[1].Split(delimiters)[1].Trim());
				maxMeasurement = Convert.ToInt32(lines[2].Split(delimiters)[1].Trim());
				ares = Convert.ToInt32(lines[3].Split(delimiters)[1].Trim());
				stepA = Convert.ToInt32(lines[4].Split(delimiters)[1].Trim());
				stepC = Convert.ToInt32(lines[5].Split(delimiters)[1].Trim());
				stepB = Convert.ToInt32(lines[6].Split(delimiters)[1].Trim());
				standardSpeed = Convert.ToInt32(lines[7].Split(delimiters)[1].Trim());

				info = new DeviceInfo(model, vendor, product, firmware, protocol, sn);
				if (ares > 0)
				{
					cosByStep = new double[ares];
					sinByStep = new double[ares];
					double step = 2 * Math.PI / ares;
					double radians;
					for (int i = 0; i < ares; ++i)
					{
						radians = (stepA + i - stepB) * step;
						cosByStep[i] = Math.Cos(radians);
						sinByStep[i] = Math.Sin(radians);
					}
				}

				//switch (info.ProductName)
				//{
				//	case "":
				//		step0 = ;
				//		stepD = ;
				//		break;

				//		case "":
				//		step0 = ;
				//		stepD = ;
				//		break;

				//		case "":
				//		step0 = ;
				//		stepD = ;
				//		break;

				//		case "":
				//		step0 = ;
				//		stepD = ;
				//		break;
				//}

			}
			catch { }
		}

		/// <summary>
		/// Returns a string representation of the HokuyoLaser object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string model;
			string port;

			if (String.IsNullOrEmpty(this.Information.Model))
				model = "Unknown device";
			else
				model = this.Information.Model;

			if (String.IsNullOrEmpty(this.PortName))
				port = "unknown port";
			else
				port = this.PortName;
			return model + " on " + port;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Manages the serialPort.DataReceived event
		/// </summary>
		/// <param name="sender">The serial port</param>
		/// <param name="e">EventArgs</param>
		private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			lock (serialPort)
			{
				dataAvailiable = true;
			}
		}

		#endregion

		#region Static Members

		/// <summary>
		/// Variable used to lock laser detection
		/// </summary>
		private static Object oDetectorLock = new Object();

		/// <summary>
		/// Stores the list of found Hokuyo USB lasers
		/// </summary>
		private static SortedList<string, HokuyoLaser> laserDevices = new SortedList<string, HokuyoLaser>();

		/// <summary>
		/// Retrieves all Hokuyo Lasers connected using emulated RS232 via USB interface
		/// </summary>
		public static HokuyoLaser[] FindUSBLasers
		{
			get
			{
				ManagementObjectCollection collection;
				HokuyoLaser laser;
				HokuyoLaser[] devices;
				string deviceId;
				string pnpDeviceId;
				string description;
				List<string> detectedDevices;

				// Adds new devices
				detectedDevices = null;
				lock (oDetectorLock)
				{
					try
					{
						using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From Win32_SerialPort"))
						{
							collection = searcher.Get();
						}

						detectedDevices = new List<string>(collection.Count);
						foreach (ManagementObject device in collection)
						{

							deviceId = ((string)device.GetPropertyValue("DeviceID")).ToUpper();
							pnpDeviceId = ((string)device.GetPropertyValue("PNPDeviceID")).ToLower();
							description = ((string)device.GetPropertyValue("Description")).ToLower();
							if (deviceId.StartsWith("COM") && (pnpDeviceId.Contains("15d1") || description.Contains("urg series") || description.Contains("uhg series")))
							{
								detectedDevices.Add(deviceId);
								if (laserDevices.ContainsKey(deviceId))
									continue;
								laser = new HokuyoLaser(deviceId);
								laserDevices.Add(laser.PortName, laser);
							}
						}
					}
					catch { }

					if (detectedDevices != null)
					{
						detectedDevices.Sort();
						for (int i = 0; i < laserDevices.Count; ++i)
						{
							if (!detectedDevices.Contains(laserDevices.Keys[i]))
								laserDevices.RemoveAt(i--);
						}
					}

					devices = new HokuyoLaser[laserDevices.Count];
					laserDevices.Values.CopyTo(devices, 0);
					return devices;
				}
			}
		}

		/// <summary>
		/// Gets a HokuyoLaser (if attached) at the specified port
		/// </summary>
		/// <param name="portName">The port where the laser is attached</param>
		/// <returns>Null if device is not a laser or port not exist</returns>
		public static HokuyoLaser GetOnPort(string portName)
		{
			HokuyoLaser laser = new HokuyoLaser();
			string response;
			int count = 0;
			laser.serialPort.ReadTimeout = 200;
			laser.serialPort.NewLine = "\n\n";

			lock (oDetectorLock)
			{
			for (count = 0; count < 2; ++count)
			{
				try
				{
					laser.serialPort.PortName = portName;
					laser.serialPort.Open();
					laser.serialPort.Write("SCIP2.0\n");
					//response = l.serialPort.ReadLine();
					response = laser.spReadLine();
					if (response != "SCIP2.0\n0")
					{
						laser.serialPort.Close();
						return laser;
					}
					laser.serialPort.Close();
				}
				catch (TimeoutException)
				{
					laser.serialPort.Write("QT\n");
					laser.serialPort.Write("RS\n");
					laser.serialPort.DiscardInBuffer();
					continue;
				}
				catch
				{
					if (laser.serialPort.IsOpen)
						laser.serialPort.Close();
					break;
				}
				if (laser.serialPort.IsOpen)
					laser.serialPort.Close();
			}
			return null;
			}

		}

		#endregion
	}
	/*

	/// <summary>
	/// Enumerates sensivity modes of the laser
	/// </summary>
	public enum HokuyoLaserSensitivity
	{
		/// <summary>
		/// Hight Laser sensivity
		/// </summary>
		HighSensitivity = 1,
		/// <summary>
		/// Normal Laser sensivity
		/// </summary>
		NormalSensitivity = 0
	};

	/// <summary>
	/// Enumerates comunication baudrates for comunicate with the laser
	/// </summary>
	public enum HokuyoLaserComSpeed
	{
		/// <summary>
		/// 19200bps
		/// </summary>
		B019K2,
		/// <summary>
		/// 57600bps
		/// </summary>
		B057K6,
		/// <summary>
		/// 115200bps
		/// </summary>
		B115K2,
		/// <summary>
		/// 250kbps
		/// </summary>
		B250K,
		/// <summary>
		/// 500kbps
		/// </summary>
		B500K,
		/// <summary>
		/// 750kbps
		/// </summary>
		B750K
	};

	/// <summary>
	/// Interfaces with a Hokuyo Laser range finder
	/// </summary>
	public class HokuyoLaser : Laser
	{
		#region Variables

		/// <summary>
		/// The serial port where the Laser Device is connected
		/// </summary>
		protected SerialPort serialPort;

		/// <summary>
		/// Sensitivity of the Laser device
		/// </summary>
		protected HokuyoLaserSensitivity sensivity = HokuyoLaserSensitivity.NormalSensitivity;

		/// <summary>
		/// Proximity treshold
		/// </summary>
		protected int threshold;

		/// <summary>
		/// Flag that indicates that there is data in the SerialPort
		/// </summary>
		private bool dataAvailiable;

		/// <summary>
		/// Speed of the serial port
		/// </summary>
		protected HokuyoLaserComSpeed comSpeed = HokuyoLaserComSpeed.B019K2;

		/// <summary>
		/// Last lecture in RAW
		/// </summary>
		protected string lastRaw = "";
		
		/// <summary>
		/// Last taken reading
		/// </summary>
		protected LaserReading[] lastReading;

		/// <summary>
		/// Laser device motor speed
		/// </summary>
		protected int motorSpeed;

		/// <summary>
		/// Stores the Cos value for the angle at specified measurement step
		/// </summary>
		private double[] cosByStep;

		/// <summary>
		/// Stores the Sin value for the angle at specified measurement step
		/// </summary>
		private double[] sinByStep;


		#region Laser specs

		/// <summary>
		/// Absolute first step
		/// </summary>
		protected int step0 = 0;
		/// <summary>
		/// First Step of the Measurement Range
		/// </summary>
		protected int stepA = 44;			// First Step of the Measurement Range 
		/// <summary>
		/// Step number on the sensor's front axis
		/// </summary>
		protected int stepB = 384;			// Step number on the sensor’s front axis
		/// <summary>
		/// Last Step of the Measurement Range
		/// </summary>
		protected int stepC = 725;			// Last Step of the Measurement Range
		/// <summary>
		/// Absolute last step
		/// </summary>
		protected int stepD = 768;			// Absolute last step
		/// <summary>
		/// Minimum Measurement [mm]
		/// </summary>
		protected int minMeasurement;		// Minimum Measurement [mm]
		/// <summary>
		/// Maximum Measurement [mm]
		/// </summary>
		protected int maxMeasurement;		// Maximum Measurement [mm]
		/// <summary>
		/// Total Number of Steps in 360º range
		/// </summary>
		protected int ares;					// Total Number of Steps in 360º range
		/// <summary>
		/// Standard motor speed [rpm]
		/// </summary>
		protected int standardSpeed;		// Standard motor speed [rpm]

		#endregion

		#endregion

		#region Constructors
		
		/// <summary>
		/// Retrieves a laser object attached o the first laser device found
		/// </summary>
		public HokuyoLaser() : base()
		{
			running = false;
			serialPort = new SerialPort();
			serialPort.BaudRate = 19200;
			serialPort.Handshake = Handshake.None;
			serialPort.Parity = Parity.None;
			serialPort.NewLine = "\n";
			serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
			density = stepC - stepA;
			threshold = -1;
		}

		/// <summary>
		/// Creates a new instance of Laser class
		/// </summary>
		/// <param name="PortName">COM port where the laser is attached</param>
		public HokuyoLaser(string PortName)
			: this()
		{
			serialPort.PortName = PortName;
		}

		/// <summary>
		/// Destructor. Releases resources and closes the serial port if open
		/// </summary>
		~HokuyoLaser()
		{
			try
			{
				sendCommand("QT", "");
			}
			catch { }
			running = false;
			try
			{
				if (mainThread != null)
				{
					mainThread.Join(100);
					if (mainThread.IsAlive)
						mainThread.Abort();
				}
			}
			catch { }
			if (serialPort != null)
			{
				lock (serialPort)
				{
					try
					{
						if (serialPort.IsOpen)
						{
							serialPort.DiscardInBuffer();
							serialPort.DiscardOutBuffer();
							serialPort.Close();
						}
						serialPort.Dispose();
						serialPort = null;
					}
					catch { }
				}
			}
		}

		#endregion

		#region Events

		#endregion

		#region Properties

		/// <summary>
		/// Gets the Absolute last step the device can reach.
		/// </summary>
		public override int AbsoluteMaximumAngularStep
		{
			get { return this.stepD; }
		}

		/// <summary>
		/// Gets the smallest angle change the device can detect or rotate. Returns 2Pi / 1024
		/// </summary>
		public override double AngularResolution
		{
			get
			{
				// Returns 2 * Pi / 1024
				return 0.0061359231515425649188723503579678;
			}
		}

		/// <summary>
		/// Gets the angle resollution bits. Always returns 10
		/// </summary>
		public override int AngularResolutionBits
		{
			get { return 10; }
		}

		/// <summary>
		/// Step number on the sensor's front axis
		/// </summary>
		public override int AngularStepZero
		{
			get { return this.stepB; }
		}

		/// <summary>
		/// Gets a value indicating the open or close status of the Laser port
		/// </summary>
		public override bool IsOpen
		{
			get { return serialPort.IsOpen; }

		}

		/// <summary>
		/// Gets the last reading array obtained from the sensor
		/// </summary>
		public override ITelemetricReading[] LastReadings
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets the maximum angle in radians the sensor can detect measured from the front of the sensor
		/// </summary>
		public override double MaximumAngle
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets the maximum distance the sensor can detect
		/// </summary>
		public override double MaximumDistance
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets the minumim distance the sensor can detect
		/// </summary>
		public override double MinimumDistance
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets the minumim angle in radians the sensor can detect measured from the front of the sensor
		/// </summary>
		public override double MinimumAngle
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets a value indicating if the continous asynchronous read operation of the sensor has been started
		/// </summary>
		public override bool Started
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		/// <summary>
		/// Gets the number of steps in a complete revolution (360º or two pi radians).
		/// </summary>
		public override int StepsPerRevolution
		{
			get { return this.ares; }
		}

		/// <summary>
		/// Gets the First Step of the Measurement Range 
		/// </summary>
		public override int ValidMinimumAngularStep
		{
			get { return this.stepA; }
		}

		/// <summary>
		/// Gets the Last Step of the Measurement Range 
		/// </summary>
		public override int ValidMaximumAngularStep
		{
			get { return this.stepC; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Checks if port exists and is availiable for reading
		/// </summary>
		/// <returns>true if port exists and can be opened, false otherwise</returns>
		protected virtual bool CheckPortAvailiability()
		{
			bool flag = false;
			if (serialPort == null)
				return false;

			List<string> portNames = new List<string>(SerialPort.GetPortNames());
			if (!portNames.Contains(serialPort.PortName))
				return false;

			flag = true;
			try
			{
				if (!serialPort.IsOpen)
					serialPort.Open();
			}
			catch { flag = false; }

			try
			{
				if (serialPort.IsOpen)
					serialPort.Close();
			}
			catch { flag = false; }
			return flag;
		}

		/// <summary>
		/// Starts communication with the laser
		/// </summary>
		public override void Connect()
		{
			if (IsOpen) return;
			if (!serialPort.IsOpen)
				serialPort.Open();

			int stat;

			serialPort.DiscardInBuffer();
			serialPort.DiscardOutBuffer();
			sendCommand("SCIP2.0", "", out stat);
			if ((stat != 0) && (stat != 14))
			{
				// Try again but stopping the device first
				sendCommand("RS", "", out stat);
				serialPort.DiscardInBuffer();
				serialPort.DiscardOutBuffer();
				sendCommand("SCIP2.0", "", out stat);
				if ((stat != 0) && (stat != 14))
				{
					serialPort.Close();
					throw new Exception("Unsupported device");
					//return;
				}
			}
			VersionInformation();
		}

		/// <summary>
		/// Stops communication with the laser
		/// </summary>
		public override void Disconnect()
		{
			if (serialPort.IsOpen)
				serialPort.Close();
		}

		/// <summary>
		/// Decrypts an integer from a byte array
		/// </summary>
		/// <param name="data">Array of data to decript</param>
		/// <returns>Integer value represented by the encrypted data</returns>
		protected int Decrypt(byte[] data)
		{
			int result = 0;
			int i;

			if (data.Length > 4) return -1;
			for (i = 0; i < data.Length; ++i)
			{
				result *= 64;
				result += data[i] - 0x30;
			}
			return result;
		}

		/// <summary>
		/// Decrypts an integer from a byte array
		/// </summary>
		/// <param name="data">Array of data to decript</param>
		/// <param name="offset">a zero-based offset where to start the decryption</param>
		/// <param name="count">Number of bytes to decrypt</param>
		/// <returns>Integer value represented by the encrypted data</returns>
		protected int Decrypt(byte[] data, int offset, int count)
		{
			int result = 0;
			int i;

			if (count > 4) return -1;
			for (i = 0; i < count; ++i)
			{
				result *= 64;
				result += data[offset + i] - 0x30;
			}
			return result;
		}

		/// <summary>
		/// Gets the cosine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the cosine is desired</param>
		/// <returns>The cosine value of the angle at provided step.</returns>
		public override double GetCosFromStep(int step)
		{
			if ((step < 0) || (step > ares))
				throw new ArgumentOutOfRangeException();
			if (cosByStep == null)
				throw new HokuyoLaserException("Uninitialized device. Make sure device is connected and Connect() method has been called.");
			return cosByStep[step];
		}

		/// <summary>
		/// Gets the response for an MD command
		/// </summary>
		/// <param name="remainingScans">The number of scans remaining of the MD command requested</param>
		/// <returns>The data readed from the laser. Null if the data is not congruent</returns>
		protected byte[] GetMDResponse(ref int remainingScans)
		{
			string result;
			string[] lines;
			MemoryStream buffer;
			byte[] sBuffer;
			int i, j;
			Int16 sum;
			byte[] data = null;

			// Get the result

			try
			{
				result = spReadLine();
				lastRaw = (string)result.Clone();
				lines = result.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				// get remaining
				if ((lines[0].Length == 15) && (lines[0].StartsWith("MD")))
					remainingScans = Int32.Parse(lines[0].Substring(13));
				if ((lines.Length < 4) || (lines[1] != "99b"))
					return null;
			}
			catch { return null; }

			// Fill the buffer and check sums
			buffer = new MemoryStream(result.Length);
			sBuffer = new byte[1];
			for (i = 3; i < lines.Length; ++i)
			{
				sum = 0;
				for (j = 0; j < lines[i].Length - 1; ++j)
				{
					sBuffer[0] = (byte)lines[i][j];
					buffer.Write(sBuffer, 0, 1);
					unchecked { sum += sBuffer[0]; }
				}
				sum &= 0x00003F;
				sum += 0x30;
				if (sum != lines[i][j]) return null;
			}
			// Check if the data is congruent
			if ((buffer.Length % 3) != 0)
				return null;

			// Get data from buffer
			data = buffer.ToArray();
			buffer.Close();
			return data;
		}

		/// <summary>
		/// Gets the sine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the sine is desired</param>
		/// <returns>The sine value of the angle at provided step.</returns>
		public override double GetSinFromStep(int step)
		{
			if ((step < 0) || (step > ares))
				throw new ArgumentOutOfRangeException();
			if (sinByStep == null)
				throw new HokuyoLaserException("Uninitialized device. Make sure device is connected and Connect() method has been called.");
			return sinByStep[step];
		}

		/// <summary>
		/// Checks if the provided command is valid
		/// </summary>
		/// <param name="command">The command to check</param>
		/// <returns>true if the command is recognized by the device, false otherwise</returns>
		protected bool IsValidCommand(string command)
		{
			//if (command.Length != 2) return false;
			switch (command)
			{
				case "SCIP2.0":
				case "MD":
				case "MS":
				case "GD":
				case "GS":
				case "BM":
				case "QT":
				case "RS":
				case "TM":
				case "SS":
				case "CR":
				case "HS":
				case "DB":
				case "VV":
				case "PP":
				case "II":
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// Performs the asynchronous device reading operation
		/// </summary>
		protected override void MainThreadTask()
		{
			bool validatePort = false;
			int stat = -1;
			int tries = 0;
			int remaining;
			LaserReading[] readings;
			short clusterCount = 0;
			int hangCounter = 0;
			int minDistance = (int)(MaximumDistance * 1000);
			int maxDistance = (int)(MinimumDistance * 1000);

			running = true;

			// Request reset the laser
			#region Request reset the laser
			do
			{
				++tries;
				try
				{
					sendCommand("QT", "", out stat);
					if (stat == 1)
						OnError(new HokuyoLaserError("Laser malfunction during initialization stop (" + tries.ToString() + " of 3"));
					sendCommand("RS", "", out stat);
					if (stat == 1)
						OnError(new HokuyoLaserError("Laser malfunction during initialization reset (" + tries.ToString() + " of 3"));
				}
				catch { OnError(new HokuyoLaserError("Laser malfunction while initialization reset (" + tries.ToString() + " of 3")); }
			} while (running && (tries < 3) && (stat != 0) && (stat != 2));
			#endregion

			// Request turn on laser
			#region Request turn on laser
			do
			{
				++tries;
				try
				{
					sendCommand("BM", "", out stat);
					if (stat == 1)
						OnError(new HokuyoLaserError("Laser malfunction while turning on (" + tries.ToString() + " of 3"));
				}
				catch { OnError(new HokuyoLaserError("Laser malfunction while turning on (" + tries.ToString() + " of 3")); }
			} while (running && (tries < 3) && (stat != 0) && (stat != 2));
			#endregion

			#region Start Event Thread
			//if((eventThread != null) && eventThread.IsAlive)
			//	eventThread.Abort();
			//eventThread = new Thread(new ThreadStart(EventThreadTask));
			//eventThread.IsBackground = true;
			////eventThread.Priority = ThreadPriority.BelowNormal;
			//eventThread.Start();

			#endregion

			#region Scan
			remaining = 0;
			while (running)
			{
				if (validatePort)
				{
					while (running && !(validatePort = CheckPortAvailiability()))
						Thread.Sleep(1000);
					try
					{
						sendCommand("RS", "", out stat);
						sendCommand("BM", "", out stat);
					}
					catch { }
				}
				try
				{
					// Request continous (99) measuring
					if (((hangCounter > 100) || (remaining <= 0)) && !SendCointinousMD(99, out clusterCount))
						OnError(new HokuyoLaserError("Cannot send MD command"));

					minDistance = (int)(MaximumDistance * 1000);
					maxDistance = (int)(MinimumDistance * 1000);
					if (ParseMDResponse(clusterCount, out readings, out minDistance, out maxDistance, ref remaining))
					{
						hangCounter = 0;
						this.lastReading = readings;
							OnReadCompleted(readings);
						if (minDistance <= threshold)
							OnTresholdExceeded();
					}
					else
					{
						++hangCounter;
						Thread.Sleep(0);
					}
				}
				catch (InvalidOperationException ioex)
				{
					validatePort = CheckPortAvailiability();
					if (validatePort)
						RequestStopLaser();
					continue;
				}
				catch (TimeoutException toex)
				{
					validatePort = CheckPortAvailiability();
					if (validatePort)
						RequestStopLaser();
					continue;
				}
				catch (ThreadAbortException taex)
				{
					taex.ToString();
					RequestStopLaser();
					break;
				}
				catch
				{
					continue;
				}
				//while (running) Thread.Sleep(1);
			}

			#endregion

			// Request stop laser
			#region Request stop laser
			RequestStopLaser();
			#endregion

			running = false;
		}

		/// <summary>
		/// Extract the readings of a MD response sent by the Hokuyo Laser device
		/// </summary>
		/// <param name="clusterCount">The number of cluster count</param>
		/// <param name="readings">When this method returns contains the array of laser readings</param>
		/// <param name="remainingScans">When this method returns contains the number of remaining scans</param>
		/// <returns>true if data was parsed successfully, false otherwise</returns>
		protected bool ParseMDResponse(short clusterCount, out LaserReading[] readings, ref int remainingScans)
		{
			int minDistance;
			int maxDistance;
			return ParseMDResponse(clusterCount, out readings, out minDistance, out maxDistance, ref remainingScans);
		}

		/// <summary>
		/// Extract the readings of a MD response sent by the Hokuyo Laser device
		/// </summary>
		/// <param name="clusterCount">The number of cluster count</param>
		/// <param name="readings">When this method returns contains the array of laser readings</param>
		/// <param name="minDistance">When this method returns contains the minimum distance measured</param>
		/// <param name="maxDistance">When this method returns contains the maximum distance measured</param>
		/// <param name="remainingScans">When this method returns contains the number of remaining scans</param>
		/// <returns>true if data was parsed successfully, false otherwise</returns>
		protected bool ParseMDResponse(short clusterCount, out LaserReading[] readings, out int minDistance, out int maxDistance, ref int remainingScans)
		{
			int i, j;
			byte[] data;
			int distance;
			double angle;
			double angleStep;

			// First assign
			readings = null;
			minDistance = Int32.MaxValue;
			maxDistance = Int32.MinValue;

			// Get the result
			data = GetMDResponse(ref remainingScans);
			if (data == null)
				return false;
			readings = new LaserReading[data.Length / 3];

			// Prepare conversion
			if (clusterCount != 0)
				angleStep = clusterCount * 360.0 / ares;
			else
				angleStep = 360.0 / ares;

			for (i = 0, j = 0; j < data.Length; ++i, j += 3)
			{
				angle = (step0 + i - stepB) * angleStep;
				distance = Decrypt(data, j, 3);
				if ((distance < minMeasurement) || (distance > maxMeasurement))
					readings[i] = new LaserReading(this, angle, this.maxMeasurement, true);
				else
				{
					readings[i] = new LaserReading(this, angle, distance, false);
					if (distance < minDistance)
						minDistance = distance;
					if (distance > maxDistance) maxDistance = distance;
				}
			}

			return true;
		}

		/// <summary>
		/// Syncronusly reads the Hokuyo Laser sensor
		/// </summary>
		/// <param name="readings">When this method returns contains the array of sensor readings if the sensor was readed successfully, null otherwise</param>
		/// <returns>true if read from the sensor was completed successfully, false otherwise</returns>
		public override bool Read(out ITelemetricReading[] readings)
		{
			LaserReading[] laserReadings;
			bool result = Read(out laserReadings);
			readings = laserReadings;
			return result;
		}

		/// <summary>
		/// Syncronusly reads the Hokuyo Laser sensor
		/// </summary>
		/// <param name="readings">When this method returns contains the array of Hokuyo Laser sensor readings if the sensor was readed successfully, null otherwise</param>
		/// <returns>true if read from the sensor was completed successfully, false otherwise</returns>
		public bool Read(out LaserReading[] readings)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Requests the laser to stop reading
		/// </summary>
		/// <returns>true if laser ws stopped, false otherwise</returns>
		protected virtual bool RequestStopLaser()
		{
			int stat = -1, tries = 0;
			do
			{
				++tries;
				try
				{
					sendCommand("QT", "", out stat);
					if ((stat != 0) && (stat != 2))
						return true;
				}
				catch { }
			} while (tries < 10);
			return false;
		}

		protected virtual LaserReading[] Scan()
		{
			int errors;
			return Scan(out errors);
		}

		protected virtual LaserReading[] Scan(out int errors)
		{
			string result;
			string[] lines;
			int stat;
			string p;
			int startingStep, endStep;
			short clusterCount, NumberOfScans;
			byte scanInterval;

			MemoryStream buffer;
			byte[] data;
			int i, j;
			int distance;
			int step;
			double angle, angleStep;
			LaserReading[] lectures;
			errors = 0;

			// El barrido se realiza de A a C, donde B es el frente (0 grados)
			// Crear el parametro
			// El parametro tiene la forma:
			// Starting Step (4B) | End Step (4B) | Cluster Count (2B) | Scan Interval (1B) | Number of Scans (2B)
			// Starting Step = A, End Step = C
			startingStep = stepA;
			endStep = stepC;
			// Cluster Count:	Numero de pasos adyacentes que seran fusionados como una sola lectura
			//					se toma la lectura mas cercana. Valores de 0 a 99
			clusterCount = Math.Min((short)((endStep - startingStep) / density), (short)99);
			// Scans Interval:	Numero de lecturas descartadas entre cada lectura tomada
			//					Default: 0
			scanInterval = 0;
			// Number of Scans:	Numero de escaneos antes de apagar el laser. Default a 1
			//					00 -> Lecturas infinitas
			NumberOfScans = 1;

			// Formar el parametro
			p = startingStep.ToString("0000");
			p += endStep.ToString("0000");
			p += clusterCount.ToString("00");
			p += scanInterval.ToString("0");
			p += NumberOfScans.ToString("00");


			// Comando MD
			// Primero prende el laser
			// Realiza N barridos
			// Apaga el laser
			result = sendCommand("MD", p, out stat);
			if ((stat != 0) && (stat != 99))
			{
				throw new HokuyoLaserException("MD command execution error", stat);
				//return null;
			}

			buffer = new MemoryStream(result.Length * 2);
			lines = result.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
			// Meto todos los datos en un buffer de memoria
			for (i = 1; i < lines.Length; ++i)
			{
				data = ASCIIEncoding.ASCII.GetBytes(lines[i]);
				buffer.Write(data, 0, data.Length - 1);
			}

			// Transformo los datos a enteros y genero el arreglo
			if ((buffer.Length % 3) != 0)
				return null;
			data = buffer.ToArray();
			lectures = new LaserReading[buffer.Length / 3];
			angleStep = clusterCount * 360.0 / ares;

			errors = 0;
			//angle = (stepA - stepB) * angleStep;
			for (i = 0, j = 0; j < buffer.Length; ++i, j += 3)
			{
				//try
				//{
				step = stepA + i;
				angle = (stepA + i - stepB) * angleStep;
				//angle += angleStep;
				distance = Decrypt(data, j, 3);
				if (distance < 20)
				{
					lectures[i] = new LaserReading(this, step, angle, distance, distance);
					++errors;
				}
				else if (distance < minMeasurement)
				{
					lectures[i] = new LaserReading(this, step, angle, this.maxMeasurement, 1);
					++errors;
				}
				else if (distance > maxMeasurement)
				{
					lectures[i] = new LaserReading(this, step, angle, this.maxMeasurement, 3);
					++errors;
				}
				else
				{
					lectures[i] = new LaserReading(this, step, angle, distance, false);
				}
				//}
				//catch { }
			}

			//if((errors > 0) && this.Error != null)
			//Error(this, new HokuyoLaserError(err.Count.ToString() + " lectures presents error"));
			return lectures;
		}

		protected bool SendCointinousMD(out short clusterCount)
		{
			return SendCointinousMD(0, out clusterCount);
		}

		protected bool SendCointinousMD(int NumberOfScans, out short clusterCount)
		{
			string cmd;
			string oldNewLine;
			int startingStep;
			int endStep;
			//short clusterCount;
			//short NumberOfScans;
			byte scanInterval;
			string[] result;
			int status;
			int sum;

			// El barrido se realiza de A a C, donde B es el frente (0 grados)
			// Crear el parametro
			// El parametro tiene la forma:
			// Starting Step (4B) | End Step (4B) | Cluster Count (2B) | Scan Interval (1B) | Number of Scans (2B)
			// Starting Step = A, End Step = C
			startingStep = step0;//startingStep = stepA;
			endStep = stepD;//endStep = stepC;
			// Cluster Count:	Numero de pasos adyacentes que seran fusionados como una sola lectura
			//					se toma la lectura mas cercana. Valores de 0 a 99
			//clusterCount = (short)Math.Min((endStep - startingStep) / density, 99); // siempre devuelve 1
			clusterCount = 0;
			// Scans Interval:	Numero de lecturas descartadas entre cada lectura tomada
			//					Default: 0
			scanInterval = 0;
			// Number of Scans:	Numero de escaneos antes de apagar el laser. Default a 1
			//					00 -> Lecturas infinitas
			//NumberOfScans = 00;

			// Pongo el comando (MD)
			cmd = "MD";
			// Formar el parametro
			cmd += startingStep.ToString("0000");
			cmd += endStep.ToString("0000");
			cmd += clusterCount.ToString("00");
			cmd += scanInterval.ToString("0");
			cmd += NumberOfScans.ToString("00");

			// Configuro puerto serie
			serialPort.ReadTimeout = 200;
			oldNewLine = serialPort.NewLine;
			serialPort.NewLine = "\n\n";

			try
			{
				// Envio comando
				serialPort.Write(cmd + "\n");

				// Leo respuesta
				status = -1;
				sum = -1;

				//result = serialPort.ReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				result = spReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				// Verifico status y Checksum
				status = Int16.Parse(result[1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				sum = (int)result[1][2];
				if ((status != 0) || (sum != 'P')) return false;
			}
			catch (InvalidOperationException ioex) { throw ioex; }
			catch (TimeoutException tex) { throw tex; }
			return true;
		}

		#region SendCommand

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param)
		{
			int status, sum;
			string text = "";
			return sendCommand(command, param, text, out status, out sum);
		}

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <param name="text">Aditional text to include with the command</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param, string text)
		{
			int status, sum;
			return sendCommand(command, param, text, out status, out sum);
		}

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <param name="status">When this method returns contains the status returned by the Hikuyo Laser device</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param, out int status)
		{
			int sum;
			string text = "";
			return sendCommand(command, param, text, out status, out sum);
		}

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <param name="status">When this method returns contains the status returned by the Hikuyo Laser device</param>
		/// <param name="sum">When this method returns contains the checksum returned by the Hikuyo Laser device</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param, out int status, out int sum)
		{
			string text = "";
			return sendCommand(command, param, text, out status, out sum);
		}

		/// <summary>
		/// Sends a command to the Hokuyo Laser device
		/// </summary>
		/// <param name="command">The command to send</param>
		/// <param name="param">The parameters to be sent with the command</param>
		/// <param name="text">Aditional text to include with the command</param>
		/// <param name="status">When this method returns contains the status returned by the Hikuyo Laser device</param>
		/// <param name="sum">When this method returns contains the checksum returned by the Hikuyo Laser device</param>
		/// <returns>A string with the response to the sent command</returns>
		protected string sendCommand(string command, string param, string text, out int status, out int sum)
		{
			if (!IsValidCommand(command)) throw new Exception("Invalid command");
			//if (param.Length >= 2) throw new Exception("Param _miVariable too long");
			if (text.Length >= 15)
			{
				text = ";" + text;
				if (text.Length > 16) throw new Exception("Text _miVariable too long");
			}

			string cmd = command + param + text;
			string oldNewLine;
			string[] result;
			status = -1;
			sum = -1;

			try
			{
				serialPort.ReadTimeout = 200;
				oldNewLine = serialPort.NewLine;
				serialPort.NewLine = "\n\n";
				if (!serialPort.IsOpen)
					serialPort.Open();
				serialPort.Write(cmd + "\n");

				//result = serialPort.ReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				result = spReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				if (((command == "MD") || (command == "MS")) && (result.Length >= 2))
				{
					status = Int16.Parse(result[1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
					sum = (int)result[1][2];
					if ((status != 0) || (sum != 'P')) return "";
					//result = serialPort.ReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
					result = spReadLine().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

					return String.Join("\n", result, 2, result.Length - 2);
				}
				serialPort.NewLine = oldNewLine;

				if ((result[0] != cmd) || (result.Length < 2))
					return "";
				status = Int16.Parse(result[1].Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
				sum = (int)result[1][2];
				if (result.Length < 3)
					return "";
				if (result.Length > 3)
					return String.Join("\n", result, 2, result.Length - 2);
				else
					return result[2];
			}
			catch (TimeoutException)
			{
				status = -2;
			}
			catch
			{
				status = -1;
			}
			return "";
		}

		#endregion

		/// <summary>
		/// Sets the laser communication speed
		/// </summary>
		/// <param name="speed">Speed required</param>
		/// <returns>True if command executed correctly</returns>
		public bool SetComSpeed(HokuyoLaserComSpeed speed)
		{
			int status;
			int baudrate, oldBaudrate;
			string strSpeed;
			switch (speed)
			{
				default:
					strSpeed = "019200";
					baudrate = 19200;
					break;

				case HokuyoLaserComSpeed.B057K6:
					strSpeed = "057600";
					baudrate = 57600;
					break;
				case HokuyoLaserComSpeed.B115K2:
					strSpeed = "115200";
					baudrate = 115200;
					break;
				case HokuyoLaserComSpeed.B250K:
					strSpeed = "250000";
					baudrate = 250000;
					break;
				case HokuyoLaserComSpeed.B500K:
					strSpeed = "500000";
					baudrate = 500000;
					break;
				case HokuyoLaserComSpeed.B750K:
					strSpeed = "750000";
					baudrate = 750000;
					break;
			}

			oldBaudrate = serialPort.BaudRate;
			try
			{
				if (serialPort.IsOpen)
				{
					serialPort.Close();
					serialPort.BaudRate = baudrate;
				}
				else
					serialPort.BaudRate = baudrate;
			}
			catch
			{
				serialPort.BaudRate = oldBaudrate;
				return false;
			}

			serialPort.BaudRate = oldBaudrate;
			sendCommand("SS", strSpeed, out status);
			if (status != 0) return false;
			if (serialPort.IsOpen)
			{
				serialPort.Close();
				serialPort.BaudRate = baudrate;
			}
			else
				serialPort.BaudRate = baudrate;
			return true;
		}

		/// <summary>
		/// Reads a line from the Hokuyo Laser Device
		/// </summary>
		/// <returns>The readed line</returns>
		protected string spReadLine()
		{
			return spReadLine(serialPort.ReadTimeout);
		}

		/// <summary>
		/// Reads a line from the Hokuyo Laser Device
		/// </summary>
		/// <param name="timeOut">The number of milliseconds before a time-out occurs when the read operation does not finish</param>
		/// <returns>The readed line</returns>
		protected string spReadLine(int timeOut)
		{
			StringBuilder rxBuffer;
			bool eol = false;
			int i = 0;
			int j = 0;
			int offset;
			int count;
			int elapsed = 0;
			string nl;
			string readed;

			rxBuffer = new StringBuilder(1024);
			//MemoryStream ms;

			nl = serialPort.NewLine;

			while (!eol)
			{
				Thread.Sleep(1);
				if (++elapsed > timeOut)
					throw new TimeoutException();
				if (!dataAvailiable)
					continue;
				lock (serialPort)
				{
					dataAvailiable = false;
				}

				readed = serialPort.ReadExisting();
				offset = rxBuffer.Length - nl.Length;
				if (offset < 0) offset = 0;
				rxBuffer.Append(readed);
				count = rxBuffer.Length - offset;

				for (i = offset; i < count; ++i)
				{
					for (j = 0; j < nl.Length; ++j)
					{
						if (rxBuffer[i + j] != nl[j])
							break;
					}
					if (j == nl.Length)
					{
						eol = true;
						break;
					}
				}
			}
			return rxBuffer.ToString();
		}

		/// <summary>
		/// Gets the Hokuyo Laser device version information
		/// </summary>
		protected void VersionInformation()
		{
			try
			{
				serialPort.ReadTimeout = 200;
				string model, vendor, product, firmware, protocol, sn;
				string[] lines;
				char[] nlSplit = { '\n' };
				char[] delimiters = { ':', ';' };

				if (!serialPort.IsOpen)
					serialPort.Open();

				// Envio comando VV: Solicitud de info del laser
				lines = sendCommand("VV", "").Split(nlSplit);

				//Obtengo la informacion del HW
				vendor = lines[0].Split(delimiters)[1].Trim();
				product = lines[1].Split(delimiters)[1].Trim();
				firmware = lines[2].Split(delimiters)[1].Trim();
				protocol = lines[3].Split(delimiters)[1].Trim();
				sn = lines[4].Split(delimiters)[1].Trim();

				// Envio comando PP: Solicitud de especificaciones del laser
				lines = sendCommand("PP", "").Split(nlSplit);
				//Obtengo la especificacion del laser
				model = lines[0].Split(delimiters)[1].Trim();
				minMeasurement = Convert.ToInt32(lines[1].Split(delimiters)[1].Trim());
				maxMeasurement = Convert.ToInt32(lines[2].Split(delimiters)[1].Trim());
				ares = Convert.ToInt32(lines[3].Split(delimiters)[1].Trim());
				stepA = Convert.ToInt32(lines[4].Split(delimiters)[1].Trim());
				stepC = Convert.ToInt32(lines[5].Split(delimiters)[1].Trim());
				stepB = Convert.ToInt32(lines[6].Split(delimiters)[1].Trim());
				standardSpeed = Convert.ToInt32(lines[7].Split(delimiters)[1].Trim());

				info = new DeviceInfo(model, vendor, product, firmware, protocol, sn);
				if (ares > 0)
				{
					cosByStep = new double[ares];
					sinByStep = new double[ares];
					double step = 2 * Math.PI / ares;
					double radians;
					for (int i = 0; i < ares; ++i)
					{
						radians = (stepA + i - stepB) * step;
						cosByStep[i] = Math.Cos(radians);
						sinByStep[i] = Math.Sin(radians);
					}
				}

				//switch (info.ProductName)
				//{
				//	case "":
				//		step0 = ;
				//		stepD = ;
				//		break;

				//		case "":
				//		step0 = ;
				//		stepD = ;
				//		break;

				//		case "":
				//		step0 = ;
				//		stepD = ;
				//		break;

				//		case "":
				//		step0 = ;
				//		stepD = ;
				//		break;
				//}

			}
			catch { }
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Manages the serialPort.DataReceived event
		/// </summary>
		/// <param name="sender">The serial port</param>
		/// <param name="e">EventArgs</param>
		private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			lock (serialPort)
			{
				dataAvailiable = true;
			}
		}

		#endregion

		#region Static Members

		/// <summary>
		/// Retrieves all Hokuyo Lasers connected using emulated RS232 via USB interface
		/// </summary>
		public static HokuyoLaser[] FindUSBLasers
		{
			get
			{
				ManagementObjectCollection collection;
				List<HokuyoLaser> laserDevices;
				HokuyoLaser laser;
				string deviceId;
				string pnpDeviceId;
				string description;

				laserDevices = new List<HokuyoLaser>();
				try
				{
					using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From Win32_SerialPort"))
					{
						collection = searcher.Get();
					}

					foreach (ManagementObject device in collection)
					{

						deviceId = ((string)device.GetPropertyValue("DeviceID")).ToUpper();
						pnpDeviceId = ((string)device.GetPropertyValue("PNPDeviceID")).ToLower();
						description = ((string)device.GetPropertyValue("Description")).ToLower();
						if (deviceId.StartsWith("COM") && (pnpDeviceId.Contains("15d1") || description.Contains("urg series") || description.Contains("uhg series")))
						{
							laser = new HokuyoLaser(deviceId);
							laserDevices.Add(laser);
						}
					}

				}
				catch { }
				return laserDevices.ToArray();
			}
		}

		/// <summary>
		/// Gets a HokuyoLaser (if attached) at the specified port
		/// </summary>
		/// <param name="portName">The port where the laser is attached</param>
		/// <returns>Null if device is not a laser or port not exist</returns>
		public static HokuyoLaser GetOnPort(string portName)
		{
			HokuyoLaser laser = new HokuyoLaser();
			string response;
			int count = 0;
			laser.serialPort.ReadTimeout = 200;
			laser.serialPort.NewLine = "\n\n";

			for (count = 0; count < 2; ++count)
			{
				try
				{
					laser.serialPort.PortName = portName;
					laser.serialPort.Open();
					laser.serialPort.Write("SCIP2.0\n");
					//response = l.serialPort.ReadLine();
					response = laser.spReadLine();
					if (response != "SCIP2.0\n0")
					{
						laser.serialPort.Close();
						return laser;
					}
					laser.serialPort.Close();
				}
				catch (TimeoutException)
				{
					laser.serialPort.Write("QT\n");
					laser.serialPort.Write("RS\n");
					laser.serialPort.DiscardInBuffer();
					continue;
				}
				catch
				{
					if (laser.serialPort.IsOpen)
						laser.serialPort.Close();
					break;
				}
				if (laser.serialPort.IsOpen)
					laser.serialPort.Close();
			}
			return null;
		}

		#endregion
	}
	 */
}
