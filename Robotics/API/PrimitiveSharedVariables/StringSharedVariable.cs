using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API.PrimitiveSharedVariables
{
	/// <summary>
	/// Gets access to an string variable stored in the Blackboard
	/// </summary>
	public class StringSharedVariable : SharedVariable<string>
	{
		#region Variables

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of StringSharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public StringSharedVariable(CommandManager commandManager, string variableName, bool initialize)
			: this(commandManager, variableName, null, initialize) { }

		/// <summary>
		/// Initializes a new instance of StringSharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public StringSharedVariable(CommandManager commandManager, string variableName, string value, bool initialize)
			: base(commandManager, variableName, value, initialize) { }

		/// <summary>
		/// Initializes a new instance of StringSharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public StringSharedVariable(string variableName)
			: this(null, variableName, null, false) { }

		/// <summary>
		/// Initializes a new instance of StringSharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public StringSharedVariable(string variableName, string value)
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
		/// Returns "string"
		/// </summary>
		public override string TypeName
		{
			get { return "string"; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or null if the deserialization failed.</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		protected override bool Deserialize(string serializedData, out string value)
		{
			return DeserializeString(serializedData, out value);
		}

		/// <summary>
		/// Sserializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed.</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		protected override bool Serialize(string value, out string serializedData)
		{
			serializedData = SerializeString(value);
			return true;
		}

		/// <summary>
		/// Serializes the provided string by escaping all double quotes (if any) and enclosing it by escaped double quotes
		/// </summary>
		/// <param name="value">String to serialize</param>
		/// <returns>Serialized string</returns>
		public static string SerializeString(string value)
		{
			string serializedData;

			if (value == null)
				return "null";

			serializedData = value;
			serializedData = serializedData.Replace(@"\", @"\\");
			serializedData = serializedData.Replace("\"", "\\\"");
			serializedData = "\"" + serializedData.ToString() + "\"";
			return serializedData;
		}

		/// <summary>
		/// Deserializes the provided string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or null if the deserialization failed.</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool DeserializeString(string serializedData, out string value)
		{
			int start;
			int end;

			if (String.IsNullOrEmpty(serializedData) || (String.Compare("null", serializedData, true) == 0))
			{
				value = null;
				return true;
			}

			value = null;
			start = serializedData.IndexOf("\"");
			end = serializedData.LastIndexOf("\"") - 1;
			if ((start < 0) || (end < 1) || (end < start))
				return false;
			value = serializedData.Substring(start + 1, end - start);
			value = value.Replace("\\\"", "\"");
			value = value.Replace(@"\\", @"\");
			return true;
		}

		#endregion
	}
}
