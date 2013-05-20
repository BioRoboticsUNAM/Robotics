using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Utilities
{
	/// <summary>
	/// Represents a writer that can write a sequential series of characters as a Log.
	/// </summary>
	public interface ILogWriter : IDisposable
	{
			#region Properties

			/// <summary>
			/// Gets or sets the default verbosity for write operations
			/// </summary>
			int DefaultVerbosity{get;set;}

			/// <summary>
			/// Gets the Encoding in which the output is written. 
			/// </summary>
			Encoding Encoding { get; }

			/// <summary>
			/// Gets an object that controls formatting. 
			/// </summary>
			IFormatProvider FormatProvider { get; }

			/// <summary>
			/// Gets or sets the line terminator string used by the current LogWriter. 
			/// </summary>
			string NewLine { get; }

			/// <summary>
			/// Gets or sets the verbosity treshold
			/// </summary>
			int VerbosityTreshold{get;set;}

			#endregion

			#region Methods
			/// <summary>
			/// Closes the current writer and releases any system resources associated with the writer. 
			/// </summary>
			void Close();

			/// <summary>
			/// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.  
			/// </summary>
			void Flush();

			/// <summary>
			/// Writes the given data type to a text stream.
			/// </summary>
			/// <param name="text">The string to write</param>
			void Write(string text);

			/// <summary>
			/// Writes the given data type to a text stream.
			/// </summary>
			/// <param name="level">The verbosity level</param>
			/// <param name="text">The string to write</param>
			void Write(int level, string text);

			/// <summary>
			/// If the default verbosity level is smaller than the verbosity treshold, writes some data as specified by the overloaded parameters, followed by a line terminator.
			/// </summary>
			/// <param name="text">The string to write</param>
			void WriteLine(string text);

			/// <summary>
			/// If the provided verbosity level is smaller than the verbosity treshold, writes some data as specified by the overloaded parameters, followed by a line terminator.
			/// </summary>
			/// <param name="level">The verbosity level</param>
			/// <param name="text">The string to write</param>
			void WriteLine(int level, string text);

			/// <summary>
			/// Writes a line terminator.
			/// </summary>
			void WriteLine();

			#endregion
		}
	}

