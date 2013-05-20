namespace Robotics
{
	#region BinaryTreeNode

	/*
	/// <summary>
	/// Represents a node of a binary tree
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BinaryTreeNode<T> : IComparable<BinaryTreeNode<T>> where T : IComparable<T>
	{
		#region Variables
		/// <summary>
		/// Stores the object of the BinaryTreeNode instance
		/// </summary>
		protected T value;
		/// <summary>
		/// Stores the parent node of the BinaryTreeNode instance
		/// </summary>
		protected BinaryTreeNode<T> parent;
		/// <summary>
		/// Stores the left child node of the BinaryTreeNode instance
		/// </summary>
		protected BinaryTreeNode<T> left;
		/// <summary>
		/// Stores the left child node of the BinaryTreeNode instance
		/// </summary>
		protected BinaryTreeNode<T> right;
		#endregion

		#region Constructors
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the object or value of the BinaryTreeNode instance
		/// </summary>
		public T Value
		{
			get { return this.value; }
			set
			{
				this.value = value;
			}
		}

		/// <summary>
		/// Gets or sets the parent BinaryTreeNode of the BinaryTreeNode instance
		/// If this value is changed, also the previous parent BinaryTreeNode instance is updated
		/// </summary>
		public BinaryTreeNode<T> Parent
		{
			get { return this.parent; }
			set
			{
				bool cl = true;
				// If the parent is the same, exit.
				if (this.parent == value) return;
				// If there is a parent node, update it.
				if (this.parent != null)
				{
					// If this is the left child node of the current parent, parent left child node to null.
					if ((this.parent.left != null) && (this.parent.left == this))
					{
						this.Parent.Left = null;
					}
					// If this is the right child node of the current parent, parent right child node to null.
					else if ((this.parent.right != null) && (this.parent.right == this))
					{
						this.Parent.Right = null;
					}
				}
				this.parent = value;
			}
		}

		/// <summary>
		/// Gets or sets the left child node of the BinaryTreeNode instance
		/// If this value is changed, also the previous child BinaryTreeNode instance is updated
		/// </summary>
		public BinaryTreeNode<T> Left
		{
			get { return this.left; }
			set
			{
				if ((this.left != null) && (value.parent != this))
					value.Parent = this;
				this.left = value;
			}
		}

		/// <summary>
		/// Gets or sets the left child BinaryTreeNode of the BinaryTreeNode instance
		/// If this value is changed, also the previous child BinaryTreeNode instance is updated
		/// </summary>
		public BinaryTreeNode<T> Right
		{
			get { return this.right; }
			set
			{
				//if (this.right == value) return;
				if ((this.right != null) && (value.parent != this))
					value.Parent = this;
				this.right = value;
			}
		}
		#endregion

		#region Methodos
		#endregion

		#region Inherited Methodos

		#region IComparable<BinaryTreeNode> Members

		/// <summary>
		/// Compares this instance to a specified BinaryTreeNode and returns an indication of their relative values
		/// </summary>
		/// <param name="other">A BinaryTreeNode to compare</param>
		/// <returns>A signed number indicating the relative values of this instance and other</returns>
		public int CompareTo(BinaryTreeNode<T> other)
		{
			if (this.value == null) return Int32.MinValue;
			if (other.value == null) return Int32.MaxValue;
			return this.value.CompareTo(other.value);
		}

		#endregion

		#endregion

		#region Operators

		/// <summary>
		/// Implicitly converts the node to it's value
		/// </summary>
		/// <param name="node">Node to convert</param>
		/// <returns>The value or object stored in the node</returns>
		public static explicit operator T(BinaryTreeNode<T> node)
		{
			return node;
		}

		#endregion


	}
	*/

	#endregion
}
