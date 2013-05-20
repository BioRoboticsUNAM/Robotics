using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.Plugins
{
	/// <summary>
	/// Represents a repository that manages plugins
	/// </summary>
	/// <typeparam name="Plugin">The Interface that the plugins must implement</typeparam>
	public interface IPluginManager<Plugin> : ICollection<Plugin>
#if DisposablePlugins
		, IDisposable
#endif
		where Plugin : IPlugin
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating if the plugins has been initialized
		/// </summary>
		bool Initialized { get; }

		#endregion

		#region Events



		#endregion

		#region Indexers

		/// <summary>
		/// Gets the plugin at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the plugin to get</param>
		/// <returns>The plugin at the specified index.</returns>
		Plugin this[int index] { get; }

		/// <summary>
		/// Gets the Plugin associated with the specified plugin name
		/// </summary>
		/// <param name="pluginName">The name of the plugin to get</param>
		/// <returns>The plugin with the specified name</returns>
		Plugin this[string pluginName] { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the IPluginManager contains a specific Plugin.
		/// </summary>
		/// <param name="pluginName">The name of the plugin to locate in the IPluginManager</param>
		/// <returns>true if plugin is found in the IPluginManager; otherwise, false.</returns>
		bool Contains(string pluginName);

#if DisposablePlugins

		/// <summary>
		/// Disposes all the plugins stored in the IPluginManager
		/// </summary>
		void DisposePlugins();

#endif

		/// <summary>
		/// Initializes all the plugins stored in the IPluginManager
		/// </summary>
		void InitializePlugins();

		/// <summary>
		/// Determines the index of a specific plugin in the IPluginManager.
		/// </summary>
		/// <param name="item">The plugin to locate in the IPluginManager.</param>
		/// <returns>The index of plugin if found in the IPluginManager; otherwise, -1.</returns>
		int IndexOf(Plugin item);

		/// <summary>
		/// Determines the index of a specific plugin in the IPluginManager.
		/// </summary>
		/// <param name="pluginName">The name of the plugin to locate in the IPluginManager.</param>
		/// <returns>The index of the plugin if found in the IPluginManager; otherwise, -1.</returns>
		int IndexOf(string pluginName);

		/// <summary>
		/// Loads all the available plugins from the Dll files located in the Plugins directory
		/// </summary>
		void LoadPlugins();
		
		/// <summary>
		/// Loads all the available plugins from the Dll files located in the specified directory
		/// </summary>
		/// <param name="pluginDirectoryPath">The path to the directory where the Dll files which contains plugins are</param>
		void LoadPlugins(string pluginDirectoryPath);

		/// <summary>
		/// Loads all the available plugins from the Dll files located in the specified directory
		/// </summary>
		/// <param name="pluginDirectoryPath">The path to the directory where the files which contains plugins are</param>
		/// <param name="pluginExtension">The extension of the plugin files</param>
		void LoadPlugins(string pluginDirectoryPath, string pluginExtension);

		/// <summary>
		/// Removes a specific plugin from the IPluginManager.
		/// </summary>
		/// <param name="pluginName">The name of the plugin to remove from the IPluginManager.</param>
		/// <returns>true if plugin was successfully removed from the IPluginManager; otherwise, false.
		/// This method also returns false if plugin is not found in the IPluginManager.</returns>
		bool Remove(string pluginName);

		/// <summary>
		/// Removes the plugin at the specified index
		/// </summary>
		/// <param name="index">The zero-based index of the plugin to remove.</param>
		void RemoveAt(int index);

		/// <summary>
		/// Starts all plugins stored in the PluginManager
		/// </summary>
		void StartPlugins();

		/// <summary>
		/// Stops all plugins stored in the PluginManager
		/// </summary>
		void StopPlugins();

		#endregion
	}
}
