using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Robotics.API
{
	/// <summary>
	/// Represents a collection of CommandExecuter objects.
	/// </summary>
	/// <remarks>pending to implement multithread sync</remarks>
	public class CommandExecuterCollection : ICollection<CommandExecuter>
	{

		#region Variables

		private ReaderWriterLock rwLock;

		/// <summary>
		/// Stores the list of CommandExecuter sorted by the command name
		/// </summary>
		protected readonly Dictionary<string, CommandExecuter> commandExecuters;

		/// <summary>
		/// The CommandExecuter object to which this CommandExecuterCollection is bound to
		/// </summary>
		private CommandManager commandManager;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of CommandExecuterCollection
		/// <param name="commandManager"></param>
		/// </summary>
		public CommandExecuterCollection(CommandManager commandManager)
		{
			commandExecuters = new Dictionary<string, CommandExecuter>();
			this.commandManager = commandManager;
			this.rwLock = new ReaderWriterLock();
		}

		#endregion

		#region Properties and Indexers

		/// <summary>
		/// Gets the CommandExecuter associated with the specified command name
		/// </summary>
		/// <param name="commandName">The name of the command managed by the CommandExecuter to get</param>
		/// <returns>The CommandExecuter associated with the specified command name. If the specified command name is not found returns null</returns>
		public CommandExecuter this[string commandName]
		{
			get
			{
				this.rwLock.AcquireReaderLock(-1);
				if (!commandExecuters.ContainsKey(commandName))
				{
					this.rwLock.ReleaseReaderLock();
					return null;
				}
				CommandExecuter cmdEx = commandExecuters[commandName];
				this.rwLock.ReleaseReaderLock();
				return cmdEx;
			}
		}

		/// <summary>
		/// Gets the CommandManager object to which this CommandExecuterCollection is bound to
		/// </summary>
		public CommandManager CommandManager
		{
			get { return commandManager; }
		}

		#region ICollection<CommandExecuter> Members

		/// <summary>
		/// Gets the number of elements actually contained in the CommandExecuterCollection.
		/// </summary>
		public int Count
		{
			get
			{
				this.rwLock.AcquireReaderLock(-1);
				int count = commandExecuters.Count;
				this.rwLock.ReleaseReaderLock();
				return count;
			}
		}

		/// <summary>
		/// This property always returns false
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#endregion

		#region Methodos

		/// <summary>
		/// Determines whether an CommandExecuter is in the CommandExecuterCollection.
		/// </summary>
		/// <param name="commandName">The name of the command which represents the CommandExecuter to locate in the CommandExecuterCollection.</param>
		/// <returns>true if the command asociated is found in the CommandExecuterCollection; otherwise, false</returns>
		public bool Contains(string commandName)
		{
			this.rwLock.AcquireReaderLock(-1);
			if (commandName == null)
			{
				this.rwLock.ReleaseReaderLock();
				throw new ArgumentNullException();
			}
			bool result = commandExecuters.ContainsKey(commandName);
			this.rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Copies the names of the entire CommandExecuterCollection to a compatible one-dimensional array, starting at the specified index of the target array
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the names of the elements copied from CommandExecuterCollection. The Array must have zero-based indexing</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
		public void CopyTo(string[] array, int arrayIndex)
		{
			this.rwLock.AcquireReaderLock(-1);
			commandExecuters.Keys.CopyTo(array, arrayIndex);
			this.rwLock.ReleaseReaderLock();
		}

		/// <summary>
		/// Removes the specified CommandExecuter from the CommandExecuterCollection
		/// </summary>
		/// <param name="commandName">The mane of the CommandExecuter to remove from the CommandExecuterCollection. The value can be a null reference (Nothing in Visual Basic) for reference types</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List</returns>
		public bool Remove(string commandName)
		{
			if (commandName == null)
				return false;
			this.rwLock.AcquireWriterLock(-1);
			if(commandExecuters.ContainsKey(commandName))
				commandExecuters[commandName].CommandManager = null;
			bool result = commandExecuters.Remove(commandName);
			this.rwLock.ReleaseWriterLock();
			return result;
		}

		/// <summary>
		/// Gets an array containing all the command executers
		/// </summary>
		/// <returns>An array containing all the command executers</returns>
		public CommandExecuter[] ToArray()
		{
			this.rwLock.AcquireReaderLock(-1);
			CommandExecuter[] acex = new CommandExecuter[commandExecuters.Count];
			int i = 0;
			foreach(CommandExecuter cex in commandExecuters.Values)
				acex[i++] = cex;
			this.rwLock.ReleaseReaderLock();
			return acex;
		}

		#region ICollection<CommandExecuter> Members

		/// <summary>
		/// Adds a CommandExecuter to the CommandExecuterCollection
		/// </summary>
		/// <param name="commandExecuter">The CommandExecuter to be added to the end of the CommandExecuterCollection.</param>
		public void Add(CommandExecuter commandExecuter)
		{
			if (commandExecuter == null) throw new ArgumentNullException();
			this.rwLock.AcquireWriterLock(-1);
			if (commandExecuters.ContainsKey(commandExecuter.CommandName))
			{
				this.rwLock.ReleaseWriterLock();
				throw new ArgumentException("The provided CommandExecuter already exists in the collection", "commandExecuter");
			}
			commandExecuter.CommandManager = this.CommandManager;
			commandExecuters.Add(commandExecuter.CommandName, commandExecuter);
			this.rwLock.ReleaseWriterLock();
		}

		/// <summary>
		/// Removes all elements from the CommandExecuterCollection
		/// </summary>
		public void Clear()
		{
			this.rwLock.AcquireWriterLock(-1);
			commandExecuters.Clear();
			this.rwLock.ReleaseWriterLock();
		}

		/// <summary>
		/// Determines whether an CommandExecuter is in the CommandExecuterCollection.
		/// </summary>
		/// <param name="item">The object to locate in the CommandExecuterCollection.</param>
		/// <returns>true if item is found in the CommandExecuterCollection; otherwise, false</returns>
		public bool Contains(CommandExecuter item)
		{
			if (item == null) throw new ArgumentNullException();
			this.rwLock.AcquireReaderLock(-1);
			bool result = commandExecuters.ContainsValue(item);
			this.rwLock.ReleaseReaderLock();
			return result;
		}

		/// <summary>
		/// Copies the entire CommandExecuterCollection to a compatible one-dimensional array, starting at the specified index of the target array
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from CommandExecuterCollection. The Array must have zero-based indexing</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
		public void CopyTo(CommandExecuter[] array, int arrayIndex)
		{
			this.rwLock.AcquireReaderLock(-1);
			commandExecuters.Values.CopyTo(array, arrayIndex);
			this.rwLock.ReleaseReaderLock();
		}

		/// <summary>
		/// Removes the specified CommandExecuter from the CommandExecuterCollection
		/// </summary>
		/// <param name="commandExecuter">The object to remove from the CommandExecuterCollection. The value can be a null reference (Nothing in Visual Basic) for reference types</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List</returns>
		public bool Remove(CommandExecuter commandExecuter)
		{
			if (commandExecuter == null) return false;
			this.rwLock.AcquireWriterLock(-1);
			bool result = Remove(commandExecuter.CommandName);
			this.rwLock.ReleaseWriterLock();
			return result;
		}

		#endregion

		#region IEnumerable<CommandExecuter> Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection</returns>
		public IEnumerator<CommandExecuter> GetEnumerator()
		{
			this.rwLock.AcquireReaderLock(-1);
			IEnumerator < CommandExecuter > e = this.commandExecuters.Values.GetEnumerator();
			this.rwLock.ReleaseReaderLock();
			return e;
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			this.rwLock.AcquireReaderLock(-1);
			System.Collections.IEnumerator e = this.commandExecuters.Values.GetEnumerator();
			this.rwLock.ReleaseReaderLock();
			return e;
		}

		#endregion

		#endregion
	}
}
