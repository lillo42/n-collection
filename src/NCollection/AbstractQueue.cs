using System;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    ///  A collection designed for holding elements prior to processing.
    /// Besides basic <see cref="ICollection{T}"/> operations, queues provide
    /// additional insertion, extraction, and inspection operations.
    /// Each of these methods exists in two forms: one throws an exception
    /// if the operation fails, the other returns a special value.  The
    /// latter form of the insert operation is designed specifically for
    /// use with capacity-restricted <see cref="IQueue{T}"/> implementations; in most
    /// implementations, insert operations cannot fail.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AbstractQueue<T> : AbstractCollection<T>, IQueue<T>
    {
        /// <inheritdoc cref="ICollection{T}"/>
        public override void Add(T item)
        {
            Enqueue(item);
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public override bool TryAdd(T item)
        {
            return TryEnqueue(item);
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public virtual void Enqueue(T item)
        {
            if (!TryEnqueue(item))
            {
                throw new InvalidOperationException("The queue is full");
            }
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public abstract bool TryEnqueue(T item);

        /// <inheritdoc cref="IQueue{T}"/>
        [return: MaybeNull]
        public virtual T Peek()
        {
            if (TryPeek(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The queue is empty");
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public abstract bool TryPeek([MaybeNull]out T item);

        /// <inheritdoc cref="IQueue{T}"/>
        [return: MaybeNull]
        public virtual T Dequeue()
        {
            if (TryDequeue(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The queue is empty");
        }
        
        /// <inheritdoc cref="IQueue{T}"/>
        public abstract bool TryDequeue([MaybeNull]out T item);

        /// <inheritdoc cref="ICollection{T}"/>
        public override void Clear()
        {
            while (TryDequeue(out _))
            {
                
            }
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public override bool Remove(T item)
        {
            if (!TryPeek(out var peek))
            {
                return false;
            }
            
            if (item == null)
            {
                if (peek == null)
                {
                    Dequeue();
                    return true;
                }

                return false;
            }

            if (item.Equals(peek))
            {
                Dequeue();
                return true;
            }

            return false;
        }
    }
}