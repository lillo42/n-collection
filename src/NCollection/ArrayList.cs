using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
        /// The array that contains elements.
        /// </summary>
        public virtual T[] Elements => _elements;
        
        /// <summary>
        /// The <see cref="IArrayProvider{T}"/> instance.
        /// </summary>
        public IArrayProvider<T> ArrayProvider { get; }

        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/>.
        /// </summary>
        public ArrayList()
            : this(ArrayProvider<T>.Instance)
        {
            
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/>
        /// </summary>
        /// <param name="arrayProvider">The instance <see cref="IArrayProvider{T}"/>.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="arrayProvider"/> is null.</exception>
        public ArrayList(IArrayProvider<T> arrayProvider)
        {
            ArrayProvider = arrayProvider ?? throw new ArgumentNullException(nameof(arrayProvider));
            _elements = ArrayProvider.GetOrCreate(4);
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0.</exception>
        public ArrayList(int initialCapacity)
            : this(initialCapacity, ArrayProvider<T>.Instance)
        {
            
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/>.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        /// <param name="arrayProvider">The instance <see cref="IArrayProvider{T}"/>.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="arrayProvider"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0.</exception>
        public ArrayList(int initialCapacity, IArrayProvider<T> arrayProvider)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            
            ArrayProvider = arrayProvider ?? throw new ArgumentNullException(nameof(arrayProvider));
            _elements = ArrayProvider.GetOrCreate(initialCapacity);
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <exception cref="ArgumentNullException">if the  <paramref name="source"/> is <see langword="null"/></exception>
        public ArrayList(IEnumerable<T> source)
            : this(source, ArrayProvider<T>.Instance)
        {
            
        }

        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <param name="arrayProvider">The instance <see cref="IArrayProvider{T}"/>.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> or <paramref name="arrayProvider"/> is null.</exception>
        public ArrayList(IEnumerable<T> source, IArrayProvider<T> arrayProvider)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            
            ArrayProvider = arrayProvider ?? throw new ArgumentNullException(nameof(arrayProvider));

            if (source is ArrayList<T> list)
            {
                _elements = ArrayProvider.GetOrCreate(list._elements.Length);
                Array.Copy(list._elements, _elements, list.Count);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = list.Count;
            }
            else if (source is System.Collections.Generic.ICollection<T> collection)
            {
                _elements = ArrayProvider.GetOrCreate(collection.Count);
                collection.CopyTo(_elements, 0);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = collection.Count;
            }
            else
            {
                _elements = ArrayProvider.GetOrCreate(4);
                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    TryAdd(item);
                }
            }
        }

        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/> copying the element in <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">The elements to be copy.</param>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0.</exception>
        public ArrayList(int initialCapacity, IEnumerable<T> source)
            : this(initialCapacity, source, ArrayProvider<T>.Instance)
        {
            
        }

        /// <summary>
        /// Initialize <see cref="ArrayList{T}"/> copying the element in <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">The elements to be copy.</param>
        /// <param name="initialCapacity">The initial capacity of the array.</param>
        /// <param name="arrayProvider">The instance <see cref="IArrayProvider{T}"/>.</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> or <paramref name="arrayProvider"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0.</exception>
        public ArrayList(int initialCapacity, IEnumerable<T> source, IArrayProvider<T> arrayProvider)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            ArrayProvider = arrayProvider ?? throw new ArgumentNullException(nameof(arrayProvider));
            
            if (source is ArrayList<T> list)
            {
                _elements = ArrayProvider.GetOrCreate(list._elements.Length);
                Array.Copy(list._elements, _elements, list.Count);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = list.Count;
            }
            else if (source is System.Collections.Generic.ICollection<T> collection)
            {
                _elements = ArrayProvider.GetOrCreate(collection.Count);
                collection.CopyTo(_elements, 0);
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = collection.Count;
            }
            else
            {
                _elements = ArrayProvider.GetOrCreate(initialCapacity);
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
                var tmp = ArrayProvider.GetOrCreate(Count << 1);
                Array.Copy(_elements, tmp, Count);
                ArrayProvider.Return(_elements);
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
            Array.Clear(_elements, 0, Count);
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
        public ArrayList<T> Clone() => new(this, ArrayProvider);

        object ICloneable.Clone() => Clone();
    }
}