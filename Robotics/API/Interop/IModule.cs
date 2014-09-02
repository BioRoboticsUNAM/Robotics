using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Robotics.API.Interop
{
	/// <summary>
	/// Represents a Module object for COM Interop
	/// </summary>
	[ComVisible(true)]
	[Guid("9809D7E6-1D86-4c34-9627-2A9392582760")]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	public interface IModule
	{

		#region Properties

		/// <summary>
		/// Gets the ConnectionManager object that the module uses to manage connections
		/// </summary>
		ConnectionManager ConnectionManager { get; }

		/// <summary>
		/// Gets the CommandManager object that the module uses to manage and execute commands
		/// </summary>
		CommandManager CommandManager { get; }

		#endregion

		#region Methodos

		/// <summary>
		/// Initializes the module
		/// </summary>
		/// <param name="moduleName">The name of the module</param>
		/// <param name="port">The connection port for tcp server</param>
		/// <returns>Zero if the object was initialized successfully, otherwise it returns the error number</returns>
		int Initialize(string moduleName, int port);

		/// <summary>
		/// Registers a method or function to work as an asynchronous command executer
		/// </summary>
		/// <param name="commandName">The name of the command that the AsyncFunctionCE will execute</param>
		/// <param name="executerMethod">The method/function that will perform the command execution</param>
		/// <param name="parametersRequired">Indicates if the method or function requires parameters to be executed</param>
		/// <returns>Zero if the method or function was registered successfully, otherwise it returns the error number</returns>
		int RegFuncForACE(string commandName, CommandExecuterMethod executerMethod, bool parametersRequired);

		/// <summary>
		/// Registers a method or function to work as a synchronous command executer
		/// </summary>
		/// <param name="commandName">The name of the command that the AsyncFunctionCE will execute</param>
		/// <param name="executerMethod">The method/function that will perform the command execution</param>
		/// <param name="parametersRequired">Indicates if the method or function requires parameters to be executed</param>
		/// <returns>Zero if the method or function was registered successfully, otherwise it returns the error number</returns>
		int RegFuncForSCE(string commandName, CommandExecuterMethod executerMethod, bool parametersRequired);

		/// <summary>
		/// Starts the module engine
		/// </summary>
		/// <returns>Zero if the method or function was started successfully, otherwise it returns the error number</returns>
		int Start();

		/// <summary>
		/// Stops the module engine
		/// </summary>
		/// <returns>Zero if the method or function was stopped successfully, otherwise it returns the error number</returns>
		int Stop();

		#endregion

	}
}
