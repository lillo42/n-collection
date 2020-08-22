using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NCollection.Exceptions;

namespace NCollection
{
    /// <summary>
    /// This class provides a skeletal implementation of the <see cref="ICollection{T}"/>
    /// interface, to minimize the effort required to implement this interface.
    ///
    /// To implement an unmodifiable collection, the programmer needs only to
    /// extend this class and provide implementations for the <see cref="IEnumerable{T}"/> and
    /// <see cref="ICollection{T}.Count"/> property.
    ///
    /// To implement a modifiable collection, the programmer must additionally
    /// override this class's {@code add} method (which otherwise throws an
    /// {@code UnsupportedOperationException}), and the iterator returned by the
    /// {@code iterator} method must additionally implement its {@code remove}
    /// method
    ///
    /// The programmer should generally provide a void (no argument) and
    /// <see cref="ICollection{T}"/> constructor, as per the recommendation in the
    /// <see cref="ICollection{T}"/> interface specification.
    /// 
    /// The documentation for each non-abstract method in this class describes its
    /// implementation in detail.  Each of these methods may be overridden if
    /// the collection being implemented admits a more efficient implementation.
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class AbstractCollection<T> : ICollection<T>
    {
        /// <summary>
        /// Sole constructor.
        /// </summary>
        protected AbstractCollection()
        {
            
        }

        #region Properties
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>>
        public abstract int Count { get; }
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>>
        public abstract bool IsReadOnly { get; }

        #endregion
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public virtual bool Contains(T item)
        {
            if (item == null)
            {
                foreach (var value in this)
                {
                    if (value == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var value in this)
                {
                    if (item.Equals(value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool ContainsAll(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            foreach (var item in source)
            {
                if (Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public virtual T[] ToArray()
        {
            var res = new T[Count];
            using var enumerator = GetEnumerator();
            for (var i = 0; i < res.Length; i++)
            {
                if (!enumerator.MoveNext())
                {
                    var newArray = new T[i];
                    Array.Copy(res, newArray, i);
                    return newArray;
                }

                res[i] = enumerator.Current;
            }

            return enumerator.MoveNext() ? FinishToArray(res, enumerator) : res;

            static T[] FinishToArray(T[] r, IEnumerator<T> enumerator)
            {
                var len = r.Length;
                var i = len;
                do
                {
                    if (i == len)
                    {
                        len >>= 1 + 1;
                        var tmp = new T[len];
                        Array.Copy(r, tmp, r.Length);
                        r = tmp;
                    }

                    r[i++] = enumerator.Current;
                    
                } while (enumerator.MoveNext());

                return r;
            }
        }
        
        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool Add(T item) => throw new UnsupportedOperationException();

        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool AddRange(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            var modified = false;
            foreach (var item in source)
            {
                if (Add(item))
                {
                    modified = true;
                }
            }

            return modified;
        }
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public abstract bool Remove(T item);
        
        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool RemoveRange(IEnumerable<T> source)
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
        
        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool RetainRange(IEnumerable<T> source)
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

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public virtual void Clear()
        {
            foreach (var item in ToArray())
            {
                Remove(item);
            }
        }
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            
            if (array.Rank != 1)
            {
                throw new ArgumentException("Rank multi-dimensional not supported", nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Need no negative number");
            }

            if (arrayIndex > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Index out  of range");
            }

            var current = ToArray();
            Array.Copy(array, 0, current, arrayIndex, Count -  arrayIndex );
        }
    }
}