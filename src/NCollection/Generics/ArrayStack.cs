using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    /// <summary>
    /// Represents a last-in-first-out (LIFO) generic collection of <typeparamref name="T"/> with array.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ArrayStack<T> : IStack<T>, IStack
    {
        private int _size = 0;
        private T[] _array;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayStack{T}"/>.
        /// </summary>
        public ArrayStack()
        {
            _array = ArrayPool<T>.Shared.Rent(4);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayStack{T}"/> class that is empty and has the specified initial capacity or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements that the <see cref="ArrayStack"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/>is less than zero.</exception>
        public ArrayStack(int initialCapacity)
        {
            _array = new T[initialCapacity];
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayStack{T}"/>.
        /// </summary>
        /// <param name="enumerable">The <see cref="IEnumerable"/> to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is null</exception>
        public ArrayStack(IEnumerable<T> enumerable)
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
        
        /// <inheritdoc cref="IStack{T}"/>
        public int Count => _size;

        /// <inheritdoc cref="IStack"/>
        public bool IsEmpty => Count == 0;

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public bool IsSynchronized => false;

        /// <inheritdoc cref="System.Collections.ICollection"/>
        public object SyncRoot => this;

        /// <inheritdoc cref="IStack{T}"/>
        public bool IsReadOnly => false;
        
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

            if (array.Length - arrayIndex < _size)
            {
                throw new ArgumentException("Invalid length");
            }

            var srcIndex = 0;
            var dstIndex = arrayIndex + _size;
            while(srcIndex < _size)
            {
                array[--dstIndex] = _array[srcIndex++];
            }
        }
        
        /// <inheritdoc cref="IStack{T}"/>
        [return: MaybeNull]
        public T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public void Clear()
        {
            Array.Clear(_array, 0, _array.Length);
            _size = 0;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public bool TryPeek([MaybeNullWhen(false)]out T item)
        {
            if (_size == 0)
            {
                item = default!;
                return false;
            }

            item = _array[_size - 1];
            return true;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public void Push(T item)
        {
            if (_size == _array.Length)
            {
                var array = ArrayPool<T>.Shared.Rent(_array.Length << 1);
                Array.Copy(_array, array, _array.Length);
                Array.Clear(_array, 0, _array.Length);
                ArrayPool<T>.Shared.Return(_array!);
                _array = array;
            }

            _array[_size] = item;
            _size++;
        }

        /// <inheritdoc cref="IStack{T}"/>
        [return: MaybeNull]
        public T Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
        /// <inheritdoc cref="IStack{T}"/>
        public bool TryPop([MaybeNullWhen(false)]out T item)
        {
            if (_size == 0)
            {
                item = default!;
                return false;
            }

            item = _array[_size - 1];
            _size--;
            return true;
        }

        /// <inheritdoc cref="IStack{T}"/>
        public bool Contains(T item)
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
                else if (_array[size] != null && EqualityComparer<T>.Default.Equals(_array[size]!,item))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Creates a shallow copy of the <see cref="ArrayStack{T}"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="ArrayStack{T}"/>.</returns>
        public ArrayStack<T> Clone()
        {
            var clone = new ArrayStack<T>(_size);
            Array.Copy(_array, clone._array, _size);
            clone._size = _size;
            return clone;
        }
        
        object ICloneable.Clone()
            => Clone();

        /// <summary>
        /// Returns an <see cref="ArrayStackEnumerator"/> for the <see cref="ArrayStack{T}"/>.
        /// </summary>
        /// <returns>An <see cref="ArrayStackEnumerator"/>  for the <see cref="ArrayStack{T}"/>.</returns>
        public ArrayStackEnumerator GetEnumerator()
            => new ArrayStackEnumerator(this);
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();
        
        #region Stack
        void IStack.Push(object? item)
            => Push((T) item!);

        bool IStack.TryPop(out object? item)
        {
            if (TryPop(out var result))
            {
                item = result;
                return true;
            }

            item = null;
            return false;
        }
        
        bool IStack.TryPeek(out object? item)
        {
            if (TryPeek(out var result))
            {
                item = result;
                return true;
            }

            item = null;
            return false;
        }

        bool ICollection.Contains(object? item) => Contains((T) item);
        #endregion

        /// <summary>
        /// Implementation of <see cref="IEnumerable{T}"/> for <see cref="ArrayStack{T}"/>.
        /// </summary>
        public struct ArrayStackEnumerator : IEnumerator<T>
        {
            private int _index;
            private T _current;
            private readonly ArrayStack<T> _stack;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="ArrayStackEnumerator"/>.
            /// </summary>
            /// <param name="stack">The <see cref="ArrayStack{T}"/>.</param>
            public ArrayStackEnumerator(ArrayStack<T> stack)
            {
                _stack = stack;
                _index = -2;
                _current = default!;
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
                _current = returnValue ? _stack._array[_index] : default!;
                return returnValue;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _index = -2;
                _current = default!;
            }

            object? IEnumerator.Current => Current;

            /// <inheritdoc />
            public T Current 
                => _index switch
                {
                    -2 => throw new InvalidOperationException("Enumerable not started"),
                    -1 => throw new InvalidOperationException("Enumerable end"),
                    _ => _current
                };

            /// <inheritdoc />
            public void Dispose()
            {
                
            }
        }
    }
}