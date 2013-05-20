using System;

namespace Robotics.HAL.Sensors.Telemetric
{
	/// <summary>
	/// Encapsulates data from a single reading of a Laser telemetric sensor device
	/// </summary>
	public class LaserReading : ITelemetricReading, ISensorReading<Laser>
	{

		#region Variables

		/// <summary>
		/// Angle of the reading measured from the front of the lasser in DEGREES 
		/// </summary>
		private double angleDeg;

		/// <summary>
		/// Angle of the reading measured from the front of the lasser in RADIANS
		/// </summary>
		private double angleRad;

		/// <summary>
		/// Distance between the lasser and the closets object in MILLIMETERS
		/// </summary>
		private int distanceMM;

		/// <summary>
		/// Distance between the lasser and the closets object in METERS
		/// </summary>
		private double distance;

		/// <summary>
		/// Indicates if the reading is erroneous
		/// </summary>
		private bool err;

		/// <summary>
		/// The error code if any
		/// </summary>
		private int errorCode;

		/// <summary>
		/// The Laser object source of this reading
		/// </summary>
		private Laser laser;

		/// <summary>
		/// Indicates if an obstacle was detected by this reading
		/// </summary>
		private bool obstacleDetected;

		/// <summary>
		/// Step at which reading was taken
		/// </summary>
		private int step;

		/// <summary>
		/// The x-coordinate of the cartesian transform of this reading
		/// </summary>
		private double x;

		/// <summary>
		/// The y-coordinate of the cartesian transform of this reading
		/// </summary>
		private double y;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the LaserReading class
		/// </summary>
		/// <param name="laser">The Laser object source of this reading</param>
		/// <param name="angle">Relative angle in radians of the reading measured from the front of the laser</param>
		/// <param name="distance">Distance in meters to the closest object</param>
		public LaserReading(Laser laser, double angle, double distance)
		{
			//if (laser == null)
			//	throw new ArgumentNullException();
			this.laser = laser;
			this.angleDeg = 180 * angle / Math.PI;
			while (angle > Math.PI / 2)
				angle -= 2 * Math.PI;
			while (angle < -Math.PI)
				angle += 2 * Math.PI;
			this.angleRad = angle;
			this.distance = distance;
			this.distanceMM = (int)(distance * 1000);
			this.err = false;
			this.errorCode = -1;
			if(laser != null)
				this.obstacleDetected = distance < laser.MaximumDistance;
			this.step = -1;
			this.x = distance * Math.Cos(angleRad);
			this.y = distance * Math.Sin(angleRad);
		}

		/// <summary>
		/// Initializes a new instance of the LaserReading class
		/// </summary>
		/// <param name="laser">The Laser object source of this reading</param>
		/// <param name="angle">Relative angle in radians of the reading measured from the front of the laser</param>
		/// <param name="distance">Distance in meters to the closest object</param>
		/// <param name="mistaken">Specifies if the reading is mistaken</param>
		public LaserReading(Laser laser, double angle, double distance, bool mistaken)
			: this(laser, angle, distance)
		{
			this.err = mistaken;
			this.obstacleDetected = !this.err;
		}

		/// <summary>
		/// Initializes a new instance of the LaserReading class
		/// </summary>
		/// <param name="laser">The Laser object source of this reading</param>
		/// <param name="step">Step at whick lecture was taken</param>
		/// <param name="angle">Relative angle in radians of the reading measured from the front of the laser</param>
		/// <param name="distance">Distance in meters to the closest object</param>
		/// <param name="mistaken">Specifies if the reading is mistaken</param>
		public LaserReading(Laser laser, int step, double angle, double distance, bool mistaken)
			: this(laser, angle, distance, mistaken)
		{
			this.step = step;
		}

		/// <summary>
		/// Initializes a new instance of the LaserReading class
		/// </summary>
		/// <param name="laser">The Laser object source of this reading</param>
		/// <param name="step">Step at whick lecture was taken</param>
		/// <param name="angle">Relative angle in radians of the reading measured from the front of the laser</param>
		/// <param name="distance">Distance in meters to the closest object</param>
		/// <param name="errorCode">Specifies the error code of the lecture. A value of -1 means no error</param>
		public LaserReading(Laser laser, int step, double angle, double distance, int errorCode)
			: this(laser, angle, distance, errorCode != -1)
		{
			this.step = step;
			this.errorCode = errorCode;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the angle of the reading measured from the front of the lasser in RADIANS
		/// </summary>
		public double Angle
		{
			get { return angleRad; }
		}

		/// <summary>
		/// Gets the angle of the reading measured from the front of the lasser in DEGREES
		/// </summary>
		public double AngleDegrees
		{
			get { return angleDeg; }
		}

		/// <summary>
		/// Gets the distance between the lasser and the closets object in METERS
		/// </summary>
		public double Distance
		{
			get { return distance; }
		}

		/// <summary>
		/// Distance between the lasser and the closets object in millimeters
		/// </summary>
		public int DistanceMillimeters
		{
			get { return distanceMM; }
		}

		/// <summary>
		/// Tells if the reading is mistaken
		/// </summary>
		public bool Mistaken
		{
			get { return err; }
		}

		/// <summary>
		/// Flag that indicates if an obstacle was detected by this reading
		/// </summary>
		public bool ObstacleDetected
		{
			get { return obstacleDetected; }
		}

		/// <summary>
		/// Gets the Laser object source of this measurement
		/// </summary>
		public Laser Sensor
		{
			get { return laser; }
		}

		/// <summary>
		/// Gets the ITelemetricSensor object source of this measurement
		/// </summary>
		ITelemetricSensor ISensorReading<ITelemetricSensor>.Sensor
		{
			get { return laser; }
		}

		/// <summary>
		/// Gets the x-coordinate of the cartesian transform of this reading in meters
		/// </summary>
		public double X
		{
			get { return this.x; }
			internal set { this.x = value; }
		}

		/// <summary>
		/// Gets the y-coordinate of the cartesian transform of this reading in meters
		/// </summary>
		public double Y
		{
			get { return this.y; }
			internal set { this.y = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <returns>A String that represents the current Object</returns>
		public override string ToString()
		{
			return "{ " + distance.ToString("0.00") + "," + angleRad.ToString("#0.000") + " }";
		}

		#endregion

		#region ISensorReading<ITelemetricSensor> Members

		

		#endregion
	}
/*
	/// <summary>
	/// Encapsulates data from a single reading of a Laser telemetric sensor device
	/// </summary>
	public class LaserReading : ITelemetricReading, ISensorReading<Laser>
	{

		#region Variables

		/// <summary>
		/// Angle of the reading measured from the front of the lasser in DEGREES 
		/// </summary>
		private double angleDeg;

		/// <summary>
		/// Angle of the reading measured from the front of the lasser in RADIANS
		/// </summary>
		private double angleRad;

		/// <summary>
		/// Distance between the lasser and the closets object in MILLIMETERS
		/// </summary>
		private int distanceMM;

		/// <summary>
		/// Distance between the lasser and the closets object in METERS
		/// </summary>
		private double distance;

		/// <summary>
		/// Indicates if the reading is erroneous
		/// </summary>
		private bool err;

		/// <summary>
		/// The error code if any
		/// </summary>
		private int errorCode;

		/// <summary>
		/// The Laser object source of this reading
		/// </summary>
		private Laser laser;

		/// <summary>
		/// Indicates if an obstacle was detected by this reading
		/// </summary>
		private bool obstacleDetected;

		/// <summary>
		/// Step at which reading was taken
		/// </summary>
		private int step;

		/// <summary>
		/// The x-coordinate of the cartesian transform of this reading
		/// </summary>
		private double x;

		/// <summary>
		/// The y-coordinate of the cartesian transform of this reading
		/// </summary>
		private double y;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the LaserReading class
		/// </summary>
		/// <param name="laser">The Laser object source of this reading</param>
		/// <param name="angle">Relative angle in radians of the reading measured from the front of the laser</param>
		/// <param name="distance">Distance in meters to the closest object</param>
		public LaserReading(Laser laser, double angle, double distance)
		{
			//if (laser == null)
			//	throw new ArgumentNullException();
			this.laser = laser;
			this.angleDeg = 180 * angle / Math.PI;
			while (angle > Math.PI / 2)
				angle -= 2 * Math.PI;
			while (angle < -Math.PI)
				angle += 2 * Math.PI;
			this.angleRad = angle;
			this.distanceMM = (int)(this.distance * 1000);
			this.distance = distance;
			this.err = false;
			this.errorCode = -1;
			this.obstacleDetected = distance < laser.MaximumDistance;
			this.step = -1;
			this.x = distance * Math.Cos(angleRad);
			this.y = distance * Math.Sin(angleRad);
		}

		/// <summary>
		/// Initializes a new instance of the LaserReading class
		/// </summary>
		/// <param name="laser">The Laser object source of this reading</param>
		/// <param name="angle">Relative angle in radians of the reading measured from the front of the laser</param>
		/// <param name="distance">Distance in meters to the closest object</param>
		/// <param name="mistaken">Specifies if the reading is mistaken</param>
		public LaserReading(Laser laser, double angle, double distance, bool mistaken)
			: this(laser, angle, distance)
		{
			this.err = mistaken;
			this.obstacleDetected = !this.err;
		}

		/// <summary>
		/// Initializes a new instance of the LaserReading class
		/// </summary>
		/// <param name="laser">The Laser object source of this reading</param>
		/// <param name="step">Step at whick lecture was taken</param>
		/// <param name="angle">Relative angle in radians of the reading measured from the front of the laser</param>
		/// <param name="distance">Distance in meters to the closest object</param>
		/// <param name="mistaken">Specifies if the reading is mistaken</param>
		public LaserReading(Laser laser, int step, double angle, double distance, bool mistaken)
			: this(laser, angle, distance, mistaken)
		{
			this.step = step;
		}

		/// <summary>
		/// Initializes a new instance of the LaserReading class
		/// </summary>
		/// <param name="laser">The Laser object source of this reading</param>
		/// <param name="step">Step at whick lecture was taken</param>
		/// <param name="angle">Relative angle in radians of the reading measured from the front of the laser</param>
		/// <param name="distance">Distance in meters to the closest object</param>
		/// <param name="errorCode">Specifies the error code of the lecture. A value of -1 means no error</param>
		public LaserReading(Laser laser, int step, double angle, double distance, int errorCode)
			: this(laser, angle, distance, errorCode != -1)
		{
			this.step = step;
			this.errorCode = errorCode;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the angle of the reading measured from the front of the lasser in RADIANS
		/// </summary>
		public double Angle
		{
			get { return angleRad; }
		}

		/// <summary>
		/// Gets the angle of the reading measured from the front of the lasser in DEGREES
		/// </summary>
		public double AngleDegrees
		{
			get { return angleDeg; }
		}

		/// <summary>
		/// Gets the distance between the lasser and the closets object in METERS
		/// </summary>
		public double Distance
		{
			get { return distance; }
		}

		/// <summary>
		/// Distance between the lasser and the closets object in millimeters
		/// </summary>
		public int DistanceMillimeters
		{
			get { return distanceMM; }
		}

		/// <summary>
		/// Tells if the reading is mistaken
		/// </summary>
		public bool Mistaken
		{
			get { return err; }
		}

		/// <summary>
		/// Flag that indicates if an obstacle was detected by this reading
		/// </summary>
		public bool ObstacleDetected
		{
			get { return obstacleDetected; }
		}

		/// <summary>
		/// Gets the Laser object source of this measurement
		/// </summary>
		public Laser Sensor
		{
			get { return laser; }
		}

		/// <summary>
		/// Gets the ISensorReading object source of this measurement
		/// </summary>
		TelemetricSensor ISensorReading<TelemetricSensor>.Sensor
		{
			get { return laser; }
		}

		/// <summary>
		/// Gets the x-coordinate of the cartesian transform of this reading in meters
		/// </summary>
		public double X
		{
			get { return this.x; }
			internal set { this.x = value; }
		}

		/// <summary>
		/// Gets the y-coordinate of the cartesian transform of this reading in meters
		/// </summary>
		public double Y
		{
			get { return this.y; }
			internal set { this.y = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <returns>A String that represents the current Object</returns>
		public override string ToString()
		{
			return "{ " + distanceMM.ToString("0.00") + "," + angleRad.ToString("#0.000") + " }";
		}

		#endregion
	}
*/
}
