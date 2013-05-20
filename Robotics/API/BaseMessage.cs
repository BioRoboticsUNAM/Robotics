using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Robotics.API
{
	/// <summary>
	/// Provides the baase methods for basic commands and it's responses
	/// </summary>
	//[ComVisible(true)]
	//[Guid("659C296D-8453-49a6-9905-F9CD8EDC814D")]
	//[ClassInterface(ClassInterfaceType.None)]
	public abstract class BaseMessage :IComparable, IComparable<BaseMessage>, IBaseMessage
	{
		#region Variables
		/// <summary>
		/// Stores the Source of the message, like a ConnectionManager or a Form capable of manage responses
		/// </summary>
		protected IMessageSource messageSource;
		/// <summary>
		/// Stores aditional data provided by the source of the message, like an IPEndPoint or a Delegate
		/// </summary>
		protected object messageSourceMetadata;
		/// <summary>
		/// Stores the source module of the command
		/// </summary>
		protected string sourceModule;
		/// <summary>
		/// Stores the destination module of the command
		/// </summary>
		protected string destinationModule;
		/// <summary>
		/// Stores the command name
		/// </summary>
		protected string command;
		/// <summary>
		/// Stores the command paramenters
		/// </summary>
		protected string parameters;
		/// <summary>
		/// Stores the command id
		/// </summary>
		protected int id;

		#endregion

		#region Static Variables

		/// <summary>
		/// Stores the Regular Expression used to match the known system control commands
		/// </summary>
		protected static readonly Regex rxIsSystemCommandName = new Regex("alive|bin|busy|ready", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		protected BaseMessage(string command, string parameters)
			: this(null, null, null, null, command, parameters, -1) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		protected BaseMessage(string command, string parameters, int id)
			: this(null, null, command, parameters, id) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		protected BaseMessage(IMessageSource messageSource, string command, string parameters, int id)
			: this(messageSource, null, null, null, command, parameters, id){ }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="messageSourceMetadata">Aditional data provided by the source of the message, like an IPEndPoint or a Delegate</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		protected BaseMessage(IMessageSource messageSource, object messageSourceMetadata, string command, string parameters, int id)
			: this(messageSource, messageSourceMetadata, null, null, command, parameters, id) { }

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="source">The source module of the command</param>
		/// <param name="destination">The destination module of the command</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		protected BaseMessage(string source, string destination, string command, string parameters, int id)
		:this(null, null, source, destination, command, parameters, id){}

		/// <summary>
		/// Initializes a new instance of Command
		/// </summary>
		/// <param name="messageSource">The Source of the message, like a ConnectionManager or a Form capable of manage responses</param>
		/// <param name="source">The source module of the command</param>
		/// <param name="destination">The destination module of the command</param>
		/// <param name="command">The command name</param>
		/// <param name="parameters">The command parameters</param>
		/// <param name="id">The command id</param>
		protected BaseMessage(IMessageSource messageSource, string source, string destination, string command, string parameters, int id)
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
		protected BaseMessage(IMessageSource messageSource, object messageSourceMetadata, string source, string destination, string command, string parameters, int id)
		{
			this.messageSource = messageSource;
			this.messageSourceMetadata = messageSourceMetadata;
			this.SourceModule = source;
			this.DestinationModule = destination;
			this.CommandName = command;
			this.Parameters = parameters;
			this.Id = id;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the parameters string with quotes escaped
		/// </summary>
		public virtual string EscapedParameters
		{
			get
			{
				if (!HasParams)
					return String.Empty;

				char c;
				StringBuilder sb = new StringBuilder(this.parameters.Length * 2);
				for (int i = 0; i < this.parameters.Length; ++i)
				{
					c = this.parameters[i];
					if (c == '\\')
					{
						sb.Append(c);
						++i;
						if (i >= this.parameters.Length)
						{
							sb.Append('\\');
							break;
						}
						sb.Append(this.parameters[i]);
						continue;
					}
					else if (c == '"')
						sb.Append("\\\"");
					else
						sb.Append(c);
				}
				return sb.ToString();
			}
			
		}

		/// <summary>
		/// Gets the object source of the message, like a ConnectionManager or a Form
		/// </summary>
		public IMessageSource MessageSource
		{
			get { return messageSource; }
			internal set { messageSource = value; }
		}

		/// <summary>
		/// Gets or sets the aditional data provided by the source of the message, like an IPEndPoint or a Delegate
		/// </summary>
		public object MessageSourceMetadata
		{
			get { return messageSourceMetadata; }
			set { messageSourceMetadata = value; }
		}

		/// <summary>
		/// Gets or Sets the source module of the command
		/// </summary>
		public virtual string SourceModule
		{
			get { return sourceModule; }
			internal set
			{
				if ((value != null) && (value.Length > 0) && (value.Length < 3))
					throw new ArgumentException("SourceModule name must be at least 3 chars long");
				sourceModule = value;
			}
		}

		/// <summary>
		/// Gets or Sets the destination module of the command
		/// </summary>
		public virtual string DestinationModule
		{
			get { return destinationModule; }
			internal set
			{
				if((value != null) && (value.Length > 0) && (value.Length < 3))
					throw new ArgumentException("DestinationModule name must be at least 3 chars long");
				destinationModule = value;
			}
		}

		/// <summary>
		/// Gets or Sets the command name
		/// </summary>
		public virtual string CommandName
		{
			get { return command; }
			protected set
			{
				if(value == null) throw new ArgumentNullException();
				value = value.Trim();
				if (value.Length < 1) throw new ArgumentException("Command name must be at least 1 chars long");
				command = value;
			}
		}

		/// <summary>
		/// Gets or Sets the command paramenters
		/// </summary>
		public virtual string Parameters
		{
			get { return parameters; }
			set
			{
				parameters = value.Trim();
			}
		}

		/// <summary>
		/// Gets or Sets the command id
		/// </summary>
		public virtual int Id
		{
			get { return id; }
			internal set { id = value; }
		}

		/// <summary>
		/// Gets a value indicating id the Command contains params
		/// </summary>
		public virtual bool HasParams
		{
			get { return ((parameters != null) && (parameters.Length > 0)); }
		}

		/// <summary>
		/// Gets a string which can be sent to a module
		/// </summary>
		public virtual string StringToSend
		{
			get
			{
				StringBuilder sb = new StringBuilder(255);
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
			if((sourceModule != null) && (sourceModule.Length >0))
				sb.Append(' ');

			if ((destinationModule != null) && (destinationModule == null)) sb.Append("Unknown");
			else sb.Append(destinationModule);

			sb.Append(' ');

			sb.Append(command);
			if (HasParams)
			{
				sb.Append(" \"");
				sb.Append(parameters);
				sb.Append("\"");
			}

			if (id >= 0)
			{
				sb.Append(" @");
				sb.Append(id);
			}

			return sb.ToString();
		}

		#region IComparable<BaseMessage> Members

		/// <summary>
		/// Compares this BaseMessage object with other BaseMessage object. The comparison is made comparing the command names
		/// </summary>
		/// <param name="other">BaseMessage object to compare with</param>
		/// <returns>true if the name of the commands is the same, false otherwise</returns>
		public int CompareTo(BaseMessage other)
		{
			if (other == null)
				return Int32.MinValue;
			return this.StringToSend.CompareTo(other.StringToSend);
		}

		#endregion

		#region IComparable Members

		/// <summary>
		/// Compares this BaseMessage object with other object. The comparison is made comparing the command names
		/// </summary>
		/// <param name="obj">BaseMessage object to compare with</param>
		/// <returns>true if the name of the commands is the same, false otherwise</returns>
		public int CompareTo(object obj)
		{
			BaseMessage otherMesaje = obj as BaseMessage;
			if (obj == null)
				return Int32.MinValue;
			return this.StringToSend.CompareTo(otherMesaje.StringToSend);
		}

		#endregion

		#endregion

		#region Static Methods

		/// <summary>
		/// Compares two messages based on its command name
		/// </summary>
		/// <param name="m1">BaseMessage object to compare</param>
		/// <param name="m2">BaseMessage object to compare</param>
		/// <returns></returns>
		public static int CompareByName(BaseMessage m1, BaseMessage m2)
		{
			if ((m1 == null) && (m2 == null))
				return 0;
			if (m2 == null)
				return 1;
			if (m1 == null)
				return -1;
			return String.Compare(m1.command, m2.command);
		}

		#endregion
	}
}