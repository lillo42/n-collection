using System;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// Represents a first-in, first-out (FIFO) collection of <see cref="object"/>.
    /// </summary>
    public interface IQueue : ICollection, ICloneable
    {
        /// <summary>
        /// Adds an <see cref="object"/> to the end of the <see cref="IQueue"/>.
        /// </summary>
        /// <param name="item">The <see cref="object"/> to add to the <see cref="IQueue"/>.</param>
        void Enqueue(object? item);

        /// <summary>
        /// Returns the <see cref="object"/> at the beginning of the <see cref="IQueue"/> without removing it.
        /// </summary>
        /// <returns>The <see cref="object"/> at the beginning of the <see cref="IQueue"/>.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        
        /// <summary>
        /// Try to returns the <see cref="object"/> at the beginning of the <see cref="IQueue"/> without removing it.
        /// </summary>
        /// <param name="item">The <see cref="object"/> at the beginning of the <see cref="IQueue"/>.</param>
        /// <returns>true if could peek item in the <see cref="IQueue"/>.; otherwise, false.</returns>
        bool TryPeek([MaybeNullWhen(true)]out object? item);

        /// <summary>
        /// Removes and returns the <see cref="object"/> at the beginning of the <see cref="IQueue"/>.
        /// </summary>
        /// <returns>The <see cref="object"/> that is removed from the beginning of the <see cref="IQueue"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="IQueue"/> is empty.</exception>
        object? Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        /// <summary>
        /// Try to Removes and returns the <see cref="object"/> at the beginning of the <see cref="IQueue"/>.
        /// </summary>
        /// <param name="item">The <see cref="object"/> that is removed from the beginning of the <see cref="IQueue"/>.</param>
        /// <returns>true if could dequeue item in the <see cref="IQueue"/>.; otherwise, false.</returns>
        bool TryDequeue([MaybeNullWhen(true)]out object? item);

        #region Collection
        void ICollection.Add(object? item) 
            => Enqueue(item);

        void ICollection.Clear()
        {
            while (TryDequeue(out _)) { }
        }
        
        bool ICollection.Remove(object? item) 
            => throw new InvalidOperationException($"To remove item, use {nameof(Dequeue)} or {nameof(TryDequeue)}");
        #endregion
    }
}