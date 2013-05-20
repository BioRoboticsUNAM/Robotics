using System;
using System.Collections.Generic;
using System.Text;

namespace Robotics.DataStructures
{
	/// <summary>
	/// Represents a node of a binary tree
	/// </summary>
	/// <typeparam name="T">IComparable data type for the node</typeparam>
	public class BinarySearchTreeNode<T> : IComparable<BinarySearchTreeNode<T>> where T : IComparable<T>
	{
		#region Variables

		/// <summary>
		/// Stores the object of the BinarySearchTreeNode instance
		/// </summary>
		protected T value;

		/// <summary>
		/// Stores the left child node of the BinarySearchTreeNode instance
		/// </summary>
		protected BinarySearchTreeNode<T> left;

		/// <summary>
		/// Stores the left child node of the BinarySearchTreeNode instance
		/// </summary>
		protected BinarySearchTreeNode<T> right;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of BinarySearchTreeNode
		/// </summary>
		/// <param name="value">Value to initialize the node with</param>
		public BinarySearchTreeNode(T value) : this(value, null, null) { }

		/// <summary>
		/// Creates a new instance of BinarySearchTreeNode
		/// </summary>
		/// <param name="value">Value to initialize the node with</param>
		/// <param name="left">The left child node</param>
		/// <param name="right">The right child node</param>
		public BinarySearchTreeNode(T value, BinarySearchTreeNode<T> left, BinarySearchTreeNode<T> right)
		{
			if (value == null) throw new ArgumentNullException("value");
			this.value = value;
			this.left = left;
			this.right = right;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the left child node of the BinarySearchTreeNode instance
		/// If this value is changed, also the previous child BinarySearchTreeNode instance is updated
		/// </summary>
		public virtual BinarySearchTreeNode<T> Left
		{
			get { return this.left; }
			set { this.left = value; }
		}

		/// <summary>
		/// Gets or sets the left child BinarySearchTreeNode of the BinarySearchTreeNode instance
		/// If this value is changed, also the previous child BinarySearchTreeNode instance is updated
		/// </summary>
		public virtual BinarySearchTreeNode<T> Right
		{
			get { return this.right; }
			set { this.right = value; }
		}

		/// <summary>
		/// Gets or sets the object or value of the BinarySearchTreeNode instance
		/// </summary>
		public virtual T Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Tells if two BinarySearchTreeNode nodes are equal.
		/// Two nodes are equal if stores the same value and points to the same left and right nodes
		/// </summary>
		/// <param name="obj">Object to compare with</param>
		/// <returns>true if obj is a BinarySearchTreeNode and both nodes are equal, false otherwise</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is BinarySearchTreeNode<T>)) return false;
			BinarySearchTreeNode<T> other = (BinarySearchTreeNode<T>)obj;
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

		/// <summary>
		/// Returns a string that represents the value of the current BinarySearchTreeNode. 
		/// </summary>
		/// <returns>A string that represents the value of the current BinarySearchTreeNode.</returns>
		public override string ToString()
		{
			return value.ToString();
		}

		#endregion

		#region Inherited Methodos

		#region IComparable<BinarySearchTreeNode> Members

		/// <summary>
		/// Compares this instance to a specified BinarySearchTreeNode and returns an indication of their relative values
		/// </summary>
		/// <param name="other">A BinarySearchTreeNode to compare</param>
		/// <returns>A signed number indicating the relative values of this instance and other</returns>
		public int CompareTo(BinarySearchTreeNode<T> other)
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
		public static explicit operator T(BinarySearchTreeNode<T> node)
		{
			return node.value;
		}

		/// <summary>
		/// Tells if two BinarySearchTreeNode nodes are equal.
		/// Two nodes are equal if stores the same value
		/// </summary>
		/// <param name="a">BinarySearchTreeNode to compare</param>
		/// <param name="b">BinarySearchTreeNode to compare</param>
		/// <returns>true if the values of the nodes are equal. false otherwise</returns>
		public static bool operator ==(BinarySearchTreeNode<T> a, BinarySearchTreeNode<T> b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
				return true;

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
				return false;

			return (a.value.CompareTo(b.value) == 0);
		}

		/// <summary>
		/// Tells if two BinarySearchTreeNode nodes are different.
		/// Two nodes are different if stores different values or points to different left or right nodes
		/// </summary>
		/// <param name="a">BinarySearchTreeNode to compare</param>
		/// <param name="b">BinarySearchTreeNode to compare</param>
		/// <returns>true if nodes are not equal. false otherwise</returns>
		public static bool operator !=(BinarySearchTreeNode<T> a, BinarySearchTreeNode<T> b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Tells if one BinarySearchTreeNode is greater than another BinarySearchTreeNode.
		/// Comparison is made using the value of the nodes
		/// </summary>
		/// <param name="a">BinarySearchTreeNode to compare</param>
		/// <param name="b">BinarySearchTreeNode to compare</param>
		/// <returns>true if BinarySearchTreeNode a is greater than BinarySearchTreeNode b</returns>
		public static bool operator >(BinarySearchTreeNode<T> a, BinarySearchTreeNode<T> b)
		{
			return (a.CompareTo(b) > 0);
		}

		/// <summary>
		/// Tells if one BinarySearchTreeNode is greater than or equal to another BinarySearchTreeNode.
		/// Comparison is made using the value of the nodes
		/// </summary>
		/// <param name="a">BinarySearchTreeNode to compare</param>
		/// <param name="b">BinarySearchTreeNode to compare</param>
		/// <returns>true if BinarySearchTreeNode a is greater than or equal to BinarySearchTreeNode b</returns>
		public static bool operator >=(BinarySearchTreeNode<T> a, BinarySearchTreeNode<T> b)
		{
			return (a.CompareTo(b) >= 0);
		}

		/// <summary>
		/// Tells if one BinarySearchTreeNode is less than another BinarySearchTreeNode.
		/// Comparison is made using the value of the nodes
		/// </summary>
		/// <param name="a">BinarySearchTreeNode to compare</param>
		/// <param name="b">BinarySearchTreeNode to compare</param>
		/// <returns>true if BinarySearchTreeNode a is less than BinarySearchTreeNode b</returns>
		public static bool operator <(BinarySearchTreeNode<T> a, BinarySearchTreeNode<T> b)
		{
			return (a.CompareTo(b) < 0);
		}

		/// <summary>
		/// Tells if one BinarySearchTreeNode is less than or equal to another BinarySearchTreeNode.
		/// Comparison is made using the value of the nodes
		/// </summary>
		/// <param name="a">BinarySearchTreeNode to compare</param>
		/// <param name="b">BinarySearchTreeNode to compare</param>
		/// <returns>true if BinarySearchTreeNode a is less than or equal to BinarySearchTreeNode b</returns>
		public static bool operator <=(BinarySearchTreeNode<T> a, BinarySearchTreeNode<T> b)
		{
			return (a.CompareTo(b) <= 0);
		}

		#endregion


	}
}