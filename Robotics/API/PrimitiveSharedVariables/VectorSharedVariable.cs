using System;
using System.Collections.Generic;
using System.Text;
using Robotics.Mathematics;

namespace Robotics.API.PrimitiveSharedVariables
{
	/// <summary>
	/// Gets access to an ARRAY OF DOUBLES variable stored in the Blackboard
	/// </summary>
	public class VectorSharedVariable : SharedVariable<Vector>
	{
		#region Variables

		/// <summary>
		/// Chars used to split strings
		/// </summary>
		private static char[] SplitChars = { ' ', '\t', '\r', '\n' };

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public VectorSharedVariable(CommandManager commandManager, string variableName, bool initialize)
			: this(commandManager, variableName, null, initialize) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public VectorSharedVariable(CommandManager commandManager, string variableName, Vector value, bool initialize)
			: base(commandManager, variableName, value, initialize) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public VectorSharedVariable(string variableName)
			: this(null, variableName, null, false) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public VectorSharedVariable(string variableName, Vector value)
			: this(null, variableName, value, false) { }

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
		/// Returns "Vector"
		/// </summary>
		public override string TypeName
		{
			get { return "Vector"; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Deserializes an array of doubles from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		protected override bool Deserialize(string serializedData, out Vector value)
		{
			string[] parts;
			double data;
			List<double> iData = new List<double>();

			value = null;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Double.TryParse(parts[i], out data))
					return false;
				iData.Add(data);
			}
			value = new Vector(iData.ToArray());
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		protected override bool Serialize(Vector value, out string serializedData)
		{
			serializedData = null;
			if ((value == null) || (value.Length < 1))
			{
				serializedData = String.Empty;
				return true;
			}

			StringBuilder sb = new StringBuilder(value.Length * 9);
			sb.AppendFormat(value[0].ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
			for (int i = 1; i < value.Length; ++i)
			{
				if (Double.IsNaN(value[i]) || Double.IsInfinity(value[i]))
					return false;
				sb.Append(' ');
				sb.Append(value[i].ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
			}
			serializedData = sb.ToString();

			return true;
		}

		#endregion
	}
}
