using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Represents a Telemetric sensor
	/// </summary>
	public abstract class TelemetricSensor<TSensor, TReading> : ITelemetricSensor, ISensor<TSensor, TReading>
		where TSensor : ITelemetricSensor
		where TReading : ITelemetricReading, ISensorReading<TSensor>
	{
		#region Variables

		/// <summary>
		/// The proximity threshold used to determinate when the ThresholdExcedeed event is rised
		/// This value is the distance between the sensor and the closest object
		/// </summary>
		protected double treshold;

		/// <summary>
		/// Indicates if the current object is being disposed
		/// </summary>
		private bool isDisposed;

		/// <summary>
		/// Indicates if the current object is being disposed
		/// </summary>
		private bool disposing;

		#endregion

		#region Constructors

		/// <summary>
		/// Free asociated resources
		/// </summary>
		~TelemetricSensor()
		{
			// Finalizer calls Dispose(false)
			Dispose(false);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether the sensor class is in the process of disposing.
		/// </summary>
		public bool Disposing
		{
			get { return disposing; }
			protected set
			{
				if (isDisposed)
					disposing = false;
				else
					disposing = value;
			}
		}

		/// <summary>
		/// Gets the number of mistaken readings in the last read
		/// </summary>
		public abstract int ErrorCount { get; protected set; }

		/// <summary>
		/// Gets a value indicating whether the sensor has been disposed of.
		/// </summary>
		public bool IsDisposed
		{
			get { return isDisposed; }
			protected set
			{
				if (isDisposed)
				{
					disposing = false;
					return;
				}
				isDisposed = value;
			}
		}

		/// <summary>
		/// Gets a value indicating if the communication port with the device is open
		/// </summary>
		public abstract bool IsOpen { get; }

		/// <summary>
		/// Gets the last reading array obtained from the sensor
		/// </summary>
		public abstract TReading[] LastReadings { get; }

		/// <summary>
		/// Gets the minumim distance the sensor can detect
		/// </summary>
		public abstract double MinimumDistance { get; }

		/// <summary>
		/// Gets the maximum distance the sensor can detect
		/// </summary>
		public abstract double MaximumDistance { get; }

		/// <summary>
		/// Gets a value indicating if the continous asynchronous read operation of the sensor has been started
		/// </summary>
		public abstract bool Started { get; }

		/// <summary>
		/// Gets or sets the proximity threshold used to determinate when the ThresholdExcedeed event is rised
		/// This value is the distance between the sensor and the closest object
		/// </summary>
		public virtual double Treshold
		{
			get { return this.treshold; }
			set
			{
				if (value <= MinimumDistance)
					throw new ArgumentOutOfRangeException();
				this.treshold = value;
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises when an error occurs
		/// </summary>
		public event SensorErrorEventHandler<TSensor> Error;

		/// <summary>
		/// Occurs when a new set of readings is acquired from the sensor
		/// </summary>
		public event SensorReadingCompletedEventHandler<TSensor, TReading> ReadCompleted;

		/// <summary>
		/// Occurs when the threshold is exceeded
		/// </summary>
		public event TelemetricSensorThresholdExceededEventHandler<TSensor> TresholdExceeded;

		#endregion

		#region Methods

		/// <summary>
		/// Connects to the sensor device
		/// </summary>
		public abstract void Connect();

		/// <summary>
		/// Disconnects from the sensor device
		/// </summary>
		public abstract void Disconnect();

		/// <summary>
		/// Defines a method to release allocated unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			disposing = true;
			Dispose(true);
			isDisposed = true;
			disposing = false;
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// When overriden release allocated resources.
		/// </summary>
		/// <param name="disposing">Indicates if Dispose() method (true) was called or it is called by the Garbage Collector (false)</param>
		protected abstract void Dispose(bool disposing);

		/// <summary>
		/// Raises the Error event
		/// </summary>
		/// <param name="sensor">Sensor to be pased to the event</param>
		/// <param name="error">IError object containing information about the error</param>
		protected void OnError(TSensor sensor, ISensorError error)
		{
			try
			{
				if (Error != null)
					Error(sensor, error);
			}
			catch { }
		}

		/// <summary>
		/// Raises the ReadCompleted event
		/// </summary>
		/// <param name="sensor">Sensor to be pased to the event</param>
		/// <param name="readings">Array of readings taken from the sensor</param>
		protected void OnReadCompleted(TSensor sensor, TReading[] readings)
		{
			try
			{
				if (ReadCompleted != null)
					ReadCompleted(sensor, readings);
			}
			catch { }
		}

		/// <summary>
		/// Raises the TresholdExceeded event
		/// </summary>
		/// <param name="sensor">Sensor to be pased to the event</param>
		protected void OnTresholdExceeded(TSensor sensor)
		{
			try
			{
				if (this.TresholdExceeded != null)
					this.TresholdExceeded(sensor);
			}
			catch { }
		}

		/// <summary>
		/// Syncronusly reads the sensor
		/// </summary>
		/// <param name="readings">When this method returns contains the array of sensor readings if the sensor was readed successfully, null otherwise</param>
		/// <returns>true if read from the sensor was completed successfully, false otherwise</returns>
		public abstract bool Read(out TReading[] readings);

		/// <summary>
		/// Starts the continous asynchronous read of the sensor
		/// </summary>
		public abstract void Start();

		/// <summary>
		/// Stops the continous asynchronous read of the sensor
		/// </summary>
		public abstract void Stop();

		#endregion
	}
}

/*
namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Represents the method that will handle the ThresholdExceeded event of a TelemetricSensor object
	/// </summary>
	/// <param name="sensor">The TelemetricSensor object which rises the event</param>
	public delegate void TelemetricSensorThresholdExceededEventHandler<TSensor>(TSensor sensor)
	where TSensor : ITelemetricSensor;

	/// <summary>
	/// Represents a Telemetric sensor
	/// </summary>
	public abstract class TelemetricSensor<TSensor, TReading> : ITelemetricSensor
		where TSensor : ITelemetricSensor
		where TReading : ITelemetricReading, ISensorReading<TSensor>
	{
		

		

		

		

		

		

		#region ISensor<ITelemetricSensor,ITelemetricReading> Members

		public event SensorErrorEventHandler<ITelemetricSensor> Error;

		public event SensorReadingCompletedEventHandler<ITelemetricSensor, ITelemetricReading> ReadCompleted;

		#endregion
	}
}
*/
/*
namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Represents the method that will handle the ThresholdExceeded event of a TelemetricSensor object
	/// </summary>
	/// <param name="sensor">The TelemetricSensor object which rises the event</param>
	public delegate void TelemetricSensorThresholdExceededEventHandler(TelemetricSensor sensor);

	/// <summary>
	/// Represents a Telemetric sensor
	/// </summary>
	public abstract class TelemetricSensor : ISensor<ITelemetricReading>
	{
		#region Variables

		/// <summary>
		/// The proximity threshold used to determinate when the ThresholdExcedeed event is rised
		/// This value is the distance between the sensor and the closest object
		/// </summary>
		protected double treshold;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of mistaken readings in the last read
		/// </summary>
		public abstract int ErrorCount { get; protected set; }

		/// <summary>
		/// Gets a value indicating if the communication port with the device is open
		/// </summary>
		public abstract bool IsOpen { get; }

		/// <summary>
		/// Gets the last reading array obtained from the sensor
		/// </summary>
		public abstract ITelemetricReading[] LastReadings { get; }

		/// <summary>
		/// Gets the minumim distance the sensor can detect
		/// </summary>
		public abstract double MinimumDistance { get; }

		/// <summary>
		/// Gets the maximum distance the sensor can detect
		/// </summary>
		public abstract double MaximumDistance { get; }

		/// <summary>
		/// Gets a value indicating if the continous asynchronous read operation of the sensor has been started
		/// </summary>
		public abstract bool Started { get; }

		/// <summary>
		/// Gets or sets the proximity threshold used to determinate when the ThresholdExcedeed event is rised
		/// This value is the distance between the sensor and the closest object
		/// </summary>
		public virtual double Treshold
		{
			get { return this.treshold; }
			set
			{
				if (value <= MinimumDistance)
					throw new ArgumentOutOfRangeException();
				this.treshold = value;
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// 
		/// </summary>
		public event SensorErrorEventHandler<> Error;

		/// <summary>
		/// Occurs when a new set of readings is acquired from the sensor
		/// </summary>
		public event SensorReadingCompletedEventHandler<TelemetricSensor, ITelemetricReading> ReadCompleted;

		/// <summary>
		/// Occurs when a new set of readings is acquired from the sensor
		/// </summary>
		private event SensorReadingCompletedEventHandler<ISensor, ISensorReading<ISensor>> iReadCompleted;

		/// <summary>
		/// Occurs when a new set of readings is acquired from the sensor
		/// </summary>
		event SensorReadingCompletedEventHandler<ISensor, ISensorReading<ISensor>> ISensor.ReadCompleted
		{
			add { lock (iReadCompleted)iReadCompleted += value; }
			remove { lock (iReadCompleted)iReadCompleted -= value; }
		}

		/// <summary>
		/// Occurs when the threshold is exceeded
		/// </summary>
		public event TelemetricSensorThresholdExceededEventHandler TresholdExceeded;

		#endregion

		#region Methods

		/// <summary>
		/// Connects to the sensor device
		/// </summary>
		public abstract void Connect();

		/// <summary>
		/// Disconnects from the sensor device
		/// </summary>
		public abstract void Disconnect();

		/// <summary>
		/// Raises the Error event
		/// </summary>
		/// <param name="error">IError object containing information about the error</param>
		protected void OnError(ISensorError error)
		{
			try
			{
				if (Error != null)
					Error(this, error);
			}
			catch { }
		}

		/// <summary>
		/// Raises the ReadCompleted event
		/// </summary>
		/// <param name="readings">Array of readings taken from the sensor</param>
		protected void OnReadCompleted(ITelemetricReading[] readings)
		{
			try
			{
				if (iReadCompleted != null)
					iReadCompleted(this, (ISensorReading<ISensor>[])readings);
			}
			catch { }
			try
			{
				if (ReadCompleted != null)
					ReadCompleted(this, readings);
			}
			catch { }
		}

		/// <summary>
		/// Raises the TresholdExceeded event
		/// </summary>
		protected void OnTresholdExceeded()
		{
			try
			{
				if (this.TresholdExceeded != null)
					this.TresholdExceeded(this);
			}
			catch { }
		}

		/// <summary>
		/// Syncronusly reads the sensor
		/// </summary>
		/// <param name="readings">When this method returns contains the array of sensor readings if the sensor was readed successfully, null otherwise</param>
		/// <returns>true if read from the sensor was completed successfully, false otherwise</returns>
		bool ISensor.Read(out ISensorReading<ISensor>[] readings)
		{
			ITelemetricReading[] telemetricReadings;
			bool result = Read(out telemetricReadings);
			readings = (ISensorReading<ISensor>[])telemetricReadings;
			return result;
		}

		/// <summary>
		/// Syncronusly reads the sensor
		/// </summary>
		/// <param name="readings">When this method returns contains the array of sensor readings if the sensor was readed successfully, null otherwise</param>
		/// <returns>true if read from the sensor was completed successfully, false otherwise</returns>
		public abstract bool Read(out ITelemetricReading[] readings);

		/// <summary>
		/// Starts the continous asynchronous read of the sensor
		/// </summary>
		public abstract void Start();

		/// <summary>
		/// Stops the continous asynchronous read of the sensor
		/// </summary>
		public abstract void Stop();

		#endregion

		#region ISensor Members

		#endregion
	}
}
*/