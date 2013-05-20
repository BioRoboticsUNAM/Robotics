using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API
{
	/// <summary>
	/// Encapsulates a Command/Response pair
	/// </summary>
	public class CommandResponsePair
	{
		#region Variables

		/// <summary>
		/// Asociated Command object
		/// </summary>
		private readonly Command command;

		/// <summary>
		/// Asociated Response object
		/// </summary>
		private readonly Response response;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new CommandResponsePair
		/// </summary>
		/// <param name="command">Asociated Command object</param>
		/// <param name="response">Asociated Response object</param>
		public CommandResponsePair(Command command, Response response)
		{
			this.command = command;
			this.response = response;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the asociated Command object
		/// </summary>
		public Command Command
		{
			get { return command; }
		}

		/// <summary>
		/// Gets the asociated Response object
		/// </summary>
		public Response Response
		{
			get { return response; }
		}

		#endregion

	}
}
