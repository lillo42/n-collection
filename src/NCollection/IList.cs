using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NCollection
{
    /// <summary>
    /// Represents a collection of objects that can be individually accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public interface IList<T> : ICollection<T>, System.Collections.Generic.IList<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        IList<T> this[Range range] { get; }
        
        /// <summary>
        /// Inserts all of the elements in the specified collection into this
        /// list at the specified position (optional operation).  Shifts the
        /// element currently at that position (if any) and any subsequent
        /// elements to the right (increases their indices).  The new elements
        /// will appear in this list in the order that they are returned by the
        /// specified collection's iterator.  The behavior of this operation is
        /// undefined if the specified collection is modified while the
        /// operation is in progress.  (Note that this will occur if the specified
        /// collection is this list, and it's nonempty.)
        /// </summary>
        /// <param name="index">index at which to insert the first element from the specified collection</param>
        /// <param name="source">The source containing elements to be added to this list</param>
        /// <returns>true if this list changed as a result of the call</returns>
        /// <exception cref="ArgumentOutOfRangeException">if the index is out of range
        /// <paramref name="index"/> is less than 0 or  <paramref name="index"/> greater than <see cref="System.Collections.Generic.ICollection{T}.Count"/>
        /// </exception>
        bool AddAll(int index, [NotNull]IEnumerable<T> source);
        
        
        /// <summary>Inserts an item to the <see cref="T:System.Collections.Generic.IList{T}" /> at the specified index.</summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList{T}" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.Generic.IList{T}" />.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IList{T}" /> is read-only.</exception>
        void Add(int index, T item);


        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList{T}" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList{T}" />.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to comparer items </param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        int IndexOf(T item, IEqualityComparer<T> comparer);
        
        /// <summary>Determines the last index of a specific item in the <see cref="T:System.Collections.Generic.IList{T}" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList{T}" />.</param>
        /// <returns>The last index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        int LastIndexOf(T item);

        /// <summary>Determines the last index of a specific item in the <see cref="T:System.Collections.Generic.IList{T}" />.</summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList{T}" />.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to comparer items </param>
        /// <returns>The last index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        int LastIndexOf(T item, IEqualityComparer<T> comparer);

        #region IList

        void System.Collections.Generic.IList<T>.Insert(int index, T item) => Add(index, item);

        #endregion
        
    }
}