using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Robotics.Plugins
{
	/// <summary>
	/// Serves as base class to manage plugins
	/// </summary>
	/// <typeparam name="Plugin">The Interface that the plugins must implement</typeparam>
	public class PluginManager<Plugin> : IPluginManager<Plugin>
		where Plugin : IPlugin
	{
		#region Variables

		/// <summary>
		/// Object used to synchronize the acces to load and initialization methods in the class
		/// </summary>
		protected readonly object lockObject;

		/// <summary>
		/// Stores the list of plugins sorted by plugin name
		/// </summary>
		protected readonly SortedList<string, Plugin> plugins;

		/// <summary>
		/// This method is used to synchronize read and write over the plugins collection
		/// </summary>
		protected readonly ReaderWriterLock rwPluginsLock;

		/// <summary>
		/// The name of the Plugin interface implemented used to locate compatible plugins
		/// </summary>
		protected readonly string pluginTypeName;

		/// <summary>
		/// Indicates if the plugins has been initialized
		/// </summary>
		private bool pluginsInitialized;

		/// <summary>
		/// Represents the StartPlugins method and it is used to perform asynchronous calls to it.
		/// </summary>
		private VoidEventHandler startPluginsCallback;

		/// <summary>
		/// Represents the StopPlugins method and it is used to perform asynchronous calls to it.
		/// </summary>
		private VoidEventHandler stopPluginsCallback;

#if DisposablePlugins

		/// <summary>
		/// Track whether Dispose has been called.
		/// </summary>
		private bool disposed = false;
		
		/// <summary>
		/// Indicates if the plugins has been disposed
		/// </summary>
		private bool pluginsDisposed;

#endif

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of PluginManager
		/// </summary>
		public PluginManager()
		{
			this.plugins = new SortedList<string, Plugin>();
			this.pluginTypeName = typeof(Plugin).Name;
			this.rwPluginsLock = new ReaderWriterLock();
			this.lockObject = new Object();
			this.startPluginsCallback = new VoidEventHandler(this.StartPlugins);
			this.stopPluginsCallback = new VoidEventHandler(this.StopPlugins);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of plugins contained in the PluginManager.
		/// </summary>
		public int Count
		{
			get
			{
				int count;
				this.rwPluginsLock.AcquireReaderLock(-1);
				count = this.plugins.Count;
				this.rwPluginsLock.ReleaseReaderLock();
				return count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the PluginManager is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating if the plugins has been initialized
		/// </summary>
		public bool Initialized { get { return this.pluginsInitialized; } }

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the plugin at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the plugin to get</param>
		/// <returns>The plugin at the specified index.</returns>
		public Plugin this[int index]
		{
			get
			{
				Plugin p;
				this.rwPluginsLock.AcquireReaderLock(-1);
				if ((index < 0) || (index >= this.plugins.Count))
				{
					this.rwPluginsLock.ReleaseReaderLock();
					throw new ArgumentOutOfRangeException("Index was outside the boundaries of the collection.");
				}
				p = this.plugins.Values[index];
				this.rwPluginsLock.ReleaseReaderLock();
				return p;
			}
		}

		/// <summary>
		/// Gets the Plugin associated with the specified plugin name
		/// </summary>
		/// <param name="pluginName">The name of the plugin to get</param>
		/// <returns>The plugin with the specified name</returns>
		public Plugin this[string pluginName]
		{
			get
			{
				if (String.IsNullOrEmpty(pluginName))
					throw new ArgumentNullException();

				Plugin p;
				this.rwPluginsLock.AcquireReaderLock(-1);
				if (!this.plugins.ContainsKey(pluginName))
				{
					this.rwPluginsLock.ReleaseReaderLock();
					throw new ArgumentException("The provided plugin name does not belong to the collection.");
				}
				p = this.plugins[pluginName];
				this.rwPluginsLock.ReleaseReaderLock();
				return p;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Asynchronously begins to start all plugins stored in the PluginManager
		/// </summary>
		/// <param name="callback">Represents the method which will be called when the asynchronous operation completes</param>
		/// <param name="o">An object which contains information for the callback</param>
		/// <returns>An object that represents the status of the asynchronous operation and can be used to synchronize threads</returns>
		public IAsyncResult BeginStartPlugins(AsyncCallback callback, object o)
		{
			return startPluginsCallback.BeginInvoke(callback, o);
		}

		/// <summary>
		/// Asynchronously begins to stop all plugins stored in the PluginManager
		/// </summary>
		/// <param name="callback">Represents the method which will be called when the asynchronous operation completes</param>
		/// <param name="o">An object which contains information for the callback</param>
		/// <returns>An object that represents the status of the asynchronous operation and can be used to synchronize threads</returns>
		public IAsyncResult BeginStopPlugins(AsyncCallback callback, object o)
		{
			return stopPluginsCallback.BeginInvoke(callback, o);
		}

		/// <summary>
		/// Determines whether the PluginManager contains a specific Plugin.
		/// </summary>
		/// <param name="pluginName">The name of the plugin to locate in the PluginManager</param>
		/// <returns>true if plugin is found in the PluginManager; otherwise, false.</returns>
		public bool Contains(string pluginName)
		{
			bool result;
			this.rwPluginsLock.AcquireReaderLock(-1);
			result = this.plugins.ContainsKey(pluginName);
			this.rwPluginsLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Wait for the asynchronous call to BeginStartPlugins to complete
		/// </summary>
		/// <param name="result">The object that represents the status of the asynchronous operation</param>
		public void EndStartPlugins(IAsyncResult result)
		{
			startPluginsCallback.EndInvoke(result);
		}

		/// <summary>
		/// Wait for the asynchronous call to BeginStopPlugins to complete
		/// </summary>
		/// <param name="result">The object that represents the status of the asynchronous operation</param>
		public void EndStopPlugins(IAsyncResult result)
		{
			stopPluginsCallback.EndInvoke(result);
		}

#if DisposablePlugins

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			// Do not make this method virtual. 
			// A derived class should not be able to override this method.

			Dispose(true);
			// This object will be cleaned up by the Dispose method. 
			// Therefore, you should call GC.SupressFinalize to 
			// take this object off the finalization queue 
			// and prevent finalization code for this object 
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// If disposing equals true, the method has been called directly 
		/// or indirectly by a user's code. Managed and unmanaged resources 
		/// can be disposed.
		/// If disposing equals false, the method has been called by the 
		/// runtime from inside the finalizer and you should not reference 
		/// other objects. Only unmanaged resources can be disposed.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			// Check to see if Dispose has already been called. 
			if (this.disposed)
				return;
			// If disposing equals true, dispose all managed 
			// and unmanaged resources. 
			if (disposing && !pluginsDisposed)
			{
				foreach (IPlugin plugin in this.plugins.Values)
					// Dispose managed resources.
					plugin.Dispose();
				pluginsDisposed = true;
			}

			// Call the appropriate methods to clean up 
			// unmanaged resources here. 
			// If disposing is false, 
			// only the following code is executed.

			// Note disposing has been done.
			disposed = true;
		}
		
		/// <summary>
		/// Disposes all the plugins stored in the PluginManager
		/// </summary>
		public virtual void DisposePlugins()
		{
			lock (lockObject)
			{
				foreach (Plugin p in this.plugins.Values)
				{
					try
					{
						p.Dispose();
					}
					catch { }
				}
				this.pluginsDisposed = true;
			}
		}

#endif

		/// <summary>
		/// Determines the index of a specific plugin in the PluginManager.
		/// </summary>
		/// <param name="item">The plugin to locate in the PluginManager.</param>
		/// <returns>The index of plugin if found in the PluginManager; otherwise, -1.</returns>
		public int IndexOf(Plugin item)
		{
			int index;
			this.rwPluginsLock.AcquireReaderLock(-1);
			index = this.plugins.IndexOfValue(item);
			this.rwPluginsLock.ReleaseReaderLock();
			return index;
		}

		/// <summary>
		/// Determines the index of a specific plugin in the PluginManager.
		/// </summary>
		/// <param name="pluginName">The name of the plugin to locate in the PluginManager.</param>
		/// <returns>The index of the plugin if found in the PluginManager; otherwise, -1.</returns>
		public int IndexOf(string pluginName)
		{
			int index;
			this.rwPluginsLock.AcquireReaderLock(-1);
			index = this.plugins.IndexOfKey(pluginName);
			this.rwPluginsLock.ReleaseReaderLock();
			return index;
		}

		/// <summary>
		/// Initializes all the plugins stored in the PluginManager
		/// </summary>
		public virtual void InitializePlugins()
		{
			lock (lockObject)
			{
				this.rwPluginsLock.AcquireReaderLock(-1);
				foreach (Plugin p in this.plugins.Values)
				{
					try
					{
						if (!p.Initialized)
							p.Initialize();
					}
					catch { }
				}
				this.rwPluginsLock.ReleaseReaderLock();
				this.pluginsInitialized = true;
			}
		}

		/// <summary>
		/// Loads all the available plugins from the Dll files located in the Plugins directory
		/// </summary>
		public void LoadPlugins()
		{
			LoadPlugins(Directory.GetCurrentDirectory() + "\\Plugins", "*.dll");
		}

		/// <summary>
		/// Loads all the available plugins from the Dll files located in the specified directory
		/// </summary>
		/// <param name="pluginDirectoryPath">The path to the directory where the Dll files which contains plugins are</param>
		public void LoadPlugins(string pluginDirectoryPath)
		{
			LoadPlugins(pluginDirectoryPath, "*.dll");
		}

		/// <summary>
		/// Loads all the available plugins from the Dll files located in the specified directory
		/// </summary>
		/// <param name="pluginDirectoryPath">The path to the directory where the files which contains plugins are</param>
		/// <param name="pluginExtension">The extension of the plugin files</param>
		public void LoadPlugins(string pluginDirectoryPath, string pluginExtension)
		{
			string[] files;
			DllInfo dllInfo;

			lock (lockObject)
			{
				if (!pluginExtension.StartsWith("*."))
					pluginExtension = "*." + pluginExtension;
				files = Directory.GetFiles(pluginDirectoryPath, pluginExtension);

				for (int i = 0; i < files.Length; ++i)
				{
					try
					{
						Assembly assembly = Assembly.LoadFrom(files[i]);
						dllInfo = DllInfo.GetDllInfo(files[i]);
						if (IntPtr.Size != dllInfo.PointerSize)
							continue;
						this.rwPluginsLock.AcquireWriterLock(-1);
						if (dllInfo.IsManagedAssembly)
							LoadPluginsFromManagedDll(dllInfo);
						else
							LoadPluginsFromUnmanagedDll(dllInfo);
						this.rwPluginsLock.ReleaseWriterLock();
					}
					//catch (BadImageFormatException)
					//{
					//    plugin = LoadUnmanagedPlugin(files[i]);
					//    if(plugin != null)
					//        plugins.Add(plugin);
					//}
					catch { }
				}
			}
		}

		/// <summary>
		/// Loads plugins from a managed Dll file
		/// </summary>
		/// <param name="dll">A DllInfo object which contains information about the file which contains the plugins to load</param>
		protected virtual void LoadPluginsFromManagedDll(DllInfo dll)
		{
			Assembly assembly;
			Type[] types;
			Plugin instance;

			assembly = Assembly.LoadFrom(dll.FilePath);
			if (assembly.ManifestModule.Name == "Robotics.dll")
				return;
			types = assembly.GetTypes();
			
			foreach (Type type in types)
			{
				if (type.GetInterface(pluginTypeName) == null)
					continue;

				try
				{
					instance = (Plugin)Activator.CreateInstance(type);

					if (String.IsNullOrEmpty(instance.Name) || plugins.ContainsKey(instance.Name))
						continue;
					this.plugins.Add(instance.Name, instance);
					this.pluginsInitialized = false;
				}
				catch { continue; }
			}
		}

		/// <summary>
		/// When overriden in a derived class, it allows the PLuginManager to load plugins from an unmanaged Dll file.
		/// By default no unmanaged plugins are loaded.
		/// </summary>
		/// <param name="dll">A DllInfo object which contains information about the file which contains the plugins to load</param>
		protected virtual void LoadPluginsFromUnmanagedDll(DllInfo dll)
		{
		}

		/// <summary>
		/// Removes a specific plugin from the PluginManager.
		/// </summary>
		/// <param name="pluginName">The name of the plugin to remove from the PluginManager.</param>
		/// <returns>true if plugin was successfully removed from the PluginManager; otherwise, false.
		/// This method also returns false if plugin is not found in the PluginManager.</returns>
		public bool Remove(string pluginName)
		{
			bool result;
			this.rwPluginsLock.AcquireWriterLock(-1);
			result = this.plugins.Remove(pluginName);
			this.rwPluginsLock.ReleaseWriterLock();
			return result;
		}

		/// <summary>
		/// Removes the plugin at the specified index
		/// </summary>
		/// <param name="index">The zero-based index of the plugin to remove.</param>
		public void RemoveAt(int index)
		{
			this.rwPluginsLock.AcquireWriterLock(-1);
			this.plugins.RemoveAt(index);
			this.rwPluginsLock.ReleaseWriterLock();
		}

		/// <summary>
		/// Starts all plugins stored in the PluginManager
		/// </summary>
		public void StartPlugins()
		{
			lock (lockObject)
			{
				if (!this.Initialized)
					throw new Exception("Not all plugins have been initialized");
				this.rwPluginsLock.AcquireReaderLock(-1);
				foreach (IPlugin plugin in this.plugins.Values)
				{
					if (!plugin.IsRunning)
						plugin.Start();
				}
				this.rwPluginsLock.ReleaseReaderLock();
			}
		}

		/// <summary>
		/// Stops all plugins stored in the PluginManager
		/// </summary>
		public void StopPlugins()
		{
			lock (lockObject)
			{
				this.rwPluginsLock.AcquireReaderLock(-1);
				foreach (IPlugin plugin in this.plugins.Values)
				{
					if (plugin.IsRunning)
						plugin.Stop();
				}
				this.rwPluginsLock.ReleaseReaderLock();
			}
		}

		#region ICollection<Plugin> Members

		/// <summary>
		/// Adds a plugin to the PluginManager.
		/// </summary>
		/// <param name="item">The plugin to add to the PluginManager</param>
		public void Add(Plugin item)
		{
			this.rwPluginsLock.AcquireWriterLock(-1);
			if ((item == null) || String.IsNullOrEmpty(item.Name))
			{
				this.rwPluginsLock.ReleaseWriterLock();
				throw new ArgumentNullException();
			}
			if (plugins.ContainsKey(item.Name))
			{
				this.rwPluginsLock.ReleaseWriterLock();
				throw new Exception("A plugin with the same name already exists in the collection");
			}
			this.plugins.Add(item.Name, item);
			if(!item.Initialized)
				this.pluginsInitialized = false;
			this.rwPluginsLock.ReleaseWriterLock();
		}

		/// <summary>
		/// Removes all plugins from the PluginManager
		/// </summary>
		public void Clear()
		{
			this.rwPluginsLock.AcquireWriterLock(-1);
			this.plugins.Clear();
			this.rwPluginsLock.ReleaseWriterLock();
		}

		/// <summary>
		/// Determines whether the PluginManager contains a specific Plugin.
		/// </summary>
		/// <param name="item">The plugin to locate in the PluginManager</param>
		/// <returns>true if plugin is found in the PluginManager; otherwise, false.</returns>
		public bool Contains(Plugin item)
		{
			bool result;
			this.rwPluginsLock.AcquireReaderLock(-1);
			result = this.plugins.ContainsValue(item);
			this.rwPluginsLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Copies the plugins of the PluginManager to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the plugins copied from PluginManager. The Array must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(Plugin[] array, int arrayIndex)
		{
			this.rwPluginsLock.AcquireReaderLock(-1);
			this.plugins.Values.CopyTo(array, arrayIndex);
			this.rwPluginsLock.ReleaseReaderLock();
		}

		/// <summary>
		/// Removes a specific plugin from the PluginManager.
		/// </summary>
		/// <param name="item">The plugin to remove from the PluginManager.</param>
		/// <returns>true if plugin was successfully removed from the PluginManager; otherwise, false.
		/// This method also returns false if plugin is not found in the PluginManager.</returns>
		public bool Remove(Plugin item)
		{
			this.rwPluginsLock.AcquireWriterLock(-1);
			int index = plugins.IndexOfValue(item);
			if (index < 0)
			{
				this.rwPluginsLock.ReleaseWriterLock();
				return false;
			}
			this.plugins.RemoveAt(index);
			this.rwPluginsLock.ReleaseWriterLock();
			return true;
		}


		#endregion

		#region IEnumerable<Plugin> Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection
		/// </summary>
		public IEnumerator<Plugin> GetEnumerator()
		{
			return plugins.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection
		/// </summary>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return plugins.Values.GetEnumerator();
		}

		#endregion

		#endregion
	}
}
