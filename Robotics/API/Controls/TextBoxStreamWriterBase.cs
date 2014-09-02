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
	public abstract class TextBoxStreamWriterBase : TextWriter, IDisposable
	{
		#region Variables

		private StreamWriter logFile;

		private ProducerConsumer<StringToken> pending;

		private int maxLines;

		private TextBox output = null;

		private StringEventHandler AppendStringEH;

		private Thread writerThread;

		private bool running;

		/// <summary>
		/// Stores a value that indicates if the time and date must be appended to each log entry
		/// </summary>
		protected bool appendDate;

		private StringBuilder waitingHadle;

		/// <summary>
		/// The default priority for messages
		/// </summary>
		private int defaultPriority;

		/// <summary>
		/// The verbosity treshold for the TextBox console. Only messages with a priority equal or higher than the treshold will be shown
		/// </summary>
		private int verbosityThreshold;
		
        /// <summary>
        /// Gets a value indicating whether the component is being disposed
        /// </summary>
        private bool disposing;

        /// <summary>
        /// Gets a value indicating whether the component is disposed
        /// </summary>
        private bool disposed;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		/// <param name="logFile">The path of the file to which dump the contents to</param>
		/// <param name="maxLines">The maximum number of lines the output TextBox object will display</param>
		public TextBoxStreamWriterBase(TextBox output, string logFile, int maxLines)
		{
			this.maxLines = maxLines;
			if (output != null)
			{
				this.output = output;
				output.Disposed += new EventHandler(output_Disposed);
				output.HandleDestroyed += new EventHandler(output_HandleDestroyed);
				output.HandleCreated += new EventHandler(output_HandleCreated);
			}
			pending = new ProducerConsumer<StringToken>(100);
			waitingHadle = new StringBuilder(1024);
			AppendStringEH = new StringEventHandler(AppendString);
			OpenLogFileStream(logFile, out this.logFile);

			appendDate = false;
			this.verbosityThreshold = 5;
			this.defaultPriority = 5;
            this.disposed = false;
            this.disposing = false;
            SetupThread();
		}

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		/// <param name="maxLines">The maximum number of lines the output TextBox object will display</param>
		public TextBoxStreamWriterBase(TextBox output, int maxLines) : this(output, "", maxLines) { }

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		public TextBoxStreamWriterBase(TextBox output) : this(output, 512) { }

		#endregion

		#region Destructor

		/// <summary>
		/// Releases all resources used by the TextBoxStreamWriter object
		/// </summary>
		~TextBoxStreamWriterBase()
		{
			this.running = false;
			Dispose(false);
			if ((writerThread != null) && writerThread.IsAlive)
			{
				writerThread.Join(100);
				if (writerThread.IsAlive)
					writerThread.Abort();
			}
			this.logFile = null;
			this.output = null;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating if the date and time of write operation must be appended to the writed data
		/// </summary>
		public bool AppendDate
		{
			get { return this.appendDate; }
			set { this.appendDate = value; }
		}

		/// <summary>
		/// Gets or sets the default priority for messages
		/// </summary>
		public int DefaultPriority
		{
			get { return this.defaultPriority; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
				this.defaultPriority = value;
			}
		}

        /// <summary>
        /// Gets a value indicating whether the component is being disposed
        /// </summary>
		public bool Disposing
		{
			get { return disposing; }
			protected set
			{
				if (disposing && !value)
					disposed = true;
				disposing = value;
			}
		}

		/// <summary>
		/// When overridden in a derived class, returns the Encoding in which the output is written
		/// </summary>
		public override System.Text.Encoding Encoding
		{
			get { return System.Text.Encoding.UTF8; }
		}

        /// <summary>
        /// Gets a value indicating whether the component is disposed
        /// </summary>
		public bool IsDisposed
		{
			get { return disposed; }
			protected set
			{
				if (disposed) return;
				disposed = value;
			}
		}

		/// <summary>
		/// Gets the TexBox object where the log is dumped
		/// </summary>
		protected TextBox Output { get { return this.output; } }

		/// <summary>
		/// Gets the stream used to write to the file where the log is dumped
		/// </summary>
		protected StreamWriter LogFile { get { return this.logFile; } }

		/// <summary>
		/// Gets or sets the verbosity treshold for the TextBox console. Only messages with a priority equal or higher than the treshold will be shown
		/// </summary>
		public int VerbosityThreshold
		{
			get { return this.verbosityThreshold; }
			set
			{
				if ((value < 0) || (value > 9))
					throw new ArgumentOutOfRangeException("Value must be between 0 and 9");
				this.verbosityThreshold = value;
			}
		}

		#endregion

		#region Methods

		#region Overloads

		#region Prioritized overloads

		#region Write overload

		/// <summary>
		/// Writes the text representation of a Boolean value to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The Boolean to write</param>
		public virtual void Write(int priority, bool value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes a character to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The character to write to the text stream</param>
		public virtual void Write(int priority, char value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="buffer">The character array to write to the text stream</param>
		public virtual void Write(int priority, char[] buffer)
		{
			pending.Produce(new StringToken(priority, new String(buffer)));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="buffer">The character array to write to the text stream</param>
		/// <param name="index">The start index at which begin the write</param>
		/// <param name="count">The number of characters to write</param>
		public virtual void Write(int priority, char[] buffer, int index, int count)
		{
			pending.Produce(new StringToken(priority, new String(buffer, index, count)));
		}

		/// <summary>
		/// Writes the text representation of a decimal value to the text stream. 
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The decimal value to write</param>
		public virtual void Write(int priority, decimal value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The double value to write</param>
		public virtual void Write(int priority, double value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The float value to write</param>
		public virtual void Write(int priority, float value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The int value to write</param>
		public virtual void Write(int priority, int value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The long value to write</param>
		public virtual void Write(int priority, long value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an object to the text stream by calling ToString on that object
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The object to write.</param>
		public virtual void Write(int priority, object value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes a string to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The string value to write</param>
		public virtual void Write(int priority, string value)
		{
			pending.Produce(new StringToken(priority, value));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The uint value to write</param>
		public virtual void Write(int priority, uint value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The ulong value to write</param>
		public virtual void Write(int priority, ulong value)
		{
			pending.Produce(new StringToken(priority, value.ToString()));
		}

		#endregion

		#region WriteLine Overload

		/// <summary>
		/// Writes the text representation of a Boolean value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The Boolean to write</param>
		public virtual void WriteLine(int priority, bool value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a character to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The character to write followed by a line terminator to the text stream.</param>
		public virtual void WriteLine(int priority, char value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="buffer">The character array to write followed by a line terminator to the text stream.</param>
		public virtual void WriteLine(int priority, char[] buffer)
		{
			pending.Produce(new StringToken(priority, new String(buffer) + base.NewLine));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="buffer">The character array to write followed by a line terminator to the text stream.</param>
		/// <param name="index">The start index at which begin the write</param>
		/// <param name="count">The number of characters to write</param>
		public virtual void WriteLine(int priority, char[] buffer, int index, int count)
		{
			pending.Produce(new StringToken(priority, new String(buffer, index, count) + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a decimal value followed by a line terminator to the text stream.. 
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The decimal value to write</param>
		public virtual void WriteLine(int priority, decimal value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The double value to write</param>
		public virtual void WriteLine(int priority, double value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The float value to write</param>
		public virtual void WriteLine(int priority, float value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The int value to write</param>
		public virtual void WriteLine(int priority, int value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The long value to write</param>
		public virtual void WriteLine(int priority, long value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an object followed by a line terminator to the text stream. by calling ToString on that object
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The object to write.</param>
		public virtual void WriteLine(int priority, object value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The string value to write</param>
		public virtual void WriteLine(int priority, string value)
		{
			pending.Produce(new StringToken(priority, value + base.NewLine));
		}
		

		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The uint value to write</param>
		public void WriteLine(int priority, uint value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="value">The ulong value to write</param>
		public void WriteLine(int priority, ulong value)
		{
			pending.Produce(new StringToken(priority, value.ToString() + base.NewLine));
		}

		#endregion

		#endregion

		#region Unprioritized Overloads

		#region Write overload

		/// <summary>
		/// Writes the text representation of a Boolean value to the text stream
		/// </summary>
		/// <param name="value">The Boolean to write</param>
		public override void Write(bool value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes a character to the stream
		/// </summary>
		/// <param name="value">The character to write to the text stream</param>
		public override void Write(char value)
		{
			//base.Write(value));
			//if (logFile != null) logFile.Write(value));
			//output.Invoke(AppendStringEH, new string[] { value.ToString() }));
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="buffer">The character array to write to the text stream</param>
		public override void Write(char[] buffer)
		{
			pending.Produce(new StringToken(defaultPriority, new String(buffer)));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="buffer">The character array to write to the text stream</param>
		/// <param name="index">The start index at which begin the write</param>
		/// <param name="count">The number of characters to write</param>
		public override void Write(char[] buffer, int index, int count)
		{
			pending.Produce(new StringToken(defaultPriority, new String(buffer, index, count)));
		}

		/// <summary>
		/// Writes the text representation of a decimal value to the text stream. 
		/// </summary>
		/// <param name="value">The decimal value to write</param>
		public override void Write(decimal value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value to the text stream
		/// </summary>
		/// <param name="value">The double value to write</param>
		public override void Write(double value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value to the text stream
		/// </summary>
		/// <param name="value">The float value to write</param>
		public override void Write(float value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer to the text stream
		/// </summary>
		/// <param name="value">The int value to write</param>
		public override void Write(int value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer to the text stream
		/// </summary>
		/// <param name="value">The long value to write</param>
		public override void Write(long value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an object to the text stream by calling ToString on that object
		/// </summary>
		/// <param name="value">The object to write.</param>
		public override void Write(object value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes a string to the text stream
		/// </summary>
		/// <param name="value">The string value to write</param>
		public override void Write(string value)
		{
			pending.Produce(new StringToken(defaultPriority, value));
			//base.Write(value));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer to the text stream
		/// </summary>
		/// <param name="value">The uint value to write</param>
		public override void Write(uint value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer to the text stream
		/// </summary>
		/// <param name="value">The ulong value to write</param>
		public override void Write(ulong value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString()));
		}

		#endregion

		#region WriteLine Overload

		/// <summary>
		/// Writes a line terminator to the text stream
		/// </summary>
		public override void WriteLine()
		{
			pending.Produce(new StringToken(defaultPriority, base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a Boolean value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The Boolean to write</param>
		public override void WriteLine(bool value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a character to the stream
		/// </summary>
		/// <param name="value">The character to write followed by a line terminator to the text stream.</param>
		public override void WriteLine(char value)
		{
			//base.Write(value));
			//if (logFile != null) logFile.Write(value));
			//output.Invoke(AppendStringEH, new string[] { value.ToString() }));
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="buffer">The character array to write followed by a line terminator to the text stream.</param>
		public override void WriteLine(char[] buffer)
		{
			pending.Produce(new StringToken(defaultPriority, new String(buffer) + base.NewLine));
		}

		/// <summary>
		/// Writes a character array to the stream
		/// </summary>
		/// <param name="buffer">The character array to write followed by a line terminator to the text stream.</param>
		/// <param name="index">The start index at which begin the write</param>
		/// <param name="count">The number of characters to write</param>
		public override void WriteLine(char[] buffer, int index, int count)
		{
			pending.Produce(new StringToken(defaultPriority, new String(buffer, index, count) + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a decimal value followed by a line terminator to the text stream.. 
		/// </summary>
		/// <param name="value">The decimal value to write</param>
		public override void WriteLine(decimal value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The double value to write</param>
		public override void WriteLine(double value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte floating-point value followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The float value to write</param>
		public override void WriteLine(float value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The int value to write</param>
		public override void WriteLine(int value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte signed integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The long value to write</param>
		public override void WriteLine(long value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an object followed by a line terminator to the text stream. by calling ToString on that object
		/// </summary>
		/// <param name="value">The object to write.</param>
		public override void WriteLine(object value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The string value to write</param>
		public override void WriteLine(string value)
		{
			pending.Produce(new StringToken(defaultPriority, value + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of a 4-byte unsigned integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The uint value to write</param>
		public override void WriteLine(uint value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		/// <summary>
		/// Writes the text representation of an 8-byte unsigned integer followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="value">The ulong value to write</param>
		public override void WriteLine(ulong value)
		{
			pending.Produce(new StringToken(defaultPriority, value.ToString() + base.NewLine));
		}

		#endregion

		#endregion

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device
		/// </summary>
		public override void Flush()
		{
			//doFlush = true;
			if (LogFile != null)
			{
				lock (LogFile)
				{
					if (LogFile.BaseStream.CanWrite)
						LogFile.Flush();
				}
			}

			base.Flush();
		}

		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			this.running = false;

			if (this.disposed || this.disposing)
				return;
			this.disposing = true;

			if (this.LogFile != null)
			{
				try { this.LogFile.Flush(); }
				catch { }
				try { this.LogFile.Close(); }
				catch { }
				try { LogFile.Dispose(); }
				catch { }
			}

			base.Dispose(disposing);

			this.disposing = false;
            this.disposed = true;
		}

		private void AppendString(string s)
		{
			if ((String.IsNullOrEmpty(s)) || (Output == null))
				return;
			try
			{
				if (((s.Length + Output.Text.Length) > Output.MaxLength) || (Output.Lines.Length > maxLines)) Output.Clear();
				Output.AppendText(s);
			}
			catch { }
		}

		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread is aborted
		/// </summary>
		protected virtual void OnThreadAborted() { }

		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread is started
		/// </summary>
		protected virtual void OnThreadStarted() { }

		/// <summary>
		/// When overriden in a derived class, it allows to perform actions where the thread finish its execution
		/// </summary>
		protected virtual void OnThreadStopped() { }

		/// <summary>
		/// When overriden in a derived class, opens a stream for the log file
		/// </summary>
		/// <param name="filePath">The path to the log file to create/open</param>
		/// <param name="stream">When this method returns contains a StreamWriter which allows to write in the specified file,
		/// or null if the file could not be created o could not be oppened</param>
		protected abstract void OpenLogFileStream(string filePath, out StreamWriter stream);

		/// <summary>
		/// Initializes the thread for asynchronous write in the file and TextBox logs
		/// </summary>
        protected virtual void SetupThread()
        {
            if(writerThread != null)
                return;
            writerThread = new Thread(new ThreadStart(WriterThread_Task));
			writerThread.IsBackground = true;
			writerThread.Priority = ThreadPriority.BelowNormal;
            this.running = true;
			writerThread.Start();
		}

		private void WriterThread_Task()
		{
			//string stringToAppend;
			// 8kB buffer
			StringBuilder fileSb = new StringBuilder(8192);
			StringBuilder textSb = new StringBuilder(8192);
			// Stopwatch to masure time
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

			OnThreadStarted();
			sw.Start();
			while (this.running && !this.disposing && !this.disposed)
			{

				try
				{
					BufferizeTokens(fileSb, textSb, sw);

					if (!running || (sw.ElapsedMilliseconds > 1000) || (fileSb.Length > fileSb.MaxCapacity / 2) || (textSb.Length > textSb.MaxCapacity / 2))
					{
						Flush(fileSb, textSb);
						sw.Reset();
						sw.Start();
					}

				}
				catch (ThreadAbortException)
				{
					OnThreadAborted();
					running = false;
					OnThreadStopped();
					return;
				}
				catch
				{
					Thread.Sleep(100);
					continue;
				}
			}
			OnThreadStopped();

			/*
			int elapsed = 0;
			StringBuilder sb;
			string s;

			this.running = true;

			while (this.running)
			{
				if (doFlush || (elapsed >= 100))
				{
					sb = new StringBuilder(4096);
					doFlush = false;
					while (pending.Count > 0)
					{
						sb.Append(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss "));
						sb.Append(pending.Dequeue());
					}
					s = sb.ToString();
					try
					{
						if (logFile != null) logFile.Write(s);
						if (this.running && output.IsHandleCreated && !output.Disposing && !output.IsDisposed)
							output.BeginInvoke(AppendStringEH, s);
					}
					catch { }
				}
				Thread.Sleep(10);
				elapsed += 10;
			}
			*/
		}

		private void BufferizeTokens(StringBuilder fileSb, StringBuilder textSb, System.Diagnostics.Stopwatch sw)
		{
			StringToken token;
			do
			{
				token = pending.Consume(100);
				if ((token == null) || String.IsNullOrEmpty(token.Value))
					continue;
				if (LogFile != null)
					fileSb.Append(TokenToFile(token));

				if (Output != null)
					textSb.Append(TokenToTextBox(token));
			} while (this.running && (pending.Count > 0) && (sw.ElapsedMilliseconds < 1000));
		}

		private void Flush(StringBuilder fileSb, StringBuilder textSb)
		{
			if (LogFile != null)
			{
				if (LogFile.BaseStream.CanWrite)
					LogFile.Write(fileSb.ToString());
				fileSb.Length = 0;
			}
			if (Output != null)
			{
				if (Output.IsHandleCreated && !Output.Disposing && !Output.IsDisposed)
					Output.Invoke(AppendStringEH, textSb.ToString());
				textSb.Length = 0;
			}
		}

		/// <summary>
		/// When overriden in a derived class returns a string to be appended to the TextBox log
		/// </summary>
		/// <param name="token">The token object used to generate the string to append</param>
		/// <returns>A string to be appended to the TextBox log</returns>
		protected abstract string TokenToTextBox(StringToken token);

		/// <summary>
		/// When overriden in a derived class returns a string to be appended to the log file
		/// </summary>
		/// <param name="token">The token object used to generate the string to append</param>
		/// <returns>A string to be appended to the log file</returns>
		protected abstract string TokenToFile(StringToken token);

		private void output_Disposed(object sender, EventArgs e)
		{
			this.Dispose();
		}

		private void output_HandleCreated(object sender, EventArgs e)
		{
			if (Output != null)
			{
				lock (Output)
				{
					if (this.running && Output.IsHandleCreated && !Output.Disposing && !Output.IsDisposed)
						Output.Invoke(AppendStringEH, waitingHadle.ToString());
				}
			}
			waitingHadle = new StringBuilder(1024);
		}

		private void output_HandleDestroyed(object sender, EventArgs e)
		{
			this.Dispose();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Releases all resources used by the TextBoxStreamWriter object
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region Event Handlers

		private void Application_ApplicationExit(object sender, EventArgs e)
		{
			this.Dispose();
		}

		#endregion
		 
		/// <summary>
		/// Encapsulates log messages with its priority and the creation time
		/// </summary>
		protected struct StringToken
		{
			/// <summary>
			/// The message string to write to the log
			/// </summary>
			private string value;
			/// <summary>
			/// The creation time of the message
			/// </summary>
			private DateTime ct;
			/// <summary>
			/// The priority of the message
			/// </summary>
			private int priority;

			/// <summary>
			/// Initializes a new instance of StringToken
			/// </summary>
			/// <param name="priority">The priority of the message</param>
			/// <param name="value">The message string to write to the log</param>
			public StringToken(int priority, string value)
			{
				this.value = value;
				this.ct = DateTime.Now;
				this.priority = priority;
			}

			/// <summary>
			/// Gets the message string to write to the log
			/// </summary>
			public string Value { get { return value; } }

			/// <summary>
			/// Gets the creation time of the message
			/// </summary>
			public DateTime CreationTime { get { return ct; } }

			/// <summary>
			/// Gets the priority of the message
			/// </summary>
			public int Priority { get { return this.priority; } }

			/// <summary>
			/// Implicitely converts a StringToken object to a string
			/// </summary>
			/// <param name="st">The StringToken object to convert</param>
			/// <returns>The message string to write to the log stored in the StringToken object</returns>
			public static implicit operator string(StringToken st)
			{
				return st.value;
			}

			/// <summary>
			/// Implicitely converts a string to a StringToken object with a priority of 5
			/// </summary>
			/// <param name="s">The message string to write to the log</param>
			/// <returns>A StringToken object with a priority of 5 with the input string as value</returns>
			public static implicit operator StringToken(string s)
			{
				return new StringToken(5, s);
			}
		}
	}
}
