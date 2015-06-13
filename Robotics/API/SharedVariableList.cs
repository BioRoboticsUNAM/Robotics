using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Robotics.API.Parsers;
using Robotics.API.PrimitiveSharedVariables;
using Robotics.API.MiscSharedVariables;
using Robotics.Utilities;

namespace Robotics.API
{
	/// <summary>
	/// Represents the method that will handle the SharedVariableAdded event and SharedVariableRemoved event of a SharedVariableList object
	/// </summary>
	/// <param name="List">The List which raises the event</param>
	/// <param name="variable">The SharedVariable object added to List</param>
	public delegate void SharedVariableAddedRemovedEventHandler(CommandManager.SharedVariableList List, SharedVariable variable);

	public partial class CommandManager
	{
		/// <summary>
		/// Represents a List of SharedVariable objects
		/// </summary>
		public sealed class SharedVariableList : IList<SharedVariable>
		{
			#region Variables

			/// <summary>
			/// The list of subscriptions
			/// </summary>
			private SortedList<string, SharedVariable> variables;

			/// <summary>
			/// The SharedVariable object this List is bound to
			/// </summary>
			private readonly CommandManager owner;

			/// <summary>
			/// Lock for concurrent readers and single writer
			/// </summary>
			private ReaderWriterLock rwLock;

			/// <summary>
			/// Used to synchronize the load of shared variables
			/// </summary>
			private object sharedVarLoadRequestLock;

			#endregion

			#region Constructors

			/// <summary>
			/// Initializes a new instance of SharedVariableList
			/// </summary>
			/// <param name="commandManager">The CommandManager object this List will be bound to</param>
			public SharedVariableList(CommandManager commandManager)
			{
				if (commandManager == null) throw new ArgumentNullException();
				this.owner = commandManager;
				this.variables = new SortedList<string, SharedVariable>();
				this.rwLock = new ReaderWriterLock();
				this.sharedVarLoadRequestLock = new Object();
			}

			#endregion

			#region Properties

			/// <summary>
			/// Gets the number of elements actually contained in the SharedVariableList. 
			/// </summary>
			public int Count
			{
				get { return variables.Count; }
			}

			/// <summary>
			/// Gets a value indicating whether the IList is read-only. Always returns false.
			/// </summary>
			public bool IsReadOnly
			{
				get { return false; }
			}

			/// <summary>
			/// Gets the CommandManager object this List is bound to
			/// </summary>
			public CommandManager Owner
			{
				get { return owner; }
			}

			/// <summary>
			/// Gets the element at the specified index
			/// </summary>
			/// <param name="index">The zero-based index of the element to get</param>
			/// <returns>The element at the specified index</returns>
			public SharedVariable this[int index]
			{
				get { return variables.Values[index]; }
				set
				{
					throw new NotSupportedException();
					//if (value == null)
					//	throw new ArgumentNullException();
					
					//value.CommandManager = this.owner;
					//if (variables.ContainsKey(value.Name))
					//{
					//	if (variables.IndexOfKey(value.Name) == index)
					//		variables[value.Name] = value;
					//	else
					//		throw new Exception("The same SharedVariable exists in the collection at another position");
					//}
					//else
					//{
					//	variables.Add(value.Name, value);
					//	if (SharedVariableAdded != null) SharedVariableAdded(this, value);
					//}						
				}
			}

			/// <summary>
			/// Gets the element with the specified name
			/// </summary>
			/// <param name="variableName">The name of the SharedVariable to get</param>
			/// <returns>The SharedVariable element with the specified name</returns>
			public SharedVariable this[string variableName]
			{
				get { return variables[variableName]; }
			}

			#endregion

			#region Events

			/// <summary>
			/// Occurs when a SharedVariable object is added to the SharedVariableList
			/// </summary>
			public event SharedVariableAddedRemovedEventHandler SharedVariableAdded;

			/// <summary>
			/// Occurs when a SharedVariable object is removed from the SharedVariableList
			/// </summary>
			public event SharedVariableAddedRemovedEventHandler SharedVariableRemoved;

			#endregion

			#region Methods

			/// <summary>
			/// Determines whether the SharedVariableList contains a specific value.
			/// </summary>
			/// <param name="item">The SharedVariable object to locate in the SharedVariableList.</param>
			/// <returns>true if item is found in the IList; otherwise, false</returns>
			public bool ContainsSharedVariable(SharedVariable item)
			{
				bool result;
				rwLock.AcquireReaderLock(-1);
				result = variables.ContainsValue(item);
				rwLock.ReleaseReaderLock();
				return result;
			}

			/// <summary>
			/// Fills the list with all the variables from the blackboard
			/// </summary>
			/// <returns>The number of loaded variables</returns>
			public int LoadFromBlackboard()
			{
				string message;
				return LoadFromBlackboard(1000, out message);
			}

			/// <summary>
			/// Fills the list with all the variables from the blackboard
			/// </summary>
			/// <param name="timeout">Data request timeout</param>
			/// <param name="message">When this method returns contains any error message produced</param>
			/// <returns>The number of loaded variables</returns>
			public int LoadFromBlackboard(int timeout, out string message)
			{
				//Regex rxVariableXtractor = SharedVariable.RxSharedVariableValidator;
				Command cmdListVars = new Command("list_vars", "");
				Command cmdReadVars;
				Response rspListVars;
				Response rspReadVars;
				int count;

				if (!Monitor.TryEnter(sharedVarLoadRequestLock, 100))
				{
					message = "Another load operation is being performed";
					return 0;
				}
				message = String.Empty;
				if (!Owner.SendAndWait(cmdListVars, timeout, out rspListVars))
				{
					message = "No response from blackboard while requesting variable list (timeout?)";
					Monitor.PulseAll(sharedVarLoadRequestLock);
					Monitor.Exit(sharedVarLoadRequestLock);
					return 0;
				}
				if (!rspListVars.Success || !rspListVars.HasParams)
				{
					message = "Blackboard has not variables defined";
					Monitor.PulseAll(sharedVarLoadRequestLock);
					Monitor.Exit(sharedVarLoadRequestLock);
					return 0;
				}
				cmdReadVars = new Command("read_vars", rspListVars.Parameters);
				if (!Owner.SendAndWait(cmdReadVars, timeout, out rspReadVars))
				{
					message = "No response from blackboard while requesting variable list (timeout?)";
					Monitor.PulseAll(sharedVarLoadRequestLock);
					Monitor.Exit(sharedVarLoadRequestLock);
					return 0;
				}

				count = UpdateFromBlackboard(rspReadVars);
				Monitor.PulseAll(sharedVarLoadRequestLock);
				Monitor.Exit(sharedVarLoadRequestLock);
				return count;
			}

			/// <summary>
			/// Registers an existing variable within the list
			/// </summary>
			/// <param name="type">The type of the variable to register</param>
			/// <param name="isArray">Indicates if the variable is an array</param>
			/// <param name="name">The name of the variable to register</param>
			/// <param name="data">The data of the variable to register received in the read operation</param>
			private bool RegisterVar(string type, bool isArray, string name, string data)
			{
				SharedVariable variable;

				variable = null;
				switch (type)
				{
					case "double":
						if (isArray)
							variable = new DoubleArraySharedVariable(this.owner, name, false);
						else
							variable = new DoubleSharedVariable(this.owner, name, false);
						break;

					case "HumanFace":
						if(!isArray)
							variable = new DetectedHumanFaces(this.owner, name, false);
						break;

					case "int":
						if (isArray)
							variable = new IntArraySharedVariable(this.owner, name, false);
						else
							variable = new IntSharedVariable(this.owner, name, false);
						break;

					case "LaserReadingArray":
						if (!isArray)
							variable = new LaserReadingArrayShV(this.owner, name, false);
						break;

					case "long":
						if (isArray)
							variable = new LongArraySharedVariable(this.owner, name, false);
						else
							variable = new LongSharedVariable(this.owner, name, false);
						break;

					case "Matrix":
						if (!isArray)
							variable = new MatrixSharedVariable(this.owner, name, false);
						break;

					case "RecognizedSpeech":
						if (!isArray)
							variable = new RecognizedSpeechSharedVariable(this.owner, name, false);
						break;

					case "var":
						variable = new VarSharedVariable(this.owner, name, false);
						isArray = false;
						break;

					case "Vector":
						if(!isArray)
							variable = new VectorSharedVariable(this.owner, name, false);
						break;

					case "string":
						if (!isArray)
							variable = new StringSharedVariable(this.owner, name, false);
						break;
				}

				if (variable == null)
					return false;

				rwLock.AcquireWriterLock(-1);
				if (!variables.ContainsKey(name))
					variables.Add(name, variable);
				else
				{
					rwLock.ReleaseWriterLock();
					return false;
				}
				rwLock.ReleaseWriterLock();

				Exception ex;
				variable.UpdateInfo(500, out ex);
				variable.Initialized = true;
				return variable.Update(type, isArray, -1, name, data, out ex);
			}

			/// <summary>
			/// Splits a string containing multiple shared variables into each shared variable
			/// </summary>
			/// <param name="s">String to split</param>
			/// <returns>Array of each shared variable in string format</returns>
			private string[] SplitMultipleSharedVariables(string s)
			{
				int cc;
				int start;
				int end;
				int counter;
				List<string> vars;

				cc = 0;
				vars = new List<string>(100);

				// Skip space at the beginning of the string
				Parser.SkipSpaces(s, ref cc);

				do
				{
					// Shared variables must be enclosed within braces
					if (!Scanner.ReadChar('{', s, ref cc))
						break;
					counter = 1;
					start = cc - 1;

					// Read untill next close brace
					while ((cc < s.Length) && (counter > 0))
					{
						switch (s[cc])
						{
							// Open braces increments counter
							case '{':
								++counter;
								break;

							// Closed braces decrements counter
							case '}':
								--counter;
								break;

							// Skips escaped chars
							case '\\':
								++cc;
								break;
						}
						++cc;
					}
					end = cc;
					if ((end - start) > 0)
						vars.Add(s.Substring(start, end - start));
				} while (cc < s.Length);

				return vars.ToArray();
			}

			/// <summary>
			/// Updates the list of shared variables with the missing variables from the blackboard
			/// </summary>
			/// <returns>The number of loaded variables</returns>
			internal int UpdateFromBlackboard(Response response)
			{
				string message;
				return UpdateFromBlackboard(response, out message);
			}

			/// <summary>
			/// Updates the list of shared variables with the missing variables from the blackboard
			/// </summary>
			/// <returns>The number of loaded variables</returns>
			internal int UpdateFromBlackboard(Response response, out string message)
			{
				int count;
				bool exists;
				string[] splitted;
				string variableType;
				bool isArray;
				int arrayLength;
				string variableName;
				string variableData;

				count = 0;
				message = string.Empty;
				splitted = SplitMultipleSharedVariables(response.Parameters);
				foreach (string s in splitted)
				{
					// 1. Fetch variable data
					if (!Parser.ParseSharedVariable(s, out variableType, out isArray, out arrayLength, out variableName, out variableData))
						continue;

					// 2. Check if variabl exists
					rwLock.AcquireReaderLock(-1);
					exists = this.variables.ContainsKey(variableName);
					rwLock.ReleaseReaderLock();
					if (exists)
					{
						//++existing;
						continue;
					}

					// 3. Register variable
					if (RegisterVar(variableType, isArray, variableName, variableData))
						++count;
					else
						message += "Can not register variable " + variableType + " " + variableName + "\r\n";
				}

				return count;
			}

			#region IList<SharedVariable> Members

			/// <summary>
			/// Adds an item to the SharedVariableList.
			/// </summary>
			/// <param name="item">The SharedVariable object to add to the SharedVariableList.</param>
			public void Add(SharedVariable item)
			{
				if (item == null)
					throw new ArgumentNullException();
				item.CommandManager = this.owner;

				rwLock.AcquireWriterLock(-1);
				if (!variables.ContainsKey(item.Name))
				{
					variables.Add(item.Name, item);
					rwLock.ReleaseWriterLock();
					item.Initialize();
					if (SharedVariableAdded != null) SharedVariableAdded(this, item);
				}
				else
				{
					rwLock.ReleaseWriterLock();
					throw new ArgumentException("An SharedVariable object with the same name already exists in the collection");
				}
			}

			/// <summary>
			/// Removes all items from the SharedVariableList.
			/// </summary>
			public void Clear()
			{
				rwLock.AcquireWriterLock(-1);
				if (SharedVariableRemoved != null)
				{
					SharedVariable item;

					while (variables.Count > 0)
					{
						item = variables.Values[0];
						variables.RemoveAt(0);
						SharedVariableRemoved(this, item);
					}
				}
				rwLock.ReleaseWriterLock();
			}

			/// <summary>
			/// Determines whether the SharedVariableList contains a specific value.
			/// </summary>
			/// <param name="variableName">The name of the SharedVariable object to locate in the SharedVariableList.</param>
			/// <returns>true if item is found in the IList; otherwise, false</returns>
			public bool Contains(string variableName)
			{
				bool result;
				rwLock.AcquireReaderLock(-1);
				result= variables.ContainsKey(variableName);
				rwLock.ReleaseReaderLock();
				return result;
			}

			/// <summary>
			/// Determines whether the SharedVariableList contains a specific value.
			/// </summary>
			/// <param name="item">The SharedVariable object to locate in the SharedVariableList.</param>
			/// <returns>true if item is found in the IList; otherwise, false</returns>
			public bool Contains(SharedVariable item)
			{
				if (item == null)
					return false;

				bool result;
				rwLock.AcquireReaderLock(-1);
				result= variables.ContainsKey(item.Name);
				rwLock.ReleaseReaderLock();
				return result;
			}

			/// <summary>
			/// Copies the elements of the SharedVariableList to an Array, starting at a particular Array index.
			/// </summary>
			/// <param name="array">The one-dimensional Array that is the destination of the elements copied from SharedVariableList. The Array must have zero-based indexing</param>
			/// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
			public void CopyTo(SharedVariable[] array, int arrayIndex)
			{
				rwLock.AcquireReaderLock(-1);
				variables.Values.CopyTo(array, arrayIndex);
				rwLock.ReleaseReaderLock();
			}

			/// <summary>
			/// Determines the index of a specific item in the SharedVariableList
			/// </summary>
			/// <param name="variableName">The name of the shared variable to locate in the SharedVariableList.</param>
			/// <returns>The index of item if found in the list; otherwise, -1</returns>
			public int IndexOf(string variableName)
			{
				int result;
				rwLock.AcquireReaderLock(-1);
				result = variables.IndexOfKey(variableName);
				rwLock.ReleaseReaderLock();
				return result;
			}

			/// <summary>
			/// Determines the index of a specific item in the SharedVariableList
			/// </summary>
			/// <param name="item">The object to locate in the SharedVariableList.</param>
			/// <returns>The index of item if found in the list; otherwise, -1</returns>
			public int IndexOf(SharedVariable item)
			{
				int result;
				rwLock.AcquireReaderLock(-1);
				result = variables.IndexOfValue(item);
				rwLock.ReleaseReaderLock();
				return result;
			}

			/// <summary>
			/// Inserts an item to the SharedVariableList at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index at which item should be inserted</param>
			/// <param name="item">The SharedVariable object to insert into the SharedVariableList</param>
			public void Insert(int index, SharedVariable item)
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Removes the item at the specified index
			/// </summary>
			/// <param name="index">The zero-based index of the item to remove.</param>
			public void RemoveAt(int index)
			{
				rwLock.AcquireWriterLock(-1);
				variables.RemoveAt(index);
				rwLock.ReleaseWriterLock();
			}

			/// <summary>
			/// Removes the SharedVariable object from the SharedVariableList.
			/// </summary>
			/// <param name="variableName">The name of the shared variable to remove from the SharedVariableList.</param>
			/// <returns>true if item was successfully removed from the SharedVariableList; otherwise, false. This method also returns false if item is not found in the original SharedVariableList</returns>
			public bool Remove(string variableName)
			{
				SharedVariable item;
				rwLock.AcquireReaderLock(-1);
				if (!variables.ContainsKey(variableName))
				{
					rwLock.ReleaseReaderLock();
					return false;
				}

				rwLock.UpgradeToWriterLock(-1);
				item = variables[variableName];
				variables.Remove(variableName);
				rwLock.ReleaseWriterLock();
				if (SharedVariableAdded != null)
					SharedVariableRemoved(this, item);
				return true;
			}

			/// <summary>
			/// Removes the SharedVariable object from the SharedVariableList.
			/// </summary>
			/// <param name="item">The SharedVariable object to remove from the SharedVariableList.</param>
			/// <returns>true if item was successfully removed from the SharedVariableList; otherwise, false. This method also returns false if item is not found in the original SharedVariableList</returns>
			public bool Remove(SharedVariable item)
			{
				bool result;
				rwLock.AcquireWriterLock(-1);
				result = variables.Remove(item.Name);
				rwLock.ReleaseWriterLock();
				if (SharedVariableAdded != null)
					SharedVariableRemoved(this, item);
				return result;
			}

			#endregion

			#region IEnumerable<SharedVariable> Members

			/// <summary>
			/// Returns an enumerator that iterates through the List
			/// </summary>
			/// <returns>A IEnumerator that can be used to iterate through the List</returns>
			public IEnumerator<SharedVariable> GetEnumerator()
			{
				IEnumerator<SharedVariable> enumerator;
				rwLock.AcquireReaderLock(-1);
				enumerator = variables.Values.GetEnumerator();
				rwLock.ReleaseReaderLock();
				return enumerator;
			}

			#endregion

			#region IEnumerable Members

			/// <summary>
			/// Returns an enumerator that iterates through the List
			/// </summary>
			/// <returns>A IEnumerator that can be used to iterate through the List</returns>
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				System.Collections.IEnumerator enumerator;
				rwLock.AcquireReaderLock(-1);
				enumerator= variables.Values.GetEnumerator();
				rwLock.ReleaseReaderLock();
				return enumerator;
			}

			#endregion

			#endregion
		}
	}
}