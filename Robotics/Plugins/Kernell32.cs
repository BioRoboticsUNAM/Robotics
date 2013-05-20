using System;
using System.Runtime.InteropServices;

namespace Robotics.Plugins
{
	/// <summary>
	/// Encapsulates Kernell32 methods for loading unmanaged plugins
	/// </summary>
	public static class Kernel32
	{
		/// <summary>
		/// Dinamically loads a Dll
		/// </summary>
		/// <param name="dllToLoad">Path to the library file to load</param>
		/// <returns>A pointer where the library was loaded</returns>
		//[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		/// <summary>
		/// Gets a pointer to a function or procedure stored in a dynamic load library
		/// </summary>
		/// <param name="hModule">A pointer to where the library was loaded in memmory</param>
		/// <param name="procedureName">The name of the procedure to retrieve</param>
		/// <returns>A pointer to the procedure stored in a dynamic load library</returns>
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		/// <summary>
		/// Dinamically unloads a Dll
		/// </summary>
		/// <param name="hModule">A pointer to where the library was loaded in memmory</param>
		/// <returns>true if the library was successfylly unloaded, false otherwise</returns>
		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);

		/// <summary>
		/// Retrieves the last error produced
		/// </summary>
		/// <returns>An signed long integer which represents an error code</returns>
		[DllImport("kernel32.dll")]
		public static extern long GetLastError();
	}
}
