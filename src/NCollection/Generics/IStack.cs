using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCollection.Generics
{
    public interface IStack<T> : IStack, ICollection<T>, ICloneable<IStack<T>>
    { 
        bool TryPeek([MaybeNullWhen(false)]out T item);
        
        [return: MaybeNull]
        new T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        void Push(T item);
        
        
        bool TryPop([MaybeNullWhen(false)]out T item);

        [return: MaybeNull]
        new T Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        new bool Contains(T item);

        #region IStack

        bool IStack.TryPeek(out object? item)
        {
            item = null;
            if (TryPeek(out var result))
            {
                item = result;
                return true;
            }
            
            return false;
        }
        
        object? IStack.Peek() 
            => Peek();

        void IStack.Push(object? item) 
            => Push((T) item!);

        object? IStack.Pop() 
            => Pop();

        bool IStack.TryPop(out object? item)
        {
            item = null;
            if (TryPop(out var result))
            {
                item = result;
                return true;
            }
            
            return false;
        }

        #endregion

        #region ICollection
        
        void ICollection<T>.Clear()
        {
            while (TryPop(out _)) { }
        }
        
        void ICollection<T>.Add(T item) 
            => Push(item);

        bool ICollection<T>.Remove(T item)
            => Remove(item);

        bool ICollection.Contains(object? item)
            => Contains((T) item!);

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        #endregion
    }
}