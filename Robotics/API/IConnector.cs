using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API
{
	/// <summary>
	/// Represents an object which allows asynchronous communication with other objects via message passing
	/// </summary>
	public interface IConnector : IService
	{
		#region Properties

		#endregion

		#region Events

		/// <summary>
		/// Occurs when a command is received
		/// </summary>
		event CommandReceivedEventHandler<IConnector> CommandReceived;

		/// <summary>
		/// Occurs when a response is received
		/// </summary>
		event ResponseReceivedEventHandler<IConnector> ResponseReceived;

		#endregion

		#region Methods

		/// <summary>
		/// Sends a Command
		/// </summary>
		/// <param name="command">Command to be sent</param>
		/// <returns>true if command was sent successfully, false otherwise</returns>
		bool Send(Command command);

		/// <summary>
		/// Sends a Response
		/// </summary>
		/// <param name="response">Response to be sent</param>
		/// <returns>true if response was sent successfully, false otherwise</returns>
		bool Send(Response response);

		#endregion
	}
}
