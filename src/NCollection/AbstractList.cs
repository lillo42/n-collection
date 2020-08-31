using System;
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
    public abstract class AbstractList<T> : AbstractCollection<T>, IList<T>
    {
        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public abstract T this[int index] { get; set; }
        
        /// <inheritdoc cref="IList{T}"/>
        public abstract IList<T> this[Range range] { get; }

        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public int IndexOf(T item) => IndexOf(item, EqualityComparer<T>.Default);

        /// <inheritdoc cref="IList{T}"/>
        public abstract int IndexOf(T item, IEqualityComparer<T> comparer);

        /// <inheritdoc cref="IList{T}"/>
        public int LastIndexOf(T item) => LastIndexOf(item, EqualityComparer<T>.Default);

        /// <inheritdoc cref="IList{T}"/>
        public abstract int LastIndexOf(T item, IEqualityComparer<T> comparer);

        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public abstract void RemoveAt(int index);
        

        /// <inheritdoc cref="IList{T}"/>
        public abstract bool AddAll(int index, IEnumerable<T> source);

        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public abstract void Add(int index, T item);
    }
}