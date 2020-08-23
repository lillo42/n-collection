using System;

namespace NCollection
{
    /// <summary>
    /// The <see cref="IStack{T}"/> interface represents a last-in-first-out
    /// (LIFO) stack of objects.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface IStack<T> : ICollection<T>
    {
        /// <summary>
        /// Pushes an item onto the top of this stack.
        /// </summary>
        /// <param name="item">The item to be pushed onto this stack.</param>
        void Push(T item);
        
        /// <summary>
        /// Try to pushes an item onto the top of this stack.
        /// </summary>
        /// <param name="item">The item to be pushed onto this stack.</param>
        /// <returns>true if could insert</returns>
        bool TryPush(T item);

        /// <summary>
        /// Retrieves and remove, the head of this stack.
        /// </summary>
        /// <returns>The remove item in tail of this stack.</returns>
        /// <exception cref="InvalidOperationException">if this stack is empty</exception>
        T Pop()
        {
            if (TryPop(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The stack is empty");
        }
        
        /// <summary>
        /// Retrieve and remove, the tail of head stack and return true,
        /// or returns false if the queue is empty
        /// </summary>
        /// <param name="item">The item in the head of this stack</param>
        /// <returns>true if could retrieves and remove the item in head of this stack</returns>
        bool TryPop(out T item);
        
        /// <summary>
        /// Retrieves, but does not remove, the head of this stack. This method
        /// differs from  <see cref="TryPeek"/> only in that it throws an exception
        /// if this queue is empty.
        /// </summary>
        /// <returns>The head of this queue</returns>
        /// <exception cref="InvalidOperationException">if this stack is empty</exception>
        T Peek()
        {
            if (TryPeek(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The queue is empty");
        }

        /// <summary>
        /// Retrieves, but does not remove, the tail of this stack and return true,
        /// or returns false if the queue is empty
        /// </summary>
        /// <param name="item">The item in the stack of this queue</param>
        /// <returns>true if could retrieves the item in stack of this queue</returns>
        bool TryPeek(out T item);
    }
}