using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IList{T}"/> using array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ArrayList<T> : AbstractList<T>, ICloneable
    {
        private T[] _elements;

        /// <summary>
        /// The array that contains elements
        /// </summary>
        [NotNull]
        public T[] Elements => _elements;

        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/>
        /// </summary>
        public ArrayList()
        {
            _elements = ArrayPool<T>.Shared.Rent(16);
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/>
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0</exception>
        public ArrayList(int initialCapacity)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            _elements = ArrayPool<T>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <exception cref="ArgumentNullException">if the  <paramref name="source"/> is <see langword="null"/></exception>
        public ArrayList([NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ArrayList<T> list)
            {
                _elements = ArrayPool<T>.Shared.Rent(list._elements.Length);
                Array.Copy(list._elements, _elements, list.Count);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = list.Count;
            }
            else if (source is System.Collections.Generic.ICollection<T> collection)
            {
                _elements = ArrayPool<T>.Shared.Rent(collection.Count);
                collection.CopyTo(_elements, 0);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = collection.Count;
            }
            else
            {
                _elements = ArrayPool<T>.Shared.Rent(16);
                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    TryAdd(item);
                }
            }
        }

        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <param name="initialCapacity">The initial capacity of the array</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is null </exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0</exception>
        public ArrayList(int initialCapacity, [NotNull] IEnumerable<T> source)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ArrayList<T> list)
            {
                _elements = ArrayPool<T>.Shared.Rent(list._elements.Length);
                Array.Copy(list._elements, _elements, list.Count);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = list.Count;
            }
            else if (source is System.Collections.Generic.ICollection<T> collection)
            {
                _elements = ArrayPool<T>.Shared.Rent(collection.Count);
                collection.CopyTo(_elements, 0);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = collection.Count;
            }
            else
            {
                _elements = ArrayPool<T>.Shared.Rent(initialCapacity);
                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    TryAdd(item);
                }
            }
        }
        
        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public override T this[int index]
        {
            get
            {
                CheckIndex(index);
                return _elements[index];
            }
            set
            {
                CheckIndex(index);
                _elements[index] = value;
            }
        }

        /// <inheritdoc cref="IList{T}"/>
        public override IList<T> this[Range range]
        {
            get
            {
                int length;
                if (range.End.IsFromEnd)
                {
                    length = Count - range.Start.Value;
                }
                else
                {
                    length = range.End.Value - range.Start.Value;
                }
                
                var list = new ArrayList<T>(length);

                Array.Copy(_elements, range.Start.Value, list._elements, 0, length);
                list.Count = length;
                return list;
            }
        }
        
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private void CheckIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index should be greater than 0");
            }
                
            if (index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index should be greater than {Count}");
            }
        }
        
        

        /// <inheritdoc cref="ICollection{T}"/>
        public override bool TryAdd(T item)
        {
            EnsureCapacity(Count + 1);
            _elements[Count] = item;
            Count++;
            return true;
        }
        
        private void EnsureCapacity(int min)
        {
            if (_elements.Length < min)
            {
                var tmp = ArrayPool<T>.Shared.Rent(Count << 1);
                Array.Copy(_elements, tmp, Count);
                ArrayPool<T>.Shared.Return(_elements, true);
                _elements = tmp;
            }
        }
        
        /// <inheritdoc cref="IList{T}"/>
        public override void Add(int index, T item)
        {
            CheckIndex(index);
            EnsureCapacity(Count + 1);
            
            if (index < Count)
            {
                Array.Copy(_elements, index, _elements, index +1, Count - index);
            }

            _elements[index] = item;
            Count++;
        }

        /// <inheritdoc cref="IList{T}"/>
        public override bool AddAll(int index, IEnumerable<T> source)
        {
            CheckIndex(index);
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ICollection<T> collection)
            {
                EnsureCapacity(Count + collection.Count);
                if (index < Count)
                {
                    Array.Copy(_elements, index, _elements, index + collection.Count, Count - index);
                }
                
                collection.CopyTo(_elements, index);
                Count += collection.Count;
            }
            else
            {
                foreach (var item in source)
                {
                    Add(index++, item);
                }
            }

            return true;
        }

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }
        
        /// <inheritdoc cref="System.Collections.Generic.IList{T}"/>
        public override void RemoveAt(int index)
        {
            CheckIndex(index);

            Count--;
            if (index < Count)
            {
                Array.Copy(_elements, index + 1, _elements, index, Count - index);
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _elements[Count] = default!;    
            }
        }
        

        /// <inheritdoc cref="IList{T}"/>
        public override int IndexOf(T item, IEqualityComparer<T> comparer)
        {
            for (var i = 0; i < Count; i++)
            {
                if (comparer.Equals(item, _elements[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc cref="IList{T}"/>
        public override int LastIndexOf(T item, IEqualityComparer<T> comparer)
        {
            for (var i = Count -1; i >= 0; i--)
            {
                if (comparer.Equals(item, _elements[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            Array.Clear(Elements, 0, Count);
            Count = 0;
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public override T[] ToArray()
        {
            var result = new T[Count];
            Array.Copy(_elements, result, Count);
            return result;
        }

        /// <inheritdoc />
        public override IEnumerator<T> GetEnumerator()
        {
            return new ArrayListEnumerator(this);
        }
        
        private struct ArrayListEnumerator : IEnumerator<T>
        {
            private readonly ArrayList<T> _list;
            private int _index;

            public ArrayListEnumerator(ArrayList<T> list)
            {
                _list = list;
                _index = -1;
                Current = default!;
            }

            public bool MoveNext()
            {
                _index++;
                if (_index >= _list.Count)
                {
                    Current = default!;
                    return false;
                }

                Current = _list._elements[_index];
                return true;
            }

            public void Reset()
            {
                _index = -1;
            }

            public T Current { get; private set; }

            object? IEnumerator.Current => Current;

            public void Dispose()
            {
                
            }
        }

        /// <summary>
        /// Creates a new <see cref="ArrayList{T}"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="ArrayList{T}"/> that is a copy of this instance.</returns>
        public ArrayList<T> Clone()
        {
            return new ArrayList<T>(this);
        }
        
        object ICloneable.Clone() => Clone();
    }
}