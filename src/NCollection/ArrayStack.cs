using System;
using System.Buffers;
using System.Collections;
using System.Diagnostics;
using NCollection.DebugViews;

namespace NCollection
{
    /// <summary>
    /// Implementation of last-in-first-out (LIFO) non-generic collection of <see cref="object"/> with array.
    /// </summary>
    [DebuggerTypeProxy(typeof(ICollectionDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public class ArrayStack : IStack
    {
        private int _size = 0;
        private object?[] _array;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayStack"/>.
        /// </summary>
        public ArrayStack()
        {
            _array = ArrayPool<object>.Shared.Rent(4);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayStack"/> class that is empty and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="ArrayStack"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public ArrayStack(int initialCapacity)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Need non-negative number");
            }
            
            _array = ArrayPool<object>.Shared.Rent(initialCapacity);
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkedStack"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public ArrayStack(IEnumerable enumerable)
            : this()
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            
            foreach (var item in enumerable)
            {
                Push(item);
            }
        }
        
        /// <inheritdoc cref="System.Collections.ICollection"/>
        public int Count => _size;

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public bool IsSynchronized => false;

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public object SyncRoot => this;

        /// <inheritdoc cref="ICollection"/>
        public bool IsEmpty => Count == 0;

        /// <inheritdoc cref="ICollection"/>
        public bool IsReadOnly => false;

        /// <inheritdoc cref="IStack"/>
        public object? Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
        /// <inheritdoc cref="IStack"/>
        public bool TryPeek(out object? item)
        {
            if (_size == 0)
            {
                item = null;
                return false;
            }

            item = _array[_size - 1];
            return true;
        }
        
        /// <inheritdoc cref="IStack"/>
        public void Push(object? item)
        {
            if (_size == _array.Length)
            {
                var array = ArrayPool<object>.Shared.Rent(_array.Length * 2);
                Array.Copy(_array, array, _array.Length);
                ArrayPool<object>.Shared.Return(_array!);
                _array = array;
            }

            _array[_size] = item;
            _size++;
        }

        /// <inheritdoc cref="IStack"/>
        public object? Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
        /// <inheritdoc cref="IStack"/>
        public bool TryPop(out object? item)
        {
            if (_size == 0)
            {
                item = null;
                return false;
            }

            item = _array[_size - 1];
            _size--;
            return true;
        }

        /// <inheritdoc cref="IStack"/>
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

            if (array.Length - index < _size)
            {
                throw new ArgumentException("Invalid length");
            }

            if (array is object?[] objArray)
            {
                for (var i = 0; i < _size; i++)
                {
                    objArray[i + index] = _array[_size - i - 1];
                }
            }
            else
            {
                for (var i = 0; i < _size; i++)
                {
                    array.SetValue(_array[_size - i - 1], i + index);
                }
            }
        }

        /// <inheritdoc cref="IStack"/>
        public bool Contains(object? item)
        {
            var size = _size;
            while (size-- > 0)
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
        
        /// <inheritdoc cref="ICollection"/>
        public void Clear()
        {
            Array.Clear(_array, 0, _size);
            _size = 0;
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="ArrayStack"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="ArrayStack"/>.</returns>
        public ArrayStack Clone()
        {
            var clone = new ArrayStack(_size);
            Array.Copy(_array, clone._array, _size);
            clone._size = _size;
            return clone;
        }
        
        /// <summary>
        /// Returns an <see cref="ArrayStackEnumerator"/> for the <see cref="ArrayStack"/>.
        /// </summary>
        /// <returns>An <see cref="ArrayStackEnumerator"/>  for the <see cref="ArrayStack"/>.</returns>
        public ArrayStackEnumerator GetEnumerator()
            => new ArrayStackEnumerator(this);
        
        object ICloneable.Clone()
            => Clone();

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        /// <summary>
        /// Implementation of <see cref="IEnumerable"/> for <see cref="ArrayStack"/>.
        /// </summary>
        public struct ArrayStackEnumerator : IEnumerator
        {
            private int _index;
            private object? _current;
            private readonly ArrayStack _stack;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="ArrayStackEnumerator"/>.
            /// </summary>
            /// <param name="stack">The <see cref="ArrayStack"/>.</param>
            public ArrayStackEnumerator(ArrayStack stack)
            {
                _stack = stack;
                _index = -2;
                _current = null;
            }
            
            /// <inheritdoc />
            public bool MoveNext()
            {
                bool returnValue;
                switch (_index)
                {
                    case -2:
                    {
                        _index = _stack._size - 1;
                        returnValue = _index > 0;
                        if (returnValue)
                        {
                            _current = _stack._array[_index];
                        }

                        return returnValue;
                    }
                    case -1:
                        return false;
                }

                returnValue = --_index >= 0;
                _current = returnValue ? _stack._array[_index] : null;
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