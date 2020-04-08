using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCollection.Generics
{
    public interface IQueue<T> : ICollection<T>, ICloneable<IQueue<T>>
    {
        void Enqueue(T item);

        [return: MaybeNull]
        T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        
        bool TryPeek([MaybeNull]out T item);

        [return: MaybeNull]
        T Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        bool TryDequeue([MaybeNull]out T item);

        void ICollection<T>.Clear()
        {
            while (TryDequeue(out _)) { }
        }

        void ICollection<T>.Add(T item) 
            => Enqueue(item);
        
        bool ICollection<T>.Remove(T item) 
            => throw new InvalidOperationException($"To remove item, use {nameof(Dequeue)} or {nameof(TryDequeue)}");
    }
}