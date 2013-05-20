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
	/// Implements a TextWriter that dumps its contents to both a System.Windows.Forms.TexBox and a file
	/// </summary>
	public class TextBoxStreamWriter : TextBoxStreamWriterBase
	{
		#region Variables
		
		/// <summary>
		/// The verbosity treshold for the log file console. Only messages with a priority equal or higher than the treshold will be shown
		/// </summary>
		private int logFileVerbosityThreshold;
		
		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		/// <param name="logFile">The path of the file to which dump the contents to</param>
		/// <param name="maxLines">The maximum number of lines the output TextBox object will display</param>
		public TextBoxStreamWriter(TextBox output, string logFile, int maxLines) :
			base(output, logFile, maxLines) { }

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		/// <param name="maxLines">The maximum number of lines the output TextBox object will display</param>
		public TextBoxStreamWriter(TextBox output, int maxLines) : this(output, "", maxLines) { }

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		public TextBoxStreamWriter(TextBox output) : this(output, 512) { }

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the verbosity treshold for the log file console. Only messages with a priority equal or higher than the treshold will be shown
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
		/// or null if the file could not be created o could not be oppened</param>
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

		/// <summary>
		/// Returns a string to be appended to the TextBox log
		/// </summary>
		/// <param name="token">The token object used to generate the string to append</param>
		/// <returns>A string to be appended to the TextBox log</returns>
		protected override string TokenToTextBox(StringToken token)
		{
			string s  = String.Empty;
			if (token.Priority > VerbosityThreshold)
				return s;
			if (appendDate)
				s = token.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff ");
			s+= token.Value;
			return s;
		}

		/// <summary>
		/// Returns a string to be appended to the log file
		/// </summary>
		/// <param name="token">The token object used to generate the string to append</param>
		/// <returns>A string to be appended to the log file</returns>
		protected override string TokenToFile(StringToken token)
		{
			string s = String.Empty;
			if (token.Priority > logFileVerbosityThreshold)
				return s;
			if (appendDate)
				s = token.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff ");
			s += token.Value;
			return s;
		}

		#endregion

		#region Static Variables
		/// <summary>
		/// TextBoxStreamWriter with output to a file with the same name as the executable
		/// </summary>
		private static TextBoxStreamWriter defaultLog;

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets a TextBoxStreamWriter with output to a file with the same name as the executable
		/// </summary>
		public static TextBoxStreamWriter DefaultLog
		{
			get
			{
				if (defaultLog == null)
					defaultLog = new TextBoxStreamWriter(new TextBox(), Application.ExecutablePath + ".log", 1);
				return defaultLog;
			}
			set { if (value != null)defaultLog = value; }
		}

		#endregion

	}
}
