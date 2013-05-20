using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.DataStructures
{
	/// <summary>
	/// Implements a Binary Search Tree
	/// </summary>
	internal class BinarySearchTree<T> : IEnumerable<BinarySearchTreeNode<T>>, ICollection<T> where T : IComparable<T>
	{
		
		#region Variables

		/// <summary>
		/// Root node of the Binary Search Tree
		/// </summary>
		protected BinarySearchTreeNode<T> root;

		/// <summary>
		/// Indicates if the tree is balanced
		/// </summary>
		protected bool balanced;

		/// <summary>
		/// Stores the number of elements in the BinarySearchTree
		/// </summary>
		protected int count;

		/// <summary>
		/// Stores the with of the BinarySearchTree
		/// </summary>
		protected int width;

		/// <summary>
		/// Stores the height of the BinarySearchTree
		/// </summary>
		protected int height;

		/// <summary>
		/// Indicates if the structure of the tree has changed
		/// </summary>
		protected bool changed;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of BinarySearchTreeNode
		/// </summary>
		public BinarySearchTree() : this(null)
		{  }

		/// <summary>
		/// Initializes a new instance of BinarySearchTreeNode
		/// </summary>
		/// <param name="root">Root node of the Binary Search Tree</param>
		public BinarySearchTree(BinarySearchTreeNode<T> root)
		{
			balanced = false;
			changed = true;
			this.root = root;
			Analyze();
		}

		/// <summary>
		/// Initializes a new instance of BinarySearchTreeNode
		/// </summary>
		/// <param name="root">The value of the root node of the Binary Search Tree</param>
		public BinarySearchTree(T root)
		{
			this.root = new BinarySearchTreeNode<T>(root);
			balanced = false;
			changed = true;
			count = 0;
			width = 0;
			height = 0;
			Analyze();
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of elements contained in the BinarySearchTree
		/// This is a O(n) operation
		/// </summary>
		public int Count
		{
			get
			{
				if (changed) Analyze();
				return count;
			}
		}

		/// <summary>
		/// Gets a value indicating if the BinarySearchTree is balanced
		/// A Balanced Binary Tree is a tree in which the height of the two subtrees of every node never differ by more than 1.
		/// </summary>
		public bool IsBalanced
		{
			get { return balanced && changed; }
		}

		/// <summary>
		/// Gets a value indicating if the BinarySearchTree is null (root is null)
		/// </summary>
		public bool IsEmpty
		{
			get { return (this.root == null); }
		}

		/// <summary>
		/// Gets a value indicating if the BinarySearchTree is full
		/// A Full Binary Tree is a tree in which every node other than the leaves has two children.
		/// This is an O(n) operation
		/// </summary>
		public bool IsFull
		{
			get
			{
				BinarySearchTreeNode<T> node = this.root;
				Stack<BinarySearchTreeNode<T>> nodes = new Stack<BinarySearchTreeNode<T>>();
				while ((nodes.Count > 0) || (node != null))
				{
					if (node != null)
					{
						nodes.Push(node);
						node = node.Left;
					}
					else
					{
						node = nodes.Pop();
						if (((node.Left == null) && (node.Right != null)) || ((node.Left != null) && (node.Right == null)))
						{
							nodes.Clear();
							return false;
						}
						node = node.Right;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating if the BinarySearchTree is full
		/// A Perfect Binary Tree is a full binary tree in which all leaves are at the same depth or same level.
		/// </summary>
		public bool IsPerfect
		{
			get
			{
				BinarySearchTreeNode<T> node = this.root;
				Stack<BinarySearchTreeNode<T>> nodes = new Stack<BinarySearchTreeNode<T>>();
				int depth = -1;

				while ((nodes.Count > 0) || (node != null))
				{
					if (node != null)
					{
						nodes.Push(node);
						node = node.Left;
					}
					else
					{
						node = nodes.Pop();
						if (((node.Left == null) && (node.Right != null)) || ((node.Left != null) && (node.Right == null)))
						{
							nodes.Clear();
							return false;
						}
						if ((node.Left == null) && (node.Right == null))
						{
							if (depth == -1) depth = nodes.Count;
							else if (depth != nodes.Count) return false;
						}
						node = node.Right;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the BinarySearchTree is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the maximum value stored in the BinarySearchTree
		/// </summary>
		public T Maximum
		{
			get
			{
				if (this.root == null) return default(T);
				BinarySearchTreeNode<T> node = this.root;
				while (node.Right != null)
					node = node.Right;
				return node.Value;
			}
		}

		/// <summary>
		/// Gets the minimum value stored in the BinarySearchTree
		/// </summary>
		public T Minimum
		{
			get
			{
				if (this.root == null) return default(T);
				BinarySearchTreeNode<T> node = this.root;
				while (node.Left != null)
					node = node.Left;
				return node.Value;
			}
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Adds the specified item to the BinarySearchTree
		/// </summary>
		/// <param name="item">item to add</param>
		public virtual void Add(T item)
		{
			BinarySearchTreeNode<T> parent = this.root;
			BinarySearchTreeNode<T> next = this.root;
			BinarySearchTreeNode<T> newNode = new BinarySearchTreeNode<T>(item);

			while (next != null)
			{
				//if (next == newNode) return;
				if (newNode < next)
				{
					parent = next;
					next = next.Left;
				}
				else
				{
					parent = next;
					next = next.Right;
				}
			}
			
			if (parent == null)
				this.root = newNode;
			else if (newNode < parent)
				parent.Left = newNode;
			else// (parent >= value)
				parent.Right = newNode;
			changed = true;
		}

		/// <summary>
		/// Adds the specified BinarySearchTreeNode and all its childs to the BinarySearchTree
		/// The order is not preserved
		/// </summary>
		/// <param name="item">item to add</param>
		public void Add(BinarySearchTreeNode<T> item)
		{
			BinarySearchTreeNode<T>[] nodes = InOrderTraversal(item);
			for (int i = 0; i < nodes.Length; ++i)
				Add(nodes[i].Value);
		}

		/// <summary>
		/// Analyzes the BinarySearchTree and calculates the number of nodes and its height
		/// </summary>
		private void Analyze()
		{
			int count = 0;
			int height = 0;
			BinarySearchTreeNode<T> node = this.root;
			Stack<BinarySearchTreeNode<T>> nodes = new Stack<BinarySearchTreeNode<T>>();
			while ((nodes.Count > 0) || (node != null))
			{
				if (node != null)
				{
					nodes.Push(node);
					node = node.Left;
				}
				else
				{
					node = nodes.Pop();
					if (nodes.Count > height) ++height;
					++count;
					node = node.Right;
				}
			}
			this.count = count;
			this.height = height;
			changed = false;
		}

		/// <summary>
		/// Balances the BinarySearchTree
		/// </summary>
		public virtual void Balance()
		{
		}

		/// <summary>
		/// Returns a list of BinarySearchTreeNode as result of an in-order traverse of the the BinarySearchTree
		/// </summary>
		/// <returns>List of BinarySearchTreeNode</returns>
		protected BinarySearchTreeNode<T>[] InOrderTraversal()
		{
			BinarySearchTreeNode<T> node = this.root;
			Stack<BinarySearchTreeNode<T>> parentNodes = new Stack<BinarySearchTreeNode<T>>();
			List<BinarySearchTreeNode<T>> nodeList = new List<BinarySearchTreeNode<T>>();
			while ((parentNodes.Count > 0) || (node != null))
			{
				if (node != null)
				{
					parentNodes.Push(node);
					node = node.Left;
				}
				else
				{
					node = parentNodes.Pop();
					nodeList.Add(node);
					node = node.Right;
				}
			}
			return nodeList.ToArray();
		}

		/// <summary>
		/// Deletes a node from the BinarySearchTree, but not its childs
		/// </summary>
		/// <param name="value">true if node was found and deleted, false otherwise</param>
		public bool Remove(T value)
		{
			bool found = false;
			BinarySearchTreeNode<T> next = this.root;
			BinarySearchTreeNode<T> node = null;
			BinarySearchTreeNode<T> parent = null;

			#region Find node to delete
			// Find the node to delete and its parent
			while (next != null)
			{
				if (value.CompareTo(next.Value) == 0)
				{
					node = next;
					found = true;
					break;
				}
				else if (value.CompareTo(next.Value) < 0)
				{
					parent = next;
					next = next.Left;
				}
				else
				{
					parent = next;
					next = next.Right;
				}
			}
			if (!found) return false;
			#endregion

			#region Node has no childs
			// If node has not childs, just delete it
			if ((node.Left == null) && (node.Right == null))
			{
				// There is no parent, so the deleted node is the root node
				if (parent == null)
				{
					this.root = null;
					changed = true;
					return true;
				}
				// Node to delete is a left node
				else if (parent.Left == node)
				{
					parent.Left = null;
					changed = true;
					return true;
				}
				// Node to delete is a right node
				else
				{
					parent.Right = null;
					changed = true;
					return true;
				}
			}
			#endregion

			#region Node has only one child
			// Node has only one child, replace the node with it's child
			if ((node.Left == null) ^ (node.Right == null))
			{
				// Get the next node
				if (node.Left == null) next = node.Right;
				else next = node.Left;

				// There is no parent, so the deleted node is the root node
				if (parent == null)
				{
					this.root = next;
					changed = true;
					return true;
				}
				// Node to delete is a left node
				else if (parent.Left == node)
				{
					parent.Left = next;
					changed = true;
					return true;
				}
				// Node to delete is a right node
				else
				{
					parent.Right = next;
					changed = true;
					return true;
				}
			}
			#endregion

			#region Node has two childs
			// If node has two childs, delete it and add its childs
			if ((node.Left == null) && (node.Right == null))
			{
				// There is no parent, so the deleted node is the root node
				if (parent == null)
					this.root = null;
				// Node to delete is a left node
				else if (parent.Left == node)
					parent.Left = null;
				// Node to delete is a right node
				else
					parent.Right = null;
				Add(node.Left);
				Add(node.Right);
				changed = true;
				return true;
			}
			#endregion

			return false;
		}

		/// <summary>
		/// Searchs a value within the BinarySearchTree
		/// </summary>
		/// <param name="value">Value to search for</param>
		/// <returns>true if value was found, false otherwise</returns>
		public bool Search(T value)
		{
			BinarySearchTreeNode<T> next = this.root;
			while (next != null)
			{
				if (value.CompareTo(next.Value) == 0) return true;
				else if (value.CompareTo(next.Value) < 0) next = next.Left;
				else next = next.Right;
			}
			return false;
		}

		/// <summary>
		/// Searchs a value within the BinarySearchTree
		/// </summary>
		/// <param name="value">Value to search for</param>
		/// <param name="node">The BinarySearchTreeNode where the value was found, or null if value was not found.</param>
		/// <returns>true if value was found, false otherwise</returns>
		public bool Search(T value, out BinarySearchTreeNode<T> node)
		{
			BinarySearchTreeNode<T> next = this.root;
			
			node = null;
			while (next != null)
			{
				if (value.CompareTo(next.Value) == 0)
				{
					node = next;
					return true;
				}
				else if (value.CompareTo(next.Value) < 0) next = next.Left;
				else next = next.Right;
			}
			return false;
		}

		/// <summary>
		/// Returns the ordered content of the BinarySearchTree as an array
		/// </summary>
		/// <returns>The ordered content of the BinarySearchTree as an array</returns>
		public BinarySearchTreeNode<T>[] ToArray()
		{
			return InOrderTraversal();
		}

		#endregion

		#region Inherited Properties and Methodos

		#region ICollection<T> Members

		/// <summary>
		/// Removes all items from the BinarySearchTree
		/// </summary>
		public void Clear()
		{
			this.root = null;
			changed = true;
		}

		/// <summary>
		/// Determines whether the BinarySearchTree contains a specific value.
		/// This is an homonimus of the BinarySearchTree.Search method
		/// </summary>
		/// <param name="item">The object to locate in the ICollection</param>
		/// <returns>true if item is found in the ICollection; otherwise, false.</returns>
		public bool Contains(T item)
		{
			return Search(item);
		}

		/// <summary>
		/// Copies the elements of the BinarySearchTree to an Array, starting at a particular Array index
		/// The copy is performed with elements retrieved in in-order traversal
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from BinarySearchTree. The Array must have zero-based indexing</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			BinarySearchTreeNode<T>[] elements = InOrderTraversal();
			for (int i = 0; i < elements.Length; ++i)
				array[arrayIndex + i] = elements[i].Value;
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Returns an enumerator that iterates through the collection
		/// </summary>
		/// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			BinarySearchTreeNode<T>[] nodes = InOrderTraversal();
			List<T> elements = new List<T>(nodes.Length);
			for (int i = 0; i < nodes.Length; ++i)
				elements.Add(nodes[i].Value);
			return elements.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return InOrderTraversal().GetEnumerator();
		}

		#endregion

		#region IEnumerable<BinarySearchTreeNode<T>> Members

		/// <summary>
		/// Returns an enumerator that iterates through the collection
		/// </summary>
		/// <returns>A IEnumerator that can be used to iterate through the collection.</returns>
		public IEnumerator<BinarySearchTreeNode<T>> GetEnumerator()
		{
			return new List<BinarySearchTreeNode<T>>(InOrderTraversal()).GetEnumerator();
		}

		#endregion

		#endregion

		#region Static Methodos

		/// <summary>
		/// Returns a list of BinarySearchTreeNode as result of an in-order traverse of the the BinarySearchTree
		/// </summary>
		/// <param name="node">BinarySearchTreeNode to start with</param>
		/// <returns>List of BinarySearchTreeNode</returns>
		protected static BinarySearchTreeNode<T>[] InOrderTraversal(BinarySearchTreeNode<T> node)
		{
			Stack<BinarySearchTreeNode<T>> parentNodes = new Stack<BinarySearchTreeNode<T>>();
			List<BinarySearchTreeNode<T>> nodeList = new List<BinarySearchTreeNode<T>>();
			while ((parentNodes.Count > 0) || (node != null))
			{
				if (node != null)
				{
					parentNodes.Push(node);
					node = node.Left;
				}
				else
				{
					node = parentNodes.Pop();
					nodeList.Add(node);
					node = node.Right;
				}
			}
			return nodeList.ToArray();
		}

		#endregion
	}
}

