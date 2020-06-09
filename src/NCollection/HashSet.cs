using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NCollection
{
    public class HashSet : ISet
    {
        /// <inheritdoc />
        public int Count { get; private set; }

        /// <inheritdoc />
        public bool IsSynchronized => false;

        /// <inheritdoc />
        public object SyncRoot => this;
        
        /// <inheritdoc />
        public bool IsEmpty => Count == 0;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        private readonly IEqualityComparer _comparer;
        
        private int[] _buckets = default!;
        private Slot[] _slots = default!;

        private int _freeList = -1;
        private int _lastIndex;
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet"/>.
        /// </summary>
        public HashSet()
            : this(EqualityComparer<object>.Default)
        {
            
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet"/>.
        /// </summary>
        /// <param name="equality">The <see cref="IEqualityComparer"/></param>
        public HashSet(IEqualityComparer equality)
        {
            _comparer = equality ?? throw new ArgumentNullException(nameof(equality));
            _buckets = ArrayPool<int>.Shared.Rent(4);
            _slots = ArrayPool<Slot>.Shared.Rent(4);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HashSet(IEnumerable enumerable)
            : this(EqualityComparer<object>.Default)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            foreach (var item in enumerable)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <param name="equality"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public HashSet(IEnumerable enumerable, IEqualityComparer equality)
            : this(equality)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            foreach (var item in enumerable)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements</param>
        public HashSet(int initialCapacity)
            : this(EqualityComparer<object>.Default)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }
            
            ArrayPool<Slot>.Shared.Return(_slots);
            ArrayPool<int>.Shared.Return(_buckets);
            
            _slots = ArrayPool<Slot>.Shared.Rent(initialCapacity);
            _buckets = ArrayPool<int>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements</param>
        /// <param name="equality">The <see cref="IEqualityComparer"/>.</param>
        public HashSet(int initialCapacity, IEqualityComparer equality)
        {
            _comparer = equality ?? throw new ArgumentNullException(nameof(equality));
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }
            
            ArrayPool<Slot>.Shared.Return(_slots);
            ArrayPool<int>.Shared.Return(_buckets);
            
            _slots = ArrayPool<Slot>.Shared.Rent(initialCapacity);
            _buckets = ArrayPool<int>.Shared.Rent(initialCapacity);
        }
        
        #endregion

        private void IncreaseCapacity()
        {
            var size = _slots.Length * 2;
            var newSlot = ArrayPool<Slot>.Shared.Rent(_slots.Length * 2);
            Array.Copy(_slots, 0, newSlot, 0, _lastIndex);
            
            var newBuckets = ArrayPool<int>.Shared.Rent(_buckets.Length * 2);
            for (var i = 0; i < _lastIndex; i++)
            {
                var bucket = Math.Abs(newSlot[i].hashCode.GetValueOrDefault() % size);
                newSlot[i].next = newBuckets[bucket] - 1;
                newBuckets[bucket] = i + 1;
            }
            
            ArrayPool<Slot>.Shared.Return(_slots, true);
            ArrayPool<int>.Shared.Return(_buckets, true);

            _slots = newSlot;
            _buckets = newBuckets;
        }
        
        /// <inheritdoc />
        public bool Add(object? item)
        {
            var hashCode = HashCode.Combine(item);
            var bucket = Math.Abs(hashCode % _buckets.Length);
            for (var i = _buckets[bucket] - 1 ; i >= 0; i = _slots[i].next)
            {
                if (_slots[i].hashCode == hashCode && _comparer.Equals(_slots[i].value, item))
                {
                    return false;
                }
            }

            int index;
            if (_freeList >= 0)
            {
                index = _freeList;
                _freeList = _slots[index].next;
            }
            else
            {
                if (_lastIndex == _slots.Length)
                {
                    IncreaseCapacity();   
                    bucket = Math.Abs(hashCode % _buckets.Length);
                }

                index = _lastIndex;
                _lastIndex++;
            }


            _slots[index].hashCode = hashCode;
            _slots[index].value = item;
            _slots[index].next = _buckets[bucket] - 1;
            _buckets[bucket] = index + 1;
            Count++;
            
            return true;
        }
        
        /// <inheritdoc />
        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("Rank multi-dimensional not supported", nameof(array));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Need no negative number");
            }

            if (index > Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index out  of range");
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException("Invalid length");
            }
            
            var numCopied = 0;
            for (var i = 0; i < _lastIndex && numCopied < Count; i++)
            {
                if (_slots[i].hashCode.HasValue)
                {
                    array.SetValue(_slots[i].value, index + numCopied);
                    numCopied++;
                }
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            Count = 0;
            _freeList = -1;
            _lastIndex = 0;
            Array.Clear(_slots, 0, _slots.Length);
            Array.Clear(_buckets, 0, _buckets.Length);
        }

        /// <inheritdoc />
        public bool Contains(object? item)
        {
            if (Count > 0)
            {
                var hashCode = HashCode.Combine(item);
                var bucket = Math.Abs(hashCode % _buckets.Length);
                for (var i = _buckets[bucket] - 1; i >= 0; i = _slots[i].next)
                {
                    if (_slots[i].hashCode == hashCode && _comparer.Equals(_slots[i].value, item))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool Remove(object? item)
        {
            if (Count > 0)
            {
                var hashCode = HashCode.Combine(item);
                var bucket = Math.Abs(hashCode % _buckets.Length);
                var last = -1;
                
                for (var i = _buckets[bucket] - 1; i >= 0; last = i, i = _slots[i].next)
                {
                    if (_slots[i].hashCode == hashCode && _comparer.Equals(_slots[i].value, item))
                    {
                        if (last < 0)
                        {
                            _buckets[bucket] = _slots[i].next + 1;
                        }
                        else
                        {
                            // subsequent iterations; update 'next' pointers
                            _slots[last].next = _slots[i].next;
                        }
                        _slots[i].hashCode = null;
                        _slots[i].value = null;
                        _slots[i].next = _freeList;

                        Count--;
                        
                        if (Count == 0)
                        {
                            _lastIndex = 0;
                            _freeList = -1;
                        }
                        else
                        {
                            _freeList = i;
                        }
                        
                        return true;
                    }
                }
            }

            return false;
        }

        #region IEnumerable

        /// <summary>
        /// Returns an <see cref="HashSetEnumerator"/> for the <see cref="HashSet"/>.
        /// </summary>
        /// <returns>An <see cref="HashSetEnumerator"/>  for the <see cref="HashSet"/>.</returns>
        public HashSetEnumerator GetEnumerator()
            => new HashSetEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="HashSetEnumerator"/>.
        /// </summary>
        public struct HashSetEnumerator : IEnumerator
        {
            private int _index;
            private readonly HashSet _set;

            /// <summary>
            /// Initializes a new instance of the <see cref="HashSetEnumerator"/>.
            /// </summary>
            /// <param name="set"></param>
            public HashSetEnumerator(HashSet set)
            {
                _set = set;
                _index = 0;
                Current = null;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                while (_index <= _set._lastIndex)
                {
                    if (_set._slots[_index].hashCode.HasValue)
                    {
                        Current = _set._slots[_index].value;
                        _index++;
                        return true;
                    }

                    _index++;
                }

                _index = _set._lastIndex + 1;
                Current = null;
                return false;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _index = 0;
                Current = null;
            }

            /// <inheritdoc />
            public object? Current { get; private set; }
        }
        #endregion
        
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="HashSet"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="HashSet"/>.</returns>
        public HashSet Clone()
        {
            var hash = new HashSet(_comparer);

            if (_buckets.Length != hash._buckets.Length)
            {
                ArrayPool<Slot>.Shared.Return(hash._slots);
                ArrayPool<int>.Shared.Return(hash._buckets);

                hash._buckets = ArrayPool<int>.Shared.Rent(_buckets.Length);
                hash._slots = ArrayPool<Slot>.Shared.Rent(_slots.Length);
            }

            Array.Copy(_buckets, hash._buckets, hash._buckets.Length);
            Array.Copy(_slots, hash._slots, hash._slots.Length);
            
            hash.Count = Count;
            hash._freeList = _freeList;
            hash._lastIndex = _lastIndex;
            
            return hash;
        }

        object ICloneable.Clone()
            => Clone();

        #endregion
        
        private struct Slot
        {
            internal int? hashCode;
            internal int next;
            internal object? value;
        }
    }
}