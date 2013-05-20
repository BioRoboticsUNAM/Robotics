using System;

namespace Robotics.HAL.Sensors
{
	/// <summary>
	/// Represents the method that will handle the Error event of a ISensor derived class
	/// </summary>
	/// <param name="sensor">The ISensor object which raises the event</param>
	/// <param name="error">The ISensorError object that contains information about the error</param>
	public delegate void SensorErrorEventHandler<TSensor>(TSensor sensor, ISensorError error)
		where TSensor : ISensor;

	/// <summary>
	/// Represents the method that will handle the ReadComplete event of a ISensor derived class
	/// </summary>
	/// <param name="sensor">The ISensor object which raises the event</param>
	/// <param name="read">Array of readings obtained from the sensor</param>
	public delegate void SensorReadingCompletedEventHandler<TSensor, TReading>(TSensor sensor, TReading[] read)
		where TSensor : ISensor
		where TReading : ISensorReading<TSensor>;

	/// <summary>
	/// Represents a generic sensor
	/// </summary>
	public interface ISensor : IDisposable
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating if the communication port with the device is open
		/// </summary>
		bool IsOpen
		{
			get;

		}

		/// <summary>
		/// Gets a value indicating if the continous asynchronous read operation of the sensor has been started
		/// </summary>
		bool Started
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Connects to the sensor device
		/// </summary>
		void Connect();

		/// <summary>
		/// Disconnects from the sensor device
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Starts the continous asynchronous read of the sensor
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the continous asynchronous read of the sensor
		/// </summary>
		void Stop();

		#endregion
	}

	/// <summary>
	/// Represents a generic sensor
	/// </summary>
	public interface ISensor<TSensor, TReading> : ISensor
		where TSensor : ISensor
		where TReading : ISensorReading<TSensor>
	{
		#region Properties

		/// <summary>
		/// Gets the last reading array obtained from the sensor
		/// </summary>
		TReading[] LastReadings { get; }

		#endregion

		#region Events

		/// <summary>
		/// Raises when an error occurs
		/// </summary>
		event SensorErrorEventHandler<TSensor> Error;

		/// <summary>
		/// Raises when the sensor completes an asynchronous read operation
		/// </summary>
		event SensorReadingCompletedEventHandler<TSensor, TReading> ReadCompleted;
		
		#endregion

		#region Methods

		/// <summary>
		/// Syncronusly reads the sensor
		/// </summary>
		/// <returns>Array of sensor readings</returns>
		bool Read(out TReading[] readings);

		#endregion
	}

	/*

	/// <summary>
	/// Represents the method that will handle the Error event of a ISensor derived class
	/// </summary>
	/// <param name="sensor">The ISensor object which raises the event</param>
	/// <param name="error">The ISensorError object that contains information about the error</param>
	public delegate void SensorErrorEventHandler<TReading>(ISensor<TReading> sensor, ISensorError error)
		where TSensor : ISensorReading<ISensor<TSensor>>;

	/// <summary>
	/// Represents the method that will handle the ReadComplete event of a ISensor derived class
	/// </summary>
	/// <param name="sensor">The ISensor object which raises the event</param>
	/// <param name="read">Array of readings obtained from the sensor</param>
	public delegate void SensorReadingCompletedEventHandler<S, U>(S sensor, U[] read)
		where S : ISensor<U>
		where U : ISensorReading<S>;
	 
	/// <summary>
	/// Represents a generic sensor
	/// </summary>
	public interface ISensor
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating if the communication port with the device is open
		/// </summary>
		bool IsOpen
		{
			get;

		}

		/// <summary>
		/// Gets a value indicating if the continous asynchronous read operation of the sensor has been started
		/// </summary>
		bool Started
		{
			get;
		}

		#endregion

		#region Events

		/// <summary>
		/// Raises when an error occurs
		/// </summary>
		event SensorErrorEventHandler<TReading> Error;

		/// <summary>
		/// Raises when the sensor completes an asynchronous read operation
		/// </summary>
		event SensorReadingCompletedEventHandler<ISensor<TReading>, ISensorReading<ISensor<TReading>>> ReadCompleted;

		#endregion

		#region Methods

		/// <summary>
		/// Connects to the sensor device
		/// </summary>
		void Connect();

		/// <summary>
		/// Disconnects from the sensor device
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Syncronusly reads the sensor
		/// </summary>
		/// <returns>Array of sensor readings</returns>
		bool Read(out ISensorReading<ISensor<TReading>>[] readings);

		/// <summary>
		/// Starts the continous asynchronous read of the sensor
		/// </summary>
		void Start();

		/// <summary>
		/// Stops the continous asynchronous read of the sensor
		/// </summary>
		void Stop();

		#endregion
	}

	*/
}