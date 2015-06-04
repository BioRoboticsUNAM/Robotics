using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Robotics.API.PrimitiveSharedVariables
{
	/// <summary>
	/// Gets access to a multi-variant-type variable stored in the Blackboard
	/// </summary>
	public sealed class VarSharedVariable : SharedVariable<string>
	{
		#region Variables

		// private Regex rxSplitter = new Regex(@"(""(\\.|[^""])"")|\S+", RegexOptions.Compiled);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of IntSharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public VarSharedVariable(CommandManager commandManager, string variableName, bool initialize)
			: this(commandManager, variableName, null, initialize) { }

		/// <summary>
		/// Initializes a new instance of IntSharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public VarSharedVariable(CommandManager commandManager, string variableName, string value, bool initialize)
			: base(commandManager, variableName, value, initialize) { }

		/// <summary>
		/// Initializes a new instance of IntSharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public VarSharedVariable(string variableName)
			: this(null, variableName, null, false) { }

		/// <summary>
		/// Initializes a new instance of IntSharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public VarSharedVariable(string variableName, string value)
			: this(null, variableName, value, false) { }

		#endregion

		#region Events

		#endregion

		#region Properties

		/// <summary>
		/// Gets the string representation of the elements contained in the BufferedData
		/// </summary>
		public string[] Elements
		{
			get
			{
				string[] elements;
				MatchCollection mc;

				if(String.IsNullOrEmpty(bufferedData))
					return new string[0];

				mc = RxSharedVariableNotificationValidator.Matches(bufferedData);
				elements = new string[mc.Count];
				for (int i = 0; i < mc.Count; ++i)
					elements[i] = mc[i].Value;
				return elements;
			}
		}

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
			get { return "var"; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		protected override bool Deserialize(string serializedData, out string value)
		{
			if (String.IsNullOrEmpty(serializedData) || (String.Compare(serializedData, "null", true) == 0))
				value = null;
			else
				value = serializedData;
			return true;
		}

		/// <summary>
		/// Sserializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		protected override bool Serialize(string value, out string serializedData)
		{
			if (String.IsNullOrEmpty(value))
				serializedData = "null";
			else 
				serializedData = value;
			return true;
		}

		#endregion
	}
}
