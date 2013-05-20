using System;

namespace Robotics.Plugins
{
	/// <summary>
	/// Enumerates the architectures of a DLL
	/// </summary>
	public enum Architecture
	{
		/// <summary>
		/// The Dll has been compiled for a x86 platform
		/// </summary>
		x86 = 0x14C,
		/// <summary>
		/// The Dll has been compiled for a x64 platform
		/// </summary>
		x64 = 0x8664,
		/// <summary>
		/// The Dll has been compiled for an unknown platform
		/// </summary>
		Unknown = 0
	}

	/// <summary>
	/// Enumerates the machine (CPU) type for executable types.
	/// </summary>
	public enum MachineType
	{
		/// <summary>
		/// Contents assumed to be applicable to any machine type.
		/// </summary>
		Unknown	= 0,
		/// <summary>
		/// CPU is an Alpha AXP™.
		/// </summary>
		Alpha = 0x184,
		/// <summary>
		/// CPU is an ARM
		/// </summary>
		ARM = 0x1C0,
		/// <summary>
		/// CPU is an Alpha AXP™ 64-bit
		/// </summary>
		Alpha64 = 0x284,
		/// <summary>
		/// CPU is an Intel 386 or later, and compatible processors.
		/// </summary>
		i386 = 0x14C,
		/// <summary>
		/// CPU is an Intel IA64™
		/// </summary>
		ia64 = 0x200,
		/// <summary>
		/// CPU is a Motorola 68000 series
		/// </summary>
		M68K = 0x268,
		/// <summary>
		/// CPU is a MIPS16
		/// </summary>
		MIPS16 = 0x266,
		/// <summary>
		/// CPU is a MIPS with FPU
		/// </summary>
		MIPSFPU = 0x366,
		/// <summary>
		/// CPU is a MIPS16 with FPU
		/// </summary>
		MIPSFPU16 = 0x466,
		/// <summary>
		/// CPU is a  Power PC, little endian.
		/// </summary>
		PowerPC = 0x1F0,
		/// <summary>
		/// CPU is a R3000
		/// </summary>
		R3000 = 0x162,
		/// <summary>
		/// CPU is a MIPS® little endian.
		/// </summary>
		R4000 = 0x166,
		/// <summary>
		/// CPU is a R10000
		/// </summary>
		R10000 = 0x168,
		/// <summary>
		/// CPU is a Hitachi SH3
		/// </summary>
		SH3 = 0x1a2,
		/// <summary>
		/// CPU is a Hitachi SH4
		/// </summary>
		SH4 = 0x1a6,
		/// <summary>
		/// CPU is a Thumb
		/// </summary>
		Thumb = 0x1C2	

	}

	/// <summary>
	/// Contains flags that indicate attributes of the object or image file
	/// </summary>
	[Flags]
	public enum Characteristics
	{
		/// <summary>
		/// Image only, Windows CE, Windows NT and above. Indicates that the file
		/// does not contain base relocations and must therefore be loaded at its
		/// preferred base address. If the base address is not available,
		/// the loader reports an error. Operating systems running on top of MS-DOS (Win32s™)
		/// are generally not able to use the preferred base address and so cannot run these images.
		/// However, beginning with version 4.0, Windows will use an application’s preferred base address.
		/// The default behavior of the linker is to strip base relocations from EXEs.
		/// </summary>
		RelocsStripped = 0x0001,
		/// <summary>
		/// Image only. Indicates that the image file is valid and can be run. If this flag is not set,
		/// it generally indicates a linker error.
		/// </summary>
		ExecutableImage = 0x0002,
		/// <summary>
		/// COFF line numbers have been removed.
		/// </summary>
		LineNumsStripped = 0x0004,
		/// <summary>
		/// COFF symbol table entries for local symbols have been removed.
		/// </summary>
		LocalSymsStripped = 0x0008,
		/// <summary>
		/// Aggressively trim working set.
		/// </summary>
		AggressiveTrim = 0x0010,
		/// <summary>
		/// App can handle > 2gb addresses.
		/// </summary>
		LargeAddressAware = 0x0020,
		/// <summary>
		/// Use of this flag is reserved for future use.
		/// </summary>
		Machine16 = 0x0040,
		/// <summary>
		/// Little endian: LSB precedes MSB in memory.
		/// </summary>
		ReversedBytesLo = 0x0080,
		/// <summary>
		/// Machine based on 32-bit-word architecture.
		/// </summary>
		Machine32 = 0x0100,
		/// <summary>
		/// Debugging information removed from image file.
		/// </summary>
		DebugStripped = 0x0200,
		///<summary>
		/// If image is on removable media, copy and run from swap file.
		/// </summary>
		SemovableRunFromSwap = 0x0400,
		/// <summary>
		/// The image file is a system file, not a user program.
		/// </summary>
		System = 0x1000,
		/// <summary>
		/// The image file is a dynamic-link library (DLL).
		/// Such files are considered executable files for almost all purposes,
		/// although they cannot be directly run.
		/// </summary>
		Dll = 0x2000,
		/// <summary>
		/// File should be run only on a UP machine.
		/// </summary>
		UpSystemOnly = 0x4000,
		/// <summary>
		/// Big endian: MSB precedes LSB in memory.
		/// </summary>
		BytesReversedHI = 0x8000
	}
}