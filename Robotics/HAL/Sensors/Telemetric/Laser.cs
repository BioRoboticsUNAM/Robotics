using System;
using System.ComponentModel;
using System.Threading;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Enumerates the laser status modes
	/// </summary>
	public enum LaserStatus
	{
		/// <summary>
		/// Unknown status or error
		/// </summary>
		Error = -1,
		/// <summary>
		/// Laser is stoped.
		/// </summary>
		Stoped = 0x00,
		/// <summary>
		/// The laser is running
		/// </summary>
		Running = 0x01,
		/// <summary>
		/// Laser is running but no lectures are taken
		/// </summary>
		Suspended = 0x02
	};

	/// <summary>
	/// Represents a telemetric laser sensor
	/// </summary>
	public abstract class Laser : TelemetricSensor<Laser, LaserReading>, IPolarDevice
	{
		#region Variables

		/// <summary>
		/// Stores the Cos value for the angle at specified measurement step
		/// </summary>
		private double[] cosByStep;

		/// <summary>
		/// Data density
		/// </summary>
		protected int density;

		/// <summary>
		/// Number of errors in the last lecture
		/// </summary>
		protected int errorsOnLastReading;

		/// <summary>
		/// Information of the laser device
		/// </summary>
		protected DeviceInfo info;

		/// <summary>
		/// Main thread for async read operations
		/// </summary>
		protected Thread mainThread;

		/// <summary>
		/// Main Thread execution flag
		/// </summary>
		protected bool running;

		/// <summary>
		/// Stores the Sin value for the angle at specified measurement step
		/// </summary>
		private double[] sinByStep;

		/// <summary>
		/// Laser device current status
		/// </summary>
		protected LaserStatus status = LaserStatus.Stoped;

		#endregion
		
		#region Properties

		/// <summary>
		/// Gets the Absolute last step the device can reach
		/// </summary>
		[DescriptionAttribute("Gets the Absolute last step the device can reach")]
		[CategoryAttribute("Hardware Capabilities")]
		public abstract int AbsoluteMaximumAngularStep { get; }

		/// <summary>
		/// Gets the smallest angle change the device can detect or rotate
		/// </summary>
		[DescriptionAttribute("Gets the smallest angle change the device can detect or rotate")]
		[CategoryAttribute("Hardware Information")]
		public abstract double AngularResolution { get; }

		/// <summary>
		/// Gets the angle resolution bits
		/// </summary>
		[DescriptionAttribute("Gets the angle resolution bits")]
		[CategoryAttribute("Hardware Information")]
		public abstract int AngularResolutionBits { get; }

		/// <summary>
		/// Step number of the sensor's front axis
		/// </summary>
		[DescriptionAttribute("Step number of the sensor's front axis")]
		[CategoryAttribute("Hardware Information")]
		public abstract int AngularStepZero { get; }

		/// <summary>
		/// Gets the number of mistaken readings in the last read
		/// </summary>
		[DescriptionAttribute("Gets the number of mistaken readings in the last read")]
		[CategoryAttribute("Device Status")]
		public override int ErrorCount
		{
			get { return errorsOnLastReading; }
			protected set
			{
				if ((value < 0) || (value > AbsoluteMaximumAngularStep))
					errorsOnLastReading = value;
			}
		}

		/// <summary>
		/// Stores the Cos value for the angle at specified measurement step
		/// </summary>
		protected double[] CosByStep
		{
			get { return cosByStep; }
		}

		/// <summary>
		/// Returns the device's information 
		/// </summary>
		[DescriptionAttribute("Returns the device's information")]
		[CategoryAttribute("Hardware Information")]
		public virtual DeviceInfo Information
		{
			get { return info; }
		}

		/// <summary>
		/// Gets the maximum angle in radians the sensor can detect measured from the front of the sensor
		/// </summary>
		[DescriptionAttribute("Gets the maximum angle in radians the sensor can detect measured from the front of the sensor")]
		[CategoryAttribute("Hardware Capabilities")]
		public abstract double MaximumAngle { get; }

		/// <summary>
		/// Gets the minumim angle in radians the sensor can detect measured from the front of the sensor
		/// </summary>
		[DescriptionAttribute("Gets the minumim angle in radians the sensor can detect measured from the front of the sensor")]
		[CategoryAttribute("Hardware Capabilities")]
		public abstract double MinimumAngle { get; }

		/// <summary>
		/// Stores the Sin value for the angle at specified measurement step
		/// </summary>
		protected double[] SinByStep
		{
			get { return sinByStep; }
		}

		/// <summary>
		/// Gets the number of steps in a complete revolution (360º or two pi radians)
		/// </summary>
		[DescriptionAttribute("Gets the number of steps in a complete revolution (360º or two pi radians)")]
		[CategoryAttribute("Hardware Information")]
		public abstract int StepsPerRevolution { get; }

		/// <summary>
		/// Gets the First Step of the Measurement Range 
		/// </summary>
		[DescriptionAttribute("Gets the First Step of the Measurement Range")]
		[CategoryAttribute("Hardware Capabilities")]
		public abstract int ValidMinimumAngularStep { get; }

		/// <summary>
		/// Gets the Last Step of the Measurement Range 
		/// </summary>
		[DescriptionAttribute("Gets the Last Step of the Measurement Range")]
		[CategoryAttribute("Hardware Capabilities")]
		public abstract int ValidMaximumAngularStep { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Gets the cosine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the cosine is desired</param>
		/// <returns>The cosine value of the angle at provided step.</returns>
		public virtual double GetCosFromStep(int step)
		{
			if ((step < 0) || (step > AbsoluteMaximumAngularStep))
				throw new ArgumentOutOfRangeException();
			if ((cosByStep == null) || (step >= cosByStep.Length))
				PrecalculateStepSinesAndCosines();
			return cosByStep[step];
		}

		/// <summary>
		/// Gets the sine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the sine is desired</param>
		/// <returns>The sine value of the angle at provided step.</returns>
		public virtual double GetSinFromStep(int step)
		{
			if ((step < 0) || (step > AbsoluteMaximumAngularStep))
				throw new ArgumentOutOfRangeException();
			if ((sinByStep == null) || (step >= sinByStep.Length))
				PrecalculateStepSinesAndCosines();
			return sinByStep[step];
		}

		/// <summary>
		/// Performs the asynchronous device reading operation
		/// </summary>
		protected abstract void MainThreadTask();

		/// <summary>
		/// Raises the Error event
		/// </summary>
		/// <param name="error">IError object containing information about the error</param>
		protected virtual void OnError(ISensorError error)
		{
			base.OnError(this, error);
		}

		/// <summary>
		/// Raises the ReadCompleted event
		/// </summary>
		/// <param name="readings">Array of readings taken from the sensor</param>
		protected virtual void OnReadCompleted(LaserReading[] readings)
		{
			base.OnReadCompleted(this, readings);
		}

		/// <summary>
		/// Raises the TresholdExceeded event
		/// </summary>
		protected void OnTresholdExceeded()
		{
			base.OnTresholdExceeded(this);
		}

		/// <summary>
		/// Precalculates the Sine and Cosine values for the angle of each step
		/// </summary>
		protected void PrecalculateStepSinesAndCosines()
		{
			if (AbsoluteMaximumAngularStep < 0)
				throw new Exception("AbsoluteMaximumAngularStep value must be greater than zero");

			double radians;
			this.cosByStep = new double[AbsoluteMaximumAngularStep];
			this.sinByStep = new double[AbsoluteMaximumAngularStep];
			double step = 2 * Math.PI / AbsoluteMaximumAngularStep;

			for (int i = 0; i < AbsoluteMaximumAngularStep; ++i)
			{
				radians = (i - AngularStepZero) * step;
				cosByStep[i] = Math.Cos(radians);
				sinByStep[i] = Math.Sin(radians);
			}
		}

		/// <summary>
		/// Syncronusly reads the sensor
		/// </summary>
		/// <param name="readings">When this method returns contains the array of sensor readings if the sensor was readed successfully, null otherwise</param>
		/// <returns>true if read from the sensor was completed successfully, false otherwise</returns>
		public abstract bool Read(out ITelemetricReading[] readings);

		/// <summary>
		/// Starts to take asynchronous readings from the Laser
		/// </summary>
		public override void Start()
		{
			if ((mainThread != null) && mainThread.IsAlive)
				return;
			mainThread = new Thread(new ThreadStart(MainThreadTask));
			mainThread.IsBackground = true;
			mainThread.Start();
			running = true;
			status = LaserStatus.Running;
		}

		/// <summary>
		/// Stops the Laser
		/// </summary>
		public override void Stop()
		{
			if ((mainThread == null) || !mainThread.IsAlive)
				return;

			running = false;
			mainThread.Join(100);
			if (mainThread.IsAlive)
			{
				mainThread.Abort();
				mainThread.Join();
			}
			status = LaserStatus.Stoped;
		}

		/// <summary>
		/// Returns a string representation of the HokuyoLaser object
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string model;

			if (String.IsNullOrEmpty(this.Information.Model))
				model = "Unknown device";
			else
				model = this.Information.Model;
			return model;
		}

		#endregion
	}
}
/*
namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Enumerates the laser status modes
	/// </summary>
	public enum LaserStatus
	{
		/// <summary>
		/// Unknown status or error
		/// </summary>
		Error = -1,
		/// <summary>
		/// Laser is stoped.
		/// </summary>
		Stoped = 0x00,
		/// <summary>
		/// The laser is running
		/// </summary>
		Running = 0x01,
		/// <summary>
		/// Laser is running but no lectures are taken
		/// </summary>
		Suspended = 0x02
	};

	/// <summary>
	/// Represents a telemetric laser sensor
	/// </summary>
	public abstract class Laser : TelemetricSensor, IPolarDevice
	{
		#region Variables

		/// <summary>
		/// Stores the Cos value for the angle at specified measurement step
		/// </summary>
		private double[] cosByStep;

		/// <summary>
		/// Data density
		/// </summary>
		protected int density;
		
		/// <summary>
		/// Number of errors in the last lecture
		/// </summary>
		protected int errorsOnLastReading;

		/// <summary>
		/// Information of the laser device
		/// </summary>
		protected DeviceInfo info;

		/// <summary>
		/// Main thread for async read operations
		/// </summary>
		protected Thread mainThread;

		/// <summary>
		/// Main Thread execution flag
		/// </summary>
		protected bool running;

		/// <summary>
		/// Stores the Sin value for the angle at specified measurement step
		/// </summary>
		private double[] sinByStep;

		/// <summary>
		/// Laser device current status
		/// </summary>
		protected LaserStatus status = LaserStatus.Stoped;

		#endregion

		#region Constructors
		#endregion

		#region Events

		#endregion

		#region Properties

		/// <summary>
		/// Gets the Absolute last step the device can reach
		/// </summary>
		public abstract int AbsoluteMaximumAngularStep { get; }
		
		/// <summary>
		/// Gets the smallest angle change the device can detect or rotate
		/// </summary>
		public abstract double AngularResolution { get; }

		/// <summary>
		/// Gets the angle resollution bits
		/// </summary>
		public abstract int AngularResolutionBits { get; }

		/// <summary>
		/// Step number on the sensor's front axis
		/// </summary>
		public abstract int AngularStepZero { get; }

		/// <summary>
		/// Stores the Cos value for the angle at specified measurement step
		/// </summary>
		protected double[] CosByStep
		{
			get { return cosByStep; }
		}

		/// <summary>
		/// Gets the number of mistaken readings in the last read
		/// </summary>
		public override int ErrorCount
		{
			get { return errorsOnLastReading; }
			protected set
			{
				if ((value < 0) || (value > AbsoluteMaximumAngularStep))
					errorsOnLastReading = value;
			}
		}

		/// <summary>
		/// Returns the device's information 
		/// </summary>
		public virtual DeviceInfo Information
		{
			get {return info;}
		}

		/// <summary>
		/// Performs the asynchronous device reading operation
		/// </summary>
		protected abstract void MainThreadTask();

		/// <summary>
		/// Gets the maximum angle in radians the sensor can detect measured from the front of the sensor
		/// </summary>
		public abstract double MaximumAngle { get; }

		/// <summary>
		/// Gets the minumim angle in radians the sensor can detect measured from the front of the sensor
		/// </summary>
		public abstract double MinimumAngle { get; }

		/// <summary>
		/// Stores the Sin value for the angle at specified measurement step
		/// </summary>
		protected double[] SinByStep
		{
			get { return sinByStep; }
		}

		/// <summary>
		/// Gets the number of steps in a complete revolution (360º or two pi radians)
		/// </summary>
		public abstract int StepsPerRevolution { get; }

		/// <summary>
		/// Gets the First Step of the Measurement Range 
		/// </summary>
		public abstract int ValidMinimumAngularStep { get; }

		/// <summary>
		/// Gets the Last Step of the Measurement Range 
		/// </summary>
		public abstract int ValidMaximumAngularStep { get; }

		#endregion

		#region Methodos

		/// <summary>
		/// Gets the cosine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the cosine is desired</param>
		/// <returns>The cosine value of the angle at provided step.</returns>
		public virtual double GetCosFromStep(int step)
		{
			if ((step < 0) || (step > AbsoluteMaximumAngularStep))
				throw new ArgumentOutOfRangeException();
			if ((cosByStep == null)|| (step >= cosByStep.Length))
				PrecalculateStepSinesAndCosines();
			return cosByStep[step];
		}
		
		/// <summary>
		/// Gets the sine value of the angle at provided step.
		/// Values provided are precalculated
		/// </summary>
		/// <param name="step">The step for which angle the sine is desired</param>
		/// <returns>The sine value of the angle at provided step.</returns>
		public virtual double GetSinFromStep(int step)
		{
			if ((step < 0) || (step > AbsoluteMaximumAngularStep))
				throw new ArgumentOutOfRangeException();
			if ((sinByStep == null) || (step >= sinByStep.Length))
				PrecalculateStepSinesAndCosines();
			return sinByStep[step];
		}

		/// <summary>
		/// Precalculates the Sine and Cosine values for the angle of each step
		/// </summary>
		protected void PrecalculateStepSinesAndCosines()
		{
			if (AbsoluteMaximumAngularStep < 0)
				throw new Exception("AbsoluteMaximumAngularStep value must be greater than zero");

			double radians;
			this.cosByStep = new double[AbsoluteMaximumAngularStep];
			this.sinByStep = new double[AbsoluteMaximumAngularStep];
			double step = 2 * Math.PI / AbsoluteMaximumAngularStep;

			for (int i = 0; i < AbsoluteMaximumAngularStep; ++i)
			{
				radians = (i - AngularStepZero) * step;
				cosByStep[i] = Math.Cos(radians);
				sinByStep[i] = Math.Sin(radians);
			}
		}

		/// <summary>
		/// Starts to take asynchronous readings from the Laser
		/// </summary>
		public override void Start()
		{
			if ((mainThread != null) && mainThread.IsAlive)
				return;
			mainThread = new Thread(new ThreadStart(MainThreadTask));
			mainThread.IsBackground = true;
			mainThread.Start();
			running = true;
			status = LaserStatus.Running;
		}

		/// <summary>
		/// Stops the Laser
		/// </summary>
		public override void Stop()
		{
			if ((mainThread == null) || !mainThread.IsAlive)
				return;

			running = false;
			mainThread.Join(100);
			if(mainThread.IsAlive)
			{
				mainThread.Abort();
				mainThread.Join();
			}
			status = LaserStatus.Stoped;
		}

		#endregion
		
	}
}
*/
