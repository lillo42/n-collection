using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCollection.Generics
{
    /// <summary>
    /// Represents a last-in-first-out (LIFO) non-generic collection of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
    public interface IStack<T> : IStack, ICollection<T>
    {
        /// <summary>
        /// Try to return the <typeparamref name="T"/> at the top of the <see cref="IStack{T}"/> without removing it.
        /// </summary>
        /// <param name="item">The return item.</param>
        /// <returns>true if could peek item in the <see cref="IStack{T}"/>.; otherwise, false.</returns>
        bool TryPeek([MaybeNullWhen(false)]out T item);
        
        
        /// <summary>
        /// Returns the <typeparamref name="T"/> at the top of the <see cref="IStack{T}"/> without removing it.
        /// </summary>
        /// <returns>The <typeparamref name="T"/> at the top of the <see cref="IStack{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The Stack is empty.</exception>
        [return: MaybeNull]
        new T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        /// <summary>
        /// Inserts an <typeparamref name="T"/> at the top of the <see cref="IStack{T}"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> to push onto the <see cref="IStack{T}"/>. The value can be null.</param>
        void Push(T item);

        /// <summary>
        /// Try to removes and returns the <typeparamref name="T"/> at the top of the <see cref="IStack{T}"/>.
        /// </summary>
        /// <param name="item">The <typeparamref name="T"/> removed from the top of the <see cref="IStack{T}"/>.</param>
        /// <returns>true if could remove item in the <see cref="Stack{T}"/>.; otherwise, false.</returns>
        bool TryPop([MaybeNullWhen(false)]out T item);

        /// <summary>
        /// Removes and returns the <typeparamref name="T"/> at the top of the <see cref="IStack{T}"/>.
        /// </summary>
        /// <returns>The <see cref="object"/> removed from the top of the <see cref="IStack{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="IStack{T}"/> is empty.</exception>
        [return: MaybeNull]
        new T Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        #region Conflicts

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IStack{T}"/>
        /// </summary>
        new int Count { get; }
        
        /// <summary>
        /// Gets a value indicating whether the <see cref="IStack{T}"/> is read-only.
        /// </summary>
        new bool IsReadOnly { get; }
        
        /// <summary>
        /// Removes all items from the <see cref="IStack{T}"/>.
        /// <exception cref="NotSupportedException">The <see cref="IStack{T}"/> is read-only.</exception>
        /// </summary>
        new void Clear();

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
        
        void ICollection.Clear() 
            => Clear();

        int ICollection<T>.Count => Count;

        int System.Collections.ICollection.Count => Count;
        
        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        #endregion

        #region Stack
        void IStack.Push(object? item)
            => Push((T) item!);

        bool IStack.TryPop(out object? item)
        {
            if (TryPop(out var result))
            {
                item = result;
                return true;
            }

            item = null;
            return false;
        }
        
        bool IStack.TryPeek(out object? item)
        {
            if (TryPeek(out var result))
            {
                item = result;
                return true;
            }

            item = null;
            return false;
        }

        #endregion
    }
}