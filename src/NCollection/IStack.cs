using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// Represents a last-in-first-out (LIFO) non-generic collection of <see cref="object"/>.
    /// </summary>
    public interface IStack : ICollection, ICloneable
    {
        /// <summary>
        /// Try to return the <see cref="object"/> at the top of the <see cref="IStack"/> without removing it.
        /// </summary>
        /// <param name="item">The return item.</param>
        /// <returns>true if could peek item in the <see cref="IStack"/>.; otherwise, false.</returns>
        bool TryPeek(out object? item);

        /// <summary>
        /// Returns the <see cref="object"/> at the top of the <see cref="IStack"/> without removing it.
        /// </summary>
        /// <returns>The <see cref="object"/> at the top of the <see cref="IStack"/>.</returns>
        /// <exception cref="InvalidOperationException">The Stack is empty.</exception>
        object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        /// <summary>
        /// Inserts an <see cref="object"/> at the top of the <see cref="IStack"/>.
        /// </summary>
        /// <param name="item">The <see cref="object"/> to push into the <see cref="IStack"/>. The value can be null.</param>
        void Push([MaybeNullWhen(true)]object? item);
        
        /// <summary>
        /// Try to removes and returns the object at the top of the <see cref="IStack"/>.
        /// </summary>
        /// <param name="item">The <see cref="object"/> removed from the top of the <see cref="IStack"/>.</param>
        /// <returns>true if could remove item in the <see cref="Stack"/>.; otherwise, false.</returns>
        bool TryPop([MaybeNullWhen(true)]out object? item);

        /// <summary>
        /// Removes and returns the object at the top of the <see cref="IStack"/>.
        /// </summary>
        /// <returns>The <see cref="object"/> removed from the top of the <see cref="IStack"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="IStack"/> is empty.</exception>
        object? Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        #region ICollection

        void ICollection.Clear()
        {
            while (TryPop(out _))
            {
                
            }
        }

        void ICollection.Add(object? item)
            => Push(item);

        bool ICollection.Remove(object? item) 
            => throw new InvalidOperationException($"TO remove item, use {nameof(Pop)} or {nameof(TryPop)}");

        #endregion
    }
}