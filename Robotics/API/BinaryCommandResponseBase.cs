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
	 * 16		md5			The MD5 signature of the command
	 * 1		uint		Length of the name of the command
	 * 1:255	string		The name of the command
	 * 2		uint		Maximum execution time allowed
	 * 1		Flags		Command execution options
	 * 4		uint		Length of the parameters subchunk
	 * n		byte[]		Parameters subchunk
	 * 
	*/

	/* 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Structure of Parameters subchunk
	 * 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Description:
	 * 
	 * Size		Value		Description
	 * 1		uint		The number of parameters in the subchunk
	 * 1		uint		The type of the first parameter (if any)
	 * n1		byte[]		The data of the first parameter (if any)
	 * 1		uint		The type of the second parameter (if any)
	 * n2		byte[]		The data of the second parameter (if any)
	 * ...
	 * 1		uint		The type of the n-th parameter (if any)
	 * nn		byte[]		The data of the n-th parameter (if any)
	 * 
	*/
	#endregion

	/// <summary>
	/// Provides base implementation for binary command and response
	/// </summary>
	public abstract class BinaryCommandResponseBase : BinaryMessage
	{
		#region Variables

		/// <summary>
		/// The name of the command
		/// </summary>
		public string commandName;

		/// <summary>
		/// The name of the source module in ASCII
		/// </summary>
		protected string sourceModuleName;

		/// <summary>
		/// Stores the list of parameters of the command or response
		/// </summary>
		protected readonly ParameterList parameters;

		/// <summary>
		/// The name of the destination module in ASCII
		/// </summary>
		protected string destinationModuleName;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of BinaryCommandResponseBase
		/// </summary>
		/// <remarks>This empty constructor is provided for serialization purposes</remarks>
		protected BinaryCommandResponseBase()
		{
			this.parameters = new ParameterList();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the command
		/// </summary>
		public string CommandName
		{
			get { return this.commandName; }
			protected set
			{
				this.commandName = value;
			}
		}

		/// <summary>
		/// Gets the name of the destination module in ASCII
		/// </summary>
		public override string DestinationModuleName
		{
			get { return this.destinationModuleName; }
		}

		/// <summary>
		/// Gets the list of parameters of the command or response
		/// </summary>
		public ParameterList Parameters
		{
			get { return this.parameters; }
		}

		/// <summary>
		/// Gets a value indicating if the Command contains parameters
		/// </summary>
		public virtual bool HasParams
		{
			get
			{
				//return ((parameters != null) && (parameters.Length > 0));
				return false;
			}
		}

		/// <summary>
		/// Gets a byte array which can be sent to a module
		/// </summary>
		public virtual byte[] SerializedDataToSend
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the name of the source module in ASCII
		/// </summary>
		public override string SourceModuleName
		{
			get { return null; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Attempts to serialize the command name of the message into the provided stream.
		/// </summary>
		/// <param name="stream">The stream where to serialize the data</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the command name was successfully serialized to the stream, false otherwise</returns>
		protected bool TrySerializeCommandName(Stream stream, out Exception ex)
		{
			ex = null;
			if (stream == null)
			{
				ex = new ArgumentNullException("Null stream provided");
				return false;
			}
			SerializeToStream(stream, this.CommandName);
			return true;
		}

		/// <summary>
		/// Attempts to serialize the parameters
		/// </summary>
		/// <param name="stream">The stream where to serialize the data</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the parameters were successfully serialized to the stream, false otherwise</returns>
		protected bool TrySerializeParameters(Stream stream, out Exception ex)
		{
			byte[] buffer;
			uint parametersLength;
			long parametersLengthPosition;
			long currentStreamPosition;

			ex = null;
			if (stream == null)
			{
				ex = new ArgumentNullException("Null stream provided");
				return false;
			}

			// Reserve space for the tength of the parameters subchunk
			parametersLengthPosition = stream.Position;
			buffer = BitConverter.GetBytes((uint)0);
			stream.Write(buffer, 0, buffer.Length);

			// Serialize the parameters
			if(!this.Parameters.TrySerialize(stream, out ex))
				return false;

			// Serialize the parameters length
			currentStreamPosition = stream.Position;
			parametersLength = (uint)(currentStreamPosition - parametersLengthPosition + 4);
			buffer = BitConverter.GetBytes(parametersLength);
			stream.Position=parametersLengthPosition;
			stream.Write(buffer, 0, buffer.Length);
			stream.Position = currentStreamPosition;

			return true;
		}

		#endregion

		#region Static Methods
		#endregion
	}
}
