using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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
        /// <returns>An <see cref="T:[]"/> containing all of the elements in this collection</returns>
        T[] ToArray();

        /// <summary>
        /// Try adds an item to the <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/></param>
        /// <returns><see langword="true"/> if could insert <paramref name="item"/> in <see cref="ICollection{T}"/></returns>
        bool TryAdd(T item);
        
        /// <summary>
        /// Return <see langword="true"/> if this if this collection contains all of the elements in the specified collection.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to be checked for containment in this collection</param>
        /// <returns>Return <see langword="true"/> if this if this collection contains all of the elements in the specified collection.</returns>
        /// <exception cref="ArgumentNullException">If the <paramref name="source"/> is <see langword="null"/></exception>
        bool ContainsAll([NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var item in source)
            {
                if (!Contains(item))
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
        /// <param name="source">The <see cref="IEnumerable{T}"/> containing elements to be added to this collection</param>
        /// <returns> <see langword="true"/> if this collection changed as a result of the call</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="true"/></exception>
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
        /// <param name="source">The <see cref="IEnumerable{T}"/> containing elements to be removed from this collection</param>
        /// <returns>true if this collection changed as a result of the call</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">if the <see cref="RemoveAll"/> method is not supported by this collection or if could remove elemet</exception>
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
        /// </summary>
        /// <param name="filter">The <see cref="Predicate{T}"/> which returns <see langword="true"/> for elements to be removed</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> if the specified <paramref name="filter"/> is <see langword="null"/></exception>
        /// <exception cref="InvalidOperationException">if elements cannot be removed from this collection.</exception>
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
        /// <param name="source">The <see cref="IEnumerable{T}"/> containing elements to be retained in this collection</param>
        /// <returns><see langword="true"/> if this collection changed as a result of the call</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/></exception>
        bool RetainAll([NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            var modified = false;

            var array = source.ToArray();
            foreach (var item in ToArray())
            {
                if (!array.Contains(item))
                {
                    Remove(item);
                    modified = true;
                }
            }

            return modified;
        }
    }
}