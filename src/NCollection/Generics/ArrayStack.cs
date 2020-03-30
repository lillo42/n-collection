using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NCollection.Generics.DebugViews;

namespace NCollection.Generics
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public class ArrayStack<T> : IStack<T>, IStack, ICloneable<ArrayStack<T>>
    {
        private int _size = 0;
        private T[] _array;
        
        public ArrayStack()
        {
            _array = ArrayPool<T>.Shared.Rent(4);
        }

        public ArrayStack(int initialCapacity)
        {
            _array = new T[initialCapacity];
        }
        
        public ArrayStack(IEnumerable<T> enumerable)
            : this()
        {
            foreach (var item in enumerable)
            {
                Push(item);
            }
        }
        
        public virtual int Count => _size;

        public virtual bool IsSynchronized => false;

        public virtual object SyncRoot => this;

        public virtual bool IsReadOnly => false;
        
        public void CopyTo(Array array, int index)
        {
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
        
        [return: MaybeNull]
        public virtual T Peek()
        {
            if (!TryPeek(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
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

        public void Push(T item)
        {
            if (_size == _array.Length)
            {
                var array = ArrayPool<T>.Shared.Rent(_array.Length * 2);
                Array.Copy(_array, array, _array.Length);
                ArrayPool<T>.Shared.Return(_array!);
                _array = array;
            }

            _array[_size] = item;
            _size++;
        }
        
        public void Push(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Push(item);
            }
        }

        [return: MaybeNull]
        public virtual T Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException("Empty stack");
            }

            return item;
        }
        
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

        public ArrayStack<T> Clone()
        {
            var clone = new ArrayStack<T>(_size);
            Array.Copy(_array, clone._array, _size);
            clone._size = _size;
            return clone;
        }

        IStack<T> ICloneable<IStack<T>>.Clone() 
            => Clone();

        IStack ICloneable<IStack>.Clone()
            => Clone();

        object ICloneable.Clone()
            => Clone();

        public ArrayStackEnumerator GetEnumerator()
            => new ArrayStackEnumerator(this);
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() 
            => GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        
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
        
        bool ICollection.Contains(object? item)
            => Contains((T) item!);

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

        public struct ArrayStackEnumerator : IEnumerator<T>
        {
            private int _index;
            private T _current;
            private readonly ArrayStack<T> _stack;
            public ArrayStackEnumerator(ArrayStack<T> stack)
            {
                _stack = stack;
                _index = -2;
                _current = default!;
            }

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

            public void Reset()
            {
                _index = -2;
                _current = default!;
            }

            object? IEnumerator.Current => Current;

            public T Current 
                => _index switch
                {
                    -2 => throw new InvalidOperationException("Enumerable not started"),
                    -1 => throw new InvalidOperationException("Enumerable end"),
                    _ => _current
                };

            public void Dispose()
            {
                
            }
        }
    }
}