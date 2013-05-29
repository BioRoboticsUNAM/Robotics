using System;
using System.Net.Sockets;
using System.Threading;

namespace Robotics.API
{
	/// <summary>
	/// Implements basic functionality for message parsing
	/// </summary>
	/// <typeparam name="T">The data type or structure from where the messages will be parsed</typeparam>
	internal abstract class MessageParser<T> : IConnector, IMessageSource
	{
		#region Variables

		/// <summary>
		/// Stores data received trough Connection Manager
		/// </summary>
		private readonly ProducerConsumer<T> dataReceived;

		/// <summary>
		/// Represents the ParserThreadTask method
		/// </summary>
		private readonly ThreadStart dlgParserThreadTask;

		/// <summary>
		/// Thread used to parse messages
		/// </summary>
		private Thread parserThread;

		/// <summary>
		/// Indicates if main thread is running
		/// </summary>
		protected bool running;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of TcpPacketParser
		/// </summary>
		public MessageParser()
		{
			this.dataReceived = new ProducerConsumer<T>(100);
			this.dlgParserThreadTask = new ThreadStart(ParserThreadTask);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a command is received
		/// </summary>
		public event CommandReceivedEventHandler<IConnector> CommandReceived;

		/// <summary>
		/// Occurs when the connector gets connected to the message source
		/// </summary>
		public event StatusChangedEventHandler<IConnector> Connected;

		/// <summary>
		/// Occurs when the connector gets disconnected from the message source
		/// </summary>
		public event StatusChangedEventHandler<IConnector> Disconnected;

		/// <summary>
		/// Occurs when a response is received
		/// </summary>
		public event ResponseReceivedEventHandler<IConnector> ResponseReceived;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating if the connector is connected to the message source
		/// </summary>
		public abstract bool IsConnected { get; }

		/// <summary>
		/// Gets a value indicating if this instance of CommandManager has been started
		/// </summary>
		public bool IsRunning
		{
			get { return running; }
		}

		/// <summary>
		/// Returns the ModuleName property of the asociated IConnectionManager object
		/// </summary>
		public abstract string ModuleName { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Clears out all the contents of dataReceived, commandsReceived, responsesReceived and unpairedResponses Queues/Lists
		/// </summary>
		protected virtual void CleanBuffers()
		{
			dataReceived.Clear();
		}

		/// <summary>
		/// Raises the CommandReceived event
		/// </summary>
		/// <param name="command">The received command</param>
		protected virtual void OnCommandReceived(Command command)
		{
			try
			{
				if (this.CommandReceived != null)
					this.CommandReceived(this, command);
			}
			catch { }
		}

		/// <summary>
		/// Raises the Connected event
		/// </summary>
		protected virtual void OnConnected()
		{
			try
			{
				if (this.Connected != null)
					this.Connected(this);
			}
			catch { }
		}

		/// <summary>
		/// Raises the Disconnected event
		/// </summary>
		protected virtual void OnDisconnected()
		{
			try
			{
				if (this.Disconnected != null)
					this.Disconnected(this);
			}
			catch { }
		}

		/// <summary>
		/// Raises the ResponseReceived event
		/// </summary>
		/// <param name="response">the received response</param>
		protected virtual void OnResponseReceived(Response response)
		{
			try
			{
				if (this.ResponseReceived != null)
					this.ResponseReceived(this, response);
			}
			catch { }
		}

		/// <summary>
		/// When overriden in a derived class, it parses received data to convert it
		/// into a message. Be sure to call the OnCommandReceived and OnResponseReceived methods
		/// to ensure communication. This method is for use within the parser thread only.
		/// </summary>
		/// <param name="data">The received data</param>
		protected abstract void Parse(T data);

		/// <summary>
		/// Executes parsing of data received
		/// </summary>
		private void ParserThreadTask()
		{
			T data;
			running = true;
			while (running)
			{
				try
				{
					data = dataReceived.Consume(100);
					if (data != null)
						Parse(data);
				}
				catch (ThreadInterruptedException)
				{
					dataReceived.Clear();
					return;
				}
				catch (ThreadAbortException)
				{
					dataReceived.Clear();
					return;
				}
				catch
				{
					continue;
				}
			}
		}

		/// <summary>
		/// Enqueues data into the Producer-Consumer queue for the parser thread.
		/// </summary>
		/// <param name="data">The data to be enequeued</param>
		protected void ProduceData(T data)
		{
			this.dataReceived.Produce(data);
		}

		/// <summary>
		/// When overriden in a derived class, receives the response
		/// generated due to a command sent by this instance.
		/// </summary>
		/// <param name="response">The response generated for the command received</param>
		public abstract void ReceiveResponse(Response response);

		/// <summary>
		/// When overriden in a derived class, sends a command through the IConnectionManager
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if the command was sent, false otherwise</returns>
		public abstract bool Send(Command command);

		/// <summary>
		/// When overriden in a derived class, sends a response through the IConnectionManager
		/// </summary>
		/// <param name="response">Response to be sent</param>
		/// <returns>true if the response was sent, false otherwise</returns>
		public abstract bool Send(Response response);

		/// <summary>
		/// Starts the parser
		/// </summary>
		public virtual void Start()
		{
			if (this.running) return;
			CleanBuffers();
			this.parserThread = new Thread(dlgParserThreadTask);
			this.parserThread.IsBackground = true;
			this.parserThread.Start();
		}

		/// <summary>
		/// Stops the parser
		/// </summary>
		public virtual void Stop()
		{
			if (!this.running)
				return;
			this.running = false;

			if ((this.parserThread != null) && this.parserThread.IsAlive)
			{
				this.parserThread.Join(100);
				if (this.parserThread.IsAlive)
					this.parserThread.Abort();
			}
			CleanBuffers();
		}

		#endregion
	}
}
