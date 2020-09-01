using System.Collections.Generic;

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
        /// Copies all of the mappings from the specified map to this map
        /// (optional operation).  The effect of this call is equivalent to that
        /// of calling <see cref="System.Collections.Generic.ICollection{T}.Add"/> on this map once
        /// for each mapping from key k to value v in the
        /// specified map.  The behavior of this operation is undefined if the
        /// specified map is modified while the operation is in progress.
        /// </summary>
        /// <param name="source"></param>
        void AddRange(IMap<TKey, TValue> source)
        {
            foreach (var (key, value) in source)
            {
                Add(key, value);
            }
        }
        
        /// <summary>
        /// If the specified key is not already associated with a value (or is mapped
        /// to null) associates it with the given value and returns
        /// null, else returns the current value.
        /// </summary>
        /// <param name="key">key with which the specified value is to be associated</param>
        /// <param name="value">value to be associated with the specified ke</param>
        /// <returns> The previous value associated with the specified key, or
        /// null if there was no mapping for the key.
        /// (A null return can also indicate that the map
        /// previously associated null with the key,
        /// if the implementation supports null values.)
        /// </returns>
        TValue AddIfAbsent(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                Add(key, value);
                return value;
            }

            return this[key];
        }
    }
}