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
	 * Structure of Binary Message
	 * 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Size		Value		Description
	 * 2		0x00E7		Start of packet
	 * 4		uint		Packet size (excluding header from the message type to the end of the packet, minimum value: )
	 * 1		Flags		Message type
	 * 4		uint		The time stamp when the message was created
	 * 1		uint		Length of the name of the source module (Use zero to allow blackboard determine it automatically)
	 * 0:255	string		The name of the source module in ASCII
	 * 1		uint		Length of the name of the destination module (Use zero to allow blackboard determine it automatically)
	 * 0:255	string		The name of the source destination in ASCII
	 * n		byte[]		The message data
	 * 4		uint		Checksum
	 * 2		0x7E00		End of packet
	 * 
	*/
	/* 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Structure of Binary Command Execution Report
	 * 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Message type: 0x02
	 * Description:
	 * 
	 * Size		Value		Description
	 * 16		md5			The MD5 signature of the command being executed
	 * 1		uint		Length of the name of the command being executed
	 * 1:255	string		The name of the command being executed
	 * 2		uint		Elapsed execution time
	 * 1		Flags		Command execution status
	 * 1		uint		Excecution progress
	 * 1		uint		Length of the custom message 
	 * 0:255	string		A custom message in ASCII
	 * 
	 * 
	 * 
	 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
	 * 
	 * Structure of Binary 
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

	#endregion
	/// <summary>
	/// Enumerates the types of binary messages
	/// </summary>
	public enum BinaryMessageType : byte
	{
		/// <summary>
		/// The binary message is a command
		/// </summary>
		BinaryCommand = 0x01,
		/// <summary>
		/// The binary message is a command excecution report
		/// </summary>
		BinaryCommandExecutionReport = 0x02,
		/// <summary>
		/// The binary message is a response
		/// </summary>
		BinaryResponse = 0x03

	}

	/// <summary>
	/// Implements a Blackboard message using binary format
	/// </summary>
	public abstract class BinaryMessage
	{
		#region Constants

		/// <summary>
		/// The value of the start of a Binary Message packet
		/// </summary>
		public const ushort PacketStart = 0x00E7;

		/// <summary>
		/// The value of the end of a Binary Message packet
		/// </summary>
		public const ushort PacketEnd = 0x7E00;

		#endregion

		#region Variables

		/// <summary>
		/// The time stamp when the message was created
		/// </summary>
		protected uint timeStamp;

		#endregion

		#region Constructors
		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the destination module in ASCII
		/// </summary>
		public abstract string DestinationModuleName { get; }

		/// <summary>
		/// Gets the type of the BinaryMessage
		/// </summary>
		public abstract BinaryMessageType MessageType { get; }

		/// <summary>
		/// Gets the name of the source module in ASCII
		/// </summary>
		public abstract string SourceModuleName { get; }

		/// <summary>
		/// Gets the time stamp when the message was created
		/// </summary>
		public uint TimeStamp
		{
			get { return this.timeStamp; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// When overriden in a derved class, it attempts to serialize a the message data, excluding the base information, into the provided stream
		/// </summary>
		/// <param name="stream">The stream where to serialize the data</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the binary message was successfully serialized to the stream, false otherwise</returns>
		protected abstract bool TrySerializeData(Stream stream, out Exception ex);

		#endregion

		#region Static Methods

		/// <summary>
		/// Calculates the checksum of a packet data.
		/// It is the two-complement of the sum of all bytes from the header to the checksum
		/// </summary>
		/// <param name="stream">The stream which contains the data to be read</param>
		/// <param name="packetStartPosition">The position at which the packet starts</param>
		/// <returns>The checksum of the packet</returns>
		private static uint CalculateChecksum(Stream stream, long packetStartPosition)
		{
			long packetChecksumPosition;
			uint sum;

			packetChecksumPosition = stream.Position;
			stream.Position = packetStartPosition;

			unchecked
			{
				sum = 0;
				while (stream.Position < packetChecksumPosition)
					sum += (uint)stream.ReadByte();
				sum = ~sum;
				++sum;
			}
			return sum;
		}

		/// <summary>
		/// Tries to read the Packet Header from the provided string
		/// </summary>
		/// <param name="stream">The stream which contains the data to be read</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the read failed, or null if the read succeeded</param>
		/// <returns>true if the Packet Head was read from the stream, false otherwise</returns>
		protected static bool ReadHeader(Stream stream, out Exception ex)
		{
			ushort header;
			int b;
			ex = null;

			if ((b = stream.ReadByte()) == -1)
			{
				ex = new IOException("Unexpected end of stream");
				return false;
			}
			header = (ushort)(b << 8);
			if ((b = stream.ReadByte()) == -1)
			{
				ex = new IOException("Unexpected end of stream");
				return false;
			}
			header |= (ushort)b;
			if (header != BinaryMessage.PacketStart)
			{
				ex = new Exception("Invalid header");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Tries to read the Packet Header from the provided string
		/// </summary>
		/// <param name="stream">The stream which contains the data to be read</param>
		/// <param name="packetSize">When this method returns contains the size of the serialized packet if the read succeeded, or zero if the deserialization failed</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the read failed, or null if the read succeeded</param>
		/// <returns>true if the Packet Size was read from the stream, false otherwise</returns>
		protected static bool ReadPacketSize(Stream stream, out uint packetSize, out Exception ex)
		{
			int b;
			byte[] sizeBytes = new byte[4];

			ex = null;
			packetSize = 0;
			for (int i = 0; i < sizeBytes.Length; ++i)
			{
				if ((b = stream.ReadByte()) == -1)
				{
					ex = new IOException("Unexpected end of stream");
					return false;
				}
				sizeBytes[i] = (byte)b;
			}
			packetSize = BitConverter.ToUInt32(sizeBytes, 0);
			return true;
		}

		/// <summary>
		/// Writes the destination module sub-chunk in the serialization stream
		/// </summary>
		/// <param name="stream">The stream where the data will be written</param>
		/// <param name="message">The message from where the destination module will be read</param>
		private static void SerializeDestinationModule(Stream stream, BinaryMessage message)
		{
			SerializeToStream(stream, message.DestinationModuleName);
		}

		/// <summary>
		/// Writes the packet size sub-chunk in the serialization stream
		/// </summary>
		/// <param name="stream">The stream where the data will be written</param>
		/// <param name="message">The message from where the source module will be read</param>
		/// <param name="packetSizePosition">The position in the stream where to write the packet size</param>
		/// <param name="packetStartPosition">The position in the stream where the packet starts</param>
		private static void SerializePacketSize(Stream stream, BinaryMessage message, long packetStartPosition, long packetSizePosition)
		{
			byte[] buffer;
			long packetChecksumPosition;
			uint packetSize;

			packetSize = (uint)(stream.Position - packetStartPosition);
			buffer = BitConverter.GetBytes((uint)packetSize);
			packetChecksumPosition = stream.Position;
			stream.Position = packetSizePosition;
			stream.Write(buffer, 0, buffer.Length);
			stream.Position = packetChecksumPosition;
		}

		/// <summary>
		/// Writes the source module sub-chunk in the serialization stream
		/// </summary>
		/// <param name="stream">The stream where the data will be written</param>
		/// <param name="message">The message from where the source module will be read</param>
		private static void SerializeSourceModule(Stream stream, BinaryMessage message)
		{
			SerializeToStream(stream, message.SourceModuleName);
		}

		/// <summary>
		/// Attempts to deserialize a binary message from the provided stream
		/// </summary>
		/// <param name="data">The byte array which contains the the binary message data to be deserialized</param>
		/// <param name="message">When this method returns contains the binary message contained in the byte array if the serialization succeeded, or null if the serialization failed</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the binary message was successfully deserialized from the byte array, false otherwise</returns>
		public static bool TryDeserialize(byte[] data, out BinaryMessage message, out Exception ex)
		{
			using (MemoryStream ms = new MemoryStream(data))
			{
				return TryDeserialize(ms, out message, out ex);
			}
		}

		/// <summary>
		/// Attempts to deserialize a binary message from the provided stream
		/// </summary>
		/// <param name="stream">The stream which contains the the binary message data to be deserialized</param>
		/// <param name="message">When this method returns contains the binary message contained in the stream if the serialization succeeded, or null if the serialization failed</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the binary message was successfully deserialized from the stream, false otherwise</returns>
		public static bool TryDeserialize(Stream stream, out BinaryMessage message, out Exception ex)
		{
			uint packetSize;

			message = null;
			ex = null;
			if (!ReadHeader(stream, out ex))
				return false;
			if(!ReadPacketSize(stream, out packetSize, out ex))
				return false;

			return true;
		}

		/// <summary>
		/// Attempts to serialize a binary message into the provided stream
		/// </summary>
		/// <param name="stream">The stream where to serialize the binary message</param>
		/// <param name="message">The binary message to serialize</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the binary message was successfully serialized to the stream, false otherwise</returns>
		public static bool TrySerialize(Stream stream, BinaryMessage message, out Exception ex)
		{
			byte[] buffer;
			long packetStartPosition;
			long packetSizePosition;

			// uint checksum;

			ex = null;

			if (stream == null)
			{
				ex = new ArgumentNullException("The stream can not be null");
				return false;
			}

			if (message == null)
			{
				ex = new ArgumentNullException("The message can not be null");
				return false;
			}

			// Write header
			packetStartPosition = stream.Position;
			buffer = BitConverter.GetBytes((ushort)BinaryMessage.PacketStart);
			stream.Write(buffer, 0, buffer.Length);

			// Reserve space for packet length
			packetSizePosition = stream.Position;
			buffer = BitConverter.GetBytes((uint)0x00000000);
			stream.Write(buffer, 0, buffer.Length);

			// Write message type
			stream.WriteByte((byte)message.MessageType);

			// Write the Timestamp
			buffer = BitConverter.GetBytes((uint)0x00000000);
			stream.Write(buffer, 0, buffer.Length);

			// Write the source module name
			SerializeSourceModule(stream, message);

			// Write the destination module name
			SerializeDestinationModule(stream, message);

			// Write the message data
			if (!message.TrySerializeData(stream, out ex))
				return false;

			// Write the packet size (it is necesary for calculating the checksum)
			SerializePacketSize(stream, message, packetStartPosition, packetSizePosition);

			// Calculate and write checksum
			// checksum = CalculateChecksum(stream, packetStartPosition);
			buffer = BitConverter.GetBytes((uint)0x00000000);
			stream.Write(buffer, 0, buffer.Length);

			stream.Flush();
			return true;
		}

		/// <summary>
		/// Serializes a string of up to 256 characters into the provided stream.
		/// This method first writes a byte with the size of the string and then writes each character in the string coded as ASCII.
		/// If the provided string is null or it is empty only the size byte is written with a value of zero.
		/// </summary>
		/// <param name="stream">The stream where the string data will be serialized</param>
		/// <param name="s">The string object to serialize</param>
		protected static void SerializeToStream(Stream stream, string s)
		{
			byte[] buffer;

			if (String.IsNullOrEmpty(s) || (s.Length > 255))
			{
				stream.WriteByte((byte)0);
				return;
			}

			stream.WriteByte((byte)s.Length);
			buffer = ASCIIEncoding.ASCII.GetBytes(s);
			stream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Serializes an 64-bit unsigned integer into a stream.
		/// </summary>
		/// <param name="stream">The stream where the string data will be serialized</param>
		/// <param name="value">The 32-bit unsigned integer to serialize</param>
		protected static void SerializeToStream(Stream stream, ulong value)
		{
			byte[] buffer;
			buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Serializes an 64-bit signed integer into a stream.
		/// </summary>
		/// <param name="stream">The stream where the string data will be serialized</param>
		/// <param name="value">The 32-bit unsigned integer to serialize</param>
		protected static void SerializeToStream(Stream stream, long value)
		{
			byte[] buffer;
			buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Serializes an 32-bit unsigned integer into a stream.
		/// </summary>
		/// <param name="stream">The stream where the string data will be serialized</param>
		/// <param name="value">The 32-bit unsigned integer to serialize</param>
		protected static void SerializeToStream(Stream stream, uint value)
		{
			byte[] buffer;
			buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Serializes an 32-bit signed integer into a stream.
		/// </summary>
		/// <param name="stream">The stream where the string data will be serialized</param>
		/// <param name="value">The 32-bit unsigned integer to serialize</param>
		protected static void SerializeToStream(Stream stream, int value)
		{
			byte[] buffer;
			buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Serializes an 16-bit unsigned integer into a stream.
		/// </summary>
		/// <param name="stream">The stream where the string data will be serialized</param>
		/// <param name="value">The 32-bit unsigned integer to serialize</param>
		protected static void SerializeToStream(Stream stream, ushort value)
		{
			byte[] buffer;
			buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Serializes an 16-bit signed integer into a stream.
		/// </summary>
		/// <param name="stream">The stream where the string data will be serialized</param>
		/// <param name="value">The 32-bit unsigned integer to serialize</param>
		protected static void SerializeToStream(Stream stream, short value)
		{
			byte[] buffer;
			buffer = BitConverter.GetBytes(value);
			stream.Write(buffer, 0, buffer.Length);
		}

		#endregion
	}
}
