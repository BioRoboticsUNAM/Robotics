using System;
using System.IO;
using System.Text;

namespace Robotics.API
{
	#region Specification
	/* 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Structure of Binary Response
	 * 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Message type: 0x03
	 * Description:
	 * 
	 * Size		Value		Description
	 * 16		md5			The MD5 signature of the executed command
	 * 1		uint		Length of the name of the executed command
	 * 1:255	string		The name of the executed command
	 * 2		uint		Execution time
	 * 1		Flags		Execution result
	 * 4		uint		Length of the parameters subchunk
	 * n		byte[]		Parameters subchunk
	 * 1		uint		Length of the custom error message 
	 * 0:255	string		A custom error message in ASCII 
	 * 
	*/
	#endregion

	/// <summary>
	/// Accomplishment status of a command execution
	/// </summary>
	public enum CommandExecutionResult : byte
	{
		/// <summary>
		/// The command was executed successfully
		/// </summary>
		Success = 0x00
	}
	
	/// <summary>
	/// 
	/// </summary>
	public class BinaryResponse : BinaryCommandResponseBase
	{
		#region Variables

		/// <summary>
		/// The maximum execution time of the command
		/// </summary>
		private int executionTime;

		/// <summary>
		/// The unique ID of the asociated command
		/// </summary>
		private ulong commandUID;

		/// <summary>
		/// A custom error message
		/// </summary>
		private string customErrorMessage;

		/// <summary>
		/// The accomplishment status of a command execution
		/// </summary>
		private CommandExecutionResult executionResult;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of BinaryResponse
		/// </summary>
		/// <remarks>This empty constructor is provided for serialization purposes</remarks>
		protected BinaryResponse()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a custom error message
		/// </summary>
		public string CustomErrorMessage
		{
			get { return this.customErrorMessage; }
			set
			{
				if ((value != null )&&(value.Length > 255))
					throw new ArgumentNullException("Maximum length is up to 255 characters");
				this.customErrorMessage = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum execution time of the command
		/// </summary>
		public int ExecutionTime
		{
			get { return this.executionTime; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("Value must be greater than zero");
				this.executionTime = value;
			}
		}

		/// <summary>
		/// Gets or sets the accomplishment status of a command execution
		/// </summary>
		public CommandExecutionResult ExecutionResult
		{
			get { return this.executionResult; }
			set { this.executionResult = value; }
		}

		/// <summary>
		/// Gets or sets the unique ID of the asociated command
		/// </summary>
		public ulong CommandUID
		{
			get { return this.commandUID; }
			set { this.commandUID = value; }
		}

		/// <summary>
		/// Gets the type of the BinaryMessage
		/// </summary>
		public override BinaryMessageType MessageType
		{
			get { return BinaryMessageType.BinaryResponse; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// When overriden in a derved class, it attempts to serialize a the message data, excluding the base information, into the provided stream
		/// </summary>
		/// <param name="stream">The stream where to serialize the data</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the binary message was successfully serialized to the stream, false otherwise</returns>
		protected override bool TrySerializeData(Stream stream, out Exception ex)
		{
			// Serialize the MD5 signature
			if (!TrySerializeCommandUID(stream, out ex))
				return false;
			// Serialize the command name string size and command name string
			if (!TrySerializeCommandName(stream, out ex))
				return false;
			// Serialize the execution time
			SerializeToStream(stream, this.ExecutionTime);

			// Serialize the command execution options
			stream.WriteByte((byte)this.ExecutionResult);

			// Serialize the command parameters
			if(!TrySerializeParameters(stream, out ex))
				return false;

			// Serialize the custom error
			SerializeToStream(stream, CustomErrorMessage);

			return true;
		}

		/// <summary>
		/// Attempts to serialize the MD5 signature of the message into the provided stream.
		/// </summary>
		/// <param name="stream">The stream where to serialize the data</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the MD5 signature was successfully serialized to the stream, false otherwise</returns>
		protected bool TrySerializeCommandUID(Stream stream, out Exception ex)
		{
			byte[] buffer;

			ex = null;
			if (stream == null)
			{
				ex = new ArgumentNullException("Null stream provided");
				return false;
			}

			buffer = BitConverter.GetBytes(this.CommandUID);
			stream.Write(buffer, 0, buffer.Length);
			return true;
		}

		#endregion

		#region Static Methods
		#endregion
	}
}
