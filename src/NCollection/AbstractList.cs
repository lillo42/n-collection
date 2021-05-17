using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCollection
{
    /// <summary>
    /// This class provides a skeletal implementation of the <see cref="IList{T}"/>
    /// interface, to minimize the effort required to implement this interface. 
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class AbstractList<T> : AbstractCollection<T>, IList<T>, IList, IReadOnlyList<T>
    {
        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public abstract T this[int index] { get; set; }
        
        /// <inheritdoc cref="IList{T}"/>
        public abstract IList<T> this[Range range] { get; }
        
        /// <inheritdoc cref="System.Collections.IList"/>
        public virtual bool IsFixedSize => false;

        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public virtual int IndexOf(T item) => IndexOf(item, EqualityComparer<T>.Default);

        /// <inheritdoc cref="IList{T}"/>
        public abstract int IndexOf(T item, IEqualityComparer<T> comparer);

        /// <inheritdoc cref="IList{T}"/>
        public virtual int LastIndexOf(T item) => LastIndexOf(item, EqualityComparer<T>.Default);

        /// <inheritdoc cref="IList{T}"/>
        public abstract int LastIndexOf(T item, IEqualityComparer<T> comparer);

        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public abstract void RemoveAt(int index);

        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public abstract void Add(int index, T item);

        /// <inheritdoc cref="IList{T}"/>
        public virtual bool AddAll(int index, IEnumerable<T> source)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index should be greater than 0");
            }
                
            if (index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index should be greater than {Count}");
            }
            
            var modify = false;
            foreach (var item in source)
            {
                Add(index++, item);
                modify = true;
            }

            return modify;
        }
        
        int IList.Add(object? value)
        {
            try
            {
                Add((T) value!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
            }

            return Count - 1;
        }

        bool IList.Contains(object? value)
        {
            try
            {
                return Contains((T) value!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
            }

            return false;
        }

        int IList.IndexOf(object? value)
        {
            try
            {
                return IndexOf((T) value!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
            }

            return -1;
        }

        void IList.Insert(int index, object? value)
        {
            try
            {
                Add(index, (T) value!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
            }
        }

        void IList.Remove(object? value)
        {
            try
            {
                Remove((T) value!);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
            }
        }
        
        object? IList.this[int index]
        {
            get => this[index]!;
            set
            {
                try
                {
                    this[index] = (T) value!;
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                }
            }
        }
    }
}