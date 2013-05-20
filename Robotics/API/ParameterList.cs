using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Robotics
{
	/// <summary>
	/// Enumerates the list of supported parameter types
	/// </summary>
	public enum ParameterType : byte
	{
		/// <summary>
		/// Represents a null parameter
		/// </summary>
		Null	= 0x00,
		/// <summary>
		/// Custom type
		/// </summary>
		CustomType = 0x00,
		/// <summary>
		/// An 8 bit unsigned integer
		/// </summary>
		Byte	= 0x02,
		/// <summary>
		/// An 8 bit signed integer
		/// </summary>
		SByte	= 0x03,
		/// <summary>
		/// An 8 bit integer which values can be either 0x00 for false or 0xFF for true
		/// </summary>
		Boolean	= 0x04,
		/// <summary>
		/// A 16 bit unicode character
		/// </summary>
		Char	= 0x05,
		/// <summary>
		/// A 16 bit signed integer
		/// </summary>
		Int16	= 0x06,
		/// <summary>
		/// A 32 bit signed integer
		/// </summary>
		Int32	= 0x07,
		/// <summary>
		/// A 64 bit signed integer
		/// </summary>
		Int64	= 0x08,
		/// <summary>
		/// A 16 bit unsigned integer
		/// </summary>
		UInt16	= 0x09,
		/// <summary>
		/// A 32 bit unsigned integer
		/// </summary>
		UInt32	= 0x0A,
		/// <summary>
		/// A 64 bit unsigned integer
		/// </summary>
		UInt64	= 0x0B,
		/// <summary>
		/// A single precision floating point type (32 bit)
		/// </summary>
		Single	= 0x0C,
		/// <summary>
		/// A double precision floating point type (64 bit)
		/// </summary>
		Double	= 0x0D,
		/// <summary>
		/// An 64 bit unsigned integer wich represents a date and time measured from
		/// Jan 1, 0001 at 00:00:00 with a resolution of 10 nanoseconds
		/// </summary>
		DateTime= 0x0E,
		/// <summary>
		/// A string of unicode characters.
		/// </summary>
		String	=0x0F,



		/// <summary>
		/// An array of 8 bit unsigned integers
		/// </summary>
		ByteArray = 0x82,
		/// <summary>
		/// An array of 8 bit signed integers
		/// </summary>
		SByteArray = 0x83,
		/// <summary>
		/// An array of booleans
		/// </summary>
		BooleanArray = 0x84,
		/// <summary>
		/// An array of 16 bit unicode characters
		/// </summary>
		CharArray = 0x85,
		/// <summary>
		/// An array of 16 bit signed integers
		/// </summary>
		Int16Array = 0x86,
		/// <summary>
		/// An array of 32 bit signed integers
		/// </summary>
		Int32Array = 0x87,
		/// <summary>
		/// An array of 64 bit signed integers
		/// </summary>
		Int64Array = 0x88,
		/// <summary>
		/// An array of 16 bit unsigned integers
		/// </summary>
		UInt16Array = 0x89,
		/// <summary>
		/// An array of 32 bit unsigned integers
		/// </summary>
		UInt32Array = 0x8A,
		/// <summary>
		/// An array of 64 bit unsigned integers
		/// </summary>
		UInt64Array = 0x8B,
		/// <summary>
		/// An array of single precision floating point types (32 bit)
		/// </summary>
		SingleArray = 0x8C,
		/// <summary>
		/// An array of double precision floating point types (64 bit)
		/// </summary>
		DoubleArray = 0x8D,
		/// <summary>
		/// An array of DateTime structs
		/// </summary>
		DateTimeArray = 0x8E,
		/// <summary>
		/// An array of strings of unicode characters.
		/// </summary>
		StringArray = 0x8F,
		
	}

	/// <summary>
	/// Encapsulates the list of parameters of a binary Command/Response
	/// </summary>
	public class ParameterList : IEnumerable<Object>
	{
		#region Variables

		/// <summary>
		/// The list of parameters of the Command/Response
		/// </summary>
		protected readonly List<Object> parameters;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Parameter List
		/// </summary>
		public ParameterList()
		{
			this.parameters = new List<Object>(parameters);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of elements contained in the Parameter List.
		/// </summary>
		public int Count
		{
			get { return this.parameters.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the Parameter List is read-only.
		/// </summary>
		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		#region Indexers

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		/// <returns>The element at the specified index.</returns>
		public object this[int index]
		{
			get { return this.parameters[index]; }
		}

		#endregion

		#endregion

		#region Methodos

		#region Add methods

		#region Single types

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(byte item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(sbyte item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(bool item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(char item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(short item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(ushort item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(int item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(uint item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(long item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(ulong item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(float item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(double item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(DateTime item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(string item)
		{
			this.parameters.Add(item);
		}

		#endregion

		#region Array types

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(byte[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(sbyte[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(bool[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(char[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(short[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(ushort[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(int[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(uint[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(long[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(ulong[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(float[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(double[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(DateTime[] item)
		{
			this.parameters.Add(item);
		}

		/// <summary>
		/// Adds an item to the Parameter List.
		/// </summary>
		/// <param name="item">The object to add to the Parameter List</param>
		public void Add(string[] item)
		{
			this.parameters.Add(item);
		}

		#endregion

		#endregion

		#region Insert methods

		#region Single types

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, byte item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, sbyte item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, char item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, short item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, ushort item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, int item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, uint item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, long item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, ulong item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, DateTime item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, string item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, bool item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, float item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, double item)
		{
			this.parameters.Insert(index, item);
		}

		#endregion

		#region Array types

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, byte[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, sbyte[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, char[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, short[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, ushort[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, int[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, uint[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, long[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, ulong[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, DateTime[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, string[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, bool[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, float[] item)
		{
			this.parameters.Insert(index, item);
		}

		/// <summary>
		/// Inserts an item to the Parameter List at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which item should be inserted.</param>
		/// <param name="item">The object to insert into the Parameter List.</param>
		public void Insert(int index, double[] item)
		{
			this.parameters.Insert(index, item);
		}

		#endregion

		#endregion

		/// <summary>
		/// Removes all items from the Parameter List
		/// </summary>
		public void Clear()
		{
			this.parameters.Clear();
		}

		/// <summary>
		/// Determines whether the Parameter List contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the Parameter List</param>
		/// <returns>true if item is found in the Parameter List; otherwise, false.</returns>
		public bool Contains(object item)
		{
			return this.parameters.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the Parameter List to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from Parameter List. The Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(object[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Determines the index of a specific item in the Parameter List.
		/// </summary>
		/// <param name="item">The object to locate in the Parameter List.</param>
		/// <returns>The index of item if found in the Parameter List; otherwise, -1.</returns>
		public int IndexOf(object item)
		{
			return this.parameters.IndexOf(item);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the Parameter List.
		/// </summary>
		/// <param name="item">The object to remove from the Parameter List.</param>
		/// <returns>true if item was successfully removed from the Parameter List; otherwise, false. This method also returns false if item is not found in the original Parameter List.</returns>
		public bool Remove(object item)
		{
			return this.parameters.Remove(item);
		}

		/// <summary>
		/// Removes the Parameter List item at the specified index
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		public void RemoveAt(int index)
		{
			this.parameters.RemoveAt(index);
		}

		#region Serialization Methods

		private byte GetParameterType(object o)
		{
			if (o == null)
				return 0;

			Type type = o.GetType();
			switch (type.Name)
			{
				case "Byte": return 0x02;
				case "SByte": return 0x03;
				case "Boolean": return 0x04;
				case "Char": return 0x05;
				case "Int16": return 0x06;
				case "Int32": return 0x07;
				case "Int64": return 0x08;
				case "UInt16": return 0x09;
				case "UInt32": return 0x0A;
				case "UInt64": return 0x0B;
				case "Single": return 0x0C;
				case "Double": return 0x0D;
				case "DateTime": return 0x0E;
				case "String": return 0x0F;

				case "Byte[]": return 0x82;
				case "SByte[]": return 0x83;
				case "Boolean[]": return 0x84;
				case "Char[]": return 0x85;
				case "Int16[]": return 0x86;
				case "Int32[]": return 0x87;
				case "Int64[]": return 0x88;
				case "UInt16[]": return 0x89;
				case "UInt32[]": return 0x8A;
				case "UInt64[]": return 0x8B;
				case "Single[]": return 0x8C;
				case "Double[]": return 0x8D;
				case "DateTime[]": return 0x8E;
				case "String[]": return 0x8F;

				default: return 0;
			}
		}

		/// <summary>
		/// Serializes the parameter at the specified position
		/// </summary>
		/// <param name="i">The zero-based index of the item to serialize.</param>
		/// <param name="stream">The stream where to serialize the data</param>
		private void SerializeParameterAt(Stream stream, int i)
		{
			byte bType;
			int arraySize;
			Array oArray;
			byte[] data = null;

			object obj = this.parameters[i];
			if (obj == null)
			{
				stream.WriteByte((byte)ParameterType.Null);
				return;
			}

			// Serialize data type
			bType = GetParameterType(obj);
			stream.WriteByte(bType);

			// Serialize single types
			oArray = obj as Array;
			if (oArray == null)
			{
				SerializeParameter(stream, (ParameterType)bType, obj);
				return;
			}
			
			// Serialize array types
			arraySize = oArray.Length;
			data = BitConverter.GetBytes((ushort)arraySize);
			stream.Write(data, 0, data.Length);
			foreach (object o in oArray)
				SerializeParameter(stream, (ParameterType)(0x7F&bType), o);

		}

		private void SerializeParameter(Stream stream, ParameterType parameterType, object o)
		{
			byte[] data = null;

			switch (parameterType)
			{
				case ParameterType.Byte: data = new byte[] { (byte)o }; break;
				case ParameterType.SByte: data = new byte[] { (byte)o }; break;
				case ParameterType.Boolean: data = BitConverter.GetBytes((bool)o); break;
				case ParameterType.Char: data = BitConverter.GetBytes((char)o); break;
				case ParameterType.Int16: data = BitConverter.GetBytes((short)o); break;
				case ParameterType.Int32: data = BitConverter.GetBytes((int)o); break;
				case ParameterType.Int64: data = BitConverter.GetBytes((long)o); break;
				case ParameterType.UInt16: data = BitConverter.GetBytes((ushort)o); break;
				case ParameterType.UInt32: data = BitConverter.GetBytes((uint)o); break;
				case ParameterType.UInt64: data = BitConverter.GetBytes((ulong)o); break;
				case ParameterType.Single: data = BitConverter.GetBytes((float)o); break;
				case ParameterType.Double: data = BitConverter.GetBytes((double)o); break;
				case ParameterType.DateTime: data = BitConverter.GetBytes(((DateTime)o).Ticks); break;
				case ParameterType.String:
					string s = o as string;
					data = BitConverter.GetBytes((ushort)s.Length);
					stream.Write(data, 0, data.Length);
					data = System.Text.UnicodeEncoding.Unicode.GetBytes(s);
					break;

				default: return;
			}

			stream.Write(data, 0, data.Length);
		}

		/// <summary>
		/// Attempts to serialize the parameters
		/// </summary>
		/// <param name="stream">The stream where to serialize the data</param>
		/// <param name="ex">When this method returns contains the exception to be thrown if the serialization failed, or null if the serialization succeeded</param>
		/// <returns>true if the parameters were successfully serialized to the stream, false otherwise</returns>
		protected internal bool TrySerialize(Stream stream, out Exception ex)
		{
			ex = null;
			if (stream == null)
			{
				ex = new ArgumentNullException("Null stream provided");
				return false;
			}

			if (this.parameters.Count == 0)
				return true;

			try
			{
				// Serialize the number of parameters
				stream.WriteByte((byte)this.parameters.Count);

				// Serialize each parameter
				for (int i = 0; i < this.parameters.Count; ++i)
					SerializeParameterAt(stream, i);

			}
			catch (Exception tex) { ex = tex; return false; }
			return true;
		}

		#endregion

		#region IEnumerable<object> Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection
		/// </summary>
		public IEnumerator<object> GetEnumerator()
		{
			return this.parameters.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.parameters.GetEnumerator();
		}

		#endregion

		#endregion
	}
}
