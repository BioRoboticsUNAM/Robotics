using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Robotics.API
{
	#region Specification
	/* 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Structure of Binary Command, inherits Binary Message
	 * 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Message type: 0x01
	 * Description:
	 * 
	 * Size		Value		Description
	 * 8		ulong		A command unique ID
	 * 1		uint		Length of the name of the command
	 * 1:255	string		The name of the command
	 * 2		uint		Maximum execution time allowed
	 * 1		Flags		Command execution options
	 * 4		uint		Length of the parameters subchunk
	 * n		byte[]		Parameters subchunk
	 * 
	*/
	#endregion

	/// <summary>
	/// Enumerates the execution options for a binary command
	/// </summary>
	[Flags]
	public enum BinaryCommandExecutionOptions : byte
	{
		/// <summary>
		/// No options are used
		/// </summary>
		None = 0x00
	}

	/// <summary>
	/// 
	/// </summary>
	public class BinaryCommand : BinaryCommandResponseBase
	{
		#region Variables

		/// <summary>
		/// Gets or sets the maximum execution time allowed
		/// </summary>
		private int maxExecutionTime;

		/// <summary>
		/// Execution options for this command
		/// </summary>
		private BinaryCommandExecutionOptions executionOptions;

		/// <summary>
		/// The unique ID of the command
		/// </summary>
		protected ulong uniqueID;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of BinaryCommand
		/// </summary>
		/// <remarks>This empty constructor is provided for serialization purposes</remarks>
		protected BinaryCommand()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the maximum execution time allowed
		/// </summary>
		public virtual int MaxExecutionTime
		{
			get { return this.maxExecutionTime; }
			set
			{
				if (value < 0)
					throw new ArgumentOutOfRangeException("Value must be greater or equal than zero");
				this.maxExecutionTime = value;
			}
		}

		/// <summary>
		/// Gets or sets the execution options for this command
		/// </summary>
		public BinaryCommandExecutionOptions ExecutionOptions
		{
			get { return this.executionOptions; }
			set { this.executionOptions = value; }
		}

		/// <summary>
		/// Gets the type of the BinaryMessage
		/// </summary>
		public override BinaryMessageType MessageType
		{
			get { return BinaryMessageType.BinaryCommand; }
		}

		/// <summary>
		/// Gets or sets the unique ID of the command
		/// </summary>
		public ulong UniqueID
		{
			get { return this.uniqueID; }
			set { this.uniqueID = value; }
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
			if(!TrySerializeUID(stream, out ex))
				return false;
			// Serialize the command name string size and command name string
			if (!TrySerializeCommandName(stream, out ex))
				return false;
			// Serialize the maximum execution time
			SerializeToStream(stream, this.MaxExecutionTime);

			// Serialize the command execution options
			stream.WriteByte((byte)this.ExecutionOptions);

			// Serialize the command parameters
			if (!TrySerializeParameters(stream, out ex))
				return false;



			return true;
		}

		/// <summary>
		/// Attempts to serialize the MD5 signature of the message into the provided stream.
		/// </summary>
		/// <param name="stream">The stream where to serialize the data</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the MD5 signature was successfully serialized to the stream, false otherwise</returns>
		protected bool TrySerializeUID(Stream stream, out Exception ex)
		{
			byte[] buffer;

			ex = null;
			if (stream == null)
			{
				ex = new ArgumentNullException("Null stream provided");
				return false;
			}

			buffer = BitConverter.GetBytes(this.UniqueID);
			stream.Write(buffer, 0, buffer.Length);
			return true;
		}

		#endregion

		#region Static Methods
		#endregion
	}
}
