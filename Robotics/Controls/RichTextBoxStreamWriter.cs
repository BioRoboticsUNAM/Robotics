using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using Robotics;
using System.Diagnostics;

namespace Robotics.Controls
{
	/// <summary>
	/// Implements a TextWriter that dumps its contents to both a System.Windows.Forms.RichTexBox and a file
	/// </summary>
	public class RichTextBoxStreamWriter : RichTextBoxStreamWriterBase
	{
		#region Variables

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the RichTextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		/// <param name="logFile">The path of the file to which dump the contents to</param>
		/// <param name="maxLines">The maximum number of lines the output TextBox object will display</param>
		public RichTextBoxStreamWriter(RichTextBox output, string logFile, int maxLines) :
			base(output, logFile, maxLines) { }

		/// <summary>
		/// Initializes a new instance of the RichTextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		/// <param name="maxLines">The maximum number of lines the output TextBox object will display</param>
		public RichTextBoxStreamWriter(RichTextBox output, int maxLines) : this(output, "", maxLines) { }

		/// <summary>
		/// Initializes a new instance of the RichTextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		public RichTextBoxStreamWriter(RichTextBox output) : this(output, 512) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the verbosity threshold for the log file console. Only messages with a priority equal or higher than the threshold will be shown
		/// </summary>
		public int LogFileVerbosityThreshold
		{
			get { return this.logFileVerbosityThreshold; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
				this.logFileVerbosityThreshold = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Opens a stream for the log file
		/// </summary>
		/// <param name="filePath">The path to the log file to create/open</param>
		/// <param name="stream">When this method returns contains a StreamWriter which allows to write in the specified file,
		/// or null if the file could not be created o could not be opened</param>
		protected override void OpenLogFileStream(string filePath, out StreamWriter stream)
		{
			try
			{
				if (!File.Exists(filePath)) stream = File.CreateText(filePath);
				else stream = new StreamWriter(filePath, true, Encoding.UTF8);
				this.LogFile.WriteLine(this.LogFile.NewLine);
				this.LogFile.WriteLine(DateTime.Now);
			}
			catch
			{
				stream = null;
			}
		}

		#endregion

		#region Static Constructors

		/// <summary>
		/// Initializes all static variables
		/// </summary>
		static RichTextBoxStreamWriter()
		{
			RichTextBoxStreamWriter.defaultLog = new RichTextBoxStreamWriter(new RichTextBox(), Application.ExecutablePath + ".log", 1);
		}

		#endregion

		#region Static Variables
		/// <summary>
		/// RichTextBoxStreamWriter with output to a file with the same name as the executable
		/// </summary>
		private static RichTextBoxStreamWriter defaultLog;

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets a RichTextBoxStreamWriter with output to a file with the same name as the executable
		/// </summary>
		public static RichTextBoxStreamWriter DefaultLog
		{
			get { return defaultLog; }
			set
			{
				if (value == null) throw new ArgumentNullException();
				defaultLog = value;
			}
		}

		#endregion

	}
}
