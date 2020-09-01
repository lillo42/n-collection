using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NCollection
{
    /// <summary>
    /// The implementation of <see cref="IQueue{T}"/> using array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ArrayQueue<T> : AbstractQueue<T>, ICloneable
    {
        private int _head;
        private int _tail;
        private T[] _elements;

        /// <summary>
        /// Initialize <see cref="ArrayQueue{T}"/>
        /// </summary>
        public ArrayQueue()
        {
            _elements = ArrayPool<T>.Shared.Rent(16);
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayQueue{T}"/>
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the array</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0</exception>
        public ArrayQueue(int initialCapacity)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "The value should be greater or equal than 1");
            }
            _elements = ArrayPool<T>.Shared.Rent(initialCapacity);
        }

        /// <summary>
        /// Initialize <see cref="ArrayQueue{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is <see langword="null"/></exception>
        public ArrayQueue([JetBrains.Annotations.NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ArrayQueue<T> queue)
            {
                _elements = ArrayPool<T>.Shared.Rent(queue._elements.Length);
                Array.Copy(queue._elements, _elements, queue.Count);
                _head = queue._head;
                _tail = queue._tail;
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = queue.Count;
            }
            else if (source is System.Collections.Generic.ICollection<T> collection)
            {
                _elements = ArrayPool<T>.Shared.Rent(collection.Count);
                collection.CopyTo(_elements, 0);
                _head = 0;
                _tail = collection.Count;
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = collection.Count;
            }
            else
            {
                _elements = ArrayPool<T>.Shared.Rent(16);
                foreach (var item in source)
                {
                    // ReSharper disable once VirtualMemberCallInConstructor
                    TryEnqueue(item);
                }
            }
        }
        
        /// <summary>
        /// Initialize <see cref="ArrayQueue{T}"/> copying the element in <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="source">The elements to be copy</param>
        /// <param name="initialCapacity">The initial capacity of the array</param>
        /// <exception cref="ArgumentNullException">if the <paramref name="source"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="initialCapacity"/> is less than 0</exception>
        public ArrayQueue(int initialCapacity, [JetBrains.Annotations.NotNull] IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ArrayQueue<T> queue)
            {
                _elements = ArrayPool<T>.Shared.Rent(queue._elements.Length);
                Array.Copy(queue._elements, _elements, queue.Count);
                _head = queue._head;
                _tail = queue._tail;
                // ReSharper disable once VirtualMemberCallInConstructor
                Count = queue.Count;
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
                    TryEnqueue(item);
                }
            }
        }
        
       

        /// <inheritdoc cref="IQueue{T}"/>
        public override bool TryEnqueue(T item)
        {
            if (Count == _elements.Length)
            {
                var tmp = ArrayPool<T>.Shared.Rent(_elements.Length << 1);
                
                if (_tail > _head)
                {
                    Array.Copy(_elements, tmp, Count);
                }
                else
                {
                    Array.Copy(_elements, _head, tmp, 0, _elements.Length - _head);
                    Array.Copy(_elements, 0, tmp,_elements.Length - _head, _tail);
                }
                
                _head = 0;
                _tail = Count;
                
                ArrayPool<T>.Shared.Return(_elements, true);
                _elements = tmp;
            }
            
            _elements[_tail] = item;
            _tail = (_tail + 1) % _elements.Length;
            Count++;
            return true;
        }
        
        /// <inheritdoc cref="IQueue{T}"/>
        public override bool TryPeek(out T item)
        {
            if (Count == 0)
            {
                item = default!;
                return false;
            }

            item = _elements[_head];
            return true;
        }

        /// <inheritdoc cref="IQueue{T}"/>
        public override bool TryDequeue(out T item)
        {
            if (Count == 0)
            {
                item = default!;
                return false;
            }

            item = _elements[_head];
            Count--;
            _head = (_head + 1) % _elements.Length;
            return true;
        }
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override void Clear()
        {
            Array.Clear(_elements, 0, Count);
            _head = 0;
            _tail = 0;
            Count = 0;
        }

        /// <inheritdoc cref="ICollection{T}"/>
        public override T[] ToArray()
        {
            var result = new T[Count];
            
            if (_tail > _head)
            {
                Array.Copy(_elements, result, Count);
            }
            else
            {
                Array.Copy(_elements, _head, result, 0, _elements.Length - _head);
                Array.Copy(_elements, 0, result,_elements.Length - _head, _tail);
            }
            
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="ArrayQueue{T}"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <see cref="ArrayQueue{T}"/> that is a copy of this instance.</returns>
        public ArrayQueue<T> Clone() => new ArrayQueue<T>(this);

        object ICloneable.Clone() => Clone();
        
        /// <inheritdoc cref="System.Collections.Generic.ICollection{T}"/>
        public override IEnumerator<T> GetEnumerator()
        {
            return new QueueEnumerator(this);
        }
        
        private struct QueueEnumerator : IEnumerator<T>
        {
            private int _index;
            private T _current;
            private readonly ArrayQueue<T> _queue;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="QueueEnumerator"/>.
            /// </summary>
            /// <param name="queue">The <see cref="ArrayQueue{T}"/>.</param>
            public QueueEnumerator(ArrayQueue<T> queue)
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
                _current = returnValue ? _queue._elements[(_queue._head + _index) % _queue._elements.Length] : default!;
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