using System;
using System.Collections.Generic;
using System.Text;
using Robotics.Utilities;

namespace Robotics.API
{
	/// <summary>
	/// Provides methods for data parsing
	/// </summary>
	public class Parser
	{
		#region Static Methods

		/// <summary>
		/// Decodes the data contained in a received response. A return value indicates whether the operation succeeded
		/// </summary>
		/// <param name="s">string which contains the data to parse</param>
		/// <param name="variableType">When this method returns contains the type of the variable coded in the input string if the conversion succeeded, or null if the conversion failed.</param>
		/// <param name="isArray">When this method returns is set to true if the conversion succeded and the variable coded in the input string is an array, false otherwise.</param>
		/// <param name="arrayLength">When this method returns contains the size of the array if the conversion succeded and the variable coded in the input string is an array, -1 otherwise.</param>
		/// <param name="variableName">When this method returns contains the name of the variable coded in the input string if the conversion succeeded, or null if the conversion failed.</param>
		/// <param name="variableData">When this method returns contains the content of the variable coded in the input string if the conversion succeeded, or the null if the conversion failed.</param>
		/// <returns>true if the the data extraction succeeded, false otherwise</returns>
		public static bool ParseSharedVariable(string s, out string variableType,
			out bool isArray, out int arrayLength, out string variableName, out string variableData)
		{
			int cc;
			int end;
			int dataSize;
			string id1;
			string id2;
			ushort arraySize;
			bool isSizedArray;

			// Initialize variables
			cc = 0;
			isSizedArray = false;
			variableType = String.Empty;
			isArray = false;
			arrayLength = -1;
			variableName = String.Empty;
			variableData = String.Empty;

			// Empty string contains no variable data. Return.
			if (String.IsNullOrEmpty(s)) return false;

			// Check if the string is enclosed within braces
			end = s.Length - 1;
			if (Scanner.ReadChar('{', s, ref cc))
			{
				if (s[end] != '}')
					return false;
				--end;
				while (Scanner.IsSpace(s[end]))
					--end;
				++end;
			}

			// Skip space characters at the beginning
			SkipSpaces(s, ref cc);

			// Read the variable type
			if (!XtractIdentifier(s, ref cc, out id1))
				return false;

			// Check if variable is an array and get the array length
			arraySize = 0;
			if ((cc < s.Length) && (s[cc++] == '['))
			{
				// Skip space characters between square bracket and array size (if any)
				SkipSpaces(s, ref cc);
				// Read array size (if any)
				if (Scanner.XtractUInt16(s, ref cc, out arraySize))
					isSizedArray = true;
				// Skip space characters between and array size (if any) and square bracket
				SkipSpaces(s, ref cc);
				if ((cc >= s.Length) || (s[cc++] != ']'))
					return false;
				isArray = true;
			}

			// Skip space characters between variable type and it's name
			SkipSpaces(s, ref cc);

			// Read the variable name
			if (!XtractIdentifier(s, ref cc, out id2))
				return false;

			// Skip space characters between variable name and variable content
			SkipSpaces(s, ref cc);

			// Calculate data size
			dataSize = 1 + end - cc;
			if (dataSize < 0)
				return false;

			// Set values
			variableType = id1;
			variableName = id2;
			arrayLength = (isArray && isSizedArray) ? arraySize : -1;
			variableData = (dataSize < 1) ? String.Empty : s.Substring(cc, dataSize);
			return true;
		}

		/// <summary>
		/// Extracts the type, name and data from a shared variable
		/// </summary>
		/// <param name="s">String from which the variable will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="varType">When this method returns contains the type of the variable found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <param name="varName">When this method returns contains the name of the variable found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <param name="isArray">When this method returns is set to true if a variable was found in s and it is an array, or false otherwise</param>
		/// <param name="arraySize">When this method returns contains the length of the array of the variable found in s if the extraction succeded and the variable is an array, or -1 otherwise</param>
		/// <returns>true if the extraction succeded, false otherwise</returns>
		[Obsolete]
		public static bool XtractSharedVariableData(string s, ref int cc, out string varType, out string varName, out bool isArray, out int arraySize)
		{
			string id1;
			string id2;
			ushort size;
			bool isSizedArray = false;

			varType = null;
			varName = null;
			isArray = false;
			arraySize = -1;

			SkipSpaces(s, ref cc);
			if (!XtractIdentifier(s, ref cc, out id1))
				return false;
			SkipSpaces(s, ref cc);

			// Array
			size = 0;
			if ((cc < s.Length) && (s[cc] == '['))
			{
				SkipSpaces(s, ref cc);
				if (Scanner.XtractUInt16(s, ref cc, out size))
					isSizedArray = true;
				SkipSpaces(s, ref cc);
				if ((cc >= s.Length) || (s[cc] != ']'))
					return false;
				isArray = true;
			}

			SkipSpaces(s, ref cc);
			if (!XtractIdentifier(s, ref cc, out id2))
				return false;

			varType = id1;
			varName = id2;
			arraySize = (isArray && isSizedArray) ? size : -1;
			return true;
		}

		/// <summary>
		/// Extracts the type, name and data from a shared variable
		/// </summary>
		/// <param name="s">String from which the variable will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="varType">When this method returns contains the type of the variable found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <param name="varName">When this method returns contains the name of the variable found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <param name="isArray">When this method returns is set to true if a variable was found in s and it is an array, or false otherwise</param>
		/// <param name="arraySize">When this method returns contains the length of the array of the variable found in s if the extraction succeded and the variable is an array, or -1 otherwise</param>
		/// <param name="data">When this method returns contains the data of the variable found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>true if the extraction succeded, false otherwise</returns>
		[Obsolete]
		public static bool XtractSharedVariableData(string s, ref int cc, out string varType, out string varName, out bool isArray, out int arraySize, out string data)
		{
			varType = null;
			varName = null;
			isArray = false;
			arraySize = -1;
			data = String.Empty;

			if(!XtractSharedVariableData(s, ref cc, out varType, out varName, out isArray, out arraySize))
				return false;
			if (cc >= s.Length - 1)
				data = String.Empty;
			else
			{
				data = s.Substring(cc);
				cc = s.Length;
			}
			return true;
		}

		/// <summary>
		/// Extracts the first module name found inside a string
		/// </summary>
		/// <param name="s">String from which the module name will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="moduleName">When this method returns contains the first module name found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid module name was found in s starting at cc, false otherwise</returns>
		public static bool XtractModuleName(string s, ref int cc, out string moduleName)
		{
			int bcc = cc;
			int length;
			moduleName = null;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (!Scanner.IsUAlpha(s[cc])) return false;
			++cc;
			while ((cc < s.Length) && ((Scanner.IsUAlNum(s[cc]) || s[cc] == '-')))
				++cc;
			length = Math.Min(cc - bcc, s.Length - bcc);
			moduleName = s.Substring(bcc, length);
			return true;
		}

		/// <summary>
		/// Extracts the first command name found inside a string
		/// </summary>
		/// <param name="s">String from which the command name will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="commandName">When this method returns contains the first command name found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid command name was found in s starting at cc, false otherwise</returns>
		public static bool XtractCommandName(string s, ref int cc, out string commandName)
		{
			int bcc = cc;
			int length;
			commandName = null;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (!Scanner.IsLAlpha(s[cc]) && (s[cc] != '_')) return false;
			++cc;
			while ((cc < s.Length) && ((Scanner.IsLAlNum(s[cc]) || s[cc] == '_')))
				++cc;
			length = Math.Min(cc - bcc, s.Length - bcc);
			commandName = s.Substring(bcc, length);
			return true;
		}

		/// <summary>
		/// Advances the read header until no spaces are found
		/// </summary>
		/// <param name="s">Input string</param>
		/// <param name="cc">Read header</param>
		public static void SkipSpaces(string s, ref int cc)
		{
			if ((cc < 0) || (cc >= s.Length))
				return;
			while (Scanner.IsSpace(s[cc])) ++cc;
		}

		/// <summary>
		/// Extracts the first command parameters found inside a string
		/// </summary>
		/// <param name="s">String from which the command parameters will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="parameters">When this method returns contains the first command parameters found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid command parameters was found in s starting at cc, false otherwise</returns>
		public static bool XtractCommandParams(string s, ref int cc, out string parameters)
		{
			int bcc = cc + 1;
			int length;
			parameters = null;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (s[cc] != '"') return false;
			++cc;
			while ((cc < s.Length) && (s[cc] != '"') && (s[cc] != 0) && (s[cc] < 128))
			{
				if (s[cc] == '\\')
					++cc;
				++cc;
			}
			length = Math.Min(cc - bcc, s.Length - bcc);
			if (s[cc] != '"') return false;
			++cc;
			if (length < 0)
				return false;
			parameters = s.Substring(bcc, length);
			return true;
		}

		/// <summary>
		/// Extracts the first @id found inside a string
		/// </summary>
		/// <param name="s">String from which the @id will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="id">When this method returns contains the id found in s if the extraction succeded, or -1 if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid id was found in s starting at cc, false otherwise</returns>
		public static bool XtractId(string s, ref int cc, out int id)
		{
			int bcc = cc + 1;
			int length;
			string sid;
			id = -1;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (s[cc] != '@') return false;
			++cc;

			while ((cc < s.Length) && Scanner.IsNumeric(s[cc])) ++cc;
			length = Math.Min(cc - bcc, s.Length - bcc);
			sid = s.Substring(bcc, length);
			return Int32.TryParse(sid, out id);
		}

		/// <summary>
		/// Extracts the first C-type identifier found inside a string
		/// </summary>
		/// <param name="s">String from which the identifier will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="identifier">When this method returns contains the first C-type identifier found in s if the extraction succeded, or null if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid C-type identifier was found in s starting at cc, false otherwise</returns>
		public static bool XtractIdentifier(string s, ref int cc, out string identifier)
		{
			int bcc = cc;
			int length;
			identifier = null;

			if ((cc < 0) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (!Scanner.IsAlpha(s[cc]) && (s[cc] != '_')) return false;
			++cc;
			while ((cc < s.Length) && (Scanner.IsAlNum(s[cc]) || (s[cc] == '_')))
				++cc;
			length = Math.Min(cc - bcc, s.Length - bcc);
			identifier = s.Substring(bcc, length);
			return true;
		}

		/// <summary>
		/// Extracts the first result (1 or 0) found inside a string
		/// </summary>
		/// <param name="s">String from which the result will be extracted</param>
		/// <param name="cc">The search starting position</param>
		/// <param name="result">When this method returns contains the result found in s if the extraction succeded, or -1 if the extraction failed.</param>
		/// <returns>True if the extraction succeded and a valid result was found in s starting at cc, false otherwise</returns>
		public static bool XtractResult(string s, ref int cc, out int result)
		{
			int ncc = cc + 1;
			result = -1;

			if ((cc < 1) || (cc >= s.Length) || (s[cc] == 0) || (s[cc] > 128))
				return false;

			if (
				Scanner.IsSpace(s[cc - 1]) &&
				((s[cc] == '1') || (s[cc] == '0')) &&
				((ncc == s.Length) || (s[ncc] == 0) || Scanner.IsSpace(s[ncc]))
				)
			{
				result = s[cc] - '0';
				return true;
			}
			return false;
		}
		
		#endregion
	}
}
