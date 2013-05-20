using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Robotics.API
{
	/// <summary>
	/// Represents the response of a command
	/// </summary>
	//[ComVisible(true)]
	//[Guid("5CD27788-2A45-4bf2-BB99-C4C17270BEE7")]
	//[ClassInterface(ClassInterfaceType.None)]
	public class Response : BaseMessage
	{
		#region Variables

		/// <summary>
		/// Stores the result contained in response
		/// </summary>
		protected bool success;

		#endregion

		#region Static Variables

		/// <summary>
		/// Regular expression used in the IsResponse method
		/// </summary>
		protected static Regex rxIsResponse = new Regex(@"^([A-Za-z][A-Za-z\-]*(\s+[A-Za-z][A-Za-z\-]*)?\s+)?[A-Za-z_]+(\s+""(\\.|[^""])*"")?\s+[10](\s+@\d+)?$");
		/// <summary>
		/// Regular expression used in the Parse method
		/// </summary>
		protected static Regex rxParse = new Regex(@"^((?<src>[A-Za-z][A-Za-z\-]*)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>(\\.|[^""])*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?$");

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The response parameters</param>
		/// <param name="success">The result contained in response</param>
		public Response(string command, string parameters, bool success)
			: this(null, null, command, parameters, success, -1) { }

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The response parameters</param>
		/// <param name="success">The result contained in response</param>
		/// <param name="id">The response id</param>
		public Response(string command, string parameters, bool success, int id)
			: this(null, null, command, parameters, success, id) { }

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="command">The command name</param>
		/// <param name="success">The result contained in response</param>
		public Response(IMessageSource messageSource, string command, bool success)
			: this(messageSource, null, messageSource != null ? messageSource.ModuleName : null, null, command, String.Empty, success, -1) { }

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The response parameters</param>
		/// <param name="success">The result contained in response</param>
		public Response(IMessageSource messageSource, string command, string parameters, bool success)
			: this(messageSource, null, messageSource != null ? messageSource.ModuleName : null, null, command, parameters, success, -1) { }

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The response parameters</param>
		/// <param name="success">The result contained in response</param>
		/// <param name="id">The response id</param>
		public Response(IMessageSource messageSource, string command, string parameters, bool success, int id)
			: this(messageSource, null, messageSource != null ? messageSource.ModuleName : null, null, command, parameters, success, id) { }

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="messageSourceMetadata">Aditional data provided by the source of the message, like an IPEndPoint or a Delegate</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The response parameters</param>
		/// <param name="success">The result contained in response</param>
		/// <param name="id">The response id</param>
		public Response(IMessageSource messageSource, object messageSourceMetadata, string command, string parameters, bool success, int id)
			: this(messageSource, messageSourceMetadata, messageSource != null ? messageSource.ModuleName : null, null, command, parameters, success, id) { }

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="source">The source module of the response</param>
		/// <param name="destination">The destination module of the response</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The response parameters</param>
		/// <param name="success">The result contained in response</param>
		/// <param name="id">The response id</param>
		public Response(string source, string destination, string command, string parameters, bool success, int id)
			:base(source, destination, command, parameters, id)
		{
			this.success = success;
		}

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="source">The source module of the response</param>
		/// <param name="destination">The destination module of the response</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The response parameters</param>
		/// <param name="success">The result contained in response</param>
		/// <param name="id">The response id</param>
		protected Response(IMessageSource messageSource, string source, string destination, string command, string parameters, bool success, int id)
			: this(messageSource, null, source, destination, command, parameters, success, id) { }

		/// <summary>
		/// Initializes a new instance of Response
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="messageSourceMetadata">Aditional data provided by the source of the message, like an IPEndPoint or a Delegate</param>
		/// <param name="source">The source module of the response</param>
		/// <param name="destination">The destination module of the response</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The response parameters</param>
		/// <param name="success">The result contained in response</param>
		/// <param name="id">The response id</param>
		protected Response(IMessageSource messageSource, object messageSourceMetadata, string source, string destination, string command, string parameters, bool success, int id)
			: base(messageSource, messageSourceMetadata, source, destination, command, parameters, id)
		{
			this.success = success;
		}

		#endregion

		#region Properties
		
		/// <summary>
		/// Gets the result contained in response
		/// </summary>
		public bool Success
		{
			get { return success; }
		}

		/// <summary>
		/// Gets a string which can be sent to a module
		/// </summary>
		public override string StringToSend
		{
			get
			{
				StringBuilder sb = new StringBuilder(255);
				sb.Append(command);
				if (HasParams)
				{
					sb.Append(" \"");
					sb.Append(parameters);
					sb.Append("\"");
				}

				sb.Append(success ? " 1" : " 0");

				if (id >= 0)
				{
					sb.Append(" @");
					sb.Append(id);
				}

				return sb.ToString();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a String that represents the current Object. (Inherited from Object.)
		/// </summary>
		/// <returns>String that represents the current Object</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(255);

			if (sourceModule == null) sb.Append("Unknown");
			else sb.Append(sourceModule);

			sb.Append(' ');

			try
			{
				if ((destinationModule != null) && (destinationModule == null)) sb.Append("Unknown");
				else sb.Append(destinationModule);
			}
			catch { }

			sb.Append(' ');

			sb.Append(command);
			if (HasParams)
			{
				sb.Append(" \"");
				sb.Append(parameters);
				sb.Append("\"");
			}

			if (success) sb.Append(" 1");
			else sb.Append(" 0");

			if (id >= 0)
			{
				sb.Append(" @");
				sb.Append(id);
			}

			return sb.ToString();
		}

		#endregion

		#region Operators
		/// <summary>
		/// Implicitly converts the Message to a string which can be sent to a module
		/// </summary>
		/// <param name="c">Response to be converted</param>
		/// <returns>A string well formated</returns>
		public static implicit operator string(Response c)
		{
			return c.StringToSend;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Creates a response from a command data
		/// </summary>
		/// <param name="command">The command to use as base for the response</param>
		/// <param name="result">true if command succeded, false otherwise</param>
		/// <returns>A generic response for the command with same parameters</returns>
		public static Response CreateFromCommand(Command command, bool result)
		{
			return new Response
			(
				command.MessageSource,
				command.MessageSourceMetadata,
				command.DestinationModule,
				command.SourceModule,
				command.CommandName,
				command.Parameters,
				result,
				command.Id
			);
		}

		/// <summary>
		/// Checks if input string is a command response
		/// </summary>
		/// <param name="s">string to analyze</param>
		/// <returns>true if input is command response, false otherwise</returns>
		public static bool IsResponse(string s)
		{
			Regex rx;

			//rx = new Regex(@"^([\w\-]+(\s+[\w\-]+)?\s+)?[A-Za-z_]+(\s+""[^""]*"")?\s+[10](\s+@\d+)?$");
			rx = rxIsResponse;
			return rx.IsMatch(s);
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// <param name="s">A string containing the response to convert</param>
		/// </summary>
		/// <returns>A response object that represents the response contained in s</returns>
		public static Response Parse(string s)
		{
			Regex rx;
			Match m;
			Response res;

			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			string sResult;
			bool responseResult;
			int id;

			// Regular Expresion Generation
			//rx = new Regex(@"((?<src>[\w\-]+)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?");
			rx = rxParse;
			m = rx.Match(s);
			// Check if input string matchs Regular Expression
			if (!m.Success)
				throw new ArgumentException("Invalid String", "s");
			// Extract Data
			sCommand = m.Result("${cmd}").ToLower();
			sParams = m.Result("${params}");
			sId = m.Result("${id}");
			sSrc = m.Result("${src}");
			sDest = ""; // m.Result("${dest}");
			sResult = m.Result("${result}");
			if ((sResult == null) || ((sResult != "1") && (sResult != "0")))
				throw new Exception("Invalid string. No suitable result value found");
			responseResult = (sResult == "1");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);

			// Check if response matchs a prototype
			//if (((sParams == null) || (sParams.Length < 1)))
			//	throw new Exception("Invalid string. The Response requires parameters");
			// Create the Response
			sParams = sParams.Replace("\\\"", "\"");
			res = new Response(sSrc, sDest, sCommand, sParams, responseResult, id);
			return res;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="result">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, out Response result)
		{
			result = null;
			Regex rx;
			Match m;

			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			string sResult;
			bool responseResult;
			int id;

			// Regular Expresion Generation
			//rx = new Regex(@"((?<src>[\w\-]+)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?");
			rx = rxParse;
			m = rx.Match(s);
			// Check if input string matchs Regular Expression
			if (!m.Success)
				return false;
			// Extract Data
			sCommand = m.Result("${cmd}").ToLower();
			sParams = m.Result("${params}");
			sId = m.Result("${id}");
			sSrc = m.Result("${src}");
			sDest = ""; // m.Result("${dest}");
			sResult = m.Result("${result}");
			if ((sResult == null) || ((sResult != "1") && (sResult != "0")))
				return false;
			responseResult = (sResult == "1");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);

			// Check if response matchs a prototype
			//if (((sParams == null) || (sParams.Length < 1)))
			//	throw new Exception("Invalid string. The Response requires parameters");
			// Create the Response
			sParams = sParams.Replace("\\\"", "\"");
			result = new Response(sSrc, sDest, sCommand, sParams, responseResult, id);
			return true;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// <param name="source">The IMessageSource object source of the command parsed</param>
		/// <param name="s">A string containing the response to convert</param>
		/// </summary>
		/// <returns>A response object that represents the response contained in s</returns>
		public static Response Parse(IMessageSource source, string s)
		{
			Regex rx;
			Match m;
			Response res;

			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			string sResult;
			bool responseResult;
			int id;

			// Regular Expresion Generation
			//rx = new Regex(@"((?<src>[\w\-]+)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?");
			rx = rxParse;
			m = rx.Match(s);
			// Check if input string matchs Regular Expression
			if (!m.Success)
				throw new ArgumentException("Invalid String", "s");
			// Extract Data
			sCommand = m.Result("${cmd}").ToLower();
			sParams = m.Result("${params}");
			sId = m.Result("${id}");
			sSrc = m.Result("${src}");
			if ((source != null) && ((sSrc == null) || (sSrc == "")))
				sSrc = source.ModuleName;
			sDest = ""; // m.Result("${dest}");
			sResult = m.Result("${result}");
			if ((sResult == null) || ((sResult != "1") && (sResult != "0")))
				throw new Exception("Invalid string. No suitable result value found");
			responseResult = (sResult == "1");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);

			// Check if response matchs a prototype
			//if (((sParams == null) || (sParams.Length < 1)))
			//	throw new Exception("Invalid string. The Response requires parameters");
			// Create the Response
			sParams = sParams.Replace("\\\"", "\"");
			res = new Response(sSrc, sDest, sCommand, sParams, responseResult, id);
			res.MessageSource = source;
			return res;
		}

		/// <summary>
		/// Converts the string representation of a response to a Response object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="source">The IMessageSource object source of the command parsed</param>
		/// <param name="s">A string containing the response to convert</param>
		/// <param name="result">When this method returns, contains the Response equivalent to the response
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(IMessageSource source, string s, out Response result)
		{
			result = null;
			Regex rx;
			Match m;

			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			string sResult;
			bool responseResult;
			int id;

			// Regular Expresion Generation
			//rx = new Regex(@"((?<src>[\w\-]+)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>[^""]*)"")?\s+(?<result>[10])(\s+@(?<id>\d+))?");
			rx = rxParse;
			m = rx.Match(s);
			// Check if input string matchs Regular Expression
			if (!m.Success)
				return false;
			// Extract Data
			sCommand = m.Result("${cmd}").ToLower();
			sParams = m.Result("${params}");
			sId = m.Result("${id}");
			sSrc = m.Result("${src}");
			if ((source != null) && ((sSrc == null) || (sSrc == "")))
				sSrc = source.ModuleName;
			sDest = ""; // m.Result("${dest}");
			sResult = m.Result("${result}");
			if ((sResult == null) || ((sResult != "1") && (sResult != "0")))
				return false;
			responseResult = (sResult == "1");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);

			// Check if response matchs a prototype
			//if (((sParams == null) || (sParams.Length < 1)))
			//	throw new Exception("Invalid string. The Response requires parameters");
			// Create the Response
			sParams = sParams.Replace("\\\"", "\"");
			result = new Response(sSrc, sDest, sCommand, sParams, responseResult, id);
			result.MessageSource = source;
			return true;
		}

		#endregion
	}
}
