using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCollection
{
    /// <summary>
    /// An object that maps keys to values.  A map cannot contain duplicate keys;
    /// each key can map to at most one value.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public interface IMap<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        ISet<IEntry> Entries { get; }
        
        new ISet<TKey> Keys { get; }
        new ICollection<TValue> Values { get; }
        
        /// <summary>
        /// Returns true if this map maps one or more keys to the
        /// specified value.  More formally, returns true if and only if
        /// this map contains at least one mapping to a value v such that
        /// object.Equals(value, v).  This operation
        /// will probably require time linear in the map size for most
        /// implementations of the <see cref="IMap{T,V}"/> interface.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ContainsValue(TValue value);

        
        /// <summary>
        ///  Try adds an item to the <see cref="IMap{TKey,TValue}"/>
        /// </summary>
        /// <param name="key">The <typeparamref name="TKey"/> </param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryAdd(TKey key, TValue value);
        
        /// <summary>
        /// Copies all of the mappings from the specified map to this map
        /// (optional operation).  The effect of this call is equivalent to that
        /// of calling <see cref="ICollection{T}.Add"/> on this map once
        /// for each mapping from key k to value v in the
        /// specified map.  The behavior of this operation is undefined if the
        /// specified map is modified while the operation is in progress.
        /// </summary>
        /// <param name="source"></param>
        void AddAll(IMap<TKey, TValue> source)
        {
            foreach (var (key, value) in source)
            {
                Add(key, value);
            }
        }

        #region IDictionary

        System.Collections.Generic.ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
        
        System.Collections.Generic.ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        #endregion
        
        /// <summary>
        /// A map entry (key-value pair).  The {@code Map.entrySet} method returns
        /// a collection-view of the map, whose elements are of this class.  The
        /// only way to obtain a reference to a map entry is from the
        /// iterator of this collection-view.  These {@code Map.Entry} objects are
        /// valid only for the duration of the iteration; more formally,
        /// the behavior of a map entry is undefined if the backing map has been
        /// modified after the entry was returned by the iterator, except through
        /// the {@code setValue} operation on the map entry.
        /// </summary>
        public interface IEntry
        {
            /// <summary>
            /// The key corresponding to this entry
            /// </summary>
            TKey Key { get; }
            
            /// <summary>
            /// The value corresponding to this entry
            /// </summary>
            TValue Value { get; set; }
        }
    }
}