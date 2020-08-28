using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NCollection.Exceptions;

namespace NCollection
{ 
    /// <summary>
    /// Defines methods to manipulate generic collections.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface ICollection<T> : System.Collections.Generic.ICollection<T>
    {
        /// <summary>
        /// Returns <see langword="true"/>  If this collection contains no elements
        /// </summary>
        bool IsEmpty => Count == 0;

        /// <summary>
        /// Returns an array containing all of the elements in this collection.
        /// If this collection makes any guarantees as to what order its elements
        /// are returned by its iterator, this method must return the elements in
        /// the same order.
        /// </summary>
        /// <returns>An array containing all of the elements in this collection</returns>
        T[] ToArray();

        /// <summary>
        /// Try adds an item to the <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/></param>
        /// <returns><see langword="true"/> if could insert <paramref name="item"/> in <see cref="ICollection{T}"/></returns>
        bool TryAdd(T item);


        /// <summary>Determines whether the <see cref="System.Collections.Generic.ICollection{T}" /> contains a specific value.</summary>
        /// <param name="item">The object to locate in the <see cref="System.Collections.Generic.ICollection{T}" /> .</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="item" /> is found in the <see cref="System.Collections.Generic.ICollection{T}" /> ; otherwise, <see langword="false" />.</returns>
        bool Contains(T item, [NotNull]IEqualityComparer<T> comparer);

        /// <summary>
        /// Return <see langword="true"/> if this if this collection contains all of the elements in the specified collection.
        /// </summary>
        /// <param name="source">The collection to be checked for containment in this collection</param>
        /// <returns>Return true if this if this collection contains all of the elements in the specified collection.</returns>
        /// <exception cref="ArgumentNullException">If the collection is null</exception>
        /// <exception cref="NullReferenceException">if the specified collection contains one or more null elements and this collection does not permit null elements</exception>
        bool ContainsAll([NotNull] IEnumerable<T> source) => ContainsAll(source, EqualityComparer<T>.Default);

        /// <summary>
        /// Return <see langword="true"/> if this if this collection contains all of the elements in the specified collection.
        /// </summary>
        /// <param name="source">The collection to be checked for containment in this collection</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns>Return true if this if this collection contains all of the elements in the specified collection.</returns>
        /// <exception cref="ArgumentNullException">If the collection is null</exception>
        /// <exception cref="NullReferenceException">if the specified collection contains one or more null elements and this collection does not permit null elements</exception>
        bool ContainsAll([NotNull] IEnumerable<T> source, [NotNull]IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (comparer == null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            foreach (var item in source)
            {
                if (!Contains(item, comparer))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds all of the elements in the specified collection to this collection
        /// (optional operation).  The behavior of this operation is undefined if
        /// the specified collection is modified while the operation is in progress.]
        /// (This implies that the behavior of this call is undefined if the
        /// specified collection is this collection, and this collection is
        /// nonempty.)
        /// </summary>
        /// <param name="source">The collection containing elements to be added to this collection</param>
        /// <returns> if this collection changed as a result of the call</returns>
        /// <exception cref="ArgumentNullException">if the specified collection contains a null element and this collection does not permit null elements, or if the specified collection is null</exception>
        /// <exception cref="UnsupportedOperationException">If the <see cref="AddAll"/>  operation is not supported by this collection</exception>
        /// <exception cref="ArgumentException">if some property of an element of the specified collection prevents it from being added to this collection</exception>
        /// <exception cref="InvalidOperationException">if not all the elements can be added at this time due to insertion restrictions</exception>
        bool AddAll([NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            var modified = false;
            foreach (var item in source)
            {
                if (TryAdd(item))
                {
                    modified = true;
                }
            }

            return modified;
        }

        /// <summary>
        /// Removes all of this collection's elements that are also contained in the
        /// specified collection (optional operation).  After this call returns,
        /// this collection will contain no elements in common with the specified collection.
        /// </summary>
        /// <param name="source">The collection containing elements to be removed from this collection</param>
        /// <returns>true if this collection changed as a result of the call</returns>
        /// <exception cref="UnsupportedOperationException">if the <see cref="RemoveAll"/> method is not supported by this collection</exception>
        /// <exception cref="NullReferenceException">if this collection contains one or more null elements and the specified collection does not support null elements or if the specified collection is null</exception>
        bool RemoveAll([NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var modified = false;
            foreach (var item in source)
            {
                if (Remove(item))
                {
                    modified = true;
                }
            }

            return modified;
        }

        
        /// <summary>
        /// Removes all of the elements of this collection that satisfy the given
        /// predicate.  Errors or runtime exceptions thrown during iteration or by
        /// the predicate are relayed to the caller.
        /// 
        /// </summary>
        /// <param name="filter">a predicate which returns true for elements to be removed</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> if the specified filter is null</exception>
        /// <exception cref="UnsupportedOperationException">if elements cannot be removed from this collection.  Implementations may throw this exception if a matching element cannot be removed or if, in general, removal is not supported.</exception>
        bool RemoveIf(Predicate<T> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            var removed = false;
            
            foreach (var item in ToArray())
            {
                if (filter(item))
                {
                    Remove(item);
                    removed = true;
                }
            }

            return removed;
        }

        /// <summary>
        /// Retains only the elements in this collection that are contained in the
        /// specified collection (optional operation).  In other words, removes from
        /// this collection all of its elements that are not contained in the
        /// specified collection.
        /// </summary>
        /// <param name="source">The collection containing elements to be retained in this collection</param>
        /// <returns>true if this collection changed as a result of the call</returns>
        /// <exception cref="NullReferenceException">if this collection contains one or more null elements and the specified collection does not permit null elements or if the specified collection is null</exception>
        bool RetainAll([NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            var modified = false;
            foreach (var item in source)
            {
                if (!Contains(item))
                {
                    Remove(item);
                    modified = true;
                }
            }

            return modified;
        }

        #region ICollection
        bool System.Collections.Generic.ICollection<T>.Contains(T item)
        {
            return Contains(item, EqualityComparer<T>.Default);
        }

        #endregion
    }
}