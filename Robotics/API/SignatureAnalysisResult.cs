using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API
{
	/// <summary>
	/// Represents the result of the analysis of the parameters of a BaseMessage object
	/// </summary>
	public class SignatureAnalysisResult
	{

		#region Variables

		/// <summary>
		/// Stores the delegate selected by the Signature object when the BaseMessage object was parsed
		/// </summary>
		protected readonly Delegate asociatedDelegate;
		
		/// <summary>
		/// The BaseMessage object which originates the result
		/// </summary>
		protected readonly BaseMessage message;

		/// <summary>
		/// List of the names of the parameters
		/// </summary>
		protected readonly SortedList<string, int> parameterNames;

		/// <summary>
		/// List of parameters
		/// </summary>
		protected readonly List<object> parameters;

		/// <summary>
		/// Stores a value indicating if the analysis succeded
		/// </summary>
		protected readonly bool success;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SignatureAnalysisResult
		/// </summary>
		/// <param name="message">The BaseMessage object which originates the result</param>
		/// <param name="success">The value indicating if the analysis succeded</param>
		/// <param name="asociatedDelegate">The delegate selected by the Signature object when the BaseMessage object was parsed</param>
		protected internal SignatureAnalysisResult(BaseMessage message, bool success, Delegate asociatedDelegate)
		{
			if (message == null) throw new ArgumentNullException("message");
			this.message = message;
			this.success = success;
			this.asociatedDelegate = asociatedDelegate;
			this.parameterNames = new SortedList<string, int>();
			this.parameters = new List<object>();
		}

		/// <summary>
		/// Initializes a new instance of SignatureAnalysisResult
		/// </summary>
		/// <param name="message">The BaseMessage object which originates the result</param>
		/// <param name="success">The value indicating if the analysis succeded</param>
		/// <param name="asociatedDelegate">The delegate selected by the Signature object when the BaseMessage object was parsed</param>
		/// <param name="namedParameters">The list of parameters sorted by its name</param>
		protected internal SignatureAnalysisResult(BaseMessage message, bool success, Delegate asociatedDelegate, Dictionary<string, object> namedParameters)
			: this(message, success, asociatedDelegate)
		{
			if (namedParameters == null) throw new ArgumentNullException("namedParameters");
			int i = 0;
			foreach (string key in namedParameters.Keys)
			{
				this.parameterNames.Add(key, i++);
				this.parameters.Add(namedParameters[key]);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the delegate selected by the Signature object when the BaseMessage object was parsed
		/// </summary>
		protected Delegate AsociatedDelegate
		{
			get { return asociatedDelegate; }
		}

		/// <summary>
		/// Gets the BaseMessage object which originates the result
		/// </summary>
		public BaseMessage Message
		{
			get { return this.message; }
		}

		/// <summary>
		/// Gets the number of parameters contained in this SignatureAnalysisResult object
		/// </summary>
		public int ParameterCount
		{
			get { return parameterNames.Count; }
		}

		/// <summary>
		/// Gets an array containing the names of the parameters contained in this SignatureAnalysisResult object
		/// </summary>
		public string[] ParameterNames
		{
			get
			{
				string[] names = new string[this.parameterNames.Count];
				int i = 0;
				foreach (string key in parameterNames.Keys)
					names[i++] = key;
				return names;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the match between the BaseMessage object and the Signature object is successful
		/// </summary>
		public bool Success
		{
			get { return success; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Executes the method represented by the asociated Delegate object
		/// </summary>
		/// <returns>Result of the Delegate execution</returns>
		public object Execute()
		{
			if (asociatedDelegate == null)
				return null;

			return asociatedDelegate.DynamicInvoke(parameters.ToArray());
			//return asociatedDelegate.Method.Invoke(asociatedDelegate.Target, parameters.ToArray());
		}

		#region GetParameter

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <typeparam name="T">The type of the returned data</typeparam>
		/// <param name="paramNumber">The number of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter<T>(int paramNumber, out T value)
		{
			Type type = typeof(T);
			value = default(T);
			if ((paramNumber < 0) || (paramNumber >= parameters.Count))
				return false;

			object oValue = parameters[paramNumber];
			if (oValue.GetType() != type)
				return false;
			value = (T)oValue;
			return true;
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <typeparam name="T">The type of the returned data</typeparam>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter<T>(string paramName, out T value)
		{
			Type type = typeof(T);
			value = default(T);
			if (!parameterNames.ContainsKey(paramName))
				return false;

			object oValue = parameters[parameterNames[paramName]];
			if (oValue.GetType() != type)
				return false;
			value = (T)oValue;
			return true;
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out object value)
		{
			return GetParameter<object>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out byte value)
		{
			return GetParameter<byte>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out byte[] value)
		{
			return GetParameter<byte[]>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out char value)
		{
			return GetParameter<char>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out char[] value)
		{
			return GetParameter<char[]>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out short value)
		{
			return GetParameter<short>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out int value)
		{
			return GetParameter<int>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out long value)
		{
			return GetParameter<long>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out ushort value)
		{
			return GetParameter<ushort>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out uint value)
		{
			return GetParameter<uint>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out ulong value)
		{
			return GetParameter<ulong>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out float value)
		{
			return GetParameter<float>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out double value)
		{
			return GetParameter<double>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out string value)
		{
			return GetParameter<string>(paramName, out value);
		}

		/// <summary>
		/// Gets a parameter by its name
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist or type mismatch contains the default value of the type</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool GetParameter(string paramName, out string[] value)
		{
			return GetParameter<string[]>(paramName, out value);
		}

		#endregion

		#region Update

		/// <summary>
		/// Updates the provided value with the data contained in the correspodng parameter of the BaseMessage object which parameters was analyzed
		/// </summary>
		/// <typeparam name="T">The type of the returned data</typeparam>
		/// <param name="paramNumber">The number of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist preserves its value</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update<T>(int paramNumber, ref T value)
		{
			Type type = typeof(T);
			if ((paramNumber < 0) || (paramNumber >= parameters.Count))
				return false;

			object oValue = parameters[paramNumber];
			if (oValue.GetType() != type)
				return false;
			value = (T)oValue;
			return true;
		}

		/// <summary>
		/// Updates the provided value with the data contained in the correspodng parameter of the BaseMessage object which parameters was analyzed
		/// </summary>
		/// <typeparam name="T">The type of the returned data</typeparam>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value contained in the parameter which name was requested if exists. If the parameter does not exist preserves its value</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update<T>(string paramName, ref T value)
		{
			Type type = typeof(T);
			if (!parameterNames.ContainsKey(paramName))
				return false;

			object oValue = parameters[parameterNames[paramName]];
			if (oValue.GetType() != type)
				return false;
			value = (T)oValue;
			return true;
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref object value)
		{
			return Update<object>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref byte value)
		{
			return Update<byte>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref byte[] value)
		{
			return Update<byte[]>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref char value)
		{
			return Update<char>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref char[] value)
		{
			return Update<char[]>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref short value)
		{
			return Update<short>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref int value)
		{
			return Update<int>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref long value)
		{
			return Update<long>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref ushort value)
		{
			return Update<ushort>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref uint value)
		{
			return Update<uint>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref ulong value)
		{
			return Update<ulong>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref float value)
		{
			return Update<float>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref double value)
		{
			return Update<double>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref string value)
		{
			return Update<string>(paramName, ref value);
		}

		/// <summary>
		/// Updates the content of the provided variable with the data of the specified parmaeter
		/// </summary>
		/// <param name="paramName">The name of the parameter to fetch</param>
		/// <param name="value">When this method returns, contains the value of the specified parameter if the parameter exists an the types match</param>
		/// <returns>true if the parameter exists and it's type is the specified, false othewrise</returns>
		public bool Update(string paramName, ref string[] value)
		{
			return Update<string[]>(paramName, ref value);
		}

		#endregion

		#endregion

	}
}
