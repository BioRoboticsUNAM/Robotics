// #define DisposablePlugins

using System;

namespace Robotics.Plugins
{

	/// <summary>
	/// Represents a Plugin
	/// </summary>
	public interface IPlugin : Robotics.IService
#if DisposablePlugins
		: IDisposable
#endif
	{
		/// <summary>
		/// Gets the name of the Plugin
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a value indicaing if the plugin has been initialized
		/// </summary>
		bool Initialized { get; }

		/// <summary>
		/// Initializes the plugin
		/// </summary>
		void Initialize();
	}
}