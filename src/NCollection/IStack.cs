using System;
using System.Collections;

namespace NCollection
{
    public interface IStack : ICollection, ICloneable<IStack>
    {
        bool TryPeek(out object? item);

        object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        void Push(object? item);

        bool TryPop(out object? item);

        object? Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        void ICollection.Clear()
        {
            while (TryPop(out _))
            {
                
            }
        }

        void ICollection.Add(object? item)
            => Push(item);

        void ICollection.Remove(object? item) 
            => throw new InvalidOperationException($"Remove only first of stack, use {nameof(Pop)} or {nameof(TryPop)}");
    }
}