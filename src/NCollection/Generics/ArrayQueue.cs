using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    /// <summary>
    /// Represents a first-in, first-out (FIFO) collection of <typeparamref name="T"/> with linked node.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ArrayQueue<T> : IQueue<T>, IQueue
    {
        private int _head;
        private int _tail;
        private T[] _array;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayQueue{T}"/>.
        /// </summary>
        public ArrayQueue()
        {
            _array = ArrayPool<T>.Shared.Rent(4);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayQueue{T}"/> class that is empty and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="ArrayQueue"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public ArrayQueue(int initialCapacity)
        {
            _array = ArrayPool<T>.Shared.Rent(initialCapacity);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayQueue{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public ArrayQueue(IEnumerable<T> enumerable)
            : this()
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            foreach (var value in enumerable)
            {
                Enqueue(value);
            }
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public void Clear()
        {
            Count = 0;
            _head = 0;
            _tail = 0;
            Array.Clear(_array, 0, _array.Length);
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public int Count { get; private set; }
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public bool IsSynchronized => false;
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public object SyncRoot => this;
        
        /// <inheritdoc cref="ICollection"/>
        public bool IsReadOnly => false;
        
        /// <inheritdoc cref="ICollection"/>
        public bool IsEmpty => Count == 0;
        
        /// <inheritdoc cref="IQueue{T}"/>
        public void Enqueue(T item)
        {
            if (Count == _array.Length)
            {
                var array = ArrayPool<T>.Shared.Rent(_array.Length << 1);
                if (_head < _tail)
                {
                    Array.Copy(_array, array, _array.Length);
                }
                else
                {
                    Array.Copy(_array, _head, array, 0, _array.Length - _head);
                    Array.Copy(_array, 0, array,  _array.Length - _head, _tail);
                }
                _head = 0;
                _tail = _array.Length;
                Array.Clear(_array, 0, _array.Length);
                ArrayPool<T>.Shared.Return(_array);
                _array = array;
            }
            
            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length; 
            Count++;
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public bool TryPeek(out T item)
        {
            if (Count == 0)
            {
                item = default!;
                return false;
            }
            
            item = _array[_head];
            return true;
        }
        
        /// <inheritdoc cref="IQueue{T}"/>
        public T Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public bool TryDequeue(out T item)
        {
            if (Count == 0)
            {
                item = default!;
                return false;
            }
            
            item = _array[_head];
            _head = (_head + 1) % _array.Length;
            Count--;
            return true;
        }

        /// <inheritdoc cref="System.Collections.ICollection"/>
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

            
            var size = _array.Length - _head < Count ? _array.Length - _head : Count;
            Array.Copy(_array, _head, array, index, size);
            
            size = Count - size;
            if (size <= 0)
            {
                return;
            }
            
            Array.Copy(_array, 0, array, index + _array.Length - _head, size);
        }
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
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

        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public void CopyTo(T[] array, int arrayIndex)
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
                throw new ArgumentException("Invalid length");
            }

            
            var size = _array.Length - _head < Count ? _array.Length - _head : Count;
            Array.Copy(_array, _head, array, arrayIndex, size);
            
            size = Count - size;
            if (size <= 0)
            {
                return;
            }
            
            Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, size);
        }
        
        object ICloneable.Clone()
            => Clone();

        /// <summary>
        /// Creates a shallow copy of the <see cref="ArrayQueue{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="ArrayQueue{T}"/>.</returns>
        public ArrayQueue<T> Clone()
        {
            var queue = new ArrayQueue<T>(_array.Length);
            Array.Copy(_array, queue._array, _array.Length);
            queue.Count = Count;
            queue._head = _head;
            queue._tail = _tail;
            return queue;
        }
        
        /// <summary>
        /// Returns an <see cref="QueueStackEnumerator"/> for the <see cref="ArrayQueue{T}"/>.
        /// </summary>
        /// <returns>An <see cref="QueueStackEnumerator"/>  for the <see cref="ArrayQueue{T}"/>.</returns>
        public QueueStackEnumerator GetEnumerator() 
            => new QueueStackEnumerator(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
        
        #region Queue

        void IQueue.Enqueue(object? item)
            => Enqueue((T) item!);
        
        bool IQueue.TryPeek(out object? item)
        {
            if (TryPeek(out var value))
            {
                item = value;
                return true;
            }

            item = null;
            return false;
        }

        bool IQueue.TryDequeue(out object? item)
        {
            if (TryDequeue(out var value))
            {
                item = value;
                return true;
            }

            item = null;
            return false;
        }
        
        bool ICollection.Contains(object? item) 
            => Contains((T) item);

        #endregion

        /// <summary>
        /// Implementation of <see cref="IEnumerable{T}"/> for <see cref="ArrayQueue{T}"/>.
        /// </summary>
        public struct QueueStackEnumerator : IEnumerator<T>
        {
            private int _index;
            private T _current;
            private readonly ArrayQueue<T> _queue;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="QueueStackEnumerator"/>.
            /// </summary>
            /// <param name="queue">The <see cref="ArrayQueue{T}"/>.</param>
            public QueueStackEnumerator(ArrayQueue<T> queue)
            {
                _queue = queue;
                _index = queue.Count == 0 ?  0 : -2;
                _current = default!;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                switch (_index)
                {
                    case -2:
                        _index = -1;
                        break;
                    case -1:
                        return false;
                }
                
                var returnValue = ++_index < _queue.Count;
                _current = returnValue ? _queue._array[(_queue._head + _index) % _queue._array.Length] : default!;
                return returnValue;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _index = -2;
                _current = default!;
            }

            /// <inheritdoc />
            public T Current => _index switch
            {
                -2 => throw new InvalidOperationException("Enumerable not started"),
                -1 => throw new InvalidOperationException("Enumerable end"),
                _ => _current
            };


            object? IEnumerator.Current
                => Current;
            
            /// <inheritdoc />
            public void Dispose() { }
        }
    }
}