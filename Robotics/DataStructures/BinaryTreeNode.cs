using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.DataStructures
{
	/// <summary>
	/// Represents a node of a binary tree
	/// </summary>
	/// <typeparam name="T">IComparable data type for the node</typeparam>
	public class BinaryTreeNode<T> : IComparable<BinaryTreeNode<T>> where T : IComparable<T>
	{
		#region Variables

		/// <summary>
		/// Stores the object of the BinaryTreeNode instance
		/// </summary>
		protected T value;

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

		/// <summary>
		/// Creates a new instance of BinaryTreeNode
		/// </summary>
		/// <param name="value">Value to initialize the node with</param>
		public BinaryTreeNode(T value) : this(value, null, null){ }

		/// <summary>
		/// Creates a new instance of BinaryTreeNode
		/// </summary>
		/// <param name="value">Value to initialize the node with</param>
		/// <param name="left">The left child node</param>
		/// <param name="right">The right child node</param>
		public BinaryTreeNode(T value, BinaryTreeNode<T> left, BinaryTreeNode<T> right)
		{
			if (value == null) throw new ArgumentNullException("value");
			this.value = value;
			this.left = left;
			this.right = right;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the left child node of the BinaryTreeNode instance
		/// If this value is changed, also the previous child BinaryTreeNode instance is updated
		/// </summary>
		public virtual BinaryTreeNode<T> Left
		{
			get { return this.left; }
			set	{ this.left = value; }
		}

		/// <summary>
		/// Gets or sets the left child BinaryTreeNode of the BinaryTreeNode instance
		/// If this value is changed, also the previous child BinaryTreeNode instance is updated
		/// </summary>
		public virtual BinaryTreeNode<T> Right
		{
			get { return this.right; }
			set { this.right = value; }
		}

		/// <summary>
		/// Gets or sets the object or value of the BinaryTreeNode instance
		/// </summary>
		public virtual T Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Tells if two BinaryTreeNode nodes are equal.
		/// Two nodes are equal if stores the same value and points to the same left and right nodes
		/// </summary>
		/// <param name="obj">Object to compare with</param>
		/// <returns>true if obj is a BinaryTreeNode and both nodes are equal, false otherwise</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is BinaryTreeNode<T>)) return false;
			BinaryTreeNode<T> other = (BinaryTreeNode<T>)obj;
			return ((this.CompareTo(other) == 0) && (this.left == other.left) && (this.right == other.right));
		}

		/// <summary>
		/// Overridden. Returns the hash code for this instance.
		/// The value corresponds to the hash code of the value contained in the node.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance</returns>
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

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
			return node.value;
		}

		/// <summary>
		/// Tells if two BinaryTreeNode nodes are equal.
		/// Two nodes are equal if stores the same value
		/// </summary>
		/// <param name="a">BinaryTreeNode to compare</param>
		/// <param name="b">BinaryTreeNode to compare</param>
		/// <returns>true if the values of the nodes are equal. false otherwise</returns>
		public static bool operator ==(BinaryTreeNode<T> a, BinaryTreeNode<T> b)
		{
			return (a.value.CompareTo(b.value) == 0);
		}

		/// <summary>
		/// Tells if two BinaryTreeNode nodes are different.
		/// Two nodes are different if stores different values or points to different left or right nodes
		/// </summary>
		/// <param name="a">BinaryTreeNode to compare</param>
		/// <param name="b">BinaryTreeNode to compare</param>
		/// <returns>true if nodes are not equal. false otherwise</returns>
		public static bool operator !=(BinaryTreeNode<T> a, BinaryTreeNode<T> b)
		{
			return (a.value.CompareTo(b.value) != 0);
		}

		/// <summary>
		/// Tells if one BinaryTreeNode is greater than another BinaryTreeNode.
		/// Comparison is made using the value of the nodes
		/// </summary>
		/// <param name="a">BinaryTreeNode to compare</param>
		/// <param name="b">BinaryTreeNode to compare</param>
		/// <returns>true if BinaryTreeNode a is greater than BinaryTreeNode b</returns>
		public static bool operator >(BinaryTreeNode<T> a, BinaryTreeNode<T> b)
		{
			return (a.CompareTo(b) > 0);
		}

		/// <summary>
		/// Tells if one BinaryTreeNode is greater than or equal to another BinaryTreeNode.
		/// Comparison is made using the value of the nodes
		/// </summary>
		/// <param name="a">BinaryTreeNode to compare</param>
		/// <param name="b">BinaryTreeNode to compare</param>
		/// <returns>true if BinaryTreeNode a is greater than or equal to BinaryTreeNode b</returns>
		public static bool operator >=(BinaryTreeNode<T> a, BinaryTreeNode<T> b)
		{
			return (a.CompareTo(b) >= 0);
		}

		/// <summary>
		/// Tells if one BinaryTreeNode is less than another BinaryTreeNode.
		/// Comparison is made using the value of the nodes
		/// </summary>
		/// <param name="a">BinaryTreeNode to compare</param>
		/// <param name="b">BinaryTreeNode to compare</param>
		/// <returns>true if BinaryTreeNode a is less than BinaryTreeNode b</returns>
		public static bool operator <(BinaryTreeNode<T> a, BinaryTreeNode<T> b)
		{
			return (a.CompareTo(b) < 0);
		}

		/// <summary>
		/// Tells if one BinaryTreeNode is less than or equal to another BinaryTreeNode.
		/// Comparison is made using the value of the nodes
		/// </summary>
		/// <param name="a">BinaryTreeNode to compare</param>
		/// <param name="b">BinaryTreeNode to compare</param>
		/// <returns>true if BinaryTreeNode a is less than or equal to BinaryTreeNode b</returns>
		public static bool operator <=(BinaryTreeNode<T> a, BinaryTreeNode<T> b)
		{
			return (a.CompareTo(b) <= 0);
		}

		#endregion


	}
}
