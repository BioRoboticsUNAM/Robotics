using System;

namespace Robotics.AppLauncher
{
	/// <summary>
	/// Enumerates the types of actions a ModuleClient can perform
	/// </summary>
	public enum ActionType {
		/// <summary>
		/// Type of action that checks if a process is running.
		/// If not running then the process is launched.
		/// </summary>
		Check,
		/// <summary>
		/// Represents an action that checks if a process is running and kills it
		/// </summary>
		Kill,
		/// <summary>
		/// Represents an action that executes a process
		/// </summary>
		Run
		/*,
		/// <summary>
		/// Represents an action that sends a recognized command to the module.
		/// </summary>
		Send,
		/// <summary>
		/// Represents an action that sends a string through a socket.
		/// This action requires an IP Address, and a port
		/// </summary>
		SocketSend
		*/
	};

	/// <summary>
	/// Provides the base interface for actions.
	/// </summary>
	public interface IAction
	{

		#region Properties

		/// <summary>
		/// Gets the type of this IAction instance
		/// </summary>
		ActionType Type
		{
			get;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Executes the actions that this IAction instance represents
		/// </summary>
		/// <returns>true if the action was executed successfully. false otherwise</returns>
		bool Execute();

		#endregion

	}
}
