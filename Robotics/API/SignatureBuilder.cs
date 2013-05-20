using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Robotics.API
{
	/// <summary>
	/// Represents an object capable of produce Signature objects
	/// </summary>
	public class SignatureBuilder
	{
		#region Variables
		/// <summary>
		/// String builer used to generate the regular expression pattern
		/// </summary>
		protected readonly StringBuilder sb;
		/// <summary>
		/// Number of parameters
		/// </summary>
		protected int parameterCount;
		/// <summary>
		/// Number of delegates
		/// </summary>
		protected int delegateCount;
		/// <summary>
		/// Number of type arrays
		/// </summary>
		protected int typeArrayCount;
		
		/// <summary>
		/// Stores the correspondance list between delegate name and delegate
		/// </summary>
		protected readonly Dictionary<string, Delegate> delegateList;
		/// <summary>
		/// Stores the correspondance list between type array name and type array
		/// </summary>
		protected readonly Dictionary<string, Type[]> typeArrayList;
		/// <summary>
		/// Stores the position of the last parameter header
		/// </summary>
		protected int headerParamAt;
		/// <summary>
		/// Stores the position of the last delegate header
		/// </summary>
		protected int headerDelegateAt;
		/// <summary>
		/// Stores the position of the last TypeArray header
		/// </summary>
		protected int headerTypeArrayAt;

		/*
		/// <summary>
		/// Stores the correspondance list between parameter name and parameter number
		/// </summary>
		protected readonly Dictionary<string, int> parameterList;
		*/

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SignatureBuilder
		/// </summary>
		public SignatureBuilder()
		{
			parameterCount = 0;
			sb = new StringBuilder();
			//parameterList = new Dictionary<string, int>();
			delegateList = new Dictionary<string, Delegate>();
			typeArrayList = new Dictionary<string, Type[]>();
			headerDelegateAt = -1;
			headerParamAt = -1;
			headerTypeArrayAt = -1;
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the regular expression pattern to match parameters
		/// </summary>
		public string RegexPattern
		{
			get { return sb.ToString(); }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Adds a signature to the SignatureBuilder using the provided types
		/// </summary>
		/// <param name="types">Array of types from which create the signature</param>
		/// <remarks>Only prmitive types are supported</remarks>
		public void AddNewFromTypes(params Type[] types)
		{
			bool first = true;

			if (types == null)
				types = new Type[0];

			BeginTypeArray(types);
			for (int i = 0; i < types.Length; ++i)
			{
				if (!first)
					AddSpace();
				else
					first = false;
				AddParameter(null, types[i]);
			}
			EndTypeArray();
		}

		/// <summary>
		/// Adds a signature to the SignatureBuilder using the parameters of the provided Delegate object
		/// </summary>
		/// <param name="d">Delegate to create the signature from</param>
		public void AddNewFromDelegate(Delegate d)
		{
			bool first = true;

			if (d == null)
				throw new ArgumentNullException();

			ParameterInfo[] parameters = d.Method.GetParameters();
			BeginDelegate(d);
			foreach (ParameterInfo pi in parameters)
			{
				if (!first)
					AddSpace();
				else
					first = false;
				AddParameter(pi.Name, pi.ParameterType);
				
			}
			EndDelegate();
		}

		/// <summary>
		/// Adds a parameter to the current signature of the specified type with the specfied name
		/// </summary>
		/// <param name="name">The name of the parameter</param>
		/// <param name="type">The type of the parameter</param>
		/// <remarks>Only prmitive types are supported</remarks>
		protected void AddParameter(string name, Type type)
		{
			switch (type.Name)
			{
				//case "Byte":
				case "Byte[]":
					AddHexadecimal(name, false);
					break;

				case "Char":
					AddCharacter(name, false);
					break;

				case "Char[]":
					AddMultipleWords(name);
					break;

				case "SByte":
				case "Int16":
				case "Int32":
				case "Int64":
					AddIntegerNumber(name, false, false);
					break;


				case "Byte":
				case "UInt16":
				case "UInt32":
				case "UInt64":
					AddIntegerNumber(name, true, false);
					break;

				case "Single":
				case "Double":
					AddRealNumber(name, false);
					break;

				case "String":
					AddIdentifier(name, false);
					break;

				case "String[]":
					AddMultipleWords(name);
					break;

				case "Int16[]":
				case "Int32[]":
				case "Int64[]":
					AddMultipleIntegerNumber(name, false);
					break;

				case "UInt16[]":
				case "UInt32[]":
				case "UInt64[]":
					AddMultipleIntegerNumber(name, true);
					break;

				case "Single[]":
				case "Double[]":
					AddMultipleRealNumbers(name);
					break;

				default:
					throw new Exception("Unsupported data type provided");
			}
		}

		/// <summary>
		/// Adds the delegate header to the stringBuilder used for regex
		/// </summary>
		/// <param name="d">The Delegate object from which create the header</param>
		protected void BeginDelegate(Delegate d)
		{
			if (d == null) throw new ArgumentNullException("d");
			string delegateName = "d" + delegateCount.ToString();
			if (delegateList.ContainsKey(delegateName) || delegateList.ContainsValue(d))
				throw new ArgumentException("Delegate already defined", "d");
			headerDelegateAt = sb.Length;
			headerParamAt = -1;
			if ((delegateCount > 0) || (typeArrayCount > 0))
				sb.Append('|');
			sb.Append("(?<");
			sb.Append(delegateName);
			sb.Append('>');
			sb.Append('^');
			delegateList.Add(delegateName, d);
			++delegateCount;
		}

		/// <summary>
		/// Adds the TypeArray header to the stringBuilder used for regex
		/// </summary>
		/// <param name="typeArray">The array of Type objects from which create the header</param>
		protected void BeginTypeArray(Type[] typeArray)
		{
			string taName = "ta" + typeArrayCount.ToString();
			if (typeArrayList.ContainsValue(typeArray))
				throw new ArgumentException("Array already defined", "typeArray");
			headerTypeArrayAt = sb.Length;
			headerParamAt = -1;
			if ((delegateCount > 0) || (typeArrayCount > 0))
				sb.Append('|');
			sb.Append("(?<");
			sb.Append(taName);
			sb.Append('>');
			sb.Append('^');
			typeArrayList.Add(taName, typeArray);
			++typeArrayCount;
		}

		/// <summary>
		/// Adds the parameter header to the stringBuilder used for regex
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		protected void BeginParameter(string paramName)
		{
			//if ((paramName == null) || (paramName.Length < 1))
			paramName = "p" + parameterCount.ToString();
			//if (parameterList.ContainsKey(paramName))
			//	throw new ArgumentException("Parameter Name already defined", "paramName");
			headerParamAt = sb.Length;
			sb.Append("(?<");
			sb.Append('p');
			sb.Append(parameterCount);
			sb.Append('>');
			//parameterList.Add(paramName, parameterCount);
			++parameterCount;
		}

		/// <summary>
		/// Deletes all the SignatureBuilder data to allow reuse the SignatureBuilder object
		/// </summary>
		public void Clear()
		{
			this.delegateCount = 0;
			this.delegateList.Clear();
			this.typeArrayList.Clear();
			this.headerDelegateAt = -1;
			this.headerParamAt = -1;
			this.parameterCount = 0;
			this.typeArrayCount = 0;
			//this.parameterList.Clear();
			this.sb.Remove(0, sb.Length);
		}

		/// <summary>
		/// Balances the parenthesis count and adds the delegate footer to the stringBuilder used for regex
		/// </summary>
		protected void EndDelegate()
		{
			if (headerDelegateAt < 0) throw new Exception("No header found");
			int pBalance = 0;
			for (int i = headerDelegateAt; i < sb.Length; ++i)
			{
				if (sb[i] == '\\')
					++i;
				else if (sb[i] == '(')
					++pBalance;
				else if (sb[i] == ')')
				{
					if (pBalance < 1) throw new Exception("Malformed string");
					--pBalance;
				}
			}

			while (pBalance > 1)
			{
				sb.Append(')');
				--pBalance;
			}
			sb.Append('$');
			sb.Append(')');
			headerDelegateAt = -1;
		}

		/// <summary>
		/// Balances the parenthesis count and adds the TypeArray footer to the stringBuilder used for regex
		/// </summary>
		protected void EndTypeArray()
		{
			if (headerTypeArrayAt < 0) throw new Exception("No header found");
			int pBalance = 0;
			for (int i = headerTypeArrayAt; i < sb.Length; ++i)
			{
				if (sb[i] == '\\')
					++i;
				else if (sb[i] == '(')
					++pBalance;
				else if (sb[i] == ')')
				{
					if (pBalance < 1) throw new Exception("Malformed string");
					--pBalance;
				}
			}

			while (pBalance > 1)
			{
				sb.Append(')');
				--pBalance;
			}
			sb.Append('$');
			sb.Append(')');
			headerTypeArrayAt = -1;
		}

		/// <summary>
		/// Balances the parenthesis count and adds the parameter footer to the stringBuilder used for regex
		/// </summary>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void EndParameter(bool optional)
		{
			if (headerParamAt < 0) throw new Exception("No header found");
			int pBalance = 0;
			for (int i = headerParamAt; i < sb.Length; ++i)
			{
				if (sb[i] == '\\')
					++i;
				else if (sb[i] == '(')
					++pBalance;
				else if (sb[i] == ')')
				{
					if (pBalance < 1) throw new Exception("Malformed string");
					--pBalance;
				}
			}

			while (pBalance > 0)
			{
				sb.Append(')');
				--pBalance;
			}
			if (optional) sb.Append('?');
			headerParamAt = -1;
		}

		/// <summary>
		/// Generates the Signature used to check the parameters for the commands and execute the apropiate function
		/// </summary>
		/// <param name="commandName">The name of the command thie Signature object will be valid for</param>
		/// <returns></returns>
		public Signature GenerateSignature(string commandName)
		{
			Signature signature = new Signature(commandName, RegexPattern, delegateList, typeArrayList);

			return signature;
		}

		/// <summary>
		/// Normalizes the input string to prevent to contain special characters used in regex
		/// </summary>
		/// <param name="stringToNormalize">The string to normalize</param>
		/// <returns>The input string normalized</returns>
		protected string NormalizeEscapes(string stringToNormalize)
		{
			string[] charsToNormalize = {".", "$", "^", "{", "[", "(", "|", ")", "*", "+", "?", "\\" };
			string normalized = stringToNormalize;

			for (int i = 0; i < charsToNormalize.Length; ++i)
			{
				normalized = normalized.Replace(charsToNormalize[i] , "\\" + charsToNormalize[i]);
			}
			return normalized;
		}

		#region Add elements

		/// <summary>
		/// Inserts a string type parameter which must be composed of characters from 'A' to 'Z' or 'a' to 'z'
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddAnsiWord(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"[A-Za-z]+");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a single character type parameter
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddCharacter(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"\.");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a constant string which may apear as paramteter
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="constant">The constant string</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddConstant(string paramName, string constant, bool optional)
		{
			if ((constant == null) || (constant.Length < 1)) throw new ArgumentException("Constant string must not be null or zero-length", "constant");
			string normalizedConstant = NormalizeEscapes(constant);
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(normalizedConstant);
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts an OR-ed array of constant strings which may apear as paramteter
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="constants">Array of constants</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddConstant(string paramName, string[] constants, bool optional)
		{
			string[] normalizedConstants;
			int i;

			if ((constants == null) || (constants.Length < 1))
				throw new ArgumentException("Invalid constants array. Constant strings must not be empty nor null", "constants");

			normalizedConstants = new string[constants.Length];
			for (i = 0; i < constants.Length; ++i)
			{
				if ((constants[i] == null) || (constants[i].Length < 1))
					throw new ArgumentException("Invalid constant string. Constant strings must not be empty nor null", "constants");
				normalizedConstants[i] = NormalizeEscapes(constants[i]);
			}
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append('(');
				sb.Append(normalizedConstants[0]);
				for (i = 1; i < normalizedConstants.Length; ++i)
				{
					sb.Append('|');
					sb.Append(normalizedConstants[i]);
				}
				sb.Append(@")");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a string used as delimiter (one or more occurences)
		/// </summary>
		/// <param name="delimiter">The string used as delimiter</param>
		protected void AddDelimiter(string delimiter)
		{
			if ((delimiter == null) || (delimiter.Length < 1))
				throw new ArgumentException("Constant string must not be null or zero-length", "constant");
			string normalizedDelimiter = NormalizeEscapes(delimiter);
			lock(sb)
			{
				sb.Append('(');
				sb.Append(normalizedDelimiter);
				sb.Append(@")+");
			}
		}

		/// <summary>
		/// Inserts a string used as delimiter (one or more occurences)
		/// </summary>
		/// <param name="delimiter">Array of valid delimiter string</param>
		protected void AddDelimiter(string[] delimiter)
		{
			string[] normalizedDelimiters;
			int i;

			if ((delimiter == null) || (delimiter.Length < 1))
				throw new ArgumentException("Invalid delimiter array. Delimiter strings must not be empty nor null", "delimiter");

			normalizedDelimiters = new string[delimiter.Length];
			for (i = 0; i < delimiter.Length; ++i)
			{
				if ((delimiter[i] == null) || (delimiter[i].Length < 1))
					throw new ArgumentException("Invalid delimiter string. Delimiter strings must not be empty nor null", "delimiter");
				normalizedDelimiters[i] = NormalizeEscapes(delimiter[i]);
			}
			lock(sb)
			{
				sb.Append('(');
				sb.Append(normalizedDelimiters[0]);
				for (i = 1; i < normalizedDelimiters.Length; ++i)
				{
					sb.Append('|');
					sb.Append(normalizedDelimiters[i]);
				}
				sb.Append(@")+");
			}
		}

		/// <summary>
		/// Inserts a integer number type parameter in hexadecimal format
		/// The parameter may be preceded by a 0x, and the letters may be uppercase or lowercase.
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddHexadecimal(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"(0x)?");
				sb.Append(@"[0-9A-Fa-f]+");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a C-type-identifier type parameter
		/// C type identifiers begin with a letter or underscore and may contain letters numbers and underscore
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddIdentifier(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"[A-Za-z_]");
				sb.Append(@"[0-9A-Za-z_]*");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a integer number type parameter
		/// The parameter may be preceded by a minus, but not by a plus.
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="signed">Indicates if the number may be negative (preceded by a minus)</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddIntegerNumber(string paramName, bool signed, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				if (signed) sb.Append(@"\-?");
				sb.Append(@"\d+");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a string type parameter which must be composed of characters from 'a' to 'z'
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddLowerCaseWord(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"[a-z]+");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts an array of integer number type parameter
		/// The parameter may be preceded by a minus, but not by a plus.
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="signed">Indicates if the number may be negative (preceded by a minus)</param>
		protected void AddMultipleIntegerNumber(string paramName, bool signed)
		{
			lock (sb)
			{
				BeginParameter(paramName);
				sb.Append(@"(");
				
				if (signed) sb.Append(@"\-?");
				sb.Append(@"\d+");

				sb.Append(@"(\s+");
				if (signed) sb.Append(@"\-?");
				sb.Append(@"\d+");
				sb.Append(@")*");

				sb.Append(@")?");
				EndParameter(false);
			}
		}

		/// <summary>
		/// Inserts an array of real numbers type parameter
		/// Each parameter may be preceded by a minus, but not by a plus.
		/// If a dot character in the number is present, it must be preceded and succeded by a number.
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		protected void AddMultipleRealNumbers(string paramName)
		{
			lock (sb)
			{
				BeginParameter(paramName);
				sb.Append(@"((\-?\d+(\.\d+)?)(\s+\-?\d+(\.\d+)?)*)?");
				EndParameter(false);
			}
		}

		/// <summary>
		/// Inserts a multiple string array delimited by space-class characters
		/// Equivalent to the regular expression pattern (\S+(\s+\S+)*)?
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		protected void AddMultipleWords(string paramName)
		{
			lock (sb)
			{
				BeginParameter(paramName);
				sb.Append(@"(\S+(\s+\S+)*)?");
				EndParameter(false);
			}
		}

		/// <summary>
		/// Inserts a real number type parameter
		/// The parameter may be preceded by a minus, but not by a plus.
		/// If a dot character in the number is present, it must be preceded and succeded by a number.
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddRealNumber(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"\-?\d+(\.\d+)?");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a space delimter (one or more occurences)
		/// </summary>
		protected void AddSpace()
		{
			lock(sb)
			{
				sb.Append(@"\s+");
			}
		}

		/// <summary>
		/// Inserts a string type parameter which must be composed of characters from 'A' to 'Z'
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddUpperCaseWord(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"[A-Z]+");
				EndParameter(optional);
			}

		}

		/// <summary>
		/// Inserts a string type parameter which must be composed of characters from letters, numbers, underscore and UNICODE
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void AddWord(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"\w+");
				EndParameter(optional);
			}
		}

		#endregion

		#endregion
	}

	#region OldCode

	/*
	
	#region Variables
		/// <summary>
		/// String builer used to generate the regular expression pattern
		/// </summary>
		private StringBuilder sb;
		/// <summary>
		/// Number of parameters
		/// </summary>
		protected int parameterCount;
		/// <summary>
		/// Stores the correspondance list between parameter name and parameter number
		/// </summary>
		private Dictionary<string, int> parameterList;
		/// <summary>
		/// Stores the position of the last header
		/// </summary>
		private int headerAt;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SignatureBuilder
		/// </summary>
		public SignatureBuilder()
		{
			parameterCount = 0;
			sb = new StringBuilder();
			parameterList = new Dictionary<string, int>();
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the StringBuiler used to generate the regular expression pattern
		/// </summary>
		protected StringBuilder sb
		{
			get { return this.sb; }
		}

		/// <summary>
		/// Gets the correspondance list between parameter name and parameter number
		/// </summary>
		protected Dictionary<string, int> parameterList
		{
			get{return parameterList;}
		}

		/// <summary>
		/// Gets the regular expression pattern to match parameters
		/// </summary>
		public string RegexPattern
		{
			get { return sb.ToString(); }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Adds the parameter header to the stringBuilder used for regex
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		protected void BeginParameter(string paramName)
		{
			if ((paramName == null) || (paramName.Length < 1))
				paramName = "p" + parameterCount.ToString();
			if (parameterList.ContainsKey(paramName))
				throw new ArgumentException("Parameter Name already defined", "paramName");
			headerAt = sb.Length;
			sb.Append("(?<");
			sb.Append('p');
			sb.Append(parameterCount);
			sb.Append('>');
			parameterList.Add(paramName, parameterCount);
			++parameterCount;
		}

		/// <summary>
		/// Balances the parenthesis count and adds the parameter footer to the stringBuilder used for regex
		/// </summary>
		/// <param name="optional">Indicates if the parameter is optional</param>
		protected void EndParameter(bool optional)
		{
			if (headerAt < 0) throw new Exception("No header found");
			int pBalance = 0;
			for (int i = headerAt; i < sb.Length; ++i)
			{
				if (sb[i] == '\\')
					++i;
				else if (sb[i] == '(')
					++pBalance;
				else if (sb[i] == ')')
				{
					if (pBalance < 1) throw new Exception("Malformed string");
					--pBalance;
				}
			}

			while (pBalance > 0)
			{
				sb.Append(')');
				--pBalance;
			}
			if (optional) sb.Append('?');
			headerAt = -1;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Signature GenerateSignature()
		{
			Signature signature = new Signature();


			return signature;
		}

		/// <summary>
		/// Normalizes the input string to prevent to contain special characters used in regex
		/// </summary>
		/// <param name="stringToNormalize">The string to normalize</param>
		/// <returns>The input string normalized</returns>
		protected string NormalizeEscapes(string stringToNormalize)
		{
			string[] charsToNormalize = {".", "$", "^", "{", "[", "(", "|", ")", "*", "+", "?", "\\" };
			string normalized = stringToNormalize;

			for (int i = 0; i < charsToNormalize.Length; ++i)
			{
				normalized = normalized.Replace(charsToNormalize[i] , "\\" + charsToNormalize[i]);
			}
			return normalized;
		}

		#region Add elements

		/// <summary>
		/// Inserts a string type parameter which must be composed of characters from 'A' to 'Z' or 'a' to 'z'
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddAnsiWord(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"[A-Za-z]+");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a single character type parameter
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddCharacter(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"\.");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a constant string which may apear as paramteter
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="constant">The constant string</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddConstant(string paramName, string constant, bool optional)
		{
			if ((constant == null) || (constant.Length < 1)) throw new ArgumentException("Constant string must not be null or zero-length", "constant");
			string normalizedConstant = NormalizeEscapes(constant);
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(normalizedConstant);
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts an OR-ed array of constant strings which may apear as paramteter
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="constants">Array of constants</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddConstant(string paramName, string[] constants, bool optional)
		{
			string[] normalizedConstants;
			int i;

			if ((constants == null) || (constants.Length < 1))
				throw new ArgumentException("Invalid constants array. Constant strings must not be empty nor null", "constants");

			normalizedConstants = new string[constants.Length];
			for (i = 0; i < constants.Length; ++i)
			{
				if ((constants[i] == null) || (constants[i].Length < 1))
					throw new ArgumentException("Invalid constant string. Constant strings must not be empty nor null", "constants");
				normalizedConstants[i] = NormalizeEscapes(constants[i]);
			}
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append('(');
				sb.Append(normalizedConstants[0]);
				for (i = 1; i < normalizedConstants.Length; ++i)
				{
					sb.Append('|');
					sb.Append(normalizedConstants[i]);
				}
				sb.Append(@")");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a string used as delimiter (one or more occurences)
		/// </summary>
		/// <param name="delimiter">The string used as delimiter</param>
		public void AddDelimiter(string delimiter)
		{
			if ((delimiter == null) || (delimiter.Length < 1))
				throw new ArgumentException("Constant string must not be null or zero-length", "constant");
			string normalizedDelimiter = NormalizeEscapes(delimiter);
			lock(sb)
			{
				sb.Append('(');
				sb.Append(normalizedDelimiter);
				sb.Append(@")+");
			}
		}

		/// <summary>
		/// Inserts a string used as delimiter (one or more occurences)
		/// </summary>
		/// <param name="delimiter">Array of valid delimiter string</param>
		public void AddDelimiter(string[] delimiter)
		{
			string[] normalizedDelimiters;
			int i;

			if((delimiter == null) || (delimiter.Length < 1))
				throw new ArgumentException("Invalid delimiter array. Delimiter strings must not be empty nor null", "delimiter");
			
			normalizedDelimiters = new string[delimiter.Length];
			for (i = 0; i < delimiter.Length; ++i)
			{
				if ((delimiter[i] == null) || (delimiter[i].Length < 1))
					throw new ArgumentException("Invalid delimiter string. Delimiter strings must not be empty nor null", "delimiter");
				normalizedDelimiters[i] = NormalizeEscapes(delimiter[i]);
			}
			lock(sb)
			{
				sb.Append('(');
				sb.Append(normalizedDelimiters[0]);
				for (i = 1; i < normalizedDelimiters.Length; ++i)
				{
					sb.Append('|');
					sb.Append(normalizedDelimiters[i]);
				}
				sb.Append(@")+");
			}
		}

		/// <summary>
		/// Inserts a integer number type parameter in hexadecimal format
		/// The parameter may be preceded by a 0x, and the letters may be uppercase or lowercase.
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddHexadecimal(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"(0x)?");
				sb.Append(@"[0-9A-Fa-f]+");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a C-type-identifier type parameter
		/// C type identifiers begin with a letter or underscore and may contain letters numbers and underscore
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddIdentifier(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"[A-Za-z_]");
				sb.Append(@"[0-9A-Za-z_]*");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a integer number type parameter
		/// The parameter may be preceded by a minus, but not by a plus.
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="signed">Indicates if the number may be negative (preceded by a minus)</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddIntegerNumber(string paramName, bool signed, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				if (signed) sb.Append(@"\-?");
				sb.Append(@"\d+");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a string type parameter which must be composed of characters from 'a' to 'z'
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddLowerCaseWord(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"[a-z]+");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a real number type parameter
		/// The parameter may be preceded by a minus, but not by a plus.
		/// If a dot character in the number is present, it must be preceded and succeded by a number.
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddRealNumber(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"\-?\d+(\.\d+)?");
				EndParameter(optional);
			}
		}

		/// <summary>
		/// Inserts a space delimter (one or more occurences)
		/// </summary>
		public void AddSpace()
		{
			lock(sb)
			{
				sb.Append(@"\s+");
			}
		}

		/// <summary>
		/// Inserts a string type parameter which must be composed of characters from 'A' to 'Z'
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddUpperCaseWord(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"[A-Z]+");
				EndParameter(optional);
			}
			
		}

		/// <summary>
		/// Inserts a string type parameter which must be composed of characters from letters, numbers, underscore and UNICODE
		/// </summary>
		/// <param name="paramName">The name of the parameter for retrieving</param>
		/// <param name="optional">Indicates if the parameter is optional</param>
		public void AddWord(string paramName, bool optional)
		{
			lock(sb)
			{
				BeginParameter(paramName);
				sb.Append(@"\w+");
				EndParameter(optional);
			}
		}

		#endregion

		#endregion
	
	*/

	#endregion

	#region Trash
	/*
	/// <summary>
	/// Specifies the type of a parameter of a message
	/// </summary>
	public enum MessageParameterType
	{
		/// <summary>
		/// An 8bit character
		/// </summary>
		Character,
		
		/// <summary>
		/// A constant (string) value
		/// </summary>
		Constant,
		
		/// <summary>
		/// A number in hex format
		/// </summary>
		Hexadecimal,
		
		/// <summary>
		/// An C-like identifier
		/// </summary>
		Identifier,
		
		/// <summary>
		/// A chain of lowercase ANSI characters from 'a' to 'z'
		/// </summary>
		LowerCaseWord,

		/// <summary>
		/// A signed integer number
		/// </summary>
		/// <remarks>The parameter may be preceded by a minus, but not by a plus</remarks>
		SignedInt,

		/// <summary>
		/// A space character (spaces, tabs, vtabs, etc)
		/// </summary>
		Space,

		/// <summary>
		/// A real number
		/// </summary>
		/// <remarks>The parameter may be preceded by a minus, but not by a plus.
		/// If a dot character in the number is present, it must be preceded and succeded by a number.</remarks>
		Real,
		
		/// <summary>
		/// A unsigned integer number
		/// </summary>
		UnsignedInt,

		/// <summary>
		/// A chain of uppercase ANSI characters from 'A' to 'Z'
		/// </summary>
		UpperCaseWord,
		
		/// <summary>
		/// An uniode word.
		/// An uniode word may contain underscore, numbers and kanas
		/// </summary>
		Word,

	}

	/// <summary>
	/// Represents an object capable of produce Signature objects
	/// </summary>
	public class SignatureBuilder
	{
		#region Variables
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SignatureBuilder
		/// </summary>
		public SignatureBuilder()
		{
		}

		#endregion

		#region Events
		#endregion

		#region Properties
		#endregion

		#region Methodos

		public void AddParameter(MessageParameterType parameterType, bool optional)
		{

		}

		public Signature GenerateSignature()
		{
			Signature signature = new Signature();
		}

		private string GetRxString(MessageParameterType parameterType, string paramName)
		{
			if ((paramName == null) || (paramName.Length == 0))
				throw new ArgumentException("paramName", "Invalid parameter name");

			StringBuilder sb = new StringBuilder();
			sb.Append("(?<" + paramName + ">");

			switch (parameterType)
			{
				case MessageParameterType.Character:
					sb.Append();

				case MessageParameterType.Constant:
					sb.Append();

				case MessageParameterType.Hexadecimal:
					sb.Append();

				case MessageParameterType.Identifier:
					sb.Append();

				case MessageParameterType.LowerCaseWord:
					sb.Append();

				case MessageParameterType.Real:
					sb.Append();

				case MessageParameterType.SignedInt:
					sb.Append();

				case MessageParameterType.Space:
					sb.Append();

				case MessageParameterType.UnsignedInt:
					sb.Append();

				case MessageParameterType.UpperCaseWord:
					sb.Append();

				case MessageParameterType.Word:
					sb.Append();
			}
		}

		#endregion
	}
	*/
	#endregion
}