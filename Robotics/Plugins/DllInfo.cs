using System;
using System.Collections.Generic;
using System.IO;

namespace Robotics.Plugins
{
	/// <summary>
	/// Encapsulates information about a Dll file
	/// </summary>
	public class DllInfo
	{
		#region Variables

		/// <summary>
		/// The path to the dll file
		/// </summary>
		private string filePath;
		/// <summary>
		/// Number identifying type of target machine. Section 3.3.1 of the PE format specification.
		/// </summary>
		private ushort machine;
		/// <summary>
		/// Number of sections; indicates size of the Section Table, which immediately follows the headers.
		/// </summary>
		private ushort sections;
		/// <summary>
		/// Time and date the file was created.
		/// </summary>
		private uint timestamp;
		/// <summary>
		/// Gets the Pointer To Symbol Table. File offset of the COFF symbol table or 0 if none is present.
		/// </summary>
		private uint pSymbolTable;
		/// <summary>
		/// Number Of Symbols. Number of entries in the symbol table.
		/// This data can be used in locating the string table, which immediately follows the symbol table
		/// </summary>
		private uint noOfSymbol;
		/// <summary>
		/// Size of the optional header, which is required for executable files but not for object files.
		/// An object file should have a value of 0 here.
		/// The format is described in the section “Optional Header” of the PE format specification.
		/// </summary>
		private ushort optionalHeaderSize;
		/// <summary>
		/// Flags indicating attributes of the file. See Section 3.3.2 of the PE format specification,
		/// “Characteristics,” for specific flag values.
		/// </summary>
		private ushort characteristics;
		private DataDictionay[] directories;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new inscance of DllInfo
		/// </summary>
		private DllInfo()
		{
			this.directories = new DataDictionay[16];
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the path to the Dll file
		/// </summary>
		public string FilePath
		{
			get { return this.filePath; }
		}

		/// <summary>
		/// Gets the target machine
		/// </summary>
		public MachineType Machine
		{
			get { return (MachineType)this.machine; }
		}

		/// <summary>
		/// Gets the number of sections; indicates size of the Section Table, which immediately follows the headers.
		/// </summary>
		public ushort Sections
		{
			get { return this.sections; }
		}

		/// <summary>
		/// Gets the time and date the file was created.
		/// </summary>
		public uint Timestamp
		{
			get { return this.timestamp; }
		}

		/// <summary>
		/// Gets the Pointer To Symbol Table. File offset of the COFF symbol table or 0 if none is present.
		/// </summary>
		public uint PointerToSymbolTable
		{
			get { return this.pSymbolTable; }
		}

		/// <summary>
		/// Gets the Number Of Symbols. Number of entries in the symbol table.
		/// This data can be used in locating the string table, which immediately follows the symbol table
		/// </summary>
		public uint NumberOfSymbols
		{
			get { return this.noOfSymbol; }
		}

		/// <summary>
		/// Size of the optional header, which is required for executable files but not for object files.
		/// An object file should have a value of 0 here.
		/// </summary>
		public ushort OptionalHeaderSize
		{
			get { return this.OptionalHeaderSize; }
		}

		/// <summary>
		/// Gets the flags indicating attributes of the file.
		/// </summary>
		public Characteristics Characteristics
		{
			get { return (Characteristics)this.characteristics; }
		}

		/// <summary>
		/// 
		/// </summary>
		public DataDictionay[] Directories
		{
			get { return this.directories; }
		}

		/// <summary>
		/// Gets a value indicating if the Dll is a .NET managed assembly
		/// </summary>
		public bool IsManagedAssembly
		{
			get
			{
				// The 15th directory consist of CLR header! if its 0, its not a CLR file :)
				return (Directories[14].RVA != 0);
			}
		}

		/// <summary>
		/// Gets the target architecture of the Dll
		/// </summary>
		public Architecture Architecture
		{
			get
			{
				switch (machine)
				{
					case (ushort)Architecture.x86:
						return Architecture.x86;

					case (ushort)Architecture.x64:
						return Architecture.x64;

					default:
						return Architecture.Unknown;

				}
			}
		}

		/// <summary>
		/// Returns 4 for 32 bit architecture, 8 for 64 bit architecture and
		/// -1 for unknown architecture
		/// </summary>
		public int PointerSize
		{
			get
			{
				switch (machine)
				{
					case (ushort)Architecture.x86:
						return 4;

					case (ushort)Architecture.x64:
						return 8;

					default:
						return -1;

				}
			}
		}

		#endregion

		#region Static Methods

		private static int GetDataDirectoriesOffset(Stream fs, BinaryReader reader)
		{
			// Enhanced with code from
			// http://stackoverflow.com/questions/8593264/determining-if-a-dll-is-a-valid-clr-dll-by-reading-the-pe-directly-64bit-issue

			long posEndOfHeader;
			ushort magic;
			int dataDirectoriesOffset;

			posEndOfHeader = fs.Position;
			magic = reader.ReadUInt16();

			// As on section 3.4 of the PE format specification.
			//0x20b == PE32+ (64Bit), 0x10b == PE32 (32Bit)
			if (magic == 0x020B)
				// Offset to data directories for 64Bit PE images
				dataDirectoriesOffset = 0x70;
			else
				// Offset to data directories for 32Bit PE images
				dataDirectoriesOffset = 0x60;

			return dataDirectoriesOffset;
		}

		/// <summary>
		/// GEts information about a Dll
		/// </summary>
		/// <param name="fileName">Path of the dll file</param>
		/// <returns>A DllInfo object which contains information about the Dll</returns>
		public static DllInfo GetDllInfo(string fileName)
		{
			// Code from
			// http://stackoverflow.com/questions/367761/how-to-determine-whether-a-dll-is-a-managed-assembly-or-native-prevent-loading
			// Enhanced with code from
			// http://stackoverflow.com/questions/8593264/determining-if-a-dll-is-a-valid-clr-dll-by-reading-the-pe-directly-64bit-issue

			uint peHeader;
			uint peHeaderSignature;
			ushort dataDictionaryStart;
			int dataDirectoriesOffset;

			Stream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			BinaryReader reader = new BinaryReader(fs);
			DllInfo dllInfo = new DllInfo();
			dllInfo.filePath = (new FileInfo(fileName)).FullName;

			//PE Header starts at 0x3C (60). Its a 4 byte header.
			fs.Position = 0x3C;
			peHeader = reader.ReadUInt32();

			//Moving to PE Header start location...
			fs.Position = peHeader;
			peHeaderSignature = reader.ReadUInt32();

			// Number identifying type of target machine.
			// Section 3.3.1 of the PE format specification.
			dllInfo.machine = reader.ReadUInt16();
			// Number of sections; indicates size of the Section Table,
			// which immediately follows the headers.
			dllInfo.sections = reader.ReadUInt16();
			// Time and date the file was created.
			dllInfo.timestamp = reader.ReadUInt32();
			// File offset of the COFF symbol table or 0 if none is present.
			dllInfo.pSymbolTable = reader.ReadUInt32();
			// Number of entries in the symbol table.
			// This data can be used in locating the string table, which immediately follows the symbol table
			dllInfo.noOfSymbol = reader.ReadUInt32();
			// Size of the optional header, which is required for executable files but not for object files
			dllInfo.optionalHeaderSize = reader.ReadUInt16();
			//
			dllInfo.characteristics = reader.ReadUInt16();

			dataDirectoriesOffset = GetDataDirectoriesOffset(fs, reader);

			// Now we are at the end of the PE Header and from here,
			// the PE Optional Headers starts...
			// To go directly to the datadictionary, we'll increase the stream’s
			// current position to with 96 (0x60).
			// 96 because, 28 for Standard fields 68 for NT-specific fields
			// From here DataDictionary starts...and its of total 128 bytes.
			//
			// DataDictionay has 16 directories in total, doing simple maths 128/16 = 8.
			// So each directory is of 8 bytes. In this 8 bytes, 4 bytes is of RVA and 4 bytes of Size.
			// By the way, the 15th directory consist of CLR header! if its 0, its not a CLR file :)
			dataDictionaryStart = Convert.ToUInt16(Convert.ToUInt16(fs.Position) + dataDirectoriesOffset);
			fs.Position = dataDictionaryStart;
			for (int i = 0; i < 15; i++)
			{
				dllInfo.directories[i] = new DataDictionay(reader.ReadUInt32(), reader.ReadUInt32());
			}
			fs.Close();

			return dllInfo;
		}

		#endregion

		#region Subclases

		/// <summary>
		/// Stores information about the address and size of a table or string used by Windows NT
		/// </summary>
		public class DataDictionay
		{
			#region Variables

			/// <summary>
			/// The Relative Virtual Address. In an image file, an RVA is always the address of an item
			/// once loaded into memory, with the base address of the image file subtracted from it.
			/// The RVA of an item will almost always differ from its position within the file on disk
			/// (File Pointer).
			/// </summary>
			private uint rva;
			/// <summary>
			/// Size in bytes of the data directory.
			/// </summary>
			private uint size;

			#endregion

			#region Constructor

			/// <summary>
			/// Initializes a new instance of DataDictionay
			/// </summary>
			/// <param name="rva">The address of the table, when loaded, relative to the base address of the image</param>
			/// <param name="size">Size in bytes of the data directory.</param>
			internal DataDictionay(uint rva, uint size)
			{
				this.rva = rva;
				this.size = size;
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets the Relative Virtual Address
			/// </summary>
			public uint RVA
			{
				get { return this.rva; }
			}

			/// <summary>
			/// Gets the size in bytes of the data directory.
			/// </summary>
			public uint Size
			{
				get { return this.size; }
			}

			#endregion

			#region Methods

			/// <summary>
			/// Converts the size into its string representation
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return "RVA = " + rva.ToString() + ", Size = " + size.ToString();
			}

			#endregion
		}

		#endregion
	}
}
