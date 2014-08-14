using System;
using System.Text.RegularExpressions;

namespace Robotics.HAL.Sensors
{
	/// <summary>
	/// Represents a human Face
	/// </summary>
	public class HumanFace
	{

		#region Constants

		/// <summary>
		/// The maximum horizontal deviation angle of the centroid of the detected human face from the center of the image in radians
		/// </summary>
		public const double MaximumPan = 1.05;

		/// <summary>
		/// The maximum vertical deviation angle of the centroid of the detected human face from the center of the image in radians
		/// </summary>
		public const double MaximumTilt = 1.05;

		/// <summary>
		/// The maximum horizontal deviation angle of the centroid of the detected human face from the center of the image in radians
		/// </summary>
		public const double MinimumPan = -1.05;

		/// <summary>
		/// The minimum vertical deviation angle of the centroid of the detected human face from the center of the image in radians
		/// </summary>
		public const double MinimumTilt = -1.05;

		#endregion

		#region Variables

		/// <summary>
		/// The asociated name to the detected human face
		/// </summary>
		private string name;

		/// <summary>
		/// The horizontal deviation angle of the centroid of the detected human face from the center of the image
		/// </summary>
		private double pan;

		/// <summary>
		/// The vertical deviation angle of the centroid of the detected human face from the center of the image
		/// </summary>
		private double tilt;

		/// <summary>
		/// Value between 0 and 1 that represents the confidence of the detected human face respect to its pattern
		/// </summary>
		private double confidence;

		/// <summary>
		/// Regular expression used to validate names
		/// </summary>
		public static readonly Regex RxNameValidator = new Regex(@"[A-Za-z_][\w\s]*", RegexOptions.Compiled);

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of HumanFace
		/// </summary>
		public HumanFace()
		{
			this.Name = "Unknown";
			this.Pan = 0;
			this.Tilt = 0;
			this.Confidence = 0;
		}

		/// <summary>
		/// Initializes a new instance of HumanFace
		/// </summary>
		/// <param name="name">The asociated name to the detected human face</param>
		/// <param name="pan">The horizontal deviation angle of the centroid of the detected human face from the center of the image</param>
		/// <param name="tilt">The vertical deviation angle of the centroid of the detected human face from the center of the image</param>
		/// <param name="confidence">Value between 0 and 1 that represents the confidence of the detected human face respect to its pattern</param>
		public HumanFace(string name, double pan, double tilt, double confidence)
		{
			this.Name = name;
			this.Pan = pan;
			this.Tilt = tilt;
			this.Confidence = confidence;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the asociated name to the detected human face
		/// </summary>
		public string Name
		{
			get { return this.name; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				if (!RxNameValidator.IsMatch(value))
					throw new ArgumentException("Invalid input string (name)");
				this.name = value;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal deviation angle of the centroid of the detected human face from the center of the image
		/// </summary>
		public double Pan
		{
			get { return this.pan; }
			set
			{
				if ((value < -1.57) || (value > 1.57))
					throw new ArgumentOutOfRangeException();
				this.pan = value;
			}
		}

		/// <summary>
		/// Gets or sets the vertical deviation angle of the centroid of the detected human face from the center of the image
		/// </summary>
		public double Tilt
		{
			get { return this.tilt; }
			set
			{
				if ((value < -1.57) || (value > 1.57))
					throw new ArgumentOutOfRangeException();
				this.tilt = value;
			}
		}

		/// <summary>
		/// Gets or sets a value between 0 and 1 that represents the confidence of the detected human face respect to its pattern
		/// </summary>
		public double Confidence
		{
			get { return this.confidence; }
			set
			{
				if ((value > 1) || (value < 0))
					throw new ArgumentOutOfRangeException();
				this.confidence = value;
			}
		}

		#endregion
	}
}
