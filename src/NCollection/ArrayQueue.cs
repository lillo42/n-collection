using System;
using System.Buffers;
using System.Collections;
using System.Diagnostics;
using NCollection.DebugViews;

namespace NCollection
{
    /// <summary>
    ///  Represents a first-in, first-out (FIFO) collection of <see cref="object"/>  with array.
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ArrayQueue : IQueue
    {
        private int _head;
        private int _tail;
        private object?[] _array;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayQueue"/>.
        /// </summary>
        public ArrayQueue()
        {
            _array = ArrayPool<object>.Shared.Rent(4);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayQueue"/> class that is empty and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="ArrayStack"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public ArrayQueue(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }
            
            _array = ArrayPool<object>.Shared.Rent(initialCapacity);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayQueue"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public ArrayQueue(IEnumerable enumerable)
            : this()
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            foreach (var item in enumerable)
            {
                Enqueue(item);
            }
        }

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual int Count { get; private set; }
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual bool IsSynchronized => false;
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual object SyncRoot => this;

        /// <inheritdoc cref="ICollection"/>
        public virtual bool IsEmpty => Count == 0;

        /// <inheritdoc cref="ICollection"/>
        public virtual bool IsReadOnly => false;
        
        /// <inheritdoc cref="IQueue"/>
        public virtual void Enqueue(object? item)
        {
            if (Count == _array.Length)
            {
                var array = ArrayPool<object>.Shared.Rent(_array.Length * 2);
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
                ArrayPool<object>.Shared.Return(array);
                _array = array;
            }
            
            _array[_tail] = item;
            _tail = (_tail + 1) % _array.Length; 
            Count++;
        }
        
        /// <inheritdoc cref="IQueue"/>
        public virtual object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        
        /// <inheritdoc cref="IQueue"/>
        public virtual bool TryPeek(out object? item)
        {
            if (Count == 0)
            {
                item = null;
                return false;
            }
            
            item = _array[_head];
            return true;
        }
        
        /// <inheritdoc cref="IQueue"/>
        public virtual object? Dequeue()
        {
            if (!TryDequeue(out var item))
            {
                throw new InvalidOperationException("Empty queue");
            }

            return item;
        }
        
        /// <inheritdoc cref="IQueue"/>
        public virtual bool TryDequeue(out object? item)
        {
            if (Count == 0)
            {
                item = null;
                return false;
            }
            
            item = _array[_head];
            _head = (_head + 1) % _array.Length;
            Count--;
            return true;
        }

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual void CopyTo(Array array, int index)
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

        /// <inheritdoc cref="IQueue"/>
        public bool Contains(object? item)
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
                else if (_array[size] != null && _array[size]!.Equals(item))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public virtual void Clear()
        {
            Array.Clear(_array, 0, _array.Length);
            Count = 0;
        }

        object ICloneable.Clone()
            => Clone();

        /// <summary>
        /// Creates a shallow copy of the <see cref="ArrayQueue"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="ArrayQueue"/>.</returns>
        public ArrayQueue Clone()
        {
            var queue = new ArrayQueue(_array.Length);
            Array.Copy(_array, queue._array, _array.Length);
            queue.Count = Count;
            queue._head = _head;
            queue._tail = _tail;
            return queue;
        }


        /// <summary>
        /// Returns an <see cref="QueueStackEnumerator"/> for the <see cref="LinkedQueue"/>.
        /// </summary>
        /// <returns>An <see cref="QueueStackEnumerator"/>  for the <see cref="LinkedQueue"/>.</returns>
        public QueueStackEnumerator GetEnumerator() 
            => new QueueStackEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="ArrayQueue"/>.
        /// </summary>
        public struct QueueStackEnumerator : IEnumerator
        {
            private int _index;
            private object? _current;
            private readonly ArrayQueue _queue;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="QueueStackEnumerator"/>.
            /// </summary>
            /// <param name="queue">The <see cref="ArrayQueue"/> queue.</param>
            public QueueStackEnumerator(ArrayQueue queue)
            {
                _queue = queue;
                _index = queue.Count == 0 ?  0 : -2;
                _current = null;
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
                _current = returnValue ? _queue._array[(_queue._head + _index) % _queue._array.Length] : null;
                return returnValue;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _index = -2;
                _current = null;
            }

            /// <inheritdoc />
            public object? Current 
                => _index switch
                {
                    -2 => throw new InvalidOperationException("Enumerable not started"),
                    -1 => throw new InvalidOperationException("Enumerable end"),
                    _ => _current
                };
        }
    }
}