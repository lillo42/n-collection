using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCollection.Generics
{
    public interface IStack<T> : ICollection<T>, ICloneable<IStack<T>>
    { 
        bool TryPeek([MaybeNullWhen(false)]out T item);
        
        [return: MaybeNull]
        T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        void Push(T item);

        void Push(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Push(item);
            }
        }
        
        bool TryPop([MaybeNullWhen(false)]out T item);

        [return: MaybeNull]
         T Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        new bool Contains(T item);

        #region ICollection

        void ICollection<T>.Clear()
        {
            while (TryPop(out _)) { }
        }
        
        void ICollection<T>.Add(T item) 
            => Push(item);

        bool ICollection<T>.Remove(T item)
            => Remove(item);
        
        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        #endregion
    }
}