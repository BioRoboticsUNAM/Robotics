using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
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
	public abstract class RichTextBoxStreamWriterBase : TextWriter, IDisposable
	{
		#region Variables

		/// <summary>
		/// The verbosity threshold for the log file console. Only messages with a priority equal or higher than the threshold will be shown
		/// </summary>
		protected int logFileVerbosityThreshold;

		private StreamWriter logFile;

		private ProducerConsumer<StringToken> pending;

		private int maxLines;

		private RichTextBox output = null;

		private AppendTokenMethod appendTokenMethod;

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
		/// The verbosity threshold for the TextBox console. Only messages with a priority equal or higher than the threshold will be shown
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
		public RichTextBoxStreamWriterBase(RichTextBox output, string logFile, int maxLines)
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
			appendTokenMethod = new AppendTokenMethod(AppendToken);
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
		public RichTextBoxStreamWriterBase(RichTextBox output, int maxLines) : this(output, "", maxLines) { }

		/// <summary>
		/// Initializes a new instance of the TextBoxStreamWriter class
		/// </summary>
		/// <param name="output">The TextBox object to dump the contents to</param>
		public RichTextBoxStreamWriterBase(RichTextBox output) : this(output, 512) { }

		#endregion

		#region Destructor

		/// <summary>
		/// Releases all resources used by the TextBoxStreamWriter object
		/// </summary>
		~RichTextBoxStreamWriterBase()
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

		#region Delegates

		/// <summary>
		/// Represents a function that receives a method as parameter
		/// </summary>
		/// <param name="token">The token</param>
		private delegate void AppendTokenMethod(StringToken token);

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating if the date and time of write operation must be appended to the written data
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
		protected RichTextBox Output { get { return this.output; } }

		/// <summary>
		/// Gets the stream used to write to the file where the log is dumped
		/// </summary>
		protected StreamWriter LogFile { get { return this.logFile; } }

		/// <summary>
		/// Gets or sets the verbosity threshold for the TextBox console. Only messages with a priority equal or higher than the threshold will be shown
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
		/// Writes a string to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="color">The color to write the string</param>
		/// <param name="value">The string value to write</param>
		public virtual void Write(int priority, Color color, string value)
		{
			pending.Produce(new StringToken(priority, value, color));
		}

		/// <summary>
		/// Writes a string to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="fontStyle">The style of the font used to write the string</param>
		/// <param name="value">The string value to write</param>
		public virtual void Write(int priority, FontStyle fontStyle, string value)
		{
			pending.Produce(new StringToken(priority, value, fontStyle));
		}

		/// <summary>
		/// Writes a string to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="color">The color to write the string</param>
		/// <param name="fontStyle">The style of the font used to write the string</param>
		/// <param name="value">The string value to write</param>
		public virtual void Write(int priority, FontStyle fontStyle, Color color, string value)
		{
			pending.Produce(new StringToken(priority, value, fontStyle, color));
		}

		/// <summary>
		/// Writes a string to the text stream
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="color">The color to write the string</param>
		/// <param name="font">The font to write the string</param>
		/// <param name="value">The string value to write</param>
		public virtual void Write(int priority, Color color, Font font, string value)
		{
			pending.Produce(new StringToken(priority, value, color, font));
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
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="color">The color to write the string</param>
		/// <param name="font">The font used to write the string</param>
		/// <param name="value">The string value to write</param>
		public virtual void WriteLine(int priority, Color color, Font font, string value)
		{
			pending.Produce(new StringToken(priority, value + base.NewLine, color, font));
		}

		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="color">The color to write the string</param>
		/// <param name="value">The string value to write</param>
		public virtual void WriteLine(int priority, Color color, string value)
		{
			pending.Produce(new StringToken(priority, value + base.NewLine, color));
		}

		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="fontStyle">The style of the font used to write the string</param>
		/// <param name="value">The string value to write</param>
		public virtual void WriteLine(int priority, FontStyle fontStyle, string value)
		{
			pending.Produce(new StringToken(priority, value + base.NewLine, fontStyle));
		}

		/// <summary>
		/// Writes a string followed by a line terminator to the text stream.
		/// </summary>
		/// <param name="priority">The priority of the value to write</param>
		/// <param name="fontStyle">The style of the font used to write the string</param>
		/// <param name="color">The color to write the string</param>
		/// <param name="value">The string value to write</param>
		public virtual void WriteLine(int priority, FontStyle fontStyle, Color color, string value)
		{
			pending.Produce(new StringToken(priority, value + base.NewLine, fontStyle, color));
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

		private void AppendToken(StringToken token)
		{
			string s;
			if (appendDate)
				s = String.Format("{0} {1}",
					token.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff"),
					token.Value);
			s = token.Value;

			lock (Output)
			{
				if (((s.Length + Output.Text.Length) > Output.MaxLength) || (Output.Lines.Length > maxLines)) Output.Clear();
				
				if (!Output.IsHandleCreated || Output.Disposing || Output.IsDisposed)
					return;
				Font oldFont = Output.Font;
				Color oldColor = Output.ForeColor;
				FontStyle oldFontStyle = Output.Font.Style;
				Output.ForeColor = token.Color;
				if (token.Font != null)
					Output.Font = token.Font;
				else
					Output.Font = new Font(Output.Font, token.FontStyle);
				Output.AppendText(s);
				Output.ForeColor = oldColor;
				Output.Font = oldFont;
			}
		}

		/// <summary>
		/// When overridden in a derived class, it allows to perform actions where the thread is aborted
		/// </summary>
		protected virtual void OnThreadAborted() { }

		/// <summary>
		/// When overridden in a derived class, it allows to perform actions where the thread is started
		/// </summary>
		protected virtual void OnThreadStarted() { }

		/// <summary>
		/// When overridden in a derived class, it allows to perform actions where the thread finish its execution
		/// </summary>
		protected virtual void OnThreadStopped() { }

		/// <summary>
		/// When overridden in a derived class, opens a stream for the log file
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
			if (writerThread != null)
				return;
			writerThread = new Thread(new ThreadStart(WriterThread_Task));
			writerThread.IsBackground = true;
			writerThread.Priority = ThreadPriority.BelowNormal;
			this.running = true;
			writerThread.Start();
		}

		private void WriterThread_Task()
		{
			StringToken token;

			OnThreadStarted();
			while (this.running && !this.disposing && !this.disposed)
			{
				try
				{
					token = pending.Consume(100);
					if ((token == null) || String.IsNullOrEmpty(token.Value))
					{
						Thread.Sleep(0);
						continue;
					}
					
					WriteTokenToFile(token);
					WriteTokenToTextBox(token);
				}
				catch (ThreadInterruptedException)
				{
					running = false;
					OnThreadStopped();
					return;
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
		}

		/// <summary>
		/// When overridden in a derived class returns a string to be appended to the TextBox log
		/// </summary>
		/// <param name="token">The token object used to generate the string to append</param>
		/// <returns>A string to be appended to the TextBox log</returns>
		protected virtual void WriteTokenToTextBox(StringToken token)
		{
			try
			{
				if ((Output == null) || (token.Priority > VerbosityThreshold))
					return;
				Output.Invoke(appendTokenMethod, token);
			}
			catch { }
		}

		/// <summary>
		/// When overridden in a derived class returns a string to be appended to the log file
		/// </summary>
		/// <param name="token">The token object used to generate the string to append</param>
		/// <returns>A string to be appended to the log file</returns>
		protected virtual void WriteTokenToFile(StringToken token)
		{
			string s;
			if ((LogFile == null) || !LogFile.BaseStream.CanWrite || (token.Priority > logFileVerbosityThreshold))
				return;
			if (appendDate)
				s = String.Format("{0} {1}",
					token.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff"),
					token.Value);
			s = token.Value;
			logFile.Write(s);
		}

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
						Output.Invoke(appendTokenMethod, waitingHadle.ToString());
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
		protected class StringToken
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
			/// The font used to write the string
			/// </summary>
			private Font font;
			/// <summary>
			/// The color used to write the text
			/// </summary>
			private Color color;
			/// <summary>
			/// The color used to write the text
			/// </summary>
			private FontStyle fontStyle;

			/// <summary>
			/// Initializes a new instance of StringToken
			/// </summary>
			/// <param name="priority">The priority of the message</param>
			/// <param name="value">The message string to write to the log</param>
			public StringToken(int priority, string value) : 
				this(priority, value, FontStyle.Regular, Color.Black) { }

			/// <summary>
			/// Initializes a new instance of StringToken
			/// </summary>
			/// <param name="priority">The priority of the message</param>
			/// <param name="value">The message string to write to the log</param>
			/// <param name="color">The color which will be used to write the text in the RichTextBox Control</param>
			/// <param name="font">The font used to write the text in the RichTextBox Control</param>
			public StringToken(int priority, string value, Color color, Font font) : 
				this(priority, value, FontStyle.Regular, color)
			{
				this.value = value;
				this.ct = DateTime.Now;
				this.priority = priority;
				if ((this.font = font) != null)
					this.fontStyle = font.Style;
			}

			/// <summary>
			/// Initializes a new instance of StringToken
			/// </summary>
			/// <param name="priority">The priority of the message</param>
			/// <param name="value">The message string to write to the log</param>
			/// <param name="color">The color which will be used to write the text in the RichTextBox Control</param>
			public StringToken(int priority, string value, Color color) :
				this(priority, value, FontStyle.Regular, color) { }

			/// <summary>
			/// Initializes a new instance of StringToken
			/// </summary>
			/// <param name="priority">The priority of the message</param>
			/// <param name="value">The message string to write to the log</param>
			/// <param name="fontStyle">The style of the font used to write the text in the RichTextBox Control</param>
			public StringToken(int priority, string value, FontStyle fontStyle) : 
				this(priority, value, fontStyle, Color.Black) { }

			/// <summary>
			/// Initializes a new instance of StringToken
			/// </summary>
			/// <param name="priority">The priority of the message</param>
			/// <param name="value">The message string to write to the log</param>
			/// <param name="color">The style of the font used to write the text in the RichTextBox Control</param>
			/// <param name="fontStyle">The color which will be used to write the text in the RichTextBox Control</param>
			public StringToken(int priority, string value, FontStyle fontStyle, Color color)
			{
				this.value = value;
				this.ct = DateTime.Now;
				this.priority = priority;
				this.font = null;
				this.fontStyle = FontStyle.Regular;
				this.color = Color.Black;
			}

			/// <summary>
			/// Gets or sets the font used to write the string
			/// </summary>
			public Font Font { get { return this.font; } }

			/// <summary>
			/// The color used to write the text
			/// </summary>
			public Color Color { get { return this.color; } }

			/// <summary>
			/// The color used to write the text
			/// </summary>
			public FontStyle FontStyle { get { return this.fontStyle; } }

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
			/// Implicitly converts a StringToken object to a string
			/// </summary>
			/// <param name="st">The StringToken object to convert</param>
			/// <returns>The message string to write to the log stored in the StringToken object</returns>
			public static implicit operator string(StringToken st)
			{
				return st.value;
			}

			/// <summary>
			/// Implicitly converts a string to a StringToken object with a priority of 5
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
