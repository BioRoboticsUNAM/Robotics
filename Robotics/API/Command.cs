using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Robotics.API
{
	/// <summary>
	/// Represents a Command to be executed
	/// </summary>
	//[ComVisible(true)]
	//[Guid("C023E4DA-56EB-4371-8B17-00A9B4E4F8CD")]
	//[ClassInterface(ClassInterfaceType.None)]
	public class Command : BaseMessage
	{
		#region Variables

		#endregion

		#region Static Variables

		/// <summary>
		/// Regular expression used in the IsCommand method
		/// </summary>
		protected static readonly Regex rxIsCommand = new Regex(@"^([A-Za-z][A-Za-z\-]*(\s+[A-Za-z][A-Za-z\-]*)?\s+)?[A-Za-z_]+(\s+""(\\.|[^""])*"")?(\s+@\d+)?$");
		/// <summary>
		/// Regular expression used in the Parse method
		/// </summary>
		protected static readonly Regex rxParse = new Regex(@"^((?<src>[A-Za-z][A-Za-z\-]*)\s+)?(?<cmd>[A-Za-z_]+)(\s+""(?<params>(\\.|[^""])*)"")?(\s+@(?<id>\d+))?$");

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		public Command(string command, string parameters)
			:this(null, null, null, null, command, parameters, -1) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		public Command(string command, string parameters, int id)
			: this(null, null, null, null, command, parameters, id) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		public Command(IMessageSource messageSource, string command, string parameters, int id)
			: this(messageSource, null, messageSource != null ? messageSource.ModuleName : null, null, command, parameters, id) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="messageSourceMetadata">Aditional data provided by the source of the message, like an IPEndPoint or a Delegate</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		public Command(IMessageSource messageSource, object messageSourceMetadata, string command, string parameters, int id)
			: this(messageSource, messageSourceMetadata, messageSource != null ? messageSource.ModuleName : null, null, command, parameters, id) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="source">The source module of the command</param>
		/// <param name="destination">The destination module of the command</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		public Command(string source, string destination, string command, string parameters, int id)
			: this(null, null, source, destination, command, parameters, id) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="source">The source module of the command</param>
		/// <param name="destination">The destination module of the command</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		protected Command(IMessageSource messageSource, string source, string destination, string command, string parameters, int id)
			: this(messageSource, null, source, destination, command, parameters, id) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="messageSourceMetadata">Aditional data provided by the source of the message, like an IPEndPoint or a Delegate</param>
		/// <param name="source">The source module of the command</param>
		/// <param name="destination">The destination module of the command</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		protected Command(IMessageSource messageSource, object messageSourceMetadata, string source, string destination, string command, string parameters, int id)
			: base(messageSource, messageSourceMetadata, source, destination, command, parameters, id) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the Command object represents a system command
		/// </summary>
		public bool IsSystemCommand
		{
			get
			{
				return rxIsSystemCommandName.IsMatch(this.command) && !HasParams;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Gets a value indicating if provided Response is a response for current command
		/// </summary>
		/// <param name="response">Response to check</param>
		/// <returns>true if provided Response is a response for command, false otherwise</returns>
		public bool IsMatch(Response response)
		{
			bool matchId = ((this.Id != -1) && (response.Id != -1)) ? (this.Id == response.Id) : true;
			return (this.CommandName == response.CommandName) && matchId;
		}

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

			if ((destinationModule != null) && (destinationModule == null)) sb.Append("Unknown");
			else sb.Append(destinationModule);

			sb.Append(' ');

			sb.Append(command);
			if (HasParams)
			{
				sb.Append(" \"");
				sb.Append(EscapedParameters);
				sb.Append("\"");
			}

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
		/// <param name="c">Command to be converted</param>
		/// <returns>A string well formated</returns>
		public static implicit operator string(Command c)
		{
			return c.StringToSend;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Checks if input string is a command
		/// </summary>
		/// <param name="s">string to analyze</param>
		/// <returns>true if input is command, false otherwise</returns>
		public static bool IsCommand(string s)
		{
			Regex rx;

			rx = rxIsCommand;
			return rx.IsMatch(s);
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// <param name="s">A string containing the command to convert</param>
		/// </summary>
		/// <returns>A command object that represents the command contained in s</returns>
		public static Command Parse(string s)
		{
			Command command;
			if (!TryParse(s, out command))
				throw new ArgumentException("Invalid String", "s");
			
			return command;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="result">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(string s, out Command result)
		{
			result = null;
			Regex rx;
			Match m;
			//ModuleClient source;
			//ModuleClient destination;
			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			int id;

			// Regular Expresion Generation
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
			sDest = ""; //  m.Result("${dest}");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);

			// Check if command matchs a prototype
			sParams = sParams.Replace("\\\"", "\"");
			result = new Command(sSrc, sDest, sCommand, sParams, id);
			//cmd.sentTime = DateTime.Now;
			return true;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// <param name="source">The IMessageSource object source of the command parsed</param>
		/// <param name="s">A string containing the command to convert</param>
		/// </summary>
		/// <returns>A command object that represents the command contained in s</returns>
		public static Command Parse(IMessageSource source, string s)
		{
			Regex rx;
			Match m;
			Command cmd;
			//ModuleClient source;
			//ModuleClient destination;
			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			int id;

			// Regular Expresion Generation
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
			sDest = ""; //  m.Result("${dest}");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);

			// Check if command matchs a prototype
			sParams = sParams.Replace("\\\"", "\"");
			cmd = new Command(sSrc, sDest, sCommand, sParams, id);
			cmd.MessageSource = source;
			//cmd.sentTime = DateTime.Now;
			return cmd;
		}

		/// <summary>
		/// Converts the string representation of a command to a Command object.
		/// A return value indicates whether the conversion succeded or not.
		/// <param name="source">The IMessageSource object source of the command parsed</param>
		/// <param name="s">A string containing the command to convert</param>
		/// <param name="result">When this method returns, contains the Command equivalent to the command
		/// contained in s, if the conversion succeeded, or null if the conversion failed.
		/// The conversion fails if the s parameter is a null reference (Nothing in Visual Basic) or is not of the correct format.
		/// This parameter is passed uninitialized</param>
		/// </summary>
		/// <returns>true if conversion was successfull, false otherwise</returns>
		public static bool TryParse(IMessageSource source, string s, out Command result)
		{
			result = null;
			Regex rx;
			Match m;
			//ModuleClient source;
			//ModuleClient destination;
			string sCommand;
			string sParams;
			string sSrc;
			string sDest;
			string sId;
			int id;

			// Regular Expresion Generation
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
			sDest = ""; //  m.Result("${dest}");
			if ((sId == null) || (sId.Length < 1)) id = -1;
			else id = Int32.Parse(sId);

			// Check if command matchs a prototype
			sParams = sParams.Replace("\\\"", "\"");
			result = new Command(sSrc, sDest, sCommand, sParams, id);
			result.MessageSource = source;
			//cmd.sentTime = DateTime.Now;
			return true;
		}

		#endregion

	}
}
