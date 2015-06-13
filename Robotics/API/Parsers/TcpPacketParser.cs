using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Robotics.Sockets;

namespace Robotics.API.Parsers
{
	/// <summary>
	/// Asynchronously parses incoming TcpPackets extracting command and responses
	/// </summary>
	internal partial class TcpPacketParser : TcpPacketParserEngine
	{
		#region Variables

		/// <summary>
		/// Stores a reference to the ConnectionManager object which handles this parser
		/// </summary>
		private readonly ConnectionManager cnnMan;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of TcpPacketParser
		/// </summary>
		public TcpPacketParser(ConnectionManager cnnMan)
		{
			this.cnnMan = cnnMan;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating wether the ParserEngine is running.
		/// </summary>
		public override bool IsRunning { get { return this.cnnMan.IsRunning; } }

		#endregion

		#region Methods

		/// <summary>
		/// When overriden in a derived class, creates a new Task of the type required
		/// for this TcpPacketParserEngine
		/// </summary>
		/// <param name="ep">The endPoint to which the task will be bound</param>
		/// <returns>A parser task</returns>
		public override AsyncTask CreateTask(IPEndPoint ep)
		{
			return new Task(ep, cnnMan);
		}

		#endregion

		internal class Task : TcpPacketParserEngine.AsyncTask
		{
			/// <summary>
			/// Required for appending valid chars
			/// </summary>
			private readonly StringBuilder sb;

			/// <summary>
			/// The ConnectionManager object which will receive persed commands and reponses
			/// </summary>
			private readonly ConnectionManager cnnMan;

			/// <summary>
			/// Initializes a new instance of AsyncTask
			/// </summary>
			/// <param name="endpoint">The endpoint this Task will be bounded to</param>
			/// <param name="cnnMan">The ConnectionManager object which will receive persed commands and reponses</param>
			public Task(IPEndPoint endpoint, ConnectionManager cnnMan)
				: base(endpoint)
			{
				this.sb = new StringBuilder(1024);
				this.cnnMan = cnnMan;
			}

			/// <summary>
			/// Parses data into Command and Rresponse objects
			/// </summary>
			protected override void ParseNext()
			{
				int next = ReadUTF8();
				if(next > 0){
					sb.Append((char)next);
				}
				else if (sb.Length > 0)
				{
					Parse(sb.ToString());
					sb.Length = 0;
				}
			}

			private void Parse(string s)
			{
				Command command;
				Response response;

				// Responses to manage
				if (Response.TryParse(s, out response))
				{
					response.MessageSource = cnnMan;
					response.MessageSourceMetadata = this.EndPoint;
					cnnMan.OnResponseReceived(response);
					return;
				}

				// Commands to parse
				if (Command.TryParse(s, out command))
				{
					command.MessageSource = cnnMan;
					command.MessageSourceMetadata = this.EndPoint;

					cnnMan.OnCommandReceived(command);
					return;
				}
			}

		}
	}
}