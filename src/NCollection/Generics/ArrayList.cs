using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    /// <summary>
    /// Represents Array list
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ArrayList<T> : IList<T>, IList
    {
        private T[] _array;

        /// <summary>
        /// Initializes a new instance of the <see cref="System.Collections.ArrayList"/>.
        /// </summary>
        public ArrayList()
        {
            _array = ArrayPool<T>.Shared.Rent(4);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayStack"/> class that is empty and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="System.Collections.ArrayList"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public ArrayList(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }
            
            _array = ArrayPool<T>.Shared.Rent(initialCapacity);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="System.Collections.ArrayList"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public ArrayList(IEnumerable<T> enumerable)
            : this()
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
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public int Count { get; private set; }

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public bool IsSynchronized => false;
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public object SyncRoot => this;

        /// <inheritdoc cref="ICollection"/>
        public bool IsEmpty => Count == 0;
        
        /// <inheritdoc cref="ICollection"/>
        public bool IsFixedSize => false;

       

        /// <inheritdoc cref="ICollection"/>
        public bool IsReadOnly => false;

        

        /// <inheritdoc />
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),  "index out of range");
                }

                return _array[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index),  "index out of range");
                }

                _array[index] =  value;
            }
        }

        /// <inheritdoc cref="IList.Add" />
        public void Add(T item)
        {
            EnsureCapacity();
            _array[Count] = item;
            Count++;
        }


        

        /// <inheritdoc cref="IList.Clear" />
        public void Clear()
        {
            Array.Clear(_array, 0, _array.Length);
            Count = 0;
        }


        /// <inheritdoc />
        public void Insert(int index, T value)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index out of range");
            }

            EnsureCapacity();

            if (index < Count)
            {
                Array.Copy(_array, index, _array, index + 1, Count - index);
            }

            _array[index] = value;
            Count++;
        }

        private void EnsureCapacity()
        {
            if (Count == _array.Length)
            {
                var array = ArrayPool<T>.Shared.Rent(_array.Length * 2);
                Array.Copy(_array, array, _array.Length);
                ArrayPool<T>.Shared.Return(_array!);
                _array = array;
            }
        }
        
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
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
            
            
            Array.Copy(_array, 0, array, index, Count);
        }
        
        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) => CopyTo((Array)array, arrayIndex);

        /// <inheritdoc cref="IList.Contains" />
        public bool Contains(T item)
        {
            var size = -1;
            while (++size < Count)
            {
                if (item == null)
                {
                    if (_array[size] == null)
                    {
                        return true;
                    }
                }
                else if (_array[size] != null && EqualityComparer<T>.Default.Equals(_array[size], item))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <inheritdoc />
        public int IndexOf(T value)
        {
            var size = -1;
            while (++size < Count)
            {
                if (value == null)
                {
                    if (_array[size] == null)
                    {
                        return size;
                    }
                }
                else if (_array[size] != null && EqualityComparer<T>.Default.Equals(_array[size], value))
                {
                    return size;
                }
            }
            
            return -1;
        }

        /// <inheritdoc cref="IList.Remove" />
        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <inheritdoc cref="System.Collections.Generic.IList{T}.RemoveAt" />
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "index out of range");
            }

            Count--;
            if (index == Count)
            {
                _array[Count] = default!;
            }
            else
            { 
                Array.Copy(_array, index + 1,  _array, index, Count - index);
            }
        }

        /// <inheritdoc />
        public void Sort(ISortAlgorithm<T> algorithm, IComparer<T> comparer)
        {
            var array = new T[Count];
            CopyTo(array, 0);
            
            algorithm.Execute(array, comparer);
            
            Array.Copy(array, _array, Count);
        }

        #region Clone

        /// <inheritdoc cref="ICloneable.Clone"/>
        public ArrayList<T> Clone() 
            => new ArrayList<T>(this);

        object ICloneable.Clone()
            => Clone();
        #endregion


        #region IEnumerator
        /// <summary>
        /// Returns an <see cref="ArrayListEnumerable"/> for the <see cref="System.Collections.ArrayList"/>.
        /// </summary>
        /// <returns>An <see cref="ArrayListEnumerable"/>  for the <see cref="System.Collections.ArrayList"/>.</returns>
        public ArrayListEnumerable GetEnumerator() 
            => new ArrayListEnumerable(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        
        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="ArrayList"/>.
        /// </summary>
        
        public struct ArrayListEnumerable : IEnumerator<T>
        {
            private int _index;
            private readonly ArrayList<T> _list;

            /// <summary>
            /// Initializes a new instance of the <see cref="ArrayListEnumerable"/>.
            /// </summary>
            /// <param name="list">The <see cref="ArrayList"/> list.</param>
            public ArrayListEnumerable(ArrayList<T> list)
            {
                _list = list ?? throw new ArgumentNullException(nameof(list));
                _index = 0;
                Current = default!;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_index == _list.Count)
                {
                    Current = default!;
                    return false;
                }
                
                
                Current = _list._array[_index++];
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                Current = default!;
                _index = 0;
            }

            /// <inheritdoc />
            public T Current { get; private set; }

            /// <inheritdoc />
            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public void Dispose()
            {
                
            }
        }
        #endregion

        #region IList

        object? System.Collections.IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T) value!;
        }

        void IList.Add(object? item)
            => Add((T) item!);
        bool IList.Remove(object? item)
            => Remove((T) item!);

        void IList.Sort(ISortAlgorithm algorithm, IComparer comparable)
            => Sort((ISortAlgorithm<T>) algorithm, (IComparer<T>) comparable);

        bool IList.Contains(object? item)
            => Contains((T) item!);
        
        int System.Collections.IList.IndexOf(object value) => IndexOf((T) value!);

        void System.Collections.IList.Insert(int index, object value) 
            => Insert(index, (T)value);

        #endregion
    }
}