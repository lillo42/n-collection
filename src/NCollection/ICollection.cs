using System;

namespace NCollection
{
    /// <summary>
    /// Defines methods to manipulate non-generic collections.
    /// </summary>
    public interface ICollection : System.Collections.ICollection
    {
        /// <summary>
        /// Gets a value indicating if the <see cref="ICollection"/> is empty.
        /// </summary>
        bool IsEmpty { get; }
        
        /// <summary>
        /// Gets a value indicating whether the <see cref="ICollection"/> is read-only.
        /// </summary>
        bool IsReadOnly { get; }
        
        /// <summary>
        /// Adds an item to the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection"/>.</param>
        /// <exception cref="NotSupportedException">The <see cref="ICollection"/> is read-only.</exception>
        void Add(object? item);
        
        /// <summary>
        /// Removes all items from the <see cref="ICollection"/>.
        /// <exception cref="NotSupportedException">The <see cref="ICollection"/> is read-only.</exception>
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Determines whether the <see cref="ICollection"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection"/>.</param>
        /// <returns>true if item is found in the <see cref="ICollection"/>.; otherwise, false.</returns>
        bool Contains(object? item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection"/>.</param>
        /// <returns>true if item was successfully removed from the  <see cref="ICollection"/>; otherwise, false. This method also returns false if item is not found in the original <see cref="ICollection"/>.</returns>
        bool Remove(object? item);
        
    }
}
