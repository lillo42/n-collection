using System;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// The <see cref="IQueue{T}"/> interface represents a first-in-first-out (FIFO) of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface IQueue<T> : ICollection<T>
    {
        /// <summary>
        /// Inserts the specified element into this queue if it is possible to do
        /// so immediately without violating capacity restrictions.
        /// When using a capacity-restricted queue, this method is generally
        /// preferable to <see cref="System.Collections.Generic.ICollection{T}.Add"/>.
        /// </summary>
        /// <param name="item">the element to add</param>
        void Enqueue(T item)
        {
            if (!TryEnqueue(item))
            {
                throw new InvalidOperationException("The queue is full");
            }
        }
        
        
        /// <summary>
        /// Try to inserts the specified element into this queue if it is possible to do
        /// so immediately without violating capacity restrictions.
        /// When using a capacity-restricted queue, this method is generally
        /// preferable to <see cref="System.Collections.Generic.ICollection{T}.Add"/>
        /// </summary>
        /// <param name="item">the element to add</param>
        /// <returns><see langword="true"/> if the element was added to this queue, otherwise <see langword="false"/></returns>
        bool TryEnqueue(T item);
        
        /// <summary>
        /// Retrieves, but does not remove, the head of this queue.
        /// </summary>
        /// <returns>The head of this queue</returns>
        /// <exception cref="InvalidOperationException">if this queue is empty</exception>
        [return: MaybeNull]
        T Peek()
        {
            if (TryPeek(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The queue is empty");
        }

        /// <summary>
        /// Try to retrieves, but does not remove, the head of this queue and return <see langword="true"/>,
        /// or returns <see langword="false"/> if the queue is empty
        /// </summary>
        /// <param name="item">The item in the head of this queue</param>
        /// <returns><see langword="true"/> if could retrieves the item in head of this queue</returns>
        bool TryPeek([MaybeNull]out T item);

        /// <summary>
        /// Retrieves and remove, the head of this queue.
        /// </summary>
        /// <returns>The remove item in head of this queue</returns>
        /// <exception cref="InvalidOperationException">if this queue is empty</exception>
        [return: MaybeNull]
        T Dequeue()
        {
            if (TryDequeue(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The queue is empty");
        }

        /// <summary>
        /// Try to retrieve and remove, the head of this queue and return <see langword="true"/>,
        /// or returns <see langword="false"/> if the queue is empty
        /// </summary>
        /// <param name="item">The item in the head of this queue</param>
        /// <returns><see langword="true"/> if could retrieves and remove the item in head of this queue</returns>
        bool TryDequeue([MaybeNull]out T item);
    }
}