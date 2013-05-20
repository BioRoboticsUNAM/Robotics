using System;
using System.IO;
using System.Text;

namespace Robotics.Utilities
{
	/// <summary>
	/// Represents a writer that can write a sequential series of characters as a Log.
	/// </summary>
	public class LogWriter : ILogWriter
	{
		#region Variables

		/// <summary>
		/// The text writer to perform write operations
		/// </summary>
		protected TextWriter textWriter;

		/// <summary>
		/// The verbosity treshold
		/// </summary>
		protected int verbosityTreshold;

		/// <summary>
		/// Default verbosity for write operations
		/// </summary>
		protected int defaultVerbosity;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the LogWriter class
		/// </summary>
		public LogWriter()
			: this(TextWriter.Null, 5)
		{

		}

		/// <summary>
		/// Initializes a new instance of the LogWriter class
		/// </summary>
		/// <param name="textWriter">The text writer to perform write operations</param>
		public LogWriter(TextWriter textWriter)
			: this(textWriter, 5) { }

		/// <summary>
		/// Initializes a new instance of the LogWriter class
		/// </summary>
		/// <param name="textWriter">The text writer to perform write operations</param>
		/// <param name="defaultVerbosity">Default verbosity for write operations</param>
		public LogWriter(TextWriter textWriter, int defaultVerbosity)
		{
			this.textWriter = textWriter;
			this.defaultVerbosity = defaultVerbosity;
			this.VerbosityTreshold = 5;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the default verbosity for write operations
		/// </summary>
		public int DefaultVerbosity
		{
			get { return defaultVerbosity; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException("value", "The verbosity must be between 0 and 9");
				defaultVerbosity = value;
			}
		}

		/// <summary>
		///   When overridden in a derived class, returns the Encoding in which the output is written. 
		/// </summary>
		public Encoding Encoding
		{
			get { return textWriter.Encoding; }
		}

		/// <summary>
		/// Gets an object that controls formatting. 
		/// </summary>
		public IFormatProvider FormatProvider
		{
			get { return textWriter.FormatProvider; }
		}

		/// <summary>
		/// Gets or sets the line terminator string used by the current LogWriter. 
		/// </summary>
		public string NewLine
		{
			get { return textWriter.NewLine; }
		}

		/// <summary>
		/// gets or sets the verbosity treshold
		/// </summary>
		public int VerbosityTreshold
		{
			get { return verbosityTreshold; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException("value", "The verbosity must be between 0 and 9");
				verbosityTreshold = value;
			}
		}

		#endregion

		#region Methods
		/// <summary>
		/// Closes the current writer and releases any system resources associated with the writer. 
		/// </summary>
		public void Close() { textWriter.Close(); }

		/// <summary>
		/// Overloaded. Releases all resources used by the TextWriter object.
		/// </summary>
		public void Dispose() { textWriter.Dispose(); }

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.  
		/// </summary>
		public void Flush() { textWriter.Flush(); }

		/// <summary>
		/// Writes the given data type to a text stream.
		/// </summary>
		/// <param name="text">The string to write</param>
		public void Write(string text)
		{
			if (defaultVerbosity >= verbosityTreshold)
				textWriter.Write(text);
		}

		/// <summary>
		/// Writes the given data type to a text stream.
		/// </summary>
		/// <param name="level">The verbosity level</param>
		/// <param name="text">The string to write</param>
		public void Write(int level, string text)
		{
			if (level <= verbosityTreshold)
				textWriter.Write(text);
		}

		/// <summary>
		/// If the provided verbosity level is smaller than the verbosity treshold, writes some data as specified by the overloaded parameters, followed by a line terminator.
		/// </summary>
		/// <param name="text">The string to write</param>
		public void WriteLine(string text)
		{
			if (defaultVerbosity <= verbosityTreshold)
				textWriter.WriteLine(text);
		}

		/// <summary>
		/// If the provided verbosity level is smaller than the verbosity treshold, writes some data as specified by the overloaded parameters, followed by a line terminator.
		/// </summary>
		/// <param name="level">The verbosity level</param>
		/// <param name="text">The string to write</param>
		public void WriteLine(int level, string text)
		{
			if (level <= verbosityTreshold)
				textWriter.WriteLine(text);
		}

		/// <summary>
		/// Writes a line terminator.
		/// </summary>
		public void WriteLine()
		{
			textWriter.WriteLine();
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			textWriter.Dispose();
		}

		#endregion
	}
}
