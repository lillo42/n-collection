using System;

namespace NCollection
{
    /// <summary>
    /// A collection designed for holding elements prior to processing.
    /// Besides basic <see cref="ICollection{T}"/> operations, queues provide
    /// additional insertion, extraction, and inspection operations.
    /// Each of these methods exists in two forms: one throws an exception
     /// if the operation fails, the other returns a special value (either
    /// null or false, depending on the operation).  The
    /// latter form of the insert operation is designed specifically for
     /// use with capacity-restricted {@code Queue} implementations; in most
     /// implementations, insert operations cannot fail.
     ///
     /// <table class="striped">
     /// <caption>Summary of Queue methods</caption>
     ///  <thead>
     ///  <tr>
     ///    <td></td>
     ///    <th scope="col" style="font-weight:normal; font-style:italic">Throws exception</th>
     ///    <th scope="col" style="font-weight:normal; font-style:italic">Returns special value</th>
     ///  </tr>
     ///  </thead>
     ///  <tbody>
     ///  <tr>
     ///    <th scope="row">Insert</th>
     ///    <td><see cref="ICollection{T}.Add"/></td>
     ///    <td><see cref="TryAdd"/></td>
     ///  </tr>
     ///  <tr>
     ///    <th scope="row">Remove</th>
     ///    <td><see cref="Dequeue"/></td>
     ///    <td><see cref="TryDequeue"/></td>
     ///  </tr>
     ///  <tr>
     ///    <th scope="row">Examine</th>
     ///    <td><see cref="Peek"/></td>
     ///    <td><see cref="TryPeek"/></td>
     ///  </tr>
     ///  </tbody>
     /// </table>
     ///
     /// Queues typically, but do not necessarily, order elements in a
     /// FIFO (first-in-first-out) manner.  Among the exceptions are
     /// priority queues, which order elements according to a supplied
     /// comparator, or the elements' natural ordering, and LIFO queues (or
     /// stacks) which order the elements LIFO (last-in-first-out).
     /// Whatever the ordering used, the head of the queue is that
     /// element which would be removed by a call to <see cref="Dequeue"/> or
     /// <see cref="TryDequeue"/>.  In a FIFO queue, all new elements are inserted at
     /// the tail of the queue. Other kinds of queues may use
     /// different placement rules.  Every {@code Queue} implementation
     /// must specify its ordering properties.
     ///
     /// The <see cref="TryAdd"/> method inserts an element if possible,
     /// otherwise returning false.  This differs from the
     /// <see cref="ICollection{T}.Add"/> method, which can fail to
     /// add an element only by throwing an unchecked exception.  The
     /// <see cref="TryAdd"/> method is designed for use when failure is a normal,
     /// rather than exceptional occurrence, for example, in fixed-capacity
     /// (or &quot;bounded&quot;) queues.
     ///
     /// The <see cref="Dequeue"/> and <see cref="TryDequeue"/> methods remove and
     /// return the head of the queue.
     /// Exactly which element is removed from the queue is a
     /// function of the queue's ordering policy, which differs from
     /// implementation to implementation. The <see cref="Dequeue"/> and
     /// <see cref="TryDequeue"/> methods differ only in their behavior when the
     /// queue is empty: the <see cref="Dequeue"/> method throws an exception,
     /// while the <see cref="TryDequeue"/> method returns false.
     ///
     /// The <see cref="Peek"/> and <see cref="TryPeek"/> methods return, but do
     /// not remove, the head of the queue.
     ///
     /// The <see cref="IQueue{T}"/> interface does not define the blocking queue
     /// methods, which are common in concurrent programming.  These methods,
     /// which wait for elements to appear or for space to become available, are
     /// defined in the {@link java.util.concurrent.BlockingQueue} interface, which
     /// extends this interface.
     ///
     /// <see cref="IQueue{T}"/> implementations generally do not allow insertion
     /// of null elements, although some implementations, such as
     /// {@link LinkedList}, do not prohibit insertion of null.
     /// Even in the implementations that permit it, null should
     /// not be inserted into a <see cref="ICollection{T}"/>, as null is also
     /// used as a special return value by the <see cref="TryDequeue"/> method to
     /// indicate that the queue contains no elements.
     /// 
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface IQueue<T> : ICollection<T>
    {
        /// <summary>
        /// Inserts the specified element into this queue if it is possible to do
        /// so immediately without violating capacity restrictions.
        /// When using a capacity-restricted queue, this method is generally
        /// preferable to <see cref="ICollection{T}.Add"/>, which can fail to insert an element only
        /// by throwing an exception.
        /// </summary>
        /// <param name="item">the element to add</param>
        void Enqueue(T item)
        {
            if (!TryEnqueue(item))
            {
                throw new InvalidOperationException("The queue is full");
            }
        }
        
        
        /// <summary>
        /// Inserts the specified element into this queue if it is possible to do
        /// so immediately without violating capacity restrictions.
        /// When using a capacity-restricted queue, this method is generally
        /// preferable to <see cref="ICollection{T}.Add"/>, which can fail to insert an element only
        /// by throwing an exception.
        /// </summary>
        /// <param name="item">the element to add</param>
        /// <returns>true if the element was added to this queue, else false</returns>
        bool TryEnqueue(T item);
        
        /// <summary>
        /// Retrieves, but does not remove, the head of this queue.  This method
        /// differs from  <see cref="TryPeek"/> only in that it throws an exception
        /// if this queue is empty.
        /// </summary>
        /// <returns>The head of this queue</returns>
        /// <exception cref="InvalidOperationException">if this queue is empty</exception>
        T Peek()
        {
            if (TryPeek(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The queue is empty");
        }

        /// <summary>
        /// Retrieves, but does not remove, the head of this queue and return true,
        /// or returns false if the queue is empty
        /// </summary>
        /// <param name="item">The item in the head of this queue</param>
        /// <returns>true if could retrieves the item in head of this queue</returns>
        bool TryPeek(out T item);

        /// <summary>
        /// Retrieves and remove, the head of this queue.
        /// </summary>
        /// <returns>The remove item in head of this queue</returns>
        /// <exception cref="InvalidOperationException">if this queue is empty</exception>
        T Dequeue()
        {
            if (TryDequeue(out var item))
            {
                return item;
            }
            
            throw new InvalidOperationException("The queue is empty");
        }

        /// <summary>
        /// Retrieve and remove, the head of this queue and return true,
        /// or returns false if the queue is empty
        /// </summary>
        /// <param name="item">The item in the head of this queue</param>
        /// <returns>true if could retrieves and remove the item in head of this queue</returns>
        bool TryDequeue(out T item);
    }
}