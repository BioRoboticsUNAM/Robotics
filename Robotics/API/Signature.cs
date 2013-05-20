using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;


namespace Robotics.API
{
	/// <summary>
	/// Represents the signature of a BaseMessage object
	/// </summary>
	/// <remarks>The sinature of a command describes the parameters and parameter types with 
	/// which a legal call to the command can be made</remarks>
	public class Signature
	{
		#region Variables
		
		/// <summary>
		/// The name of the command this signature is valid for
		/// </summary>
		protected string commandName;
		/// <summary>
		/// Regular expression used to validate the command signature
		/// </summary>
		protected Regex rx;
		/// <summary>
		/// Stores the pattern used to create the Signature object
		/// </summary>
		private readonly string pattern;

		/// <summary>
		/// Stores the correspondance list between delegate number and delegate
		/// </summary>
		private Dictionary<string, Delegate> delegateList;

		/// <summary>
		/// Stores the list of added arrays of types
		/// </summary>
		private Dictionary<string, Type[]> typeArrayList;

		/*
		/// <summary>
		/// Stores the correspondance list between parameter name and parameter number
		/// </summary>
		private Dictionary<string, int> parameterList;
		*/

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Signature
		/// </summary>
		/// <param name="commandName">The name of command this signature will manage</param>
		/// <param name="delegateList">List of asociated delegates for parameter parse</param>
		/// <param name="pattern">The pattern for the regular expression</param>
		internal Signature(string commandName, string pattern, Dictionary<string, Delegate> delegateList)
			: this(commandName, pattern, delegateList, new Dictionary<string, Type[]>()) { }

		/// <summary>
		/// Initializes a new instance of Signature
		/// </summary>
		/// <param name="commandName">The name of command this signature will manage</param>
		/// <param name="pattern">The pattern for the regular expression</param>
		/// <param name="delegateList">List of asociated delegates for parameter parse</param>
		/// <param name="typeArrayList">The list of added arrays of types</param>
		internal Signature(string commandName, string pattern, Dictionary<string, Delegate> delegateList, Dictionary<string, Type[]> typeArrayList)
		{
			this.commandName = commandName;
			this.pattern = pattern;
			this.rx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
			this.delegateList = new Dictionary<string, Delegate>();
			foreach (string key in delegateList.Keys)
				this.delegateList.Add(key, delegateList[key]);
			this.typeArrayList = new Dictionary<string, Type[]>();
			foreach (string key in typeArrayList.Keys)
				this.typeArrayList.Add(key, typeArrayList[key]);
			//this.parameterList = new Dictionary<string, int>();
		}

		#endregion

		#region Properties

		/*
		/// <summary>
		/// Gets the correspondance list between parameter name and parameter number
		/// </summary>
		protected internal Dictionary<string, int> ParameterList
		{
			get { return parameterList; }
		}
		*/

		/// <summary>
		/// Gets the regular expression pattern to match parameters
		/// </summary>
		public string RegexPattern
		{
			get { return pattern; }
		}

		/// <summary>
		/// Gets the name of the command this signature is designed for
		/// </summary>
		public string CommandName
		{
			get { return commandName; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Analyzes the arguments of the provided BaseMessage object
		/// </summary>
		/// <param name="message">The BaseMessage object to match</param>
		/// <returns>The result of the analisys</returns>
		public SignatureAnalysisResult Analyze(BaseMessage message)
		{
			if (message == null) throw new ArgumentNullException("message");

			if (message.CommandName != this.commandName)
				return new SignatureAnalysisResult(message, false, null);

			Dictionary<string, object> parameters;
			SignatureAnalysisResult result = new SignatureAnalysisResult(message, false, null);
			// Find adequate delegate
			string[] sParams;
			Delegate d = GetDelegate(message.Parameters, out sParams);
			if (d != null)
			{
				// Extract parameters
				ExtractParameters(d, sParams, out parameters);
				// Generate result
				result = new SignatureAnalysisResult(message, true, d, parameters);
				return result;
			}
			// If there is no delegate, try with array of types
			Type[] typeArray = GetTypeArray(message.Parameters, out sParams);
			if (typeArray != null)
			{
				// Extract parameters
				ExtractParameters(typeArray, sParams, out parameters);
				// Generate result
				result = new SignatureAnalysisResult(message, true, d, parameters);
			}
			return result;
		}

		/// <summary>
		/// Extracts the arguments of the provided BaseMessage object if it matches the signature
		/// </summary>
		/// <param name="message">The BaseMessage object to match</param>
		/// <param name="parameters">An array containing the parameters stored in the BaseMessage</param>
		/// <returns>true if the provided BaseMessage object matches the signature and the arguments was extracted, false otherwise</returns>
		public bool GetParameters(BaseMessage message, out object[] parameters)
		{
			parameters = null;
			if (message == null) throw new ArgumentNullException("message");
			if (message.CommandName != this.commandName)
				return false;

			// Find adequate delegate
			string[] sParams;
			Delegate d = GetDelegate(message.Parameters, out sParams);
			if (d != null)
			{
				// Extract parameters
				parameters = ExtractParameters(d, sParams);
				return true;
			}

			// Find adequate array of types
			Type[] typeArray = GetTypeArray(message.Parameters, out sParams);
			if (typeArray != null)
			{
				// Extract parameters
				parameters = ExtractParameters(typeArray, sParams);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Indicates whether the command name and parameters specified of the Signature object matches the
		/// command name and parameters of the provided BaseMessage object
		/// </summary>
		/// <param name="message">The BaseMessage object to match</param>
		/// <returns>true if the message matches the signature; otherwise, false</returns>
		public bool IsMatch(BaseMessage message)
		{
			if (message == null)
				return false;
			if(message.CommandName != this.commandName)
				return false;

			//Match m = rx.Match(message.Parameters);

			/*
			CaptureCollection cc = m.Captures;
			Console.WriteLine("Captures:");
			foreach (Capture c in cc)
				Console.WriteLine("\tNumber: " + c.Index.ToString() + " Length: " + c.Length.ToString() + " Value: " + c.Value);
			GroupCollection gc = m.Groups;
			Console.WriteLine("Groups:");
			foreach (Group g in gc)
			{
				Console.WriteLine("\tNumber: " + g.Index.ToString() + " Success: " + g.Success.ToString() + " Length: " + g.Length.ToString() + " Value: " + g.Value);
			}
			Console.WriteLine();
			*/

			return rx.IsMatch(message.Parameters);
		}

		/// <summary>
		/// Executes the appropiate delegate if the provided BaseMessage object matches the signature
		/// </summary>
		/// <param name="message">The BaseMessage object to match</param>
		/// <returns>true if the provided BaseMessage object matches the signature and the appropiate delegate was executed, false otherwise</returns>
		public bool CallIfMatch(BaseMessage message)
		{
			object executionResult;
			return CallIfMatch(message, out executionResult);
		}

		/// <summary>
		/// Executes the appropiate delegate if the provided BaseMessage object matches the signature
		/// </summary>
		/// <param name="message">The BaseMessage object to match</param>
		/// <param name="executionResult">The object resulted of the execution ot the method represented by the asociated delegate</param>
		/// <returns>true if the provided BaseMessage object matches the signature and the appropiate delegate was executed, false otherwise</returns>
		public bool CallIfMatch(BaseMessage message, out object executionResult)
		{
			executionResult = null;
			if (message == null) throw new ArgumentNullException("message");
			if (message.CommandName != this.commandName)
				return false;

			// Find adequate delegate
			string[] sParams;
			object[] callParams;
			Delegate d = GetDelegate(message.Parameters, out sParams);
			if (d == null)
				return false;

			// Extract parameters
			callParams = ExtractParameters(d, sParams);
			executionResult = d.DynamicInvoke(callParams);
			//Method.Invoke(d.Target, callParams);

			return true;
		}

		/// <summary>
		/// Gets the asociated delegate which matches the provided parameter signature
		/// </summary>
		/// <param name="parameters">Parameters used to get the adequate delegate</param>
		/// <param name="sParams">Array which contains the extracted parameters for the delegate</param>
		/// <returns>The Delegate object asociated with the signature. If there is no adequare delegate returns null</returns>
		private Delegate GetDelegate(string parameters, out string[] sParams)
		{
			Match match;
			string[] names;
			List<string> paramList;
			Delegate d = null;
			int i;
			string name;
			Group group;

			sParams = null;
			match = rx.Match(parameters);
			if (!match.Success)
				return null;
			names = rx.GetGroupNames();
			paramList = new List<string>();
			for (i = 0; i < names.Length; ++i )
			{
				name = names[i];
				if (name[0] != 'd') continue;
				group = match.Groups[name];
				if (group.Success)
				{
					d = delegateList[name];
					break;
				}
			}

			if (d == null)
				return null;

			for (i = i + 1; i < names.Length; ++i)
			{
				name = names[i];
				if (name[0] != 'p') break;
				group = match.Groups[name];
				paramList.Add(group.Value);
			}
			sParams = paramList.ToArray();
			return d;
		}

		/// <summary>
		/// Gets the asociated array of types which matches the provided parameter signature
		/// </summary>
		/// <param name="parameters">Parameters used to get the adequate array of types</param>
		/// <param name="sParams">Array which contains the extracted parameters for the delegate</param>
		/// <returns>The array of types object asociated with the signature. If there is no adequare delegate returns null</returns>
		private Type[] GetTypeArray(string parameters, out string[] sParams)
		{
			Match match;
			string[] names;
			List<string> paramList;
			Type[] typeArray = null;
			int i;
			string name;
			Group group;

			sParams = null;
			match = rx.Match(parameters);
			if (!match.Success)
				return null;
			names = rx.GetGroupNames();
			paramList = new List<string>();
			for (i = 0; i < names.Length; ++i)
			{
				name = names[i];
				if (!name.StartsWith("ta")) continue;
				group = match.Groups[name];
				if (group.Success)
				{
					typeArray = typeArrayList[name];
					break;
				}
			}

			if (typeArray == null)
				return null;

			for (i = i + 1; i < names.Length; ++i)
			{
				name = names[i];
				if (name[0] != 'p') break;
				group = match.Groups[name];
				paramList.Add(group.Value);
			}
			sParams = paramList.ToArray();
			return typeArray;
		}

		/// <summary>
		/// Extracts the parameters from a regular expression match for a delegate
		/// </summary>
		/// <param name="d">Delegate for get the object types</param>
		/// <param name="sParams">Array of strings containing the string representation of each parameter</param>
		/// <returns>The parameters for the Delegate</returns>
		private object[] ExtractParameters(Delegate d, string[] sParams)
		{
			Dictionary<string, object> namedParams;
			return ExtractParameters(d, sParams, out namedParams);
		}

		/// <summary>
		/// Extracts the parameters from a regular expression match for a delegate
		/// </summary>
		/// <param name="d">Delegate for get the object types</param>
		/// <param name="sParams">Array of strings containing the string representation of each parameter</param>
		/// <param name="paramsByName">The parameters for the Delegate ordered by name</param>
		/// <returns>The parameters for the Delegate</returns>
		private object[] ExtractParameters(Delegate d, string[] sParams, out Dictionary<string, object> paramsByName)
		{
			ParameterInfo[] delegateParams;
			object[] callParams;

			delegateParams = d.Method.GetParameters();
			if (delegateParams.Length != sParams.Length)
				throw new ArgumentException("The provided string array does not match the delegate param count", "sParams");
			if (delegateParams.Length == 0)
			{
				paramsByName = new Dictionary<string, object>();
				return null;
			}

			paramsByName = new Dictionary<string, object>(delegateParams.Length);
			callParams = new object[delegateParams.Length];

			for (int i = 0; i < delegateParams.Length; ++i)
			{
				ParameterInfo pi = delegateParams[i];

				object currentParam = ParseParameter(pi.ParameterType, sParams[i]);
				callParams[i] = currentParam;
				paramsByName.Add(pi.Name, currentParam);
			}
			return callParams;
		}

		/// <summary>
		/// Extracts the parameters from a regular expression match for a delegate
		/// </summary>
		/// <param name="typeArray">Array of types from which get the object types</param>
		/// <param name="sParams">Array of strings containing the string representation of each parameter</param>
		/// <returns>The parameters for the Delegate</returns>
		private object[] ExtractParameters(Type[] typeArray, string[] sParams)
		{
			Dictionary<string, object> namedParams;
			return ExtractParameters(typeArray, sParams, out namedParams);
		}

		/// <summary>
		/// Extracts the parameters from a regular expression match for a delegate
		/// </summary>
		/// <param name="typeArray">Array of types from which get the object types</param>
		/// <param name="sParams">Array of strings containing the string representation of each parameter</param>
		/// <param name="paramsByName">The parameters for the Delegate ordered by name</param>
		/// <returns>The parameters for the Delegate</returns>
		private object[] ExtractParameters(Type[] typeArray, string[] sParams, out Dictionary<string, object> paramsByName)
		{
			object[] callParams;

			if (typeArray.Length != sParams.Length)
				throw new ArgumentException("The provided string array does not match the typeArray param count", "sParams");
			if (typeArray.Length == 0)
			{
				paramsByName = new Dictionary<string, object>();
				return null;
			}

			paramsByName = new Dictionary<string, object>(typeArray.Length);
			callParams = new object[typeArray.Length];

			for (int i = 0; i < typeArray.Length; ++i)
			{
				object currentParam = ParseParameter(typeArray[i], sParams[i]);
				callParams[i] = currentParam;
				paramsByName.Add("p" + i.ToString(), currentParam);
			}
			return callParams;
		}

		/// <summary>
		/// Extracts the parameters from a regular expression match
		/// </summary>
		/// <param name="type">The expected type of the parameter</param>
		/// <param name="sParam">The string representation of the parameter</param>
		/// <returns>Converted parameter value</returns>
		private object ParseParameter(Type type, string sParam)
		{
			object currentParam;
			string[] parts;

			switch (type.Name)
			{
				//case "Byte":
				case "Byte[]":
					int j = 0;
					int len = sParam.Length;
					if (sParam.StartsWith("0x")) j += 2;
					byte[] data = new byte[len - j];
					for (int k = 0; j < len; j += 2, ++k)
						data[k] = Byte.Parse(sParam.Substring(j, Math.Min(2, len - j)), System.Globalization.NumberStyles.HexNumber);
					currentParam = data;
					break;

				case "Char":
					currentParam = Char.Parse(sParam);
					break;

				case "Char[]":
					currentParam = sParam.ToCharArray();
					break;

				case "Int16":
					currentParam = Int16.Parse(sParam);
					break;

				case "Int32":
					currentParam = Int32.Parse(sParam);
					break;

				case "Int64":
					currentParam = Int64.Parse(sParam);
					break;

				case "UInt16":
					currentParam = UInt16.Parse(sParam);
					break;

				case "UInt32":
					currentParam = UInt32.Parse(sParam);
					break;

				case "UInt64":
					currentParam = UInt64.Parse(sParam);
					break;

				case "Single":
					currentParam = Single.Parse(sParam);
					break;

				case "Double":
					currentParam = Double.Parse(sParam);
					break;

				case "String":
					currentParam = sParam;
					break;

				case "String[]":
					currentParam = Regex.Split(sParam, @"\s+");
					break;

				#region Arrays

				case "UInt16[]":
					parts = Regex.Split(sParam, @"\s+");
					List<UInt16> uselements = new List<UInt16>(parts.Length);
					UInt16 uselement;
					for (int i = 0; i < parts.Length; ++i)
					{
						if (UInt16.TryParse(parts[i], out uselement))
							uselements.Add(uselement);
					}
					currentParam = uselements.ToArray();
					break;

				case "UInt32[]":
					parts = Regex.Split(sParam, @"\s+");
					List<UInt32> uelements = new List<UInt32>(parts.Length);
					UInt32 uelement;
					for (int i = 0; i < parts.Length; ++i)
					{
						if (UInt32.TryParse(parts[i], out uelement))
							uelements.Add(uelement);
					}
					currentParam = uelements.ToArray();
					break;

				case "UInt64[]":
					parts = Regex.Split(sParam, @"\s+");
					List<UInt64> ulelements = new List<UInt64>(parts.Length);
					UInt64 ulelement;
					for (int i = 0; i < parts.Length; ++i)
					{
						if (UInt64.TryParse(parts[i], out ulelement))
							ulelements.Add(ulelement);
					}
					currentParam = ulelements.ToArray();
					break;

				case "Int16[]":
					parts = Regex.Split(sParam, @"\s+");
					List<Int16> selements = new List<Int16>(parts.Length);
					Int16 selement;
					for (int i = 0; i < parts.Length; ++i)
					{
						if (Int16.TryParse(parts[i], out selement))
							selements.Add(selement);
					}
					currentParam = selements.ToArray();
					break;

				case "Int32[]":
					parts = Regex.Split(sParam, @"\s+");
					List<Int32> elements = new List<Int32>(parts.Length);
					Int32 element;
					for (int i = 0; i < parts.Length; ++i)
					{
						if (Int32.TryParse(parts[i], out element))
							elements.Add(element);
					}
					currentParam = elements.ToArray();
					break;

				case "Int64[]":
					parts = Regex.Split(sParam, @"\s+");
					List<Int64> lelements = new List<Int64>(parts.Length);
					Int64 lelement;
					for (int i = 0; i < parts.Length; ++i)
					{
						if (Int64.TryParse(parts[i], out lelement))
							lelements.Add(lelement);
					}
					currentParam = lelements.ToArray();
					break;

				case "Single[]":
					parts = Regex.Split(sParam, @"\s+");
					List<Single> felements = new List<Single>(parts.Length);
					Single felement;
					for (int i = 0; i < parts.Length; ++i)
					{
						if (Single.TryParse(parts[i], out felement))
							felements.Add(felement);
					}
					currentParam = felements.ToArray();
					break;

				case "Double[]":
					parts = Regex.Split(sParam, @"\s+");
					List<Double> delements = new List<Double>(parts.Length);
					Double delement;
					for (int i = 0; i < parts.Length; ++i)
					{
						if (Double.TryParse(parts[i], out delement))
							delements.Add(delement);
					}
					currentParam = delements.ToArray();
					break;

				#endregion

				default:
					throw new Exception("Unsupported data type in delegate");
			}

			return currentParam;
		}

		#endregion
	}
}
