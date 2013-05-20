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
	/// Implements a TextWriter that dumps its contents to both a System.Windows.Forms.TexBox and a xml file
	/// </summary>
	public class XmlTextBoxStreamWriter : TextBoxStreamWriterBase
	{
		#region Variables

		/// <summary>
		/// Stores an Id for the messages of the log session
		/// </summary>
		private int recordId;
		private bool logRecordTagOpen;
		private bool logTagOpen;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		/// <param name="logFile">The path of the file to which dump the contents to</param>
		/// <param name="maxLines">The maximum number of lines the output TextBox object will display</param>
		public XmlTextBoxStreamWriter(TextBox output, string logFile, int maxLines)
			: base(output, logFile, maxLines)
		{
			this.recordId = 0;
			this.logRecordTagOpen = false;
			this.logTagOpen = false;
		}

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		/// <param name="maxLines">The maximum number of lines the output TextBox object will display</param>
		public XmlTextBoxStreamWriter(TextBox output, int maxLines) : this(output, "", maxLines) { }

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		public XmlTextBoxStreamWriter(TextBox output) : this(output, 512) { }

		#endregion

		#region Properties

		#endregion

		#region Methods

		private void CloseLogTag()
		{
			try
			{
				if (logTagOpen && (LogFile != null))
					LogFile.WriteLine("</log>");
				LogFile.Flush();
			}
			catch { }
			this.logTagOpen = false;
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			OnThreadStopped();
			base.Dispose(disposing);
		}

		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread is aborted
		/// </summary>
		protected override void OnThreadAborted()
		{
		}

		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread is started
		/// </summary>
		protected override void OnThreadStarted()
		{
			if (LogFile == null)
				return;
			LogFile.WriteLine();
			LogFile.WriteLine("<log startTime=\"" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\">");
			logTagOpen = true;
		}

		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread finish its execution
		/// </summary>
		protected override void OnThreadStopped()
		{
			CloseLogTag();
		}

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
				if (!File.Exists(filePath))
				{
					stream = File.CreateText(filePath);
					LogFile.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
				}
				else stream = new StreamWriter(filePath, true, Encoding.UTF8);					
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
			string s = String.Empty;
			if (token.Priority > VerbosityThreshold)
				return s;
			if (appendDate)
				s = token.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff ");
			s+=token.Value;
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
			string value;

			if (!logRecordTagOpen)
			{
				s = "\t<logRecord id=\"" + recordId.ToString() + "\" time=\"" +
					token.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff") +
					"\" priority=\"" + token.Priority.ToString() + "\"" + ">";
				++recordId;
				logRecordTagOpen = true;
			}

			value= token.Value.Replace("<", "&lt;").Replace(">", "&gt;");
			if (!string.IsNullOrEmpty(LogFile.NewLine) && token.Value.EndsWith(LogFile.NewLine))
			{
				s += value.Substring(0, value.Length - LogFile.NewLine.Length) + "</logRecord>" + LogFile.NewLine;
				logRecordTagOpen = false;
			}
			else
				s += value;

			return s;
		}

		#endregion

		#region Static Variables
		/// <summary>
		/// TextBoxStreamWriter with output to a file with the same name as the executable
		/// </summary>
		private static XmlTextBoxStreamWriter defaultLog;

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets a TextBoxStreamWriter with output to a file with the same name as the executable
		/// </summary>
		public static XmlTextBoxStreamWriter DefaultLog
		{
			get
			{
				if (defaultLog == null)
					defaultLog = new XmlTextBoxStreamWriter(new TextBox(), Application.ExecutablePath + ".log.xml", 1);
				return defaultLog;
			}
			set { if (value != null)defaultLog = value; }
		}

		#endregion

	}
}
