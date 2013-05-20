using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;


namespace Robotics.API
{
	/// <summary>
	/// Encapsulates a CommandManager, ConnectionManager and other objects required
	/// to create a module for connection to Blackboard. This class is designed for 
	/// COM Interop
	/// </summary>
	[ComVisible(true)]
	[Guid("32D80501-E4C7-4740-8A5B-2B45CEE074EE")]
	[ClassInterface(ClassInterfaceType.None)]
	public class Module : IModule
	{
		#region Variables

		private ConnectionManager cnnMan;
		private CommandManager cmdMan;
		/// <summary>
		/// Flag that indicates if the module has been initialized
		/// </summary>
		private bool initialized;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of Module
		/// </summary>
		public Module()
		{
			this.cmdMan = new CommandManager();
			this.cnnMan = new ConnectionManager();
			this.initialized = false;
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the ConnectionManager object that the module uses to manage connections
		/// </summary>
		public ConnectionManager ConnectionManager
		{ get { return this.cnnMan; } }

		/// <summary>
		/// Gets the CommandManager object that the module uses to manage and execute commands
		/// </summary>
		public CommandManager CommandManager
		{ get { return this.cmdMan; } }

		#endregion

		#region Methodos

		/// <summary>
		/// Gets an error code based on an Exeption object
		/// </summary>
		/// <param name="exception">The exception to get the error code from</param>
		/// <returns>An integer that represents the exception</returns>
		private int GetErrorCode(Exception exception)
		{
			Exception ex;
			Win32Exception w32ex;

			if (exception == null)
				return -1;
			ex = exception;
			w32ex = null;
			do
			{
				w32ex = ex as Win32Exception;
			} while ((w32ex == null) && (ex != null) && ((ex = ex.InnerException) != null));
			return (w32ex != null) ? w32ex.ErrorCode : exception.Message.GetHashCode();
		}

		/// <summary>
		/// Initializes the module
		/// </summary>
		/// <param name="moduleName">The name of the module</param>
		/// <param name="port">The connection port for tcp server</param>
		/// <returns>Zero if the object was initialized successfully, otherwise it returns the error number</returns>
		public int Initialize(string moduleName, int port)
		{
			this.initialized = false;
			try
			{
				this.cnnMan.ModuleName = moduleName;
				this.cnnMan.PortIn = port;
				this.cnnMan.PortOut = port;
				this.cnnMan.CommandManager = cmdMan;
			}
			catch (Exception ex)
			{
				return GetErrorCode(ex);
			}
			this.initialized = true;
			return 0;
		}

		/// <summary>
		/// Registers a method or function to work as an asynchronous command executer
		/// </summary>
		/// <param name="commandName">The name of the command that the AsyncFunctionCE will execute</param>
		/// <param name="executerMethod">The method/function that will perform the command execution</param>
		/// <param name="parametersRequired">Indicates if the method or function requires parameters to be executed</param>
		/// <returns>Zero if the method or function was registered successfully, otherwise it returns the error number</returns>
		public int RegFuncForACE(string commandName, CommandExecuterMethod executerMethod, bool parametersRequired)
		{
			try
			{
				AsyncFunctionCE ace = new AsyncFunctionCE(commandName, executerMethod, parametersRequired);
				cmdMan.CommandExecuters.Add(ace);
			}
			catch (Exception ex)
			{
				return GetErrorCode(ex);
			}
			return 0;
		}

		/// <summary>
		/// Registers a method or function to work as a synchronous command executer
		/// </summary>
		/// <param name="commandName">The name of the command that the AsyncFunctionCE will execute</param>
		/// <param name="executerMethod">The method/function that will perform the command execution</param>
		/// <param name="parametersRequired">Indicates if the method or function requires parameters to be executed</param>
		/// <returns>Zero if the method or function was registered successfully, otherwise it returns the error number</returns>
		public int RegFuncForSCE(string commandName, CommandExecuterMethod executerMethod, bool parametersRequired)
		{
			try
			{
				SyncFunctionCE sce = new SyncFunctionCE(commandName, executerMethod, parametersRequired);
				cmdMan.CommandExecuters.Add(sce);
			}
			catch (Exception ex)
			{
				return GetErrorCode(ex);
			}
			return 0;
		}

		/// <summary>
		/// Starts the module engine
		/// </summary>
		/// <returns>Zero if the method or function was started successfully, otherwise it returns the error number</returns>
		public int Start()
		{
			if (!initialized)
				return -1;
			try
			{
				this.cnnMan.Start();
				this.cmdMan.Start();
			}
			catch (Exception ex)
			{
				return GetErrorCode(ex);
			}
				return 0;
		}

		/// <summary>
		/// Stops the module engine
		/// </summary>
		/// <returns>Zero if the method or function was stopped successfully, otherwise it returns the error number</returns>
		public int Stop()
		{
			if (!initialized)
				return -1;
			try
			{
				this.cmdMan.Stop();
				this.cnnMan.Stop();
			}
			catch (Exception ex)
			{
				return GetErrorCode(ex);
			}
			return 0;
		}

		#endregion
	}
}
