using System;
using System.Collections;
using System.Collections.Generic;

namespace NCollection
{
    /// <summary>
    /// Represents a non-generic collection of objects that can be individually accessed by index.
    /// </summary>
    public interface IList : System.Collections.IList, ICollection, ICloneable
    {

        /// <summary>
        /// Sort list based on algorithm.
        /// </summary>
        /// <param name="algorithm">The <see cref="ISortAlgorithm"/>.</param>
        void Sort(ISortAlgorithm algorithm)
            => Sort(algorithm, Comparer<object>.Default);
        
        /// <summary>
        /// Sort list based on algorithm.
        /// </summary>
        /// <param name="algorithm">The <see cref="ISortAlgorithm"/>.</param>
        /// <param name="comparable">The <see cref="IComparer"/>.</param>
        void Sort(ISortAlgorithm algorithm, IComparer comparable);
        
        #region Conflict
        
        
        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection"/> is read-only.
        /// </summary>
        new bool IsReadOnly { get; }
        
        /// <summary>
        /// Adds an item to the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection"/>.</param>
        /// <exception cref="NotSupportedException">The <see cref="ICollection"/> is read-only.</exception>
        new void Add(object? item);
        
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection"/>.</param>
        /// <returns>true if item was successfully removed from the  <see cref="ICollection"/>; otherwise, false. This method also returns false if item is not found in the original <see cref="ICollection"/>.</returns>
        new bool Remove(object? item);

        /// <summary>
        /// Removes all items from the <see cref="ICollection"/>.
        /// <exception cref="NotSupportedException">The <see cref="ICollection"/> is read-only.</exception>
        /// </summary>
        new void Clear();
        
        /// <summary>
        /// Determines whether the <see cref="ICollection"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection"/>.</param>
        /// <returns>true if item is found in the <see cref="ICollection"/>.; otherwise, false.</returns>
        new bool Contains(object? item);
        #endregion

        #region IList

        bool System.Collections.IList.IsReadOnly => IsReadOnly;

        void System.Collections.IList.Remove(object value) => Remove(value);

        void System.Collections.IList.Clear() => Clear();

        bool System.Collections.IList.Contains(object value) => Contains(value);

        int System.Collections.IList.Add(object value)
        {
            Add(value);
            return Count;
        }

        #endregion

        #region ICollection

        bool ICollection.IsReadOnly => IsReadOnly;

        void ICollection.Add(object? item) => Add(item);

        bool ICollection.Remove(object? item) => Remove(item);

        void ICollection.Clear() => Clear();

        bool ICollection.Contains(object? item) => Contains(item);

        #endregion
    }
}