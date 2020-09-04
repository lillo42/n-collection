using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NCollection
{
    /// <summary>
    /// This class provides a skeletal implementation of the <see cref="IMap{TKey,TValue}"/>
    /// interface, to minimize the effort required to implement this interface.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public abstract class AbstractMap<TKey, TValue> : AbstractCollection<KeyValuePair<TKey, TValue>>,  IMap<TKey, TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual IEqualityComparer<TKey> Comparer { get; }

        /// <summary>
        /// 
        /// </summary>
        protected AbstractMap()
        {
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        protected AbstractMap([NotNull] IEqualityComparer<TKey> comparer)
        { 
            Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

        }
        
        /// <inheritdoc cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>
        public abstract TValue this[TKey key] { get; set; }
        
        /// <inheritdoc cref="IMap{TKey, TValue}"/>
        public abstract ISet<IMap<TKey, TValue>.IEntry> Entries { get; }

        /// <inheritdoc cref="IMap{TKey, TValue}"/>
        public abstract ISet<TKey> Keys { get; }
        
        /// <inheritdoc cref="IMap{TKey, TValue}"/>
        public abstract ICollection<TValue> Values { get; }
        
        /// <inheritdoc cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>
        public virtual void Add(TKey key, TValue value)
        {
            if (!TryAdd(new KeyValuePair<TKey, TValue>(key, value)))
            {
                throw new InvalidOperationException("Duplicated key");
            }
        }
        
        /// <inheritdoc cref="IMap{TKey, TValue}"/>
        public abstract bool TryAdd(TKey key, TValue value);

        /// <inheritdoc />
        public override bool TryAdd(KeyValuePair<TKey, TValue> item)
        {
            return TryAdd(item.Key, item.Value);
        }

        /// <inheritdoc cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>
        public abstract bool Remove(TKey key);

        /// <inheritdoc />
        public override bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

        /// <inheritdoc cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>
        public abstract bool TryGetValue(TKey key, out TValue value);

        /// <inheritdoc cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>
        public virtual bool ContainsKey(TKey key)
        {
            foreach (var item in Keys)
            {
                if (Comparer.Equals(item, key))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc cref="IMap{TKey, TValue}"/>
        public virtual bool ContainsValue(TValue value)
        {
            foreach (var item in Values)
            {
                if (EqualityComparer<TValue>.Default.Equals(item, value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}