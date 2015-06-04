using System;
using System.Collections.Generic;
using System.Text;
using Robotics.API;
using Robotics.HAL.Sensors.Telemetric;

namespace Robotics.API.MiscSharedVariables
{
	/// <summary>
	/// Gets access to an array of LaserReading variable stored in the Blackboard
	/// </summary>
	public class LaserReadingArrayShV : SharedVariable<LaserReading[]>
	{
		#region Variables

		/*
		/// <summary>
		/// Chars used to split strings
		/// </summary>
		private static char[] SplitChars = { ' ', '\t', '\r', '\n' };
		*/

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public LaserReadingArrayShV(CommandManager commandManager, string variableName, bool initialize)
			: this(commandManager, variableName, null, initialize) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="values">The value to store in the shared variable if it does not exist</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public LaserReadingArrayShV(CommandManager commandManager, string variableName, LaserReading[] values, bool initialize)
			: base(commandManager, variableName, values, initialize) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public LaserReadingArrayShV(string variableName)
			: this(null, variableName, null, false) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="values">The value to store in the shared variable if it does not exist</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public LaserReadingArrayShV(string variableName, LaserReading[] values)
			: this(null, variableName, values, false) { }

		#endregion

		#region Events

		#endregion

		#region Properties

		/// <summary>
		/// Returns false
		/// </summary>
		public override bool IsArray
		{
			get { return false; }
		}

		/// <summary>
		/// Returns -1
		/// </summary>
		public override int Length { get { return -1; } }

		/// <summary>
		/// Returns "int"
		/// </summary>
		public override string TypeName
		{
			get { return "LaserReadingArray"; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Deserializes an array of doubles from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		protected override bool Deserialize(string serializedData, out LaserReading[] values)
		{
			/*
			int rows;
			int cols;
			int xPos;
			int sPos;

			value = null;
			if (String.IsNullOrEmpty(serializedData))
				return true;

			xPos = serializedData.IndexOf('x');
			sPos = serializedData.IndexOf(' ');
			if ((xPos < 1) || (sPos < 3) || (sPos <= xPos + 1))
				return false;
			if (!Int32.TryParse(serializedData.Substring(0, xPos), out rows) || rows < 1)
				return false;
			if (!Int32.TryParse(serializedData.Substring(xPos +1 , sPos - (xPos+1)), out cols) || cols < 1)
				return false;
			return Deserialize(serializedData.Substring(sPos + 1), rows, cols, out value);			
			*/
			values = null;
			return false;
		}

		/// <summary>
		/// Deserializes an array of from a string which starts with "double[]" and is followed only by double representations
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="rows">Number of matrix rows</param>
		/// <param name="cols">Number of matrix columns</param>
		/// <param name="values">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		private bool Deserialize(string serializedData, int rows, int cols, out LaserReading[] values)
		{
			/*
			int length;
			string[] parts;
			double[] elements;
			
			value = null;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
			length = rows * cols;
			if (parts.Length != length)
				return false;
			elements = new double[length];
			for (int i = 0; i < length; ++i)
			{
				if (!Double.TryParse(parts[i], out elements[i]))
					return false;
			}
			if (elements.Length != length)
				return false;
			value = new Matrix(elements, rows, cols);
			return true;
			*/
			values = null;
			return false;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		protected override bool Serialize(LaserReading[] values, out string serializedData)
		{
			/*
			serializedData = null;
			StringBuilder sb;
			int count = 0;
			double minAngle = 0;
			double step = 0;

			if (values == null)
			{
				serializedData = String.Empty;
				return true;
			}
			else if (values.Length == 0)
			{
				count = 0;
				minAngle = 0;
				step = 0;
			}
			else if (values.Length == 1)
			{
				count = 1;
				minAngle = values[0].Angle;
				step = 0;
			}
			else
			{
				count = values.Length;
				minAngle = values[0].Angle;
				step = values[0].Angle - values[1].Angle;

			}
			sb = new StringBuilder(50 + values.Length * 6);

			sb.Append("count ");
			sb.Append(count);
			sb.Append("start ");
			sb.AppendFormat(minAngle.ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
			sb.Append("step ");
			sb.AppendFormat(step.ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat));

			for (int i = 0; i < values.Length; ++i)
			{
					if (Double.IsNaN(values[i].Distance) || Double.IsInfinity(value[i].Distance))
						return false;
					sb.Append(' ');
					sb.AppendFormat(values[i, j].ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
			}
			serializedData = sb.ToString();

			return true;
			*/
			serializedData = null;
			return false;
		}

		#endregion
	}
}
