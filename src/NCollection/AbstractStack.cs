using System;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    ///  This class provides a skeletal implementation of the <see cref="IStack{T}"/>
    /// interface, to minimize the effort required to implement this interface. 
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public abstract class AbstractStack<T> : AbstractCollection<T>, IStack<T>
    {
        
        /// <inheritdoc cref="ICollection{T}"/>
        public override bool TryAdd(T item)
        {
            return TryPush(item);
        }

        /// <inheritdoc cref="IStack{T}"/>
        public virtual void Push(T item)
        {
            if (!TryPush(item))
            {
                throw new InvalidOperationException("The stack is empty");
            }
        }

        /// <inheritdoc cref="IStack{T}"/>
        public abstract bool TryPush(T item);

        /// <inheritdoc cref="IStack{T}"/>
        public virtual T Pop()
        {
            if (TryPop(out var item))
            {
                return item!;
            }
            
            throw new InvalidOperationException("The stack is empty");
        }
        
        /// <inheritdoc cref="IStack{T}"/>
        public abstract bool TryPop([MaybeNull]out T item);
        
        /// <inheritdoc cref="IStack{T}"/>
        public virtual T Peek()
        {
            if (TryPeek(out var item))
            {
                return item!;
            }
            
            throw new InvalidOperationException("The stack is empty");
        }

        /// <inheritdoc cref="IStack{T}"/>
        public abstract bool TryPeek([MaybeNull]out T item);

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
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
                    return TryPop(out _);
                }

                return false;
            }

            if (item.Equals(peek))
            {
                return TryPop(out _);
            }
            
            return false;
        }
    }
}