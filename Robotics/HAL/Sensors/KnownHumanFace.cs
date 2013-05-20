using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.HAL.Sensors
{
	/// <summary>
	/// Represents a known human Face
	/// </summary>
	public class KnownHumanFace
	{
		#region Variables

		/// <summary>
		/// The asociated name to the human face
		/// </summary>
		private string name;

		/// <summary>
		/// The number of patterns trained asociated to the face name
		/// </summary>
		private int patterns;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of HumanFace
		/// </summary>
		public KnownHumanFace()
		{
			this.name = "Unknown";
			this.patterns = 0;
		}

		/// <summary>
		/// Initializes a new instance of HumanFace
		/// </summary>
		/// <param name="name">The asociated name to the human face</param>
		/// <param name="patterns">The number of patterns trained asociated to the face name</param>
		public KnownHumanFace(string name, int patterns)
		{
			this.Name = name;
			this.Patterns = patterns;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the asociated name to the human face
		/// </summary>
		public string Name
		{
			get { return this.name; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				if (!HumanFace.RxNameValidator.IsMatch(value) || (String.Compare("unknown", value, true) == 0))
					throw new ArgumentException("Invalid input string");
				this.name = value;
			}
		}

		/// <summary>
		/// Gets or sets the number of patterns trained asociated to the face name
		/// </summary>
		public int Patterns
		{
			get { return this.patterns; }
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException();
				this.patterns = value;
			}
		}

		#endregion
	}
}
