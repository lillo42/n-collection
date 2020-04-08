using System;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    public interface IQueue : ICollection, ICloneable<IQueue>
    {
        void Enqueue(object? item);

        object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        
        bool TryPeek([NotNullWhen(true)]out object? item);

        object? Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        bool TryDequeue([NotNullWhen(true)]out object? item);


        void ICollection.Add(object? item) 
            => Enqueue(item);

        void ICollection.Clear()
        {
            while (TryDequeue(out _))
            {
                
            }
        }
        
        void ICollection.Remove(object? item) 
            => throw new InvalidOperationException($"To remove item, use {nameof(Dequeue)} or {nameof(TryDequeue)}");
    }
}