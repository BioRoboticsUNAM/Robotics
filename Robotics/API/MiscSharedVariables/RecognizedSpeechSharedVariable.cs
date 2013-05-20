using System;
using System.Text;
using Robotics.API;
using Robotics.API.PrimitiveSharedVariables;
using Robotics.HAL.Sensors;
using Robotics.Utilities;

namespace Robotics.API.MiscSharedVariables
{
	/// <summary>
	/// Gets access to a RecognizedSpeech variable stored in the Blackboard
	/// </summary>
	public class RecognizedSpeechSharedVariable : SharedVariable<RecognizedSpeech>
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
		public RecognizedSpeechSharedVariable(CommandManager commandManager, string variableName, bool initialize)
			: this(commandManager, variableName, null, initialize) { }

		/// <summary>
		/// Initializes a new instance of StringSharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public RecognizedSpeechSharedVariable(CommandManager commandManager, string variableName, RecognizedSpeech value, bool initialize)
			: base(commandManager, variableName, value, initialize) { }

		/// <summary>
		/// Initializes a new instance of StringSharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public RecognizedSpeechSharedVariable(string variableName)
			: this(null, variableName, null, false) { }

		/// <summary>
		/// Initializes a new instance of StringSharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public RecognizedSpeechSharedVariable(string variableName, RecognizedSpeech value)
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
			get { return "RecognizedSpeech"; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool SDeserialize(string serializedData, out RecognizedSpeech value)
		{
			return (new RecognizedSpeechSharedVariable("none")).Deserialize(serializedData, out value);
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		protected override bool Deserialize(string serializedData, out RecognizedSpeech value)
		{
			ushort count;
			RecognizedSpeech rs;
			int cc;
			int start;
			int end;
			string text;
			double confidence;

			if (String.IsNullOrEmpty(serializedData) || (String.Compare("null", serializedData, true) == 0))
			{
				value = null;
				return true;
			}

			value = null;
			if (serializedData.Length < 4)
				return false;
			cc = 0;
			// 1. Read open brace '{'
			if (serializedData[cc++] != '{')
				return false;
			// 2. Read white space
			if (serializedData[cc++] != ' ')
				return false;

			// 3. Read number of alternates
			if (!Scanner.XtractUInt16(serializedData, ref cc, out count))
				return false;

			// 4. Read alternates
			rs = new RecognizedSpeech(count);
			for(int i = 0; i < count; ++i)
			{
				// 4.1. Read text
				// 4.1.1. Read double quotes
				Scanner.SkipSpaces(serializedData, ref cc);
				if(!Scanner.ReadChar('"', serializedData, ref cc))
					return false;
				// 4.1.2. Read text
				start = cc;
				while (cc < serializedData.Length)
				{
					// 4.1.3. Read double quotes
					if (Scanner.ReadChar('"', serializedData, ref cc))
						break;
					++cc;
				}
				end = cc - 1;
				// 4.1.4. Extract text
				if((end - start) < 1)
					return false;
				text = serializedData.Substring(start, end - start);

				// 4.2. Read white space
				if (!Scanner.ReadChar(' ', serializedData, ref cc))
					return false;

				// 4.3 Read confidence
				if (!Scanner.XtractDouble(serializedData, ref cc, out confidence))
					return false;

				// 4.4. Read white space
				if (!Scanner.ReadChar(' ', serializedData, ref cc))
					return false;

				rs.Add(text, (float)confidence);
			}

			// 5. Read closing brace '}'
			if (!Scanner.ReadChar('}', serializedData, ref cc))
				return false;

			value = rs;
			return true;
		}

		/// <summary>
		/// Sserializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		protected override bool Serialize(RecognizedSpeech value, out string serializedData)
		{
			StringBuilder sb;

			if (value == null)
			{
				serializedData = "null";
				return true;
			}
			serializedData = null;

			sb = new StringBuilder();
			
			// 1. Write open brace '{'
			// 2. Write white space
			sb.Append("{ ");
			
			// 3. Write the number of alternates
			sb.Append(value.Count);

			// 4. Write white space
			sb.Append(' ');

			// 5. Write all Alternates
			for (int i = 0; i < value.Count; ++i)
			{
				// 5.1. Write alternate text
				if(value[i].Text.Contains("\""))
					return false;
				//sb.Append("\\\"");
				sb.Append("\"");
				if(value[i] != null)
					sb.Append(value[i].Text);
				//sb.Append("\\\"");
				sb.Append("\"");
				// 5.2. Write white space
				sb.Append(' ');
				// 5.3. Write confidence
				sb.Append(
					(value[i] != null) ? 
					value[i].Confidence.ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat) :
					"0");
				// 5.4. Write white space
				sb.Append(' ');
			}

			// 1. Write close brace '}'
			sb.Append('}');
			serializedData = sb.ToString();
			
			return true;
		}

		#endregion
	}
}
