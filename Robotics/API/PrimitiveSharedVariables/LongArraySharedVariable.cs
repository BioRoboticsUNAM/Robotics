using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API.PrimitiveSharedVariables
{
	/// <summary>
	/// Gets access to an ARRAY OF LONG INTEGERS variable stored in the Blackboard
	/// </summary>
	public class LongArraySharedVariable : SharedVariable<long[]>
	{
		#region Variables

		/// <summary>
		/// Chars used to split strings
		/// </summary>
		private static char[] SplitChars = { ' ', '\t', '\r', '\n' };

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of IntArraySharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public LongArraySharedVariable(CommandManager commandManager, string variableName, bool initialize)
			: this(commandManager, variableName, null, initialize) { }

		/// <summary>
		/// Initializes a new instance of IntArraySharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public LongArraySharedVariable(CommandManager commandManager, string variableName, long[] value, bool initialize)
			: base(commandManager, variableName, value, initialize) { }

		/// <summary>
		/// Initializes a new instance of IntArraySharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public LongArraySharedVariable(string variableName)
			: this(null, variableName, null, false) { }

		/// <summary>
		/// Initializes a new instance of IntArraySharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public LongArraySharedVariable(string variableName, long[] value)
			: this(null, variableName, value, false) { }

		#endregion

		#region Events

		#endregion

		#region Properties

		/// <summary>
		/// Returns true
		/// </summary>
		public override bool IsArray
		{
			get { return true; }
		}

		/// <summary>
		/// Returns -1
		/// </summary>
		public override int Length { get { return -1; } }

		/// <summary>
		/// Returns "long"
		/// </summary>
		public override string TypeName
		{
			get { return "long"; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Deserializes an array of longs from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		protected override bool Deserialize(string serializedData, out long[] values)
		{
			string[] parts;
			long data;
			List<long> iData = new List<long>();

			values = null;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Int64.TryParse(parts[i], out data))
					return false;
				iData.Add(data);
			}
			values = iData.ToArray();
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		protected override bool Serialize(long[] values, out string serializedData)
		{
			if ((values == null) || (values.Length < 1))
			{
				serializedData = String.Empty;
				return true;
			}

			StringBuilder sb = new StringBuilder(values.Length * 6);
			sb.Append(values[0].ToString());
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i].ToString());
			}
			serializedData = sb.ToString();

			return true;
		}

		#endregion
	}
}
