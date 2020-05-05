using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCollection.Generics
{
    /// <summary>
    /// Represents a first-in, first-out (FIFO) collection of instances of the same specified <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    public interface IQueue<T> :ICollection<T>, ICloneable
    {
        /// <summary>
        /// Adds an <typeparamref name="T"/> to the end of the <see cref="IQueue{T}"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> to add to the <see cref="IQueue{T}"/>. The value can be null.</param>
        void Enqueue(T item);

        /// <summary>
        /// Returns the <typeparamref name="T"/> at the beginning of the <see cref="IQueue{T}"/> without removing it.
        /// </summary>
        /// <returns>The <typeparamref name="T"/> at the beginning of the <see cref="IQueue{T}"/>.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [return: MaybeNull]
        new T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        
        /// <summary>
        /// Try to returns the <typeparamref name="T"/> at the beginning of the <see cref="IQueue{T}"/> without removing it.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> at the beginning of the <see cref="IQueue{T}"/>.</param>
        /// <returns>true if could peek item in the <see cref="IQueue{T}"/>.; otherwise, false.</returns>
        bool TryPeek([MaybeNull]out T item);

        /// <summary>
        /// Removes and returns the <typeparamref name="T"/> at the beginning of the <see cref="IQueue{T}"/>.
        /// </summary>
        /// <returns>The <typeparamref name="T"/> that is removed from the beginning of the <see cref="IQueue{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="IQueue{T}"/> is empty.</exception>
        [return: MaybeNull]
        new T Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        /// <summary>
        /// Try to Removes and returns the <typeparamref name="T"/> at the beginning of the <see cref="IQueue{T}"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> that is removed from the beginning of the <see cref="IQueue{T}"/>.</param>
        /// <returns>true if could dequeue item in the <see cref="IQueue{T}"/>.; otherwise, false.</returns>
        bool TryDequeue([MaybeNull]out T item);
        
        #region ICollection

        void System.Collections.Generic.ICollection<T>.Clear()
        {
            while (TryDequeue(out _)) { }
        }

        void System.Collections.Generic.ICollection<T>.Add(T item) 
            => Enqueue(item);
        
        bool System.Collections.Generic.ICollection<T>.Remove(T item) 
            => throw new InvalidOperationException($"To remove item, use {nameof(Dequeue)} or {nameof(TryDequeue)}");

        #endregion
    }
}