using System;
using System.Collections.Generic;
using System.Text;


namespace Robotics.HAL.Sensors
{
	/// <summary>
	/// Represents a collection of Recognized Speech Alternates
	/// </summary>
	public class RecognizedSpeech : IEnumerable<RecognizedSpeechAlternate>
	{
		#region Variables

		/// <summary>
		/// The list of alternates
		/// </summary>
		private List<RecognizedSpeechAlternate> alternates;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of RecognizedSpeech
		/// </summary>
		internal RecognizedSpeech()
		{
			this.alternates = new List<RecognizedSpeechAlternate>(10);
		}

		/// <summary>
		/// Initializes a new instance of RecognizedSpeech
		/// </summary>
		/// <param name="capacity">Initial capacity for alternates</param>
		internal RecognizedSpeech(int capacity)
		{
			this.alternates = new List<RecognizedSpeechAlternate>(capacity);
		}

		/// <summary>
		/// Initializes a new instance of RecognizedSpeech
		/// </summary>
		/// <param name="alternates">Collection of recognized speech alternates</param>
		public RecognizedSpeech(IEnumerable<RecognizedSpeechAlternate> alternates)
		{
			if(alternates == null)
				throw new ArgumentNullException();
			this.alternates = new List<RecognizedSpeechAlternate>(alternates);
			this.alternates.Sort();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the confidence of the alternate with highest confidence
		/// </summary>
		public float Confidence
		{
			get { return (alternates.Count > 0) ? alternates[0].Confidence : 0; }
		}

		/// <summary>
		/// Gets the number of elements contained in the Collection.
		/// </summary>
		public int Count
		{
			get { return alternates.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the Collection is read-only.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the text of the alternate with highest confidence
		/// </summary>
		public string Text
		{
			get { return (alternates.Count > 0) ? alternates[0].Text : String.Empty; }
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the RecognizedSpeechAlternate with the specified index in the collection
		/// </summary>
		/// <param name="index">The zero-based index of the element to retrieve</param>
		public RecognizedSpeechAlternate this[int index]
		{
			get { return this.alternates[index]; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Adds an item to the Collection
		/// </summary>
		/// <param name="item">The object to add to the</param>
		internal void Add(RecognizedSpeechAlternate item)
		{
			this.alternates.Add(item);
			//this.alternates.Sort();
		}

		/// <summary>
		/// Adds an item to the Collection
		/// </summary>
		/// <param name="confidence">The measure of certainty for a RecognizedSpeechAlternate</param>
		/// <param name="text">The normalized text obtained by a recognition engine</param>
		internal void Add(string text, float confidence)
		{
			this.alternates.Add(new RecognizedSpeechAlternate(text, confidence));
			//this.alternates.Sort();
		}

		/// <summary>
		/// Removes all items from the Collection
		/// </summary>
		internal void Clear()
		{
			this.alternates.Clear();
		}

		/// <summary>
		/// Determines whether the Collection contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the Collection.</param>
		/// <returns>true if item is found in the Collection otherwise, false</returns>
		public bool Contains(RecognizedSpeechAlternate item)
		{
			return this.alternates.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the Collection to an Array, starting at a particular Array index.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from Collection. The Array must have zero-based indexing</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins</param>
		public void CopyTo(RecognizedSpeechAlternate[] array, int arrayIndex)
		{
			this.alternates.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the Collection.
		/// </summary>
		/// <param name="item">The object to remove from the Collection</param>
		/// <returns>true if item was successfully removed from the Collection; otherwise, false. This method also returns false if item is not found in the original Collection.</returns>
		internal bool Remove(RecognizedSpeechAlternate item)
		{
			return this.alternates.Remove(item);
		}

		/// <summary>
		/// Sorts the internal array by confidence
		/// </summary>
		internal void Sort()
		{
			this.alternates.Sort();
		}

		#region IEnumerable<RecognizedSpeechAlternate> Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		public IEnumerator<RecognizedSpeechAlternate> GetEnumerator()
		{
			return this.alternates.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator that iterates through a collection
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.alternates.GetEnumerator();
		}

		#endregion

		#endregion
	}

	/// <summary>
	/// Represents a of Recognized Speech Alternate
	/// </summary>
	public class RecognizedSpeechAlternate : IComparable<RecognizedSpeechAlternate>
	{
		#region Variables

		/// <summary>
		/// The measure of certainty for a RecognizedSpeechAlternate
		/// </summary>
		private float confidence;

		/// <summary>
		/// The normalized text obtained by a recognition engine
		/// </summary>
		private string text;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of RecognizedSpeechAlternate
		/// </summary>
		/// <param name="confidence">The measure of certainty for a RecognizedSpeechAlternate</param>
		/// <param name="text">The normalized text obtained by a recognition engine</param>
		public RecognizedSpeechAlternate(string text, float confidence)
		{
			this.Text = text;
			this.Confidence = confidence;
		}

		#endregion

		#region Events

		#endregion

		#region Properties

		/// <summary>
		/// Returns the measure of certainty for a RecognizedSpeechAlternate
		/// </summary>
		public float Confidence
		{
			get { return this.confidence; }
			protected set
			{
				if ((value < 0) || (value > 1))
					throw new ArgumentOutOfRangeException("Value must be between 0 and 1");
				this.confidence = value;
			}
		}

		/// <summary>
		/// Returns the normalized text obtained by a recognition engine
		/// </summary>
		public string Text
		{
			get { return this.text; }
			protected set
			{
				if(String.IsNullOrEmpty(value))
				throw new ArgumentNullException("String cannot be null nor empty");
				this.text = value;
			}
		}

		#endregion

		#region Methodos

		#region IComparable<RecognizedSpeechAlternate> Members

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
		/// Less than zero -> This object is less than the other parameter.
		/// Zero -> This object is equal to other.
		/// Greater than zero -> This object is greater than other.
		/// </returns>
		public int CompareTo(RecognizedSpeechAlternate other)
		{
			if (other != null)
			{
				if (String.Compare(this.text, other.text, true) == 0)
					return 0;
				return other.confidence.CompareTo(this.confidence);
			}
			else
				return Int32.MinValue;
		}

		#endregion

		#endregion
	}
}
