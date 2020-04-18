using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCollection.Generics
{
    /// <summary>
    /// Represents a first-in, first-out (FIFO) collection of instances of the same specified <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    public interface IQueue<T> : IQueue, ICollection<T>
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

        #region Conflicts

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IQueue{T}"/>
        /// </summary>
        new int Count { get; }
        
        /// <summary>
        /// Gets a value indicating whether the <see cref="IQueue{T}"/> is read-only.
        /// </summary>
        new bool IsReadOnly { get; }
        
        /// <summary>
        /// Removes all items from the <see cref="IQueue{T}"/>.
        /// <exception cref="NotSupportedException">The <see cref="IQueue{T}"/> is read-only.</exception>
        /// </summary>
        new void Clear();

        #endregion
        
        #region ICollection

        int ICollection<T>.Count => Count;
        int System.Collections.ICollection.Count => Count;

        bool ICollection.IsReadOnly => IsReadOnly;
        bool ICollection<T>.IsReadOnly => IsReadOnly;

        void ICollection<T>.Clear()
        {
            while (TryDequeue(out _)) { }
        }

        void ICollection<T>.Add(T item) 
            => Enqueue(item);
        
        bool ICollection<T>.Remove(T item) 
            => throw new InvalidOperationException($"To remove item, use {nameof(Dequeue)} or {nameof(TryDequeue)}");
        
        bool ICollection.Contains(object? item)
            => Contains((T) item!);

        void ICollection.Clear() => Clear();

        #endregion

        #region Queue

        void IQueue.Enqueue(object? item)
            => Enqueue((T) item!);
        
        bool IQueue.TryPeek(out object? item)
        {
            if (TryPeek(out var value))
            {
                item = value;
                return true;
            }

            item = null;
            return false;
        }

        bool IQueue.TryDequeue(out object? item)
        {
            if (TryDequeue(out var value))
            {
                item = value;
                return true;
            }

            item = null;
            return false;
        }

        #endregion
    }
}