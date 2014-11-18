using System;
using System.Collections.Generic;
using System.Text;
using Robotics.Utilities;

namespace Robotics.API.PrimitiveSharedVariables
{
	/// <summary>
	/// Provudes serialization for basic types
	/// </summary>
	public class PrimitiveSerializer
	{
		/// <summary>
		/// Chars used to split strings
		/// </summary>
		private static char[] SplitChars = { ' ', '\t', '\r', '\n' };

		#region Deserialization: Primitive data types

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out bool value)
		{
			string tsd = serializedData.Trim();
			if (Boolean.TryParse(tsd, out value)) return true;
			int iValue;
			if (Int32.TryParse(tsd, out iValue))
			{
				value = iValue != 0;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out byte value)
		{
			return Byte.TryParse(serializedData, out value);
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out short value)
		{
			return Int16.TryParse(serializedData, out value);
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out int value)
		{
			return Int32.TryParse(serializedData, out value);
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out long value)
		{
			return Int64.TryParse(serializedData, out value);
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out float value)
		{
			return Single.TryParse(serializedData, out value);
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or zero if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out double value)
		{
			return Double.TryParse(serializedData, out value);
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed.</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out string value)
		{
			return DeserializeString(serializedData, out value);
		}

		/// <summary>
		/// Deserializes the provided object from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed.</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out MimeFile value)
		{
			value = null;
			if (String.IsNullOrEmpty(serializedData)) return true;

			int bcc;
			int cc = 0;
			string base64;
			string mimeType;

			if (!Scanner.ReadString("MimeType=", serializedData, ref cc))
				return false;
			bcc = cc;
			if (!Scanner.AdvanceToChar(' ', serializedData, ref cc))
				return false;
			mimeType = serializedData.Substring(bcc, cc - bcc);
			++cc;
			if (!Scanner.ReadString("Data=", serializedData, ref cc))
				return false;
			bcc = cc;
			if (!Scanner.AdvanceToChar('=', serializedData, ref cc))
				return false;
			while (Scanner.ReadChar('=', serializedData, ref cc)) ;
			base64 = serializedData.Substring(bcc, cc - bcc);
			value = new MimeFile(mimeType, Convert.FromBase64String(base64));
			return true;
		}

		/// <summary>
		/// Deserializes the provided string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="value">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed.</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool DeserializeString(string serializedData, out string value)
		{
			int start;
			int end;

			if (String.IsNullOrEmpty(serializedData) || (String.Compare("null", serializedData, true) == 0))
			{
				value = null;
				return true;
			}

			value = null;
			start = serializedData.IndexOf("\"");
			end = serializedData.LastIndexOf("\"") - 1;
			if ((start < 0) || (end < 1) || (end < start))
				return false;
			value = serializedData.Substring(start + 1, end - start);
			value = value.Replace("\\\"", "\"");
			value = value.Replace(@"\\", @"\");
			return true;
		}

		#endregion

		#region Deserialization: Primitive array data types

		/// <summary>
		/// Deserializes an array of bolean values from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out bool[] values)
		{
			string[] parts;
			bool data;
			List<bool> iData = new List<bool>();

			values = null;
			if (String.IsNullOrEmpty(serializedData)) return true;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Deserialize(parts[i], out data))
					return false;
				iData.Add(data);
			}
			values = iData.ToArray();
			return true;
		}

		/// <summary>
		/// Deserializes an array of byte values from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out byte[] values)
		{
			string[] parts;
			byte data;
			List<byte> iData = new List<byte>();

			values = null;
			if (String.IsNullOrEmpty(serializedData)) return true;
			if (serializedData.StartsWith("0x") || serializedData.StartsWith("0X"))
				return DeserializeHexByteArray(serializedData, out values);
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Byte.TryParse(parts[i], out data))
					return false;
				iData.Add(data);
			}
			values = iData.ToArray();
			return true;
		}

		/// <summary>
		/// Deserializes an array of short values from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out short[] values)
		{
			string[] parts;
			short data;
			List<short> iData = new List<short>();

			values = null;
			if (String.IsNullOrEmpty(serializedData)) return true;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Int16.TryParse(parts[i], out data))
					return false;
				iData.Add(data);
			}
			values = iData.ToArray();
			return true;
		}

		/// <summary>
		/// Deserializes an array of int values from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out int[] values)
		{
			string[] parts;
			int data;
			List<int> iData = new List<int>();

			values = null;
			if (String.IsNullOrEmpty(serializedData)) return true;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Int32.TryParse(parts[i], out data))
					return false;
				iData.Add(data);
			}
			values = iData.ToArray();
			return true;
		}

		/// <summary>
		/// Deserializes an array of long values from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out long[] values)
		{
			string[] parts;
			long data;
			List<long> iData = new List<long>();

			values = null;
			if (String.IsNullOrEmpty(serializedData)) return true;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Int64.TryParse(parts[i], out data))
					return false;
				iData.Add(data);
			}
			values = iData.ToArray();
			return true;
		}

		/// <summary>
		/// Deserializes an array of float values from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out float[] values)
		{
			string[] parts;
			float data;
			List<float> iData = new List<float>();

			values = null;
			if (String.IsNullOrEmpty(serializedData)) return true;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Single.TryParse(parts[i], out data))
					return false;
				iData.Add(data);
			}
			values = iData.ToArray();
			return true;
		}

		/// <summary>
		/// Deserializes an array of double values from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool Deserialize(string serializedData, out double[] values)
		{
			string[] parts;
			double data;
			List<double> iData = new List<double>();

			values = null;
			if (String.IsNullOrEmpty(serializedData)) return true;
			parts = serializedData.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < parts.Length; ++i)
			{
				if (!Double.TryParse(parts[i], out data))
					return false;
				iData.Add(data);
			}
			values = iData.ToArray();
			return true;
		}

		/// <summary>
		/// Deserializes an array of byte values from a string
		/// </summary>
		/// <param name="serializedData">String containing the serialized object</param>
		/// <param name="values">When this method returns contains the value stored in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be parsed. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool DeserializeHexByteArray(string serializedData, out byte[] values)
		{
			int cc;
			values = null;
			byte data;
			List<byte> bData;

			if (String.IsNullOrEmpty(serializedData)) return true;
			// Eat white spaces
			cc = 0;
			while ((cc < serializedData.Length) && Char.IsWhiteSpace(serializedData[cc]))
				++cc;

			// No data. Return.
			if ((serializedData.Length - cc) < 2)
				return false;

			// Skip prefix 0x and 0X
			if ((serializedData[cc] == '0') && ((serializedData[cc + 1] == 'X') || (serializedData[cc + 1] == 'x')))
				cc += 2;

			// Parse data
			// 0x 48 60 28 68 64 57 63 2a
			bData = new List<byte>(serializedData.Length / 2);
			while (cc < serializedData.Length - 1)
			{
				if (!Byte.TryParse(
					serializedData.Substring(cc, 2),
					System.Globalization.NumberStyles.HexNumber,
					System.Globalization.CultureInfo.InvariantCulture,
					out data))
					return false;
				bData.Add(data);
				cc += 2;
			}
			values = bData.ToArray();
			return true;
		}

		#endregion

		#region Serialization: Primitive data types

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(bool value, out string serializedData)
		{
			serializedData = value ? "true" : "false";
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(bool value, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");
			sb.Append(value ? "true" : "false");
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(byte value, out string serializedData)
		{
			serializedData = ((int)value).ToString();
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(byte value, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");
			sb.Append((int)value);
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(short value, out string serializedData)
		{
			serializedData = value.ToString();
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(short value, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");
			sb.Append(value);
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(int value, out string serializedData)
		{
			serializedData = value.ToString();
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(int value, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");
			sb.Append(value);
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(long value, out string serializedData)
		{
			serializedData = value.ToString();
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(long value, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");
			sb.Append(value);
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(float value, out string serializedData)
		{
			serializedData = String.Empty;
			if (Single.IsNaN(value) || Single.IsInfinity(value))
				return false;
			serializedData = value.ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(float value, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");
			if (Single.IsNaN(value) || Single.IsInfinity(value))
				return false;
			sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "G8", value);
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or zero if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(double value, out string serializedData)
		{
			serializedData = String.Empty;
			if (Double.IsNaN(value) || Double.IsInfinity(value))
				return false;
			serializedData = value.ToString("G8", System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(double value, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");
			if (Double.IsNaN(value) || Double.IsInfinity(value))
				return false;
			sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture.NumberFormat, "G8", value);
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed.</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(string value, out string serializedData)
		{
			serializedData = SerializeString(value);
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(string value, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");
			sb.Append(SerializeString(value));
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(MimeFile value, out string serializedData)
		{
			serializedData = String.Empty;
			if (value == null)
				return false;

			string base64 = Convert.ToBase64String(value.RawData, Base64FormattingOptions.None);
			serializedData = String.Format("MimeType={0} Data={1}", value.MimeType, base64);
			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="value">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(MimeFile value, StringBuilder sb)
		{
			if (value == null)
				return false;

			string base64 = Convert.ToBase64String(value.RawData, Base64FormattingOptions.None);
			sb.Append("MimeType=");
			sb.Append(value.MimeType);
			sb.Append(" Data=");
			sb.Append(base64);
			return true;
		}

		/// <summary>
		/// Serializes the provided string by escaping all double quotes (if any) and enclosing it by escaped double quotes
		/// </summary>
		/// <param name="value">String to serialize</param>
		/// <returns>Serialized string</returns>
		public static string SerializeString(string value)
		{
			string serializedData;

			if (value == null)
				return "null";

			serializedData = value;
			serializedData = serializedData.Replace(@"\", @"\\");
			serializedData = serializedData.Replace("\"", "\\\"");
			serializedData = "\"" + serializedData.ToString() + "\"";
			return serializedData;
		}

		#endregion

		#region Serialization: Primitive array data types

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(bool[] values, out string serializedData)
		{
			serializedData = String.Empty;
			if ((values == null) || (values.Length < 1))
				return true;

			StringBuilder sb = new StringBuilder(values.Length * 6);
			sb.Append(values[0].ToString());
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i] ? "true" : "false");
			}
			serializedData = sb.ToString();

			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(bool[] values, StringBuilder sb)
		{
			if ((values == null) || (values.Length < 1))
				return true;

			sb.Append(values[0] ? "true" : "false");
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i] ? "true" : "false");
			}
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(byte[] values, out string serializedData)
		{
			serializedData = String.Empty;
			if ((values == null) || (values.Length < 1))
				return true;

			StringBuilder sb = new StringBuilder(values.Length * 3);
			sb.Append(values[0].ToString());
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i].ToString());
			}
			serializedData = sb.ToString();

			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(byte[] values, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");

			if ((values == null) || (values.Length < 1))
				return true;

			sb.Append((int)values[0]);
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append((int)values[i]);
			}
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(short[] values, out string serializedData)
		{
			serializedData = String.Empty;
			if ((values == null) || (values.Length < 1))
				return true;

			StringBuilder sb = new StringBuilder(values.Length * 6);
			sb.Append(values[0].ToString());
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i].ToString());
			}
			serializedData = sb.ToString();

			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(short[] values, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");

			if ((values == null) || (values.Length < 1))
				return true;

			sb.Append(values[0]);
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i]);
			}
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(int[] values, out string serializedData)
		{
			serializedData = String.Empty;
			if ((values == null) || (values.Length < 1))
				return true;

			StringBuilder sb = new StringBuilder(values.Length * 11);
			sb.Append(values[0].ToString());
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i].ToString());
			}
			serializedData = sb.ToString();

			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(int[] values, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");

			if ((values == null) || (values.Length < 1))
				return true;

			sb.Append(values[0]);
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i]);
			}

			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(long[] values, out string serializedData)
		{
			serializedData = String.Empty;
			if ((values == null) || (values.Length < 1))
				return true;

			StringBuilder sb = new StringBuilder(values.Length * 21);
			sb.Append(values[0].ToString());
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i].ToString());
			}
			serializedData = sb.ToString();

			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(long[] values, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");

			if ((values == null) || (values.Length < 1))
				return true;

			sb.Append(values[0]);
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				sb.Append(values[i]);
			}
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(float[] values, out string serializedData)
		{
			serializedData = String.Empty;
			if ((values == null) || (values.Length < 1))
				return true;

			string sValue;
			StringBuilder sb = new StringBuilder(values.Length * 21);
			if (!Serialize(values[0], out sValue)) return false;
			sb.Append(sValue);
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				if (!Serialize(values[i], out sValue)) return false;
				sb.Append(sValue);
			}
			serializedData = sb.ToString();

			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(float[] values, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");

			if ((values == null) || (values.Length < 1))
				return true;

			int initialLength = sb.Length;
			if (!Serialize(values[0], sb))
			{
				sb.Length = initialLength;
				return false;
			}
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				if (!Serialize(values[i], sb))
				{
					sb.Length = initialLength;
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(double[] values, out string serializedData)
		{
			serializedData = String.Empty;
			if ((values == null) || (values.Length < 1))
				return true;

			string sValue;
			StringBuilder sb = new StringBuilder(values.Length * 21);
			if (!Serialize(values[0], out sValue)) return false;
			sb.Append(sValue);
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				if (!Serialize(values[i], out sValue)) return false;
				sb.Append(sValue);
			}
			serializedData = sb.ToString();

			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool Serialize(double[] values, StringBuilder sb)
		{
			if (sb == null)
				throw new ArgumentNullException("The StringBuilder object is required to write serialized data");

			if ((values == null) || (values.Length < 1))
				return true;

			int initialLength = sb.Length;
			if (!Serialize(values[0], sb))
			{
				sb.Length = initialLength;
				return false;
			}
			for (int i = 1; i < values.Length; ++i)
			{
				sb.Append(' ');
				if (!Serialize(values[i], sb))
				{
					sb.Length = initialLength;
					return false;
				}
			}
			return true;
		}
		
		/// <summary>
		/// Serializes the provided object to a string
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="serializedData">When this method returns contains value serialized if the serialization succeeded, or null if the serialization failed. The serialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or outside the specification for the type. This parameter is passed uninitialized</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool SerializeHexByteArray(byte[] values, out string serializedData)
		{
			StringBuilder sb;
			if (values == null)
				values = new byte[0];
			sb = new StringBuilder(2 + values.Length * 3);
			sb.Append("0x");
			for (int i = 0; i < values.Length; ++i)
				sb.Append(values[i].ToString("x2"));
			serializedData = sb.ToString();

			return true;
		}

		/// <summary>
		/// Serializes the provided object into a StringBuilder object
		/// </summary>
		/// <param name="values">Object to be serialized</param>
		/// <param name="sb">A StringBuilder object where serialized data will be written</param>
		/// <returns>true if value was serialized successfully; otherwise, false</returns>
		public static bool SerializeHexByteArray(byte[] values, StringBuilder sb)
		{
			if (values == null)
				values = new byte[0];
			sb.Append("0x");
			for (int i = 0; i < values.Length; ++i)
				sb.Append(values[i].ToString("x2"));
			return true;
		}

		#endregion

		/// <summary>
		/// Extracts an attribute or parameter serialized in text as MemberName={ ... }
		/// </summary>
		/// <param name="serializedData">The serialized data string</param>
		/// <param name="attributeName">When this method returns contains the name of the extracted attribute in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be extracted. This parameter is passed uninitialized</param>
		/// <param name="attributeData">When this method returns contains the serialized data of the extracted attribute in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be extracted. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool XtractNamedValueData(string serializedData, out string attributeName, out string attributeData)
		{
			int cc = 0;
			return XtractNamedValueData(serializedData, ref cc, out attributeName, out attributeData);
		}

		/// <summary>
		/// Extracts an attribute or parameter serialized in text as MemberName={ ... }
		/// </summary>
		/// <param name="serializedData">The serialized data string</param>
		/// <param name="cc">The read header position where to start the parsin operation. If extraction fails, the value is not modified</param>
		/// <param name="attributeName">When this method returns contains the name of the extracted attribute in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be extracted. This parameter is passed uninitialized</param>
		/// <param name="attributeData">When this method returns contains the serialized data of the extracted attribute in serializedData if the deserialization succeeded, or null if the deserialization failed. The deserialization fails if the serializedData parameter is a null reference (Nothing in Visual Basic) or its content could not be extracted. This parameter is passed uninitialized</param>
		/// <returns>true if serializedData was deserialized successfully; otherwise, false</returns>
		public static bool XtractNamedValueData(string serializedData, ref int cc, out string attributeName, out string attributeData)
		{
			int tcc = cc;
			int bcc = cc;
			int opar = 1;
			attributeData = null;
			Scanner.SkipSpaces(serializedData, ref tcc);
			if(!Parser.XtractIdentifier(serializedData, ref tcc, out attributeName))
				return false;
			Scanner.SkipSpaces(serializedData, ref tcc);
			if (!Scanner.ReadChar('=', serializedData, ref tcc))
				return false;
			if (!Scanner.ReadChar('{', serializedData, ref tcc))
				return false;
			while ((tcc < serializedData.Length) && (opar > 0))
			{
				if (serializedData[tcc] == '{') ++opar;
				if (serializedData[tcc] == '}') --opar;
				++tcc;
			}
			if (opar != 0)
				return false;
			cc = tcc;
			attributeData = serializedData.Substring(bcc, tcc - bcc);
			return true;
		}
	}
}
