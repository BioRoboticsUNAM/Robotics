using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.API
{
	/// <summary>
	/// Represents a collection of CommandExecuter objects.
	/// </summary>
	/// <remarks>pending to implement multithread sync</remarks>
	public class CommandExecuterCollection : ICollection<CommandExecuter>
	{

		#region Variables

		/// <summary>
		/// Stores the list of CommandExecuter sorted by the command name
		/// </summary>
		private SortedList<string, CommandExecuter> commandExecuterList;

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
			commandExecuterList = new SortedList<string, CommandExecuter>();
			this.commandManager = commandManager;
		}

		#endregion

		#region Events
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
				if (!CommandExecuterList.ContainsKey(commandName)) return null;
				return CommandExecuterList[commandName];
			}
		}

		/// <summary>
		/// Gets the CommandManager object to which this CommandExecuterCollection is bound to
		/// </summary>
		public CommandManager CommandManager
		{
			get { return commandManager; }
		}

		/// <summary>
		/// Gets the list of CommandExecuter sorted by the command name
		/// </summary>
		protected SortedList<string, CommandExecuter> CommandExecuterList
		{
			get { return commandExecuterList; }
		}

		#region ICollection<CommandExecuter> Members

		/// <summary>
		/// Gets the number of elements actually contained in the CommandExecuterCollection.
		/// </summary>
		public int Count
		{
			get { return CommandExecuterList.Count; }
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
			if (commandName == null) throw new ArgumentNullException();
			return CommandExecuterList.ContainsKey(commandName);
		}

		/// <summary>
		/// Copies the names of the entire CommandExecuterCollection to a compatible one-dimensional array, starting at the specified index of the target array
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the names of the elements copied from CommandExecuterCollection. The Array must have zero-based indexing</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
		public void CopyTo(string[] array, int arrayIndex)
		{
			CommandExecuterList.Keys.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified CommandExecuter from the CommandExecuterCollection
		/// </summary>
		/// <param name="commandName">The mane of the CommandExecuter to remove from the CommandExecuterCollection. The value can be a null reference (Nothing in Visual Basic) for reference types</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List</returns>
		public bool Remove(string commandName)
		{
			if (commandName == null) return false;
			if(CommandExecuterList.ContainsKey(commandName))
				CommandExecuterList[commandName].CommandManager = null;
			return CommandExecuterList.Remove(commandName);
		}

		#region ICollection<CommandExecuter> Members

		/// <summary>
		/// Adds a CommandExecuter to the CommandExecuterCollection
		/// </summary>
		/// <param name="commandExecuter">The CommandExecuter to be added to the end of the CommandExecuterCollection.</param>
		public void Add(CommandExecuter commandExecuter)
		{
			if (commandExecuter == null) throw new ArgumentNullException();
			if (CommandExecuterList.ContainsKey(commandExecuter.CommandName))
				throw new ArgumentException("The provided CommandExecuter already exists in the collection", "commandExecuter");
			commandExecuter.CommandManager = this.CommandManager;
			CommandExecuterList.Add(commandExecuter.CommandName, commandExecuter);
		}

		/// <summary>
		/// Removes all elements from the CommandExecuterCollection
		/// </summary>
		public void Clear()
		{
			CommandExecuterList.Clear();
			commandExecuterList.TrimExcess();
		}

		/// <summary>
		/// Determines whether an CommandExecuter is in the CommandExecuterCollection.
		/// </summary>
		/// <param name="item">The object to locate in the CommandExecuterCollection.</param>
		/// <returns>true if item is found in the CommandExecuterCollection; otherwise, false</returns>
		public bool Contains(CommandExecuter item)
		{
			if (item == null) throw new ArgumentNullException();
			return CommandExecuterList.ContainsValue(item);
		}

		/// <summary>
		/// Copies the entire CommandExecuterCollection to a compatible one-dimensional array, starting at the specified index of the target array
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from CommandExecuterCollection. The Array must have zero-based indexing</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
		public void CopyTo(CommandExecuter[] array, int arrayIndex)
		{
			CommandExecuterList.Values.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the specified CommandExecuter from the CommandExecuterCollection
		/// </summary>
		/// <param name="commandExecuter">The object to remove from the CommandExecuterCollection. The value can be a null reference (Nothing in Visual Basic) for reference types</param>
		/// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the List</returns>
		public bool Remove(CommandExecuter commandExecuter)
		{
			if (commandExecuter == null) return false;
			return Remove(commandExecuter.CommandName);
		}

		#endregion

		#region IEnumerable<CommandExecuter> Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection</returns>
		public IEnumerator<CommandExecuter> GetEnumerator()
		{
			return this.CommandExecuterList.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.CommandExecuterList.Values.GetEnumerator();
		}

		#endregion

		#endregion

		#region Inherited Methodos
		#endregion

		#region EventHandler Functions
		#endregion
	}
}
