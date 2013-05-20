using System;
using System.Collections.Generic;
using System.Text;
using Robotics.Mathematics;

namespace Robotics.API.PrimitiveSharedVariables
{
	/// <summary>
	/// Gets access to a Matrix variable stored in the Blackboard
	/// </summary>
	public class MatrixSharedVariable : SharedVariable<Matrix>
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
		public MatrixSharedVariable(CommandManager commandManager, string variableName, bool initialize)
			: this(commandManager, variableName, null, initialize) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="commandManager">The CommandManager object used to communicate with the Blackboard</param>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <param name="initialize">Indicates if the shared variable will be automatically initialized if the commandManager is different from null</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public MatrixSharedVariable(CommandManager commandManager, string variableName, Matrix value, bool initialize)
			: base(commandManager, variableName, value, initialize) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public MatrixSharedVariable(string variableName)
			: this(null, variableName, null, false) { }

		/// <summary>
		/// Initializes a new instance of DoubleArraySharedVariable
		/// </summary>
		/// <param name="variableName">The name of the variable in the Blackboard</param>
		/// <param name="value">The value to store in the shared variable if it does not exist</param>
		/// <remarks>If there is no variable with the provided name in the blackboard, a new variable with the asociated name is created</remarks>
		public MatrixSharedVariable(string variableName, Matrix value)
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
		/// Returns "int"
		/// </summary>
		public override string TypeName
		{
			get { return "Matrix"; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Deserializes an array of doubles from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		protected override bool Deserialize(string serializedData, out Matrix value)
		{
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
		}

		/// <summary>
		/// Deserializes an array of from a string which starts with "double[]" and is followed only by double representations
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="rows">Number of matrix rows</param>
		/// <param name="cols">Number of matrix columns</param>
		/// <param name="value">When this method returns contains the value stored in serializedData the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		private bool Deserialize(string serializedData, int rows, int cols, out Matrix value)
		{
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
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		protected override bool Serialize(Matrix value, out string serializedData)
		{
			serializedData = null;
			StringBuilder sb;
			if (value == null)
			{
				serializedData = String.Empty;
				return true;
			}
			sb = new StringBuilder(12 + value.Rows * value.Columns * 15);
			sb.Append(value.Rows);
			sb.Append('x');
			sb.Append(value.Columns);

			for (int i = 0; i < value.Rows; ++i)
			{
				for (int j = 0; j < value.Columns; ++j)
				{
					if (Double.IsNaN(value[i, j]) || Double.IsInfinity(value[i, j]))
						return false;
					sb.Append(' ');
					sb.Append(value[i, j].ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat));
				}
			}
			serializedData = sb.ToString();

			return true;
		}

		#endregion
	}
}
