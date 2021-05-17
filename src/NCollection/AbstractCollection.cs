using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCollection
{
    /// <summary>
    /// This class provides a skeletal implementation of the <see cref="ICollection{T}"/>
    /// interface, to minimize the effort required to implement this interface.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class AbstractCollection<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>
    {
        #region Properties

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public virtual int Count { get; protected set; }

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual bool IsSynchronized => false;

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual object SyncRoot => this;

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public virtual bool IsReadOnly { get; } = false;

        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool IsEmpty => Count == 0;
        
        #endregion

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public abstract IEnumerator<T> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public virtual bool Contains(T item)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var value in this)
            {
                if (EqualityComparer<T>.Default.Equals(item, value))
                {
                    return true;
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
                if (!Contains(item))
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

                    r[i] = enumerator.Current;
                    i++;

                } while (enumerator.MoveNext());

                return r;
            }
        }
        
        /// <inheritdoc cref="ICollection{T}"/>
        public abstract bool TryAdd(T item);

        /// <inheritdoc cref="ICollection{T}"/>
        public virtual void Add(T item)
        {
            if (!TryAdd(item))
            {
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool AddAll(IEnumerable<T> source)
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
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public abstract bool Remove(T item);
        
        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool RemoveAll(IEnumerable<T> source)
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
        public bool RemoveIf(Predicate<T> filter)
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

        /// <inheritdoc cref="ICollection{T}"/>
        public virtual bool RetainAll(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (Count == 0)
            {
                return false;
            }

            if (Equals(source, this))
            {
                Clear();
                return true;
            }

            var modified = false; 
            foreach (var item in source)
            {
                modified &= Remove(item);
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
            
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("Invalid  length");
            }

            var current = ToArray();
            Array.Copy(current, 0, array, arrayIndex, Count -  arrayIndex );
        }
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);
    }
}