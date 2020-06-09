using System;
using System.Collections;

namespace NCollection
{
    /// <summary>
    /// Provides the base interface for the abstraction of sets.
    /// </summary>
    public interface ISet : ICollection, ICloneable
    {
        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns><see langword="true" /> if the element is added to the set; <see langword="false" /> if the element is already in the set.</returns>
        new bool Add(object? item);

        #region ICollection

        void ICollection.Add(object? item) => Add(item);

        #endregion
    }
}