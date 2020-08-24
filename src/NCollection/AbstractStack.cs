using System;

namespace NCollection
{
    public abstract class AbstractStack<T> : AbstractCollection<T>, IStack<T>
    {
        /// <inheritdoc cref="ICollection{T}"/>
        public override void Add(T item)
        {
            Push(item);
        }

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
                return item;
            }
            
            throw new InvalidOperationException("The stack is empty");
        }
        
        /// <inheritdoc cref="IStack{T}"/>
        public abstract bool TryPop(out T item);
        
        /// <inheritdoc cref="IStack{T}"/>
        public virtual T Peek()
        {
            if (TryPeek(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The stack is empty");
        }

        /// <inheritdoc cref="IStack{T}"/>
        public abstract bool TryPeek(out T item);

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override bool Remove(T item)
        {
            if (TryPeek(out var peek))
            {
                return false;
            }

            if (item == null)
            {
                if (peek == null)
                {
                    Pop();
                    return true;
                }

                return false;
            }

            if (item.Equals(peek))
            {
                Pop();
                return true;
            }

            return false;
        }
    }
}